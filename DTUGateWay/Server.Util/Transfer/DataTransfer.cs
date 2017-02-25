using DTU.GateWay.Protocol;
using Maticsoft.Model;

namespace Server.Util.Transfer
{
    /// <summary>
    /// 服务端程序，产生的数据需要向上层交互时传输用到的类
    /// </summary>
    public class DataTransfer
    {
        /// <summary>
        /// 如果为发送数据或命令，必须指定一个序列号
        /// </summary>
        public long DeviceInfo
        {
            get;
            set;
        }

        /// <summary>
        /// 数据传输类型
        /// </summary>
        public byte TransferType
        {
            get;
            set;
        }

        /// <summary>
        /// 如果类型为日志信息
        /// TrnsferInfo将保存日志
        /// </summary>
        public string TransferLogInfo
        {
            get;
            set;
        }

        /// <summary>
        /// 如果类型为真实数据
        /// TransferDataInfo将保存真实数据信息
        /// </summary>
        public DataTransferInfo TransferDataInfo
        {
            get;
            set;
        }
        /// <summary>
        /// 直接传送command指令
        /// </summary>
        public Command TransferCommand
        {
            get;
            set;
        }

        /// <summary>
        /// 发送deviceList到前端界面
        /// </summary>
        public Device DeviceList
        {
            get;
            set;
        }

        /// <summary>
        /// 发送DeviceEvent到前端界面
        /// </summary>
        public DeviceEvent DeviceEvent
        {
            get;
            set;
        }

        /// <summary>
        /// 01 更新，02 移除
        /// </summary>
        public string KeyType
        {
            get;
            set;
        }

        /// <summary>
        /// 关键字
        /// </summary>
        public string Key
        {
            get;
            set;
        }
    }

    public class DataTransferInfo
    {
        public byte[] Data
        {
            get;
            set;
        }

        public int Offset
        {
            get;
            set;
        }
        public int Length
        {
            get;
            set;
        }

        public long DeviceNo
        {
            get;
            set;
        }

    }

    public class DataTransferType
    {
        /// <summary>
        /// 向上层发送日志信息
        /// </summary>
        public const byte DataTransferLog = 0x01;

        /// <summary>
        /// 发送真实数据,向DTU发送
        /// </summary>
        public const byte DataTransferDataInfo = 0x02;

        /// <summary>
        /// 发送命令
        /// </summary>
        public const byte DataTransferCommand = 0x03;

        /// <summary>
        /// 发送devicelist
        /// </summary>
        public const byte DataTransferDeviceList = 0x04;

        /// <summary>
        /// 更新Device
        /// </summary>
        public const byte DataTransferUpdateDevice = 0x05;

        /// <summary>
        /// 更新District
        /// </summary>
        public const byte DataTransferUpdateDistrict = 0x06;

        /// <summary>
        /// 更新CardUser
        /// </summary>
        public const byte DataTransferUpdateCardUser = 0x07;
    }
}
