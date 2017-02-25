using DTU.GateWay.Protocol;
using Maticsoft.Model;
using Module;
using Server.Core.ProtocolProcess;
using Server.Util.Cache;
using Server.Util.Log;
using Server.Util.Service;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server.Core.ServerCore
{
    public delegate void ServerHandler(ServerSocket server, string msg);
    public delegate void ServerExceptionHandler(ServerSocket server, Exception e, string msg);
    public delegate void ServerCloseSocketHandler(ServerSocket server, AsyncSocketUserToken userToken);
    public delegate void ServerExceptionSocketHandler(ServerSocket server, AsyncSocketUserToken userToken, Exception e);
    public delegate void ServerDataSocketHandler(ServerSocket server, AsyncSocketUserToken userToken, Object data);

    public class ServerSocket
    {
        log4net.ILog LogHelper = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //监听Socket
        private Socket listenSocket;

        //允许最大的连接数
        private int numConnections;
        //用来限制接收连接的线程数，以用来控制并发数
        private Semaphore maxConnection;

        private int asyncReceiveBufferSize;

        private AsyncSocketUserTokenPool asyncSocketUserTokenPool;
        private AsyncSocketUserTokenList asyncSocketUserTokenList;

        protected ServerHandler onServerConnectedHandler = null;

        public ServerDataSocketHandler onServerDataSocketHandler = null;

        public AsyncSocketUserTokenList AsyncSocketUserTokenList
        {
            get
            {
                return asyncSocketUserTokenList;
            }
        }

        private bool showInfoLog;

        public bool ShowInfoLog
        {
            get
            {
                return showInfoLog;
            }
            set
            {
                showInfoLog = value;
            }
        }

        private bool showErrorLog;

        public bool ShowErrorLog
        {
            get
            {
                return showErrorLog;
            }
            set
            {
                showErrorLog = value;
            }
        }

        private DeamonThread deamonThread;

        public DeamonThread DeamonThread
        {
            get
            {
                return deamonThread;
            }
        }

        private int timeout;

        public int Timeout
        {
            get
            {
                return timeout;
            }
            set
            {
                timeout = value;
            }
        }

        private bool recordEvent;

        public bool RecordEevent
        {
            get
            {
                return recordEvent;
            }
            set
            {
                recordEvent = value;
            }
        }
        /// <summary>
        /// bug 记录：启动守护进程时，并没有传入正确的timeout
        /// 因为在dtumain中通过属性传值
        /// 在守护进程是在构造函数中启动的。所有导致timeout值为0,应将守护进程的开启放到start的函数中
        /// </summary>
        /// <param name="numberConnection"></param>
        public ServerSocket(int numberConnection)
        {
            this.numConnections = numberConnection;
            asyncReceiveBufferSize = ProtocolConst.ReceiveBufferSize;
            asyncSocketUserTokenList = new AsyncSocketUserTokenList();
            asyncSocketUserTokenPool = new AsyncSocketUserTokenPool(numConnections);
            this.maxConnection = new Semaphore(numConnections, numConnections);
            showErrorLog = true;
            showInfoLog = true;
            recordEvent = true;

            init();
        }

        private void init()
        {
            AsyncSocketUserToken userToken;

            for (int i = 0; i < numConnections; i++)
            {
                userToken = new AsyncSocketUserToken(asyncReceiveBufferSize);
                userToken.ReceiveAysncEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Complete);
                userToken.SendAysncEvetnArgs.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Complete);
                this.asyncSocketUserTokenPool.push(userToken);
            }
        }

        //监听启动，以下为数据服务内容2015-12-17理解zlc
        public void start(IPEndPoint point)//any的IP地址+用户在xml中设置的PORT
        {
            try
            {

                this.listenSocket = new Socket(point.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                listenSocket.Bind(point);
                listenSocket.Listen(numConnections);
                if (SysCache.ShowInfoLog)
                {
                    LogHelper.Info("DTU通信服务已经开启！！");
                    ShowLogData.add("DTU通信服务已经开启！！");
                }
                //开启守护进程
                deamonThread = new DeamonThread(this, timeout);
                startAccept(null);
            }
            catch (Exception e)
            {
                if (SysCache.ShowErrorLog)
                {
                    string error = string.Format("启动DTU服务器{0}出错,错误信息为{1}", point.ToString(), e.Message);
                    LogHelper.Error(error);
                }
                string msg = string.Format("服务程序启动失败，端口号{0}被占用，程序已经启动正在运行！", point.Port);
                if (onServerConnectedHandler != null)
                {
                    onServerConnectedHandler(this, msg);
                }
            }
        }

        private void startAccept(SocketAsyncEventArgs args)
        {
            if (args == null)
            {
                args = new SocketAsyncEventArgs();
                args.Completed += new EventHandler<SocketAsyncEventArgs>(AcceptedEventArgs_Complete);
            }
            else
            {
                args.AcceptSocket = null;
            }
            //获取信号量，阻止当前线程
            this.maxConnection.WaitOne();
            //当前服务器开始一个异步操作来接受一个传入的尝试连接
            bool willRaiseEvent = listenSocket.AcceptAsync(args);
            if (!willRaiseEvent)
            {
                processAccepte(args);
            }
        }
        #region 异步连接成功后，处理逻辑
        private void processAccepte(SocketAsyncEventArgs args)
        {
            //从池中取出一个空闲的异步操作对象
            AsyncSocketUserToken token = this.asyncSocketUserTokenPool.pop();
            //将此对象添加到正在连接集合中
            this.asyncSocketUserTokenList.add(token);

            //设置保存连接socket
            token.ConnectedSocket = args.AcceptSocket;
            token.ConnectedTime = DateTime.Now;
            token.ActiveDateTime = DateTime.Now;

            try
            {
                //对连接后的socket对象投递接收请求
                //投递异步操作对象AsyncSocketUserToken中的ReceiveAysncEventArgs
                //进行数据接收处理时，也是用这个ReceiveAysncEventArgs
                if (SysCache.ShowInfoLog)
                {
                    if (token.ConnectedSocket.RemoteEndPoint != null)
                    {
                        LogHelper.Info(string.Format("DTU通信服务连接上一个客户端,信息为{0}", token.ConnectedSocket.RemoteEndPoint.ToString()));
                        ShowLogData.add(string.Format("DTU通信服务连接上一个客户端,信息为{0}", token.ConnectedSocket.RemoteEndPoint.ToString()));
                    }
                }

                //ShowLogData.add(string.Format("剩余线程池数量：{0}", this.asyncSocketUserTokenPool.Count));

                #region add 2015-4-21 当连接上来后，回传一个"%"
                if (token.InvokeElement == null)
                {
                    token.InvokeElement = new DTUInvokeElement(this, token);
                }
                //token.InvokeElement.SendData.addData(ProtocolKey.SendHeartThrob);
                //token.InvokeElement.send();
                #endregion
                bool willRaiseEvent = token.ConnectedSocket.ReceiveAsync(token.ReceiveAysncEventArgs);

                if (!willRaiseEvent)
                {
                    lock (token)
                    {
                        processReceive(token.ReceiveAysncEventArgs);
                    }
                }

            }
            catch (Exception e)
            {
                if (SysCache.ShowErrorLog)
                {
                    string error = "";
                    if (token.ConnectedSocket.RemoteEndPoint != null)
                    {
                        error = string.Format("DTU通信服务客户端{0}在投递接收请求时出错，错误信息为{1}", token.ConnectedSocket.RemoteEndPoint.ToString(), e.Message);
                    }
                    else
                    {
                        error = string.Format("在投递接收请求时出错，错误信息为{0}", e.Message);
                    }
                    LogHelper.Error(error);
                }
                closeClientSocket(token, true);
            }
            finally
            {
                //数据是否处理成功，都要进行下一次的异步接收
                startAccept(args);
            }
        }
        #endregion
        private void AcceptedEventArgs_Complete(Object sender, SocketAsyncEventArgs args)
        {
            try
            {
                processAccepte(args);
            }
            catch (Exception e)
            {
                if (SysCache.ShowErrorLog)
                {
                    string error = string.Format("程序出错，错误信息：{0}", e.Message);
                    LogHelper.Error(error);
                }
            }
        }

        private void IO_Complete(Object sender, SocketAsyncEventArgs args)
        {
            AsyncSocketUserToken userToken = args.UserToken as AsyncSocketUserToken;
            userToken.ActiveDateTime = DateTime.Now;
            try
            {
                lock (userToken)
                {
                    if (args.LastOperation == SocketAsyncOperation.Receive)
                    {
                        processReceive(args);
                    }
                    else if (args.LastOperation == SocketAsyncOperation.Send)
                    {
                        processSend(args);
                    }
                    else
                    {
                        if (SysCache.ShowErrorLog)
                        {
                            if (userToken.ConnectedSocket.RemoteEndPoint != null)
                            {
                                string error = string.Format("程序的最后一次操作在{0}并不是receive或者send", userToken.ConnectedSocket.RemoteEndPoint.ToString());
                                LogHelper.Error(error);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (SysCache.ShowErrorLog)
                {
                    string error = string.Format("程序处理IO时出错，错误信息{0}", e.Message);
                    LogHelper.Error(error);
                }
                closeClientSocket(userToken, true);
            }

        }

        private void processReceive(SocketAsyncEventArgs args)
        {
            AsyncSocketUserToken token = args.UserToken as AsyncSocketUserToken;
            if (token.ConnectedSocket == null)
            {
                return;
            }
            try
            {
                //接收数据缓冲区的偏移量
                int offset = token.ReceiveAysncEventArgs.Offset;
                //当前缓冲区接收到数据的总量
                int count = token.ReceiveAysncEventArgs.BytesTransferred;
                //接收到数据后，更新活动时间
                token.ActiveDateTime = DateTime.Now;

                if (token.ReceiveAysncEventArgs.BytesTransferred > 0 && token.ReceiveAysncEventArgs.SocketError == SocketError.Success)
                {

                    if (count > 0)
                    {
                        if (!token.InvokeElement.processReceive(token.ReceiveAysncEventArgs.Buffer, offset, count))
                        {
                            closeClientSocket(token, true);
                        }
                        else
                        {
                            bool willRaiseEvent = token.ConnectedSocket.ReceiveAsync(token.ReceiveAysncEventArgs);
                            if (!willRaiseEvent)
                            {
                                processReceive(token.ReceiveAysncEventArgs);
                            }
                        }
                    }
                    else
                    {
                        bool willRaiseEvent = token.ConnectedSocket.ReceiveAsync(token.ReceiveAysncEventArgs);
                        if (!willRaiseEvent)
                        {
                            processReceive(token.ReceiveAysncEventArgs);
                        }
                    }
                }
                else
                {
                    closeClientSocket(token, true);
                }
            }
            catch (Exception e)
            {
                if (SysCache.ShowErrorLog)
                {
                    string error = string.Format("程序处理接收时出错，错误信息:{0}", e.Message);
                    LogHelper.Error(error);
                }
                closeClientSocket(token, true);
            }
        }

        private bool processSend(SocketAsyncEventArgs args)
        {
            AsyncSocketUserToken token = args.UserToken as AsyncSocketUserToken;
            if (token.ConnectedSocket == null)
            {
                return false;
            }
            try
            {
                if (args.SocketError == SocketError.Success)
                {
                    //设置发送的数据
                    //接收到什么数据就发送什么数据
                    //args.SetBuffer(args.Offset, args.BytesTransferred);
                    //设置数据发送缓冲区后，可以在这里进行下一次的数据发送
                    return token.InvokeElement.sendComplete();
                    //return false;
                }
                else
                {
                    closeClientSocket(token, true);
                    return false;
                }
            }
            catch (Exception e)
            {
                if (SysCache.ShowErrorLog)
                {
                    string error = string.Format("程序处理发送时出错，错误信息{0}", e.Message);
                    LogHelper.Error(error);
                }
                closeClientSocket(token, true);
                return false;
            }
        }
        #region 异步发送
        public bool sendAsyncEvent(Socket connectedSocket, SocketAsyncEventArgs sendAsyncEventArgs, byte[] buffer, int offset, int count)
        {
            if (connectedSocket == null)
            {
                return false;
            }
            sendAsyncEventArgs.SetBuffer(buffer, offset, count);

            try
            {
                bool willRaiseEvent = connectedSocket.SendAsync(sendAsyncEventArgs);
                if (!willRaiseEvent)
                {
                    return processSend(sendAsyncEventArgs);
                }
                else
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                if (SysCache.ShowErrorLog)
                {
                    string error = string.Format("程序处理发送时出错，错误信息{0}", e.Message);
                    LogHelper.Error(error);
                }
                closeClientSocket(sendAsyncEventArgs.UserToken as AsyncSocketUserToken, true);
                return false;
            }

        }
        #endregion

        public virtual event ServerHandler OnServerConnectedHandler
        {
            add { onServerConnectedHandler += value; }
            remove { onServerConnectedHandler -= value; }
        }

        public virtual event ServerDataSocketHandler OnServerDataSocketHandler
        {
            add { onServerDataSocketHandler += value; }
            remove { onServerDataSocketHandler -= value; }
        }

        public void updateDevice(Device device)
        {
            if (SysCache.ShowInfoLog)
            {
                string info = string.Format(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + device.Id + "]：准备修改token设备信息，id：{0}", device.Id);
                ShowLogData.add(info);
                LogHelper.Info(info);
            }

            bool isUpdate = false;

            try
            {
                if (AsyncSocketUserTokenList != null)
                {
                    if (AsyncSocketUserTokenList.UserTokenList != null)
                    {
                        if (SysCache.ShowInfoLog)
                        {
                            string info4 = string.Format(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + device.Id + "]：AsyncSocketUserTokenList.UserTokenList数量：{0}", AsyncSocketUserTokenList.UserTokenList.Count);
                            ShowLogData.add(info4);
                            LogHelper.Info(info4);
                        }
                        foreach (AsyncSocketUserToken token in AsyncSocketUserTokenList.UserTokenList)
                        {
                            if (token.DeviceList == null)
                            {
                                closeClientSocket(token, false);
                            }

                            if (token.DeviceList != null && token.DeviceList.Id == device.Id)
                            {
                                if (SysCache.ShowInfoLog)
                                {
                                    string info1 = string.Format(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + token.ConnectedSocket.RemoteEndPoint.ToString() + "]：DTU通信服务，设备信息修改，id：{0}", token.DeviceList.Id);
                                    ShowLogData.add(info1);
                                    LogHelper.Info(info1);
                                }
                                token.DeviceList = device;

                                isUpdate = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        if (SysCache.ShowInfoLog)
                        {
                            string info4 = string.Format(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + device.Id + "]：无法修改token设备信息，id：{0}，AsyncSocketUserTokenList.UserTokenList不存在", device.Id);
                            ShowLogData.add(info4);
                            LogHelper.Info(info4);
                        }
                    }
                }
                else
                {
                    if (SysCache.ShowInfoLog)
                    {
                        string info3 = string.Format(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + device.Id + "]：无法修改token设备信息，id：{0}，AsyncSocketUserTokenList不存在", device.Id);
                        ShowLogData.add(info3);
                        LogHelper.Info(info3);
                    }
                }
            }
            catch (Exception ex)
            {
                if (SysCache.ShowInfoLog)
                {
                    string info3 = string.Format(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + device.Id + "]：修改token设备信息出错，id：{0}，错误：{1}", device.Id, ex.Message);
                    ShowLogData.add(info3);
                    LogHelper.Info(info3);
                }
            }

            if (SysCache.ShowInfoLog)
            {
                if (isUpdate)
                {
                    string info2 = string.Format(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + device.Id + "]：修改token设备信息成功，id：{0}", device.Id);
                    ShowLogData.add(info2);
                    LogHelper.Info(info2);
                }
                else
                {
                    string info2 = string.Format(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + device.Id + "]：修改token设备信息失败，id：{0}", device.Id);
                    ShowLogData.add(info2);
                    LogHelper.Info(info2);
                }
            }
        }

        public void closeClientSocket(AsyncSocketUserToken token, bool isUpdate)
        {
            if (token.DeviceList != null)
            {
                if (token.isClose)
                {
                    if (SysCache.ShowInfoLog)
                    {
                        string info = string.Format(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + token.ConnectedSocket.RemoteEndPoint.ToString() + "]：DTU通信服务，客户端{0}正在关闭", token.DeviceList.DeviceNo);
                        ShowLogData.add(info);
                        LogHelper.Info(info);
                    }
                    return;
                }
                token.isClose = true;

                if (SysCache.ShowInfoLog)
                {
                    string info = string.Format(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + token.ConnectedSocket.RemoteEndPoint.ToString() + "]：DTU通信服务，客户端{0}，断开连接", token.DeviceList.DeviceNo);
                    ShowLogData.add(info);
                    LogHelper.Info(info);
                }

                if (isUpdate)
                {
                    token.DeviceList.Online = 0;
                    token.DeviceList.LastUpdate = DateTime.Now;
                    token.DeviceList.TerminalState = "设备断开连接";
                    token.InvokeElement.proxySendDeviceList(token.DeviceList, null);

                    /*未完成 更新终端状态*/
                    token.InvokeElement.updateDeviceList(token.DeviceList);
                }

                List<string> list = OnlineDeviceService.GetDeviceNoSubList(token.DeviceList.Id);
                if (list != null && list.Count > 0)
                {
                    foreach (string str in list)
                    {
                        Device device = OnlineDeviceService.GetOnline(str);
                        if (device != null)
                        {
                            if (SysCache.ShowInfoLog)
                            {
                                string info = string.Format(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + token.ConnectedSocket.RemoteEndPoint.ToString() + "]：DTU通信服务，客户端{0}，从站断开连接，主站{1}断开", device.DeviceNo, token.DeviceList.DeviceNo);
                                ShowLogData.add(info);
                                LogHelper.Info(info);
                            }

                            device.Online = 0;
                            device.LastUpdate = DateTime.Now;
                            device.TerminalState = "设备断开连接";
                            token.InvokeElement.proxySendDeviceList(device, null);

                            token.InvokeElement.updateDeviceList(device);
                        }
                    }
                }

                string deviceNo = DeviceModule.GetFullDeviceNoByID(token.DeviceList.Id);
                OnlineDeviceService.RemoveOnline(deviceNo);
            }

            //修改于 2015-4-20
            if (token.ConnectedSocket == null)
            {

                return;
            }

            try
            {
                token.ConnectedSocket.Shutdown(SocketShutdown.Both);
            }
            catch (SocketException)
            {
            }
            catch (NullReferenceException e)
            {
                if (SysCache.ShowErrorLog)
                {
                    string error = string.Format("程序关闭连接时出错，错误信息{0}", e.Message);
                    LogHelper.Error(error);
                }
            }
            /*
            //释放信号量资源
            this.maxConnection.Release();
            if (showInfoLog)
            {
                string info = string.Format(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + token.ConnectedSocket.RemoteEndPoint.ToString() + "]：DTU通信服务，释放信号量资源");
                ShowLogData.add(info);
                LogHelper.Info(info);
            }
            */
            //start update by kqz 2017-2-3修改，由之前的注释修改成不注释
            //由于在关闭掉客户端的时候，没有将此连接上来的客户端从已经连接的客户端集合中删除掉，所以导致一个客户端成功连接上后，再次断开
            //下次连接，如果IP地址不同则连接不成功现象。
            //并将此客户端资源回收到池中
            this.asyncSocketUserTokenPool.push(token);
            if (showInfoLog)
            {
                string info = string.Format(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + token.ConnectedSocket.RemoteEndPoint.ToString() + "]：DTU通信服务，并将此客户端资源回收到池中，池中剩余数量{0}", asyncSocketUserTokenPool.Count);
                ShowLogData.add(info);
                LogHelper.Info(info);
            }

            //正在连接的集合中祛除此客户端信息
            this.asyncSocketUserTokenList.remove(token);
            if (showInfoLog)
            {
                string info = string.Format(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + token.ConnectedSocket.RemoteEndPoint.ToString() + "]：DTU通信服务，正在连接的集合中祛除此客户端信息，剩余连接数量{0}", asyncSocketUserTokenList.count());
                ShowLogData.add(info);
                LogHelper.Info(info);
            }
           // end update by kqz 2017-2-3修改，由之前的注释修改成不注释
            token.ConnectedSocket.Close();
            token.ConnectedSocket = null;
            token.isClose = false;
        }

        private void proxySendConnected(int connected)
        {
            if (onServerConnectedHandler != null)
            {
                onServerConnectedHandler(this, connected.ToString());
            }
        }
    }
}
