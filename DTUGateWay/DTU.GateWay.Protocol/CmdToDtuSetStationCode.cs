using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTU.GateWay.Protocol
{
    public class CmdToDtuSetStationCode : BaseMessage
    {
        public CmdToDtuSetStationCode()
        {
            BeginChar = BaseProtocol.BeginChar;
            EndChar = BaseProtocol.EndChar;

            //AFN = 0x56;
            AFN = (byte)BaseProtocol.AFN.ToDtuSetStationCode;
            //ControlField = 0x33;
            ControlField = (byte)BaseProtocol.ControlField.ToDtu;
        }

        public CmdToDtuSetStationCode(BaseMessage bm)
        {
            BeginChar = BaseProtocol.BeginChar;
            EndChar = BaseProtocol.EndChar;

            //AFN = 0x56;
            AFN = (byte)BaseProtocol.AFN.ToDtuSetStationCode;
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
        /// 射频地址list
        /// </summary>
        public List<int> StationCodeList
        {
            set;
            get;
        }

        public override byte[] WriteMsg()
        {
            string data = "";
            foreach (int StationCodeSub in StationCodeList)
            {
                data += StationCodeSub.ToString("X").PadLeft(4, '0');
            }

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
            StationCodeList = new List<int>();

            try
            {
                byte[] StationCodeListBytes = new byte[UserDataBytes.Length];
                Array.Copy(UserDataBytes, 0, StationCodeListBytes, 0, StationCodeListBytes.Length);
                for (int i = 0; i < StationCodeListBytes.Length / 2; i++)
                {
                    byte[] bytes = new byte[] { StationCodeListBytes[i * 2], StationCodeListBytes[i * 2 + 1] };
                    StationCodeList.Add(Convert.ToInt32(HexStringUtility.ByteArrayToHexString(bytes), 16));
                }
            }
            catch(Exception ex)
            {
                return "获取射频地址list出错！" + ex.Message;
            }

            return "";
        }
    }
}
