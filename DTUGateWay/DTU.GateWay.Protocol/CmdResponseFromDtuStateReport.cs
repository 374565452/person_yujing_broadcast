using Common;
using System;
using System.Collections.Generic;

namespace DTU.GateWay.Protocol
{
    public class CmdResponseFromDtuStateReport : BaseMessage
    {
        public CmdResponseFromDtuStateReport()
        {
            BeginChar = BaseProtocol.BeginChar;
            EndChar = BaseProtocol.EndChar;

            //AFN = 0x81;
            AFN = (byte)BaseProtocol.AFN.FromDtuStateReport;
            //ControlField = 0x33;
            ControlField = (byte)BaseProtocol.ControlField.ToDtu;
        }

        public CmdResponseFromDtuStateReport(BaseMessage bm)
        {
            BeginChar = BaseProtocol.BeginChar;
            EndChar = BaseProtocol.EndChar;

            //AFN = 0x81;
            AFN = (byte)BaseProtocol.AFN.FromDtuStateReport;
            //ControlField = 0x33;
            ControlField = (byte)BaseProtocol.ControlField.ToDtu;

            this.RawDataStr = bm.RawDataStr;
            this.RawDataChar = bm.RawDataChar;
            this.Length = bm.Length;
            this.AddressField = bm.AddressField;
            this.StationType = bm.StationType;
            this.StationCode = bm.StationCode;
            this.UserData = bm.UserData;
            this.UserDataBytes = bm.UserDataBytes;
            this.CC = bm.CC;
            this.IsPW = bm.IsPW;
            this.PW = bm.PW;
            this.IsTP = bm.IsTP;
            this.TP = bm.TP;
        }

        /// <summary>
        /// 应答结果，0 失败，1 成功
        /// </summary>
        public byte Result
        {
            set;
            get;
        }

        /// <summary>
        /// 设置时间
        /// </summary>
        public DateTime DateTimeNew
        {
            set;
            get;
        }

        public override byte[] WriteMsg()
        {
            int yy = DateTimeNew.Year - 2000;
            int MM = DateTimeNew.Month;
            int dd = DateTimeNew.Day;
            int HH = DateTimeNew.Hour;
            int mm = DateTimeNew.Minute;
            int ss = DateTimeNew.Second;
            int dw = (int)DateTimeNew.DayOfWeek;
            if (dw == 0) dw = 7;

            string tt = Convert.ToString(dw, 2).PadLeft(3, '0') + (MM >= 10 ? "1" : "0");
            string dw_MM = string.Format("{0:x}", Convert.ToInt32(tt, 2)) + (MM % 10).ToString() + "";

            string data = HexStringUtility.ByteArrayToHexString(new byte[] { Result }) +
                ss.ToString().PadLeft(2, '0') +
                mm.ToString().PadLeft(2, '0') +
                HH.ToString().PadLeft(2, '0') +
                dd.ToString().PadLeft(2, '0') +
                dw_MM +
                yy.ToString().PadLeft(2, '0') +
                "";

            IsPW = false;
            PW = "";
            IsTP = false;
            TP = "";

            UserData = data;

            UserDataBytes = HexStringUtility.HexStringToByteArray(UserData);

            return WriteMsg2();
        }

        public override string ReadMsg()
        {
            try
            {
                string data = UserData;
                Result = HexStringUtility.HexStringToByteArray(UserData)[0];
                return "";
            }
            catch
            {
                return "获取状态自报响应信息出错";
            }
        }
    }
}
