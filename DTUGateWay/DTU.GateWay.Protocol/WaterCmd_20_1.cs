using Common;
using System;
using System.Globalization;

namespace DTU.GateWay.Protocol
{
    public class WaterCmd_20_1 : WaterBaseMessage
    {
        public WaterCmd_20_1()
        {
            AFN = (byte)WaterBaseProtocol.AFN._20;
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

        public byte[] Values
        {
            get;
            set;
        }

        public string WriteMsg()
        {
            if (Values == null || Values.Length != 12)
            {
                return "雨量预警阈值设置非法！";
            }

            UserData = "";
            UserData += SerialNumber.ToString("X").PadLeft(4, '0');
            UserData += SendTime.ToString("yyMMddHHmmss").PadLeft(12, '0');
            UserData += "01";
            UserData += Values[0].ToString("X").PadLeft(2, '0');
            UserData += Values[1].ToString("X").PadLeft(2, '0');
            UserData += Values[2].ToString("X").PadLeft(2, '0');
            UserData += "03";
            UserData += Values[3].ToString("X").PadLeft(2, '0');
            UserData += Values[4].ToString("X").PadLeft(2, '0');
            UserData += Values[5].ToString("X").PadLeft(2, '0');
            UserData += "06";
            UserData += Values[6].ToString("X").PadLeft(2, '0');
            UserData += Values[7].ToString("X").PadLeft(2, '0');
            UserData += Values[8].ToString("X").PadLeft(2, '0');
            UserData += "12";
            UserData += Values[9].ToString("X").PadLeft(2, '0');
            UserData += Values[10].ToString("X").PadLeft(2, '0');
            UserData += Values[11].ToString("X").PadLeft(2, '0');

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

            Values = new byte[12];

            try
            {
                Values[0] = Convert.ToByte(UserData.Substring(18, 2), 16);
            }
            catch (Exception ex)
            {
                if (ShowLog) logHelper.Error(ex.Message + Environment.NewLine + "获取1小时黄色预警值出错" + " " + RawDataStr);
                return "获取1小时黄色预警值出错";
            }

            try
            {
                Values[1] = Convert.ToByte(UserData.Substring(20, 2), 16);
            }
            catch (Exception ex)
            {
                if (ShowLog) logHelper.Error(ex.Message + Environment.NewLine + "获取1小时橙色预警值出错" + " " + RawDataStr);
                return "获取1小时橙色预警值出错";
            }

            try
            {
                Values[2] = Convert.ToByte(UserData.Substring(22, 2), 16);
            }
            catch (Exception ex)
            {
                if (ShowLog) logHelper.Error(ex.Message + Environment.NewLine + "获取1小时红色预警值出错" + " " + RawDataStr);
                return "获取1小时红色预警值出错";
            }

            try
            {
                Values[3] = Convert.ToByte(UserData.Substring(26, 2), 16);
            }
            catch (Exception ex)
            {
                if (ShowLog) logHelper.Error(ex.Message + Environment.NewLine + "获取3小时黄色预警值出错" + " " + RawDataStr);
                return "获取3小时黄色预警值出错";
            }

            try
            {
                Values[4] = Convert.ToByte(UserData.Substring(28, 2), 16);
            }
            catch (Exception ex)
            {
                if (ShowLog) logHelper.Error(ex.Message + Environment.NewLine + "获取3小时橙色预警值出错" + " " + RawDataStr);
                return "获取3小时橙色预警值出错";
            }

            try
            {
                Values[5] = Convert.ToByte(UserData.Substring(30, 2), 16);
            }
            catch (Exception ex)
            {
                if (ShowLog) logHelper.Error(ex.Message + Environment.NewLine + "获取3小时红色预警值出错" + " " + RawDataStr);
                return "获取3小时红色预警值出错";
            }

            try
            {
                Values[6] = Convert.ToByte(UserData.Substring(34, 2), 16);
            }
            catch (Exception ex)
            {
                if (ShowLog) logHelper.Error(ex.Message + Environment.NewLine + "获取6小时黄色预警值出错" + " " + RawDataStr);
                return "获取6小时黄色预警值出错";
            }

            try
            {
                Values[7] = Convert.ToByte(UserData.Substring(36, 2), 16);
            }
            catch (Exception ex)
            {
                if (ShowLog) logHelper.Error(ex.Message + Environment.NewLine + "获取6小时橙色预警值出错" + " " + RawDataStr);
                return "获取6小时橙色预警值出错";
            }

            try
            {
                Values[8] = Convert.ToByte(UserData.Substring(38, 2), 16);
            }
            catch (Exception ex)
            {
                if (ShowLog) logHelper.Error(ex.Message + Environment.NewLine + "获取6小时红色预警值出错" + " " + RawDataStr);
                return "获取6小时红色预警值出错";
            }

            try
            {
                Values[9] = Convert.ToByte(UserData.Substring(42, 2), 16);
            }
            catch (Exception ex)
            {
                if (ShowLog) logHelper.Error(ex.Message + Environment.NewLine + "获取12小时黄色预警值出错" + " " + RawDataStr);
                return "获取12小时黄色预警值出错";
            }

            try
            {
                Values[10] = Convert.ToByte(UserData.Substring(44, 2), 16);
            }
            catch (Exception ex)
            {
                if (ShowLog) logHelper.Error(ex.Message + Environment.NewLine + "获取12小时橙色预警值出错" + " " + RawDataStr);
                return "获取12小时橙色预警值出错";
            }

            try
            {
                Values[11] = Convert.ToByte(UserData.Substring(46, 2), 16);
            }
            catch (Exception ex)
            {
                if (ShowLog) logHelper.Error(ex.Message + Environment.NewLine + "获取12小时红色预警值出错" + " " + RawDataStr);
                return "获取12小时红色预警值出错";
            }

            return "";
        }
    }
}
