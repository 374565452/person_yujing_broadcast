using System;
using System.Runtime.Serialization;

namespace Maticsoft.Model
{
    /// <summary>
    /// 菜单
    /// </summary>
    [Serializable]
    [DataContract]
    public class Menu
    {
        /// <summary>
        /// Id
        /// </summary>	
        [DataMember]
        public long Id { get; set; }

        /// <summary>
        /// 菜单名字
        /// </summary>	
        [DataMember]
        public string MenuName { get; set; }

        /// <summary>
        /// 是否有效
        /// </summary>	
        [DataMember]
        public int IsAllow { get; set; }

        /// <summary>
        /// 菜单链接
        /// </summary>	
        [DataMember]
        public string MenuUrl { get; set; }

        /// <summary>
        /// 父菜单ID
        /// </summary>	
        [DataMember]
        public long ParentId { get; set; }

        /// <summary>
        /// 顺序号
        /// </summary>	
        [DataMember]
        public int OrderNumber { get; set; }

        /// <summary>
        /// 样式文件
        /// </summary>	
        [DataMember]
        public string CssFile { get; set; }

    }
}