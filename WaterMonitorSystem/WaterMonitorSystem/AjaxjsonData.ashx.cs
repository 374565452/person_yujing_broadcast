using Common;
using DTU.GateWay.Protocol;
using Maticsoft.Model;
using Module;
using Newtonsoft.Json;
using System;
using System.Web;

namespace WaterMonitorSystem
{
    /// <summary>
    /// AjaxjsonData 的摘要说明
    /// </summary>
    public class AjaxjsonData : IHttpHandler
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
                    case "HelloWorld": result = HelloWorld(request); break;
                    case "OpenPump": result = OpenPump(request); break;
                    case "ClosePump": result = ClosePump(request); break;
                    case "CardUserWaterLog": result = CardUserWaterLog(request); break;
                    case "OpenPumpCmd": result = OpenPumpCmd(request); break;
                    case "ClosePumpCmd": result = ClosePumpCmd(request); break;
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

        private JavaScriptObject HelloWorld(HttpRequest request)
        {
            JavaScriptObject result = new JavaScriptObject();
            result.Add("Result", true);
            result.Add("Message", "HelloWorld");

            return result;
        }

        private JavaScriptObject OpenPump(HttpRequest request)
        {
            string DeviceNo = request["DeviceNo"] ?? "";
            string SerialNumber = request["SerialNumber"] ?? "";
            string UserNo = request["UserNo"] ?? "";
            string EventTime = request["EventTime"] ?? "";
            string StartTime = request["StartTime"] ?? "";
            string StartResidualWater = request["StartResidualWater"] ?? "";
            string StartResidualElectric = request["StartResidualElectric"] ?? "";
            string YearWaterUsed = request["YearWaterUsed"] ?? "";
            string YearElectricUsed = request["YearElectricUsed"] ?? "";
            string YearSurplus = request["YearSurplus"] ?? "";
            string YearExploitation = request["YearExploitation"] ?? "";

            JavaScriptObject result = new JavaScriptObject();
            result.Add("Result", false);
            result.Add("Message", "");

            try
            {
                Device device = DeviceModule.GetDeviceByFullDeviceNo(DeviceNo);
                if (device == null)
                {
                    result["Message"] = "设备长编号" + DeviceNo + "不存在！";
                    return result;
                }

                CmdFromDtuOpenPump cmd = new CmdFromDtuOpenPump();
                cmd.AddressField = DeviceNo.Substring(0, 12) + Convert.ToInt32(DeviceNo.Substring(12, 3)).ToString("X").PadLeft(2, '0');
                cmd.StationType = (byte)device.StationType;
                cmd.StationCode = device.StationCode;
                cmd.UserNo = UserNo;
                cmd.SerialNumber = SerialNumber;
                cmd.StartTime = DateTime.Parse(StartTime);
                cmd.StartResidualWater = decimal.Parse(StartResidualWater);
                cmd.StartResidualElectric = decimal.Parse(StartResidualElectric);
                cmd.YearWaterUsed = decimal.Parse(YearWaterUsed);
                cmd.YearElectricUsed = decimal.Parse(YearElectricUsed);
                cmd.YearExploitation = decimal.Parse(YearExploitation);
                cmd.YearSurplus = decimal.Parse(YearSurplus);

                cmd.RawDataChar = cmd.WriteMsg();
                cmd.RawDataStr = HexStringUtility.ByteArrayToHexString(cmd.RawDataChar);

                DeviceEvent deviceEvent = new DeviceEvent();
                DeviceEventModule.InitDeviceEvent(deviceEvent);
                deviceEvent.DeviceNo = DeviceNo;
                deviceEvent.EventTime = cmd.StartTime;
                deviceEvent.EventType = "开泵上报";
                deviceEvent.UserNo = cmd.UserNo;
                deviceEvent.SerialNumber = cmd.SerialNumber;
                deviceEvent.StartTime = cmd.StartTime;
                deviceEvent.StartResidualWater = cmd.StartResidualWater;
                deviceEvent.StartResidualElectric = cmd.StartResidualElectric;
                deviceEvent.YearWaterUsed = cmd.YearWaterUsed;
                deviceEvent.YearElectricUsed = cmd.YearElectricUsed;
                deviceEvent.YearExploitation = cmd.YearExploitation;
                deviceEvent.YearSurplus = cmd.YearSurplus;
                deviceEvent.RawData = cmd.RawDataStr;
                deviceEvent.DeviceTime = cmd.StartTime;
                deviceEvent.Remark = "开泵时间：" + deviceEvent.StartTime.ToString("yyyy-MM-dd HH:mm:ss") + "，卡号：" + deviceEvent.UserNo + "，卡剩余水量：" + deviceEvent.StartResidualWater + "，卡剩余电量：" + deviceEvent.StartResidualElectric + "";

                DeviceEventModule.AddDeviceEvent(deviceEvent);

                result["Result"] = true;
            }
            catch (Exception ex)
            {
                result["Message"] = "开泵上报保存出错！";
                myLogger.Error(ex.Message);
            }

            return result;
        }

