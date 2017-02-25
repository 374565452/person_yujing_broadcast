using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    #region delegage

    /// <summary>
    /// client accept
    /// </summary>
    /// <param name="socket"></param>
    public delegate void IOCPHandler(IOCPSocket socket);

    public delegate void IOCPClosedHandler(IOCPSocket socket, string msg);
    //程序异常
    public delegate void IOCPExceptionHandler(IOCPSocket socket, AppExceptionEnum id, string msg);
    #endregion

    public enum AppExceptionEnum
    {
        StartOK,
        StopOK,
        Error,
        SocketDisponsed
    };

    public class IOCPObject
    {
        public const int BufferLengthMax = 10240;
        public Socket ClientSocket;
        public byte[] Buffer;
        public int DataLength;

        public IOCPObject()
        {
            Buffer = new byte[BufferLengthMax];
        }
    }

    public class IOCPSocket
    {
        #region private var

        private int _serverPort = 9098;
        private IPEndPoint _ipe;

        protected bool _islogined = false;
        protected long _deviceId = 0;
        protected string _deviceSerial = "";

        protected Guid _id;

        protected bool _closeInvoke = false;

        protected DateTime _lastRevDate;
        protected DateTime _createTime;
        protected DateTime _LastSendTime;

        protected IOCPObject _socketObject = null;

        protected IOCPHandler _acceptHandler = null;
        protected IOCPHandler _dataHandler = null;
        protected IOCPClosedHandler _closeHandler = null;
        protected IOCPExceptionHandler _exceptionHandler = null;
        protected IOCPExceptionHandler _appExcptionHandler = null;

        #endregion

        #region 公共属性
        /// <summary>
        /// socket ID
        /// </summary>
        public Guid ID
        {
            set { _id = value; }
            get { return _id; }
        }

        /// <summary>
        /// 最后数据发送时间
        /// </summary>
        public DateTime LastSendTime
        {
            get { return _LastSendTime; }
            set { _LastSendTime = value; }
        }
        /// <summary>
        /// client socket
        /// </summary>
        public IOCPObject SocketObject
        {
            get { return _socketObject; }
            set { _socketObject = value; }
        }
        /// <summary>
        /// 最后接收数据时间
        /// </summary>
        public DateTime LastRecvDataTime
        {
            get { return _lastRevDate; }
            set { _lastRevDate = value; }
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDateTime
        {
            get { return _createTime; }
            set { _createTime = value; }
        }

        /// <summary>
        /// 是否已经登录
        /// </summary>
        public bool IsLogined
        {
            get { return _islogined; }
            set { _islogined = value; }
        }

        /// <summary>
        /// 设备ID
        /// </summary>
        public long DeviceId
        {
            get { return _deviceId; }
            set { _deviceId = value; }
        }
        /// <summary>
        /// 设备ID的字串表示
        /// </summary>
        public string Serial
        {
            get { return _deviceSerial; }
            set { _deviceSerial = value; }
        }
        #endregion

        #region Event
        /// <summary>
        /// AcceptHandler 
        /// </summary>
        public virtual event IOCPHandler OnAcceptHandler
        {
            add { _acceptHandler += value; }
            remove { _acceptHandler -= value; }
        }

        /// <summary>
        /// DataHandler 
        /// </summary>
        public virtual event IOCPHandler OnDataHandler
        {
            add { _dataHandler += value; }
            remove { _dataHandler -= value; }
        }

        /// <summary>
        /// CloseHandler
        /// </summary>
        public virtual event IOCPClosedHandler OnCloseHandler
        {
            add { _closeHandler += value; }
            remove { _closeHandler -= value; }
        }

        /// <summary>
        /// ExceptionHandler
        /// </summary>
        public virtual event IOCPExceptionHandler OnExceptionHandler
        {
            add { _exceptionHandler += value; }
            remove { _exceptionHandler -= value; }
        }

        public virtual event IOCPExceptionHandler OnAppExceptionHandler
        {
            add { _appExcptionHandler += value; }
            remove { _appExcptionHandler -= value; }
        }

        #endregion

        #region 公共方法


        public IOCPSocket()
        {
            _ipe = new IPEndPoint(IPAddress.Any, _serverPort);

            _createTime = DateTime.Now;
        }

        public IOCPSocket(int tcpPort)
        {
            _serverPort = tcpPort;
            _ipe = new IPEndPoint(IPAddress.Any, _serverPort);
            _createTime = DateTime.Now;
        }
        /// <summary>
        /// 强制调用关闭
        /// </summary>
        public virtual void Close()
        {
            if (_closeInvoke) return;

            try
            {
                if (_closeHandler != null)
                {
                    _closeHandler(this, "");
                }
            }
            catch
            {
            }

            if (_socketObject == null)
            {
                return;
            }


            try
            {
                _socketObject.ClientSocket.Close();
                _closeInvoke = true;
            }
            catch (Exception ex)
            {

            }
        }
        public virtual void Send(byte[] data)
        {
            _LastSendTime = DateTime.Now;
            Send(data, 0, data.Length);
        }

        public virtual void Send(byte[] data, int startIndex, int length)
        {
            _LastSendTime = DateTime.Now;
            try
            {
                if (_socketObject == null)
                {
                    Close();
                    return;
                }

                _socketObject.ClientSocket.BeginSend(data, startIndex, length, SocketFlags.None, new AsyncCallback(SendCallback), _socketObject);
            }
            catch (Exception ex)
            {
                Close();
            }
        }

        /// <summary>
        /// 设置为登录成功
        /// </summary>
        /// <param name="deviceId"></param>
        public virtual void SetLogined(string serial)
        {
            IsLogined = true;
            _deviceSerial = serial;
            DeviceId = long.Parse(serial);
        }
        #endregion

        #region 私有方法
        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                IOCPObject obj = ar.AsyncState as IOCPObject;

                obj.ClientSocket.EndSend(ar);
            }
            catch (Exception ex)
            {
                Close();
            }
        }

        #endregion

        /// <summary>
        /// 本地更新变量
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        protected IOCPSocket acceptEventHandler(Socket client)
        {
            IOCPSocket socket = new IOCPSocket();
            socket.ID = Guid.NewGuid();

            socket.IsLogined = IsLogined;
            socket.DeviceId = DeviceId;
            socket.Serial = _deviceSerial;

            socket.SocketObject = new IOCPObject();
            socket.SocketObject.DataLength = 0;
            socket.SocketObject.Buffer = new byte[IOCPObject.BufferLengthMax];

            socket.SocketObject.ClientSocket = client;

            socket.OnDataHandler += _dataHandler;
            socket.OnAcceptHandler += _acceptHandler;
            socket.OnCloseHandler += _closeHandler;
            socket.OnExceptionHandler += _exceptionHandler;
            socket.OnAppExceptionHandler += _appExcptionHandler;

            return socket;
        }

    }
}
