using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketLib
{
    public class ProtocolConst
    {
        //解析命令缓存
        public static int InitBufferSize = 1024 * 64;
        //IOCP接收数据缓存大小，设置过小会造成事件响应增多，设置过大会造成内存占用偏多
        public static int ReceiveBufferSize = 1024 * 64;
    }
}
