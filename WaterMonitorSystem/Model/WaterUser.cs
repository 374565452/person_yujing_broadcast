using System;
using System.Runtime.Serialization;

namespace Maticsoft.Model
{
    /// <summary>
    /// 用水户
    /// </summary>
    [Serializable]
    [DataContract]
    public class WaterUser
    {
        /// <summary>
        /// id
        /// </summary>	
        [DataMember]
        public long id { get; set; }

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
        /// 水价ID
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