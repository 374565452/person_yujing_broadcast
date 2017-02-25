using System;
using System.Runtime.Serialization;

namespace Maticsoft.Model
{
    /// <summary>
    /// T_GroundWater
    /// </summary>
    [Serializable]
    [DataContract]
    public class T_GroundWater
    {
        /// <summary>
        /// NO
        /// </summary>	
        [DataMember]
        public int NO { get; set; }

        /// <summary>
        /// StationID
        /// </summary>	
        [DataMember]
        public string StationID { get; set; }

        /// <summary>
        /// GroundWaterLevel
        /// </summary>	
        [DataMember]
        public decimal GroundWaterLevel { get; set; }

        /// <summary>
        /// LineLength
        /// </summary>	
        [DataMember]
        public decimal LineLength { get; set; }

        /// <summary>
        /// GroundWaterTempture
        /// </summary>	
        [DataMember]
        public decimal GroundWaterTempture { get; set; }

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