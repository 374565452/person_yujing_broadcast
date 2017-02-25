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
    /// QuotaManageService 的摘要说明
    /// </summary>
    [WebService(Name = "定额管理服务", Description = "提供单位定额和用户定额管理服务", Namespace = "http://www.data86.net/"), WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class QuotaManageService : System.Web.Services.WebService
    {
        private HttpContext context = HttpContext.Current;

        public QuotaManageService()
        {
            this.context.Response.Buffer = true;
            this.context.Response.ExpiresAbsolute = DateTime.Now.AddSeconds(-1.0);
            this.context.Response.Expires = 0;
            this.context.Response.CacheControl = "no-cache";
        }

        [WebMethod]
        public string AddUnitQuota(string loginIdentifer, string unitQuotaJson)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            obj2["Result"] = false;
            obj2["Message"] = "";
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
            JavaScriptObject obj3 = (JavaScriptObject)JavaScriptConvert.DeserializeObject(unitQuotaJson);
            if (obj3 == null)
            {
                obj2["Message"] = "参数格式错误";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            try
            {
                Crop crop = new Crop();
                crop.CropName = obj3["名称"].ToString();
                crop.WaterPerMu = decimal.Parse(obj3["单位定额"].ToString());
                crop.Remark = "";

                if (CropModule.ExistsCropName(crop.CropName))
                {
                    obj2["Result"] = false;
                    obj2["Message"] = "作物名称重复！";
                    return JavaScriptConvert.SerializeObject(obj2);
                }

                string msg = CropModule.AddCropInfo(crop);
                if (msg == "添加成功")//msg.Result
                {
                    GlobalAppModule.IsInitMainLib = true;
                    CropModule.LoadUnitQuotaInfos();
                    GlobalAppModule.IsInitMainLib = false;
                    obj2["Result"] = true;
                    obj2["Message"] = "成功";
                }
                else
                {
                    obj2["Message"] = msg;
                }
                try
                {
                    SysLog log = new SysLog();
                    log.LogUserId = loginUser.UserId;
                    log.LogUserName = loginUser.LoginName;
                    log.LogAddress = ToolsWeb.GetIP(context.Request);
                    log.LogTime = DateTime.Now;
                    log.LogType = "添加作物";
                    log.LogContent = msg + "|" + ModelHandler<Crop>.ToString(crop);
                    SysLogModule.Add(log);
                }
                catch { }
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod]
        public string ModifyUnitQuotas(string loginIdentifer, string unitQuotaJson)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            obj2["Result"] = false;
            obj2["Message"] = "";
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
            JavaScriptObject obj3 = (JavaScriptObject)JavaScriptConvert.DeserializeObject(unitQuotaJson);
            if (obj3 == null)
            {
                obj2["Message"] = "参数格式错误";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            try
            {
                Crop crop = CropModule.GetCrop(long.Parse(obj3["ID"].ToString()));
                crop.CropName = obj3["名称"].ToString();
                crop.WaterPerMu = decimal.Parse(obj3["单位定额"].ToString());
                crop.Remark = "";

                string msg = CropModule.ModifyCropInfo(crop);
                if (msg == "修改成功")//msg.Result
                {
                    GlobalAppModule.IsInitMainLib = true;
                    CropModule.LoadUnitQuotaInfos();
                    GlobalAppModule.IsInitMainLib = false;
                    obj2["Result"] = true;
                    obj2["Message"] = "成功";
                }
                else
                {
                    obj2["Message"] = msg;
                }
                try
                {
                    SysLog log = new SysLog();
                    log.LogUserId = loginUser.UserId;
                    log.LogUserName = loginUser.LoginName;
                    log.LogAddress = ToolsWeb.GetIP(context.Request);
                    log.LogTime = DateTime.Now;
                    log.LogType = "修改作物";
                    log.LogContent = msg + "|" + ModelHandler<Crop>.ToString(crop);
                    SysLogModule.Add(log);
                }
                catch { }
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod]
        public string DeleteUnitQuota(string loginIdentifer, string unitQuotaId)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            obj2["Result"] = false;
            obj2["Message"] = "";
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
                string msg = CropModule.DeleteCropInfo(long.Parse(unitQuotaId));
                if (msg == "删除成功")//msg.Result
                {
                    GlobalAppModule.IsInitMainLib = true;
                    CropModule.LoadUnitQuotaInfos();
                    GlobalAppModule.IsInitMainLib = false;
                    obj2["Result"] = true;
                    obj2["Message"] = "成功";
                }
                else
                {
                    obj2["Message"] = msg;
                }
                try
                {
                    SysLog log = new SysLog();
                    log.LogUserId = loginUser.UserId;
                    log.LogUserName = loginUser.LoginName;
                    log.LogAddress = ToolsWeb.GetIP(context.Request);
                    log.LogTime = DateTime.Now;
                    log.LogType = "删除作物";
                    log.LogContent = msg + "|" + unitQuotaId;
                    SysLogModule.Add(log);
                }
                catch { }
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod]
        public string GetUnitQuota(string loginIdentifer, string unitQuotaId)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            obj2["Result"] = false;
            obj2["Message"] = "";
            try
            {
                ResMsg unitQuotaInfoById = CommonUtil.CheckLoginState(loginIdentifer, false);
                if (!unitQuotaInfoById.Result)
                {
                    obj2["Message"] = unitQuotaInfoById.Message;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                if ((unitQuotaId == null) || (unitQuotaId.Trim() == ""))
                {
                    obj2["Message"] = "参数'unitQuotaId'无效";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                Crop crop = CropModule.GetCrop(long.Parse(unitQuotaId));
                JavaScriptObject obj3 = this.UnitQuotaInfoToJson(crop);
                obj2["Result"] = true;
                obj2["UnitQuota"] = obj3;
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod]
        public string GetUnitQuotasByIds(string loginIdentifer, string unitQuotaIds)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            JavaScriptArray array = new JavaScriptArray();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("UnitQuotas", array);
            try
            {
                ResMsg unitQuotaInfosByIds = CommonUtil.CheckLoginState(loginIdentifer, false);
                if (!unitQuotaInfosByIds.Result)
                {
                    obj2["Message"] = unitQuotaInfosByIds.Message;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                if ((unitQuotaIds == null) || (unitQuotaIds.Trim() == ""))
                {
                    obj2["Message"] = "参数'unitQuotaId'无效";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                List<Crop> CropList = CropModule.GetCropInfosByIds(unitQuotaIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                if (!unitQuotaInfosByIds.Result)
                {
                    obj2["Message"] = unitQuotaInfosByIds.Message;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                for (int i = 0; i < CropList.Count; i++)
                {
                    array.Add(this.UnitQuotaInfoToJson(CropList[i]));
                }
                obj2["Result"] = true;
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod]
        public string GetUnitQuotasByType(string loginIdentifer, string unitQuotaType)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            obj2["Result"] = false;
            obj2["Message"] = "";
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
            List<Crop> cropList = CropModule.GetAllCrop();
            JavaScriptArray array = new JavaScriptArray();
            foreach (Crop info in cropList)
            {
                JavaScriptObject item = this.UnitQuotaInfoToJson(info);
                array.Add(item);
            }
            obj2["Result"] = true;
            obj2["UnitQuotas"] = array;
            return JavaScriptConvert.SerializeObject(obj2);
        }

        private JavaScriptObject UnitQuotaInfoToJson(Crop uni)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            obj2["ID"] = uni.Id;
            obj2["名称"] = uni.CropName;
            //obj2["定额方式ID"] = uni.ModeId;
            obj2["单位定额"] = uni.WaterPerMu;
            return obj2;
        }
    }
}
