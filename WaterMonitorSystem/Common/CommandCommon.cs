using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class CommandCommon
    {

        /// <summary>
        /// 透传类型
        /// </summary>
        public enum CmdTranTargetType
        {
            /// <summary>
            /// 透传至所有目标socket
            /// </summary>
            ToAllTarget,
            /// <summary>
            /// 透传至指定socket
            /// </summary>
            ToDeviceTarget
        }

        /// <summary>
        /// 透传方式
        /// </summary>
        public enum CmdTranType
        {
            /// <summary>
            /// 只透传指令数据
            /// </summary>
            CmdParamData,
            /// <summary>
            /// 透传整条指令
            /// </summary>
            CmdData
        }

        /// <summary>
        /// 指令类别
        /// </summary>
        public enum SourceCmdType
        {
            /// <summary>
            /// 设备指令
            /// </summary>
            DeviceCmd,
            /// <summary>
            /// 平台指令
            /// </summary>
            PlatformCmd,
            /// <summary>
            /// 未知指令
            /// </summary>
            UnknowCmd
        }

        /// <summary>
        /// 当前数据来源
        /// </summary>
        public enum CmdSource
        {
            /// <summary>
            /// DTU终端
            /// </summary>
            CMD_SRC_DTU_DEVICE,
            /// <summary>
            /// BS管理平台
            /// </summary>
            CMD_SRC_WEB,
            /// <summary>
            /// 上级平台
            /// </summary>
            CMD_SRC_PLATFORM
        }

        /// <summary>
        /// 网络通信缓存
        /// </summary>
        public class SocketBuffer
        {
            public CmdBuffer Buffer;
            public IOCPSocket Socket;
            public DateTime LastDataReceived;

            public SocketBuffer()
            {
                LastDataReceived = DateTime.Now;
                Buffer = new CmdBuffer();
                Socket = new IOCPSocket();
            }
        }


        #region 基本配置参数定义
        /// <summary>
        /// 平台转义码
        /// </summary>
        public const byte CMD_PLATFORM_SAFE_CODE = 0x55;

        /// <summary>
        /// 设备协议转义码
        /// </summary>
        public const byte CMD_DEVICE_SAFE_CODE = 0x54;

        /// <summary>
        /// 消息体最长长度
        /// </summary>
        public const ushort CMD_MSG_BODY_LENGTH = 2 * 1024;

        /// <summary>
        /// buffer缓存数据长度
        /// </summary>
        public const int CMD_BUFFER_LENGTH = 1024 * 10;
        /// <summary>
        /// 单个命令最长数据长度
        /// </summary>
        public const int CMD_MAX_LENGTH = 1024 * 2;
        /// <summary>
        /// 单个命令最短数据长度
        /// </summary>
        public const int CMD_MIN_LENGTH = 8;

        /// <summary>
        /// 队列最多未处理的数据长度
        /// </summary>
        public const int QUEUE_MAX_COUNT = 5000;

        #endregion
    }

    /// <summary>
    /// 设备发送的命令
    /// </summary>
    public class DeviceCommand
    {
        /// <summary>
        /// 0x0001 通用应答
        /// </summary>
        public const UInt16 CMD_0x0001 = 0x0001;
        /// <summary>
        /// 0x0002 心跳
        /// </summary>
        public const UInt16 CMD_0x0002 = 0x0002;
        /// <summary>
        /// 0x0004 校时请求
        /// </summary>
        public const UInt16 CMD_0x0004 = 0x0004;
        /// <summary>
        /// 0x0102 鉴权
        /// </summary>
        public const UInt16 CMD_0x0102 = 0x0102;
        /// <summary>
        /// 0x0104 终端参数应答
        /// </summary>
        public const UInt16 CMD_0x0104 = 0x0104;
        /// <summary>
        /// 0x0105 升级结果报告
        /// </summary>
        public const UInt16 CMD_0x0105 = 0x0105;
        /// <summary>
        /// 0x0106 终端自检报告
        /// </summary>
        public const UInt16 CMD_0x0106 = 0x0106;
        /// <summary>
        /// 0x0200 实时数据汇报
        /// </summary>
        public const UInt16 CMD_0x0200 = 0x0200;
        /// <summary>
        /// 0x0201 实时查询应答
        /// </summary>
        public const UInt16 CMD_0x0201 = 0x0201;
        /// <summary>
        /// 0x0202 发送历史数据
        /// </summary>
        public const UInt16 CMD_0x0202 = 0x0202;

    }

    /// <summary>
    /// 服务器发送的命令
    /// </summary>
    public class ServerCommand
    {
        /// <summary>
        /// 0x8001 服务端通用应答
        /// </summary>
        public const UInt16 CMD_0x8001 = 0x8001;
        /// <summary>
        /// 0x8004 校时应答
        /// </summary>
        public const UInt16 CMD_0x8004 = 0x8004;
        /// <summary>
        /// 0x8103 设置终端参数
        /// </summary>
        public const UInt16 CMD_0x8103 = 0x8103;
        /// <summary>
        /// 0x8104 查询终端参数
        /// </summary>
        public const UInt16 CMD_0x8104 = 0x8104;
        /// <summary>
        /// 0x8105 终端控制
        /// </summary>
        public const UInt16 CMD_0x8105 = 0x8105;
        /// <summary>
        /// 0x8B04 报警确认
        /// </summary>
        public const UInt16 CMD_0x8B04 = 0x8B04;
        /// <summary>
        /// 0x8201 实时查询
        /// </summary>
        public const UInt16 CMD_0x8201 = 0x8201;
    }
}
