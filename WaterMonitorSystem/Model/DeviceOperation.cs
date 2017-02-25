using System;
using System.Runtime.Serialization;

namespace Maticsoft.Model
{
    /// <summary>
    /// DeviceOperation
    /// </summary>
    [Serializable]
    [DataContract]
    public class DeviceOperation
    {
        /// <summary>
        /// Id
        /// </summary>	
        [DataMember]
        public long Id { get; set; }

        /// <summary>
        /// 设备序号
        /// </summary>	
        [DataMember]
        public string DeviceNo { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>	
        [DataMember]
        public string DeviceName { get; set; }

        /// <summary>
        /// 时间类型
        /// </summary>	
        [DataMember]
        public string OperationType { get; set; }

        /// <summary>
        /// 时间时间
        /// </summary>	
        [DataMember]
        public DateTime OperationTime { get; set; }

        /// <summary>
        /// 操作用户Id
        /// </summary>	
        [DataMember]
        public long UserId { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>	
        [DataMember]
        public string UserName { get; set; }

        /// <summary>
        /// 原始数据
        /// </summary>	
        [DataMember]
        public string RawData { get; set; }

        /// <summary>
        /// 备注
        /// </summary>	
        [DataMember]
        public string Remark { get; set; }

        /// <summary>
        /// 状态
        /// </summary>	
        [DataMember]
        public string State { get; set; }

    }
}