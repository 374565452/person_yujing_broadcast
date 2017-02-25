using Common;
using DTU.GateWay.Protocol.WaterMessageClass;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace DTU.GateWay.Protocol
{
    public class WaterCmd_31_1 : WaterBaseMessage
    {
        public WaterCmd_31_1()
        {
            AFN = (byte)WaterBaseProtocol.AFN._31;
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

        public List<Identifier> List_Identifier
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
            UserDataAll += SerialNumber.ToString("X").PadLeft(4, '0');
            UserDataAll += SendTime.ToString("yyMMddHHmmss").PadLeft(12, '0');

            UserDataBytesAll = HexStringUtility.HexStringToByteArray(UserDataAll);

            DataBeginChar = (byte)WaterBaseProtocol.DataBeginChar.STX;
            if (UserDataBytesAll.Length > WaterBaseProtocol.PocketSize)
            {
                TotalPackage = (UserDataBytesAll.Length - 1) / WaterBaseProtocol.PocketSize + 1;
                DataBeginChar = (byte)WaterBaseProtocol.DataBeginChar.SYN;
            }
            else
            {
                TotalPackage = 1;
            }
            MsgList = new WaterBaseMessage[TotalPackage];

            for (int i = 0; i < TotalPackage; i++)
            {
                WaterBaseMessage bm = new WaterBaseMessage();
                bm.CenterStation = this.CenterStation;
                bm.RemoteStation = this.RemoteStation;
                bm.PW = this.PW;
                bm.AFN = this.AFN;
                bm.UpOrDown = this.UpOrDown;
                bm.TotalPackage = this.TotalPackage;
                bm.CurrentPackage = i + 1;
                bm.DataBeginChar = this.DataBeginChar;
                byte[] bs = null;
                if (i == TotalPackage - 1)
                {
                    bs = new byte[UserDataBytesAll.Length - WaterBaseProtocol.PocketSize * i];
                    DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Up.ETX;
                }
                else
                {
                    bs = new byte[WaterBaseProtocol.PocketSize];
                    DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Up.ETB;
                }
                bm.DataEndChar = this.DataEndChar;
                Array.Copy(UserDataBytesAll, WaterBaseProtocol.PocketSize * i, bs, 0, bs.Length);
                bm.UserDataBytes = bs;
                //bm.UserData = HexStringUtility.ByteArrayToHexString(bs);

                string msg = bm.WriteMsgBase();
                if (msg == "")
                {
                    MsgList[i] = bm;
                }
                else
                {
                    MsgList = null;
                    if (ShowLog) logHelper.Error((i + 1) + "：" + msg);
                    return (i + 1) + "：" + msg;
                }
            }

            return "";
        }

        public string ReadMsg()
        {
            if (MsgList == null || MsgList.Length == 0)
            {
                if (ShowLog) logHelper.Error("无信息，无法分析！");
                return "无信息，无法分析！";
            }

            UserDataAll = "";
            int c = 0;
            foreach (WaterBaseMessage bm in MsgList)
            {
                c++;
                if (bm != null && bm.UserData != null)
                {
                    UserDataAll += bm.UserData;
                }
                else
                {
                    if (ShowLog) logHelper.Error("第" + c + "包无信息，无法分析！");
                    return "第" + c + "包无信息，无法分析！";
                }
            }

            UserDataBytesAll = HexStringUtility.HexStringToByteArray(UserDataAll);

            return ReadMsg(UserDataAll);
        }

        public string ReadMsg(string UserDataAll)
        {
            try
            {
                SerialNumber = Convert.ToInt16(UserDataAll.Substring(0, 4), 16);
            }
            catch (Exception ex)
            {
                if (ShowLog) logHelper.Error(ex.Message + Environment.NewLine + "获取流水号出错" + " " + RawDataStr);
                return "获取流水号出错";
            }

            try
            {
                SendTime = DateTime.ParseExact("20" + UserDataAll.Substring(4, 12), "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                if (ShowLog) logHelper.Error(ex.Message + Environment.NewLine + "获取发报时间出错" + " " + RawDataStr);
                return "获取发报时间出错";
            }

            string Remain = UserDataAll.Substring(16).ToUpper();

            string msg = "";
            List_Identifier = Identifier.analyse(Remain, AFN, out msg);
            return msg;
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
                if (List_Identifier != null)
                {
                    foreach (Identifier ib in List_Identifier)
                    {
                        sb.Append(ib.ToString());
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
