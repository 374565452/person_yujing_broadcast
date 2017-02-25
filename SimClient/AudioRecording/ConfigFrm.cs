using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AudioRecording
{
    public partial class ConfigFrm : Form
    {

        private string ip;

        private string port;

        private XmlHelper xmlHelper;

        private string xmlFilePath;

        private string sampleRate;
        private string sampleBits;

        public ConfigFrm(string m_xmlFilePath,string m_ip,string m_port,string m_rate,string m_bits)
        {
            InitializeComponent();
            ip = m_ip;
            port = m_port;
            this.ipTxt.Text = ip;
            this.portTxt.Text = port;
            this.sampleRate = m_rate;
            this.sampleBits = m_bits;
            for (int i = 0; i < bitRate.Items.Count; i++)
            {
                if (bitRate.Items[i].ToString().Equals(sampleRate))
                {
                    bitRate.SelectedIndex = i;
                }
            }

            for (int i = 0; i < bits.Items.Count; i++)
            {
                if (bits.Items[i].ToString().Equals(sampleBits))
                {
                    bits.SelectedIndex = i;
                }
            }

            xmlFilePath = m_xmlFilePath;
            xmlHelper = new XmlHelper(xmlFilePath);
        }

        private void sureBtn_Click(object sender, EventArgs e)
        {
            string newIp = this.ipTxt.Text.Trim();
            string newPort = this.portTxt.Text.Trim();
            xmlHelper.saveConfig("uploadServerIp", newIp);
            xmlHelper.saveConfig("uploadServerPort", newPort);
            xmlHelper.saveConfig("sampleRate", bitRate.SelectedItem.ToString());
            xmlHelper.saveConfig("sampleBits", bits.SelectedItem.ToString());
        }


    }
}
