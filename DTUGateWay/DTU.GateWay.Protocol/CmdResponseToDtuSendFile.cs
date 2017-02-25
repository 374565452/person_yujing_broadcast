using Common;
using System;

namespace DTU.GateWay.Protocol
{
    public class CmdResponseToDtuSendFile : BaseMessage
    {
        public CmdResponseToDtuSendFile()
        {
            BeginChar = BaseProtocol.BeginChar;
            EndChar = BaseProtocol.EndChar;

            //AFN = 0x87;
            AFN = (byte)BaseProtocol.AFN.ToDtuSendFile;
            //ControlField = 0xB3;
            ControlField = (byte)BaseProtocol.ControlField.FromDtu;
        }

        public CmdResponseToDtuSendFile(BaseMessage bm)
        {
            BeginChar = BaseProtocol.BeginChar;
            EndChar = BaseProtocol.EndChar;

            //AFN = 0x87;
            AFN = (byte)BaseProtocol.AFN.ToDtuSendFile;
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
        /// 总包数，2字节
        /// </summary>
        public short Sum
        {
            set;
            get;
        }

        /// <summary>
        /// 当前包数，2字节
        /// </summary>
        public short Curr
        {
            set;
            get;
        }

        /// <summary>
        /// 应答结果，0 退出，1 继续
        /// </summary>
        public byte Result
        {
            set;
            get;
        }

        public override byte[] WriteMsg()
        {
            string data = Sum.ToString("X").PadLeft(4, '0');
            data += Curr.ToString("X").PadLeft(4, '0');
            data += HexStringUtility.ByteArrayToHexString(new byte[] { Result });

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
                Sum = Convert.ToInt16(UserData.Substring(0, 4), 16);
            }
            catch (Exception ex)
            {
                Sum = 0;
                if (ShowLog)
                    logHelper.Error(ex.Message + Environment.NewLine + "获取总包数出错" + " " + RawDataStr);
                return "获取总包数出错";
            }

            try
            {
                Curr = Convert.ToInt16(UserData.Substring(4, 4), 16);
            }
            catch (Exception ex)
            {
                Curr = 0;
                if (ShowLog)
                    logHelper.Error(ex.Message + Environment.NewLine + "获取当前包数出错" + " " + RawDataStr);
                return "获取当前包数出错";
            }


            try
            {
                Result = HexStringUtility.HexStringToByteArray(UserData)[4];
            }
            catch (Exception ex)
            {
                if (ShowLog)
                    logHelper.Error(ex.Message + Environment.NewLine + "获取应答结果出错" + " " + RawDataStr);
                return "获取应答结果出错";
            }

            return "";
        }
    }
}
