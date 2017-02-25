using System;
using System.Runtime.Serialization;

namespace Maticsoft.Model
{
    /// <summary>
    /// SysUserConfig
    /// </summary>
    [Serializable]
    [DataContract]
    public class SysUserConfig
    {
        /// <summary>
        /// SysUserId
        /// </summary>	
        [DataMember]
        public long SysUserId { get; set; }

        /// <summary>
        /// ValType
        /// </summary>	
        [DataMember]
        public string ValType { get; set; }

        /// <summary>
        /// ValName
        /// </summary>	
        [DataMember]
        public string ValName { get; set; }

        /// <summary>
        /// IsShow
        /// </summary>	
        [DataMember]
        public bool IsShow { get; set; }

        /// <summary>
        /// OrderNum
        /// </summary>	
        [DataMember]
        public int OrderNum { get; set; }

    }
}