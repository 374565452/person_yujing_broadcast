using Common;
using System;
using System.Globalization;
using System.Text;

namespace DTU.GateWay.Protocol
{
    public class WaterCmd_27_1 : WaterBaseMessage
    {
        public WaterCmd_27_1()
        {
            AFN = (byte)WaterBaseProtocol.AFN._27;
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
        /// 播报次数
        /// </summary>
        public byte PlayCount
        {
            get;
            set;
        }

        /// <summary>
        /// 播报角色
        /// </summary>
        public byte PlayRole
        {
            get;
            set;
        }

        /// <summary>
        /// 播报速度
        /// </summary>
        public byte PlaySpeed
        {
            get;
            set;
        }

        /// <summary>
        /// 播报内容
        /// </summary>
        public string PlayContext
        {
            get;
            set;
        }

        public string WriteMsg()
        {
            UserData = "";
            UserData += SerialNumber.ToString("X").PadLeft(4, '0');
            UserData += SendTime.ToString("yyMMddHHmmss").PadLeft(12, '0');
            if (PlayCount < 1) PlayCount = 1;
            if (PlayCount > 255) PlayCount = 255;
            UserData += PlayCount.ToString("X").PadLeft(2, '0');
            if (PlayRole < 1) PlayRole = 1;
            if (PlayRole > 15) PlayRole = 15;
            if (PlaySpeed < 1) PlaySpeed = 1;
            if (PlaySpeed > 15) PlaySpeed = 15;
            UserData += PlaySpeed.ToString("X") + PlayRole.ToString("X");
            if (PlayContext.Length > 400)
            {
                PlayContext = PlayContext.Substring(0, 400);
            }
            UserData += HexStringUtility.StrToHexString(PlayContext.Trim());

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
                PlayCount = Convert.ToByte(UserData.Substring(16, 2), 16);
            }
            catch (Exception ex)
            {
                if (ShowLog) logHelper.Error(ex.Message + Environment.NewLine + "获取播报次数出错" + " " + RawDataStr);
                return "获取播报次数出错";
            }

            try
            {
                PlaySpeed = Convert.ToByte(UserData.Substring(18, 1), 16);
            }
            catch (Exception ex)
            {
                if (ShowLog) logHelper.Error(ex.Message + Environment.NewLine + "获取播报语速出错" + " " + RawDataStr);
                return "获取播报语速出错";
            }

            try
            {
                PlayRole = Convert.ToByte(UserData.Substring(19, 1), 16);
            }
            catch (Exception ex)
            {
                if (ShowLog) logHelper.Error(ex.Message + Environment.NewLine + "获取播报角色出错" + " " + RawDataStr);
                return "获取播报角色出错";
            }

            try
            {
                PlayContext = HexStringUtility.HexStringToStr(UserData.Substring(20));
            }
            catch (Exception ex)
            {
                if (ShowLog) logHelper.Error(ex.Message + Environment.NewLine + "获取播报内容出错" + " " + RawDataStr);
                return "获取播报内容出错";
            }

            return "";
        }
    }
}
