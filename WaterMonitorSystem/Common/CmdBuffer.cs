using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// 用于接收数据的缓存
    /// </summary>
    public class CmdBuffer
    {
        /// <summary>
        /// 最长缓存长度
        /// </summary>
        public const int BUFFER_MAX_LENGTH = 1024 * 30;
        /// <summary>
        /// 已有数据长度
        /// </summary>
        public int Length;
        /// <summary>
        /// 存放的数据
        /// </summary>

        public byte[] Buffer;


        public CmdBuffer()
        {
            Length = 0;
            Buffer = new byte[BUFFER_MAX_LENGTH];

        }
    }
}
