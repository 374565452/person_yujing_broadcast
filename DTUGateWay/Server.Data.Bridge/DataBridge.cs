using DTU.GateWay.Protocol;

namespace Server.Data.Bridge
{
    public class DataBridge
    {
        //dtu服务端，维护着前端的设备dtu
        private Server.Core.ServerCore.ServerSocket dtuServerSocket;
        //web服务端，维护着客户端web连接
        private Server.Web.ServerCore.ServerSocket webServerSocket;

        public DataBridge(Server.Core.ServerCore.ServerSocket dtuServerSocket, Server.Web.ServerCore.ServerSocket webServerSocket)
        {
            this.dtuServerSocket = dtuServerSocket;
            this.webServerSocket = webServerSocket;
        }
        #region 将web端接收到的指令发送到前端dtu设备上
        /// <summary>
        /// 功能：将web端接收到的指令发送到前端的DTU设备上
        ///       此功能是根据前端设备的手机号码来唯一确定的，所有
        ///       web端发过来的指令只可能发送到一个dtu设备上，循环遍历所有连接上来的找到一个就跳出循环
        /// 注：
        ///     如果web端要发送的指令在已经连接上来的dtu设备中，没有找到相对应的
        ///     则不做相应处理，webu前端做处理
        /// </summary>
        /// <param name="deviceId">设备号，用来查找相对应的DTU设备</param>
        /// <param name="buffer">数据缓冲区</param>
        /// <param name="offset">数据开始位置</param>
        /// <param name="length">数据长度</param>
        public bool sendToDtu(long deviceId, long deviceIdMain, byte[] buffer, int offset, int length)
        {
            //线程间的互斥
            lock (dtuServerSocket.AsyncSocketUserTokenList)
            {
                if (dtuServerSocket.AsyncSocketUserTokenList.count() <= 0)
                {
                    return false;
                }
                foreach (Server.Core.ServerCore.AsyncSocketUserToken userToken in dtuServerSocket.AsyncSocketUserTokenList.UserTokenList)
                {
                    if (null != userToken.ConnectedSocket && null != userToken.DeviceInfo)
                    {
                        if (userToken.DeviceInfo.SerialLong == deviceId)
                        {
                            return userToken.InvokeElement.send(buffer, offset, length);
                        }
                    }
                }
                foreach (Server.Core.ServerCore.AsyncSocketUserToken userToken in dtuServerSocket.AsyncSocketUserTokenList.UserTokenList)
                {
                    if (null != userToken.ConnectedSocket && null != userToken.DeviceInfo)
                    {
                        if (userToken.DeviceInfo.SerialLong == deviceIdMain)
                        {
                            return userToken.InvokeElement.send(buffer, offset, length);
                        }
                    }
                }
                return false;
            }
        }

        public bool sendToDtu(string fullDeviceNo, string fullDeviceNoMain, byte[] buffer, int offset, int length)
        {
            //线程间的互斥
            lock (dtuServerSocket.AsyncSocketUserTokenList)
            {
                if (dtuServerSocket.AsyncSocketUserTokenList.count() <= 0)
                {
                    return false;
                }
                foreach (Server.Core.ServerCore.AsyncSocketUserToken userToken in dtuServerSocket.AsyncSocketUserTokenList.UserTokenList)
                {
                    if (null != userToken.ConnectedSocket && null != userToken.DeviceInfo)
                    {
                        if (userToken.DeviceInfo.DeviceNo == fullDeviceNo)
                        {
                            return userToken.InvokeElement.send(buffer, offset, length);
                        }
                    }
                }
                foreach (Server.Core.ServerCore.AsyncSocketUserToken userToken in dtuServerSocket.AsyncSocketUserTokenList.UserTokenList)
                {
                    if (null != userToken.ConnectedSocket && null != userToken.DeviceInfo)
                    {
                        if (userToken.DeviceInfo.DeviceNo == fullDeviceNoMain)
                        {
                            return userToken.InvokeElement.send(buffer, offset, length);
                        }
                    }
                }
                return false;
            }
        }
        #endregion
        #region 将DTU设备执行完相应的操作后，回传的信息发送给请求的web客户端
        /// <summary>
        /// 功能：将DTU设备执行完相应的操作后，回传的信息发送给请求的web客户端
        ///       同样是通过设备号（手机号码）来确定web客户端
        ///       这里与向dtu发送指令不同的是，不能说只找到一个就结束，因为可能同时有好几个操作向DTU发送指令
        /// 注：
        ///     如果在连接上来的web客户端中没有找到相对应的，则数据不作相应的处理
        /// </summary>
        /// <param name="deviceId">设备号</param>
        /// <param name="command">发送指令，需要作相应的转换</param>
        public bool sendToWeb(long deviceId, Command command)
        {
            //线程间互斥
            lock (webServerSocket.AsyncSocketUserTokenList)
            {
                if (webServerSocket.AsyncSocketUserTokenList.count() <= 0)
                {
                    return false;
                }
                foreach (Server.Web.ServerCore.AsyncSocketUserToken userToken in webServerSocket.AsyncSocketUserTokenList.UserTokenList)
                {
                    if (null != userToken.ConnectedSocket && null != userToken.DeviceInfo)
                    {
                        //2015-5-4修改
                        if (userToken.DeviceInfo.SerialLong == deviceId)
                        {
                            return userToken.InvokeElement.send(command);
                        }
                    }
                }
                return false;
            }
        }

        public bool sendToWeb(long deviceId, byte[] buffer, int offset, int length)
        {
            //线程间互斥
            lock (webServerSocket.AsyncSocketUserTokenList)
            {
                if (webServerSocket.AsyncSocketUserTokenList.count() <= 0)
                {
                    return false;
                }
                foreach (Server.Web.ServerCore.AsyncSocketUserToken userToken in webServerSocket.AsyncSocketUserTokenList.UserTokenList)
                {
                    if (null != userToken.ConnectedSocket && null != userToken.DeviceInfo)
                    {
                        //2015-5-4修改
                        if (userToken.DeviceInfo.SerialLong == deviceId)
                        {
                            return userToken.InvokeElement.send(buffer, offset, length);
                        }
                    }
                }
                return false;
            }
        }

        public bool sendToWeb(string fullDeviceNo, byte[] buffer, int offset, int length)
        {
            //线程间互斥
            lock (webServerSocket.AsyncSocketUserTokenList)
            {
                if (webServerSocket.AsyncSocketUserTokenList.count() <= 0)
                {
                    return false;
                }
                foreach (Server.Web.ServerCore.AsyncSocketUserToken userToken in webServerSocket.AsyncSocketUserTokenList.UserTokenList)
                {
                    if (null != userToken.ConnectedSocket && null != userToken.DeviceInfo)
                    {
                        //2015-5-4修改
                        if (userToken.DeviceInfo.DeviceNo == fullDeviceNo)
                        {
                            return userToken.InvokeElement.send(buffer, offset, length);
                        }
                    }
                }
                return false;
            }
        }
        #endregion

    }
}
