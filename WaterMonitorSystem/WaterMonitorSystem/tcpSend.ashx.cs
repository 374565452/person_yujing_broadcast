using Common;
using DTU.GateWay.Protocol;
using Maticsoft.Model;
using Module;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using WaterMonitorSystem.Src;

namespace WaterMonitorSystem
{
    /// <summary>
    /// tcpSend 的摘要说明
    /// </summary>
    public class tcpSend : IHttpHandler
    {
        static log4net.ILog myLogger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            //context.Response.Write("Hello World");
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;

            JavaScriptObject result = new JavaScriptObject();
            result.Add("Result", false);
            result.Add("Message", "");

            try
            {
                string method = request["Method"] ?? "";
                switch (method)
                {
                    case "send": result = send(request); break;
                    case "getDeviceNo": result = getDeviceNo(request); break;
                    case "getAllDeviceNo": result = getAllDeviceNo(request); break;
                    default:
                        result["Message"] = "缺少参数！";
                        break;
                }
            }
            catch (Exception ex)
            {
                result["Message"] = "错误！" + ex.Message;
            }

            string str = JavaScriptConvert.SerializeObject(result);
            response.Write(str);
            response.End();
        }

        private JavaScriptObject send(HttpRequest request)
        {
            string DeviceNo = request["DeviceNo"] ?? "";
            string k = request["k"] ?? "";
            string content = request["content"] ?? "";
            string DeviceTime = request["DeviceTime"] ?? "";
            string YearExploitation = request["YearExploitation"] ?? "";
            string SerialNumber = request["SerialNumber"] ?? "";
            string Range = request["Range"] ?? "";
            string LineLength = request["LineLength"] ?? "";

            JavaScriptObject result = new JavaScriptObject();
            result.Add("Result", false);
            result.Add("Message", "");

            if (DeviceNo.Length != 15)
            {
                result["Message"] = "设备编号格式不对！";
                return result;
            }

            Device device = DeviceModule.GetDeviceByFullDeviceNo(DeviceNo);
            if (device == null)
            {
                result["Message"] = "设备编号不存在！" + DeviceNo;
                return result;
            }

            try
            {
                bool waitRsp = false;

                TcpCommunication tcpService = new TcpCommunication();

                int timeDelay = 0;
                //待socket准备好
                while (timeDelay < tcpService.TcpWait)
                {
                    if ((tcpService.SOCKET_STATE == TcpCommunication.TCP_SocketState.SOCKET_CONNECTED)
                        || (tcpService.SOCKET_STATE == TcpCommunication.TCP_SocketState.SOCKET_CLOSED))
                    {
                        break;
                    }

                    Thread.Sleep(100);
                    timeDelay = timeDelay + 1;
                }

                if (tcpService.SOCKET_STATE != TcpCommunication.TCP_SocketState.SOCKET_CONNECTED)
                {
                    result["Message"] = "与网关通讯失败！";
                    return result;
                }

                byte[] cmd_send = null;

                if (k == "1")
                {
                    CmdToDtuSetDateTime cmd = new CmdToDtuSetDateTime();
                    cmd.AddressField = DeviceNo.Substring(0, 12) + Convert.ToInt32(DeviceNo.Substring(12, 3)).ToString("X").PadLeft(2, '0');
                    cmd.StationType = (byte)device.StationType;
                    cmd.StationCode = device.StationType == 2 ? device.StationCode : 0;
                    cmd.DateTimeNew = DateTime.Parse(DeviceTime);
                    cmd.RawDataChar = cmd.WriteMsg();
                    cmd.RawDataStr = HexStringUtility.ByteArrayToHexString(cmd.RawDataChar);

                    myLogger.Info(cmd.RawDataStr);

                    cmd_send = cmd.RawDataChar;
                }
                else if (k == "2")
                {
                    CmdToDtuQueryDateTime cmd = new CmdToDtuQueryDateTime();
                    cmd.AddressField = DeviceNo.Substring(0, 12) + Convert.ToInt32(DeviceNo.Substring(12, 3)).ToString("X").PadLeft(2, '0');
                    cmd.StationType = (byte)device.StationType;
                    cmd.StationCode = device.StationType == 2 ? device.StationCode : 0;
                    cmd.RawDataChar = cmd.WriteMsg();
                    cmd.RawDataStr = HexStringUtility.ByteArrayToHexString(cmd.RawDataChar);

                    myLogger.Info(cmd.RawDataStr);

                    cmd_send = cmd.RawDataChar;
                }
                else if (k == "3")
                {
                    CmdToDtuSetYearExploitation cmd = new CmdToDtuSetYearExploitation();
                    cmd.AddressField = DeviceNo.Substring(0, 12) + Convert.ToInt32(DeviceNo.Substring(12, 3)).ToString("X").PadLeft(2, '0');
                    cmd.StationType = (byte)device.StationType;
                    cmd.StationCode = device.StationType == 2 ? device.StationCode : 0;
                    cmd.YearExploitation = decimal.Parse(YearExploitation);
                    cmd.RawDataChar = cmd.WriteMsg();
                    cmd.RawDataStr = HexStringUtility.ByteArrayToHexString(cmd.RawDataChar);

                    myLogger.Info(cmd.RawDataStr);

                    cmd_send = cmd.RawDataChar;
                }
                else if (k == "4")
                {
                    CmdToDtuQueryYearExploitation cmd = new CmdToDtuQueryYearExploitation();
                    cmd.AddressField = DeviceNo.Substring(0, 12) + Convert.ToInt32(DeviceNo.Substring(12, 3)).ToString("X").PadLeft(2, '0');
                    cmd.StationType = (byte)device.StationType;
                    cmd.StationCode = device.StationType == 2 ? device.StationCode : 0;
                    cmd.RawDataChar = cmd.WriteMsg();
                    cmd.RawDataStr = HexStringUtility.ByteArrayToHexString(cmd.RawDataChar);

                    myLogger.Info(cmd.RawDataStr);

                    cmd_send = cmd.RawDataChar;
                }
                else if (k == "5")
                {
                    CmdToDtuOpenPump cmd = new CmdToDtuOpenPump();
                    cmd.AddressField = DeviceNo.Substring(0, 12) + Convert.ToInt32(DeviceNo.Substring(12, 3)).ToString("X").PadLeft(2, '0');
                    cmd.StationType = (byte)device.StationType;
                    cmd.StationCode = device.StationType == 2 ? device.StationCode : 0;
                    cmd.RawDataChar = cmd.WriteMsg();
                    cmd.RawDataStr = HexStringUtility.ByteArrayToHexString(cmd.RawDataChar);

                    myLogger.Info(cmd.RawDataStr);

                    cmd_send = cmd.RawDataChar;
                }
                else if (k == "6")
                {
                    CmdToDtuClosePump cmd = new CmdToDtuClosePump();
                    cmd.AddressField = DeviceNo.Substring(0, 12) + Convert.ToInt32(DeviceNo.Substring(12, 3)).ToString("X").PadLeft(2, '0');
                    cmd.StationType = (byte)device.StationType;
                    cmd.StationCode = device.StationType == 2 ? device.StationCode : 0;
                    cmd.RawDataChar = cmd.WriteMsg();
                    cmd.RawDataStr = HexStringUtility.ByteArrayToHexString(cmd.RawDataChar);

                    myLogger.Info(cmd.RawDataStr);

                    cmd_send = cmd.RawDataChar;
                }
                else if (k == "7")
                {
                    CmdToDtuSetStationCode cmd = new CmdToDtuSetStationCode();
                    
                    if (device.StationType == 0)
                    {
                        result["Message"] = "非主从站无法设置分站射频地址！";
                        return result;
                    }
                    else if (device.StationType == 1)
                    {
                        cmd.AddressField = DeviceNo.Substring(0, 12) + Convert.ToInt32(DeviceNo.Substring(12, 3)).ToString("X").PadLeft(2, '0');
                        cmd.StationType = (byte)device.StationType;
                        cmd.StationCode = device.StationType == 2 ? device.StationCode : 0;
                        List<Device> DeviceSubList = DeviceModule.GetAllDeviceSubList(device.Id);
                        List<int> list = new List<int>();
                        foreach (Device DeviceSub in DeviceSubList)
                        {
                            list.Add(DeviceSub.StationCode);
                        }
                        cmd.StationCodeList = list;
                    }
                    else if (device.StationType == 2)
                    {
                        Device DeviceMain = DeviceModule.GetDeviceByID(device.MainId);
                        string DeviceMainNo = DeviceModule.GetFullDeviceNoByID(device.MainId);
                        cmd.AddressField = DeviceMainNo.Substring(0, 12) + Convert.ToInt32(DeviceMainNo.Substring(12, 3)).ToString("X").PadLeft(2, '0');
                        cmd.StationType = (byte)DeviceMain.StationType;
                        cmd.StationCode = DeviceMain.StationType == 2 ? DeviceMain.StationCode : 0;
                        List<Device> DeviceSubList = DeviceModule.GetAllDeviceSubList(device.MainId);
                        List<int> list = new List<int>();
                        foreach (Device DeviceSub in DeviceSubList)
                        {
                            list.Add(DeviceSub.StationCode);
                        }
                        cmd.StationCodeList = list;
                    }

                    cmd.RawDataChar = cmd.WriteMsg();
                    cmd.RawDataStr = HexStringUtility.ByteArrayToHexString(cmd.RawDataChar);

                    myLogger.Info(cmd.RawDataStr);

                    cmd_send = cmd.RawDataChar;
                }
                else if (k == "8")
                {
                    if (SerialNumber.Length != 8)
                    {
                        result["Message"] = "卡号长度只能为8位！";
                        return result;
                    }
                    if (!Regex.IsMatch(SerialNumber, "^[0-9A-Fa-f]+$"))
                    {
                        result["Message"] = "卡号只能为0-9A-Fa-f！"; 
                        return result;
                    }

                    CmdToDtuShieldSerialNumber cmd = new CmdToDtuShieldSerialNumber();
                    cmd.AddressField = DeviceNo.Substring(0, 12) + Convert.ToInt32(DeviceNo.Substring(12, 3)).ToString("X").PadLeft(2, '0');
                    cmd.StationType = (byte)device.StationType;
                    cmd.StationCode = device.StationType == 2 ? device.StationCode : 0;
                    cmd.SerialNumber = SerialNumber;
                    cmd.RawDataChar = cmd.WriteMsg();
                    cmd.RawDataStr = HexStringUtility.ByteArrayToHexString(cmd.RawDataChar);

                    myLogger.Info(cmd.RawDataStr);

                    cmd_send = cmd.RawDataChar;
                }
                else if (k == "9")
                {
                    if (SerialNumber.Length != 8)
                    {
                        result["Message"] = "卡号长度只能为8位！";
                        return result;
                    }
                    if (!Regex.IsMatch(SerialNumber, "^[0-9A-Fa-f]+$"))
                    {
                        result["Message"] = "卡号只能为0-9A-Fa-f！";
                        return result;
                    }

                    CmdToDtuShieldSerialNumberCancel cmd = new CmdToDtuShieldSerialNumberCancel();
                    cmd.AddressField = DeviceNo.Substring(0, 12) + Convert.ToInt32(DeviceNo.Substring(12, 3)).ToString("X").PadLeft(2, '0');
                    cmd.StationType = (byte)device.StationType;
                    cmd.StationCode = device.StationType == 2 ? device.StationCode : 0;
                    cmd.SerialNumber = SerialNumber;
                    cmd.RawDataChar = cmd.WriteMsg();
                    cmd.RawDataStr = HexStringUtility.ByteArrayToHexString(cmd.RawDataChar);

                    myLogger.Info(cmd.RawDataStr);

                    cmd_send = cmd.RawDataChar;
                }
                else if (k == "10")
                {
                    CmdToDtuSetGroundWaterParam cmd = new CmdToDtuSetGroundWaterParam();
                    cmd.AddressField = DeviceNo.Substring(0, 12) + Convert.ToInt32(DeviceNo.Substring(12, 3)).ToString("X").PadLeft(2, '0');
                    cmd.StationType = (byte)device.StationType;
                    cmd.StationCode = device.StationType == 2 ? device.StationCode : 0;
                    cmd.Range = byte.Parse(Range);
                    cmd.LineLength = double.Parse(LineLength);
                    cmd.RawDataChar = cmd.WriteMsg();
                    cmd.RawDataStr = HexStringUtility.ByteArrayToHexString(cmd.RawDataChar);

                    myLogger.Info(cmd.RawDataStr);

                    cmd_send = cmd.RawDataChar;
                }
                else if (k == "11")
                {
                    CmdToDtuQueryGroundWaterParam cmd = new CmdToDtuQueryGroundWaterParam();
                    cmd.AddressField = DeviceNo.Substring(0, 12) + Convert.ToInt32(DeviceNo.Substring(12, 3)).ToString("X").PadLeft(2, '0');
                    cmd.StationType = (byte)device.StationType;
                    cmd.StationCode = device.StationType == 2 ? device.StationCode : 0;
                    cmd.RawDataChar = cmd.WriteMsg();
                    cmd.RawDataStr = HexStringUtility.ByteArrayToHexString(cmd.RawDataChar);

                    myLogger.Info(cmd.RawDataStr);

                    cmd_send = cmd.RawDataChar;
                }
                else if (k == "12")
                {
                    CmdToDtuQueryGroundWater cmd = new CmdToDtuQueryGroundWater();
                    cmd.AddressField = DeviceNo.Substring(0, 12) + Convert.ToInt32(DeviceNo.Substring(12, 3)).ToString("X").PadLeft(2, '0');
                    cmd.StationType = (byte)device.StationType;
                    cmd.StationCode = device.StationType == 2 ? device.StationCode : 0;
                    cmd.RawDataChar = cmd.WriteMsg();
                    cmd.RawDataStr = HexStringUtility.ByteArrayToHexString(cmd.RawDataChar);

                    myLogger.Info(cmd.RawDataStr);

                    cmd_send = cmd.RawDataChar;
                }
                else if (k == "13")
                {
                    CmdToDtuQueryState cmd = new CmdToDtuQueryState();
                    cmd.AddressField = DeviceNo.Substring(0, 12) + Convert.ToInt32(DeviceNo.Substring(12, 3)).ToString("X").PadLeft(2, '0');
                    cmd.StationType = (byte)device.StationType;
                    cmd.StationCode = device.StationType == 2 ? device.StationCode : 0;
                    cmd.RawDataChar = cmd.WriteMsg();
                    cmd.RawDataStr = HexStringUtility.ByteArrayToHexString(cmd.RawDataChar);

                    myLogger.Info(cmd.RawDataStr);

                    cmd_send = cmd.RawDataChar;
                }
                else
                {
                    result["Message"] = "参数错误！";
                    return result;
                }
                tcpService.SendData(cmd_send, 0, cmd_send.Length);

                timeDelay = 0;
                while (timeDelay < tcpService.TcpWait)
                {
                    if (tcpService.socketData.Buffer.Length > 0)
                    {
                        myLogger.Info(HexStringUtility.ByteArrayToHexString(tcpService.socketData.Buffer.Buffer));
                    }
                    if (tcpService.socketData.Buffer.Length >= CommandCommon.CMD_MIN_LENGTH)
                    {
                        byte[] re = tcpService.socketData.Buffer.Buffer;
                        byte[] buffer_new = new byte[tcpService.socketData.Buffer.Length];
                        Array.Copy(re, buffer_new, tcpService.socketData.Buffer.Length);
                        string receiveHex = HexStringUtility.ByteArrayToHexString(buffer_new);
                        //string str = HexStringUtility.HexStringToStr(receiveHex);
                        waitRsp = true;
                        result["Result"] = true;
                        if (receiveHex == BaseProtocol.DeviceOffline)
                        {
                            result["Message"] = "查询终端不在线";
                        }
                        else
                        {
                            result["Message"] = "终端在线返回结果：" + receiveHex;
                        }
                    }

                    if (waitRsp == true)
                    {
                        myLogger.Info("获取响应结束");
                        break;
                    }

                    if (tcpService.SOCKET_STATE == TcpCommunication.TCP_SocketState.SOCKET_CLOSED)
                    {
                        myLogger.Info("Socket关闭结束");
                        break;
                    }

                    Thread.Sleep(100);
                    timeDelay = timeDelay + 1;
                }

                tcpService.Close();

                if (waitRsp == false)
                {
                    result["Message"] = "等待设备回复超时！";
                }
            }
            catch (Exception exception)
            {
                result["Message"] = exception.Message;
                myLogger.Error(exception.Message);
            }

            return result;
        }

        private JavaScriptObject getDeviceNo(HttpRequest request)
        {
            string DeviceNo = request["DeviceNo"] ?? "";

            JavaScriptObject result = new JavaScriptObject();
            result.Add("Result", false);
            result.Add("Message", "");

            try
            {
                Device device = DeviceModule.GetDeviceByFullDeviceNo(DeviceNo);
                if (device != null)
                {
                    result["Result"] = true;
                    result["Message"] = "设备已存在";
                }
                else
                {
                    result["Message"] = "设备不存在";
                }
            }
            catch (Exception exception)
            {
                result["Message"] = exception.Message;
                myLogger.Error(exception.Message);
            }

            return result;
        }

        private JavaScriptObject getAllDeviceNo(HttpRequest request)
        {
            JavaScriptObject result = new JavaScriptObject();
            result.Add("Result", false);
            result.Add("Message", "");

            try
            {
                List<string> list = DeviceModule.GetAllDeviceNo();
                result["Result"] = true;
                result["Message"] = string.Join(",", list.ToArray());
            }
            catch (Exception exception)
            {
                result["Message"] = exception.Message;
                myLogger.Error(exception.Message);
            }

            return result;
        }
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}