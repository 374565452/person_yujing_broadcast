using System.ComponentModel;

namespace DTU.GateWay.Protocol
{
    public class BaseProtocol
    {
        /// <summary>
        /// 起始字符
        /// </summary>
        public static byte BeginChar = 0x68;

        /// <summary>
        /// 结束字符
        /// </summary>
        public static byte EndChar = 0x16;

        /// <summary>
        /// 终端不在线返回web结果
        /// </summary>
        public static string DeviceOffline = "123456789123456789123456789123456789";

        /// <summary>
        /// 发送完成返回web结果
        /// </summary>
        public static string DeviceSend = "987654321987654321987654321987654321";

        public enum AFN
        {
            [Description("设置遥测终端时钟")]
            ToDtuSetDateTime = 0x11,
            [Description("查询遥测终端时钟")]
            ToDtuQueryDateTime = 0x51,
            [Description("设置遥测终端的年度可开采水量")]
            ToDtuSetYearExploitation = 0x23,
            [Description("查询遥测终端的年度可开采水量")]
            ToDtuQueryYearExploitation = 0x24,
            [Description("状态自报数据")]
            FromDtuStateReport = 0x81,
            [Description("查询遥测终端状态数据")]
            ToDtuQueryState = 0x12,
            [Description("开泵数据")]
            FromDtuOpenPump = 0x83,
            [Description("关泵数据")]
            FromDtuClosePump = 0x84,
            [Description("终端注册登录")]
            FromDtuLogin = 0x55,
            [Description("远程开泵")]
            ToDtuOpenPump = 0x53,
            [Description("远程关泵")]
            ToDtuClosePump = 0x54,
            [Description("设置分站射频地址列表")]
            ToDtuSetStationCode = 0x56,
            [Description("水位上报")]
            FromDtuGroundWater = 0x61,
            [Description("气象上报")]
            FromDtuMeteorological = 0x62,
            [Description("墒情上报")]
            FromDtuSOILMOISTURE = 0x63,
            [Description("水位查询")]
            ToDtuQueryGroundWater = 0x71,
            [Description("气象查询")]
            ToDtuQueryMeteorological = 0x72,
            [Description("墒情查询")]
            ToDtuQuerySOILMOISTURE = 0x73,
            [Description("屏蔽卡号")]
            ToDtuShieldSerialNumber = 0x85,
            [Description("解除屏蔽卡号")]
            ToDtuShieldSerialNumberCancel = 0x86,
            [Description("水位计参数设置")]
            ToDtuSetGroundWaterParam = 0x57,
            [Description("水位计参数查询")]
            ToDtuQueryGroundWaterParam = 0x58,
            [Description("中心下发文件")]
            ToDtuSendFile = 0x87,
            [Description("终端上传文件")]
            FromDtuUploadFile = 0x88

        }

        public enum ControlField
        {
            [Description("终端接收数据")]
            ToDtu = 0x33,
            [Description("终端发送数据")]
            FromDtu = 0xB3
        }
    }
}
