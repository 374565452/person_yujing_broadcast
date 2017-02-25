using Common;
using Maticsoft.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace CardOperationSystem
{
    public partial class frmCardUser : Form
    {
        private int icdev;
        frmMain pf;
        public frmCardUser(frmMain pf)
        {
            InitializeComponent();
            this.icdev = pf.icdev;
            this.pf = pf;
        }

        private void frmCardUser_Load(object sender, EventArgs e)
        {
            this.groupBox1.Hide();

            this.listBox1.Items.Clear();
            if (InfoSys.ListDevices != null && InfoSys.ListDevices.Count > 0)
            {
                foreach (Device device in InfoSys.ListDevices)
                {
                    //this.listBox1.Items.Add(device.AddressCode1 + "-" + device.AddressCode2 + "-" + Tools.RepeatStr("0", 3 - device.AddressCode3.ToString().Length) + device.AddressCode3 + "-" + device.Description);
                    District d5 = InfoSys.GetDistrictById(device.DistrictId);
                    District d4 = InfoSys.GetDistrictById(d5.ParentId);
                    District d3 = InfoSys.GetDistrictById(d4.ParentId);
                    District d2 = InfoSys.GetDistrictById(d3.ParentId);
                    District d1 = InfoSys.GetDistrictById(d2.ParentId);
                    this.listBox1.Items.Add(d1.DistrictCode + d2.DistrictCode + d3.DistrictCode + "-" +
                        d4.DistrictCode + d5.DistrictCode + "-" +
                        device.DeviceNo.PadLeft(3, '0') + "-" + device.DeviceName);
                }
            }

            clear();
        }

        private void clear()
        {
            SerialNumber_old = "";
            this.lbCardType.Text = InfoSys.StrUnknown;
            this.lbSerialNumber.Text = InfoSys.StrUnknown;
            this.lbState.Text = InfoSys.StrState;
            this.txtUserNo.Text = "";
            this.txtUserName.Text = "";
            this.txtIdentityNumber.Text = "";
            this.txtTelephone.Text = "";
            this.txtResidualWater.Text = "";
            this.txtResidualElectric.Text = "";
            this.txtTotalWater.Text = "";
            this.txtTotalElectric.Text = "";

            this.listBox1.SelectedIndex = -1;
            this.listBox2.Items.Clear();
        }

        public string getCardType()
        {
            return this.lbCardType.Text.Trim();
        }

        public string getSerialNumber()
        {
            return this.lbSerialNumber.Text.Trim();
        }

        public string getUserNo()
        {
            return this.txtUserNo.Text.Trim();
        }

        public string getUserName()
        {
            return this.txtUserName.Text.Trim();
        }

        public string getIdentityNumber()
        {
            return this.txtIdentityNumber.Text.Trim();
        }

        public string getTelephone()
        {
            return this.txtTelephone.Text.Trim();
        }

        public string getResidualWater()
        {
            return this.txtResidualWater.Text.Trim();
        }

        public string getResidualElectric()
        {
            return this.txtResidualElectric.Text.Trim();
        }

        public string getTotalWater()
        {
            return this.txtTotalWater.Text.Trim();
        }

        public string getTotalElectric()
        {
            return this.txtTotalElectric.Text.Trim();
        }

        public string getDeviceList_old()
        {
            return DeviceList_old;
        }

        public void getNew()
        {
            button1_Click(null, null);
        }

        string SerialNumber_old = "";
        string UserNo_old = "";
        string UserName_old = "";
        string IdentityNumber_old = "";
        string Telephone_old = "";
        string DeviceList_old = "";
        string ResidualWater_old = "";
        string ResidualElectric_old = "";
        string TotalWater_old = "";
        string TotalElectric_old = "";
        private void button1_Click(object sender, EventArgs e)
        {
            clear();

            pf.BeginLog(InfoSys.CardTypeStrUser, InfoSys.MethodReadCard);

            if (!pf.SeedIC())
            {
                this.lbCardType.Text = InfoSys.StrSeekFailure;
                pf.EndLog(InfoSys.CardTypeStrUser, InfoSys.MethodReadCard);
                return;
            }

            this.lbSerialNumber.Text = pf.getSnr().ToString("X");

            int mode = 4; //以B密码认证
            int sec = 1; //扇区
            int block = 0;
            string key = pf.getKeyB(); //读卡密码
            string result_ReadIC = "";
            string result_AuthIC = "";

            //读取扇区1内容
            sec = 1;
            //认证卡密码B
            result_AuthIC = CardCommon.AuthIC(icdev, mode, sec, key);
            pf.AuthLog(InfoSys.CardTypeStrUser, InfoSys.MethodReadCard, sec, result_AuthIC);
            if (result_AuthIC == InfoSys.StrAuthSuccess)
            {
                //读取数据块0，数据块0为卡类型（1字节）、用户卡号（4字节）
                block = 0;
                result_ReadIC = CardCommon.ReadIC(icdev, sec, block);
                if (result_ReadIC.Length == 32)
                {
                    this.lbCardType.Text = result_ReadIC.Substring(0, 2);
                    if (this.lbCardType.Text != InfoSys.CardTypeUser)
                    {
                        this.lbState.Text = "非" + InfoSys.CardTypeStrUser + "，" + InfoSys.StrCannotRead;
                        pf.Log(this.lbState.Text);
                        pf.EndLog(InfoSys.CardTypeStrUser, InfoSys.MethodReadCard);
                        return;
                    }
                    this.txtUserNo.Text = result_ReadIC.Substring(2, 8).TrimStart('0');
                }
                else
                {
                    this.lbState.Text = InfoSys.InfoReadFailure(sec, block, InfoSys.CardTypeStrUser, InfoSys.StrReadFailure);
                    pf.Log(this.lbState.Text);
                    pf.EndLog(InfoSys.CardTypeStrUser, InfoSys.MethodReadCard);
                    return;
                }

                //读取数据块1，数据块1为用户名（16字节）
                block = 1;
                result_ReadIC = CardCommon.ReadIC(icdev, sec, block);
                if (result_ReadIC.Length == 32)
                {
                    this.txtUserName.Text = HexStringUtility.ByteArrayToStr(HexStringUtility.HexStringToByteArray(result_ReadIC)).Trim();
                }
                else
                {
                    this.lbState.Text = InfoSys.InfoReadFailure(sec, block, InfoSys.CardTypeStrUser, InfoSys.StrReadFailure);
                    pf.Log(this.lbState.Text);
                    pf.EndLog(InfoSys.CardTypeStrUser, InfoSys.MethodReadCard);
                    return;
                }

                //读取数据块2，数据块2为身份证号（9字节）、联系电话（6字节）
                block = 2;
                result_ReadIC = CardCommon.ReadIC(icdev, sec, block);
                if (result_ReadIC.Length == 32)
                {
                    this.txtIdentityNumber.Text = result_ReadIC.Substring(0, 18).ToUpper().Replace("A", "X");
                    this.txtTelephone.Text = result_ReadIC.Substring(18, 12).TrimStart('0');
                }
                else
                {
                    this.lbState.Text = InfoSys.InfoReadFailure(sec, block, InfoSys.CardTypeStrUser, InfoSys.StrReadFailure);
                    pf.Log(this.lbState.Text);
                    pf.EndLog(InfoSys.CardTypeStrUser, InfoSys.MethodReadCard);
                    return;
                }
            }

            //读取扇区2内容
            sec = 2;
            //认证卡密码B
            result_AuthIC = CardCommon.AuthIC(icdev, mode, sec, key);
            pf.AuthLog(InfoSys.CardTypeStrUser, InfoSys.MethodReadCard, sec, result_AuthIC);
            if (result_AuthIC == InfoSys.StrAuthSuccess)
            {
                //读取数据块0，剩余可用水量（4字节）剩余可用电量（4字节）累计用水量（4字节）累计用电量（4字节）
                block = 0;
                result_ReadIC = CardCommon.ReadIC(icdev, sec, block);
                if (result_ReadIC.Length == 32)
                {
                    ToolsForm.setTextTextBox(this.txtResidualWater, result_ReadIC.Substring(0, 8), "0", 1);
                    ToolsForm.setTextTextBox(this.txtResidualElectric, result_ReadIC.Substring(8, 8), "0", 1);
                    ToolsForm.setTextTextBox(this.txtTotalWater, result_ReadIC.Substring(16, 8), "0", 1);
                    ToolsForm.setTextTextBox(this.txtTotalElectric, result_ReadIC.Substring(24, 8), "0", 1);
                }
                else
                {
                    this.lbState.Text = InfoSys.InfoReadFailure(sec, block, InfoSys.CardTypeStrUser, InfoSys.StrReadFailure);
                    pf.Log(this.lbState.Text);
                    pf.EndLog(InfoSys.CardTypeStrUser, InfoSys.MethodReadCard);
                    return;
                }

                //读取数据块1，地址码1（7字节）地址码2（7字节）
                block = 1;
                result_ReadIC = CardCommon.ReadIC(icdev, sec, block);
                if (result_ReadIC.Length == 32)
                {
                    string device1 = result_ReadIC.Substring(0, 14);
                    if (device1 != "".PadRight(14, '0'))
                    {
                        int i = Convert.ToInt32("0x" + device1.Substring(12, 2), 16);
                        string data = device1.Substring(0, 6) + "-" + device1.Substring(6, 6) + "-" + i.ToString().PadLeft(3, '0');
                        this.listBox2.Items.Add(data);
                    }
                    string device2 = result_ReadIC.Substring(14, 14);
                    if (device2 != "".PadRight(14, '0'))
                    {
                        int i = Convert.ToInt32("0x" + device2.Substring(12, 2), 16);
                        string data = device2.Substring(0, 6) + "-" + device2.Substring(6, 6) + "-" + i.ToString().PadLeft(3, '0');
                        this.listBox2.Items.Add(data);
                    }
                }
                else
                {
                    this.lbState.Text = InfoSys.InfoReadFailure(sec, block, InfoSys.CardTypeStrUser, InfoSys.StrReadFailure);
                    pf.Log(this.lbState.Text);
                    pf.EndLog(InfoSys.CardTypeStrUser, InfoSys.MethodReadCard);
                    return;
                }

                //读取数据块2，地址码1（7字节）地址码2（7字节）
                block = 2;
                result_ReadIC = CardCommon.ReadIC(icdev, sec, block);
                if (result_ReadIC.Length == 32)
                {
                    string device1 = result_ReadIC.Substring(0, 14);
                    if (device1 != "".PadRight(14, '0'))
                    {
                        int i = Convert.ToInt32("0x" + device1.Substring(12, 2), 16);
                        string data = device1.Substring(0, 6) + "-" + device1.Substring(6, 6) + "-" + i.ToString().PadLeft(3, '0');
                        this.listBox2.Items.Add(data);
                    }
                    string device2 = result_ReadIC.Substring(14, 14);
                    if (device2 != "".PadRight(14, '0'))
                    {
                        int i = Convert.ToInt32("0x" + device2.Substring(12, 2), 16);
                        string data = device2.Substring(0, 6) + "-" + device2.Substring(6, 6) + "-" + i.ToString().PadLeft(3, '0');
                        this.listBox2.Items.Add(data);
                    }
                }
                else
                {
                    this.lbState.Text = InfoSys.InfoReadFailure(sec, block, InfoSys.CardTypeStrUser, InfoSys.StrReadFailure);
                    pf.Log(this.lbState.Text);
                    pf.EndLog(InfoSys.CardTypeStrUser, InfoSys.MethodReadCard);
                    return;
                }
            }

            //读取扇区3内容
            sec = 3;
            //认证卡密码B
            result_AuthIC = CardCommon.AuthIC(icdev, mode, sec, key);
            pf.AuthLog(InfoSys.CardTypeStrUser, InfoSys.MethodReadCard, sec, result_AuthIC);
            if (result_AuthIC == InfoSys.StrAuthSuccess)
            {
                //读取数据块0，地址码5（7字节）地址码6（7字节）
                block = 0;
                result_ReadIC = CardCommon.ReadIC(icdev, sec, block);
                if (result_ReadIC.Length == 32)
                {
                    string device1 = result_ReadIC.Substring(0, 14);
                    if (device1 != "".PadRight(14, '0'))
                    {
                        int i = Convert.ToInt32("0x" + device1.Substring(12, 2), 16);
                        string data = device1.Substring(0, 6) + "-" + device1.Substring(6, 6) + "-" + i.ToString().PadLeft(3, '0');
                        this.listBox2.Items.Add(data);
                    }
                    string device2 = result_ReadIC.Substring(14, 14);
                    if (device2 != "".PadRight(14, '0'))
                    {
                        int i = Convert.ToInt32("0x" + device2.Substring(12, 2), 16);
                        string data = device2.Substring(0, 6) + "-" + device2.Substring(6, 6) + "-" + i.ToString().PadLeft(3, '0');
                        this.listBox2.Items.Add(data);
                    }
                }
                else
                {
                    this.lbState.Text = InfoSys.InfoReadFailure(sec, block, InfoSys.CardTypeStrUser, InfoSys.StrReadFailure);
                    pf.Log(this.lbState.Text);
                    pf.EndLog(InfoSys.CardTypeStrUser, InfoSys.MethodReadCard);
                    return;
                }

                //读取数据块1，地址码7（7字节）地址码8（7字节）
                block = 1;
                result_ReadIC = CardCommon.ReadIC(icdev, sec, block);
                if (result_ReadIC.Length == 32)
                {
                    string device1 = result_ReadIC.Substring(0, 14);
                    if (device1 != "".PadRight(14, '0'))
                    {
                        int i = Convert.ToInt32("0x" + device1.Substring(12, 2), 16);
                        string data = device1.Substring(0, 6) + "-" + device1.Substring(6, 6) + "-" + i.ToString().PadLeft(3, '0');
                        this.listBox2.Items.Add(data);
                    }
                    string device2 = result_ReadIC.Substring(14, 14);
                    if (device2 != "".PadRight(14, '0'))
                    {
                        int i = Convert.ToInt32("0x" + device2.Substring(12, 2), 16);
                        string data = device2.Substring(0, 6) + "-" + device2.Substring(6, 6) + "-" + i.ToString().PadLeft(3, '0');
                        this.listBox2.Items.Add(data);
                    }
                }
                else
                {
                    this.lbState.Text = InfoSys.InfoReadFailure(sec, block, InfoSys.CardTypeStrUser, InfoSys.StrReadFailure);
                    pf.Log(this.lbState.Text);
                    pf.EndLog(InfoSys.CardTypeStrUser, InfoSys.MethodReadCard);
                    return;
                }

                //读取数据块2，地址码9（7字节）地址码10（7字节）
                block = 2;
                result_ReadIC = CardCommon.ReadIC(icdev, sec, block);
                if (result_ReadIC.Length == 32)
                {
                    string device1 = result_ReadIC.Substring(0, 14);
                    if (device1 != "".PadRight(14, '0'))
                    {
                        int i = Convert.ToInt32("0x" + device1.Substring(12, 2), 16);
                        string data = device1.Substring(0, 6) + "-" + device1.Substring(6, 6) + "-" + i.ToString().PadLeft(3, '0');
                        this.listBox2.Items.Add(data);
                    }
                    string device2 = result_ReadIC.Substring(14, 14);
                    if (device2 != "".PadRight(14, '0'))
                    {
                        int i = Convert.ToInt32("0x" + device2.Substring(12, 2), 16);
                        string data = device2.Substring(0, 6) + "-" + device2.Substring(6, 6) + "-" + i.ToString().PadLeft(3, '0');
                        this.listBox2.Items.Add(data);
                    }
                }
                else
                {
                    this.lbState.Text = InfoSys.InfoReadFailure(sec, block, InfoSys.CardTypeStrUser, InfoSys.StrReadFailure);
                    pf.Log(this.lbState.Text);
                    pf.EndLog(InfoSys.CardTypeStrUser, InfoSys.MethodReadCard);
                    return;
                }
            }

            if (this.lbCardType.Text == InfoSys.CardTypeUser)
            {
                SerialNumber_old = this.lbSerialNumber.Text.Trim();
                UserNo_old = this.txtUserNo.Text.Trim();
                UserName_old = this.txtUserName.Text.Trim();
                IdentityNumber_old = this.txtIdentityNumber.Text.Trim();
                Telephone_old = this.txtTelephone.Text.Trim();
                DeviceList_old = "";
                for (int i = 0; i < listBox2.Items.Count; i++)
                {
                    string[] ss = listBox2.Items[i].ToString().Split('-');
                    try
                    {
                        string hex = int.Parse(ss[2]).ToString("X").PadLeft(2, '0');
                        DeviceList_old += ss[0] + ss[1] + hex + ",";
                    }
                    catch { }
                }
                ResidualWater_old = this.txtResidualWater.Text.Trim();
                ResidualElectric_old = this.txtResidualElectric.Text.Trim();
                TotalWater_old = this.txtTotalWater.Text.Trim();
                TotalElectric_old = this.txtTotalElectric.Text.Trim();
                this.lbState.Text = InfoSys.StrReadSuccess;
            }
            else
            {
                this.lbState.Text = "非" + InfoSys.CardTypeStrUser + "，" + InfoSys.StrCannotRead;
                MessageBox.Show(this.lbState.Text);
            }

            pf.EndLog(InfoSys.CardTypeStrUser, InfoSys.MethodReadCard);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定" + InfoSys.CardTypeStrUser + InfoSys.MethodOpenCard + "？",
                InfoSys.CardTypeStrUser + InfoSys.MethodOpenCard, MessageBoxButtons.YesNo,
                MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
            {
                return;
            }

            pf.BeginLog(InfoSys.CardTypeStrUser, InfoSys.MethodOpenCard);

            if (!CheckValue())
            {
                pf.EndLog(InfoSys.CardTypeStrUser, InfoSys.MethodOpenCard);
                return;
            }

            if (!pf.SeedIC())
            {
                this.lbCardType.Text = InfoSys.StrSeekFailure;
                pf.EndLog(InfoSys.CardTypeStrUser, InfoSys.MethodOpenCard);
                return;
            }

            this.lbSerialNumber.Text = pf.getSnr().ToString("X");

            //保存远程服务器数据库
            string str = DataTransfer.OpenCardUser(this.lbSerialNumber.Text, UserNo, UserName, IdentityNumber, Telephone, DeviceList);
            JavaScriptObject result = (JavaScriptObject)JavaScriptConvert.DeserializeObject(str);
            if (!bool.Parse(result["Result"].ToString()))
            {
                string txt = result["Message"].ToString();
                MessageBox.Show(txt);
                pf.Log(txt);
                pf.EndLog(InfoSys.CardTypeStrUser, InfoSys.MethodOpenCard);
                return;
            }

            int mode = 4; //以B密码认证
            int sec = 1; //扇区
            int block = 0;
            string key = pf.getKeyA(); //读卡密码
            string keyNew = pf.getKeyB(); //读卡密码
            string result_WriteIC = "";
            string result_AuthIC = "";

            //设置扇区1内容
            sec = 1;
            //认证卡密码B
            result_AuthIC = CardCommon.AuthIC(icdev, mode, sec, key);
            pf.AuthLog(InfoSys.CardTypeStrUser, InfoSys.MethodOpenCard, sec, result_AuthIC);
            if (result_AuthIC == InfoSys.StrAuthSuccess)
            {
                //写数据块0，数据块0为卡类型（1字节）、用户卡号（4字节）
                block = 0;
                result_WriteIC = CardCommon.WriteIC(icdev, sec, block, (InfoSys.CardTypeUser + UserNo.PadLeft(8, '0')).PadRight(32, '0'));
                if (result_WriteIC != "")
                {
                    this.lbState.Text = InfoSys.InfoWriteFailure(sec, block, InfoSys.CardTypeStrUser, InfoSys.StrOpenFailure);
                    pf.Log(this.lbState.Text);
                    pf.EndLog(InfoSys.CardTypeStrUser, InfoSys.MethodOpenCard);
                    return;
                }

                //写数据块1，数据块1为用户名（16字节）
                block = 1;
                result_WriteIC = CardCommon.WriteIC(icdev, sec, block, HexStringUtility.ByteArrayToHexString(newUserName));
                if (result_WriteIC != "")
                {
                    this.lbState.Text = InfoSys.InfoWriteFailure(sec, block, InfoSys.CardTypeStrUser, InfoSys.StrOpenFailure);
                    pf.Log(this.lbState.Text);
                    pf.EndLog(InfoSys.CardTypeStrUser, InfoSys.MethodOpenCard);
                    return;
                }

                //写数据块2，数据块2为身份证号（9字节）、联系电话（6字节）
                block = 2;
                result_WriteIC = CardCommon.WriteIC(icdev, sec, block, (IdentityNumber + "0" + Telephone).PadRight(32, '0'));
                if (result_WriteIC != "")
                {
                    this.lbState.Text = InfoSys.InfoWriteFailure(sec, block, InfoSys.CardTypeStrUser, InfoSys.StrOpenFailure);
                    pf.Log(this.lbState.Text);
                    pf.EndLog(InfoSys.CardTypeStrUser, InfoSys.MethodOpenCard);
                    return;
                }
            }
            else
            {
                this.lbState.Text = InfoSys.StrCannotOpen + InfoSys.StrOpenFailure;
                pf.Log(this.lbState.Text);
                pf.EndLog(InfoSys.CardTypeStrUser, InfoSys.MethodOpenCard);
                return;
            }

            string[] s = { "", "", "", "", "", "", "", "", "", "" };

            for (int i = 0; i < listBox1.SelectedItems.Count; i++)
            {
                string[] ss = listBox1.SelectedItems[i].ToString().Split('-');
                try
                {
                    string hex = int.Parse(ss[2]).ToString("X").PadLeft(2, '0');
                    s[i] = ss[0] + ss[1] + hex;
                }
                catch { }
            }

            //设置扇区2内容
            sec = 2;
            //认证卡密码B
            result_AuthIC = CardCommon.AuthIC(icdev, mode, sec, key);
            pf.AuthLog(InfoSys.CardTypeStrUser, InfoSys.MethodOpenCard, sec, result_AuthIC);
            if (result_AuthIC == InfoSys.StrAuthSuccess)
            {
                string data = "";
                //写数据块1，地址码1（7字节）地址码2（7字节）
                block = 1;
                data = (s[0] + s[1]).PadRight(32, '0');
                result_WriteIC = CardCommon.WriteIC(icdev, sec, block, data);
                if (result_WriteIC != "")
                {
                    this.lbState.Text = InfoSys.InfoWriteFailure(sec, block, InfoSys.CardTypeStrUser, InfoSys.StrOpenFailure);
                    pf.Log(this.lbState.Text);
                    pf.EndLog(InfoSys.CardTypeStrUser, InfoSys.MethodOpenCard);
                    return;
                }

                //写数据块2，地址码1（7字节）地址码2（7字节）
                block = 2;
                data = (s[2] + s[3]).PadRight(32, '0');
                result_WriteIC = CardCommon.WriteIC(icdev, sec, block, data);
                if (result_WriteIC != "")
                {
                    this.lbState.Text = InfoSys.InfoWriteFailure(sec, block, InfoSys.CardTypeStrUser, InfoSys.StrOpenFailure);
                    pf.Log(this.lbState.Text);
                    pf.EndLog(InfoSys.CardTypeStrUser, InfoSys.MethodOpenCard);
                    return;
                }
            }

            //设置扇区3内容
            sec = 3;
            //认证卡密码B
            result_AuthIC = CardCommon.AuthIC(icdev, mode, sec, key);
            pf.AuthLog(InfoSys.CardTypeStrUser, InfoSys.MethodOpenCard, sec, result_AuthIC);
            if (result_AuthIC == InfoSys.StrAuthSuccess)
            {
                string data = "";
                //写数据块0，地址码5（7字节）地址码6（7字节）
                block = 0;
                data = (s[4] + s[5]).PadRight(32, '0');
                result_WriteIC = CardCommon.WriteIC(icdev, sec, block, data);
                if (result_WriteIC != "")
                {
                    this.lbState.Text = InfoSys.InfoWriteFailure(sec, block, InfoSys.CardTypeStrUser, InfoSys.StrOpenFailure);
                    pf.Log(this.lbState.Text);
                    pf.EndLog(InfoSys.CardTypeStrUser, InfoSys.MethodOpenCard);
                    return;
                }

                //写数据块1，地址码7（7字节）地址码8（7字节）
                block = 1;
                data = (s[6] + s[7]).PadRight(32, '0');
                result_WriteIC = CardCommon.WriteIC(icdev, sec, block, data);
                if (result_WriteIC != "")
                {
                    this.lbState.Text = InfoSys.InfoWriteFailure(sec, block, InfoSys.CardTypeStrUser, InfoSys.StrOpenFailure);
                    pf.Log(this.lbState.Text);
                    pf.EndLog(InfoSys.CardTypeStrUser, InfoSys.MethodOpenCard);
                    return;
                }

                //写数据块2，地址码9（7字节）地址码10（7字节）
                block = 2;
                data = (s[8] + s[9]).PadRight(32, '0');
                result_WriteIC = CardCommon.WriteIC(icdev, sec, block, data);
                if (result_WriteIC != "")
                {
                    this.lbState.Text = InfoSys.InfoWriteFailure(sec, block, InfoSys.CardTypeStrUser, InfoSys.StrOpenFailure);
                    pf.Log(this.lbState.Text);
                    pf.EndLog(InfoSys.CardTypeStrUser, InfoSys.MethodOpenCard);
                    return;
                }
            }

            for (int i = 0; i < pf.getSize(); i++)
            {
                result_AuthIC = CardCommon.AuthIC(icdev, mode, i, key);
                if (result_AuthIC == InfoSys.StrAuthSuccess)
                {
                    //写数据块3，密码eeeeeeeeeeee
                    block = 3;
                    CardCommon.WritePWD(icdev, i, block, keyNew, InfoSys.KeyControl, keyNew);
                }
            }

            SerialNumber_old = "";
            this.lbCardType.Text = InfoSys.CardTypeUser;
            this.lbState.Text = InfoSys.StrOpenSuccess;
            pf.Log(this.lbState.Text);
            pf.EndLog(InfoSys.CardTypeStrUser, InfoSys.MethodOpenCard);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (SerialNumber_old == "")
            {
                MessageBox.Show("请先读卡再修改卡！");
                return;
            }

            if (MessageBox.Show("确定" + InfoSys.CardTypeStrUser + InfoSys.MethodModifyCard + "？",
                InfoSys.CardTypeStrUser + InfoSys.MethodModifyCard, MessageBoxButtons.YesNo,
                MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
            {
                return;
            }

            pf.BeginLog(InfoSys.CardTypeStrUser, InfoSys.MethodModifyCard);

            if (!CheckValue())
            {
                pf.EndLog(InfoSys.CardTypeStrUser, InfoSys.MethodModifyCard);
                return;
            }

            if (!pf.SeedIC())
            {
                this.lbCardType.Text = InfoSys.StrSeekFailure;
                pf.EndLog(InfoSys.CardTypeStrUser, InfoSys.MethodModifyCard);
                return;
            }

            this.lbSerialNumber.Text = pf.getSnr().ToString("X");

            if (SerialNumber_old != this.lbSerialNumber.Text)
            {
                MessageBox.Show("请重新读卡再修改卡！");
                pf.EndLog(InfoSys.CardTypeStrUser, InfoSys.MethodModifyCard);
                return;
            }

            //保存远程服务器数据库
            string str = DataTransfer.ModifyCardUser(this.lbSerialNumber.Text, UserNo, UserName, IdentityNumber, Telephone, DeviceList);
            JavaScriptObject result = (JavaScriptObject)JavaScriptConvert.DeserializeObject(str);
            if (!bool.Parse(result["Result"].ToString()))
            {
                string txt = result["Message"].ToString();
                MessageBox.Show(txt);
                pf.Log(txt);
                pf.EndLog(InfoSys.CardTypeStrUser, InfoSys.MethodModifyCard);
                return;
            }
            
            int mode = 4; //以B密码认证
            int sec = 1; //扇区
            int block = 0;
            string key = pf.getKeyB(); //读卡密码
            string result_WriteIC = "";
            string result_AuthIC = "";

            //设置扇区1内容
            sec = 1;
            //认证卡密码B
            result_AuthIC = CardCommon.AuthIC(icdev, mode, sec, key);
            pf.AuthLog(InfoSys.CardTypeStrUser, InfoSys.MethodModifyCard, sec, result_AuthIC);
            if (result_AuthIC == InfoSys.StrAuthSuccess)
            {
                //写数据块0，数据块0为卡类型（1字节）、用户卡号（4字节）
                block = 0;
                result_WriteIC = CardCommon.WriteIC(icdev, sec, block, (InfoSys.CardTypeUser + UserNo.PadLeft(8, '0')).PadRight(32, '0'));
                if (result_WriteIC != "")
                {
                    this.lbState.Text = InfoSys.InfoWriteFailure(sec, block, InfoSys.CardTypeStrUser, InfoSys.StrModifyFailure);
                    pf.Log(this.lbState.Text);
                    pf.EndLog(InfoSys.CardTypeStrUser, InfoSys.MethodModifyCard);
                    return;
                }

                //写数据块1，数据块1为用户名（16字节）
                block = 1;
                result_WriteIC = CardCommon.WriteIC(icdev, sec, block, HexStringUtility.ByteArrayToHexString(newUserName));
                if (result_WriteIC != "")
                {
                    this.lbState.Text = InfoSys.InfoWriteFailure(sec, block, InfoSys.CardTypeStrUser, InfoSys.StrModifyFailure);
                    pf.Log(this.lbState.Text);
                    pf.EndLog(InfoSys.CardTypeStrUser, InfoSys.MethodModifyCard);
                    return;
                }

                //写数据块2，数据块2为身份证号（9字节）、联系电话（6字节）
                block = 2;
                result_WriteIC = CardCommon.WriteIC(icdev, sec, 2, (IdentityNumber + "0" + Telephone).PadRight(32, '0'));
                if (result_WriteIC != "")
                {
                    this.lbState.Text = InfoSys.InfoWriteFailure(sec, block, InfoSys.CardTypeStrUser, InfoSys.StrModifyFailure);
                    pf.Log(this.lbState.Text);
                    pf.EndLog(InfoSys.CardTypeStrUser, InfoSys.MethodModifyCard);
                    return;
                }
            }

            string[] s = { "", "", "", "", "", "", "", "", "", "" };

            for (int i = 0; i < listBox1.SelectedItems.Count; i++)
            {
                string[] ss = listBox1.SelectedItems[i].ToString().Split('-');
                try
                {
                    string hex = int.Parse(ss[2]).ToString("X").PadLeft(2, '0');
                    s[i] = ss[0] + ss[1] + hex;
                }
                catch { }
            }

            //设置扇区2内容
            sec = 2;
            //认证卡密码B
            result_AuthIC = CardCommon.AuthIC(icdev, mode, sec, key);
            pf.AuthLog(InfoSys.CardTypeStrUser, InfoSys.MethodModifyCard, sec, result_AuthIC);
            if (result_AuthIC == InfoSys.StrAuthSuccess)
            {
                /*
                //写数据块0，剩余可用水量（4字节）剩余可用电量（4字节）累计用水量（4字节）累计用电量（4字节）
                block = 0;
                double d1 = Tools.StringToDouble(this.txtResidualWater.Text, 0);
                double d2 = Tools.StringToDouble(this.txtResidualElectric.Text, 0);
                double d3 = Tools.StringToDouble(this.txtTotalWater.Text, 0);
                double d4 = Tools.StringToDouble(this.txtTotalElectric.Text, 0);               
                result_WriteIC = CardCommon.WriteIC(icdev, sec, block,
                    Tools.RepeatStr("0", 8 - d1.ToString().Length) + d1.ToString() +
                    Tools.RepeatStr("0", 8 - d2.ToString().Length) + d2.ToString() +
                    Tools.RepeatStr("0", 8 - d3.ToString().Length) + d3.ToString() +
                    Tools.RepeatStr("0", 8 - d4.ToString().Length) + d4.ToString());
                if (result_WriteIC != "")
                {
                    this.lbState.Text = InfoSys.InfoWriteFailure(sec, block, InfoSys.CardTypeStrUser, InfoSys.StrOpenFailure);
                    pf.Log(this.lbState.Text);
                    pf.EndLog(InfoSys.CardTypeStrUser, InfoSys.MethodOpenCard);
                    return;
                }
                */

                string data = "";
                //写数据块1，地址码1（7字节）地址码2（7字节）
                block = 1;
                data = (s[0] + s[1]).PadRight(32, '0');
                result_WriteIC = CardCommon.WriteIC(icdev, sec, block, data);
                if (result_WriteIC != "")
                {
                    this.lbState.Text = InfoSys.InfoWriteFailure(sec, block, InfoSys.CardTypeStrUser, InfoSys.StrOpenFailure);
                    pf.Log(this.lbState.Text);
                    pf.EndLog(InfoSys.CardTypeStrUser, InfoSys.MethodModifyCard);
                    return;
                }

                //写数据块2，地址码1（7字节）地址码2（7字节）
                block = 2;
                data = (s[2] + s[3]).PadRight(32, '0');
                result_WriteIC = CardCommon.WriteIC(icdev, sec, block, data);
                if (result_WriteIC != "")
                {
                    this.lbState.Text = InfoSys.InfoWriteFailure(sec, block, InfoSys.CardTypeStrUser, InfoSys.StrOpenFailure);
                    pf.Log(this.lbState.Text);
                    pf.EndLog(InfoSys.CardTypeStrUser, InfoSys.MethodModifyCard);
                    return;
                }
            }

            //设置扇区3内容
            sec = 3;
            //认证卡密码B
            result_AuthIC = CardCommon.AuthIC(icdev, mode, sec, key);
            pf.AuthLog(InfoSys.CardTypeStrUser, InfoSys.MethodModifyCard, sec, result_AuthIC);
            if (result_AuthIC == InfoSys.StrAuthSuccess)
            {
                string data = "";
                //写数据块0，地址码5（7字节）地址码6（7字节）
                block = 0;
                data = (s[4] + s[5]).PadRight(32, '0');
                result_WriteIC = CardCommon.WriteIC(icdev, sec, block, data);
                if (result_WriteIC != "")
                {
                    this.lbState.Text = InfoSys.InfoWriteFailure(sec, block, InfoSys.CardTypeStrUser, InfoSys.StrOpenFailure);
                    pf.Log(this.lbState.Text);
                    pf.EndLog(InfoSys.CardTypeStrUser, InfoSys.MethodModifyCard);
                    return;
                }

                //写数据块1，地址码7（7字节）地址码8（7字节）
                block = 1;
                data = (s[6] + s[7]).PadRight(32, '0');
                result_WriteIC = CardCommon.WriteIC(icdev, sec, block, data);
                if (result_WriteIC != "")
                {
                    this.lbState.Text = InfoSys.InfoWriteFailure(sec, block, InfoSys.CardTypeStrUser, InfoSys.StrOpenFailure);
                    pf.Log(this.lbState.Text);
                    pf.EndLog(InfoSys.CardTypeStrUser, InfoSys.MethodModifyCard);
                    return;
                }

                //写数据块2，地址码9（7字节）地址码10（7字节）
                block = 2;
                data = (s[8] + s[9]).PadRight(32, '0');
                result_WriteIC = CardCommon.WriteIC(icdev, sec, block, data);
                if (result_WriteIC != "")
                {
                    this.lbState.Text = InfoSys.InfoWriteFailure(sec, block, InfoSys.CardTypeStrUser, InfoSys.StrOpenFailure);
                    pf.Log(this.lbState.Text);
                    pf.EndLog(InfoSys.CardTypeStrUser, InfoSys.MethodModifyCard);
                    return;
                }
            }

            SerialNumber_old = "";
            this.lbCardType.Text = InfoSys.CardTypeUser;
            this.lbState.Text = InfoSys.StrModifySuccess;
            pf.Log(this.lbState.Text);
            pf.EndLog(InfoSys.CardTypeStrUser, InfoSys.MethodModifyCard);
        }

        private void listBox1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItems.Count > 10)
            {
                for (int i = 0; i < listBox1.SelectedItems.Count; i++)
                {
                    if (listBox1.SelectedItem != listBox1.SelectedItems[i])
                    {
                        listBox1.SetSelected(listBox1.SelectedIndex, false);
                        return;
                    }
                }
            }
        }

        string UserNo = "";
        string UserName = "";
        byte[] newUserName = null;
        string IdentityNumber = "";
        string Telephone = "";
        string DeviceList = "";
        private bool CheckValue()
        {
            string regex1 = @"^\d{1,8}$";
            UserNo = this.txtUserNo.Text.Trim();
            if (!Regex.IsMatch(UserNo, regex1))
            {
                MessageBox.Show("用户卡号格式不对，只能是0-99999999范围整形数字");
                return false;
            }

            UserName = this.txtUserName.Text.Trim();
            byte[] bs = HexStringUtility.StrToByteArray(UserName);
            int len = bs.Length;
            if (len > 16 || len < 1)
            {
                MessageBox.Show("用户名格式不对，不能为空，最长8个中文字符");
                return false;
            }
            newUserName = HexStringUtility.StrToByteArray("".PadRight(16, ' '));
            for (int i = 0; i < len; i++)
            {
                newUserName[i] = bs[i];
            }

            string regex2 = @"^\d{18}$|^\d{17}(\d|X|x)$";
            IdentityNumber = this.txtIdentityNumber.Text.Trim();
            if (!Regex.IsMatch(IdentityNumber, regex2))
            {
                MessageBox.Show("身份证号格式不正确，必须18位");
                return false;
            }

            IdentityNumber = IdentityNumber.ToUpper().Replace("X", "A");

            string regex3 = @"^[1]\d{10}$";
            Telephone = this.txtTelephone.Text.Trim();
            if (!Regex.IsMatch(Telephone, regex3))
            {
                MessageBox.Show("电话号码格式不正确，1开头的11位数字");
                return false;
            }

            DeviceList = "";
            for (int i = 0; i < listBox1.SelectedItems.Count; i++)
            {
                string[] ss = listBox1.SelectedItems[i].ToString().Split('-');
                try
                {
                    string hex = int.Parse(ss[2]).ToString("X").PadLeft(2, '0');
                    DeviceList += ss[0] + ss[1] + hex + ",";
                }
                catch { }
            }

            return true;
        }

        frmRecharge fRecharge = null;
        private void button4_Click(object sender, EventArgs e)
        {
            if (fRecharge == null) //如果子窗体为空则创造实例 并显示
            {
                fRecharge = new frmRecharge(pf, this);
                fRecharge.Show();
            }
            else
            {
                if (fRecharge.IsDisposed)//若子窗体关闭 则打开新子窗体 并显示
                {
                    fRecharge = new frmRecharge(pf, this);
                    fRecharge.Show();
                }
                else
                {
                    fRecharge.Activate(); //使子窗体获得焦点
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (SerialNumber_old == "")
            {
                MessageBox.Show("请先读卡再注销卡！");
                return;
            }

            if (this.lbCardType.Text != InfoSys.CardTypeUser)
            {
                MessageBox.Show("非" + InfoSys.CardTypeStrUser + "无法注销！");
                return;
            }

            if (MessageBox.Show("确定" + InfoSys.CardTypeStrUser + InfoSys.MethodCancelCard + "？",
                InfoSys.CardTypeStrUser + InfoSys.MethodCancelCard, MessageBoxButtons.YesNo,
                MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
            {
                return;
            }

            pf.BeginLog(InfoSys.CardTypeStrUser, InfoSys.MethodCancelCard);

            if (!pf.SeedIC())
            {
                this.lbCardType.Text = InfoSys.StrSeekFailure;
                pf.EndLog(InfoSys.CardTypeStrUser, InfoSys.MethodCancelCard);
                return;
            }

            this.lbSerialNumber.Text = pf.getSnr().ToString("X");

            if (SerialNumber_old != this.lbSerialNumber.Text)
            {
                MessageBox.Show("请重新读卡再注销卡！");
                pf.EndLog(InfoSys.CardTypeStrUser, InfoSys.MethodCancelCard);
                return;
            }

            //保存远程服务器数据库
            string str = DataTransfer.CancelCardUser(this.lbSerialNumber.Text);
            JavaScriptObject result = (JavaScriptObject)JavaScriptConvert.DeserializeObject(str);
            if (!bool.Parse(result["Result"].ToString()))
            {
                string txt = result["Message"].ToString();
                MessageBox.Show(txt);
                pf.Log(txt);
                pf.EndLog(InfoSys.CardTypeStrUser, InfoSys.MethodCancelCard);
                return;
            }
            
            int mode = 0; //以A密码认证
            string key = pf.getKeyB(); //读卡密码
            string keyOld = pf.getKeyA(); //读卡密码
            string result_WriteIC = "";
            string result_AuthIC = "";

            for (int i = 0; i < pf.getSize(); i++)
            {
                result_AuthIC = CardCommon.AuthIC(icdev, mode, i, key);
                pf.AuthLog(InfoSys.CardTypeStrUser, InfoSys.MethodCancelCard, i, result_AuthIC);
                if (result_AuthIC != InfoSys.StrAuthSuccess)
                {
                    continue;
                }
                else
                {
                    int begin = 0;
                    if (i == 0)
                    {
                        begin = 1;
                    }

                    for (int j = begin; j < 3; j++)
                    {
                        result_WriteIC = CardCommon.WriteIC(icdev, i, j, "".PadRight(32, '0'));
                        pf.Log("注销卡扇区：" + i + " 数据块：" + j + " 结果：" + (result_WriteIC == "" ? "成功" : result_WriteIC));
                    }

                    {
                        int block = 3;
                        result_WriteIC = CardCommon.WriteIC(icdev, i, block, keyOld + InfoSys.KeyControl + keyOld);
                        pf.Log("注销卡写密码扇区：" + i + " 数据块：" + block + " 结果：" + (result_WriteIC == "" ? "成功" : result_WriteIC));
                    }
                }
            }

            SerialNumber_old = "";
            this.lbState.Text = InfoSys.MethodCancelCard + "结束";
            pf.EndLog(InfoSys.CardTypeStrUser, InfoSys.MethodCancelCard);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.txtIdentityNumber.Text = "";
            this.txtTelephone.Text = "";

            string UserName = this.txtUserName.Text.Trim();
            
            if (UserName.Length < 1)
            {
                MessageBox.Show("用户名至少输入1位");
                return;
            }
            
            string str = DataTransfer.GetWaterUserInfo(UserName, "", "");
            JavaScriptObject result = (JavaScriptObject)JavaScriptConvert.DeserializeObject(str);
            if (bool.Parse(result["Result"].ToString()))
            {
                List<WaterUser> list = JavaScriptConvert.DeserializeObject<List<WaterUser>>(result["Message"].ToString());
                if (list.Count == 1)
                {
                    this.txtUserName.Text = list[0].UserName;
                    this.txtIdentityNumber.Text = list[0].IdentityNumber;
                    this.txtTelephone.Text = list[0].Telephone;
                }
                else if (list.Count > 1)
                {
                    this.groupBox1.Location = new System.Drawing.Point(12, 12);
                    showUserList(list);
                    this.groupBox1.Show();
                }
                else
                {
                    MessageBox.Show("未查询出用户，请检查用户名");
                }
            }
            else
            {
                MessageBox.Show(result["Message"].ToString());
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.txtUserName.Text = "";
            this.txtTelephone.Text = "";

            string IdentityNumber = this.txtIdentityNumber.Text.Trim();
            
            if (IdentityNumber.Length < 6)
            {
                MessageBox.Show("身份证号至少输入6位");
                return;
            }
            
            string str = DataTransfer.GetWaterUserInfo("", IdentityNumber, "");
            JavaScriptObject result = (JavaScriptObject)JavaScriptConvert.DeserializeObject(str);
            if (bool.Parse(result["Result"].ToString()))
            {
                List<WaterUser> list = JavaScriptConvert.DeserializeObject<List<WaterUser>>(result["Message"].ToString());
                if (list.Count == 1)
                {
                    this.txtUserName.Text = list[0].UserName;
                    this.txtIdentityNumber.Text = list[0].IdentityNumber;
                    this.txtTelephone.Text = list[0].Telephone;
                }
                else if (list.Count > 1)
                {
                    this.groupBox1.Location = new System.Drawing.Point(12, 12);
                    showUserList(list);
                    this.groupBox1.Show();
                }
                else
                {
                    MessageBox.Show("未查询出用户，请检查身份证号");
                }
            }
            else
            {
                MessageBox.Show(result["Message"].ToString());
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            this.txtUserName.Text = "";
            this.txtIdentityNumber.Text = "";

            string Telephone = this.txtTelephone.Text.Trim();
            
            if (Telephone.Length < 1)
            {
                MessageBox.Show("联系电话至少输入1位");
                return;
            }
            
            string str = DataTransfer.GetWaterUserInfo("", "", Telephone);
            JavaScriptObject result = (JavaScriptObject)JavaScriptConvert.DeserializeObject(str);
            if (bool.Parse(result["Result"].ToString()))
            {
                List<WaterUser> list = JavaScriptConvert.DeserializeObject<List<WaterUser>>(result["Message"].ToString());
                if (list.Count == 1)
                {
                    this.txtUserName.Text = list[0].UserName;
                    this.txtIdentityNumber.Text = list[0].IdentityNumber;
                    this.txtTelephone.Text = list[0].Telephone;
                }
                else if (list.Count > 1)
                {
                    this.groupBox1.Location = new System.Drawing.Point(12, 12);
                    showUserList(list);
                    this.groupBox1.Show();
                }
                else
                {
                    MessageBox.Show("未查询出用户，请检查联系电话");
                }
            }
            else
            {
                MessageBox.Show(result["Message"].ToString());
            }
        }

        private void showUserList(List<WaterUser> list)
        {
            this.dataGridView1.Rows.Clear();
            if (list == null || list.Count == 0)
            {
                return;
            }
            
            foreach (WaterUser wu in list)
            {
                DataGridViewRow viewRow = new DataGridViewRow();

                DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                cell.Value = wu.UserName;
                viewRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = wu.IdentityNumber;
                viewRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = wu.Telephone;
                viewRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = wu.Address;
                viewRow.Cells.Add(cell);

                this.dataGridView1.Rows.Add(viewRow);
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedRows.Count > 0)
            {
                this.txtUserName.Text = this.dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
                this.txtIdentityNumber.Text = this.dataGridView1.SelectedRows[0].Cells[1].Value.ToString();
                this.txtTelephone.Text = this.dataGridView1.SelectedRows[0].Cells[2].Value.ToString();
            }

            this.groupBox1.Hide();
            this.dataGridView1.Rows.Clear();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            //this.groupBox1.Visible = false;
            this.groupBox1.Hide();
            this.dataGridView1.Rows.Clear();
        }

        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedRows.Count > 0)
            {
                this.txtUserName.Text = this.dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
                this.txtIdentityNumber.Text = this.dataGridView1.SelectedRows[0].Cells[1].Value.ToString();
                this.txtTelephone.Text = this.dataGridView1.SelectedRows[0].Cells[2].Value.ToString();
            }

            this.groupBox1.Hide();
            this.dataGridView1.Rows.Clear();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            string str = DataTransfer.GetWaterUserInfo("", "", "");
            JavaScriptObject result = (JavaScriptObject)JavaScriptConvert.DeserializeObject(str);
            if (bool.Parse(result["Result"].ToString()))
            {
                List<WaterUser> list = JavaScriptConvert.DeserializeObject<List<WaterUser>>(result["Message"].ToString());
                if (list.Count == 1)
                {
                    this.txtUserName.Text = list[0].UserName;
                    this.txtIdentityNumber.Text = list[0].IdentityNumber;
                    this.txtTelephone.Text = list[0].Telephone;
                }
                else if (list.Count > 1)
                {
                    this.groupBox1.Location = new System.Drawing.Point(12, 12);
                    showUserList(list);
                    this.groupBox1.Show();
                }
                else
                {
                    MessageBox.Show("无用水户！");
                }
            }
            else
            {
                MessageBox.Show(result["Message"].ToString());
            }
        }
    }
}
