using Common;
using System;

namespace DTU.GateWay.Protocol
{
    public class CmdResponseToDtuQueryGroundWaterParam: BaseMessage
    {
        public CmdResponseToDtuQueryGroundWaterParam()
        {
            BeginChar = BaseProtocol.BeginChar;
            EndChar = BaseProtocol.EndChar;

            //AFN = 0x58;
            AFN = (byte)BaseProtocol.AFN.ToDtuQueryGroundWaterParam;
            //ControlField = 0xB3;
            ControlField = (byte)BaseProtocol.ControlField.FromDtu;
        }

        public CmdResponseToDtuQueryGroundWaterParam(BaseMessage bm)
        {
            BeginChar = BaseProtocol.BeginChar;
            EndChar = BaseProtocol.EndChar;

            //AFN = 0x58;
            AFN = (byte)BaseProtocol.AFN.ToDtuQueryGroundWaterParam;
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
        /// 量程，1字节，米
        /// </summary>
        public byte Range
        {
            set;
            get;
        }

        /// <summary>
        /// 投入线长，2字节,两位小数
        /// </summary>
        public double LineLength
        {
            set;
            get;
        }

        public override byte[] WriteMsg()
        {
            string data = Range.ToString("X").PadLeft(2, '0') + ((int)(LineLength * 100)).ToString("X").PadLeft(4, '0');

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
            Range = 0;
            LineLength = 0;

            string data = UserData;

            try
            {
                Range = Convert.ToByte(data.Substring(0, 2), 16);
            }
            catch
            {
                return "获取量程出错";
            }

            try
            {
                LineLength = Convert.ToInt32(data.Substring(2, 4), 16) / 100.0;
            }
            catch
            {
                return "获取投入线长出错";
            }

            return "";
        }
    }
}
