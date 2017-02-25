using Common;
using System;
using System.Collections.Generic;

namespace DTU.GateWay.Protocol
{
    public class BaseMessage
    {
        public log4net.ILog logHelper = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static bool ShowLog = false;

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
        /// 起始字符
        /// </summary>
        public byte BeginChar
        {
            get;
            set;
        }

        /// <summary>
        /// 起始字符
        /// </summary>
        public byte EndChar
        {
            get;
            set;
        }

        /// <summary>
        /// 长度
        /// </summary>
        public int Length
        {
            set;
            get;
        }

        /// <summary>
        /// 控制域C,0x33代表控制终端接收数据；0xB3代表控制终端发送数据
        /// </summary>
        public byte ControlField
        {
            set;
            get;
        }

        /// <summary>
        /// 地址域
        /// </summary>
        public string AddressField
        {
            set;
            get;
        }

        /// <summary>
        /// 用户数据
        /// </summary>
        public string UserData
        {
            set;
            get;
        }

        /// <summary>
        /// 用户数据
        /// </summary>
        public byte[] UserDataBytes
        {
            set;
            get;
        }

        /// <summary>
        /// 校验码
        /// </summary>
        public byte CC
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
        /// 设备类型，单、主、从
        /// </summary>
        public byte StationType
        {
            set;
            get;
        }

        /// <summary>
        /// 射频地址 2字节
        /// </summary>
        public int StationCode
        {
            set;
            get;
        }

        /// <summary>
        /// 是否包含附加信息域-密码
        /// </summary>
        public bool IsPW
        {
            set;
            get;
        }

        /// <summary>
        /// 附加信息域-密码
        /// </summary>
        public string PW
        {
            set;
            get;
        }

        /// <summary>
        /// 是否包含附加信息域-时间标签
        /// </summary>
        public bool IsTP
        {
            set;
            get;
        }

        /// <summary>
        /// 附加信息域-时间标签
        /// </summary>
        public string TP
        {
            set;
            get;
        }

        public virtual byte[] WriteMsg() {
            DateTime dateNow = DateTime.Now;
            List<byte> list = new List<byte>();
            List<byte> BodyList = new List<byte>();

            BodyList.Add(ControlField);
            BodyList.AddRange(HexStringUtility.HexStringToByteArray(AddressField));
            BodyList.Add(StationType);
            BodyList.AddRange(HexStringUtility.HexStringToByteArray(StationCode.ToString("X").PadLeft(4, '0')));
            BodyList.Add(AFN);
            BodyList.AddRange(UserDataBytes);
            if (IsPW) BodyList.AddRange(HexStringUtility.HexStringToByteArray(PW));
            if (IsTP) BodyList.AddRange(HexStringUtility.HexStringToByteArray(TP));

            Length = BodyList.Count;

            CC = FormatHelper.GetXorByte(BodyList.ToArray(), 0, BodyList.ToArray().Length);

            list.Add(BeginChar);
            list.Add(FormatHelper.IntToByte(Length));
            //list.Add(BeginChar);
            byte bState = 0;
            if (IsPW) bState += 1;
            if (IsTP) bState += 2;
            list.Add(bState);
            list.AddRange(BodyList);
            list.Add(CC);
            list.Add(EndChar);

            return list.ToArray();
        }

        public byte[] WriteMsg2()
        {
            DateTime dateNow = DateTime.Now;
            List<byte> list = new List<byte>();
            List<byte> BodyList = new List<byte>();

            BodyList.Add(ControlField);
            BodyList.AddRange(HexStringUtility.HexStringToByteArray(AddressField));
            BodyList.Add(StationType);
            BodyList.AddRange(HexStringUtility.HexStringToByteArray(StationCode.ToString("X").PadLeft(4, '0')));
            BodyList.Add(AFN);
            BodyList.AddRange(UserDataBytes);
            if (IsPW) BodyList.AddRange(HexStringUtility.HexStringToByteArray(PW));
            if (IsTP) BodyList.AddRange(HexStringUtility.HexStringToByteArray(TP));

            Length = BodyList.Count;

            CC = FormatHelper.GetXorByte(BodyList.ToArray(), 0, BodyList.ToArray().Length);

            list.Add(BeginChar);
            list.Add(FormatHelper.IntToByte(Length));
            //list.Add(BeginChar);
            byte bState = 0;
            if (IsPW) bState += 1;
            if (IsTP) bState += 2;
            list.Add(bState);
            list.AddRange(BodyList);
            list.Add(CC);
            list.Add(EndChar);

            return list.ToArray();
        }

