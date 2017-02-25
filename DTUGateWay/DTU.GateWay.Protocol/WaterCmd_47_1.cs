using Common;
using DTU.GateWay.Protocol.WaterMessageClass;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace DTU.GateWay.Protocol
{
    public class WaterCmd_47_1 : WaterBaseMessage
    {
        public WaterCmd_47_1()
        {
            AFN = (byte)WaterBaseProtocol.AFN._47;
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

        public RTUParam_97 rp
        {
            get;
            set;
        }

        public string WriteMsg()
        {
            UserData = "";
            UserData += SerialNumber.ToString("X").PadLeft(4, '0');
            UserData += SendTime.ToString("yyMMddHHmmss").PadLeft(12, '0');
            UserData += rp.GetHexStr();

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

            string Remain = UserData.Substring(16).ToUpper();

            string msg = "";
            List<RTUParam> List_RTUParam = RTUParam.analyse(Remain, AFN, out msg);
            if (msg == "")
            {
                if (List_RTUParam.Count == 1)
                {
                    if (List_RTUParam[0].GetKey() == (byte)RTUParam.RTUParamKey._97)
                    {
                        rp = (RTUParam_97)List_RTUParam[0];
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
