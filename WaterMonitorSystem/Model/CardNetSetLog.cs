using System;
using System.Runtime.Serialization;

namespace Maticsoft.Model
{
    /// <summary>
    /// 网络设置卡操作日志
    /// </summary>
    [Serializable]
    [DataContract]
    public class CardNetSetLog
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

    }
}