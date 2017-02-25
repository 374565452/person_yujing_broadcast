using System.IO;
using System.Xml;

namespace Common
{
    public class XmlHelper
    {
        private XmlDocument xml;

        private string xmlFilePath = string.Empty;

        public XmlHelper(string xmlPath)
        {
            xml = new XmlDocument();
            xmlFilePath = xmlPath;
            xml.Load(xmlFilePath);
        }

        #region 根据key获取value
        public string getValue(string key)
        {
            //xml.Load("config/system.xml");
            string value;
            string path = @"//configuration/appSettings/add[@key='" + key + "']";
            XmlNodeList xmlAdds = xml.SelectNodes(path);
            if (xmlAdds.Count == 1)
            {
                XmlElement xmlAdd = (XmlElement)xmlAdds[0];
                value = xmlAdd.GetAttribute("value");
            }
            else
            {
                value = null;
            }
            return value;
        }
        #endregion

        #region 保存配置
        public void saveConfig(string key, string value)
        {
            XmlNodeList list = xml.GetElementsByTagName("add");
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Attributes[0].Value == key)
                {
                    list[i].Attributes[1].Value = value;
                }
            }
            StreamWriter swriter = new StreamWriter(xmlFilePath);
            XmlTextWriter xw = new XmlTextWriter(swriter);
            xw.Formatting = Formatting.Indented;
            xml.WriteTo(xw);
            xw.Close();
        }
        #endregion
    }
}
