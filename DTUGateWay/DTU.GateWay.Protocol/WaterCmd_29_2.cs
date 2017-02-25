using Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTU.GateWay.Protocol
{
    //kqz 2017-1-1 添加
    public class WaterCmd_29_2 :WaterBaseMessage
    {
        public WaterCmd_29_2()
        {
            AFN = (byte)WaterBaseProtocol.AFN._29;
            UpOrDown = (int)WaterBaseProtocol.UpOrDown.Up;
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
        /// 黄色水位预警值
        /// </summary>
        public short YellowLevel
        {
            get;
            set;
        }
        /// <summary>
        /// 橙色水位预警值
        /// </summary>
        public short OrangeLevel
        {
            get;
            set;
        }
        /// <summary>
        /// 红色水位预警值
        /// </summary>
        public short RedLevel
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
            /*UserData = "";
            UserData += SerialNumber.ToString("X").PadLeft(4, '0');
            UserData += SendTime.ToString("yyMMddHHmmss").PadLeft(12, '0');

            UserData += YellowLevel.ToString("X").PadLeft(4, '0');
            UserData += OrangeLevel.ToString("X").PadLeft(4, '0');
            UserData += RedLevel.ToString("X").PadLeft(4, '0');

            UserDataBytes = HexStringUtility.HexStringToByteArray(UserData);
            return WriteMsgBase();*/
            UserDataAll += SerialNumber.ToString("X").PadLeft(4, '0');
            UserDataAll += SendTime.ToString("yyMMddHHmmss").PadLeft(12, '0');
            UserDataAll += YellowLevel.ToString("X").PadLeft(4, '0');
            UserDataAll += OrangeLevel.ToString("X").PadLeft(4, '0');
            UserDataAll += RedLevel.ToString("X").PadLeft(4, '0');
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
            /*if (UserDataBytes == null || UserDataBytes.Length == 0)
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
                YellowLevel = Convert.ToInt16(UserData.Substring(16, 4), 16);
            }
            catch (Exception ex)
            {
                if (ShowLog) logHelper.Error(ex.Message + Environment.NewLine + "获取水位黄色预警阈值出错" + " " + RawDataStr);
                return "获取水位黄色预警阈值出错";
            }
            try
            {
                OrangeLevel = Convert.ToInt16(UserData.Substring(20, 4), 16);
            }
            catch (Exception ex)
            {
                if (ShowLog) logHelper.Error(ex.Message + Environment.NewLine + "获取水位橙色预警阈值出错" + " " + RawDataStr);
                return "获取水位橙色预警阈值出错";
            }
            try
            {
                RedLevel = Convert.ToInt16(UserData.Substring(24, 4), 16);
            }
            catch (Exception ex)
            {
                if (ShowLog) logHelper.Error(ex.Message + Environment.NewLine + "获取水位红色预警阈值出错" + " " + RawDataStr);
                return "获取水位红色预警阈值出错";
            }
            return "";*/
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
                YellowLevel = Convert.ToInt16(UserDataAll.Substring(16, 4), 16);
            }
            catch (Exception ex)
            {
                if (ShowLog) logHelper.Error(ex.Message + Environment.NewLine + "获取水位黄色预警阈值出错" + " " + RawDataStr);
                return "获取水位黄色预警阈值出错";
            }
            try
            {
                OrangeLevel = Convert.ToInt16(UserDataAll.Substring(20, 4), 16);
            }
            catch (Exception ex)
            {
                if (ShowLog) logHelper.Error(ex.Message + Environment.NewLine + "获取水位橙色预警阈值出错" + " " + RawDataStr);
                return "获取水位橙色预警阈值出错";
            }
            try
            {
                RedLevel = Convert.ToInt16(UserDataAll.Substring(24, 4), 16);
            }
            catch (Exception ex)
            {
                if (ShowLog) logHelper.Error(ex.Message + Environment.NewLine + "获取水位红色预警阈值出错" + " " + RawDataStr);
                return "获取水位红色预警阈值出错";
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
               // string RTypeStr = "未知" + ;
                //if (RType == 1) RTypeStr = "雨量站";
                //else if (RType == 2) RTypeStr = "水位站";
                //else if (RType == 3) RTypeStr = "雨量水位站";
                StringBuilder sb = new StringBuilder();
                sb.Append("【" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN) + "】：");
                sb.Append("【流水号】：" + SerialNumber + "，");
                sb.Append("【发报时间】：" + SendTime.ToString("yyyy-MM-dd HH:mm:ss") + "，");
               // sb.Append("【遥测站类型】：" + RTypeStr + "，");
               // sb.Append("【是否发送短信】：" + (IsSend == 1 ? "发送" : "不发") + "，");
                return sb.ToString().TrimEnd('，');
            }
            catch
            {
                return "【" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN) + "】：" + " ToString error";
            }
        }
    }
    //kqz 2017-1-1 添加
}
