using Common;
using DTU.GateWay.Protocol.WaterMessageClass;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace DTU.GateWay.Protocol
{
    public class WaterCmd_4B_1 : WaterBaseMessage
    {
        public WaterCmd_4B_1()
        {
            AFN = (byte)WaterBaseProtocol.AFN._4B;
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

        public bool isUsed
        {
            get;
            set;
        }

        public string WriteMsg()
        {
            UserData = "";
            UserData += SerialNumber.ToString("X").PadLeft(4, '0');
            UserData += SendTime.ToString("yyMMddHHmmss").PadLeft(12, '0');
            Identifier_45 iden = new Identifier_45();
            iden.AlarmStateV = "0".PadLeft(32, '0');
            iden.AlarmStateV = (isUsed ? "1" : "0").PadRight(10, '0').PadLeft(32, '0');
            UserData += iden.GetHexStr();
            UserDataBytes = HexStringUtility.HexStringToByteArray(UserData);
            return WriteMsgBase();
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

            string Remain = UserData.Substring(16);

            string msg = "";
            List<Identifier> List_RTUParam = Identifier.analyse(Remain, AFN, out msg);
            if (msg == "")
            {
                if (List_RTUParam.Count == 1)
                {
                    if (List_RTUParam[0].GetKey() == (byte)Identifier_Standard._45)
                    {
                        Identifier_45 iden = (Identifier_45)List_RTUParam[0];
                        isUsed = iden.AlarmStateV[32 - (int)AlarmState._10] == '1';
                    }
                    else
                    {
                        msg = "数据体非法，参数关键字非法";
                    }
                }
                else
                {
                    msg = "数据体非法，参数超过1个";
                }
            }
            return msg;
        }
    }
}
