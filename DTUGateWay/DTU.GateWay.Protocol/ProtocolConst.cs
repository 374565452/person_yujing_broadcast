namespace DTU.GateWay.Protocol
{
    public class ProtocolConst
    {
        //解析命令缓存
        public static int InitBufferSize = 1024 * 4;
        //IOCP接收数据缓存大小，设置过小会造成事件响应增多，设置过大会造成内存占用偏多
        public static int ReceiveBufferSize = 1024 * 4;

        /// <summary>
        /// 平台协议转义码
        /// </summary>
        public const byte CmdPlatFormSafeCode = 0x55;

        /// <summary>
        /// 设备协议转义码
        /// </summary>
        public const byte CmdDeviceSafeCode = 0x54;

        /// <summary>
        /// 单个命令最长数据长度
        /// </summary>
        public const int CmdMaxLength = 1024 * 2;
        /// <summary>
        /// 单个命令最短数据长度
        /// </summary>
        public const int CmdMinLength = 12;

        /// <summary>
        /// 更新网关缓存起始十六进制字符串，6位
        /// </summary>
        public const string WebToGateUpdateCache = "215487";

        /// <summary>
        /// 更新网关终端缓存
        /// </summary>
        public const string UpdateCache_Device = "01";

        /// <summary>
        /// 更新网关用户卡缓存
        /// </summary>
        public const string UpdateCache_CardUser = "02";

        /// <summary>
        /// 更新网关地区缓存
        /// </summary>
        public const string UpdateCache_District = "03";
    }

    public class ProtocolKey
    {
        //回复DTU的心跳
        public static string SendHeartThrob = "%";
        //接收到的心跳
        public static string ReceiveHeartThrob = "$";
    }
}
