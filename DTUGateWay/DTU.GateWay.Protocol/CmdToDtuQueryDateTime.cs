using Common;
using System.Collections.Generic;

namespace DTU.GateWay.Protocol
{
    public class CmdToDtuQueryDateTime : BaseMessage
    {
        public CmdToDtuQueryDateTime()
        {
            BeginChar = BaseProtocol.BeginChar;
            EndChar = BaseProtocol.EndChar;

            //AFN = 0x51;
            AFN = (byte)BaseProtocol.AFN.ToDtuQueryDateTime;
            //ControlField = 0x33;
            ControlField = (byte)BaseProtocol.ControlField.ToDtu;
        }

        public CmdToDtuQueryDateTime(BaseMessage bm)
        {
            BeginChar = BaseProtocol.BeginChar;
            EndChar = BaseProtocol.EndChar;

            //AFN = 0x51;
            AFN = (byte)BaseProtocol.AFN.ToDtuQueryDateTime;
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

        public override byte[] WriteMsg()
        {
            string data = "";

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
            return "";
        }
    }
}
