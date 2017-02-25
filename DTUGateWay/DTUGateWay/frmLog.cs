using Server.Util.Log;
using System;
using System.Threading;
using System.Windows.Forms;

namespace DTUGateWay
{
    public partial class frmLog : Form
    {
        public frmLog()
        {
            InitializeComponent();
            Thread t = new Thread(getlog);
            t.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SetCalClearResult();
        }

        private void frmLog_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true; 
        }

        private void frmLog_Load(object sender, EventArgs e)
        {
            
        }

        private void getlog()
        {
            while (ShowLogData.isShow)
            {
                string s = ShowLogData.get();
                if (s.Length > 0)
                {
                    SetCalResult(s);
                    Thread.Sleep(1);
                }
                else
                {
                    Thread.Sleep(100);
                }
            }
        }

        public delegate void SetTextHandler(string result);
        private void SetCalResult(string result)
        {
            if (textBox1.InvokeRequired == true)
            {
                SetTextHandler set = new SetTextHandler(SetCalResult);//委托的方法参数应和SetCalResult一致  
                textBox1.Invoke(set, new object[] { result }); //此方法第二参数用于传入方法,代替形参result  
            }
            else
            {
                string str = result + Environment.NewLine + this.textBox1.Text;
                if (str.Length > 8000)
                {
                    str = str.Substring(0, 8000);
                }
                this.textBox1.Text = str;
            }
        }

        public delegate void SetTextClearHandler();
        private void SetCalClearResult()
        {
            if (textBox1.InvokeRequired == true)
            {
                SetTextClearHandler set = new SetTextClearHandler(SetCalClearResult);//委托的方法参数应和SetCalResult一致  
                textBox1.Invoke(set, new object[] { }); //此方法第二参数用于传入方法,代替形参result  
            }
            else
            {
                this.textBox1.Clear();
            }
        }
    }
}
