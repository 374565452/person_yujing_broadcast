using Newtonsoft.Json;
using System;
using System.Windows.Forms;

namespace CardOperationSystem
{
    public partial class frmCardRead : Form
    {
        private int icdev;
        frmMain pf;
        public frmCardRead(frmMain pf)
        {
            InitializeComponent();
            this.icdev = pf.icdev;
            this.pf = pf;
        }

        private void frmCardRead_Load(object sender, EventArgs e)
        {
            clear();
        }

        private void clear()
        {
            SerialNumber_old = "";
            this.lbCardType.Text = InfoSys.StrUnknown;
            this.lbSerialNumber.Text = InfoSys.StrUnknown;
            this.lbState.Text = InfoSys.StrState;

            foreach (Control c in Controls)
            {
                if (c is Label)
                {
                    if (c.Tag != null && c.Tag.ToString() == "a")
                        (c as Label).Text = "";
                }
            }
        }

        string SerialNumber_old = "";
        private void button1_Click(object sender, EventArgs e)
        {
            clear();

            pf.BeginLog(InfoSys.CardTypeStrRead, InfoSys.MethodReadCard);

            if (!pf.SeedIC())
            {
                this.lbCardType.Text = InfoSys.StrSeekFailure;
                pf.EndLog(InfoSys.CardTypeStrRead, InfoSys.MethodReadCard);
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
            pf.AuthLog(InfoSys.CardTypeStrRead, InfoSys.MethodReadCard, sec, result_AuthIC);
            if (result_AuthIC == InfoSys.StrAuthSuccess)
            {
                //读数据块0，数据块0为卡类型（1字节）
                block = 0;
                result_ReadIC = CardCommon.ReadIC(icdev, sec, block);
                if (result_ReadIC.Length == 32)
                {
                    this.lbCardType.Text = result_ReadIC.Substring(0, 2);
                    if (this.lbCardType.Text != InfoSys.CardTypeRead)
                    {
                        this.lbState.Text = "非" + InfoSys.CardTypeStrRead + "，" + InfoSys.StrCannotRead;
                        pf.Log(this.lbState.Text);
                        pf.EndLog(InfoSys.CardTypeStrRead, InfoSys.MethodReadCard);
                        return;
                    }
                }
                else
                {
                    this.lbState.Text = InfoSys.InfoReadFailure(sec, block, InfoSys.CardTypeStrRead, InfoSys.StrReadFailure);
                    pf.Log(this.lbState.Text);
                    pf.EndLog(InfoSys.CardTypeStrRead, InfoSys.MethodReadCard);
                    return;
                }

                //读数据块1，累计用水量（4字节），累计用电量（4字节），年可开采量（4字节），年剩余可开采量（4字节）
                block = 1;
                result_ReadIC = CardCommon.ReadIC(icdev, sec, block);
                if (result_ReadIC.Length == 32)
                {
                    ToolsForm.setTextLabel(this.label35, result_ReadIC.Substring(0, 8), "0", 1);
                    ToolsForm.setTextLabel(this.label36, result_ReadIC.Substring(8, 8), "0", 1);
                    ToolsForm.setTextLabel(this.label37, result_ReadIC.Substring(16, 8), "0", 1);
                    ToolsForm.setTextLabel(this.label38, result_ReadIC.Substring(24, 8), "0", 1);
                }
                else
                {
                    this.lbState.Text = InfoSys.InfoReadFailure(sec, block, InfoSys.CardTypeStrRead, InfoSys.StrReadFailure);
                    pf.Log(this.lbState.Text);
                    pf.EndLog(InfoSys.CardTypeStrRead, InfoSys.MethodReadCard);
                    return;
                }

                //读数据块2，地址码（7字节），设备类型（1字节）
                block = 2;
                result_ReadIC = CardCommon.ReadIC(icdev, sec, block);
                if (result_ReadIC.Length == 32)
                {
                    this.label33.Text = result_ReadIC.Substring(0, 8);
                    this.label34.Text = result_ReadIC.Substring(8, 2);
                    if (this.label34.Text.Trim() == "00")
                    {
                        this.label34.Text += " - 单站";
                    }
                    else if (this.label34.Text.Trim() == "01")
                    {
                        this.label34.Text += " - 主站";
                    }
                    else if (this.label34.Text.Trim() == "02")
                    {
                        this.label34.Text += " - 从站";
                    }
                    else
                    {
                        this.label34.Text += " - 未知类型";
                    }
                }
                else
                {
                    this.lbState.Text = InfoSys.InfoReadFailure(sec, block, InfoSys.CardTypeStrRead, InfoSys.StrReadFailure);
                    pf.Log(this.lbState.Text);
                    pf.EndLog(InfoSys.CardTypeStrRead, InfoSys.MethodReadCard);
                    return;
                }
            }

            //读取扇区2内容
            sec = 2;
            //认证卡密码B
            result_AuthIC = CardCommon.AuthIC(icdev, mode, sec, key);
            pf.AuthLog(InfoSys.CardTypeStrRead, InfoSys.MethodReadCard, sec, result_AuthIC);
            if (result_AuthIC == InfoSys.StrAuthSuccess)
            {
                //读数据块0，用户卡号（4字节），ic序列号（4字节），开泵剩余水量（4字节），开泵剩余电量（4字节）
                block = 0;
                result_ReadIC = CardCommon.ReadIC(icdev, sec, block);
                if (result_ReadIC.Length == 32)
                {
                    this.label39.Text = result_ReadIC.Substring(0, 8);
                    this.label40.Text = result_ReadIC.Substring(8, 8);
                    ToolsForm.setTextLabel(this.label43, result_ReadIC.Substring(16, 8), "0", 1);
                    ToolsForm.setTextLabel(this.label44, result_ReadIC.Substring(24, 8), "0", 1);
                }
                else
                {
                    this.lbState.Text = InfoSys.InfoReadFailure(sec, block, InfoSys.CardTypeStrRead, InfoSys.StrReadFailure);
                    pf.Log(this.lbState.Text);
                    pf.EndLog(InfoSys.CardTypeStrRead, InfoSys.MethodReadCard);
                    return;
                }

                //读数据块1，本次用水量（4字节），本次用电量（4字节），关泵剩余水量（4字节），关泵剩余电量（4字节）
                block = 1;
                result_ReadIC = CardCommon.ReadIC(icdev, sec, block);
                if (result_ReadIC.Length == 32)
                {
                    ToolsForm.setTextLabel(this.label49, result_ReadIC.Substring(0, 8), "0", 1);
                    ToolsForm.setTextLabel(this.label50, result_ReadIC.Substring(8, 8), "0", 1);
                    ToolsForm.setTextLabel(this.label47, result_ReadIC.Substring(16, 8), "0", 1);
                    ToolsForm.setTextLabel(this.label48, result_ReadIC.Substring(24, 8), "0", 1);
                }
                else
                {
                    this.lbState.Text = InfoSys.InfoReadFailure(sec, block, InfoSys.CardTypeStrRead, InfoSys.StrReadFailure);
                    pf.Log(this.lbState.Text);
                    pf.EndLog(InfoSys.CardTypeStrRead, InfoSys.MethodReadCard);
                    return;
                }

                //读数据块1，开泵时间（6字节），关泵时间（6字节），记录类型（1字节），开泵类型（1字节），关泵类型（1字节）
                block = 2;
                result_ReadIC = CardCommon.ReadIC(icdev, sec, block);
                if (result_ReadIC.Length == 32)
                {
                    this.label41.Text = result_ReadIC.Substring(0, 12);
                    this.label45.Text = result_ReadIC.Substring(12, 12);
                    this.label51.Text = result_ReadIC.Substring(24, 2);
                    if (this.label51.Text.Trim() == "00")
                    {
                        this.label51.Text += " - 正常扣费";
                    }
                    else if (this.label51.Text.Trim() == "01")
                    {
                        this.label51.Text += " - 未扣费";
                    }
                    else if (this.label51.Text.Trim() == "02")
                    {
                        this.label51.Text += " - 免费";
                    }
                    else
                    {
                        this.label51.Text += " - 未知";
                    }
                    this.label42.Text = result_ReadIC.Substring(26, 2);
                    if (this.label42.Text.Trim() == "01")
                    {
                        this.label42.Text += " - 刷卡开泵";
                    }
                    else if (this.label42.Text.Trim() == "02")
                    {
                        this.label42.Text += " - 手动开泵";
                    }
                    else if (this.label42.Text.Trim() == "03")
                    {
                        this.label42.Text += " - 远程GPRS开泵";
                    }
                    else if (this.label42.Text.Trim() == "04")
                    {
                        this.label42.Text += " - 远程短信开泵";
                    }
                    else
                    {
                        this.label42.Text += " - 未知";
                    }
                    this.label46.Text = result_ReadIC.Substring(28, 2);
                    if (this.label46.Text.Trim() == "01")
                    {
                        this.label46.Text += " - 刷卡关泵";
                    }
                    else if (this.label46.Text.Trim() == "02")
                    {
                        this.label46.Text += " - 手动关泵";
                    }
                    else if (this.label46.Text.Trim() == "03")
                    {
                        this.label46.Text += " - 远程GPRS关泵";
                    }
                    else if (this.label46.Text.Trim() == "04")
                    {
                        this.label46.Text += " - 远程短信关泵";
                    }
                    else if (this.label46.Text.Trim() == "05")
                    {
                        this.label46.Text += " - 欠费关泵";
                    }
                    else
                    {
                        this.label46.Text += " - 未知";
                    }
                }
                else
                {
                    this.lbState.Text = InfoSys.InfoReadFailure(sec, block, InfoSys.CardTypeStrRead, InfoSys.StrReadFailure);
                    pf.Log(this.lbState.Text);
                    pf.EndLog(InfoSys.CardTypeStrRead, InfoSys.MethodReadCard);
                    return;
                }
            }

            //读取扇区3内容
            sec = 3;
            //认证卡密码B
            result_AuthIC = CardCommon.AuthIC(icdev, mode, sec, key);
            pf.AuthLog(InfoSys.CardTypeStrRead, InfoSys.MethodReadCard, sec, result_AuthIC);
            if (result_AuthIC == InfoSys.StrAuthSuccess)
            {
                //读数据块0，用户卡号（4字节），ic序列号（4字节），开泵剩余水量（4字节），开泵剩余电量（4字节）
                block = 0;
                result_ReadIC = CardCommon.ReadIC(icdev, sec, block);
                if (result_ReadIC.Length == 32)
                {
                    this.label52.Text = result_ReadIC.Substring(0, 8);
                    this.label53.Text = result_ReadIC.Substring(8, 8);
                    ToolsForm.setTextLabel(this.label56, result_ReadIC.Substring(16, 8), "0", 1);
                    ToolsForm.setTextLabel(this.label57, result_ReadIC.Substring(24, 8), "0", 1);
                }
                else
                {
                    this.lbState.Text = InfoSys.InfoReadFailure(sec, block, InfoSys.CardTypeStrRead, InfoSys.StrReadFailure);
                    pf.Log(this.lbState.Text);
                    pf.EndLog(InfoSys.CardTypeStrRead, InfoSys.MethodReadCard);
                    return;
                }

                //读数据块1，本次用水量（4字节），本次用电量（4字节），关泵剩余水量（4字节），关泵剩余电量（4字节）
                block = 1;
                result_ReadIC = CardCommon.ReadIC(icdev, sec, block);
                if (result_ReadIC.Length == 32)
                {
                    ToolsForm.setTextLabel(this.label62, result_ReadIC.Substring(0, 8), "0", 1);
                    ToolsForm.setTextLabel(this.label63, result_ReadIC.Substring(8, 8), "0", 1);
                    ToolsForm.setTextLabel(this.label60, result_ReadIC.Substring(16, 8), "0", 1);
                    ToolsForm.setTextLabel(this.label61, result_ReadIC.Substring(24, 8), "0", 1);
                }
                else
                {
                    this.lbState.Text = InfoSys.InfoReadFailure(sec, block, InfoSys.CardTypeStrRead, InfoSys.StrReadFailure);
                    pf.Log(this.lbState.Text);
                    pf.EndLog(InfoSys.CardTypeStrRead, InfoSys.MethodReadCard);
                    return;
                }

                //读数据块1，开泵时间（6字节），关泵时间（6字节），记录类型（1字节），开泵类型（1字节），关泵类型（1字节）
                block = 2;
                result_ReadIC = CardCommon.ReadIC(icdev, sec, block);
                if (result_ReadIC.Length == 32)
                {
                    this.label54.Text = result_ReadIC.Substring(0, 12);
                    this.label58.Text = result_ReadIC.Substring(12, 12);
                    this.label64.Text = result_ReadIC.Substring(24, 2);
                    if (this.label64.Text.Trim() == "00")
                    {
                        this.label64.Text += " - 正常扣费";
                    }
                    else if (this.label64.Text.Trim() == "01")
                    {
                        this.label64.Text += " - 未扣费";
                    }
                    else if (this.label64.Text.Trim() == "02")
                    {
                        this.label64.Text += " - 免费";
                    }
                    else
                    {
                        this.label64.Text += " - 未知";
                    }
                    this.label55.Text = result_ReadIC.Substring(26, 2);
                    if (this.label55.Text.Trim() == "01")
                    {
                        this.label55.Text += " - 刷卡开泵";
                    }
                    else if (this.label55.Text.Trim() == "02")
                    {
                        this.label55.Text += " - 手动开泵";
                    }
                    else if (this.label55.Text.Trim() == "03")
                    {
                        this.label55.Text += " - 远程GPRS开泵";
                    }
                    else if (this.label55.Text.Trim() == "04")
                    {
                        this.label55.Text += " - 远程短信开泵";
                    }
                    else
                    {
                        this.label55.Text += " - 未知";
                    }
                    this.label59.Text = result_ReadIC.Substring(28, 2);
                    if (this.label59.Text.Trim() == "01")
                    {
                        this.label59.Text += " - 刷卡关泵";
                    }
                    else if (this.label59.Text.Trim() == "02")
                    {
                        this.label59.Text += " - 手动关泵";
                    }
                    else if (this.label59.Text.Trim() == "03")
                    {
                        this.label59.Text += " - 远程GPRS关泵";
                    }
                    else if (this.label59.Text.Trim() == "04")
                    {
                        this.label59.Text += " - 远程短信关泵";
                    }
                    else if (this.label59.Text.Trim() == "05")
                    {
                        this.label59.Text += " - 欠费关泵";
                    }
                    else
                    {
                        this.label59.Text += " - 未知";
                    }
                }
                else
                {
                    this.lbState.Text = InfoSys.InfoReadFailure(sec, block, InfoSys.CardTypeStrRead, InfoSys.StrReadFailure);
                    pf.Log(this.lbState.Text);
                    pf.EndLog(InfoSys.CardTypeStrRead, InfoSys.MethodReadCard);
                    return;
                }
            }

            if (this.lbCardType.Text == InfoSys.CardTypeRead)
            {
                SerialNumber_old = this.lbSerialNumber.Text.Trim();
                this.lbState.Text = InfoSys.StrReadSuccess;
            }
            else
            {
                this.lbState.Text = "非" + InfoSys.CardTypeStrRead + "，" + InfoSys.StrCannotRead;
                MessageBox.Show(this.lbState.Text);
            }
            pf.EndLog(InfoSys.CardTypeStrRead, InfoSys.MethodReadCard);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定" + InfoSys.CardTypeStrRead + InfoSys.MethodOpenCard + "？",
                InfoSys.CardTypeStrRead + InfoSys.MethodOpenCard, MessageBoxButtons.YesNo,
                MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
            {
                return;
            }

            pf.BeginLog(InfoSys.CardTypeStrRead, InfoSys.MethodOpenCard);

            if (!CheckValue())
            {
                pf.EndLog(InfoSys.CardTypeStrRead, InfoSys.MethodOpenCard);
                return;
            }

            if (!pf.SeedIC())
            {
                this.lbCardType.Text = InfoSys.StrSeekFailure;
                pf.EndLog(InfoSys.CardTypeStrRead, InfoSys.MethodOpenCard);
                return;
            }

            this.lbSerialNumber.Text = pf.getSnr().ToString("X");

            //保存远程服务器数据库
            string str = DataTransfer.OpenCardRead(this.lbSerialNumber.Text, "", "", "0");
            JavaScriptObject result = (JavaScriptObject)JavaScriptConvert.DeserializeObject(str);
            if (!bool.Parse(result["Result"].ToString()))
            {
                string txt = result["Message"].ToString();
                MessageBox.Show(txt);
                pf.Log(txt);
                pf.EndLog(InfoSys.CardTypeStrRead, InfoSys.MethodOpenCard);
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
            pf.AuthLog(InfoSys.CardTypeStrRead, InfoSys.MethodOpenCard, sec, result_AuthIC);
            if (result_AuthIC == InfoSys.StrAuthSuccess)
            {
                //写数据块0，数据块0为卡类型（1字节）
                block = 0;
                result_WriteIC = CardCommon.WriteIC(icdev, sec, block, (InfoSys.CardTypeRead).PadRight(32, '0'));
                if (result_WriteIC != "")
                {
                    this.lbState.Text = InfoSys.InfoWriteFailure(sec, block, InfoSys.CardTypeStrRead, InfoSys.StrOpenFailure);
                    pf.Log(this.lbState.Text);
                    pf.EndLog(InfoSys.CardTypeStrRead, InfoSys.MethodOpenCard);
                    return;
                }
            }
            else
            {
                this.lbState.Text = InfoSys.StrCannotOpen + InfoSys.StrOpenFailure;
                pf.Log(this.lbState.Text);
                pf.EndLog(InfoSys.CardTypeStrRead, InfoSys.MethodOpenCard);
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
            this.lbCardType.Text = InfoSys.CardTypeRead;
            this.lbState.Text = InfoSys.StrOpenSuccess;
            pf.Log(this.lbState.Text);
            pf.EndLog(InfoSys.CardTypeStrRead, InfoSys.MethodOpenCard);
        }


        private bool CheckValue()
        {
            return true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (SerialNumber_old == "")
            {
                MessageBox.Show("请先读卡再注销卡！");
                return;
            }

            if (this.lbCardType.Text != InfoSys.CardTypeRead)
            {
                MessageBox.Show("非" + InfoSys.CardTypeStrRead + "无法注销！");
                return;
            }

            if (MessageBox.Show("确定" + InfoSys.CardTypeStrRead + InfoSys.MethodCancelCard + "？",
                InfoSys.CardTypeStrRead + InfoSys.MethodCancelCard, MessageBoxButtons.YesNo,
                MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
            {
                return;
            }

            pf.BeginLog(InfoSys.CardTypeStrRead, InfoSys.MethodCancelCard);

            if (!pf.SeedIC())
            {
                this.lbCardType.Text = InfoSys.StrSeekFailure;
                pf.EndLog(InfoSys.CardTypeStrRead, InfoSys.MethodCancelCard);
                return;
            }

            this.lbSerialNumber.Text = pf.getSnr().ToString("X");

            if (SerialNumber_old != this.lbSerialNumber.Text)
            {
                MessageBox.Show("请重新读卡再注销卡！");
                pf.EndLog(InfoSys.CardTypeStrRead, InfoSys.MethodCancelCard);
                return;
            }

            //保存远程服务器数据库
            string str = DataTransfer.CancelCardRead(this.lbSerialNumber.Text);
            JavaScriptObject result = (JavaScriptObject)JavaScriptConvert.DeserializeObject(str);
            if (!bool.Parse(result["Result"].ToString()))
            {
                string txt = result["Message"].ToString();
                MessageBox.Show(txt);
                pf.Log(txt);
                pf.EndLog(InfoSys.CardTypeStrRead, InfoSys.MethodCancelCard);
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
                pf.AuthLog(InfoSys.CardTypeStrRead, InfoSys.MethodCancelCard, i, result_AuthIC);
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
            pf.EndLog(InfoSys.CardTypeStrRead, InfoSys.MethodCancelCard);
        }
    }
}
