using Common;
using DTU.GateWay.Protocol.WaterMessageClass;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace DTU.GateWay.Protocol
{
    public class WaterCmd_4B_2 : WaterBaseMessage
    {
        public WaterCmd_4B_2()
        {
            AFN = (byte)WaterBaseProtocol.AFN._4B;
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

        public bool isUsed
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
            UserDataAll += Iden_F1.GetHexStr();
            Identifier_45 iden = new Identifier_45();
            iden.AlarmStateV = "0".PadLeft(32, '0');
            iden.AlarmStateV = (isUsed ? "1" : "0").PadRight(10, '0').PadLeft(32, '0');
            UserData += iden.GetHexStr();

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

            string Remain = UserDataAll.Substring(30);

            msg = "";
            List<Identifier> List_RTUParam = Identifier.analyse(Remain, AFN, out msg);
            if (msg == "")
            {
                if (List_RTUParam.Count == 1)
                {
                    if (List_RTUParam[0].GetKey() == (byte)Identifier_Standard._45)
                    {
                        Identifier_45 iden = (Identifier_45)List_RTUParam[0];
                        isUsed = iden.AlarmStateV[32 - (int)AlarmState._10] == '1';
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
                sb.Append(Iden_F1.ToString());
                sb.Append("【IC卡功能有效 】：" + (isUsed ? "IC卡有效" : "关闭") + "，");
                return sb.ToString().TrimEnd('，');
            }
            catch
            {
                return "【" + EnumUtils.GetDescription(typeof(WaterBaseProtocol.AFN), AFN) + "】：" + " ToString error";
            }
        }
    }
}
