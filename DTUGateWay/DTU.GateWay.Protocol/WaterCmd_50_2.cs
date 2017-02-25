using Common;
using DTU.GateWay.Protocol.WaterMessageClass;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace DTU.GateWay.Protocol
{
    public class WaterCmd_50_2 : WaterBaseMessage
    {
        public WaterCmd_50_2()
        {
            AFN = (byte)WaterBaseProtocol.AFN._50;
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

        public Identifier_F1 Iden_F1
        {
            get;
            set;
        }

        public List<short> States
        {
            get;
            set;
        }

        string[] stateDec = new string[] { 
            "历史数据初始化记录","参数变更记录","状态量变位记录","传感器及仪表故障记录","密码修改记录","终端故障记录","交流失电记录",
            "蓄电池电压低告警记录","终端箱非法打开记录","水泵故障记录","剩余水量超限告警记录","水位超限告警记录","水压超限告警记录",
            "水质参数超限告警记录","数据出错记录","发报文记录","收报文记录","发报文出错记录"
        };

        public string[] GetStateDec()
        {
            return stateDec;
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
            UserDataAll += Iden_F1.GetHexStr();
            for (int i = 0; i < stateDec.Length; i++)
            {
                if (States.Count > i)
                {
                    UserDataAll += States[i].ToString("X").PadLeft(4, '0');
                }
                else
                {
                    UserDataAll += "0000";
                }

            }

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
                Iden_F1 = new Identifier_F1();
                Iden_F1.RemoteStation = UserDataAll.Substring(20, 10);
            }
            catch (Exception ex)
            {
                if (ShowLog) logHelper.Error(ex.Message + Environment.NewLine + "获取遥测站出错" + " " + RawDataStr);
                return "获取遥测站出错";
            }

            States = new List<short>();
            for (int i = 0; i < stateDec.Length; i++)
            {
                if (UserDataAll.Length >= 30 + 4 * (i + 1))
                {
                    string s = UserDataAll.Substring(30 + 4 * i, 4);
                    States.Add(Convert.ToInt16(s, 16));
                }
                else
                {
                    States.Add(0);
                }
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
                sb.Append("【遥测终端】：" + Iden_F1.ToString());
                for (int i = 0; i < stateDec.Length; i++)
                {
                    if(States.Count>i)
                        sb.Append("【" + stateDec[i] + "】：" + States[i] + "，");
                    else
                        sb.Append("【" + stateDec[i] + "】：0，");
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
