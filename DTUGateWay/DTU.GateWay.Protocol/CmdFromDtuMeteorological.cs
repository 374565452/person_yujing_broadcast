using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTU.GateWay.Protocol
{
    public class CmdFromDtuMeteorological : BaseMessage
    {
        public CmdFromDtuMeteorological()
        {
            BeginChar = BaseProtocol.BeginChar;
            EndChar = BaseProtocol.EndChar;

            //AFN = 0x62;
            AFN = (byte)BaseProtocol.AFN.FromDtuMeteorological;
            //ControlField = 0xB3;
            ControlField = (byte)BaseProtocol.ControlField.FromDtu;
        }

        public CmdFromDtuMeteorological(BaseMessage bm)
        {
            BeginChar = BaseProtocol.BeginChar;
            EndChar = BaseProtocol.EndChar;

            //AFN = 0x62;
            AFN = (byte)BaseProtocol.AFN.FromDtuMeteorological;
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
                    logHelper.Error(ex.Message + Environment.NewLine + "获取气象上报信息出错 " + RawDataStr);
                return "获取气象上报信息出错";
            }
        }
    }
}
