using Common;
using DTU.GateWay.Protocol;
using DTU.GateWay.Protocol.WaterMessageClass;
using Maticsoft.Model;
using Module;
using Server.Data.Bridge;
using Server.Util.Cache;
using Server.Util.Service;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace DTUGateWay
{
    public partial class frmCS1 : Form
    {
        DataBridge bridge;
        public frmCS1()
        {
            InitializeComponent();

            init();
        }

        public frmCS1(DataBridge bridge)
        {
            InitializeComponent();

            init();

            this.bridge = bridge;
        }

        private void frmCS1_Load(object sender, EventArgs e)
        {

        }

        private void init()
        {
            SafeSetLabel(this.label8, "", false);
            SafeSetLabel(this.label9, "", false);
        }

        #region 多线程修改控件

        private delegate void DG_SafeSetLabel(Label lbl, string text, bool flag);
        /// <summary>
        /// 修改Label文本值
        /// </summary>
        /// <param name="lbl">Label对象</param>
        /// <param name="text">文本内容</param>
        /// <param name="flag">是否追加内容</param>
        private void SafeSetLabel(Label lbl, string text, bool flag)
        {
            if (this.InvokeRequired)
            {
                DG_SafeSetLabel call = delegate(Label l, string s, bool f)
                {
                    if (f)
                    {
                        l.Text += s;
                    }
                    else
                    {
                        l.Text = s;
                    }
                };

                this.Invoke(call, lbl, text, flag);
            }
            else
            {
                if (flag)
                {
                    lbl.Text += text;
                }
                else
                {
                    lbl.Text = text;
                }
            }
        }

        private delegate void DG_SafeSetText(TextBox txt, string text, bool flag);
        /// <summary>
        /// 修改TextBox文本值
        /// </summary>
        /// <param name="txt">TextBox对象</param>
        /// <param name="text">文本内容</param>
        /// <param name="flag">是否追加内容</param>
        private void SafeSetText(TextBox txt, string text, bool flag)
        {
            if (this.InvokeRequired)
            {
                DG_SafeSetText call = delegate(TextBox t, string s, bool f)
                {
                    if (f)
                    {
                        t.Text += s;
                    }
                    else
                    {
                        t.Text = s;
                    }
                };

                this.Invoke(call, txt, text, flag);
            }
            else
            {
                if (flag)
                {
                    txt.Text += text;
                }
                else
                {
                    txt.Text = text;
                }
            }
        }

        private delegate void DG_SafeSetButton(Button btn, bool IsEnable);
        /// <summary>
        /// 设置Button是否可用
        /// </summary>
        /// <param name="btn">Button对象</param>
        /// <param name="IsEnable">是否可用</param>
        private void SafeSetButton(Button btn, bool IsEnable)
        {
            if (this.InvokeRequired)
            {
                DG_SafeSetButton call = delegate(Button b, bool isE)
                {
                    b.Enabled = isE;
                };

                this.Invoke(call, btn, IsEnable);
            }
            else
            {
                btn.Enabled = IsEnable;
            }
        }

        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(new ParameterizedThreadStart(send));
            t.Start(1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(new ParameterizedThreadStart(send));
            t.Start(2);
        }

        #region 发送命令

        private bool isRun = true;
        private void time()
        {
            SafeSetLabel(this.label9, "启动", false);
            while (isRun)
            {
                SafeSetLabel(this.label9, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), false);
                Thread.Sleep(10);
            }
            SafeSetLabel(this.label9, "停止", false);
        }

        private void send(object o)
        {
            SafeSetLabel(this.label9, "", false);

            setAllButtonEnable(false);

            string OperationType = "";
            int k = 0;
            try
            {
                k = Convert.ToInt32(o);
                switch (k)
                {
                    case 1: OperationType = "时段数据查询"; break;
                    case 2: OperationType = "实时数据查询"; break;
                    default: OperationType = ""; break;
                }
                if (OperationType.Trim() == "")
                {
                    SafeSetText(textBox2, "发送命令非法！命令字：" + k, false);
                    setAllButtonEnable(true);
                    return;
                }
            }
            catch (Exception ex)
            {
                SafeSetText(textBox2, "发送命令非法！" + ex.Message, false);
                setAllButtonEnable(true);
                return;
            }

            string CenterStation = this.txtCenterStation.Text.ToString().Trim();
            string RemoteStation = this.txtRemoteStation.Text.ToString().Trim();
            string PW = this.txtPW.Text.ToString().Trim();

            try
            {
                Device device = DeviceModule.GetDeviceByRemoteStation(RemoteStation);
                if (device == null)
                {
                    SafeSetText(textBox2, "遥测站编号不存在！", false);
                    setAllButtonEnable(true);
                    return;
                }
                string deviceNo = DeviceModule.GetFullDeviceNoByID(device.Id);
                string resDeviceNo = DeviceModule.GetDeviceNoMain(deviceNo);

                WaterBaseMessage cmd = new WaterBaseMessage();
                cmd.CenterStation = Convert.ToByte(CenterStation, 16);
                cmd.RemoteStation = RemoteStation;
                cmd.PW = PW;

                string cKey = cmd.RemoteStation + cmd.CenterStation.ToString("X").PadLeft(2, '0').ToUpper();
                switch (k)
                {
                    case 1:
                        WaterCmd_38_1 cmd1 = new WaterCmd_38_1();
                        cmd1.CenterStation = cmd.CenterStation;
                        cmd1.RemoteStation = cmd.RemoteStation;
                        cmd1.PW = cmd.PW;

                        cmd1.SerialNumber = 0;
                        cmd1.SendTime = DateTime.Now;
                        cmd1.StartTime = this.dtpStartTime.Value;
                        cmd1.EndTime = this.dtpEndTime.Value;
                        cmd1.Iden_04 = new Identifier_04();
                        cmd1.Iden_04.SetVal(0, 0, this.txtIdentifier.Text.Trim().Substring(4, 6));
                        //cmd1.Iden = this.txtIdentifier.Text.Trim();
                        byte Key = Convert.ToByte(this.txtIdentifier.Text.Trim().Substring(0, 2), 16);
                        if (Key == (byte)Identifier_Standard._FF)
                        {
                            byte KeySub = Convert.ToByte(this.txtIdentifier.Text.Trim().Substring(2, 2), 16);
                            if (KeySub == (byte)Identifier_Custom._03)
                            {
                                cmd1.Iden = new Identifier_FF_03();
                            }
                            else if (KeySub == (byte)Identifier_Custom._0E)
                            {
                                cmd1.Iden = new Identifier_FF_0E();
                            }
                        }
                        else
                        {
                            if (Key == (byte)Identifier_Standard._03)
                            {
                                cmd1.Iden = new Identifier_03();
                            }
                            else if (Key == (byte)Identifier_Standard._0E)
                            {
                                cmd1.Iden = new Identifier_0E();
                            }
                            else if (Key == (byte)Identifier_Standard._1A)
                            {
                                cmd1.Iden = new Identifier_1A();
                            }
                            else if (Key == (byte)Identifier_Standard._1F)
                            {
                                cmd1.Iden = new Identifier_1F();
                            }
                            else if (Key == (byte)Identifier_Standard._20)
                            {
                                cmd1.Iden = new Identifier_20();
                            }
                            else if (Key == (byte)Identifier_Standard._26)
                            {
                                cmd1.Iden = new Identifier_26();
                            }
                            else if (Key == (byte)Identifier_Standard._38)
                            {
                                cmd1.Iden = new Identifier_38();
                            }
                            else if (Key == (byte)Identifier_Standard._39)
                            {
                                cmd1.Iden = new Identifier_39();
                            }
                            else if (Key == (byte)Identifier_Standard._F4)
                            {
                                cmd1.Iden = new Identifier_F4();
                            }
                            else if (Key == (byte)Identifier_Standard._F5)
                            {
                                cmd1.Iden = new Identifier_F5();
                            }
                        }

                        string msg1 = cmd1.WriteMsg();
                        if (msg1 == "")
                        {
                            cmd.AFN = cmd1.AFN;
                            cmd.RawDataStr = cmd1.RawDataStr;
                            cmd.RawDataChar = HexStringUtility.HexStringToByteArray(cmd.RawDataStr);
                        }
                        else
                        {
                            SafeSetText(textBox2, Environment.NewLine + OperationType + "命令生成出错！" + msg1, true);
                            return;
                        }
                        break;
                    case 2:
                        WaterCmd_37_1 cmd2 = new WaterCmd_37_1();
                        cmd2.CenterStation = cmd.CenterStation;
                        cmd2.RemoteStation = cmd.RemoteStation;
                        cmd2.PW = cmd.PW;

                        cmd2.SerialNumber = 0;
                        cmd2.SendTime = DateTime.Now;
                        string msg2 = cmd2.WriteMsg();
                        if (msg2 == "")
                        {
                            cmd.AFN = cmd2.AFN;
                            cmd.RawDataStr = cmd2.RawDataStr;
                            cmd.RawDataChar = HexStringUtility.HexStringToByteArray(cmd.RawDataStr);
                        }
                        else
                        {
                            SafeSetText(textBox2, Environment.NewLine + OperationType + "命令生成出错！" + msg2, true);
                            return;
                        }
                        break;
                    default: break;
                }

                SafeSetText(textBox2, cmd.RawDataStr, false);
                SafeSetText(textBox2, Environment.NewLine + "添加发送命令保存！" + cKey + "-" + cmd.AFN, true);

                DeviceOperation op = new DeviceOperation();
                op.DeviceNo = deviceNo;
                op.DeviceName = DeviceModule.GetDeviceByFullDeviceNo(deviceNo).DeviceName;
                op.OperationTime = DateTime.Now;
                op.OperationType = OperationType;
                op.RawData = cmd.RawDataStr;
                op.Remark = "";
                op.UserId = -1;
                op.UserName = "网关测试";
                op.State = "等待发送";

                ToWaterDtuCommand.AddBaseMessageToDtu(cKey + "-" + cmd.AFN, cmd);

                if (OnlineDeviceService.GetOnline(deviceNo) != null || OnlineDeviceService.GetOnline(resDeviceNo) != null)
                {
                    string state = "等待发送";
                    if (bridge != null)
                    {
                        if (bridge.sendToDtu(deviceNo, resDeviceNo, cmd.RawDataChar, 0, cmd.RawDataChar.Length))
                        {
                            state = "发送成功";
                        }
                        else
                        {
                            state = "发送失败";
                        }
                    }

                    op.State = state;
                    SafeSetText(textBox2, Environment.NewLine + state, true);

                    if (ToWaterDtuCommand.GetBaseMessageToDtuByKey(cKey + "-" + cmd.AFN) != null)
                    {
                        int timeDelay = 0;
                        bool flag = false;
                        while (timeDelay < 300)
                        {
                            WaterBaseMessage res = null;
                            switch (k)
                            {
                                case 1:
                                    res = ToWaterDtuCommand.GetBaseMessageFromDtuByKey(cKey + "-" + cmd.AFN);
                                    if (res != null)
                                    {
                                        SafeSetText(textBox2, Environment.NewLine + OperationType + "成功！" + timeDelay + "，返回结果：" + ((WaterCmd_38_2)res).ToString(), true);
                                        ToWaterDtuCommand.RemoveBaseMessageFromDtu(cKey + "-" + cmd.AFN);
                                        flag = true;
                                    }
                                    break;
                                case 2:
                                    res = ToWaterDtuCommand.GetBaseMessageFromDtuByKey(cKey + "-" + cmd.AFN);
                                    if (res != null)
                                    {
                                        SafeSetText(textBox2, Environment.NewLine + OperationType + "成功！" + timeDelay + "，返回结果：" + ((WaterCmd_37_2)res).ToString(), true);
                                        ToWaterDtuCommand.RemoveBaseMessageFromDtu(cKey + "-" + cmd.AFN);
                                        flag = true;
                                    }
                                    break;
                            }
                            if (flag)
                            {
                                break;
                            }
                            Thread.Sleep(100);
                            timeDelay = timeDelay + 1;
                        }
                        if (!flag)
                        {
                            SafeSetText(textBox2, Environment.NewLine + OperationType + "出错！" + "响应超时", true);
                        }
                    }
                    else
                    {
                        SafeSetText(textBox2, Environment.NewLine + OperationType + "出错！" + "无命令保存", true);
                    }
                }
                else
                {
                    op.State = "终端离线";
                    SafeSetText(textBox2, Environment.NewLine + OperationType + "出错！" + "终端离线", true);
                }

                DeviceOperationModule.AddDeviceOperation(op);
            }
            catch (Exception ex)
            {
                SafeSetText(textBox2, OperationType + "出错！" + Environment.NewLine + ex.Message, false);
            }
            setAllButtonEnable(true);
        }

        DateTime date1 = DateTime.Now;
        DateTime date2 = DateTime.Now;
        private void setAllButtonEnable(bool isEnable)
        {
            SafeSetButton(button1, isEnable);
            SafeSetButton(button2, isEnable);

            isRun = !isEnable;

            if (isRun)
            {
                SafeSetLabel(this.label7, "", false);

                date1 = DateTime.Now;
                string s1 = "开始时间：" + date1.ToString("yyyy-MM-dd HH:mm:ss.fff");
                SafeSetLabel(this.label8, s1, false);

                Thread thTime = new Thread(new ThreadStart(time));
                thTime.Start();
            }
            else
            {
                date2 = DateTime.Now;
                string s2 = "结束时间：" + date2.ToString("yyyy-MM-dd HH:mm:ss.fff");
                SafeSetLabel(this.label8, Environment.NewLine + s2 + Environment.NewLine + "耗时：" + (date2 - date1).TotalSeconds + "秒", true);
            }
        }

        #endregion

        private void button3_Click(object sender, EventArgs e)
        {
            WaterCmd_37_2 wa = new WaterCmd_37_2();
            //wa.RawDataStr = "7E 7E 16 00 00 00 00 01 00 00 37 00 24 02 00 00 16 08 30 16 58 21 F1 F1 00 00 00 00 01 47 F0 F0 16 08 30 16 58 F0 F0 16 08 30 16 58 0E 1A 00 18 79 03 03 07 46".Replace(" ", "");
            wa.RawDataStr = textBox1.Text.Trim().Replace(" ", "");
            wa.RawDataChar = HexStringUtility.HexStringToByteArray(wa.RawDataStr);
            string msg = wa.ReadMsgBase();
            if (msg == "")
            {
                string msg1 = wa.ReadMsg(wa.UserData);
                if (msg1 == "")
                {
                    MessageBox.Show("解析正确");
                    textBox3.Text = wa.ToString();
                }
                else
                {
                    MessageBox.Show("msg1:" + msg1);
                }
            }
            else
            {
                MessageBox.Show("msg:" + msg);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            WaterCmd_38_2 wa = new WaterCmd_38_2();
            //wa.RawDataStr = "FE FE 16 00 00 00 00 01 00 00 38 4F 02 00 00 16 09 01 09 15 27 F1 F1 00 00 00 00 01 47 F0 F0 16 08 23 08 00 04 18 01 00 00 FF 03 0C 02 30 02 30 02 30 02 30 02 30 02 30 FF FF FF FF FF FF FF FF FF FF FF FF 02 30 02 30 02 30 02 30 02 30 02 30 02 30 02 30 02 30 02 30 02 30 02 30 03 27 89".Replace(" ", "");
            wa.RawDataStr = textBox1.Text.Trim().Replace(" ", "");
            wa.RawDataChar = HexStringUtility.HexStringToByteArray(wa.RawDataStr);
            string msg = wa.ReadMsgBase();
            if (msg == "")
            {
                string msg1 = wa.ReadMsg(wa.UserData);
                if (msg1 == "")
                {
                    MessageBox.Show("解析正确");
                    textBox3.Text = wa.ToString();
                }
                else
                {
                    MessageBox.Show("msg1:" + msg1);
                }
            }
            else
            {
                MessageBox.Show("msg:" + msg);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            WaterCmd_38_2 wa = new WaterCmd_38_2();
            string UserDataAll = textBox1.Text.Trim().Replace(" ", "");
            string msg = wa.ReadMsg(UserDataAll);
            if (msg == "")
            {
                MessageBox.Show("解析正确");
            }
            else
            {
                MessageBox.Show("msg:" + msg);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {

            byte[] buffer = HexStringUtility.HexStringToByteArray(textBox1.Text.Trim().Replace(" ", ""));
            int length = buffer.Length;
            textBox3.Text += Environment.NewLine + "【总长度】：" + length;
            int pos = 0;
            while (pos < length)
            {
                try
                {
                    if (buffer[pos] == HexStringUtility.StrToByteArray(ProtocolKey.ReceiveHeartThrob)[0] || length - pos < 14)
                    {

                        if (length - pos < 14)
                        {
                            textBox3.Text += Environment.NewLine + "【非法数据】长度：" + (length - pos);
                            break;
                        }
                        else
                        {
                            textBox3.Text += Environment.NewLine + "【非法数据】心跳：" + ProtocolKey.ReceiveHeartThrob;
                            pos += 1;
                        }
                    }
                    else if (buffer[pos] == BaseProtocol.BeginChar)
                    {
                        int bodyLength = buffer[pos + 1]; //长度
                        byte[] bs = new byte[5 + bodyLength];
                        Array.Copy(buffer, pos, bs, 0, bs.Length);

                        textBox3.Text += Environment.NewLine + "【自定义数据】：" + FormatHelper.ByteArrayToHexString(bs);

                        pos += 5 + bodyLength;
                    }
                    else if (buffer[pos] == WaterBaseProtocol.BeginChar[0] && buffer[pos + 1] == WaterBaseProtocol.BeginChar[1])
                    {
                        byte[] bs_UpOrDown_Length = new byte[2];
                        Array.Copy(buffer, pos + 11, bs_UpOrDown_Length, 0, 2);

                        string str_UpOrDown_Length = HexStringUtility.ByteArrayToHexString(bs_UpOrDown_Length).ToUpper();
                        string str_Length = str_UpOrDown_Length.Substring(1, 3);
                        int bodyLength = Convert.ToInt32(str_Length, 16); //正文长度

                        int baseLength = 0; //其它长度
                        baseLength += 2;//帧起始符
                        baseLength += 6;//中心站及遥测站
                        baseLength += 2;//密码
                        baseLength += 1;//功能码
                        baseLength += 2;//报文上下行表示及长度
                        baseLength += 1;//报文起始符
                        baseLength += 1;//报文结束符
                        baseLength += 2;//校验码
                        byte[] bs = new byte[baseLength + bodyLength];
                        Array.Copy(buffer, pos, bs, 0, bs.Length);

                        textBox3.Text += Environment.NewLine + "【水文数据】：" + FormatHelper.ByteArrayToHexString(bs);
                        FX(bs);
                        pos += baseLength + bodyLength;
                    }
                    else
                    {
                        pos += 1;
                    }
                }
                catch (Exception ex)
                {
                    textBox3.Text += Environment.NewLine + "【错误】【pos：" + pos + "】：" + ex.Message;
                    break;
                }
            }
        }


        private void FX(byte[] buffer)
        {
            WaterBaseMessage message_water = new WaterBaseMessage();
            message_water.RawDataChar = buffer;
            message_water.RawDataStr = HexStringUtility.ByteArrayToHexString(buffer);
            try
            {
                string msg = message_water.ReadMsgBase();
                if (msg == "")
                {
                    string RemoteStation = message_water.RemoteStation;
                    byte CenterStation = message_water.CenterStation;
                    byte AFN = message_water.AFN;
                    int UpOrDown = message_water.UpOrDown;
                    int Length = message_water.Length;
                    byte DataBeginChar = message_water.DataBeginChar;
                    int TotalPackage = message_water.TotalPackage;
                    int CurrentPackage = message_water.CurrentPackage;
                    byte DataEndChar = message_water.DataEndChar;

                    string cKey = RemoteStation + CenterStation.ToString("X").PadLeft(2, '0').ToUpper();

                    WaterBaseMessage[] MsgList = WaterDeviceService.Get(cKey);
                    if (TotalPackage > 0)
                    {
                        if (MsgList == null)
                            MsgList = new WaterBaseMessage[TotalPackage];
                        MsgList[CurrentPackage - 1] = message_water;
                    }
                    else
                    {
                        MsgList = new WaterBaseMessage[1];
                        MsgList[0] = message_water;
                    }
                    WaterDeviceService.Add(cKey, MsgList);

                    if (UpOrDown == (int)WaterBaseProtocol.UpOrDown.Up)
                    {
                        #region 上行数据
                        textBox3.Text = "【上行数据】【" +
                            EnumUtils.GetDescription(typeof(WaterBaseProtocol.DataEndChar_Up), DataEndChar) + "】【" +
                            EnumUtils.GetDescription(typeof(WaterBaseProtocol.DataBeginChar), DataBeginChar) + "】";
                        if (DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETX
                            || DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Up.ETB && DataBeginChar == (byte)WaterBaseProtocol.DataBeginChar.STX)
                        {
                            #region 数据传输完成，可以解析
                            textBox3.Text += Environment.NewLine + "【数据传输完成，可以解析】";

                            if (AFN == (byte)WaterBaseProtocol.AFN._2F)
                            {
                                #region _2F
                                WaterCmd_2F_1 cmdreceive = new WaterCmd_2F_1();
                                cmdreceive.MsgList = MsgList;
                                string msgreceive = cmdreceive.ReadMsg();
                                if (msgreceive == "")
                                {
                                    textBox3.Text += Environment.NewLine + cmdreceive.ToString();
                                }
                                else
                                {
                                    textBox3.Text += Environment.NewLine + msgreceive;
                                }
                                #endregion
                            }
                            else if (AFN == (byte)WaterBaseProtocol.AFN._30)
                            {
                                #region _30
                                WaterCmd_30_1 cmdreceive = new WaterCmd_30_1();
                                cmdreceive.MsgList = MsgList;
                                string msgreceive = cmdreceive.ReadMsg();
                                if (msgreceive == "")
                                {
                                    textBox3.Text += Environment.NewLine + cmdreceive.ToString();
                                }
                                else
                                {
                                    textBox3.Text += Environment.NewLine + msgreceive;
                                }
                                #endregion
                            }
                            else if (AFN == (byte)WaterBaseProtocol.AFN._31)
                            {
                                #region _31
                                WaterCmd_31_1 cmdreceive = new WaterCmd_31_1();
                                cmdreceive.MsgList = MsgList;
                                string msgreceive = cmdreceive.ReadMsg();
                                if (msgreceive == "")
                                {
                                    textBox3.Text += Environment.NewLine + cmdreceive.ToString();
                                }
                                else
                                {
                                    textBox3.Text += Environment.NewLine + msgreceive;
                                }
                                #endregion
                            }
                            else if (AFN == (byte)WaterBaseProtocol.AFN._32)
                            {
                                #region _32
                                WaterCmd_32_1 cmdreceive = new WaterCmd_32_1();
                                cmdreceive.MsgList = MsgList;
                                string msgreceive = cmdreceive.ReadMsg();
                                if (msgreceive == "")
                                {
                                    textBox3.Text += Environment.NewLine + cmdreceive.ToString();
                                }
                                else
                                {
                                    textBox3.Text += Environment.NewLine + msgreceive;
                                }
                                #endregion
                            }
                            else if (AFN == (byte)WaterBaseProtocol.AFN._33)
                            {
                                #region _33
                                WaterCmd_33_1 cmdreceive = new WaterCmd_33_1();
                                cmdreceive.MsgList = MsgList;
                                string msgreceive = cmdreceive.ReadMsg();
                                if (msgreceive == "")
                                {
                                    textBox3.Text += Environment.NewLine + cmdreceive.ToString();
                                }
                                else
                                {
                                    textBox3.Text += Environment.NewLine + msgreceive;
                                }
                                #endregion
                            }
                            else if (AFN == (byte)WaterBaseProtocol.AFN._34)
                            {
                                #region _34
                                WaterCmd_34_1 cmdreceive = new WaterCmd_34_1();
                                cmdreceive.MsgList = MsgList;
                                string msgreceive = cmdreceive.ReadMsg();
                                if (msgreceive == "")
                                {
                                    textBox3.Text += Environment.NewLine + cmdreceive.ToString();
                                }
                                else
                                {
                                    textBox3.Text += Environment.NewLine + msgreceive;
                                }
                                #endregion
                            }
                            else if (AFN == (byte)WaterBaseProtocol.AFN._35)
                            {
                                #region _35
                                WaterCmd_35_1 cmdreceive = new WaterCmd_35_1();
                                cmdreceive.MsgList = MsgList;
                                string msgreceive = cmdreceive.ReadMsg();
                                if (msgreceive == "")
                                {
                                    textBox3.Text += Environment.NewLine + cmdreceive.ToString();
                                }
                                else
                                {
                                    textBox3.Text += Environment.NewLine + msgreceive;
                                }
                                #endregion
                            }
                            else if (AFN == (byte)WaterBaseProtocol.AFN._36)
                            {
                                #region _36
                                WaterCmd_36_2 cmdreceive = new WaterCmd_36_2();
                                cmdreceive.MsgList = MsgList;
                                string msgreceive = cmdreceive.ReadMsg();
                                if (msgreceive == "")
                                {
                                    textBox3.Text += Environment.NewLine + cmdreceive.ToString();
                                    if (cmdreceive.iden_F3 != null)
                                    {
                                        byte[] bs = cmdreceive.iden_F3.ImgContent;
                                        Image img = byteArrayToImage(bs);
                                        img.Save("temp.jpg");
                                    }
                                }
                                else
                                {
                                    textBox3.Text += Environment.NewLine + msgreceive;
                                }
                                #endregion
                            }
                            else if (AFN == (byte)WaterBaseProtocol.AFN._37)
                            {
                                #region _37
                                WaterCmd_37_2 cmdreceive = new WaterCmd_37_2();
                                cmdreceive.MsgList = MsgList;
                                string msgreceive = cmdreceive.ReadMsg();
                                if (msgreceive == "")
                                {
                                    textBox3.Text += Environment.NewLine + cmdreceive.ToString();
                                }
                                else
                                {
                                    textBox3.Text += Environment.NewLine + msgreceive;
                                }
                                #endregion
                            }
                            else if (AFN == (byte)WaterBaseProtocol.AFN._38)
                            {
                                #region _38
                                WaterCmd_38_2 cmdreceive = new WaterCmd_38_2();
                                cmdreceive.MsgList = MsgList;
                                string msgreceive = cmdreceive.ReadMsg();
                                if (msgreceive == "")
                                {
                                    textBox3.Text += Environment.NewLine + cmdreceive.ToString();
                                }
                                else
                                {
                                    textBox3.Text += Environment.NewLine + msgreceive;
                                }
                                #endregion
                            }
                            else if (AFN == (byte)WaterBaseProtocol.AFN._39)
                            {
                                #region _39
                                WaterCmd_39_2 cmdreceive = new WaterCmd_39_2();
                                cmdreceive.MsgList = MsgList;
                                string msgreceive = cmdreceive.ReadMsg();
                                if (msgreceive == "")
                                {
                                    textBox3.Text += Environment.NewLine + cmdreceive.ToString();
                                }
                                else
                                {
                                    textBox3.Text += Environment.NewLine + msgreceive;
                                }
                                #endregion
                            }
                            else if (AFN == (byte)WaterBaseProtocol.AFN._3A)
                            {
                                #region _3A
                                WaterCmd_3A_2 cmdreceive = new WaterCmd_3A_2();
                                cmdreceive.MsgList = MsgList;
                                string msgreceive = cmdreceive.ReadMsg();
                                if (msgreceive == "")
                                {
                                    textBox3.Text += Environment.NewLine + cmdreceive.ToString();
                                }
                                else
                                {
                                    textBox3.Text += Environment.NewLine + msgreceive;
                                }
                                #endregion
                            }
                            else if (AFN == (byte)WaterBaseProtocol.AFN._40)
                            {
                                #region _40
                                WaterCmd_40_2 cmdreceive = new WaterCmd_40_2();
                                cmdreceive.MsgList = MsgList;
                                string msgreceive = cmdreceive.ReadMsg();
                                if (msgreceive == "")
                                {
                                    textBox3.Text += Environment.NewLine + cmdreceive.ToString();
                                }
                                else
                                {
                                    textBox3.Text += Environment.NewLine + msgreceive;
                                }
                                #endregion
                            }
                            else if (AFN == (byte)WaterBaseProtocol.AFN._41)
                            {
                                #region _41
                                WaterCmd_41_2 cmdreceive = new WaterCmd_41_2();
                                cmdreceive.MsgList = MsgList;
                                string msgreceive = cmdreceive.ReadMsg();
                                if (msgreceive == "")
                                {
                                    textBox3.Text += Environment.NewLine + cmdreceive.ToString();
                                }
                                else
                                {
                                    textBox3.Text += Environment.NewLine + msgreceive;
                                }
                                #endregion
                            }
                            else if (AFN == (byte)WaterBaseProtocol.AFN._42)
                            {
                                #region _42
                                WaterCmd_42_2 cmdreceive = new WaterCmd_42_2();
                                cmdreceive.MsgList = MsgList;
                                string msgreceive = cmdreceive.ReadMsg();
                                if (msgreceive == "")
                                {
                                    textBox3.Text += Environment.NewLine + cmdreceive.ToString();
                                }
                                else
                                {
                                    textBox3.Text += Environment.NewLine + msgreceive;
                                }
                                #endregion
                            }
                            else if (AFN == (byte)WaterBaseProtocol.AFN._43)
                            {
                                #region _43
                                WaterCmd_43_2 cmdreceive = new WaterCmd_43_2();
                                cmdreceive.MsgList = MsgList;
                                string msgreceive = cmdreceive.ReadMsg();
                                if (msgreceive == "")
                                {
                                    textBox3.Text += Environment.NewLine + cmdreceive.ToString();
                                }
                                else
                                {
                                    textBox3.Text += Environment.NewLine + msgreceive;
                                }
                                #endregion
                            }
                            else if (AFN == (byte)WaterBaseProtocol.AFN._44)
                            {
                                #region _44
                                WaterCmd_44_2 cmdreceive = new WaterCmd_44_2();
                                cmdreceive.MsgList = MsgList;
                                string msgreceive = cmdreceive.ReadMsg();
                                if (msgreceive == "")
                                {
                                    textBox3.Text += Environment.NewLine + cmdreceive.ToString();
                                }
                                else
                                {
                                    textBox3.Text += Environment.NewLine + msgreceive;
                                }
                                #endregion
                            }
                            else if (AFN == (byte)WaterBaseProtocol.AFN._45)
                            {
                                #region _45
                                WaterCmd_45_2 cmdreceive = new WaterCmd_45_2();
                                cmdreceive.MsgList = MsgList;
                                string msgreceive = cmdreceive.ReadMsg();
                                if (msgreceive == "")
                                {
                                    textBox3.Text += Environment.NewLine + cmdreceive.ToString();
                                }
                                else
                                {
                                    textBox3.Text += Environment.NewLine + msgreceive;
                                }
                                #endregion
                            }
                            else if (AFN == (byte)WaterBaseProtocol.AFN._46)
                            {
                                #region _46
                                WaterCmd_46_2 cmdreceive = new WaterCmd_46_2();
                                cmdreceive.MsgList = MsgList;
                                string msgreceive = cmdreceive.ReadMsg();
                                if (msgreceive == "")
                                {
                                    textBox3.Text += Environment.NewLine + cmdreceive.ToString();
                                }
                                else
                                {
                                    textBox3.Text += Environment.NewLine + msgreceive;
                                }
                                #endregion
                            }
                            else if (AFN == (byte)WaterBaseProtocol.AFN._47)
                            {
                                #region _47
                                WaterCmd_47_2 cmdreceive = new WaterCmd_47_2();
                                cmdreceive.MsgList = MsgList;
                                string msgreceive = cmdreceive.ReadMsg();
                                if (msgreceive == "")
                                {
                                    textBox3.Text += Environment.NewLine + cmdreceive.ToString();
                                }
                                else
                                {
                                    textBox3.Text += Environment.NewLine + msgreceive;
                                }
                                #endregion
                            }
                            else if (AFN == (byte)WaterBaseProtocol.AFN._48)
                            {
                                #region _48
                                WaterCmd_48_2 cmdreceive = new WaterCmd_48_2();
                                cmdreceive.MsgList = MsgList;
                                string msgreceive = cmdreceive.ReadMsg();
                                if (msgreceive == "")
                                {
                                    textBox3.Text += Environment.NewLine + cmdreceive.ToString();
                                }
                                else
                                {
                                    textBox3.Text += Environment.NewLine + msgreceive;
                                }
                                #endregion
                            }
                            else if (AFN == (byte)WaterBaseProtocol.AFN._49)
                            {
                                #region _49
                                WaterCmd_49_2 cmdreceive = new WaterCmd_49_2();
                                cmdreceive.MsgList = MsgList;
                                string msgreceive = cmdreceive.ReadMsg();
                                if (msgreceive == "")
                                {
                                    textBox3.Text += Environment.NewLine + cmdreceive.ToString();
                                }
                                else
                                {
                                    textBox3.Text += Environment.NewLine + msgreceive;
                                }
                                #endregion
                            }
                            else if (AFN == (byte)WaterBaseProtocol.AFN._4A)
                            {
                                #region _4A
                                WaterCmd_4A_2 cmdreceive = new WaterCmd_4A_2();
                                cmdreceive.MsgList = MsgList;
                                string msgreceive = cmdreceive.ReadMsg();
                                if (msgreceive == "")
                                {
                                    textBox3.Text += Environment.NewLine + cmdreceive.ToString();
                                }
                                else
                                {
                                    textBox3.Text += Environment.NewLine + msgreceive;
                                }
                                #endregion
                            }
                            else if (AFN == (byte)WaterBaseProtocol.AFN._4B)
                            {
                                #region _4B
                                WaterCmd_4B_2 cmdreceive = new WaterCmd_4B_2();
                                cmdreceive.MsgList = MsgList;
                                string msgreceive = cmdreceive.ReadMsg();
                                if (msgreceive == "")
                                {
                                    textBox3.Text += Environment.NewLine + cmdreceive.ToString();
                                }
                                else
                                {
                                    textBox3.Text += Environment.NewLine + msgreceive;
                                }
                                #endregion
                            }
                            else if (AFN == (byte)WaterBaseProtocol.AFN._4C)
                            {
                                #region _4C
                                WaterCmd_4C_2 cmdreceive = new WaterCmd_4C_2();
                                cmdreceive.MsgList = MsgList;
                                string msgreceive = cmdreceive.ReadMsg();
                                if (msgreceive == "")
                                {
                                    textBox3.Text += Environment.NewLine + cmdreceive.ToString();
                                }
                                else
                                {
                                    textBox3.Text += Environment.NewLine + msgreceive;
                                }
                                #endregion
                            }
                            else if (AFN == (byte)WaterBaseProtocol.AFN._4D)
                            {
                                #region _4D
                                WaterCmd_4D_2 cmdreceive = new WaterCmd_4D_2();
                                cmdreceive.MsgList = MsgList;
                                string msgreceive = cmdreceive.ReadMsg();
                                if (msgreceive == "")
                                {
                                    textBox3.Text += Environment.NewLine + cmdreceive.ToString();
                                }
                                else
                                {
                                    textBox3.Text += Environment.NewLine + msgreceive;
                                }
                                #endregion
                            }
                            else if (AFN == (byte)WaterBaseProtocol.AFN._4E)
                            {
                                #region _4E
                                WaterCmd_4E_2 cmdreceive = new WaterCmd_4E_2();
                                cmdreceive.MsgList = MsgList;
                                string msgreceive = cmdreceive.ReadMsg();
                                if (msgreceive == "")
                                {
                                    textBox3.Text += Environment.NewLine + cmdreceive.ToString();
                                }
                                else
                                {
                                    textBox3.Text += Environment.NewLine + msgreceive;
                                }
                                #endregion
                            }
                            else if (AFN == (byte)WaterBaseProtocol.AFN._4F)
                            {
                                #region _4F
                                WaterCmd_4F_2 cmdreceive = new WaterCmd_4F_2();
                                cmdreceive.MsgList = MsgList;
                                string msgreceive = cmdreceive.ReadMsg();
                                if (msgreceive == "")
                                {
                                    textBox3.Text += Environment.NewLine + cmdreceive.ToString();
                                }
                                else
                                {
                                    textBox3.Text += Environment.NewLine + msgreceive;
                                }
                                #endregion
                            }
                            else if (AFN == (byte)WaterBaseProtocol.AFN._50)
                            {
                                #region _50
                                WaterCmd_50_2 cmdreceive = new WaterCmd_50_2();
                                cmdreceive.MsgList = MsgList;
                                string msgreceive = cmdreceive.ReadMsg();
                                if (msgreceive == "")
                                {
                                    textBox3.Text += Environment.NewLine + cmdreceive.ToString();
                                }
                                else
                                {
                                    textBox3.Text += Environment.NewLine + msgreceive;
                                }
                                #endregion
                            }
                            else if (AFN == (byte)WaterBaseProtocol.AFN._51)
                            {
                                #region _51
                                WaterCmd_51_2 cmdreceive = new WaterCmd_51_2();
                                cmdreceive.MsgList = MsgList;
                                string msgreceive = cmdreceive.ReadMsg();
                                if (msgreceive == "")
                                {
                                    textBox3.Text += Environment.NewLine + cmdreceive.ToString();
                                }
                                else
                                {
                                    textBox3.Text += Environment.NewLine + msgreceive;
                                }
                                #endregion
                            }
                            else
                            {
                                textBox3.Text += Environment.NewLine + "【未知标识AFN：" + AFN + "，无法解析】";
                            }

                            MsgList = null;
                            WaterDeviceService.Remove(cKey);

                            #endregion
                        }
                        else
                        {
                            #region 下行数据
                            textBox3.Text = "【数据传输完成，不可以解析，继续接收】";

                            #endregion
                        }

                        #endregion
                    }
                    else
                    {
                        #region 下行数据
                        textBox3.Text = "【下行数据】【" +
                            EnumUtils.GetDescription(typeof(WaterBaseProtocol.DataEndChar_Up), DataEndChar) + "】【" +
                            EnumUtils.GetDescription(typeof(WaterBaseProtocol.DataBeginChar), DataBeginChar) + "】";
                        #endregion
                    }
                }
                else
                {
                    if (message_water.IsWaterMessage)
                    {
                        textBox3.Text = "【解析出错】" + Environment.NewLine + "数据为地下水，无法解析。" + Environment.NewLine + msg;
                    }
                    else
                    {
                        textBox3.Text = "【解析出错】" + Environment.NewLine + "数据不为地下水，无法解析。" + Environment.NewLine + msg;
                    }
                }
            }
            catch (Exception ex)
            {
                textBox3.Text = "【解析出错】" + Environment.NewLine + ex.Message;
            }
        }

        private Image byteArrayToImage(byte[] Bytes)
        {
            MemoryStream ms = new MemoryStream(Bytes);
            return Bitmap.FromStream(ms, true);
        }
    }
}
