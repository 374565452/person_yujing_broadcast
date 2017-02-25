using Common;
using Maticsoft.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace CardOperationSystem
{
    public partial class frmRecharge : Form
    {
        private int icdev;
        frmMain pf;
        frmCardUser pfu;
        public frmRecharge(frmMain pf, frmCardUser pfu)
        {
            InitializeComponent();
            this.icdev = pf.icdev;
            this.pf = pf;
            this.pfu = pfu;
        }

        private void frmRecharge_Load(object sender, EventArgs e)
        {
            this.button1.Enabled = false;
            this.lbState.Text = "";

            getNew();

            getInfo();

            if (InfoSys.LoginUserName == "admin")
            {
                checkBox1.Visible = true;
            }
        }

        private void getInfo()
        {
            string str = DataTransfer.GetWaterUserInfo("", this.txtIdentityNumber.Text.Trim(), "");
            JavaScriptObject result = (JavaScriptObject)JavaScriptConvert.DeserializeObject(str);
            if (bool.Parse(result["Result"].ToString()))
            {
                List<WaterUser> list = JavaScriptConvert.DeserializeObject<List<WaterUser>>(result["Message"].ToString());
                if (list.Count == 1)
                {
                    this.txtWaterQuota.Text = list[0].WaterQuota.ToString();
                    this.txtElectricQuota.Text = list[0].ElectricQuota.ToString();

                    string str1 = DataTransfer.GetCardUserRechargeInfo(list[0].id);
                    JavaScriptObject result1 = (JavaScriptObject)JavaScriptConvert.DeserializeObject(str1);
                    if (bool.Parse(result1["Result"].ToString()))
                    {
                        string[] ss = result1["Message"].ToString().Split('|');
                        if (ss.Length == 6)
                        {
                            this.button1.Enabled = true;

                            this.txtWaterUsed.Text = ss[0];
                            this.txtElectricUsed.Text = ss[1];
                            this.txtWaterLevel.Text = ss[2];
                            this.txtElectricLevel.Text = ss[3];
                            this.txtUnitPriceWater.Text = decimal.Parse(ss[4]).ToString("0.00");
                            this.txtUnitPriceElectric.Text = decimal.Parse(ss[5]).ToString("0.00");
                        }
                        else
                        {
                            MessageBox.Show("无法充值！" + "未查询出用户年充值信息");
                        }
                    }
                    else
                    {
                        MessageBox.Show("无法充值！" + "未查询出用户年充值信息");
                    }
                }
                else
                {
                    MessageBox.Show("无法充值！" + "未查询出用户，请检查身份证号");
                }
            }
            else
            {
                MessageBox.Show("无法充值！" + result["Message"].ToString());
            }
        }

        string DeviceList_old = "";
        private void getNew()
        {
            this.lbCardType.Text = pfu.getCardType();
            this.lbSerialNumber.Text = pfu.getSerialNumber();
            this.txtUserNo.Text = pfu.getUserNo();
            this.txtUserName.Text = pfu.getUserName();
            this.txtIdentityNumber.Text = pfu.getIdentityNumber();
            this.txtTelephone.Text = pfu.getTelephone();
            this.txtResidualWater.Text = pfu.getResidualWater();
            this.txtResidualElectric.Text = pfu.getResidualElectric();
            this.txtTotalWater.Text = pfu.getTotalWater();
            this.txtTotalElectric.Text = pfu.getTotalElectric();
            DeviceList_old = pfu.getDeviceList_old();


        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.lbState.Text = "";

            if (checkBox1.Checked)
            {
                if (MessageBox.Show("确定减少水电量？",
                    "减少确认", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                {
                    return;
                }
            }
            else
            {
                if (MessageBox.Show("确定充值水电量？",
                    "充值确认", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                {
                    return;
                }
            }

            pfu.getNew();

            getNew();

            if (this.lbCardType.Text.Trim() != InfoSys.CardTypeUser)
            {
                MessageBox.Show("非用户卡不可充值");
                return;
            }

            if (!CheckValue())
            {
                return;
            }

            double d_UnitPriceWater = Tools.StringToDouble(UnitPriceWater, 0);
            double d_UnitPriceElectric = Tools.StringToDouble(UnitPriceElectric, 0);
            double d_NumberWater = Tools.StringToDouble(WaterNum, 0);
            double d_NumberElectric = Tools.StringToDouble(ElectricNum, 0);
            double d_WaterPrice = Tools.StringToDouble(WaterPrice, 0);
            double d_ElectricPrice = Tools.StringToDouble(ElectricPrice, 0);     

            if (checkBox1.Checked)
            {
                d_NumberWater = -d_NumberWater;
                d_NumberElectric = -d_NumberElectric;
                d_WaterPrice = -d_WaterPrice;
                d_ElectricPrice = -d_ElectricPrice;
            }

            double d_ResidualWater = Tools.StringToDouble(this.txtResidualWater.Text, 0);
            double d_ResidualElectric = Tools.StringToDouble(this.txtResidualElectric.Text, 0);

            int i_ResidualWater_new = (int)((d_ResidualWater + d_NumberWater) * 10);
            if (i_ResidualWater_new < 0) i_ResidualWater_new = 0;
            if (i_ResidualWater_new > 99999999) i_ResidualWater_new = 99999999;
            int i_ResidualElectric_new = (int)((d_ResidualElectric + d_NumberElectric) * 10);
            if (i_ResidualElectric_new < 0) i_ResidualElectric_new = 0;
            if (i_ResidualElectric_new > 99999999) i_ResidualElectric_new = 99999999;


            //MessageBox.Show("可以充值");
            //保存远程服务器数据库
            string str = DataTransfer.RechargeCardUser(this.lbSerialNumber.Text.Trim(), this.txtUserNo.Text.Trim(),
                this.txtUserName.Text.Trim(),this.txtIdentityNumber.Text.Trim(),this.txtTelephone.Text.Trim(),
                d_WaterPrice.ToString(), d_NumberWater.ToString(), d_ElectricPrice.ToString(), d_NumberElectric.ToString(),
                this.txtWaterUsed.Text, this.txtElectricUsed.Text, this.txtRemark.Text.Trim());
            JavaScriptObject result = (JavaScriptObject)JavaScriptConvert.DeserializeObject(str);
            if (!bool.Parse(result["Result"].ToString()))
            {
                string txt = result["Message"].ToString();
                MessageBox.Show(txt);
                pf.Log(txt);
                this.lbState.Text = txt;
                pf.Log(pf.getDateStr() + this.lbState.Text);
                return;
            }

            int mode = 4; //以B密码认证
            int sec = 1; //扇区
            int block = 0;
            string key = pf.getKeyB(); //读卡密码
            string result_WriteIC = "";
            string result_AuthIC = "";

            //设置扇区2内容
            sec = 2;
            //认证卡密码B
            result_AuthIC = CardCommon.AuthIC(icdev, mode, sec, key);
            pf.AuthLog(InfoSys.CardTypeStrUser, InfoSys.MethodModifyCard, sec, result_AuthIC);
            if (result_AuthIC == InfoSys.StrAuthSuccess)
            {

                //写数据块0，剩余可用水量（4字节）剩余可用电量（4字节）累计用水量（4字节）累计用电量（4字节）
                block = 0;
                double d3 = Tools.StringToDouble(this.txtTotalWater.Text, 0);
                double d4 = Tools.StringToDouble(this.txtTotalElectric.Text, 0);
                result_WriteIC = CardCommon.WriteIC(icdev, sec, block,
                    i_ResidualWater_new.ToString().PadLeft(8, '0') +
                    i_ResidualElectric_new.ToString().PadLeft(8, '0') +
                    d3.ToString().PadLeft(8, '0') +
                    d4.ToString().PadLeft(8, '0'));
                if (result_WriteIC != "")
                {
                    this.lbState.Text = "写卡出错，充值失败！";
                    pf.Log(pf.getDateStr() + this.lbState.Text);
                    return;
                }

            }
            else
            {
                this.lbState.Text = "认证出错，充值失败！";
                pf.Log(pf.getDateStr() + this.lbState.Text);
                return;
            }

            this.lbState.Text = "充值成功！";
            getInfo();
            this.txtNumberWater.Text = "0";
            this.txtTotalPriceWater.Text = "0";
            this.txtNumberElectric.Text = "0"; 
            this.txtTotalPriceElectric.Text = "0";
            pf.Log(pf.getDateStr() + this.lbState.Text + "充值后剩余水量：" + i_ResidualWater_new + "，充值后剩余电量：" + i_ResidualElectric_new);
            return;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        string UnitPriceWater = "";
        string WaterNum = "";
        string WaterPrice = "";

        string UnitPriceElectric = "";
        string ElectricNum = "";
        string ElectricPrice = "";

        private bool CheckValue()
        {
            string regex1 = @"^(\d{1,6})(\.\d{1,2})?$";
            string regex2 = @"^(\d{1,7})(\.\d{1})?$";


            UnitPriceWater = this.txtUnitPriceWater.Text.Trim();
            if (!Regex.IsMatch(UnitPriceWater, regex1))
            {
                MessageBox.Show("充值水价格式不正确，0-999999.99");
                return false;
            }

            WaterNum = this.txtNumberWater.Text.Trim();
            if (!Regex.IsMatch(WaterNum, regex2))
            {
                MessageBox.Show("充值水量格式不正确，0-9999999.9");
                return false;
            }
            /*
            if (WaterNum == "0")
            {
                MessageBox.Show("充值水量必须大于0，0-9999999.9");
                return false;
            }
            */
            WaterPrice = this.txtTotalPriceWater.Text.Trim();
            if (!Regex.IsMatch(WaterPrice, regex1))
            {
                MessageBox.Show("充值水金额格式不正确，0-999999.99");
                return false;
            }

            UnitPriceElectric = this.txtUnitPriceElectric.Text.Trim();
            if (!Regex.IsMatch(UnitPriceElectric, regex1))
            {
                MessageBox.Show("充值电价格式不正确，0-999999.99");
                return false;
            }

            ElectricNum = this.txtNumberElectric.Text.Trim();
            if (!Regex.IsMatch(ElectricNum, regex1))
            {
                MessageBox.Show("充值电量格式不正确，0-9999999.9");
                return false;
            }
            /*
            if (ElectricNum == "0")
            {
                MessageBox.Show("充值电量必须大于0，0-9999999.9");
                return false;
            }
            */
            ElectricPrice = this.txtTotalPriceElectric.Text.Trim();
            if (!Regex.IsMatch(ElectricPrice, regex1))
            {
                MessageBox.Show("充值电金额格式不正确，0-999999.99");
                return false;
            }

            if (WaterNum == "0" && ElectricNum == "0")
            {
                MessageBox.Show("充值水电量必须有一个大于0，0-9999999.9");
                return false;
            }
            return true;
        }

        private void txtNumberWater_TextChanged(object sender, EventArgs e)
        {
            try
            {
                decimal d = decimal.Parse(this.txtNumberWater.Text);
                decimal span = decimal.Parse(this.txtWaterLevel.Text);
                if (d > span && span > 0)
                {
                    d = 0;
                    this.txtNumberWater.Text = "0";
                    MessageBox.Show("最大充值水量为：" + (span));
                }
                this.txtTotalPriceWater.Text = (d * decimal.Parse(this.txtUnitPriceWater.Text)).ToString("0.00");
            }
            catch { this.txtNumberWater.Text = "0"; this.txtTotalPriceWater.Text = "0"; }
        }

        private void txtNumberElectric_TextChanged(object sender, EventArgs e)
        {
            try
            {
                decimal d = decimal.Parse(this.txtNumberElectric.Text);
                decimal span = decimal.Parse(this.txtElectricLevel.Text);
                if (d > span && span > 0)
                {
                    d = 0;
                    this.txtNumberElectric.Text = "0";
                    MessageBox.Show("最大充值电量为：" + (span));
                }
                this.txtTotalPriceElectric.Text = (d * decimal.Parse(this.txtUnitPriceElectric.Text)).ToString("0.00");
            }
            catch { this.txtNumberElectric.Text = "0"; this.txtTotalPriceElectric.Text = "0"; }
        }
    }
}
