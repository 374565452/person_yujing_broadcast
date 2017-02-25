using System;
using System.Runtime.Serialization;

namespace Maticsoft.Model
{
    /// <summary>
    /// 阶梯水价
    /// </summary>
    [Serializable]
    [DataContract]
    public class WaterPrice
    {
        /// <summary>
        /// Id
        /// </summary>	
        [DataMember]
        public long Id { get; set; }

        /// <summary>
        /// 水价名称
        /// </summary>	
        [DataMember]
        public string PriceName { get; set; }

        /// <summary>
        /// 价格数量
        /// </summary>	
        [DataMember]
        public int PriceCount { get; set; }

        /// <summary>
        /// 价格类型
        /// </summary>	
        [DataMember]
        public string PriceType { get; set; }

        /// <summary>
        /// 数量1
        /// </summary>	
        [DataMember]
        public decimal Num1 { get; set; }

        /// <summary>
        /// 价格1
        /// </summary>	
        [DataMember]
        public decimal Price1 { get; set; }

        /// <summary>
        /// 数量2
        /// </summary>	
        [DataMember]
        public decimal Num2 { get; set; }

        /// <summary>
        /// 价格2
        /// </summary>	
        [DataMember]
        public decimal Price2 { get; set; }

        /// <summary>
        /// 数量3
        /// </summary>	
        [DataMember]
        public decimal Num3 { get; set; }

        /// <summary>
        /// 价格3
        /// </summary>	
        [DataMember]
        public decimal Price3 { get; set; }

        /// <summary>
        /// 数量4
        /// </summary>	
        [DataMember]
        public decimal Num4 { get; set; }

        /// <summary>
        /// 价格4
        /// </summary>	
        [DataMember]
        public decimal Price4 { get; set; }

        /// <summary>
        /// 数量5
        /// </summary>	
        [DataMember]
        public decimal Num5 { get; set; }

        /// <summary>
        /// 价格5
        /// </summary>	
        [DataMember]
        public decimal Price5 { get; set; }

    }
}