using System;
using System.Runtime.Serialization;

namespace Maticsoft.Model
{
    /// <summary>
    /// 设置卡
    /// </summary>
    [Serializable]
    [DataContract]
    public class CardDevice
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
        /// 行政区划码
        /// </summary>	
        [DataMember]
        public string AddressCode1 { get; set; }

        /// <summary>
        /// 镇（乡）村编码
        /// </summary>	
        [DataMember]
        public string AddressCode2 { get; set; }

        /// <summary>
        /// 测站编码
        /// </summary>	
        [DataMember]
        public int AddressCode3 { get; set; }

        /// <summary>
        /// 年可开采水量
        /// </summary>	
        [DataMember]
        public decimal YearExploitation { get; set; }

        /// <summary>
        /// 可用水量提醒
        /// </summary>	
        [DataMember]
        public int AlertAvailableWater { get; set; }

        /// <summary>
        /// 可用电量提醒
        /// </summary>	
        [DataMember]
        public int AlertAvailableElectric { get; set; }

        /// <summary>
        /// 流量计类型
        /// </summary>	
        [DataMember]
        public int TypeCode { get; set; }

        /// <summary>
        /// 电表脉冲数
        /// </summary>	
        [DataMember]
        public int MeterPulse { get; set; }

        /// <summary>
        /// 水位报警值
        /// </summary>	
        [DataMember]
        public decimal AlertWaterLevel { get; set; }

        /// <summary>
        /// 站类型，0-单站，01-主站，02-从站
        /// </summary>	
        [DataMember]
        public int StationType { get; set; }

        /// <summary>
        /// 地址码，主站地址码为0，从站地址码1-32
        /// </summary>	
        [DataMember]
        public int StationCode { get; set; }

        /// <summary>
        /// 通信频率00-1F
        /// </summary>	
        [DataMember]
        public int Frequency { get; set; }

        /// <summary>
        /// 开卡操作人id
        /// </summary>	
        [DataMember]
        public long OpenUserId { get; set; }

        /// <summary>
        /// 开卡操作人名字
        /// </summary>	
        [DataMember]
        public string OpenUserName { get; set; }

        /// <summary>
        /// 开卡地址
        /// </summary>	
        [DataMember]
        public string OpenAddress { get; set; }

        /// <summary>
        /// 开卡时间
        /// </summary>	
        [DataMember]
        public DateTime OpenTime { get; set; }

        /// <summary>
        /// 最后一次修改操作人id
        /// </summary>	
        [DataMember]
        public long LastUpdateUserId { get; set; }

        /// <summary>
        /// 最后一次修改操作人名字
        /// </summary>	
        [DataMember]
        public string LastUpdateUserName { get; set; }

        /// <summary>
        /// 最后修改地址
        /// </summary>	
        [DataMember]
        public string LastUpdateAddress { get; set; }

        /// <summary>
        /// 最后一次修改时间
        /// </summary>	
        [DataMember]
        public DateTime LastUpdateTime { get; set; }

    }
}