using Common;
using System;
using System.Text;

namespace DTU.GateWay.Protocol
{
    public class WaterCmd_27_2 : WaterBaseMessage
    {
        public WaterCmd_27_2()
        {
            AFN = (byte)WaterBaseProtocol.AFN._27;
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
                return sb.ToString().TrimEnd('，');
            }
            catch
            {
                return "【" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN) + "】：" + " ToString error";
            }
        }
    }
}
