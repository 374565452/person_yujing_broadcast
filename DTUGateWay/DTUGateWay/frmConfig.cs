using DTU.GateWay.Protocol;
using Server.Util.Cache;
using System;
using System.Windows.Forms;

namespace DTUGateWay
{
    public partial class frmConfig : Form
    {
        private bool loadConfigFlag = true;
        private XmlHelper xmlHelper;

        //dtu服务端口号
        private int dtuPort = 0;

        //web服务端口号
        private int webPort = 0;

        //超时时间
        private int timeout = 0;

        //所能承受的最大连接数
        private int connectedCount = 0;

        //是否开启信息日志
        private bool showInfoLog = true;
        //错误日志开启
        private bool showErrorLog = true;

        //市级平台地址
        private string CityServerIp = "";
        //市级平台端口
        private int CityServerPort = 0;

        //是否允许进行事件记录功能，供查询、检索使用
        private bool recordEventEnable = true;

        public frmConfig()
        {
            InitializeComponent();
        }

        private void frmConfig_Load(object sender, EventArgs e)
        {
            try
            {
                string xmlConfig = Application.StartupPath + "\\setup.xml";
                xmlHelper = new XmlHelper(xmlConfig);//xml配置文件路径实例化
                loadConfigFlag = loadConfig();// 获取xml中的配置参数

                this.textBox1.Text = dtuPort.ToString();
                this.textBox2.Text = webPort.ToString();
                this.textBox3.Text = timeout.ToString();
                this.textBox4.Text = connectedCount.ToString();
                this.textBox5.Text = CityServerIp.ToString();
                this.textBox6.Text = CityServerPort.ToString();

                this.checkBox1.Checked = showInfoLog;
                this.checkBox2.Checked = showErrorLog;
                this.checkBox3.Checked = recordEventEnable;
            }
            catch (Exception ex)
            {
                MessageBox.Show("配置窗体加载失败！" + Environment.NewLine + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 初始化参数  port   timeout   xmal.config 里面的配置   获取成功返回ture
        /// </summary>
        /// <returns></returns>
        private bool loadConfig()
        {
            try
            {
                dtuPort = int.Parse(xmlHelper.getValue("dtuServerPort"));
                webPort = int.Parse(xmlHelper.getValue("webServerPort"));
                timeout = int.Parse(xmlHelper.getValue("socketTimeout"));
                connectedCount = int.Parse(xmlHelper.getValue("connectedCount"));
                int infoLog = int.Parse(xmlHelper.getValue("infoLogEnable"));
                int errorLog = int.Parse(xmlHelper.getValue("errorLogEnable"));
                int recordEvent = int.Parse(xmlHelper.getValue("recordEventEnable"));
                if (infoLog == 1)
                {
                    showInfoLog = true;
                }
                else
                {
                    showInfoLog = false;
                }
                if (errorLog == 1)
                {
                    showErrorLog = true;
                }
                else
                {
                    showErrorLog = false;
                }
                if (recordEvent == 1)
                {
                    this.recordEventEnable = true;
                }
                else
                {
                    this.recordEventEnable = false;
                }

                CityServerIp = xmlHelper.getValue("CityServerIp");
                try
                {
                    CityServerPort = int.Parse(xmlHelper.getValue("CityServerPort"));
                }
                catch { CityServerPort = 0; }
                return true;
            }
            catch (Exception)
            {
                MessageBox.Show("读取配置信息出错！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                dtuPort = int.Parse(this.textBox1.Text.Trim());
            }
            catch (Exception ex)
            {
                MessageBox.Show("dtu服务端口号格式不对", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (dtuPort < 1 || dtuPort > 65535)
            {
                MessageBox.Show("dtu服务端口号范围1-65535", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                webPort = int.Parse(this.textBox2.Text.Trim());
            }
            catch (Exception ex)
            {
                MessageBox.Show("web服务端口号格式不对", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (webPort < 1 || webPort > 65535)
            {
                MessageBox.Show("web服务端口号范围1-65535", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                timeout = int.Parse(this.textBox3.Text.Trim());
            }
            catch (Exception ex)
            {
                MessageBox.Show("超时时间格式不对", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (timeout < 60 || timeout > 60 * 30)
            {
                MessageBox.Show("超时时间范围60-1800", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                connectedCount = int.Parse(this.textBox4.Text.Trim());
            }
            catch 
            {
                MessageBox.Show("最大连接数格式不对", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (connectedCount < 10 || connectedCount > 50000)
            {
                MessageBox.Show("最大连接数范围10-50000", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            showInfoLog = this.checkBox1.Checked;

            showErrorLog = this.checkBox2.Checked;

            recordEventEnable = this.checkBox3.Checked;

            CityServerIp = this.textBox5.Text.Trim();

            try
            {
                CityServerPort = int.Parse(this.textBox6.Text.Trim());
            }
            catch 
            {
                MessageBox.Show("市级平台端口格式不对", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (CityServerPort < 0 || CityServerPort > 65535)
            {
                MessageBox.Show("市级平台服务端口号范围0-65535", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (MessageBox.Show("确定保存配置信息？必须重新启动系统配置信息才会生效！", "保存", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                xmlHelper.saveConfig("dtuServerPort", dtuPort.ToString());
                xmlHelper.saveConfig("webServerPort", webPort.ToString());
                xmlHelper.saveConfig("socketTimeout", timeout.ToString());
                xmlHelper.saveConfig("connectedCount", connectedCount.ToString());
                xmlHelper.saveConfig("infoLogEnable", showInfoLog ? "1" : "0");
                xmlHelper.saveConfig("errorLogEnable", showErrorLog ? "1" : "0");
                xmlHelper.saveConfig("recordEventEnable", recordEventEnable ? "1" : "0");
                xmlHelper.saveConfig("CityServerIp", CityServerIp);
                xmlHelper.saveConfig("CityServerPort", CityServerPort.ToString());

                SysCache.ShowInfoLog = showInfoLog;
                SysCache.ShowErrorLog = showErrorLog;
                SysCache.RecordEevent = recordEventEnable;

                BaseMessage.ShowLog = SysCache.ShowInfoLog;
                WaterBaseMessage.ShowLog = SysCache.ShowInfoLog;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.textBox1.Text = "7002";
            this.textBox2.Text = "9008";
            this.textBox3.Text = "120";
            this.textBox4.Text = "400";
            this.textBox5.Text = "";
            this.textBox6.Text = "0";

            this.checkBox1.Checked = true;
            this.checkBox2.Checked = true;
            this.checkBox3.Checked = true;
        }

        private void frmConfig_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true; 
        }
    }
}
