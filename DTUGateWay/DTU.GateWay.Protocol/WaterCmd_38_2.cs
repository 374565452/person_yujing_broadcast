using Common;
using DTU.GateWay.Protocol.WaterMessageClass;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace DTU.GateWay.Protocol
{
    public class WaterCmd_38_2 : WaterBaseMessage
    {
        public WaterCmd_38_2()
        {
            AFN = (byte)WaterBaseProtocol.AFN._38;
            UpOrDown = (int)WaterBaseProtocol.UpOrDown.Up;
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

        public Identifier_F1 Iden_F1
        {
            get;
            set;
        }

        public Identifier_F0 Iden_F0
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
        public List<Identifier> Idens
        {
            get;
            set;
        }

        /// <summary>
        /// 报文正文
        /// </summary>
        public string UserDataAll
        {
            set;
            get;
        }

        /// <summary>
        /// 报文正文
        /// </summary>
        public byte[] UserDataBytesAll
        {
            set;
            get;
        }

        public WaterBaseMessage[] MsgList
        {
            get;
            set;
        }

        public string WriteMsg()
        {
            if (Iden_04 == null)
            {
                return "时间步长码出错";
            }
            UserDataAll = "";
            UserDataAll += SerialNumber.ToString("X").PadLeft(4, '0');
            UserDataAll += SendTime.ToString("yyMMddHHmmss").PadLeft(12, '0');
            /*
            UserDataAll += HexStringUtility.ByteArrayToHexString(new byte[] { 0xF1, 0xF1 });
            UserDataAll += RemoteStation.PadLeft(10, '0');
            UserDataAll += StationType.ToString("X").PadLeft(2, '0');
             * */
            UserDataAll += Iden_F1.GetHexStr();
            /*
            UserDataAll += HexStringUtility.ByteArrayToHexString(new byte[] { 0xF0, 0xF0 });
            UserDataAll += ObsTime.ToString("yyMMddHHmm").PadLeft(10, '0');
             * */
            UserDataAll += Iden_F0.GetHexStr();
            UserDataAll += Iden_04.GetHexStr();
            if (Idens != null && Idens.Count > 0)
            {
                UserDataAll += Idens[0].GetHexStrHead();
                if (Idens[0].GetKey() != (byte)Identifier_Standard._FF)
                {
                    foreach (Identifier obj in Idens)
                    {
                        UserDataAll += obj.GetHexStr().Substring(4);
                    }
                }
                else
                {
                    foreach (Identifier obj in Idens)
                    {
                        UserDataAll += obj.GetHexStr().Substring(6);
                    }
                }
            }
            else
            {
                return "无有效数据";
            }

            byte[] UserDataBytesAllTmp;
            WaterBaseMessage[] MsgListTmp;
            string msg = WaterBaseMessageService.GetMsgList_WriteMsg(this, UserDataAll, out UserDataBytesAllTmp, out MsgListTmp);
            if (msg == "")
            {
                UserDataBytesAll = UserDataBytesAllTmp;
                MsgList = MsgListTmp;
            }
            else
            {
                if (ShowLog) logHelper.Error(msg);
            }
            return msg;
        }

        public string ReadMsg()
        {
            string UserDataAllTmp;
            byte[] UserDataBytesAllTmp;
            string msg = WaterBaseMessageService.ReadMsg(MsgList, out UserDataAllTmp, out UserDataBytesAllTmp);
            if (msg == "")
            {
                UserDataAll = UserDataAllTmp;
                UserDataBytesAll = UserDataBytesAllTmp;

                return ReadMsg(UserDataAll);
            }
            else
            {
                if (ShowLog) logHelper.Error(msg);
                return msg;
            }
        }

        public string ReadMsg(string UserDataAll)
        {
            short SerialNumberTmp;
            DateTime SendTimeTmp;
            string msg = WaterBaseMessageService.GetSerialNumberAndSendTime(UserDataAll, out SerialNumberTmp, out SendTimeTmp);
            if (msg == "")
            {
                SerialNumber = SerialNumberTmp;
                SendTime = SendTimeTmp;
            }
            else
            {
                if (ShowLog) logHelper.Error(msg);
                return msg;
            }

            try
            {
                //StationType = Convert.ToByte(UserDataAll.Substring(30, 2), 16);
                Iden_F1 = new Identifier_F1();
                Iden_F1.RemoteStation = UserDataAll.Substring(20, 10);
                Iden_F1.StationType = Convert.ToByte(UserDataAll.Substring(30, 2), 16);
            }
            catch (Exception ex)
            {
                if (ShowLog) logHelper.Error(ex.Message + Environment.NewLine + "获取遥测站出错" + " " + RawDataStr);
                return "获取遥测站出错";
            }

            try
            {
                //ObsTime = DateTime.ParseExact("20" + UserDataAll.Substring(36, 10) + "00", "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                Iden_F0 = new Identifier_F0();
                Iden_F0.ObsTime = DateTime.ParseExact("20" + UserDataAll.Substring(36, 10) + "00", "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                if (ShowLog) logHelper.Error(ex.Message + Environment.NewLine + "获取观测时间出错" + " " + RawDataStr);
                return "获取观测时间出错";
            }

            try
            {
                byte key_04 = Convert.ToByte(UserDataAll.Substring(46, 2),16);
                if (key_04 != (int)Identifier_Standard._04)
                {
                    return "获取时间步长码出错";
                }
                string LengthStr_04 = UserDataAll.Substring(48, 2);
                int[] LS_04 = WaterBaseProtocol.GetLengthList(LengthStr_04);
                int ByteCount_04 = LS_04[0];
                int Digits_04 = LS_04[1];

                Iden_04 = new Identifier_04();
                Iden_04.SetVal(ByteCount_04, Digits_04, UserDataAll.Substring(50, 6));
            }
            catch (Exception ex)
            {
                if (ShowLog) logHelper.Error(ex.Message + Environment.NewLine + "获取时间步长码出错" + " " + RawDataStr);
                return "获取时间步长码出错";
            }

            string Remain = UserDataAll.Substring(56).ToUpper();
            if (Remain.Length > 4)
            {
                byte Key = Convert.ToByte(Remain.Substring(0, 2), 16);
                Idens = new List<Identifier>();
                if (Key == (byte)Identifier_Standard._FF)
                {
                    if (Remain.Length > 6)
                    {
                        byte KeySub = Convert.ToByte(Remain.Substring(2, 2), 16);
                        Remain = Remain.Substring(6);
                        if (KeySub == (byte)Identifier_Custom._03)
                        {
                            while (Remain.Length >= 24)
                            {
                                Identifier_FF_03 id = new Identifier_FF_03();
                                id.SetVal(0, 0, Remain.Substring(0, 24));
                                Idens.Add(id);
                                Remain = Remain.Substring(24);
                            }
                        }
                        else if (KeySub == (byte)Identifier_Custom._0E)
                        {
                            while (Remain.Length >= 36)
                            {
                                Identifier_FF_0E id = new Identifier_FF_0E();
                                id.SetVal(0, 0, Remain.Substring(0, 36));
                                Idens.Add(id);
                                Remain = Remain.Substring(24);
                            }
                        }
                    }
                }
                else
                {
                    string LengthStr = Remain.Substring(2, 2);
                    int[] LS = WaterBaseProtocol.GetLengthList(LengthStr);
                    int ByteCount = LS[0];
                    int Digits = LS[1];
                    if (ByteCount > 0)
                    {
                        Remain = Remain.Substring(4);
                        if (Key == (byte)Identifier_Standard._1A)
                        {
                            while (Remain.Length >= ByteCount * 2)
                            {
                                Identifier_1A id = new Identifier_1A();
                                id.SetVal(ByteCount, Digits, Remain.Substring(0, ByteCount * 2));
                                Idens.Add(id);
                                Remain = Remain.Substring(ByteCount * 2);
                            }
                        }
                        else if (Key == (byte)Identifier_Standard._1F)
                        {
                            while (Remain.Length >= ByteCount * 2)
                            {
                                Identifier_1F id = new Identifier_1F();
                                id.SetVal(ByteCount, Digits, Remain.Substring(0, ByteCount * 2));
                                Idens.Add(id);
                                Remain = Remain.Substring(ByteCount * 2);
                            }
                        }
                        else if (Key == (byte)Identifier_Standard._20)
                        {
                            while (Remain.Length >= ByteCount * 2)
                            {
                                Identifier_20 id = new Identifier_20();
                                id.SetVal(ByteCount, Digits, Remain.Substring(0, ByteCount * 2));
                                Idens.Add(id);
                                Remain = Remain.Substring(ByteCount * 2);
                            }
                        }
                        else if (Key == (byte)Identifier_Standard._26)
                        {
                            while (Remain.Length >= ByteCount * 2)
                            {
                                Identifier_26 id = new Identifier_26();
                                id.SetVal(ByteCount, Digits, Remain.Substring(0, ByteCount * 2));
                                Idens.Add(id);
                                Remain = Remain.Substring(ByteCount * 2);
                            }
                        }
                        else if (Key == (byte)Identifier_Standard._38)
                        {
                            while (Remain.Length >= ByteCount * 2)
                            {
                                Identifier_38 id = new Identifier_38();
                                id.SetVal(ByteCount, Digits, Remain.Substring(0, ByteCount * 2));
                                Idens.Add(id);
                                Remain = Remain.Substring(ByteCount * 2);
                            }
                        }
                        else if (Key == (byte)Identifier_Standard._39)
                        {
                            while (Remain.Length >= ByteCount * 2)
                            {
                                Identifier_39 id = new Identifier_39();
                                id.SetVal(ByteCount, Digits, Remain.Substring(0, ByteCount * 2));
                                Idens.Add(id);
                                Remain = Remain.Substring(ByteCount * 2);
                            }
                        }
                        else if (Key == (byte)Identifier_Standard._F4)
                        {
                            while (Remain.Length >= ByteCount * 2)
                            {
                                Identifier_F4 id = new Identifier_F4();
                                id.SetVal(ByteCount, Digits, Remain.Substring(0, ByteCount * 2));
                                Idens.Add(id);
                                Remain = Remain.Substring(ByteCount * 2);
                            }
                        }
                        else if (Key == (byte)Identifier_Standard._F5)
                        {
                            while (Remain.Length >= ByteCount * 2)
                            {
                                Identifier_F5 id = new Identifier_F5();
                                id.SetVal(ByteCount, Digits, Remain.Substring(0, ByteCount * 2));
                                Idens.Add(id);
                                Remain = Remain.Substring(ByteCount * 2);
                            }
                        }

                    }
                }
            }
            return "";
        }

        public string ReadMsg(byte[] bs)
        {
            return ReadMsg(HexStringUtility.ByteArrayToHexString(bs));
        }

        public override string ToString()
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("【" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN) + "】：");
                sb.Append("【流水号】：" + SerialNumber + "，");
                sb.Append("【发报时间】：" + SendTime.ToString("yyyy-MM-dd HH:mm:ss") + "，");
                if (Iden_04 != null)
                {
                    sb.Append(Iden_04.ToString());
                }
                else
                {
                    sb.Append("时间步长码解析出错！");
                }
                
                if (Idens != null && Idens.Count > 0)
                {
                    foreach (Identifier id in Idens)
                    {
                        sb.Append(id.ToString());
                    }
                }
                else
                {
                    sb.Append("数据解析失败！");
                }
                return sb.ToString().TrimEnd('，');
            }
            catch
            {
                return "【" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN) + "】：" + " ToString error";
            }
        }
    }
}
