using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace WaterMonitorSystem.Src
{
    public class TcpCommunication
    {
        public enum TCP_SocketState
        {
            SOCKET_CONNECTION,
            SOCKET_CLOSED,
            SOCKET_CONNECTED
        }
        /// <summary>
        /// 加到缓存的变量
        /// </summary>
        public class SocketCommandTag
        {
            public IOCPSocket Socket;
            public CmdBuffer Buffer;

            public SocketCommandTag()
            {
                Buffer = new CmdBuffer();
            }
        }



        AsyClient asyClient;

        private bool socketClosed;
        private bool socketReady;

        private int sendCounter = 0;

        private bool isLogined = false;

        /// <summary>
        /// 当前SOCKET状态
        /// </summary>
        public TCP_SocketState SOCKET_STATE;
        /// <summary>
        /// 从网接收到的数据
        /// </summary>
        public SocketCommandTag socketData;

        /// <summary>
        /// 等待时长
        /// </summary>
        public int TcpWait;

        #region socket处理部分
        void asyClient_OnExceptionHandler(IOCPSocket socket, AppExceptionEnum id, string msg)
        {
            if ((id == AppExceptionEnum.Error) || (id == AppExceptionEnum.SocketDisponsed))
            {
                socketClosed = true;
                SOCKET_STATE = TCP_SocketState.SOCKET_CLOSED;
            }
        }

        void asyClient_OnAppExceptionHandler(IOCPSocket socket, AppExceptionEnum id, string msg)
        {
            if (id == AppExceptionEnum.StopOK)
            {
                socketClosed = true;
                SOCKET_STATE = TCP_SocketState.SOCKET_CLOSED;
            }
            if (id == AppExceptionEnum.StartOK)
            {
                socketReady = true;
                SOCKET_STATE = TCP_SocketState.SOCKET_CONNECTED;

            }
        }

        void asyClient_OnCloseHandler(IOCPSocket socket, string msg)
        {
            socketClosed = true;
            socketReady = false;
            SOCKET_STATE = TCP_SocketState.SOCKET_CLOSED;
        }

        private void asyClient_OnDataHandler(IOCPSocket socket)
        {
            SOCKET_STATE = TCP_SocketState.SOCKET_CONNECTED;

            try
            {
                Array.Copy(socket.SocketObject.Buffer, 0, socketData.Buffer.Buffer, socketData.Buffer.Length, socket.SocketObject.DataLength);
                socketData.Buffer.Length = socketData.Buffer.Length + socket.SocketObject.DataLength;

            }
            catch
            {
                return;
            }

        }
        #endregion

        public TcpCommunication()
        {
            socketData = new SocketCommandTag();

            isLogined = false;

            //等待时长
            TcpWait = int.Parse(CommonUtil.ReadAppSetting("GetwayWait"));

            //正在连接
            SOCKET_STATE = TCP_SocketState.SOCKET_CONNECTION;

            asyClient = new AsyClient(IPAddress.Parse(CommonUtil.ReadAppSetting("GetwayIP")), int.Parse(CommonUtil.ReadAppSetting("GetwayPort")));

            asyClient.OnDataHandler += asyClient_OnDataHandler;
            asyClient.OnCloseHandler += asyClient_OnCloseHandler;
            asyClient.OnAppExceptionHandler += asyClient_OnAppExceptionHandler;
            asyClient.OnExceptionHandler += asyClient_OnExceptionHandler;

            asyClient.StartClient();


        }
        /// <summary>
        /// 关闭
        /// </summary>
        public void Close()
        {
            try
            {
                asyClient.Close();
            }
            catch
            {
            }
        }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        public void SendData(byte[] data, int start, int length)
        {
            try
            {
                asyClient.Send(data, start, length);
            }
            catch
            {
            }
        }
    }
}