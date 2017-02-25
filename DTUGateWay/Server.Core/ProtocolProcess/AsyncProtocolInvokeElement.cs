using Common;
using DTU.GateWay.Protocol;
using DTU.GateWay.Protocol.WaterMessageClass;
using Maticsoft.Model;
using Module;
using Server.Core.ServerCore;
using Server.Util.BufferManager;
using Server.Util.Cache;
using Server.Util.DataProcess;
using Server.Util.Log;
using Server.Util.Service;
using Server.Util.Transfer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Server.Core.ProtocolProcess
{
    public class AsyncProtocolInvokeElement
    {
        log4net.ILog LogHelper = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static Dictionary<string, DateTime> DictDeviceGetNew = new Dictionary<string, DateTime>();

        protected ServerSocket server;

        protected AsyncSocketUserToken userToken;

        protected SendData sendData;

        public SendData SendData
        {
            get
            {
                return this.sendData;
            }
        }

        protected bool isSending;

        public AsyncProtocolInvokeElement(ServerSocket server, AsyncSocketUserToken userToken)
        {
            this.server = server;
            this.userToken = userToken;
            isSending = false;
            sendData = new SendData();
        }
        public virtual bool processReceive(byte[] buffer, int offset, int length)
        {
            DynamicBufferManager bufferManager = userToken.DynamicBufferManager;
            bufferManager.writeBuffer(buffer, offset, length);
            bool result = processPacket(bufferManager.Buffer, offset, length);
            bufferManager.clearBuffer(length);
            return result;
        }

        public bool processPacket(byte[] buffer, int offset, int length)
        {
            int pos = 0;
            while (pos < length)
            {
                try
                {
                    if (buffer[pos] == HexStringUtility.StrToByteArray(ProtocolKey.ReceiveHeartThrob)[0] || length - pos < 14)
                    {
                        if (userToken.DeviceInfo != null && userToken.DeviceList != null)
                        {
                            userToken.DeviceList.Online = 1;
                            userToken.DeviceList.LastUpdate = DateTime.Now;
                            userToken.DeviceList.Remark = FormatHelper.ByteArrayToHexString(buffer, offset + pos, length - pos);
                            userToken.DeviceList.TerminalState = "心跳";
                            updateDeviceList(userToken.DeviceList);
                            string FullDeviceNo = DeviceModule.GetFullDeviceNoByID(userToken.DeviceList.Id);
                            OnlineDeviceService.AddOnline(FullDeviceNo, userToken.DeviceList);
                            DeviceEvent deviceEvent = new DeviceEvent();
                            deviceEvent.DeviceNo = userToken.DeviceInfo.DeviceNo;
                            deviceEvent.EventTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.EventType = userToken.DeviceList.TerminalState;
                            deviceEvent.Remark = "";
                            deviceEvent.RawData = FormatHelper.ByteArrayToHexString(buffer, offset + pos, length - pos);
                            proxySendDeviceList(userToken.DeviceList, deviceEvent);
                        }
                        byte[] data = HexStringUtility.StrToByteArray("%");
                        send(data, 0, data.Length);
                        if (SysCache.ShowInfoLog)
                        {
                            string info1 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]响应（信息不对）：" + "长度不到14";
                            LogHelper.Info(info1);
                            ShowLogData.add(info1);
                        }

                        if (length - pos < 14)
                        {
                            break;
                        }
                        else
                        {
                            pos += 1;
                        }
                    }
                    else if (buffer[pos] == BaseProtocol.BeginChar)
                    {
                        int bodyLength = buffer[pos + 1]; //长度
                        byte[] bs = new byte[5 + bodyLength];
                        Array.Copy(buffer, pos, bs, 0, bs.Length);

                        processCommandBase(bs);

                        pos += 5 + bodyLength;
                    }
                    else if (buffer[pos] == WaterBaseProtocol.BeginChar[0] && buffer[pos + 1] == WaterBaseProtocol.BeginChar[1])
                    {
                        byte[] bs_UpOrDown_Length = new byte[2];
                        Array.Copy(buffer, pos + 11, bs_UpOrDown_Length, 0, 2);

                        string str_UpOrDown_Length = HexStringUtility.ByteArrayToHexString(bs_UpOrDown_Length).ToUpper();
                        string str_Length = str_UpOrDown_Length.Substring(1, 3);
                        int bodyLength = Convert.ToInt32(str_Length, 16); //正文长度

                        int baseLength = 0; //其它长度
                        baseLength += 2;//帧起始符
                        baseLength += 6;//中心站及遥测站
                        baseLength += 2;//密码
                        baseLength += 1;//功能码
                        baseLength += 2;//报文上下行表示及长度
                        baseLength += 1;//报文起始符
                        baseLength += 1;//报文结束符
                        baseLength += 2;//校验码
                        byte[] bs = new byte[baseLength + bodyLength];
                        Array.Copy(buffer, pos, bs, 0, bs.Length);

                        processCommandWater(bs);

                        pos += baseLength + bodyLength;
                    }
                    else
                    {
                        pos += 1;
                    }
                }
                catch (Exception ex)
                {
                    if (userToken.DeviceInfo != null && userToken.DeviceList != null)
                    {
                        userToken.DeviceList.Online = 1;
                        userToken.DeviceList.LastUpdate = DateTime.Now;
                        userToken.DeviceList.Remark = FormatHelper.ByteArrayToHexString(buffer, offset, length);
                        userToken.DeviceList.TerminalState = "心跳";
                        updateDeviceList(userToken.DeviceList);
                        string FullDeviceNo = DeviceModule.GetFullDeviceNoByID(userToken.DeviceList.Id);
                        OnlineDeviceService.AddOnline(FullDeviceNo, userToken.DeviceList);
                        DeviceEvent deviceEvent = new DeviceEvent();
                        deviceEvent.DeviceNo = userToken.DeviceInfo.DeviceNo;
                        deviceEvent.EventTime = userToken.DeviceList.LastUpdate;
                        deviceEvent.EventType = userToken.DeviceList.TerminalState;
                        deviceEvent.Remark = "";
                        deviceEvent.RawData = FormatHelper.ByteArrayToHexString(buffer, offset + pos, length - pos);
                        proxySendDeviceList(userToken.DeviceList, deviceEvent);
                    }
                    byte[] data = HexStringUtility.StrToByteArray("%");
                    send(data, 0, data.Length);
                    if (SysCache.ShowInfoLog)
                    {
                        string info1 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]响应（信息不对）：" + "【错误】【pos：" + pos + "】：" + ex.Message;
                        LogHelper.Info(info1);
                        ShowLogData.add(info1);
                    }
                    break;
                }
                
            }
            return true;
        }

        //重载不同参数 的两个方法 在DTUInvokeElement中重写
        public virtual bool processCommand(string receive)
        {
            return true;
        }

        public virtual bool processCommand(byte[] buffer, int offset, int length)
        {
            return true;
        }

        public virtual bool processCommandBase(byte[] buffer)
        {
            return true;
        }

        public virtual bool processCommandWater(byte[] buffer)
        {
            return true;
        }

        protected Image byteArrayToImage(byte[] Bytes)
        {
            MemoryStream ms = new MemoryStream(Bytes);
            return Bitmap.FromStream(ms, true);
        }

        #region 数据发送
        public bool sendComplete()
        {
            AsyncSendBufferManager sendBufferManager = userToken.AsyncSendBufferManager;
            sendBufferManager.clearFirstPacket();
            isSending = false;
            int packetOffset = 0;
            int packetCount = 0;
            if (sendBufferManager.getFirstPacket(ref packetOffset, ref packetCount))
            {
                isSending = true;
                return server.sendAsyncEvent(userToken.ConnectedSocket,
                    userToken.SendAysncEvetnArgs, sendBufferManager.DynamicBufferManager.Buffer, packetOffset, packetCount);
            }
            else
            {
                return sendCallBack();
            }
        }

        //用来清理一些必要的资源
        public virtual void close()
        {
        }

        //发送完成后，用来执行一些其他的一些操作
        public virtual bool sendCallBack()
        {
            return true;
        }

        public bool send(byte[] buffer, int offset, int length)
        {
            string info = "";
            if (userToken.DeviceInfo != null)
            {
                info = string.Format("向DTU客户端（序号为：{0}）发送数据,十六进制表示为：{1}", userToken.DeviceInfo.SerialString, FormatHelper.ByteArrayToHexString(buffer, offset, length));
            }
            else
            {
                info = string.Format("向DTU客户端发送数据,十六进制表示为：{0}", FormatHelper.ByteArrayToHexString(buffer, offset, length));
            }
            ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送：" + info);
            LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送：" + info);

            AsyncSendBufferManager sendBufferManager = userToken.AsyncSendBufferManager;
            sendBufferManager.startPacket();
            sendBufferManager.DynamicBufferManager.writeBuffer(buffer, offset, length);
            sendBufferManager.endPacket();
            bool result = true;
            if (!isSending)
            {
                int packetOffset = 0;
                int packetCount = 0;
                if (sendBufferManager.getFirstPacket(ref packetOffset, ref packetCount))
                {
                    isSending = true;
                    //每一次发送时，更新时间
                    userToken.ActiveDateTime = DateTime.Now;
                    result = server.sendAsyncEvent(userToken.ConnectedSocket,
                        userToken.SendAysncEvetnArgs, sendBufferManager.DynamicBufferManager.Buffer, packetOffset, packetCount);
                }
            }
            return result;
        }

        public bool send()
        {
            AsyncSendBufferManager sendBufferManager = userToken.AsyncSendBufferManager;
            string data = sendData.getSendData();
            byte[] sendByte = FormatHelper.GetBytesFromGBKString(data);
            sendBufferManager.startPacket();
            sendBufferManager.DynamicBufferManager.writeBuffer(sendByte);
            sendBufferManager.endPacket();
            bool result = true;
            if (!isSending)
            {
                int packetOffset = 0;
                int packetCount = 0;
                if (sendBufferManager.getFirstPacket(ref packetOffset, ref packetCount))
                {
                    isSending = true;
                    //每一次发送时，更新时间
                    userToken.ActiveDateTime = DateTime.Now;
                    result = server.sendAsyncEvent(userToken.ConnectedSocket,
                        userToken.SendAysncEvetnArgs, sendBufferManager.DynamicBufferManager.Buffer, packetOffset, packetCount);
                }
            }
            return result;
        }
        #endregion

        #region proxySend
        //日志
        protected void proxySendLog(string info)
        {
            /*
            DataTransfer transfer = new DataTransfer();
            transfer.TransferType = DataTransferType.DataTransferLog;
            transfer.TransferLogInfo = info;
            if (server.onServerDataSocketHandler != null)
            {
                server.onServerDataSocketHandler(server, userToken, transfer);
            }
             * */
        }

        //命令
        protected void proxySendCommand(DTU.GateWay.Protocol.Command command)
        {
            DataTransfer transfer = new DataTransfer();
            transfer.TransferType = DataTransferType.DataTransferCommand;
            transfer.TransferCommand = command;
            if (server.onServerDataSocketHandler != null)
            {
                server.onServerDataSocketHandler(server, userToken, transfer);
            }
        }

        //设备信息
        public void proxySendDeviceList(Device deviceList, DeviceEvent deviceEvent)
        {
            DataTransfer transfer = new DataTransfer();
            transfer.TransferType = DataTransferType.DataTransferDeviceList;
            transfer.DeviceList = deviceList;
            transfer.DeviceEvent = deviceEvent;
            if (server.onServerDataSocketHandler != null)
            {
                server.onServerDataSocketHandler(server, userToken, transfer);
            }
        }

        public void updateDeviceList(Device deviceList)
        {
            /*未完成 保存终端信息*/
            //DeviceModule.UpdateDeviceInfo(deviceList);
            DeviceModule.ModifyDevice(deviceList);
        }

        //增加 2015-8-18
        public void saveDeviceEvent(DeviceEvent deviceEvent)
        {
            /*未完成 保存终端事件*/
            DeviceEventModule.AddDeviceEvent(deviceEvent);
        }

        #endregion
    }
}
