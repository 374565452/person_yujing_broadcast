using Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Services;
using WaterMonitorSystem.Src;

namespace WaterMonitorSystem.WebServices
{
    /// <summary>
    /// GlobalAppService 的摘要说明
    /// </summary>
    [Serializable, WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1), ToolboxItem(false), WebService(Description = "提供获取或更改全局应用配置和重启网站等服务", Name = "全局应用服务", Namespace = "http://www.data86.net/")]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class GlobalAppService : System.Web.Services.WebService
    {
        log4net.ILog myLogger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private HttpContext context = HttpContext.Current;

        public GlobalAppService()
        {
            this.context.Response.Buffer = true;
            this.context.Response.ExpiresAbsolute = DateTime.Now.AddSeconds(-1.0);
            this.context.Response.Expires = 0;
            this.context.Response.CacheControl = "no-cache";
        }

        [WebMethod]
        public string GetState()
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            obj2.Add("Result", false);
            obj2.Add("Message", "");

            if (SysInfo.IsReg)
            {
                string path = context.Server.MapPath("~/");

                SysInfo.SetFilePath(path);

                SysInfo.IsRegSuccess = false;
                string regStr = "";
                if (FileHelper.IsExists(SysInfo.fileName))
                {
                    regStr = FileHelper.ReadFile(SysInfo.fileName);
                }
                else
                {
                    regStr = "00000000000000000000000000000000";
                    FileHelper.writeFile(SysInfo.fileName, regStr);
                }

                if (regStr != SysInfo.GetRegStr2())
                {
                    myLogger.Info("注册码不对！序列号为：" + SysInfo.GetRegStr1());
                    obj2["Message"] = "系统未注册";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                else
                {
                    SysInfo.IsRegSuccess = true;
                }
            }

            obj2["Result"] = true;
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod]
        public string GetDeviceAppGroups()
        {
            List<DeviceAppGroup> deviceAppGroups = GlobalAppModule.GetDeviceAppGroups();
            JavaScriptArray array = new JavaScriptArray();
            foreach (DeviceAppGroup group in deviceAppGroups)
            {
                JavaScriptObject item = new JavaScriptObject();
                item.Add("名称", group.GroupName);
                item.Add("实时监测Url", group.MonitorUrl);
                item.Add("电子地图Url", group.MapURL);
                item.Add("用户站参数", group.UserStationParms.ToArray());
                JavaScriptArray array2 = new JavaScriptArray();
                for (int i = 0; i < group.ChildDeviceGroup.Count; i++)
                {
                    JavaScriptObject obj3 = new JavaScriptObject();
                    obj3.Add("名称", group.ChildDeviceGroup[i].GroupName);
                    obj3.Add("用户站参数", group.ChildDeviceGroup[i].UserStationParms.ToArray());
                    array2.Add(obj3);
                }
                item.Add("子类", array2);
                array.Add(item);
            }
            return JavaScriptConvert.SerializeObject(array);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>重启通讯服务器</span><br/><p>输入参数：loginIdentifer=登录用户标识，superPassword=超级密码<br/>返回数据格式：{'Result':bool,'Message':string}</p>")]
        public string ReStartCTS(string loginIdentifer, string superPassword)
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
            if (superPassword != "data86@data86")
            {
                obj2["Message"] = "密码错误";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            GlobalAppModule.ReStartCTS(loginUser.UserId, this.context.Request.UserHostAddress);
            obj2["Result"] = true;
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>重启网站</span><br/><p>输入参数：loginIdentifer=登录用户标识，superPassword=超级密码<br/>返回数据格式：{'Result':bool,'Message':string}</p>")]
        public string ReStartWebApp(string loginIdentifer, string superPassword)
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
            if (superPassword != "JSSL2016@2016")
            {
                obj2["Message"] = "密码错误";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            ResMsg msg = GlobalAppModule.ReStartWebApp(loginUser.UserId, this.context.Request.UserHostAddress);
            obj2["Result"] = msg.Result;
            obj2["Message"] = msg.Message;
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>测试连接</span><br/><p>返回数据：'连接成功'</p>")]
        public string TestConnection()
        {
            return "连接成功";
        }
    }
}
