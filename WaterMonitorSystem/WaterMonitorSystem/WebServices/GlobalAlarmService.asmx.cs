using DBUtility;
using Maticsoft.Model;
using Module;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Web;
using System.Web.Services;
using WaterMonitorSystem.Src;

namespace WaterMonitorSystem.WebServices
{
    /// <summary>
    /// GlobalAlarmService 的摘要说明
    /// </summary>
    [Serializable, ToolboxItem(false), WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1), WebService(Description = "获取全局报警配置和报警信息", Name = "全局报警服务", Namespace = "http://www.data86.net/")]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class GlobalAlarmService : System.Web.Services.WebService
    {
        private HttpContext context = HttpContext.Current;
        private static int intBackYear;
        private static bool isExisteTable = false;
        private static string strDisposeAlarmCookieName = "";

        public GlobalAlarmService()
        {
            this.context.Response.Buffer = true;
            this.context.Response.ExpiresAbsolute = DateTime.Now.AddSeconds(-1.0);
            this.context.Response.Expires = 0;
            this.context.Response.CacheControl = "no-cache";
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>获取全局报警配置</span><br/><p style='text-indent:15px'>返回数据格式：{'Result':false,'Message':'','AlarmConfig':{'UseAlarm':'','UseVoice':'','AutoPopup':''}</p>")]
        public string GetGlobalAlarmConfig(string loginIdentifer)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            JavaScriptObject obj3 = new JavaScriptObject();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("AlarmConfig", obj3);
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
            GlobalAlarmConfig globalAlarmConfigByUserId = GlobalAlarmModule.GetGlobalAlarmConfigByUserId(loginUser.UserId.ToString());
            obj2["Result"] = true;
            obj3.Add("UseAlarm", globalAlarmConfigByUserId.UseAlarm.ToString());
            obj3.Add("UseVoice", globalAlarmConfigByUserId.UseVoice.ToString());
            obj3.Add("AutoPopup", globalAlarmConfigByUserId.AutoPopup.ToString());
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>设置全局报警配置</span><br/><p style='text-indent:15px'>输入参数：UseAlarm=启用报警，UseVoice=启用声音，AutoPopup=自动弹出，返回数据格式：{'Result':false,'Message':''}</p>")]
        public string SetGlobalAlarmConfig(string loginIdentifer, string UseAlarm, string UseVoice, string AutoPopup)
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
            try
            {
                bool blUseAlarm = UseAlarm.ToLower() == "true";
                bool blUseVoice = UseVoice.ToLower() == "true";
                bool blAutoPopup = AutoPopup.ToLower() == "true";
                GlobalAlarmModule.SetGlobalAlarmConfig(loginUser.UserId.ToString(), blUseAlarm, blUseVoice, blAutoPopup);
                obj2["Result"] = true;
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(EnableSession = true, Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>获取全局报警信息</span><br/><p style='text-indent:15px'>返回数据格式：{'Result':false,'Message':''}</p>")]
        public string GetGlobalAlarmInfos(string loginIdentifer)
        {
            if (strDisposeAlarmCookieName == "")
            {
                strDisposeAlarmCookieName = CommonUtil.GetCookieName(this.context.Request, "DisposeAlarmTime");
            }
            JavaScriptObject obj2 = new JavaScriptObject();
            JavaScriptArray array = new JavaScriptArray();
            obj2.Add("CookieName", strDisposeAlarmCookieName);
            obj2.Add("AlarmRecords", array);

            LoginUser loginUser = GlobalAppModule.GetLoginUser(loginIdentifer);
            if (loginUser == null)
            {
                return JavaScriptConvert.SerializeObject(obj2);
            }
            if (loginUser.LoginTimeout)
            {
                return JavaScriptConvert.SerializeObject(obj2);
            }

            string str = DateTime.Now.AddHours(-1).ToString("yyyy-MM-dd HH:mm:ss");
            if (this.context.Session["QueryAlarmStartTime"] != null)
            {
                str = this.context.Session["QueryAlarmStartTime"].ToString();
            }
            else
            {
                string str2 = this.context.Server.UrlEncode(strDisposeAlarmCookieName);
                if (this.context.Request.Cookies[str2] != null)
                {
                    str = this.context.Server.UrlDecode(this.context.Request.Cookies[str2].Value);
                }
                this.context.Session["QueryAlarmStartTime"] = str;
            }
            DateTime now = DateTime.Now;
            if (!CommonUtil.CheckTableExiste("DeviceAlarm_" + now.Year.ToString()))
            {
                return JavaScriptConvert.SerializeObject(obj2);
            }

            string deviceNoFulls = "";
            List<long> allDevicesForManageID = DeviceModule.GetAllDevicesForManageID(SysUserModule.GetUser(loginUser.UserId).DistrictId);
            for (int i = 0; i < allDevicesForManageID.Count; i++)
            {
                deviceNoFulls += "'" + DeviceModule.GetFullDeviceNoByID(allDevicesForManageID[i]) + "',";
            }

            if (deviceNoFulls == "")
            {
                return JavaScriptConvert.SerializeObject(obj2);
            }

            string sqlstr = "select * from DeviceAlarm_" + now.Year.ToString() + " a " +
                " where a.DeviceNo in (" + deviceNoFulls.TrimEnd(',') + ") and a.State = 'New'" +
                " and a.StartTime > '" + str.ToString() + "' and a.StartTime <= '" + now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                " order by a.id";
            DataTable table = DbHelperSQL.QueryDataTable(sqlstr);
            if (table != null)
            {
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    string AlarmType = table.Rows[i]["AlarmType"].ToString();
                    string AlarmValue = table.Rows[i]["AlarmValue"].ToString();
                    string AlarmTypeDesc = "";
                    string AlarmValueDesc = "";
                    bool flag = false;
                    if (AlarmType == "1" && AlarmValue == "1")
                    {
                        AlarmTypeDesc = "交流电充电状态";
                        AlarmValueDesc = "停电";
                        flag = true;
                    }
                    else if (AlarmType == "2" && AlarmValue == "1")
                    {
                        AlarmTypeDesc = "蓄电池电压状态";
                        AlarmValueDesc = "电压低";
                        flag = true;
                    }
                    else if (AlarmType == "3" && AlarmValue == "1")
                    {
                        AlarmTypeDesc = "水位超限报警状态";
                        AlarmValueDesc = "报警";
                        flag = true;
                    }
                    else if (AlarmType == "4" && AlarmValue == "1")
                    {
                        AlarmTypeDesc = "流量超限报警状态";
                        AlarmValueDesc = "报警";
                        flag = true;
                    }
                    else if (AlarmType == "5" && AlarmValue == "1")
                    {
                        AlarmTypeDesc = "电压状态";
                        AlarmValueDesc = "报警";
                        flag = true;
                    }
                    else if (AlarmType == "6" && AlarmValue == "1")
                    {
                        AlarmTypeDesc = "流量仪表状态";
                        AlarmValueDesc = "故障";
                        flag = true;
                    }
                    else if (AlarmType == "7" && AlarmValue == "1")
                    {
                        AlarmTypeDesc = "水位仪表状态";
                        AlarmValueDesc = "故障";
                        flag = true;
                    }
                    else if (AlarmType == "9" && AlarmValue == "1")
                    {
                        AlarmTypeDesc = "存储器状态";
                        AlarmValueDesc = "异常";
                        flag = true;
                    }
                    else if (AlarmType == "12" && AlarmValue == "1")
                    {
                        AlarmTypeDesc = "水量超采";
                        AlarmValueDesc = "异常";
                        flag = true;
                    }
                    else if (AlarmType == "13" && AlarmValue == "1")
                    {
                        AlarmTypeDesc = "回路报警";
                        AlarmValueDesc = "报警";
                        flag = true;
                    }
                    else if (AlarmType == "14" && AlarmValue == "1")
                    {
                        AlarmTypeDesc = "输入断相";
                        AlarmValueDesc = "断相";
                        flag = true;
                    }
                    else if (AlarmType == "15" && AlarmValue == "1")
                    {
                        AlarmTypeDesc = "输出断相";
                        AlarmValueDesc = "断相";
                        flag = true;
                    }
                    else if (AlarmType == "16" && AlarmValue == "1")
                    {
                        AlarmTypeDesc = "电表信号报警";
                        AlarmValueDesc = "故障";
                        flag = true;
                    }
                    else if (AlarmType == "17" && AlarmValue == "1")
                    {
                        AlarmTypeDesc = "过载报警";
                        AlarmValueDesc = "故障";
                        flag = true;
                    }
                    else if (AlarmType == "18" && AlarmValue == "1")
                    {
                        AlarmTypeDesc = "用户剩余水量";
                        AlarmValueDesc = "故障";
                        flag = true;
                    }
                    else if (AlarmType == "19" && AlarmValue == "1")
                    {
                        AlarmTypeDesc = "用户剩余电量";
                        AlarmValueDesc = "故障";
                        flag = true;
                    }
                    if (flag)
                    {
                        JavaScriptObject item = new JavaScriptObject();
                        item.Add("单位名称", table.Rows[i]["DeviceNo"].ToString());
                        item.Add("测点名称", table.Rows[i]["DeviceNo"].ToString());
                        item.Add("报警时间", table.Rows[i]["StartTime"].ToString());
                        item.Add("报警类型", AlarmTypeDesc);
                        item.Add("报警描述", AlarmTypeDesc + "：" + AlarmValueDesc);
                        array.Add(item);
                    }
                }
            }

            this.context.Session["QueryAlarmStartTime"] = now.ToString("yyyy-MM-dd HH:mm:ss");
            return JavaScriptConvert.SerializeObject(obj2);
        }

    }
}
