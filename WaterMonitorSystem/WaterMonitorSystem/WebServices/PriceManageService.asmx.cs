using Common;
using Maticsoft.Model;
using Module;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Xml;
using WaterMonitorSystem.Src;

namespace WaterMonitorSystem.WebServices
{
    /// <summary>
    /// PriceManageService 的摘要说明
    /// </summary>
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1), WebService(Name = "价格管理服务", Description = "提供价格的增删查改服务", Namespace = "http://www.data86.net/")]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class PriceManageService : System.Web.Services.WebService
    {
        private HttpContext context = HttpContext.Current;

        public PriceManageService()
        {
            this.context.Response.Buffer = true;
            this.context.Response.ExpiresAbsolute = DateTime.Now.AddSeconds(-1.0);
            this.context.Response.Expires = 0;
            this.context.Response.CacheControl = "no-cache";
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>添加价格信息</span><br/><p>输入参数：loginIdentifer=登录用户标识，priceJson=价格JSON对象字符串<br/>返回数据格式：{'Result':bool,'Message':string}</p>")]
        public string AddPriceInfo(string loginIdentifer, string priceJson)
        {
            string[] strArray = new string[] { "一阶名称", "二阶名称", "三阶名称", "四阶名称" };
            string[] strArray2 = new string[4];
            string[] strArray3 = new string[4];
            XmlDocument document = new XmlDocument();
            string filename = AppDomain.CurrentDomain.BaseDirectory.ToString() + @"App_Config\Price.config";
            document.Load(filename);
            XmlNode node = document.GetElementsByTagName("水价").Item(0);
            XmlNode node2 = document.GetElementsByTagName("电价").Item(0);
            int num = int.Parse(document.SelectSingleNode("价格设置/水价/阶梯数量").InnerText);
            int num2 = int.Parse(document.SelectSingleNode("价格设置/电价/阶梯数量").InnerText);
            for (int i = 0; (i < num) && (i < strArray.Length); i++)
            {
                XmlNode node5 = node.SelectSingleNode(strArray[i]);
                strArray2[i] = node5.InnerText;
            }
            for (int j = 0; (j < num2) && (j < strArray.Length); j++)
            {
                XmlNode node6 = node2.SelectSingleNode(strArray[j]);
                strArray3[j] = node6.InnerText;
            }
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
            JavaScriptObject jso = (JavaScriptObject)JavaScriptConvert.DeserializeObject(priceJson);
            if (jso == null)
            {
                obj2["Message"] = "参数priceJson格式不正确";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            PriceInfo pi = null;
            try
            {
                pi = this.JsonToPriceInfo(jso);
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
                return JavaScriptConvert.SerializeObject(obj2);
            }
            ResMsg msg = PriceModule.AddPriceInfo(pi);
            obj2["Result"] = msg.Result;
            obj2["Message"] = msg.Message;
            try
            {
                SysLog log = new SysLog();
                log.LogUserId = loginUser.UserId;
                log.LogUserName = loginUser.LoginName;
                log.LogAddress = ToolsWeb.GetIP(context.Request);
                log.LogTime = DateTime.Now;
                log.LogType = "添加价格信息";
                log.LogContent = msg + "|" + ModelHandler<PriceInfo>.ToString(pi);
                SysLogModule.Add(log);
            }
            catch { }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>删除价格信息</span><br/><p>输入参数：loginIdentifer=登录用户标识，priceId=价格ID<br/>返回数据格式：{'Result':bool,'Message':string}</p>")]
        public string DeletePriceInfo(string loginIdentifer, string priceId)
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
            ResMsg msg = PriceModule.DeletePriceInfoById(priceId);
            obj2["Result"] = msg.Result;
            obj2["Message"] = msg.Message;
            try
            {
                SysLog log = new SysLog();
                log.LogUserId = loginUser.UserId;
                log.LogUserName = loginUser.LoginName;
                log.LogAddress = ToolsWeb.GetIP(context.Request);
                log.LogTime = DateTime.Now;
                log.LogType = "删除价格信息";
                log.LogContent = msg + "|" + priceId;
                SysLogModule.Add(log);
            }
            catch { }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>获取所有价格信息</span><br/><p>输入参数：loginIdentifer=登录用户标识<br/>返回数据格式：{'Result':bool,'Message':string,'PriceInfos':[object1,...,objectn]}</p>")]
        public string GetAllPriceInfos(string loginIdentifer)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            JavaScriptArray array = new JavaScriptArray();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("PriceInfos", array);
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
            foreach (PriceInfo info in PriceModule.GetAllPriceInfos())
            {
                array.Add(this.PriceInfoToJson(info));
            }
            obj2["Result"] = true;
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>获取所有的电价信息</span><br/><p>输入参数：loginIdentifer=登录用户标识<br/>返回数据格式：{'Result':bool,'Message':string,'PriceInfos':[object1,...,objectn]}</p>")]
        public string GetPowerPriceInfos(string loginIdentifer)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            JavaScriptArray array = new JavaScriptArray();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("PriceInfos", array);
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
            foreach (PriceInfo info in PriceModule.GetAllPriceInfos())
            {
                if (info.Type == "2")
                {
                    array.Add(this.PriceInfoToJson(info));
                }
            }
            obj2["Result"] = true;
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>根据ID获取价格信息</span><br/><p>输入参数：loginIdentifer=登录用户标识，priceId=价格ID<br/>返回数据格式：{'Result':bool,'Message':string,'PriceInfo':object}</p>")]
        public string GetPriceInfo(string loginIdentifer, string priceId)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("PriceInfo", new JavaScriptObject());
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
            PriceInfo priceInfoById = PriceModule.GetPriceInfoById(priceId);
            if (priceInfoById != null)
            {
                obj2["PriceInfo"] = this.PriceInfoToJson(priceInfoById);
                obj2["Result"] = true;
            }
            else
            {
                obj2["Message"] = "价格ID为" + priceId + "价格信息不存在";
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>获取所有的水价信息</span><br/><p>输入参数：loginIdentifer=登录用户标识<br/>返回数据格式：{'Result':bool,'Message':string,'PriceInfos':[object1,...,objectn]}</p>")]
        public string GetWaterPriceInfos(string loginIdentifer)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            JavaScriptArray array = new JavaScriptArray();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("PriceInfos", array);
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
            foreach (PriceInfo info in PriceModule.GetAllPriceInfos())
            {
                if (info.Type == "1")
                {
                    array.Add(this.PriceInfoToJson(info));
                }
            }
            obj2["Result"] = true;
            return JavaScriptConvert.SerializeObject(obj2);
        }

        private PriceInfo JsonToPriceInfo(JavaScriptObject jso)
        {
            PriceInfo info = new PriceInfo
            {
                Id = (!jso.ContainsKey("ID") || (jso["ID"] == null)) ? "" : jso["ID"].ToString(),
                Name = jso["名称"].ToString(),
                Type = jso["价格类型"].ToString(),
                SetTime = DateTime.Now,
                LaddersCount = Convert.ToInt32(jso["阶梯数量"].ToString()),
                LadderType = (PriceInfo.LaddersType)Enum.Parse(typeof(PriceInfo.LaddersType), jso["阶梯类型"].ToString()),
                StartTime = (!jso.ContainsKey("起始时间") || (jso["起始时间"] == null)) ? "" : jso["起始时间"].ToString(),
                EndTime = (!jso.ContainsKey("结束时间") || (jso["结束时间"] == null)) ? "" : jso["结束时间"].ToString()
            };
            if (info.LaddersCount > 0)
            {
                info.FirstPrice = Convert.ToDecimal(jso["一阶价格"].ToString());
                info.FirstVolume = Convert.ToDecimal(jso["一阶水量"].ToString());
            }
            if (info.LaddersCount > 1)
            {
                info.SecondPrice = Convert.ToDecimal(jso["二阶价格"].ToString());
                info.SecondVolume = Convert.ToDecimal(jso["二阶水量"].ToString());
            }
            if (info.LaddersCount > 2)
            {
                info.ThirdPrice = Convert.ToDecimal(jso["三阶价格"].ToString());
                info.ThirdVolume = Convert.ToDecimal(jso["三阶水量"].ToString());
            }
            if (info.LaddersCount > 3)
            {
                info.FourthPrice = Convert.ToDecimal(jso["四阶价格"].ToString());
                info.FourthVolume = Convert.ToDecimal(jso["四阶水量"].ToString());
            }
            return info;
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>获取价格设置配置文件信息</span><br/><p><br/>返回数据格式：{'Result':bool,'Message':string,'WaterStepsCount':int,'ElectricStepsCount':int,'WaterColumns':[object1,...,objectn],'ElectricColumns':[object1,...,objectn]}</p>")]
        public string LoadPriceConfig(string loginIdentifer)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            obj2["Result"] = false;
            obj2["Message"] = "";
            JavaScriptObject obj3 = new JavaScriptObject();
            JavaScriptArray array = new JavaScriptArray();
            JavaScriptArray array2 = new JavaScriptArray();
            string[] strArray = new string[] { "一阶名称", "二阶名称", "三阶名称", "四阶名称" };
            string[] strArray2 = new string[] { "一阶定额", "二阶定额", "三阶定额", "四阶定额" };
            try
            {
                XmlDocument document = new XmlDocument();
                string filename = AppDomain.CurrentDomain.BaseDirectory.ToString() + @"App_Config\Price.config";
                document.Load(filename);
                XmlNode node = document.GetElementsByTagName("水价").Item(0);
                XmlNode node2 = document.GetElementsByTagName("电价").Item(0);
                JavaScriptObject item = new JavaScriptObject();
                item["field"] = "name";
                item["title"] = "名称";
                item["width"] = 100;
                array.Add(item);
                string innerText = document.SelectSingleNode("价格设置/水价/阶梯类型").InnerText;
                obj3["WaterStepsType"] = innerText;
                int num = int.Parse(document.SelectSingleNode("价格设置/水价/阶梯数量").InnerText);
                obj3["WaterStepsCount"] = num;
                for (int i = 0; (i < num) && (i < strArray.Length); i++)
                {
                    JavaScriptObject obj5 = new JavaScriptObject();
                    XmlNode node5 = node.SelectSingleNode(strArray[i]);
                    XmlNode node6 = node.SelectSingleNode(strArray2[i]);
                    obj5["field"] = "waterStep" + (i + 1);
                    obj5["title"] = node5.InnerText;
                    obj5["width"] = 100;
                    obj5["percent"] = node6.InnerText;
                    array.Add(obj5);
                }
                obj3["WaterColumns"] = array;
                JavaScriptObject obj6 = new JavaScriptObject();
                obj6["field"] = "name";
                obj6["title"] = "名称";
                obj6["width"] = 100;
                array2.Add(obj6);
                string str3 = document.SelectSingleNode("价格设置/电价/阶梯类型").InnerText;
                obj3["PowerStepsType"] = str3;
                int num3 = int.Parse(document.SelectSingleNode("价格设置/电价/阶梯数量").InnerText);
                obj3["PowerStepsCount"] = num3;
                for (int j = 0; (j < num3) && (j < strArray.Length); j++)
                {
                    JavaScriptObject obj7 = new JavaScriptObject();
                    XmlNode node9 = node2.SelectSingleNode(strArray[j]);
                    XmlNode node10 = node2.SelectSingleNode(strArray2[j]);
                    obj7["field"] = "powerStep" + (j + 1);
                    obj7["title"] = node9.InnerText;
                    obj7["width"] = 100;
                    obj7["percent"] = node10.InnerText;
                    array2.Add(obj7);
                }
                obj3["PowerColumns"] = array2;
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
                return JavaScriptConvert.SerializeObject(obj2);
            }
            obj2["PriceConfig"] = obj3;
            obj2["Result"] = true;
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>修改价格信息</span><br/><p>输入参数：loginIdentifer=登录用户标识，priceJson=价格JSON对象字符串<br/>返回数据格式：{'Result':bool,'Message':string}</p>")]
        public string ModifyPriceInfo(string loginIdentifer, string priceJson)
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
            JavaScriptObject jso = (JavaScriptObject)JavaScriptConvert.DeserializeObject(priceJson);
            if (jso == null)
            {
                obj2["Message"] = "参数priceJson格式不正确";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            if ((!jso.ContainsKey("ID") || (jso["ID"] == null)) || (jso["ID"].ToString().Trim() == ""))
            {
                obj2["Message"] = "价格ID不能为空";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            PriceInfo pi = null;
            try
            {
                pi = this.JsonToPriceInfo(jso);
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
                return JavaScriptConvert.SerializeObject(obj2);
            }
            ResMsg msg = PriceModule.ModifyPriceInfo(pi);
            string[] strArray = new string[] { "一阶名称", "二阶名称", "三阶名称", "四阶名称" };
            string[] strArray2 = new string[4];
            string[] strArray3 = new string[4];
            XmlDocument document = new XmlDocument();
            string filename = AppDomain.CurrentDomain.BaseDirectory.ToString() + @"App_Config\Price.config";
            document.Load(filename);
            XmlNode node = document.GetElementsByTagName("水价").Item(0);
            XmlNode node2 = document.GetElementsByTagName("电价").Item(0);
            int num = int.Parse(document.SelectSingleNode("价格设置/水价/阶梯数量").InnerText);
            int num2 = int.Parse(document.SelectSingleNode("价格设置/电价/阶梯数量").InnerText);
            for (int i = 0; (i < num) && (i < strArray.Length); i++)
            {
                XmlNode node5 = node.SelectSingleNode(strArray[i]);
                strArray2[i] = node5.InnerText;
            }
            for (int j = 0; (j < num2) && (j < strArray.Length); j++)
            {
                XmlNode node6 = node2.SelectSingleNode(strArray[j]);
                strArray3[j] = node6.InnerText;
            }
            obj2["Result"] = msg.Result;
            obj2["Message"] = msg.Message;
            try
            {
                SysLog log = new SysLog();
                log.LogUserId = loginUser.UserId;
                log.LogUserName = loginUser.LoginName;
                log.LogAddress = ToolsWeb.GetIP(context.Request);
                log.LogTime = DateTime.Now;
                log.LogType = "修改价格信息";
                log.LogContent = msg + "|" + ModelHandler<PriceInfo>.ToString(pi);
                SysLogModule.Add(log);
            }
            catch { }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        private JavaScriptObject PriceInfoToJson(PriceInfo pi)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            obj2.Add("ID", pi.Id);
            obj2.Add("名称", pi.Name);
            obj2.Add("价格类型", pi.Type);
            obj2.Add("阶梯数量", pi.LaddersCount.ToString());
            obj2.Add("阶梯类型", pi.LadderType.ToString());
            obj2.Add("一阶水量", pi.FourthVolume);
            obj2.Add("一阶价格", pi.FirstPrice);
            obj2.Add("二阶水量", pi.SecondVolume);
            obj2.Add("二阶价格", pi.SecondPrice);
            obj2.Add("三阶水量", pi.ThirdVolume);
            obj2.Add("三阶价格", pi.ThirdPrice);
            obj2.Add("四阶水量", pi.FourthVolume);
            obj2.Add("四阶价格", pi.FourthPrice);
            obj2.Add("设置时间", pi.SetTime.ToString("yyyy-MM-dd HH:mm:ss"));
            return obj2;
        }
    }
}