        public virtual string ReadMsg()
        {
            string msg = "";
            if (RawDataChar == null)
            {
                return "无数据";
            }

            if (RawDataChar.Length >= 14)
            {
                if (RawDataChar[0] == BaseProtocol.BeginChar && RawDataChar[RawDataChar.Length - 1] == BaseProtocol.EndChar)
                {
                    Length = (int)RawDataChar[1];
                    byte[] Body_b = new byte[Length];
                    byte bState = RawDataChar[2];

                    IsPW = bState % 2 > 0;
                    IsTP = bState % 4 > 2;

                    Array.Copy(RawDataChar, 3, Body_b, 0, Body_b.Length);
                    CC = RawDataChar[RawDataChar.Length - 2];
                    byte CCN = FormatHelper.GetXorByte(Body_b, 0, Body_b.Length);
                    if (CCN == CC)
                    {
                        ControlField = RawDataChar[3];
                        //if (ControlField == (byte)BaseProtocol.ControlField.FromDtu)
                        if (Enum.IsDefined(typeof(BaseProtocol.ControlField), (int)ControlField))
                        {
                            StationType = RawDataChar[11];
                            StationCode = Convert.ToInt32(HexStringUtility.ByteArrayToHexString(new byte[] { RawDataChar[12], RawDataChar[13] }), 16);
                            AFN = RawDataChar[14];
                            if (Enum.IsDefined(typeof(BaseProtocol.AFN), (int)AFN))
                            {
                                byte[] AddressField_b = new byte[7];
                                Array.Copy(RawDataChar, 4, AddressField_b, 0, 7);
                                AddressField = HexStringUtility.ByteArrayToHexString(AddressField_b);

                                //用户数据长度为长度减控制域1字节减地址域7字节减应用层功能码1字节减设备类型1字节减射频地址2字节减密码减时间戳
                                UserDataBytes = new byte[Length - 1 - 7 - 1 - 1 - 2 - (IsTP ? 2 : 0) - (IsPW ? 5 : 0)];
                                Array.Copy(RawDataChar, 15, UserDataBytes, 0, UserDataBytes.Length);
                                UserData = HexStringUtility.ByteArrayToHexString(UserDataBytes);

                                try
                                {
                                    if (IsTP)
                                    {
                                        byte[] TPBytes = new byte[5];
                                        Array.Copy(RawDataChar, RawDataChar.Length - 2 - 5, TPBytes, 0, TPBytes.Length);
                                        TP = HexStringUtility.ByteArrayToHexString(TPBytes);
                                    }
                                    if (IsPW)
                                    {
                                        byte[] PWBytes = new byte[2];
                                        Array.Copy(RawDataChar, RawDataChar.Length - 2 - 2 - (IsTP ? 5 : 0), PWBytes, 0, PWBytes.Length);
                                        PW = HexStringUtility.ByteArrayToHexString(PWBytes);
                                    }
                                }
                                catch
                                {
                                    msg = "密码时间戳格式不正确，IsPW：" + (IsPW ? "yes" : "no") + "，IsTP：" + (IsTP ? "yes" : "no");
                                }
                            }
                            else
                            {
                                msg = "功能码不对，" + AFN.ToString("X");
                            }
                        }
                        else
                        {
                            msg = "控制域不对，" + ControlField.ToString("X");
                        }
                    }
                    else
                    {
                        msg = "校验码不对，" + CC;
                    }
                }
                else
                {
                    msg = "开始结束字符不对，" + RawDataChar[0].ToString("X") + " " + RawDataChar[RawDataChar.Length - 1].ToString("X");
                }
            }
            else
            {
                msg = "长度不足14，" + RawDataChar.Length;
            }

            return msg;
        }
    }
}
