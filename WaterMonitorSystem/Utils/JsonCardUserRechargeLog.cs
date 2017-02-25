using System;
using System.Runtime.Serialization;

namespace Utils
{
    [Serializable, DataContract]
    public class JsonCardUserRechargeLog
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
        public long WateUserId { get; set; }

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
        /// 充值类型
        /// </summary>	
        [DataMember]
        public string RechargeType { get; set; }

        /// <summary>
        /// 充水金额
        /// </summary>	
        [DataMember]
        public decimal WaterPrice { get; set; }

        /// <summary>
        /// 充水数量
        /// </summary>	
        [DataMember]
        public decimal WaterNum { get; set; }

        /// <summary>
        /// 充电金额
        /// </summary>	
        [DataMember]
        public decimal ElectricPrice { get; set; }

        /// <summary>
        /// 充电数量
        /// </summary>	
        [DataMember]
        public decimal ElectricNum { get; set; }

        /// <summary>
        /// 总金额
        /// </summary>	
        [DataMember]
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// 备注
        /// </summary>	
        [DataMember]
        public string Remark { get; set; }

        /// <summary>
        /// 水量年限额
        /// </summary>	
        [DataMember]
        public decimal WaterQuota { get; set; }

        /// <summary>
        /// 电量年限额
        /// </summary>	
        [DataMember]
        public decimal ElectricQuota { get; set; }

        /// <summary>
        /// 水价Id
        /// </summary>	
        [DataMember]
        public long WaterPriceId { get; set; }

        /// <summary>
        /// 电价Id
        /// </summary>	
        [DataMember]
        public long ElectricPriceId { get; set; }

        /// <summary>
        /// 年累计用水量
        /// </summary>	
        [DataMember]
        public decimal WaterUsed { get; set; }

        /// <summary>
        /// 年累计用电量
        /// </summary>	
        [DataMember]
        public decimal ElectricUsed { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>	
        [DataMember]
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>	
        [DataMember]
        public string Password { get; set; }

        /// <summary>
        /// 行政区域Id
        /// </summary>	
        [DataMember]
        public long DistrictId { get; set; }

        /// <summary>
        /// 身份证号
        /// </summary>	
        [DataMember]
        public string IdentityNumber { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>	
        [DataMember]
        public string Telephone { get; set; }
    }
}
