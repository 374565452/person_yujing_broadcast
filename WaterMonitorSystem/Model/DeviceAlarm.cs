using System;
using System.Runtime.Serialization;

namespace Maticsoft.Model
{
    /// <summary>
    /// 设备报警记录
    /// </summary>
    [Serializable]
    [DataContract]
    public class DeviceAlarm
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
        /// 报警类型
        /// </summary>	
        [DataMember]
        public string AlarmType { get; set; }

        /// <summary>
        /// 报警值
        /// </summary>	
        [DataMember]
        public string AlarmValue { get; set; }

        /// <summary>
        /// 报警开始时间
        /// </summary>	
        [DataMember]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 报警结束时间
        /// </summary>	
        [DataMember]
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 持续时长（秒）
        /// </summary>	
        [DataMember]
        public long Duration { get; set; }

        /// <summary>
        /// 状态
        /// </summary>	
        [DataMember]
        public string State { get; set; }

        /// <summary>
        /// 系统保存时间
        /// </summary>	
        [DataMember]
        public DateTime SaveTime { get; set; }
    }
}