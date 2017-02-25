namespace DTU.GateWay.Protocol
{
    public class Command
    {
        /// <summary>
        /// 当前指令的socketID
        /// </summary>
        public string SocketID
        {
            get;
            set;
        }
        /// <summary>
        /// 协议体
        /// </summary>
        public byte CommandType
        {
            get;
            set;
        }
        /// <summary>
        /// 命令字
        /// </summary>
        public byte CommandCode
        {
            get;
            set;
        }
        /// <summary>
        /// 执行状态
        /// </summary>
        public byte CommandState
        {
            get;
            set;
        }
        /// <summary>
        /// 设备信息
        /// </summary>
        public DeviceInfo DeviceInfo
        {
            get;
            set;
        }
        /// <summary>
        /// 数据区
        /// </summary>
        public byte[] Data
        {
            get;
            set;
        }

        public Command()
        {
            CommandState = 0x00;
            DeviceInfo = new DeviceInfo();
            Data = new byte[0];
            SocketID = "";
        }
    }
    /// <summary>
    /// 命令执行状态
    /// </summary>
    public class CommandState
    {
        /// <summary>
        /// 命令执行成功
        /// </summary>
        public const byte CmdStateSuccess = 0x00;
        /// <summary>
        /// 命令执行失败
        /// </summary>
        public const byte CmdStateFail = 0x01;
        /// <summary>
        /// 主动发送的指令
        /// </summary>
        public const byte CmdStateSend = 0x02;
        /// <summary>
        /// 命令不支持
        /// </summary>
        public const byte CmdStateNotSupport = 0x03;


    }
}
