using Common;
using System;
using System.Globalization;

namespace DTU.GateWay.Protocol
{
    public class WaterCmd_23_1 : WaterBaseMessage
    {
        public WaterCmd_23_1()
        {
            AFN = (byte)WaterBaseProtocol.AFN._23;
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

        public byte OrderNum
        {
            get;
            set;
        }

        public string Phone
        {
            get;
            set;
        }

        public string WriteMsg()
        {
            if (Phone.Trim().Length > 11)
            {
                return "预警责任人号码修改非法！";
            }

            if (Phone.Trim() != "" && Tools.GetTest(Phone.Trim()) != "数字")
            {
                return "预警责任人号码修改非法！";
            }

            UserData = "";
            UserData += SerialNumber.ToString("X").PadLeft(4, '0');
            UserData += SendTime.ToString("yyMMddHHmmss").PadLeft(12, '0');
            UserData += OrderNum.ToString("X").PadLeft(2, '0');
            UserData += HexStringUtility.StrToHexString(Phone.Trim());

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

            try
            {
                OrderNum = Convert.ToByte(UserData.Substring(16, 2), 16);
            }
            catch (Exception ex)
            {
                if (ShowLog) logHelper.Error(ex.Message + Environment.NewLine + "获取手机号序号出错" + " " + RawDataStr);
                return "获取手机号序号出错";
            }

            try
            {
                if (UserDataBytes.Length * 2 - 18 > 0)
                {
                    string str = UserData.Substring(18, UserDataBytes.Length * 2 - 18);
                    Phone = HexStringUtility.HexStringToStr(str);
                }
                else
                {
                    Phone = "";
                }
            }
            catch (Exception ex)
            {
                if (ShowLog) logHelper.Error(ex.Message + Environment.NewLine + "获取手机号出错" + " " + RawDataStr);
                return "获取手机号出错";
            }

            return "";
        }
    }
}
