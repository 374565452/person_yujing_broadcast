using Common;
using DTU.GateWay.Protocol;
using DTU.GateWay.Protocol.WaterMessageClass;
using Maticsoft.Model;
using Module;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Services;
using System.Xml;
using WaterMonitorSystem.Src;

namespace WaterMonitorSystem.WebServices
{
    /// <summary>
    /// DeviceMonitorService 的摘要说明
    /// </summary>
    [Serializable, WebService(Description = "提供设备监视服务服务，包括获取最后有效数据、获取通讯服务器中最新数据、即时召测现场设备数据、控制现场设备动作、读取或设置现场设备参数", Name = "设备监视服务", Namespace = "http://www.data86.net/"), WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class DeviceMonitorService : System.Web.Services.WebService
    {
        static log4net.ILog myLogger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private HttpContext context = HttpContext.Current;
        private int ctrlCommandTimeOut = 15;
        private static Dictionary<string, SwitchDisplayRule> dicSwitchDisplayRule;
        private JavaScriptArray jsaGetParams = new JavaScriptArray();

        public DeviceMonitorService()
        {
            this.context.Response.Buffer = true;
            this.context.Response.ExpiresAbsolute = DateTime.Now.AddSeconds(-1.0);
            this.context.Response.Expires = 0;
            this.context.Response.CacheControl = "no-cache";
            this.ctrlCommandTimeOut = GlobalAppModule.CommandTimeOut;
            if (dicSwitchDisplayRule == null)
            {
                this.GetRealTimeMonitorConfigInfo();
            }
        }

        private void GetRealTimeMonitorConfigInfo()
        {
            dicSwitchDisplayRule = new Dictionary<string, SwitchDisplayRule>();
            XmlDocument document = new XmlDocument();
            document.Load(this.context.Server.MapPath("~/App_Config/RealTimeMonitor.config"));
            try
            {
                XmlNode node = document.SelectSingleNode("RealTimeMonitor");
                if (((node != null) && (node.SelectSingleNode("/RealTimeMonitor/显示设置组/是否启用") != null)) && ((node.SelectSingleNode("/RealTimeMonitor/显示设置组/是否启用").InnerText.Trim() != "") && (node.SelectSingleNode("/RealTimeMonitor/显示设置组/是否启用").InnerText.ToLower().Trim() == "true")))
                {
                    XmlNodeList list = document.SelectNodes("/RealTimeMonitor/显示设置组/显示设置");
                    if ((list != null) && (list.Count > 0))
                    {
                        foreach (XmlNode node2 in list)
                        {
                            SwitchDisplayRule rule = new SwitchDisplayRule();
                            if ((node2["量名称"] != null) && (node2["量名称"].InnerText != ""))
                            {
                                rule.Name = node2["量名称"].InnerText;
                            }
                            if ((node2["显示方式"] != null) && (node2["显示方式"].InnerText != ""))
                            {
                                rule.SwitchDisplayMode = node2["显示方式"].InnerText;
                            }
                            if ((node2["量描述"] != null) && (node2["量描述"].InnerText != ""))
                            {
                                rule.SwitchDescription = node2["量描述"].InnerText.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                            }
                            if ((node2["量显示"] != null) && (node2["量显示"].InnerText != ""))
                            {
                                rule.SwitchDisplay = node2["量显示"].InnerText.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                            }
                            dicSwitchDisplayRule.Add(rule.Name, rule);
                        }
                    }
                }
            }
            catch
            {
            }
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>获取指定设备的最新的实时数据</span><br/><p>输入参数：loginIdentifer=登录用户标识，devIDs=设备ID,多个设备用','隔开<br/>返回数据格式：{'Result':bool,'Message':string,'DeviceInfo':object}</p>")]
        public string GetDeviceRealTimeDatas(string loginIdentifer, string devIDs)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            JavaScriptObject obj3 = new JavaScriptObject();
            JavaScriptObject obj4 = new JavaScriptObject();
            JavaScriptArray array = new JavaScriptArray();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("Columns", obj3);
            obj2.Add("SwitchDisplay", obj4);
            obj2.Add("DevDatas", array);
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
            List<long> allDevicesForManageID = new List<long>();
            if ((devIDs == null) || (devIDs.Trim() == ""))
            {
                allDevicesForManageID = DeviceModule.GetAllDevicesForManageID(SysUserModule.GetUser(loginUser.UserId).DistrictId);
            }
            else
            {
                foreach (string s in devIDs.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    try
                    {
                        allDevicesForManageID.Add(long.Parse(s));
                    }
                    catch { }
                }
            }
            List<string> templateIds = new List<string>();
            for (int i = 0; i < allDevicesForManageID.Count; i++)
            {
                templateIds.Add("1");
            }

            List<string> realTimeDataColumns = new List<string>();
            realTimeDataColumns.Add("村庄");
            realTimeDataColumns.Add("村庄Id");
            realTimeDataColumns.Add("设备");
            realTimeDataColumns.Add("经纬度");
            realTimeDataColumns.Add("设备类型");
            realTimeDataColumns.Add("通讯状态");
            realTimeDataColumns.Add("累计用水量");
            realTimeDataColumns.Add("运行状态");
            realTimeDataColumns.Add("更新时间");
            realTimeDataColumns.Add("设备编号");
            realTimeDataColumns.Add("长编号");
            realTimeDataColumns.Add("用户卡号");
            realTimeDataColumns.Add("卡序列号");
            realTimeDataColumns.Add("开泵时间");
            realTimeDataColumns.Add("开泵剩余水量");
            realTimeDataColumns.Add("开泵剩余电量");
            realTimeDataColumns.Add("关泵时间");
            realTimeDataColumns.Add("关泵剩余水量");
            realTimeDataColumns.Add("关泵剩余电量");
            realTimeDataColumns.Add("本次用水量");
            realTimeDataColumns.Add("本次用电量");
            realTimeDataColumns.Add("年累计用水量");
            realTimeDataColumns.Add("年累计用电量");
            realTimeDataColumns.Add("年可开采量");
            realTimeDataColumns.Add("年剩余可开采量");
            realTimeDataColumns.Add("流量仪表状态");
            realTimeDataColumns.Add("终端箱门状态");
            realTimeDataColumns.Add("IC卡功能有效");
            realTimeDataColumns.Add("水泵工作状态");
            realTimeDataColumns.Add("井剩余水量报警");
            realTimeDataColumns.Add("电表信号报警");
            realTimeDataColumns.Add("用户剩余水量报警");
            realTimeDataColumns.Add("用户剩余电量报警");
            realTimeDataColumns.Add("操作");
            List<string> showLevelAlias = new List<string>();
            showLevelAlias.Add("村庄");
            showLevelAlias.Add("设备");

            for (int j = 0; j < realTimeDataColumns.Count; j++)
            {
                JavaScriptObject obj5 = new JavaScriptObject();
                obj5.Add("Field", ChinesePY.GetPinYinIndex(realTimeDataColumns[j]) + ((j + 1)).ToString());
                obj5.Add("HeadText", realTimeDataColumns[j]);
                obj3.Add(realTimeDataColumns[j], obj5);
            }

            for (int k = 0; k < allDevicesForManageID.Count; k++)
            {
                JavaScriptObject obj7 = this.DevDatasToJson(allDevicesForManageID[k], showLevelAlias, realTimeDataColumns);
                if (obj7 != null)
                {
                    array.Add(obj7);
                }
            }
            obj2["Result"] = true;
            return JavaScriptConvert.SerializeObject(obj2);
        }

        private JavaScriptObject DevDatasToJson(long devId, List<string> liLevelAlias, List<string> displayColumns)
        {
            Device device = DeviceModule.GetDeviceByID(devId);
            if (device == null)
            {
                return null;
            }

            DeviceEvent[] array = DeviceEventModule.GetEventNewByDeviceNo(DeviceModule.GetFullDeviceNoByID(devId), DateTime.Now);

            District district = DistrictModule.ReturnDistrictInfo(device.DistrictId);
            JavaScriptObject obj2 = new JavaScriptObject();
            for (int i = 0; i < displayColumns.Count; i++)
            {
                string str = displayColumns[i];
                string str2 = ChinesePY.GetPinYinIndex(str) + ((i + 1)).ToString();
                JavaScriptObject obj3 = new JavaScriptObject();
                obj3.Add("Field", str2);
                switch (str)
                {
                    case "村庄": obj3.Add("Value", district.DistrictName); break;
                    case "村庄Id": obj3.Add("Value", district.Id); break;
                    case "设备": obj3.Add("Value", device.DeviceName); break;
                    case "经纬度": obj3.Add("Value", device.LON / 1000000.0 + "|" + device.LAT / 1000000.0); break;
                    case "设备类型": obj3.Add("Value", device.DeviceType); break;
                    case "通讯状态": obj3.Add("Value", device.Online == 1 && SystemService.isConnect ? "全部正常" : ""); break;
                    case "运行状态": if (device.Online == 1 && SystemService.isConnect) obj3.Add("Value", (array[0] == null ? "-" : (array[0].DeviceState[32 - 11] == '1' ? "水泵停机" : "水泵工作"))); else obj3.Add("Value", "未知"); break;
                    case "设备编号": obj3.Add("Value", device.DeviceNo); break;
                    case "长编号": obj3.Add("Value", DeviceModule.GetFullDeviceNoByID(device.Id)); break;
                    case "更新时间": obj3.Add("Value", device.LastUpdate.ToString("yyyy-MM-dd HH:mm:ss")); break;
                    case "操作": obj3.Add("Value", device.Id); break;

                    case "累计用水量": obj3.Add("Value", device.WaterUsed); break;

                    case "年累计用电量": obj3.Add("Value", (array[1] == null ? (array[2] == null ? "-" : array[2].YearElectricUsed.ToString()) : array[1].YearElectricUsed.ToString())); break;
                    case "年累计用水量": obj3.Add("Value", (array[1] == null ? (array[2] == null ? "-" : array[2].YearWaterUsed.ToString()) : array[1].YearWaterUsed.ToString())); break;
                    case "年剩余可开采量": obj3.Add("Value", (array[1] == null ? (array[2] == null ? "-" : array[2].YearSurplus.ToString()) : array[1].YearSurplus.ToString())); break;
                    case "井剩余水量报警": obj3.Add("Value", (array[0] == null ? "-" : (array[0].DeviceState[32 - 12] == '1' ? "水量超采" : "未超限"))); break;
                    case "水泵工作状态": obj3.Add("Value", (array[0] == null ? "-" : (array[0].DeviceState[32 - 11] == '1' ? "水泵停机" : "水泵工作"))); break;
                    case "用水户": obj3.Add("Value", (array[1] == null ? (array[2] == null ? "-" : array[2].UserNo.ToString()) : array[1].UserNo.ToString())); break;
                    case "开泵时间": obj3.Add("Value", (array[1] == null ? (array[2] == null ? "-" : array[2].StartTime.ToString()) : array[1].StartTime.ToString())); break;
                    case "关泵时间": obj3.Add("Value", (array[1] == null ? (array[2] == null ? "-" : array[2].EndTime.ToString()) : "-")); break;
                    case "灌溉时长": obj3.Add("Value", (array[1] == null ? (array[2] == null ? "-" : (array[2].EndTime - array[2].StartTime).TotalSeconds.ToString()) : "-")); break;
                    case "开泵剩余水量": obj3.Add("Value", (array[1] == null ? (array[2] == null ? "-" : array[2].StartResidualWater.ToString()) : array[1].StartResidualWater.ToString())); break;
                    case "关泵剩余水量": obj3.Add("Value", (array[1] == null ? (array[2] == null ? "-" : array[2].EndResidualWater.ToString()) : "-")); break;
                    case "本次用水量": obj3.Add("Value", (array[1] == null ? (array[2] == null ? "-" : array[2].WaterUsed.ToString()) : "-")); break;
                    case "开泵剩余电量": obj3.Add("Value", (array[1] == null ? (array[2] == null ? "-" : array[2].StartResidualElectric.ToString()) : array[1].StartResidualElectric.ToString())); break;
                    case "关泵剩余电量": obj3.Add("Value", (array[1] == null ? (array[2] == null ? "-" : array[2].EndResidualElectric.ToString()) : "-")); break;
                    case "本次用电量": obj3.Add("Value", (array[1] == null ? (array[2] == null ? "-" : array[2].ElectricUsed.ToString()) : "-")); break;

                    case "用户剩余水量报警": obj3.Add("Value", (array[0] == null ? "-" : (array[0].DeviceState[32 - 18] == '1' ? "报警" : "正常"))); break;
                    case "用户剩余电量报警": obj3.Add("Value", (array[0] == null ? "-" : (array[0].DeviceState[32 - 19] == '1' ? "报警" : "正常"))); break;
                    case "电表信号报警": obj3.Add("Value", (array[0] == null ? "-" : (array[0].DeviceState[32 - 16] == '1' ? "报警" : "正常"))); break;
                    case "流量仪表状态": obj3.Add("Value", (array[0] == null ? "-" : (array[0].DeviceState[32 - 6] == '1' ? "故障" : "正常"))); break;
                    case "终端箱门状态": obj3.Add("Value", (array[0] == null ? "-" : (array[0].DeviceState[32 - 8] == '1' ? "关闭" : "开启"))); break;
                    case "IC卡功能有效": obj3.Add("Value", (array[0] == null ? "-" : (array[0].DeviceState[32 - 10] == '1' ? "IC卡有效" : "关闭"))); break;
                    default: obj3.Add("Value", "未知"); break;
                }

                if (obj3.ContainsKey("Value"))
                {
                    obj2.Add(str, obj3);
                }
            }
            return obj2;
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>获取设备信息，包括即时刷新、控制参数、设备参数和监测数据</span><br/><p>输入参数：loginIdentifer=登录用户标识，devID=设备ID<br/>返回数据格式：{'Result':bool,'Message':string,'DeviceInfo':object}</p>")]
        public string GetDeviceInfoForWell(string loginIdentifer, string devID)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            JavaScriptObject obj3 = new JavaScriptObject();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("DeviceInfo", obj3);
            try
            {
                ResMsg msg = CommonUtil.CheckLoginState(loginIdentifer, true);
                if (!msg.Result)
                {
                    obj2["Message"] = msg.Message;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                Device device = DeviceModule.GetDeviceByID(long.Parse(devID));
                if (device == null)
                {
                    obj2["Message"] = "设备不存在";
                    return JavaScriptConvert.SerializeObject(obj2);
                }

                DeviceEvent[] array = DeviceEventModule.GetEventNewByDeviceNo(DeviceModule.GetFullDeviceNoByID(device.Id), DateTime.Now);

                obj3.Add("SupportRefresh", false);

                List<string> list = new List<string>();
                if (device.DeviceType.Contains("水泵"))
                {
                    list.Add("开泵");
                    list.Add("关泵");
                    list.Add("设置地址");
                    list.Add("状态查询");
                }
                obj3.Add("ControlNames", new JavaScriptArray(list.ToArray()));
                obj3.Add("SupportPhoto", false);
                obj3.Add("SupportControl", list.Count > 0);
                obj3.Add("SupportParam", device.DeviceType.Contains("水泵"));

                if (device.DeviceType.Contains("水位仪"))
                    obj3.Add("SupportWaterView", device.RemoteStation.ToString().Trim().Length > 0);

                JavaScriptArray array2 = new JavaScriptArray();
                obj3.Add("RealDatas", array2);
                JavaScriptObject item = new JavaScriptObject();
                item.Add("Name", "设备类型");
                item.Add("Value", device.DeviceType);
                array2.Add(item);
                item = new JavaScriptObject();
                item.Add("Name", "通讯状态");
                item.Add("Value", device.Online == 1 && SystemService.isConnect ? "正常" : "异常");
                array2.Add(item);
                item = new JavaScriptObject();
                item.Add("Name", "更新时间");
                item.Add("Value", device.LastUpdate.ToString("yyyy-MM-dd HH:mm:ss"));
                array2.Add(item);
                item = new JavaScriptObject();
                item.Add("Name", "设备编号");
                item.Add("Value", device.DeviceNo);
                array2.Add(item);
                item = new JavaScriptObject();
                item.Add("Name", "用水户");
                item.Add("Value", (array[1] == null ? (array[2] == null ? "-" : array[2].UserName.ToString()) : array[1].UserName.ToString()));
                array2.Add(item);
                item = new JavaScriptObject();
                item.Add("Name", "用户卡号");
                item.Add("Value", (array[1] == null ? (array[2] == null ? "-" : array[2].UserNo.ToString()) : array[1].UserNo.ToString()));
                array2.Add(item);
                item = new JavaScriptObject();
                item.Add("Name", "卡序列号");
                item.Add("Value", (array[1] == null ? (array[2] == null ? "-" : array[2].SerialNumber.ToString()) : array[1].SerialNumber.ToString()));
                array2.Add(item);
                item = new JavaScriptObject();
                item.Add("Name", "开泵时间");
                item.Add("Value", (array[1] == null ? (array[2] == null ? "-" : array[2].StartTime.ToString("yyyy-MM-dd HH:mm:ss")) : array[1].StartTime.ToString("yyyy-MM-dd HH:mm:ss")));
                array2.Add(item);
                item = new JavaScriptObject();
                item.Add("Name", "开泵剩余水量");
                item.Add("Value", (array[1] == null ? (array[2] == null ? "-" : array[2].StartResidualWater.ToString()) : array[1].StartResidualWater.ToString()));
                array2.Add(item);
                item = new JavaScriptObject();
                item.Add("Name", "开泵剩余电量");
                item.Add("Value", (array[1] == null ? (array[2] == null ? "-" : array[2].StartResidualElectric.ToString()) : array[1].StartResidualElectric.ToString()));
                array2.Add(item);
                item = new JavaScriptObject();
                item.Add("Name", "关泵时间");
                item.Add("Value", (array[1] == null ? (array[2] == null ? "-" : array[2].EndTime.ToString("yyyy-MM-dd HH:mm:ss")) : "-"));
                array2.Add(item);
                item = new JavaScriptObject();
                item.Add("Name", "关泵剩余水量");
                item.Add("Value", (array[1] == null ? (array[2] == null ? "-" : array[2].EndResidualWater.ToString()) : "-"));
                array2.Add(item);
                item = new JavaScriptObject();
                item.Add("Name", "关泵剩余电量");
                item.Add("Value", (array[1] == null ? (array[2] == null ? "-" : array[2].EndResidualElectric.ToString()) : "-"));
                array2.Add(item);
                item = new JavaScriptObject();
                item.Add("Name", "灌溉时长");
                item.Add("Value", (array[1] == null ? (array[2] == null ? "-" : (array[2].EndTime - array[2].StartTime).TotalSeconds.ToString()) : "-"));
                array2.Add(item);
                item = new JavaScriptObject();
                item.Add("Name", "本次用水量");
                item.Add("Value", (array[1] == null ? (array[2] == null ? "-" : array[2].WaterUsed.ToString()) : "-"));
                array2.Add(item);
                item = new JavaScriptObject();
                item.Add("Name", "本次用电量");
                item.Add("Value", (array[1] == null ? (array[2] == null ? "-" : array[2].ElectricUsed.ToString()) : "-"));
                array2.Add(item);
                item = new JavaScriptObject();
                item.Add("Name", "年累计用水量");
                item.Add("Value", (array[1] == null ? (array[2] == null ? "-" : array[2].YearWaterUsed.ToString()) : array[1].YearWaterUsed.ToString()));
                array2.Add(item);
                item = new JavaScriptObject();
                item.Add("Name", "年累计用电量");
                item.Add("Value", (array[1] == null ? (array[2] == null ? "-" : array[2].YearElectricUsed.ToString()) : array[1].YearElectricUsed.ToString()));
                array2.Add(item);
                item = new JavaScriptObject();
                item.Add("Name", "年可开采量");
                item.Add("Value", (array[1] == null ? (array[2] == null ? "-" : array[2].YearExploitation.ToString()) : array[1].YearExploitation.ToString()));
                array2.Add(item);
                item = new JavaScriptObject();
                item.Add("Name", "年剩余可开采量");
                item.Add("Value", (array[1] == null ? (array[2] == null ? "-" : array[2].YearSurplus.ToString()) : array[1].YearSurplus.ToString()));
                array2.Add(item);
                item = new JavaScriptObject();
                item.Add("Name", "流量仪表状态");
                item.Add("Value", (array[0] == null ? "-" : (array[0].DeviceState[26] == '1' ? "故障" : "正常")));
                array2.Add(item);
                item = new JavaScriptObject();
                item.Add("Name", "终端箱门状态");
                item.Add("Value", (array[0] == null ? "-" : (array[0].DeviceState[24] == '1' ? "关闭" : "开启")));
                array2.Add(item);
                item = new JavaScriptObject();
                item.Add("Name", "IC卡功能有效");
                item.Add("Value", (array[0] == null ? "-" : (array[0].DeviceState[22] == '1' ? "IC 卡有效" : "关闭")));
                array2.Add(item);
                item = new JavaScriptObject();
                item.Add("Name", "水泵工作状态");
                item.Add("Value", (array[0] == null ? "-" : (array[0].DeviceState[21] == '1' ? "停机" : "工作")));
                array2.Add(item);
                item = new JavaScriptObject();
                item.Add("Name", "井剩余水量报警");
                item.Add("Value", (array[0] == null ? "-" : (array[0].DeviceState[20] == '1' ? "超采" : "未超限")));
                array2.Add(item);
                item = new JavaScriptObject();
                item.Add("Name", "电表信号报警");
                item.Add("Value", (array[0] == null ? "-" : (array[0].DeviceState[16] == '1' ? "故障" : "正常")));
                array2.Add(item);
                item = new JavaScriptObject();
                item.Add("Name", "用户剩余水量报警");
                item.Add("Value", (array[0] == null ? "-" : (array[0].DeviceState[14] == '1' ? "故障" : "正常")));
                array2.Add(item);
                item = new JavaScriptObject();
                item.Add("Name", "用户剩余电量报警");
                item.Add("Value", (array[0] == null ? "-" : (array[0].DeviceState[13] == '1' ? "故障" : "正常")));
                item = new JavaScriptObject();
                item.Add("Name", "长编号");
                item.Add("Value", DeviceModule.GetFullDeviceNoByID(device.Id));
                array2.Add(item);
                item = new JavaScriptObject();
                item.Add("Name", "累计用水量");
                item.Add("Value", device.WaterUsed);
                array2.Add(item);

                obj2["Result"] = true;
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>获取指定设备的最新的实时数据</span><br/><p>输入参数：loginIdentifer=登录用户标识，devIDs=设备ID,多个设备用','隔开<br/>返回数据格式：{'Result':bool,'Message':string,'DeviceInfo':object}</p>")]
        public string GetDeviceRealTimeDatasForWell(string loginIdentifer, string devIDs)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            JavaScriptObject obj3 = new JavaScriptObject();
            JavaScriptObject obj4 = new JavaScriptObject();
            JavaScriptArray array = new JavaScriptArray();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("Columns", obj3);
            obj2.Add("SwitchDisplay", obj4);
            obj2.Add("DevDatas", array);
            try
            {
                ResMsg msg = CommonUtil.CheckLoginState(loginIdentifer, true);
                if (!msg.Result)
                {
                    obj2["Message"] = msg.Message;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                string message = msg.Message;
                List<long> allDevicesForManageID = new List<long>();
                if ((devIDs == null) || (devIDs.Trim() == ""))
                {
                    allDevicesForManageID = DeviceModule.GetAllDevicesForManageID(SysUserModule.GetUser(long.Parse(message)).DistrictId);
                }
                else
                {
                    foreach (string s in devIDs.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        try
                        {
                            allDevicesForManageID.Add(long.Parse(s));
                        }
                        catch { }
                    }
                }
                List<string> templateIds = new List<string>();
                for (int i = 0; i < allDevicesForManageID.Count; i++)
                {
                    templateIds.Add("1");
                }

                List<string> realTimeDataColumns = new List<string>();
                realTimeDataColumns.Add("村庄");
                realTimeDataColumns.Add("村庄Id");
                realTimeDataColumns.Add("设备");
                realTimeDataColumns.Add("设备类型");
                realTimeDataColumns.Add("用水户");
                realTimeDataColumns.Add("通讯状态");
                realTimeDataColumns.Add("设备状态");
                realTimeDataColumns.Add("累计用水量");
                realTimeDataColumns.Add("更新时间");
                realTimeDataColumns.Add("设备编号");
                realTimeDataColumns.Add("长编号");
                realTimeDataColumns.Add("用户卡号");
                realTimeDataColumns.Add("卡序列号");
                realTimeDataColumns.Add("开泵时间");
                realTimeDataColumns.Add("开泵剩余水量");
                realTimeDataColumns.Add("开泵剩余电量");
                realTimeDataColumns.Add("关泵时间");
                realTimeDataColumns.Add("关泵剩余水量");
                realTimeDataColumns.Add("关泵剩余电量");
                realTimeDataColumns.Add("灌溉时长");
                realTimeDataColumns.Add("本次用水量");
                realTimeDataColumns.Add("本次用电量");
                realTimeDataColumns.Add("年累计用水量");
                realTimeDataColumns.Add("年累计用电量");
                realTimeDataColumns.Add("年可开采量");
                realTimeDataColumns.Add("年剩余可开采量");
                realTimeDataColumns.Add("流量仪表状态");
                realTimeDataColumns.Add("终端箱门状态");
                realTimeDataColumns.Add("IC卡功能有效");
                realTimeDataColumns.Add("水泵工作状态");
                realTimeDataColumns.Add("井剩余水量报警");
                realTimeDataColumns.Add("电表信号报警");
                realTimeDataColumns.Add("用户剩余水量报警");
                realTimeDataColumns.Add("用户剩余电量报警");
                realTimeDataColumns.Add("操作");
                List<string> showLevelAlias = new List<string>();
                showLevelAlias.Add("村庄");
                showLevelAlias.Add("设备");

                for (int j = 0; j < realTimeDataColumns.Count; j++)
                {
                    JavaScriptObject obj5 = new JavaScriptObject();
                    obj5.Add("Field", ChinesePY.GetPinYinIndex(realTimeDataColumns[j]) + ((j + 1)).ToString());
                    obj5.Add("HeadText", realTimeDataColumns[j]);
                    obj3.Add(realTimeDataColumns[j], obj5);
                }

                for (int k = 0; k < allDevicesForManageID.Count; k++)
                {
                    JavaScriptObject obj7 = this.DevDatasToJsonForWell(allDevicesForManageID[k], showLevelAlias, realTimeDataColumns);
                    if (obj7 != null)
                    {
                        array.Add(obj7);
                    }
                }
                obj2["Result"] = true;
                myLogger.Info("查询终端列表数量：" + array.Count);
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        private JavaScriptObject DevDatasToJsonForWell(long devId, List<string> liLevelAlias, List<string> displayColumns)
        {
            Device device = DeviceModule.GetDeviceByID(devId);
            if (device == null)
            {
                return null;
            }

            string StationType = "未知";
            if (device.StationType == 0)
            {
                StationType = "单站";
            }
            else if (device.StationType == 1)
            {
                StationType = "主站";
            }
            else if (device.StationType == 2)
            {
                StationType = "从站";
            }
            DeviceEvent[] array = DeviceEventModule.GetEventNewByDeviceNo(DeviceModule.GetFullDeviceNoByID(device.Id), DateTime.Now);

            District district = DistrictModule.ReturnDistrictInfo(device.DistrictId);
            JavaScriptObject obj2 = new JavaScriptObject();
            for (int i = 0; i < displayColumns.Count; i++)
            {
                string str = displayColumns[i];
                string str2 = ChinesePY.GetPinYinIndex(str) + ((i + 1)).ToString();
                JavaScriptObject obj3 = new JavaScriptObject();
                obj3.Add("Field", str2);
                switch (str)
                {
                    case "村庄": obj3.Add("Value", district.DistrictName); break;
                    case "村庄Id": obj3.Add("Value", district.Id); break;
                    case "设备": obj3.Add("Value", device.DeviceName); break;
                    case "设备类型": obj3.Add("Value", device.DeviceType + "(" + StationType + ")"); break;
                    case "通讯状态": obj3.Add("Value", device.Online == 1 && SystemService.isConnect ? "全部正常" : ""); break;
                    case "设备状态": obj3.Add("Value", "未知"); break;
                    case "设备编号": obj3.Add("Value", device.DeviceNo); break;
                    case "长编号": obj3.Add("Value", DeviceModule.GetFullDeviceNoByID(device.Id)); break;
                    case "更新时间": obj3.Add("Value", device.LastUpdate.ToString("yyyy-MM-dd HH:mm:ss")); break;
                    case "操作": obj3.Add("Value", device.Id); break;

                    case "累计用水量": obj3.Add("Value", device.WaterUsed); break;

                    case "年累计用电量": obj3.Add("Value", (array[1] == null ? (array[2] == null ? "-" : array[2].YearElectricUsed.ToString()) : array[1].YearElectricUsed.ToString())); break;
                    case "年累计用水量": obj3.Add("Value", (array[1] == null ? (array[2] == null ? "-" : array[2].YearWaterUsed.ToString()) : array[1].YearWaterUsed.ToString())); break;
                    case "年剩余可开采量": obj3.Add("Value", (array[1] == null ? (array[2] == null ? "-" : array[2].YearSurplus.ToString()) : array[1].YearSurplus.ToString())); break;
                    case "井剩余水量报警": obj3.Add("Value", (array[0] == null ? "-" : (array[0].DeviceState[32 - 12] == '1' ? "水量超采" : "未超限"))); break;
                    case "水泵工作状态": obj3.Add("Value", (array[0] == null ? "-" : (array[0].DeviceState[32 - 11] == '1' ? "水泵停机" : "水泵工作"))); break;
                    case "用水户": obj3.Add("Value", (array[1] == null ? (array[2] == null ? "-" : array[2].UserNo.ToString()) : array[1].UserNo.ToString())); break;
                    case "开泵时间": obj3.Add("Value", (array[1] == null ? (array[2] == null ? "-" : array[2].StartTime.ToString()) : array[1].StartTime.ToString())); break;
                    case "关泵时间": obj3.Add("Value", (array[1] == null ? (array[2] == null ? "-" : array[2].EndTime.ToString()) : "-")); break;
                    case "灌溉时长": obj3.Add("Value", (array[1] == null ? (array[2] == null ? "-" : (array[2].EndTime - array[2].StartTime).TotalSeconds.ToString()) : "-")); break;
                    case "开泵剩余水量": obj3.Add("Value", (array[1] == null ? (array[2] == null ? "-" : array[2].StartResidualWater.ToString()) : array[1].StartResidualWater.ToString())); break;
                    case "关泵剩余水量": obj3.Add("Value", (array[1] == null ? (array[2] == null ? "-" : array[2].EndResidualWater.ToString()) : "-")); break;
                    case "本次用水量": obj3.Add("Value", (array[1] == null ? (array[2] == null ? "-" : array[2].WaterUsed.ToString()) : "-")); break;
                    case "开泵剩余电量": obj3.Add("Value", (array[1] == null ? (array[2] == null ? "-" : array[2].StartResidualElectric.ToString()) : array[1].StartResidualElectric.ToString())); break;
                    case "关泵剩余电量": obj3.Add("Value", (array[1] == null ? (array[2] == null ? "-" : array[2].EndResidualElectric.ToString()) : "-")); break;
                    case "本次用电量": obj3.Add("Value", (array[1] == null ? (array[2] == null ? "-" : array[2].ElectricUsed.ToString()) : "-")); break;

                    case "用户剩余水量报警": obj3.Add("Value", (array[0] == null ? "-" : (array[0].DeviceState[32 - 18] == '1' ? "报警" : "正常"))); break;
                    case "用户剩余电量报警": obj3.Add("Value", (array[0] == null ? "-" : (array[0].DeviceState[32 - 19] == '1' ? "报警" : "正常"))); break;
                    case "电表信号报警": obj3.Add("Value", (array[0] == null ? "-" : (array[0].DeviceState[32 - 16] == '1' ? "报警" : "正常"))); break;
                    case "流量仪表状态": obj3.Add("Value", (array[0] == null ? "-" : (array[0].DeviceState[32 - 6] == '1' ? "故障" : "正常"))); break;
                    case "终端箱门状态": obj3.Add("Value", (array[0] == null ? "-" : (array[0].DeviceState[32 - 8] == '1' ? "关闭" : "开启"))); break;
                    case "IC卡功能有效": obj3.Add("Value", (array[0] == null ? "-" : (array[0].DeviceState[32 - 10] == '1' ? "IC卡有效" : "关闭"))); break;
                    default: obj3.Add("Value", "未知"); break;
                }

                if (obj3.ContainsKey("Value"))
                {
                    obj2.Add(str, obj3);
                }
            }
            return obj2;
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>读取现场设备指定参数</span><br/><p>输入参数：loginIdentifer=登录用户标识，devID=设备ID，paramName=参数名称<br/>返回数据格式：{'Result':bool,'Message':string,'Params':object}</p>")]
        public string GetParam(string loginIdentifer, string devID, string paramName)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("Params", "");
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
            if (!SystemService.isConnect)
            {
                obj2["Message"] = "与通讯服务器连接中断";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            Device device = DeviceModule.GetDeviceByID(long.Parse(devID));
            if (device == null)
            {
                obj2["Message"] = "要操作的设备不存在";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            string DeviceNo = DeviceModule.GetFullDeviceNoByID(device.Id);

            DeviceOperation op = new DeviceOperation();
            op.DeviceNo = DeviceNo;
            op.DeviceName = device.DeviceName;
            op.OperationTime = DateTime.Now;
            //op.OperationType = "远程开泵";
            //op.RawData = cmd.RawDataStr;
            op.Remark = "";
            op.UserId = loginUser.UserId;
            op.UserName = SysUserModule.GetUser(loginUser.UserId).UserName;
            op.State = "等待发送";

            byte[] cmd_send = null;
            if (paramName == "遥测终端时钟")
            {
                CmdToDtuQueryDateTime cmd = new CmdToDtuQueryDateTime();
                cmd.AddressField = DeviceNo.Substring(0, 12) + Convert.ToInt32(DeviceNo.Substring(12, 3)).ToString("X").PadLeft(2, '0');
                cmd.StationType = (byte)device.StationType;
                cmd.StationCode = device.StationType == 2 ? device.StationCode : 0;
                cmd.RawDataChar = cmd.WriteMsg();
                cmd.RawDataStr = HexStringUtility.ByteArrayToHexString(cmd.RawDataChar);

                op.OperationType = "查询时间";
                op.RawData = cmd.RawDataStr;

                myLogger.Info(op.OperationType + "：" + cmd.RawDataStr);
                cmd_send = cmd.RawDataChar;
            }
            else if (paramName == "年可开采量")
            {
                CmdToDtuQueryYearExploitation cmd = new CmdToDtuQueryYearExploitation();
                cmd.AddressField = DeviceNo.Substring(0, 12) + Convert.ToInt32(DeviceNo.Substring(12, 3)).ToString("X").PadLeft(2, '0');
                cmd.StationType = (byte)device.StationType;
                cmd.StationCode = device.StationType == 2 ? device.StationCode : 0;
                cmd.RawDataChar = cmd.WriteMsg();
                cmd.RawDataStr = HexStringUtility.ByteArrayToHexString(cmd.RawDataChar);

                op.OperationType = "查询开采量";
                op.RawData = cmd.RawDataStr;

                myLogger.Info(op.OperationType + "：" + cmd.RawDataStr);
                cmd_send = cmd.RawDataChar;
            }
            else
            {
                obj2["Message"] = "参数非法";
                return JavaScriptConvert.SerializeObject(obj2);
            }

            /*开始发送命令读取参数*/
            ResMsg resMsg = SendCmd(cmd_send);

            if (resMsg.Result)
            {
                if (resMsg.Message == BaseProtocol.DeviceOffline)
                {
                    obj2["Message"] = "查询终端不在线";
                }
                else
                {
                    byte[] cmd_receive = HexStringUtility.HexStringToByteArray(resMsg.Message);

                    BaseMessage message = new BaseMessage();
                    message.RawDataChar = cmd_receive;
                    string msg = message.ReadMsg();
                    if (msg == "")
                    {
                        if (paramName == "遥测终端时钟")
                        {
                            CmdResponseToDtuQueryDateTime res = new CmdResponseToDtuQueryDateTime(message);
                            string msg1 = res.ReadMsg();
                            if (msg1 == "")
                            {
                                obj2["Result"] = true;
                                op.Remark = res.DateTimeNew.ToString("yyyy-MM-dd HH:mm:ss");
                                obj2["Params"] = res.DateTimeNew.ToString("yyyy-MM-dd HH:mm:ss");
                            }
                            else
                            {
                                obj2["Message"] = msg1;
                            }
                            op.State = "发送成功";
                        }
                        else if (paramName == "年可开采量")
                        {
                            CmdResponseToDtuQueryYearExploitation res = new CmdResponseToDtuQueryYearExploitation(message);
                            string msg1 = res.ReadMsg();
                            if (msg1 == "")
                            {
                                obj2["Result"] = true;
                                op.Remark = res.YearExploitation.ToString();
                                obj2["Params"] = res.YearExploitation.ToString();
                            }
                            else
                            {
                                obj2["Message"] = msg1;
                            }
                            op.State = "发送成功";
                        }
                    }
                    else
                    {
                        obj2["Message"] = msg;
                    }
                }
            }
            else
            {
                obj2["Message"] = resMsg.Message;
            }

            if (op.State != "发送成功")
            {
                op.State = "发送失败";
                op.State += "|" + obj2["Message"].ToString();
            }
            DeviceOperationModule.AddDeviceOperation(op);
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>设置现场设备指定参数</span><br/><p>输入参数：loginIdentifer=登录用户标识，devID=设备ID，paramName=参数名称，paramValue=参数值<br/>返回数据格式：{'Result':bool,'Message':string}</p>")]
        public string SetParam(string loginIdentifer, string devID, string paramName, string paramValue)
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
            if (!SystemService.isConnect)
            {
                obj2["Message"] = "与通讯服务器连接中断";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            Device device = DeviceModule.GetDeviceByID(long.Parse(devID));
            if (device == null)
            {
                obj2["Message"] = "要操作的设备不存在";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            string DeviceNo = DeviceModule.GetFullDeviceNoByID(device.Id);

            DeviceOperation op = new DeviceOperation();
            op.DeviceNo = DeviceNo;
            op.DeviceName = device.DeviceName;
            op.OperationTime = DateTime.Now;
            //op.OperationType = "远程开泵";
            //op.RawData = cmd.RawDataStr;
            op.Remark = "";
            op.UserId = loginUser.UserId;
            op.UserName = SysUserModule.GetUser(loginUser.UserId).UserName;
            op.State = "等待发送";

            byte[] cmd_send = null;
            if (paramName == "遥测终端时钟")
            {
                CmdToDtuSetDateTime cmd = new CmdToDtuSetDateTime();
                cmd.AddressField = DeviceNo.Substring(0, 12) + Convert.ToInt32(DeviceNo.Substring(12, 3)).ToString("X").PadLeft(2, '0');
                cmd.StationType = (byte)device.StationType;
                cmd.StationCode = device.StationType == 2 ? device.StationCode : 0;
                try
                {
                    cmd.DateTimeNew = DateTime.Parse(paramValue);
                }
                catch
                {
                    obj2["Message"] = "时间格式不正确！";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                cmd.RawDataChar = cmd.WriteMsg();
                cmd.RawDataStr = HexStringUtility.ByteArrayToHexString(cmd.RawDataChar);

                op.OperationType = "设置时间";
                op.RawData = cmd.RawDataStr;
                op.Remark = cmd.DateTimeNew.ToString("yyyy-MM-dd");

                myLogger.Info(op.OperationType + "：" + cmd.RawDataStr);
                cmd_send = cmd.RawDataChar;
            }
            else if (paramName == "年可开采量")
            {
                CmdToDtuSetYearExploitation cmd = new CmdToDtuSetYearExploitation();
                cmd.AddressField = DeviceNo.Substring(0, 12) + Convert.ToInt32(DeviceNo.Substring(12, 3)).ToString("X").PadLeft(2, '0');
                cmd.StationType = (byte)device.StationType;
                cmd.StationCode = device.StationType == 2 ? device.StationCode : 0;
                try
                {
                    cmd.YearExploitation = Convert.ToDecimal(paramValue);
                }
                catch
                {
                    obj2["Message"] = "年可开采量格式不正确！";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                cmd.RawDataChar = cmd.WriteMsg();
                cmd.RawDataStr = HexStringUtility.ByteArrayToHexString(cmd.RawDataChar);

                op.OperationType = "设置开采量";
                op.RawData = cmd.RawDataStr;
                op.Remark = cmd.YearExploitation.ToString();

                myLogger.Info(op.OperationType + "：" + cmd.RawDataStr);
                cmd_send = cmd.RawDataChar;
            }
            else if (paramName == "屏蔽")
            {
                CmdToDtuShieldSerialNumber cmd = new CmdToDtuShieldSerialNumber();
                cmd.AddressField = DeviceNo.Substring(0, 12) + Convert.ToInt32(DeviceNo.Substring(12, 3)).ToString("X").PadLeft(2, '0');
                cmd.StationType = (byte)device.StationType;
                cmd.StationCode = device.StationType == 2 ? device.StationCode : 0;

                if (paramValue.Length != 8)
                {
                    obj2["Message"] = "卡号长度只能为8位！";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                if (!Regex.IsMatch(paramValue, "^[0-9A-Fa-f]+$"))
                {
                    obj2["Message"] = "卡号只能为0-9A-Fa-f！";
                    return JavaScriptConvert.SerializeObject(obj2);
                }

                cmd.SerialNumber = paramValue;
                cmd.RawDataChar = cmd.WriteMsg();
                cmd.RawDataStr = HexStringUtility.ByteArrayToHexString(cmd.RawDataChar);

                op.OperationType = "屏蔽卡号";
                op.RawData = cmd.RawDataStr;
                op.Remark = cmd.SerialNumber.ToString();

                myLogger.Info(op.OperationType + "：" + cmd.RawDataStr);
                cmd_send = cmd.RawDataChar;
            }
            else if (paramName == "解除屏蔽")
            {
                CmdToDtuShieldSerialNumberCancel cmd = new CmdToDtuShieldSerialNumberCancel();
                cmd.AddressField = DeviceNo.Substring(0, 12) + Convert.ToInt32(DeviceNo.Substring(12, 3)).ToString("X").PadLeft(2, '0');
                cmd.StationType = (byte)device.StationType;
                cmd.StationCode = device.StationType == 2 ? device.StationCode : 0;

                if (paramValue.Length != 8)
                {
                    obj2["Message"] = "卡号长度只能为8位！";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                if (!Regex.IsMatch(paramValue, "^[0-9A-Fa-f]+$"))
                {
                    obj2["Message"] = "卡号只能为0-9A-Fa-f！";
                    return JavaScriptConvert.SerializeObject(obj2);
                }

                cmd.SerialNumber = paramValue;
                cmd.RawDataChar = cmd.WriteMsg();
                cmd.RawDataStr = HexStringUtility.ByteArrayToHexString(cmd.RawDataChar);

                op.OperationType = "解除屏蔽卡号";
                op.RawData = cmd.RawDataStr;
                op.Remark = cmd.SerialNumber.ToString();

                myLogger.Info(op.OperationType + "：" + cmd.RawDataStr);
                cmd_send = cmd.RawDataChar;
            }
            else
            {
                obj2["Message"] = "参数非法";
                return JavaScriptConvert.SerializeObject(obj2);
            }

            /*开始发送命令设置参数*/
            ResMsg resMsg = SendCmd(cmd_send);

            if (resMsg.Result)
            {
                if (resMsg.Message == BaseProtocol.DeviceOffline)
                {
                    obj2["Message"] = "查询终端不在线";
                }
                else
                {
                    byte[] cmd_receive = HexStringUtility.HexStringToByteArray(resMsg.Message);

                    BaseMessage message = new BaseMessage();
                    message.RawDataChar = cmd_receive;
                    string msg = message.ReadMsg();
                    if (msg == "")
                    {
                        if (paramName == "遥测终端时钟")
                        {
                            CmdResponseToDtuSetDateTime res = new CmdResponseToDtuSetDateTime(message);
                            string msg1 = res.ReadMsg();
                            if (msg1 == "")
                            {
                                obj2["Result"] = true;
                                obj2["Params"] = res.DateTimeNew.ToString("yyyy-MM-dd HH:mm:ss");
                            }
                            else
                            {
                                obj2["Message"] = msg1;
                            }
                            op.State = "发送成功";
                        }
                        else if (paramName == "年可开采量")
                        {
                            CmdResponseToDtuSetYearExploitation res = new CmdResponseToDtuSetYearExploitation(message);
                            string msg1 = res.ReadMsg();
                            if (msg1 == "")
                            {
                                obj2["Result"] = true;
                                obj2["Params"] = res.YearExploitation.ToString();
                            }
                            else
                            {
                                obj2["Message"] = msg1;
                            }
                            op.State = "发送成功";
                        }
                        else if (paramName == "屏蔽")
                        {
                            CmdResponseToDtuShieldSerialNumber res = new CmdResponseToDtuShieldSerialNumber(message);
                            string msg1 = res.ReadMsg();
                            if (msg1 == "")
                            {
                                obj2["Result"] = true;
                                obj2["Params"] = res.Result.ToString();
                            }
                            else
                            {
                                obj2["Message"] = msg1;
                            }
                            op.State = "发送成功";
                        }
                        else if (paramName == "解除屏蔽")
                        {
                            CmdResponseToDtuShieldSerialNumberCancel res = new CmdResponseToDtuShieldSerialNumberCancel(message);
                            string msg1 = res.ReadMsg();
                            if (msg1 == "")
                            {
                                obj2["Result"] = true;
                                obj2["Params"] = res.Result.ToString();
                            }
                            else
                            {
                                obj2["Message"] = msg1;
                            }
                            op.State = "发送成功";
                        }
                    }
                    else
                    {
                        obj2["Message"] = msg;
                    }
                }
            }
            else
            {
                obj2["Message"] = resMsg.Message;
            }

            if (op.State != "发送成功")
            {
                op.State = "发送失败";
                op.State += "|" + obj2["Message"].ToString();
            }
            DeviceOperationModule.AddDeviceOperation(op);
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>控制现场设备动作</span><br/><p>输入参数：loginIdentifer=登录用户标识，devID=设备ID，ctrlName=控制名称<br/>返回数据格式：{'Result':bool,'Message':string}</p>")]
        public string ControlDevice(string loginIdentifer, string devID, string ctrlName)
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
            if (!SystemService.isConnect)
            {
                obj2["Message"] = "与通讯服务器连接中断";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            Device device = DeviceModule.GetDeviceByID(long.Parse(devID));
            if (device == null)
            {
                obj2["Message"] = "要操作的设备不存在";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            string DeviceNo = DeviceModule.GetFullDeviceNoByID(device.Id);

            DeviceOperation op = new DeviceOperation();
            op.DeviceNo = DeviceNo;
            op.DeviceName = device.DeviceName;
            op.OperationTime = DateTime.Now;
            //op.OperationType = "远程开泵";
            //op.RawData = cmd.RawDataStr;
            op.Remark = "";
            op.UserId = loginUser.UserId;
            op.UserName = SysUserModule.GetUser(loginUser.UserId).UserName;
            op.State = "等待发送";

            byte[] cmd_send = null;
            if (ctrlName == "开泵")
            {
                CmdToDtuOpenPump cmd = new CmdToDtuOpenPump();
                cmd.AddressField = DeviceNo.Substring(0, 12) + Convert.ToInt32(DeviceNo.Substring(12, 3)).ToString("X").PadLeft(2, '0');
                cmd.StationType = (byte)device.StationType;
                cmd.StationCode = device.StationType == 2 ? device.StationCode : 0;
                cmd.RawDataChar = cmd.WriteMsg();
                cmd.RawDataStr = HexStringUtility.ByteArrayToHexString(cmd.RawDataChar);

                op.OperationType = "远程开泵";
                op.RawData = cmd.RawDataStr;

                myLogger.Info(op.OperationType + "：" + cmd.RawDataStr);
                cmd_send = cmd.RawDataChar;
            }
            else if (ctrlName == "关泵")
            {
                CmdToDtuClosePump cmd = new CmdToDtuClosePump();
                cmd.AddressField = DeviceNo.Substring(0, 12) + Convert.ToInt32(DeviceNo.Substring(12, 3)).ToString("X").PadLeft(2, '0');
                cmd.StationType = (byte)device.StationType;
                cmd.StationCode = device.StationType == 2 ? device.StationCode : 0;
                cmd.RawDataChar = cmd.WriteMsg();
                cmd.RawDataStr = HexStringUtility.ByteArrayToHexString(cmd.RawDataChar);

                op.OperationType = "远程关泵";
                op.RawData = cmd.RawDataStr;

                myLogger.Info(op.OperationType + "：" + cmd.RawDataStr);
                cmd_send = cmd.RawDataChar;
            }
            else if (ctrlName == "设置主站射频地址")
            {
                CmdToDtuSetStationCode cmd = new CmdToDtuSetStationCode();
                if (device.StationType == 0)
                {
                    obj2["Message"] = "非主从站无法设置分站射频地址！";
                    return JavaScriptConvert.SerializeObject(obj2);
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

                op.OperationType = "设置主站射频地址";
                op.RawData = cmd.RawDataStr;

                myLogger.Info(op.OperationType + "：" + cmd.RawDataStr);
                cmd_send = cmd.RawDataChar;
            }
            else if (ctrlName == "状态查询")
            {
                CmdToDtuQueryState cmd = new CmdToDtuQueryState();
                cmd.AddressField = DeviceNo.Substring(0, 12) + Convert.ToInt32(DeviceNo.Substring(12, 3)).ToString("X").PadLeft(2, '0');
                cmd.StationType = (byte)device.StationType;
                cmd.StationCode = device.StationType == 2 ? device.StationCode : 0;
                cmd.RawDataChar = cmd.WriteMsg();
                cmd.RawDataStr = HexStringUtility.ByteArrayToHexString(cmd.RawDataChar);

                op.OperationType = "状态查询";
                op.RawData = cmd.RawDataStr;

                myLogger.Info(op.OperationType + "：" + cmd.RawDataStr);
                cmd_send = cmd.RawDataChar;
            }
            else
            {
                obj2["Message"] = "参数非法";
                return JavaScriptConvert.SerializeObject(obj2);
            }

            /*开始发送命令控制终端*/
            ResMsg resMsg = SendCmd(cmd_send);

            if (resMsg.Result)
            {
                if (resMsg.Message == BaseProtocol.DeviceOffline)
                {
                    obj2["Message"] = "查询终端不在线";
                }
                else
                {
                    byte[] cmd_receive = HexStringUtility.HexStringToByteArray(resMsg.Message);

                    BaseMessage message = new BaseMessage();
                    message.RawDataChar = cmd_receive;
                    string msg = message.ReadMsg();
                    if (msg == "")
                    {
                        if (ctrlName == "开泵")
                        {
                            CmdResponseToDtuOpenPump res = new CmdResponseToDtuOpenPump(message);
                            string msg1 = res.ReadMsg();
                            if (msg1 == "")
                            {
                                if (res.Result == 1)
                                {
                                    obj2["Result"] = true;
                                }
                                else
                                {
                                    obj2["Message"] = "命令发送成功！开泵失败！";
                                }
                            }
                            else
                            {
                                obj2["Message"] = msg1;
                            }
                            op.State = "发送成功";
                        }
                        else if (ctrlName == "关泵")
                        {
                            CmdResponseToDtuClosePump res = new CmdResponseToDtuClosePump(message);
                            string msg1 = res.ReadMsg();
                            if (msg1 == "")
                            {
                                if (res.Result == 1)
                                {
                                    obj2["Result"] = true;
                                }
                                else
                                {
                                    obj2["Message"] = "命令发送成功！关泵失败！";
                                }
                            }
                            else
                            {
                                obj2["Message"] = msg1;
                            }
                            op.State = "发送成功";
                        }
                        else if (ctrlName == "设置主站射频地址")
                        {
                            CmdResponseToDtuSetStationCode res = new CmdResponseToDtuSetStationCode(message);
                            string msg1 = res.ReadMsg();
                            if (msg1 == "")
                            {
                                if (res.Result == 1)
                                {
                                    obj2["Result"] = true;
                                }
                                else
                                {
                                    obj2["Message"] = "命令发送成功！设置主站射频地址失败！";
                                }
                            }
                            else
                            {
                                obj2["Message"] = msg1;
                            }
                            op.State = "发送成功";
                        }
                        else if (ctrlName == "状态查询")
                        {
                            CmdResponseToDtuQueryState res = new CmdResponseToDtuQueryState(message);
                            string msg1 = res.ReadMsg();
                            if (msg1 == "")
                            {
                                obj2["Result"] = true;
                                obj2["Message"] = "状态查询成功！";
                            }
                            else
                            {
                                obj2["Message"] = msg1;
                            }
                            op.State = "发送成功";
                        }
                    }
                    else
                    {
                        obj2["Message"] = msg;
                    }
                }
            }
            else
            {
                obj2["Message"] = resMsg.Message;
            }

            if (op.State != "发送成功")
            {
                op.State = "发送失败";
                op.State += "|" + obj2["Message"].ToString();
            }
            DeviceOperationModule.AddDeviceOperation(op);
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>控制水位仪设备</span><br/><p>输入参数：loginIdentifer=登录用户标识，ctrlName=控制名称，devID=设备ID，Range=量程，LineLength=投入线长<br/>返回数据格式：{'Result':bool,'Message':string,'Range':string,'LineLength':string,'GroundWaterLevel':string,'Acp_Time':string}</p>")]
        public string ControlDevice2(string loginIdentifer, string devID, string ctrlName, string Range, string LineLength)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("Range", "");
            obj2.Add("LineLength", "");
            obj2.Add("GroundWaterLevel", "");
            obj2.Add("Acq_Time", "");
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
            if (!SystemService.isConnect)
            {
                obj2["Message"] = "与通讯服务器连接中断";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            Device device = DeviceModule.GetDeviceByID(long.Parse(devID));
            if (device == null)
            {
                obj2["Message"] = "要操作的设备不存在";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            string DeviceNo = DeviceModule.GetFullDeviceNoByID(device.Id);

            DeviceOperation op = new DeviceOperation();
            op.DeviceNo = DeviceNo;
            op.DeviceName = device.DeviceName;
            op.OperationTime = DateTime.Now;
            //op.OperationType = "远程开泵";
            //op.RawData = cmd.RawDataStr;
            op.Remark = "";
            op.UserId = loginUser.UserId;
            op.UserName = SysUserModule.GetUser(loginUser.UserId).UserName;
            op.State = "等待发送";

            byte[] cmd_send = null;
            if (ctrlName == "参数设置")
            {
                byte RangeTmp = 0;
                try
                {
                    RangeTmp = byte.Parse(Range);
                }
                catch
                {
                    obj2["Message"] = "量程必须为整数且范围是0-255！";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                double LineLengthTmp = -1;
                try
                {
                    LineLengthTmp = double.Parse(LineLength);
                }
                catch
                {
                    obj2["Message"] = "投入线长为小数点两位小数，范围0-655.35！";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                if (LineLengthTmp < 0 || LineLengthTmp > 655.35)
                {
                    obj2["Message"] = "投入线长为小数点两位小数，范围0-655.35！";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                CmdToDtuSetGroundWaterParam cmd = new CmdToDtuSetGroundWaterParam();
                cmd.AddressField = DeviceNo.Substring(0, 12) + Convert.ToInt32(DeviceNo.Substring(12, 3)).ToString("X").PadLeft(2, '0');
                cmd.StationType = (byte)device.StationType;
                cmd.StationCode = device.StationType == 2 ? device.StationCode : 0;
                cmd.Range = RangeTmp;
                cmd.LineLength = LineLengthTmp;
                cmd.RawDataChar = cmd.WriteMsg();
                cmd.RawDataStr = HexStringUtility.ByteArrayToHexString(cmd.RawDataChar);

                op.OperationType = "水位计参数设置";
                op.RawData = cmd.RawDataStr;

                myLogger.Info(op.OperationType + "：" + cmd.RawDataStr);
                cmd_send = cmd.RawDataChar;
            }
            else if (ctrlName == "参数查询")
            {
                CmdToDtuQueryGroundWaterParam cmd = new CmdToDtuQueryGroundWaterParam();
                cmd.AddressField = DeviceNo.Substring(0, 12) + Convert.ToInt32(DeviceNo.Substring(12, 3)).ToString("X").PadLeft(2, '0');
                cmd.StationType = (byte)device.StationType;
                cmd.StationCode = device.StationType == 2 ? device.StationCode : 0;
                cmd.RawDataChar = cmd.WriteMsg();
                cmd.RawDataStr = HexStringUtility.ByteArrayToHexString(cmd.RawDataChar);

                op.OperationType = "水位计参数查询";
                op.RawData = cmd.RawDataStr;

                myLogger.Info(op.OperationType + "：" + cmd.RawDataStr);
                cmd_send = cmd.RawDataChar;
            }
            else if (ctrlName == "水位查询")
            {
                CmdToDtuQueryGroundWater cmd = new CmdToDtuQueryGroundWater();
                cmd.AddressField = DeviceNo.Substring(0, 12) + Convert.ToInt32(DeviceNo.Substring(12, 3)).ToString("X").PadLeft(2, '0');
                cmd.StationType = (byte)device.StationType;
                cmd.StationCode = device.StationType == 2 ? device.StationCode : 0;
                cmd.RawDataChar = cmd.WriteMsg();
                cmd.RawDataStr = HexStringUtility.ByteArrayToHexString(cmd.RawDataChar);

                op.OperationType = "水位查询";
                op.RawData = cmd.RawDataStr;

                myLogger.Info(op.OperationType + "：" + cmd.RawDataStr);
                cmd_send = cmd.RawDataChar;
            }
            else
            {
                obj2["Message"] = "参数非法";
                return JavaScriptConvert.SerializeObject(obj2);
            }

            /*开始发送命令控制终端*/
            ResMsg resMsg = SendCmd(cmd_send);

            if (resMsg.Result)
            {
                if (resMsg.Message == BaseProtocol.DeviceOffline)
                {
                    obj2["Message"] = "查询终端不在线";
                }
                else
                {
                    byte[] cmd_receive = HexStringUtility.HexStringToByteArray(resMsg.Message);

                    BaseMessage message = new BaseMessage();
                    message.RawDataChar = cmd_receive;
                    string msg = message.ReadMsg();
                    if (msg == "")
                    {
                        if (ctrlName == "参数设置")
                        {
                            CmdResponseToDtuSetGroundWaterParam res = new CmdResponseToDtuSetGroundWaterParam(message);
                            string msg1 = res.ReadMsg();
                            if (msg1 == "")
                            {
                                if (res.Result == 1)
                                {
                                    obj2["Result"] = true;
                                }
                                else
                                {
                                    obj2["Message"] = "命令发送成功！参数设置失败！";
                                }
                            }
                            else
                            {
                                obj2["Message"] = msg1;
                            }
                            op.State = "发送成功";
                        }
                        else if (ctrlName == "参数查询")
                        {
                            CmdResponseToDtuQueryGroundWaterParam res = new CmdResponseToDtuQueryGroundWaterParam(message);
                            string msg1 = res.ReadMsg();
                            if (msg1 == "")
                            {
                                obj2["Result"] = true;
                                obj2["Range"] = res.Range.ToString();
                                obj2["LineLength"] = res.LineLength.ToString();
                            }
                            else
                            {
                                obj2["Message"] = msg1;
                            }
                            op.State = "发送成功";
                        }
                        else if (ctrlName == "水位查询")
                        {
                            CmdResponseToDtuQueryGroundWater res = new CmdResponseToDtuQueryGroundWater(message);
                            string msg1 = res.ReadMsg();
                            if (msg1 == "")
                            {
                                obj2["Result"] = true;
                                obj2["GroundWaterLevel"] = res.GroundWaterLevel.ToString();
                                obj2["LineLength"] = res.LineLength.ToString();
                                obj2["Acq_Time"] = res.Acq_Time.ToString("yyyy-MM-dd HH:mm:ss");
                            }
                            else
                            {
                                obj2["Message"] = msg1;
                            }
                            op.State = "发送成功";
                        }
                    }
                    else
                    {
                        obj2["Message"] = msg;
                    }
                }
            }
            else
            {
                obj2["Message"] = resMsg.Message;
            }

            if (op.State != "发送成功")
            {
                op.State = "发送失败";
                op.State += "|" + obj2["Message"].ToString();
            }
            DeviceOperationModule.AddDeviceOperation(op);
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>控制水位仪设备</span><br/><p>输入参数：loginIdentifer=登录用户标识，ctrlName=控制名称，devID=设备ID，BeginTime=开始时间，EndTime=结束时间，Identifier=查询要素<br/>返回数据格式：{'Result':bool,'Message':string}</p>")]
        public string ControlDevice3(string loginIdentifer, string devID, string ctrlName, string BeginTime, string EndTime, string Identifier, string step1, string step2, string step3)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("Info", "");
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
            if (!SystemService.isConnect)
            {
                obj2["Message"] = "与通讯服务器连接中断";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            Device device = DeviceModule.GetDeviceByID(long.Parse(devID));
            if (device == null)
            {
                obj2["Message"] = "要操作的设备不存在";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            string DeviceNo = DeviceModule.GetFullDeviceNoByID(device.Id);
            string RemoteStation = device.RemoteStation.Trim();
            if (RemoteStation == "")
            {
                obj2["Message"] = "要操作的设备无法操作";
                return JavaScriptConvert.SerializeObject(obj2);
            }

            /*
            obj2["Result"] = true;
            obj2["Info"] = ctrlName + " | " + RemoteStation + " | " 
                + time1.ToString("yyyy-MM-dd HH:mm:ss") + " | " + time2.ToString("yyyy-MM-dd HH:mm:ss") + " | " 
                + iden;
            */

            DeviceOperation op = new DeviceOperation();
            op.DeviceNo = DeviceNo;
            op.DeviceName = device.DeviceName;
            op.OperationTime = DateTime.Now;
            op.Remark = "";
            op.UserId = loginUser.UserId;
            op.UserName = SysUserModule.GetUser(loginUser.UserId).UserName;
            op.State = "等待发送";

            byte[] cmd_send = null;
            if (ctrlName == "时段数据查询")
            {
                DateTime time1 = DateTime.Now;
                try
                {
                    time1 = DateTime.Parse(BeginTime + ":0:0");
                }
                catch
                {
                    obj2["Message"] = "起始时间非法";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                DateTime time2 = time1;
                try
                {
                    time2 = DateTime.Parse(EndTime + ":0:0");
                }
                catch
                {
                    obj2["Message"] = "结束时间非法";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                string iden = "非法";
                int dd = 0;
                int HH = 0;
                int mm = 0;
                if (Identifier == "日埋深")
                {
                    iden = "FF0E";
                    dd = 1;
                }
                else if (Identifier == "日水温")
                {
                    iden = "FF03";
                    dd = 1;
                }
                else if (Identifier == "埋深")
                {
                    iden = "0E";
                    dd = 1;
                }
                else if (Identifier == "水温")
                {
                    iden = "03";
                    dd = 1;
                }
                else if (Identifier == "1小时时段降水量")
                {
                    iden = "1A";
                    HH = 1;
                }
                else if (Identifier == "日降水量")
                {
                    iden = "1F";
                    dd = 1;
                }
                else if (Identifier == "当前降水量")
                {
                    iden = "20";
                    HH = 1;
                }
                else if (Identifier == "降水量累计值")
                {
                    iden = "26";
                    HH = 1;
                }
                else if (Identifier == "瞬时水位")
                {
                    iden = "39";
                    HH = 1;
                }
                else if (Identifier == "1小时内每5分钟时段雨量") //48小时
                {
                    iden = "F4";
                    mm = 5;
                }
                else if (Identifier == "1小时内5分钟间隔相对水位1") //24小时
                {
                    iden = "F5";
                    mm = 5;
                }
                else
                {
                    obj2["Message"] = "查询要素非法";
                    return JavaScriptConvert.SerializeObject(obj2);
                }

                WaterCmd_38_1 cmd = new WaterCmd_38_1();
                cmd.CenterStation = WaterBaseProtocol.CenterStation;
                cmd.RemoteStation = RemoteStation.PadLeft(10, '0');
                cmd.PW = WaterBaseProtocol.PW;
                cmd.SerialNumber = 0;
                cmd.SendTime = DateTime.Now;
                cmd.StartTime = time1;
                cmd.EndTime = time2;
                //cmd.TimeSpan = "0418010000";
                cmd.Iden_04 = new Identifier_04();
                //cmd.Iden_04.SetVal(0, 0, "0418010000".Substring(4, 6));
                cmd.Iden_04.dd = dd;
                cmd.Iden_04.HH = HH;
                cmd.Iden_04.mm = mm;
                //cmd.Identifier = iden;
                byte Key = Convert.ToByte(iden.Substring(0, 2), 16);
                if (Key == (byte)Identifier_Standard._FF)
                {
                    byte KeySub = Convert.ToByte(iden.Substring(2, 2), 16);
                    if (KeySub == (byte)Identifier_Custom._03)
                    {
                        cmd.Iden = new Identifier_FF_03();
                    }
                    else if (KeySub == (byte)Identifier_Custom._0E)
                    {
                        cmd.Iden = new Identifier_FF_0E();
                    }
                }
                else
                {
                    if (Key == (byte)Identifier_Standard._03)
                    {
                        cmd.Iden = new Identifier_03();
                    }
                    else if (Key == (byte)Identifier_Standard._0E)
                    {
                        cmd.Iden = new Identifier_0E();
                    }
                    else if (Key == (byte)Identifier_Standard._1A)
                    {
                        cmd.Iden = new Identifier_1A();
                    }
                    else if (Key == (byte)Identifier_Standard._1F)
                    {
                        cmd.Iden = new Identifier_1F();
                    }
                    else if (Key == (byte)Identifier_Standard._20)
                    {
                        cmd.Iden = new Identifier_20();
                    }
                    else if (Key == (byte)Identifier_Standard._26)
                    {
                        cmd.Iden = new Identifier_26();
                    }
                    else if (Key == (byte)Identifier_Standard._38)
                    {
                        cmd.Iden = new Identifier_38();
                    }
                    else if (Key == (byte)Identifier_Standard._39)
                    {
                        cmd.Iden = new Identifier_39();
                    }
                    else if (Key == (byte)Identifier_Standard._F4)
                    {
                        cmd.Iden = new Identifier_F4();
                    }
                    else if (Key == (byte)Identifier_Standard._F5)
                    {
                        cmd.Iden = new Identifier_F5();
                    }
                }

                string msg = cmd.WriteMsg();
                if (msg == "")
                {
                    op.OperationType = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), cmd.AFN);
                    op.RawData = cmd.RawDataStr;

                    myLogger.Info(op.OperationType + "：" + cmd.RawDataStr);
                    cmd_send = cmd.RawDataChar;
                }
                else
                {
                    obj2["Message"] = "生成命令失败！" + msg;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
            }
            else if (ctrlName == "实时数据查询")
            {
                WaterCmd_37_1 cmd = new WaterCmd_37_1();
                cmd.CenterStation = WaterBaseProtocol.CenterStation;
                cmd.RemoteStation = RemoteStation.PadLeft(10, '0');
                cmd.PW = WaterBaseProtocol.PW;
                cmd.SerialNumber = 0;
                cmd.SendTime = DateTime.Now;
                string msg = cmd.WriteMsg();
                if (msg == "")
                {
                    op.OperationType = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), cmd.AFN);
                    op.RawData = cmd.RawDataStr;

                    myLogger.Info(op.OperationType + "：" + cmd.RawDataStr);
                    cmd_send = cmd.RawDataChar;
                }
                else
                {
                    obj2["Message"] = "生成命令失败！" + msg;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
            }
            else
            {
                obj2["Message"] = "参数非法";
                return JavaScriptConvert.SerializeObject(obj2);
            }

            ResMsg resMsg = SendCmd(cmd_send);

            if (resMsg.Result)
            {
                if (resMsg.Message == BaseProtocol.DeviceOffline)
                {
                    obj2["Message"] = "查询终端不在线";
                }
                else
                {
                    byte[] cmd_receive = HexStringUtility.HexStringToByteArray(resMsg.Message);
                    if (ctrlName == "时段数据查询")
                    {
                        WaterCmd_38_2 cmd = new WaterCmd_38_2();
                        string msg = cmd.ReadMsg(cmd_receive);
                        if (msg == "")
                        {
                            obj2["Result"] = true;
                            obj2["Info"] = cmd.ToString();
                        }
                        else
                        {
                            obj2["Message"] = "时段数据查询数据解析出错！" + msg;
                        }
                    }
                    else if (ctrlName == "实时数据查询")
                    {
                        WaterCmd_37_2 cmd = new WaterCmd_37_2();
                        string msg = cmd.ReadMsg(cmd_receive);
                        if (msg == "")
                        {
                            obj2["Result"] = true;
                            obj2["Info"] = cmd.ToString();
                        }
                        else
                        {
                            obj2["Message"] = "实时数据查询数据解析出错！" + msg;
                        }
                    }
                }
            }
            else
            {
                obj2["Message"] = resMsg.Message;
            }

            if (op.State != "发送成功")
            {
                op.State = "发送失败";
                op.State += "|" + obj2["Message"].ToString();
            }
            DeviceOperationModule.AddDeviceOperation(op);
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>控制水位仪设备参数</span><br/><p>输入参数：loginIdentifer=登录用户标识，ctrlName=控制名称，devID=设备ID，Params=参数列表<br/>返回数据格式：{'Result':bool,'Message':string}</p>")]
        public string ControlDevice4(string loginIdentifer, string devID, string ctrlName, string Params)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("Info", "");

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
            if (!SystemService.isConnect)
            {
                obj2["Message"] = "与通讯服务器连接中断";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            Device device = DeviceModule.GetDeviceByID(long.Parse(devID));
            if (device == null)
            {
                obj2["Message"] = "要操作的设备不存在";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            string DeviceNo = DeviceModule.GetFullDeviceNoByID(device.Id);
            string RemoteStation = device.RemoteStation.Trim();
            if (RemoteStation == "")
            {
                obj2["Message"] = "要操作的设备无法操作";
                return JavaScriptConvert.SerializeObject(obj2);
            }

            List<RTUParam> list = new List<RTUParam>();
            string[] kvs = Params.Trim(',').Split(',');
            if (ctrlName == "读取运行参数" || ctrlName == "设置运行参数")
            {
                foreach (string kv in kvs)
                {
                    try
                    {
                        RTUParam rp = null;
                        byte k = Convert.ToByte(kv.Split(':')[0].Trim(), 16);
                        if (k == (byte)RTUParam.RTUParamKey._20)
                        {
                            rp = new RTUParam_20();
                        }
                        else if (k == (byte)RTUParam.RTUParamKey._21)
                        {
                            rp = new RTUParam_21();
                        }
                        else if (k == (byte)RTUParam.RTUParamKey._22)
                        {
                            rp = new RTUParam_22();
                        }
                        else if (k == (byte)RTUParam.RTUParamKey._23)
                        {
                            rp = new RTUParam_23();
                        }
                        else if (k == (byte)RTUParam.RTUParamKey._24)
                        {
                            rp = new RTUParam_24();
                        }
                        else if (k == (byte)RTUParam.RTUParamKey._25)
                        {
                            rp = new RTUParam_25();
                        }
                        else if (k == (byte)RTUParam.RTUParamKey._26)
                        {
                            rp = new RTUParam_26();
                        }
                        else if (k == (byte)RTUParam.RTUParamKey._27)
                        {
                            rp = new RTUParam_27();
                        }
                        else if (k == (byte)RTUParam.RTUParamKey._28)
                        {
                            rp = new RTUParam_28();
                        }
                        else if (k == (byte)RTUParam.RTUParamKey._29)
                        {
                            rp = new RTUParam_29();
                        }
                        else if (k == (byte)RTUParam.RTUParamKey._2B)
                        {
                            rp = new RTUParam_2B();
                        }
                        else if (k == (byte)RTUParam.RTUParamKey._2C)
                        {
                            rp = new RTUParam_2C();
                        }
                        else if (k == (byte)RTUParam.RTUParamKey._2D)
                        {
                            rp = new RTUParam_2D();
                        }
                        else if (k == (byte)RTUParam.RTUParamKey._2E)
                        {
                            rp = new RTUParam_2E();
                        }
                        else if (k == (byte)RTUParam.RTUParamKey._2F)
                        {
                            rp = new RTUParam_2F();
                        }
                        else if (k == (byte)RTUParam.RTUParamKey._30)
                        {
                            rp = new RTUParam_30();
                        }
                        else if (k == (byte)RTUParam.RTUParamKey._31)
                        {
                            rp = new RTUParam_31();
                        }
                        else if (k == (byte)RTUParam.RTUParamKey._32)
                        {
                            rp = new RTUParam_32();
                        }
                        else if (k == (byte)RTUParam.RTUParamKey._33)
                        {
                            rp = new RTUParam_33();
                        }
                        else if (k == (byte)RTUParam.RTUParamKey._34)
                        {
                            rp = new RTUParam_34();
                        }
                        else if (k == (byte)RTUParam.RTUParamKey._35)
                        {
                            rp = new RTUParam_35();
                        }
                        else if (k == (byte)RTUParam.RTUParamKey._36)
                        {
                            rp = new RTUParam_36();
                        }
                        else if (k == (byte)RTUParam.RTUParamKey._37)
                        {
                            rp = new RTUParam_37();
                        }
                        else if (k == (byte)RTUParam.RTUParamKey._38)
                        {
                            rp = new RTUParam_38();
                        }
                        else if (k == (byte)RTUParam.RTUParamKey._39)
                        {
                            rp = new RTUParam_39();
                        }
                        else if (k == (byte)RTUParam.RTUParamKey._3B)
                        {
                            rp = new RTUParam_3B();
                        }
                        else if (k == (byte)RTUParam.RTUParamKey._3C)
                        {
                            rp = new RTUParam_3C();
                        }
                        else if (k == (byte)RTUParam.RTUParamKey._3D)
                        {
                            rp = new RTUParam_3D();
                        }
                        else if (k == (byte)RTUParam.RTUParamKey._3E)
                        {
                            rp = new RTUParam_3E();
                        }
                        else if (k == (byte)RTUParam.RTUParamKey._3F)
                        {
                            rp = new RTUParam_3F();
                        }
                        else if (k == (byte)RTUParam.RTUParamKey._40)
                        {
                            rp = new RTUParam_40();
                        }
                        else if (k == (byte)RTUParam.RTUParamKey._41)
                        {
                            rp = new RTUParam_41();
                        }

                        if (rp != null)
                        {
                            if (ctrlName == "读取运行参数")
                            {
                                list.Add(rp);
                            }
                            else if (ctrlName == "设置运行参数" && kv.Split(':')[1].Trim() != "")
                            {
                                rp.SetVal(kv.Split(':')[1].Trim());
                                list.Add(rp);
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }
            else if (ctrlName == "读取配置参数" || ctrlName == "设置配置参数")
            {
                int p1 = 0;
                int p2 = 0;
                int p3 = 0;
                int p4 = 0;
                foreach (string kv in kvs)
                {
                    try
                    {
                        RTUParam rp = null;
                        byte k = Convert.ToByte(kv.Split(':')[0].Trim(), 16);
                        if (k == (byte)RTUParam.RTUParamKey._01)
                        {
                            rp = new RTUParam_01();
                            if (ctrlName == "设置配置参数")
                            {
                                string val = kv.Split(':')[1];
                                string[] vals = val.Split('|');
                                ((RTUParam_01)rp).CenterStations = new int[4];
                                if (vals[0] != "")
                                    ((RTUParam_01)rp).CenterStations[0] = Convert.ToInt32(vals[0]);
                                else
                                    ((RTUParam_01)rp).CenterStations[0] = Convert.ToInt32(0);
                                if (vals[1] != "")
                                    ((RTUParam_01)rp).CenterStations[1] = Convert.ToInt32(vals[1]);
                                else
                                    ((RTUParam_01)rp).CenterStations[1] = Convert.ToInt32(0);
                                if (vals[2] != "")
                                    ((RTUParam_01)rp).CenterStations[2] = Convert.ToInt32(vals[2]);
                                else
                                    ((RTUParam_01)rp).CenterStations[2] = Convert.ToInt32(0);
                                if (vals[3] != "")
                                    ((RTUParam_01)rp).CenterStations[3] = Convert.ToInt32(vals[3]);
                                else
                                    ((RTUParam_01)rp).CenterStations[3] = Convert.ToInt32(0);

                                p1 = ((RTUParam_01)rp).CenterStations[0];
                                p2 = ((RTUParam_01)rp).CenterStations[1];
                                p3 = ((RTUParam_01)rp).CenterStations[2];
                                p4 = ((RTUParam_01)rp).CenterStations[3];
                            }
                            list.Add(rp);
                        }
                        else if (k == (byte)RTUParam.RTUParamKey._02)
                        {
                            rp = new RTUParam_02();
                            if (ctrlName == "设置配置参数")
                            {
                                string val = kv.Split(':')[1];
                                if (Tools.GetTest(val) == "数字")
                                    if (val.Length < 10)
                                        ((RTUParam_02)rp).RemoteStation = val.PadLeft(10, '0');
                                    else
                                        ((RTUParam_02)rp).RemoteStation = val.Substring(0, 10);
                                else
                                {
                                    obj2["Message"] = "遥测站地址必须为全数字";
                                    return JavaScriptConvert.SerializeObject(obj2);
                                }
                            }
                            list.Add(rp);
                        }
                        else if (k == (byte)RTUParam.RTUParamKey._03)
                        {
                            rp = new RTUParam_03();
                            if (ctrlName == "设置配置参数")
                            {
                                continue;
                            }
                            list.Add(rp);
                        }
                        else if (k == (byte)RTUParam.RTUParamKey._04)
                        {
                            //if (p1 == 0 && ctrlName == "设置配置参数") continue;
                            rp = new RTUParam_04();
                            if (ctrlName == "设置配置参数" && p1 > 0)
                            {
                                string val = kv.Split(':')[1];
                                string[] vals = val.Split('|');
                                if (vals.Length != 3)
                                {
                                    obj2["Message"] = "信道配置非法！" + val;
                                    return JavaScriptConvert.SerializeObject(obj2);
                                }
                                ((RTUParam_04)rp).ChannelTypeV = Convert.ToInt32(vals[0]);
                                if (((RTUParam_04)rp).ChannelTypeV == 2)
                                {
                                    string IP1 = vals[1].Split('.')[0];
                                    string IP2 = vals[1].Split('.')[1];
                                    string IP3 = vals[1].Split('.')[2];
                                    string IP4 = vals[1].Split('.')[3];
                                    ((RTUParam_04)rp).IP = IP1 + "." + IP2 + "." + IP3 + "." + IP4;
                                    ((RTUParam_04)rp).Port = Convert.ToInt32(vals[2]);
                                }
                                else
                                {
                                    ((RTUParam_04)rp).Add = vals[1];
                                }
                            }
                            list.Add(rp);
                        }
                        else if (k == (byte)RTUParam.RTUParamKey._05)
                        {
                            //if (p1 == 0 && ctrlName == "设置配置参数") continue;
                            rp = new RTUParam_05();
                            if (ctrlName == "设置配置参数" && p1 > 0)
                            {
                                string val = kv.Split(':')[1];
                                string[] vals = val.Split('|');
                                if (vals.Length != 3)
                                {
                                    obj2["Message"] = "信道配置非法！" + val;
                                    return JavaScriptConvert.SerializeObject(obj2);
                                }
                                ((RTUParam_05)rp).ChannelTypeV = Convert.ToInt32(vals[0]);
                                if (((RTUParam_05)rp).ChannelTypeV == 2)
                                {
                                    string IP1 = vals[1].Split('.')[0];
                                    string IP2 = vals[1].Split('.')[1];
                                    string IP3 = vals[1].Split('.')[2];
                                    string IP4 = vals[1].Split('.')[3];
                                    ((RTUParam_05)rp).IP = IP1 + "." + IP2 + "." + IP3 + "." + IP4;
                                    ((RTUParam_05)rp).Port = Convert.ToInt32(vals[2]);
                                }
                                else
                                {
                                    ((RTUParam_05)rp).Add = vals[1];
                                }
                            }
                            list.Add(rp);
                        }
                        else if (k == (byte)RTUParam.RTUParamKey._06)
                        {
                            //if (p2 == 0 && ctrlName == "设置配置参数") continue;
                            rp = new RTUParam_06();
                            if (ctrlName == "设置配置参数" && p2 > 0)
                            {
                                string val = kv.Split(':')[1];
                                string[] vals = val.Split('|');
                                if (vals.Length != 3)
                                {
                                    obj2["Message"] = "信道配置非法！" + val;
                                    return JavaScriptConvert.SerializeObject(obj2);
                                }
                                ((RTUParam_06)rp).ChannelTypeV = Convert.ToInt32(vals[0]);
                                if (((RTUParam_06)rp).ChannelTypeV == 2)
                                {
                                    string IP1 = vals[1].Split('.')[0];
                                    string IP2 = vals[1].Split('.')[1];
                                    string IP3 = vals[1].Split('.')[2];
                                    string IP4 = vals[1].Split('.')[3];
                                    ((RTUParam_06)rp).IP = IP1 + "." + IP2 + "." + IP3 + "." + IP4;
                                    ((RTUParam_06)rp).Port = Convert.ToInt32(vals[2]);
                                }
                                else
                                {
                                    ((RTUParam_06)rp).Add = vals[1];
                                }
                            }
                            list.Add(rp);
                        }
                        else if (k == (byte)RTUParam.RTUParamKey._07)
                        {
                            //if (p2 == 0 && ctrlName == "设置配置参数") continue;
                            rp = new RTUParam_07();
                            if (ctrlName == "设置配置参数" && p2 > 0)
                            {
                                string val = kv.Split(':')[1];
                                string[] vals = val.Split('|');
                                if (vals.Length != 3)
                                {
                                    obj2["Message"] = "信道配置非法！" + val;
                                    return JavaScriptConvert.SerializeObject(obj2);
                                }
                                ((RTUParam_07)rp).ChannelTypeV = Convert.ToInt32(vals[0]);
                                if (((RTUParam_07)rp).ChannelTypeV == 2)
                                {
                                    string IP1 = vals[1].Split('.')[0];
                                    string IP2 = vals[1].Split('.')[1];
                                    string IP3 = vals[1].Split('.')[2];
                                    string IP4 = vals[1].Split('.')[3];
                                    ((RTUParam_07)rp).IP = IP1 + "." + IP2 + "." + IP3 + "." + IP4;
                                    ((RTUParam_07)rp).Port = Convert.ToInt32(vals[2]);
                                }
                                else
                                {
                                    ((RTUParam_07)rp).Add = vals[1];
                                }
                            }
                            list.Add(rp);
                        }
                        else if (k == (byte)RTUParam.RTUParamKey._08)
                        {
                            //if (p3 == 0 && ctrlName == "设置配置参数") continue;
                            rp = new RTUParam_08();
                            if (ctrlName == "设置配置参数" && p3 > 0)
                            {
                                string val = kv.Split(':')[1];
                                string[] vals = val.Split('|');
                                if (vals.Length != 3)
                                {
                                    obj2["Message"] = "信道配置非法！" + val;
                                    return JavaScriptConvert.SerializeObject(obj2);
                                }
                                ((RTUParam_08)rp).ChannelTypeV = Convert.ToInt32(vals[0]);
                                if (((RTUParam_08)rp).ChannelTypeV == 2)
                                {
                                    string IP1 = vals[1].Split('.')[0];
                                    string IP2 = vals[1].Split('.')[1];
                                    string IP3 = vals[1].Split('.')[2];
                                    string IP4 = vals[1].Split('.')[3];
                                    ((RTUParam_08)rp).IP = IP1 + "." + IP2 + "." + IP3 + "." + IP4;
                                    ((RTUParam_08)rp).Port = Convert.ToInt32(vals[2]);
                                }
                                else
                                {
                                    ((RTUParam_08)rp).Add = vals[1];
                                }
                            }
                            list.Add(rp);
                        }
                        else if (k == (byte)RTUParam.RTUParamKey._09)
                        {
                            //if (p3 == 0 && ctrlName == "设置配置参数") continue;
                            rp = new RTUParam_09();
                            if (ctrlName == "设置配置参数" && p3 > 0)
                            {
                                string val = kv.Split(':')[1];
                                string[] vals = val.Split('|');
                                if (vals.Length != 3)
                                {
                                    obj2["Message"] = "信道配置非法！" + val;
                                    return JavaScriptConvert.SerializeObject(obj2);
                                }
                                ((RTUParam_09)rp).ChannelTypeV = Convert.ToInt32(vals[0]);
                                if (((RTUParam_09)rp).ChannelTypeV == 2)
                                {
                                    string IP1 = vals[1].Split('.')[0];
                                    string IP2 = vals[1].Split('.')[1];
                                    string IP3 = vals[1].Split('.')[2];
                                    string IP4 = vals[1].Split('.')[3];
                                    ((RTUParam_09)rp).IP = IP1 + "." + IP2 + "." + IP3 + "." + IP4;
                                    ((RTUParam_09)rp).Port = Convert.ToInt32(vals[2]);
                                }
                                else
                                {
                                    ((RTUParam_09)rp).Add = vals[1];
                                }
                            }
                            list.Add(rp);
                        }
                        else if (k == (byte)RTUParam.RTUParamKey._0A)
                        {
                            //if (p4 == 0 && ctrlName == "设置配置参数") continue;
                            rp = new RTUParam_0A();
                            if (ctrlName == "设置配置参数" && p4 > 0)
                            {
                                string val = kv.Split(':')[1];
                                string[] vals = val.Split('|');
                                if (vals.Length != 3)
                                {
                                    obj2["Message"] = "信道配置非法！" + val;
                                    return JavaScriptConvert.SerializeObject(obj2);
                                }
                                ((RTUParam_0A)rp).ChannelTypeV = Convert.ToInt32(vals[0]);
                                if (((RTUParam_0A)rp).ChannelTypeV == 2)
                                {
                                    string IP1 = vals[1].Split('.')[0];
                                    string IP2 = vals[1].Split('.')[1];
                                    string IP3 = vals[1].Split('.')[2];
                                    string IP4 = vals[1].Split('.')[3];
                                    ((RTUParam_0A)rp).IP = IP1 + "." + IP2 + "." + IP3 + "." + IP4;
                                    ((RTUParam_0A)rp).Port = Convert.ToInt32(vals[2]);
                                }
                                else
                                {
                                    ((RTUParam_0A)rp).Add = vals[1];
                                }
                            }
                            list.Add(rp);
                        }
                        else if (k == (byte)RTUParam.RTUParamKey._0B)
                        {
                            //if (p4 == 0 && ctrlName == "设置配置参数") continue;
                            rp = new RTUParam_0B();
                            if (ctrlName == "设置配置参数" && p4 > 0)
                            {
                                string val = kv.Split(':')[1];
                                string[] vals = val.Split('|');
                                if (vals.Length != 3)
                                {
                                    obj2["Message"] = "信道配置非法！" + val;
                                    return JavaScriptConvert.SerializeObject(obj2);
                                }
                                ((RTUParam_0B)rp).ChannelTypeV = Convert.ToInt32(vals[0]);
                                if (((RTUParam_0B)rp).ChannelTypeV == 2)
                                {
                                    string IP1 = vals[1].Split('.')[0];
                                    string IP2 = vals[1].Split('.')[1];
                                    string IP3 = vals[1].Split('.')[2];
                                    string IP4 = vals[1].Split('.')[3];
                                    ((RTUParam_0B)rp).IP = IP1 + "." + IP2 + "." + IP3 + "." + IP4;
                                    ((RTUParam_0B)rp).Port = Convert.ToInt32(vals[2]);
                                }
                                else
                                {
                                    ((RTUParam_0B)rp).Add = vals[1];
                                }
                            }
                            list.Add(rp);
                        }
                        else if (k == (byte)RTUParam.RTUParamKey._0C)
                        {
                            rp = new RTUParam_0C();
                            if (ctrlName == "设置配置参数")
                            {
                                string val = kv.Split(':')[1];
                                ((RTUParam_0C)rp).WorkTypeV = Convert.ToInt32(val);
                            }
                            list.Add(rp);
                        }
                        else if (k == (byte)RTUParam.RTUParamKey._0D)
                        {
                            rp = new RTUParam_0D();
                            if (ctrlName == "设置配置参数")
                            {
                                string val = kv.Split(':')[1];
                                string[] vals = val.Trim('|').Split('|');
                                string[] ss = new string[64];
                                for (int i = 0; i < ss.Length; i++)
                                {
                                    ss[i] = "0";
                                }
                                for (int i = 0; i < vals.Length; i++)
                                {
                                    ss[Convert.ToInt32(vals[i]) - 1] = "1";
                                }
                                string s = "";
                                for (int i = 0; i < ss.Length; i++)
                                {
                                    s += ss[i];
                                }
                                ((RTUParam_0D)rp).BitStr = s;
                            }
                            list.Add(rp);
                        }
                        else if (k == (byte)RTUParam.RTUParamKey._0F)
                        {
                            rp = new RTUParam_0F();
                            if (ctrlName == "设置配置参数")
                            {
                                string val = kv.Split(':')[1];
                                string[] vals = val.Split('|');
                                if (vals.Length != 2)
                                {
                                    obj2["Message"] = "通信设备识别号配置非法！" + val;
                                    return JavaScriptConvert.SerializeObject(obj2);
                                }
                                ((RTUParam_0F)rp).SimNoTypeV = Convert.ToInt32(vals[0]);
                                ((RTUParam_0F)rp).SimNo = vals[1];
                            }
                            list.Add(rp);
                        }
                    }
                    catch
                    {
                    }
                }
            }
            else
            {
                obj2["Message"] = "参数非法";
                return JavaScriptConvert.SerializeObject(obj2);
            }

            DeviceOperation op = new DeviceOperation();
            op.DeviceNo = DeviceNo;
            op.DeviceName = device.DeviceName;
            op.OperationTime = DateTime.Now;
            op.Remark = "";
            op.UserId = loginUser.UserId;
            op.UserName = SysUserModule.GetUser(loginUser.UserId).UserName;
            op.State = "等待发送";

            byte[] cmd_send = null;
            if (ctrlName == "读取运行参数")
            {
                WaterCmd_43_1 cmd = new WaterCmd_43_1();
                cmd.CenterStation = WaterBaseProtocol.CenterStation;
                cmd.RemoteStation = RemoteStation.PadLeft(10, '0');
                cmd.PW = WaterBaseProtocol.PW;
                cmd.SerialNumber = 0;
                cmd.SendTime = DateTime.Now;
                cmd.List_RTUParam = list;
                string msg = cmd.WriteMsg();
                if (msg == "")
                {
                    op.OperationType = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), cmd.AFN); ;
                    op.RawData = cmd.RawDataStr;

                    myLogger.Info(op.OperationType + "：" + cmd.RawDataStr);
                    cmd_send = cmd.RawDataChar;
                }
                else
                {
                    obj2["Message"] = "生成命令失败！" + msg;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
            }
            else if (ctrlName == "设置运行参数")
            {
                WaterCmd_42_1 cmd = new WaterCmd_42_1();
                cmd.CenterStation = WaterBaseProtocol.CenterStation;
                cmd.RemoteStation = RemoteStation.PadLeft(10, '0');
                cmd.PW = WaterBaseProtocol.PW;
                cmd.SerialNumber = 0;
                cmd.SendTime = DateTime.Now;
                cmd.List_RTUParam = list;
                string msg = cmd.WriteMsg();
                if (msg == "")
                {
                    op.OperationType = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), cmd.AFN); ;
                    op.RawData = cmd.RawDataStr;

                    myLogger.Info(op.OperationType + "：" + cmd.RawDataStr);
                    cmd_send = cmd.RawDataChar;
                }
                else
                {
                    obj2["Message"] = "生成命令失败！" + msg;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
            }
            if (ctrlName == "读取配置参数")
            {
                WaterCmd_41_1 cmd = new WaterCmd_41_1();
                cmd.CenterStation = WaterBaseProtocol.CenterStation;
                cmd.RemoteStation = RemoteStation.PadLeft(10, '0');
                cmd.PW = WaterBaseProtocol.PW;
                cmd.SerialNumber = 0;
                cmd.SendTime = DateTime.Now;
                cmd.List_RTUParam = list;
                string msg = cmd.WriteMsg();
                if (msg == "")
                {
                    op.OperationType = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), cmd.AFN); ;
                    op.RawData = cmd.RawDataStr;

                    myLogger.Info(op.OperationType + "：" + cmd.RawDataStr);
                    cmd_send = cmd.RawDataChar;
                }
                else
                {
                    obj2["Message"] = "生成命令失败！" + msg;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
            }
            else if (ctrlName == "设置配置参数")
            {
                WaterCmd_40_1 cmd = new WaterCmd_40_1();
                cmd.CenterStation = WaterBaseProtocol.CenterStation;
                cmd.RemoteStation = RemoteStation.PadLeft(10, '0');
                cmd.PW = WaterBaseProtocol.PW;
                cmd.SerialNumber = 0;
                cmd.SendTime = DateTime.Now;
                cmd.List_RTUParam = list;
                string msg = cmd.WriteMsg();
                if (msg == "")
                {
                    op.OperationType = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), cmd.AFN); ;
                    op.RawData = cmd.RawDataStr;

                    myLogger.Info(op.OperationType + "：" + cmd.RawDataStr);
                    cmd_send = cmd.RawDataChar;
                }
                else
                {
                    obj2["Message"] = "生成命令失败！" + msg;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
            }
            

            ResMsg resMsg = SendCmd(cmd_send);

            if (resMsg.Result)
            {
                if (resMsg.Message == BaseProtocol.DeviceOffline)
                {
                    obj2["Message"] = "查询终端不在线";
                }
                else
                {
                    byte[] cmd_receive = HexStringUtility.HexStringToByteArray(resMsg.Message);
                    if (ctrlName == "设置运行参数")
                    {
                        WaterCmd_42_2 cmd = new WaterCmd_42_2();
                        string msg = cmd.ReadMsg(cmd_receive);
                        if (msg == "")
                        {
                            obj2["Result"] = true;
                            string info = "";
                            foreach (RTUParam rp in cmd.List_RTUParam)
                            {
                                info += rp.GetKey().ToString("X") + ":" + rp.GetVal() + ",";
                            }
                            obj2["Info"] = info.Trim(',');
                        }
                        else
                        {
                            obj2["Message"] = "设置运行参数数据解析出错！" + msg;
                        }
                    }
                    else if (ctrlName == "读取运行参数")
                    {
                        WaterCmd_43_2 cmd = new WaterCmd_43_2();
                        string msg = cmd.ReadMsg(cmd_receive);
                        if (msg == "")
                        {
                            obj2["Result"] = true;
                            string info = "";
                            foreach (RTUParam rp in cmd.List_RTUParam)
                            {
                                info += rp.GetKey().ToString("X") + ":" + rp.GetVal() + ",";
                            }
                            obj2["Info"] = info.Trim(',');
                        }
                        else
                        {
                            obj2["Message"] = "读取运行参数数据解析出错！" + msg;
                        }
                    }
                    else if (ctrlName == "设置配置参数")
                    {
                        WaterCmd_40_2 cmd = new WaterCmd_40_2();
                        string msg = cmd.ReadMsg(cmd_receive);
                        if (msg == "")
                        {
                            obj2["Result"] = true;
                            string info = "";
                            foreach (RTUParam rp in cmd.List_RTUParam)
                            {
                                info += rp.GetKey().ToString("X").PadLeft(2, '0') + ":" + rp.GetVal() + ",";
                            }
                            obj2["Info"] = info.Trim(',');
                        }
                        else
                        {
                            obj2["Message"] = "设置配置参数数据解析出错！" + msg;
                        }
                    }
                    else if (ctrlName == "读取配置参数")
                    {
                        WaterCmd_41_2 cmd = new WaterCmd_41_2();
                        string msg = cmd.ReadMsg(cmd_receive);
                        if (msg == "")
                        {
                            obj2["Result"] = true;
                            string info = "";
                            foreach (RTUParam rp in cmd.List_RTUParam)
                            {
                                info += rp.GetKey().ToString("X").PadLeft(2,'0') + ":" + rp.GetVal() + ",";
                            }
                            obj2["Info"] = info.Trim(',');
                        }
                        else
                        {
                            obj2["Message"] = "读取配置参数数据解析出错！" + msg;
                        }
                    }
                }
            }
            else
            {
                obj2["Message"] = resMsg.Message;
            }

            if (op.State != "发送成功")
            {
                op.State = "发送失败";
                op.State += "|" + obj2["Message"].ToString();
            }
            DeviceOperationModule.AddDeviceOperation(op);
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>控制水位仪设备</span><br/><p>输入参数：loginIdentifer=登录用户标识，ctrlName=控制名称，devID=设备ID，Params=参数列表<br/>返回数据格式：{'Result':bool,'Message':string}</p>")]
        public string ControlDevice5(string loginIdentifer, string devID, string ctrlName, string Params)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("Info", "");
            obj2.Add("Vals", "");

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
            if (!SystemService.isConnect)
            {
                obj2["Message"] = "与通讯服务器连接中断";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            Device device = DeviceModule.GetDeviceByID(long.Parse(devID));
            if (device == null)
            {
                obj2["Message"] = "要操作的设备不存在";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            string DeviceNo = DeviceModule.GetFullDeviceNoByID(device.Id);
            string RemoteStation = device.RemoteStation.Trim();
            if (RemoteStation == "")
            {
                obj2["Message"] = "要操作的设备无法操作";
                return JavaScriptConvert.SerializeObject(obj2);
            }

            DeviceOperation op = new DeviceOperation();
            op.DeviceNo = DeviceNo;
            op.DeviceName = device.DeviceName;
            op.OperationTime = DateTime.Now;
            op.Remark = "";
            op.UserId = loginUser.UserId;
            op.UserName = SysUserModule.GetUser(loginUser.UserId).UserName;
            op.State = "等待发送";

            byte[] cmd_send = null;
            if (ctrlName == "设置IC卡状态")
            {
                WaterCmd_4B_1 cmd = new WaterCmd_4B_1();
                cmd.CenterStation = WaterBaseProtocol.CenterStation;
                cmd.RemoteStation = RemoteStation.PadLeft(10, '0');
                cmd.PW = WaterBaseProtocol.PW;
                cmd.SerialNumber = 0;
                cmd.SendTime = DateTime.Now;

                cmd.isUsed = Params == "1";

                string msg = cmd.WriteMsg();
                if (msg == "")
                {
                    op.OperationType = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), cmd.AFN);
                    op.RawData = cmd.RawDataStr;

                    myLogger.Info(op.OperationType + "：" + cmd.RawDataStr);
                    cmd_send = cmd.RawDataChar;
                }
                else
                {
                    obj2["Message"] = "生成命令失败！" + msg;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
            }
            else if (ctrlName == "水量定值控制")
            {
                WaterCmd_4F_1 cmd = new WaterCmd_4F_1();
                cmd.CenterStation = WaterBaseProtocol.CenterStation;
                cmd.RemoteStation = RemoteStation.PadLeft(10, '0');
                cmd.PW = WaterBaseProtocol.PW;
                cmd.SerialNumber = 0;
                cmd.SendTime = DateTime.Now;
                cmd.RC = Convert.ToByte(Params, 16);

                string msg = cmd.WriteMsg();
                if (msg == "")
                {
                    op.OperationType = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), cmd.AFN);
                    op.RawData = cmd.RawDataStr;

                    myLogger.Info(op.OperationType + "：" + cmd.RawDataStr);
                    cmd_send = cmd.RawDataChar;
                }
                else
                {
                    obj2["Message"] = "生成命令失败！" + msg;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
            }
            else if (ctrlName == "查询指定要素")
            {
                string[] Params_iden = Params.Trim('|').Split('|');
                if (Params_iden.Length == 0)
                {
                    obj2["Message"] = "生成命令失败！" + Params;
                    return JavaScriptConvert.SerializeObject(obj2);
                }

                WaterCmd_3A_1 cmd = new WaterCmd_3A_1();
                cmd.CenterStation = WaterBaseProtocol.CenterStation;
                cmd.RemoteStation = RemoteStation.PadLeft(10, '0');
                cmd.PW = WaterBaseProtocol.PW;
                cmd.SerialNumber = 0;
                cmd.SendTime = DateTime.Now;
                cmd.List_Identifier = new List<DTU.GateWay.Protocol.WaterMessageClass.Identifier>();
                for (int i = 0; i < Params_iden.Length; i++)
                {
                    if (Params_iden[i] == "FF0E")
                    {
                        cmd.List_Identifier.Add(new Identifier_FF_0E());
                    }
                    else if (Params_iden[i] == "FF03")
                    {
                        cmd.List_Identifier.Add(new Identifier_FF_03());
                    }
                    else if (Params_iden[i] == "0E")
                    {
                        cmd.List_Identifier.Add(new Identifier_0E());
                    }
                    else if (Params_iden[i] == "03")
                    {
                        cmd.List_Identifier.Add(new Identifier_03());
                    }
                    else if (Params_iden[i] == "1A")
                    {
                        cmd.List_Identifier.Add(new Identifier_1A());
                    }
                    else if (Params_iden[i] == "1F")
                    {
                        cmd.List_Identifier.Add(new Identifier_1F());
                    }
                    else if (Params_iden[i] == "20")
                    {
                        cmd.List_Identifier.Add(new Identifier_20());
                    }
                    else if (Params_iden[i] == "26")
                    {
                        cmd.List_Identifier.Add(new Identifier_26());
                    }
                    else if (Params_iden[i] == "39")
                    {
                        cmd.List_Identifier.Add(new Identifier_39());
                    }
                    else if (Params_iden[i] == "F4")
                    {
                        cmd.List_Identifier.Add(new Identifier_F4());
                    }
                    else if (Params_iden[i] == "F5")
                    {
                        cmd.List_Identifier.Add(new Identifier_F5());
                    }
                }

                string msg = cmd.WriteMsg();
                if (msg == "")
                {
                    op.OperationType = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), cmd.AFN);
                    op.RawData = cmd.RawDataStr;

                    myLogger.Info(op.OperationType + "：" + cmd.RawDataStr);
                    cmd_send = cmd.RawDataChar;
                }
                else
                {
                    obj2["Message"] = "生成命令失败！" + msg;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
            }
            else if (ctrlName == "修改密码")
            {
                if (Params.Split(',').Length != 2)
                {
                    obj2["Message"] = "生成命令失败！" + "密码格式错误！" + Params;
                    return JavaScriptConvert.SerializeObject(obj2);
                }

                try
                {
                    int i1 = Convert.ToInt32(Params.Split(',')[0]);
                    int i2 = Convert.ToInt32(Params.Split(',')[0]);
                    if (i1 < 0 || i1 > 0xFFFF || i2 < 0 || i2 > 0xFFFF)
                    {
                        obj2["Message"] = "生成命令失败！" + "密码格式错误！" + Params;
                        return JavaScriptConvert.SerializeObject(obj2);
                    }
                }
                catch
                {
                    obj2["Message"] = "生成命令失败！" + "密码格式错误！" + Params;
                    return JavaScriptConvert.SerializeObject(obj2);
                }

                WaterCmd_49_1 cmd = new WaterCmd_49_1();
                cmd.CenterStation = WaterBaseProtocol.CenterStation;
                cmd.RemoteStation = RemoteStation.PadLeft(10, '0');
                cmd.PW = WaterBaseProtocol.PW;
                cmd.SerialNumber = 0;
                cmd.SendTime = DateTime.Now;
                cmd.oldPW = new RTUParam_03();
                cmd.oldPW.Password = Params.Split(',')[0];
                cmd.newPW = new RTUParam_03();
                cmd.newPW.Password = Params.Split(',')[1];

                string msg = cmd.WriteMsg();
                if (msg == "")
                {
                    op.OperationType = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), cmd.AFN);
                    op.RawData = cmd.RawDataStr;

                    myLogger.Info(op.OperationType + "：" + cmd.RawDataStr);
                    cmd_send = cmd.RawDataChar;
                }
                else
                {
                    obj2["Message"] = "生成命令失败！" + msg;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
            }
            else if (ctrlName == "设置时钟")
            {
                WaterCmd_4A_1 cmd = new WaterCmd_4A_1();
                cmd.CenterStation = WaterBaseProtocol.CenterStation;
                cmd.RemoteStation = RemoteStation.PadLeft(10, '0');
                cmd.PW = WaterBaseProtocol.PW;
                cmd.SerialNumber = 0;
                cmd.SendTime = DateTime.Now;

                string msg = cmd.WriteMsg();
                if (msg == "")
                {
                    op.OperationType = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), cmd.AFN);
                    op.RawData = cmd.RawDataStr;

                    myLogger.Info(op.OperationType + "：" + cmd.RawDataStr);
                    cmd_send = cmd.RawDataChar;
                }
                else
                {
                    obj2["Message"] = "生成命令失败！" + msg;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
            }
            else if (ctrlName == "查询时钟")
            {
                WaterCmd_51_1 cmd = new WaterCmd_51_1();
                cmd.CenterStation = WaterBaseProtocol.CenterStation;
                cmd.RemoteStation = RemoteStation.PadLeft(10, '0');
                cmd.PW = WaterBaseProtocol.PW;
                cmd.SerialNumber = 0;
                cmd.SendTime = DateTime.Now;

                string msg = cmd.WriteMsg();
                if (msg == "")
                {
                    op.OperationType = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), cmd.AFN);
                    op.RawData = cmd.RawDataStr;

                    myLogger.Info(op.OperationType + "：" + cmd.RawDataStr);
                    cmd_send = cmd.RawDataChar;
                }
                else
                {
                    obj2["Message"] = "生成命令失败！" + msg;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
            }
            else if (ctrlName == "查询人工置数")
            {
                WaterCmd_39_1 cmd = new WaterCmd_39_1();
                cmd.CenterStation = WaterBaseProtocol.CenterStation;
                cmd.RemoteStation = RemoteStation.PadLeft(10, '0');
                cmd.PW = WaterBaseProtocol.PW;
                cmd.SerialNumber = 0;
                cmd.SendTime = DateTime.Now;

                string msg = cmd.WriteMsg();
                if (msg == "")
                {
                    op.OperationType = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), cmd.AFN);
                    op.RawData = cmd.RawDataStr;

                    myLogger.Info(op.OperationType + "：" + cmd.RawDataStr);
                    cmd_send = cmd.RawDataChar;
                }
                else
                {
                    obj2["Message"] = "生成命令失败！" + msg;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
            }
            else if (ctrlName == "查询电机实时")
            {
                WaterCmd_44_1 cmd = new WaterCmd_44_1();
                cmd.CenterStation = WaterBaseProtocol.CenterStation;
                cmd.RemoteStation = RemoteStation.PadLeft(10, '0');
                cmd.PW = WaterBaseProtocol.PW;
                cmd.SerialNumber = 0;
                cmd.SendTime = DateTime.Now;

                string msg = cmd.WriteMsg();
                if (msg == "")
                {
                    op.OperationType = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), cmd.AFN);
                    op.RawData = cmd.RawDataStr;

                    myLogger.Info(op.OperationType + "：" + cmd.RawDataStr);
                    cmd_send = cmd.RawDataChar;
                }
                else
                {
                    obj2["Message"] = "生成命令失败！" + msg;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
            }
            else if (ctrlName == "查询软件版本")
            {
                WaterCmd_45_1 cmd = new WaterCmd_45_1();
                cmd.CenterStation = WaterBaseProtocol.CenterStation;
                cmd.RemoteStation = RemoteStation.PadLeft(10, '0');
                cmd.PW = WaterBaseProtocol.PW;
                cmd.SerialNumber = 0;
                cmd.SendTime = DateTime.Now;

                string msg = cmd.WriteMsg();
                if (msg == "")
                {
                    op.OperationType = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), cmd.AFN);
                    op.RawData = cmd.RawDataStr;

                    myLogger.Info(op.OperationType + "：" + cmd.RawDataStr);
                    cmd_send = cmd.RawDataChar;
                }
                else
                {
                    obj2["Message"] = "生成命令失败！" + msg;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
            }
            else if (ctrlName == "查询状态报警")
            {
                WaterCmd_46_1 cmd = new WaterCmd_46_1();
                cmd.CenterStation = WaterBaseProtocol.CenterStation;
                cmd.RemoteStation = RemoteStation.PadLeft(10, '0');
                cmd.PW = WaterBaseProtocol.PW;
                cmd.SerialNumber = 0;
                cmd.SendTime = DateTime.Now;

                string msg = cmd.WriteMsg();
                if (msg == "")
                {
                    op.OperationType = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), cmd.AFN);
                    op.RawData = cmd.RawDataStr;

                    myLogger.Info(op.OperationType + "：" + cmd.RawDataStr);
                    cmd_send = cmd.RawDataChar;
                }
                else
                {
                    obj2["Message"] = "生成命令失败！" + msg;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
            }
            else if (ctrlName == "初始化数据")
            {
                WaterCmd_47_1 cmd = new WaterCmd_47_1();
                cmd.CenterStation = WaterBaseProtocol.CenterStation;
                cmd.RemoteStation = RemoteStation.PadLeft(10, '0');
                cmd.PW = WaterBaseProtocol.PW;
                cmd.SerialNumber = 0;
                cmd.SendTime = DateTime.Now;
                cmd.rp = new RTUParam_97();

                string msg = cmd.WriteMsg();
                if (msg == "")
                {
                    op.OperationType = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), cmd.AFN);
                    op.RawData = cmd.RawDataStr;

                    myLogger.Info(op.OperationType + "：" + cmd.RawDataStr);
                    cmd_send = cmd.RawDataChar;
                }
                else
                {
                    obj2["Message"] = "生成命令失败！" + msg;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
            }
            else if (ctrlName == "恢复出厂设置")
            {
                WaterCmd_48_1 cmd = new WaterCmd_48_1();
                cmd.CenterStation = WaterBaseProtocol.CenterStation;
                cmd.RemoteStation = RemoteStation.PadLeft(10, '0');
                cmd.PW = WaterBaseProtocol.PW;
                cmd.SerialNumber = 0;
                cmd.SendTime = DateTime.Now;
                cmd.rp = new RTUParam_98();

                string msg = cmd.WriteMsg();
                if (msg == "")
                {
                    op.OperationType = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), cmd.AFN);
                    op.RawData = cmd.RawDataStr;

                    myLogger.Info(op.OperationType + "：" + cmd.RawDataStr);
                    cmd_send = cmd.RawDataChar;
                }
                else
                {
                    obj2["Message"] = "生成命令失败！" + msg;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
            }
            else if (ctrlName == "查询事件记录")
            {
                WaterCmd_50_1 cmd = new WaterCmd_50_1();
                cmd.CenterStation = WaterBaseProtocol.CenterStation;
                cmd.RemoteStation = RemoteStation.PadLeft(10, '0');
                cmd.PW = WaterBaseProtocol.PW;
                cmd.SerialNumber = 0;
                cmd.SendTime = DateTime.Now;

                string msg = cmd.WriteMsg();
                if (msg == "")
                {
                    op.OperationType = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), cmd.AFN);
                    op.RawData = cmd.RawDataStr;

                    myLogger.Info(op.OperationType + "：" + cmd.RawDataStr);
                    cmd_send = cmd.RawDataChar;
                }
                else
                {
                    obj2["Message"] = "生成命令失败！" + msg;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
            }
            else if (ctrlName == "控制水泵开关")
            {
                string[] Params_Ps = Params.Trim('|').Split('|');
                if (Params_Ps.Length == 0)
                {
                    obj2["Message"] = "生成命令失败！" + Params;
                    return JavaScriptConvert.SerializeObject(obj2);
                }

                WaterCmd_4C_1 cmd = new WaterCmd_4C_1();
                cmd.CenterStation = WaterBaseProtocol.CenterStation;
                cmd.RemoteStation = RemoteStation.PadLeft(10, '0');
                cmd.PW = WaterBaseProtocol.PW;
                cmd.SerialNumber = 0;
                cmd.SendTime = DateTime.Now;
                
                cmd.Ps = new bool[Params_Ps.Length];
                for (int i = 0; i < Params_Ps.Length; i++)
                {
                    cmd.Ps[i] = Params_Ps[i] == "1";
                }

                string msg = cmd.WriteMsg();
                if (msg == "")
                {
                    op.OperationType = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), cmd.AFN);
                    op.RawData = cmd.RawDataStr;

                    myLogger.Info(op.OperationType + "：" + cmd.RawDataStr);
                    cmd_send = cmd.RawDataChar;
                }
                else
                {
                    obj2["Message"] = "生成命令失败！" + msg;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
            }
            else if (ctrlName == "控制阀门开关")
            {
                string[] Params_Ps = Params.Trim('|').Split('|');
                if (Params_Ps.Length == 0)
                {
                    obj2["Message"] = "生成命令失败！" + Params;
                    return JavaScriptConvert.SerializeObject(obj2);
                }

                WaterCmd_4D_1 cmd = new WaterCmd_4D_1();
                cmd.CenterStation = WaterBaseProtocol.CenterStation;
                cmd.RemoteStation = RemoteStation.PadLeft(10, '0');
                cmd.PW = WaterBaseProtocol.PW;
                cmd.SerialNumber = 0;
                cmd.SendTime = DateTime.Now;

                cmd.Ps = new bool[Params_Ps.Length];
                for (int i = 0; i < Params_Ps.Length; i++)
                {
                    cmd.Ps[i] = Params_Ps[i] == "1";
                }

                string msg = cmd.WriteMsg();
                if (msg == "")
                {
                    op.OperationType = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), cmd.AFN);
                    op.RawData = cmd.RawDataStr;

                    myLogger.Info(op.OperationType + "：" + cmd.RawDataStr);
                    cmd_send = cmd.RawDataChar;
                }
                else
                {
                    obj2["Message"] = "生成命令失败！" + msg;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
            }
            else if (ctrlName == "控制闸门开关")
            {
                string[] Params_Ps = Params.Trim('#').Split('#');
                if (Params_Ps.Length != 2)
                {
                    obj2["Message"] = "生成命令失败！" + Params;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                string[] Params_Ps1 = Params_Ps[0].Trim('|').Split('|');
                string[] Params_Ps2 = Params_Ps[1].Trim('|').Split('|');
                if (Params_Ps1.Length == 0 || Params_Ps1.Length != Params_Ps2.Length)
                {
                    obj2["Message"] = "生成命令失败！" + Params;
                    return JavaScriptConvert.SerializeObject(obj2);
                }

                WaterCmd_4E_1 cmd = new WaterCmd_4E_1();
                cmd.CenterStation = WaterBaseProtocol.CenterStation;
                cmd.RemoteStation = RemoteStation.PadLeft(10, '0');
                cmd.PW = WaterBaseProtocol.PW;
                cmd.SerialNumber = 0;
                cmd.SendTime = DateTime.Now;

                cmd.Ps = new bool[Params_Ps1.Length];
                cmd.PsOpen = new short[Params_Ps1.Length];
                for (int i = 0; i < Params_Ps1.Length; i++)
                {
                    cmd.Ps[i] = Params_Ps1[i] == "1";
                    cmd.PsOpen[i] = 0;
                    if (cmd.Ps[i])
                    {
                        cmd.PsOpen[i] = Convert.ToInt16(Params_Ps2[i]);
                    }
                }
                
                string msg = cmd.WriteMsg();
                if (msg == "")
                {
                    op.OperationType = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), cmd.AFN);
                    op.RawData = cmd.RawDataStr;

                    myLogger.Info(op.OperationType + "：" + cmd.RawDataStr);
                    cmd_send = cmd.RawDataChar;
                }
                else
                {
                    obj2["Message"] = "生成命令失败！" + msg;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
            }
            else if (ctrlName == "预警阈值设置")
            {
                string[] Params_Ps = Params.Trim('|').Split('|');
                if (Params_Ps.Length != 12)
                {
                    obj2["Message"] = "生成命令失败！" + Params;
                    return JavaScriptConvert.SerializeObject(obj2);
                }

                WaterCmd_20_1 cmd = new WaterCmd_20_1();
                cmd.CenterStation = WaterBaseProtocol.CenterStation;
                cmd.RemoteStation = RemoteStation.PadLeft(10, '0');
                cmd.PW = WaterBaseProtocol.PW;
                cmd.SerialNumber = 0;
                cmd.SendTime = DateTime.Now;
                cmd.Values = new byte[12];
                for (int i = 0; i < Params_Ps.Length; i++)
                {
                    cmd.Values[i] = 0;
                    try
                    {
                        cmd.Values[i] = Convert.ToByte(Params_Ps[i]);
                    }
                    catch
                    {
                        obj2["Message"] = "生成命令失败！" + "预警阀值取值0-255";
                        return JavaScriptConvert.SerializeObject(obj2);
                    }
                }
                string msg = cmd.WriteMsg();
                if (msg == "")
                {
                    op.OperationType = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), cmd.AFN);
                    op.RawData = cmd.RawDataStr;

                    myLogger.Info(op.OperationType + "：" + cmd.RawDataStr);
                    cmd_send = cmd.RawDataChar;
                }
                else
                {
                    obj2["Message"] = "生成命令失败！" + msg;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
            }
            else if (ctrlName == "预警阈值读取")
            {
                WaterCmd_22_1 cmd = new WaterCmd_22_1();
                cmd.CenterStation = WaterBaseProtocol.CenterStation;
                cmd.RemoteStation = RemoteStation.PadLeft(10, '0');
                cmd.PW = WaterBaseProtocol.PW;
                cmd.SerialNumber = 0;
                cmd.SendTime = DateTime.Now;
                string msg = cmd.WriteMsg();
                if (msg == "")
                {
                    op.OperationType = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), cmd.AFN);
                    op.RawData = cmd.RawDataStr;

                    myLogger.Info(op.OperationType + "：" + cmd.RawDataStr);
                    cmd_send = cmd.RawDataChar;
                }
                else
                {
                    obj2["Message"] = "生成命令失败！" + msg;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
            }
            else if (ctrlName == "预警发布")
            {
                string[] Params_Ps = Params.Trim('|').Split('|');
                if (Params_Ps.Length != 2)
                {
                    obj2["Message"] = "生成命令失败！" + Params;
                    return JavaScriptConvert.SerializeObject(obj2);
                }

                WaterCmd_21_1 cmd = new WaterCmd_21_1();
                cmd.CenterStation = WaterBaseProtocol.CenterStation;
                cmd.RemoteStation = RemoteStation.PadLeft(10, '0');
                cmd.PW = WaterBaseProtocol.PW;
                cmd.SerialNumber = 0;
                cmd.SendTime = DateTime.Now;
                try
                {
                    cmd.AlarmType = Convert.ToByte(Params_Ps[0]);
                }
                catch
                {
                    obj2["Message"] = "生成命令失败！" + "预警类型非法";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                try
                {
                    cmd.AlarmValue = Convert.ToByte(Params_Ps[1]);
                }
                catch
                {
                    obj2["Message"] = "生成命令失败！" + "预警级别非法";
                    return JavaScriptConvert.SerializeObject(obj2);
                }

                string msg = cmd.WriteMsg();
                if (msg == "")
                {
                    op.OperationType = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), cmd.AFN);
                    op.RawData = cmd.RawDataStr;

                    myLogger.Info(op.OperationType + "：" + cmd.RawDataStr);
                    cmd_send = cmd.RawDataChar;
                }
                else
                {
                    obj2["Message"] = "生成命令失败！" + msg;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
            }
            else if (ctrlName == "扩展参数设置")
            {
                string[] Params_Ps = Params.Trim('|').Split('|');
                //kqz 2016-12-31 修改
                //if (Params_Ps.Length != 2)
                if(Params_Ps.Length !=3)
                //kqz 2016-12-31 修改
                {
                    obj2["Message"] = "生成命令失败！" + Params;
                    return JavaScriptConvert.SerializeObject(obj2);
                }

                WaterCmd_25_1 cmd = new WaterCmd_25_1();
                cmd.CenterStation = WaterBaseProtocol.CenterStation;
                cmd.RemoteStation = RemoteStation.PadLeft(10, '0');
                cmd.PW = WaterBaseProtocol.PW;
                cmd.SerialNumber = 0;
                cmd.SendTime = DateTime.Now;
                try
                {
                    cmd.RType = Convert.ToByte(Params_Ps[0]);
                }
                catch
                {
                    obj2["Message"] = "生成命令失败！" + "遥测站类型非法";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                try
                {
                    cmd.IsSend = Convert.ToByte(Params_Ps[1]);
                }
                catch
                {
                    obj2["Message"] = "生成命令失败！" + "是否发送短信非法";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                //kqz 2016-12-31 增加
                try
                {
                    cmd.NumAuthenType = Convert.ToByte(Params_Ps[2]);
                }
                catch
                {
                    obj2["Message"] = "生成命令失败！" + "号码认证方式";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                //kqz 2016-12-31 增加
                string msg = cmd.WriteMsg();
                if (msg == "")
                {
                    op.OperationType = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), cmd.AFN);
                    op.RawData = cmd.RawDataStr;

                    myLogger.Info(op.OperationType + "：" + cmd.RawDataStr);
                    cmd_send = cmd.RawDataChar;
                }
                else
                {
                    obj2["Message"] = "生成命令失败！" + msg;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
            }
            else if (ctrlName == "扩展参数读取")
            {
                WaterCmd_26_1 cmd = new WaterCmd_26_1();
                cmd.CenterStation = WaterBaseProtocol.CenterStation;
                cmd.RemoteStation = RemoteStation.PadLeft(10, '0');
                cmd.PW = WaterBaseProtocol.PW;
                cmd.SerialNumber = 0;
                cmd.SendTime = DateTime.Now;

                string msg = cmd.WriteMsg();
                if (msg == "")
                {
                    op.OperationType = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), cmd.AFN);
                    op.RawData = cmd.RawDataStr;

                    myLogger.Info(op.OperationType + "：" + cmd.RawDataStr);
                    cmd_send = cmd.RawDataChar;
                }
                else
                {
                    obj2["Message"] = "生成命令失败！" + msg;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
            }
            //kqz 2017-1-1 添加
            else if (ctrlName == "水位预警阈值设置")
            {

                string[] Params_Ps = Params.Trim('|').Split('|');    
                if (Params_Ps.Length != 3)
                {
                    obj2["Message"] = "生成命令失败！" + Params;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                WaterCmd_28_1 cmd = new WaterCmd_28_1();
                cmd.CenterStation = WaterBaseProtocol.CenterStation;
                cmd.RemoteStation = RemoteStation.PadLeft(10, '0');
                cmd.PW = WaterBaseProtocol.PW;
                cmd.SerialNumber = 0;
                cmd.SendTime = DateTime.Now;

                try
                {
                    cmd.YellowLevel = Convert.ToInt16(Params_Ps[0]);
                }
                catch
                {
                    obj2["Message"] = "生成命令失败！" + "水位预警阈值黄色预警值类型非法";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                try
                {
                    cmd.OrangeLevel = Convert.ToInt16(Params_Ps[1]);
                }
                catch
                {
                    obj2["Message"] = "生成命令失败！" + "水位预警阈值橙色预警值类型非法";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
            
                try
                {
                    cmd.RedLevel = Convert.ToInt16(Params_Ps[2]);
                }
                catch
                {
                    obj2["Message"] = "生成命令失败！" + "水位预警阈值红色预警值类型非法";
                    return JavaScriptConvert.SerializeObject(obj2);
                }

                string msg = cmd.WriteMsg();
                if (msg == "")
                {
                    op.OperationType = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), cmd.AFN);
                    op.RawData = cmd.RawDataStr;

                    myLogger.Info(op.OperationType + "：" + cmd.RawDataStr);
                    cmd_send = cmd.RawDataChar;
                }
                else
                {
                    obj2["Message"] = "生成命令失败！" + msg;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
            }
            //kqz 2017-1-1 添加
            //kqz 2017-1-1 添加
            else if (ctrlName == "水位预警阈值读取")
            {
                WaterCmd_29_1 cmd = new WaterCmd_29_1();
                cmd.CenterStation = WaterBaseProtocol.CenterStation;
                cmd.RemoteStation = RemoteStation.PadLeft(10, '0');
                cmd.PW = WaterBaseProtocol.PW;
                cmd.SerialNumber = 0;
                cmd.SendTime = DateTime.Now;

                string msg = cmd.WriteMsg();
                if (msg == "")
                {
                    op.OperationType = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), cmd.AFN);
                    op.RawData = cmd.RawDataStr;

                    myLogger.Info(op.OperationType + "：" + cmd.RawDataStr);
                    cmd_send = cmd.RawDataChar;
                }
                else
                {
                    obj2["Message"] = "生成命令失败！" + msg;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
            }
            //kqz 2017-1-1 添加
            else if (ctrlName == "拍照")
            {
                WaterCmd_36_1 cmd = new WaterCmd_36_1();
                cmd.CenterStation = WaterBaseProtocol.CenterStation;
                cmd.RemoteStation = RemoteStation.PadLeft(10, '0');
                cmd.PW = WaterBaseProtocol.PW;
                cmd.SerialNumber = 0;
                cmd.SendTime = DateTime.Now;

                string msg = cmd.WriteMsg();
                if (msg == "")
                {
                    op.OperationType = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), cmd.AFN);
                    op.RawData = cmd.RawDataStr;

                    myLogger.Info(op.OperationType + "：" + cmd.RawDataStr);
                    cmd_send = cmd.RawDataChar;
                }
                else
                {
                    obj2["Message"] = "生成命令失败！" + msg;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
            }
            else
            {
                obj2["Message"] = "参数非法";
                return JavaScriptConvert.SerializeObject(obj2);
            }

            ResMsg resMsg = SendCmd(cmd_send);

            if (resMsg.Result)
            {
                if (resMsg.Message == BaseProtocol.DeviceOffline)
                {
                    obj2["Message"] = "查询终端不在线";
                }
                else if (resMsg.Message == BaseProtocol.DeviceSend)
                {
                    obj2["Message"] = "命令发送完成";
                }
                else
                {
                    byte[] cmd_receive = HexStringUtility.HexStringToByteArray(resMsg.Message);
 //System.Diagnostics.Debug.Print(resMsg.Message);
                    string msg_obj = "非法";
                    string info_obj = "";
                    if (ctrlName == "设置IC卡状态")
                    {
                        WaterCmd_4B_2 cmd = new WaterCmd_4B_2();
                        msg_obj = cmd.ReadMsg(cmd_receive);
                        info_obj = msg_obj == "" ? cmd.ToString() : "";
                    }
                    else if (ctrlName == "水量定值控制")
                    {
                        WaterCmd_4F_2 cmd = new WaterCmd_4F_2();
                        msg_obj = cmd.ReadMsg(cmd_receive);
                        info_obj = msg_obj == "" ? cmd.ToString() : "";
                    }
                    else if (ctrlName == "查询指定要素")
                    {
                        WaterCmd_3A_2 cmd = new WaterCmd_3A_2();
                        msg_obj = cmd.ReadMsg(cmd_receive);
                        info_obj = msg_obj == "" ? cmd.ToString() : "";
                    }
                    else if (ctrlName == "修改密码")
                    {
                        WaterCmd_49_2 cmd = new WaterCmd_49_2();
                        msg_obj = cmd.ReadMsg(cmd_receive);
                        info_obj = msg_obj == "" ? cmd.ToString() : "";
                    }
                    else if (ctrlName == "设置时钟")
                    {
                        WaterCmd_4A_2 cmd = new WaterCmd_4A_2();
                        msg_obj = cmd.ReadMsg(cmd_receive);
                        info_obj = msg_obj == "" ? cmd.ToString() : "";
                    }
                    else if (ctrlName == "查询时钟")
                    {
                        WaterCmd_51_2 cmd = new WaterCmd_51_2();
                        msg_obj = cmd.ReadMsg(cmd_receive);
                        info_obj = msg_obj == "" ? cmd.ToString() : "";
                    }
                    else if (ctrlName == "查询人工置数")
                    {
                        WaterCmd_39_2 cmd = new WaterCmd_39_2();
                        msg_obj = cmd.ReadMsg(cmd_receive);
                        info_obj = msg_obj == "" ? cmd.ToString() : "";
                    }
                    else if (ctrlName == "查询电机实时")
                    {
                        WaterCmd_44_2 cmd = new WaterCmd_44_2();
                        msg_obj = cmd.ReadMsg(cmd_receive);
                        info_obj = msg_obj == "" ? cmd.ToString() : "";
                    }
                    else if (ctrlName == "查询软件版本")
                    {
                        WaterCmd_45_2 cmd = new WaterCmd_45_2();
                        msg_obj = cmd.ReadMsg(cmd_receive);
                        info_obj = msg_obj == "" ? cmd.ToString() : "";
                    }
                    else if (ctrlName == "查询状态报警")
                    {
                        WaterCmd_46_2 cmd = new WaterCmd_46_2();
                        msg_obj = cmd.ReadMsg(cmd_receive);
                        info_obj = msg_obj == "" ? cmd.ToString() : "";
                    }
                    else if (ctrlName == "初始化数据")
                    {
                        WaterCmd_47_2 cmd = new WaterCmd_47_2();
                        msg_obj = cmd.ReadMsg(cmd_receive);
                        info_obj = msg_obj == "" ? cmd.ToString() : "";
                    }
                    else if (ctrlName == "恢复出厂设置")
                    {
                        WaterCmd_48_2 cmd = new WaterCmd_48_2();
                        msg_obj = cmd.ReadMsg(cmd_receive);
                        info_obj = msg_obj == "" ? cmd.ToString() : "";
                    }
                    else if (ctrlName == "查询事件记录")
                    {
                        WaterCmd_50_2 cmd = new WaterCmd_50_2();
                        msg_obj = cmd.ReadMsg(cmd_receive);
                        info_obj = msg_obj == "" ? cmd.ToString() : "";
                    }
                    else if (ctrlName == "控制水泵开关")
                    {
                        WaterCmd_4C_2 cmd = new WaterCmd_4C_2();
                        msg_obj = cmd.ReadMsg(cmd_receive);
                        info_obj = msg_obj == "" ? cmd.ToString() : "";
                    }
                    else if (ctrlName == "控制阀门开关")
                    {
                        WaterCmd_4D_2 cmd = new WaterCmd_4D_2();
                        msg_obj = cmd.ReadMsg(cmd_receive);
                        info_obj = msg_obj == "" ? cmd.ToString() : "";
                    }
                    else if (ctrlName == "控制闸门开关")
                    {
                        WaterCmd_4E_2 cmd = new WaterCmd_4E_2();
                        msg_obj = cmd.ReadMsg(cmd_receive);
                        info_obj = msg_obj == "" ? cmd.ToString() : "";
                    }
                    else if (ctrlName == "预警阈值设置")
                    {
                        WaterCmd_20_2 cmd = new WaterCmd_20_2();
                        msg_obj = cmd.ReadMsg(cmd_receive);
                        info_obj = msg_obj == "" ? cmd.ToString() : "";
                    }
                    else if (ctrlName == "预警阈值读取")
                    {
                        WaterCmd_22_2 cmd = new WaterCmd_22_2();
                        msg_obj = cmd.ReadMsg(cmd_receive);
                        info_obj = msg_obj == "" ? cmd.ToString() : "";
                        if (msg_obj == "" && cmd.Values.Length == 12)
                        {
                            string s = "";
                            foreach (byte b in cmd.Values)
                            {
                                s += b.ToString() + ",";
                            }
                            obj2["Vals"] = s.Trim(',');
                        }
                    }
                    else if (ctrlName == "预警发布")
                    {
                        WaterCmd_21_2 cmd = new WaterCmd_21_2();
                        msg_obj = cmd.ReadMsg(cmd_receive);
                        info_obj = msg_obj == "" ? cmd.ToString() : "";
                    }
                    else if (ctrlName == "扩展参数设置")
                    {
                        WaterCmd_25_2 cmd = new WaterCmd_25_2();
                        msg_obj = cmd.ReadMsg(cmd_receive);
                        info_obj = msg_obj == "" ? cmd.ToString() : "";
                    }
                    //kqz 2017-1-1 添加
                    else if (ctrlName == "水位预警阈值设置")
                    {
                        WaterCmd_28_2 cmd = new WaterCmd_28_2();
                        msg_obj = cmd.ReadMsg(cmd_receive);
                        info_obj = msg_obj == "" ? cmd.ToString() : "";
                    }
                    //kqz 2017-1-1 添加
                    else if (ctrlName == "扩展参数读取")
                    {
                        WaterCmd_26_2 cmd = new WaterCmd_26_2();
                        msg_obj = cmd.ReadMsg(cmd_receive);
                        info_obj = msg_obj == "" ? cmd.ToString() : "";
                        obj2["Vals"] = cmd.RType + "," + cmd.IsSend + "," + cmd.NumAuthenType;
                    }
                    //kqz 2017-1-1 添加
                    else if (ctrlName == "水位预警阈值读取")
                    {
                        WaterCmd_29_2 cmd = new WaterCmd_29_2();
                        msg_obj = cmd.ReadMsg(cmd_receive);
                        info_obj = msg_obj == "" ? cmd.ToString() : "";
                        obj2["Vals"] = cmd.YellowLevel + "," + cmd.OrangeLevel + "," + cmd.RedLevel;
                    }
                    //kqz 2017-1-1 添加
                    obj2["Result"] = msg_obj == "";
                    obj2["Info"] = info_obj;
                    obj2["Message"] = msg_obj == "" ? "" : ctrlName + "数据解析出错！" + msg_obj;
                }
            }
            else
            {
                obj2["Message"] = resMsg.Message;
            }

            if (op.State != "发送成功")
            {
                op.State = "发送失败";
                op.State += "|" + obj2["Message"].ToString();
            }

            DeviceOperationModule.AddDeviceOperation(op);
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>控制水位仪设备预警责任人</span><br/><p>输入参数：loginIdentifer=登录用户标识，ctrlName=控制名称，devID=设备ID，OrderNum=序号，Phone=号码<br/>返回数据格式：{'Result':bool,'Message':string}</p>")]
        public string ControlDevice6(string loginIdentifer, string devID, string ctrlName, string OrderNum, string Phone)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("Info", "");
            obj2.Add("OrderNum", "");
            obj2.Add("Phone", "");

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
            if (!SystemService.isConnect)
            {
                obj2["Message"] = "与通讯服务器连接中断";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            Device device = DeviceModule.GetDeviceByID(long.Parse(devID));
            if (device == null)
            {
                obj2["Message"] = "要操作的设备不存在";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            string DeviceNo = DeviceModule.GetFullDeviceNoByID(device.Id);
            string RemoteStation = device.RemoteStation.Trim();
            if (RemoteStation == "")
            {
                obj2["Message"] = "要操作的设备无法操作";
                return JavaScriptConvert.SerializeObject(obj2);
            }

            DeviceOperation op = new DeviceOperation();
            op.DeviceNo = DeviceNo;
            op.DeviceName = device.DeviceName;
            op.OperationTime = DateTime.Now;
            op.Remark = "";
            op.UserId = loginUser.UserId;
            op.UserName = SysUserModule.GetUser(loginUser.UserId).UserName;
            op.State = "等待发送";

            byte[] cmd_send = null;
            if (ctrlName == "设置")
            {
                WaterCmd_23_1 cmd = new WaterCmd_23_1();
                cmd.CenterStation = WaterBaseProtocol.CenterStation;
                cmd.RemoteStation = RemoteStation.PadLeft(10, '0');
                cmd.PW = WaterBaseProtocol.PW;
                cmd.SerialNumber = 0;
                cmd.SendTime = DateTime.Now;

                try
                {
                    cmd.OrderNum = Convert.ToByte(OrderNum);
                }
                catch
                {
                    obj2["Message"] = "生成命令失败！" + "序号格式错误！" + OrderNum;
                    return JavaScriptConvert.SerializeObject(obj2);
                }

                if (Phone.Trim().Length > 11 || (Phone.Trim() != "" && Tools.GetTest(Phone.Trim()) != "数字"))
                {
                    obj2["Message"] = "生成命令失败！" + "号码格式错误，只能为数字不能为字母、空格、字符！" + Phone;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                cmd.Phone = Phone.Trim();

                string msg = cmd.WriteMsg();
                if (msg == "")
                {
                    op.OperationType = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), cmd.AFN);
                    op.RawData = cmd.RawDataStr;

                    myLogger.Info(op.OperationType + "：" + cmd.RawDataStr);
                    cmd_send = cmd.RawDataChar;
                }
                else
                {
                    obj2["Message"] = "生成命令失败！" + msg;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
            }
            else if (ctrlName == "读取")
            {
                WaterCmd_24_1 cmd = new WaterCmd_24_1();
                cmd.CenterStation = WaterBaseProtocol.CenterStation;
                cmd.RemoteStation = RemoteStation.PadLeft(10, '0');
                cmd.PW = WaterBaseProtocol.PW;
                cmd.SerialNumber = 0;
                cmd.SendTime = DateTime.Now;

                try
                {
                    cmd.OrderNum = Convert.ToByte(OrderNum);
                }
                catch
                {
                    obj2["Message"] = "生成命令失败！" + "序号格式错误！" + OrderNum;
                    return JavaScriptConvert.SerializeObject(obj2);
                }

                string msg = cmd.WriteMsg();
                if (msg == "")
                {
                    op.OperationType = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), cmd.AFN);
                    op.RawData = cmd.RawDataStr;

                    myLogger.Info(op.OperationType + "：" + cmd.RawDataStr);
                    cmd_send = cmd.RawDataChar;
                }
                else
                {
                    obj2["Message"] = "生成命令失败！" + msg;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
            }

            ResMsg resMsg = SendCmd(cmd_send);

            if (resMsg.Result)
            {
                if (resMsg.Message == BaseProtocol.DeviceOffline)
                {
                    obj2["Message"] = "查询终端不在线";
                }
                else
                {
                    byte[] cmd_receive = HexStringUtility.HexStringToByteArray(resMsg.Message);
                    string msg_obj = "非法";
                    string info_obj = "";
                    if (ctrlName == "设置")
                    {
                        WaterCmd_23_2 cmd = new WaterCmd_23_2();
                        msg_obj = cmd.ReadMsg(cmd_receive);
                        info_obj = msg_obj == "" ? cmd.ToString() : "";
                    }
                    else if (ctrlName == "读取")
                    {
                        WaterCmd_24_2 cmd = new WaterCmd_24_2();
                        msg_obj = cmd.ReadMsg(cmd_receive);
                        info_obj = msg_obj == "" ? cmd.ToString() : "";
                        obj2["OrderNum"] = msg_obj == "" ? cmd.OrderNum.ToString().PadLeft(2, '0') : "";
                        obj2["Phone"] = msg_obj == "" ? cmd.Phone : "";
                    }

                    obj2["Result"] = msg_obj == "";
                    obj2["Info"] = info_obj;
                    obj2["Message"] = msg_obj == "" ? "" : ctrlName + "数据解析出错！" + msg_obj;
                }
            }
            else
            {
                obj2["Message"] = resMsg.Message;
            }

            if (op.State != "发送成功")
            {
                op.State = "发送失败";
                op.State += "|" + obj2["Message"].ToString();
            }
            DeviceOperationModule.AddDeviceOperation(op);
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>播报内容下发</span><br/><p>输入参数：loginIdentifer=登录用户标识，devID=设备ID<br/>返回数据格式：{'Result':bool,'Message':string}</p>")]
        public string ControlDevice7(string loginIdentifer, string devID, string PlayCount, string PlayRole, string PlaySpeed, string PlayContext)
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
            if (!SystemService.isConnect)
            {
                obj2["Message"] = "与通讯服务器连接中断";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            Device device = DeviceModule.GetDeviceByID(long.Parse(devID));
            if (device == null)
            {
                obj2["Message"] = "要操作的设备不存在";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            string DeviceNo = DeviceModule.GetFullDeviceNoByID(device.Id);
            string RemoteStation = device.RemoteStation.Trim();
            if (RemoteStation == "")
            {
                obj2["Message"] = "要操作的设备无法操作";
                return JavaScriptConvert.SerializeObject(obj2);
            }

            DeviceOperation op = new DeviceOperation();
            op.DeviceNo = DeviceNo;
            op.DeviceName = device.DeviceName;
            op.OperationTime = DateTime.Now;
            op.Remark = "";
            op.UserId = loginUser.UserId;
            op.UserName = SysUserModule.GetUser(loginUser.UserId).UserName;
            op.State = "等待发送";

            byte[] cmd_send = null;
            WaterCmd_27_1 cmd = new WaterCmd_27_1();
            cmd.CenterStation = WaterBaseProtocol.CenterStation;
            cmd.RemoteStation = RemoteStation.PadLeft(10, '0');
            cmd.PW = WaterBaseProtocol.PW;
            cmd.SerialNumber = 0;
            cmd.SendTime = DateTime.Now;

            try
            {
                cmd.PlayCount = Convert.ToByte(PlayCount);
            }
            catch
            {
                obj2["Message"] = "生成命令失败！" + "播报次数格式错误！" + PlayCount;
                return JavaScriptConvert.SerializeObject(obj2);
            }
            if (cmd.PlayCount < 1 || cmd.PlayCount > 255)
            {
                obj2["Message"] = "生成命令失败！" + "播报次数范围1-255！" + PlayCount;
                return JavaScriptConvert.SerializeObject(obj2);
            }

            try
            {
                cmd.PlayRole = Convert.ToByte(PlayRole);
            }
            catch
            {
                obj2["Message"] = "生成命令失败！" + "播报角色格式错误！" + PlayRole;
                return JavaScriptConvert.SerializeObject(obj2);
            }
            if (cmd.PlayRole < 1 || cmd.PlayRole > 15)
            {
                obj2["Message"] = "生成命令失败！" + "播报次数范围1-15！" + PlayRole;
                return JavaScriptConvert.SerializeObject(obj2);
            }

            try
            {
                cmd.PlaySpeed = Convert.ToByte(PlaySpeed);
            }
            catch
            {
                obj2["Message"] = "生成命令失败！" + "播报速度格式错误！" + PlaySpeed;
                return JavaScriptConvert.SerializeObject(obj2);
            }
            if (cmd.PlaySpeed < 1 || cmd.PlaySpeed > 15)
            {
                obj2["Message"] = "生成命令失败！" + "播报速度范围1-15！" + PlaySpeed;
                return JavaScriptConvert.SerializeObject(obj2);
            }

            cmd.PlayContext = PlayContext.Trim();
  // System.Diagnostics.Debug.Print(cmd.PlayContext);
            if (cmd.PlayContext.Length == 0 || cmd.PlayContext.Length > 400)
            {
                obj2["Message"] = "生成命令失败！" + "播报内容字数范围1-400！" + cmd.PlayContext.Length;
                return JavaScriptConvert.SerializeObject(obj2);
            }

            string msg = cmd.WriteMsg();
            if (msg == "")
            {
                op.OperationType = EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), cmd.AFN);
                op.RawData = cmd.RawDataStr;

                myLogger.Info(op.OperationType + "：" + cmd.RawDataStr);
//System.Diagnostics.Debug.Print(op.OperationType +" :"+cmd.RawDataStr);
                cmd_send = cmd.RawDataChar;
            }
            else
            {
                obj2["Message"] = "生成命令失败！" + msg;
                return JavaScriptConvert.SerializeObject(obj2);
            }

            ResMsg resMsg = SendCmd(cmd_send);

            if (resMsg.Result)
            {
                if (resMsg.Message == BaseProtocol.DeviceOffline)
                {
                    obj2["Message"] = "查询终端不在线";
                }
                else
                {
                    byte[] cmd_receive = HexStringUtility.HexStringToByteArray(resMsg.Message);

                    WaterCmd_27_2 cmd2 = new WaterCmd_27_2();
                    string msg_receive = cmd2.ReadMsg(cmd_receive);

                    obj2["Result"] = msg_receive == "";
                    obj2["Message"] = msg_receive == "" ? "" : "播报内容响应数据解析出错！" + msg_receive;
                }
            }
            else
            {
                obj2["Message"] = resMsg.Message;
            }

            if (op.State != "发送成功")
            {
                op.State = "发送失败";
                op.State += "|" + obj2["Message"].ToString();
            }
            DeviceOperationModule.AddDeviceOperation(op);

            return JavaScriptConvert.SerializeObject(obj2);
        }

        private ResMsg SendCmd(byte[] cmd_send)
        {
            ResMsg msg = new ResMsg(false, "");

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
                msg.Message = "与网关通讯失败！";
                return msg;
            }

            tcpService.SendData(cmd_send, 0, cmd_send.Length);

            bool waitRsp = false;
            timeDelay = 0;
            while (timeDelay < tcpService.TcpWait)
            {
          //      System.Diagnostics.Debug.Print("接收到的数据为====" + tcpService.socketData.Buffer.Length);
                if (tcpService.socketData.Buffer.Length >= CommandCommon.CMD_MIN_LENGTH)
                {
                    byte[] re = tcpService.socketData.Buffer.Buffer;
                    byte[] buffer_new = new byte[tcpService.socketData.Buffer.Length];
                    Array.Copy(re, buffer_new, tcpService.socketData.Buffer.Length);
                    waitRsp = true;
                    msg.Result = true;
                    msg.Message = HexStringUtility.ByteArrayToHexString(buffer_new);

                    myLogger.Info("收到数据：" + msg.Message);
                }

                if (waitRsp == true)
                {
                    //myLogger.Info("获取响应结束");
                    break;
                }

                if (tcpService.SOCKET_STATE == TcpCommunication.TCP_SocketState.SOCKET_CLOSED)
                {
                    //myLogger.Info("Socket关闭结束");
                    break;
                }

                Thread.Sleep(100);
                timeDelay = timeDelay + 1;
            }

            tcpService.Close();

            if (waitRsp == false)
            {
                msg.Message = "等待设备回复超时！";
            }
            return msg;
        }
    }
}
