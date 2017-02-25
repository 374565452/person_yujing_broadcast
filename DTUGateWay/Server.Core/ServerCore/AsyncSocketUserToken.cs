using DTU.GateWay.Protocol;
using Maticsoft.Model;
using Server.Core.ProtocolProcess;
using Server.Util.BufferManager;
using System;
using System.Net.Sockets;

namespace Server.Core.ServerCore
{
    public class AsyncSocketUserToken
    {
        #region 接收数据缓冲区 ,用来为receiveAsyncEventArgs设置一个缓冲区
        private byte[] asyncReceiveBuffer;
        #endregion
        #region 连接的Socket对象，用来投递异步发送与接收请求
        private Socket connectedSocket;

        //记录登陆上来的设备信息
        //设备的序列
        private DeviceInfo deviceInfo;
        public DeviceInfo DeviceInfo
        {
            get
            {
                return deviceInfo;
            }
            set
            {
                deviceInfo = value;
            }
        }

        //添加一个DeviceList对象
        //用以操作数据
        private Device deviceList;

        public Device DeviceList
        {
            get
            {
                return deviceList;
            }
            set
            {
                deviceList = value;
            }
        }

        //是否已经注册登陆
        public bool IsLogined
        {
            get;
            set;
        }

        private AsyncProtocolInvokeElement invokeElement;

        public AsyncProtocolInvokeElement InvokeElement
        {
            get
            {
                return invokeElement;
            }
            set
            {
                invokeElement = value;
            }
        }

        public Socket ConnectedSocket
        {
            get
            {
                return connectedSocket;
            }
            set
            {
                connectedSocket = value;
                //清理缓存
                if (connectedSocket == null)
                {
                    IsLogined = false;
                    if (DeviceInfo != null)
                    {
                        DeviceInfo = null;
                    }
                    if (invokeElement != null)
                    {
                        invokeElement.close();
                    }
                    if (deviceList != null)
                    {
                        deviceList = null;
                    }
                    
                    dynamicBufferManager.clearAll();
                    asyncSendBufferManager.clearPacket();
                }
                DeviceInfo = null;
                invokeElement = null;
                IsLogined = false;
                deviceList = null;
                this.receiveAysncEventArgs.AcceptSocket = connectedSocket;
                this.sendAysncEventArgs.AcceptSocket = connectedSocket;
            }
        }
        #endregion

        #region 异步接收
        private SocketAsyncEventArgs receiveAysncEventArgs;

        public SocketAsyncEventArgs ReceiveAysncEventArgs
        {
            get
            {
                return receiveAysncEventArgs;
            }
            set
            {
                receiveAysncEventArgs = value;
            }
        }
        #endregion

        #region 异步发送
        private SocketAsyncEventArgs sendAysncEventArgs;

        public SocketAsyncEventArgs SendAysncEvetnArgs
        {
            get
            {
                return sendAysncEventArgs;
            }
            set
            {
                sendAysncEventArgs = value;
            }
        }
        #endregion

        #region 数据（命令）处理缓冲区
        private DynamicBufferManager dynamicBufferManager;
        public DynamicBufferManager DynamicBufferManager
        {
            get
            {
                return dynamicBufferManager;
            }
        }
        #endregion

        #region 发送数据缓冲区
        private AsyncSendBufferManager asyncSendBufferManager;
        public AsyncSendBufferManager AsyncSendBufferManager
        {
            get
            {
                return asyncSendBufferManager;
            }
        }
        #endregion


        public bool isClose = false;

        //通信通道处于活动状态，不是虚连接
        private DateTime activeDateTime;
        public DateTime ActiveDateTime
        {
            get
            {
                return activeDateTime;
            }
            set
            {
                activeDateTime = value;
            }
        }

        private DateTime connectedTime;

        public DateTime ConnectedTime
        {
            get
            {
                return connectedTime;
            }
            set
            {
                connectedTime = value;
            }
        }

        public AsyncSocketUserToken(int receiveBufferSize)
        {
            this.connectedSocket = null;
            IsLogined = false;
            this.receiveAysncEventArgs = new SocketAsyncEventArgs();
            //此处一定要为receiveAsyncEventArgs设置一个缓冲区，否则会一直出现System.NullReferenceException: 未将对象引用设置到对象的实例
            //ps：解决此问题的网址为http://www.cnblogs.com/jzywh/archive/2011/12/28/SuperSocket1_4SP2.html#2693439
            //byte[] buffer = new byte[1024];
            asyncReceiveBuffer = new byte[receiveBufferSize];
            this.receiveAysncEventArgs.SetBuffer(asyncReceiveBuffer, 0, asyncReceiveBuffer.Length);
            this.receiveAysncEventArgs.UserToken = this;

            this.sendAysncEventArgs = new SocketAsyncEventArgs();
            this.sendAysncEventArgs.UserToken = this;

            dynamicBufferManager = new DynamicBufferManager(ProtocolConst.InitBufferSize);

            asyncSendBufferManager = new AsyncSendBufferManager(ProtocolConst.InitBufferSize);

            invokeElement = null;
            DeviceInfo = null;
        }
    }
}
