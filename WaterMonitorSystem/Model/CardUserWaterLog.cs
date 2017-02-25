using System;
using System.Runtime.Serialization;

namespace Maticsoft.Model
{
    /// <summary>
    /// 用户卡用水记录
    /// </summary>
    [Serializable]
    [DataContract]
    public class CardUserWaterLog
    {
        /// <summary>
        /// Id
        /// </summary>	
        [DataMember]
        public long Id { get; set; }

        /// <summary>
        /// IC卡序列号
        /// </summary>	
        [DataMember]
        public string SerialNumber { get; set; }

        /// <summary>
        /// 用水户Id
        /// </summary>	
        [DataMember]
        public long WateUserId { get; set; }

        /// <summary>
        /// 用户卡号
        /// </summary>	
        [DataMember]
        public string UserNo { get; set; }

        /// <summary>
        /// 设备Id
        /// </summary>	
        [DataMember]
        public long DeviceId { get; set; }

        /// <summary>
        /// 设备序号
        /// </summary>	
        [DataMember]
        public string DeviceNo { get; set; }

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
        /// 灌溉时长（秒）
        /// </summary>	
        [DataMember]
        public decimal Duration { get; set; }

        /// <summary>
        /// 本次用水量
        /// </summary>	
        [DataMember]
        public decimal WaterUsed { get; set; }

        /// <summary>
        /// 本次用电量
        /// </summary>	
        [DataMember]
        public decimal ElectricUsed { get; set; }

    }
}