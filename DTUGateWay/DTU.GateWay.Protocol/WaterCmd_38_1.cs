using Common;
using DTU.GateWay.Protocol.WaterMessageClass;
using System;
using System.Globalization;

namespace DTU.GateWay.Protocol
{
    public class WaterCmd_38_1 : WaterBaseMessage
    {
        public WaterCmd_38_1()
        {
            AFN = (byte)WaterBaseProtocol.AFN._38;
            UpOrDown = (int)WaterBaseProtocol.UpOrDown.Down;
            DataBeginChar = (byte)WaterBaseProtocol.DataBeginChar.STX;
            DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Down.ENQ;
            SerialNumber = 0;
        }

        /// <summary>
        /// 流水号
        /// </summary>
        public short SerialNumber
        {
            get;
            set;
        }

        /// <summary>
        /// 发报时间
        /// </summary>
        public DateTime SendTime
        {
            get;
            set;
        }

        /// <summary>
        /// 起始时间
        /// </summary>
        public DateTime StartTime
        {
            get;
            set;
        }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime
        {
            get;
            set;
        }

        /// <summary>
        /// 时间步长码
        /// </summary>
        public Identifier_04 Iden_04
        {
            get;
            set;
        }

        /// <summary>
        /// 要素标识符值
        /// </summary>
        public Identifier Iden
        {
            get;
            set;
        }

        public string WriteMsg()
        {
            try
            {
                if (Iden_04 == null)
                {
                    return "时间步长码出错";
                }

                if (Iden == null)
                {
                    return "要素标识符非法";
                }

                UserData = "";
                UserData += SerialNumber.ToString("X").PadLeft(4, '0');
                UserData += SendTime.ToString("yyMMddHHmmss").PadLeft(12, '0');

                UserData += StartTime.ToString("yyMMddHH").PadLeft(8, '0');
                UserData += EndTime.ToString("yyMMddHH").PadLeft(8, '0');

                UserData += Iden_04.GetHexStr();
                UserData += Iden.GetHexStrHead();

                UserDataBytes = HexStringUtility.HexStringToByteArray(UserData);
                return WriteMsgBase();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string ReadMsg()
        {
            if (UserDataBytes == null || UserDataBytes.Length == 0)
            {
                if (ShowLog) logHelper.Error("无信息，无法分析！");
                return "无信息，无法分析！";
            }

            UserData = HexStringUtility.ByteArrayToHexString(UserDataBytes);

            try
            {
                SerialNumber = Convert.ToInt16(UserData.Substring(0, 4), 16);
            }
            catch (Exception ex)
            {
                if (ShowLog) logHelper.Error(ex.Message + Environment.NewLine + "获取流水号出错" + " " + RawDataStr);
                return "获取流水号出错";
            }

            try
            {
                SendTime = DateTime.ParseExact("20" + UserData.Substring(4, 12), "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                if (ShowLog) logHelper.Error(ex.Message + Environment.NewLine + "获取发报时间出错" + " " + RawDataStr);
                return "获取发报时间出错";
            }

            try
            {
                StartTime = DateTime.ParseExact("20" + UserData.Substring(16, 8) + "0000", "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                if (ShowLog) logHelper.Error(ex.Message + Environment.NewLine + "获取起始时间出错" + " " + RawDataStr);
                return "获取起始时间出错";
            }

            try
            {
                EndTime = DateTime.ParseExact("20" + UserData.Substring(24, 8) + "0000", "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                if (ShowLog) logHelper.Error(ex.Message + Environment.NewLine + "获取结束时间出错" + " " + RawDataStr);
                return "获取结束时间出错";
            }

            try
            {
                byte key_04 = Convert.ToByte(UserData.Substring(32, 2), 16);
                if (key_04 != (int)Identifier_Standard._04)
                {
                    return "获取时间步长码出错";
                }
                string LengthStr_04 = UserData.Substring(34, 2);
                int[] LS_04 = WaterBaseProtocol.GetLengthList(LengthStr_04);
                int ByteCount_04 = LS_04[0];
                int Digits_04 = LS_04[1];

                Iden_04 = new Identifier_04();
                Iden_04.SetVal(ByteCount_04, Digits_04, UserData.Substring(36, 6));
            }
            catch (Exception ex)
            {
                if (ShowLog) logHelper.Error(ex.Message + Environment.NewLine + "获取时间步长码出错" + " " + RawDataStr);
                return "获取时间步长码出错";
            }

            try
            {
                byte Key = Convert.ToByte(UserData.Substring(42, 2), 16);
                if (Key == (byte)Identifier_Standard._FF)
                {
                    byte KeySub = Convert.ToByte(UserData.Substring(44, 2), 16);
                    if (KeySub == (byte)Identifier_Custom._03)
                    {
                        Iden = new Identifier_FF_03();
                    }
                    else if (KeySub == (byte)Identifier_Custom._0E)
                    {
                        Iden = new Identifier_FF_0E();
                    }
                }
                else
                {
                    if (Key == (byte)Identifier_Standard._03)
                    {
                        Iden = new Identifier_03();
                    }
                    else if (Key == (byte)Identifier_Standard._0E)
                    {
                        Iden = new Identifier_0E();
                    }
                    else if (Key == (byte)Identifier_Standard._1A)
                    {
                        Iden = new Identifier_1A();
                    }
                    else if (Key == (byte)Identifier_Standard._1F)
                    {
                        Iden = new Identifier_1F();
                    }
                    else if (Key == (byte)Identifier_Standard._20)
                    {
                        Iden = new Identifier_20();
                    }
                    else if (Key == (byte)Identifier_Standard._26)
                    {
                        Iden = new Identifier_26();
                    }
                    else if (Key == (byte)Identifier_Standard._38)
                    {
                        Iden = new Identifier_38();
                    }
                    else if (Key == (byte)Identifier_Standard._39)
                    {
                        Iden = new Identifier_39();
                    }
                    else if (Key == (byte)Identifier_Standard._F4)
                    {
                        Iden = new Identifier_F4();
                    }
                    else if (Key == (byte)Identifier_Standard._F5)
                    {
                        Iden = new Identifier_F5();
                    }
                }
            }
            catch (Exception ex)
            {
                if (ShowLog) logHelper.Error(ex.Message + Environment.NewLine + "获取要素标识符出错" + " " + RawDataStr);
                return "获取要素标识符出错";
            }

            return "";
        }
    }
}
