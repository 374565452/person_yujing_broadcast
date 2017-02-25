using System;
using System.Runtime.Serialization;

namespace Maticsoft.Model
{
    /// <summary>
    /// BaseInfo
    /// </summary>
    [Serializable]
    [DataContract]
    public class BaseInfo
    {
        /// <summary>
        /// BaseType
        /// </summary>	
        [DataMember]
        public string BaseType { get; set; }

        /// <summary>
        /// BaseKey
        /// </summary>	
        [DataMember]
        public string BaseKey { get; set; }

        /// <summary>
        /// BaseDesc
        /// </summary>	
        [DataMember]
        public string BaseDesc { get; set; }

        /// <summary>
        /// BaseValue0
        /// </summary>	
        [DataMember]
        public string BaseValue0 { get; set; }

        /// <summary>
        /// BaseValue1
        /// </summary>	
        [DataMember]
        public string BaseValue1 { get; set; }

    }
}