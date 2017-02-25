using System;
using System.Runtime.Serialization;

namespace Maticsoft.Model
{
    /// <summary>
    /// 作物
    /// </summary>
    [Serializable]
    [DataContract]
    public class Crop
    {
        /// <summary>
        /// Id
        /// </summary>	
        [DataMember]
        public long Id { get; set; }

        /// <summary>
        /// 作物名称
        /// </summary>	
        [DataMember]
        public string CropName { get; set; }

        /// <summary>
        /// 每亩用水量
        /// </summary>	
        [DataMember]
        public decimal WaterPerMu { get; set; }

        /// <summary>
        /// 备注
        /// </summary>	
        [DataMember]
        public string Remark { get; set; }

    }
}