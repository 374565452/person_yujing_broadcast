using System;
using System.Runtime.Serialization;

namespace Maticsoft.Model
{
    /// <summary>
    /// 行政区域
    /// </summary>
    [Serializable]
    [DataContract]
    public class District
    {
        /// <summary>
        /// Id
        /// </summary>	
        [DataMember]
        public long Id { get; set; }

        /// <summary>
        /// 行政区划名
        /// </summary>	
        [DataMember]
        public string DistrictName { get; set; }

        /// <summary>
        /// 经度
        /// </summary>	
        [DataMember]
        public long LON { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>	
        [DataMember]
        public long LAT { get; set; }

        /// <summary>
        /// 行政编码（）
        /// </summary>	
        [DataMember]
        public string DistrictCode { get; set; }

        /// <summary>
        /// 行政区划类型:省,市,县,镇,村
        /// </summary>	
        [DataMember]
        public int DistrictType { get; set; }

        /// <summary>
        /// 父级ID
        /// </summary>	
        [DataMember]
        public long ParentId { get; set; }

    }
}