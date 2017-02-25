using System;
using System.Runtime.Serialization;

namespace Maticsoft.Model
{
    /// <summary>
    /// 设备操作日志
    /// </summary>
    [Serializable]
    [DataContract]
    public class DeviceLog
    {
        /// <summary>
        /// Id
        /// </summary>	
        [DataMember]
        public long Id { get; set; }

        /// <summary>
        /// 设备Id
        /// </summary>	
        [DataMember]
        public long DeviceId { get; set; }

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
        /// 手机号码
        /// </summary>	
        [DataMember]
        public string SimNo { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>	
        [DataMember]
        public string DeviceName { get; set; }

        /// <summary>
        /// 描述
        /// </summary>	
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// 安装时间
        /// </summary>	
        [DataMember]
        public DateTime SetupDate { get; set; }

        /// <summary>
        /// 安装地点
        /// </summary>	
        [DataMember]
        public string SetupAddress { get; set; }

        /// <summary>
        /// 经度_Baidu
        /// </summary>	
        [DataMember]
        public long LON { get; set; }

        /// <summary>
        /// 纬度_Baidu
        /// </summary>	
        [DataMember]
        public long LAT { get; set; }

        /// <summary>
        /// 当前设备是否有效,当移除设备后，当前设备无效，不参与计算
        /// </summary>	
        [DataMember]
        public int IsValid { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>	
        [DataMember]
        public DateTime LastUpdate { get; set; }

        /// <summary>
        /// 所属行政区域
        /// </summary>	
        [DataMember]
        public long DistrictId { get; set; }

        /// <summary>
        /// 设备序号
        /// </summary>	
        [DataMember]
        public string DeviceNo { get; set; }

        /// <summary>
        /// 0:离线，1:在线
        /// </summary>	
        [DataMember]
        public int Online { get; set; }

        /// <summary>
        /// 最后在线时间
        /// </summary>	
        [DataMember]
        public DateTime OnlineTime { get; set; }

        /// <summary>
        /// 年可开采水量
        /// </summary>	
        [DataMember]
        public decimal YearExploitation { get; set; }

        /// <summary>
        /// 可用水量提醒
        /// </summary>	
        [DataMember]
        public int AlertAvailableWater { get; set; }

        /// <summary>
        /// 可用电量提醒
        /// </summary>	
        [DataMember]
        public int AlertAvailableElectric { get; set; }

        /// <summary>
        /// 流量计类型Id
        /// </summary>	
        [DataMember]
        public int DeviceTypeCodeId { get; set; }

        /// <summary>
        /// 电表脉冲数
        /// </summary>	
        [DataMember]
        public int MeterPulse { get; set; }

        /// <summary>
        /// 水位报警值
        /// </summary>	
        [DataMember]
        public decimal AlertWaterLevel { get; set; }

        /// <summary>
        /// 设备状态
        /// </summary>	
        [DataMember]
        public string TerminalState { get; set; }

        /// <summary>
        /// 备注
        /// </summary>	
        [DataMember]
        public string Remark { get; set; }

        /// <summary>
        /// 作物Id
        /// </summary>	
        [DataMember]
        public long CropId { get; set; }

        /// <summary>
        /// 地块面积
        /// </summary>	
        [DataMember]
        public decimal Area { get; set; }

        /// <summary>
        /// 站类型，0-单站，01-主站，02-从站
        /// </summary>	
        [DataMember]
        public int StationType { get; set; }

        /// <summary>
        /// 地址码，主站地址码为0，从站地址码1-32
        /// </summary>	
        [DataMember]
        public int StationCode { get; set; }

        /// <summary>
        /// 通信频率00-1F
        /// </summary>	
        [DataMember]
        public int Frequency { get; set; }

        /// <summary>
        /// 主站主键Id
        /// </summary>	
        [DataMember]
        public long MainId { get; set; }

        /// <summary>
        /// 设备类型，水泵、水位仪、气象仪、墒情仪
        /// </summary>	
        [DataMember]
        public string DeviceType { get; set; }
    }
}