using Common;
using System;

namespace DTU.GateWay.Protocol
{
    public class CmdResponseToDtuQuerySOILMOISTURE : BaseMessage
    {
        public CmdResponseToDtuQuerySOILMOISTURE()
        {
            BeginChar = BaseProtocol.BeginChar;
            EndChar = BaseProtocol.EndChar;

            //AFN = 0x73;
            AFN = (byte)BaseProtocol.AFN.ToDtuQuerySOILMOISTURE;
            //ControlField = 0xB3;
            ControlField = (byte)BaseProtocol.ControlField.FromDtu;
        }

        public CmdResponseToDtuQuerySOILMOISTURE(BaseMessage bm)
        {
            BeginChar = BaseProtocol.BeginChar;
            EndChar = BaseProtocol.EndChar;

            //AFN = 0x73;
            AFN = (byte)BaseProtocol.AFN.ToDtuQuerySOILMOISTURE;
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
            try
            {
                string data = UserData;

                return "";
            }
            catch (Exception ex)
            {
                if (ShowLog)
                    logHelper.Error(ex.Message + Environment.NewLine + "获取墒情查询响应信息出错" + " " + RawDataStr);
                return "获取墒情查询响应信息出错";
            }
        }
    }
}
