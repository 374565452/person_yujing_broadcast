using Common;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace CardOperationSystem
{
    public partial class frmConfig : Form
    {
        bool isLogin;
        public frmConfig(bool isLogin)
        {
            InitializeComponent();
            this.isLogin = isLogin;

            this.lbServerState.Text = "";
            this.txtServerIP.Text = DataTransfer.ServerIP;
            this.txtServerPort.Text = DataTransfer.ServerPort.ToString();

            this.txtServerIP.Enabled = !isLogin;
            this.txtServerPort.Enabled = !isLogin;
            this.btnSaveServer.Enabled = !isLogin;
        }

        private void frmConfig_Load(object sender, EventArgs e)
        {

        }

        private void btnSaveServer_Click(object sender, EventArgs e)
        {
            this.lbServerState.Text = "";
            DataTransfer.ServerIP = this.txtServerIP.Text.Trim();
            ConfigHelper.UpdateAppConfig("ServerIP", DataTransfer.ServerIP);
            DataTransfer.ServerPort = Tools.StringToInt(this.txtServerPort.Text.Trim(), 8100);
            ConfigHelper.UpdateAppConfig("ServerPort", DataTransfer.ServerPort.ToString());
            this.lbServerState.ForeColor = Color.Black;
            this.lbServerState.Text = "保存成功";
        }

        private void btnServerTest_Click(object sender, EventArgs e)
        {
            string ip = this.txtServerIP.Text.Trim();
            int port = 0;
            try
            {
                port = int.Parse(this.txtServerPort.Text.Trim());
            }
            catch { }
            if (port < 1 || port > 65535)
            {
                this.lbServerState.ForeColor = Color.Red;
                this.lbServerState.Text = "服务器端口取值范围是1-65535";
                return;
            }
            this.lbServerState.ForeColor = Color.Black;
            this.lbServerState.Text = "正在连接服务器。。。";
            this.btnServerTest.Enabled = false;
            this.btnSaveServer.Enabled = false;
            using (BackgroundWorker bw = new BackgroundWorker())
            {
                bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
                bw.DoWork += new DoWorkEventHandler(bw_DoWork);
                bw.RunWorkerAsync(ip + "," + port);
            }
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            //MessageBox.Show(Thread.CurrentThread.ManagedThreadId.ToString());
            //e.Result = e.Argument;//这里只是简单的把参数当做结果返回，当然您也可以在这里做复杂的处理后，再返回自己想要的结果(这里的操作是在另一个线程上完成的)
            
            JavaScriptObject result = new JavaScriptObject();
            result.Add("Result", false);
            result.Add("Message", "");
            try
            {
                string s = e.Argument.ToString();
                string ip = s.Split(',')[0];
                int port = int.Parse(s.Split(',')[1]);
                string str = DataTransfer.HelloWorld(ip, port);

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
                this.lbServerState.ForeColor = Color.Green;
                this.lbServerState.Text = "服务器连接成功！";
            }
            else
            {
                this.lbServerState.ForeColor = Color.Red;
                this.lbServerState.Text = result["Message"].ToString();
            }

            if(!isLogin)
                this.btnSaveServer.Enabled = true;

            this.btnServerTest.Enabled = true;
        } 
    }
}
