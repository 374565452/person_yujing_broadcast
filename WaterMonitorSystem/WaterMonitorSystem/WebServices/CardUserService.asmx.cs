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
    /// CardUserService 的摘要说明
    /// </summary>
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1), WebService(Name = "用水户卡管理服务", Description = "提供用水户卡的增删查改服务", Namespace = "http://www.data86.net/")]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class CardUserService : System.Web.Services.WebService
    {
        private HttpContext context = HttpContext.Current;

        public CardUserService()
        {
            this.context.Response.Buffer = true;
            this.context.Response.ExpiresAbsolute = DateTime.Now.AddSeconds(-1.0);
            this.context.Response.Expires = 0;
            this.context.Response.CacheControl = "no-cache";
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>根据村庄ID获取该村所有用水户卡</span><br/><p>输入参数：loginIdentifer=登录用户标识，villageId=村庄ID，isExport=是否导出Excel<br/>返回数据格式：{'Result':bool,'Message':string,'CardUsers':[object1...objectn]}</p>")]
        public string GetCardUsersByVillageId(string loginIdentifer, string villageId, bool isExport)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            JavaScriptArray array = new JavaScriptArray();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("CardUsers", array);
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
                //List<CardUser> liCardUsers = CardUserModule.GetCardUsersByDistrictId(long.Parse(villageId), true);
                //foreach (CardUser info in liCardUsers)
                //{
                //    array.Add(CardUserModule.CardUserToJson(info));
                //}
                List<WaterUser> liWaterUser = WaterUserModule.GetWaterUsersByDistrictId(long.Parse(villageId), true);
                foreach (WaterUser wui in liWaterUser)
                {
                    List<CardUser> liCardUsers = CardUserModule.GetCardUsersByWaterUserId(wui.id, true);
                    foreach (CardUser info in liCardUsers)
                    {
                        array.Add(CardUserModule.CardUserToJson(info));
                    }
                }
                obj2["Result"] = true;
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>根据村庄ID获取该村所有正常用水户卡</span><br/><p>输入参数：loginIdentifer=登录用户标识，waterUserIds=用水户ID列表<br/>返回数据格式：{'Result':bool,'Message':string,'CardUsers':[object1...objectn]}</p>")]
        public string GetCardUsersByWaterUserIds(string loginIdentifer, string waterUserIds)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            JavaScriptArray array = new JavaScriptArray();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("CardUsers", array);
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
                    List<CardUser> liCardUsers = CardUserModule.GetCardUsersByWaterUserId(wui.id, true);
                    foreach (CardUser info in liCardUsers)
                    {
                        array.Add(CardUserModule.CardUserToJson(info));
                    }
                }
                obj2["Result"] = true;
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }


        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>根据用水户卡ID获取用水户卡信息</span><br/><p>输入参数：loginIdentifer=登录用户标识，cardUserID=用水户卡ID<br/>返回数据格式：{'Result':bool,'Message':string,'CardUser':object}</p>")]
        public string GetCardUserById(string loginIdentifer, string cardUserID)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("CardUser", new JavaScriptObject());
            try
            {
                ResMsg msg = CommonUtil.CheckLoginState(loginIdentifer, true);
                if (!msg.Result)
                {
                    obj2["Message"] = msg.Message;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                if ((cardUserID == null) || (cardUserID.Trim() == ""))
                {
                    obj2["Message"] = "用水户卡ID不能为空";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                CardUser cardUser = CardUserModule.GetCardUserById(long.Parse(cardUserID));
                if (cardUser == null)
                {
                    obj2["Message"] = "ID为" + cardUserID + "用水户卡不存在";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                obj2["Result"] = true;
                obj2["CardUser"] = CardUserModule.CardUserToJson(cardUser);
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }

    }
}
