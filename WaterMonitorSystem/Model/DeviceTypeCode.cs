using System;
using System.Runtime.Serialization;

namespace Maticsoft.Model
{
    /// <summary>
    /// 流量计类型
    /// </summary>
    [Serializable]
    [DataContract]
    public class DeviceTypeCode
    {
        /// <summary>
        /// Id
        /// </summary>	
        [DataMember]
        public long Id { get; set; }

        /// <summary>
        /// 键
        /// </summary>	
        [DataMember]
        public int k { get; set; }

        /// <summary>
        /// 值
        /// </summary>	
        [DataMember]
        public string v { get; set; }

    }
}