        private JavaScriptObject ClosePump(HttpRequest request)
        {
            string DeviceNo = request["DeviceNo"] ?? "";
            string SerialNumber = request["SerialNumber"] ?? "";
            string UserNo = request["UserNo"] ?? "";
            string EventTime = request["EventTime"] ?? "";
            string StartTime = request["StartTime"] ?? "";
            string StartResidualWater = request["StartResidualWater"] ?? "";
            string StartResidualElectric = request["StartResidualElectric"] ?? "";
            string EndTime = request["EndTime"] ?? "";
            string EndResidualWater = request["EndResidualWater"] ?? "";
            string EndResidualElectric = request["EndResidualElectric"] ?? "";
            string WaterUsed = request["WaterUsed"] ?? "";
            string ElectricUsed = request["ElectricUsed"] ?? "";
            string YearWaterUsed = request["YearWaterUsed"] ?? "";
            string YearElectricUsed = request["YearElectricUsed"] ?? "";
            string YearSurplus = request["YearSurplus"] ?? "";
            string YearExploitation = request["YearExploitation"] ?? "";
            string RecordType = request["RecordType"] ?? "";
            string REV1 = request["REV1"] ?? "";
            string REV2 = request["REV2"] ?? "";

            JavaScriptObject result = new JavaScriptObject();
            result.Add("Result", false);
            result.Add("Message", "");

            try
            {
                Device device = DeviceModule.GetDeviceByFullDeviceNo(DeviceNo);
                if (device == null)
                {
                    result["Message"] = "设备长编号" + DeviceNo + "不存在！";
                    return result;
                }

                CmdFromDtuClosePump cmd = new CmdFromDtuClosePump();
                cmd.AddressField = DeviceNo.Substring(0, 12) + Convert.ToInt32(DeviceNo.Substring(12, 3)).ToString("X").PadLeft(2, '0');
                cmd.StationType = (byte)device.StationType;
                cmd.StationCode = device.StationCode;
                cmd.UserNo = UserNo;
                cmd.SerialNumber = SerialNumber;
                cmd.StartTime = DateTime.Parse(StartTime);
                cmd.StartResidualWater = decimal.Parse(StartResidualWater);
                cmd.StartResidualElectric = decimal.Parse(StartResidualElectric);
                cmd.EndTime = DateTime.Parse(EndTime);
                cmd.EndResidualWater = decimal.Parse(EndResidualWater);
                cmd.EndResidualElectric = decimal.Parse(EndResidualElectric);
                cmd.WaterUsed = decimal.Parse(WaterUsed);
                cmd.ElectricUsed = decimal.Parse(ElectricUsed);
                cmd.YearWaterUsed = decimal.Parse(YearWaterUsed);
                cmd.YearElectricUsed = decimal.Parse(YearElectricUsed);
                cmd.YearExploitation = decimal.Parse(YearExploitation);
                cmd.YearSurplus = decimal.Parse(YearSurplus);

                cmd.RecordType = byte.Parse(RecordType);
                cmd.REV1 = byte.Parse(REV1);
                cmd.REV2 = byte.Parse(REV2);
                cmd.CRC8 = 0x00;

                cmd.RawDataChar = cmd.WriteMsg();
                cmd.RawDataStr = HexStringUtility.ByteArrayToHexString(cmd.RawDataChar);

                DeviceEvent deviceEvent = new DeviceEvent();
                DeviceEventModule.InitDeviceEvent(deviceEvent);
                deviceEvent.DeviceNo = DeviceNo;
                deviceEvent.EventTime = cmd.EndTime;
                deviceEvent.EventType = "关泵上报";
                deviceEvent.UserNo = cmd.UserNo;
                deviceEvent.SerialNumber = cmd.SerialNumber;
                deviceEvent.StartTime = cmd.StartTime;
                deviceEvent.StartResidualWater = cmd.StartResidualWater;
                deviceEvent.StartResidualElectric = cmd.StartResidualElectric;
                deviceEvent.EndTime = cmd.EndTime;
                deviceEvent.EndResidualWater = cmd.EndResidualWater;
                deviceEvent.EndResidualElectric = cmd.EndResidualElectric;
                deviceEvent.WaterUsed = cmd.WaterUsed;
                deviceEvent.ElectricUsed = cmd.ElectricUsed;
                deviceEvent.YearWaterUsed = cmd.YearWaterUsed;
                deviceEvent.YearElectricUsed = cmd.YearElectricUsed;
                deviceEvent.YearExploitation = cmd.YearExploitation;
                deviceEvent.YearSurplus = cmd.YearSurplus;
                deviceEvent.RawData = cmd.RawDataStr;
                deviceEvent.DeviceTime = cmd.EndTime;
                deviceEvent.Remark = "关泵时间：" + deviceEvent.EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "，卡号：" + deviceEvent.UserNo + "，卡剩余水量：" + deviceEvent.StartResidualWater + "，卡剩余电量：" + deviceEvent.StartResidualElectric +
                    ",本次水量：" + deviceEvent.WaterUsed + "，电量：" + deviceEvent.ElectricUsed +
                    "，记录类型：" + deviceEvent.RecordType + "，开泵类型：" + deviceEvent.REV1 + "，关泵类型：" + deviceEvent.REV2 + "";

                DeviceEventModule.AddDeviceEvent(deviceEvent);

                result["Result"] = true;
            }
            catch (Exception ex)
            {
                result["Message"] = "关泵上报保存出错！";
                myLogger.Error(ex.Message);
            }

            return result;
        }

