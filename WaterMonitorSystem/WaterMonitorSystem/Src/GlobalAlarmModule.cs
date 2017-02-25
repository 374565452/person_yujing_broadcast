using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace WaterMonitorSystem.Src
{
    public static class GlobalAlarmModule
    {
        private static Dictionary<string, GlobalAlarmConfig> dicGlobalAlarmConfig = new Dictionary<string, GlobalAlarmConfig>();
        private static string strXmlFile = (AppDomain.CurrentDomain.BaseDirectory + @"App_Config\GlobalAlarmConfig.config");
        private static XmlDocument xmlDoc = new XmlDocument();

        static GlobalAlarmModule()
        {
            FileInfo info = new FileInfo(strXmlFile);
            if (info.Exists)
            {
                try
                {
                    xmlDoc.Load(info.FullName);
                    bool flag = false;
                    XmlNodeList list = xmlDoc.SelectNodes("/GlobalAlarmConfig/报警配置");
                    if ((list != null) && (list.Count != 0))
                    {
                        foreach (XmlNode node in list)
                        {
                            if (node["用户ID"] != null)
                            {
                                GlobalAlarmConfig config = new GlobalAlarmConfig
                                {
                                    UserId = node["用户ID"].InnerText.Trim()
                                };
                                if (node["启用报警功能"] == null)
                                {
                                    XmlElement newChild = xmlDoc.CreateElement("启用报警功能");
                                    newChild.InnerText = "是";
                                    node.AppendChild(newChild);
                                    flag = true;
                                }
                                else if (node["启用报警功能"].InnerText.Trim() == "否")
                                {
                                    config.UseAlarm = false;
                                }
                                if (node["启用声音报警"] == null)
                                {
                                    XmlElement element2 = xmlDoc.CreateElement("启用声音报警");
                                    element2.InnerText = "是";
                                    node.AppendChild(element2);
                                    flag = true;
                                }
                                else if (node["启用声音报警"].InnerText.Trim() == "否")
                                {
                                    config.UseVoice = false;
                                }
                                if (node["启用自动弹出"] == null)
                                {
                                    XmlElement element3 = xmlDoc.CreateElement("启用自动弹出");
                                    element3.InnerText = "是";
                                    node.AppendChild(element3);
                                    flag = true;
                                }
                                else if (node["启用自动弹出"].InnerText.Trim() == "否")
                                {
                                    config.AutoPopup = false;
                                }
                                if (dicGlobalAlarmConfig.ContainsKey(config.UserId))
                                {
                                    dicGlobalAlarmConfig.Add(config.UserId, config);
                                }
                                else
                                {
                                    dicGlobalAlarmConfig[config.UserId] = config;
                                }
                            }
                        }
                    }
                    if (flag)
                    {
                        xmlDoc.Save(strXmlFile);
                    }
                }
                catch
                {
                }
            }
            else
            {
                try
                {
                    xmlDoc.LoadXml("<?xml version=\"1.0\" encoding=\"gb2312\"?><GlobalAlarmConfig><报警配置><!--0为默认配置--><用户ID>0</用户ID><!--是/否--><启用报警功能>是</启用报警功能><!--是/否--><启用声音报警>是</启用声音报警><!--是/否--><启用自动弹出>是</启用自动弹出></报警配置></GlobalAlarmConfig>");
                    xmlDoc.Save(strXmlFile);
                    GlobalAlarmConfig config2 = new GlobalAlarmConfig
                    {
                        UserId = "0"
                    };
                    dicGlobalAlarmConfig.Add("0", config2);
                }
                catch
                {
                }
            }
        }

        public static GlobalAlarmConfig GetGlobalAlarmConfigByUserId(string strUserId)
        {
            if (dicGlobalAlarmConfig.ContainsKey(strUserId))
            {
                return dicGlobalAlarmConfig[strUserId].Copy();
            }
            if (dicGlobalAlarmConfig.ContainsKey("0"))
            {
                return dicGlobalAlarmConfig["0"].Copy();
            }
            return new GlobalAlarmConfig();
        }

        public static bool SetGlobalAlarmConfig(string strUserId, bool blUseAlarm, bool blUseVoice, bool blAutoPopup)
        {
            bool flag = false;
            GlobalAlarmConfig config = new GlobalAlarmConfig(strUserId, blUseAlarm, blUseVoice, blAutoPopup);
            if (dicGlobalAlarmConfig.ContainsKey(strUserId))
            {
                dicGlobalAlarmConfig[strUserId] = config;
                XmlNodeList list = xmlDoc.SelectNodes("/GlobalAlarmConfig/报警配置");
                if ((list != null) && (list.Count != 0))
                {
                    foreach (XmlNode node in list)
                    {
                        if ((node["用户ID"] != null) && (node["用户ID"].InnerText.Trim() == strUserId))
                        {
                            string str = blUseAlarm ? "是" : "否";
                            if (node["启用报警功能"] != null)
                            {
                                node["启用报警功能"].InnerText = str;
                            }
                            else
                            {
                                XmlElement element = xmlDoc.CreateElement("启用报警功能");
                                element.InnerText = str;
                                node.AppendChild(element);
                            }
                            string str2 = blUseVoice ? "是" : "否";
                            if (node["启用声音报警"] != null)
                            {
                                node["启用声音报警"].InnerText = str2;
                            }
                            else
                            {
                                XmlElement element2 = xmlDoc.CreateElement("启用声音报警");
                                element2.InnerText = str2;
                                node.AppendChild(element2);
                            }
                            string str3 = blAutoPopup ? "是" : "否";
                            if (node["启用自动弹出"] != null)
                            {
                                node["启用自动弹出"].InnerText = str3;
                            }
                            else
                            {
                                XmlElement element3 = xmlDoc.CreateElement("启用自动弹出");
                                element3.InnerText = str3;
                                node.AppendChild(element3);
                            }
                            try
                            {
                                xmlDoc.Save(strXmlFile);
                                flag = true;
                            }
                            catch
                            {
                            }
                        }
                    }
                }
                return flag;
            }
            dicGlobalAlarmConfig.Add(strUserId, config);
            XmlNodeList list2 = xmlDoc.SelectNodes("/GlobalAlarmConfig/报警配置");
            if ((list2 == null) || (list2.Count == 0))
            {
                xmlDoc.LoadXml("<?xml version=\"1.0\" encoding=\"gb2312\"?><GlobalAlarmConfig><报警配置><!--0为默认配置--><用户ID>0</用户ID><!--是/否--><启用报警功能>是</启用报警功能><!--是/否--><启用声音报警>是</启用声音报警><!--是/否--><启用自动弹出>是</启用自动弹出></报警配置></GlobalAlarmConfig>");
            }
            XmlElement newChild = xmlDoc.CreateElement("报警配置");
            XmlElement element5 = xmlDoc.CreateElement("用户ID");
            element5.InnerText = strUserId;
            XmlElement element6 = xmlDoc.CreateElement("启用报警功能");
            element6.InnerText = blUseAlarm ? "是" : "否";
            XmlElement element7 = xmlDoc.CreateElement("启用声音报警");
            element7.InnerText = blUseVoice ? "是" : "否";
            XmlElement element8 = xmlDoc.CreateElement("启用自动弹出");
            element8.InnerText = blAutoPopup ? "是" : "否";
            newChild.AppendChild(element5);
            newChild.AppendChild(element6);
            newChild.AppendChild(element7);
            newChild.AppendChild(element8);
            xmlDoc.SelectSingleNode("/GlobalAlarmConfig").AppendChild(newChild);
            try
            {
                xmlDoc.Save(strXmlFile);
                flag = true;
            }
            catch
            {
            }
            return flag;
        }
    }
}