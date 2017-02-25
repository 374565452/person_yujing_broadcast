using System;
using System.ComponentModel;

namespace DTU.GateWay.Protocol
{
    public class WaterBaseProtocol
    {
        /// <summary>
        /// 默认中心站地址
        /// </summary>
        public static byte CenterStation = 0x02;

        /// <summary>
        /// 起始字符
        /// </summary>
        public static byte[] BeginChar = new byte[] { 0x7E, 0x7E };

        /// <summary>
        /// 遥测站分类码
        /// </summary>
        public enum StationType
        {
            /// <summary>
            /// 地下水
            /// </summary>
            [Description("地下水")]
            GroundWater = 0x47,
            /// <summary>
            /// 河道
            /// </summary>
            [Description("河道")]
            River = 0x48
        }

        /// <summary>
        /// 默认密码
        /// </summary>
        public static string PW = "0000";

        /// <summary>
        /// 分包每包正文大小，字节数
        /// </summary>
        public static int PocketSize = 223;

        /// <summary>
        /// 上行or下行
        /// </summary>
        public enum UpOrDown
        {
            /// <summary>
            /// 上行
            /// </summary>
            [Description("上行")]
            Up = 0,
            /// <summary>
            /// 下行
            /// </summary>
            [Description("下行")]
            Down = 8
        }

        /// <summary>
        /// 功能码
        /// </summary>
        public enum AFN
        {
            /// <summary>
            /// 雨量预警阈值设置
            /// </summary>
            [Description("雨量预警阈值设置")]
            _20 = 0x20,
            /// <summary>
            /// 手动雨量或水位预警发布
            /// </summary>
            [Description("手动雨量或水位预警发布")]
            _21 = 0x21,
            /// <summary>
            /// 雨量预警阈值读取
            /// </summary>
            [Description("雨量预警阈值读取")]
            _22 = 0x22,
            /// <summary>
            /// 预警责任人号码修改
            /// </summary>
            [Description("预警责任人号码修改")]
            _23 = 0x23,
            /// <summary>
            /// 预警责任人号码读取
            /// </summary>
            [Description("预警责任人号码读取")]
            _24 = 0x24,
            /// <summary>
            /// 扩展参数读取
            /// </summary>
            [Description("扩展参数设置")]
            _25 = 0x25,
            /// <summary>
            /// 扩展参数设置
            /// </summary>
            [Description("扩展参数读取")]
            _26 = 0x26,
            /// <summary>
            /// 扩展参数设置
            /// </summary>
            [Description("语音播报")]
            _27 = 0x27,

            //kqz 2017-1-1 添加
            /// <summary>
            /// 水位预警阈值设置
            /// </summary>
            [Description("水位预警阈值设置")]
            _28 = 0x28,
            /// <summary>
            /// 水位预警阈值读取
            /// </summary>
            [Description("水位预警阈值读取")]
            _29 = 0x29,
            //kqz 2017-1-1 添加
            /// <summary>
            /// 链路维持报
            /// </summary>
            [Description("链路维持报")]
            _2F = 0x2F,
            /// <summary>
            /// 测试报
            /// </summary>
            [Description("测试报")]
            _30 = 0x30,
            /// <summary>
            /// 均匀时段水文信息报
            /// </summary>
            [Description("均匀时段水文信息报")]
            _31 = 0x31,
            /// <summary>
            /// 遥测站定时报
            /// </summary>
            [Description("遥测站定时报")]
            _32 = 0x32,
            /// <summary>
            /// 遥测站加报报
            /// </summary>
            [Description("遥测站加报报")]
            _33 = 0x33,
            /// <summary>
            /// 遥测站小时报
            /// </summary>
            [Description("遥测站小时报")]
            _34 = 0x34,
            /// <summary>
            /// 遥测站人工置数报
            /// </summary>
            [Description("遥测站人工置数报")]
            _35 = 0x35,
            /// <summary>
            /// 遥测站图片报或中心站查询遥测站图片采集信息
            /// </summary>
            [Description("遥测站图片报或中心站查询遥测站图片采集信息")]
            _36 = 0x36,
            /// <summary>
            /// 中心站查询遥测站实时数据
            /// </summary>
            [Description("中心站查询遥测站实时数据")]
            _37 = 0x37,
            /// <summary>
            /// 中心站查询遥测站时段数据
            /// </summary>
            [Description("中心站查询遥测站时段数据")]
            _38 = 0x38,
            /// <summary>
            /// 中心站查询遥测站人工置数
            /// </summary>
            [Description("中心站查询遥测站人工置数")]
            _39 = 0x39,
            /// <summary>
            /// 中心站查询遥测站指定要素实时数据
            /// </summary>
            [Description("中心站查询遥测站指定要素实时数据")]
            _3A = 0x3A,
            /// <summary>
            /// 中心站修改遥测站基本配置表
            /// </summary>
            [Description("中心站修改遥测站基本配置表")]
            _40 = 0x40,
            /// <summary>
            /// 中心站读取遥测站基本配置表/遥测站自报基本配置表
            /// </summary>
            [Description("中心站读取遥测站基本配置表/遥测站自报基本配置表")]
            _41 = 0x41,
            /// <summary>
            /// 中心站修改遥测站运行参数配置表
            /// </summary>
            [Description("中心站修改遥测站运行参数配置表")]
            _42 = 0x42,
            /// <summary>
            /// 中心站读取遥测站运行参数配置表/遥测站自报运行参数配置表
            /// </summary>
            [Description("中心站读取遥测站运行参数配置表/遥测站自报运行参数配置表")]
            _43 = 0x43,
            /// <summary>
            /// 中心站查询水泵电机实时工作数据
            /// </summary>
            [Description("中心站查询水泵电机实时工作数据")]
            _44 = 0x44,
            /// <summary>
            /// 中心站查询遥测站软件版本
            /// </summary>
            [Description("中心站查询遥测站软件版本")]
            _45 = 0x45,
            /// <summary>
            /// 中心站查询遥测站状态和报警信息
            /// </summary>
            [Description("中心站查询遥测站状态和报警信息")]
            _46 = 0x46,
            /// <summary>
            /// 初始化固态存储数据
            /// </summary>
            [Description("初始化固态存储数据")]
            _47 = 0x47,
            /// <summary>
            /// 恢复遥测站出厂设置
            /// </summary>
            [Description("恢复遥测站出厂设置")]
            _48 = 0x48,
            /// <summary>
            /// 修改密码
            /// </summary>
            [Description("修改密码")]
            _49 = 0x49,
            /// <summary>
            /// 设置遥测站时钟
            /// </summary>
            [Description("设置遥测站时钟")]
            _4A = 0x4A,
            /// <summary>
            /// 设置遥测站 IC 卡状态
            /// </summary>
            [Description("设置遥测站 IC 卡状态")]
            _4B = 0x4B,
            /// <summary>
            /// 控制水泵开关命令/水泵状态自报
            /// </summary>
            [Description("控制水泵开关命令/水泵状态自报")]
            _4C = 0x4C,
            /// <summary>
            /// 控制阀门开关命令/阀门状态信息自报
            /// </summary>
            [Description("控制阀门开关命令/阀门状态信息自报")]
            _4D = 0x4D,
            /// <summary>
            /// 控制闸门开关命令/闸门状态信息自报
            /// </summary>
            [Description("控制闸门开关命令/闸门状态信息自报")]
            _4E = 0x4E,
            /// <summary>
            /// 水量定值控制命令
            /// </summary>
            [Description("水量定值控制命令")]
            _4F = 0x4F,
            /// <summary>
            /// 中心站查询遥测站事件记录
            /// </summary>
            [Description("中心站查询遥测站事件记录")]
            _50 = 0x50,
            /// <summary>
            /// 中心站查询遥测站时钟
            /// </summary>
            [Description("中心站查询遥测站时钟")]
            _51 = 0x51

        }

