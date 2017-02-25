using Common;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace DTU.GateWay.Protocol
{
    public class CmdFromDtuStateReport : BaseMessage
    {
        public CmdFromDtuStateReport()
        {
            BeginChar = BaseProtocol.BeginChar;
            EndChar = BaseProtocol.EndChar;

            //AFN = 0x81;
            AFN = (byte)BaseProtocol.AFN.FromDtuStateReport;
            //ControlField = 0xB3;
            ControlField = (byte)BaseProtocol.ControlField.FromDtu;
        }

        public CmdFromDtuStateReport(BaseMessage bm)
        {
            BeginChar = BaseProtocol.BeginChar;
            EndChar = BaseProtocol.EndChar;

            //AFN = 0x81;
            AFN = (byte)BaseProtocol.AFN.FromDtuStateReport;
            //ControlField = 0xB3;
            ControlField = (byte)BaseProtocol.ControlField.FromDtu;

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
        /// 状态，4字节，32个二进制0或1
        /// </summary>
        public string State
        {
            set;
            get;
        }

        /// <summary>
        /// 上报时间
        /// </summary>
        public DateTime DateTimeNew
        {
            set;
            get;
        }

        /// <summary>
        /// 累计用电量，4字节，压缩 BCD，取值范围 0～9999999.9，单位：立方米
        /// </summary>
        //public decimal ElectricUsed
        //{
        //    set;
        //    get;
        //}

        /// <summary>
        /// 累计用水量，4字节，压缩 BCD，取值范围 0～9999999.9，单位：立方米
        /// </summary>
        public decimal WaterUsed
        {
            set;
            get;
        }

        public override byte[] WriteMsg()
        {
            string data = HexStringUtility.BinStringToHexString(State).PadLeft(8, '0');
            data += DateTimeNew.ToString("yyyyMMddHHmmss").Substring(2).PadLeft(12, '0');
            //data += ((int)(ElectricUsed * 10)).ToString().PadLeft(8, '0');
            data += ((int)(WaterUsed * 10)).ToString().PadLeft(8, '0');

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
            State = "0".PadLeft(32, '0');
            DateTimeNew = DateTime.Parse("2000-1-1");
            WaterUsed = 0;

            string data = UserData;

            try
            {
                State = Convert.ToString(Convert.ToInt32(data.Substring(0, 8), 16), 2).PadLeft(32, '0');
            }
            catch (Exception ex)
            {
                if (ShowLog)
                    logHelper.Error(ex.Message + Environment.NewLine + "获取状态出错" + " " + RawDataStr);
                return "获取状态出错";
            }

            try
            {
                DateTimeNew = DateTime.ParseExact("20" + data.Substring(8, 12), "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                if (ShowLog)
                    logHelper.Error(ex.Message + Environment.NewLine + "获取上报时间出错" + " " + RawDataStr);
                return "获取上报时间出错";
            }

            if (DateTimeNew.Year < 2000)
            {
                if (ShowLog)
                    logHelper.Error("获取上报时间出错!时间年份不能小于2000" + Environment.NewLine + RawDataStr);
                return "获取上报时间出错!时间年份不能小于2000";
            }

            try
            {
                WaterUsed = decimal.Parse(data.Substring(20, 8)) / 10m;
            }
            catch (Exception ex)
            {
                if (ShowLog)
                    logHelper.Error(ex.Message + Environment.NewLine + "获取累计用水量出错" + " " + RawDataStr);
                return "获取累计用水量出错";
            }

            return "";
        }
    }
}
