﻿using System;
using System.Runtime.Serialization;

namespace Maticsoft.Model
{
    /// <summary>
    /// 设备
    /// </summary>
    [Serializable]
    [DataContract]
    public class Device
    {
        /// <summary>
        /// Id
        /// </summary>	
        [DataMember]
        public long Id { get; set; }

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

        /// <summary>
        /// 累计水量
        /// </summary>	
        [DataMember]
        public decimal WaterUsed { get; set; }

        /// <summary>
        /// 水位仪遥测站编码
        /// </summary>	
        [DataMember]
        public string RemoteStation { get; set; }

        /// <summary>
        /// 当前降雨量，毫米
        /// </summary>	
        [DataMember]
        public decimal Rainfall { get; set; }

        /// <summary>
        /// 小时降雨量，毫米
        /// </summary>	
        [DataMember]
        public decimal Rainfall_Hour { get; set; }

        /// <summary>
        /// 日降雨量，毫米
        /// </summary>	
        [DataMember]
        public decimal Rainfall_Day { get; set; }

        /// <summary>
        /// 总降雨量，毫米
        /// </summary>	
        [DataMember]
        public decimal Rainfall_Total { get; set; }

        /// <summary>
        /// 水位，米
        /// </summary>	
        [DataMember]
        public decimal WaterLevel { get; set; }

        /// <summary>
        /// 记录时间
        /// </summary>	
        [DataMember]
        public DateTime Acq_Time { get; set; }

        /***********start add by kqz 2017-3-5***************/
        /// <summary>
        /// 当前降水量
        /// </summary>
        public decimal CurRainfall { get; set; }
        /// <summary>
        /// 降水量累计值
        /// </summary>
        public decimal TotalRainfall { get; set; }
        /// <summary>
        /// 瞬时河道水位
        /// </summary>
        public decimal RiverLevel { get; set; }
        /// <summary>
        /// 供电方式
        /// </summary>
        public int PowerSupplyWay { get; set; }
        /// <summary>
        /// 预警状态
        /// </summary>
        public int YuJingState { get; set; }
        /// <summary>
        /// 电池电压
        /// </summary>
        public decimal PowerVal { get; set; }
        /// <summary>
        /// 与北斗服务器连接状态
        /// </summary>
        public int BeidouState { get; set; }
        /***********end add by kqz 2017-3-5***************/
        /***********start add by kqz 2017-3-8 ****************/
        public Device()
        {
            this.CurRainfall = -1.0M;
            this.TotalRainfall = -1.0M;
            this.RiverLevel = -1.00M;
            this.PowerSupplyWay = -1;
            this.YuJingState = -1;
            this.PowerVal = -1.0M;
            this.BeidouState = -1;
        }
        /***********end add by kqz 2017-3-8 *****************/
    }
}