        private JavaScriptObject CardUserWaterLog(HttpRequest request)
        {
            string SerialNumber = request["SerialNumber"] ?? "";
            string UserNo = request["UserNo"] ?? "";
            string DeviceNo = request["DeviceNo"] ?? "";
            string StartTime = request["StartTime"] ?? "";
            string StartResidualWater = request["StartResidualWater"] ?? "";
            string StartResidualElectric = request["StartResidualElectric"] ?? "";
            string EndTime = request["EndTime"] ?? "";
            string EndResidualWater = request["EndResidualWater"] ?? "";
            string EndResidualElectric = request["EndResidualElectric"] ?? "";

            JavaScriptObject result = new JavaScriptObject();
            result.Add("Result", false);
            result.Add("Message", "");

            try
            {
                CardUser cu = CardUserModule.GetCardUserBySerialNumber(SerialNumber);
                if (cu == null)
                {
                    result["Message"] = "卡序列号" + SerialNumber + "的用水户不存在！";
                    return result;
                }

                Device device = DeviceModule.GetDeviceByFullDeviceNo(DeviceNo);
                if (device == null)
                {
                    result["Message"] = "设备长编号" + DeviceNo + "不存在！";
                    return result;
                }

                CardUserWaterLog cuwLog = new CardUserWaterLog();
                cuwLog.SerialNumber = SerialNumber;
                cuwLog.WateUserId = cu.WaterUserId;
                cuwLog.UserNo = UserNo;
                cuwLog.DeviceId = device.Id;
                cuwLog.DeviceNo = DeviceNo;
                cuwLog.StartTime = DateTime.Parse(StartTime);
                cuwLog.StartResidualWater = decimal.Parse(StartResidualWater);
                cuwLog.StartResidualElectric = decimal.Parse(StartResidualElectric);
                cuwLog.EndTime = DateTime.Parse(EndTime);
                cuwLog.EndResidualWater = decimal.Parse(EndResidualWater);
                cuwLog.EndResidualElectric = decimal.Parse(EndResidualElectric);
                cuwLog.WaterUsed = decimal.Parse(EndResidualWater) - decimal.Parse(StartResidualWater);
                cuwLog.ElectricUsed = decimal.Parse(EndResidualElectric) - decimal.Parse(StartResidualElectric);
                cuwLog.Duration = Convert.ToDecimal((DateTime.Parse(EndTime) - DateTime.Parse(StartTime)).TotalSeconds);
                CardUserWaterLogModule.AddCardUserWaterLog(cuwLog);

                result["Result"] = true;
            }
            catch(Exception ex)
            {
                result["Message"] = "用户用水保存出错！";
                myLogger.Error(ex.Message);
            }

            return result;
        }

