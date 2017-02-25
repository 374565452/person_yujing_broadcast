using System;
using System.Runtime.Serialization;

namespace Maticsoft.Model
{
    /// <summary>
    /// 设备事件
    /// </summary>
    [Serializable]
    [DataContract]
    public class DeviceEvent
    {
        /// <summary>
        /// Id
        /// </summary>	
        [DataMember]
        public long Id { get; set; }

        /// <summary>
        /// 设备序号
        /// </summary>	
        [DataMember]
        public string DeviceNo { get; set; }

        /// <summary>
        /// 时间类型
        /// </summary>	
        [DataMember]
        public string EventType { get; set; }

        /// <summary>
        /// 时间时间
        /// </summary>	
        [DataMember]
        public DateTime EventTime { get; set; }

        /// <summary>
        /// 设备时间
        /// </summary>	
        [DataMember]
        public DateTime DeviceTime { get; set; }

        /// <summary>
        /// 设备状态
        /// </summary>	
        [DataMember]
        public string DeviceState { get; set; }

        /// <summary>
        /// IC卡序列号
        /// </summary>	
        [DataMember]
        public string SerialNumber { get; set; }

        /// <summary>
        /// 用户卡号
        /// </summary>	
        [DataMember]
        public string UserNo { get; set; }

        /// <summary>
        /// 开泵时间
        /// </summary>	
        [DataMember]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 开泵剩余水量
        /// </summary>	
        [DataMember]
        public decimal StartResidualWater { get; set; }

        /// <summary>
        /// 开泵剩余电量
        /// </summary>	
        [DataMember]
        public decimal StartResidualElectric { get; set; }

        /// <summary>
        /// 关泵时间
        /// </summary>	
        [DataMember]
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 关泵剩余水量
        /// </summary>	
        [DataMember]
        public decimal EndResidualWater { get; set; }

        /// <summary>
        /// 关泵剩余电量
        /// </summary>	
        [DataMember]
        public decimal EndResidualElectric { get; set; }

        /// <summary>
        /// 本次使用水量
        /// </summary>	
        [DataMember]
        public decimal WaterUsed { get; set; }

        /// <summary>
        /// 本次使用电量
        /// </summary>	
        [DataMember]
        public decimal ElectricUsed { get; set; }

        /// <summary>
        /// 年累计用水量
        /// </summary>	
        [DataMember]
        public decimal YearWaterUsed { get; set; }

        /// <summary>
        /// 年累计用电量
        /// </summary>	
        [DataMember]
        public decimal YearElectricUsed { get; set; }

        /// <summary>
        /// 年剩余可开采量
        /// </summary>	
        [DataMember]
        public decimal YearSurplus { get; set; }

        /// <summary>
        /// 年可开采量
        /// </summary>	
        [DataMember]
        public decimal YearExploitation { get; set; }

        /// <summary>
        /// 记录类型，0 正常扣费，1 未扣费，2 免费
        /// </summary>	
        [DataMember]
        public byte RecordType { get; set; }

        /// <summary>
        /// 开泵类型，1 刷卡开泵，2 手动开泵，3 远程GPRS开泵，4 远程短信开泵
        /// </summary>	
        [DataMember]
        public byte REV1 { get; set; }

        /// <summary>
        /// 关泵类型，1 刷卡关泵，2 手动关泵，3 远程GPRS关泵，4 远程短信关泵，5 欠费关泵
        /// </summary>
        public byte REV2 { get; set; }

        /// <summary>
        /// 备注
        /// </summary>	
        [DataMember]
        public string Remark { get; set; }

        /// <summary>
        /// 操作用户Id
        /// </summary>	
        [DataMember]
        public long UserId { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>	
        [DataMember]
        public string UserName { get; set; }

        /// <summary>
        /// 原始数据
        /// </summary>	
        [DataMember]
        public string RawData { get; set; }

        /// <summary>
        /// 发送状态
        /// </summary>	
        [DataMember]
        public string SendSate { get; set; }

    }
}