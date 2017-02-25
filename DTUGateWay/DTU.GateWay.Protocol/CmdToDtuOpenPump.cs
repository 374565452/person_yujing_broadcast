using Common;
using System;
using System.Collections.Generic;

namespace DTU.GateWay.Protocol
{
    public class CmdToDtuOpenPump : BaseMessage
    {
        public CmdToDtuOpenPump()
        {
            BeginChar = BaseProtocol.BeginChar;
            EndChar = BaseProtocol.EndChar;

            //AFN = 0x53;
            AFN = (byte)BaseProtocol.AFN.ToDtuOpenPump;
            //ControlField = 0x33;
            ControlField = (byte)BaseProtocol.ControlField.ToDtu;
        }

        public CmdToDtuOpenPump(BaseMessage bm)
        {
            BeginChar = BaseProtocol.BeginChar;
            EndChar = BaseProtocol.EndChar;

            //AFN = 0x53;
            AFN = (byte)BaseProtocol.AFN.ToDtuOpenPump;
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

            DateTime dateNow = DateTime.Now;
            IsPW = true;
            PW = "0000";
            IsTP = true;
            TP = dateNow.ToString("ssmmHHdd") + "00";

            UserData = data;

            UserDataBytes = HexStringUtility.HexStringToByteArray(UserData);

            return WriteMsg2();
        }

        public byte[] WriteMsg(DateTime dt)
        {
            string data = "";

            DateTime dateNow = dt;
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
            return "";
        }
    }
}
