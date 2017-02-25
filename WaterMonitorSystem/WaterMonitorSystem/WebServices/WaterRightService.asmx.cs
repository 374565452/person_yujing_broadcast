using Common;
using Maticsoft.Model;
using Module;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using WaterMonitorSystem.Src;

namespace WaterMonitorSystem.WebServices
{
    /// <summary>
    /// WaterRightService 的摘要说明
    /// </summary>
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1), WebService(Name = "水权管理服务", Description = "提供水权管理服务", Namespace = "http://www.data86.net/")]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class WaterRightService : System.Web.Services.WebService
    {
        private HttpContext context = HttpContext.Current;

        public WaterRightService()
        {
            this.context.Response.Buffer = true;
            this.context.Response.ExpiresAbsolute = DateTime.Now.AddSeconds(-1.0);
            this.context.Response.Expires = 0;
            this.context.Response.CacheControl = "no-cache";
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>添加水权交易信息</span><br/><p>输入参数：loginIdentifer=登录用户标识，userid1=出卖方ID，userid1=出卖方ID，userid2=买受方ID,n=交易水量<br/>返回数据格式：{'Result':bool,'Message':string}</p>")]
        public string Recharge(string loginIdentifer, string userid1, string userid2, string n)
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

            string msg = "交易失败！";
            long nl = 0;
            try
            {
                nl = long.Parse(n);
            }
            catch
            {

            }
            if (nl <= 0)
            {
                msg = "交易失败！水量非法！";
            }
            else
            {
                try
                {
                    WaterUser wu1 = WaterUserModule.GetWaterUserById(long.Parse(userid1));
                    if (wu1.WaterQuota - nl > 0)
                    {
                        WaterUser wu2 = WaterUserModule.GetWaterUserById(long.Parse(userid2));

                        wu2.WaterQuota = wu2.WaterQuota + nl;
                        ResMsg r2 = WaterUserModule.ModifyWaterUser(wu2);
                        if (r2.Result)
                        {
                            wu1.WaterQuota = wu1.WaterQuota - nl;
                            ResMsg r1 = WaterUserModule.ModifyWaterUser(wu1);
                            if (r1.Result)
                            {
                                obj2["Result"] = true;
                                msg = "交易成功！";
                            }
                            else
                            {
                                wu2 = WaterUserModule.GetWaterUserById(long.Parse(userid2));
                                wu2.WaterQuota = wu2.WaterQuota - nl;
                                WaterUserModule.ModifyWaterUser(wu2);
                                msg = "交易失败！出卖方异常！";
                            }
                        }
                        else
                        {
                            msg = "交易失败！买受方异常！";
                        }
                    }
                    else
                    {
                        msg = "交易失败！水量不足！";
                    }
                }
                catch (Exception ex)
                {
                    msg = "交易失败！" + ex.Message;
                }
            }

            obj2["Message"] = msg;
            try
            {
                SysLog log = new SysLog();
                log.LogUserId = loginUser.UserId;
                log.LogUserName = loginUser.LoginName;
                log.LogAddress = ToolsWeb.GetIP(context.Request);
                log.LogTime = DateTime.Now;
                log.LogType = "水权交易";
                log.LogContent = msg + "|" + userid1 + "|" + userid2 + "|" + nl;
                SysLogModule.Add(log);
            }
            catch { }

            return JavaScriptConvert.SerializeObject(obj2);
        }
    }
}
