﻿using Newtonsoft.Json;
using System;
using System.Windows.Forms;

namespace CardOperationSystem
{
    public partial class frmCardClear : Form
    {
        private int icdev;
        frmMain pf;
        public frmCardClear(frmMain pf)
        {
            InitializeComponent();
            this.icdev = pf.icdev;
            this.pf = pf;
        }

        private void frmCardClear_Load(object sender, EventArgs e)
        {
            clear();
        }

        private void clear()
        {
            SerialNumber_old = "";
            this.lbCardType.Text = InfoSys.StrUnknown;
            this.lbSerialNumber.Text = InfoSys.StrUnknown;
            this.lbState.Text = InfoSys.StrState;
        }

        string SerialNumber_old = "";
        private void button1_Click(object sender, EventArgs e)
        {
            clear();

            pf.BeginLog(InfoSys.CardTypeStrClear, InfoSys.MethodReadCard);

            if (!pf.SeedIC())
            {
                this.lbCardType.Text = InfoSys.StrSeekFailure;
                pf.EndLog(InfoSys.CardTypeStrClear, InfoSys.MethodReadCard);
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
            pf.AuthLog(InfoSys.CardTypeStrClear, InfoSys.MethodReadCard, sec, result_AuthIC);
            if (result_AuthIC == InfoSys.StrAuthSuccess)
            {
                //读数据块0，数据块0为卡类型（1字节）
                block = 0;
                result_ReadIC = CardCommon.ReadIC(icdev, sec, block);
                if (result_ReadIC.Length == 32)
                {
                    this.lbCardType.Text = result_ReadIC.Substring(0, 2);
                    if (this.lbCardType.Text != InfoSys.CardTypeClear)
                    {
                        this.lbState.Text = "非" + InfoSys.CardTypeStrClear + "，" + InfoSys.StrCannotRead;
                        pf.Log(this.lbState.Text);
                        pf.EndLog(InfoSys.CardTypeStrClear, InfoSys.MethodReadCard);
                        return;
                    }
                }
                else
                {
                    this.lbState.Text = InfoSys.InfoWriteFailure(sec, block, InfoSys.CardTypeStrClear, InfoSys.StrReadFailure);
                    pf.Log(this.lbState.Text);
                    pf.EndLog(InfoSys.CardTypeStrClear, InfoSys.MethodOpenCard);
                    return;
                }
            }

            if (this.lbCardType.Text == InfoSys.CardTypeClear)
            {
                SerialNumber_old = this.lbSerialNumber.Text.Trim();
                this.lbState.Text = InfoSys.StrReadSuccess;
            }
            else
            {
                this.lbState.Text = "非" + InfoSys.CardTypeStrClear + "，" + InfoSys.StrCannotRead;
                MessageBox.Show(this.lbState.Text);
            }
            pf.EndLog(InfoSys.CardTypeStrClear, InfoSys.MethodReadCard);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定" + InfoSys.CardTypeStrClear + InfoSys.MethodOpenCard + "？",
                InfoSys.CardTypeStrClear + InfoSys.MethodOpenCard, MessageBoxButtons.YesNo,
                MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
            {
                return;
            }

            pf.BeginLog(InfoSys.CardTypeStrClear, InfoSys.MethodOpenCard);

            if (!pf.SeedIC())
            {
                this.lbCardType.Text = InfoSys.StrSeekFailure;
                pf.EndLog(InfoSys.CardTypeStrClear, InfoSys.MethodOpenCard);
                return;
            }

            this.lbSerialNumber.Text = pf.getSnr().ToString("X");

            //保存远程服务器数据库
            string str = DataTransfer.OpenCardClear(this.lbSerialNumber.Text);
            JavaScriptObject result = (JavaScriptObject)JavaScriptConvert.DeserializeObject(str);
            if (!bool.Parse(result["Result"].ToString()))
            {
                string txt = result["Message"].ToString();
                MessageBox.Show(txt);
                pf.Log(txt);
                pf.EndLog(InfoSys.CardTypeStrClear, InfoSys.MethodOpenCard);
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
            pf.AuthLog(InfoSys.CardTypeStrClear, InfoSys.MethodOpenCard, sec, result_AuthIC);
            if (result_AuthIC == InfoSys.StrAuthSuccess)
            {
                //写数据块0，数据块0为卡类型（1字节）
                block = 0;
                result_WriteIC = CardCommon.WriteIC(icdev, sec, block, InfoSys.CardTypeClear.PadRight(32, '0'));
                if (result_WriteIC != "")
                {
                    this.lbState.Text = InfoSys.InfoWriteFailure(sec, block, InfoSys.CardTypeStrClear, InfoSys.StrOpenFailure);
                    pf.Log(this.lbState.Text);
                    pf.EndLog(InfoSys.CardTypeStrClear, InfoSys.MethodOpenCard);
                    return;
                }
            }
            else
            {
                this.lbState.Text = InfoSys.StrCannotOpen + InfoSys.StrOpenFailure;
                pf.Log(this.lbState.Text);
                pf.EndLog(InfoSys.CardTypeStrClear, InfoSys.MethodOpenCard);
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
            this.lbCardType.Text = InfoSys.CardTypeClear;
            this.lbState.Text = InfoSys.StrOpenSuccess;
            pf.Log(this.lbState.Text);
            pf.EndLog(InfoSys.CardTypeStrClear, InfoSys.MethodOpenCard);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (SerialNumber_old == "")
            {
                MessageBox.Show("请先读卡再注销卡！");
                return;
            }

            if (this.lbCardType.Text != InfoSys.CardTypeClear)
            {
                MessageBox.Show("非" + InfoSys.CardTypeStrClear + "无法注销！");
                return;
            }

            if (MessageBox.Show("确定" + InfoSys.CardTypeStrClear + InfoSys.MethodCancelCard + "？",
                InfoSys.CardTypeStrClear + InfoSys.MethodCancelCard, MessageBoxButtons.YesNo,
                MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
            {
                return;
            }

            pf.BeginLog(InfoSys.CardTypeStrClear, InfoSys.MethodCancelCard);

            if (!pf.SeedIC())
            {
                this.lbCardType.Text = InfoSys.StrSeekFailure;
                pf.EndLog(InfoSys.CardTypeStrClear, InfoSys.MethodCancelCard);
                return;
            }

            this.lbSerialNumber.Text = pf.getSnr().ToString("X");

            if (SerialNumber_old != this.lbSerialNumber.Text)
            {
                MessageBox.Show("请重新读卡再注销卡！");
                pf.EndLog(InfoSys.CardTypeStrClear, InfoSys.MethodCancelCard);
                return;
            }

            //保存远程服务器数据库
            string str = DataTransfer.CancelCardClear(this.lbSerialNumber.Text);
            JavaScriptObject result = (JavaScriptObject)JavaScriptConvert.DeserializeObject(str);
            if (!bool.Parse(result["Result"].ToString()))
            {
                string txt = result["Message"].ToString();
                MessageBox.Show(txt);
                pf.Log(txt);
                pf.EndLog(InfoSys.CardTypeStrClear, InfoSys.MethodCancelCard);
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
                pf.AuthLog(InfoSys.CardTypeStrClear, InfoSys.MethodCancelCard, i, result_AuthIC);
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
            pf.EndLog(InfoSys.CardTypeStrClear, InfoSys.MethodCancelCard);
        }
    }
}
