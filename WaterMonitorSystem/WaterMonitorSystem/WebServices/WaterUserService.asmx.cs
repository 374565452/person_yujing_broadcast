using Common;
using Maticsoft.Model;
using Module;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using WaterMonitorSystem.Src;

namespace WaterMonitorSystem.WebServices
{
    /// <summary>
    /// WaterUserService 的摘要说明
    /// </summary>
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1), WebService(Name = "用水户管理服务", Description = "提供用水户的增删查改服务", Namespace = "http://www.data86.net/")]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class WaterUserService : System.Web.Services.WebService
    {
        private HttpContext context = HttpContext.Current;

        public WaterUserService()
        {
            this.context.Response.Buffer = true;
            this.context.Response.ExpiresAbsolute = DateTime.Now.AddSeconds(-1.0);
            this.context.Response.Expires = 0;
            this.context.Response.CacheControl = "no-cache";
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>添加用水户</span><br/><p>输入参数：loginIdentifer=登录用户标识，waterUserJson=用水户JSON对象字符串<br/>返回数据格式：{'Result':bool,'Message':string}</p>")]
        public string AddWaterUser(string loginIdentifer, string waterUserJson)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            try
            {
                ResMsg msg = CommonUtil.CheckLoginState(loginIdentifer, true);
                if (!msg.Result)
                {
                    obj2["Message"] = msg.Message;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                LoginUser loginUser = GlobalAppModule.GetLoginUser(loginIdentifer);
                string message = msg.Message;
                JavaScriptObject obj3 = (JavaScriptObject)JavaScriptConvert.DeserializeObject(waterUserJson);
                if (obj3 == null)
                {
                    obj2["Message"] = "参数waterUserJson格式不正确";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                WaterUser wui = new WaterUser();
                wui.UserName = obj3["名称"].ToString();
                wui.Password = "";
                wui.TrueName = "";
                wui.DistrictId = long.Parse(obj3["管理ID"].ToString());
                wui.IdentityNumber = obj3["身份证号"].ToString();
                wui.Telephone = obj3["电话"].ToString();
                wui.Address = obj3["地址"].ToString();
                wui.State = "正常";
                wui.WaterQuota = decimal.Parse(obj3["用水定额"].ToString());
                wui.ElectricQuota = decimal.Parse(obj3["用电定额"].ToString());
                wui.水价ID = int.Parse(obj3["水价ID"].ToString());
                wui.电价ID = int.Parse(obj3["电价ID"].ToString());

                if (WaterUserModule.ExistsUserName(wui.DistrictId, wui.UserName))
                {
                    obj2["Message"] = "存在相同户名";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                if (WaterUserModule.ExistsIdentityNumber(wui.IdentityNumber))
                {
                    obj2["Message"] = "存在相同身份证号";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                if (WaterUserModule.ExistsTelephone(wui.Telephone))
                {
                    obj2["Message"] = "存在相同身份证号";
                    return JavaScriptConvert.SerializeObject(obj2);
                }

                msg = WaterUserModule.AddWaterUser(wui);
                if (msg.Result)
                {
                    obj2["Result"] = true;
                }
                else
                {
                    obj2["Message"] = msg.Message;
                }
                try
                {
                    //添加日志
                    WaterUserLog log = new WaterUserLog();
                    log.WaterUserId = wui.id;
                    log.LogUserId = loginUser.UserId;
                    log.LogUserName = loginUser.LoginName;
                    log.LogAddress = ToolsWeb.GetIP(context.Request);
                    log.LogTime = loginUser.LastOperateTime;
                    log.LogType = "添加";
                    log.LogContent = msg.Message;
                    log.UserName = wui.UserName;
                    log.Password = wui.Password;
                    log.DistrictId = wui.DistrictId;
                    log.TrueName = wui.TrueName;
                    log.IdentityNumber = wui.IdentityNumber;
                    log.Telephone = wui.Telephone;
                    log.Address = wui.Address;
                    log.WaterQuota = wui.WaterQuota;
                    log.ElectricQuota = wui.ElectricQuota;
                    log.Remark = wui.Remark;
                    log.水价ID = wui.水价ID;
                    log.电价ID = wui.电价ID;
                    log.State = wui.State;
                    WaterUserLogModule.Add(log);
                }
                catch
                {
                }
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>修改用水户信息</span><br/><p>输入参数：loginIdentifer=登录用户标识，waterUserJson=用水户JSON对象字符串，deleteQuotaId=需要删除的用水定额的ID字符串<br/>返回数据格式：{'Result':bool,'Message':string}</p>")]
        public string ModifyWaterUser(string loginIdentifer, string waterUserJson, string deleteQuotaId)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            try
            {
                ResMsg msg = CommonUtil.CheckLoginState(loginIdentifer, true);
                if (!msg.Result)
                {
                    obj2["Message"] = msg.Message;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                LoginUser loginUser = GlobalAppModule.GetLoginUser(loginIdentifer);
                JavaScriptObject obj3 = (JavaScriptObject)JavaScriptConvert.DeserializeObject(waterUserJson);
                if (obj3 == null)
                {
                    obj2["Message"] = "参数waterUserJson格式不正确";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                WaterUser wui = WaterUserModule.GetWaterUserById(long.Parse(obj3["ID"].ToString()));
                if (wui == null)
                {
                    obj2["Message"] = "ID为" + obj3["ID"].ToString() + "用水户不存在";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                wui.UserName = obj3["名称"].ToString();
                wui.Password = "";
                wui.TrueName = "";
                wui.DistrictId = long.Parse(obj3["管理ID"].ToString());
                wui.IdentityNumber = obj3["身份证号"].ToString();
                wui.Telephone = obj3["电话"].ToString();
                wui.Address = obj3["地址"].ToString();
                wui.State = "正常";
                wui.WaterQuota = decimal.Parse(obj3["用水定额"].ToString());
                wui.ElectricQuota = decimal.Parse(obj3["用电定额"].ToString());
                wui.水价ID = int.Parse(obj3["水价ID"].ToString());
                wui.电价ID = int.Parse(obj3["电价ID"].ToString());

                if (WaterUserModule.ExistsUserName(wui.DistrictId, wui.UserName, wui.id))
                {
                    obj2["Message"] = "存在相同户名";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                if (WaterUserModule.ExistsIdentityNumber(wui.IdentityNumber, wui.id))
                {
                    obj2["Message"] = "存在相同身份证号";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                if (WaterUserModule.ExistsTelephone(wui.Telephone, wui.id))
                {
                    obj2["Message"] = "存在相同身份证号";
                    return JavaScriptConvert.SerializeObject(obj2);
                }

                msg = WaterUserModule.ModifyWaterUser(wui);
                if (msg.Result)
                {
                    obj2["Result"] = true;
                }
                else
                {
                    obj2["Message"] = msg.Message;
                }
                try
                {
                    //添加日志
                    WaterUserLog log = new WaterUserLog();
                    log.WaterUserId = wui.id;
                    log.LogUserId = loginUser.UserId;
                    log.LogUserName = loginUser.LoginName;
                    log.LogAddress = ToolsWeb.GetIP(context.Request);
                    log.LogTime = loginUser.LastOperateTime;
                    log.LogType = "修改";
                    log.LogContent = msg.Message;
                    log.UserName = wui.UserName;
                    log.Password = wui.Password;
                    log.DistrictId = wui.DistrictId;
                    log.TrueName = wui.TrueName;
                    log.IdentityNumber = wui.IdentityNumber;
                    log.Telephone = wui.Telephone;
                    log.Address = wui.Address;
                    log.WaterQuota = wui.WaterQuota;
                    log.ElectricQuota = wui.ElectricQuota;
                    log.Remark = wui.Remark;
                    log.水价ID = wui.水价ID;
                    log.电价ID = wui.电价ID;
                    log.State = wui.State;
                    WaterUserLogModule.Add(log);
                }
                catch
                {
                }
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>根据村庄ID获取该村所有用水户(含已注销)</span><br/><p>输入参数：loginIdentifer=登录用户标识，villageId=村庄ID<br/>返回数据格式：{'Result':bool,'Message':string,'WaterUsers':[object1...objectn]}</p>")]
        public string GetAllWaterUsersByVillageId(string loginIdentifer, string villageId)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            JavaScriptArray array = new JavaScriptArray();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("WaterUsers", array);
            try
            {
                ResMsg msg = CommonUtil.CheckLoginState(loginIdentifer, true);
                if (!msg.Result)
                {
                    obj2["Message"] = msg.Message;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                if ((villageId == null) || (villageId.Trim() == ""))
                {
                    obj2["Message"] = "参数villageId不能为空";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                if (DistrictModule.ReturnDistrictInfo(long.Parse(villageId)) == null)
                {
                    obj2["Message"] = "ID为" + villageId + "的村庄不存在";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                /*
                List<WaterUserInfo> liWaterUsers = null;
                msg = SaleWaterModule.GetWaterUsersByManagerId(villageId, true, ref liWaterUsers);
                if (!msg.Result)
                {
                    obj2["Message"] = msg.Message;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                foreach (WaterUserInfo info in liWaterUsers)
                {
                    array.Add(SaleWaterModule.WaterUserToJson(info));
                }
                 * */
                obj2["Result"] = true;
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>根据用水户完整编码(含省市县乡村)获取用水户信息</span><br/><p>输入参数：loginIdentifer=登录用户标识，allCode=用水户完整编码<br/>返回数据格式：{'Result':bool,'Message':string,'WaterUser':object}</p>")]
        public string GetWaterUserByAllCode(string loginIdentifer, string allCode)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("WaterUser", new JavaScriptObject());
            try
            {
                ResMsg msg = CommonUtil.CheckLoginState(loginIdentifer, true);
                if (!msg.Result)
                {
                    obj2["Message"] = msg.Message;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                if ((allCode == null) || (allCode.Trim() == ""))
                {
                    obj2["Message"] = "用水户完整编码为空";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                /*
                WaterUserInfo waterUserByAllCode = SaleWaterModule.GetWaterUserByAllCode(allCode);
                if (waterUserByAllCode == null)
                {
                    obj2["Message"] = "完整编码为" + allCode + "用水户不存在";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                obj2["Result"] = true;
                obj2["WaterUser"] = SaleWaterModule.WaterUserToJson(waterUserByAllCode);
                 * */
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>根据用水户ID获取用水户信息</span><br/><p>输入参数：loginIdentifer=登录用户标识，waterUserId=用水户ID<br/>返回数据格式：{'Result':bool,'Message':string,'WaterUser':object}</p>")]
        public string GetWaterUserById(string loginIdentifer, string waterUserId)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("WaterUser", new JavaScriptObject());
            try
            {
                ResMsg msg = CommonUtil.CheckLoginState(loginIdentifer, true);
                if (!msg.Result)
                {
                    obj2["Message"] = msg.Message;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                if ((waterUserId == null) || (waterUserId.Trim() == ""))
                {
                    obj2["Message"] = "用水户ID不能为空";
                    return JavaScriptConvert.SerializeObject(obj2);
                }

                WaterUser wui = WaterUserModule.GetWaterUserById(long.Parse(waterUserId));
                if (wui == null)
                {
                    obj2["Message"] = "ID为" + waterUserId + "用水户不存在";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                obj2["Result"] = true;
                obj2["WaterUser"] = WaterUserModule.WaterUserToJson(wui);
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>根据用水户完整编码(含省市县乡村)获取用水户信息</span><br/><p>输入参数：loginIdentifer=登录用户标识，userSerialNumber=用户卡号<br/>返回数据格式：{'Result':bool,'Message':string,'WaterUser':object}</p>")]
        public string GetWaterUserByUserSerialNumber(string loginIdentifer, string userSerialNumber)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("WaterUser", new JavaScriptObject());
            try
            {
                ResMsg waterUserByUserSerialNumber = CommonUtil.CheckLoginState(loginIdentifer, true);
                if (!waterUserByUserSerialNumber.Result)
                {
                    obj2["Message"] = waterUserByUserSerialNumber.Message;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                if ((userSerialNumber == null) || (userSerialNumber.Trim() == ""))
                {
                    obj2["Message"] = "用户卡号为空";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                /*
                WaterUserInfo wui = null;
                waterUserByUserSerialNumber = SaleWaterModule.GetWaterUserByUserSerialNumber(userSerialNumber, ref wui);
                if (!waterUserByUserSerialNumber.Result)
                {
                    obj2["Message"] = waterUserByUserSerialNumber.Message;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                obj2["Result"] = true;
                obj2["WaterUser"] = SaleWaterModule.WaterUserToJson(wui);
                 * */
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>根据用水户ID获取用水户信息</span><br/><p>输入参数：loginIdentifer=登录用户标识，waterUserId=用水户ID<br/>返回数据格式：{'Result':bool,'Message':string,'WaterUser':object}</p>")]
        public string GetWaterUserHasQuotasById(string loginIdentifer, string waterUserId)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("WaterUser", new JavaScriptObject());
            try
            {
                ResMsg msg = CommonUtil.CheckLoginState(loginIdentifer, true);
                if (!msg.Result)
                {
                    obj2["Message"] = msg.Message;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                if ((waterUserId == null) || (waterUserId.Trim() == ""))
                {
                    obj2["Message"] = "用水户ID不能为空";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                WaterUser wui = WaterUserModule.GetWaterUserById(long.Parse(waterUserId));
                if (wui == null)
                {
                    obj2["Message"] = "ID为" + waterUserId + "用水户不存在";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                obj2["Result"] = true;
                obj2["WaterUser"] = WaterUserModule.WaterUserToJson(wui);
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>根据用水定额ID获取用水定额信息</span><br/><p>输入参数：loginIdentifer=登录用户标识，waterUserQuotaId=用水定额ID<br/>返回数据格式：{'Result':bool,'Message':string,'WaterUserQuota':object}</p>")]
        public string GetWaterUserQuotaById(string loginIdentifer, string waterUserQuotaId)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("WaterUserQuota", new JavaScriptObject());
            try
            {
                ResMsg msg = CommonUtil.CheckLoginState(loginIdentifer, false);
                if (!msg.Result)
                {
                    obj2["Message"] = msg.Message;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                /*
                WaterUserQuota waterUserQuotaById = SaleWaterModule.GetWaterUserQuotaById(waterUserQuotaId);
                if (waterUserQuotaById == null)
                {
                    obj2["Message"] = "ID为" + waterUserQuotaId + "用水定额不存在";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                obj2["WaterUser"] = SaleWaterModule.WaterUserQuotaToJson(waterUserQuotaById);
                 * */
                obj2["Result"] = true;
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>根据用水户ID获取此人此年的用水定额</span><br/><p>输入参数：loginIdentifer=登录用户标识，waterUserId=用水户ID，year=年份<br/>返回数据格式：{'Result':bool,'Message':string,'WaterUserQuota':[object1...objectn]}</p>")]
        public string GetWaterUserQuotaByWaterUserId(string loginIdentifer, string waterUserId, string year)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            JavaScriptArray array = new JavaScriptArray();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("WaterUserQuota", array);
            try
            {
                ResMsg msg = CommonUtil.CheckLoginState(loginIdentifer, false);
                if (!msg.Result)
                {
                    obj2["Message"] = msg.Message;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                if ((waterUserId == null) || (waterUserId.Trim() == ""))
                {
                    obj2["Message"] = "用水户ID不能为空";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                if ((year == null) || (year.Trim() == ""))
                {
                    obj2["Message"] = "年份不能为空";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                /*
                List<WaterUserQuota> waterUserQuotaByWaterUserIdAndYear = SaleWaterModule.GetWaterUserQuotaByWaterUserIdAndYear(waterUserId, year);
                if (waterUserQuotaByWaterUserIdAndYear == null)
                {
                    obj2["Message"] = "ID为" + waterUserId + "用水户不存在用水定额";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                foreach (WaterUserQuota quota in waterUserQuotaByWaterUserIdAndYear)
                {
                    array.Add(SaleWaterModule.WaterUserQuotaToJson(quota));
                }
                 * */
                obj2["Result"] = true;
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>根据用水户ID获取此人所有用水定额中不重复的年份</span><br/><p>输入参数：loginIdentifer=登录用户标识，waterUserId=用水户ID<br/>返回数据格式：{'Result':bool,'Message':string,'Years':[object1...objectn]}</p>")]
        public string GetWaterUserQuotaYearByWaterUserId(string loginIdentifer, string waterUserId)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            JavaScriptArray array = new JavaScriptArray();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("Years", array);
            try
            {
                ResMsg msg = CommonUtil.CheckLoginState(loginIdentifer, false);
                if (!msg.Result)
                {
                    obj2["Message"] = msg.Message;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                if ((waterUserId == null) || (waterUserId.Trim() == ""))
                {
                    obj2["Message"] = "用水户ID不能为空";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                /*
                List<string> waterUserQuotaByWaterUserId = SaleWaterModule.GetWaterUserQuotaByWaterUserId(waterUserId);
                if (waterUserQuotaByWaterUserId == null)
                {
                    obj2["Message"] = "ID为" + waterUserId + "用水户不存在用水定额";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                foreach (string str in waterUserQuotaByWaterUserId)
                {
                    JavaScriptObject item = new JavaScriptObject();
                    item.Add("年份", str);
                    array.Add(item);
                }
                 * */
                obj2["Result"] = true;
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>根据村庄ID获取该村所有正常用水户</span><br/><p>输入参数：loginIdentifer=登录用户标识，villageId=村庄ID，isExport=是否导出Excel<br/>返回数据格式：{'Result':bool,'Message':string,'WaterUsers':[object1...objectn]}</p>")]
        public string GetWaterUsersByIds(string loginIdentifer, string waterUserIds)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            JavaScriptArray array = new JavaScriptArray();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("WaterUsers", array);
            try
            {
                ResMsg msg = CommonUtil.CheckLoginState(loginIdentifer, true);
                if (!msg.Result)
                {
                    obj2["Message"] = msg.Message;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                if ((waterUserIds == null) || (waterUserIds.Trim() == ""))
                {
                    obj2["Message"] = "用水户ID不能为空";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                foreach (string str in waterUserIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    /*
                    WaterUserInfo waterUserById = SaleWaterModule.GetWaterUserById(str);
                    if (waterUserById == null)
                    {
                        obj2["Message"] = "ID为" + str + "的用水户不存在";
                        return JavaScriptConvert.SerializeObject(obj2);
                    }
                    array.Add(SaleWaterModule.WaterUserToJson(waterUserById));
                     * */
                }
                obj2["Result"] = true;
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>根据村庄ID获取该村所有正常用水户</span><br/><p>输入参数：loginIdentifer=登录用户标识，villageId=村庄ID，isExport=是否导出Excel<br/>返回数据格式：{'Result':bool,'Message':string,'WaterUsers':[object1...objectn]}</p>")]
        public string GetWaterUsersByVillageId(string loginIdentifer, string villageId, bool isExport)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            JavaScriptArray array = new JavaScriptArray();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("WaterUsers", array);
            try
            {
                ResMsg msg = CommonUtil.CheckLoginState(loginIdentifer, true);
                if (!msg.Result)
                {
                    obj2["Message"] = msg.Message;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                if ((villageId == null) || (villageId.Trim() == ""))
                {
                    obj2["Message"] = "参数villageId不能为空";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                if (DistrictModule.ReturnDistrictInfo(long.Parse(villageId)) == null)
                {
                    obj2["Message"] = "ID为" + villageId + "的村庄不存在";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                List<WaterUser> liWaterUsers = WaterUserModule.GetWaterUsersByDistrictId(long.Parse(villageId), false);
                foreach (WaterUser info in liWaterUsers)
                {
                    array.Add(WaterUserModule.WaterUserToJson(info));
                }
                obj2["Result"] = true;
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>根据村庄ID获取该村所有正常用水户</span><br/><p>输入参数：loginIdentifer=登录用户标识，villageId=村庄ID，isExport=是否导出Excel<br/>返回数据格式：{'Result':bool,'Message':string,'WaterUsers':[object1...objectn]}</p>")]
        public string GetWaterUsersHasQuotasByIds(string loginIdentifer, string waterUserIds)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            JavaScriptArray array = new JavaScriptArray();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("WaterUsers", array);
            try
            {
                ResMsg msg = CommonUtil.CheckLoginState(loginIdentifer, true);
                if (!msg.Result)
                {
                    obj2["Message"] = msg.Message;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                if ((waterUserIds == null) || (waterUserIds.Trim() == ""))
                {
                    obj2["Message"] = "用水户ID不能为空";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                foreach (string str in waterUserIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    WaterUser wui = WaterUserModule.GetWaterUserById(long.Parse(str));
                    if (wui == null)
                    {
                        obj2["Message"] = "ID为" + str + "的用水户不存在";
                        return JavaScriptConvert.SerializeObject(obj2);
                    }
                    array.Add(WaterUserModule.WaterUserToJson(wui));
                }
                obj2["Result"] = true;
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>根据村庄ID获取该村所有正常用水户</span><br/><p>输入参数：loginIdentifer=登录用户标识，villageId=村庄ID，isExport=是否导出Excel<br/>返回数据格式：{'Result':bool,'Message':string,'WaterUsers':[object1...objectn]}</p>")]
        public string GetWaterUsersHasQuotasByVillageId(string loginIdentifer, string villageId, bool isExport)
        {

            return GetWaterUsersByVillageId(loginIdentifer, villageId, isExport);
            /*
            JavaScriptObject obj2 = new JavaScriptObject();
            JavaScriptArray array = new JavaScriptArray();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("WaterUsers", array);
            try
            {
                ResMsg msg = CommonUtil.CheckLoginState(loginIdentifer, true);
                if (!msg.Result)
                {
                    obj2["Message"] = msg.Message;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                if ((villageId == null) || (villageId.Trim() == ""))
                {
                    obj2["Message"] = "参数villageId不能为空";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                if (DistrictModule.ReturnDistrictInfo(long.Parse(villageId)) == null)
                {
                    obj2["Message"] = "ID为" + villageId + "的村庄不存在";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                List<WaterUserInfo> liWaterUsers = null;
                msg = SaleWaterModule.GetWaterUsersByManagerId(villageId, false, ref liWaterUsers);
                if (!msg.Result)
                {
                    obj2["Message"] = msg.Message;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                foreach (WaterUserInfo info in liWaterUsers)
                {
                    array.Add(SaleWaterModule.WaterUserToJsonHasQuotas(info));
                }
                obj2["Result"] = true;
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
            }
            return JavaScriptConvert.SerializeObject(obj2);
             * */
        }

        

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>修改用水户定额信息</span><br/><p>输入参数：loginIdentifer=登录用户标识，waterUserQuotaJson=用水户定额JSON对象字符串<br/>返回数据格式：{'Result':bool,'Message':string}</p>")]
        public string ModifyWaterUserQuota(string loginIdentifer, string waterUserQuotaJson)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            try
            {
                ResMsg msg = CommonUtil.CheckLoginState(loginIdentifer, true);
                if (!msg.Result)
                {
                    obj2["Message"] = msg.Message;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                JavaScriptObject obj3 = (JavaScriptObject)JavaScriptConvert.DeserializeObject(waterUserQuotaJson);
                if (obj3 == null)
                {
                    obj2["Message"] = "参数jsoWaterUserQuota格式不正确";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                /*
                WaterUserQuota wuq = new WaterUserQuota
                {
                    Id = obj3["ID"].ToString(),
                    WaterUserId = obj3["用水户ID"].ToString(),
                    Year = obj3["年份"].ToString(),
                    UnitQuotaId = obj3["单位定额ID"].ToString(),
                    Acreage = Convert.ToDouble(obj3["面积"].ToString()),
                    Quota = Convert.ToDouble(obj3["定额"].ToString())
                };
                msg = SaleWaterModule.ModifyWaterUserQuota(wuq);
                if (msg.Result)
                {
                    obj2["Result"] = true;
                }
                else
                {
                    obj2["Message"] = msg.Message;
                }
                 * */
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>根据用水户ID删除用水户</span><br/><p>输入参数：loginIdentifer=登录用户标识，waterUserId=用水户ID<br/>返回数据格式：{'Result':bool,'Message':string}</p>")]
        public string WriteOffWaterUserById(string loginIdentifer, string waterUserId)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            try
            {
                ResMsg msg = CommonUtil.CheckLoginState(loginIdentifer, true);
                if (!msg.Result)
                {
                    obj2["Message"] = msg.Message;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                LoginUser loginUser = GlobalAppModule.GetLoginUser(loginIdentifer);
                string message = msg.Message;
                if ((waterUserId == null) || (waterUserId.Trim() == ""))
                {
                    obj2["Message"] = "用水户ID不能为空";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                WaterUser wui = WaterUserModule.GetWaterUserById(long.Parse(waterUserId));
                if (wui == null)
                {
                    obj2["Message"] = "ID为" + waterUserId + "的用水户不存在";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                msg = WaterUserModule.WriteOffWaterUserById(long.Parse(waterUserId));
                if (msg.Result)
                {
                    obj2["Result"] = true;
                }
                else
                {
                    obj2["Message"] = msg.Message;
                }
                try
                {
                    //添加日志
                    WaterUserLog log = new WaterUserLog();
                    log.WaterUserId = wui.id;
                    log.LogUserId = loginUser.UserId;
                    log.LogUserName = loginUser.LoginName;
                    log.LogAddress = ToolsWeb.GetIP(context.Request);
                    log.LogTime = loginUser.LastOperateTime;
                    log.LogType = "注销";
                    log.LogContent = msg.Message;
                    log.UserName = wui.UserName;
                    log.Password = wui.Password;
                    log.DistrictId = wui.DistrictId;
                    log.TrueName = wui.TrueName;
                    log.IdentityNumber = wui.IdentityNumber;
                    log.Telephone = wui.Telephone;
                    log.Address = wui.Address;
                    log.WaterQuota = wui.WaterQuota;
                    log.ElectricQuota = wui.ElectricQuota;
                    log.Remark = wui.Remark;
                    log.水价ID = wui.水价ID;
                    log.电价ID = wui.电价ID;
                    log.State = wui.State;
                    WaterUserLogModule.Add(log);
                }
                catch
                {
                }
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>根据用户卡号注销用水户</span><br/><p>输入参数：loginIdentifer=登录用户标识，userSerialNumber=用户卡号<br/>返回数据格式：{'Result':bool,'Message':string}</p>")]
        public string WriteOffWaterUserByUserSerialNumber(string loginIdentifer, string userSerialNumber)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            try
            {
                ResMsg msg = CommonUtil.CheckLoginState(loginIdentifer, true);
                if (!msg.Result)
                {
                    obj2["Message"] = msg.Message;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                LoginUser loginUser = GlobalAppModule.GetLoginUser(loginIdentifer);
                if ((userSerialNumber == null) || (userSerialNumber.Trim() == ""))
                {
                    obj2["Message"] = "用户卡号不能为空";
                    return JavaScriptConvert.SerializeObject(obj2);
                }

                CardUser cu = CardUserModule.GetCardUserBySerialNumber(userSerialNumber);
                if (cu == null)
                {
                    obj2["Message"] = "卡号为" + userSerialNumber + "的用户卡不存在";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                WaterUser wui = WaterUserModule.GetWaterUserById(cu.WaterUserId);
                if (wui == null)
                {
                    obj2["Message"] = "卡号为" + userSerialNumber + "的用水户不存在";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                ResMsg msg1 = WaterUserModule.WriteOffWaterUserById(cu.WaterUserId);
                if (msg1.Result)//msg.Result
                {
                    obj2["Result"] = true;
                }
                else
                {
                    obj2["Message"] = msg.Message;
                }
                try
                {
                    //添加日志
                    WaterUserLog log = new WaterUserLog();
                    log.WaterUserId = wui.id;
                    log.LogUserId = loginUser.UserId;
                    log.LogUserName = loginUser.LoginName;
                    log.LogAddress = ToolsWeb.GetIP(context.Request);
                    log.LogTime = loginUser.LastOperateTime;
                    log.LogType = "注销";
                    log.LogContent = msg.Message;
                    log.UserName = wui.UserName;
                    log.Password = wui.Password;
                    log.DistrictId = wui.DistrictId;
                    log.TrueName = wui.TrueName;
                    log.IdentityNumber = wui.IdentityNumber;
                    log.Telephone = wui.Telephone;
                    log.Address = wui.Address;
                    log.WaterQuota = wui.WaterQuota;
                    log.ElectricQuota = wui.ElectricQuota;
                    log.Remark = wui.Remark;
                    log.水价ID = wui.水价ID;
                    log.电价ID = wui.电价ID;
                    log.State = wui.State;
                    WaterUserLogModule.Add(log);
                }
                catch
                {
                }
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }
    }
}
