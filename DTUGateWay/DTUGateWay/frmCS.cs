using Common;
using DTU.GateWay.Protocol;
using Maticsoft.Model;
using Module;
using Server.Data.Bridge;
using Server.Util.Cache;
using Server.Util.Service;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace DTUGateWay
{
    public partial class frmCS : Form
    {
        DataBridge bridge;
        public frmCS()
        {
            InitializeComponent();

            init();
        }

        public frmCS(DataBridge bridge)
        {
            InitializeComponent();

            init();

            this.bridge = bridge;
        }

        private void frmCS_Load(object sender, EventArgs e)
        {

        }

        private void init()
        {
            SafeSetLabel(this.label6, "", false);
            SafeSetLabel(this.label7, "", false);

            this.comboBox1.Items.Clear();
            this.comboBox1.Items.Add("-请选择-");
            if (DeviceModule.GetAllDevice() == null || DeviceModule.GetAllDevice().Count == 0)
            {
                DistrictModule.UpdateLevelInfo();
                DistrictModule.UpdateDistrictInfo();
                DeviceModule.LoadDevices();
            }
            List<string> list = Module.DeviceModule.GetAllDeviceNo();
            foreach (string deviceNo in list)
            {
                this.comboBox1.Items.Add(deviceNo);
            }
            this.comboBox1.SelectedIndex = 0;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(new ParameterizedThreadStart(send));
            t.Start(1);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(new ParameterizedThreadStart(send));
            t.Start(2);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(new ParameterizedThreadStart(send));
            t.Start(3);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(new ParameterizedThreadStart(send));
            t.Start(4);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(new ParameterizedThreadStart(send));
            t.Start(5);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(new ParameterizedThreadStart(send));
            t.Start(6);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(new ParameterizedThreadStart(send));
            t.Start(7);
        }

        #region 发送命令

        private bool isRun = true;
        private void time()
        {
            SafeSetLabel(this.label7, "启动", false);
            while (isRun)
            {
                SafeSetLabel(this.label7, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), false);
                Thread.Sleep(10);
            }
            SafeSetLabel(this.label7, "停止", false);
        }

        private void send(object o)
        {
            SafeSetLabel(this.label7, "", false);

            setAllButtonEnable(false);
            
            string OperationType = "";
            int k = 0;
            try
            {
                k = Convert.ToInt32(o);
                switch (k)
                {
                    case 1: OperationType = "设置时间"; break;
                    case 2: OperationType = "设置开采量"; break;
                    case 3: OperationType = "查询时间"; break;
                    case 4: OperationType = "查询开采量"; break;
                    case 5: OperationType = "远程开泵"; break;
                    case 6: OperationType = "远程关泵"; break;
                    case 7: OperationType = "设置主站射频地址"; break;
                    case 8: OperationType = "屏蔽卡号"; break;
                    case 9: OperationType = "解除屏蔽卡号"; break;
                    case 10: OperationType = "水位计参数设置"; break;
                    case 11: OperationType = "水位计参数查询"; break;
                    case 12: OperationType = "水位查询"; break;
                    case 13: OperationType = "状态查询"; break;
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

            try
            {
                if (this.comboBox1.SelectedIndex < 1)
                {
                    SafeSetText(textBox2, "请选择一个终端", false);
                    setAllButtonEnable(true);
                    return;
                }
                string deviceNo = this.comboBox1.Text.ToString().Trim();
                Device device = DeviceModule.GetDeviceByFullDeviceNo(deviceNo);
                string resDeviceNo = DeviceModule.GetDeviceNoMain(deviceNo);
                Device resDevice = DeviceModule.GetDeviceByFullDeviceNo(resDeviceNo);
                if (resDeviceNo.Length != 15)
                {
                    SafeSetText(textBox2, "终端编号错误！" + resDeviceNo, false);
                    setAllButtonEnable(true);
                    return;
                }

                BaseMessage cmd = new BaseMessage();
                switch (k)
                {
                    case 1:
                        if (dateTimePicker1.Value < DateTime.Now.AddDays(-1) || dateTimePicker1.Value > DateTime.Now.AddDays(1))
                        {
                            SafeSetText(textBox2, "终端时钟取值范围是当前时间前后一天", false);
                            setAllButtonEnable(true);
                            return;
                        }

                        CmdToDtuSetDateTime cmd1 = new CmdToDtuSetDateTime();
                        cmd1.AddressField = DeviceModule.DeviceNo_Normal2Hex(deviceNo);
                        cmd1.StationType = (byte)device.StationType;
                        cmd1.StationCode = device.StationType == 2 ? device.StationCode : 0;
                        cmd1.DateTimeNew = dateTimePicker1.Value;
                        cmd1.RawDataChar = cmd1.WriteMsg();
                        cmd1.RawDataStr = HexStringUtility.ByteArrayToHexString(cmd1.RawDataChar);
                        ToDtuCommand.AddBaseMessageToDtu(cmd1.AddressField + "-" + cmd1.AFN, cmd1);

                        cmd.AddressField = cmd1.AddressField;
                        cmd.RawDataStr = cmd1.RawDataStr;
                        cmd.RawDataChar = cmd1.RawDataChar;
                        cmd.AFN = cmd1.AFN;
                        break;
                    case 2:
                        decimal YearExploitation = 0;
                        try
                        {
                            YearExploitation = decimal.Parse(this.textBox4.Text.Trim());
                        }
                        catch { }
                        if (YearExploitation < 0 || YearExploitation > 9999999.9M)
                        {
                            SafeSetText(textBox2, "开采量取值范围是0-9999999.9", false);
                            setAllButtonEnable(true);
                            return;
                        }

                        CmdToDtuSetYearExploitation cmd2 = new CmdToDtuSetYearExploitation();
                        cmd2.AddressField = DeviceModule.DeviceNo_Normal2Hex(deviceNo);
                        cmd2.StationType = (byte)device.StationType;
                        cmd2.StationCode = device.StationType == 2 ? device.StationCode : 0;
                        cmd2.YearExploitation = YearExploitation;
                        cmd2.RawDataChar = cmd2.WriteMsg();
                        cmd2.RawDataStr = HexStringUtility.ByteArrayToHexString(cmd2.RawDataChar);
                        ToDtuCommand.AddBaseMessageToDtu(cmd2.AddressField + "-" + cmd2.AFN, cmd2);

                        cmd.AddressField = cmd2.AddressField;
                        cmd.RawDataStr = cmd2.RawDataStr;
                        cmd.RawDataChar = cmd2.RawDataChar;
                        cmd.AFN = cmd2.AFN;
                        break;
                    case 3:
                        CmdToDtuQueryDateTime cmd3 = new CmdToDtuQueryDateTime();
                        cmd3.AddressField = DeviceModule.DeviceNo_Normal2Hex(deviceNo);
                        cmd3.StationType = (byte)device.StationType;
                        cmd3.StationCode = device.StationType == 2 ? device.StationCode : 0;
                        cmd3.RawDataChar = cmd3.WriteMsg();
                        cmd3.RawDataStr = HexStringUtility.ByteArrayToHexString(cmd3.RawDataChar);
                        ToDtuCommand.AddBaseMessageToDtu(cmd3.AddressField + "-" + cmd3.AFN, cmd3);

                        cmd.AddressField = cmd3.AddressField;
                        cmd.RawDataStr = cmd3.RawDataStr;
                        cmd.RawDataChar = cmd3.RawDataChar;
                        cmd.AFN = cmd3.AFN;
                        break;
                    case 4:
                        CmdToDtuQueryYearExploitation cmd4 = new CmdToDtuQueryYearExploitation();
                        cmd4.AddressField = DeviceModule.DeviceNo_Normal2Hex(deviceNo);
                        cmd4.StationType = (byte)device.StationType;
                        cmd4.StationCode = device.StationType == 2 ? device.StationCode : 0;
                        cmd4.RawDataChar = cmd4.WriteMsg();
                        cmd4.RawDataStr = HexStringUtility.ByteArrayToHexString(cmd4.RawDataChar);
                        ToDtuCommand.AddBaseMessageToDtu(cmd4.AddressField + "-" + cmd4.AFN, cmd4);

                        cmd.AddressField = cmd4.AddressField;
                        cmd.RawDataStr = cmd4.RawDataStr;
                        cmd.RawDataChar = cmd4.RawDataChar;
                        cmd.AFN = cmd4.AFN;
                        break;
                    case 5:
                        CmdToDtuOpenPump cmd5 = new CmdToDtuOpenPump();
                        cmd5.AddressField = DeviceModule.DeviceNo_Normal2Hex(deviceNo);
                        cmd5.StationType = (byte)device.StationType;
                        cmd5.StationCode = device.StationType == 2 ? device.StationCode : 0;
                        cmd5.RawDataChar = cmd5.WriteMsg();
                        cmd5.RawDataStr = HexStringUtility.ByteArrayToHexString(cmd5.RawDataChar);
                        ToDtuCommand.AddBaseMessageToDtu(cmd5.AddressField + "-" + cmd5.AFN, cmd5);

                        cmd.AddressField = cmd5.AddressField;
                        cmd.RawDataStr = cmd5.RawDataStr;
                        cmd.RawDataChar = cmd5.RawDataChar;
                        cmd.AFN = cmd5.AFN;
                        break;
                    case 6:
                        CmdToDtuClosePump cmd6 = new CmdToDtuClosePump();
                        cmd6.AddressField = DeviceModule.DeviceNo_Normal2Hex(deviceNo);
                        cmd6.StationType = (byte)device.StationType;
                        cmd6.StationCode = device.StationType == 2 ? device.StationCode : 0;
                        cmd6.RawDataChar = cmd6.WriteMsg();
                        cmd6.RawDataStr = HexStringUtility.ByteArrayToHexString(cmd6.RawDataChar);
                        ToDtuCommand.AddBaseMessageToDtu(cmd6.AddressField + "-" + cmd6.AFN, cmd6);

                        cmd.AddressField = cmd6.AddressField;
                        cmd.RawDataStr = cmd6.RawDataStr;
                        cmd.RawDataChar = cmd6.RawDataChar;
                        cmd.AFN = cmd6.AFN;
                        break;
                    case 7:
                        CmdToDtuSetStationCode cmd7 = new CmdToDtuSetStationCode();
                        if (device.StationType == 0)
                        {
                            SafeSetText(textBox2, "非主从站无法设置分站射频地址", false);
                            setAllButtonEnable(true);
                            return;
                        }
                        else if (device.StationType == 1)
                        {
                            cmd7.AddressField = deviceNo.Substring(0, 12) + Convert.ToInt32(deviceNo.Substring(12, 3)).ToString("X").PadLeft(2, '0');
                            cmd7.StationType = (byte)device.StationType;
                            cmd7.StationCode = device.StationType == 2 ? device.StationCode : 0;
                            List<Device> DeviceSubList = DeviceModule.GetAllDeviceSubList(device.Id);
                            List<int> list = new List<int>();
                            foreach (Device DeviceSub in DeviceSubList)
                            {
                                list.Add(DeviceSub.StationCode);
                            }
                            cmd7.StationCodeList = list;
                        }
                        else if (device.StationType == 2)
                        {
                            cmd7.AddressField = resDeviceNo.Substring(0, 12) + Convert.ToInt32(resDeviceNo.Substring(12, 3)).ToString("X").PadLeft(2, '0');
                            cmd7.StationType = (byte)resDevice.StationType;
                            cmd7.StationCode = resDevice.StationType == 2 ? resDevice.StationCode : 0;
                            List<Device> DeviceSubList = DeviceModule.GetAllDeviceSubList(resDevice.Id);
                            List<int> list = new List<int>();
                            foreach (Device DeviceSub in DeviceSubList)
                            {
                                list.Add(DeviceSub.StationCode);
                            }
                            cmd7.StationCodeList = list;
                        }
                        cmd7.RawDataChar = cmd7.WriteMsg();
                        cmd7.RawDataStr = HexStringUtility.ByteArrayToHexString(cmd7.RawDataChar);

                        ToDtuCommand.AddBaseMessageToDtu(cmd7.AddressField + "-" + cmd7.AFN, cmd7);

                        cmd.AddressField = cmd7.AddressField;
                        cmd.RawDataStr = cmd7.RawDataStr;
                        cmd.RawDataChar = cmd7.RawDataChar;
                        cmd.AFN = cmd7.AFN;

                        break;
                    case 8:
                        string SerialNumber1 = this.textBox3.Text.Trim();
                        if (SerialNumber1.Length != 8)
                        {
                            SafeSetText(textBox2, "卡号长度只能为8位", false);
                            setAllButtonEnable(true);
                            return;
                        }
                        if (!Regex.IsMatch(SerialNumber1, "^[0-9A-Fa-f]+$"))
                        {
                            SafeSetText(textBox2, "卡号只能为0-9A-Fa-f", false);
                            setAllButtonEnable(true);
                            return;
                        }

                        CmdToDtuShieldSerialNumber cmd8 = new CmdToDtuShieldSerialNumber();
                        cmd8.AddressField = DeviceModule.DeviceNo_Normal2Hex(deviceNo);
                        cmd8.StationType = (byte)device.StationType;
                        cmd8.StationCode = device.StationType == 2 ? device.StationCode : 0;
                        cmd8.SerialNumber = SerialNumber1;
                        cmd8.RawDataChar = cmd8.WriteMsg();
                        cmd8.RawDataStr = HexStringUtility.ByteArrayToHexString(cmd8.RawDataChar);
                        ToDtuCommand.AddBaseMessageToDtu(cmd8.AddressField + "-" + cmd8.AFN, cmd8);

                        cmd.AddressField = cmd8.AddressField;
                        cmd.RawDataStr = cmd8.RawDataStr;
                        cmd.RawDataChar = cmd8.RawDataChar;
                        cmd.AFN = cmd8.AFN;
                        break;
                    case 9:
                        string SerialNumber2 = this.textBox3.Text.Trim();
                        if (SerialNumber2.Length != 8)
                        {
                            SafeSetText(textBox2, "卡号长度只能为8位", false);
                            setAllButtonEnable(true);
                            return;
                        }
                        if (!Regex.IsMatch(SerialNumber2, "^[0-9A-Fa-f]+$"))
                        {
                            SafeSetText(textBox2, "卡号只能为0-9A-Fa-f", false);
                            setAllButtonEnable(true);
                            return;
                        }

                        CmdToDtuShieldSerialNumberCancel cmd9 = new CmdToDtuShieldSerialNumberCancel();
                        cmd9.AddressField = DeviceModule.DeviceNo_Normal2Hex(deviceNo);
                        cmd9.StationType = (byte)device.StationType;
                        cmd9.StationCode = device.StationType == 2 ? device.StationCode : 0;
                        cmd9.SerialNumber = SerialNumber2;
                        cmd9.RawDataChar = cmd9.WriteMsg();
                        cmd9.RawDataStr = HexStringUtility.ByteArrayToHexString(cmd9.RawDataChar);
                        ToDtuCommand.AddBaseMessageToDtu(cmd9.AddressField + "-" + cmd9.AFN, cmd9);

                        cmd.AddressField = cmd9.AddressField;
                        cmd.RawDataStr = cmd9.RawDataStr;
                        cmd.RawDataChar = cmd9.RawDataChar;
                        cmd.AFN = cmd9.AFN;
                        break;
                    case 10:
                        CmdToDtuSetGroundWaterParam cmd10 = new CmdToDtuSetGroundWaterParam();
                        cmd10.AddressField = DeviceModule.DeviceNo_Normal2Hex(deviceNo);
                        cmd10.StationType = (byte)device.StationType;
                        cmd10.StationCode = device.StationType == 2 ? device.StationCode : 0;
                        cmd10.Range = byte.Parse(this.txtRange.Text.Trim());
                        cmd10.LineLength = double.Parse(this.txtLineLength.Text.Trim());
                        cmd10.RawDataChar = cmd10.WriteMsg();
                        cmd10.RawDataStr = HexStringUtility.ByteArrayToHexString(cmd10.RawDataChar);
                        ToDtuCommand.AddBaseMessageToDtu(cmd10.AddressField + "-" + cmd10.AFN, cmd10);

                        cmd.AddressField = cmd10.AddressField;
                        cmd.RawDataStr = cmd10.RawDataStr;
                        cmd.RawDataChar = cmd10.RawDataChar;
                        cmd.AFN = cmd10.AFN;
                        break;
                    case 11:
                        CmdToDtuQueryGroundWaterParam cmd11 = new CmdToDtuQueryGroundWaterParam();
                        cmd11.AddressField = DeviceModule.DeviceNo_Normal2Hex(deviceNo);
                        cmd11.StationType = (byte)device.StationType;
                        cmd11.StationCode = device.StationType == 2 ? device.StationCode : 0;
                        cmd11.RawDataChar = cmd11.WriteMsg();
                        cmd11.RawDataStr = HexStringUtility.ByteArrayToHexString(cmd11.RawDataChar);
                        ToDtuCommand.AddBaseMessageToDtu(cmd11.AddressField + "-" + cmd11.AFN, cmd11);

                        cmd.AddressField = cmd11.AddressField;
                        cmd.RawDataStr = cmd11.RawDataStr;
                        cmd.RawDataChar = cmd11.RawDataChar;
                        cmd.AFN = cmd11.AFN;
                        break;
                    case 12:
                        CmdToDtuQueryGroundWater cmd12 = new CmdToDtuQueryGroundWater();
                        cmd12.AddressField = DeviceModule.DeviceNo_Normal2Hex(deviceNo);
                        cmd12.StationType = (byte)device.StationType;
                        cmd12.StationCode = device.StationType == 2 ? device.StationCode : 0;
                        cmd12.RawDataChar = cmd12.WriteMsg();
                        cmd12.RawDataStr = HexStringUtility.ByteArrayToHexString(cmd12.RawDataChar);
                        ToDtuCommand.AddBaseMessageToDtu(cmd12.AddressField + "-" + cmd12.AFN, cmd12);

                        cmd.AddressField = cmd12.AddressField;
                        cmd.RawDataStr = cmd12.RawDataStr;
                        cmd.RawDataChar = cmd12.RawDataChar;
                        cmd.AFN = cmd12.AFN;
                        break;
                    case 13:
                        CmdToDtuQueryState cmd13 = new CmdToDtuQueryState();
                        cmd13.AddressField = DeviceModule.DeviceNo_Normal2Hex(deviceNo);
                        cmd13.StationType = (byte)device.StationType;
                        cmd13.StationCode = device.StationType == 2 ? device.StationCode : 0;
                        cmd13.RawDataChar = cmd13.WriteMsg();
                        cmd13.RawDataStr = HexStringUtility.ByteArrayToHexString(cmd13.RawDataChar);
                        ToDtuCommand.AddBaseMessageToDtu(cmd13.AddressField + "-" + cmd13.AFN, cmd13);

                        cmd.AddressField = cmd13.AddressField;
                        cmd.RawDataStr = cmd13.RawDataStr;
                        cmd.RawDataChar = cmd13.RawDataChar;
                        cmd.AFN = cmd13.AFN;
                        break;
                }

                SafeSetText(textBox2, cmd.RawDataStr, false);
                SafeSetText(textBox2, Environment.NewLine + "添加发送命令保存！" + cmd.AddressField + "-" + cmd.AFN, true);

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

                if (OnlineDeviceService.GetOnline(deviceNo) != null || OnlineDeviceService.GetOnline(resDeviceNo) != null)
                {
                    if (bridge != null)
                    {
                        if (bridge.sendToDtu(deviceNo, resDeviceNo, cmd.RawDataChar, 0, cmd.RawDataChar.Length))
                        {
                            op.State = "发送成功";
                        }
                        else
                        {
                            op.State = "发送失败";
                        }
                    }

                    DeviceOperationModule.AddDeviceOperation(op);

                    if (op.State == "发送失败")
                    {
                        ToDtuCommand.RemoveBaseMessageFromDtu(cmd.AddressField + "-" + cmd.AFN);
                    }

                    if (ToDtuCommand.GetBaseMessageToDtuByKey(cmd.AddressField + "-" + cmd.AFN) != null)
                    {
                        int timeDelay = 0;
                        bool flag = false;
                        while (timeDelay < 300)
                        {
                            BaseMessage res = null;
                            switch (k)
                            {
                                case 1:
                                    res = (CmdResponseToDtuSetDateTime)ToDtuCommand.GetBaseMessageFromDtuByKey(cmd.AddressField + "-" + cmd.AFN);
                                    if (res != null)
                                    {
                                        SafeSetText(textBox2, Environment.NewLine + OperationType + "成功！" + timeDelay + "，返回结果：" + ((CmdResponseToDtuSetDateTime)res).DateTimeNew.ToString("yyyy-MM-dd HH:mm:ss"), true);
                                        ToDtuCommand.RemoveBaseMessageFromDtu(cmd.AddressField + "-" + cmd.AFN);
                                        flag = true;
                                    }
                                    break;
                                case 2:
                                    res = (CmdResponseToDtuSetYearExploitation)ToDtuCommand.GetBaseMessageFromDtuByKey(cmd.AddressField + "-" + cmd.AFN);
                                    if (res != null)
                                    {
                                        SafeSetText(textBox2, Environment.NewLine + OperationType + "成功！" + timeDelay + "，返回结果：" + ((CmdResponseToDtuSetYearExploitation)res).YearExploitation, true);
                                        ToDtuCommand.RemoveBaseMessageFromDtu(cmd.AddressField + "-" + cmd.AFN);
                                        flag = true;
                                    }
                                    break;
                                case 3:
                                    res = (CmdResponseToDtuQueryDateTime)ToDtuCommand.GetBaseMessageFromDtuByKey(cmd.AddressField + "-" + cmd.AFN);
                                    if (res != null)
                                    {
                                        SafeSetText(textBox2, Environment.NewLine + OperationType + "成功！" + timeDelay + "，返回结果：" + ((CmdResponseToDtuQueryDateTime)res).DateTimeNew.ToString("yyyy-MM-dd HH:mm:ss"), true);
                                        ToDtuCommand.RemoveBaseMessageFromDtu(cmd.AddressField + "-" + cmd.AFN);
                                        flag = true;
                                    }
                                    break;
                                case 4:
                                    res = (CmdResponseToDtuQueryYearExploitation)ToDtuCommand.GetBaseMessageFromDtuByKey(cmd.AddressField + "-" + cmd.AFN);
                                    if (res != null)
                                    {
                                        SafeSetText(textBox2, Environment.NewLine + OperationType + "成功！" + timeDelay + "，返回结果：" + ((CmdResponseToDtuQueryYearExploitation)res).YearExploitation, true);
                                        ToDtuCommand.RemoveBaseMessageFromDtu(cmd.AddressField + "-" + cmd.AFN);
                                        flag = true;
                                    }
                                    break;
                                case 5:
                                    res = (CmdResponseToDtuOpenPump)ToDtuCommand.GetBaseMessageFromDtuByKey(cmd.AddressField + "-" + cmd.AFN);
                                    if (res != null)
                                    {
                                        SafeSetText(textBox2, Environment.NewLine + OperationType + "成功！" + timeDelay + "，返回结果：" + ((CmdResponseToDtuOpenPump)res).Result, true);
                                        ToDtuCommand.RemoveBaseMessageFromDtu(cmd.AddressField + "-" + cmd.AFN);
                                        flag = true;
                                    }
                                    break;
                                case 6:
                                    res = (CmdResponseToDtuClosePump)ToDtuCommand.GetBaseMessageFromDtuByKey(cmd.AddressField + "-" + cmd.AFN);
                                    if (res != null)
                                    {
                                        SafeSetText(textBox2, Environment.NewLine + OperationType + "成功！" + timeDelay + "，返回结果：" + ((CmdResponseToDtuClosePump)res).Result, true);
                                        ToDtuCommand.RemoveBaseMessageFromDtu(cmd.AddressField + "-" + cmd.AFN);
                                        flag = true;
                                    }
                                    break;
                                case 7:
                                    res = (CmdResponseToDtuSetStationCode)ToDtuCommand.GetBaseMessageFromDtuByKey(cmd.AddressField + "-" + cmd.AFN);
                                    if (res != null)
                                    {
                                        SafeSetText(textBox2, Environment.NewLine + OperationType + "成功！" + timeDelay + "，返回结果：" + ((CmdResponseToDtuSetStationCode)res).Result, true);
                                        ToDtuCommand.RemoveBaseMessageFromDtu(cmd.AddressField + "-" + cmd.AFN);
                                        flag = true;
                                    }
                                    break;
                                case 8:
                                    res = (CmdResponseToDtuShieldSerialNumber)ToDtuCommand.GetBaseMessageFromDtuByKey(cmd.AddressField + "-" + cmd.AFN);
                                    if (res != null)
                                    {
                                        SafeSetText(textBox2, Environment.NewLine + OperationType + "成功！" + timeDelay + "，返回结果：" + ((CmdResponseToDtuShieldSerialNumber)res).Result, true);
                                        ToDtuCommand.RemoveBaseMessageFromDtu(cmd.AddressField + "-" + cmd.AFN);
                                        flag = true;
                                    }
                                    break;
                                case 9:
                                    res = (CmdResponseToDtuShieldSerialNumberCancel)ToDtuCommand.GetBaseMessageFromDtuByKey(cmd.AddressField + "-" + cmd.AFN);
                                    if (res != null)
                                    {
                                        SafeSetText(textBox2, Environment.NewLine + OperationType + "成功！" + timeDelay + "，返回结果：" + ((CmdResponseToDtuShieldSerialNumberCancel)res).Result, true);
                                        ToDtuCommand.RemoveBaseMessageFromDtu(cmd.AddressField + "-" + cmd.AFN);
                                        flag = true;
                                    }
                                    break;
                                case 10:
                                    res = (CmdResponseToDtuSetGroundWaterParam)ToDtuCommand.GetBaseMessageFromDtuByKey(cmd.AddressField + "-" + cmd.AFN);
                                    if (res != null)
                                    {
                                        SafeSetText(textBox2, Environment.NewLine + OperationType + "成功！" + timeDelay + "，返回结果：" + ((CmdResponseToDtuSetGroundWaterParam)res).Result, true);
                                        ToDtuCommand.RemoveBaseMessageFromDtu(cmd.AddressField + "-" + cmd.AFN);
                                        flag = true;
                                    }
                                    break;
                                case 11:
                                    res = (CmdResponseToDtuQueryGroundWaterParam)ToDtuCommand.GetBaseMessageFromDtuByKey(cmd.AddressField + "-" + cmd.AFN);
                                    if (res != null)
                                    {
                                        SafeSetText(textBox2, Environment.NewLine + OperationType + "成功！" + timeDelay + "，返回结果：" + ((CmdResponseToDtuQueryGroundWaterParam)res).Range + " | " + ((CmdResponseToDtuQueryGroundWaterParam)res).LineLength, true);
                                        SafeSetText(txtRange, ((CmdResponseToDtuQueryGroundWaterParam)res).Range.ToString(), false);
                                        SafeSetText(txtLineLength, ((CmdResponseToDtuQueryGroundWaterParam)res).LineLength.ToString(), false);
                                        ToDtuCommand.RemoveBaseMessageFromDtu(cmd.AddressField + "-" + cmd.AFN);
                                        flag = true;
                                    }
                                    break;
                                case 12:
                                    res = (CmdResponseToDtuQueryGroundWater)ToDtuCommand.GetBaseMessageFromDtuByKey(cmd.AddressField + "-" + cmd.AFN);
                                    if (res != null)
                                    {
                                        SafeSetText(textBox2, Environment.NewLine + OperationType + "成功！" + timeDelay + "，返回结果：" + ((CmdResponseToDtuQueryGroundWater)res).Acq_Time.ToString("yyyy-MM-dd HH:mm:ss") + " | " + ((CmdResponseToDtuQueryGroundWater)res).GroundWaterLevel + " | " + ((CmdResponseToDtuQueryGroundWater)res).LineLength, true);
                                        SafeSetText(txtAcq_Time, ((CmdResponseToDtuQueryGroundWater)res).Acq_Time.ToString("yyyy-MM-dd HH:mm:ss"), false);
                                        SafeSetText(txtGroundWaterLevel, ((CmdResponseToDtuQueryGroundWater)res).GroundWaterLevel.ToString(), false);
                                        SafeSetText(txtLineLength, ((CmdResponseToDtuQueryGroundWater)res).LineLength.ToString(), false);
                                        ToDtuCommand.RemoveBaseMessageFromDtu(cmd.AddressField + "-" + cmd.AFN);
                                        flag = true;
                                    }
                                    break;
                                case 13:
                                    res = (CmdResponseToDtuQueryState)ToDtuCommand.GetBaseMessageFromDtuByKey(cmd.AddressField + "-" + cmd.AFN);
                                    if (res != null)
                                    {
                                        SafeSetText(textBox2, Environment.NewLine + OperationType + "成功！" + timeDelay + "，返回结果：" + ((CmdResponseToDtuQueryState)res).State + 
                                            "，上报时间：" + ((CmdResponseToDtuQueryState)res).DateTimeNew.ToString("yyyy-MM-dd HH:mm:ss") + 
                                            //"，累计用电量：" + ((CmdResponseToDtuQueryState)res).ElectricUsed + 
                                            "，累计用水量：" + ((CmdResponseToDtuQueryState)res).WaterUsed, true);
                                        ToDtuCommand.RemoveBaseMessageFromDtu(cmd.AddressField + "-" + cmd.AFN);
                                        flag = true;
                                    }
                                    break;
                            }
                            if (flag)
                            {
                                if (res != null)
                                {
                                    SafeSetText(textBox2, Environment.NewLine + "是否带密码：" + (res.IsPW ? "1|" + res.PW : "0") + "，是否带时间戳：" + (res.IsTP ? "1|" + res.TP : "0"), true);
                                }
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
            SafeSetButton(button3, isEnable);
            SafeSetButton(button4, isEnable);
            SafeSetButton(button5, isEnable);
            SafeSetButton(button6, isEnable);
            SafeSetButton(button9, isEnable);
            SafeSetButton(button10, isEnable);
            SafeSetButton(button7, isEnable);
            SafeSetButton(button8, isEnable);
            SafeSetButton(button11, isEnable);
            SafeSetButton(button12, isEnable);
            SafeSetButton(button13, isEnable);
            SafeSetButton(button14, isEnable);
            SafeSetButton(button15, isEnable);

            isRun = !isEnable;

            if (isRun)
            {
                SafeSetLabel(this.label7, "", false);

                date1 = DateTime.Now;
                string s1 = "开始时间：" + date1.ToString("yyyy-MM-dd HH:mm:ss.fff");
                SafeSetLabel(this.label6, s1, false);

                Thread thTime = new Thread(new ThreadStart(time));
                thTime.Start();
            }
            else
            {
                date2 = DateTime.Now;
                string s2 = "结束时间：" + date2.ToString("yyyy-MM-dd HH:mm:ss.fff");
                SafeSetLabel(this.label6, Environment.NewLine + s2 + Environment.NewLine + "耗时：" + (date2 - date1).TotalSeconds + "秒", true);
            }
        }
        #endregion

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

        private void button8_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(new ParameterizedThreadStart(send));
            t.Start(8);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(new ParameterizedThreadStart(send));
            t.Start(9);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(new ParameterizedThreadStart(send));
            t.Start(10);
        }

        private void button13_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(new ParameterizedThreadStart(send));
            t.Start(11);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(new ParameterizedThreadStart(send));
            t.Start(12);
        }

        private void button15_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(new ParameterizedThreadStart(send));
            t.Start(13);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.comboBox1.SelectedIndex < 1)
            {
                SafeSetText(textBox2, "请选择一个终端", false);
                setAllButtonEnable(true);
                return;
            }
            string deviceNo = this.comboBox1.Text.ToString().Trim();
            Device device = DeviceModule.GetDeviceByFullDeviceNo(deviceNo);
            string resDeviceNo = DeviceModule.GetDeviceNoMain(deviceNo);
            Device resDevice = DeviceModule.GetDeviceByFullDeviceNo(resDeviceNo);
            if (resDeviceNo.Length != 15)
            {
                SafeSetText(textBox2, "终端编号错误！" + resDeviceNo, false);
                setAllButtonEnable(true);
                return;
            }

            CmdToDtuOpenPump cmd5 = new CmdToDtuOpenPump();
            cmd5.AddressField = DeviceModule.DeviceNo_Normal2Hex(deviceNo);
            cmd5.StationType = (byte)device.StationType;
            cmd5.StationCode = device.StationType == 2 ? device.StationCode : 0;
            cmd5.RawDataChar = cmd5.WriteMsg(dateTimePicker1.Value);
            cmd5.RawDataStr = HexStringUtility.ByteArrayToHexString(cmd5.RawDataChar);

            this.textBox2.Text = cmd5.RawDataStr;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.comboBox1.SelectedIndex < 1)
            {
                SafeSetText(textBox2, "请选择一个终端", false);
                setAllButtonEnable(true);
                return;
            }
            string deviceNo = this.comboBox1.Text.ToString().Trim();
            Device device = DeviceModule.GetDeviceByFullDeviceNo(deviceNo);
            string resDeviceNo = DeviceModule.GetDeviceNoMain(deviceNo);
            Device resDevice = DeviceModule.GetDeviceByFullDeviceNo(resDeviceNo);
            if (resDeviceNo.Length != 15)
            {
                SafeSetText(textBox2, "终端编号错误！" + resDeviceNo, false);
                setAllButtonEnable(true);
                return;
            }

            CmdToDtuClosePump cmd6 = new CmdToDtuClosePump();
            cmd6.AddressField = DeviceModule.DeviceNo_Normal2Hex(deviceNo);
            cmd6.StationType = (byte)device.StationType;
            cmd6.StationCode = device.StationType == 2 ? device.StationCode : 0;
            cmd6.RawDataChar = cmd6.WriteMsg(dateTimePicker1.Value);
            cmd6.RawDataStr = HexStringUtility.ByteArrayToHexString(cmd6.RawDataChar);

            this.textBox2.Text = cmd6.RawDataStr;
        }
    }
}
