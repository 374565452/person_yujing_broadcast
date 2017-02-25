using Common;
using System;

namespace DTU.GateWay.Protocol
{
    public class CmdToDtuShieldSerialNumberCancel: BaseMessage
    {
        public CmdToDtuShieldSerialNumberCancel()
        {
            BeginChar = BaseProtocol.BeginChar;
            EndChar = BaseProtocol.EndChar;

            //AFN = 0x86;
            AFN = (byte)BaseProtocol.AFN.ToDtuShieldSerialNumberCancel;
            //ControlField = 0x33;
            ControlField = (byte)BaseProtocol.ControlField.ToDtu;
        }

        public CmdToDtuShieldSerialNumberCancel(BaseMessage bm)
        {
            BeginChar = BaseProtocol.BeginChar;
            EndChar = BaseProtocol.EndChar;

            //AFN = 0x86;
            AFN = (byte)BaseProtocol.AFN.ToDtuShieldSerialNumberCancel;
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
        /// 卡出厂序列号，4字节，16进制
        /// </summary>
        public string SerialNumber
        {
            set;
            get;
        }

        public override byte[] WriteMsg()
        {
            string data = "";
            data += SerialNumber.PadLeft(8, '0');

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
            SerialNumber = "00000000";

            string data = UserData;

            try
            {
                SerialNumber = data.Substring(0, 8);
            }
            catch
            {
                return "获取卡出厂序列号出错";
            }

            return "";
        }
    }
}
