using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Maticsoft.Model
{
    /// <summary>
    /// 系统用户
    /// </summary>
    [Serializable]
    [DataContract]
    public class SysUser
    {
        /// <summary>
        /// ID
        /// </summary>
        [DataMember]
        [Description("key")]
        public long ID { get; set; }

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
        /// 注册时间
        /// </summary>
        [DataMember]
        public DateTime RegDate { get; set; }

        /// <summary>
        /// 登录日期
        /// </summary>
        [DataMember]
        public DateTime LogonDate { get; set; }

        /// <summary>
        /// 登录IP
        /// </summary>
        [DataMember]
        public string LogonIP { get; set; }

        /// <summary>
        /// 登录Mac
        /// </summary>
        [DataMember]
        public string LogonMac { get; set; }

        /// <summary>
        /// 是否有效
        /// </summary>
        [DataMember]
        public int IsAllow { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        [DataMember]
        public long RoleId { get; set; }

        /// <summary>
        /// 所属行政区划
        /// </summary>
        [DataMember]
        public long DistrictId { get; set; }

        /// <summary>
        /// 真实姓名
        /// </summary>
        [DataMember]
        public string TrueName { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        [DataMember]
        public string Sex { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        [DataMember]
        public string Mobile { get; set; }

        /// <summary>
        /// 住址
        /// </summary>
        [DataMember]
        public string Address { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [DataMember]
        public string Remark { get; set; }

    }
}