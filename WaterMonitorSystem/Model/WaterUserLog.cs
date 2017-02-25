using System;
using System.Runtime.Serialization;

namespace Maticsoft.Model
{
    /// <summary>
    /// 用水户操作日志
    /// </summary>
    [Serializable]
    [DataContract]
    public class WaterUserLog
    {
        /// <summary>
        /// Id
        /// </summary>	
        [DataMember]
        public long Id { get; set; }

        /// <summary>
        /// 用水户Id
        /// </summary>	
        [DataMember]
        public long WaterUserId { get; set; }

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
        /// DistrictId
        /// </summary>	
        [DataMember]
        public long DistrictId { get; set; }

        /// <summary>
        /// 真是姓名
        /// </summary>	
        [DataMember]
        public string TrueName { get; set; }

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

        /// <summary>
        /// 地址
        /// </summary>	
        [DataMember]
        public string Address { get; set; }

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
        /// 备注
        /// </summary>	
        [DataMember]
        public string Remark { get; set; }

        /// <summary>
        /// 水价Id
        /// </summary>	
        [DataMember]
        public int 水价ID { get; set; }

        /// <summary>
        /// 电价ID
        /// </summary>	
        [DataMember]
        public int 电价ID { get; set; }

        /// <summary>
        /// 状态
        /// </summary>	
        [DataMember]
        public string State { get; set; }

    }
}