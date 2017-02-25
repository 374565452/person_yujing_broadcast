using System;
using System.Runtime.Serialization;

namespace Maticsoft.Model
{
    /// <summary>
    /// 角色
    /// </summary>
    [Serializable]
    [DataContract]
    public class Role
    {
        /// <summary>
        /// Id
        /// </summary>	
        [DataMember]
        public long Id { get; set; }

        /// <summary>
        /// 角色名字
        /// </summary>	
        [DataMember]
        public string RoleName { get; set; }

        /// <summary>
        /// 是否有效
        /// </summary>	
        [DataMember]
        public int IsAllow { get; set; }

        /// <summary>
        /// 权重
        /// </summary>	
        [DataMember]
        public int Weight { get; set; }

    }
}