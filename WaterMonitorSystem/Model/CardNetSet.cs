using System;
using System.Runtime.Serialization;

namespace Maticsoft.Model
{
    /// <summary>
    /// 网络设置卡
    /// </summary>
    [Serializable]
    [DataContract]
    public class CardNetSet
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
        /// IP地址或域名
        /// </summary>	
        [DataMember]
        public string IP { get; set; }

        /// <summary>
        /// 端口
        /// </summary>	
        [DataMember]
        public int Port { get; set; }

        /// <summary>
        /// IsDomain
        /// </summary>	
        [DataMember]
        public string IsDomain { get; set; }

        /// <summary>
        /// APN名称
        /// </summary>	
        [DataMember]
        public string APNName { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>	
        [DataMember]
        public string APNUserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>	
        [DataMember]
        public string APNPassword { get; set; }

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

    }
}