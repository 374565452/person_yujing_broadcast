using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Web;
using System.Web.Services;
using System.Xml;

namespace WaterMonitorSystem.WebServices
{
    /// <summary>
    /// GlobalViewService 的摘要说明
    /// </summary>
    [Serializable, ToolboxItem(false), WebService(Description = "支持获取地图配置", Name = "全局总览服务", Namespace = "http://www.data86.net/"), WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class GlobalViewService : System.Web.Services.WebService
    {
        private HttpContext context = HttpContext.Current;

        public GlobalViewService()
        {
            this.context.Response.Buffer = true;
            this.context.Response.ExpiresAbsolute = DateTime.Now.AddSeconds(-1.0);
            this.context.Response.Expires = 0;
            this.context.Response.CacheControl = "no-cache";
        }

        [WebMethod]
        public string GetMapConfig()
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            JavaScriptObject obj3 = new JavaScriptObject();
            obj2.Add("MapConfig", obj3);
            this.context.Server.MapPath("MapConfig.xml");
            XmlDocument document = new XmlDocument();
            document.Load(AppDomain.CurrentDomain.BaseDirectory + @"App_Config\MapConfig.config");
            XmlNode node = null;
            try
            {
                node = document.SelectSingleNode("MapConfig");
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
            }
            if (node != null)
            {
                XmlNode node2 = node.SelectSingleNode("基本设置");
                JavaScriptObject obj4 = new JavaScriptObject();
                obj3.Add("基本设置", obj4);
                obj4.Add("刷新时间", node2["刷新时间"].InnerText);
                obj4.Add("中心坐标", node2["中心坐标"].InnerText);
                obj4.Add("初始缩放级别", node2["初始缩放级别"].InnerText);
                obj4.Add("最小缩放级别", node2["最小缩放级别"].InnerText);
                obj4.Add("最大缩放级别", node2["最大缩放级别"].InnerText);
                XmlNodeList list = node.SelectNodes("图例信息");
                JavaScriptArray array = new JavaScriptArray();
                obj3.Add("图例信息", array);
                foreach (XmlNode node3 in list)
                {
                    XmlElement element = (XmlElement)node3;
                    JavaScriptObject item = new JavaScriptObject();
                    item.Add("Left", element.GetAttribute("Left"));
                    item.Add("Top", element.GetAttribute("Top"));
                    item.Add("Width", element.GetAttribute("Width"));
                    item.Add("Height", element.GetAttribute("Height"));
                    item.Add("BackColor", element.GetAttribute("BackColor"));
                    item.Add("BackOpacity", element.GetAttribute("BackOpacity"));
                    item.Add("BorderColor", element.GetAttribute("BorderColor"));
                    XmlNodeList list2 = element.SelectNodes("图例");
                    JavaScriptArray array2 = new JavaScriptArray();
                    foreach (XmlNode node4 in list2)
                    {
                        JavaScriptObject obj6 = new JavaScriptObject();
                        obj6.Add("图例状态", node4.InnerText.Split(new char[] { ',' })[0]);
                        obj6.Add("图例图片", node4.InnerText.Split(new char[] { ',' })[1]);
                        obj6.Add("图片宽度", node4.Attributes["PicWidth"].Value);
                        obj6.Add("图片高度", node4.Attributes["PicHeight"].Value);
                        array2.Add(obj6);
                    }
                    item.Add("图例", array2);
                    array.Add(item);
                }
                XmlNodeList list3 = node.SelectNodes("用户模板");
                JavaScriptArray array3 = new JavaScriptArray();
                obj3.Add("用户模板", array3);
                foreach (XmlNode node5 in list3)
                {
                    XmlElement element2 = (XmlElement)node5;
                    JavaScriptObject obj7 = new JavaScriptObject();
                    obj7.Add("ID", element2["ID"].InnerText);
                    obj7.Add("采集量", element2["采集量"].InnerText);
                    obj7.Add("状态量", element2["状态量"].InnerText);
                    XmlNode node6 = element2.SelectSingleNode("状态信息");
                    JavaScriptArray array4 = new JavaScriptArray();
                    foreach (XmlNode node7 in node6)
                    {
                        JavaScriptObject obj8 = new JavaScriptObject();
                        obj8.Add("状态信息", node7.InnerText.Split(new char[] { ',' })[0]);
                        obj8.Add("状态图片", node7.InnerText.Split(new char[] { ',' })[1]);
                        array4.Add(obj8);
                    }
                    obj7.Add("状态信息", array4);
                    array3.Add(obj7);
                }
                obj2["Result"] = true;
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }
    }
}
