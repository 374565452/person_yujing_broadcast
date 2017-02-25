using System;
using System.Runtime.Serialization;

namespace Maticsoft.Model
{
    /// <summary>
    /// T_RainFall
    /// </summary>
    [Serializable]
    [DataContract]
    public class T_RainFall
    {
        /// <summary>
        /// NO
        /// </summary>	
        [DataMember]
        public long NO { get; set; }

        /// <summary>
        /// StationID
        /// </summary>	
        [DataMember]
        public string StationID { get; set; }

        /// <summary>
        /// Rainfall
        /// </summary>	
        [DataMember]
        public decimal Rainfall { get; set; }

        /// <summary>
        /// Rainfall_Hour
        /// </summary>	
        [DataMember]
        public decimal Rainfall_Hour { get; set; }

        /// <summary>
        /// Rainfall_Day
        /// </summary>	
        [DataMember]
        public decimal Rainfall_Day { get; set; }

        /// <summary>
        /// Rainfall_Total
        /// </summary>	
        [DataMember]
        public decimal Rainfall_Total { get; set; }

        /// <summary>
        /// WaterLevel
        /// </summary>	
        [DataMember]
        public decimal WaterLevel { get; set; }

        /// <summary>
        /// BV
        /// </summary>	
        [DataMember]
        public decimal BV { get; set; }

        /// <summary>
        /// Acq_Time
        /// </summary>	
        [DataMember]
        public DateTime Acq_Time { get; set; }

        /// <summary>
        /// CREATE_TIME
        /// </summary>	
        [DataMember]
        public DateTime CREATE_TIME { get; set; }

    }
}