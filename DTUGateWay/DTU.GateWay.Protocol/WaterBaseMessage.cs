using Common;
using System;
using System.Collections.Generic;

namespace DTU.GateWay.Protocol
{
    public class WaterBaseMessage
    {
        public log4net.ILog logHelper = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static bool ShowLog = false;

        public WaterBaseMessage()
        {
            BeginChar = new byte[2];
            Array.Copy(WaterBaseProtocol.BeginChar, 0, BeginChar, 0, 2);
            PW = WaterBaseProtocol.PW;
            TotalPackage = 0;
            CurrentPackage = 0;
        }

        public bool IsWaterMessage
        {
            get;
            set;
        }

        /// <summary>
        /// 原始报文，16进制字符串
        /// </summary>
        public string RawDataStr
        {
            set;
            get;
        }

        /// <summary>
        /// 原始报文，字节数组
        /// </summary>
        public byte[] RawDataChar
        {
            set;
            get;
        }

        /// <summary>
        /// 帧起始符，2字节
        /// </summary>
        public byte[] BeginChar
        {
            get;
            set;
        }

        /// <summary>
        /// 中心站地址
        /// </summary>
        public byte CenterStation
        {
            get;
            set;
        }

        /// <summary>
        /// 遥测站地址，5字节，BCD码
        /// </summary>
        public string RemoteStation
        {
            get;
            set;
        }

        /// <summary>
        /// 密码，2字节
        /// </summary>
        public string PW
        {
            set;
            get;
        }

        /// <summary>
        /// 应用层功能码
        /// </summary>
        public byte AFN
        {
            set;
            get;
        }

        /// <summary>
        /// 长度，报文正文长度，1-4095
        /// </summary>
        public int Length
        {
            set;
            get;
        }

        /// <summary>
        /// 上下行标识，0表示上行，8表示下行
        /// </summary>
        public int UpOrDown
        {
            set;
            get;
        }

        /// <summary>
        /// 报文起始符，STX（02H）或SYN（16H）
        /// </summary>
        public byte DataBeginChar
        {
            get;
            set;
        }

        /// <summary>
        /// 包总数，1-4095
        /// </summary>
        public int TotalPackage
        {
            set;
            get;
        }

        /// <summary>
        /// 序列号，1-4095
        /// </summary>
        public int CurrentPackage
        {
            set;
            get;
        }

        /// <summary>
        /// 报文正文
        /// </summary>
        public string UserData
        {
            set;
            get;
        }

        /// <summary>
        /// 报文正文
        /// </summary>
        public byte[] UserDataBytes
        {
            set;
            get;
        }

        /// <summary>
        /// 报文结束符，上行ETB（17H）或ETX（03H），下行ENQ（05H）或ACK（06H）或NAK（15H）或EOT（04H）或ESC（1BH）
        /// </summary>
        public byte DataEndChar
        {
            get;
            set;
        }

        /// <summary>
        /// 校验码，2字节
        /// </summary>
        public byte[] CC
        {
            set;
            get;
        }

        private bool isCorrect = false;
        private byte ROW = 0;

        public string WriteMsgBase()
        {
            isCorrect = false;
            ROW = 1;

            if (!Enum.IsDefined(typeof(WaterBaseProtocol.UpOrDown), (int)UpOrDown))
            {
                if (ShowLog) logHelper.Error("上下行标识非法，0或8！" + UpOrDown);
                return "上下行标识非法，0或8！" + UpOrDown;
            }

            if (DataBeginChar == (byte)WaterBaseProtocol.DataBeginChar.SYN)
            {
                if (TotalPackage > 0xFFF || TotalPackage < 0)
                {
                    if (ShowLog) logHelper.Error("包总数非法，0-4095！" + TotalPackage);
                    return "包总数非法，0-4095！" + TotalPackage;
                }

                if (CurrentPackage > 0xFFF || CurrentPackage < 0)
                {
                    if (ShowLog) logHelper.Error("序列号非法，0-4095！" + CurrentPackage);
                    return "序列号非法，0-4095！" + CurrentPackage;
                }

                Length = 3 + UserDataBytes.Length;
            }
            else
            {
                DataBeginChar = (byte)WaterBaseProtocol.DataBeginChar.STX;
                Length = UserDataBytes.Length;
            }

            if (Length > 0xFFF || Length < 0x001)
            {
                if (ShowLog) logHelper.Error("长度非法，0-4095！" + Length);
                return "长度非法，0-4095！" + Length;
            }

            try
            {
                List<byte> list = new List<byte>();

                list.AddRange(BeginChar);
                if (UpOrDown == (int)WaterBaseProtocol.UpOrDown.Up)
                {
                    list.Add(CenterStation);
                    list.AddRange(HexStringUtility.HexStringToByteArray(RemoteStation));
                }
                else if (UpOrDown == (int)WaterBaseProtocol.UpOrDown.Down)
                {
                    list.AddRange(HexStringUtility.HexStringToByteArray(RemoteStation));
                    list.Add(CenterStation);
                }
                list.AddRange(HexStringUtility.HexStringToByteArray(PW));
                list.Add(AFN);

                list.AddRange(HexStringUtility.HexStringToByteArray(UpOrDown.ToString() + Convert.ToString(Length, 16).ToUpper().PadLeft(3, '0')));
                list.Add(DataBeginChar);
                if (DataBeginChar == (byte)WaterBaseProtocol.DataBeginChar.SYN)
                {
                    list.AddRange(HexStringUtility.HexStringToByteArray(Convert.ToString(TotalPackage, 16).ToUpper().PadLeft(3, '0') +
                        Convert.ToString(CurrentPackage, 16).ToUpper().PadLeft(3, '0')));
                }
                if (UserDataBytes.Length > 0)
                {
                    list.AddRange(UserDataBytes);
                }
                list.Add(DataEndChar);
                CC = CRC.crc16(list.ToArray());
                Array.Reverse(CC);
                list.AddRange(CC);

                RawDataChar = list.ToArray();
                RawDataStr = HexStringUtility.ByteArrayToHexString(RawDataChar).ToUpper();
                UserData = HexStringUtility.ByteArrayToHexString(UserDataBytes).ToUpper();

                isCorrect = true;
                return "";
            }
            catch (Exception ex)
            {
                if (ShowLog) logHelper.Error("出错！" + ex.Message);
                return "出错！" + ex.Message;
            }
        }

