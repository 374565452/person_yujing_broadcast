using Common;
using DTU.GateWay.Protocol;
using DTU.GateWay.Protocol.WaterMessageClass;
using Maticsoft.Model;
using Module;
using Server.Core.ServerCore;
using Server.Util.Cache;
using Server.Util.Log;
using Server.Util.Service;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace Server.Core.ProtocolProcess
{
    public class DTUInvokeElement : AsyncProtocolInvokeElement//继承
    {
        log4net.ILog LogHelper = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static Dictionary<string, DateTime> DictDeviceGetNew = new Dictionary<string, DateTime>();

        ServerSocket server = null;
        AsyncSocketUserToken userToken = null;
        public DTUInvokeElement(ServerSocket server, AsyncSocketUserToken userToken)
            : base(server, userToken)
        {
            this.server = server;
            this.userToken = userToken;
        }

        public override bool processCommand(string receive)//对父类AsyncprotocolInvokeElement中的方法重写
        {
            LogHelper.Info("收到的数据为：[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]" + receive);
            ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]" + receive);

            byte[] data = Encoding.UTF8.GetBytes(receive);
            //data[data.Length - 1] = FormatHelper.CheckSum(data, 0, data.Length - 1);
            send(data, 0, data.Length);

            return true;
        }

        public override bool processCommand(byte[] buffer, int offset, int length)
        {
            string receive = HexStringUtility.ByteArrayToHexString(buffer);
            LogHelper.Info("收到的数据偏移量为：" + offset + "，长度为：" + length + "，内容为：" + receive);
            ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":" + receive);
            return true;
        }

        //自定义水泵地下水通讯规约解析
        public override bool processCommandBase(byte[] buffer)
        {
            int length = buffer.Length;
            //string receive = HexStringUtility.ByteArrayToHexString(buffer);
            string receive = FormatHelper.ByteArrayToHexString(buffer);
            LogHelper.Info("收到的数据长度为：" + buffer.Length + "，内容为：" + receive);
            ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":" + receive);

            #region 自定义水泵地下水通讯规约
            BaseMessage message = new BaseMessage();
            message.RawDataChar = new byte[length];
            try
            {
                Array.Copy(buffer, message.RawDataChar, length);//message.RawDataChar = buffer;
                message.RawDataStr = HexStringUtility.ByteArrayToHexString(message.RawDataChar);

                string msg = message.ReadMsg();
                if (msg != "")
                {
                    if (userToken.DeviceInfo != null && userToken.DeviceList != null)
                    {
                        userToken.DeviceList.Online = 1;
                        userToken.DeviceList.LastUpdate = DateTime.Now;
                        userToken.DeviceList.Remark = message.RawDataStr;
                        userToken.DeviceList.TerminalState = "心跳";
                        updateDeviceList(userToken.DeviceList);
                        string FullDeviceNo = DeviceModule.GetFullDeviceNoByID(userToken.DeviceList.Id);
                        OnlineDeviceService.AddOnline(FullDeviceNo, userToken.DeviceList);
                        DeviceEvent deviceEvent = new DeviceEvent();
                        deviceEvent.DeviceNo = userToken.DeviceInfo.DeviceNo;
                        deviceEvent.EventTime = userToken.DeviceList.LastUpdate;
                        deviceEvent.EventType = userToken.DeviceList.TerminalState;
                        deviceEvent.Remark = "";
                        deviceEvent.RawData = message.RawDataStr;
                        proxySendDeviceList(userToken.DeviceList, deviceEvent);
                    }
                    byte[] data = HexStringUtility.StrToByteArray("%");
                    send(data, 0, data.Length);
                    string info1 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]响应（信息不对）：" + msg;
                    LogHelper.Info(info1);
                    ShowLogData.add(info1);
                }
                else
                {
                    string deviceNo = DeviceModule.DeviceNo_Hex2Normal(message.AddressField);
                    Device d = DeviceModule.GetDeviceByFullDeviceNo(deviceNo);
                    if (d == null)
                    {
                        DateTime dateNew = DateTime.Now;
                        if (DictDeviceGetNew.ContainsKey(deviceNo))
                        {
                            DateTime dateOld = DictDeviceGetNew[deviceNo];
                            if ((dateNew - dateOld).TotalSeconds > 60 * 2)
                            {
                                d = DeviceModule.GetDeviceByFullDeviceNo_DB(deviceNo);
                                if (DictDeviceGetNew.ContainsKey(deviceNo)) DictDeviceGetNew.Remove(deviceNo);
                                DictDeviceGetNew.Add(deviceNo, dateNew);
                                string info1 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]响应：" + "数据库新查询终端：" + deviceNo;
                                ShowLogData.add(info1);
                                LogHelper.Info(info1);
                            }
                        }
                        else
                        {
                            d = DeviceModule.GetDeviceByFullDeviceNo_DB(deviceNo);
                            DictDeviceGetNew.Add(deviceNo, dateNew);
                            string info1 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]响应：" + "数据库新查询终端：" + deviceNo;
                            ShowLogData.add(info1);
                            LogHelper.Info(info1);
                        }
                    }
                    if (d == null)
                    {
                        string info1 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]响应：" + "未查询到终端：" + deviceNo;
                        ShowLogData.add(info1);
                        LogHelper.Info(info1);
                        return false;
                    }
                    else
                    {
                        string info1 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]响应：" + "查询到终端：" + deviceNo + "，终端名称：" + d.DeviceName;
                        ShowLogData.add(info1);
                        LogHelper.Info(info1);
                    }

                    Device deviceMain = null;
                    if (d.StationType == 2)
                    {
                        deviceMain = DeviceModule.GetDeviceByID(d.MainId);
                    }

                    if (message.ControlField == (byte)BaseProtocol.ControlField.FromDtu
                         && message.AFN == (byte)BaseProtocol.AFN.FromDtuLogin)
                    {
                        CmdResponseFromDtuLogin cmdres = new CmdResponseFromDtuLogin();
                        cmdres.AddressField = message.AddressField;
                        if (d == null)
                        {
                            cmdres.Result = 0;
                        }
                        else
                        {
                            string infoZC = "";
                            if (userToken.DeviceList == null)
                            {
                                infoZC = "[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]：userToken.DeviceList不存在，新添加";
                                userToken.DeviceInfo = new DeviceInfo(message.AddressField);
                                userToken.DeviceList = d;
                            }
                            else
                            {
                                if (deviceMain == null)
                                {
                                    infoZC = "[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]：userToken.DeviceList已存在，无主站，修改为自己";
                                    userToken.DeviceInfo = new DeviceInfo(message.AddressField);
                                    userToken.DeviceList = d;
                                }
                                else
                                {
                                    if (userToken.DeviceList.Id == d.MainId)
                                    {
                                        infoZC = "[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]：userToken.DeviceList已存在，有主站，当前id为主站id，不修改";
                                    }
                                    else
                                    {
                                        infoZC = "[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]：userToken.DeviceList已存在，有主站，当前id不为主站id，修改为自己";
                                        userToken.DeviceInfo = new DeviceInfo(message.AddressField);
                                        userToken.DeviceList = d;
                                    }
                                }
                            }
                            ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":" + infoZC);
                            LogHelper.Info(infoZC);

                            List<AsyncSocketUserToken> list = new List<AsyncSocketUserToken>();
                            foreach (AsyncSocketUserToken tokenTmp in server.AsyncSocketUserTokenList.UserTokenList)
                            {
                                if (tokenTmp.DeviceInfo != null && userToken.DeviceInfo != null)
                                {
                                    string d1 = tokenTmp.DeviceInfo.SerialString;
                                    string d2 = userToken.DeviceInfo.SerialString;
                                    string c1 = tokenTmp.ConnectedSocket.RemoteEndPoint.ToString();
                                    string c2 = userToken.ConnectedSocket.RemoteEndPoint.ToString();
                                    if (d1 == d2 && c1 != c2)
                                    {
                                        string info1 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + tokenTmp.ConnectedSocket.RemoteEndPoint.ToString() + "]：连接关闭，通讯设备相同IP地址不同";
                                        ShowLogData.add(info1);
                                        LogHelper.Info(info1);
                                        list.Add(tokenTmp);
                                        break;
                                    }

                                    if (tokenTmp.DeviceList.Id == d.Id && c1 != c2)
                                    {
                                        string info1 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + tokenTmp.ConnectedSocket.RemoteEndPoint.ToString() + "]：连接关闭，已存在相同设备连接";
                                        ShowLogData.add(info1);
                                        LogHelper.Info(info1);
                                        list.Add(tokenTmp);
                                    }
                                }
                            }

                            foreach (AsyncSocketUserToken tokenTmp in list)
                            {
                                server.closeClientSocket(tokenTmp, false);
                            }

                            d.Online = 1;
                            d.OnlineTime = DateTime.Now;
                            d.LastUpdate = DateTime.Now;
                            d.Remark = message.RawDataStr;
                            d.TerminalState = "设备登录";
                            updateDeviceList(d);
                            OnlineDeviceService.AddOnline(deviceNo, d);
                            DeviceEvent deviceEvent = new DeviceEvent();
                            DeviceEventModule.InitDeviceEvent(deviceEvent);
                            deviceEvent.DeviceNo = deviceNo;
                            deviceEvent.EventTime = d.LastUpdate;
                            deviceEvent.EventType = d.TerminalState;
                            deviceEvent.DeviceTime = d.LastUpdate;
                            deviceEvent.Remark = deviceNo + "注册登录";
                            deviceEvent.RawData = message.RawDataStr;
                            saveDeviceEvent(deviceEvent);
                            cmdres.StationType = (byte)d.StationType;
                            cmdres.StationCode = d.StationType == 2 ? d.StationCode : 0;
                            cmdres.Result = 1;
                            proxySendDeviceList(d, deviceEvent);
                        }
                        cmdres.RawDataChar = cmdres.WriteMsg();
                        cmdres.RawDataStr = HexStringUtility.ByteArrayToHexString(cmdres.RawDataChar);
                        send(cmdres.RawDataChar, 0, cmdres.RawDataChar.Length);
                    }
                    else if (message.ControlField == (byte)BaseProtocol.ControlField.FromDtu
                         && message.AFN == (byte)BaseProtocol.AFN.FromDtuStateReport)
                    {
                        CmdFromDtuStateReport cmdreceive = new CmdFromDtuStateReport(message);
                        //cmdreceive.UserData = message.UserData;
                        string msgreceive = cmdreceive.ReadMsg();

                        CmdResponseFromDtuStateReport cmdres = new CmdResponseFromDtuStateReport();
                        cmdres.AddressField = message.AddressField;
                        if (d == null)
                        {
                            cmdres.Result = 0;
                        }
                        else
                        {
                            d.Online = 1;
                            d.LastUpdate = DateTime.Now;
                            d.Remark = message.RawDataStr;
                            d.TerminalState = "状态自报";
                            if (cmdreceive.WaterUsed > 0)
                                d.WaterUsed = cmdreceive.WaterUsed;
                            updateDeviceList(d);
                            OnlineDeviceService.AddOnline(deviceNo, d);
                            DeviceEvent deviceEvent = new DeviceEvent();
                            DeviceEventModule.InitDeviceEvent(deviceEvent);
                            deviceEvent.DeviceNo = deviceNo;
                            deviceEvent.EventTime = d.LastUpdate;
                            deviceEvent.EventType = d.TerminalState;
                            deviceEvent.DeviceState = cmdreceive.State == null ? "0".PadLeft(32, '0') : cmdreceive.State.PadLeft(32, '0');
                            deviceEvent.YearWaterUsed = cmdreceive.WaterUsed;
                            deviceEvent.RawData = message.RawDataStr;
                            AlarmService.DeviceAlarmAnalyse(deviceEvent);
                            if (msgreceive != "")
                            {
                                cmdres.Result = 0;
                                deviceEvent.DeviceTime = d.LastUpdate;
                                deviceEvent.Remark = "状态自报出错：" + msgreceive;
                            }
                            else
                            {
                                cmdres.Result = 1;
                                deviceEvent.DeviceTime = cmdreceive.DateTimeNew;
                                deviceEvent.Remark = "终端自报状态码：" + cmdreceive.State +
                                    "，上报时间：" + cmdreceive.DateTimeNew.ToString("yyyy-MM-dd HH:mm:ss") +
                                    //"，累计用电量：" + cmdreceive.ElectricUsed + 
                                    "，累计用水量：" + cmdreceive.WaterUsed;
                            }
                            saveDeviceEvent(deviceEvent);
                            cmdres.StationType = (byte)d.StationType;
                            cmdres.StationCode = d.StationType == 2 ? d.StationCode : 0;

                            proxySendDeviceList(d, deviceEvent);
                        }
                        cmdres.DateTimeNew = DateTime.Now;
                        cmdres.RawDataChar = cmdres.WriteMsg();
                        cmdres.RawDataStr = HexStringUtility.ByteArrayToHexString(cmdres.RawDataChar);
                        send(cmdres.RawDataChar, 0, cmdres.RawDataChar.Length);
                    }
                    else if (message.ControlField == (byte)BaseProtocol.ControlField.FromDtu
                         && message.AFN == (byte)BaseProtocol.AFN.FromDtuOpenPump)
                    {
                        CmdFromDtuOpenPump cmdreceive = new CmdFromDtuOpenPump(message);
                        //cmdreceive.UserData = message.UserData;
                        string msgreceive = cmdreceive.ReadMsg();

                        CmdResponseFromDtuOpenPump cmdres = new CmdResponseFromDtuOpenPump();
                        cmdres.AddressField = message.AddressField;
                        if (d == null)
                        {
                            cmdres.Result = 0;
                        }
                        else
                        {
                            d.Online = 1;
                            d.LastUpdate = DateTime.Now;
                            d.Remark = message.RawDataStr;
                            d.TerminalState = "开泵上报";
                            updateDeviceList(d);
                            OnlineDeviceService.AddOnline(deviceNo, d);
                            DeviceEvent deviceEvent = new DeviceEvent();
                            DeviceEventModule.InitDeviceEvent(deviceEvent);
                            deviceEvent.DeviceNo = deviceNo;
                            deviceEvent.EventTime = d.LastUpdate;
                            deviceEvent.EventType = d.TerminalState;
                            deviceEvent.UserNo = cmdreceive.UserNo;
                            deviceEvent.SerialNumber = cmdreceive.SerialNumber;
                            deviceEvent.StartTime = cmdreceive.StartTime;
                            deviceEvent.StartResidualWater = cmdreceive.StartResidualWater;
                            deviceEvent.StartResidualElectric = cmdreceive.StartResidualElectric;
                            deviceEvent.YearWaterUsed = cmdreceive.YearWaterUsed;
                            deviceEvent.YearElectricUsed = cmdreceive.YearElectricUsed;
                            deviceEvent.YearExploitation = cmdreceive.YearExploitation;
                            deviceEvent.YearSurplus = cmdreceive.YearSurplus;
                            deviceEvent.RawData = message.RawDataStr;
                            if (msgreceive != "")
                            {
                                cmdres.Result = 0;
                                deviceEvent.DeviceTime = d.LastUpdate;
                                deviceEvent.Remark = "开泵上报出错：" + msgreceive;

                            }
                            else
                            {
                                cmdres.Result = 1;
                                deviceEvent.DeviceTime = cmdreceive.StartTime;
                                deviceEvent.Remark = "开泵时间：" + deviceEvent.StartTime.ToString("yyyy-MM-dd HH:mm:ss") + "，卡号：" + deviceEvent.UserNo + "，卡剩余水量：" + deviceEvent.StartResidualWater + "，卡剩余电量：" + deviceEvent.StartResidualElectric + "";
                            }
                            saveDeviceEvent(deviceEvent);
                            cmdres.StationType = (byte)d.StationType;
                            cmdres.StationCode = d.StationType == 2 ? d.StationCode : 0;

                            proxySendDeviceList(d, deviceEvent);
                        }
                        cmdres.RawDataChar = cmdres.WriteMsg();
                        cmdres.RawDataStr = HexStringUtility.ByteArrayToHexString(cmdres.RawDataChar);
                        send(cmdres.RawDataChar, 0, cmdres.RawDataChar.Length);
                    }
                    else if (message.ControlField == (byte)BaseProtocol.ControlField.FromDtu
                       && message.AFN == (byte)BaseProtocol.AFN.FromDtuClosePump)
                    {
                        CmdFromDtuClosePump cmdreceive = new CmdFromDtuClosePump(message);
                        //cmdreceive.UserData = message.UserData;
                        string msgreceive = cmdreceive.ReadMsg();

                        CmdResponseFromDtuClosePump cmdres = new CmdResponseFromDtuClosePump();
                        cmdres.AddressField = message.AddressField;
                        if (d == null)
                        {
                            cmdres.Result = 0;
                        }
                        else
                        {
                            d.Online = 1;
                            d.LastUpdate = DateTime.Now;
                            d.Remark = message.RawDataStr;
                            d.TerminalState = "关泵上报";
                            updateDeviceList(d);
                            OnlineDeviceService.AddOnline(deviceNo, d);
                            DeviceEvent deviceEvent = new DeviceEvent();
                            DeviceEventModule.InitDeviceEvent(deviceEvent);
                            deviceEvent.DeviceNo = deviceNo;
                            deviceEvent.EventTime = d.LastUpdate;
                            deviceEvent.EventType = d.TerminalState;
                            deviceEvent.UserNo = cmdreceive.UserNo;
                            deviceEvent.SerialNumber = cmdreceive.SerialNumber;
                            deviceEvent.StartTime = cmdreceive.StartTime;
                            deviceEvent.StartResidualWater = cmdreceive.StartResidualWater;
                            deviceEvent.StartResidualElectric = cmdreceive.StartResidualElectric;
                            deviceEvent.EndTime = cmdreceive.EndTime;
                            deviceEvent.EndResidualWater = cmdreceive.EndResidualWater;
                            deviceEvent.EndResidualElectric = cmdreceive.EndResidualElectric;
                            deviceEvent.WaterUsed = cmdreceive.WaterUsed;
                            deviceEvent.ElectricUsed = cmdreceive.ElectricUsed;
                            deviceEvent.YearWaterUsed = cmdreceive.YearWaterUsed;
                            deviceEvent.YearElectricUsed = cmdreceive.YearElectricUsed;
                            deviceEvent.YearExploitation = cmdreceive.YearExploitation;
                            deviceEvent.YearSurplus = cmdreceive.YearSurplus;
                            deviceEvent.RawData = message.RawDataStr;
                            if (msgreceive != "")
                            {
                                cmdres.Result = 0;
                                deviceEvent.DeviceTime = d.LastUpdate;
                                deviceEvent.Remark = "关泵上报出错：" + msgreceive;

                            }
                            else
                            {
                                cmdres.Result = 1;
                                deviceEvent.DeviceTime = cmdreceive.EndTime;
                                deviceEvent.Remark = "关泵时间：" + deviceEvent.EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "，卡号：" + deviceEvent.UserNo + "，卡剩余水量：" + deviceEvent.StartResidualWater + "，卡剩余电量：" + deviceEvent.StartResidualElectric +
                                    ",本次水量：" + deviceEvent.WaterUsed + "，电量：" + deviceEvent.ElectricUsed +
                                    "，记录类型：" + deviceEvent.RecordType + "，开泵类型：" + deviceEvent.REV1 + "，关泵类型：" + deviceEvent.REV2 + "";

                                try
                                {
                                    CardUser cu = CardUserModule.GetCardUserBySerialNumber(cmdreceive.SerialNumber);
                                    if (cu == null)
                                    {
                                        cu = CardUserModule.GetCardUserBySerialNumber_DB(cmdreceive.SerialNumber);
                                    }
                                    if (cu != null)
                                    {
                                        if (d != null)
                                            cu.LastConsumptionDeviceId = d.Id;
                                        else
                                            cu.LastConsumptionDeviceId = -1;
                                        cu.LastConsumptionDeviceNo = deviceNo;
                                        cu.LastConsumptionTime = cmdreceive.EndTime;
                                        cu.ResidualWater = deviceEvent.EndResidualWater;
                                        cu.ResidualElectric = deviceEvent.EndResidualElectric;
                                        cu.TotalWater = deviceEvent.YearWaterUsed;
                                        cu.TotalElectric = deviceEvent.YearElectricUsed;
                                        CardUserModule.ModifyCardUser(cu);

                                        CardUserWaterLog cuwLog = new CardUserWaterLog();
                                        cuwLog.SerialNumber = cmdreceive.SerialNumber;
                                        cuwLog.WateUserId = cu.WaterUserId;
                                        cuwLog.UserNo = cmdreceive.UserNo;
                                        if (d != null)
                                            cuwLog.DeviceId = d.Id;
                                        else
                                            cuwLog.DeviceId = -1;
                                        cuwLog.DeviceNo = deviceNo;
                                        cuwLog.StartTime = cmdreceive.StartTime;
                                        cuwLog.StartResidualWater = cmdreceive.StartResidualWater;
                                        cuwLog.StartResidualElectric = cmdreceive.StartResidualElectric;
                                        cuwLog.EndTime = cmdreceive.EndTime;
                                        cuwLog.EndResidualWater = cmdreceive.EndResidualWater;
                                        cuwLog.EndResidualElectric = cmdreceive.EndResidualElectric;
                                        cuwLog.WaterUsed = cmdreceive.WaterUsed;
                                        cuwLog.ElectricUsed = cmdreceive.ElectricUsed;
                                        cuwLog.Duration = Convert.ToDecimal((cmdreceive.EndTime - cmdreceive.StartTime).TotalSeconds);
                                        CardUserWaterLogModule.AddCardUserWaterLog(cuwLog);
                                    }
                                }
                                catch
                                {
                                }
                            }
                            saveDeviceEvent(deviceEvent);

                            cmdres.StationType = (byte)d.StationType;
                            cmdres.StationCode = d.StationType == 2 ? d.StationCode : 0;
                            proxySendDeviceList(d, deviceEvent);
                        }
                        cmdres.RawDataChar = cmdres.WriteMsg();
                        cmdres.RawDataStr = HexStringUtility.ByteArrayToHexString(cmdres.RawDataChar);
                        send(cmdres.RawDataChar, 0, cmdres.RawDataChar.Length);
                    }
                    else if (message.ControlField == (byte)BaseProtocol.ControlField.FromDtu
                        && message.AFN == (byte)BaseProtocol.AFN.ToDtuQueryDateTime)
                    {
                        CmdResponseToDtuQueryDateTime cmdreceive = new CmdResponseToDtuQueryDateTime(message);
                        //cmdreceive.UserData = message.UserData;
                        string msgreceive = cmdreceive.ReadMsg();
                        if (d != null)
                        {
                            d.Online = 1;
                            d.LastUpdate = DateTime.Now;
                            d.Remark = message.RawDataStr;
                            d.TerminalState = "查询时间响应";
                            updateDeviceList(d);
                            OnlineDeviceService.AddOnline(deviceNo, d);
                            DeviceEvent deviceEvent = new DeviceEvent();
                            DeviceEventModule.InitDeviceEvent(deviceEvent);
                            deviceEvent.DeviceNo = deviceNo;
                            deviceEvent.EventTime = d.LastUpdate;
                            deviceEvent.EventType = d.TerminalState;
                            deviceEvent.RawData = message.RawDataStr;
                            if (msgreceive != "")
                            {
                                deviceEvent.DeviceTime = d.LastUpdate;
                                deviceEvent.Remark = "查询时间响应出错：" + msgreceive;
                            }
                            else
                            {
                                deviceEvent.DeviceTime = cmdreceive.DateTimeNew;
                                deviceEvent.Remark = "查询终端时间为：" + cmdreceive.DateTimeNew.ToString("yyyy-MM-dd HH:mm:ss");
                            }
                            saveDeviceEvent(deviceEvent);
                            proxySendDeviceList(d, deviceEvent);

                            if (ToDtuCommand.GetBaseMessageToDtuByKey(message.AddressField + "-" + message.AFN) != null)
                            {
                                ToDtuCommand.AddBaseMessageFromDtu(cmdreceive.AddressField + "-" + cmdreceive.AFN, cmdreceive);
                                //ShowLogData.add("添加发送命令响应保存！" + cmdreceive.AddressField + "-" + cmdreceive.AFN + "：" + cmdreceive.RawDataStr);
                            }
                        }

                        //proxySendCommand(command);
                    }
                    else if (message.ControlField == (byte)BaseProtocol.ControlField.FromDtu
                        && message.AFN == (byte)BaseProtocol.AFN.ToDtuQueryYearExploitation)
                    {
                        CmdResponseToDtuQueryYearExploitation cmdreceive = new CmdResponseToDtuQueryYearExploitation(message);
                        //cmdreceive.UserData = message.UserData;
                        string msgreceive = cmdreceive.ReadMsg();
                        if (d != null)
                        {
                            d.Online = 1;
                            d.LastUpdate = DateTime.Now;
                            d.Remark = message.RawDataStr;
                            d.TerminalState = "查询开采量响应";
                            updateDeviceList(d);
                            OnlineDeviceService.AddOnline(deviceNo, d);
                            DeviceEvent deviceEvent = new DeviceEvent();
                            DeviceEventModule.InitDeviceEvent(deviceEvent);
                            deviceEvent.DeviceNo = deviceNo;
                            deviceEvent.EventTime = d.LastUpdate;
                            deviceEvent.EventType = d.TerminalState;
                            deviceEvent.DeviceTime = d.LastUpdate;
                            deviceEvent.YearExploitation = cmdreceive.YearExploitation;
                            deviceEvent.RawData = message.RawDataStr;
                            if (msgreceive != "")
                            {
                                deviceEvent.Remark = "查询开采量响应出错：" + msgreceive;
                            }
                            else
                            {
                                deviceEvent.Remark = "查询终端年可开采量为：" + cmdreceive.YearExploitation.ToString();
                            }
                            saveDeviceEvent(deviceEvent);
                            proxySendDeviceList(d, deviceEvent);

                            if (ToDtuCommand.GetBaseMessageToDtuByKey(message.AddressField + "-" + message.AFN) != null)
                            {
                                ToDtuCommand.AddBaseMessageFromDtu(cmdreceive.AddressField + "-" + cmdreceive.AFN, cmdreceive);
                                //ShowLogData.add("添加发送命令响应保存！" + cmdreceive.AddressField + "-" + cmdreceive.AFN + "：" + cmdreceive.RawDataStr);
                            }
                        }

                        //proxySendCommand(command);
                    }
                    else if (message.ControlField == (byte)BaseProtocol.ControlField.FromDtu
                        && message.AFN == (byte)BaseProtocol.AFN.ToDtuSetDateTime)
                    {
                        CmdResponseToDtuSetDateTime cmdreceive = new CmdResponseToDtuSetDateTime(message);
                        //cmdreceive.UserData = message.UserData;
                        string msgreceive = cmdreceive.ReadMsg();
                        if (d != null)
                        {
                            d.Online = 1;
                            d.LastUpdate = DateTime.Now;
                            d.Remark = message.RawDataStr;
                            d.TerminalState = "设置时间响应";
                            updateDeviceList(d);
                            OnlineDeviceService.AddOnline(deviceNo, d);
                            DeviceEvent deviceEvent = new DeviceEvent();
                            DeviceEventModule.InitDeviceEvent(deviceEvent);
                            deviceEvent.DeviceNo = deviceNo;
                            deviceEvent.EventTime = d.LastUpdate;
                            deviceEvent.EventType = d.TerminalState;
                            deviceEvent.RawData = message.RawDataStr;
                            if (msgreceive != "")
                            {
                                deviceEvent.DeviceTime = d.LastUpdate;
                                deviceEvent.Remark = "设置时间响应出错：" + msgreceive;
                            }
                            else
                            {
                                deviceEvent.DeviceTime = cmdreceive.DateTimeNew;
                                deviceEvent.Remark = "设置终端时间为：" + cmdreceive.DateTimeNew.ToString("yyyy-MM-dd HH:mm:ss");
                            }
                            saveDeviceEvent(deviceEvent);
                            proxySendDeviceList(d, deviceEvent);

                            if (ToDtuCommand.GetBaseMessageToDtuByKey(message.AddressField + "-" + message.AFN) != null)
                            {
                                ToDtuCommand.AddBaseMessageFromDtu(cmdreceive.AddressField + "-" + cmdreceive.AFN, cmdreceive);
                                //ShowLogData.add("添加发送命令响应保存！" + cmdreceive.AddressField + "-" + cmdreceive.AFN + "：" + cmdreceive.RawDataStr);
                            }
                        }

                        //proxySendCommand(command);
                    }
                    else if (message.ControlField == (byte)BaseProtocol.ControlField.FromDtu
                        && message.AFN == (byte)BaseProtocol.AFN.ToDtuSetYearExploitation)
                    {
                        CmdResponseToDtuSetYearExploitation cmdreceive = new CmdResponseToDtuSetYearExploitation(message);
                        //cmdreceive.UserData = message.UserData;
                        string msgreceive = cmdreceive.ReadMsg();
                        if (d != null)
                        {
                            d.Online = 1;
                            d.LastUpdate = DateTime.Now;
                            d.Remark = message.RawDataStr;
                            d.TerminalState = "设置开采量响应";
                            updateDeviceList(d);
                            OnlineDeviceService.AddOnline(deviceNo, d);
                            DeviceEvent deviceEvent = new DeviceEvent();
                            DeviceEventModule.InitDeviceEvent(deviceEvent);
                            deviceEvent.DeviceNo = deviceNo;
                            deviceEvent.EventTime = d.LastUpdate;
                            deviceEvent.EventType = d.TerminalState;
                            deviceEvent.DeviceTime = d.LastUpdate;
                            deviceEvent.YearExploitation = cmdreceive.YearExploitation;
                            deviceEvent.RawData = message.RawDataStr;
                            if (msgreceive != "")
                            {
                                deviceEvent.Remark = "设置开采量响应出错：" + msgreceive;
                            }
                            else
                            {
                                deviceEvent.Remark = "设置终端年可开采量为：" + cmdreceive.YearExploitation.ToString();
                            }
                            saveDeviceEvent(deviceEvent);
                            proxySendDeviceList(d, deviceEvent);

                            if (ToDtuCommand.GetBaseMessageToDtuByKey(message.AddressField + "-" + message.AFN) != null)
                            {
                                ToDtuCommand.AddBaseMessageFromDtu(cmdreceive.AddressField + "-" + cmdreceive.AFN, cmdreceive);
                                //ShowLogData.add("添加发送命令响应保存！" + cmdreceive.AddressField + "-" + cmdreceive.AFN + "：" + cmdreceive.RawDataStr);
                            }
                        }

                        //proxySendCommand(command);
                    }
                    else if (message.ControlField == (byte)BaseProtocol.ControlField.FromDtu
                        && message.AFN == (byte)BaseProtocol.AFN.ToDtuOpenPump)
                    {
                        CmdResponseToDtuOpenPump cmdreceive = new CmdResponseToDtuOpenPump(message);
                        //cmdreceive.UserData = message.UserData;
                        string msgreceive = cmdreceive.ReadMsg();
                        if (d != null)
                        {
                            d.Online = 1;
                            d.LastUpdate = DateTime.Now;
                            d.Remark = message.RawDataStr;
                            d.TerminalState = "远程开泵响应";
                            updateDeviceList(d);
                            OnlineDeviceService.AddOnline(deviceNo, d);
                            DeviceEvent deviceEvent = new DeviceEvent();
                            DeviceEventModule.InitDeviceEvent(deviceEvent);
                            deviceEvent.DeviceNo = deviceNo;
                            deviceEvent.EventTime = d.LastUpdate;
                            deviceEvent.EventType = d.TerminalState;
                            deviceEvent.DeviceTime = d.LastUpdate;
                            deviceEvent.RawData = message.RawDataStr;
                            if (msgreceive != "")
                            {
                                deviceEvent.Remark = "远程开泵响应出错：" + msgreceive;
                            }
                            else
                            {
                                deviceEvent.Remark = "远程开泵返回结果：远程开泵" + (cmdreceive.Result == 1 ? "成功" : "失败");
                            }
                            saveDeviceEvent(deviceEvent);
                            proxySendDeviceList(d, deviceEvent);

                            if (ToDtuCommand.GetBaseMessageToDtuByKey(message.AddressField + "-" + message.AFN) != null)
                            {
                                ToDtuCommand.AddBaseMessageFromDtu(cmdreceive.AddressField + "-" + cmdreceive.AFN, cmdreceive);
                                //ShowLogData.add("添加发送命令响应保存！" + cmdreceive.AddressField + "-" + cmdreceive.AFN + "：" + cmdreceive.RawDataStr);
                            }
                        }

                        //proxySendCommand(command);
                    }
                    else if (message.ControlField == (byte)BaseProtocol.ControlField.FromDtu
                        && message.AFN == (byte)BaseProtocol.AFN.ToDtuClosePump)
                    {
                        CmdResponseToDtuClosePump cmdreceive = new CmdResponseToDtuClosePump(message);
                        //cmdreceive.UserData = message.UserData;
                        string msgreceive = cmdreceive.ReadMsg();
                        if (d != null)
                        {
                            d.Online = 1;
                            d.LastUpdate = DateTime.Now;
                            d.Remark = message.RawDataStr;
                            d.TerminalState = "远程关泵响应";
                            updateDeviceList(d);
                            OnlineDeviceService.AddOnline(deviceNo, d);
                            DeviceEvent deviceEvent = new DeviceEvent();
                            DeviceEventModule.InitDeviceEvent(deviceEvent);
                            deviceEvent.DeviceNo = deviceNo;
                            deviceEvent.EventTime = d.LastUpdate;
                            deviceEvent.EventType = d.TerminalState;
                            deviceEvent.DeviceTime = d.LastUpdate;
                            deviceEvent.RawData = message.RawDataStr;
                            if (msgreceive != "")
                            {
                                deviceEvent.Remark = "远程关泵响应出错：" + msgreceive;
                            }
                            else
                            {
                                deviceEvent.Remark = "远程关泵返回结果：远程关泵" + (cmdreceive.Result == 1 ? "成功" : "失败");
                            }
                            saveDeviceEvent(deviceEvent);
                            proxySendDeviceList(d, deviceEvent);

                            if (ToDtuCommand.GetBaseMessageToDtuByKey(message.AddressField + "-" + message.AFN) != null)
                            {
                                ToDtuCommand.AddBaseMessageFromDtu(cmdreceive.AddressField + "-" + cmdreceive.AFN, cmdreceive);
                                //ShowLogData.add("添加发送命令响应保存！" + cmdreceive.AddressField + "-" + cmdreceive.AFN + "：" + cmdreceive.RawDataStr);
                            }
                        }

                        //proxySendCommand(command);
                    }
                    else if (message.ControlField == (byte)BaseProtocol.ControlField.FromDtu
                        && message.AFN == (byte)BaseProtocol.AFN.ToDtuSetStationCode)
                    {
                        CmdResponseToDtuSetStationCode cmdreceive = new CmdResponseToDtuSetStationCode(message);
                        //cmdreceive.UserData = message.UserData;
                        string msgreceive = cmdreceive.ReadMsg();
                        if (d != null)
                        {
                            d.Online = 1;
                            d.LastUpdate = DateTime.Now;
                            d.Remark = message.RawDataStr;
                            d.TerminalState = "设置主站射频地址响应";
                            updateDeviceList(d);
                            OnlineDeviceService.AddOnline(deviceNo, d);
                            DeviceEvent deviceEvent = new DeviceEvent();
                            DeviceEventModule.InitDeviceEvent(deviceEvent);
                            deviceEvent.DeviceNo = deviceNo;
                            deviceEvent.EventTime = d.LastUpdate;
                            deviceEvent.EventType = d.TerminalState;
                            deviceEvent.DeviceTime = d.LastUpdate;
                            deviceEvent.RawData = message.RawDataStr;
                            if (msgreceive != "")
                            {
                                deviceEvent.Remark = "设置主站射频地址响应出错：" + msgreceive;
                            }
                            else
                            {
                                deviceEvent.Remark = "设置主站射频地址返回结果：设置主站射频地址" + (cmdreceive.Result == 1 ? "成功" : "失败");
                            }
                            saveDeviceEvent(deviceEvent);
                            proxySendDeviceList(d, deviceEvent);

                            if (ToDtuCommand.GetBaseMessageToDtuByKey(message.AddressField + "-" + message.AFN) != null)
                            {
                                ToDtuCommand.AddBaseMessageFromDtu(cmdreceive.AddressField + "-" + cmdreceive.AFN, cmdreceive);
                                //ShowLogData.add("添加发送命令响应保存！" + cmdreceive.AddressField + "-" + cmdreceive.AFN + "：" + cmdreceive.RawDataStr);
                            }
                        }

                        //proxySendCommand(command);
                    }
                    else if (message.ControlField == (byte)BaseProtocol.ControlField.FromDtu
                        && message.AFN == (byte)BaseProtocol.AFN.ToDtuShieldSerialNumber)
                    {
                        CmdResponseToDtuShieldSerialNumber cmdreceive = new CmdResponseToDtuShieldSerialNumber(message);
                        //cmdreceive.UserData = message.UserData;
                        string msgreceive = cmdreceive.ReadMsg();
                        if (d != null)
                        {
                            d.Online = 1;
                            d.LastUpdate = DateTime.Now;
                            d.Remark = message.RawDataStr;
                            d.TerminalState = "屏蔽卡号响应";
                            updateDeviceList(d);
                            OnlineDeviceService.AddOnline(deviceNo, d);
                            DeviceEvent deviceEvent = new DeviceEvent();
                            DeviceEventModule.InitDeviceEvent(deviceEvent);
                            deviceEvent.DeviceNo = deviceNo;
                            deviceEvent.EventTime = d.LastUpdate;
                            deviceEvent.EventType = d.TerminalState;
                            deviceEvent.DeviceTime = d.LastUpdate;
                            deviceEvent.RawData = message.RawDataStr;
                            if (msgreceive != "")
                            {
                                deviceEvent.Remark = "屏蔽卡号响应出错：" + msgreceive;
                            }
                            else
                            {
                                deviceEvent.Remark = "屏蔽卡号返回结果：屏蔽卡号" + (cmdreceive.Result == 1 ? "成功" : "失败");
                            }
                            saveDeviceEvent(deviceEvent);
                            proxySendDeviceList(d, deviceEvent);

                            if (ToDtuCommand.GetBaseMessageToDtuByKey(message.AddressField + "-" + message.AFN) != null)
                            {
                                ToDtuCommand.AddBaseMessageFromDtu(cmdreceive.AddressField + "-" + cmdreceive.AFN, cmdreceive);
                                //ShowLogData.add("添加发送命令响应保存！" + cmdreceive.AddressField + "-" + cmdreceive.AFN + "：" + cmdreceive.RawDataStr);
                            }
                        }

                        //proxySendCommand(command);
                    }
                    else if (message.ControlField == (byte)BaseProtocol.ControlField.FromDtu
                        && message.AFN == (byte)BaseProtocol.AFN.ToDtuShieldSerialNumberCancel)
                    {
                        CmdResponseToDtuShieldSerialNumberCancel cmdreceive = new CmdResponseToDtuShieldSerialNumberCancel(message);
                        //cmdreceive.UserData = message.UserData;
                        string msgreceive = cmdreceive.ReadMsg();
                        if (d != null)
                        {
                            d.Online = 1;
                            d.LastUpdate = DateTime.Now;
                            d.Remark = message.RawDataStr;
                            d.TerminalState = "解除屏蔽卡号响应";
                            updateDeviceList(d);
                            OnlineDeviceService.AddOnline(deviceNo, d);
                            DeviceEvent deviceEvent = new DeviceEvent();
                            DeviceEventModule.InitDeviceEvent(deviceEvent);
                            deviceEvent.DeviceNo = deviceNo;
                            deviceEvent.EventTime = d.LastUpdate;
                            deviceEvent.EventType = d.TerminalState;
                            deviceEvent.DeviceTime = d.LastUpdate;
                            deviceEvent.RawData = message.RawDataStr;
                            if (msgreceive != "")
                            {
                                deviceEvent.Remark = "解除屏蔽卡号响应出错：" + msgreceive;
                            }
                            else
                            {
                                deviceEvent.Remark = "解除屏蔽卡号返回结果：解除屏蔽卡号" + (cmdreceive.Result == 1 ? "成功" : "失败");
                            }
                            saveDeviceEvent(deviceEvent);
                            proxySendDeviceList(d, deviceEvent);

                            if (ToDtuCommand.GetBaseMessageToDtuByKey(message.AddressField + "-" + message.AFN) != null)
                            {
                                ToDtuCommand.AddBaseMessageFromDtu(cmdreceive.AddressField + "-" + cmdreceive.AFN, cmdreceive);
                                //ShowLogData.add("添加发送命令响应保存！" + cmdreceive.AddressField + "-" + cmdreceive.AFN + "：" + cmdreceive.RawDataStr);
                            }
                        }

                        //proxySendCommand(command);
                    }
                    else if (message.ControlField == (byte)BaseProtocol.ControlField.FromDtu
                        && message.AFN == (byte)BaseProtocol.AFN.ToDtuSetGroundWaterParam)
                    {
                        CmdResponseToDtuSetGroundWaterParam cmdreceive = new CmdResponseToDtuSetGroundWaterParam(message);
                        //cmdreceive.UserData = message.UserData;
                        string msgreceive = cmdreceive.ReadMsg();
                        if (d != null)
                        {
                            d.Online = 1;
                            d.LastUpdate = DateTime.Now;
                            d.Remark = message.RawDataStr;
                            d.TerminalState = "水位计参数设置响应";
                            updateDeviceList(d);
                            OnlineDeviceService.AddOnline(deviceNo, d);
                            DeviceEvent deviceEvent = new DeviceEvent();
                            DeviceEventModule.InitDeviceEvent(deviceEvent);
                            deviceEvent.DeviceNo = deviceNo;
                            deviceEvent.EventTime = d.LastUpdate;
                            deviceEvent.EventType = d.TerminalState;
                            deviceEvent.DeviceTime = d.LastUpdate;
                            deviceEvent.RawData = message.RawDataStr;
                            if (msgreceive != "")
                            {
                                deviceEvent.Remark = "水位计参数设置响应出错：" + msgreceive;
                            }
                            else
                            {
                                deviceEvent.Remark = "水位计参数设置返回结果：水位计参数设置" + (cmdreceive.Result == 1 ? "成功" : "失败");
                            }
                            saveDeviceEvent(deviceEvent);
                            proxySendDeviceList(d, deviceEvent);

                            if (ToDtuCommand.GetBaseMessageToDtuByKey(message.AddressField + "-" + message.AFN) != null)
                            {
                                ToDtuCommand.AddBaseMessageFromDtu(cmdreceive.AddressField + "-" + cmdreceive.AFN, cmdreceive);
                                //ShowLogData.add("添加发送命令响应保存！" + cmdreceive.AddressField + "-" + cmdreceive.AFN + "：" + cmdreceive.RawDataStr);
                            }
                        }

                        //proxySendCommand(command);
                    }
                    else if (message.ControlField == (byte)BaseProtocol.ControlField.FromDtu
                        && message.AFN == (byte)BaseProtocol.AFN.ToDtuQueryGroundWaterParam)
                    {
                        CmdResponseToDtuQueryGroundWaterParam cmdreceive = new CmdResponseToDtuQueryGroundWaterParam(message);
                        //cmdreceive.UserData = message.UserData;
                        string msgreceive = cmdreceive.ReadMsg();
                        if (d != null)
                        {
                            d.Online = 1;
                            d.LastUpdate = DateTime.Now;
                            d.Remark = message.RawDataStr;
                            d.TerminalState = "水位计参数查询响应";
                            updateDeviceList(d);
                            OnlineDeviceService.AddOnline(deviceNo, d);
                            DeviceEvent deviceEvent = new DeviceEvent();
                            DeviceEventModule.InitDeviceEvent(deviceEvent);
                            deviceEvent.DeviceNo = deviceNo;
                            deviceEvent.EventTime = d.LastUpdate;
                            deviceEvent.EventType = d.TerminalState;
                            deviceEvent.DeviceTime = d.LastUpdate;
                            deviceEvent.RawData = message.RawDataStr;
                            if (msgreceive != "")
                            {
                                deviceEvent.Remark = "水位计参数查询响应出错：" + msgreceive;
                            }
                            else
                            {
                                deviceEvent.Remark = "水位计参数查询返回结果：量程" + cmdreceive.Range + "，投入线长" + cmdreceive.LineLength;
                            }
                            saveDeviceEvent(deviceEvent);
                            proxySendDeviceList(d, deviceEvent);

                            if (ToDtuCommand.GetBaseMessageToDtuByKey(message.AddressField + "-" + message.AFN) != null)
                            {
                                ToDtuCommand.AddBaseMessageFromDtu(cmdreceive.AddressField + "-" + cmdreceive.AFN, cmdreceive);
                                //ShowLogData.add("添加发送命令响应保存！" + cmdreceive.AddressField + "-" + cmdreceive.AFN + "：" + cmdreceive.RawDataStr);
                            }
                        }

                        //proxySendCommand(command);
                    }
                    else if (message.ControlField == (byte)BaseProtocol.ControlField.FromDtu
                        && message.AFN == (byte)BaseProtocol.AFN.ToDtuQueryGroundWater)
                    {
                        CmdResponseToDtuQueryGroundWater cmdreceive = new CmdResponseToDtuQueryGroundWater(message);
                        //cmdreceive.UserData = message.UserData;
                        string msgreceive = cmdreceive.ReadMsg();
                        if (d != null)
                        {
                            d.Online = 1;
                            d.LastUpdate = DateTime.Now;
                            d.Remark = message.RawDataStr;
                            d.TerminalState = "水位查询响应";
                            updateDeviceList(d);
                            OnlineDeviceService.AddOnline(deviceNo, d);
                            DeviceEvent deviceEvent = new DeviceEvent();
                            DeviceEventModule.InitDeviceEvent(deviceEvent);
                            deviceEvent.DeviceNo = deviceNo;
                            deviceEvent.EventTime = d.LastUpdate;
                            deviceEvent.EventType = d.TerminalState;
                            deviceEvent.DeviceTime = d.LastUpdate;
                            deviceEvent.RawData = message.RawDataStr;
                            if (msgreceive != "")
                            {
                                deviceEvent.Remark = "水位查询响应出错：" + msgreceive;
                            }
                            else
                            {
                                deviceEvent.Remark = "水位查询返回结果：观测时间" + cmdreceive.Acq_Time.ToString("yyyy-MM-dd HH:mm:ss") + "，水位计数据" + cmdreceive.GroundWaterLevel + "，投入线长" + cmdreceive.LineLength + "，水温" + cmdreceive.GroundWaterTempture;
                                try
                                {
                                    T_GroundWater tgw = new T_GroundWater();
                                    tgw.StationID = deviceNo;
                                    tgw.GroundWaterLevel = (decimal)cmdreceive.GroundWaterLevel;
                                    tgw.LineLength = (decimal)cmdreceive.LineLength;
                                    tgw.GroundWaterTempture = (decimal)cmdreceive.GroundWaterTempture;
                                    tgw.BV = (decimal)10.62;
                                    tgw.Acq_Time = cmdreceive.Acq_Time;
                                    tgw.CREATE_TIME = DateTime.Now;
                                    T_GroundWaterModule.Add(tgw);
                                }
                                catch
                                {
                                }
                            }
                            saveDeviceEvent(deviceEvent);
                            proxySendDeviceList(d, deviceEvent);

                            if (ToDtuCommand.GetBaseMessageToDtuByKey(message.AddressField + "-" + message.AFN) != null)
                            {
                                ToDtuCommand.AddBaseMessageFromDtu(cmdreceive.AddressField + "-" + cmdreceive.AFN, cmdreceive);
                                //ShowLogData.add("添加发送命令响应保存！" + cmdreceive.AddressField + "-" + cmdreceive.AFN + "：" + cmdreceive.RawDataStr);
                            }
                        }

                        //proxySendCommand(command);
                    }
                    else if (message.ControlField == (byte)BaseProtocol.ControlField.FromDtu
                        && message.AFN == (byte)BaseProtocol.AFN.FromDtuGroundWater)
                    {
                        CmdFromDtuGroundWater cmdreceive = new CmdFromDtuGroundWater(message);
                        //cmdreceive.UserData = message.UserData;
                        string msgreceive = cmdreceive.ReadMsg();

                        CmdResponseFromDtuGroundWater cmdres = new CmdResponseFromDtuGroundWater();
                        cmdres.AddressField = message.AddressField;
                        if (d == null)
                        {
                            cmdres.Result = 0;
                        }
                        else
                        {
                            d.Online = 1;
                            d.LastUpdate = DateTime.Now;
                            d.Remark = message.RawDataStr;
                            d.TerminalState = "水位上报";
                            updateDeviceList(d);
                            OnlineDeviceService.AddOnline(deviceNo, d);
                            DeviceEvent deviceEvent = new DeviceEvent();
                            DeviceEventModule.InitDeviceEvent(deviceEvent);
                            deviceEvent.DeviceNo = deviceNo;
                            deviceEvent.EventTime = d.LastUpdate;
                            deviceEvent.EventType = d.TerminalState;
                            deviceEvent.DeviceTime = d.LastUpdate;
                            deviceEvent.RawData = message.RawDataStr;
                            if (msgreceive != "")
                            {
                                cmdres.Result = 0;
                                deviceEvent.Remark = "水位上报出错：" + msgreceive;
                            }
                            else
                            {
                                cmdres.Result = 1;
                                deviceEvent.Remark = "水位上报结果：观测时间" + cmdreceive.Acq_Time.ToString("yyyy-MM-dd HH:mm:ss") + "，水位计数据" + cmdreceive.GroundWaterLevel + "，投入线长" + cmdreceive.LineLength + "，水温" + cmdreceive.GroundWaterTempture;
                                try
                                {
                                    T_GroundWater tgw = new T_GroundWater();
                                    tgw.StationID = deviceNo;
                                    tgw.GroundWaterLevel = (decimal)cmdreceive.GroundWaterLevel;
                                    tgw.LineLength = (decimal)cmdreceive.LineLength;
                                    tgw.GroundWaterTempture = (decimal)cmdreceive.GroundWaterTempture;
                                    tgw.BV = (decimal)10.61;
                                    tgw.Acq_Time = cmdreceive.Acq_Time;
                                    tgw.CREATE_TIME = DateTime.Now;
                                    T_GroundWaterModule.Add(tgw);
                                }
                                catch
                                {
                                }
                            }
                            saveDeviceEvent(deviceEvent);
                            proxySendDeviceList(d, deviceEvent);
                        }
                        cmdres.DateTimeNew = DateTime.Now;
                        cmdres.RawDataChar = cmdres.WriteMsg();
                        cmdres.RawDataStr = HexStringUtility.ByteArrayToHexString(cmdres.RawDataChar);
                        send(cmdres.RawDataChar, 0, cmdres.RawDataChar.Length);
                    }
                    else if (message.ControlField == (byte)BaseProtocol.ControlField.FromDtu
                        && message.AFN == (byte)BaseProtocol.AFN.ToDtuQueryState)
                    {
                        CmdResponseToDtuQueryState cmdreceive = new CmdResponseToDtuQueryState(message);
                        //cmdreceive.UserData = message.UserData;
                        string msgreceive = cmdreceive.ReadMsg();
                        if (d != null)
                        {
                            d.Online = 1;
                            d.LastUpdate = DateTime.Now;
                            d.Remark = message.RawDataStr;
                            d.TerminalState = "状态自报";
                            if (cmdreceive.WaterUsed > 0)
                                d.WaterUsed = cmdreceive.WaterUsed;
                            updateDeviceList(d);
                            OnlineDeviceService.AddOnline(deviceNo, d);
                            DeviceEvent deviceEvent = new DeviceEvent();
                            DeviceEventModule.InitDeviceEvent(deviceEvent);
                            deviceEvent.DeviceNo = deviceNo;
                            deviceEvent.EventTime = d.LastUpdate;
                            deviceEvent.EventType = d.TerminalState;
                            deviceEvent.DeviceState = cmdreceive.State == null ? "0".PadLeft(32, '0') : cmdreceive.State.PadLeft(32, '0');
                            deviceEvent.YearWaterUsed = cmdreceive.WaterUsed;
                            //deviceEvent.YearElectricUsed = cmdreceive.ElectricUsed;
                            deviceEvent.RawData = message.RawDataStr;
                            if (msgreceive != "")
                            {
                                deviceEvent.DeviceTime = d.LastUpdate;
                                deviceEvent.Remark = "状态查询响应出错：" + msgreceive;
                            }
                            else
                            {
                                deviceEvent.DeviceTime = cmdreceive.DateTimeNew;
                                deviceEvent.Remark = "状态查询状态码：" + cmdreceive.State +
                                    "，上报时间：" + cmdreceive.DateTimeNew.ToString("yyyy-MM-dd HH:mm:ss") +
                                    //"，累计用电量：" + cmdreceive.ElectricUsed + 
                                    "，累计用水量：" + cmdreceive.WaterUsed;
                            }
                            AlarmService.DeviceAlarmAnalyse(deviceEvent);
                            saveDeviceEvent(deviceEvent);
                            proxySendDeviceList(d, deviceEvent);

                            if (ToDtuCommand.GetBaseMessageToDtuByKey(message.AddressField + "-" + message.AFN) != null)
                            {
                                ToDtuCommand.AddBaseMessageFromDtu(cmdreceive.AddressField + "-" + cmdreceive.AFN, cmdreceive);
                                //ShowLogData.add("添加发送命令响应保存！" + cmdreceive.AddressField + "-" + cmdreceive.AFN + "：" + cmdreceive.RawDataStr);
                            }
                        }

                        //proxySendCommand(command);
                    }
                    else if (message.ControlField == (byte)BaseProtocol.ControlField.FromDtu
                        && message.AFN == (byte)BaseProtocol.AFN.FromDtuUploadFile)
                    {
                        CmdFromDtuUploadFile cmdreceive = new CmdFromDtuUploadFile(message);
                        //cmdreceive.UserData = message.UserData;
                        string msgreceive = cmdreceive.ReadMsg();

                        CmdResponseFromDtuUploadFile cmdres = new CmdResponseFromDtuUploadFile();
                        cmdres.AddressField = message.AddressField;
                        cmdres.Sum = cmdreceive.Sum;
                        cmdres.Curr = cmdreceive.Curr;
                        if (d == null)
                        {
                            cmdres.Result = 0;
                        }
                        else
                        {
                            d.Online = 1;
                            d.LastUpdate = DateTime.Now;
                            d.Remark = message.RawDataStr;
                            d.TerminalState = "终端上传文件";
                            updateDeviceList(d);
                            OnlineDeviceService.AddOnline(deviceNo, d);
                            DeviceEvent deviceEvent = new DeviceEvent();
                            DeviceEventModule.InitDeviceEvent(deviceEvent);
                            deviceEvent.DeviceNo = deviceNo;
                            deviceEvent.EventTime = d.LastUpdate;
                            deviceEvent.EventType = d.TerminalState;
                
                            deviceEvent.RawData = message.RawDataStr;
                            if (msgreceive != "" || cmdreceive.Sum == 0)
                            {
                                cmdres.Result = 0;
                                deviceEvent.DeviceTime = d.LastUpdate;
                                deviceEvent.Remark = "终端上传文件出错：" + msgreceive;
                            }
                            else
                            {
                                cmdres.Result = 1;
                                deviceEvent.DeviceTime = d.LastUpdate;
                                deviceEvent.Remark = "终端上传文件时间：" + d.LastUpdate.ToString("yyyy-MM-dd HH:mm:ss") + "，总包数：" + cmdreceive.Sum + "，当前包数" + cmdreceive.Curr;

                                object[] fo = FilePathCache.GetFileCacheFromDTU(message.AddressField + "-" + message.AFN);
                                if (fo == null || cmdreceive.Curr == 1)
                                {
                                    fo = new object[cmdreceive.Sum];
                                }
                                fo[cmdreceive.Curr - 1] = cmdreceive.Content;
                                FilePathCache.AddFileCacheFromDTU(message.AddressField + "-" + message.AFN, fo);

                                if (cmdreceive.Sum == cmdreceive.Curr)
                                {
                                    List<byte> list = new List<byte>();
                                    for (short i = 0; i < fo.Length; i++)
                                    {
                                        byte[] bss = (byte[])fo[i];
                                        list.AddRange(bss);
                                    }

                                    string pathRoot = Environment.CurrentDirectory;
                                    if (FilePathCache.Path != "")
                                    {
                                        pathRoot = FilePathCache.Path;
                                    }
                                    string path = pathRoot + "/UploadFiles/Device/" + DeviceModule.DeviceNo_Hex2Normal(message.AddressField) + "/";
                                    DirectoryInfo dir = new DirectoryInfo(path);
                                    if (!dir.Exists)
                                        dir.Create();

                                    string FileName = path + DateTime.Now.ToString("yyyyMMddHHmmss");
                                    if (list[0] == 0x01)
                                    {
                                        FileName += ".jpg";
                                    }
                                    else if (list[0] == 0x02)
                                    {
                                        FileName += ".bin";
                                    }

                                    list.RemoveAt(0);
                                    byte[] bs = list.ToArray();
                                    using (FileStream fs = new FileStream(FileName, FileMode.Create))
                                    {
                                        fs.Write(bs, 0, bs.Length);
                                    }

                                    fo = null;
                                    FilePathCache.RemoveFileCacheFromDTU(message.AddressField + "-" + message.AFN);
                                }
                            }
                            saveDeviceEvent(deviceEvent);
                            
                            cmdres.StationType = (byte)d.StationType;
                            cmdres.StationCode = d.StationType == 2 ? d.StationCode : 0;

                            proxySendDeviceList(d, deviceEvent);
                        }
                        cmdres.RawDataChar = cmdres.WriteMsg();
                        cmdres.RawDataStr = HexStringUtility.ByteArrayToHexString(cmdres.RawDataChar);
                        if (cmdreceive.Sum == cmdreceive.Curr || cmdres.Result == 0)
                        {
                            send(cmdres.RawDataChar, 0, cmdres.RawDataChar.Length);
                        }
                    }
                    else if (message.ControlField == (byte)BaseProtocol.ControlField.FromDtu
                        && message.AFN == (byte)BaseProtocol.AFN.ToDtuSendFile)
                    {
                        CmdResponseToDtuSendFile cmdreceive = new CmdResponseToDtuSendFile(message);
                        //cmdreceive.UserData = message.UserData;
                        string msgreceive = cmdreceive.ReadMsg();
                        if (d != null)
                        {
                            d.Online = 1;
                            d.LastUpdate = DateTime.Now;
                            d.Remark = message.RawDataStr;
                            d.TerminalState = "中心下发文件响应";
                            updateDeviceList(d);
                            OnlineDeviceService.AddOnline(deviceNo, d);
                            DeviceEvent deviceEvent = new DeviceEvent();
                            DeviceEventModule.InitDeviceEvent(deviceEvent);
                            deviceEvent.DeviceNo = deviceNo;
                            deviceEvent.EventTime = d.LastUpdate;
                            deviceEvent.EventType = d.TerminalState;
                            deviceEvent.RawData = message.RawDataStr;
                            if (msgreceive != "")
                            {
                                deviceEvent.DeviceTime = d.LastUpdate;
                                deviceEvent.Remark = "中心下发文件响应出错：" + msgreceive;
                            }
                            else
                            {
                                deviceEvent.DeviceTime = d.LastUpdate;
                                deviceEvent.Remark = "中心下发文件响应：" + cmdreceive.Result +
                                    "，时间：" + d.LastUpdate.ToString("yyyy-MM-dd HH:mm:ss") + "，总包数：" + cmdreceive.Sum + "，当前包数" + cmdreceive.Curr;
                            }
                            AlarmService.DeviceAlarmAnalyse(deviceEvent);
                            saveDeviceEvent(deviceEvent);
                            proxySendDeviceList(d, deviceEvent);

                            if (ToDtuCommand.GetBaseMessageToDtuByKey(message.AddressField + "-" + message.AFN) != null)
                            {
                                ToDtuCommand.AddBaseMessageFromDtu(cmdreceive.AddressField + "-" + cmdreceive.AFN, cmdreceive);
                                //ShowLogData.add("添加发送命令响应保存！" + cmdreceive.AddressField + "-" + cmdreceive.AFN + "：" + cmdreceive.RawDataStr);
                            }
                        }
                    }
                    else
                    {
                        if (userToken.DeviceList != null)
                        {
                            userToken.DeviceList.Online = 1;
                            userToken.DeviceList.LastUpdate = DateTime.Now;
                            userToken.DeviceList.Remark = message.RawDataStr;
                            userToken.DeviceList.TerminalState = "心跳";
                            updateDeviceList(userToken.DeviceList);
                            OnlineDeviceService.AddOnline(deviceNo, userToken.DeviceList);
                            DeviceEvent deviceEvent = new DeviceEvent();
                            deviceEvent.DeviceNo = deviceNo;
                            deviceEvent.EventTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.EventType = userToken.DeviceList.TerminalState;
                            deviceEvent.RawData = message.RawDataStr;
                            proxySendDeviceList(userToken.DeviceList, deviceEvent);
                        }
                        byte[] data = HexStringUtility.StrToByteArray("unknownAFN");
                        send(data, 0, data.Length);
                        LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：unknownAFN 未知功能码");
                        ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]响应：unknownAFN 未知功能码");
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]出错：" + ex.Message);
                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]出错：" + ex.Message);

                if (userToken.DeviceList != null)
                {
                    userToken.DeviceList.Online = 1;
                    userToken.DeviceList.LastUpdate = DateTime.Now;
                    userToken.DeviceList.TerminalState = "未知";
                    updateDeviceList(userToken.DeviceList);

                    string FullDeviceNo = DeviceModule.GetFullDeviceNoByID(userToken.DeviceList.Id);
                    OnlineDeviceService.AddOnline(FullDeviceNo, userToken.DeviceList);
                    DeviceEvent deviceEvent = new DeviceEvent();
                    DeviceEventModule.InitDeviceEvent(deviceEvent);
                    deviceEvent.DeviceNo = FullDeviceNo;
                    deviceEvent.EventTime = userToken.DeviceList.LastUpdate;
                    deviceEvent.EventType = userToken.DeviceList.TerminalState;
                    deviceEvent.DeviceTime = userToken.DeviceList.LastUpdate;
                    deviceEvent.Remark = "错误数据";
                    deviceEvent.RawData = message.RawDataStr;
                    saveDeviceEvent(deviceEvent);
                    proxySendDeviceList(userToken.DeviceList, deviceEvent);
                }
            }
            #endregion

            return true;
        }

        //水文监测数据通信规约解析
        public override bool processCommandWater(byte[] buffer)
        {
            int length = buffer.Length;
            //string receive = HexStringUtility.ByteArrayToHexString(buffer);
            string receive = FormatHelper.ByteArrayToHexString(buffer);
            LogHelper.Info("收到的数据长度为：" + buffer.Length + "，内容为：" + receive);
            ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":" + receive);

            #region 水文监测数据通信规约
            WaterBaseMessage message_water = new WaterBaseMessage();
            message_water.RawDataChar = new byte[length];
            try
            {
                Array.Copy(buffer, message_water.RawDataChar, length);//message.RawDataChar = buffer;
                message_water.RawDataStr = HexStringUtility.ByteArrayToHexString(message_water.RawDataChar);

                string msg = message_water.ReadMsgBase();
                if (msg == "")
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

                    if (userToken.DeviceInfo == null)
                    {
                        Device d = DeviceModule.GetDeviceByRemoteStation(RemoteStation);
                        if (d == null)
                        {
                            ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：无此编号水文监测设备：" + RemoteStation);
                            LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：无此编号水文监测设备：" + RemoteStation);
                            return true;
                        }
                        d.Online = 1;
                        d.OnlineTime = DateTime.Now;
                        d.LastUpdate = DateTime.Now;
                        d.Remark = message_water.RawDataStr;
                        userToken.DeviceList = d;
                        DeviceModule.UpdateDeviceInfo(d);

                        string FullDeviceNo = DeviceModule.GetFullDeviceNoByID(userToken.DeviceList.Id);
                        OnlineDeviceService.AddOnline(FullDeviceNo, userToken.DeviceList);
                        userToken.DeviceInfo = new DeviceInfo(DeviceModule.DeviceNo_Normal2Hex(FullDeviceNo));
                    }
                    else
                    {
                        if (userToken.DeviceList.RemoteStation != RemoteStation)
                        {
                            ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：无此编号水文监测设备：" + RemoteStation + "," + userToken.DeviceList.RemoteStation + Environment.NewLine + message_water.RawDataStr);
                            LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：无此编号水文监测设备：" + RemoteStation + "," + userToken.DeviceList.RemoteStation + Environment.NewLine + message_water.RawDataStr);
                            //userToken.DeviceList.RemoteStation = RemoteStation;
                            return true;
                        }
                    }

                    List<AsyncSocketUserToken> list = new List<AsyncSocketUserToken>();
                    foreach (AsyncSocketUserToken tokenTmp in server.AsyncSocketUserTokenList.UserTokenList)
                    {
                        if (tokenTmp.DeviceInfo != null && userToken.DeviceInfo != null)
                        {
                            string d1 = tokenTmp.DeviceInfo.SerialString;
                            string d2 = userToken.DeviceInfo.SerialString;
                            string c1 = tokenTmp.ConnectedSocket.RemoteEndPoint.ToString();
                            string c2 = userToken.ConnectedSocket.RemoteEndPoint.ToString();
                            if (d1 == d2 && c1 != c2)
                            {
                                list.Add(tokenTmp);
                            }
                        }
                    }

                    foreach (AsyncSocketUserToken tokenTmp in list)
                    {
                        string info1 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + tokenTmp.ConnectedSocket.RemoteEndPoint.ToString() + "]：连接关闭";
                        //ShowLogData.add(info1);
                        LogHelper.Info(info1);
                        server.closeClientSocket(tokenTmp, false);
                    }

                    WaterBaseMessage[] MsgList = WaterDeviceService.Get(cKey);
                    if (TotalPackage > 0)
                    {
                        if (MsgList == null)
                            MsgList = new WaterBaseMessage[TotalPackage];
                        MsgList[CurrentPackage - 1] = message_water;
                    }
                    else
                    {
                        MsgList = new WaterBaseMessage[1];
                        MsgList[0] = message_water;
                    }
                    WaterDeviceService.Add(cKey, MsgList);

                    string waterInfo = "水文监测数据：";
                    if (AFN == (byte)WaterBaseProtocol.AFN._2F && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Up)
                    {
                        #region _2F
                        waterInfo += EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                        ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);
                        LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);

                        WaterCmd_2F_1 cmdreceive = null;
                        if (DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETX
                            || DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB && DataBeginChar == (byte)WaterBaseProtocol.DataBeginChar.STX)
                        {
                            cmdreceive = new WaterCmd_2F_1();
                            cmdreceive.MsgList = MsgList;
                            string msgreceive = cmdreceive.ReadMsg();
                            if (msgreceive == "")
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                            }

                            MsgList = null;
                            WaterDeviceService.Remove(cKey);
                        }

                        if (userToken.DeviceList != null)
                        {
                            userToken.DeviceList.Online = 1;
                            userToken.DeviceList.LastUpdate = DateTime.Now;
                            userToken.DeviceList.TerminalState = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                            updateDeviceList(userToken.DeviceList);

                            string FullDeviceNo = DeviceModule.GetFullDeviceNoByID(userToken.DeviceList.Id);
                            OnlineDeviceService.AddOnline(FullDeviceNo, userToken.DeviceList);
                            DeviceEvent deviceEvent = new DeviceEvent();
                            DeviceEventModule.InitDeviceEvent(deviceEvent);
                            deviceEvent.DeviceNo = FullDeviceNo;
                            deviceEvent.EventTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.EventType = userToken.DeviceList.TerminalState;
                            deviceEvent.DeviceTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.Remark = cmdreceive == null ? (userToken.DeviceList.TerminalState + " " + TotalPackage + " " + CurrentPackage) : cmdreceive.ToString();
                            deviceEvent.RawData = message_water.RawDataStr;
                            saveDeviceEvent(deviceEvent);
                            proxySendDeviceList(userToken.DeviceList, deviceEvent);

                        }

                        #endregion
                    }
                    else if (AFN == (byte)WaterBaseProtocol.AFN._30 && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Up)
                    {
                        #region _30
                        waterInfo += EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                        ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);
                        LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);

                        WaterCmd_30_1 cmdreceive = null;
                        if (DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETX
                            || DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB && DataBeginChar == (byte)WaterBaseProtocol.DataBeginChar.STX)
                        {
                            cmdreceive = new WaterCmd_30_1();
                            cmdreceive.MsgList = MsgList;
                            string msgreceive = cmdreceive.ReadMsg();
                            if (msgreceive == "")
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                            }

                            WaterCmd_30_2 cmdres = new WaterCmd_30_2();
                            cmdres.CenterStation = message_water.CenterStation;
                            cmdres.RemoteStation = message_water.RemoteStation;
                            cmdres.TotalPackage = message_water.TotalPackage;
                            cmdres.CurrentPackage = message_water.CurrentPackage;
                            cmdres.PW = message_water.PW;
                            if (message_water.DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB)
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.ACK;
                            else
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.EOT;
                            cmdres.SerialNumber = cmdreceive.SerialNumber;
                            cmdres.SendTime = DateTime.Now;

                            string msgres = cmdres.WriteMsg();
                            if (msgres == "")
                            {
                                send(cmdres.RawDataChar, 0, cmdres.RawDataChar.Length);
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                            }

                            MsgList = null;
                            WaterDeviceService.Remove(cKey);
                        }

                        if (userToken.DeviceList != null)
                        {
                            userToken.DeviceList.Online = 1;
                            userToken.DeviceList.LastUpdate = DateTime.Now;
                            userToken.DeviceList.TerminalState = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                            updateDeviceList(userToken.DeviceList);

                            string FullDeviceNo = DeviceModule.GetFullDeviceNoByID(userToken.DeviceList.Id);
                            OnlineDeviceService.AddOnline(FullDeviceNo, userToken.DeviceList);
                            DeviceEvent deviceEvent = new DeviceEvent();
                            DeviceEventModule.InitDeviceEvent(deviceEvent);
                            deviceEvent.DeviceNo = FullDeviceNo;
                            deviceEvent.EventTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.EventType = userToken.DeviceList.TerminalState;
                            deviceEvent.DeviceTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.Remark = cmdreceive == null ? (userToken.DeviceList.TerminalState + " " + TotalPackage + " " + CurrentPackage) : cmdreceive.ToString();
                            deviceEvent.RawData = message_water.RawDataStr;
                            saveDeviceEvent(deviceEvent);
                            proxySendDeviceList(userToken.DeviceList, deviceEvent);

                        }

                        #endregion
                    }
                    else if (AFN == (byte)WaterBaseProtocol.AFN._31 && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Up)
                    {
                        #region _31
                        waterInfo += EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                        ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);
                        LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);

                        WaterCmd_31_1 cmdreceive = null;
                        if (DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETX
                            || DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB && DataBeginChar == (byte)WaterBaseProtocol.DataBeginChar.STX)
                        {
                            cmdreceive = new WaterCmd_31_1();
                            cmdreceive.MsgList = MsgList;
                            string msgreceive = cmdreceive.ReadMsg();
                            if (msgreceive == "")
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                            }

                            WaterCmd_31_2 cmdres = new WaterCmd_31_2();
                            cmdres.CenterStation = message_water.CenterStation;
                            cmdres.RemoteStation = message_water.RemoteStation;
                            cmdres.TotalPackage = message_water.TotalPackage;
                            cmdres.CurrentPackage = message_water.CurrentPackage;
                            cmdres.PW = message_water.PW;
                            if (message_water.DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB)
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.ACK;
                            else
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.EOT;
                            cmdres.SerialNumber = cmdreceive.SerialNumber;
                            cmdres.SendTime = DateTime.Now;

                            string msgres = cmdres.WriteMsg();
                            if (msgres == "")
                            {
                                send(cmdres.RawDataChar, 0, cmdres.RawDataChar.Length);
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                            }

                            MsgList = null;
                            WaterDeviceService.Remove(cKey);
                        }

                        if (userToken.DeviceList != null)
                        {
                            userToken.DeviceList.Online = 1;
                            userToken.DeviceList.LastUpdate = DateTime.Now;
                            userToken.DeviceList.TerminalState = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                            updateDeviceList(userToken.DeviceList);

                            string FullDeviceNo = DeviceModule.GetFullDeviceNoByID(userToken.DeviceList.Id);
                            OnlineDeviceService.AddOnline(FullDeviceNo, userToken.DeviceList);
                            DeviceEvent deviceEvent = new DeviceEvent();
                            DeviceEventModule.InitDeviceEvent(deviceEvent);
                            deviceEvent.DeviceNo = FullDeviceNo;
                            deviceEvent.EventTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.EventType = userToken.DeviceList.TerminalState;
                            deviceEvent.DeviceTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.Remark = cmdreceive == null ? (userToken.DeviceList.TerminalState + " " + TotalPackage + " " + CurrentPackage) : cmdreceive.ToString();
                            deviceEvent.RawData = message_water.RawDataStr;
                            saveDeviceEvent(deviceEvent);
                            proxySendDeviceList(userToken.DeviceList, deviceEvent);

                        }

                        #endregion
                    }
                    else if (AFN == (byte)WaterBaseProtocol.AFN._32 && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Up)
                    {
                        #region _32
                        waterInfo += EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                        ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);
                        LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);

                        WaterCmd_32_1 cmdreceive = null;
                        if (DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETX
                            || DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB && DataBeginChar == (byte)WaterBaseProtocol.DataBeginChar.STX)
                        {
                            cmdreceive = new WaterCmd_32_1();
                            cmdreceive.MsgList = MsgList;
                            string msgreceive = cmdreceive.ReadMsg();
                            if (msgreceive == "")
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());

                                decimal?[] WTDay = null;
                                decimal? WT = null;
                                decimal?[] DepthDay = null;
                                decimal? Depth = null;
                                decimal? Voltage = 0;
                                DateTime? ObsTime = null;
                                decimal? Precipitation = null;
                                decimal? Precipitation_Hour = null;
                                decimal? Precipitation_Day = null;
                                decimal? CumulativePrecipitation = null;
                                decimal? WaterLevel = null;

                                foreach (Identifier iden in cmdreceive.List_Identifier)
                                {
                                    if (iden.GetKey() == 0xFF)
                                    {
                                        if (((Identifier_FF)iden).GetKeySub() == 0x03)
                                        {
                                            WTDay = ((Identifier_FF_03)iden).WTDay;
                                        }
                                        else if (((Identifier_FF)iden).GetKeySub() == 0x0E)
                                        {
                                            DepthDay = ((Identifier_FF_0E)iden).DepthDay;
                                        }
                                    }
                                    else if (iden.GetKey() == 0x03)
                                    {
                                        WT = ((Identifier_03)iden).WT;
                                    }
                                    else if (iden.GetKey() == 0x0E)
                                    {
                                        Depth = ((Identifier_0E)iden).Depth;
                                    }
                                    else if (iden.GetKey() == 0x1A)
                                    {
                                        Precipitation_Hour = ((Identifier_1A)iden).Precipitation;
                                    }
                                    else if (iden.GetKey() == 0x1F)
                                    {
                                        Precipitation_Day = ((Identifier_1F)iden).Precipitation;
                                    }
                                    else if (iden.GetKey() == 0x20)
                                    {
                                        Precipitation = ((Identifier_20)iden).Precipitation;
                                    }
                                    else if (iden.GetKey() == 0x26)
                                    {
                                        CumulativePrecipitation = ((Identifier_26)iden).CumulativePrecipitation;
                                    }
                                    else if (iden.GetKey() == 0x39)
                                    {
                                        WaterLevel = ((Identifier_39)iden).WaterLevel;
                                    }
                                    else if (iden.GetKey() == 0x38)
                                    {
                                        Voltage = ((Identifier_38)iden).Voltage;
                                    }
                                    else if (iden.GetKey() == 0xF0)
                                    {
                                        ObsTime = ((Identifier_F0)iden).ObsTime;
                                    }
                                }

                                if (DepthDay != null && WTDay != null && ObsTime.HasValue)
                                {
                                    for (int i = 0; i < 6; i++)
                                    {
                                        try
                                        {
                                            if (DepthDay[i].HasValue && WTDay[i].HasValue)
                                            {
                                                T_GroundWater tgw = new T_GroundWater();
                                                tgw.StationID = message_water.RemoteStation;
                                                tgw.GroundWaterLevel = DepthDay[i].Value;
                                                tgw.LineLength = 0;
                                                tgw.GroundWaterTempture = WTDay[i].Value;
                                                tgw.BV = Voltage.HasValue ? Voltage.Value : 0;
                                                tgw.Acq_Time = ObsTime.Value.AddHours(4 * (i - 5));
                                                tgw.CREATE_TIME = DateTime.Now;
                                                T_GroundWaterModule.Add(tgw);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]数据保存失败：" + ex.Message);
                                            LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]数据保存失败：" + ex.Message);
                                        }
                                    }
                                }

                                if (Precipitation.HasValue && ObsTime.HasValue)
                                {
                                    try
                                    {
                                        T_RainFall trf = new T_RainFall();
                                        trf.StationID = message_water.RemoteStation;
                                        trf.Rainfall = Precipitation.Value;
                                        trf.Rainfall_Day = Precipitation_Day.HasValue ? Precipitation_Day.Value : 0;
                                        trf.Rainfall_Hour = Precipitation_Hour.HasValue ? Precipitation_Hour.Value : 0;
                                        trf.Rainfall_Total = CumulativePrecipitation.HasValue ? CumulativePrecipitation.Value : 0;
                                        trf.WaterLevel = WaterLevel.HasValue ? WaterLevel.Value : 0;
                                        trf.BV = Voltage.HasValue ? Voltage.Value : 0;
                                        trf.Acq_Time = ObsTime.Value;
                                        trf.CREATE_TIME = DateTime.Now;
                                        //T_RainFallModule.Add(trf);

                                        try
                                        {
                                            if (userToken.DeviceList != null)
                                            {
                                                userToken.DeviceList.Rainfall = trf.Rainfall;
                                                userToken.DeviceList.Rainfall_Day = trf.Rainfall_Day;
                                                userToken.DeviceList.Rainfall_Hour = trf.Rainfall_Hour;
                                                userToken.DeviceList.Rainfall_Total = trf.Rainfall_Total;
                                                userToken.DeviceList.WaterLevel = trf.WaterLevel;
                                                userToken.DeviceList.Acq_Time = trf.Acq_Time;
                                            }
                                        }
                                        catch { }
                                    }
                                    catch (Exception ex)
                                    {
                                        ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]数据保存失败：" + ex.Message);
                                        LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]数据保存失败：" + ex.Message);
                                    }
                                }
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                            }

                            WaterCmd_32_2 cmdres = new WaterCmd_32_2();
                            cmdres.CenterStation = message_water.CenterStation;
                            cmdres.RemoteStation = message_water.RemoteStation;
                            cmdres.TotalPackage = message_water.TotalPackage;
                            cmdres.CurrentPackage = message_water.CurrentPackage;
                            cmdres.PW = message_water.PW;
                            if (message_water.DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB)
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.ACK;
                            else
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.EOT;
                            cmdres.SerialNumber = cmdreceive.SerialNumber;
                            cmdres.SendTime = DateTime.Now;

                            string msgres = cmdres.WriteMsg();
                            if (msgres == "")
                            {
                                send(cmdres.RawDataChar, 0, cmdres.RawDataChar.Length);
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                            }

                            MsgList = null;
                            WaterDeviceService.Remove(cKey);
                        }

                        if (userToken.DeviceList != null)
                        {
                            userToken.DeviceList.Online = 1;
                            userToken.DeviceList.LastUpdate = DateTime.Now;
                            userToken.DeviceList.TerminalState = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                            updateDeviceList(userToken.DeviceList);

                            string FullDeviceNo = DeviceModule.GetFullDeviceNoByID(userToken.DeviceList.Id);
                            OnlineDeviceService.AddOnline(FullDeviceNo, userToken.DeviceList);
                            DeviceEvent deviceEvent = new DeviceEvent();
                            DeviceEventModule.InitDeviceEvent(deviceEvent);
                            deviceEvent.DeviceNo = FullDeviceNo;
                            deviceEvent.EventTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.EventType = userToken.DeviceList.TerminalState;
                            deviceEvent.DeviceTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.Remark = cmdreceive == null ? (userToken.DeviceList.TerminalState + " " + TotalPackage + " " + CurrentPackage) : cmdreceive.ToString();
                            deviceEvent.RawData = message_water.RawDataStr;
                            saveDeviceEvent(deviceEvent);
                            proxySendDeviceList(userToken.DeviceList, deviceEvent);

                        }

                        #endregion
                    }
                    else if (AFN == (byte)WaterBaseProtocol.AFN._33 && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Up)
                    {
                        #region _33
                        waterInfo += EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                        ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);
                        LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);

                        WaterCmd_33_1 cmdreceive = null;
                        if (DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETX
                            || DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB && DataBeginChar == (byte)WaterBaseProtocol.DataBeginChar.STX)
                        {
                            cmdreceive = new WaterCmd_33_1();
                            cmdreceive.MsgList = MsgList;
                            string msgreceive = cmdreceive.ReadMsg();
                            if (msgreceive == "")
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());

                                decimal? Voltage = 0;
                                DateTime? ObsTime = null;
                                decimal? Precipitation = null;
                                decimal? Precipitation_Hour = null;
                                decimal? Precipitation_Day = null;
                                decimal? CumulativePrecipitation = null;
                                decimal? WaterLevel = null;

                                foreach (Identifier iden in cmdreceive.List_Identifier)
                                {
                                    if (iden.GetKey() == 0x1A)
                                    {
                                        Precipitation_Hour = ((Identifier_1A)iden).Precipitation;
                                    }
                                    else if (iden.GetKey() == 0x1F)
                                    {
                                        Precipitation_Day = ((Identifier_1F)iden).Precipitation;
                                    }
                                    else if (iden.GetKey() == 0x20)
                                    {
                                        Precipitation = ((Identifier_20)iden).Precipitation;
                                    }
                                    else if (iden.GetKey() == 0x26)
                                    {
                                        CumulativePrecipitation = ((Identifier_26)iden).CumulativePrecipitation;
                                    }
                                    else if (iden.GetKey() == 0x39)
                                    {
                                        WaterLevel = ((Identifier_39)iden).WaterLevel;
                                    }
                                    else if (iden.GetKey() == 0x38)
                                    {
                                        Voltage = ((Identifier_38)iden).Voltage;
                                    }
                                    else if (iden.GetKey() == 0xF0)
                                    {
                                        ObsTime = ((Identifier_F0)iden).ObsTime;
                                    }
                                }

                                if (Precipitation.HasValue && ObsTime.HasValue)
                                {
                                    try
                                    {
                                        T_RainFall trf = new T_RainFall();
                                        trf.StationID = message_water.RemoteStation;
                                        trf.Rainfall = Precipitation.Value;
                                        trf.Rainfall_Day = Precipitation_Day.HasValue ? Precipitation_Day.Value : 0;
                                        trf.Rainfall_Hour = Precipitation_Hour.HasValue ? Precipitation_Hour.Value : 0;
                                        trf.Rainfall_Total = CumulativePrecipitation.HasValue ? CumulativePrecipitation.Value : 0;
                                        trf.WaterLevel = WaterLevel.HasValue ? WaterLevel.Value : 0;
                                        trf.BV = Voltage.HasValue ? Voltage.Value : 0;
                                        trf.Acq_Time = ObsTime.Value;
                                        trf.CREATE_TIME = DateTime.Now;
                                        //T_RainFallModule.Add(trf);

                                        try
                                        {
                                            if (userToken.DeviceList != null)
                                            {
                                                userToken.DeviceList.Rainfall = trf.Rainfall;
                                                userToken.DeviceList.Rainfall_Day = trf.Rainfall_Day;
                                                userToken.DeviceList.Rainfall_Hour = trf.Rainfall_Hour;
                                                userToken.DeviceList.Rainfall_Total = trf.Rainfall_Total;
                                                userToken.DeviceList.WaterLevel = trf.WaterLevel;
                                                userToken.DeviceList.Acq_Time = trf.Acq_Time;
                                            }
                                        }
                                        catch { }
                                    }
                                    catch (Exception ex)
                                    {
                                        ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]数据保存失败：" + ex.Message);
                                        LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]数据保存失败：" + ex.Message);
                                    }
                                }
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                            }

                            WaterCmd_33_2 cmdres = new WaterCmd_33_2();
                            cmdres.CenterStation = message_water.CenterStation;
                            cmdres.RemoteStation = message_water.RemoteStation;
                            cmdres.TotalPackage = message_water.TotalPackage;
                            cmdres.CurrentPackage = message_water.CurrentPackage;
                            cmdres.PW = message_water.PW;
                            if (message_water.DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB)
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.ACK;
                            else
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.EOT;
                            cmdres.SerialNumber = cmdreceive.SerialNumber;
                            cmdres.SendTime = DateTime.Now;

                            string msgres = cmdres.WriteMsg();
                            if (msgres == "")
                            {
                                send(cmdres.RawDataChar, 0, cmdres.RawDataChar.Length);
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                            }

                            MsgList = null;
                            WaterDeviceService.Remove(cKey);
                        }

                        if (userToken.DeviceList != null)
                        {
                            userToken.DeviceList.Online = 1;
                            userToken.DeviceList.LastUpdate = DateTime.Now;
                            userToken.DeviceList.TerminalState = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                            updateDeviceList(userToken.DeviceList);

                            string FullDeviceNo = DeviceModule.GetFullDeviceNoByID(userToken.DeviceList.Id);
                            OnlineDeviceService.AddOnline(FullDeviceNo, userToken.DeviceList);
                            DeviceEvent deviceEvent = new DeviceEvent();
                            DeviceEventModule.InitDeviceEvent(deviceEvent);
                            deviceEvent.DeviceNo = FullDeviceNo;
                            deviceEvent.EventTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.EventType = userToken.DeviceList.TerminalState;
                            deviceEvent.DeviceTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.Remark = cmdreceive == null ? (userToken.DeviceList.TerminalState + " " + TotalPackage + " " + CurrentPackage) : cmdreceive.ToString();
                            deviceEvent.RawData = message_water.RawDataStr;
                            saveDeviceEvent(deviceEvent);
                            proxySendDeviceList(userToken.DeviceList, deviceEvent);

                        }

                        #endregion
                    }
                    else if (AFN == (byte)WaterBaseProtocol.AFN._34 && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Up)
                    {
                        #region _34
                        waterInfo += EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                        ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);
                        LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);

                        WaterCmd_34_1 cmdreceive = null;
                        if (DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETX
                            || DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB && DataBeginChar == (byte)WaterBaseProtocol.DataBeginChar.STX)
                        {
                            cmdreceive = new WaterCmd_34_1();
                            cmdreceive.MsgList = MsgList;
                            string msgreceive = cmdreceive.ReadMsg();
                            if (msgreceive == "")
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());

                                //DateTime? ObsTime = null;
                                long ticks = 0;
                                DateTime? ObsTime_Precipitation = null;
                                DateTime? ObsTime_WaterLevel = null;
                                DateTime? ObsTime_Voltage = null;
                                DateTime? ObsTime_CumulativePrecipitation = null;
                                decimal?[] Precipitation = null;
                                decimal?[] WaterLevel = null;
                                decimal? Voltage = 0;
                                decimal? CumulativePrecipitation = null;

                                foreach (Identifier iden in cmdreceive.List_Identifier)
                                {
                                    if (iden.GetKey() == 0xF0)
                                    {
                                        ticks = ((Identifier_F0)iden).ObsTime.Value.Ticks;
                                    }
                                    else if (iden.GetKey() == 0x26)
                                    {
                                        CumulativePrecipitation = ((Identifier_26)iden).CumulativePrecipitation;
                                        ObsTime_CumulativePrecipitation = new DateTime(ticks);
                                    }
                                    else if (iden.GetKey() == 0x38)
                                    {
                                        Voltage = ((Identifier_38)iden).Voltage;
                                        ObsTime_Voltage = new DateTime(ticks);
                                    }
                                    else if (iden.GetKey() == 0xF4)
                                    {
                                        Precipitation = ((Identifier_F4)iden).Precipitation;
                                        ObsTime_Precipitation = new DateTime(ticks);
                                    }
                                    else if (iden.GetKey() == 0xF5)
                                    {
                                        WaterLevel = ((Identifier_F5)iden).WaterLevel;
                                        ObsTime_WaterLevel = new DateTime(ticks);
                                    }
                                }

                                if (Precipitation != null && ObsTime_Precipitation.HasValue)
                                {
                                    decimal Rainfall_Hour = 0;
                                    for (int i = 0; i < 12; i++)
                                    {
                                        try
                                        {
                                            if (Precipitation[i].HasValue)
                                            {
                                                T_RainFall trf = new T_RainFall();
                                                trf.StationID = message_water.RemoteStation;
                                                trf.Rainfall = Precipitation[i].Value;
                                                trf.Rainfall_Day = 0;
                                                Rainfall_Hour += Precipitation[i].Value;
                                                trf.Rainfall_Hour = Rainfall_Hour;
                                                trf.Rainfall_Total = CumulativePrecipitation.HasValue ? CumulativePrecipitation.Value : 0;
                                                if (WaterLevel != null && WaterLevel.Length > i)
                                                    trf.WaterLevel = WaterLevel[i].HasValue ? WaterLevel[i].Value : 0;
                                                else
                                                    trf.WaterLevel = 0;
                                                trf.BV = Voltage.HasValue ? Voltage.Value : 0;
                                                trf.Acq_Time = ObsTime_Precipitation.Value.AddMinutes(5 * i);
                                                trf.CREATE_TIME = DateTime.Now;
                                                T_RainFallModule.Add(trf);

                                                if (i == 11)
                                                {
                                                    try
                                                    {
                                                        if (userToken.DeviceList != null)
                                                        {
                                                            userToken.DeviceList.Rainfall = trf.Rainfall;
                                                            userToken.DeviceList.Rainfall_Day = trf.Rainfall_Day;
                                                            userToken.DeviceList.Rainfall_Hour = trf.Rainfall_Hour;
                                                            userToken.DeviceList.Rainfall_Total = trf.Rainfall_Total;
                                                            userToken.DeviceList.WaterLevel = trf.WaterLevel;
                                                            userToken.DeviceList.Acq_Time = trf.Acq_Time;
                                                        }
                                                    }
                                                    catch { }
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]数据保存失败：" + ex.Message);
                                            LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]数据保存失败：" + ex.Message);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                            }

                            WaterCmd_34_2 cmdres = new WaterCmd_34_2();
                            cmdres.CenterStation = message_water.CenterStation;
                            cmdres.RemoteStation = message_water.RemoteStation;
                            cmdres.TotalPackage = message_water.TotalPackage;
                            cmdres.CurrentPackage = message_water.CurrentPackage;
                            cmdres.PW = message_water.PW;
                            if (message_water.DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB)
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.ACK;
                            else
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.EOT;
                            cmdres.SerialNumber = cmdreceive.SerialNumber;
                            cmdres.SendTime = DateTime.Now;

                            string msgres = cmdres.WriteMsg();
                            if (msgres == "")
                            {
                                send(cmdres.RawDataChar, 0, cmdres.RawDataChar.Length);
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                            }

                            MsgList = null;
                            WaterDeviceService.Remove(cKey);
                        }

                        if (userToken.DeviceList != null)
                        {
                            userToken.DeviceList.Online = 1;
                            userToken.DeviceList.LastUpdate = DateTime.Now;
                            userToken.DeviceList.TerminalState = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                            updateDeviceList(userToken.DeviceList);

                            string FullDeviceNo = DeviceModule.GetFullDeviceNoByID(userToken.DeviceList.Id);
                            OnlineDeviceService.AddOnline(FullDeviceNo, userToken.DeviceList);
                            DeviceEvent deviceEvent = new DeviceEvent();
                            DeviceEventModule.InitDeviceEvent(deviceEvent);
                            deviceEvent.DeviceNo = FullDeviceNo;
                            deviceEvent.EventTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.EventType = userToken.DeviceList.TerminalState;
                            deviceEvent.DeviceTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.Remark = cmdreceive == null ? (userToken.DeviceList.TerminalState + " " + TotalPackage + " " + CurrentPackage) : cmdreceive.ToString();
                            deviceEvent.RawData = message_water.RawDataStr;
                            saveDeviceEvent(deviceEvent);
                            proxySendDeviceList(userToken.DeviceList, deviceEvent);

                        }

                        #endregion
                    }
                    else if (AFN == (byte)WaterBaseProtocol.AFN._35 && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Up)
                    {
                        #region _35
                        waterInfo += EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                        ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);
                        LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);

                        WaterCmd_35_1 cmdreceive = null;
                        if (DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETX
                            || DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB && DataBeginChar == (byte)WaterBaseProtocol.DataBeginChar.STX)
                        {
                            cmdreceive = new WaterCmd_35_1();
                            cmdreceive.MsgList = MsgList;
                            string msgreceive = cmdreceive.ReadMsg();
                            if (msgreceive == "")
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                            }

                            WaterCmd_35_2 cmdres = new WaterCmd_35_2();
                            cmdres.CenterStation = message_water.CenterStation;
                            cmdres.RemoteStation = message_water.RemoteStation;
                            cmdres.TotalPackage = message_water.TotalPackage;
                            cmdres.CurrentPackage = message_water.CurrentPackage;
                            cmdres.PW = message_water.PW;
                            if (message_water.DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB)
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.ACK;
                            else
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.EOT;
                            cmdres.SerialNumber = cmdreceive.SerialNumber;
                            cmdres.SendTime = DateTime.Now;

                            string msgres = cmdres.WriteMsg();
                            if (msgres == "")
                            {
                                send(cmdres.RawDataChar, 0, cmdres.RawDataChar.Length);
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                            }

                            MsgList = null;
                            WaterDeviceService.Remove(cKey);
                        }

                        if (userToken.DeviceList != null)
                        {
                            userToken.DeviceList.Online = 1;
                            userToken.DeviceList.LastUpdate = DateTime.Now;
                            userToken.DeviceList.TerminalState = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                            updateDeviceList(userToken.DeviceList);

                            string FullDeviceNo = DeviceModule.GetFullDeviceNoByID(userToken.DeviceList.Id);
                            OnlineDeviceService.AddOnline(FullDeviceNo, userToken.DeviceList);
                            DeviceEvent deviceEvent = new DeviceEvent();
                            DeviceEventModule.InitDeviceEvent(deviceEvent);
                            deviceEvent.DeviceNo = FullDeviceNo;
                            deviceEvent.EventTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.EventType = userToken.DeviceList.TerminalState;
                            deviceEvent.DeviceTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.Remark = cmdreceive == null ? (userToken.DeviceList.TerminalState + " " + TotalPackage + " " + CurrentPackage) : cmdreceive.ToString();
                            deviceEvent.RawData = message_water.RawDataStr;
                            saveDeviceEvent(deviceEvent);
                            proxySendDeviceList(userToken.DeviceList, deviceEvent);

                        }

                        #endregion
                    }
                    else if (AFN == (byte)WaterBaseProtocol.AFN._36 && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Up)
                    {
                        #region _36
                        waterInfo += EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                        ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);
                        LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);

                        WaterCmd_36_2 cmdreceive = null;
                        if (DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETX
                            || DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB && DataBeginChar == (byte)WaterBaseProtocol.DataBeginChar.STX)
                        {
                            cmdreceive = new WaterCmd_36_2();
                            cmdreceive.MsgList = MsgList;
                            string msgreceive = cmdreceive.ReadMsg();
                            if (msgreceive == "")
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());

                                try
                                {
                                    byte[] bs = cmdreceive.iden_F3.ImgContent;
                                    Image img = byteArrayToImage(bs);
                                    string pathRoot = Environment.CurrentDirectory;
                                    if (FilePathCache.Path != "")
                                    {
                                        pathRoot = FilePathCache.Path;
                                    }
                                    string path = pathRoot + "/UploadImg/" + message_water.RemoteStation + "/";
                                    DirectoryInfo dir = new DirectoryInfo(path);
                                    if (!dir.Exists)
                                        dir.Create();
                                    string filename = path + DateTime.Now.ToString("yyyyMMddHHmmss") + ".jpg";
                                    img.Save(filename);
                                }
                                catch
                                {
                                    ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]图片保存失败：" + cmdreceive.ToString());
                                    LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]图片保存失败：" + cmdreceive.ToString());
                                }
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                            }

                            WaterCmd_36_3 cmdres = new WaterCmd_36_3();
                            cmdres.CenterStation = message_water.CenterStation;
                            cmdres.RemoteStation = message_water.RemoteStation;
                            cmdres.TotalPackage = message_water.TotalPackage;
                            cmdres.CurrentPackage = message_water.CurrentPackage;
                            cmdres.PW = message_water.PW;
                            if (message_water.DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB)
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.ACK;
                            else
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.EOT;
                            cmdres.SerialNumber = cmdreceive.SerialNumber;
                            cmdres.SendTime = DateTime.Now;

                            string msgres = cmdres.WriteMsg();
                            if (msgres == "")
                            {
                                send(cmdres.RawDataChar, 0, cmdres.RawDataChar.Length);
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                            }

                            MsgList = null;
                            WaterDeviceService.Remove(cKey);
                        }

                        if (userToken.DeviceList != null)
                        {
                            userToken.DeviceList.Online = 1;
                            userToken.DeviceList.LastUpdate = DateTime.Now;
                            userToken.DeviceList.TerminalState = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                            updateDeviceList(userToken.DeviceList);

                            string FullDeviceNo = DeviceModule.GetFullDeviceNoByID(userToken.DeviceList.Id);
                            OnlineDeviceService.AddOnline(FullDeviceNo, userToken.DeviceList);
                            DeviceEvent deviceEvent = new DeviceEvent();
                            DeviceEventModule.InitDeviceEvent(deviceEvent);
                            deviceEvent.DeviceNo = FullDeviceNo;
                            deviceEvent.EventTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.EventType = userToken.DeviceList.TerminalState;
                            deviceEvent.DeviceTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.Remark = cmdreceive == null ? (userToken.DeviceList.TerminalState + " " + TotalPackage + " " + CurrentPackage) : cmdreceive.ToString();
                            deviceEvent.RawData = message_water.RawDataStr;
                            saveDeviceEvent(deviceEvent);
                            proxySendDeviceList(userToken.DeviceList, deviceEvent);

                        }

                        #endregion
                    }
                    else if (AFN == (byte)WaterBaseProtocol.AFN._37 && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Up)
                    {
                        #region _37
                        waterInfo += EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                        ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);
                        LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);

                        WaterCmd_37_2 cmdreceive = null;
                        if (DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETX
                            || DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB && DataBeginChar == (byte)WaterBaseProtocol.DataBeginChar.STX)
                        {
                            cmdreceive = new WaterCmd_37_2();
                            cmdreceive.MsgList = MsgList;
                            string msgreceive = cmdreceive.ReadMsg();
                            if (msgreceive == "")
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());

                                decimal?[] WTDay = null;
                                decimal? WT = null;
                                decimal?[] DepthDay = null;
                                decimal? Depth = null;
                                decimal? Voltage = 0;
                                DateTime? ObsTime = null;
                                decimal? Precipitation = null;
                                decimal? Precipitation_Hour = null;
                                decimal? Precipitation_Day = null;
                                decimal? CumulativePrecipitation = null;
                                decimal? WaterLevel = null;

                                foreach (Identifier iden in cmdreceive.List_Identifier)
                                {
                                    if (iden.GetKey() == 0xFF)
                                    {
                                        if (((Identifier_FF)iden).GetKeySub() == 0x03)
                                        {
                                            WTDay = ((Identifier_FF_03)iden).WTDay;
                                        }
                                        else if (((Identifier_FF)iden).GetKeySub() == 0x0E)
                                        {
                                            DepthDay = ((Identifier_FF_0E)iden).DepthDay;
                                        }
                                    }
                                    else if (iden.GetKey() == 0x03)
                                    {
                                        WT = ((Identifier_03)iden).WT;
                                    }
                                    else if (iden.GetKey() == 0x0E)
                                    {
                                        Depth = ((Identifier_0E)iden).Depth;
                                    }
                                    else if (iden.GetKey() == 0x1A)
                                    {
                                        Precipitation_Hour = ((Identifier_1A)iden).Precipitation;
                                    }
                                    else if (iden.GetKey() == 0x1F)
                                    {
                                        Precipitation_Day = ((Identifier_1F)iden).Precipitation;
                                    }
                                    else if (iden.GetKey() == 0x20)
                                    {
                                        Precipitation = ((Identifier_20)iden).Precipitation;
                                    }
                                    else if (iden.GetKey() == 0x26)
                                    {
                                        CumulativePrecipitation = ((Identifier_26)iden).CumulativePrecipitation;
                                    }
                                    else if (iden.GetKey() == 0x39)
                                    {
                                        WaterLevel = ((Identifier_39)iden).WaterLevel;
                                    }
                                    else if (iden.GetKey() == 0x38)
                                    {
                                        Voltage = ((Identifier_38)iden).Voltage;
                                    }
                                    else if (iden.GetKey() == 0xF0)
                                    {
                                        ObsTime = ((Identifier_F0)iden).ObsTime;
                                    }
                                }

                                if (Depth.HasValue && WT.HasValue && ObsTime.HasValue)
                                {
                                    try
                                    {
                                        T_GroundWater tgw = new T_GroundWater();
                                        tgw.StationID = message_water.RemoteStation;
                                        tgw.GroundWaterLevel = Depth.Value;
                                        tgw.LineLength = 0;
                                        tgw.GroundWaterTempture = WT.Value;
                                        tgw.BV = Voltage.Value;
                                        tgw.Acq_Time = ObsTime.Value;
                                        tgw.CREATE_TIME = DateTime.Now;
                                        T_GroundWaterModule.Add(tgw);
                                    }
                                    catch (Exception ex)
                                    {
                                        ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]数据保存失败：" + ex.Message);
                                        LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]数据保存失败：" + ex.Message);
                                    }
                                }

                                if (Precipitation.HasValue && ObsTime.HasValue)
                                {
                                    try
                                    {
                                        T_RainFall trf = new T_RainFall();
                                        trf.StationID = message_water.RemoteStation;
                                        trf.Rainfall = Precipitation.Value;
                                        trf.Rainfall_Day = Precipitation_Day.HasValue ? Precipitation_Day.Value : 0;
                                        trf.Rainfall_Hour = Precipitation_Hour.HasValue ? Precipitation_Hour.Value : 0;
                                        trf.Rainfall_Total = CumulativePrecipitation.HasValue ? CumulativePrecipitation.Value : 0;
                                        trf.WaterLevel = WaterLevel.HasValue ? WaterLevel.Value : 0;
                                        trf.BV = Voltage.HasValue ? Voltage.Value : 0;
                                        trf.Acq_Time = ObsTime.Value;
                                        trf.CREATE_TIME = DateTime.Now;
                                        T_RainFallModule.Add(trf);

                                        try
                                        {
                                            if (userToken.DeviceList != null)
                                            {
                                                userToken.DeviceList.Rainfall = trf.Rainfall;
                                                userToken.DeviceList.Rainfall_Day = trf.Rainfall_Day;
                                                userToken.DeviceList.Rainfall_Hour = trf.Rainfall_Hour;
                                                userToken.DeviceList.Rainfall_Total = trf.Rainfall_Total;
                                                userToken.DeviceList.WaterLevel = trf.WaterLevel;
                                                userToken.DeviceList.Acq_Time = trf.Acq_Time;
                                            }
                                        }
                                        catch { }
                                    }
                                    catch (Exception ex)
                                    {
                                        ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]数据保存失败：" + ex.Message);
                                        LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]数据保存失败：" + ex.Message);
                                    }
                                }
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                            }

                            if (ToWaterDtuCommand.GetBaseMessageToDtuByKey(cKey + "-" + AFN) != null)
                            {
                                ToWaterDtuCommand.AddBaseMessageFromDtu(cKey + "-" + AFN, cmdreceive);
                                ShowLogData.add("添加发送命令响应保存！" + cKey + "-" + AFN + "：" + cmdreceive.UserDataAll);
                            }
                            else
                            {
                                ShowLogData.add("无法添加发送命令响应保存！" + cKey + "-" + AFN + "：" + cmdreceive.UserDataAll);
                            }

                            WaterCmd_37_3 cmdres = new WaterCmd_37_3();
                            cmdres.CenterStation = message_water.CenterStation;
                            cmdres.RemoteStation = message_water.RemoteStation;
                            cmdres.TotalPackage = message_water.TotalPackage;
                            cmdres.CurrentPackage = message_water.CurrentPackage;
                            cmdres.PW = message_water.PW;
                            if (message_water.DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB)
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.ACK;
                            else
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.EOT;
                            cmdres.SerialNumber = cmdreceive.SerialNumber;
                            cmdres.SendTime = DateTime.Now;

                            string msgres = cmdres.WriteMsg();
                            if (msgres == "")
                            {
                                send(cmdres.RawDataChar, 0, cmdres.RawDataChar.Length);
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                            }

                            MsgList = null;
                            WaterDeviceService.Remove(cKey);
                        }

                        if (userToken.DeviceList != null)
                        {
                            userToken.DeviceList.Online = 1;
                            userToken.DeviceList.LastUpdate = DateTime.Now;
                            userToken.DeviceList.TerminalState = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                            updateDeviceList(userToken.DeviceList);

                            string FullDeviceNo = DeviceModule.GetFullDeviceNoByID(userToken.DeviceList.Id);
                            OnlineDeviceService.AddOnline(FullDeviceNo, userToken.DeviceList);
                            DeviceEvent deviceEvent = new DeviceEvent();
                            DeviceEventModule.InitDeviceEvent(deviceEvent);
                            deviceEvent.DeviceNo = FullDeviceNo;
                            deviceEvent.EventTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.EventType = userToken.DeviceList.TerminalState;
                            deviceEvent.DeviceTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.Remark = cmdreceive == null ? (userToken.DeviceList.TerminalState + " " + TotalPackage + " " + CurrentPackage) : cmdreceive.ToString();
                            deviceEvent.RawData = message_water.RawDataStr;
                            saveDeviceEvent(deviceEvent);
                            proxySendDeviceList(userToken.DeviceList, deviceEvent);

                        }

                        #endregion
                    }
                    else if (AFN == (byte)WaterBaseProtocol.AFN._38 && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Up)
                    {
                        #region _38
                        waterInfo += EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                        ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);
                        LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);

                        WaterCmd_38_2 cmdreceive = null;
                        if (DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETX
                            || DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB && DataBeginChar == (byte)WaterBaseProtocol.DataBeginChar.STX)
                        {
                            cmdreceive = new WaterCmd_38_2();
                            cmdreceive.MsgList = MsgList;
                            string msgreceive = cmdreceive.ReadMsg();
                            if (msgreceive == "")
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                            }

                            if (ToWaterDtuCommand.GetBaseMessageToDtuByKey(cKey + "-" + AFN) != null)
                            {
                                ToWaterDtuCommand.AddBaseMessageFromDtu(cKey + "-" + AFN, cmdreceive);
                                ShowLogData.add("添加发送命令响应保存！" + cKey + "-" + AFN + "：" + cmdreceive.UserDataAll);
                            }
                            else
                            {
                                ShowLogData.add("无法添加发送命令响应保存！" + cKey + "-" + AFN + "：" + cmdreceive.UserDataAll);
                            }

                            WaterCmd_38_3 cmdres = new WaterCmd_38_3();
                            cmdres.CenterStation = message_water.CenterStation;
                            cmdres.RemoteStation = message_water.RemoteStation;
                            cmdres.TotalPackage = message_water.TotalPackage;
                            cmdres.CurrentPackage = message_water.CurrentPackage;
                            cmdres.PW = message_water.PW;
                            if (message_water.DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB)
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.ACK;
                            else
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.EOT;
                            cmdres.SerialNumber = cmdreceive.SerialNumber;
                            cmdres.SendTime = DateTime.Now;

                            string msgres = cmdres.WriteMsg();
                            if (msgres == "")
                            {
                                send(cmdres.RawDataChar, 0, cmdres.RawDataChar.Length);
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                            }

                            MsgList = null;
                            WaterDeviceService.Remove(cKey);
                        }

                        if (userToken.DeviceList != null)
                        {
                            userToken.DeviceList.Online = 1;
                            userToken.DeviceList.LastUpdate = DateTime.Now;
                            userToken.DeviceList.TerminalState = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                            updateDeviceList(userToken.DeviceList);

                            string FullDeviceNo = DeviceModule.GetFullDeviceNoByID(userToken.DeviceList.Id);
                            OnlineDeviceService.AddOnline(FullDeviceNo, userToken.DeviceList);
                            DeviceEvent deviceEvent = new DeviceEvent();
                            DeviceEventModule.InitDeviceEvent(deviceEvent);
                            deviceEvent.DeviceNo = FullDeviceNo;
                            deviceEvent.EventTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.EventType = userToken.DeviceList.TerminalState;
                            deviceEvent.DeviceTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.Remark = cmdreceive == null ? userToken.DeviceList.TerminalState : cmdreceive.ToString();
                            deviceEvent.RawData = message_water.RawDataStr;
                            saveDeviceEvent(deviceEvent);
                            proxySendDeviceList(userToken.DeviceList, deviceEvent);

                        }

                        #endregion
                    }
                    else if (AFN == (byte)WaterBaseProtocol.AFN._39 && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Up)
                    {
                        #region _39
                        waterInfo += EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                        ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);
                        LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);

                        WaterCmd_39_2 cmdreceive = null;
                        if (DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETX
                            || DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB && DataBeginChar == (byte)WaterBaseProtocol.DataBeginChar.STX)
                        {
                            cmdreceive = new WaterCmd_39_2();
                            cmdreceive.MsgList = MsgList;
                            string msgreceive = cmdreceive.ReadMsg();
                            if (msgreceive == "")
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                            }

                            if (ToWaterDtuCommand.GetBaseMessageToDtuByKey(cKey + "-" + AFN) != null)
                            {
                                ToWaterDtuCommand.AddBaseMessageFromDtu(cKey + "-" + AFN, cmdreceive);
                                ShowLogData.add("添加发送命令响应保存！" + cKey + "-" + AFN + "：" + cmdreceive.UserDataAll);
                            }
                            else
                            {
                                ShowLogData.add("无法添加发送命令响应保存！" + cKey + "-" + AFN + "：" + cmdreceive.UserDataAll);
                            }

                            WaterCmd_39_3 cmdres = new WaterCmd_39_3();
                            cmdres.CenterStation = message_water.CenterStation;
                            cmdres.RemoteStation = message_water.RemoteStation;
                            cmdres.TotalPackage = message_water.TotalPackage;
                            cmdres.CurrentPackage = message_water.CurrentPackage;
                            cmdres.PW = message_water.PW;
                            if (message_water.DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB)
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.ACK;
                            else
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.EOT;
                            cmdres.SerialNumber = cmdreceive.SerialNumber;
                            cmdres.SendTime = DateTime.Now;

                            string msgres = cmdres.WriteMsg();
                            if (msgres == "")
                            {
                                send(cmdres.RawDataChar, 0, cmdres.RawDataChar.Length);
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                            }

                            MsgList = null;
                            WaterDeviceService.Remove(cKey);
                        }

                        if (userToken.DeviceList != null)
                        {
                            userToken.DeviceList.Online = 1;
                            userToken.DeviceList.LastUpdate = DateTime.Now;
                            userToken.DeviceList.TerminalState = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                            updateDeviceList(userToken.DeviceList);

                            string FullDeviceNo = DeviceModule.GetFullDeviceNoByID(userToken.DeviceList.Id);
                            OnlineDeviceService.AddOnline(FullDeviceNo, userToken.DeviceList);
                            DeviceEvent deviceEvent = new DeviceEvent();
                            DeviceEventModule.InitDeviceEvent(deviceEvent);
                            deviceEvent.DeviceNo = FullDeviceNo;
                            deviceEvent.EventTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.EventType = userToken.DeviceList.TerminalState;
                            deviceEvent.DeviceTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.Remark = cmdreceive == null ? userToken.DeviceList.TerminalState : cmdreceive.ToString();
                            deviceEvent.RawData = message_water.RawDataStr;
                            saveDeviceEvent(deviceEvent);
                            proxySendDeviceList(userToken.DeviceList, deviceEvent);

                        }

                        #endregion
                    }
                    else if (AFN == (byte)WaterBaseProtocol.AFN._3A && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Up)
                    {
                        #region _3A
                        waterInfo += EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                        ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);
                        LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);

                        WaterCmd_3A_2 cmdreceive = null;
                        if (DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETX
                            || DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB && DataBeginChar == (byte)WaterBaseProtocol.DataBeginChar.STX)
                        {
                            cmdreceive = new WaterCmd_3A_2();
                            cmdreceive.MsgList = MsgList;
                            string msgreceive = cmdreceive.ReadMsg();
                            if (msgreceive == "")
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                            }

                            if (ToWaterDtuCommand.GetBaseMessageToDtuByKey(cKey + "-" + AFN) != null)
                            {
                                ToWaterDtuCommand.AddBaseMessageFromDtu(cKey + "-" + AFN, cmdreceive);
                                ShowLogData.add("添加发送命令响应保存！" + cKey + "-" + AFN + "：" + cmdreceive.UserDataAll);
                            }
                            else
                            {
                                ShowLogData.add("无法添加发送命令响应保存！" + cKey + "-" + AFN + "：" + cmdreceive.UserDataAll);
                            }

                            WaterCmd_3A_3 cmdres = new WaterCmd_3A_3();
                            cmdres.CenterStation = message_water.CenterStation;
                            cmdres.RemoteStation = message_water.RemoteStation;
                            cmdres.TotalPackage = message_water.TotalPackage;
                            cmdres.CurrentPackage = message_water.CurrentPackage;
                            cmdres.PW = message_water.PW;
                            if (message_water.DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB)
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.ACK;
                            else
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.EOT;
                            cmdres.SerialNumber = cmdreceive.SerialNumber;
                            cmdres.SendTime = DateTime.Now;

                            string msgres = cmdres.WriteMsg();
                            if (msgres == "")
                            {
                                send(cmdres.RawDataChar, 0, cmdres.RawDataChar.Length);
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                            }

                            MsgList = null;
                            WaterDeviceService.Remove(cKey);
                        }

                        if (userToken.DeviceList != null)
                        {
                            userToken.DeviceList.Online = 1;
                            userToken.DeviceList.LastUpdate = DateTime.Now;
                            userToken.DeviceList.TerminalState = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                            updateDeviceList(userToken.DeviceList);

                            string FullDeviceNo = DeviceModule.GetFullDeviceNoByID(userToken.DeviceList.Id);
                            OnlineDeviceService.AddOnline(FullDeviceNo, userToken.DeviceList);
                            DeviceEvent deviceEvent = new DeviceEvent();
                            DeviceEventModule.InitDeviceEvent(deviceEvent);
                            deviceEvent.DeviceNo = FullDeviceNo;
                            deviceEvent.EventTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.EventType = userToken.DeviceList.TerminalState;
                            deviceEvent.DeviceTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.Remark = cmdreceive == null ? userToken.DeviceList.TerminalState : cmdreceive.ToString();
                            deviceEvent.RawData = message_water.RawDataStr;
                            saveDeviceEvent(deviceEvent);
                            proxySendDeviceList(userToken.DeviceList, deviceEvent);

                        }

                        #endregion
                    }
                    else if (AFN == (byte)WaterBaseProtocol.AFN._40 && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Up)
                    {
                        #region _40
                        waterInfo += EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                        ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);
                        LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);

                        WaterCmd_40_2 cmdreceive = null;
                        if (DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETX
                            || DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB && DataBeginChar == (byte)WaterBaseProtocol.DataBeginChar.STX)
                        {
                            cmdreceive = new WaterCmd_40_2();
                            cmdreceive.MsgList = MsgList;
                            string msgreceive = cmdreceive.ReadMsg();
                            if (msgreceive == "")
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                            }

                            if (ToWaterDtuCommand.GetBaseMessageToDtuByKey(cKey + "-" + AFN) != null)
                            {
                                ToWaterDtuCommand.AddBaseMessageFromDtu(cKey + "-" + AFN, cmdreceive);
                                ShowLogData.add("添加发送命令响应保存！" + cKey + "-" + AFN + "：" + cmdreceive.UserDataAll);
                            }
                            else
                            {
                                ShowLogData.add("无法添加发送命令响应保存！" + cKey + "-" + AFN + "：" + cmdreceive.UserDataAll);
                            }

                            WaterCmd_40_3 cmdres = new WaterCmd_40_3();
                            cmdres.CenterStation = message_water.CenterStation;
                            cmdres.RemoteStation = message_water.RemoteStation;
                            cmdres.TotalPackage = message_water.TotalPackage;
                            cmdres.CurrentPackage = message_water.CurrentPackage;
                            cmdres.PW = message_water.PW;
                            if (message_water.DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB)
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.ACK;
                            else
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.EOT;
                            cmdres.SerialNumber = cmdreceive.SerialNumber;
                            cmdres.SendTime = DateTime.Now;

                            string msgres = cmdres.WriteMsg();
                            if (msgres == "")
                            {
                                send(cmdres.RawDataChar, 0, cmdres.RawDataChar.Length);
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                            }

                            MsgList = null;
                            WaterDeviceService.Remove(cKey);
                        }

                        if (userToken.DeviceList != null)
                        {
                            userToken.DeviceList.Online = 1;
                            userToken.DeviceList.LastUpdate = DateTime.Now;
                            userToken.DeviceList.TerminalState = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                            updateDeviceList(userToken.DeviceList);

                            string FullDeviceNo = DeviceModule.GetFullDeviceNoByID(userToken.DeviceList.Id);
                            OnlineDeviceService.AddOnline(FullDeviceNo, userToken.DeviceList);
                            DeviceEvent deviceEvent = new DeviceEvent();
                            DeviceEventModule.InitDeviceEvent(deviceEvent);
                            deviceEvent.DeviceNo = FullDeviceNo;
                            deviceEvent.EventTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.EventType = userToken.DeviceList.TerminalState;
                            deviceEvent.DeviceTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.Remark = cmdreceive == null ? userToken.DeviceList.TerminalState : cmdreceive.ToString();
                            deviceEvent.RawData = message_water.RawDataStr;
                            saveDeviceEvent(deviceEvent);
                            proxySendDeviceList(userToken.DeviceList, deviceEvent);

                        }

                        #endregion
                    }
                    else if (AFN == (byte)WaterBaseProtocol.AFN._41 && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Up)
                    {
                        #region _41
                        waterInfo += EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                        ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);
                        LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);

                        WaterCmd_41_2 cmdreceive = null;
                        if (DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETX
                            || DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB && DataBeginChar == (byte)WaterBaseProtocol.DataBeginChar.STX)
                        {
                            cmdreceive = new WaterCmd_41_2();
                            cmdreceive.MsgList = MsgList;
                            string msgreceive = cmdreceive.ReadMsg();
                            if (msgreceive == "")
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                            }

                            if (ToWaterDtuCommand.GetBaseMessageToDtuByKey(cKey + "-" + AFN) != null)
                            {
                                ToWaterDtuCommand.AddBaseMessageFromDtu(cKey + "-" + AFN, cmdreceive);
                                ShowLogData.add("添加发送命令响应保存！" + cKey + "-" + AFN + "：" + cmdreceive.UserDataAll);
                            }
                            else
                            {
                                ShowLogData.add("无法添加发送命令响应保存！" + cKey + "-" + AFN + "：" + cmdreceive.UserDataAll);
                            }

                            WaterCmd_41_3 cmdres = new WaterCmd_41_3();
                            cmdres.CenterStation = message_water.CenterStation;
                            cmdres.RemoteStation = message_water.RemoteStation;
                            cmdres.TotalPackage = message_water.TotalPackage;
                            cmdres.CurrentPackage = message_water.CurrentPackage;
                            cmdres.PW = message_water.PW;
                            if (message_water.DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB)
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.ACK;
                            else
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.EOT;
                            cmdres.SerialNumber = cmdreceive.SerialNumber;
                            cmdres.SendTime = DateTime.Now;

                            string msgres = cmdres.WriteMsg();
                            if (msgres == "")
                            {
                                send(cmdres.RawDataChar, 0, cmdres.RawDataChar.Length);
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                            }

                            MsgList = null;
                            WaterDeviceService.Remove(cKey);
                        }

                        if (userToken.DeviceList != null)
                        {
                            userToken.DeviceList.Online = 1;
                            userToken.DeviceList.LastUpdate = DateTime.Now;
                            userToken.DeviceList.TerminalState = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                            updateDeviceList(userToken.DeviceList);

                            string FullDeviceNo = DeviceModule.GetFullDeviceNoByID(userToken.DeviceList.Id);
                            OnlineDeviceService.AddOnline(FullDeviceNo, userToken.DeviceList);
                            DeviceEvent deviceEvent = new DeviceEvent();
                            DeviceEventModule.InitDeviceEvent(deviceEvent);
                            deviceEvent.DeviceNo = FullDeviceNo;
                            deviceEvent.EventTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.EventType = userToken.DeviceList.TerminalState;
                            deviceEvent.DeviceTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.Remark = cmdreceive == null ? userToken.DeviceList.TerminalState : cmdreceive.ToString();
                            deviceEvent.RawData = message_water.RawDataStr;
                            saveDeviceEvent(deviceEvent);
                            proxySendDeviceList(userToken.DeviceList, deviceEvent);

                        }

                        #endregion
                    }
                    else if (AFN == (byte)WaterBaseProtocol.AFN._42 && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Up)
                    {
                        #region _42
                        waterInfo += EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                        ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);
                        LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);

                        WaterCmd_42_2 cmdreceive = null;
                        if (DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETX
                            || DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB && DataBeginChar == (byte)WaterBaseProtocol.DataBeginChar.STX)
                        {
                            cmdreceive = new WaterCmd_42_2();
                            cmdreceive.MsgList = MsgList;
                            string msgreceive = cmdreceive.ReadMsg();
                            if (msgreceive == "")
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                            }

                            if (ToWaterDtuCommand.GetBaseMessageToDtuByKey(cKey + "-" + AFN) != null)
                            {
                                ToWaterDtuCommand.AddBaseMessageFromDtu(cKey + "-" + AFN, cmdreceive);
                                ShowLogData.add("添加发送命令响应保存！" + cKey + "-" + AFN + "：" + cmdreceive.UserDataAll);
                            }
                            else
                            {
                                ShowLogData.add("无法添加发送命令响应保存！" + cKey + "-" + AFN + "：" + cmdreceive.UserDataAll);
                            }

                            WaterCmd_42_3 cmdres = new WaterCmd_42_3();
                            cmdres.CenterStation = message_water.CenterStation;
                            cmdres.RemoteStation = message_water.RemoteStation;
                            cmdres.TotalPackage = message_water.TotalPackage;
                            cmdres.CurrentPackage = message_water.CurrentPackage;
                            cmdres.PW = message_water.PW;
                            if (message_water.DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB)
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.ACK;
                            else
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.EOT;
                            cmdres.SerialNumber = cmdreceive.SerialNumber;
                            cmdres.SendTime = DateTime.Now;

                            string msgres = cmdres.WriteMsg();
                            if (msgres == "")
                            {
                                send(cmdres.RawDataChar, 0, cmdres.RawDataChar.Length);
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                            }

                            MsgList = null;
                            WaterDeviceService.Remove(cKey);
                        }

                        if (userToken.DeviceList != null)
                        {
                            userToken.DeviceList.Online = 1;
                            userToken.DeviceList.LastUpdate = DateTime.Now;
                            userToken.DeviceList.TerminalState = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                            updateDeviceList(userToken.DeviceList);

                            string FullDeviceNo = DeviceModule.GetFullDeviceNoByID(userToken.DeviceList.Id);
                            OnlineDeviceService.AddOnline(FullDeviceNo, userToken.DeviceList);
                            DeviceEvent deviceEvent = new DeviceEvent();
                            DeviceEventModule.InitDeviceEvent(deviceEvent);
                            deviceEvent.DeviceNo = FullDeviceNo;
                            deviceEvent.EventTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.EventType = userToken.DeviceList.TerminalState;
                            deviceEvent.DeviceTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.Remark = cmdreceive == null ? userToken.DeviceList.TerminalState : cmdreceive.ToString();
                            deviceEvent.RawData = message_water.RawDataStr;
                            saveDeviceEvent(deviceEvent);
                            proxySendDeviceList(userToken.DeviceList, deviceEvent);

                        }

                        #endregion
                    }
                    else if (AFN == (byte)WaterBaseProtocol.AFN._43 && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Up)
                    {
                        #region _43
                        waterInfo += EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                        ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);
                        LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);

                        WaterCmd_43_2 cmdreceive = null;
                        if (DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETX
                            || DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB && DataBeginChar == (byte)WaterBaseProtocol.DataBeginChar.STX)
                        {
                            cmdreceive = new WaterCmd_43_2();
                            cmdreceive.MsgList = MsgList;
                            string msgreceive = cmdreceive.ReadMsg();
                            if (msgreceive == "")
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                            }

                            if (ToWaterDtuCommand.GetBaseMessageToDtuByKey(cKey + "-" + AFN) != null)
                            {
                                ToWaterDtuCommand.AddBaseMessageFromDtu(cKey + "-" + AFN, cmdreceive);
                                ShowLogData.add("添加发送命令响应保存！" + cKey + "-" + AFN + "：" + cmdreceive.UserDataAll);
                            }
                            else
                            {
                                ShowLogData.add("无法添加发送命令响应保存！" + cKey + "-" + AFN + "：" + cmdreceive.UserDataAll);
                            }

                            WaterCmd_43_3 cmdres = new WaterCmd_43_3();
                            cmdres.CenterStation = message_water.CenterStation;
                            cmdres.RemoteStation = message_water.RemoteStation;
                            cmdres.TotalPackage = message_water.TotalPackage;
                            cmdres.CurrentPackage = message_water.CurrentPackage;
                            cmdres.PW = message_water.PW;
                            if (message_water.DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB)
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.ACK;
                            else
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.EOT;
                            cmdres.SerialNumber = cmdreceive.SerialNumber;
                            cmdres.SendTime = DateTime.Now;

                            string msgres = cmdres.WriteMsg();
                            if (msgres == "")
                            {
                                send(cmdres.RawDataChar, 0, cmdres.RawDataChar.Length);
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                            }

                            MsgList = null;
                            WaterDeviceService.Remove(cKey);
                        }

                        if (userToken.DeviceList != null)
                        {
                            userToken.DeviceList.Online = 1;
                            userToken.DeviceList.LastUpdate = DateTime.Now;
                            userToken.DeviceList.TerminalState = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                            updateDeviceList(userToken.DeviceList);

                            string FullDeviceNo = DeviceModule.GetFullDeviceNoByID(userToken.DeviceList.Id);
                            OnlineDeviceService.AddOnline(FullDeviceNo, userToken.DeviceList);
                            DeviceEvent deviceEvent = new DeviceEvent();
                            DeviceEventModule.InitDeviceEvent(deviceEvent);
                            deviceEvent.DeviceNo = FullDeviceNo;
                            deviceEvent.EventTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.EventType = userToken.DeviceList.TerminalState;
                            deviceEvent.DeviceTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.Remark = cmdreceive == null ? userToken.DeviceList.TerminalState : cmdreceive.ToString();
                            deviceEvent.RawData = message_water.RawDataStr;
                            saveDeviceEvent(deviceEvent);
                            proxySendDeviceList(userToken.DeviceList, deviceEvent);

                        }

                        #endregion
                    }
                    else if (AFN == (byte)WaterBaseProtocol.AFN._44 && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Up)
                    {
                        #region _44
                        waterInfo += EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                        ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);
                        LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);

                        WaterCmd_44_2 cmdreceive = null;
                        if (DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETX
                            || DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB && DataBeginChar == (byte)WaterBaseProtocol.DataBeginChar.STX)
                        {
                            cmdreceive = new WaterCmd_44_2();
                            cmdreceive.MsgList = MsgList;
                            string msgreceive = cmdreceive.ReadMsg();
                            if (msgreceive == "")
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                            }

                            if (ToWaterDtuCommand.GetBaseMessageToDtuByKey(cKey + "-" + AFN) != null)
                            {
                                ToWaterDtuCommand.AddBaseMessageFromDtu(cKey + "-" + AFN, cmdreceive);
                                ShowLogData.add("添加发送命令响应保存！" + cKey + "-" + AFN + "：" + cmdreceive.UserDataAll);
                            }
                            else
                            {
                                ShowLogData.add("无法添加发送命令响应保存！" + cKey + "-" + AFN + "：" + cmdreceive.UserDataAll);
                            }

                            WaterCmd_44_3 cmdres = new WaterCmd_44_3();
                            cmdres.CenterStation = message_water.CenterStation;
                            cmdres.RemoteStation = message_water.RemoteStation;
                            cmdres.TotalPackage = message_water.TotalPackage;
                            cmdres.CurrentPackage = message_water.CurrentPackage;
                            cmdres.PW = message_water.PW;
                            if (message_water.DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB)
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.ACK;
                            else
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.EOT;
                            cmdres.SerialNumber = cmdreceive.SerialNumber;
                            cmdres.SendTime = DateTime.Now;

                            string msgres = cmdres.WriteMsg();
                            if (msgres == "")
                            {
                                send(cmdres.RawDataChar, 0, cmdres.RawDataChar.Length);
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                            }

                            MsgList = null;
                            WaterDeviceService.Remove(cKey);
                        }

                        if (userToken.DeviceList != null)
                        {
                            userToken.DeviceList.Online = 1;
                            userToken.DeviceList.LastUpdate = DateTime.Now;
                            userToken.DeviceList.TerminalState = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                            updateDeviceList(userToken.DeviceList);

                            string FullDeviceNo = DeviceModule.GetFullDeviceNoByID(userToken.DeviceList.Id);
                            OnlineDeviceService.AddOnline(FullDeviceNo, userToken.DeviceList);
                            DeviceEvent deviceEvent = new DeviceEvent();
                            DeviceEventModule.InitDeviceEvent(deviceEvent);
                            deviceEvent.DeviceNo = FullDeviceNo;
                            deviceEvent.EventTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.EventType = userToken.DeviceList.TerminalState;
                            deviceEvent.DeviceTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.Remark = cmdreceive == null ? userToken.DeviceList.TerminalState : cmdreceive.ToString();
                            deviceEvent.RawData = message_water.RawDataStr;
                            saveDeviceEvent(deviceEvent);
                            proxySendDeviceList(userToken.DeviceList, deviceEvent);

                        }

                        #endregion
                    }
                    else if (AFN == (byte)WaterBaseProtocol.AFN._45 && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Up)
                    {
                        #region _45
                        waterInfo += EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                        ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);
                        LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);

                        WaterCmd_45_2 cmdreceive = null;
                        if (DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETX
                            || DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB && DataBeginChar == (byte)WaterBaseProtocol.DataBeginChar.STX)
                        {
                            cmdreceive = new WaterCmd_45_2();
                            cmdreceive.MsgList = MsgList;
                            string msgreceive = cmdreceive.ReadMsg();
                            if (msgreceive == "")
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                            }

                            if (ToWaterDtuCommand.GetBaseMessageToDtuByKey(cKey + "-" + AFN) != null)
                            {
                                ToWaterDtuCommand.AddBaseMessageFromDtu(cKey + "-" + AFN, cmdreceive);
                                ShowLogData.add("添加发送命令响应保存！" + cKey + "-" + AFN + "：" + cmdreceive.UserDataAll);
                            }
                            else
                            {
                                ShowLogData.add("无法添加发送命令响应保存！" + cKey + "-" + AFN + "：" + cmdreceive.UserDataAll);
                            }

                            WaterCmd_45_3 cmdres = new WaterCmd_45_3();
                            cmdres.CenterStation = message_water.CenterStation;
                            cmdres.RemoteStation = message_water.RemoteStation;
                            cmdres.TotalPackage = message_water.TotalPackage;
                            cmdres.CurrentPackage = message_water.CurrentPackage;
                            cmdres.PW = message_water.PW;
                            if (message_water.DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB)
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.ACK;
                            else
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.EOT;
                            cmdres.SerialNumber = cmdreceive.SerialNumber;
                            cmdres.SendTime = DateTime.Now;

                            string msgres = cmdres.WriteMsg();
                            if (msgres == "")
                            {
                                send(cmdres.RawDataChar, 0, cmdres.RawDataChar.Length);
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                            }

                            MsgList = null;
                            WaterDeviceService.Remove(cKey);
                        }

                        if (userToken.DeviceList != null)
                        {
                            userToken.DeviceList.Online = 1;
                            userToken.DeviceList.LastUpdate = DateTime.Now;
                            userToken.DeviceList.TerminalState = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                            updateDeviceList(userToken.DeviceList);

                            string FullDeviceNo = DeviceModule.GetFullDeviceNoByID(userToken.DeviceList.Id);
                            OnlineDeviceService.AddOnline(FullDeviceNo, userToken.DeviceList);
                            DeviceEvent deviceEvent = new DeviceEvent();
                            DeviceEventModule.InitDeviceEvent(deviceEvent);
                            deviceEvent.DeviceNo = FullDeviceNo;
                            deviceEvent.EventTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.EventType = userToken.DeviceList.TerminalState;
                            deviceEvent.DeviceTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.Remark = cmdreceive == null ? userToken.DeviceList.TerminalState : cmdreceive.ToString();
                            deviceEvent.RawData = message_water.RawDataStr;
                            saveDeviceEvent(deviceEvent);
                            proxySendDeviceList(userToken.DeviceList, deviceEvent);

                        }

                        #endregion
                    }
                    else if (AFN == (byte)WaterBaseProtocol.AFN._46 && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Up)
                    {
                        #region _46
                        waterInfo += EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                        ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);
                        LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);

                        WaterCmd_46_2 cmdreceive = null;
                        if (DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETX
                            || DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB && DataBeginChar == (byte)WaterBaseProtocol.DataBeginChar.STX)
                        {
                            cmdreceive = new WaterCmd_46_2();
                            cmdreceive.MsgList = MsgList;
                            string msgreceive = cmdreceive.ReadMsg();
                            if (msgreceive == "")
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                            }

                            if (ToWaterDtuCommand.GetBaseMessageToDtuByKey(cKey + "-" + AFN) != null)
                            {
                                ToWaterDtuCommand.AddBaseMessageFromDtu(cKey + "-" + AFN, cmdreceive);
                                ShowLogData.add("添加发送命令响应保存！" + cKey + "-" + AFN + "：" + cmdreceive.UserDataAll);
                            }
                            else
                            {
                                ShowLogData.add("无法添加发送命令响应保存！" + cKey + "-" + AFN + "：" + cmdreceive.UserDataAll);
                            }

                            WaterCmd_46_3 cmdres = new WaterCmd_46_3();
                            cmdres.CenterStation = message_water.CenterStation;
                            cmdres.RemoteStation = message_water.RemoteStation;
                            cmdres.TotalPackage = message_water.TotalPackage;
                            cmdres.CurrentPackage = message_water.CurrentPackage;
                            cmdres.PW = message_water.PW;
                            if (message_water.DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB)
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.ACK;
                            else
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.EOT;
                            cmdres.SerialNumber = cmdreceive.SerialNumber;
                            cmdres.SendTime = DateTime.Now;

                            string msgres = cmdres.WriteMsg();
                            if (msgres == "")
                            {
                                send(cmdres.RawDataChar, 0, cmdres.RawDataChar.Length);
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                            }

                            MsgList = null;
                            WaterDeviceService.Remove(cKey);
                        }

                        if (userToken.DeviceList != null)
                        {
                            userToken.DeviceList.Online = 1;
                            userToken.DeviceList.LastUpdate = DateTime.Now;
                            userToken.DeviceList.TerminalState = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                            updateDeviceList(userToken.DeviceList);

                            string FullDeviceNo = DeviceModule.GetFullDeviceNoByID(userToken.DeviceList.Id);
                            OnlineDeviceService.AddOnline(FullDeviceNo, userToken.DeviceList);
                            DeviceEvent deviceEvent = new DeviceEvent();
                            DeviceEventModule.InitDeviceEvent(deviceEvent);
                            deviceEvent.DeviceNo = FullDeviceNo;
                            deviceEvent.EventTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.EventType = userToken.DeviceList.TerminalState;
                            deviceEvent.DeviceTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.Remark = cmdreceive == null ? userToken.DeviceList.TerminalState : cmdreceive.ToString();
                            deviceEvent.RawData = message_water.RawDataStr;
                            saveDeviceEvent(deviceEvent);
                            proxySendDeviceList(userToken.DeviceList, deviceEvent);

                        }

                        #endregion
                    }
                    else if (AFN == (byte)WaterBaseProtocol.AFN._47 && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Up)
                    {
                        #region _47
                        waterInfo += EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                        ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);
                        LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);

                        WaterCmd_47_2 cmdreceive = null;
                        if (DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETX
                            || DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB && DataBeginChar == (byte)WaterBaseProtocol.DataBeginChar.STX)
                        {
                            cmdreceive = new WaterCmd_47_2();
                            cmdreceive.MsgList = MsgList;
                            string msgreceive = cmdreceive.ReadMsg();
                            if (msgreceive == "")
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                            }

                            if (ToWaterDtuCommand.GetBaseMessageToDtuByKey(cKey + "-" + AFN) != null)
                            {
                                ToWaterDtuCommand.AddBaseMessageFromDtu(cKey + "-" + AFN, cmdreceive);
                                ShowLogData.add("添加发送命令响应保存！" + cKey + "-" + AFN + "：" + cmdreceive.UserDataAll);
                            }
                            else
                            {
                                ShowLogData.add("无法添加发送命令响应保存！" + cKey + "-" + AFN + "：" + cmdreceive.UserDataAll);
                            }

                            WaterCmd_47_3 cmdres = new WaterCmd_47_3();
                            cmdres.CenterStation = message_water.CenterStation;
                            cmdres.RemoteStation = message_water.RemoteStation;
                            cmdres.TotalPackage = message_water.TotalPackage;
                            cmdres.CurrentPackage = message_water.CurrentPackage;
                            cmdres.PW = message_water.PW;
                            if (message_water.DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB)
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.ACK;
                            else
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.EOT;
                            cmdres.SerialNumber = cmdreceive.SerialNumber;
                            cmdres.SendTime = DateTime.Now;

                            string msgres = cmdres.WriteMsg();
                            if (msgres == "")
                            {
                                send(cmdres.RawDataChar, 0, cmdres.RawDataChar.Length);
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                            }

                            MsgList = null;
                            WaterDeviceService.Remove(cKey);
                        }

                        if (userToken.DeviceList != null)
                        {
                            userToken.DeviceList.Online = 1;
                            userToken.DeviceList.LastUpdate = DateTime.Now;
                            userToken.DeviceList.TerminalState = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                            updateDeviceList(userToken.DeviceList);

                            string FullDeviceNo = DeviceModule.GetFullDeviceNoByID(userToken.DeviceList.Id);
                            OnlineDeviceService.AddOnline(FullDeviceNo, userToken.DeviceList);
                            DeviceEvent deviceEvent = new DeviceEvent();
                            DeviceEventModule.InitDeviceEvent(deviceEvent);
                            deviceEvent.DeviceNo = FullDeviceNo;
                            deviceEvent.EventTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.EventType = userToken.DeviceList.TerminalState;
                            deviceEvent.DeviceTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.Remark = cmdreceive == null ? userToken.DeviceList.TerminalState : cmdreceive.ToString();
                            deviceEvent.RawData = message_water.RawDataStr;
                            saveDeviceEvent(deviceEvent);
                            proxySendDeviceList(userToken.DeviceList, deviceEvent);

                        }

                        #endregion
                    }
                    else if (AFN == (byte)WaterBaseProtocol.AFN._48 && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Up)
                    {
                        #region _48
                        waterInfo += EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                        ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);
                        LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);

                        WaterCmd_48_2 cmdreceive = null;
                        if (DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETX
                            || DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB && DataBeginChar == (byte)WaterBaseProtocol.DataBeginChar.STX)
                        {
                            cmdreceive = new WaterCmd_48_2();
                            cmdreceive.MsgList = MsgList;
                            string msgreceive = cmdreceive.ReadMsg();
                            if (msgreceive == "")
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                            }

                            if (ToWaterDtuCommand.GetBaseMessageToDtuByKey(cKey + "-" + AFN) != null)
                            {
                                ToWaterDtuCommand.AddBaseMessageFromDtu(cKey + "-" + AFN, cmdreceive);
                                ShowLogData.add("添加发送命令响应保存！" + cKey + "-" + AFN + "：" + cmdreceive.UserDataAll);
                            }
                            else
                            {
                                ShowLogData.add("无法添加发送命令响应保存！" + cKey + "-" + AFN + "：" + cmdreceive.UserDataAll);
                            }

                            WaterCmd_48_3 cmdres = new WaterCmd_48_3();
                            cmdres.CenterStation = message_water.CenterStation;
                            cmdres.RemoteStation = message_water.RemoteStation;
                            cmdres.TotalPackage = message_water.TotalPackage;
                            cmdres.CurrentPackage = message_water.CurrentPackage;
                            cmdres.PW = message_water.PW;
                            if (message_water.DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB)
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.ACK;
                            else
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.EOT;
                            cmdres.SerialNumber = cmdreceive.SerialNumber;
                            cmdres.SendTime = DateTime.Now;

                            string msgres = cmdres.WriteMsg();
                            if (msgres == "")
                            {
                                send(cmdres.RawDataChar, 0, cmdres.RawDataChar.Length);
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                            }

                            MsgList = null;
                            WaterDeviceService.Remove(cKey);
                        }

                        if (userToken.DeviceList != null)
                        {
                            userToken.DeviceList.Online = 1;
                            userToken.DeviceList.LastUpdate = DateTime.Now;
                            userToken.DeviceList.TerminalState = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                            updateDeviceList(userToken.DeviceList);

                            string FullDeviceNo = DeviceModule.GetFullDeviceNoByID(userToken.DeviceList.Id);
                            OnlineDeviceService.AddOnline(FullDeviceNo, userToken.DeviceList);
                            DeviceEvent deviceEvent = new DeviceEvent();
                            DeviceEventModule.InitDeviceEvent(deviceEvent);
                            deviceEvent.DeviceNo = FullDeviceNo;
                            deviceEvent.EventTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.EventType = userToken.DeviceList.TerminalState;
                            deviceEvent.DeviceTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.Remark = cmdreceive == null ? userToken.DeviceList.TerminalState : cmdreceive.ToString();
                            deviceEvent.RawData = message_water.RawDataStr;
                            saveDeviceEvent(deviceEvent);
                            proxySendDeviceList(userToken.DeviceList, deviceEvent);

                        }

                        #endregion
                    }
                    else if (AFN == (byte)WaterBaseProtocol.AFN._49 && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Up)
                    {
                        #region _49
                        waterInfo += EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                        ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);
                        LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);

                        WaterCmd_49_2 cmdreceive = null;
                        if (DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETX
                            || DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB && DataBeginChar == (byte)WaterBaseProtocol.DataBeginChar.STX)
                        {
                            cmdreceive = new WaterCmd_49_2();
                            cmdreceive.MsgList = MsgList;
                            string msgreceive = cmdreceive.ReadMsg();
                            if (msgreceive == "")
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                            }

                            if (ToWaterDtuCommand.GetBaseMessageToDtuByKey(cKey + "-" + AFN) != null)
                            {
                                ToWaterDtuCommand.AddBaseMessageFromDtu(cKey + "-" + AFN, cmdreceive);
                                ShowLogData.add("添加发送命令响应保存！" + cKey + "-" + AFN + "：" + cmdreceive.UserDataAll);
                            }
                            else
                            {
                                ShowLogData.add("无法添加发送命令响应保存！" + cKey + "-" + AFN + "：" + cmdreceive.UserDataAll);
                            }

                            WaterCmd_49_3 cmdres = new WaterCmd_49_3();
                            cmdres.CenterStation = message_water.CenterStation;
                            cmdres.RemoteStation = message_water.RemoteStation;
                            cmdres.TotalPackage = message_water.TotalPackage;
                            cmdres.CurrentPackage = message_water.CurrentPackage;
                            cmdres.PW = message_water.PW;
                            if (message_water.DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB)
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.ACK;
                            else
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.EOT;
                            cmdres.SerialNumber = cmdreceive.SerialNumber;
                            cmdres.SendTime = DateTime.Now;

                            string msgres = cmdres.WriteMsg();
                            if (msgres == "")
                            {
                                send(cmdres.RawDataChar, 0, cmdres.RawDataChar.Length);
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                            }

                            MsgList = null;
                            WaterDeviceService.Remove(cKey);
                        }

                        if (userToken.DeviceList != null)
                        {
                            userToken.DeviceList.Online = 1;
                            userToken.DeviceList.LastUpdate = DateTime.Now;
                            userToken.DeviceList.TerminalState = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                            updateDeviceList(userToken.DeviceList);

                            string FullDeviceNo = DeviceModule.GetFullDeviceNoByID(userToken.DeviceList.Id);
                            OnlineDeviceService.AddOnline(FullDeviceNo, userToken.DeviceList);
                            DeviceEvent deviceEvent = new DeviceEvent();
                            DeviceEventModule.InitDeviceEvent(deviceEvent);
                            deviceEvent.DeviceNo = FullDeviceNo;
                            deviceEvent.EventTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.EventType = userToken.DeviceList.TerminalState;
                            deviceEvent.DeviceTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.Remark = cmdreceive == null ? userToken.DeviceList.TerminalState : cmdreceive.ToString();
                            deviceEvent.RawData = message_water.RawDataStr;
                            saveDeviceEvent(deviceEvent);
                            proxySendDeviceList(userToken.DeviceList, deviceEvent);

                        }

                        #endregion
                    }
                    else if (AFN == (byte)WaterBaseProtocol.AFN._4A && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Up)
                    {
                        #region _4A
                        waterInfo += EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                        ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);
                        LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);

                        WaterCmd_4A_2 cmdreceive = null;
                        if (DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETX
                            || DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB && DataBeginChar == (byte)WaterBaseProtocol.DataBeginChar.STX)
                        {
                            cmdreceive = new WaterCmd_4A_2();
                            cmdreceive.MsgList = MsgList;
                            string msgreceive = cmdreceive.ReadMsg();
                            if (msgreceive == "")
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                            }

                            if (ToWaterDtuCommand.GetBaseMessageToDtuByKey(cKey + "-" + AFN) != null)
                            {
                                ToWaterDtuCommand.AddBaseMessageFromDtu(cKey + "-" + AFN, cmdreceive);
                                ShowLogData.add("添加发送命令响应保存！" + cKey + "-" + AFN + "：" + cmdreceive.UserDataAll);
                            }
                            else
                            {
                                ShowLogData.add("无法添加发送命令响应保存！" + cKey + "-" + AFN + "：" + cmdreceive.UserDataAll);
                            }

                            WaterCmd_4A_3 cmdres = new WaterCmd_4A_3();
                            cmdres.CenterStation = message_water.CenterStation;
                            cmdres.RemoteStation = message_water.RemoteStation;
                            cmdres.TotalPackage = message_water.TotalPackage;
                            cmdres.CurrentPackage = message_water.CurrentPackage;
                            cmdres.PW = message_water.PW;
                            if (message_water.DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB)
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.ACK;
                            else
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.EOT;
                            cmdres.SerialNumber = cmdreceive.SerialNumber;
                            cmdres.SendTime = DateTime.Now;

                            string msgres = cmdres.WriteMsg();
                            if (msgres == "")
                            {
                                send(cmdres.RawDataChar, 0, cmdres.RawDataChar.Length);
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                            }

                            MsgList = null;
                            WaterDeviceService.Remove(cKey);
                        }

                        if (userToken.DeviceList != null)
                        {
                            userToken.DeviceList.Online = 1;
                            userToken.DeviceList.LastUpdate = DateTime.Now;
                            userToken.DeviceList.TerminalState = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                            updateDeviceList(userToken.DeviceList);

                            string FullDeviceNo = DeviceModule.GetFullDeviceNoByID(userToken.DeviceList.Id);
                            OnlineDeviceService.AddOnline(FullDeviceNo, userToken.DeviceList);
                            DeviceEvent deviceEvent = new DeviceEvent();
                            DeviceEventModule.InitDeviceEvent(deviceEvent);
                            deviceEvent.DeviceNo = FullDeviceNo;
                            deviceEvent.EventTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.EventType = userToken.DeviceList.TerminalState;
                            deviceEvent.DeviceTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.Remark = cmdreceive == null ? userToken.DeviceList.TerminalState : cmdreceive.ToString();
                            deviceEvent.RawData = message_water.RawDataStr;
                            saveDeviceEvent(deviceEvent);
                            proxySendDeviceList(userToken.DeviceList, deviceEvent);

                        }

                        #endregion
                    }
                    else if (AFN == (byte)WaterBaseProtocol.AFN._4B && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Up)
                    {
                        #region _4B
                        waterInfo += EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                        ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);
                        LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);

                        WaterCmd_4B_2 cmdreceive = null;
                        if (DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETX
                            || DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB && DataBeginChar == (byte)WaterBaseProtocol.DataBeginChar.STX)
                        {
                            cmdreceive = new WaterCmd_4B_2();
                            cmdreceive.MsgList = MsgList;
                            string msgreceive = cmdreceive.ReadMsg();
                            if (msgreceive == "")
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                            }

                            if (ToWaterDtuCommand.GetBaseMessageToDtuByKey(cKey + "-" + AFN) != null)
                            {
                                ToWaterDtuCommand.AddBaseMessageFromDtu(cKey + "-" + AFN, cmdreceive);
                                ShowLogData.add("添加发送命令响应保存！" + cKey + "-" + AFN + "：" + cmdreceive.UserDataAll);
                            }
                            else
                            {
                                ShowLogData.add("无法添加发送命令响应保存！" + cKey + "-" + AFN + "：" + cmdreceive.UserDataAll);
                            }

                            WaterCmd_4B_3 cmdres = new WaterCmd_4B_3();
                            cmdres.CenterStation = message_water.CenterStation;
                            cmdres.RemoteStation = message_water.RemoteStation;
                            cmdres.TotalPackage = message_water.TotalPackage;
                            cmdres.CurrentPackage = message_water.CurrentPackage;
                            cmdres.PW = message_water.PW;
                            if (message_water.DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB)
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.ACK;
                            else
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.EOT;
                            cmdres.SerialNumber = cmdreceive.SerialNumber;
                            cmdres.SendTime = DateTime.Now;

                            string msgres = cmdres.WriteMsg();
                            if (msgres == "")
                            {
                                send(cmdres.RawDataChar, 0, cmdres.RawDataChar.Length);
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                            }

                            MsgList = null;
                            WaterDeviceService.Remove(cKey);
                        }

                        if (userToken.DeviceList != null)
                        {
                            userToken.DeviceList.Online = 1;
                            userToken.DeviceList.LastUpdate = DateTime.Now;
                            userToken.DeviceList.TerminalState = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                            updateDeviceList(userToken.DeviceList);

                            string FullDeviceNo = DeviceModule.GetFullDeviceNoByID(userToken.DeviceList.Id);
                            OnlineDeviceService.AddOnline(FullDeviceNo, userToken.DeviceList);
                            DeviceEvent deviceEvent = new DeviceEvent();
                            DeviceEventModule.InitDeviceEvent(deviceEvent);
                            deviceEvent.DeviceNo = FullDeviceNo;
                            deviceEvent.EventTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.EventType = userToken.DeviceList.TerminalState;
                            deviceEvent.DeviceTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.Remark = cmdreceive == null ? userToken.DeviceList.TerminalState : cmdreceive.ToString();
                            deviceEvent.RawData = message_water.RawDataStr;
                            saveDeviceEvent(deviceEvent);
                            proxySendDeviceList(userToken.DeviceList, deviceEvent);

                        }

                        #endregion
                    }
                    else if (AFN == (byte)WaterBaseProtocol.AFN._4C && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Up)
                    {
                        #region _4C
                        waterInfo += EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                        ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);
                        LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);

                        WaterCmd_4C_2 cmdreceive = null;
                        if (DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETX
                            || DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB && DataBeginChar == (byte)WaterBaseProtocol.DataBeginChar.STX)
                        {
                            cmdreceive = new WaterCmd_4C_2();
                            cmdreceive.MsgList = MsgList;
                            string msgreceive = cmdreceive.ReadMsg();
                            if (msgreceive == "")
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                            }

                            if (ToWaterDtuCommand.GetBaseMessageToDtuByKey(cKey + "-" + AFN) != null)
                            {
                                ToWaterDtuCommand.AddBaseMessageFromDtu(cKey + "-" + AFN, cmdreceive);
                                ShowLogData.add("添加发送命令响应保存！" + cKey + "-" + AFN + "：" + cmdreceive.UserDataAll);
                            }
                            else
                            {
                                ShowLogData.add("无法添加发送命令响应保存！" + cKey + "-" + AFN + "：" + cmdreceive.UserDataAll);
                            }

                            WaterCmd_4C_3 cmdres = new WaterCmd_4C_3();
                            cmdres.CenterStation = message_water.CenterStation;
                            cmdres.RemoteStation = message_water.RemoteStation;
                            cmdres.TotalPackage = message_water.TotalPackage;
                            cmdres.CurrentPackage = message_water.CurrentPackage;
                            cmdres.PW = message_water.PW;
                            if (message_water.DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB)
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.ACK;
                            else
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.EOT;
                            cmdres.SerialNumber = cmdreceive.SerialNumber;
                            cmdres.SendTime = DateTime.Now;

                            string msgres = cmdres.WriteMsg();
                            if (msgres == "")
                            {
                                send(cmdres.RawDataChar, 0, cmdres.RawDataChar.Length);
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                            }

                            MsgList = null;
                            WaterDeviceService.Remove(cKey);
                        }

                        if (userToken.DeviceList != null)
                        {
                            userToken.DeviceList.Online = 1;
                            userToken.DeviceList.LastUpdate = DateTime.Now;
                            userToken.DeviceList.TerminalState = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                            updateDeviceList(userToken.DeviceList);

                            string FullDeviceNo = DeviceModule.GetFullDeviceNoByID(userToken.DeviceList.Id);
                            OnlineDeviceService.AddOnline(FullDeviceNo, userToken.DeviceList);
                            DeviceEvent deviceEvent = new DeviceEvent();
                            DeviceEventModule.InitDeviceEvent(deviceEvent);
                            deviceEvent.DeviceNo = FullDeviceNo;
                            deviceEvent.EventTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.EventType = userToken.DeviceList.TerminalState;
                            deviceEvent.DeviceTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.Remark = cmdreceive == null ? userToken.DeviceList.TerminalState : cmdreceive.ToString();
                            deviceEvent.RawData = message_water.RawDataStr;
                            saveDeviceEvent(deviceEvent);
                            proxySendDeviceList(userToken.DeviceList, deviceEvent);

                        }

                        #endregion
                    }
                    else if (AFN == (byte)WaterBaseProtocol.AFN._4D && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Up)
                    {
                        #region _4D
                        waterInfo += EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                        ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);
                        LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);

                        WaterCmd_4D_2 cmdreceive = null;
                        if (DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETX
                            || DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB && DataBeginChar == (byte)WaterBaseProtocol.DataBeginChar.STX)
                        {
                            cmdreceive = new WaterCmd_4D_2();
                            cmdreceive.MsgList = MsgList;
                            string msgreceive = cmdreceive.ReadMsg();
                            if (msgreceive == "")
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                            }

                            if (ToWaterDtuCommand.GetBaseMessageToDtuByKey(cKey + "-" + AFN) != null)
                            {
                                ToWaterDtuCommand.AddBaseMessageFromDtu(cKey + "-" + AFN, cmdreceive);
                                ShowLogData.add("添加发送命令响应保存！" + cKey + "-" + AFN + "：" + cmdreceive.UserDataAll);
                            }
                            else
                            {
                                ShowLogData.add("无法添加发送命令响应保存！" + cKey + "-" + AFN + "：" + cmdreceive.UserDataAll);
                            }

                            WaterCmd_4D_3 cmdres = new WaterCmd_4D_3();
                            cmdres.CenterStation = message_water.CenterStation;
                            cmdres.RemoteStation = message_water.RemoteStation;
                            cmdres.TotalPackage = message_water.TotalPackage;
                            cmdres.CurrentPackage = message_water.CurrentPackage;
                            cmdres.PW = message_water.PW;
                            if (message_water.DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB)
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.ACK;
                            else
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.EOT;
                            cmdres.SerialNumber = cmdreceive.SerialNumber;
                            cmdres.SendTime = DateTime.Now;

                            string msgres = cmdres.WriteMsg();
                            if (msgres == "")
                            {
                                send(cmdres.RawDataChar, 0, cmdres.RawDataChar.Length);
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                            }

                            MsgList = null;
                            WaterDeviceService.Remove(cKey);
                        }

                        if (userToken.DeviceList != null)
                        {
                            userToken.DeviceList.Online = 1;
                            userToken.DeviceList.LastUpdate = DateTime.Now;
                            userToken.DeviceList.TerminalState = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                            updateDeviceList(userToken.DeviceList);

                            string FullDeviceNo = DeviceModule.GetFullDeviceNoByID(userToken.DeviceList.Id);
                            OnlineDeviceService.AddOnline(FullDeviceNo, userToken.DeviceList);
                            DeviceEvent deviceEvent = new DeviceEvent();
                            DeviceEventModule.InitDeviceEvent(deviceEvent);
                            deviceEvent.DeviceNo = FullDeviceNo;
                            deviceEvent.EventTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.EventType = userToken.DeviceList.TerminalState;
                            deviceEvent.DeviceTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.Remark = cmdreceive == null ? userToken.DeviceList.TerminalState : cmdreceive.ToString();
                            deviceEvent.RawData = message_water.RawDataStr;
                            saveDeviceEvent(deviceEvent);
                            proxySendDeviceList(userToken.DeviceList, deviceEvent);

                        }

                        #endregion
                    }
                    else if (AFN == (byte)WaterBaseProtocol.AFN._4E && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Up)
                    {
                        #region _4E
                        waterInfo += EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                        ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);
                        LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);

                        WaterCmd_4E_2 cmdreceive = null;
                        if (DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETX
                            || DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB && DataBeginChar == (byte)WaterBaseProtocol.DataBeginChar.STX)
                        {
                            cmdreceive = new WaterCmd_4E_2();
                            cmdreceive.MsgList = MsgList;
                            string msgreceive = cmdreceive.ReadMsg();
                            if (msgreceive == "")
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                            }

                            if (ToWaterDtuCommand.GetBaseMessageToDtuByKey(cKey + "-" + AFN) != null)
                            {
                                ToWaterDtuCommand.AddBaseMessageFromDtu(cKey + "-" + AFN, cmdreceive);
                                ShowLogData.add("添加发送命令响应保存！" + cKey + "-" + AFN + "：" + cmdreceive.UserDataAll);
                            }
                            else
                            {
                                ShowLogData.add("无法添加发送命令响应保存！" + cKey + "-" + AFN + "：" + cmdreceive.UserDataAll);
                            }

                            WaterCmd_4E_3 cmdres = new WaterCmd_4E_3();
                            cmdres.CenterStation = message_water.CenterStation;
                            cmdres.RemoteStation = message_water.RemoteStation;
                            cmdres.TotalPackage = message_water.TotalPackage;
                            cmdres.CurrentPackage = message_water.CurrentPackage;
                            cmdres.PW = message_water.PW;
                            if (message_water.DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB)
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.ACK;
                            else
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.EOT;
                            cmdres.SerialNumber = cmdreceive.SerialNumber;
                            cmdres.SendTime = DateTime.Now;

                            string msgres = cmdres.WriteMsg();
                            if (msgres == "")
                            {
                                send(cmdres.RawDataChar, 0, cmdres.RawDataChar.Length);
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                            }

                            MsgList = null;
                            WaterDeviceService.Remove(cKey);
                        }

                        if (userToken.DeviceList != null)
                        {
                            userToken.DeviceList.Online = 1;
                            userToken.DeviceList.LastUpdate = DateTime.Now;
                            userToken.DeviceList.TerminalState = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                            updateDeviceList(userToken.DeviceList);

                            string FullDeviceNo = DeviceModule.GetFullDeviceNoByID(userToken.DeviceList.Id);
                            OnlineDeviceService.AddOnline(FullDeviceNo, userToken.DeviceList);
                            DeviceEvent deviceEvent = new DeviceEvent();
                            DeviceEventModule.InitDeviceEvent(deviceEvent);
                            deviceEvent.DeviceNo = FullDeviceNo;
                            deviceEvent.EventTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.EventType = userToken.DeviceList.TerminalState;
                            deviceEvent.DeviceTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.Remark = cmdreceive == null ? userToken.DeviceList.TerminalState : cmdreceive.ToString();
                            deviceEvent.RawData = message_water.RawDataStr;
                            saveDeviceEvent(deviceEvent);
                            proxySendDeviceList(userToken.DeviceList, deviceEvent);

                        }

                        #endregion
                    }
                    else if (AFN == (byte)WaterBaseProtocol.AFN._4F && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Up)
                    {
                        #region _4F
                        waterInfo += EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                        ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);
                        LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);

                        WaterCmd_4F_2 cmdreceive = null;
                        if (DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETX
                            || DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB && DataBeginChar == (byte)WaterBaseProtocol.DataBeginChar.STX)
                        {
                            cmdreceive = new WaterCmd_4F_2();
                            cmdreceive.MsgList = MsgList;
                            string msgreceive = cmdreceive.ReadMsg();
                            if (msgreceive == "")
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                            }

                            if (ToWaterDtuCommand.GetBaseMessageToDtuByKey(cKey + "-" + AFN) != null)
                            {
                                ToWaterDtuCommand.AddBaseMessageFromDtu(cKey + "-" + AFN, cmdreceive);
                                ShowLogData.add("添加发送命令响应保存！" + cKey + "-" + AFN + "：" + cmdreceive.UserDataAll);
                            }
                            else
                            {
                                ShowLogData.add("无法添加发送命令响应保存！" + cKey + "-" + AFN + "：" + cmdreceive.UserDataAll);
                            }

                            WaterCmd_4F_3 cmdres = new WaterCmd_4F_3();
                            cmdres.CenterStation = message_water.CenterStation;
                            cmdres.RemoteStation = message_water.RemoteStation;
                            cmdres.TotalPackage = message_water.TotalPackage;
                            cmdres.CurrentPackage = message_water.CurrentPackage;
                            cmdres.PW = message_water.PW;
                            if (message_water.DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB)
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.ACK;
                            else
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.EOT;
                            cmdres.SerialNumber = cmdreceive.SerialNumber;
                            cmdres.SendTime = DateTime.Now;

                            string msgres = cmdres.WriteMsg();
                            if (msgres == "")
                            {
                                send(cmdres.RawDataChar, 0, cmdres.RawDataChar.Length);
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                            }

                            MsgList = null;
                            WaterDeviceService.Remove(cKey);
                        }

                        if (userToken.DeviceList != null)
                        {
                            userToken.DeviceList.Online = 1;
                            userToken.DeviceList.LastUpdate = DateTime.Now;
                            userToken.DeviceList.TerminalState = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                            updateDeviceList(userToken.DeviceList);

                            string FullDeviceNo = DeviceModule.GetFullDeviceNoByID(userToken.DeviceList.Id);
                            OnlineDeviceService.AddOnline(FullDeviceNo, userToken.DeviceList);
                            DeviceEvent deviceEvent = new DeviceEvent();
                            DeviceEventModule.InitDeviceEvent(deviceEvent);
                            deviceEvent.DeviceNo = FullDeviceNo;
                            deviceEvent.EventTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.EventType = userToken.DeviceList.TerminalState;
                            deviceEvent.DeviceTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.Remark = cmdreceive == null ? userToken.DeviceList.TerminalState : cmdreceive.ToString();
                            deviceEvent.RawData = message_water.RawDataStr;
                            saveDeviceEvent(deviceEvent);
                            proxySendDeviceList(userToken.DeviceList, deviceEvent);

                        }

                        #endregion
                    }
                    else if (AFN == (byte)WaterBaseProtocol.AFN._50 && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Up)
                    {
                        #region _50
                        waterInfo += EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                        ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);
                        LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);

                        WaterCmd_50_2 cmdreceive = null;
                        if (DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETX
                            || DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB && DataBeginChar == (byte)WaterBaseProtocol.DataBeginChar.STX)
                        {
                            cmdreceive = new WaterCmd_50_2();
                            cmdreceive.MsgList = MsgList;
                            string msgreceive = cmdreceive.ReadMsg();
                            if (msgreceive == "")
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                            }

                            if (ToWaterDtuCommand.GetBaseMessageToDtuByKey(cKey + "-" + AFN) != null)
                            {
                                ToWaterDtuCommand.AddBaseMessageFromDtu(cKey + "-" + AFN, cmdreceive);
                                ShowLogData.add("添加发送命令响应保存！" + cKey + "-" + AFN + "：" + cmdreceive.UserDataAll);
                            }
                            else
                            {
                                ShowLogData.add("无法添加发送命令响应保存！" + cKey + "-" + AFN + "：" + cmdreceive.UserDataAll);
                            }

                            WaterCmd_50_3 cmdres = new WaterCmd_50_3();
                            cmdres.CenterStation = message_water.CenterStation;
                            cmdres.RemoteStation = message_water.RemoteStation;
                            cmdres.TotalPackage = message_water.TotalPackage;
                            cmdres.CurrentPackage = message_water.CurrentPackage;
                            cmdres.PW = message_water.PW;
                            if (message_water.DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB)
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.ACK;
                            else
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.EOT;
                            cmdres.SerialNumber = cmdreceive.SerialNumber;
                            cmdres.SendTime = DateTime.Now;

                            string msgres = cmdres.WriteMsg();
                            if (msgres == "")
                            {
                                send(cmdres.RawDataChar, 0, cmdres.RawDataChar.Length);
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                            }

                            MsgList = null;
                            WaterDeviceService.Remove(cKey);
                        }

                        if (userToken.DeviceList != null)
                        {
                            userToken.DeviceList.Online = 1;
                            userToken.DeviceList.LastUpdate = DateTime.Now;
                            userToken.DeviceList.TerminalState = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                            updateDeviceList(userToken.DeviceList);

                            string FullDeviceNo = DeviceModule.GetFullDeviceNoByID(userToken.DeviceList.Id);
                            OnlineDeviceService.AddOnline(FullDeviceNo, userToken.DeviceList);
                            DeviceEvent deviceEvent = new DeviceEvent();
                            DeviceEventModule.InitDeviceEvent(deviceEvent);
                            deviceEvent.DeviceNo = FullDeviceNo;
                            deviceEvent.EventTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.EventType = userToken.DeviceList.TerminalState;
                            deviceEvent.DeviceTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.Remark = cmdreceive == null ? userToken.DeviceList.TerminalState : cmdreceive.ToString();
                            deviceEvent.RawData = message_water.RawDataStr;
                            saveDeviceEvent(deviceEvent);
                            proxySendDeviceList(userToken.DeviceList, deviceEvent);

                        }

                        #endregion
                    }
                    else if (AFN == (byte)WaterBaseProtocol.AFN._51 && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Up)
                    {
                        #region _51
                        waterInfo += EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                        ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);
                        LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);

                        WaterCmd_51_2 cmdreceive = null;
                        if (DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETX
                            || DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB && DataBeginChar == (byte)WaterBaseProtocol.DataBeginChar.STX)
                        {
                            cmdreceive = new WaterCmd_51_2();
                            cmdreceive.MsgList = MsgList;
                            string msgreceive = cmdreceive.ReadMsg();
                            if (msgreceive == "")
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                            }

                            if (ToWaterDtuCommand.GetBaseMessageToDtuByKey(cKey + "-" + AFN) != null)
                            {
                                ToWaterDtuCommand.AddBaseMessageFromDtu(cKey + "-" + AFN, cmdreceive);
                                ShowLogData.add("添加发送命令响应保存！" + cKey + "-" + AFN + "：" + cmdreceive.UserDataAll);
                            }
                            else
                            {
                                ShowLogData.add("无法添加发送命令响应保存！" + cKey + "-" + AFN + "：" + cmdreceive.UserDataAll);
                            }

                            WaterCmd_51_3 cmdres = new WaterCmd_51_3();
                            cmdres.CenterStation = message_water.CenterStation;
                            cmdres.RemoteStation = message_water.RemoteStation;
                            cmdres.TotalPackage = message_water.TotalPackage;
                            cmdres.CurrentPackage = message_water.CurrentPackage;
                            cmdres.PW = message_water.PW;
                            if (message_water.DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB)
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.ACK;
                            else
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.EOT;
                            cmdres.SerialNumber = cmdreceive.SerialNumber;
                            cmdres.SendTime = DateTime.Now;

                            string msgres = cmdres.WriteMsg();
                            if (msgres == "")
                            {
                                send(cmdres.RawDataChar, 0, cmdres.RawDataChar.Length);
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                            }

                            MsgList = null;
                            WaterDeviceService.Remove(cKey);
                        }

                        if (userToken.DeviceList != null)
                        {
                            userToken.DeviceList.Online = 1;
                            userToken.DeviceList.LastUpdate = DateTime.Now;
                            userToken.DeviceList.TerminalState = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                            updateDeviceList(userToken.DeviceList);

                            string FullDeviceNo = DeviceModule.GetFullDeviceNoByID(userToken.DeviceList.Id);
                            OnlineDeviceService.AddOnline(FullDeviceNo, userToken.DeviceList);
                            DeviceEvent deviceEvent = new DeviceEvent();
                            DeviceEventModule.InitDeviceEvent(deviceEvent);
                            deviceEvent.DeviceNo = FullDeviceNo;
                            deviceEvent.EventTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.EventType = userToken.DeviceList.TerminalState;
                            deviceEvent.DeviceTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.Remark = cmdreceive == null ? userToken.DeviceList.TerminalState : cmdreceive.ToString();
                            deviceEvent.RawData = message_water.RawDataStr;
                            saveDeviceEvent(deviceEvent);
                            proxySendDeviceList(userToken.DeviceList, deviceEvent);

                        }

                        #endregion
                    }
                    else if (AFN == (byte)WaterBaseProtocol.AFN._20 && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Up)
                    {
                        #region _20
                        waterInfo += EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                        ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);
                        LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);

                        WaterCmd_20_2 cmdreceive = null;
                        if (DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETX
                            || DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB && DataBeginChar == (byte)WaterBaseProtocol.DataBeginChar.STX)
                        {
                            cmdreceive = new WaterCmd_20_2();
                            cmdreceive.MsgList = MsgList;
                            string msgreceive = cmdreceive.ReadMsg();
                            if (msgreceive == "")
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                            }

                            if (ToWaterDtuCommand.GetBaseMessageToDtuByKey(cKey + "-" + AFN) != null)
                            {
                                ToWaterDtuCommand.AddBaseMessageFromDtu(cKey + "-" + AFN, cmdreceive);
                                ShowLogData.add("添加发送命令响应保存！" + cKey + "-" + AFN + "：" + cmdreceive.UserDataAll);
                            }
                            else
                            {
                                ShowLogData.add("无法添加发送命令响应保存！" + cKey + "-" + AFN + "：" + cmdreceive.UserDataAll);
                            }

                            WaterCmd_20_3 cmdres = new WaterCmd_20_3();
                            cmdres.CenterStation = message_water.CenterStation;
                            cmdres.RemoteStation = message_water.RemoteStation;
                            cmdres.TotalPackage = message_water.TotalPackage;
                            cmdres.CurrentPackage = message_water.CurrentPackage;
                            cmdres.PW = message_water.PW;
                            if (message_water.DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB)
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.ACK;
                            else
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.EOT;
                            cmdres.SerialNumber = cmdreceive.SerialNumber;
                            cmdres.SendTime = DateTime.Now;

                            string msgres = cmdres.WriteMsg();
                            if (msgres == "")
                            {
                                send(cmdres.RawDataChar, 0, cmdres.RawDataChar.Length);
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                            }

                            MsgList = null;
                            WaterDeviceService.Remove(cKey);
                        }

                        if (userToken.DeviceList != null)
                        {
                            userToken.DeviceList.Online = 1;
                            userToken.DeviceList.LastUpdate = DateTime.Now;
                            userToken.DeviceList.TerminalState = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                            updateDeviceList(userToken.DeviceList);

                            string FullDeviceNo = DeviceModule.GetFullDeviceNoByID(userToken.DeviceList.Id);
                            OnlineDeviceService.AddOnline(FullDeviceNo, userToken.DeviceList);
                            DeviceEvent deviceEvent = new DeviceEvent();
                            DeviceEventModule.InitDeviceEvent(deviceEvent);
                            deviceEvent.DeviceNo = FullDeviceNo;
                            deviceEvent.EventTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.EventType = userToken.DeviceList.TerminalState;
                            deviceEvent.DeviceTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.Remark = cmdreceive == null ? userToken.DeviceList.TerminalState : cmdreceive.ToString();
                            deviceEvent.RawData = message_water.RawDataStr;
                            saveDeviceEvent(deviceEvent);
                            proxySendDeviceList(userToken.DeviceList, deviceEvent);

                        }

                        #endregion
                    }
                    else if (AFN == (byte)WaterBaseProtocol.AFN._21 && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Up)
                    {
                        #region _21
                        waterInfo += EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                        ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);
                        LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);

                        WaterCmd_21_2 cmdreceive = null;
                        if (DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETX
                            || DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB && DataBeginChar == (byte)WaterBaseProtocol.DataBeginChar.STX)
                        {
                            cmdreceive = new WaterCmd_21_2();
                            cmdreceive.MsgList = MsgList;
                            string msgreceive = cmdreceive.ReadMsg();
                            if (msgreceive == "")
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                            }

                            if (ToWaterDtuCommand.GetBaseMessageToDtuByKey(cKey + "-" + AFN) != null)
                            {
                                ToWaterDtuCommand.AddBaseMessageFromDtu(cKey + "-" + AFN, cmdreceive);
                                ShowLogData.add("添加发送命令响应保存！" + cKey + "-" + AFN + "：" + cmdreceive.UserDataAll);
                            }
                            else
                            {
                                ShowLogData.add("无法添加发送命令响应保存！" + cKey + "-" + AFN + "：" + cmdreceive.UserDataAll);
                            }

                            WaterCmd_21_3 cmdres = new WaterCmd_21_3();
                            cmdres.CenterStation = message_water.CenterStation;
                            cmdres.RemoteStation = message_water.RemoteStation;
                            cmdres.TotalPackage = message_water.TotalPackage;
                            cmdres.CurrentPackage = message_water.CurrentPackage;
                            cmdres.PW = message_water.PW;
                            if (message_water.DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB)
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.ACK;
                            else
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.EOT;
                            cmdres.SerialNumber = cmdreceive.SerialNumber;
                            cmdres.SendTime = DateTime.Now;

                            string msgres = cmdres.WriteMsg();
                            if (msgres == "")
                            {
                                send(cmdres.RawDataChar, 0, cmdres.RawDataChar.Length);
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                            }

                            MsgList = null;
                            WaterDeviceService.Remove(cKey);
                        }

                        if (userToken.DeviceList != null)
                        {
                            userToken.DeviceList.Online = 1;
                            userToken.DeviceList.LastUpdate = DateTime.Now;
                            userToken.DeviceList.TerminalState = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                            updateDeviceList(userToken.DeviceList);

                            string FullDeviceNo = DeviceModule.GetFullDeviceNoByID(userToken.DeviceList.Id);
                            OnlineDeviceService.AddOnline(FullDeviceNo, userToken.DeviceList);
                            DeviceEvent deviceEvent = new DeviceEvent();
                            DeviceEventModule.InitDeviceEvent(deviceEvent);
                            deviceEvent.DeviceNo = FullDeviceNo;
                            deviceEvent.EventTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.EventType = userToken.DeviceList.TerminalState;
                            deviceEvent.DeviceTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.Remark = cmdreceive == null ? userToken.DeviceList.TerminalState : cmdreceive.ToString();
                            deviceEvent.RawData = message_water.RawDataStr;
                            saveDeviceEvent(deviceEvent);
                            proxySendDeviceList(userToken.DeviceList, deviceEvent);

                        }

                        #endregion
                    }
                    else if (AFN == (byte)WaterBaseProtocol.AFN._22 && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Up)
                    {
                        #region _22
                        waterInfo += EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                        ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);
                        LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);

                        WaterCmd_22_2 cmdreceive = null;
                        if (DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETX
                            || DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB && DataBeginChar == (byte)WaterBaseProtocol.DataBeginChar.STX)
                        {
                            cmdreceive = new WaterCmd_22_2();
                            cmdreceive.MsgList = MsgList;
                            string msgreceive = cmdreceive.ReadMsg();
                            if (msgreceive == "")
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                            }

                            if (ToWaterDtuCommand.GetBaseMessageToDtuByKey(cKey + "-" + AFN) != null)
                            {
                                ToWaterDtuCommand.AddBaseMessageFromDtu(cKey + "-" + AFN, cmdreceive);
                                ShowLogData.add("添加发送命令响应保存！" + cKey + "-" + AFN + "：" + cmdreceive.UserDataAll);
                            }
                            else
                            {
                                ShowLogData.add("无法添加发送命令响应保存！" + cKey + "-" + AFN + "：" + cmdreceive.UserDataAll);
                            }

                            WaterCmd_22_3 cmdres = new WaterCmd_22_3();
                            cmdres.CenterStation = message_water.CenterStation;
                            cmdres.RemoteStation = message_water.RemoteStation;
                            cmdres.TotalPackage = message_water.TotalPackage;
                            cmdres.CurrentPackage = message_water.CurrentPackage;
                            cmdres.PW = message_water.PW;
                            if (message_water.DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB)
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.ACK;
                            else
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.EOT;
                            cmdres.SerialNumber = cmdreceive.SerialNumber;
                            cmdres.SendTime = DateTime.Now;

                            string msgres = cmdres.WriteMsg();
                            if (msgres == "")
                            {
                                send(cmdres.RawDataChar, 0, cmdres.RawDataChar.Length);
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                            }

                            MsgList = null;
                            WaterDeviceService.Remove(cKey);
                        }

                        if (userToken.DeviceList != null)
                        {
                            userToken.DeviceList.Online = 1;
                            userToken.DeviceList.LastUpdate = DateTime.Now;
                            userToken.DeviceList.TerminalState = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                            updateDeviceList(userToken.DeviceList);

                            string FullDeviceNo = DeviceModule.GetFullDeviceNoByID(userToken.DeviceList.Id);
                            OnlineDeviceService.AddOnline(FullDeviceNo, userToken.DeviceList);
                            DeviceEvent deviceEvent = new DeviceEvent();
                            DeviceEventModule.InitDeviceEvent(deviceEvent);
                            deviceEvent.DeviceNo = FullDeviceNo;
                            deviceEvent.EventTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.EventType = userToken.DeviceList.TerminalState;
                            deviceEvent.DeviceTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.Remark = cmdreceive == null ? userToken.DeviceList.TerminalState : cmdreceive.ToString();
                            deviceEvent.RawData = message_water.RawDataStr;
                            saveDeviceEvent(deviceEvent);
                            proxySendDeviceList(userToken.DeviceList, deviceEvent);

                        }

                        #endregion
                    }
                    else if (AFN == (byte)WaterBaseProtocol.AFN._23 && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Up)
                    {
                        #region _23
                        waterInfo += EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                        ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);
                        LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);

                        WaterCmd_23_2 cmdreceive = null;
                        if (DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETX
                            || DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB && DataBeginChar == (byte)WaterBaseProtocol.DataBeginChar.STX)
                        {
                            cmdreceive = new WaterCmd_23_2();
                            cmdreceive.MsgList = MsgList;
                            string msgreceive = cmdreceive.ReadMsg();
                            if (msgreceive == "")
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                            }

                            if (ToWaterDtuCommand.GetBaseMessageToDtuByKey(cKey + "-" + AFN) != null)
                            {
                                ToWaterDtuCommand.AddBaseMessageFromDtu(cKey + "-" + AFN, cmdreceive);
                                ShowLogData.add("添加发送命令响应保存！" + cKey + "-" + AFN + "：" + cmdreceive.UserDataAll);
                            }
                            else
                            {
                                ShowLogData.add("无法添加发送命令响应保存！" + cKey + "-" + AFN + "：" + cmdreceive.UserDataAll);
                            }

                            WaterCmd_23_3 cmdres = new WaterCmd_23_3();
                            cmdres.CenterStation = message_water.CenterStation;
                            cmdres.RemoteStation = message_water.RemoteStation;
                            cmdres.TotalPackage = message_water.TotalPackage;
                            cmdres.CurrentPackage = message_water.CurrentPackage;
                            cmdres.PW = message_water.PW;
                            if (message_water.DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB)
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.ACK;
                            else
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.EOT;
                            cmdres.SerialNumber = cmdreceive.SerialNumber;
                            cmdres.SendTime = DateTime.Now;

                            string msgres = cmdres.WriteMsg();
                            if (msgres == "")
                            {
                                send(cmdres.RawDataChar, 0, cmdres.RawDataChar.Length);
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                            }

                            MsgList = null;
                            WaterDeviceService.Remove(cKey);
                        }

                        if (userToken.DeviceList != null)
                        {
                            userToken.DeviceList.Online = 1;
                            userToken.DeviceList.LastUpdate = DateTime.Now;
                            userToken.DeviceList.TerminalState = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                            updateDeviceList(userToken.DeviceList);

                            string FullDeviceNo = DeviceModule.GetFullDeviceNoByID(userToken.DeviceList.Id);
                            OnlineDeviceService.AddOnline(FullDeviceNo, userToken.DeviceList);
                            DeviceEvent deviceEvent = new DeviceEvent();
                            DeviceEventModule.InitDeviceEvent(deviceEvent);
                            deviceEvent.DeviceNo = FullDeviceNo;
                            deviceEvent.EventTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.EventType = userToken.DeviceList.TerminalState;
                            deviceEvent.DeviceTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.Remark = cmdreceive == null ? userToken.DeviceList.TerminalState : cmdreceive.ToString();
                            deviceEvent.RawData = message_water.RawDataStr;
                            saveDeviceEvent(deviceEvent);
                            proxySendDeviceList(userToken.DeviceList, deviceEvent);

                        }

                        #endregion
                    }
                    else if (AFN == (byte)WaterBaseProtocol.AFN._24 && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Up)
                    {
                        #region _24
                        waterInfo += EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                        ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);
                        LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);

                        WaterCmd_24_2 cmdreceive = null;
                        if (DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETX
                            || DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB && DataBeginChar == (byte)WaterBaseProtocol.DataBeginChar.STX)
                        {
                            cmdreceive = new WaterCmd_24_2();
                            cmdreceive.MsgList = MsgList;
                            string msgreceive = cmdreceive.ReadMsg();
                            if (msgreceive == "")
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                            }

                            if (ToWaterDtuCommand.GetBaseMessageToDtuByKey(cKey + "-" + AFN) != null)
                            {
                                ToWaterDtuCommand.AddBaseMessageFromDtu(cKey + "-" + AFN, cmdreceive);
                                ShowLogData.add("添加发送命令响应保存！" + cKey + "-" + AFN + "：" + cmdreceive.UserDataAll);
                            }
                            else
                            {
                                ShowLogData.add("无法添加发送命令响应保存！" + cKey + "-" + AFN + "：" + cmdreceive.UserDataAll);
                            }

                            WaterCmd_24_3 cmdres = new WaterCmd_24_3();
                            cmdres.CenterStation = message_water.CenterStation;
                            cmdres.RemoteStation = message_water.RemoteStation;
                            cmdres.TotalPackage = message_water.TotalPackage;
                            cmdres.CurrentPackage = message_water.CurrentPackage;
                            cmdres.PW = message_water.PW;
                            if (message_water.DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB)
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.ACK;
                            else
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.EOT;
                            cmdres.SerialNumber = cmdreceive.SerialNumber;
                            cmdres.SendTime = DateTime.Now;

                            string msgres = cmdres.WriteMsg();
                            if (msgres == "")
                            {
                                send(cmdres.RawDataChar, 0, cmdres.RawDataChar.Length);
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                            }

                            MsgList = null;
                            WaterDeviceService.Remove(cKey);
                        }

                        if (userToken.DeviceList != null)
                        {
                            userToken.DeviceList.Online = 1;
                            userToken.DeviceList.LastUpdate = DateTime.Now;
                            userToken.DeviceList.TerminalState = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                            updateDeviceList(userToken.DeviceList);

                            string FullDeviceNo = DeviceModule.GetFullDeviceNoByID(userToken.DeviceList.Id);
                            OnlineDeviceService.AddOnline(FullDeviceNo, userToken.DeviceList);
                            DeviceEvent deviceEvent = new DeviceEvent();
                            DeviceEventModule.InitDeviceEvent(deviceEvent);
                            deviceEvent.DeviceNo = FullDeviceNo;
                            deviceEvent.EventTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.EventType = userToken.DeviceList.TerminalState;
                            deviceEvent.DeviceTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.Remark = cmdreceive == null ? userToken.DeviceList.TerminalState : cmdreceive.ToString();
                            deviceEvent.RawData = message_water.RawDataStr;
                            saveDeviceEvent(deviceEvent);
                            proxySendDeviceList(userToken.DeviceList, deviceEvent);

                        }

                        #endregion
                    }
                    else if (AFN == (byte)WaterBaseProtocol.AFN._25 && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Up)
                    {
                        #region _25
                        waterInfo += EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                        ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);
                        LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);

                        WaterCmd_25_2 cmdreceive = null;
                        if (DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETX
                            || DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB && DataBeginChar == (byte)WaterBaseProtocol.DataBeginChar.STX)
                        {
                            cmdreceive = new WaterCmd_25_2();
                            cmdreceive.MsgList = MsgList;
                            string msgreceive = cmdreceive.ReadMsg();
                            if (msgreceive == "")
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                            }

                            if (ToWaterDtuCommand.GetBaseMessageToDtuByKey(cKey + "-" + AFN) != null)
                            {
                                ToWaterDtuCommand.AddBaseMessageFromDtu(cKey + "-" + AFN, cmdreceive);
                                ShowLogData.add("添加发送命令响应保存！" + cKey + "-" + AFN + "：" + cmdreceive.UserDataAll);
                            }
                            else
                            {
                                ShowLogData.add("无法添加发送命令响应保存！" + cKey + "-" + AFN + "：" + cmdreceive.UserDataAll);
                            }

                            WaterCmd_25_3 cmdres = new WaterCmd_25_3();
                            cmdres.CenterStation = message_water.CenterStation;
                            cmdres.RemoteStation = message_water.RemoteStation;
                            cmdres.TotalPackage = message_water.TotalPackage;
                            cmdres.CurrentPackage = message_water.CurrentPackage;
                            cmdres.PW = message_water.PW;
                            if (message_water.DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB)
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.ACK;
                            else
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.EOT;
                            cmdres.SerialNumber = cmdreceive.SerialNumber;
                            cmdres.SendTime = DateTime.Now;

                            string msgres = cmdres.WriteMsg();
                            if (msgres == "")
                            {
                                send(cmdres.RawDataChar, 0, cmdres.RawDataChar.Length);
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                            }

                            MsgList = null;
                            WaterDeviceService.Remove(cKey);
                        }

                        if (userToken.DeviceList != null)
                        {
                            userToken.DeviceList.Online = 1;
                            userToken.DeviceList.LastUpdate = DateTime.Now;
                            userToken.DeviceList.TerminalState = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                            updateDeviceList(userToken.DeviceList);

                            string FullDeviceNo = DeviceModule.GetFullDeviceNoByID(userToken.DeviceList.Id);
                            OnlineDeviceService.AddOnline(FullDeviceNo, userToken.DeviceList);
                            DeviceEvent deviceEvent = new DeviceEvent();
                            DeviceEventModule.InitDeviceEvent(deviceEvent);
                            deviceEvent.DeviceNo = FullDeviceNo;
                            deviceEvent.EventTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.EventType = userToken.DeviceList.TerminalState;
                            deviceEvent.DeviceTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.Remark = cmdreceive == null ? userToken.DeviceList.TerminalState : cmdreceive.ToString();
                            deviceEvent.RawData = message_water.RawDataStr;
                            saveDeviceEvent(deviceEvent);
                            proxySendDeviceList(userToken.DeviceList, deviceEvent);

                        }

                        #endregion
                    }
                    else if (AFN == (byte)WaterBaseProtocol.AFN._26 && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Up)
                    {
                        #region _26
                        waterInfo += EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                        ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);
                        LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);

                        WaterCmd_26_2 cmdreceive = null;
                        if (DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETX
                            || DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB && DataBeginChar == (byte)WaterBaseProtocol.DataBeginChar.STX)
                        {
                            cmdreceive = new WaterCmd_26_2();
                            cmdreceive.MsgList = MsgList;
                            string msgreceive = cmdreceive.ReadMsg();
                            if (msgreceive == "")
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                            }

                            if (ToWaterDtuCommand.GetBaseMessageToDtuByKey(cKey + "-" + AFN) != null)
                            {
                                ToWaterDtuCommand.AddBaseMessageFromDtu(cKey + "-" + AFN, cmdreceive);
                                ShowLogData.add("添加发送命令响应保存！" + cKey + "-" + AFN + "：" + cmdreceive.UserDataAll);
                            }
                            else
                            {
                                ShowLogData.add("无法添加发送命令响应保存！" + cKey + "-" + AFN + "：" + cmdreceive.UserDataAll);
                            }

                            WaterCmd_26_3 cmdres = new WaterCmd_26_3();
                            cmdres.CenterStation = message_water.CenterStation;
                            cmdres.RemoteStation = message_water.RemoteStation;
                            cmdres.TotalPackage = message_water.TotalPackage;
                            cmdres.CurrentPackage = message_water.CurrentPackage;
                            cmdres.PW = message_water.PW;
                            if (message_water.DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB)
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.ACK;
                            else
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.EOT;
                            cmdres.SerialNumber = cmdreceive.SerialNumber;
                            cmdres.SendTime = DateTime.Now;

                            string msgres = cmdres.WriteMsg();
                            if (msgres == "")
                            {
                                send(cmdres.RawDataChar, 0, cmdres.RawDataChar.Length);
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                            }

                            MsgList = null;
                            WaterDeviceService.Remove(cKey);
                        }

                        if (userToken.DeviceList != null)
                        {
                            userToken.DeviceList.Online = 1;
                            userToken.DeviceList.LastUpdate = DateTime.Now;
                            userToken.DeviceList.TerminalState = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                            updateDeviceList(userToken.DeviceList);

                            string FullDeviceNo = DeviceModule.GetFullDeviceNoByID(userToken.DeviceList.Id);
                            OnlineDeviceService.AddOnline(FullDeviceNo, userToken.DeviceList);
                            DeviceEvent deviceEvent = new DeviceEvent();
                            DeviceEventModule.InitDeviceEvent(deviceEvent);
                            deviceEvent.DeviceNo = FullDeviceNo;
                            deviceEvent.EventTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.EventType = userToken.DeviceList.TerminalState;
                            deviceEvent.DeviceTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.Remark = cmdreceive == null ? userToken.DeviceList.TerminalState : cmdreceive.ToString();
                            deviceEvent.RawData = message_water.RawDataStr;
                            saveDeviceEvent(deviceEvent);
                            proxySendDeviceList(userToken.DeviceList, deviceEvent);

                        }

                        #endregion
                    }
                    else if (AFN == (byte)WaterBaseProtocol.AFN._27 && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Up)
                    {
                        #region _27
                        waterInfo += EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                        ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);
                        LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);

                        WaterCmd_27_2 cmdreceive = null;
                        if (DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETX
                            || DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB && DataBeginChar == (byte)WaterBaseProtocol.DataBeginChar.STX)
                        {
                            cmdreceive = new WaterCmd_27_2();
                            cmdreceive.MsgList = MsgList;
                            string msgreceive = cmdreceive.ReadMsg();
                            if (msgreceive == "")
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                            }

                            if (ToWaterDtuCommand.GetBaseMessageToDtuByKey(cKey + "-" + AFN) != null)
                            {
                                ToWaterDtuCommand.AddBaseMessageFromDtu(cKey + "-" + AFN, cmdreceive);
                                ShowLogData.add("添加发送命令响应保存！" + cKey + "-" + AFN + "：" + cmdreceive.UserDataAll);
                            }
                            else
                            {
                                ShowLogData.add("无法添加发送命令响应保存！" + cKey + "-" + AFN + "：" + cmdreceive.UserDataAll);
                            }

                            WaterCmd_27_3 cmdres = new WaterCmd_27_3();
                            cmdres.CenterStation = message_water.CenterStation;
                            cmdres.RemoteStation = message_water.RemoteStation;
                            cmdres.TotalPackage = message_water.TotalPackage;
                            cmdres.CurrentPackage = message_water.CurrentPackage;
                            cmdres.PW = message_water.PW;
                            if (message_water.DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB)
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.ACK;
                            else
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.EOT;
                            cmdres.SerialNumber = cmdreceive.SerialNumber;
                            cmdres.SendTime = DateTime.Now;

                            string msgres = cmdres.WriteMsg();
                            if (msgres == "")
                            {
                                send(cmdres.RawDataChar, 0, cmdres.RawDataChar.Length);
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                            }

                            MsgList = null;
                            WaterDeviceService.Remove(cKey);
                        }

                        if (userToken.DeviceList != null)
                        {
                            userToken.DeviceList.Online = 1;
                            userToken.DeviceList.LastUpdate = DateTime.Now;
                            userToken.DeviceList.TerminalState = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                            updateDeviceList(userToken.DeviceList);

                            string FullDeviceNo = DeviceModule.GetFullDeviceNoByID(userToken.DeviceList.Id);
                            OnlineDeviceService.AddOnline(FullDeviceNo, userToken.DeviceList);
                            DeviceEvent deviceEvent = new DeviceEvent();
                            DeviceEventModule.InitDeviceEvent(deviceEvent);
                            deviceEvent.DeviceNo = FullDeviceNo;
                            deviceEvent.EventTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.EventType = userToken.DeviceList.TerminalState;
                            deviceEvent.DeviceTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.Remark = cmdreceive == null ? userToken.DeviceList.TerminalState : cmdreceive.ToString();
                            deviceEvent.RawData = message_water.RawDataStr;
                            saveDeviceEvent(deviceEvent);
                            proxySendDeviceList(userToken.DeviceList, deviceEvent);

                        }

                        #endregion
                    }
                    //kqz 2017-1-1添加
                    else if (AFN == (byte)WaterBaseProtocol.AFN._28 && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Up)
                    {
                        #region _28
                        waterInfo += EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                        ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);
                        LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);

                        WaterCmd_28_2 cmdreceive = null;
                        if (DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETX
                            || DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB && DataBeginChar == (byte)WaterBaseProtocol.DataBeginChar.STX)
                        {
                            cmdreceive = new WaterCmd_28_2();
                            cmdreceive.MsgList = MsgList;
                            string msgreceive = cmdreceive.ReadMsg();
                            if (msgreceive == "")
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                            }

                            if (ToWaterDtuCommand.GetBaseMessageToDtuByKey(cKey + "-" + AFN) != null)
                            {
                                ToWaterDtuCommand.AddBaseMessageFromDtu(cKey + "-" + AFN, cmdreceive);
                                ShowLogData.add("添加发送命令响应保存！" + cKey + "-" + AFN + "：" + cmdreceive.UserDataAll);
                            }
                            else
                            {
                                ShowLogData.add("无法添加发送命令响应保存！" + cKey + "-" + AFN + "：" + cmdreceive.UserDataAll);
                            }

                            WaterCmd_28_3 cmdres = new WaterCmd_28_3();
                            cmdres.CenterStation = message_water.CenterStation;
                            cmdres.RemoteStation = message_water.RemoteStation;
                            cmdres.TotalPackage = message_water.TotalPackage;
                            cmdres.CurrentPackage = message_water.CurrentPackage;
                            cmdres.PW = message_water.PW;
                            if (message_water.DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB)
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.ACK;
                            else
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.EOT;
                            cmdres.SerialNumber = cmdreceive.SerialNumber;
                            cmdres.SendTime = DateTime.Now;

                            string msgres = cmdres.WriteMsg();
                            if (msgres == "")
                            {
                                send(cmdres.RawDataChar, 0, cmdres.RawDataChar.Length);
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                            }

                            MsgList = null;
                            WaterDeviceService.Remove(cKey);
                        }

                        if (userToken.DeviceList != null)
                        {
                            userToken.DeviceList.Online = 1;
                            userToken.DeviceList.LastUpdate = DateTime.Now;
                            userToken.DeviceList.TerminalState = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                            updateDeviceList(userToken.DeviceList);

                            string FullDeviceNo = DeviceModule.GetFullDeviceNoByID(userToken.DeviceList.Id);
                            OnlineDeviceService.AddOnline(FullDeviceNo, userToken.DeviceList);
                            DeviceEvent deviceEvent = new DeviceEvent();
                            DeviceEventModule.InitDeviceEvent(deviceEvent);
                            deviceEvent.DeviceNo = FullDeviceNo;
                            deviceEvent.EventTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.EventType = userToken.DeviceList.TerminalState;
                            deviceEvent.DeviceTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.Remark = cmdreceive == null ? userToken.DeviceList.TerminalState : cmdreceive.ToString();
                            deviceEvent.RawData = message_water.RawDataStr;
                            saveDeviceEvent(deviceEvent);
                            proxySendDeviceList(userToken.DeviceList, deviceEvent);

                        }

                        #endregion
                    }

                    else if (AFN == (byte)WaterBaseProtocol.AFN._29 && UpOrDown == (int)WaterBaseProtocol.UpOrDown.Up)
                    {
                        #region _29
                        waterInfo += EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                        ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);
                        LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收：" + waterInfo);

                        WaterCmd_29_2 cmdreceive = null;
                        if (DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETX
                            || DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB && DataBeginChar == (byte)WaterBaseProtocol.DataBeginChar.STX)
                        {
                            cmdreceive = new WaterCmd_29_2();
                            cmdreceive.MsgList = MsgList;
                            string msgreceive = cmdreceive.ReadMsg();
                            if (msgreceive == "")
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收分析：" + cmdreceive.ToString());
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]接收出错：" + msgreceive);
                            }

                            if (ToWaterDtuCommand.GetBaseMessageToDtuByKey(cKey + "-" + AFN) != null)
                            {
                                ToWaterDtuCommand.AddBaseMessageFromDtu(cKey + "-" + AFN, cmdreceive);
                                ShowLogData.add("添加发送命令响应保存！" + cKey + "-" + AFN + "：" + cmdreceive.UserDataAll);
                            }
                            else
                            {
                                ShowLogData.add("无法添加发送命令响应保存！" + cKey + "-" + AFN + "：" + cmdreceive.UserDataAll);
                            }

                            WaterCmd_29_3 cmdres = new WaterCmd_29_3();
                            cmdres.CenterStation = message_water.CenterStation;
                            cmdres.RemoteStation = message_water.RemoteStation;
                            cmdres.TotalPackage = message_water.TotalPackage;
                            cmdres.CurrentPackage = message_water.CurrentPackage;
                            cmdres.PW = message_water.PW;
                            if (message_water.DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB)
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.ACK;
                            else
                                cmdres.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.EOT;
                            cmdres.SerialNumber = cmdreceive.SerialNumber;
                            cmdres.SendTime = DateTime.Now;

                            string msgres = cmdres.WriteMsg();
                            if (msgres == "")
                            {
                                send(cmdres.RawDataChar, 0, cmdres.RawDataChar.Length);
                            }
                            else
                            {
                                ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                                LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]发送失败：" + msgres);
                            }

                            MsgList = null;
                            WaterDeviceService.Remove(cKey);
                        }

                        if (userToken.DeviceList != null)
                        {
                            userToken.DeviceList.Online = 1;
                            userToken.DeviceList.LastUpdate = DateTime.Now;
                            userToken.DeviceList.TerminalState = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN);
                            updateDeviceList(userToken.DeviceList);

                            string FullDeviceNo = DeviceModule.GetFullDeviceNoByID(userToken.DeviceList.Id);
                            OnlineDeviceService.AddOnline(FullDeviceNo, userToken.DeviceList);
                            DeviceEvent deviceEvent = new DeviceEvent();
                            DeviceEventModule.InitDeviceEvent(deviceEvent);
                            deviceEvent.DeviceNo = FullDeviceNo;
                            deviceEvent.EventTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.EventType = userToken.DeviceList.TerminalState;
                            deviceEvent.DeviceTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.Remark = cmdreceive == null ? userToken.DeviceList.TerminalState : cmdreceive.ToString();
                            deviceEvent.RawData = message_water.RawDataStr;
                            saveDeviceEvent(deviceEvent);
                            proxySendDeviceList(userToken.DeviceList, deviceEvent);

                        }

                        #endregion
                    }

                    //kqz 2017-1-1 添加
                    return true;
                }
                else
                {
                    if (message_water.IsWaterMessage)
                    {
                        ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]分析出错：信息为水文监测数据，停止！");
                        LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]分析出错：信息为水文监测数据，停止！");

                        if (userToken.DeviceList != null)
                        {
                            userToken.DeviceList.Online = 1;
                            userToken.DeviceList.LastUpdate = DateTime.Now;
                            userToken.DeviceList.TerminalState = "未知";
                            updateDeviceList(userToken.DeviceList);

                            string FullDeviceNo = DeviceModule.GetFullDeviceNoByID(userToken.DeviceList.Id);
                            OnlineDeviceService.AddOnline(FullDeviceNo, userToken.DeviceList);
                            DeviceEvent deviceEvent = new DeviceEvent();
                            DeviceEventModule.InitDeviceEvent(deviceEvent);
                            deviceEvent.DeviceNo = FullDeviceNo;
                            deviceEvent.EventTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.EventType = userToken.DeviceList.TerminalState;
                            deviceEvent.DeviceTime = userToken.DeviceList.LastUpdate;
                            deviceEvent.Remark = "错误数据";
                            deviceEvent.RawData = message_water.RawDataStr;
                            saveDeviceEvent(deviceEvent);
                            proxySendDeviceList(userToken.DeviceList, deviceEvent);

                        }

                        return true;
                    }
                }
            }
            catch
            {
            }
            #endregion

            return true;
        }

    }
}
