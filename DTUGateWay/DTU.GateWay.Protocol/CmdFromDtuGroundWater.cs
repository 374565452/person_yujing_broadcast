using Common;
using System;

namespace DTU.GateWay.Protocol
{
    public class CmdFromDtuGroundWater : BaseMessage
    {
        public CmdFromDtuGroundWater()
        {
            BeginChar = BaseProtocol.BeginChar;
            EndChar = BaseProtocol.EndChar;

            //AFN = 0x61;
            AFN = (byte)BaseProtocol.AFN.FromDtuGroundWater;
            //ControlField = 0xB3;
            ControlField = (byte)BaseProtocol.ControlField.FromDtu;
        }

        public CmdFromDtuGroundWater(BaseMessage bm)
        {
            BeginChar = BaseProtocol.BeginChar;
            EndChar = BaseProtocol.EndChar;

            //AFN = 0x61;
            AFN = (byte)BaseProtocol.AFN.FromDtuGroundWater;
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
        /// 观测时间，6字节
        /// </summary>
        public DateTime Acq_Time
        {
            set;
            get;
        }

        /// <summary>
        /// 水位计数据，2字节，小数两位，单位米
        /// </summary>
        public double GroundWaterLevel
        {
            set;
            get;
        }

        /// <summary>
        /// 投入线长，2字节，小数两位，单位米
        /// </summary>
        public double LineLength
        {
            set;
            get;
        }

        /// <summary>
        /// 水温，2字节，小数一位，单位℃
        /// </summary>
        public double GroundWaterTempture
        {
            set;
            get;
        }

        /// <summary>
        /// 保留数据，3字节
        /// </summary>
        public byte Retain1
        {
            set;
            get;
        }

        public byte Retain2
        {
            set;
            get;
        }

        public byte Retain3
        {
            set;
            get;
        }

        /// <summary>
        /// 单独校验，1字节
        /// </summary>
        public byte CRC
        {
            set;
            get;
        }

        public override byte[] WriteMsg()
        {
            int yy = Acq_Time.Year - 2000;
            int MM = Acq_Time.Month;
            int dd = Acq_Time.Day;
            int HH = Acq_Time.Hour;
            int mm = Acq_Time.Minute;
            int ss = Acq_Time.Second;

            string data = yy.ToString().PadLeft(2, '0') + MM.ToString().PadLeft(2, '0') + dd.ToString().PadLeft(2, '0') +
                HH.ToString().PadLeft(2, '0') + mm.ToString().PadLeft(2, '0') + ss.ToString().PadLeft(2, '0') +
                ((int)(GroundWaterLevel * 100)).ToString("X").PadLeft(4, '0') +
                ((int)(LineLength * 100)).ToString("X").PadLeft(4, '0') +
                ((short)(GroundWaterTempture * 10)).ToString("X").PadLeft(4, '0') +
                Retain1.ToString("X").PadLeft(2, '0') +
                Retain2.ToString("X").PadLeft(2, '0') +
                Retain3.ToString("X").PadLeft(2, '0') +
                CRC.ToString("X").PadLeft(2, '0') +
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
            Acq_Time = DateTime.Parse("2000-1-1");
            GroundWaterLevel = 0;
            LineLength = 0;
            GroundWaterTempture = 0;
            Retain1 = 0;
            Retain2 = 0;
            Retain3 = 0;
            CRC = 0;

            string data = UserData;

            try
            {
                int yy = int.Parse(data.Substring(0, 2)) + 2000;
                int MM = int.Parse(data.Substring(2, 2));
                int dd = int.Parse(data.Substring(4, 2));
                int HH = int.Parse(data.Substring(6, 2));
                int mm = int.Parse(data.Substring(8, 2));
                int ss = int.Parse(data.Substring(10, 2));
                Acq_Time = new DateTime(yy, MM, dd, HH, mm, ss);
            }
            catch (Exception ex)
            {
                if (ShowLog)
                    logHelper.Error(ex.Message + Environment.NewLine + "获取观测时间出错 " + RawDataStr);
                return "获取观测时间出错";
            }

            try
            {
                GroundWaterLevel = Convert.ToInt32(data.Substring(12, 4), 16) / 100.0;
            }
            catch (Exception ex)
            {
                if (ShowLog)
                    logHelper.Error(ex.Message + Environment.NewLine + "获取水位计数据出错 " + RawDataStr);
                return "获取水位计数据出错";
            }

            try
            {
                LineLength = Convert.ToInt32(data.Substring(16, 4), 16) / 100.0;
            }
            catch (Exception ex)
            {
                if (ShowLog)
                    logHelper.Error(ex.Message + Environment.NewLine + "获取投入线长出错 " + RawDataStr);
                return "获取投入线长出错";
            }

            try
            {
                GroundWaterTempture = Convert.ToInt16(data.Substring(20, 4), 16) / 10.0;
            }
            catch (Exception ex)
            {
                if (ShowLog)
                    logHelper.Error(ex.Message + Environment.NewLine + "获取水温出错 " + RawDataStr);
                return "获取水温出错";
            }

            try
            {
                Retain1 = Convert.ToByte(data.Substring(24, 2), 16);
            }
            catch (Exception ex)
            {
                if (ShowLog)
                    logHelper.Error(ex.Message + Environment.NewLine + "获取保留数据1出错 " + RawDataStr);
                return "获取保留数据1出错";
            }

            try
            {
                Retain2 = Convert.ToByte(data.Substring(26, 2), 16);
            }
            catch (Exception ex)
            {
                if (ShowLog)
                    logHelper.Error(ex.Message + Environment.NewLine + "获取保留数据2出错 " + RawDataStr);
                return "获取保留数据2出错";
            }

            try
            {
                Retain3 = Convert.ToByte(data.Substring(28, 2), 16);
            }
            catch (Exception ex)
            {
                if (ShowLog)
                    logHelper.Error(ex.Message + Environment.NewLine + "获取保留数据3出错 " + RawDataStr);
                return "获取保留数据3出错";
            }

            try
            {
                CRC = Convert.ToByte(data.Substring(30, 2), 16);
            }
            catch (Exception ex)
            {
                if (ShowLog)
                    logHelper.Error(ex.Message + Environment.NewLine + "获取单独校验出错 " + RawDataStr);
                return "获取单独校验出错";
            }

            return "";
        }
    }
}
