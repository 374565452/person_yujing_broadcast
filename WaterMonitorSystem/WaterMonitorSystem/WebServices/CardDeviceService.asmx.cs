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
    /// CardDeviceService 的摘要说明
    /// </summary>
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1), WebService(Name = "设备卡管理服务", Description = "提供设备的增删查改服务", Namespace = "http://www.data86.net/")]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class CardDeviceService : System.Web.Services.WebService
    {
        private HttpContext context = HttpContext.Current;

        public CardDeviceService()
        {
            this.context.Response.Buffer = true;
            this.context.Response.ExpiresAbsolute = DateTime.Now.AddSeconds(-1.0);
            this.context.Response.Expires = 0;
            this.context.Response.CacheControl = "no-cache";
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>根据村庄ID获取该村所有设备卡</span><br/><p>输入参数：loginIdentifer=登录用户标识，villageId=村庄ID，isExport=是否导出Excel<br/>返回数据格式：{'Result':bool,'Message':string,'CardDevices':[object1...objectn]}</p>")]
        public string GetCardDevicesByVillageId(string loginIdentifer, string villageId, bool isExport)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            JavaScriptArray array = new JavaScriptArray();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("CardDevices", array);
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
                District dist = DistrictModule.ReturnDistrictInfo(long.Parse(villageId));
                if (dist == null)
                {
                    obj2["Message"] = "ID为" + villageId + "的村庄不存在";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                List<CardDevice> liCardDevices = CardDeviceModule.GetCardDevicesByDistrict(dist);
                foreach (CardDevice info in liCardDevices)
                {
                    array.Add(CardDeviceModule.CardDeviceToJson(info));
                }
                obj2["Result"] = true;
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>根据设备卡ID列表获取设备卡</span><br/><p>输入参数：loginIdentifer=登录用户标识，Ids=设备卡ID列表<br/>返回数据格式：{'Result':bool,'Message':string,'CardDevices':[object1...objectn]}</p>")]
        public string GetCardDeviceByIds(string loginIdentifer, string Ids)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            JavaScriptArray array = new JavaScriptArray();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("CardDevices", array);
            try
            {
                ResMsg msg = CommonUtil.CheckLoginState(loginIdentifer, true);
                if (!msg.Result)
                {
                    obj2["Message"] = msg.Message;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                if ((Ids == null) || (Ids.Trim() == ""))
                {
                    obj2["Message"] = "设备卡ID不能为空";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                foreach (string str in Ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    CardDevice card = CardDeviceModule.GetCardDeviceById(long.Parse(str));
                    if (card == null)
                    {
                        obj2["Message"] = "ID为" + str + "的设备卡不存在";
                        return JavaScriptConvert.SerializeObject(obj2);
                    }
                    array.Add(CardDeviceModule.CardDeviceToJson(card));
                }
                obj2["Result"] = true;
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }


        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>根据设备卡ID获取设备卡信息</span><br/><p>输入参数：loginIdentifer=登录用户标识，cardDeviceID=设备卡ID<br/>返回数据格式：{'Result':bool,'Message':string,'CardDevice':object}</p>")]
        public string GetCardDeviceById(string loginIdentifer, string cardDeviceID)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("CardDevice", new JavaScriptObject());
            try
            {
                ResMsg msg = CommonUtil.CheckLoginState(loginIdentifer, true);
                if (!msg.Result)
                {
                    obj2["Message"] = msg.Message;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                if ((cardDeviceID == null) || (cardDeviceID.Trim() == ""))
                {
                    obj2["Message"] = "设备卡ID不能为空";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                CardDevice cardDevice = CardDeviceModule.GetCardDeviceById(long.Parse(cardDeviceID));
                if (cardDevice == null)
                {
                    obj2["Message"] = "ID为" + cardDeviceID + "设备卡不存在";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                obj2["Result"] = true;
                obj2["CardDevice"] = CardDeviceModule.CardDeviceToJson(cardDevice);
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }

    }
}
