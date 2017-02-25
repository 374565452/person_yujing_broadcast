using Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Utils;

namespace CardOperationSystem
{
    public partial class frmQuery : Form
    {
        int h = 0;

        public frmQuery()
        {
            InitializeComponent();
        }

        public frmQuery(string UserNo, string UserName, string IdentityNumber, string Telephone)
        {
            InitializeComponent();

            this.txtUserNo.Text = UserNo;
            this.txtUserName.Text = UserName;
            this.txtIdentityNumber.Text = IdentityNumber;
            this.txtTelephone.Text = Telephone;
        }

        private void frmQuery_Load(object sender, EventArgs e)
        {
            DateTime dateNow = DateTime.Now;
            this.dateTimePicker1.Value = DateTime.Parse(dateNow.Year + "-1-1");
            this.dateTimePicker2.Value = dateNow.Date;

            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView2.AutoGenerateColumns = false;

            h = this.Height;

            dataGridView1.Columns["dg1_充值时间"].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss ";
            dataGridView2.Columns["dg2_开泵时间"].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss ";
            dataGridView2.Columns["dg2_关泵时间"].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss ";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.groupBox2.Text = "充值记录";
            this.groupBox3.Text = "用水记录";
            this.dataGridView1.DataSource = null;
            this.dataGridView2.DataSource = null;

            string UserNo = this.txtUserNo.Text.Trim();
            string UserName = this.txtUserName.Text.Trim();
            string IdentityNumber = this.txtIdentityNumber.Text.Trim();
            string Telephone = this.txtTelephone.Text.Trim();

            string str1 = DataTransfer.GetCardUserRechargeLogList(UserNo, UserName, IdentityNumber, Telephone);
            JavaScriptObject result1 = (JavaScriptObject)JavaScriptConvert.DeserializeObject(str1);
            if (bool.Parse(result1["Result"].ToString()))
            {
                List<JsonCardUserRechargeLog> list = JavaScriptConvert.DeserializeObject<List<JsonCardUserRechargeLog>>(result1["Message"].ToString());
                if (list.Count > 0)
                {
                    ModelHandler<JsonCardUserRechargeLog> mh = new ModelHandler<JsonCardUserRechargeLog>();
                    DataTable dt = mh.FillDataTable(list);
                    this.dataGridView1.DataSource = dt.DefaultView;
                    this.dataGridView1.ClearSelection();
                }
                else
                {
                    this.dataGridView1.DataSource = null;
                    //MessageBox.Show("未查询到记录");
                }
                this.groupBox2.Text = "充值记录" + "  记录数量：" + list.Count;
            }
            else
            {
                this.groupBox2.Text = "充值记录" + "  查询出错";
                MessageBox.Show("充值记录查询出错！" + result1["Message"].ToString());
            }

            string str2 = DataTransfer.GetCardUserWaterLogList(UserNo, UserName, IdentityNumber, Telephone);
            JavaScriptObject result2 = (JavaScriptObject)JavaScriptConvert.DeserializeObject(str2);
            if (bool.Parse(result2["Result"].ToString()))
            {
                List<JsonCardUserWaterLog> list = JavaScriptConvert.DeserializeObject<List<JsonCardUserWaterLog>>(result2["Message"].ToString());
                if (list.Count > 0)
                {
                    ModelHandler<JsonCardUserWaterLog> mh = new ModelHandler<JsonCardUserWaterLog>();
                    DataTable dt = mh.FillDataTable(list);
                    this.dataGridView2.DataSource = dt.DefaultView;
                    this.dataGridView2.ClearSelection();
                }
                else
                {
                    this.dataGridView1.DataSource = null;
                    //MessageBox.Show("未查询到记录");
                }
                this.groupBox3.Text = "用水记录" + "  记录数量：" + list.Count;
            }
            else
            {
                this.groupBox3.Text = "用水记录" + "  查询出错";
                MessageBox.Show("用水记录查询出错！" + result2["Message"].ToString());
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.groupBox2.Text = "充值记录";
            this.groupBox3.Text = "用水记录";
            this.dataGridView1.DataSource = null;
            this.dataGridView2.DataSource = null;

            string UserNo = this.txtUserNo.Text.Trim();
            string UserName = this.txtUserName.Text.Trim();
            string IdentityNumber = this.txtIdentityNumber.Text.Trim();
            string Telephone = this.txtTelephone.Text.Trim();

            string str1 = DataTransfer.GetCardUserRechargeLogList("", UserName, IdentityNumber, Telephone);
            JavaScriptObject result1 = (JavaScriptObject)JavaScriptConvert.DeserializeObject(str1);
            if (bool.Parse(result1["Result"].ToString()))
            {
                List<JsonCardUserRechargeLog> list = JavaScriptConvert.DeserializeObject<List<JsonCardUserRechargeLog>>(result1["Message"].ToString());
                if (list.Count > 0)
                {
                    ModelHandler<JsonCardUserRechargeLog> mh = new ModelHandler<JsonCardUserRechargeLog>();
                    DataTable dt = mh.FillDataTable(list);
                    this.dataGridView1.DataSource = dt.DefaultView;
                    this.dataGridView1.ClearSelection();
                }
                else
                {
                    this.dataGridView1.DataSource = null;
                    //MessageBox.Show("未查询到记录");
                }
                this.groupBox2.Text = "充值记录" + "  记录数量：" + list.Count;
            }
            else
            {
                this.groupBox2.Text = "充值记录" + "  查询出错";
                MessageBox.Show("充值记录查询出错！" + result1["Message"].ToString());
            }

            string str2 = DataTransfer.GetCardUserWaterLogList("", UserName, IdentityNumber, Telephone);
            JavaScriptObject result2 = (JavaScriptObject)JavaScriptConvert.DeserializeObject(str2);
            if (bool.Parse(result2["Result"].ToString()))
            {
                List<JsonCardUserWaterLog> list = JavaScriptConvert.DeserializeObject<List<JsonCardUserWaterLog>>(result2["Message"].ToString());
                if (list.Count > 0)
                {
                    ModelHandler<JsonCardUserWaterLog> mh = new ModelHandler<JsonCardUserWaterLog>();
                    DataTable dt = mh.FillDataTable(list);
                    this.dataGridView2.DataSource = dt.DefaultView;
                    this.dataGridView2.ClearSelection();
                }
                else
                {
                    this.dataGridView1.DataSource = null;
                    //MessageBox.Show("未查询到记录");
                }
                this.groupBox3.Text = "用水记录" + "  记录数量：" + list.Count;
            }
            else
            {
                this.groupBox3.Text = "用水记录" + "  查询出错";
                MessageBox.Show("用水记录查询出错！" + result2["Message"].ToString());
            }
        }

        private void frmQuery_Resize(object sender, EventArgs e)
        {
            //MessageBox.Show("frmQuery_Resize");
            int diffHeight = this.Height - h;
            h = this.Height;

            //MessageBox.Show(diffHeight + " | " + this.groupBox2.Height + " | " + this.groupBox3.Height);

            this.groupBox2.Height = this.groupBox2.Height + diffHeight / 2;
            this.groupBox3.Height = this.groupBox3.Height + diffHeight / 2;
            this.groupBox3.Top = this.groupBox3.Top - diffHeight / 2;

            //MessageBox.Show(diffHeight + " | " + this.groupBox2.Height + " | " + this.groupBox3.Height);
        }

        private void frmQuery_SizeChanged(object sender, EventArgs e)
        {
            //MessageBox.Show("frmQuery_SizeChanged");
        }
    }
}
