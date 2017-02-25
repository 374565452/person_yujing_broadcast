using DTU.GateWay.Protocol;
using Server.Util.Cache;
using Server.Util.Log;
using Server.Web.ProtocolProcess;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server.Web.ServerCore
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

        protected ServerHandler onServerStartHandler = null;

        protected ServerHandler onServerConnectedClientHandler = null;

        public ServerDataSocketHandler onServerDataSocketHandler = null;

        public ServerCloseSocketHandler onServerCloseSocketHandler = null;

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

        public ServerSocket(int numberConnection)
        {
            this.numConnections = numberConnection;
            asyncReceiveBufferSize = ProtocolConst.ReceiveBufferSize;
            asyncSocketUserTokenList = new AsyncSocketUserTokenList();
            asyncSocketUserTokenPool = new AsyncSocketUserTokenPool(numConnections);
            this.maxConnection = new Semaphore(numConnections, numConnections);
            showInfoLog = true;
            showErrorLog = true;
            init();
        }

        private void init()
        {
            //Console.WriteLine("the server init.....");

            AsyncSocketUserToken userToken;

            for (int i = 0; i < numConnections; i++)
            {
                userToken = new AsyncSocketUserToken(asyncReceiveBufferSize);
                userToken.ReceiveAysncEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Complete);
                userToken.SendAysncEvetnArgs.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Complete);
                this.asyncSocketUserTokenPool.push(userToken);
            }
        }

        public void start(IPEndPoint point)
        {
            try
            {

                this.listenSocket = new Socket(point.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                listenSocket.Bind(point);
                listenSocket.Listen(numConnections);
                if (SysCache.ShowInfoLog)
                {
                    LogHelper.Info("WEB通信服务已经开启！！");
                    ShowLogData.add("WEB通信服务已经开启！！");
                }
                startAccept(null);
            }
            catch (Exception e)
            {
                if (SysCache.ShowErrorLog)
                {
                    string error = string.Format("启动WEB服务器{0}出错,错误信息为{1}", point.ToString(), e.Message);
                    LogHelper.Error(error);
                }
                string msg = string.Format("服务程序启动失败，端口号{0}被占用，程序退出！", point.Port);
                if (onServerStartHandler != null)
                {
                    onServerStartHandler(this, msg);
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
            lock (this.asyncSocketUserTokenList)
            {
                this.asyncSocketUserTokenList.add(token);
            }

            //设置保存连接socket
            token.ConnectedSocket = args.AcceptSocket;
            if (SysCache.ShowInfoLog)
            {
                if (token.ConnectedSocket.RemoteEndPoint != null)
                {
                    LogHelper.Info(string.Format("WEB通信服务连接上一个客户端,信息为{0}", token.ConnectedSocket.RemoteEndPoint.ToString()));
                }
            }
            try
            {
                //对连接后的socket对象投递接收请求
                //投递异步操作对象AsyncSocketUserToken中的ReceiveAysncEventArgs
                //进行数据接收处理时，也是用这个ReceiveAysncEventArgs

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
                string error = "";
                if (token.ConnectedSocket.RemoteEndPoint != null)
                {
                    error = string.Format("WEB通信服务客户端{0}在投递接收请求时出错，错误信息为{1}", token.ConnectedSocket.RemoteEndPoint.ToString(), e.Message);
                }
                else
                {
                    error = string.Format("在投递接收请求时出错，错误信息为{0}", e.Message);
                }
                closeClientSocket(token);
            }
            finally
            {
                //数据是否处理成功，都要进行下一次的异步接收
                if (onServerConnectedClientHandler != null)
                {
                    string connectedCount = string.Format("当前连接上来的web客户端总数为:{0},缓冲区大小为：{1}", this.asyncSocketUserTokenList.count(), this.asyncSocketUserTokenPool.Count);
                    onServerConnectedClientHandler(this, connectedCount);
                }
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
                    string error = string.Format("程序出错，错误信息{0}", e.Message);
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
                closeClientSocket(userToken);
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

                if (token.ReceiveAysncEventArgs.BytesTransferred > 0 && token.ReceiveAysncEventArgs.SocketError == SocketError.Success)
                {
                    if (token.InvokeElement == null)
                    {
                        token.InvokeElement = new WebInvokeElement(this, token);
                    }
                    if (count > 0)
                    {
                        if (!token.InvokeElement.processReceive(token.ReceiveAysncEventArgs.Buffer, offset, count))
                        {
                            closeClientSocket(token);
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
                    closeClientSocket(token);
                }
            }
            catch (Exception)
            {
                closeClientSocket(token);
            }

        }

        private bool processSend(SocketAsyncEventArgs args)
        {
            AsyncSocketUserToken token = args.UserToken as AsyncSocketUserToken;
            if (token.ConnectedSocket == null)
            {
                return false;
            }
            //token.ActiveDateTime = DateTime.Now;
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
                closeClientSocket(token);
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
                return false;
            }

        }
        #endregion
        #region event事件定义
        public virtual event ServerHandler OnServerStartHandler
        {
            add { onServerStartHandler += value; }
            remove { onServerStartHandler -= value; }
        }

        public virtual event ServerDataSocketHandler OnServerDataSocketHandler
        {
            add { onServerDataSocketHandler += value; }
            remove { onServerDataSocketHandler -= value; }
        }

        public virtual event ServerCloseSocketHandler OnServerCloseSocketHandler
        {
            add { onServerCloseSocketHandler += value; }
            remove { onServerCloseSocketHandler -= value; }
        }

        public virtual event ServerHandler OnServerConnectedClientHandler
        {
            add { onServerConnectedClientHandler += value; }
            remove { onServerConnectedClientHandler -= value; }
        }
        #endregion

        public void closeClientSocket(AsyncSocketUserToken token)
        {
            if (onServerCloseSocketHandler != null)
            {
                onServerCloseSocketHandler(this, token);
            }
            if (token.ConnectedSocket == null)
            {
                return;
            }
            try
            {
                token.ConnectedSocket.Shutdown(SocketShutdown.Both);
            }
            catch (Exception e)
            {
                if (SysCache.ShowErrorLog)
                {
                    string error = string.Format("程序关闭连接时出错，错误信息{0}", e.Message);
                    LogHelper.Error(error);
                }
            }
            token.ConnectedSocket.Close();
            token.ConnectedSocket = null;
            //释放信号量资源
            this.maxConnection.Release();
            //并将此客户端资源回收到池中
            this.asyncSocketUserTokenPool.push(token);
            //正在连接的集合中祛除此客户端信息
            lock (this.asyncSocketUserTokenList)
            {
                this.asyncSocketUserTokenList.remove(token);
            }

            if (onServerConnectedClientHandler != null)
            {
                string connectedCount = string.Format("当前连接上来的web客户端总数为:{0},缓冲区大小为：{1}", this.asyncSocketUserTokenList.count(), this.asyncSocketUserTokenPool.Count);
                onServerConnectedClientHandler(this, connectedCount);
            }
        }
    }
}