        private JavaScriptObject OpenPumpCmd(HttpRequest request)
        {
            string DeviceNo = request["DeviceNo"] ?? "";
            string SendTime = request["SendTime"] ?? "";

            JavaScriptObject result = new JavaScriptObject();
            result.Add("Result", false);
            result.Add("Message", "");
            try
            {
                DateTime OperationTime = DateTime.Parse(SendTime);

                CmdToDtuOpenPump cmd = new CmdToDtuOpenPump();
                cmd.AddressField = DeviceNo.Substring(0, 12) + Convert.ToInt32(DeviceNo.Substring(12, 3)).ToString("X").PadLeft(2, '0');
                byte[] bs = cmd.WriteMsg(OperationTime);
                string str = HexStringUtility.ByteArrayToHexString(bs);

                Device device = DeviceModule.GetDeviceByFullDeviceNo(DeviceNo);
                if (device == null)
                {
                    result["Message"] = "设备长编号" + DeviceNo + "不存在！";
                }
                else
                {
                    DeviceOperation op = new DeviceOperation();

                    op.DeviceNo = DeviceNo;
                    op.DeviceName = device.DeviceName;
                    op.OperationTime = OperationTime;
                    op.OperationType = "远程开泵";
                    op.RawData = str;
                    op.Remark = "";
                    op.UserId = 0;
                    op.UserName = "0";
                    op.State = "发送成功";

                    DeviceOperationModule.AddDeviceOperation(op);

                    result["Result"] = true;
                }
            }
            catch (Exception ex)
            {
                result["Message"] = "开泵命令保存出错！";
                myLogger.Error(ex.Message);
            }
            return result;
        }

        private JavaScriptObject ClosePumpCmd(HttpRequest request)
        {
            string DeviceNo = request["DeviceNo"] ?? "";
            string SendTime = request["SendTime"] ?? "";

            JavaScriptObject result = new JavaScriptObject();
            result.Add("Result", false);
            result.Add("Message", "");
            try
            {
                DateTime OperationTime = DateTime.Parse(SendTime);
                
                CmdToDtuClosePump cmd = new CmdToDtuClosePump();
                cmd.AddressField = DeviceNo.Substring(0, 12) + Convert.ToInt32(DeviceNo.Substring(12, 3)).ToString("X").PadLeft(2, '0');
                byte[] bs = cmd.WriteMsg(OperationTime);
                string str = HexStringUtility.ByteArrayToHexString(bs);

                Device device = DeviceModule.GetDeviceByFullDeviceNo(DeviceNo);
                if (device == null)
                {
                    result["Message"] = "设备长编号" + DeviceNo + "不存在！";
                }
                else
                {
                    DeviceOperation op = new DeviceOperation();

                    op.DeviceNo = DeviceNo;
                    op.DeviceName = device.DeviceName;
                    op.OperationTime = OperationTime;
                    op.OperationType = "远程关泵";
                    op.RawData = str;
                    op.Remark = "";
                    op.UserId = 0;
                    op.UserName = "0";
                    op.State = "发送成功";

                    DeviceOperationModule.AddDeviceOperation(op);

                    result["Result"] = true;
                }
            }
            catch (Exception ex)
            {
                result["Message"] = "关泵命令保存出错！";
                myLogger.Error(ex.Message);
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