using System;
using System.Windows.Forms;

namespace DTUGateWay
{
    public partial class frmReg : Form
    {
        public static bool isClose = false;
        public static bool isSuccess = false;
        public frmReg()
        {
            InitializeComponent();

            isClose = false;
            isSuccess = false;

            string regStr = "";
            if (FileHelper.IsExists(SysInfo.fileName))
            {
                regStr = FileHelper.ReadFile(SysInfo.fileName);
            }
            else
            {
                regStr = "00000000000000000000000000000000";
                FileHelper.writeFile(SysInfo.fileName, regStr);
            }

            this.textBox1.Text = SysInfo.GetRegStr1();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.textBox3.Text.Trim() != SysInfo.GetRegStr2())
            {
                MessageBox.Show("请输入正确的注册码！");
            }
            else
            {
                FileHelper.writeFile(SysInfo.fileName, this.textBox3.Text.Trim());
                isSuccess = true;
                this.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!isSuccess)
                isClose = true;
            this.Close();
        }

        private void frmReg_Load(object sender, EventArgs e)
        {
            this.label4.Text = SysInfo.RegStr;
        }

        private void frmReg_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(!isSuccess)
                isClose = true;
        }
    }
}
