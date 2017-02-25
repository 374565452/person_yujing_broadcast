using System;
using System.Runtime.Serialization;

namespace Maticsoft.Model
{
    /// <summary>
    /// 读取卡
    /// </summary>
    [Serializable]
    [DataContract]
    public class CardRead
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