        public string ReadMsgBase()
        {
            isCorrect = false;
            ROW = 2;
            IsWaterMessage = false;

            try
            {
                if (RawDataChar == null)
                {
                    if (ShowLog) logHelper.Error("无法获取信息！");
                    return "无法获取信息！";
                }
                if (RawDataChar.Length < 18)
                {
                    if (ShowLog) logHelper.Error("无法获取信息长度不够！");
                    return "无法获取信息长度不够！";
                }

                BeginChar = new byte[2];
                Array.Copy(RawDataChar, 0, BeginChar, 0, 2);
                if (BeginChar[0] != WaterBaseProtocol.BeginChar[0] || BeginChar[1] != WaterBaseProtocol.BeginChar[1])
                {
                    if (ShowLog) logHelper.Error("起始字符非法！" + HexStringUtility.ByteArrayToHexString(BeginChar).ToUpper());
                    return "起始字符非法！" + HexStringUtility.ByteArrayToHexString(BeginChar).ToUpper();
                }

                byte[] CCP = new byte[RawDataChar.Length - 2];
                Array.Copy(RawDataChar, 0, CCP, 0, CCP.Length);
                byte[] CCC = CRC.crc16(CCP);
                Array.Reverse(CCC);
                CC = new byte[2];
                Array.Copy(RawDataChar, RawDataChar.Length - 2, CC, 0, 2);
                if (CCC[0] != CC[0] || CCC[1] != CC[1])
                {
                    if (ShowLog) logHelper.Error("校验码非法！" + HexStringUtility.ByteArrayToHexString(CC).ToUpper() + "|" + HexStringUtility.ByteArrayToHexString(CCC).ToUpper());
                    return "校验码非法！" + HexStringUtility.ByteArrayToHexString(CC).ToUpper() + "|" + HexStringUtility.ByteArrayToHexString(CCC).ToUpper();
                }

                IsWaterMessage = true;

                AFN = RawDataChar[10];
                if (!Enum.IsDefined(typeof(WaterBaseProtocol.AFN), (int)AFN))
                {
                    if (ShowLog) logHelper.Error("功能码非法！" + Convert.ToString(AFN, 16));
                    return "功能码非法！" + Convert.ToString(AFN, 16);
                }

                DataBeginChar = RawDataChar[13];
                if (!Enum.IsDefined(typeof(WaterBaseProtocol.DataBeginChar), (int)DataBeginChar))
                {
                    if (ShowLog) logHelper.Error("报文起始符非法！" + Convert.ToString(DataBeginChar, 16));
                    return "报文起始符非法！" + Convert.ToString(DataBeginChar, 16);
                }

                byte[] bs_UpOrDown_Length = new byte[2];
                Array.Copy(RawDataChar, 11, bs_UpOrDown_Length, 0, 2);
                string str_UpOrDown_Length = HexStringUtility.ByteArrayToHexString(bs_UpOrDown_Length).ToUpper();
                string str_UpOrDown = str_UpOrDown_Length.Substring(0, 1);
                UpOrDown = Convert.ToInt32(str_UpOrDown, 16);
                if (!Enum.IsDefined(typeof(WaterBaseProtocol.UpOrDown), (int)UpOrDown))
                {
                    if (ShowLog) logHelper.Error("上下行标识非法！" + Convert.ToString(UpOrDown, 16));
                    return "上下行标识非法！" + Convert.ToString(UpOrDown, 16);
                }

                DataEndChar = RawDataChar[RawDataChar.Length - 3];
                if (UpOrDown == (int)WaterBaseProtocol.UpOrDown.Up)
                {
                    if (!Enum.IsDefined(typeof(WaterBaseProtocol.DataEndChar_Up), (int)DataEndChar))
                    {
                        if (ShowLog) logHelper.Error("上行报文结束符非法！" + Convert.ToString(DataBeginChar, 16));
                        return "上行报文结束符非法！" + Convert.ToString(DataBeginChar, 16);
                    }
                }
                else if (UpOrDown == (int)WaterBaseProtocol.UpOrDown.Down)
                {
                    if (!Enum.IsDefined(typeof(WaterBaseProtocol.DataEndChar_Down), (int)DataEndChar))
                    {
                        if (ShowLog) logHelper.Error("下行报文结束符非法！" + Convert.ToString(DataBeginChar, 16));
                        return "下行报文结束符非法！" + Convert.ToString(DataBeginChar, 16);
                    }
                }

                string str_Length = str_UpOrDown_Length.Substring(1, 3);
                Length = Convert.ToInt32(str_Length, 16);

                byte[] bs_RemoteStation = new byte[5];
                if (UpOrDown == (int)WaterBaseProtocol.UpOrDown.Up)
                {
                    CenterStation = RawDataChar[2];
                    Array.Copy(RawDataChar, 3, bs_RemoteStation, 0, 5);
                }
                else if (UpOrDown == (int)WaterBaseProtocol.UpOrDown.Down)
                {
                    Array.Copy(RawDataChar, 2, bs_RemoteStation, 0, 5);
                    CenterStation = RawDataChar[7];
                }
                RemoteStation = HexStringUtility.ByteArrayToHexString(bs_RemoteStation).ToUpper();

                byte[] bs_PW = new byte[2];
                Array.Copy(RawDataChar, 8, bs_PW, 0, 2);
                PW = HexStringUtility.ByteArrayToHexString(bs_PW).ToUpper();

                if (DataBeginChar == 0x16)
                {
                    byte[] bs_TotalPackage_CurrentPackage = new byte[3];
                    Array.Copy(RawDataChar, 13 + 1, bs_TotalPackage_CurrentPackage, 0, 3);
                    string str_TotalPackage_CurrentPackage = HexStringUtility.ByteArrayToHexString(bs_TotalPackage_CurrentPackage).ToUpper();
                    string str_TotalPackage = str_TotalPackage_CurrentPackage.Substring(0, 3);
                    TotalPackage = Convert.ToInt32(str_TotalPackage, 16);
                    string str_CurrentPackage = str_TotalPackage_CurrentPackage.Substring(3, 3);
                    CurrentPackage = Convert.ToInt32(str_CurrentPackage, 16);
                    UserDataBytes = new byte[Length - 3];
                    Array.Copy(RawDataChar, 13 + 1 + 3, UserDataBytes, 0, UserDataBytes.Length);
                }
                else
                {
                    TotalPackage = 0;
                    CurrentPackage = 0;
                    if (Length > 0)
                    {
                        UserDataBytes = new byte[Length];
                        Array.Copy(RawDataChar, 13 + 1, UserDataBytes, 0, UserDataBytes.Length);
                    }
                    else
                    {
                        UserDataBytes = new byte[0];
                    }
                }
                UserData = HexStringUtility.ByteArrayToHexString(UserDataBytes).ToUpper();

                isCorrect = true;
                return "";
            }
            catch (Exception ex)
            {
                if (ShowLog) logHelper.Error("出错！" + ex.Message);
                return "出错！" + ex.Message;
            }
        }

        public override string ToString()
        {
            string str = "";
            if (!isCorrect)
            {
                if (ROW == 1)
                    str = "写信息失败！";
                if (ROW == 2)
                    str = "读信息失败！";
            }
            else
            {
                if (ROW == 1)
                    str = "写信息成功！";
                if (ROW == 2)
                    str = "读信息成功！";

                str += Environment.NewLine + (UpOrDown == (int)WaterBaseProtocol.UpOrDown.Down ? "下行" : "上行") + "，" + "中心站地址：" + Convert.ToString(CenterStation, 16) + "，" + "遥测站地址：" + RemoteStation;
                str += Environment.NewLine + "密码：" + PW + "，" + "功能码：" + Convert.ToString(AFN, 16) + "，" + "长度：" + Length;
                str += Environment.NewLine + "报文起始符：" + Convert.ToString(DataBeginChar, 16) + "，" + "报文结束符：" + Convert.ToString(DataEndChar, 16);
                str += Environment.NewLine + "包总数及序列号：" + Convert.ToString(TotalPackage, 16) + "，" + Convert.ToString(CurrentPackage, 16);
                str += Environment.NewLine + "报文正文：" + Environment.NewLine + UserData;
            }
            return str;
        }
    }
}
