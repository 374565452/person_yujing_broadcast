using System;
using System.Runtime.Serialization;

namespace Maticsoft.Model
{
    /// <summary>
    /// 用户卡操作日志
    /// </summary>
    [Serializable]
    [DataContract]
    public class CardUserLog
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
        /// 操作人id
        /// </summary>	
        [DataMember]
        public long LogUserId { get; set; }

        /// <summary>
        /// 操作人名字
        /// </summary>	
        [DataMember]
        public string LogUserName { get; set; }

        /// <summary>
        /// 操作地址
        /// </summary>	
        [DataMember]
        public string LogAddress { get; set; }

        /// <summary>
        /// 操作时间
        /// </summary>	
        [DataMember]
        public DateTime LogTime { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>	
        [DataMember]
        public string LogType { get; set; }

        /// <summary>
        /// 操作内容
        /// </summary>	
        [DataMember]
        public string LogContent { get; set; }

        /// <summary>
        /// 用户卡号
        /// </summary>	
        [DataMember]
        public string UserNo { get; set; }

        /// <summary>
        /// 用水户Id
        /// </summary>	
        [DataMember]
        public long WaterUserId { get; set; }

        /// <summary>
        /// 剩余水量
        /// </summary>	
        [DataMember]
        public decimal ResidualWater { get; set; }

        /// <summary>
        /// 剩余电量
        /// </summary>	
        [DataMember]
        public decimal ResidualElectric { get; set; }

        /// <summary>
        /// 累计水量
        /// </summary>	
        [DataMember]
        public decimal TotalWater { get; set; }

        /// <summary>
        /// 累计电量
        /// </summary>	
        [DataMember]
        public decimal TotalElectric { get; set; }

        /// <summary>
        /// 剩余金额
        /// </summary>	
        [DataMember]
        public decimal ResidualMoney { get; set; }

        /// <summary>
        /// 累计金额
        /// </summary>	
        [DataMember]
        public decimal TotallMoney { get; set; }

        /// <summary>
        /// 可刷终端列表
        /// </summary>	
        [DataMember]
        public string DeviceList { get; set; }

        /// <summary>
        /// 备注
        /// </summary>	
        [DataMember]
        public string Remark { get; set; }

    }
}