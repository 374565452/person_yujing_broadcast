using Common;
using DTU.GateWay.Protocol;
using Maticsoft.Model;
using Module;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Web;
using System.Web.Services;
using WaterMonitorSystem.Src;

namespace WaterMonitorSystem.WebServices
{
    /// <summary>
    /// DeviceNodeService 的摘要说明
    /// </summary>
    [Serializable, WebService(Description = "提供设备节点操作服务，包括获取设备信息、添加设备、修改设备、删除设备", Name = "设备节点服务", Namespace = "http://www.data86.net/"), ToolboxItem(false), WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class DeviceNodeService : System.Web.Services.WebService
    {
        static log4net.ILog myLogger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private HttpContext context = HttpContext.Current;

        public DeviceNodeService()
        {
            this.context.Response.Buffer = true;
            this.context.Response.ExpiresAbsolute = DateTime.Now.AddSeconds(-1.0);
            this.context.Response.Expires = 0;
            this.context.Response.CacheControl = "no-cache";
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>添加设备节点信息</span><br/><p>输入参数：loginIdentifer=登录标识，deviceJSONString=设备JSON对象字符串<br/>返回数据格式：{'Result':bool,'Message':string}</p>")]
        public string AddDevice(string loginIdentifer, string deviceJSONString)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            LoginUser loginUser = GlobalAppModule.GetLoginUser(loginIdentifer);
            if (loginUser == null)
            {
                obj2["Message"] = "未登录";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            if (loginUser.LoginTimeout)
            {
                obj2["Message"] = "登录超时";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            JavaScriptObject obj3 = (JavaScriptObject)JavaScriptConvert.DeserializeObject(deviceJSONString);
            if (obj3 == null)
            {
                obj2["Message"] = "参数deviceJSONString格式不正确";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            loginUser.LastOperateTime = DateTime.Now;
            CommonUtil.WaitMainLibInit();

            Device device = new Device();
            device.SimNo = obj3["手机卡号"].ToString();
            device.DeviceName = obj3["名称"].ToString();
            device.Description = "";
            try
            {
                device.SetupDate = DateTime.Parse(obj3["安装时间"].ToString() + ":00");
            }
            catch
            {
                obj2["Message"] = "安装时间格式错误";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            device.SetupAddress = obj3["安装位置"].ToString();
            try
            {
                string[] arrMap = obj3["经纬度"].ToString().Trim().Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                device.LON = (long)(double.Parse(arrMap[0]) * 1000000);
                device.LAT = (long)(double.Parse(arrMap[1]) * 1000000);
            }
            catch { device.LON = 0; device.LAT = 0; }
            device.IsValid = 1;
            device.LastUpdate = DateTime.Parse("2000-1-1");
            try
            {
                device.DistrictId = long.Parse(obj3["管理ID"].ToString());
            }
            catch
            {
                obj2["Message"] = "请选择区域";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            device.DeviceNo = obj3["编码"].ToString();
            try
            {
                if (device.DeviceNo.Length > 3)
                {
                    obj2["Message"] = "设备序号必须为整数，范围1-255";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                int i = Convert.ToInt16(device.DeviceNo);
                if (i < 1 || i > 255)
                {
                    obj2["Message"] = "设备序号必须为整数，范围1-255";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
            }
            catch
            {
                obj2["Message"] = "设备序号必须为整数，范围1-255";
                return JavaScriptConvert.SerializeObject(obj2);
            }

            device.Online = 0;
            device.OnlineTime = DateTime.Parse("2000-1-1");
            try
            {
                device.YearExploitation = decimal.Parse(obj3["年可开采水量"].ToString());
            }
            catch { device.YearExploitation = 0; }
            try
            {
                device.AlertAvailableWater = int.Parse(obj3["可用水量提醒"].ToString());
            }
            catch { device.AlertAvailableWater = 0; }
            try
            {
                device.AlertAvailableElectric = int.Parse(obj3["可用电量提醒"].ToString());
            }
            catch { device.AlertAvailableElectric = 0; }
            try
            {
                device.DeviceTypeCodeId = int.Parse(obj3["流量计类型"].ToString());
            }
            catch { device.DeviceTypeCodeId = 0; }
            try
            {
                device.MeterPulse = int.Parse(obj3["电表脉冲数"].ToString());
            }
            catch { device.MeterPulse = 0; }
            try
            {
                device.AlertWaterLevel = decimal.Parse(obj3["水位报警值"].ToString());
            }
            catch { device.AlertWaterLevel = 0; }
            device.TerminalState = "正常";
            device.Remark = "";
            try
            {
                device.CropId = long.Parse(obj3["作物"].ToString());
            }
            catch
            {
                obj2["Message"] = "请选择作物";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            try
            {
                device.Area = decimal.Parse(obj3["面积"].ToString());
            }
            catch
            {
                device.Area = 0;
            }
            try
            {
                device.StationType = int.Parse(obj3["站类型"].ToString());
            }
            catch
            {
                device.StationType = 0;
            }
            try
            {
                device.StationCode = int.Parse(obj3["地址码"].ToString());
            }
            catch
            {
                device.StationCode = 0;
            }
            if (device.StationCode < 0 || device.StationCode > 65535)
            {
                obj2["Message"] = "地址码必须为整数，范围0-65535";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            try
            {
                //device.Frequency = int.Parse(obj3["通信频率"].ToString());
                device.Frequency = 0;
            }
            catch
            {
                device.Frequency = 0;
            }
            try
            {
                string MainDevNo = obj3["主站编码"].ToString().PadLeft(3, '0');
                Device MainD = DeviceModule.GetDeviceByDistrictId(device.DistrictId, MainDevNo);
                if (MainD != null)
                {
                    device.MainId = MainD.Id;
                }
            }
            catch
            {
                device.MainId = 0;
            }
            device.DeviceType = obj3["设备类型"].ToString();
            device.RemoteStation = obj3["水位仪编码"].ToString().Trim();

            if (DeviceModule.ExistsSimNo(device.SimNo))
            {
                obj2["Message"] = "已存在手机号码";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            if (DeviceModule.ExistsDeviceName(device.DeviceName, device.DistrictId))
            {
                obj2["Message"] = "已存在设备名称";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            if (DeviceModule.ExistsDeviceNo(device.DeviceNo, device.DistrictId))
            {
                obj2["Message"] = "已存在设备编码";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            if (device.RemoteStation != null && device.RemoteStation.Length > 0)
            {
                if (DeviceModule.ExistsRemoteStation(device.RemoteStation, device.Id))
                {
                    obj2["Message"] = "已存在水位仪编码";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
            }

            ResMsg msg = DeviceModule.AddDevice(device);
            if (msg.Result)
            {
                GlobalAppModule.IsInitMainLib = true;
                try
                {
                    //更新网关设备缓存
                    Thread parameterThread = new Thread(new ParameterizedThreadStart(TcpRunThread.ParameterRun));
                    parameterThread.Start(ProtocolConst.WebToGateUpdateCache + ProtocolConst.UpdateCache_Device + "01" + DeviceModule.GetFullDeviceNoByID(device.Id).PadLeft(16, '0'));  
                }
                catch (Exception exception)
                {
                    //new Guard().Logger(exception, "GetOperateDevice");
                    myLogger.Error(exception.Message);
                }
                GlobalAppModule.IsInitMainLib = false;
                obj2["Result"] = true;
                obj2["Message"] = "成功";
            }
            else
            {
                obj2["Message"] = msg.Message;
            }
            try
            {
                //添加日志
                DeviceLog log = new DeviceLog();
                log.DeviceId = device.Id;
                log.LogUserId = loginUser.UserId;
                log.LogUserName = loginUser.LoginName;
                log.LogAddress = ToolsWeb.GetIP(context.Request);
                log.LogTime = loginUser.LastOperateTime;
                log.LogType = "添加";
                log.LogContent = msg.Message + " | " + JavaScriptConvert.SerializeObject(device);
                log.SimNo = device.SimNo;
                log.DeviceName = device.DeviceName;
                log.Description = device.Description;
                log.SetupDate = device.SetupDate;
                log.SetupAddress = device.SetupAddress;
                log.LON = device.LON;
                log.LAT = device.LAT;
                log.IsValid = device.IsValid;
                log.LastUpdate = device.LastUpdate;
                log.DistrictId = device.DistrictId;
                log.DeviceNo = device.DeviceNo;
                log.Online = device.Online;
                log.OnlineTime = device.OnlineTime;
                log.YearExploitation = device.YearExploitation;
                log.AlertAvailableWater = device.AlertAvailableWater;
                log.AlertAvailableElectric = device.AlertAvailableElectric;
                log.DeviceTypeCodeId = device.DeviceTypeCodeId;
                log.MeterPulse = device.MeterPulse;
                log.AlertWaterLevel = device.AlertWaterLevel;
                log.TerminalState = device.TerminalState;
                log.Remark = device.Remark;
                log.CropId = device.CropId;
                log.Area = device.Area;
                log.StationType = device.StationType;
                log.StationCode = device.StationCode;
                log.Frequency = device.Frequency;
                log.MainId = device.MainId;
                log.DeviceType = device.DeviceType;
                DeviceLogModule.Add(log);
            }
            catch
            {
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>修改设备节点信息</span><br/><p>输入参数：loginIdentifer=登录标识，deviceJSONString=设备JSON对象字符串<br/>返回数据格式：{'Result':bool,'Message':string}</p>")]
        public string ModifyDevice(string loginIdentifer, string deviceJSONString)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            LoginUser loginUser = GlobalAppModule.GetLoginUser(loginIdentifer);
            if (loginUser == null)
            {
                obj2["Message"] = "未登录";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            if (loginUser.LoginTimeout)
            {
                obj2["Message"] = "登录超时";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            JavaScriptObject obj3 = (JavaScriptObject)JavaScriptConvert.DeserializeObject(deviceJSONString);
            if (obj3 == null)
            {
                obj2["Message"] = "参数deviceJSONString格式不正确";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            loginUser.LastOperateTime = DateTime.Now;
            CommonUtil.WaitMainLibInit();

            Device device = DeviceModule.GetDeviceByID(long.Parse(obj3["ID"].ToString()));
            if (device == null)
            {
                obj2["Message"] = "ID为" + obj3["ID"].ToString() + "设备不存在";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            device.SimNo = obj3["手机卡号"].ToString();
            device.DeviceName = obj3["名称"].ToString();
            //device.Description = "";
            try
            {
                device.SetupDate = DateTime.Parse(obj3["安装时间"].ToString() + ":00");
            }
            catch
            {
                obj2["Message"] = "安装时间格式错误";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            device.SetupAddress = obj3["安装位置"].ToString();
            try
            {
                string[] arrMap = obj3["经纬度"].ToString().Trim().Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                device.LON = (long)(double.Parse(arrMap[0]) * 1000000);
                device.LAT = (long)(double.Parse(arrMap[1]) * 1000000);
            }
            catch { device.LON = 0; device.LAT = 0; }
            device.IsValid = 1;
            //device.LastUpdate = DateTime.Now;
            try
            {
                device.DistrictId = long.Parse(obj3["管理ID"].ToString());
            }
            catch
            {
                obj2["Message"] = "请选择区域";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            device.DeviceNo = obj3["编码"].ToString();
            try
            {
                if (device.DeviceNo.Length > 3)
                {
                    obj2["Message"] = "设备序号必须为整数，范围1-255";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                int i = Convert.ToInt16(device.DeviceNo);
                if (i < 1 || i > 255)
                {
                    obj2["Message"] = "设备序号必须为整数，范围1-255";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
            }
            catch
            {
                obj2["Message"] = "设备序号必须为整数，范围1-255";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            //device.Online = 0;
            //device.OnlineTime = DateTime.Now;
            try
            {
                device.YearExploitation = decimal.Parse(obj3["年可开采水量"].ToString());
            }
            catch { device.YearExploitation = 0; }
            try
            {
                device.AlertAvailableWater = int.Parse(obj3["可用水量提醒"].ToString());
            }
            catch { device.AlertAvailableWater = 0; }
            try
            {
                device.AlertAvailableElectric = int.Parse(obj3["可用电量提醒"].ToString());
            }
            catch { device.AlertAvailableElectric = 0; }
            try
            {
                device.DeviceTypeCodeId = int.Parse(obj3["流量计类型"].ToString());
            }
            catch { device.DeviceTypeCodeId = 0; }
            try
            {
                device.MeterPulse = int.Parse(obj3["电表脉冲数"].ToString());
            }
            catch { device.MeterPulse = 0; }
            try
            {
                device.AlertWaterLevel = decimal.Parse(obj3["水位报警值"].ToString());
            }
            catch { device.AlertWaterLevel = 0; }
            //device.TerminalState = "正常";
            //device.Remark = "";
            try
            {
                device.CropId = long.Parse(obj3["作物"].ToString());
            }
            catch
            {
                obj2["Message"] = "请选择作物";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            try
            {
                device.Area = decimal.Parse(obj3["面积"].ToString());
            }
            catch
            {
                device.Area = 0;
            }
            try
            {
                device.StationType = int.Parse(obj3["站类型"].ToString());
            }
            catch
            {
                device.StationType = 0;
            }
            try
            {
                device.StationCode = int.Parse(obj3["地址码"].ToString());
            }
            catch
            {
                device.StationCode = 0;
            }
            if (device.StationCode < 0 || device.StationCode > 65535)
            {
                obj2["Message"] = "地址码必须为整数，范围0-65535";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            try
            {
                //device.Frequency = int.Parse(obj3["通信频率"].ToString());
                device.Frequency = 0;
            }
            catch
            {
                device.Frequency = 0;
            }
            try
            {
                string MainDevNo = obj3["主站编码"].ToString().PadLeft(3, '0');
                Device MainD = DeviceModule.GetDeviceByDistrictId(device.DistrictId, MainDevNo);
                if (MainD != null)
                {
                    device.MainId = MainD.Id;
                }
            }
            catch
            {
                device.MainId = 0;
            }
            device.DeviceType = obj3["设备类型"].ToString();
            device.RemoteStation = obj3["水位仪编码"].ToString().Trim();

            if (DeviceModule.ExistsSimNo(device.SimNo, device.Id))
            {
                obj2["Message"] = "已存在手机号码";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            if (DeviceModule.ExistsDeviceName(device.DeviceName, device.DistrictId, device.Id))
            {
                obj2["Message"] = "已存在设备名称";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            if (DeviceModule.ExistsDeviceNo(device.DeviceNo, device.DistrictId, device.Id))
            {
                obj2["Message"] = "已存在设备编码";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            if (device.RemoteStation != null && device.RemoteStation.Length > 0)
            {
                if (DeviceModule.ExistsRemoteStation(device.RemoteStation, device.Id))
                {
                    obj2["Message"] = "已存在水位仪编码";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
            }

            ResMsg msg = DeviceModule.ModifyDevice(device);
            if (msg.Result)
            {
                GlobalAppModule.IsInitMainLib = true;
                try
                {
                    //更新网关设备缓存
                    Thread parameterThread = new Thread(new ParameterizedThreadStart(TcpRunThread.ParameterRun));
                    parameterThread.Start(ProtocolConst.WebToGateUpdateCache + ProtocolConst.UpdateCache_Device + "01" + DeviceModule.GetFullDeviceNoByID(device.Id).PadLeft(16, '0'));  
                }
                catch (Exception exception)
                {
                    //new Guard().Logger(exception, "GetOperateDevice");
                    myLogger.Error(exception.Message);
                }
                GlobalAppModule.IsInitMainLib = false;
                obj2["Result"] = true;
                obj2["Message"] = "成功";
            }
            else
            {
                obj2["Message"] = msg.Message;
            }
            try
            {
                //添加日志
                DeviceLog log = new DeviceLog();
                log.DeviceId = device.Id;
                log.LogUserId = loginUser.UserId;
                log.LogUserName = loginUser.LoginName;
                log.LogAddress = ToolsWeb.GetIP(context.Request);
                log.LogTime = loginUser.LastOperateTime;
                log.LogType = "修改";
                log.LogContent = msg.Message + " | " + JavaScriptConvert.SerializeObject(device);
                log.SimNo = device.SimNo;
                log.DeviceName = device.DeviceName;
                log.Description = device.Description;
                log.SetupDate = device.SetupDate;
                log.SetupAddress = device.SetupAddress;
                log.LON = device.LON;
                log.LAT = device.LAT;
                log.IsValid = device.IsValid;
                log.LastUpdate = device.LastUpdate;
                log.DistrictId = device.DistrictId;
                log.DeviceNo = device.DeviceNo;
                log.Online = device.Online;
                log.OnlineTime = device.OnlineTime;
                log.YearExploitation = device.YearExploitation;
                log.AlertAvailableWater = device.AlertAvailableWater;
                log.AlertAvailableElectric = device.AlertAvailableElectric;
                log.DeviceTypeCodeId = device.DeviceTypeCodeId;
                log.MeterPulse = device.MeterPulse;
                log.AlertWaterLevel = device.AlertWaterLevel;
                log.TerminalState = device.TerminalState;
                log.Remark = device.Remark;
                log.CropId = device.CropId;
                log.Area = device.Area;
                log.StationType = device.StationType;
                log.StationCode = device.StationCode;
                log.Frequency = device.Frequency;
                log.MainId = device.MainId;
                log.DeviceType = device.DeviceType;
                DeviceLogModule.Add(log);
            }
            catch
            {
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>根据设备ID删除设备节点信息</span><br/><p>输入参数：loginIdentifer=登录标识，devID=设备ID<br/>返回数据格式：{'Result':bool,'Message':string}</p>")]
        public string DeleteDevice(string loginIdentifer, string devID)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            LoginUser loginUser = GlobalAppModule.GetLoginUser(loginIdentifer);
            if (loginUser == null)
            {
                obj2["Message"] = "未登录";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            if (loginUser.LoginTimeout)
            {
                obj2["Message"] = "登录超时";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            loginUser.LastOperateTime = DateTime.Now;
            CommonUtil.WaitMainLibInit();

            Device device = DeviceModule.GetDeviceByID(long.Parse(devID));
            if (device == null)
            {
                obj2["Result"] = true;
                obj2["Message"] = "noDevice";
                return JavaScriptConvert.SerializeObject(obj2);
            }

            string FullDeviceNo = DeviceModule.GetFullDeviceNoByID(device.Id);
            
            ResMsg msg = DeviceModule.DeleteDevice(long.Parse(devID));
            if (msg.Result)
            {
                GlobalAppModule.IsInitMainLib = true;
                try
                {
                    //更新网关设备缓存
                    Thread parameterThread = new Thread(new ParameterizedThreadStart(TcpRunThread.ParameterRun));
                    parameterThread.Start(ProtocolConst.WebToGateUpdateCache + ProtocolConst.UpdateCache_Device + "02" + FullDeviceNo.PadLeft(16, '0'));  
                }
                catch (Exception exception)
                {
                    //new Guard().Logger(exception, "GetOperateDevice");
                    myLogger.Error(exception.Message);
                }
                GlobalAppModule.IsInitMainLib = false;
                obj2["Result"] = true;
                obj2["Message"] = "成功";
            }
            else
            {
                obj2["Message"] = msg.Message;
            }
            try
            {
                //添加日志
                DeviceLog log = new DeviceLog();
                log.DeviceId = device.Id;
                log.LogUserId = loginUser.UserId;
                log.LogUserName = loginUser.LoginName;
                log.LogAddress = ToolsWeb.GetIP(context.Request);
                log.LogTime = loginUser.LastOperateTime;
                log.LogType = "删除";
                log.LogContent = msg.Message + " | " + JavaScriptConvert.SerializeObject(device);
                log.SimNo = device.SimNo;
                log.DeviceName = device.DeviceName;
                log.Description = device.Description;
                log.SetupDate = device.SetupDate;
                log.SetupAddress = device.SetupAddress;
                log.LON = device.LON;
                log.LAT = device.LAT;
                log.IsValid = device.IsValid;
                log.LastUpdate = device.LastUpdate;
                log.DistrictId = device.DistrictId;
                log.DeviceNo = device.DeviceNo;
                log.Online = device.Online;
                log.OnlineTime = device.OnlineTime;
                log.YearExploitation = device.YearExploitation;
                log.AlertAvailableWater = device.AlertAvailableWater;
                log.AlertAvailableElectric = device.AlertAvailableElectric;
                log.DeviceTypeCodeId = device.DeviceTypeCodeId;
                log.MeterPulse = device.MeterPulse;
                log.AlertWaterLevel = device.AlertWaterLevel;
                log.TerminalState = device.TerminalState;
                log.Remark = device.Remark;
                log.CropId = device.CropId;
                log.Area = device.Area;
                log.StationType = device.StationType;
                log.StationCode = device.StationCode;
                log.Frequency = device.Frequency;
                log.MainId = device.MainId;
                log.DeviceType = device.DeviceType;
                DeviceLogModule.Add(log);
            }
            catch
            {
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>获取指定管理下直属的设备或及其子管理下的设备，isRecursive为true是递归获取所有设备</span><br/><p>输入参数：loginIdentifer=登录标识，mnID=管理ID，isRecursive=是否递归，isExport=是否导出Excel<br/>返回数据格式：{'Result':bool,'Message':string,'DeviceNodes':[object1...objectn]}</p>")]
        public string GetDeviceNodeInfosByMnId(string loginIdentifer, string mnID, bool isRecursive, bool isExport)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            JavaScriptArray array = new JavaScriptArray();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("DeviceNodes", array);
            obj2.Add("ExcelURL", "");
            LoginUser loginUser = GlobalAppModule.GetLoginUser(loginIdentifer);
            if (loginUser == null)
            {
                obj2["Message"] = "未登录";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            if (loginUser.LoginTimeout)
            {
                obj2["Message"] = "登录超时";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            loginUser.LastOperateTime = DateTime.Now;
            CommonUtil.WaitMainLibInit();
            if (DistrictModule.ReturnDistrictInfo(long.Parse(mnID)) == null)
            {
                obj2["Message"] = "noExiste";
            }
            else
            {
                Device node = null;
                List<long> allDevicesForManageID = null;
                if (isRecursive)
                {
                    allDevicesForManageID = DeviceModule.GetAllDevicesForManageID(long.Parse(mnID));
                }
                else
                {
                    allDevicesForManageID = DeviceModule.GetDevicesForManageID(long.Parse(mnID));
                }
                for (int i = 0; i < allDevicesForManageID.Count; i++)
                {
                    node = DeviceModule.GetDeviceByID(allDevicesForManageID[i]);
                    if (node != null)
                    {
                        array.Add(this.DeviceNodeToJson(node));
                    }
                }
                obj2["Result"] = true;
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        private JavaScriptObject DeviceNodeToJson(Device device)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            //obj2["ID"] = device.Id;
            obj2.Add("ID", device.Id);
            obj2["SimNo"] = device.SimNo;
            //obj2["DeviceName"] = device.DeviceName;
            obj2.Add("名称", device.DeviceName);
            obj2["Description"] = device.Description;
            obj2["SetupDate"] = device.SetupDate.ToString("yyyy-MM-dd HH:mm");
            obj2["SetupAddress"] = device.SetupAddress;
            obj2["LON"] = device.LON / 1000000.0;
            obj2["LAT"] = device.LAT / 1000000.0;
            obj2["IsValid"] = device.IsValid;
            obj2["LastUpdate"] = device.LastUpdate.ToString("yyyy-MM-dd HH:mm");
            //obj2["DistrictId"] = device.DistrictId;
            obj2.Add("管理ID", device.DistrictId);
            string districtName = DistrictModule.GetDistrictName(device.DistrictId);
            if (districtName != null)
                obj2.Add("管理名称", districtName);
            else
                obj2.Add("管理名称", "未知");
            obj2["DeviceNo"] = device.DeviceNo;
            obj2["Online"] = device.Online;
            obj2["OnlineTime"] = device.OnlineTime;
            obj2["YearExploitation"] = device.YearExploitation;
            obj2["AlertAvailableWater"] = device.AlertAvailableWater;
            obj2["AlertAvailableElectric"] = device.AlertAvailableElectric;
            obj2["DeviceTypeCodeId"] = device.DeviceTypeCodeId;
            obj2["MeterPulse"] = device.MeterPulse;
            obj2["AlertWaterLevel"] = device.AlertWaterLevel;
            obj2["TerminalState"] = device.TerminalState;
            obj2["Remark"] = device.Remark;
            obj2["CropId"] = device.CropId;
            Crop crop = CropModule.GetCrop(device.CropId);
            if (crop != null)
                obj2["Crop"] = crop.CropName;
            else
                obj2["Crop"] = "未知";
            obj2["Area"] = device.Area;
            obj2["StationType"] = device.StationType.ToString().PadLeft(2,'0');
            obj2["StationCode"] = device.StationCode;
            //obj2["Frequency"] = device.Frequency.ToString("X").PadLeft(2, '0');

            switch (device.StationType)
            {
                case 0: obj2["StationTypeStr"] = "单站"; break;
                case 1: obj2["StationTypeStr"] = "主站"; break;
                case 2: obj2["StationTypeStr"] = "从站"; break;
                default: obj2["StationTypeStr"] = "单站"; break;
            }
            Device MainD = DeviceModule.GetDeviceByID(device.MainId);
            if (MainD != null)
            {
                obj2["MainDevNum"] = MainD.DeviceNo.PadLeft(3, '0');
            }
            else
            {
                obj2["MainDevNum"] = "";
            }
            obj2["DeviceType"] = device.DeviceType;
            obj2["SlaveList"] = "";
            obj2["RemoteStation"] = device.RemoteStation;
            List<Device> list = DeviceModule.GetAllDeviceSubList(device.Id);
            if (list != null && list.Count > 0)
            {
                JavaScriptArray array = new JavaScriptArray();
                foreach (Device dSub in list)
                {
                    JavaScriptObject objSub = new JavaScriptObject();
                    objSub.Add("ID", dSub.Id);
                    objSub.Add("名称", dSub.DeviceName);
                    objSub.Add("编号", dSub.DeviceNo);
                    objSub.Add("StationCode", dSub.StationCode);
                    objSub.Add("DeviceType", dSub.DeviceType);
                    array.Add(objSub);
                }
                obj2["SlaveList"] = array;
            }
            
            return obj2;
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>获取登录用户下的所能看到的设备信息</span><br/><p>输入参数：loginIdentifer=登录标识<br/>返回数据格式：{'Result':bool,'Message':string,'DeviceNodes':[object1...objectn]}</p>")]
        public string GetDeviceNodeInfosByLoginUser(string loginIdentifer)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            JavaScriptArray array = new JavaScriptArray();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("DeviceInfo", array);
            LoginUser loginUser = GlobalAppModule.GetLoginUser(loginIdentifer);
            if (loginUser == null)
            {
                obj2["Message"] = "未登录";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            if (loginUser.LoginTimeout)
            {
                obj2["Message"] = "登录超时";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            loginUser.LastOperateTime = DateTime.Now;
            return this.GetDeviceNodeInfosByMnId(loginIdentifer, SysUserModule.GetUser(loginUser.UserId).DistrictId.ToString(), true, false);
            /*
            string userInfo = "";
            if (!(UserModule.GetUserInfo(loginUser.UserId, "自定义测点") == "是"))
            {
                return this.GetDeviceNodeInfosByMnId(loginIdentifer, UserModule.GetUserInfo(loginUser.UserId, "管理ID"), true, false);
            }
            userInfo = UserModule.GetUserInfo(loginUser.UserId, "设备ID");
            if (userInfo != "")
            {
                string[] strArray = userInfo.Split(new char[] { ',' });
                DeviceNode deviceNodeByID = null;
                for (int i = 0; i < userInfo.Length; i++)
                {
                    deviceNodeByID = DevicesManager.GetDeviceNodeByID(strArray[i]);
                    if (deviceNodeByID != null)
                    {
                        array.Add(this.DeviceNodeToJson(deviceNodeByID));
                    }
                }
                obj2["Result"] = true;
            }
            return JavaScriptConvert.SerializeObject(obj2);
             * */
        }
    }
}
