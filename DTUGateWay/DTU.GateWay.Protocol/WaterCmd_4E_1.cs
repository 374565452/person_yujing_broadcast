using Common;
using System;
using System.Globalization;

namespace DTU.GateWay.Protocol
{
    public class WaterCmd_4E_1 : WaterBaseMessage
    {
        public WaterCmd_4E_1()
        {
            AFN = (byte)WaterBaseProtocol.AFN._4E;
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

        public bool[] Ps
        {
            get;
            set;
        }

        public short[] PsOpen
        {
            get;
            set;
        }

        public string WriteMsg()
        {
            UserData = "";
            UserData += SerialNumber.ToString("X").PadLeft(4, '0');
            UserData += SendTime.ToString("yyMMddHHmmss").PadLeft(12, '0');

            int count = Ps.Length < PsOpen.Length ? Ps.Length : PsOpen.Length;
            UserData += count.ToString("X").PadLeft(2, '0');
            string s = "";
            for (int i = 0; i < count; i++)
            {
                if (Ps[i])
                {
                    s = "1" + s;
                }
                else
                {
                    s = "0" + s;
                }
            }
            s = s.PadLeft(8 * ((count - 1) / 8 + 1), '0');
            UserData += HexStringUtility.BinStringToHexString(s);

            for (int i = 0; i < count; i++)
            {
                if (PsOpen[i] <= 9999)
                {
                    UserData += PsOpen[i].ToString().PadLeft(4, '0');
                }
                else if (PsOpen[i] > 9999)
                {
                    UserData += PsOpen[i].ToString().Substring(PsOpen[i].ToString().Length - 4);
                }
            }

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
                int count = Convert.ToInt32(UserData.Substring(16, 2), 16);
                Ps = new bool[count];
                PsOpen = new short[count];
                string hexStr = UserData.Substring(18, ((count - 1) / 8 + 1) * 2);
                string binStr = HexStringUtility.HexStringToBinString(hexStr);
                for (int i = 0; i < binStr.Length; i++)
                {
                    if (count > binStr.Length - i - 1)
                    {
                        Ps[binStr.Length - i - 1] = binStr[i] == '1';
                    }
                }
                for (int i = 0; i < count; i++)
                {
                    string s = UserData.Substring(18 + ((count - 1) / 8 + 1) * 2 + 4 * i, 4);
                    PsOpen[i] = Convert.ToInt16(s);
                }
            }
            catch (Exception ex)
            {
                if (ShowLog) logHelper.Error(ex.Message + Environment.NewLine + "获取闸门开关状态出错" + " " + RawDataStr);
                return "获取闸门开关状态出错";
            }

            return "";
        }
    }
}
