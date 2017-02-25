using Common;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace DTU.GateWay.Protocol
{
    public class CmdFromDtuOpenPump : BaseMessage
    {
        public CmdFromDtuOpenPump()
        {
            BeginChar = BaseProtocol.BeginChar;
            EndChar = BaseProtocol.EndChar;

            //AFN = 0x83;
            AFN = (byte)BaseProtocol.AFN.FromDtuOpenPump;
            //ControlField = 0xB3;
            ControlField = (byte)BaseProtocol.ControlField.FromDtu;
        }

        public CmdFromDtuOpenPump(BaseMessage bm)
        {
            BeginChar = BaseProtocol.BeginChar;
            EndChar = BaseProtocol.EndChar;

            //AFN = 0x83;
            AFN = (byte)BaseProtocol.AFN.FromDtuOpenPump;
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
        /// 用户卡号，4字节，压缩 BCD，非IC卡开泵用户号为0，IC卡开泵取值范围1～99999999
        /// </summary>
        public string UserNo
        {
            set;
            get;
        }

        /// <summary>
        /// 卡出厂序列号，4字节，16进制
        /// </summary>
        public string SerialNumber
        {
            set;
            get;
        }

        /// <summary>
        /// 开泵时间，6字节
        /// </summary>
        public DateTime StartTime
        {
            set;
            get;
        }

        /// <summary>
        /// 开泵卡剩余水量，4字节，压缩 BCD，取值范围 0～9999999.9，单位：立方米
        /// </summary>
        public decimal StartResidualWater
        {
            set;
            get;
        }

        /// <summary>
        /// 开泵卡剩余电量，4字节，压缩 BCD，取值范围 0～9999999.9，单位：kW.h
        /// </summary>
        public decimal StartResidualElectric
        {
            set;
            get;
        }

        /// <summary>
        /// 年累计用水量，4字节，压缩 BCD，取值范围 0～9999999.9，单位：立方米
        /// </summary>
        public decimal YearWaterUsed
        {
            set;
            get;
        }

        /// <summary>
        /// 年累计用电量，4字节，压缩 BCD，取值范围 0～9999999.9，单位：kW.h
        /// </summary>
        public decimal YearElectricUsed
        {
            set;
            get;
        }

        /// <summary>
        /// 年可开采量，4字节，压缩 BCD，取值范围 0～9999999.9，单位：立方米
        /// </summary>
        public decimal YearExploitation
        {
            set;
            get;
        }

        /// <summary>
        /// 年剩余可开采量，4字节，压缩 BCD，取值范围 0～9999999.9，单位：立方米
        /// </summary>
        public decimal YearSurplus
        {
            set;
            get;
        }

        public override byte[] WriteMsg()
        {
            string data = "";
            data += UserNo.PadLeft(8, '0');
            data += SerialNumber.PadLeft(8, '0');
            data += StartTime.ToString("yyyyMMddHHmmss").Substring(2).PadLeft(12, '0');
            data += ((int)(StartResidualWater * 10)).ToString().PadLeft(8, '0');
            data += ((int)(StartResidualElectric * 10)).ToString().PadLeft(8, '0');
            data += ((int)(YearElectricUsed * 10)).ToString().PadLeft(8, '0');
            data += ((int)(YearWaterUsed * 10)).ToString().PadLeft(8, '0');
            data += ((int)(YearExploitation * 10)).ToString().PadLeft(8, '0');
            data += ((int)(YearSurplus * 10)).ToString().PadLeft(8, '0');

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
            UserNo = "00000000";
            SerialNumber = "00000000";
            StartTime = DateTime.Parse("2000-1-1");
            StartResidualWater = 0;
            StartResidualElectric = 0;
            YearElectricUsed = 0;
            YearWaterUsed = 0;
            YearExploitation = 0;
            YearSurplus = 0;

            string data = UserData;

            try
            {
                UserNo = data.Substring(0, 8).TrimStart('0');
            }
            catch (Exception ex)
            {
                if (ShowLog)
                    logHelper.Error(ex.Message + Environment.NewLine + "获取用户卡号出错" + " " + RawDataStr);
                return "获取用户卡号出错";
            }

            try
            {
                SerialNumber = data.Substring(8, 8);
            }
            catch (Exception ex)
            {
                if (ShowLog)
                    logHelper.Error(ex.Message + Environment.NewLine + "获取卡出厂序列号出错" + " " + RawDataStr);
                return "获取卡出厂序列号出错";
            }

            try
            {
                StartTime = DateTime.ParseExact("20" + data.Substring(16, 12), "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                if (ShowLog)
                    logHelper.Error(ex.Message + Environment.NewLine + "获取开泵时间出错" + " " + RawDataStr);
                return "获取开泵时间出错";
            }

            if (StartTime.Year < 2000)
            {
                if (ShowLog)
                    logHelper.Error("获取开泵时间出错!时间年份不能小于2000" + Environment.NewLine + RawDataStr);
                return "获取开泵时间出错!时间年份不能小于2000";
            }

            try
            {
                StartResidualWater = decimal.Parse(data.Substring(28, 8)) / 10m;
            }
            catch (Exception ex)
            {
                if (ShowLog)
                    logHelper.Error(ex.Message + Environment.NewLine + "获取开泵卡剩余水量出错" + " " + RawDataStr);
                return "获取开泵卡剩余水量出错";
            }

            try
            {
                StartResidualElectric = decimal.Parse(data.Substring(36, 8)) / 10m;
            }
            catch (Exception ex)
            {
                if (ShowLog)
                    logHelper.Error(ex.Message + Environment.NewLine + "获取开泵卡剩余电量出错" + " " + RawDataStr);
                return "获取开泵卡剩余电量出错";
            }

            try
            {
                YearElectricUsed = decimal.Parse(data.Substring(44, 8)) / 10m;
            }
            catch (Exception ex)
            {
                if (ShowLog)
                    logHelper.Error(ex.Message + Environment.NewLine + "获取年累计用电量出错" + " " + RawDataStr);
                return "获取年累计用电量出错";
            }

            try
            {
                YearWaterUsed = decimal.Parse(data.Substring(52, 8)) / 10m;
            }
            catch (Exception ex)
            {
                if (ShowLog)
                    logHelper.Error(ex.Message + Environment.NewLine + "获取年累计用水量出错" + " " + RawDataStr);
                return "获取年累计用水量出错";
            }

            try
            {
                YearExploitation = decimal.Parse(data.Substring(60, 8)) / 10m;
            }
            catch (Exception ex)
            {
                if (ShowLog)
                    logHelper.Error(ex.Message + Environment.NewLine + "获取年可开采量出错" + " " + RawDataStr);
                return "获取年可开采量出错";
            }

            try
            {
                YearSurplus = decimal.Parse(data.Substring(68, 8)) / 10m;
            }
            catch (Exception ex)
            {
                if (ShowLog)
                    logHelper.Error(ex.Message + Environment.NewLine + "获取年剩余可开采量出错" + " " + RawDataStr);
                return "获取年剩余可开采量出错";
            }

            return "";
        }
    }
}
