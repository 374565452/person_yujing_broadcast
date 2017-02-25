using DTU.GateWay.Protocol;
using Server.Util.DataProcess;
using System;

namespace Server.Web.ParseData
{
    public class WebParseData : ReceiveData
    {
        public Command Command
        {
            get;
            set;
        }

        public WebParseData()
        {
            Command = null;
        }

        public override bool parsePacket(byte[] packet, int offset, int length)
        {
            parseCommand(packet, length);
            return true;
        }

        public override object returnObject()
        {
            return Command;
        }

        private void parseCommand(byte[] packet, int length)
        {
            Command = new Command();
            Command.CommandType = packet[3];
            Command.CommandCode = packet[4];
            Command.CommandState = packet[5];

            Array.Copy(packet, 6, Command.DeviceInfo.SerialNum, 0, 7);
            Command.DeviceInfo.Parse(Command.DeviceInfo.SerialNum);

            Command.Data = new byte[length - ProtocolConst.CmdMinLength];
            Array.Copy(packet, 13, Command.Data, 0, Command.Data.Length);
        }
    }
}
