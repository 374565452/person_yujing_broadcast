using System.IO;
using System.Xml;

namespace AudioRecording
{
    public class XmlHelper
    {
        private XmlDocument xml = new XmlDocument();
        private string xmlFilePath = string.Empty;

        public XmlHelper(string xmlPath)
        {
            this.xmlFilePath = xmlPath;
            //this.xml.Load(this.xmlFilePath);
            loadXml();
        }
        public void loadXml()
        {
            this.xml.Load(this.xmlFilePath);
        }
        public string getValue(string key)
        {
            string xpath = "//configuration/appSettings/add[@key='" + key + "']";
            XmlNodeList list = this.xml.SelectNodes(xpath);
            if (list.Count == 1)
            {
                XmlElement element = (XmlElement)list[0];
                return element.GetAttribute("value");
            }
            return null;
        }

        public void saveConfig(string key, string value)
        {
            XmlNodeList elementsByTagName = this.xml.GetElementsByTagName("add");
            for (int i = 0; i < elementsByTagName.Count; i++)
            {
                if (elementsByTagName[i].Attributes[0].Value == key)
                {
                    elementsByTagName[i].Attributes[1].Value = value;
                }
            }
            StreamWriter w = new StreamWriter(this.xmlFilePath);
            XmlTextWriter writer2 = new XmlTextWriter(w)
            {
                Formatting = Formatting.Indented
            };
            this.xml.WriteTo(writer2);
            writer2.Close();
            w.Close();
        }
    }
}
