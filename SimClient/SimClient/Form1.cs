using DTU.GateWay.Protocol;
using DTU.GateWay.Protocol.WaterMessageClass;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private Socket clientSocket = null;
        private Thread thread = null;
        private bool isConnect = false;

        private void Form1_Load(object sender, EventArgs e)
        {
            setBtnEnabled(isConnect);

            this.txtCenterStation.Text = WaterBaseProtocol.CenterStation.ToString();
            this.txtPW.Text = WaterBaseProtocol.PW.ToString();
        }

        public void setBtnEnabled(bool isConnect)
        {
            setEnabled(button1, !isConnect);
            setEnabled(button2, isConnect);
            setEnabled(button3, isConnect);
        }

        public delegate void SetTextHandler(string result);
        private void SetCalResult(string result)
        {
            if (txtLog.InvokeRequired == true)
            {
                SetTextHandler set = new SetTextHandler(SetCalResult);//委托的方法参数应和SetCalResult一致  
                txtLog.Invoke(set, new object[] { result }); //此方法第二参数用于传入方法,代替形参result  
            }
            else
            {
                string str = "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "] " + result + Environment.NewLine + this.txtLog.Text;
                this.txtLog.Text = str;
            }
        }

        public delegate void SetTextHandler2(TextBox txt, string result);
        private void SetCalResult2(TextBox txt, string result)
        {
            if (txt.InvokeRequired == true)
            {
                SetTextHandler2 set = new SetTextHandler2(SetCalResult2);//委托的方法参数应和SetCalResult一致  
                txt.Invoke(set, new object[] { txt, result }); //此方法第二参数用于传入方法,代替形参result  
            }
            else
            {
                txt.Text = result;
            }
        }

        public delegate void SetTextClearHandler();
        private void SetCalClearResult()
        {
            if (txtLog.InvokeRequired == true)
            {
                SetTextClearHandler set = new SetTextClearHandler(SetCalClearResult);//委托的方法参数应和SetCalResult一致  
                txtLog.Invoke(set, new object[] { }); //此方法第二参数用于传入方法,代替形参result  
            }
            else
            {
                this.txtLog.Text = "";
            }
        }

        public delegate void setEnabledHandler(Button obj, bool flag);
        private void setEnabled(Button obj, bool flag)
        {
            if (obj.InvokeRequired == true)
            {
                setEnabledHandler set = new setEnabledHandler(setEnabled);//委托的方法参数应和SetCalResult一致  
                obj.Invoke(set, new object[] { obj, flag }); //此方法第二参数用于传入方法,代替形参result  
            }
            else
            {
                obj.Enabled = flag;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                SetCalResult("开始连接 " + this.txtIP.Text.Trim() + " " + this.txtPort.Text.Trim());
                clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress ip = IPAddress.Parse(this.txtIP.Text.Trim());
                IPEndPoint endPoint = new IPEndPoint(ip, int.Parse(this.txtPort.Text.Trim()));
                clientSocket.Connect(endPoint);
                SetCalResult("打开连接成功 " + this.txtIP.Text.Trim() + " " + this.txtPort.Text.Trim());
                isConnect = true;

                Thread tXT = new Thread(XT);
                tXT.Start();
            }
            catch (Exception ex)
            {
                SetCalResult("打开连接失败");
                SetCalResult(ex.Message);
                return;
            }

            if (clientSocket != null)
            {
                try
                {
                    thread = new Thread(ReceMsg);
                    thread.IsBackground = true;
                    thread.Start();

                    SetCalResult("接收数据线程启动成功");
                    setBtnEnabled(true);
                }
                catch (Exception ex)
                {
                    CloseSocket();
                    SetCalResult("接收数据线程启动失败");
                    SetCalResult(ex.Message);
                }
            }
        }

        short count = 1;
        private void ReceMsg()
        {
            while (isConnect)
            {
                try
                {
                    byte[] buffer = new byte[1024 * 4];
                    clientSocket.Receive(buffer);
                    string recvMsg = HexStringUtility.ByteArrayToHexString(buffer).TrimEnd('0');
                    if (recvMsg.Length % 2 == 1)
                    {
                        recvMsg += "0";
                    }
                    if (recvMsg != "")
                    {
                        SetCalResult("数据接收：" + recvMsg);

                        WaterBaseMessage message = new WaterBaseMessage();
                        message.RawDataStr = recvMsg;
                        message.RawDataChar = HexStringUtility.HexStringToByteArray(recvMsg);
                        string msg = message.ReadMsgBase();
                        if (msg == "")
                        {
                            if (message.UpOrDown == (int)WaterBaseProtocol.UpOrDown.Down)
                            {
                                if (message.DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Down.ENQ)
                                {
                                    SetCalResult("数据接收：下行查询与控制");
                                    if (message.AFN == (byte)WaterBaseProtocol.AFN._37)
                                    {
                                        #region _37
                                        SetCalResult("数据接收：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), message.AFN));
                                        count++;
                                        if (count == 0)
                                        {
                                            count = 1;
                                        }
                                        WaterCmd_37_2 cmdres = new WaterCmd_37_2();
                                        cmdres.CenterStation = message.CenterStation;
                                        cmdres.RemoteStation = message.RemoteStation;
                                        cmdres.PW = message.PW;

                                        cmdres.SerialNumber = count;
                                        cmdres.SendTime = DateTime.Now;
                                        cmdres.List_Identifier = new List<Identifier>();

                                        Identifier_F1 Iden_F1 = new Identifier_F1();
                                        Iden_F1.RemoteStation = message.RemoteStation;
                                        Iden_F1.StationType = 0x46;
                                        cmdres.List_Identifier.Add(Iden_F1);
                                        Identifier_F0 Iden_F0 = new Identifier_F0();
                                        Iden_F0.ObsTime = DateTime.Now;
                                        cmdres.List_Identifier.Add(Iden_F0);

                                        string msgres = cmdres.WriteMsg();
                                        if (msgres == "")
                                        {
                                            WaterBaseMessage[] list = cmdres.MsgList;
                                            foreach (WaterBaseMessage wbm in list)
                                            {
                                                if (wbm.RawDataStr != null)
                                                {
                                                    SendMsg(wbm.RawDataStr);
                                                }
                                                else
                                                {
                                                    SetCalResult("数据发送：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), message.AFN) + " 失败");
                                                }
                                            }
                                        }
                                        else
                                        {
                                            SetCalResult("数据发送：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), message.AFN) + " 失败：" + msgres);
                                        }
                                        #endregion
                                    }
                                    else if (message.AFN == (byte)WaterBaseProtocol.AFN._38)
                                    {
                                        #region _38
                                        SetCalResult("数据接收：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), message.AFN));
                                        WaterCmd_38_1 cmdreceive = new WaterCmd_38_1();
                                        cmdreceive.UserDataBytes = message.UserDataBytes;
                                        cmdreceive.UserData = message.UserData;
                                        string msgreceive = cmdreceive.ReadMsg();
                                        if (msgreceive != "")
                                        {
                                            SetCalResult("数据接收：" + msgreceive);
                                        }
                                        else
                                        {
                                            count++;
                                            if (count == 0)
                                            {
                                                count = 1;
                                            }
                                            WaterCmd_38_2 cmdres = new WaterCmd_38_2();
                                            cmdres.CenterStation = message.CenterStation;
                                            cmdres.RemoteStation = message.RemoteStation;
                                            cmdres.PW = message.PW;

                                            cmdres.SerialNumber = count;
                                            cmdres.SendTime = DateTime.Now;
                                            cmdres.Iden_F1 = new Identifier_F1();
                                            cmdres.Iden_F1.RemoteStation = message.RemoteStation;
                                            cmdres.Iden_F1.StationType = 0x46;
                                            cmdres.Iden_F0 = new Identifier_F0();
                                            cmdres.Iden_F0.ObsTime = DateTime.Now;
                                            cmdres.Iden_04 = cmdreceive.Iden_04;
                                            cmdres.Idens = new List<Identifier>();
                                            for (int i = 0; i < 10; i++)
                                            {
                                                byte key = cmdreceive.Iden.GetKey();
                                                if (key == (byte)Identifier_Standard._FF)
                                                {
                                                    byte keySub = ((Identifier_FF)cmdreceive.Iden).GetKeySub();
                                                }
                                                else
                                                {
                                                }
                                            }

                                            string msgres = cmdres.WriteMsg();
                                            if (msgres == "")
                                            {
                                                WaterBaseMessage[] list = cmdres.MsgList;
                                                foreach (WaterBaseMessage wbm in list)
                                                {
                                                    if (wbm.RawDataStr != null)
                                                    {
                                                        SendMsg(wbm.RawDataStr);
                                                    }
                                                    else
                                                    {
                                                        SetCalResult("数据发送：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), message.AFN) + " 失败");
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                SetCalResult("数据发送：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), message.AFN) + " 失败：" + msgres);
                                            }
                                        }
                                        #endregion
                                    }
                                    else if (message.AFN == (byte)WaterBaseProtocol.AFN._39)
                                    {
                                        #region _39
                                        SetCalResult("数据接收：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), message.AFN));
                                        WaterCmd_39_1 cmdreceive = new WaterCmd_39_1();
                                        cmdreceive.UserDataBytes = message.UserDataBytes;
                                        cmdreceive.UserData = message.UserData;
                                        string msgreceive = cmdreceive.ReadMsg();
                                        if (msgreceive != "")
                                        {
                                            SetCalResult("数据接收：" + msgreceive);
                                        }
                                        else
                                        {
                                            count++;
                                            if (count == 0)
                                            {
                                                count = 1;
                                            }
                                            WaterCmd_39_2 cmdres = new WaterCmd_39_2();
                                            cmdres.CenterStation = message.CenterStation;
                                            cmdres.RemoteStation = message.RemoteStation;
                                            cmdres.PW = message.PW;

                                            cmdres.SerialNumber = count;
                                            cmdres.SendTime = DateTime.Now;
                                            cmdres.iden_F2 = new Identifier_F2();
                                            cmdres.iden_F2.Content = "51203233342E3536";

                                            string msgres = cmdres.WriteMsg();
                                            if (msgres == "")
                                            {
                                                WaterBaseMessage[] list = cmdres.MsgList;
                                                foreach (WaterBaseMessage wbm in list)
                                                {
                                                    if (wbm.RawDataStr != null)
                                                    {
                                                        SendMsg(wbm.RawDataStr);
                                                    }
                                                    else
                                                    {
                                                        SetCalResult("数据发送：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), message.AFN) + " 失败");
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                SetCalResult("数据发送：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), message.AFN) + " 失败：" + msgres);
                                            }
                                        }
                                        #endregion
                                    }
                                    else if (message.AFN == (byte)WaterBaseProtocol.AFN._3A)
                                    {
                                        #region _3A
                                        SetCalResult("数据接收：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), message.AFN));
                                        WaterCmd_3A_1 cmdreceive = new WaterCmd_3A_1();
                                        cmdreceive.UserDataBytes = message.UserDataBytes;
                                        cmdreceive.UserData = message.UserData;
                                        string msgreceive = cmdreceive.ReadMsg();
                                        if (msgreceive != "")
                                        {
                                            SetCalResult("数据接收：" + msgreceive);
                                        }
                                        else
                                        {
                                            count++;
                                            if (count == 0)
                                            {
                                                count = 1;
                                            }
                                            WaterCmd_3A_2 cmdres = new WaterCmd_3A_2();
                                            cmdres.CenterStation = message.CenterStation;
                                            cmdres.RemoteStation = message.RemoteStation;
                                            cmdres.PW = message.PW;

                                            cmdres.SerialNumber = count;
                                            cmdres.SendTime = DateTime.Now;
                                            cmdres.List_Identifier = cmdreceive.List_Identifier;
                                            foreach (Identifier iden in cmdres.List_Identifier)
                                            {
                                            }
                                            string msgres = cmdres.WriteMsg();
                                            if (msgres == "")
                                            {
                                                WaterBaseMessage[] list = cmdres.MsgList;
                                                foreach (WaterBaseMessage wbm in list)
                                                {
                                                    if (wbm.RawDataStr != null)
                                                    {
                                                        SendMsg(wbm.RawDataStr);
                                                    }
                                                    else
                                                    {
                                                        SetCalResult("数据发送：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), message.AFN) + " 失败");
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                SetCalResult("数据发送：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), message.AFN) + " 失败：" + msgres);
                                            }
                                        }
                                        #endregion
                                    }
                                    else if (message.AFN == (byte)WaterBaseProtocol.AFN._40)
                                    {
                                        #region _40
                                        SetCalResult("数据接收：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), message.AFN));
                                        WaterCmd_40_1 cmdreceive = new WaterCmd_40_1();
                                        cmdreceive.UserDataBytes = message.UserDataBytes;
                                        cmdreceive.UserData = message.UserData;
                                        string msgreceive = cmdreceive.ReadMsg();
                                        if (msgreceive != "")
                                        {
                                            SetCalResult("数据接收：" + msgreceive);
                                        }
                                        else
                                        {
                                            count++;
                                            if (count == 0)
                                            {
                                                count = 1;
                                            }

                                            WaterCmd_40_2 cmdres = new WaterCmd_40_2();
                                            cmdres.CenterStation = message.CenterStation;
                                            cmdres.RemoteStation = message.RemoteStation;
                                            cmdres.PW = message.PW;

                                            cmdres.SerialNumber = count;
                                            cmdres.SendTime = DateTime.Now;
                                            cmdres.Iden_F1 = new Identifier_F1();
                                            cmdres.Iden_F1.RemoteStation = message.RemoteStation;
                                            cmdres.List_RTUParam = cmdreceive.List_RTUParam;

                                            string msgres = cmdres.WriteMsg();
                                            if (msgres == "")
                                            {
                                                SetCalResult(cmdres.ToString());
                                                WaterBaseMessage[] list = cmdres.MsgList;
                                                foreach (WaterBaseMessage wbm in list)
                                                {
                                                    if (wbm.RawDataStr != null)
                                                    {
                                                        SendMsg(wbm.RawDataStr);
                                                    }
                                                    else
                                                    {
                                                        SetCalResult("数据发送：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), message.AFN) + " 失败");
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                SetCalResult("数据发送：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), message.AFN) + " 失败：" + msgres);
                                            }
                                        }
                                        #endregion
                                    }
                                    else if (message.AFN == (byte)WaterBaseProtocol.AFN._41)
                                    {
                                        #region _41
                                        SetCalResult("数据接收：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), message.AFN));
                                        WaterCmd_41_1 cmdreceive = new WaterCmd_41_1();
                                        cmdreceive.UserDataBytes = message.UserDataBytes;
                                        cmdreceive.UserData = message.UserData;
                                        string msgreceive = cmdreceive.ReadMsg();
                                        if (msgreceive != "")
                                        {
                                            SetCalResult("数据接收：" + msgreceive);
                                        }
                                        else
                                        {
                                            count++;
                                            if (count == 0)
                                            {
                                                count = 1;
                                            }

                                            WaterCmd_41_2 cmdres = new WaterCmd_41_2();
                                            cmdres.CenterStation = message.CenterStation;
                                            cmdres.RemoteStation = message.RemoteStation;
                                            cmdres.PW = message.PW;

                                            cmdres.SerialNumber = count;
                                            cmdres.SendTime = DateTime.Now;
                                            cmdres.Iden_F1 = new Identifier_F1();
                                            cmdres.Iden_F1.RemoteStation = message.RemoteStation;
                                            cmdres.List_RTUParam = cmdreceive.List_RTUParam;
                                            foreach (RTUParam rp in cmdres.List_RTUParam)
                                            {
                                                byte Key = rp.GetKey();
                                                string LengthStr = rp.GetLength().ToString("X");
                                                int[] LS = WaterBaseProtocol.GetLengthList(LengthStr);
                                                int ByteCount = LS[0];
                                                int Digits = LS[1];
                                                if (Key == (byte)RTUParam.RTUParamKey._01)
                                                {
                                                    rp.SetVal(0, 0, "02000000");
                                                }
                                                else if (Key == (byte)RTUParam.RTUParamKey._02)
                                                {
                                                    rp.SetVal(0, 0, "0012345678");
                                                }
                                                else if (Key == (byte)RTUParam.RTUParamKey._03)
                                                {
                                                    rp.SetVal(0, 0, "ABCD");
                                                }
                                                else if (Key == (byte)RTUParam.RTUParamKey._04)
                                                {
                                                    rp.SetVal(0, 0, "02192168001002005000");
                                                }
                                                else if (Key == (byte)RTUParam.RTUParamKey._05)
                                                {
                                                    rp.SetVal(0, 0, "03141123");
                                                }
                                                else if (Key == (byte)RTUParam.RTUParamKey._06)
                                                {
                                                    ((RTUParam_06)rp).ChannelTypeV = 0;
                                                }
                                                else if (Key == (byte)RTUParam.RTUParamKey._07)
                                                {
                                                    ((RTUParam_07)rp).ChannelTypeV = 0;
                                                }
                                                else if (Key == (byte)RTUParam.RTUParamKey._08)
                                                {
                                                    ((RTUParam_08)rp).ChannelTypeV = 0;
                                                }
                                                else if (Key == (byte)RTUParam.RTUParamKey._09)
                                                {
                                                    ((RTUParam_09)rp).ChannelTypeV = 0;
                                                }
                                                else if (Key == (byte)RTUParam.RTUParamKey._0A)
                                                {
                                                    ((RTUParam_0A)rp).ChannelTypeV = 0;
                                                }
                                                else if (Key == (byte)RTUParam.RTUParamKey._0B)
                                                {
                                                    ((RTUParam_0B)rp).ChannelTypeV = 0;
                                                }
                                                else if (Key == (byte)RTUParam.RTUParamKey._0C)
                                                {
                                                    rp.SetVal(0, 0, "02");
                                                }
                                                else if (Key == (byte)RTUParam.RTUParamKey._0D)
                                                {
                                                    rp.SetVal(0, 0, "8001000000000000");
                                                }
                                                else if (Key == (byte)RTUParam.RTUParamKey._0F)
                                                {
                                                    rp.SetVal(0, 0, "31303133303132333435363738");
                                                }
                                            }
                                            string msgres = cmdres.WriteMsg();
                                            if (msgres == "")
                                            {
                                                SetCalResult(cmdres.ToString());
                                                WaterBaseMessage[] list = cmdres.MsgList;
                                                foreach (WaterBaseMessage wbm in list)
                                                {
                                                    if (wbm.RawDataStr != null)
                                                    {
                                                        SendMsg(wbm.RawDataStr);
                                                    }
                                                    else
                                                    {
                                                        SetCalResult("数据发送：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), message.AFN) + " 失败");
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                SetCalResult("数据发送：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), message.AFN) + " 失败：" + msgres);
                                            }
                                        }
                                        #endregion
                                    }
                                    else if (message.AFN == (byte)WaterBaseProtocol.AFN._42)
                                    {
                                        #region _42
                                        SetCalResult("数据接收：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), message.AFN));
                                        WaterCmd_42_1 cmdreceive = new WaterCmd_42_1();
                                        cmdreceive.UserDataBytes = message.UserDataBytes;
                                        cmdreceive.UserData = message.UserData;
                                        string msgreceive = cmdreceive.ReadMsg();
                                        if (msgreceive != "")
                                        {
                                            SetCalResult("数据接收：" + msgreceive);
                                        }
                                        else
                                        {
                                            count++;
                                            if (count == 0)
                                            {
                                                count = 1;
                                            }

                                            WaterCmd_42_2 cmdres = new WaterCmd_42_2();
                                            cmdres.CenterStation = message.CenterStation;
                                            cmdres.RemoteStation = message.RemoteStation;
                                            cmdres.PW = message.PW;

                                            cmdres.SerialNumber = count;
                                            cmdres.SendTime = DateTime.Now;
                                            cmdres.Iden_F1 = new Identifier_F1();
                                            cmdres.Iden_F1.RemoteStation = message.RemoteStation;
                                            cmdres.List_RTUParam = cmdreceive.List_RTUParam;

                                            string msgres = cmdres.WriteMsg();
                                            if (msgres == "")
                                            {
                                                SetCalResult(cmdres.ToString());
                                                WaterBaseMessage[] list = cmdres.MsgList;
                                                foreach (WaterBaseMessage wbm in list)
                                                {
                                                    if (wbm.RawDataStr != null)
                                                    {
                                                        SendMsg(wbm.RawDataStr);
                                                    }
                                                    else
                                                    {
                                                        SetCalResult("数据发送：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), message.AFN) + " 失败");
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                SetCalResult("数据发送：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), message.AFN) + " 失败：" + msgres);
                                            }
                                        }
                                        #endregion
                                    }
                                    else if (message.AFN == (byte)WaterBaseProtocol.AFN._43)
                                    {
                                        #region _43
                                        SetCalResult("数据接收：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), message.AFN));
                                        WaterCmd_43_1 cmdreceive = new WaterCmd_43_1();
                                        cmdreceive.UserDataBytes = message.UserDataBytes;
                                        cmdreceive.UserData = message.UserData;
                                        string msgreceive = cmdreceive.ReadMsg();
                                        if (msgreceive != "")
                                        {
                                            SetCalResult("数据接收：" + msgreceive);
                                        }
                                        else
                                        {
                                            count++;
                                            if (count == 0)
                                            {
                                                count = 1;
                                            }

                                            WaterCmd_43_2 cmdres = new WaterCmd_43_2();
                                            cmdres.CenterStation = message.CenterStation;
                                            cmdres.RemoteStation = message.RemoteStation;
                                            cmdres.PW = message.PW;

                                            cmdres.SerialNumber = count;
                                            cmdres.SendTime = DateTime.Now;
                                            cmdres.Iden_F1 = new Identifier_F1();
                                            cmdres.Iden_F1.RemoteStation = message.RemoteStation;
                                            cmdres.List_RTUParam = cmdreceive.List_RTUParam;
                                            foreach (RTUParam rp in cmdres.List_RTUParam)
                                            {
                                                byte Key = rp.GetKey();
                                                string LengthStr = rp.GetLength().ToString("X");
                                                int[] LS = WaterBaseProtocol.GetLengthList(LengthStr);
                                                int ByteCount = LS[0];
                                                int Digits = LS[1];
                                                if (Key == (byte)RTUParam.RTUParamKey._20)
                                                {
                                                    rp.SetVal(0, 0, (1 * Math.Pow(10, Digits)).ToString());
                                                }
                                                else if (Key == (byte)RTUParam.RTUParamKey._21)
                                                {
                                                    rp.SetVal(0, 0, (5 * Math.Pow(10, Digits)).ToString());
                                                }
                                                else if (Key == (byte)RTUParam.RTUParamKey._22)
                                                {
                                                    rp.SetVal(0, 0, (8 * Math.Pow(10, Digits)).ToString());
                                                }
                                                else if (Key == (byte)RTUParam.RTUParamKey._23)
                                                {
                                                    rp.SetVal(0, 0, (300 * Math.Pow(10, Digits)).ToString());
                                                }
                                                else if (Key == (byte)RTUParam.RTUParamKey._24)
                                                {
                                                    rp.SetVal(0, 0, (5 * Math.Pow(10, Digits)).ToString());
                                                }
                                                else if (Key == (byte)RTUParam.RTUParamKey._25)
                                                {
                                                    rp.SetVal(0, 0, (1 * Math.Pow(10, Digits)).ToString());
                                                }
                                                else if (Key == (byte)RTUParam.RTUParamKey._26)
                                                {
                                                    rp.SetVal(0, 0, (1 * Math.Pow(10, Digits)).ToString());
                                                }
                                                else if (Key == (byte)RTUParam.RTUParamKey._27)
                                                {
                                                    rp.SetVal(0, 0, (2 * Math.Pow(10, Digits)).ToString());
                                                }
                                                else if (Key == (byte)RTUParam.RTUParamKey._28)
                                                {
                                                    rp.SetVal(0, 0, (10 * Math.Pow(10, Digits)).ToString());
                                                }
                                                else if (Key == (byte)RTUParam.RTUParamKey._2A)
                                                {
                                                    rp.SetVal(0, 0, (5 * Math.Pow(10, Digits)).ToString());
                                                }
                                                else if (Key == (byte)RTUParam.RTUParamKey._2B)
                                                {
                                                    rp.SetVal(0, 0, (5 * Math.Pow(10, Digits)).ToString());
                                                }
                                                else if (Key == (byte)RTUParam.RTUParamKey._2C)
                                                {
                                                    rp.SetVal(0, 0, (5 * Math.Pow(10, Digits)).ToString());
                                                }
                                                else if (Key == (byte)RTUParam.RTUParamKey._2D)
                                                {
                                                    rp.SetVal(0, 0, (5 * Math.Pow(10, Digits)).ToString());
                                                }
                                                else if (Key == (byte)RTUParam.RTUParamKey._2E)
                                                {
                                                    rp.SetVal(0, 0, (5 * Math.Pow(10, Digits)).ToString());
                                                }
                                                else if (Key == (byte)RTUParam.RTUParamKey._2F)
                                                {
                                                    rp.SetVal(0, 0, (5 * Math.Pow(10, Digits)).ToString());
                                                }
                                                else if (Key == (byte)RTUParam.RTUParamKey._30)
                                                {
                                                    rp.SetVal(0, 0, (1 * Math.Pow(10, Digits)).ToString());
                                                }
                                                else if (Key == (byte)RTUParam.RTUParamKey._31)
                                                {
                                                    rp.SetVal(0, 0, (5 * Math.Pow(10, Digits)).ToString());
                                                }
                                                else if (Key == (byte)RTUParam.RTUParamKey._32)
                                                {
                                                    rp.SetVal(0, 0, (5 * Math.Pow(10, Digits)).ToString());
                                                }
                                                else if (Key == (byte)RTUParam.RTUParamKey._33)
                                                {
                                                    rp.SetVal(0, 0, (5 * Math.Pow(10, Digits)).ToString());
                                                }
                                                else if (Key == (byte)RTUParam.RTUParamKey._34)
                                                {
                                                    rp.SetVal(0, 0, (5 * Math.Pow(10, Digits)).ToString());
                                                }
                                                else if (Key == (byte)RTUParam.RTUParamKey._35)
                                                {
                                                    rp.SetVal(0, 0, (5 * Math.Pow(10, Digits)).ToString());
                                                }
                                                else if (Key == (byte)RTUParam.RTUParamKey._36)
                                                {
                                                    rp.SetVal(0, 0, (5 * Math.Pow(10, Digits)).ToString());
                                                }
                                                else if (Key == (byte)RTUParam.RTUParamKey._37)
                                                {
                                                    rp.SetVal(0, 0, (5 * Math.Pow(10, Digits)).ToString());
                                                }
                                                else if (Key == (byte)RTUParam.RTUParamKey._38)
                                                {
                                                    rp.SetVal(0, 0, (5 * Math.Pow(10, Digits)).ToString());
                                                }
                                                else if (Key == (byte)RTUParam.RTUParamKey._39)
                                                {
                                                    rp.SetVal(0, 0, (5 * Math.Pow(10, Digits)).ToString());
                                                }
                                                else if (Key == (byte)RTUParam.RTUParamKey._3A)
                                                {
                                                    rp.SetVal(0, 0, (5 * Math.Pow(10, Digits)).ToString());
                                                }
                                                else if (Key == (byte)RTUParam.RTUParamKey._3B)
                                                {
                                                    rp.SetVal(0, 0, (5 * Math.Pow(10, Digits)).ToString());
                                                }
                                                else if (Key == (byte)RTUParam.RTUParamKey._3C)
                                                {
                                                    rp.SetVal(0, 0, (5 * Math.Pow(10, Digits)).ToString());
                                                }
                                                else if (Key == (byte)RTUParam.RTUParamKey._3D)
                                                {
                                                    rp.SetVal(0, 0, (5 * Math.Pow(10, Digits)).ToString());
                                                }
                                                else if (Key == (byte)RTUParam.RTUParamKey._3E)
                                                {
                                                    rp.SetVal(0, 0, (5 * Math.Pow(10, Digits)).ToString());
                                                }
                                                else if (Key == (byte)RTUParam.RTUParamKey._3F)
                                                {
                                                    rp.SetVal(0, 0, (5 * Math.Pow(10, Digits)).ToString());
                                                }
                                                else if (Key == (byte)RTUParam.RTUParamKey._40)
                                                {
                                                    rp.SetVal(0, 0, (0.02 * Math.Pow(10, Digits)).ToString());
                                                }
                                                else if (Key == (byte)RTUParam.RTUParamKey._41)
                                                {
                                                    rp.SetVal(0, 0, (0.3 * Math.Pow(10, Digits)).ToString());
                                                }
                                            }
                                            string msgres = cmdres.WriteMsg();
                                            if (msgres == "")
                                            {
                                                SetCalResult(cmdres.ToString());
                                                WaterBaseMessage[] list = cmdres.MsgList;
                                                foreach (WaterBaseMessage wbm in list)
                                                {
                                                    if (wbm.RawDataStr != null)
                                                    {
                                                        SendMsg(wbm.RawDataStr);
                                                    }
                                                    else
                                                    {
                                                        SetCalResult("数据发送：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), message.AFN) + " 失败");
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                SetCalResult("数据发送：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), message.AFN) + " 失败：" + msgres);
                                            }
                                        }
                                        #endregion
                                    }
                                    else if (message.AFN == (byte)WaterBaseProtocol.AFN._20)
                                    {
                                        #region _20
                                        SetCalResult("数据接收：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), message.AFN));
                                        WaterCmd_20_1 cmdreceive = new WaterCmd_20_1();
                                        cmdreceive.UserDataBytes = message.UserDataBytes;
                                        cmdreceive.UserData = message.UserData;
                                        string msgreceive = cmdreceive.ReadMsg();
                                        if (msgreceive != "")
                                        {
                                            SetCalResult("数据接收：" + msgreceive);
                                        }
                                        else
                                        {
                                            SetCalResult2(txtAlarm_01_1, cmdreceive.Values[0].ToString());
                                            SetCalResult2(txtAlarm_01_2, cmdreceive.Values[1].ToString());
                                            SetCalResult2(txtAlarm_01_3, cmdreceive.Values[2].ToString());
                                            SetCalResult2(txtAlarm_03_1, cmdreceive.Values[3].ToString());
                                            SetCalResult2(txtAlarm_03_2, cmdreceive.Values[4].ToString());
                                            SetCalResult2(txtAlarm_03_3, cmdreceive.Values[5].ToString());
                                            SetCalResult2(txtAlarm_06_1, cmdreceive.Values[6].ToString());
                                            SetCalResult2(txtAlarm_06_2, cmdreceive.Values[7].ToString());
                                            SetCalResult2(txtAlarm_06_3, cmdreceive.Values[8].ToString());
                                            SetCalResult2(txtAlarm_12_1, cmdreceive.Values[9].ToString());
                                            SetCalResult2(txtAlarm_12_2, cmdreceive.Values[10].ToString());
                                            SetCalResult2(txtAlarm_12_3, cmdreceive.Values[11].ToString());

                                            count++;
                                            if (count == 0)
                                            {
                                                count = 1;
                                            }
                                            WaterCmd_20_2 cmdres = new WaterCmd_20_2();
                                            cmdres.CenterStation = message.CenterStation;
                                            cmdres.RemoteStation = message.RemoteStation;
                                            cmdres.PW = message.PW;

                                            cmdres.SerialNumber = count;
                                            cmdres.SendTime = DateTime.Now;
                                            cmdres.Result = 1;

                                            string msgres = cmdres.WriteMsg();
                                            if (msgres == "")
                                            {
                                                WaterBaseMessage[] list = cmdres.MsgList;
                                                foreach (WaterBaseMessage wbm in list)
                                                {
                                                    if (wbm.RawDataStr != null)
                                                    {
                                                        SendMsg(wbm.RawDataStr);
                                                    }
                                                    else
                                                    {
                                                        SetCalResult("数据发送：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), message.AFN) + " 失败");
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                SetCalResult("数据发送：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), message.AFN) + " 失败：" + msgres);
                                            }
                                        }
                                        #endregion
                                    }
                                    else if (message.AFN == (byte)WaterBaseProtocol.AFN._21)
                                    {
                                        #region _21
                                        SetCalResult("数据接收：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), message.AFN));
                                        WaterCmd_21_1 cmdreceive = new WaterCmd_21_1();
                                        cmdreceive.UserDataBytes = message.UserDataBytes;
                                        cmdreceive.UserData = message.UserData;
                                        string msgreceive = cmdreceive.ReadMsg();
                                        if (msgreceive != "")
                                        {
                                            SetCalResult("数据接收：" + msgreceive);
                                        }
                                        else
                                        {
                                            count++;
                                            if (count == 0)
                                            {
                                                count = 1;
                                            }
                                            WaterCmd_21_2 cmdres = new WaterCmd_21_2();
                                            cmdres.CenterStation = message.CenterStation;
                                            cmdres.RemoteStation = message.RemoteStation;
                                            cmdres.PW = message.PW;

                                            cmdres.SerialNumber = count;
                                            cmdres.SendTime = DateTime.Now;
                                            cmdres.Result = 1;

                                            string msgres = cmdres.WriteMsg();
                                            if (msgres == "")
                                            {
                                                WaterBaseMessage[] list = cmdres.MsgList;
                                                foreach (WaterBaseMessage wbm in list)
                                                {
                                                    if (wbm.RawDataStr != null)
                                                    {
                                                        SendMsg(wbm.RawDataStr);
                                                    }
                                                    else
                                                    {
                                                        SetCalResult("数据发送：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), message.AFN) + " 失败");
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                SetCalResult("数据发送：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), message.AFN) + " 失败：" + msgres);
                                            }
                                        }
                                        #endregion
                                    }
                                    else if (message.AFN == (byte)WaterBaseProtocol.AFN._22)
                                    {
                                        #region _22
                                        SetCalResult("数据接收：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), message.AFN));
                                        WaterCmd_22_1 cmdreceive = new WaterCmd_22_1();
                                        cmdreceive.UserDataBytes = message.UserDataBytes;
                                        cmdreceive.UserData = message.UserData;
                                        string msgreceive = cmdreceive.ReadMsg();
                                        if (msgreceive != "")
                                        {
                                            SetCalResult("数据接收：" + msgreceive);
                                        }
                                        else
                                        {
                                            count++;
                                            if (count == 0)
                                            {
                                                count = 1;
                                            }
                                            WaterCmd_22_2 cmdres = new WaterCmd_22_2();
                                            cmdres.CenterStation = message.CenterStation;
                                            cmdres.RemoteStation = message.RemoteStation;
                                            cmdres.PW = message.PW;

                                            cmdres.SerialNumber = count;
                                            cmdres.SendTime = DateTime.Now;
                                            cmdres.Values = new byte[12];

                                            for (int i = 0; i < cmdres.Values.Length; i++)
                                            {
                                                try
                                                {
                                                    if (i == 0)
                                                    {
                                                        cmdres.Values[i] = Convert.ToByte(this.txtAlarm_01_1.Text.Trim());
                                                    }
                                                    else if (i == 1)
                                                    {
                                                        cmdres.Values[i] = Convert.ToByte(this.txtAlarm_01_2.Text.Trim());
                                                    }
                                                    else if (i == 2)
                                                    {
                                                        cmdres.Values[i] = Convert.ToByte(this.txtAlarm_01_3.Text.Trim());
                                                    }
                                                    else if (i == 3)
                                                    {
                                                        cmdres.Values[i] = Convert.ToByte(this.txtAlarm_03_1.Text.Trim());
                                                    }
                                                    else if (i == 4)
                                                    {
                                                        cmdres.Values[i] = Convert.ToByte(this.txtAlarm_03_2.Text.Trim());
                                                    }
                                                    else if (i == 5)
                                                    {
                                                        cmdres.Values[i] = Convert.ToByte(this.txtAlarm_03_3.Text.Trim());
                                                    }
                                                    else if (i == 6)
                                                    {
                                                        cmdres.Values[i] = Convert.ToByte(this.txtAlarm_06_1.Text.Trim());
                                                    }
                                                    else if (i == 7)
                                                    {
                                                        cmdres.Values[i] = Convert.ToByte(this.txtAlarm_06_2.Text.Trim());
                                                    }
                                                    else if (i == 8)
                                                    {
                                                        cmdres.Values[i] = Convert.ToByte(this.txtAlarm_06_3.Text.Trim());
                                                    }
                                                    else if (i == 9)
                                                    {
                                                        cmdres.Values[i] = Convert.ToByte(this.txtAlarm_12_1.Text.Trim());
                                                    }
                                                    else if (i == 10)
                                                    {
                                                        cmdres.Values[i] = Convert.ToByte(this.txtAlarm_12_2.Text.Trim());
                                                    }
                                                    else if (i == 11)
                                                    {
                                                        cmdres.Values[i] = Convert.ToByte(this.txtAlarm_12_3.Text.Trim());
                                                    }
                                                }
                                                catch
                                                {
                                                    cmdres.Values[i] = 0;
                                                }
                                            }

                                            string msgres = cmdres.WriteMsg();
                                            if (msgres == "")
                                            {
                                                WaterBaseMessage[] list = cmdres.MsgList;
                                                foreach (WaterBaseMessage wbm in list)
                                                {
                                                    if (wbm.RawDataStr != null)
                                                    {
                                                        SendMsg(wbm.RawDataStr);
                                                    }
                                                    else
                                                    {
                                                        SetCalResult("数据发送：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), message.AFN) + " 失败");
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                SetCalResult("数据发送：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), message.AFN) + " 失败：" + msgres);
                                            }
                                        }
                                        #endregion
                                    }
                                    else if (message.AFN == (byte)WaterBaseProtocol.AFN._23)
                                    {
                                        #region _23
                                        SetCalResult("数据接收：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), message.AFN));
                                        WaterCmd_23_1 cmdreceive = new WaterCmd_23_1();
                                        cmdreceive.UserDataBytes = message.UserDataBytes;
                                        cmdreceive.UserData = message.UserData;
                                        string msgreceive = cmdreceive.ReadMsg();
                                        if (msgreceive != "")
                                        {
                                            SetCalResult("数据接收：" + msgreceive);
                                        }
                                        else
                                        {
                                            byte OrderNum = cmdreceive.OrderNum;
                                            if (OrderNum == 1)
                                            {
                                                SetCalResult2(txtPhone_01, cmdreceive.Phone);
                                            }
                                            else if (OrderNum == 2)
                                            {
                                                SetCalResult2(txtPhone_02, cmdreceive.Phone);
                                            }
                                            else if (OrderNum == 3)
                                            {
                                                SetCalResult2(txtPhone_03, cmdreceive.Phone);
                                            }
                                            else if (OrderNum == 4)
                                            {
                                                SetCalResult2(txtPhone_04, cmdreceive.Phone);
                                            }
                                            else if (OrderNum == 5)
                                            {
                                                SetCalResult2(txtPhone_05, cmdreceive.Phone);
                                            }
                                            else if (OrderNum == 6)
                                            {
                                                SetCalResult2(txtPhone_06, cmdreceive.Phone);
                                            }
                                            else if (OrderNum == 7)
                                            {
                                                SetCalResult2(txtPhone_07, cmdreceive.Phone);
                                            }
                                            else if (OrderNum == 8)
                                            {
                                                SetCalResult2(txtPhone_08, cmdreceive.Phone);
                                            }
                                            else if (OrderNum == 9)
                                            {
                                                SetCalResult2(txtPhone_09, cmdreceive.Phone);
                                            }
                                            else if (OrderNum == 10)
                                            {
                                                SetCalResult2(txtPhone_10, cmdreceive.Phone);
                                            }
                                            count++;
                                            if (count == 0)
                                            {
                                                count = 1;
                                            }
                                            WaterCmd_23_2 cmdres = new WaterCmd_23_2();
                                            cmdres.CenterStation = message.CenterStation;
                                            cmdres.RemoteStation = message.RemoteStation;
                                            cmdres.PW = message.PW;

                                            cmdres.SerialNumber = count;
                                            cmdres.SendTime = DateTime.Now;
                                            cmdres.Result = 1;

                                            string msgres = cmdres.WriteMsg();
                                            if (msgres == "")
                                            {
                                                WaterBaseMessage[] list = cmdres.MsgList;
                                                foreach (WaterBaseMessage wbm in list)
                                                {
                                                    if (wbm.RawDataStr != null)
                                                    {
                                                        SendMsg(wbm.RawDataStr);
                                                    }
                                                    else
                                                    {
                                                        SetCalResult("数据发送：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), message.AFN) + " 失败");
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                SetCalResult("数据发送：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), message.AFN) + " 失败：" + msgres);
                                            }
                                        }
                                        #endregion
                                    }
                                    else if (message.AFN == (byte)WaterBaseProtocol.AFN._24)
                                    {
                                        #region _24
                                        SetCalResult("数据接收：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), message.AFN));
                                        WaterCmd_24_1 cmdreceive = new WaterCmd_24_1();
                                        cmdreceive.UserDataBytes = message.UserDataBytes;
                                        cmdreceive.UserData = message.UserData;
                                        string msgreceive = cmdreceive.ReadMsg();
                                        if (msgreceive != "")
                                        {
                                            SetCalResult("数据接收：" + msgreceive);
                                        }
                                        else
                                        {
                                            byte OrderNum = cmdreceive.OrderNum;

                                            count++;
                                            if (count == 0)
                                            {
                                                count = 1;
                                            }
                                            WaterCmd_24_2 cmdres = new WaterCmd_24_2();
                                            cmdres.CenterStation = message.CenterStation;
                                            cmdres.RemoteStation = message.RemoteStation;
                                            cmdres.PW = message.PW;

                                            cmdres.SerialNumber = count;
                                            cmdres.SendTime = DateTime.Now;
                                            cmdres.OrderNum = OrderNum;
                                            cmdres.Phone = "";
                                            if (OrderNum == 1)
                                            {
                                                cmdres.Phone = this.txtPhone_01.Text.Trim();
                                            }
                                            else if (OrderNum == 2)
                                            {
                                                cmdres.Phone = this.txtPhone_02.Text.Trim();
                                            }
                                            else if (OrderNum == 3)
                                            {
                                                cmdres.Phone = this.txtPhone_03.Text.Trim();
                                            }
                                            else if (OrderNum == 4)
                                            {
                                                cmdres.Phone = this.txtPhone_04.Text.Trim();
                                            }
                                            else if (OrderNum == 5)
                                            {
                                                cmdres.Phone = this.txtPhone_05.Text.Trim();
                                            }
                                            else if (OrderNum == 6)
                                            {
                                                cmdres.Phone = this.txtPhone_06.Text.Trim();
                                            }
                                            else if (OrderNum == 7)
                                            {
                                                cmdres.Phone = this.txtPhone_07.Text.Trim();
                                            }
                                            else if (OrderNum == 8)
                                            {
                                                cmdres.Phone = this.txtPhone_08.Text.Trim();
                                            }
                                            else if (OrderNum == 9)
                                            {
                                                cmdres.Phone = this.txtPhone_09.Text.Trim();
                                            }
                                            else if (OrderNum == 10)
                                            {
                                                cmdres.Phone = this.txtPhone_10.Text.Trim();
                                            }

                                            string msgres = cmdres.WriteMsg();
                                            if (msgres == "")
                                            {
                                                WaterBaseMessage[] list = cmdres.MsgList;
                                                foreach (WaterBaseMessage wbm in list)
                                                {
                                                    if (wbm.RawDataStr != null)
                                                    {
                                                        SendMsg(wbm.RawDataStr);
                                                    }
                                                    else
                                                    {
                                                        SetCalResult("数据发送：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), message.AFN) + " 失败");
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                SetCalResult("数据发送：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), message.AFN) + " 失败：" + msgres);
                                            }
                                        }
                                        #endregion
                                    }
                                    else if (message.AFN == (byte)WaterBaseProtocol.AFN._25)
                                    {
                                        #region _25
                                        SetCalResult("数据接收：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), message.AFN));
                                        WaterCmd_25_1 cmdreceive = new WaterCmd_25_1();
                                        cmdreceive.UserDataBytes = message.UserDataBytes;
                                        cmdreceive.UserData = message.UserData;
                                        string msgreceive = cmdreceive.ReadMsg();
                                        if (msgreceive != "")
                                        {
                                            SetCalResult("数据接收：" + msgreceive);
                                        }
                                        else
                                        {
                                            byte RType = cmdreceive.RType;
                                            byte IsSend = cmdreceive.IsSend;
                                            byte numAuthType = cmdreceive.NumAuthenType;
                                            SetCalResult("类型值：" + RType + "，是否发送值：" + IsSend +",认证方式：" +(numAuthType==0?"密码":"白名单"));
                                            SetCalResult2(this.textBox2, RType.ToString());
                                            SetCalResult2(this.textBox3, IsSend.ToString());

                                            count++;
                                            if (count == 0)
                                            {
                                                count = 1;
                                            }
                                            WaterCmd_25_2 cmdres = new WaterCmd_25_2();
                                            cmdres.CenterStation = message.CenterStation;
                                            cmdres.RemoteStation = message.RemoteStation;
                                            cmdres.PW = message.PW;

                                            cmdres.SerialNumber = count;
                                            cmdres.SendTime = DateTime.Now;
                                            cmdres.Result = 1;

                                            string msgres = cmdres.WriteMsg();
                                            if (msgres == "")
                                            {
                                                WaterBaseMessage[] list = cmdres.MsgList;
                                                foreach (WaterBaseMessage wbm in list)
                                                {
                                                    if (wbm.RawDataStr != null)
                                                    {
                                                        SendMsg(wbm.RawDataStr);
                                                    }
                                                    else
                                                    {
                                                        SetCalResult("数据发送：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), message.AFN) + " 失败");
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                SetCalResult("数据发送：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), message.AFN) + " 失败：" + msgres);
                                            }
                                        }
                                        #endregion
                                    }
                                    else if (message.AFN == (byte)WaterBaseProtocol.AFN._26)
                                    {
                                        #region _26
                                        SetCalResult("数据接收：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), message.AFN));
                                        WaterCmd_26_1 cmdreceive = new WaterCmd_26_1();
                                        cmdreceive.UserDataBytes = message.UserDataBytes;
                                        cmdreceive.UserData = message.UserData;
                                        string msgreceive = cmdreceive.ReadMsg();
                                        if (msgreceive != "")
                                        {
                                            SetCalResult("数据接收：" + msgreceive);
                                        }
                                        else
                                        {
                                            count++;
                                            if (count == 0)
                                            {
                                                count = 1;
                                            }
                                            WaterCmd_26_2 cmdres = new WaterCmd_26_2();
                                            cmdres.CenterStation = message.CenterStation;
                                            cmdres.RemoteStation = message.RemoteStation;
                                            cmdres.PW = message.PW;

                                            cmdres.SerialNumber = count;
                                            cmdres.SendTime = DateTime.Now;
                                            try
                                            {
                                                cmdres.RType = Convert.ToByte(this.textBox2.Text.Trim());
                                            }
                                            catch { cmdres.RType = 0; SetCalResult("遥测站类型错误：" + this.textBox2.Text.Trim()); }
                                            try
                                            {
                                                cmdres.IsSend = Convert.ToByte(this.textBox3.Text.Trim());
                                            }
                                            catch { cmdres.IsSend = 0; SetCalResult("是否发送预警短信错误：" + this.textBox3.Text.Trim()); }

                                            cmdres.NumAuthenType = 1;//始终显示白名单验证方式。

                                            string msgres = cmdres.WriteMsg();
                                            if (msgres == "")
                                            {
                                                WaterBaseMessage[] list = cmdres.MsgList;
                                                foreach (WaterBaseMessage wbm in list)
                                                {
                                                    if (wbm.RawDataStr != null)
                                                    {
                                                        SendMsg(wbm.RawDataStr);
                                                    }
                                                    else
                                                    {
                                                        SetCalResult("数据发送：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), message.AFN) + " 失败");
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                SetCalResult("数据发送：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), message.AFN) + " 失败：" + msgres);
                                            }
                                        }
                                        #endregion
                                    }
                                    else if (message.AFN == (byte)WaterBaseProtocol.AFN._27)
                                    {
                                        #region _27
                                        SetCalResult("数据接收：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), message.AFN));
                                        WaterCmd_27_1 cmdreceive = new WaterCmd_27_1();
                                        cmdreceive.UserDataBytes = message.UserDataBytes;
                                        cmdreceive.UserData = message.UserData;
                                        string msgreceive = cmdreceive.ReadMsg();
                                        if (msgreceive != "")
                                        {
                                            SetCalResult("数据接收：" + msgreceive);
                                        }
                                        else
                                        {
                                            SetCalResult("播报次数：" + cmdreceive.PlayCount);
                                            SetCalResult("播报角色：" + cmdreceive.PlayRole);
                                            SetCalResult("播报速度：" + cmdreceive.PlaySpeed);
                                            SetCalResult("播报内容：" + cmdreceive.PlayContext);

                                            count++;
                                            if (count == 0)
                                            {
                                                count = 1;
                                            }
                                            WaterCmd_27_2 cmdres = new WaterCmd_27_2();
                                            cmdres.CenterStation = message.CenterStation;
                                            cmdres.RemoteStation = message.RemoteStation;
                                            cmdres.PW = message.PW;

                                            cmdres.SerialNumber = count;
                                            cmdres.SendTime = DateTime.Now;
                                            
                                            string msgres = cmdres.WriteMsg();
                                            if (msgres == "")
                                            {
                                                WaterBaseMessage[] list = cmdres.MsgList;
                                                foreach (WaterBaseMessage wbm in list)
                                                {
                                                    if (wbm.RawDataStr != null)
                                                    {
                                                        SendMsg(wbm.RawDataStr);
                                                    }
                                                    else
                                                    {
                                                        SetCalResult("数据发送：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), message.AFN) + " 失败");
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                SetCalResult("数据发送：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), message.AFN) + " 失败：" + msgres);
                                            }
                                        }
                                        #endregion
                                    }
                                    else if (message.AFN == (byte)WaterBaseProtocol.AFN._28)
                                    {
                                        #region _28
                                        SetCalResult("数据接收：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), message.AFN));
                                        WaterCmd_28_1 cmdreceive = new WaterCmd_28_1();
                                        cmdreceive.UserDataBytes = message.UserDataBytes;
                                        cmdreceive.UserData = message.UserData;
                                        string msgreceive = cmdreceive.ReadMsg();
                                        if (msgreceive != "")
                                        {
                                            SetCalResult("数据接收：" + msgreceive);
                                        }
                                        else
                                        {
                                            SetCalResult("黄色预警值：" + cmdreceive.YellowLevel);
                                            SetCalResult("橙色预警值：" + cmdreceive.OrangeLevel);
                                            SetCalResult("红色预警值：" + cmdreceive.RedLevel);
                                            //SetCalResult("播报次数：" + cmdreceive.PlayCount);
                                            //SetCalResult("播报角色：" + cmdreceive.PlayRole);
                                            //SetCalResult("播报速度：" + cmdreceive.PlaySpeed);
                                            //SetCalResult("播报内容：" + cmdreceive.PlayContext);

                                            count++;
                                            if (count == 0)
                                            {
                                                count = 1;
                                            }
                                            WaterCmd_28_2 cmdres = new WaterCmd_28_2();
                                            cmdres.CenterStation = message.CenterStation;
                                            cmdres.RemoteStation = message.RemoteStation;
                                            cmdres.PW = message.PW;

                                            cmdres.SerialNumber = count;
                                            cmdres.SendTime = DateTime.Now;

                                            string msgres = cmdres.WriteMsg();
                                            if (msgres == "")
                                            {
                                                WaterBaseMessage[] list = cmdres.MsgList;
                                                foreach (WaterBaseMessage wbm in list)
                                                {
                                                    if (wbm.RawDataStr != null)
                                                    {
                                                        SendMsg(wbm.RawDataStr);
                                                    }
                                                    else
                                                    {
                                                        SetCalResult("数据发送：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), message.AFN) + " 失败");
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                SetCalResult("数据发送：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), message.AFN) + " 失败：" + msgres);
                                            }
                                        }
                                        #endregion
                                    }
                                    else if (message.AFN == (byte)WaterBaseProtocol.AFN._29)
                                    {
                                        #region _29
                                        SetCalResult("数据接收：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), message.AFN));
                                        WaterCmd_29_1 cmdreceive = new WaterCmd_29_1();
                                        cmdreceive.UserDataBytes = message.UserDataBytes;
                                        cmdreceive.UserData = message.UserData;
                                        string msgreceive = cmdreceive.ReadMsg();
                                        if (msgreceive != "")
                                        {
                                            SetCalResult("数据接收：" + msgreceive);
                                        }
                                        else
                                        {
                                            count++;
                                            if (count == 0)
                                            {
                                                count = 1;
                                            }
                                            WaterCmd_29_2 cmdres = new WaterCmd_29_2();
                                            cmdres.CenterStation = message.CenterStation;
                                            cmdres.RemoteStation = message.RemoteStation;
                                            cmdres.PW = message.PW;

                                            cmdres.SerialNumber = count;
                                            cmdres.SendTime = DateTime.Now;
                                            cmdres.YellowLevel = 1211;
                                            cmdres.OrangeLevel = 1212;
                                            cmdres.RedLevel = 1213;

                                            string msgres = cmdres.WriteMsg();
                                            if (msgres == "")
                                            {
                                                WaterBaseMessage[] list = cmdres.MsgList;
                                                foreach (WaterBaseMessage wbm in list)
                                                {
                                                    if (wbm.RawDataStr != null)
                                                    {
                                                        SendMsg(wbm.RawDataStr);
                                                    }
                                                    else
                                                    {
                                                        SetCalResult("数据发送：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), message.AFN) + " 失败");
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                SetCalResult("数据发送：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), message.AFN) + " 失败：" + msgres);
                                            }
                                        }
                                        #endregion
                                    }
                                    else if (message.AFN == (byte)WaterBaseProtocol.AFN._51)
                                    {
                                        #region _51
                                        SetCalResult("数据接收：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), message.AFN));
                                        WaterCmd_51_1 cmdreceive = new WaterCmd_51_1();
                                        cmdreceive.UserDataBytes = message.UserDataBytes;
                                        cmdreceive.UserData = message.UserData;
                                        string msgreceive = cmdreceive.ReadMsg();
                                        if (msgreceive != "")
                                        {
                                            SetCalResult("数据接收：" + msgreceive);
                                        }
                                        else
                                        {
                                            count++;
                                            if (count == 0)
                                            {
                                                count = 1;
                                            }
                                            WaterCmd_51_2 cmdres = new WaterCmd_51_2();
                                            cmdres.CenterStation = message.CenterStation;
                                            cmdres.RemoteStation = message.RemoteStation;
                                            cmdres.PW = message.PW;

                                            cmdres.SerialNumber = count;
                                            cmdres.SendTime = DateTime.Now;
                                            Identifier_F1 Iden_F1 = new Identifier_F1();
                                            Iden_F1.RemoteStation = message.RemoteStation;
                                            Iden_F1.StationType = 0x46;
                                            cmdres.Iden_F1 = Iden_F1;

                                            string msgres = cmdres.WriteMsg();
                                            if (msgres == "")
                                            {
                                                WaterBaseMessage[] list = cmdres.MsgList;
                                                foreach (WaterBaseMessage wbm in list)
                                                {
                                                    if (wbm.RawDataStr != null)
                                                    {
                                                        SendMsg(wbm.RawDataStr);
                                                    }
                                                    else
                                                    {
                                                        SetCalResult("数据发送：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), message.AFN) + " 失败");
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                SetCalResult("数据发送：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), message.AFN) + " 失败：" + msgres);
                                            }
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        SetCalResult("数据接收：非法下行数据AFN，不处理！" + message.AFN.ToString("X").PadLeft(2, '0'));
                                    }
                                }
                                else if (message.DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Down.ACK)
                                {
                                    SetCalResult("数据接收：ETB肯定确认，继续发送");
                                }
                                else if (message.DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Down.NAK)
                                {
                                    SetCalResult("数据接收：ETB否定应答，反馈重发");
                                }
                                else if (message.DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Down.EOT)
                                {
                                    SetCalResult("数据接收：ETX传输结束，退出");
                                }
                                else if (message.DataEndChar == (byte)WaterBaseProtocol.DataEndChar_Down.ESC)
                                {
                                    SetCalResult("数据接收：ETX传输结束，终端保持在线");
                                }
                                else
                                {
                                    SetCalResult("数据接收：非法下行数据DataEndChar，不处理");
                                }
                            }
                            else
                            {
                                SetCalResult("数据接收：非下行数据，不处理");
                            }
                        }
                        else
                        {
                            SetCalResult("数据接收：" + msg);
                        }
                    }
                }
                catch (Exception ex)
                {
                    SetCalResult(ex.Message);
                    isConnect = false;
                }
            }
            SetCalResult("数据接收连接断开");
            setBtnEnabled(false);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CloseSocket();
        }

        public void CloseSocket()
        {
            isConnect = false;
            try
            {
                if (clientSocket != null)
                {
                    clientSocket.Close(1000);
                    setBtnEnabled(false);
                }
            }
            catch { }
        }

        private void XT()
        {
            SetCalResult("心跳开启");
            int c = 0;
            while (isConnect)
            {
                Thread.Sleep(1000);
                c++;

                if (c >= 60 * 2)
                {
                    c = 0;

                    SendMsg(HexStringUtility.StrToHexString(ProtocolKey.ReceiveHeartThrob));
                    SetCalResult("发送心跳");
                }
            }
            SetCalResult("心跳停止");
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseSocket();
        }

        public void SendMsg(string msg)
        {
            try
            {
                if (clientSocket != null)
                {
                    clientSocket.Send(HexStringUtility.HexStringToByteArray(msg));
                    SetCalResult("发送数据成功！" + msg);
                }
                else
                {
                    SetCalResult("连接不存在无法发送数据");
                    isConnect = false;
                    setBtnEnabled(false);
                }
            }
            catch (Exception ex)
            {
                CloseSocket();
                SetCalResult("发送数据失败");
                SetCalResult(ex.Message);
            }
        }

        public void SendMsg(byte[] msg)
        {
            try
            {
                if (clientSocket != null)
                {
                    clientSocket.Send(msg);
                    SetCalResult("发送数据成功！" + HexStringUtility.ByteArrayToHexString(msg));
                }
                else
                {
                    SetCalResult("连接不存在无法发送数据");
                    isConnect = false;
                    setBtnEnabled(false);
                }
            }
            catch (Exception ex)
            {
                CloseSocket();
                SetCalResult("发送数据失败");
                SetCalResult(ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string msg = this.textBox1.Text.Trim().Replace(" ", "");
            SendMsg(msg);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string txt = this.textBox1.Text.Trim().Replace(" ", "");
            WaterBaseMessage wbm = new WaterBaseMessage();
            wbm.RawDataStr = txt;
            wbm.RawDataChar = HexStringUtility.HexStringToByteArray(txt);
            string msg = wbm.ReadMsgBase();
            if (msg == "")
            {
                SetCalResult("水文信息校验成功！");
            }
            else
            {
                SetCalResult("水文信息校验失败！" + msg);
                if (wbm.IsWaterMessage)
                {
                    SetCalResult("水文信息校验失败！" + "是水文信息数据！");
                }
                else
                {
                    SetCalResult("水文信息校验失败！" + "非水文信息数据！");

                    BaseMessage bm = new BaseMessage();
                    bm.RawDataStr = txt;
                    bm.RawDataChar = HexStringUtility.HexStringToByteArray(txt);
                    string msg1 = wbm.ReadMsgBase();
                    if (msg1 == "")
                    {
                        SetCalResult("自定义信息校验成功！");
                    }
                    else
                    {
                        SetCalResult("自定义信息校验失败！" + msg1);
                    }
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.textBox1.Text = "";
        }

        private void button6_Click(object sender, EventArgs e)
        {
            SetCalClearResult();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                WaterCmd_2F_1 cmd = new WaterCmd_2F_1();
                cmd.CenterStation = GetCenterStation();
                cmd.RemoteStation = GetRemoteStation();
                cmd.PW = GetPW();
                cmd.SerialNumber = GetCount();
                cmd.SendTime = DateTime.Now;

                string msg = cmd.WriteMsg();
                if (msg == "")
                {
                    SetCalResult(cmd.ToString());
                    WaterBaseMessage[] list = cmd.MsgList;
                    foreach (WaterBaseMessage wbm in list)
                    {
                        if (wbm.RawDataStr != null)
                        {
                            SendMsg(wbm.RawDataStr);
                        }
                        else
                        {
                            SetCalResult("数据发送：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), cmd.AFN) + " 失败");
                        }
                    }
                }
                else
                {
                    SetCalResult("数据发送：" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), cmd.AFN) + " 失败：" + msg);
                }
            }
            catch (Exception ex)
            {
                SetCalResult("数据生成失败：" + ex.Message);
            }
        }

        Form2 f;
        private void button8_Click(object sender, EventArgs e)
        {
            if (f == null || f.IsDisposed)
            {
                f = new Form2(this);

            }

            f.Show();
            f.Activate();
        }

        public byte GetCenterStation()
        {
            try
            {
                return Convert.ToByte(txtCenterStation.Text.Trim(), 16);
            }
            catch
            {
                return 0;
            }
        }

        public string GetRemoteStation()
        {
            string RemoteStation = txtRemoteStation.Text.Trim().PadLeft(10, '0');
            if (RemoteStation.Length > 10)
            {
                RemoteStation = RemoteStation.Substring(0, 10);
            }
            return RemoteStation;
        }

        public string GetPW()
        {
            string PW = txtPW.Text.Trim().PadLeft(10, '0');
            if (PW.Length > 4)
            {
                PW = PW.Substring(0, 4);
            }
            return PW;
        }

        public short GetCount()
        {
            count++;
            if (count == 0)
            {
                count = 1;
            }
            return count;
        }

        frmParamBase frmParamBase;
        private void button9_Click(object sender, EventArgs e)
        {
            if (frmParamBase == null || frmParamBase.IsDisposed)
            {
                frmParamBase = new frmParamBase();

            }

            frmParamBase.Show();
            frmParamBase.Activate();
        }

        frmParamRun frmParamRun;
        private void button10_Click(object sender, EventArgs e)
        {
            if (frmParamRun == null || frmParamRun.IsDisposed)
            {
                frmParamRun = new frmParamRun();

            }

            frmParamRun.Show();
            frmParamRun.Activate();
        }
    }
}
