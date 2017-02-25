using System;
using System.Runtime.Serialization;

namespace Utils
{
    [Serializable, DataContract]
    public class JsonSysUser
    {
        [DataMember]
        public long DistrictId { get; set; }

        [DataMember]
        public string DistrictName { get; set; }

        [DataMember]
        public long GroupId { get; set; }

        [DataMember]
        public string GroupName { get; set; }

        [DataMember]
        public string TrueName { get; set; }

        [DataMember]
        public long UserId { get; set; }
    }
}
