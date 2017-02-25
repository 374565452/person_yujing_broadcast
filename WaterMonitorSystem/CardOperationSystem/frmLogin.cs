using Common;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Utils;

namespace CardOperationSystem
{
    public partial class frmLogin : Form
    {
        public frmLogin()
        {
            InitializeComponent();

            this.skinEngine1.SkinFile = Application.StartupPath + "/Skins/SportsBlue.ssk";

            string app_ServerIP = ConfigHelper.GetAppConfig("ServerIP");
            if (app_ServerIP != null)
            {
                DataTransfer.ServerIP = app_ServerIP;
            }
            else
            {
                ConfigHelper.UpdateAppConfig("ServerIP", DataTransfer.ServerIP);
            }

            string app_ServerPort = ConfigHelper.GetAppConfig("ServerPort");
            if (app_ServerPort != null)
            {
                DataTransfer.ServerPort = Tools.StringToInt(app_ServerPort, 8100);
            }
            else
            {
                ConfigHelper.UpdateAppConfig("ServerPort", DataTransfer.ServerPort.ToString());
            }
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {
            this.label3.Text = "";
            InfoSys.NetCardIP = WinInfo.GetNetCardIP();
            InfoSys.NetCardMAC = WinInfo.GetNetCardMAC();
            InfoSys.HostName = WinInfo.Get_HostName();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.label3.ForeColor = Color.Blue;
            this.label3.Text = "正在登录，请等待。。。";

            InfoSys.LoginIsLogin = false;
            this.button1.Enabled = false;
            using (BackgroundWorker bw = new BackgroundWorker())
            {
                bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
                bw.DoWork += new DoWorkEventHandler(bw_DoWork);
                bw.RunWorkerAsync("");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        frmConfig fConfig = null;
        private void button3_Click(object sender, EventArgs e)
        {
            if (fConfig == null) //如果子窗体为空则创造实例 并显示
            {
                fConfig = new frmConfig(false);
                fConfig.Show();
            }
            else
            {
                if (fConfig.IsDisposed)//若子窗体关闭 则打开新子窗体 并显示
                {
                    fConfig = new frmConfig(false);
                    fConfig.Show();
                }
                else
                {
                    fConfig.Activate(); //使子窗体获得焦点
                }
            }
        }

        private void frmLogin_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!InfoSys.LoginIsLogin)
            {
                if (MessageBox.Show("确定退出程序？", "退出", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {

                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            //MessageBox.Show(Thread.CurrentThread.ManagedThreadId.ToString());
            //e.Result = e.Argument;//这里只是简单的把参数当做结果返回，当然您也可以在这里做复杂的处理后，再返回自己想要的结果(这里的操作是在另一个线程上完成的)
            JavaScriptObject result = new JavaScriptObject();
            try
            {
                InfoSys.LoginUserName = this.txtUserName.Text.Trim();
                InfoSys.LoginPassword = this.txtPassword.Text;
                string str = DataTransfer.UserLogin();

                result = (JavaScriptObject)JavaScriptConvert.DeserializeObject(str);
            }
            catch (Exception ex)
            {
                result["Message"] = ex.Message;
            }

            e.Result = result;
        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //这时后台线程已经完成，并返回了主线程，所以可以直接使用UI控件了
            //this.textBox1.Text = e.Result.ToString();

            JavaScriptObject result = (JavaScriptObject)e.Result;
            if (bool.Parse(result["Result"].ToString()))
            {
                InfoSys.LoginIsLogin = true;

                InfoSys.GetBaseInfo();

                JsonSysUser user = JavaScriptConvert.DeserializeObject<JsonSysUser>(result["Message"].ToString());
                InfoSys.UserId = user.UserId;
                InfoSys.UserTrueName = user.TrueName;
                InfoSys.UserGroupName = user.GroupName;
                InfoSys.UserDistrictName = user.DistrictName;

                frmMain f = new frmMain();
                f.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show(result["Message"].ToString());
                this.label3.ForeColor = Color.Red;
                this.label3.Text = "请检查服务器配置";
                this.button1.Enabled = true;
            }
        } 
    }
}
