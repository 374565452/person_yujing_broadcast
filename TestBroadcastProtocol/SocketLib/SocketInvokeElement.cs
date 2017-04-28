using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocketLib
{
    public class SocketInvokeElement
    {
        public Socket socket;


        protected bool showInfoLog;

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

        protected bool showErrorLog;

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

        protected bool isConnected;

        protected bool sendAsync;

        public bool Connected
        {
            get
            {
                return isConnected;
            }
        }

        protected DynamicBufferManager receiveBufferManager;

        public DynamicBufferManager ReceiveBufferManager
        {
            get
            {
                return receiveBufferManager;
            }
        }

        protected SendBufferManager sendBufferManager;

        public SendBufferManager SendBufferManager
        {
            get
            {
                return sendBufferManager;
            }
        }

        // protected OutgoingDataAssembler outgoingDataAssembler;
        //protected IncomingDataParse incomingDataParse;

        public SocketInvokeElement()
        {
            isConnected = false;
            sendAsync = false;
            receiveBufferManager = new DynamicBufferManager(ProtocolConst.InitBufferSize);
            sendBufferManager = new SendBufferManager(ProtocolConst.InitBufferSize);
            //outgoingDataAssembler = new OutgoingDataAssembler();
            //incomingDataParse = new IncomingDataParse();
        }

        #region 连接服务器
        public void connect(string ip, int port)
        {
            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress address = IPAddress.Parse(ip);
                IPEndPoint endPoint = new IPEndPoint(address, port);
                //socket.BeginConnect(endPoint, new AsyncCallback(connectCallBakc), socket);
                socket.Connect(endPoint);

                receive(socket);
                isConnected = true;
                //connectDone.WaitOne();
            }
            catch (Exception e)
            {
                isConnected = false;
                if (showErrorLog)
                {
                    string error = string.Format("在连接山脉平台时出错，错误信息为{0}", e.Message);
                    //LogHelper.log(error, this, e, LogLevel.Error);
                }
                //throw e;

            }
        }
        #endregion

        #region 连接回调函数
        private void connectCallBakc(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;
                //Console.WriteLine(client.LocalEndPoint.ToString());
                client.EndConnect(ar);
                //string info = String.Format("Socket connected to {0} \n", client.RemoteEndPoint.ToString());
                //this.showInfo(info);
                //connectDone.Set();
                isConnected = true;
                receive(client);
                //connectDone.Set();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        #endregion

        #region 接收数据
        public void receive(Socket client)
        {
            try
            {
                SocketState state = new SocketState(ProtocolConst.ReceiveBufferSize);
                state.Socket = client;
                client.BeginReceive(state.ReceiveBuffer, 0, state.ReceiveBuffer.Length, 0, new AsyncCallback(receiveCallBack), state);
            }
            catch (Exception e)
            {
                //throw new Exception(e.Message);
                if (showErrorLog)
                {
                    string error = string.Format("在接收山脉平台发送来的数据时（receive)中出错，错误信息为{0}", e.Message);
                    //LogHelper.log(error, this, e, LogLevel.Error);
                }
            }
        }
        #endregion

        #region 接收回调函数
        private void receiveCallBack(IAsyncResult ar)
        {
            try
            {
                SocketState state = (SocketState)ar.AsyncState;
                Socket client = state.Socket;
                int byteReceive = client.EndReceive(ar);
                if (byteReceive > 0)
                {
                    receiveBufferManager.writeBuffer(state.ReceiveBuffer, 0, byteReceive);
                    receiveProcess(receiveBufferManager);
                    client.BeginReceive(state.ReceiveBuffer, 0, state.ReceiveBuffer.Length, 0, new AsyncCallback(receiveCallBack), state);
                }

            }
            catch (Exception e)
            {
                //throw new Exception(e.Message);
                if (showErrorLog)
                {
                    string error = string.Format("在接收山脉平台发送来的数据时(receiveCallBack)中出错，错误信息为{0}", e.Message);
                    //LogHelper.log(error, this, e, LogLevel.Error);
                }
            }
        }
        #endregion

        #region 进行接收数据的处理,由具体的子类来完成
        public virtual bool receiveProcess(DynamicBufferManager bufferManager)
        {
            return true;
        }
        #endregion

        #region 发送数据
        private void send(byte[] buffer, int offset, int count)
        {
            socket.BeginSend(buffer, 0, count, 0, new AsyncCallback(sendCallBack), socket);
        }
        #endregion

        #region 发送回调函数
        private void sendCallBack(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;
                int bytes = client.EndSend(ar);
                sendCallBack(bytes);
                doSendComplete();
                //dynamicBufferManager.clearAll();
                // string info = String.Format("sent {0} bytes to server \n", bytes);
                //this.receiveTextBox.AppendText(info + "\n");
                //this.showInfo(info);

            }
            catch (Exception e)
            {
                //throw new Exception(e.Message);
                if (showErrorLog)
                {
                    string error = string.Format("在向山脉平台发送数据时出错，错误信息为{0}", e.Message);
                    //LogHelper.log(error, this, e, LogLevel.Error);
                }
            }
        }
        #endregion

        #region 子类重写后，可以获取发送数据的长度
        public virtual void sendCallBack(int sendBytes)
        {
        }
        #endregion

        public void doSendComplete()
        {
            sendAsync = false;
            int offset = 0;
            int count = 0;
            sendBufferManager.clearFirstPacket();
            if (sendBufferManager.getFirstPacket(ref offset, ref count))
            {
                sendAsync = true;
                send(sendBufferManager.DynamicBufferManager.Buffer, offset, count);
            }
            else
            {
                sendContinue();
            }
        }
        #region 子类继承用于继续发送数据
        public virtual void sendContinue()
        {
        }
        #endregion

        //只发命令，不加数据
        public void doSendResult()
        {
            // string commandTxt = outgoingDataAssembler.getProtocolText();
            // byte[] commandByte = Encoding.UTF8.GetBytes(commandTxt);
            //int totalLen = sizeof(int) + commandByte.Length;

            // sendBufferManager.startPacket();
            // sendBufferManager.DynamicBufferManager.writeBuffer(new byte[] { (byte)flag });
            // sendBufferManager.DynamicBufferManager.writeInt(totalLen, false);
            // sendBufferManager.DynamicBufferManager.writeInt(commandByte.Length, false);
            //  sendBufferManager.DynamicBufferManager.writeBuffer(commandByte);
            // sendBufferManager.endPacket();

            if (!sendAsync)
            {
                int packetOffset = 0;
                int packetCount = 0;
                if (sendBufferManager.getFirstPacket(ref packetOffset, ref packetCount))
                {
                    sendAsync = true;
                    send(sendBufferManager.DynamicBufferManager.Buffer, packetOffset, packetCount);
                }
            }
        }

        public void doSendCommand(byte[] datas)
        {
            if (isConnected)
            {
                //byte[] sendData = BuildMountainCommand.buildCommand(command);
                //AsyncSendBufferManager sendBufferManager = userToken.AsyncSendBufferManager;

                sendBufferManager.startPacket();
                sendBufferManager.DynamicBufferManager.writeBuffer(datas);
                sendBufferManager.endPacket();
                if (!sendAsync)
                {
                    int packetOffset = 0;
                    int packetCount = 0;
                    if (sendBufferManager.getFirstPacket(ref packetOffset, ref packetCount))
                    {
                        sendAsync = true;
                        send(sendBufferManager.DynamicBufferManager.Buffer, packetOffset, packetCount);
                    }
                    Thread.Sleep(100);
                }
            }
        }

        //命令+数据
        public void doSendResult(byte[] buffer, int offset, int count)
        {
            // string commandTxt = outgoingDataAssembler.getProtocolText();
            //byte[] commandByte = Encoding.UTF8.GetBytes(commandTxt);
            //  int totalLen = sizeof(int) + commandByte.Length + count;

            sendBufferManager.startPacket();
            // sendBufferManager.DynamicBufferManager.writeBuffer(new byte[] { (byte)flag });
            //  sendBufferManager.DynamicBufferManager.writeInt(totalLen, false);
            // sendBufferManager.DynamicBufferManager.writeInt(commandByte.Length, false);
            // sendBufferManager.DynamicBufferManager.writeBuffer(commandByte);
            // sendBufferManager.DynamicBufferManager.writeBuffer(buffer, offset, count);
            // sendBufferManager.endPacket();

            if (!sendAsync)
            {
                int packetOffset = 0;
                int packetCount = 0;
                if (sendBufferManager.getFirstPacket(ref packetOffset, ref packetCount))
                {
                    sendAsync = true;
                    send(sendBufferManager.DynamicBufferManager.Buffer, packetOffset, packetCount);
                }
            }
        }
        //只有数据
        public void doSendBuffer(byte[] buffer, int offset, int count)
        {
            sendBufferManager.startPacket();
            sendBufferManager.DynamicBufferManager.writeBuffer(buffer, offset, count);
            sendBufferManager.endPacket();

            if (!sendAsync)
            {
                int packetOffset = 0;
                int packetCount = 0;
                if (sendBufferManager.getFirstPacket(ref packetOffset, ref packetCount))
                {
                    sendAsync = true;
                    send(sendBufferManager.DynamicBufferManager.Buffer, packetOffset, packetCount);
                }
            }
        }
    }
}
