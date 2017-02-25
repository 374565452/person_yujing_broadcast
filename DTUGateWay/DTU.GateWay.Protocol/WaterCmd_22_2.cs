using Common;
using System;
using System.Globalization;
using System.Text;

namespace DTU.GateWay.Protocol
{
    public class WaterCmd_22_2 : WaterBaseMessage
    {
        public WaterCmd_22_2()
        {
            AFN = (byte)WaterBaseProtocol.AFN._22;
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

        public byte[] Values
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
            if (Values == null || Values.Length != 12)
            {
                return "雨量预警阈值读取非法！";
            }

            UserDataAll = "";
            UserDataAll += SerialNumber.ToString("X").PadLeft(4, '0');
            UserDataAll += SendTime.ToString("yyMMddHHmmss").PadLeft(12, '0');
            UserDataAll += "01";
            UserDataAll += Values[0].ToString("X").PadLeft(2, '0');
            UserDataAll += Values[1].ToString("X").PadLeft(2, '0');
            UserDataAll += Values[2].ToString("X").PadLeft(2, '0');
            UserDataAll += "03";
            UserDataAll += Values[3].ToString("X").PadLeft(2, '0');
            UserDataAll += Values[4].ToString("X").PadLeft(2, '0');
            UserDataAll += Values[5].ToString("X").PadLeft(2, '0');
            UserDataAll += "06";
            UserDataAll += Values[6].ToString("X").PadLeft(2, '0');
            UserDataAll += Values[7].ToString("X").PadLeft(2, '0');
            UserDataAll += Values[8].ToString("X").PadLeft(2, '0');
            UserDataAll += "12";
            UserDataAll += Values[9].ToString("X").PadLeft(2, '0');
            UserDataAll += Values[10].ToString("X").PadLeft(2, '0');
            UserDataAll += Values[11].ToString("X").PadLeft(2, '0');

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

            Values = new byte[12];

            try
            {
                Values[0] = Convert.ToByte(UserDataAll.Substring(18, 2), 16);
            }
            catch (Exception ex)
            {
                if (ShowLog) logHelper.Error(ex.Message + Environment.NewLine + "获取1小时黄色预警值出错" + " " + RawDataStr);
                return "获取1小时黄色预警值出错";
            }

            try
            {
                Values[1] = Convert.ToByte(UserDataAll.Substring(20, 2), 16);
            }
            catch (Exception ex)
            {
                if (ShowLog) logHelper.Error(ex.Message + Environment.NewLine + "获取1小时橙色预警值出错" + " " + RawDataStr);
                return "获取1小时橙色预警值出错";
            }

            try
            {
                Values[2] = Convert.ToByte(UserDataAll.Substring(22, 2), 16);
            }
            catch (Exception ex)
            {
                if (ShowLog) logHelper.Error(ex.Message + Environment.NewLine + "获取1小时红色预警值出错" + " " + RawDataStr);
                return "获取1小时红色预警值出错";
            }

            try
            {
                Values[3] = Convert.ToByte(UserDataAll.Substring(26, 2), 16);
            }
            catch (Exception ex)
            {
                if (ShowLog) logHelper.Error(ex.Message + Environment.NewLine + "获取3小时黄色预警值出错" + " " + RawDataStr);
                return "获取3小时黄色预警值出错";
            }

            try
            {
                Values[4] = Convert.ToByte(UserDataAll.Substring(28, 2), 16);
            }
            catch (Exception ex)
            {
                if (ShowLog) logHelper.Error(ex.Message + Environment.NewLine + "获取3小时橙色预警值出错" + " " + RawDataStr);
                return "获取3小时橙色预警值出错";
            }

            try
            {
                Values[5] = Convert.ToByte(UserDataAll.Substring(30, 2), 16);
            }
            catch (Exception ex)
            {
                if (ShowLog) logHelper.Error(ex.Message + Environment.NewLine + "获取3小时红色预警值出错" + " " + RawDataStr);
                return "获取3小时红色预警值出错";
            }

            try
            {
                Values[6] = Convert.ToByte(UserDataAll.Substring(34, 2), 16);
            }
            catch (Exception ex)
            {
                if (ShowLog) logHelper.Error(ex.Message + Environment.NewLine + "获取6小时黄色预警值出错" + " " + RawDataStr);
                return "获取6小时黄色预警值出错";
            }

            try
            {
                Values[7] = Convert.ToByte(UserDataAll.Substring(36, 2), 16);
            }
            catch (Exception ex)
            {
                if (ShowLog) logHelper.Error(ex.Message + Environment.NewLine + "获取6小时橙色预警值出错" + " " + RawDataStr);
                return "获取6小时橙色预警值出错";
            }

            try
            {
                Values[8] = Convert.ToByte(UserDataAll.Substring(38, 2), 16);
            }
            catch (Exception ex)
            {
                if (ShowLog) logHelper.Error(ex.Message + Environment.NewLine + "获取6小时红色预警值出错" + " " + RawDataStr);
                return "获取6小时红色预警值出错";
            }

            try
            {
                Values[9] = Convert.ToByte(UserDataAll.Substring(42, 2), 16);
            }
            catch (Exception ex)
            {
                if (ShowLog) logHelper.Error(ex.Message + Environment.NewLine + "获取12小时黄色预警值出错" + " " + RawDataStr);
                return "获取12小时黄色预警值出错";
            }

            try
            {
                Values[10] = Convert.ToByte(UserDataAll.Substring(44, 2), 16);
            }
            catch (Exception ex)
            {
                if (ShowLog) logHelper.Error(ex.Message + Environment.NewLine + "获取12小时橙色预警值出错" + " " + RawDataStr);
                return "获取12小时橙色预警值出错";
            }

            try
            {
                Values[11] = Convert.ToByte(UserDataAll.Substring(46, 2), 16);
            }
            catch (Exception ex)
            {
                if (ShowLog) logHelper.Error(ex.Message + Environment.NewLine + "获取12小时红色预警值出错" + " " + RawDataStr);
                return "获取12小时红色预警值出错";
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
                if (Values.Length == 12)
                {
                    sb.Append("【1小时预警值】：" + Values[0] + "mm、" + Values[1] + "mm、" + Values[2] + "mm，");
                    sb.Append("【3小时预警值】：" + Values[3] + "mm、" + Values[4] + "mm、" + Values[5] + "mm，");
                    sb.Append("【6小时预警值】：" + Values[6] + "mm、" + Values[7] + "mm、" + Values[8] + "mm，");
                    sb.Append("【12小时预警值】：" + Values[9] + "mm、" + Values[10] + "mm、" + Values[11] + "mm，");
                }
                else
                {
                    sb.Append("【预警值获取失败】：，"); 
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
