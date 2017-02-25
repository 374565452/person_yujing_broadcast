using Common;
using DTU.GateWay.Protocol;
using Maticsoft.Model;
using Module;
using Server.Util.BufferManager;
using Server.Util.Cache;
using Server.Util.DataProcess;
using Server.Util.Log;
using Server.Util.Service;
using Server.Util.Transfer;
using Server.Web.ParseData;
using Server.Web.ServerCore;
using System;
using System.Threading;

namespace Server.Web.ProtocolProcess
{
    public class AsyncProtocolInvokeElement
    {
        log4net.ILog LogHelper = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected ServerSocket server;

        protected AsyncSocketUserToken userToken;

        protected SendData sendData;

        protected ReceiveData receiveData;

        protected bool isSending;

        public AsyncProtocolInvokeElement(ServerSocket server, AsyncSocketUserToken userToken)
        {
            this.server = server;
            this.userToken = userToken;
            isSending = false;
            sendData = new SendData();
            receiveData = new WebParseData();
        }
        public virtual bool processReceive(byte[] buffer, int offset, int length)
        {
            //DataTransfer transfer = new DataTransfer();
            DynamicBufferManager receiveBuffer = userToken.DynamicBufferManager;
            receiveBuffer.writeBuffer(buffer, offset, length);

            byte[] buffer_new = new byte[receiveBuffer.DataCount];
            Array.Copy(buffer, buffer_new, receiveBuffer.DataCount);
            string receiveHex = HexStringUtility.ByteArrayToHexString(buffer_new);
            if (receiveBuffer.DataCount >= ProtocolConst.CmdMinLength)
            {
                if (SysCache.ShowInfoLog)
                {
                    string info = string.Format("从web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收到的数据，以十六进制显示为：{0}", receiveHex);
                    LogHelper.Info(info);
                    proxySendLog(receiveHex);
                }

                if (receiveHex.Substring(0, 6) == ProtocolConst.WebToGateUpdateCache)
                {
                    //01 终端更新，02 用户卡更新，03 地区更新
                    string t1 = receiveHex.Substring(6, 2);
                    //01 重新从数据库查询信息更新缓存，02 删除指定缓存，对终端和用户卡有效，地区直接重新获取全部缓存
                    string t2 = receiveHex.Substring(8, 2);
                    //终端表示15位终端完全编号，用户卡表示8位卡序列号
                    string key = receiveHex.Substring(10, 16);

                    if (t1 == ProtocolConst.UpdateCache_District)
                    {
                        proxyUpdateDistrict(t2, key);
                        return true;
                    }
                    else if (t1 == ProtocolConst.UpdateCache_CardUser)
                    {
                        proxyUpdateCardUser(t2, key.Substring(8, 8));
                        return true;
                    }
                    else if (t1 == ProtocolConst.UpdateCache_Device)
                    {
                        proxyUpdateDevice(t2, key.Substring(1, 15));
                        return true;
                    }
                }

                WaterBaseMessage message_water = new WaterBaseMessage();
                message_water.RawDataChar = buffer_new;
                message_water.RawDataStr = receiveHex;
                string msg_water = message_water.ReadMsgBase();
                if (msg_water == "")
                {
                    string RemoteStation = message_water.RemoteStation;
                    byte CenterStation = message_water.CenterStation;
                    byte AFN = message_water.AFN;
                    int UpOrDown = message_water.UpOrDown;
                    int Length = message_water.Length;
                    byte DataBeginChar = message_water.DataBeginChar;
                    int TotalPackage = message_water.TotalPackage;
                    int CurrentPackage = message_water.CurrentPackage;
                    byte DataEndChar = message_water.DataEndChar;

                    string cKey = RemoteStation + CenterStation.ToString("X").PadLeft(2, '0').ToUpper();

                    if (AFN == (byte)WaterBaseProtocol.AFN._36 && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Down)
                    {
                        ShowLogData.add("从web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收到的数据：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN));
                    }
                    else if (AFN == (byte)WaterBaseProtocol.AFN._37 && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Down)
                    {
                        ShowLogData.add("从web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收到的数据：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN));
                    }
                    else if (AFN == (byte)WaterBaseProtocol.AFN._38 && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Down)
                    {
                        ShowLogData.add("从web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收到的数据：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN));
                    }
                    else if (AFN == (byte)WaterBaseProtocol.AFN._39 && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Down)
                    {
                        ShowLogData.add("从web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收到的数据：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN));
                    }
                    else if (AFN == (byte)WaterBaseProtocol.AFN._3A && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Down)
                    {
                        ShowLogData.add("从web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收到的数据：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN));
                    }
                    else if (AFN == (byte)WaterBaseProtocol.AFN._40 && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Down)
                    {
                        ShowLogData.add("从web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收到的数据：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN));
                    }
                    else if (AFN == (byte)WaterBaseProtocol.AFN._41 && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Down)
                    {
                        ShowLogData.add("从web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收到的数据：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN));
                    }
                    else if (AFN == (byte)WaterBaseProtocol.AFN._42 && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Down)
                    {
                        ShowLogData.add("从web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收到的数据：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN));
                    }
                    else if (AFN == (byte)WaterBaseProtocol.AFN._43 && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Down)
                    {
                        ShowLogData.add("从web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收到的数据：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN));
                    }
                    else if (AFN == (byte)WaterBaseProtocol.AFN._44 && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Down)
                    {
                        ShowLogData.add("从web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收到的数据：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN));
                    }
                    else if (AFN == (byte)WaterBaseProtocol.AFN._45 && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Down)
                    {
                        ShowLogData.add("从web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收到的数据：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN));
                    }
                    else if (AFN == (byte)WaterBaseProtocol.AFN._46 && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Down)
                    {
                        ShowLogData.add("从web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收到的数据：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN));
                    }
                    else if (AFN == (byte)WaterBaseProtocol.AFN._47 && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Down)
                    {
                        ShowLogData.add("从web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收到的数据：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN));
                    }
                    else if (AFN == (byte)WaterBaseProtocol.AFN._48 && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Down)
                    {
                        ShowLogData.add("从web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收到的数据：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN));
                    }
                    else if (AFN == (byte)WaterBaseProtocol.AFN._49 && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Down)
                    {
                        ShowLogData.add("从web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收到的数据：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN));
                    }
                    else if (AFN == (byte)WaterBaseProtocol.AFN._4A && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Down)
                    {
                        ShowLogData.add("从web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收到的数据：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN));
                    }
                    else if (AFN == (byte)WaterBaseProtocol.AFN._4B && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Down)
                    {
                        ShowLogData.add("从web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收到的数据：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN));
                    }
                    else if (AFN == (byte)WaterBaseProtocol.AFN._4C && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Down)
                    {
                        ShowLogData.add("从web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收到的数据：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN));
                    }
                    else if (AFN == (byte)WaterBaseProtocol.AFN._4D && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Down)
                    {
                        ShowLogData.add("从web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收到的数据：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN));
                    }
                    else if (AFN == (byte)WaterBaseProtocol.AFN._4E && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Down)
                    {
                        ShowLogData.add("从web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收到的数据：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN));
                    }
                    else if (AFN == (byte)WaterBaseProtocol.AFN._4F && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Down)
                    {
                        ShowLogData.add("从web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收到的数据：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN));
                    }
                    else if (AFN == (byte)WaterBaseProtocol.AFN._50 && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Down)
                    {
                        ShowLogData.add("从web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收到的数据：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN));
                    }
                    else if (AFN == (byte)WaterBaseProtocol.AFN._51 && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Down)
                    {
                        ShowLogData.add("从web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收到的数据：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN));
                    }
                    else if (AFN == (byte)WaterBaseProtocol.AFN._20 && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Down)
                    {
                        ShowLogData.add("从web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收到的数据：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN));
                    }
                    else if (AFN == (byte)WaterBaseProtocol.AFN._21 && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Down)
                    {
                        ShowLogData.add("从web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收到的数据：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN));
                    }
                    else if (AFN == (byte)WaterBaseProtocol.AFN._22 && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Down)
                    {
                        ShowLogData.add("从web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收到的数据：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN));
                    }
                    else if (AFN == (byte)WaterBaseProtocol.AFN._23 && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Down)
                    {
                        ShowLogData.add("从web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收到的数据：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN));
                    }
                    else if (AFN == (byte)WaterBaseProtocol.AFN._24 && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Down)
                    {
                        ShowLogData.add("从web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收到的数据：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN));
                    }
                    else if (AFN == (byte)WaterBaseProtocol.AFN._25 && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Down)
                    {
                        ShowLogData.add("从web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收到的数据：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN));
                    }
                    else if (AFN == (byte)WaterBaseProtocol.AFN._26 && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Down)
                    {
                        ShowLogData.add("从web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收到的数据：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN));
                    }
                    else if (AFN == (byte)WaterBaseProtocol.AFN._27 && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Down)
                    {
                        ShowLogData.add("从web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收到的数据：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN));
                    }
                        //kqz 2017-1-1添加
                    else if (AFN == (byte)WaterBaseProtocol.AFN._28 && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Down)
                    {
                        ShowLogData.add("从web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收到的数据：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN));
                    }
                    else if (AFN == (byte)WaterBaseProtocol.AFN._29 && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Down)
                    {
                        ShowLogData.add("从web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收到的数据：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN));
                    }
                    //kqz 2017-1-1添加
                    else
                    {
                        ShowLogData.add("从web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收到的数据：水文监测无效数据");
                        return true;
                    }

                    Device device = DeviceModule.GetDeviceByRemoteStation(RemoteStation);
                    string deviceNo = DeviceModule.GetFullDeviceNoByID(device.Id);
                    string resDeviceNo = DeviceModule.GetDeviceNoMain(deviceNo);

                    /*
                    if (userToken.DeviceInfo == null)
                    {
                        ShowLogData.add("发送给终端编号为：" + resDeviceNo);
                        userToken.DeviceInfo = new DeviceInfo(DeviceModule.DeviceNo_Normal2Hex(resDeviceNo));
                    }
                    */

                    if (OnlineDeviceService.IsOnline(deviceNo))
                    {
                        ShowLogData.add("发送给终端编号为：" + deviceNo);
                        userToken.DeviceInfo = new DeviceInfo(DeviceModule.DeviceNo_Normal2Hex(deviceNo));
                    }
                    else if (OnlineDeviceService.IsOnline(resDeviceNo))
                    {
                        ShowLogData.add("发送给终端编号为：" + resDeviceNo);
                        userToken.DeviceInfo = new DeviceInfo(DeviceModule.DeviceNo_Normal2Hex(resDeviceNo));
                    }
                    else
                    {
                        string HexStr = BaseProtocol.DeviceOffline;
                        byte[] bs = HexStringUtility.HexStringToByteArray(HexStr);
                        send(bs, 0, bs.Length);
                        ShowLogData.add("返回web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]结果：终端不在线！" + HexStr);

                        receiveBuffer.clearBuffer(length);
                        ShowLogData.add("终端编号" + deviceNo + (deviceNo != resDeviceNo ? "或" + resDeviceNo : "") + "终端不在线！无法发送！");
                        return false;
                    }

                    if (message_water.AFN != (byte)WaterBaseProtocol.AFN._36)
                    {
                        ToWaterDtuCommand.AddBaseMessageToDtu(cKey + "-" + message_water.AFN, message_water);
                        ShowLogData.add("添加发送命令保存！" + cKey + "-" + message_water.AFN);
                    }
                    else
                    {
                        ShowLogData.add("不添加添加发送命令保存！" + cKey + "-" + message_water.AFN);
                    }

                    proxySend(buffer_new, 0, buffer_new.Length);

                    if (message_water.AFN != (byte)WaterBaseProtocol.AFN._36)
                    {
                        if (ToWaterDtuCommand.GetBaseMessageToDtuByKey(cKey + "-" + message_water.AFN) != null)
                        {
                            int timeDelay = 0;
                            while (timeDelay < 300)
                            {
                                WaterBaseMessage res = ToWaterDtuCommand.GetBaseMessageFromDtuByKey(cKey + "-" + message_water.AFN);
                                if (res != null)
                                {
                                    if (message_water.AFN == (byte)WaterBaseProtocol.AFN._37)
                                    {
                                        WaterCmd_37_2 cmd2 = (WaterCmd_37_2)res;
                                        byte[] bs = cmd2.UserDataBytesAll;
                                        send(bs, 0, bs.Length);
                                        ShowLogData.add("返回web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]结果：" + cmd2.UserDataAll);
                                        break;
                                    }
                                    else if (message_water.AFN == (byte)WaterBaseProtocol.AFN._38)
                                    {
                                        WaterCmd_38_2 cmd2 = (WaterCmd_38_2)res;
                                        byte[] bs = cmd2.UserDataBytesAll;
                                        send(bs, 0, bs.Length);
                                        ShowLogData.add("返回web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]结果：" + cmd2.UserDataAll);
                                        break;
                                    }
                                    else if (message_water.AFN == (byte)WaterBaseProtocol.AFN._39)
                                    {
                                        WaterCmd_39_2 cmd2 = (WaterCmd_39_2)res;
                                        byte[] bs = cmd2.UserDataBytesAll;
                                        send(bs, 0, bs.Length);
                                        ShowLogData.add("返回web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]结果：" + cmd2.UserDataAll);
                                        break;
                                    }
                                    else if (message_water.AFN == (byte)WaterBaseProtocol.AFN._3A)
                                    {
                                        WaterCmd_3A_2 cmd2 = (WaterCmd_3A_2)res;
                                        byte[] bs = cmd2.UserDataBytesAll;
                                        send(bs, 0, bs.Length);
                                        ShowLogData.add("返回web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]结果：" + cmd2.UserDataAll);
                                        break;
                                    }
                                    else if (message_water.AFN == (byte)WaterBaseProtocol.AFN._40)
                                    {
                                        WaterCmd_40_2 cmd2 = (WaterCmd_40_2)res;
                                        byte[] bs = cmd2.UserDataBytesAll;
                                        send(bs, 0, bs.Length);
                                        ShowLogData.add("返回web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]结果：" + cmd2.UserDataAll);
                                        break;
                                    }
                                    else if (message_water.AFN == (byte)WaterBaseProtocol.AFN._41)
                                    {
                                        WaterCmd_41_2 cmd2 = (WaterCmd_41_2)res;
                                        byte[] bs = cmd2.UserDataBytesAll;
                                        send(bs, 0, bs.Length);
                                        ShowLogData.add("返回web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]结果：" + cmd2.UserDataAll);
                                        break;
                                    }
                                    else if (message_water.AFN == (byte)WaterBaseProtocol.AFN._42)
                                    {
                                        WaterCmd_42_2 cmd2 = (WaterCmd_42_2)res;
                                        byte[] bs = cmd2.UserDataBytesAll;
                                        send(bs, 0, bs.Length);
                                        ShowLogData.add("返回web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]结果：" + cmd2.UserDataAll);
                                        break;
                                    }
                                    else if (message_water.AFN == (byte)WaterBaseProtocol.AFN._43)
                                    {
                                        WaterCmd_43_2 cmd2 = (WaterCmd_43_2)res;
                                        byte[] bs = cmd2.UserDataBytesAll;
                                        send(bs, 0, bs.Length);
                                        ShowLogData.add("返回web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]结果：" + cmd2.UserDataAll);
                                        break;
                                    }
                                    else if (message_water.AFN == (byte)WaterBaseProtocol.AFN._44)
                                    {
                                        WaterCmd_44_2 cmd2 = (WaterCmd_44_2)res;
                                        byte[] bs = cmd2.UserDataBytesAll;
                                        send(bs, 0, bs.Length);
                                        ShowLogData.add("返回web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]结果：" + cmd2.UserDataAll);
                                        break;
                                    }
                                    else if (message_water.AFN == (byte)WaterBaseProtocol.AFN._45)
                                    {
                                        WaterCmd_45_2 cmd2 = (WaterCmd_45_2)res;
                                        byte[] bs = cmd2.UserDataBytesAll;
                                        send(bs, 0, bs.Length);
                                        ShowLogData.add("返回web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]结果：" + cmd2.UserDataAll);
                                        break;
                                    }
                                    else if (message_water.AFN == (byte)WaterBaseProtocol.AFN._46)
                                    {
                                        WaterCmd_46_2 cmd2 = (WaterCmd_46_2)res;
                                        byte[] bs = cmd2.UserDataBytesAll;
                                        send(bs, 0, bs.Length);
                                        ShowLogData.add("返回web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]结果：" + cmd2.UserDataAll);
                                        break;
                                    }
                                    else if (message_water.AFN == (byte)WaterBaseProtocol.AFN._47)
                                    {
                                        WaterCmd_47_2 cmd2 = (WaterCmd_47_2)res;
                                        byte[] bs = cmd2.UserDataBytesAll;
                                        send(bs, 0, bs.Length);
                                        ShowLogData.add("返回web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]结果：" + cmd2.UserDataAll);
                                        break;
                                    }
                                    else if (message_water.AFN == (byte)WaterBaseProtocol.AFN._48)
                                    {
                                        WaterCmd_48_2 cmd2 = (WaterCmd_48_2)res;
                                        byte[] bs = cmd2.UserDataBytesAll;
                                        send(bs, 0, bs.Length);
                                        ShowLogData.add("返回web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]结果：" + cmd2.UserDataAll);
                                        break;
                                    }
                                    else if (message_water.AFN == (byte)WaterBaseProtocol.AFN._49)
                                    {
                                        WaterCmd_49_2 cmd2 = (WaterCmd_49_2)res;
                                        byte[] bs = cmd2.UserDataBytesAll;
                                        send(bs, 0, bs.Length);
                                        ShowLogData.add("返回web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]结果：" + cmd2.UserDataAll);
                                        break;
                                    }
                                    else if (message_water.AFN == (byte)WaterBaseProtocol.AFN._4A)
                                    {
                                        WaterCmd_4A_2 cmd2 = (WaterCmd_4A_2)res;
                                        byte[] bs = cmd2.UserDataBytesAll;
                                        send(bs, 0, bs.Length);
                                        ShowLogData.add("返回web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]结果：" + cmd2.UserDataAll);
                                        break;
                                    }
                                    else if (message_water.AFN == (byte)WaterBaseProtocol.AFN._4B)
                                    {
                                        WaterCmd_4B_2 cmd2 = (WaterCmd_4B_2)res;
                                        byte[] bs = cmd2.UserDataBytesAll;
                                        send(bs, 0, bs.Length);
                                        ShowLogData.add("返回web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]结果：" + cmd2.UserDataAll);
                                        break;
                                    }
                                    else if (message_water.AFN == (byte)WaterBaseProtocol.AFN._4C)
                                    {
                                        WaterCmd_4C_2 cmd2 = (WaterCmd_4C_2)res;
                                        byte[] bs = cmd2.UserDataBytesAll;
                                        send(bs, 0, bs.Length);
                                        ShowLogData.add("返回web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]结果：" + cmd2.UserDataAll);
                                        break;
                                    }
                                    else if (message_water.AFN == (byte)WaterBaseProtocol.AFN._4D)
                                    {
                                        WaterCmd_4D_2 cmd2 = (WaterCmd_4D_2)res;
                                        byte[] bs = cmd2.UserDataBytesAll;
                                        send(bs, 0, bs.Length);
                                        ShowLogData.add("返回web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]结果：" + cmd2.UserDataAll);
                                        break;
                                    }
                                    else if (message_water.AFN == (byte)WaterBaseProtocol.AFN._4E)
                                    {
                                        WaterCmd_4E_2 cmd2 = (WaterCmd_4E_2)res;
                                        byte[] bs = cmd2.UserDataBytesAll;
                                        send(bs, 0, bs.Length);
                                        ShowLogData.add("返回web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]结果：" + cmd2.UserDataAll);
                                        break;
                                    }
                                    else if (message_water.AFN == (byte)WaterBaseProtocol.AFN._4F)
                                    {
                                        WaterCmd_4F_2 cmd2 = (WaterCmd_4F_2)res;
                                        byte[] bs = cmd2.UserDataBytesAll;
                                        send(bs, 0, bs.Length);
                                        ShowLogData.add("返回web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]结果：" + cmd2.UserDataAll);
                                        break;
                                    }
                                    else if (message_water.AFN == (byte)WaterBaseProtocol.AFN._50)
                                    {
                                        WaterCmd_50_2 cmd2 = (WaterCmd_50_2)res;
                                        byte[] bs = cmd2.UserDataBytesAll;
                                        send(bs, 0, bs.Length);
                                        ShowLogData.add("返回web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]结果：" + cmd2.UserDataAll);
                                        break;
                                    }
                                    else if (message_water.AFN == (byte)WaterBaseProtocol.AFN._51)
                                    {
                                        WaterCmd_51_2 cmd2 = (WaterCmd_51_2)res;
                                        byte[] bs = cmd2.UserDataBytesAll;
                                        send(bs, 0, bs.Length);
                                        ShowLogData.add("返回web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]结果：" + cmd2.UserDataAll);
                                        break;
                                    }
                                    else if (message_water.AFN == (byte)WaterBaseProtocol.AFN._20)
                                    {
                                        WaterCmd_20_2 cmd2 = (WaterCmd_20_2)res;
                                        byte[] bs = cmd2.UserDataBytesAll;
                                        send(bs, 0, bs.Length);
                                        ShowLogData.add("返回web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]结果：" + cmd2.UserDataAll);
                                        break;
                                    }
                                    else if (message_water.AFN == (byte)WaterBaseProtocol.AFN._21)
                                    {
                                        WaterCmd_21_2 cmd2 = (WaterCmd_21_2)res;
                                        byte[] bs = cmd2.UserDataBytesAll;
                                        send(bs, 0, bs.Length);
                                        ShowLogData.add("返回web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]结果：" + cmd2.UserDataAll);
                                        break;
                                    }
                                    else if (message_water.AFN == (byte)WaterBaseProtocol.AFN._22)
                                    {
                                        WaterCmd_22_2 cmd2 = (WaterCmd_22_2)res;
                                        byte[] bs = cmd2.UserDataBytesAll;
                                        send(bs, 0, bs.Length);
                                        ShowLogData.add("返回web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]结果：" + cmd2.UserDataAll);
                                        break;
                                    }
                                    else if (message_water.AFN == (byte)WaterBaseProtocol.AFN._23)
                                    {
                                        WaterCmd_23_2 cmd2 = (WaterCmd_23_2)res;
                                        byte[] bs = cmd2.UserDataBytesAll;
                                        send(bs, 0, bs.Length);
                                        ShowLogData.add("返回web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]结果：" + cmd2.UserDataAll);
                                        break;
                                    }
                                    else if (message_water.AFN == (byte)WaterBaseProtocol.AFN._24)
                                    {
                                        WaterCmd_24_2 cmd2 = (WaterCmd_24_2)res;
                                        byte[] bs = cmd2.UserDataBytesAll;
                                        send(bs, 0, bs.Length);
                                        ShowLogData.add("返回web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]结果：" + cmd2.UserDataAll);
                                        break;
                                    }
                                    else if (message_water.AFN == (byte)WaterBaseProtocol.AFN._25)
                                    {
                                        WaterCmd_25_2 cmd2 = (WaterCmd_25_2)res;
                                        byte[] bs = cmd2.UserDataBytesAll;
                                        send(bs, 0, bs.Length);
                                        ShowLogData.add("返回web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]结果：" + cmd2.UserDataAll);
                                        break;
                                    }
                                    else if (message_water.AFN == (byte)WaterBaseProtocol.AFN._26)
                                    {
                                        WaterCmd_26_2 cmd2 = (WaterCmd_26_2)res;
                                        byte[] bs = cmd2.UserDataBytesAll;
                                        send(bs, 0, bs.Length);
                                        ShowLogData.add("返回web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]结果：" + cmd2.UserDataAll);
                                        break;
                                    }
                                    else if (message_water.AFN == (byte)WaterBaseProtocol.AFN._27)
                                    {
                                        WaterCmd_27_2 cmd2 = (WaterCmd_27_2)res;
                                        byte[] bs = cmd2.UserDataBytesAll;
                                        send(bs, 0, bs.Length);
                                        ShowLogData.add("返回web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]结果：" + cmd2.UserDataAll);
                                        break;
                                    }
                                    //kqz 2017-1-1添加
                                    else if (message_water.AFN == (byte)WaterBaseProtocol.AFN._28)
                                    {
                                        WaterCmd_28_2 cmd2 = (WaterCmd_28_2)res;
                                        byte[] bs = cmd2.UserDataBytesAll;
                                        send(bs, 0, bs.Length);
                                        ShowLogData.add("返回web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]结果：" + cmd2.UserDataAll);
                                        break;
                                    }
                                    else if (message_water.AFN == (byte)WaterBaseProtocol.AFN._29)
                                    {
                                        WaterCmd_29_2 cmd2 = (WaterCmd_29_2)res;
                                        byte[] bs = cmd2.UserDataBytesAll;
                                        send(bs, 0, bs.Length);
                                        ShowLogData.add("返回web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]结果：" + cmd2.UserDataAll);
                                        break;
                                    }
                                    //kqz 2017-1-1添加
                                }
                                Thread.Sleep(100);
                                timeDelay = timeDelay + 1;
                            }
                        }
                        else
                        {
                            string HexStr = BaseProtocol.DeviceOffline;
                            byte[] bs = HexStringUtility.HexStringToByteArray(HexStr);
                            send(bs, 0, bs.Length);
                            ShowLogData.add("为查询到保存的发送命令！" + cKey + "-" + message_water.AFN);
                        }
                    }
                    else
                    {
                        string HexStr = BaseProtocol.DeviceSend;
                        byte[] bs = HexStringUtility.HexStringToByteArray(HexStr);
                        send(bs, 0, bs.Length);
                        ShowLogData.add("返回web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]结果：发送完成！" + HexStr);

                        receiveBuffer.clearBuffer(length);
                        ShowLogData.add("终端编号" + deviceNo + (deviceNo != resDeviceNo ? "或" + resDeviceNo : "") + "发送完成！");
                    }

                    receiveBuffer.clearBuffer(length);
                    return true;
                }
                else
                {
                    if (message_water.IsWaterMessage)
                    {
                        ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]分析出错：信息为水文监测数据，停止！");
                        return true;
                    }
                }

                BaseMessage message = new BaseMessage();
                message.RawDataStr = receiveHex;
                message.RawDataChar = buffer_new;
                string msg = message.ReadMsg();
                if (msg == "")
                {
                    if (message.ControlField == (byte)BaseProtocol.ControlField.ToDtu)
                    {
                        string deviceNo = DeviceModule.DeviceNo_Hex2Normal(message.AddressField);
                        if (deviceNo == "")
                        {
                            receiveBuffer.clearBuffer(length);
                            ShowLogData.add("从web端接收到的数据：终端编号出错");
                            return false;
                        }
                        string resDeviceNo = DeviceModule.GetDeviceNoMain(deviceNo);

                        /*
                        if (userToken.DeviceInfo == null)
                        {
                            ShowLogData.add("发送给终端编号为：" + resDeviceNo);
                            userToken.DeviceInfo = new DeviceInfo(DeviceModule.DeviceNo_Normal2Hex(resDeviceNo));
                        }
                        */
                        if (message.AFN == (byte)BaseProtocol.AFN.ToDtuSetDateTime)
                        {
                            ShowLogData.add("从web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收到的数据：设置终端时间");
                        }
                        else if (message.AFN == (byte)BaseProtocol.AFN.ToDtuSetYearExploitation)
                        {
                            ShowLogData.add("从web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收到的数据：设置终端年开采量");
                        }
                        else if (message.AFN == (byte)BaseProtocol.AFN.ToDtuQueryDateTime)
                        {
                            ShowLogData.add("从web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收到的数据：查询终端时间");
                        }
                        else if (message.AFN == (byte)BaseProtocol.AFN.ToDtuQueryYearExploitation)
                        {
                            ShowLogData.add("从web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收到的数据：查询终端年开采量");
                        }
                        else if (message.AFN == (byte)BaseProtocol.AFN.ToDtuOpenPump)
                        {
                            ShowLogData.add("从web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收到的数据：远程开泵");
                        }
                        else if (message.AFN == (byte)BaseProtocol.AFN.ToDtuClosePump)
                        {
                            ShowLogData.add("从web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收到的数据：远程关泵");
                        }
                        else if (message.AFN == (byte)BaseProtocol.AFN.ToDtuSetStationCode)
                        {
                            ShowLogData.add("从web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收到的数据：设置分站射频地址列表");
                        }
                        else if (message.AFN == (byte)BaseProtocol.AFN.ToDtuShieldSerialNumber)
                        {
                            ShowLogData.add("从web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收到的数据：屏蔽卡号");
                        }
                        else if (message.AFN == (byte)BaseProtocol.AFN.ToDtuShieldSerialNumberCancel)
                        {
                            ShowLogData.add("从web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收到的数据：解除屏蔽卡号");
                        }
                        else if (message.AFN == (byte)BaseProtocol.AFN.ToDtuSetGroundWaterParam)
                        {
                            ShowLogData.add("从web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收到的数据：水位计参数设置");
                        }
                        else if (message.AFN == (byte)BaseProtocol.AFN.ToDtuQueryGroundWaterParam)
                        {
                            ShowLogData.add("从web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收到的数据：水位计参数查询");
                        }
                        else if (message.AFN == (byte)BaseProtocol.AFN.ToDtuQueryGroundWater)
                        {
                            ShowLogData.add("从web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收到的数据：水位查询");
                        }
                        else if (message.AFN == (byte)BaseProtocol.AFN.ToDtuSendFile)
                        {
                            ShowLogData.add("从web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收到的数据：中心下发文件");
                        }

                        if (OnlineDeviceService.IsOnline(deviceNo))
                        {
                            ShowLogData.add("发送给终端编号为：" + deviceNo);
                            userToken.DeviceInfo = new DeviceInfo(DeviceModule.DeviceNo_Normal2Hex(deviceNo));
                        }
                        else if (OnlineDeviceService.IsOnline(resDeviceNo))
                        {
                            ShowLogData.add("发送给终端编号为：" + resDeviceNo);
                            userToken.DeviceInfo = new DeviceInfo(DeviceModule.DeviceNo_Normal2Hex(resDeviceNo));
                        }
                        else
                        {
                            string HexStr = BaseProtocol.DeviceOffline;
                            byte[] bs = HexStringUtility.HexStringToByteArray(HexStr);
                            send(bs, 0, bs.Length);
                            ShowLogData.add("返回web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]结果：终端不在线！" + HexStr);

                            receiveBuffer.clearBuffer(length);
                            ShowLogData.add("终端编号" + deviceNo + (deviceNo != resDeviceNo ? "或" + resDeviceNo : "") + "终端不在线！无法发送！");
                            return false;
                        }

                        ToDtuCommand.AddBaseMessageToDtu(message.AddressField + "-" + message.AFN, message);
                        proxySend(buffer_new, 0, buffer_new.Length);

                        if (ToDtuCommand.GetBaseMessageToDtuByKey(message.AddressField + "-" + message.AFN) != null)
                        {
                            int timeDelay = 0;
                            while (timeDelay < 300)
                            {
                                BaseMessage res = ToDtuCommand.GetBaseMessageFromDtuByKey(message.AddressField + "-" + message.AFN);
                                if (res != null)
                                {
                                    byte[] bs = res.RawDataChar;
                                    send(bs, 0, bs.Length);
                                    ShowLogData.add("返回web端[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]结果：" + res.RawDataStr);
                                    break;
                                }
                                Thread.Sleep(100);
                                timeDelay = timeDelay + 1;
                            }
                        }

                        receiveBuffer.clearBuffer(length);
                        return true;
                    }
                    else
                    {
                        receiveBuffer.clearBuffer(length);
                        ShowLogData.add("从web端接收到的数据：不是发给终端");
                        return false;
                    }
                }
                else
                {
                    receiveBuffer.clearBuffer(length);
                    if (SysCache.ShowInfoLog)
                    {
                        LogHelper.Info("从web端接收到的数据：" + receiveHex + "，" + msg);
                        ShowLogData.add("从web端接收到的数据：" + receiveHex + "，" + msg);
                    }
                    return false;
                }
            }
            else
            {
                if (SysCache.ShowInfoLog)
                {
                    LogHelper.Info("接收数据长度不够：" + receiveHex);
                    ShowLogData.add("接收数据长度不够：" + receiveHex);
                }
                //清除掉本次接收到的数据
                receiveBuffer.clearBuffer(length);
                return false;
            }

            //return processPacket(buffer, offset, length);
        }
        public bool processPacket(byte[] buffer, int offset, int length)
        {
            //通过receiveData对象的parsePacket来解析出命令
            receiveData.parsePacket(buffer, offset, length);
            DTU.GateWay.Protocol.Command command = (DTU.GateWay.Protocol.Command)receiveData.returnObject();
            string lenthEqual = "";
            lenthEqual = lenthEqual + string.Format("手机号码为：{0}={1}", command.DeviceInfo.SerialString, command.DeviceInfo.SerialLong);
            lenthEqual = lenthEqual + string.Format("数据为:{0}.", FormatHelper.GetGBKStringFromBytes(command.Data, 0, command.Data.Length));
            //将设备信息与socket客户端相关联，确定唯一
            userToken.DeviceInfo = command.DeviceInfo;
            if (SysCache.ShowInfoLog)
            {
                string info = string.Format("从web端接收到的数据，解析为command后，数据为：{0}", lenthEqual);
                LogHelper.Info(info);
                proxySendLog(lenthEqual);
            }

            processCommand(command);

            return true;
        }

        public virtual bool processCommand(DTU.GateWay.Protocol.Command command)
        {
            return true;
        }

        public bool sendComplete()
        {
            AsyncSendBufferManager sendBufferManager = userToken.AsyncSendBufferManager;
            sendBufferManager.clearFirstPacket();
            isSending = false;
            int offset = 0;
            int count = 0;
            if (sendBufferManager.getFirstPacket(ref offset, ref count))
            {
                isSending = true;
                return server.sendAsyncEvent(userToken.ConnectedSocket,
                    userToken.SendAysncEvetnArgs, sendBufferManager.DynamicBufferManager.Buffer, offset, count);
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
                    result = server.sendAsyncEvent(userToken.ConnectedSocket,
                        userToken.SendAysncEvetnArgs, sendBufferManager.DynamicBufferManager.Buffer, packetOffset, packetCount);
                }
            }
            return result;
        }

        public bool send(DTU.GateWay.Protocol.Command command)
        {
            byte[] sendData = ProtocolCommand.buildPlatFormCommand(command);
            AsyncSendBufferManager sendBufferManager = userToken.AsyncSendBufferManager;

            sendBufferManager.startPacket();
            sendBufferManager.DynamicBufferManager.writeBuffer(sendData);
            sendBufferManager.endPacket();


            bool result = true;
            if (!isSending)
            {
                int offset = 0;
                int count = 0;
                if (sendBufferManager.getFirstPacket(ref offset, ref count))
                {
                    isSending = true;
                    result = server.sendAsyncEvent(userToken.ConnectedSocket,
                        userToken.SendAysncEvetnArgs, sendBufferManager.DynamicBufferManager.Buffer, offset, count);
                }
            }
            return result;
        }

        public bool send()
        {
            AsyncSendBufferManager sendBufferManager = userToken.AsyncSendBufferManager;
            string data = sendData.getSendData();
            byte[] sendByte = FormatHelper.GetBytesFromAsciiString(data);

            sendBufferManager.startPacket();
            sendBufferManager.DynamicBufferManager.writeBuffer(sendByte);
            sendBufferManager.endPacket();


            bool result = true;
            if (!isSending)
            {
                int offset = 0;
                int count = 0;
                if (sendBufferManager.getFirstPacket(ref offset, ref count))
                {
                    isSending = true;
                    result = server.sendAsyncEvent(userToken.ConnectedSocket,
                        userToken.SendAysncEvetnArgs, sendBufferManager.DynamicBufferManager.Buffer, offset, count);
                }
            }
            return result;
        }

        #region 代理发送数据
        //日志
        protected void proxySendLog(string info)
        {
            DataTransfer transfer = new DataTransfer();
            transfer.TransferType = DataTransferType.DataTransferLog;
            transfer.TransferLogInfo = info;
            if (server.onServerDataSocketHandler != null)
            {
                server.onServerDataSocketHandler(server, userToken, transfer);
            }
        }
        //更新Device
        protected void proxyUpdateDevice(string kt, string info)
        {
            DataTransfer transfer = new DataTransfer();
            transfer.TransferType = DataTransferType.DataTransferUpdateDevice;
            transfer.KeyType = kt;
            transfer.Key = info;
            if (server.onServerDataSocketHandler != null)
            {
                server.onServerDataSocketHandler(server, userToken, transfer);
            }
        }
        //更新District
        protected void proxyUpdateDistrict(string kt, string info)
        {
            DataTransfer transfer = new DataTransfer();
            transfer.TransferType = DataTransferType.DataTransferUpdateDistrict;
            transfer.KeyType = kt;
            transfer.Key = info;
            if (server.onServerDataSocketHandler != null)
            {
                server.onServerDataSocketHandler(server, userToken, transfer);
            }
        }
        //更新CardUser
        protected void proxyUpdateCardUser(string kt, string info)
        {
            DataTransfer transfer = new DataTransfer();
            transfer.TransferType = DataTransferType.DataTransferUpdateCardUser;
            transfer.KeyType = kt;
            transfer.Key = info;
            if (server.onServerDataSocketHandler != null)
            {
                server.onServerDataSocketHandler(server, userToken, transfer);
            }
        }
        //数据
        protected void proxySend(byte[] buffer, int offset, int length)
        {
            DataTransfer transfer = new DataTransfer();
            transfer.TransferType = DataTransferType.DataTransferDataInfo;
            transfer.DeviceInfo = userToken.DeviceInfo.SerialLong;
            DataTransferInfo info = new DataTransferInfo();
            info.Data = buffer;
            info.Offset = offset;
            info.Length = length;
            info.DeviceNo = userToken.DeviceInfo.SerialLong;
            transfer.TransferDataInfo = info;

            if (server.onServerDataSocketHandler != null)
            {
                server.onServerDataSocketHandler(server, userToken, transfer);
            }
        }

        protected void proxySend(byte[] buffer)
        {
            proxySend(buffer, 0, buffer.Length);
        }

        protected void proxySend(string val)
        {
            byte[] buffer = FormatHelper.GetBytesFromGBKString(val);
            proxySend(buffer);
        }
        #endregion


    }
}
