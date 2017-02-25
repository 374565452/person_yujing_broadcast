using Common;
using System;
using System.Globalization;
using System.Text;

namespace DTU.GateWay.Protocol
{
    public class WaterCmd_26_2 : WaterBaseMessage
    {
        public WaterCmd_26_2()
        {
            AFN = (byte)WaterBaseProtocol.AFN._26;
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

        public byte RType
        {
            get;
            set;
        }

        public byte IsSend
        {
            get;
            set;
        }

        //kqz 2016-12-31 增加
        public byte NumAuthenType //号码的认证方式
        {
            get;
            set;
        }
        //kqz 2016-12-31 增加
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
            UserDataAll += RType.ToString("X").PadLeft(2, '0');
            UserDataAll += IsSend.ToString("X").PadLeft(2, '0');
            //kqz 2016-12-31 增加
            UserDataAll += NumAuthenType.ToString("X").PadLeft(2, '0');
            //kqz 2016-12-31 增加
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
                RType = Convert.ToByte(UserDataAll.Substring(16, 2), 16);
            }
            catch (Exception ex)
            {
                if (ShowLog) logHelper.Error(ex.Message + Environment.NewLine + "获取遥测站类型出错" + " " + RawDataStr);
                return "获取遥测站类型出错";
            }

            try
            {
                IsSend = Convert.ToByte(UserDataAll.Substring(18, 2), 16);
            }
            catch (Exception ex)
            {
                if (ShowLog) logHelper.Error(ex.Message + Environment.NewLine + "获取是否发送预警短信出错" + " " + RawDataStr);
                return "获取是否发送预警短信出错";
            }
            //kqz 2016-12-31 增加
            try
            {
                NumAuthenType = Convert.ToByte(UserDataAll.Substring(20, 2), 16);
            }
            catch (Exception ex)
            {
                if (ShowLog) logHelper.Error(ex.Message + Environment.NewLine + "获取电话加密方式出错" + " " + RawDataStr);
                return "获取电话加密方式出错";
            }
            //kqz 2016-12-31 增加
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
                string RTypeStr = "未知" + RType;
                if (RType == 1) RTypeStr = "雨量站";
                else if (RType == 2) RTypeStr = "水位站";
                else if (RType == 3) RTypeStr = "雨量水位站";
                StringBuilder sb = new StringBuilder();
                sb.Append("【" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN) + "】：");
                sb.Append("【流水号】：" + SerialNumber + "，");
                sb.Append("【发报时间】：" + SendTime.ToString("yyyy-MM-dd HH:mm:ss") + "，");
                sb.Append("【遥测站类型】：" + RTypeStr + "，");
                sb.Append("【是否发送短信】：" + (IsSend == 1 ? "发送" : "不发") + "，");
                sb.Append("【电话加密方式】：" + (NumAuthenType == 0 ? "密码" : "白名单") + "，");
                return sb.ToString().TrimEnd('，');
            }
            catch
            {
                return "【" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN) + "】：" + " ToString error";
            }
        }
    }
}
