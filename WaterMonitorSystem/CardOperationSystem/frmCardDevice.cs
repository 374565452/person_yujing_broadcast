using Common;
using Maticsoft.Model;
using Newtonsoft.Json;
using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace CardOperationSystem
{
    public partial class frmCardDevice : Form
    {
        private int icdev;
        frmMain pf;
        public frmCardDevice(frmMain pf)
        {
            InitializeComponent();
            this.icdev = pf.icdev;
            this.pf = pf;
        }

        private void frmCardDevice_Load(object sender, EventArgs e)
        {
            this.cboDeviceList.Items.Clear();
            this.cboDeviceList.Items.Add("[终端列表]");
            if (InfoSys.ListDevices != null && InfoSys.ListDevices.Count > 0)
            {
                foreach (Device device in InfoSys.ListDevices)
                {
                    //this.cboDeviceList.Items.Add(device.AddressCode1 + "-" + device.AddressCode2 + "-" + Tools.RepeatStr("0", 3 - device.AddressCode3.ToString().Length) + device.AddressCode3 + "-" + device.Description);
                    District d5 = InfoSys.GetDistrictById(device.DistrictId);
                    District d4 = InfoSys.GetDistrictById(d5.ParentId);
                    District d3 = InfoSys.GetDistrictById(d4.ParentId);
                    District d2 = InfoSys.GetDistrictById(d3.ParentId);
                    District d1 = InfoSys.GetDistrictById(d2.ParentId);
                    this.cboDeviceList.Items.Add(d1.DistrictCode + d2.DistrictCode + d3.DistrictCode + "-" +
                        d4.DistrictCode + d5.DistrictCode + "-" +
                        device.DeviceNo.PadLeft(3, '0') + "-" + device.DeviceName);
                }
            }

            this.cboTypeCode.Items.Clear();
            this.cboTypeCode.Items.Add("00 - 请选择");
            if (InfoSys.ListDeviceTypeCodes != null && InfoSys.ListDeviceTypeCodes.Count > 0)
            {
                foreach (DeviceTypeCode code in InfoSys.ListDeviceTypeCodes)
                {
                    this.cboTypeCode.Items.Add(code.k.ToString().PadLeft(3, '0') + "-" + code.v);
                }
            }

            this.comboBox1.Items.Clear();
            this.comboBox1.Items.Add("00 - 单站");
            this.comboBox1.Items.Add("01 - 主站");
            this.comboBox1.Items.Add("02 - 从站");

            clear();
        }

        private void clear()
        {
            SerialNumber_old = "";
            this.lbCardType.Text = InfoSys.StrUnknown;
            this.lbSerialNumber.Text = InfoSys.StrUnknown;
            this.lbState.Text = InfoSys.StrState;
            this.txtAddressCode1.Text = "";
            this.txtAddressCode2.Text = "";
            this.txtAddressCode3.Text = "";
            this.txtYearExploitation.Text = "";
            this.txtAlertAvailableWater.Text = "";
            this.txtAlertAvailableElectric.Text = "";
            this.cboTypeCode.SelectedIndex = 0;
            this.txtMeterPulse.Text = "";
            this.txtAlertWaterLevel.Text = "";
            this.cboDeviceList.SelectedIndex = 0;
            this.comboBox1.SelectedIndex = 0;
            this.txtStationCode.Text = "";
            this.textBox1.Text = "";
        }

        string SerialNumber_old = "";
        private void button1_Click(object sender, EventArgs e)
        {
            clear();

            pf.BeginLog(InfoSys.CardTypeStrDevice, InfoSys.MethodReadCard);

            if (!pf.SeedIC())
            {
                this.lbCardType.Text = InfoSys.StrSeekFailure;
                pf.EndLog(InfoSys.CardTypeStrDevice, InfoSys.MethodReadCard);
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
            pf.AuthLog(InfoSys.CardTypeStrDevice, InfoSys.MethodReadCard, sec, result_AuthIC);
            if (result_AuthIC == InfoSys.StrAuthSuccess)
            {
                //读数据块0，数据块0为卡类型（1字节）、行政区划码（3字节）、镇（乡）村编码（3字节）、测站编码（1字节）、年度可开采水量（4字节）、可用水量提醒值（2字节）、可用水量提醒值（2字节）
                block = 0;
                result_ReadIC = CardCommon.ReadIC(icdev, sec, block);
                if (result_ReadIC.Length == 32)
                {
                    this.lbCardType.Text = result_ReadIC.Substring(0, 2);
                    if (this.lbCardType.Text != InfoSys.CardTypeDevice)
                    {
                        this.lbState.Text = "非" + InfoSys.CardTypeStrDevice + "，" + InfoSys.StrCannotRead;
                        pf.Log(this.lbState.Text);
                        pf.EndLog(InfoSys.CardTypeStrDevice, InfoSys.MethodReadCard);
                        return;
                    }
                    this.txtAddressCode1.Text = result_ReadIC.Substring(2, 6);
                    this.txtAddressCode2.Text = result_ReadIC.Substring(8, 6);
                    int i_AddressCode3 = Convert.ToInt32("0x" + result_ReadIC.Substring(14, 2), 16);
                    this.txtAddressCode3.Text = i_AddressCode3.ToString();
                    ToolsForm.setTextTextBox(this.txtYearExploitation, result_ReadIC.Substring(16, 8), "0", 1);
                    try
                    {
                        ToolsForm.setTextTextBox(this.txtAlertAvailableWater, Convert.ToInt32("0x" + result_ReadIC.Substring(24, 4), 16).ToString(), "0", 0);
                    }
                    catch { }
                    try
                    {
                        ToolsForm.setTextTextBox(this.txtAlertAvailableElectric, Convert.ToInt32("0x" + result_ReadIC.Substring(28, 4), 16).ToString(), "0", 0);
                    }
                    catch { }
                }
                else
                {
                    this.lbState.Text = InfoSys.InfoReadFailure(sec, block, InfoSys.CardTypeStrDevice, InfoSys.StrReadFailure);
                    pf.Log(this.lbState.Text);
                    pf.EndLog(InfoSys.CardTypeStrDevice, InfoSys.MethodReadCard);
                    return;
                }

                //读数据块1，数据块1为流量计类型（1字节）、电表脉冲数（2字节bcd码）、水位报警值（4字节）
                block = 1;
                result_ReadIC = CardCommon.ReadIC(icdev, sec, block);
                if (result_ReadIC.Length == 32)
                {
                    string s = result_ReadIC.Substring(0, 2);
                    this.cboTypeCode.SelectedIndex = 0;
                    foreach (object item in this.cboTypeCode.Items)
                    {
                        if (int.Parse(item.ToString().Split('-')[0]) == Convert.ToInt32("0x" + s, 16))
                        {
                            this.cboTypeCode.SelectedItem = item;
                            break;
                        }
                    }
                    this.txtMeterPulse.Text = result_ReadIC.Substring(2, 4).TrimStart('0');
                    ToolsForm.setTextTextBox(this.txtAlertWaterLevel, result_ReadIC.Substring(6, 8), "0", 1);
                }
                else
                {
                    this.lbState.Text = InfoSys.InfoReadFailure(sec, block, InfoSys.CardTypeStrDevice, InfoSys.StrReadFailure);
                    pf.Log(this.lbState.Text);
                    pf.EndLog(InfoSys.CardTypeStrDevice, InfoSys.MethodReadCard);
                    return;
                }

                //读数据块2，数据块2为站类型（1字节）、地址码（2字节）
                block = 2;
                result_ReadIC = CardCommon.ReadIC(icdev, sec, block);
                if (result_ReadIC.Length == 32)
                {
                    string s1 = result_ReadIC.Substring(0, 2);
                    this.comboBox1.SelectedIndex = 0;
                    foreach (object item in this.comboBox1.Items)
                    {
                        if (int.Parse(item.ToString().Split('-')[0]) == Convert.ToInt32("0x" + s1, 16))
                        {
                            this.comboBox1.SelectedItem = item;
                            break;
                        }
                    }

                    string s2 = result_ReadIC.Substring(2, 4);
                    try
                    {
                        this.txtStationCode.Text = Convert.ToInt32("0x" + s2, 16).ToString().TrimStart('0');
                    }
                    catch { }
                    if (this.txtStationCode.Text == "") this.txtStationCode.Text = "0";
                }
                else
                {
                    this.lbState.Text = InfoSys.InfoReadFailure(sec, block, InfoSys.CardTypeStrDevice, InfoSys.StrReadFailure);
                    pf.Log(this.lbState.Text);
                    pf.EndLog(InfoSys.CardTypeStrDevice, InfoSys.MethodReadCard);
                    return;
                }
            }

            if (this.lbCardType.Text == InfoSys.CardTypeDevice)
            {
                SerialNumber_old = this.lbSerialNumber.Text.Trim();
                this.lbState.Text = InfoSys.StrReadSuccess;
            }
            else
            {
                this.lbState.Text = "非" + InfoSys.CardTypeStrDevice + "，" + InfoSys.StrCannotRead;
                MessageBox.Show(this.lbState.Text);
            }
            pf.EndLog(InfoSys.CardTypeStrDevice, InfoSys.MethodReadCard);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定" + InfoSys.CardTypeStrDevice + InfoSys.MethodOpenCard + "？",
                InfoSys.CardTypeStrDevice + InfoSys.MethodOpenCard, MessageBoxButtons.YesNo,
                MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
            {
                return;
            }

            pf.BeginLog(InfoSys.CardTypeStrDevice, InfoSys.MethodOpenCard);

            if (!CheckValue())
            {
                pf.EndLog(InfoSys.CardTypeStrDevice, InfoSys.MethodOpenCard);
                return;
            }

            if (!pf.SeedIC())
            {
                this.lbCardType.Text = InfoSys.StrSeekFailure;
                pf.EndLog(InfoSys.CardTypeStrDevice, InfoSys.MethodOpenCard);
                return;
            }

            this.lbSerialNumber.Text = pf.getSnr().ToString("X");

            //保存远程服务器数据库
            string str = DataTransfer.OpenCardDevice(this.lbSerialNumber.Text, AddressCode1, AddressCode2, AddressCode3,
                YearExploitation, AlertAvailableWater, AlertAvailableElectric, TypeCode, MeterPulse, AlertWaterLevel, StationType, StationCode, Frequency);
            JavaScriptObject result = (JavaScriptObject)JavaScriptConvert.DeserializeObject(str);
            if (!bool.Parse(result["Result"].ToString()))
            {
                string txt = result["Message"].ToString();
                MessageBox.Show(txt);
                pf.Log(txt);
                pf.EndLog(InfoSys.CardTypeStrDevice, InfoSys.MethodOpenCard);
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
            pf.AuthLog(InfoSys.CardTypeStrDevice, InfoSys.MethodOpenCard, sec, result_AuthIC);
            if (result_AuthIC == InfoSys.StrAuthSuccess)
            {
                //写数据块0，数据块0为卡类型（1字节）、行政区划码（3字节）、镇（乡）村编码（3字节）、测站编码（1字节）、年度可开采水量（4字节）、可用水量提醒值（2字节）、可用水量提醒值（2字节）
                block = 0;
                string hex_AddressCode3 = int.Parse(AddressCode3).ToString("X").PadLeft(2, '0');
                double d_YearExploitation = Tools.StringToDoubleMultiply10(YearExploitation, 0);
                string hex_AlertAvailableWater = int.Parse(AlertAvailableWater).ToString("X").PadLeft(4, '0');
                string hex_AlertAvailableElectric = int.Parse(AlertAvailableElectric).ToString("X").PadLeft(4, '0');
                result_WriteIC = CardCommon.WriteIC(icdev, sec, block, InfoSys.CardTypeDevice + AddressCode1 + AddressCode2 +
                    hex_AddressCode3 + d_YearExploitation.ToString().PadLeft(8, '0') + hex_AlertAvailableWater + hex_AlertAvailableElectric);
                if (result_WriteIC != "")
                {
                    this.lbState.Text = InfoSys.InfoWriteFailure(sec, block, InfoSys.CardTypeStrDevice, InfoSys.StrModifyFailure);
                    pf.Log(this.lbState.Text);
                    pf.EndLog(InfoSys.CardTypeStrDevice, InfoSys.MethodModifyCard);
                    return;
                }

                //写数据块1，数据块1为流量计类型（1字节）、电表脉冲数（2字节bcd码）、水位报警值（4字节）
                block = 1;
                string hex_TypeCode = int.Parse(TypeCode).ToString("X").PadLeft(2, '0');
                double d_AlertWaterLevel = Tools.StringToDoubleMultiply10(AlertWaterLevel, 0);
                result_WriteIC = CardCommon.WriteIC(icdev, sec, block, (hex_TypeCode +
                     MeterPulse.ToString().PadLeft(4, '0') +
                     d_AlertWaterLevel.ToString().PadLeft(8, '0')).PadRight(32, '0'));
                if (result_WriteIC != "")
                {
                    this.lbState.Text = InfoSys.InfoWriteFailure(sec, block, InfoSys.CardTypeStrDevice, InfoSys.StrModifyFailure);
                    pf.Log(this.lbState.Text);
                    pf.EndLog(InfoSys.CardTypeStrDevice, InfoSys.MethodModifyCard);
                    return;
                }

                //写数据块2，数据块2为站类型（1字节）、地址码（2字节）
                block = 2;
                string hex_StationType = int.Parse(StationType).ToString("X").PadLeft(2, '0');
                string hex_StationCode = int.Parse(StationCode).ToString("X").PadLeft(4, '0');
                result_WriteIC = CardCommon.WriteIC(icdev, sec, block, (hex_StationType + hex_StationCode).PadRight(32, '0'));
                if (result_WriteIC != "")
                {
                    this.lbState.Text = InfoSys.InfoWriteFailure(sec, block, InfoSys.CardTypeStrDevice, InfoSys.StrOpenFailure);
                    pf.Log(this.lbState.Text);
                    pf.EndLog(InfoSys.CardTypeStrDevice, InfoSys.MethodOpenCard);
                    return;
                }
            }
            else
            {
                this.lbState.Text = InfoSys.StrCannotOpen + InfoSys.StrOpenFailure;
                pf.Log(this.lbState.Text);
                pf.EndLog(InfoSys.CardTypeStrDevice, InfoSys.MethodOpenCard);
                return;
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
            this.lbCardType.Text = InfoSys.CardTypeDevice;
            this.lbState.Text = InfoSys.StrOpenSuccess;
            pf.Log(this.lbState.Text);
            pf.EndLog(InfoSys.CardTypeStrDevice, InfoSys.MethodOpenCard);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (SerialNumber_old == "")
            {
                MessageBox.Show("请先读卡再修改卡！");
                return;
            }

            if (MessageBox.Show("确定" + InfoSys.CardTypeStrDevice + InfoSys.MethodModifyCard + "？",
                InfoSys.CardTypeStrDevice + InfoSys.MethodModifyCard, MessageBoxButtons.YesNo,
                MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
            {
                return;
            }

            pf.BeginLog(InfoSys.CardTypeStrDevice, InfoSys.MethodModifyCard);

            if (!CheckValue())
            {
                pf.EndLog(InfoSys.CardTypeStrDevice, InfoSys.MethodModifyCard);
                return;
            }

            if (!pf.SeedIC())
            {
                this.lbCardType.Text = InfoSys.StrSeekFailure;
                pf.EndLog(InfoSys.CardTypeStrDevice, InfoSys.MethodModifyCard);
                return;
            }

            this.lbSerialNumber.Text = pf.getSnr().ToString("X");

            if (SerialNumber_old != this.lbSerialNumber.Text)
            {
                MessageBox.Show("请重新读卡再修改卡！");
                pf.EndLog(InfoSys.CardTypeStrDevice, InfoSys.MethodModifyCard);
                return;
            }

            //保存远程服务器数据库
            string str = DataTransfer.ModifyCardDevice(this.lbSerialNumber.Text, AddressCode1, AddressCode2, AddressCode3,
                YearExploitation, AlertAvailableWater, AlertAvailableElectric, TypeCode, MeterPulse, AlertWaterLevel, StationType, StationCode, Frequency);
            JavaScriptObject result = (JavaScriptObject)JavaScriptConvert.DeserializeObject(str);
            if (!bool.Parse(result["Result"].ToString()))
            {
                string txt = result["Message"].ToString();
                MessageBox.Show(txt);
                pf.Log(txt);
                pf.EndLog(InfoSys.CardTypeStrDevice, InfoSys.MethodModifyCard);
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
            pf.AuthLog(InfoSys.CardTypeStrDevice, InfoSys.MethodModifyCard, sec, result_AuthIC);
            if (result_AuthIC == InfoSys.StrAuthSuccess)
            {
                //写数据块0，数据块0为卡类型（1字节）、行政区划码（3字节）、镇（乡）村编码（3字节）、测站编码（1字节）、年度可开采水量（4字节）、可用水量提醒值（2字节）、可用水量提醒值（2字节）
                block = 0;
                string hex_AddressCode3 = int.Parse(AddressCode3).ToString("X").PadLeft(2, '0');
                double d_YearExploitation = Tools.StringToDoubleMultiply10(YearExploitation, 0);
                string hex_AlertAvailableWater = int.Parse(AlertAvailableWater).ToString("X").PadLeft(4, '0');
                string hex_AlertAvailableElectric = int.Parse(AlertAvailableElectric).ToString("X").PadLeft(4, '0');
                result_WriteIC = CardCommon.WriteIC(icdev, sec, block, InfoSys.CardTypeDevice + AddressCode1 + AddressCode2 +
                    hex_AddressCode3 + d_YearExploitation.ToString().PadLeft(8, '0') + hex_AlertAvailableWater + hex_AlertAvailableElectric);
                if (result_WriteIC != "")
                {
                    this.lbState.Text = InfoSys.InfoWriteFailure(sec, block, InfoSys.CardTypeStrDevice, InfoSys.StrModifyFailure);
                    pf.Log(this.lbState.Text);
                    pf.EndLog(InfoSys.CardTypeStrDevice, InfoSys.MethodModifyCard);
                    return;
                }

                //写数据块1，数据块1为流量计类型（1字节）、电表脉冲数（2字节bcd码）、水位报警值（4字节）
                block = 1;
                string hex_TypeCode = int.Parse(TypeCode).ToString("X").PadLeft(2, '0');
                double d_AlertWaterLevel = Tools.StringToDoubleMultiply10(AlertWaterLevel, 0);
                result_WriteIC = CardCommon.WriteIC(icdev, sec, block, (hex_TypeCode +
                     MeterPulse.ToString().PadLeft(4, '0') +
                     d_AlertWaterLevel.ToString().PadLeft(8, '0')).PadRight(32, '0'));
                if (result_WriteIC != "")
                {
                    this.lbState.Text = InfoSys.InfoWriteFailure(sec, block, InfoSys.CardTypeStrDevice, InfoSys.StrModifyFailure);
                    pf.Log(this.lbState.Text);
                    pf.EndLog(InfoSys.CardTypeStrDevice, InfoSys.MethodModifyCard);
                    return;
                }

                //写数据块2，数据块2为站类型（1字节）、地址码（2字节）
                block = 2;
                string hex_StationType = int.Parse(StationType).ToString("X").PadLeft(2, '0');
                string hex_StationCode = int.Parse(StationCode).ToString("X").PadLeft(4, '0');
                result_WriteIC = CardCommon.WriteIC(icdev, sec, block, (hex_StationType + hex_StationCode).PadRight(32, '0'));
                if (result_WriteIC != "")
                {
                    this.lbState.Text = InfoSys.InfoWriteFailure(sec, block, InfoSys.CardTypeStrDevice, InfoSys.StrOpenFailure);
                    pf.Log(this.lbState.Text);
                    pf.EndLog(InfoSys.CardTypeStrDevice, InfoSys.MethodOpenCard);
                    return;
                }
            }

            SerialNumber_old = "";
            this.lbCardType.Text = InfoSys.CardTypeDevice;
            this.lbState.Text = InfoSys.StrModifySuccess;
            pf.Log(this.lbState.Text);
            pf.EndLog(InfoSys.CardTypeStrDevice, InfoSys.MethodModifyCard);
        }

        string AddressCode1 = "";
        string AddressCode2 = "";
        string AddressCode3 = "";
        string YearExploitation = "";
        string AlertAvailableWater = "";
        string TypeCode = "";
        string MeterPulse = "";
        string AlertWaterLevel = "";
        string AlertAvailableElectric = "";
        string StationType = "";
        string StationCode = "";
        string Frequency = "";
        string MainAddressCode3 = "";
        private bool CheckValue()
        {
            string regex1 = @"^\d{6}$";
            AddressCode1 = this.txtAddressCode1.Text.Trim();
            if (!Regex.IsMatch(AddressCode1, regex1))
            {
                MessageBox.Show("行政区划码必须6位数字");
                return false;
            }

            AddressCode2 = this.txtAddressCode2.Text.Trim();
            if (!Regex.IsMatch(AddressCode2, regex1))
            {
                MessageBox.Show("镇（乡）村编码必须6位数字");
                return false;
            }

            AddressCode3 = this.txtAddressCode3.Text.Trim();
            if (!Tools.CheckValue(AddressCode3, "int", 0, 0, 1, 254))
            {
                MessageBox.Show("测站选址范围为 1-254");
                return false;
            }

            string regex2 = @"^(\d{1,7})(\.\d{1})?$";
            YearExploitation = this.txtYearExploitation.Text.Trim();
            if (!Regex.IsMatch(YearExploitation, regex2))
            {
                MessageBox.Show("年度可开采水量格式不正确，0-9999999.9");
                return false;
            }

            AlertAvailableWater = this.txtAlertAvailableWater.Text.Trim();
            if (!Tools.CheckValue(AlertAvailableWater, "int", 0, 0, 0, 65535))
            {
                MessageBox.Show("可用水量提醒值范围为 0-65535");
                return false;
            }

            AlertAvailableElectric = this.txtAlertAvailableElectric.Text.Trim();
            if (!Tools.CheckValue(AlertAvailableElectric, "int", 0, 0, 0, 65535))
            {
                MessageBox.Show("可用电量提醒值范围为 0-65535");
                return false;
            }

            if (this.cboTypeCode.SelectedIndex < 1)
            {
                MessageBox.Show("请选择流量计类型");
                return false;
            }

            TypeCode = this.cboTypeCode.SelectedItem.ToString().Split('-')[0].Trim();

            string regex3 = @"^\d{1,4}$";
            MeterPulse = this.txtMeterPulse.Text.Trim();
            if (!Regex.IsMatch(MeterPulse, regex3))
            {
                MessageBox.Show("电表脉冲数格式不对，只能是0-9999范围整形数字");
                return false;
            }

            AlertWaterLevel = this.txtAlertWaterLevel.Text.Trim();
            if (!Regex.IsMatch(AlertWaterLevel, regex2))
            {
                MessageBox.Show("水位报警值格式不正确，0-9999999.9");
                return false;
            }

            StationType = this.comboBox1.SelectedItem.ToString().Split('-')[0].Trim();

            StationCode = this.txtStationCode.Text.Trim();
            if (!Tools.CheckValue(StationCode, "int", 0, 0, 0, 65535))
            {
                MessageBox.Show("可用电量提醒值范围为 0-65535");
                return false;
            }

            MainAddressCode3 = this.textBox1.Text.Trim();
            if (MainAddressCode3 == "") MainAddressCode3 = "0";
            if (!Tools.CheckValue(MainAddressCode3, "int", 0, 0, 0, 254))
            {
                MessageBox.Show("主站编码范围为 0-254");
                return false;
            }

            Frequency = "0";

            return true;
        }

        private void cboDeviceList_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.textBox1.Text = "0";
            if (this.cboDeviceList.SelectedIndex > 0)
            {
                string[] ss = this.cboDeviceList.SelectedItem.ToString().Split('-');
                try
                {
                    this.txtAddressCode1.Text = ss[0];
                    this.txtAddressCode2.Text = ss[1];
                    this.txtAddressCode3.Text = ss[2];

                    if (InfoSys.ListDevices != null && InfoSys.ListDevices.Count > 0)
                    {
                        foreach (Device device in InfoSys.ListDevices)
                        {
                            District d5 = InfoSys.GetDistrictById(device.DistrictId);
                            District d4 = InfoSys.GetDistrictById(d5.ParentId);
                            District d3 = InfoSys.GetDistrictById(d4.ParentId);
                            District d2 = InfoSys.GetDistrictById(d3.ParentId);
                            District d1 = InfoSys.GetDistrictById(d2.ParentId);

                            if (d1.DistrictCode + d2.DistrictCode + d3.DistrictCode == ss[0] &&
                                d4.DistrictCode + d5.DistrictCode == ss[1] && int.Parse(device.DeviceNo) == int.Parse(ss[2]))
                            {
                                this.txtYearExploitation.Text = device.YearExploitation.ToString();
                                this.txtAlertAvailableWater.Text = device.AlertAvailableWater.ToString();
                                this.txtAlertAvailableElectric.Text = device.AlertAvailableElectric.ToString();
                                this.txtMeterPulse.Text = device.MeterPulse.ToString();
                                this.txtAlertWaterLevel.Text = device.AlertWaterLevel.ToString();

                                foreach (object item in this.cboTypeCode.Items)
                                {
                                    if (int.Parse(item.ToString().Split('-')[0]) == device.DeviceTypeCodeId)
                                    {
                                        this.cboTypeCode.SelectedItem = item;
                                        break;
                                    }
                                }

                                foreach (object item in this.comboBox1.Items)
                                {
                                    if (int.Parse(item.ToString().Split('-')[0]) == device.StationType)
                                    {
                                        this.comboBox1.SelectedItem = item;
                                        break;
                                    }
                                }

                                this.txtStationCode.Text = device.StationCode.ToString();

                                if (device.MainId > 0)
                                {
                                    foreach (Device device1 in InfoSys.ListDevices)
                                    {
                                        if (device1.Id == device.MainId)
                                        {
                                            this.textBox1.Text = device1.DeviceNo;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch { }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (SerialNumber_old == "")
            {
                MessageBox.Show("请先读卡再注销卡！");
                return;
            }

            if (this.lbCardType.Text != InfoSys.CardTypeDevice)
            {
                MessageBox.Show("非" + InfoSys.CardTypeStrDevice + "无法注销！");
                return;
            }

            if (MessageBox.Show("确定" + InfoSys.CardTypeStrDevice + InfoSys.MethodCancelCard + "？",
                InfoSys.CardTypeStrDevice + InfoSys.MethodCancelCard, MessageBoxButtons.YesNo,
                MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
            {
                return;
            }

            pf.BeginLog(InfoSys.CardTypeStrDevice, InfoSys.MethodCancelCard);

            if (!pf.SeedIC())
            {
                this.lbCardType.Text = InfoSys.StrSeekFailure;
                pf.EndLog(InfoSys.CardTypeStrDevice, InfoSys.MethodCancelCard);
                return;
            }

            this.lbSerialNumber.Text = pf.getSnr().ToString("X");

            if (SerialNumber_old != this.lbSerialNumber.Text)
            {
                MessageBox.Show("请重新读卡再注销卡！");
                pf.EndLog(InfoSys.CardTypeStrDevice, InfoSys.MethodCancelCard);
                return;
            }

            //保存远程服务器数据库
            string str = DataTransfer.CancelCardDevice(this.lbSerialNumber.Text);
            JavaScriptObject result = (JavaScriptObject)JavaScriptConvert.DeserializeObject(str);
            if (!bool.Parse(result["Result"].ToString()))
            {
                string txt = result["Message"].ToString();
                MessageBox.Show(txt);
                pf.Log(txt);
                pf.EndLog(InfoSys.CardTypeStrDevice, InfoSys.MethodCancelCard);
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
                pf.AuthLog(InfoSys.CardTypeStrDevice, InfoSys.MethodCancelCard, i, result_AuthIC);
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
            pf.EndLog(InfoSys.CardTypeStrDevice, InfoSys.MethodCancelCard);
        }
    }
}
