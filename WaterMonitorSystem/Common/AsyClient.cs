using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class AsyClient : IOCPSocket
    {
        private int _serverport = 9000;
        private TcpClient _client = null;
        private IPAddress _serverIP = null;

        #region 私有方法

        //data arrive
        private void DataCallback(IAsyncResult ar)
        {
            IOCPSocket socket = null;

            try
            {
                if (ar.AsyncState == null)
                {
                    if (_appExcptionHandler != null)
                    {
                        _appExcptionHandler(socket, AppExceptionEnum.Error, "Socket已释放");
                    }
                    return;
                }

            }
            catch (Exception ex)
            {

            }

            try
            {
                socket = ar.AsyncState as IOCPSocket;

                socket.SocketObject.DataLength = socket.SocketObject.ClientSocket.EndReceive(ar);

                if (socket.SocketObject.DataLength > 0)
                {
                    if (_dataHandler != null)
                    {
                        _dataHandler(socket);
                    }
                    socket.LastRecvDataTime = DateTime.Now;
                }
                else
                {
                    socket.Close();
                    return;
                }

                IOCPSocket _socket = socket;
                _socket.SocketObject.DataLength = 0;

                socket.SocketObject.ClientSocket.BeginReceive(_socket.SocketObject.Buffer, _socket.SocketObject.DataLength, IOCPObject.BufferLengthMax, SocketFlags.None, new AsyncCallback(DataCallback), _socket);

            }
            catch (Exception ex)
            {
                if (socket != null)
                {
                    _id = socket.ID;
                }
                Close();
            }
        }

        //允许接入
        private void AcceptClientHandlerCallback(IAsyncResult ar)
        {
            IOCPSocket socket = null;

            try
            {
                _client.EndConnect(ar);

                socket = acceptEventHandler(_client.Client);

                socket.LastRecvDataTime = DateTime.Now;
                socket.CreateDateTime = DateTime.Now;
                _socketObject = socket.SocketObject;

                if (_appExcptionHandler != null)
                {
                    _appExcptionHandler(socket, AppExceptionEnum.StartOK, "启动成功");
                }
                if (_acceptHandler != null)
                {
                    _acceptHandler(socket);
                }

                _client.Client.BeginReceive(socket.SocketObject.Buffer, socket.SocketObject.DataLength, IOCPObject.BufferLengthMax, SocketFlags.None, new AsyncCallback(DataCallback), socket);

            }
            catch (SocketException ex)
            {
                if (_exceptionHandler != null)
                {
                    _exceptionHandler(socket, AppExceptionEnum.Error, ex.Message);
                }
                Close();
            }
            catch (ObjectDisposedException ex)
            {
                if (_exceptionHandler != null)
                {
                    _exceptionHandler(socket, AppExceptionEnum.Error, ex.Message);
                }
            }
            catch (Exception ex)
            {
                if (_exceptionHandler != null)
                {
                    _exceptionHandler(socket, AppExceptionEnum.Error, ex.Message);
                }
                Close();
            }
        }
        #endregion

        public AsyClient()
        {
            _serverIP = IPAddress.Any;
            _lastRevDate = DateTime.Now;
            _client = new TcpClient();
        }

        public AsyClient(IPAddress serverIP, int serverPort)
        {
            _serverport = serverPort;
            _serverIP = serverIP;
            _lastRevDate = DateTime.Now;

            _client = new TcpClient();
        }

        /// <summary>
        /// 启动TCP客户端
        /// </summary>
        public void StartClient()
        {
            try
            {
                _client.BeginConnect(_serverIP, _serverport, new AsyncCallback(AcceptClientHandlerCallback), this);
            }
            catch (ObjectDisposedException ex)
            {
                if (_appExcptionHandler != null)
                {
                    _appExcptionHandler(null, AppExceptionEnum.SocketDisponsed, ex.Message);
                }
            }
            catch (Exception ex)
            {
                if (_appExcptionHandler != null)
                {
                    _appExcptionHandler(null, AppExceptionEnum.Error, ex.Message);
                }
            }
        }

        public void StopClient()
        {
            try
            {
                Close();

                if (_appExcptionHandler != null)
                {
                    _appExcptionHandler(null, AppExceptionEnum.StopOK, "关闭成功");
                }
            }
            catch (Exception ex)
            {
                if (_appExcptionHandler != null)
                {
                    _appExcptionHandler(null, AppExceptionEnum.Error, ex.Message);
                }
            }
        }
    }
}
