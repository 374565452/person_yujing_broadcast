using Common;
using System;
using System.Collections.Generic;

namespace DTU.GateWay.Protocol
{
    public class CmdToDtuSetDateTime : BaseMessage
    {
        public CmdToDtuSetDateTime()
        {
            BeginChar = BaseProtocol.BeginChar;
            EndChar = BaseProtocol.EndChar;

            //AFN = 0x11;
            AFN = (byte)BaseProtocol.AFN.ToDtuSetDateTime;
            //ControlField = 0x33;
            ControlField = (byte)BaseProtocol.ControlField.ToDtu;
        }

        public CmdToDtuSetDateTime(BaseMessage bm)
        {
            BeginChar = BaseProtocol.BeginChar;
            EndChar = BaseProtocol.EndChar;

            //AFN = 0x11;
            AFN = (byte)BaseProtocol.AFN.ToDtuSetDateTime;
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

            string data = ss.ToString().PadLeft(2, '0') +
                mm.ToString().PadLeft(2, '0') +
                HH.ToString().PadLeft(2, '0') +
                dd.ToString().PadLeft(2, '0') +
                dw_MM +
                yy.ToString().PadLeft(2, '0') +
                "";

            DateTime dateNow = DateTime.Now;
            IsPW = true;
            PW = "0000";
            IsTP = true;
            TP = dateNow.ToString("ssmmHHdd") + "00";

            UserData = data;

            UserDataBytes = HexStringUtility.HexStringToByteArray(UserData);

            return WriteMsg2();
        }

        public override string ReadMsg()
        {
            DateTimeNew = DateTime.Parse("2000-1-1");

            string data = UserData;

            try
            {
                int ss = int.Parse(data.Substring(0, 2));
                int mm = int.Parse(data.Substring(2, 2));
                int HH = int.Parse(data.Substring(4, 2));
                int dd = int.Parse(data.Substring(6, 2));
                string mm_dw = Convert.ToString(Convert.ToInt32(data.Substring(8, 1), 16), 2);
                int MM = int.Parse(mm_dw.Substring(3) + data.Substring(9, 1));
                int yy = int.Parse(data.Substring(10, 2)) + 2000;
                DateTimeNew = new DateTime(yy, MM, dd, HH, mm, ss);
            }
            catch
            {
                return "获取时间出错";
            }

            return "";
        }

    }
}
