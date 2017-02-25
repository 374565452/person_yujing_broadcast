using System;
using System.Runtime.Serialization;

namespace Maticsoft.Model
{
    /// <summary>
    /// 读取卡操作日志
    /// </summary>
    [Serializable]
    [DataContract]
    public class CardReadLog
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
        /// 行政区划码
        /// </summary>	
        [DataMember]
        public string AddressCode1 { get; set; }

        /// <summary>
        /// 镇（乡）村编码
        /// </summary>	
        [DataMember]
        public string AddressCode2 { get; set; }

        /// <summary>
        /// 测站编码
        /// </summary>	
        [DataMember]
        public int AddressCode3 { get; set; }

    }
}