using Module;
using Server.Core.ServerCore;
using System;
using System.Data;
using System.Drawing;
using System.Net;
using System.Windows.Forms;

namespace DTUGateWay
{
    public partial class frmCache : Form
    {
        ServerSocket dtuServer;
        public frmCache(ServerSocket dtuServer)
        {
            InitializeComponent();

            this.dtuServer = dtuServer;
        }

        private void frmCache_Load(object sender, EventArgs e)
        {
            button1_Click(null, null);
            button2_Click(null, null);
            button3_Click(null, null);
            button4_Click(null, null);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.label7.Text = "";
            try
            {
                CardUserModule.LoadCardUsers();
                this.label4.Text = CardUserModule.GetAllCardUser(true).Count.ToString();
                this.label7.ForeColor = Color.Green;
                this.label7.Text = "更新成功";
            }
            catch
            {
                this.label7.ForeColor = Color.Red;
                this.label7.Text = "更新失败";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.label8.Text = "";
            try
            {
                DistrictModule.UpdateLevelInfo();
                DistrictModule.UpdateDistrictInfo();
                this.label5.Text = DistrictModule.GetAllDistrict().Count.ToString();
                this.label8.ForeColor = Color.Green;
                this.label8.Text = "更新成功";
            }
            catch
            {
                this.label8.ForeColor = Color.Red;
                this.label8.Text = "更新失败";
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.label9.Text = "";
            try
            {
                DeviceModule.LoadDevices();
                this.label6.Text = DeviceModule.GetAllDevice().Count.ToString();
                this.label9.ForeColor = Color.Green;
                this.label9.Text = "更新成功";
            }
            catch
            {
                this.label9.ForeColor = Color.Red;
                this.label9.Text = "更新失败";
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.label12.Text = "";
            try
            {
                int count = dtuServer.AsyncSocketUserTokenList.count();
                DataTable dt = new DataTable();
                dt.Columns.Add("IP");
                dt.Columns.Add("Port");
                dt.Columns.Add("deviceNo");
                for (int i = 0; i < count; i++)
                {
                    try
                    {
                        AsyncSocketUserToken token = dtuServer.AsyncSocketUserTokenList.UserTokenList[i];
                        DataRow dr = dt.NewRow();
                        dr[0] = ((IPEndPoint)token.ConnectedSocket.RemoteEndPoint).Address.ToString();
                        dr[1] = ((IPEndPoint)token.ConnectedSocket.RemoteEndPoint).Port;
                        dr[2] = token.DeviceInfo != null ? token.DeviceInfo.DeviceNo : "无设备";
                        dt.Rows.Add(dr);
                    }
                    catch { }
                }
                this.dataGridView1.DataSource = dt;
                this.label11.Text = dt.Rows.Count.ToString();
                this.label12.ForeColor = Color.Green;
                this.label12.Text = "更新成功";
            }
            catch
            {
                this.label12.ForeColor = Color.Red;
                this.label12.Text = "更新失败";
            }
        }
    }
}
