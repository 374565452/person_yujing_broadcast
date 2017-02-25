using System;
using System.Runtime.Serialization;

namespace Maticsoft.Model
{
    /// <summary>
    /// 用户卡
    /// </summary>
    [Serializable]
    [DataContract]
    public class CardUser
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
        /// 最后一次消费终端id
        /// </summary>	
        [DataMember]
        public long LastConsumptionDeviceId { get; set; }

        /// <summary>
        /// 最后一次消费终端编号
        /// </summary>	
        [DataMember]
        public string LastConsumptionDeviceNo { get; set; }

        /// <summary>
        /// 最后一次消费时间
        /// </summary>	
        [DataMember]
        public DateTime LastConsumptionTime { get; set; }

        /// <summary>
        /// 最后一次充值操作人id
        /// </summary>	
        [DataMember]
        public long LastChargeUserId { get; set; }

        /// <summary>
        /// 最后一次充值操作人名字
        /// </summary>	
        [DataMember]
        public string LastChargeUserName { get; set; }

        /// <summary>
        /// 最后充值地址
        /// </summary>	
        [DataMember]
        public string LastChargeAddress { get; set; }

        /// <summary>
        /// 最后一次充值时间
        /// </summary>	
        [DataMember]
        public DateTime LastChargeTime { get; set; }

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

        /// <summary>
        /// 是否注销
        /// </summary>	
        [DataMember]
        public int IsCountermand { get; set; }

        /// <summary>
        /// 注销说明
        /// </summary>	
        [DataMember]
        public string CountermandContent { get; set; }

        /// <summary>
        /// 注销操作人id
        /// </summary>	
        [DataMember]
        public long CountermandUserId { get; set; }

        /// <summary>
        /// 注销操作人名字
        /// </summary>	
        [DataMember]
        public string CountermandUserName { get; set; }

        /// <summary>
        /// 注销地址
        /// </summary>	
        [DataMember]
        public string CountermandAddress { get; set; }

        /// <summary>
        /// 注销时间
        /// </summary>	
        [DataMember]
        public DateTime CountermandTime { get; set; }

        /// <summary>
        /// 取消注销说明
        /// </summary>	
        [DataMember]
        public string CountermandCancelContent { get; set; }

        /// <summary>
        /// 取消注销操作人id
        /// </summary>	
        [DataMember]
        public long CountermandCancelUserId { get; set; }

        /// <summary>
        /// 取消注销操作人名字
        /// </summary>	
        [DataMember]
        public string CountermandCancelUserName { get; set; }

        /// <summary>
        /// 取消注销地址
        /// </summary>	
        [DataMember]
        public string CountermandCancelAddress { get; set; }

        /// <summary>
        /// 取消注销时间
        /// </summary>	
        [DataMember]
        public DateTime CountermandCancelTime { get; set; }

    }
}