        /// <summary>
        /// 控制字符定义，起始字符
        /// </summary>
        public enum DataBeginChar
        {
            /// <summary>
            /// 传输正文起始，不分包
            /// </summary>
            [Description("传输正文起始，不分包")]
            STX = 0x02,
            /// <summary>
            /// 多包传输正文起始，分包
            /// </summary>
            [Description("多包传输正文起始，分包")]
            SYN = 0x16
        }

        /// <summary>
        /// 控制字符定义，上行结束字符
        /// </summary>
        public enum DataEndChar_Up
        {
            /// <summary>
            /// 报文结束，后续有报文
            /// </summary>
            [Description("报文结束，后续有报文")]
            ETB = 0x17,
            /// <summary>
            /// 报文结束，后续无报文
            /// </summary>
            [Description("报文结束，后续无报文")]
            ETX = 0x03
        }

        /// <summary>
        /// 控制字符定义，上行结束字符
        /// </summary>
        public enum DataEndChar_Down
        {
            /// <summary>
            /// 下行查询与控制 下行结束符
            /// </summary>
            [Description("下行查询与控制")]
            ENQ = 0x05,
            /// <summary>
            /// ETB肯定确认，继续发送
            /// </summary>
            [Description("ETB肯定确认，继续发送")]
            ACK = 0x06,
            /// <summary>
            /// ETB否定应答，反馈重发
            /// </summary>
            [Description("ETB否定应答，反馈重发")]
            NAK = 0x15,
            /// <summary>
            /// ETX传输结束，退出
            /// </summary>
            [Description("ETX传输结束，退出")]
            EOT = 0x04,
            /// <summary>
            /// ETX传输结束，终端保持在线
            /// </summary>
            [Description("ETX传输结束，终端保持在线")]
            ESC = 0x1B
        }

        public static int[] GetLengthList(string HEXStr)
        {
            int[] list = new int[2] { 0, 0 };
            try
            {
                string str = HEXStr;
                if (HEXStr.Length > 2)
                {
                    str = HEXStr.Substring(0, 2);
                }
                string BinaryStr = Convert.ToString(Convert.ToInt32(str, 16), 2).PadLeft(8, '0');
                string ByteCount = BinaryStr.Substring(0, 5);
                string Digits = BinaryStr.Substring(5, 3);
                list[0] = Convert.ToInt32(ByteCount, 2);
                list[1] = Convert.ToInt32(Digits, 2);
            }
            catch 
            {
            }
            return list;
        }

        public static string GetLengthHexStr(int ByteCount, int Digits)
        {
            if (ByteCount < 0 || ByteCount > 31 || Digits < 0 || Digits > 7)
            {
                return "00";
            }
            else
            {
                string BinaryStr = Convert.ToString(ByteCount, 2).PadLeft(5, '0') + Convert.ToString(Digits, 2).PadLeft(3, '0');
                return Convert.ToString(Convert.ToInt32(BinaryStr, 2), 16).ToUpper();
            }
        }
    }
}
