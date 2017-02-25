using System;
using System.Net;
using System.Text;

namespace Server.Util.BufferManager
{
    public class DynamicBufferManager
    {
        public byte[] Buffer { get; set; }//写入数据的缓冲区

        public int DataCount { get; set; }//写入缓冲区的数据的大小

        public DynamicBufferManager(int bufferSize)
        {
            DataCount = 0;
            Buffer = new byte[bufferSize];
        }

        #region 得到已写数据大小
        public int getCount()
        {
            return DataCount;
        }
        #endregion

        #region 剩余多少缓冲区可用
        public int getReserveBufferSize()
        {
            return (Buffer.Length - DataCount);
        }
        #endregion

        #region 清除指定大小的缓冲区
        public void clearBuffer(int len)
        {
            if (len >= DataCount)
            {
                DataCount = 0;
            }
            else
            {
                //将len之后的数据往前移
                for (int i = 0; i < DataCount - len; i++)
                {
                    Buffer[i] = Buffer[len + i];
                }
                DataCount = DataCount - len;
            }
        }
        #endregion

        #region 清除所有
        public void clearAll()
        {
            clearBuffer(DataCount);
        }
        #endregion

        #region 写数据
        public void writeBuffer(byte[] buffer, int offset, int count)
        {
            //如果剩余的字节数够用，则 不需要申请空间
            if (getReserveBufferSize() >= count)
            {
                Array.Copy(buffer, offset, Buffer, DataCount, count);
                DataCount = DataCount + count;
            }
            //剩余的字节数不够，需要申请空间
            else
            {
                //写这次数据，所需要的总空间大小
                int totalSize = Buffer.Length + count - getReserveBufferSize();
                //申请空间
                byte[] newBuffer = new byte[totalSize];
                //复制老数据到新空间中
                Array.Copy(Buffer, 0, newBuffer, 0, DataCount);
                //复制新数据到新空间中
                Array.Copy(buffer, offset, newBuffer, DataCount, count);
                DataCount = DataCount + count;
                Buffer = newBuffer;
            }
        }
        #endregion

        #region 写缓存数据
        public void writeBuffer(byte[] buffer)
        {
            writeBuffer(buffer, 0, buffer.Length);
        }
        #endregion

        #region 写short数据
        /// <summary>
        /// 写short数据
        /// convert是否进行字节的转换，因为.net中是小头结构（低位在前，高位在后），而在网络中是以大头结构（高位在前，低位在后）表示的。
        /// 这个需要客户端与服务端进行商议好
        /// </summary>
        /// <param name="value"></param>
        /// <param name="convert"></param>
        public void writeShort(short value, bool convert)
        {
            if (convert)
            {
                value = System.Net.IPAddress.HostToNetworkOrder(value);
            }
            byte[] bytes = BitConverter.GetBytes(value);
            writeBuffer(bytes);
        }
        #endregion

        #region 写int 数据
        public void writeInt(int value, bool convert)
        {
            if (convert)
            {
                value = IPAddress.HostToNetworkOrder(value);
            }
            byte[] bytes = BitConverter.GetBytes(value);
            writeBuffer(bytes);
        }
        #endregion

        #region 写long 数据
        public void writeLong(long value, bool convert)
        {
            if (convert)
            {
                value = IPAddress.HostToNetworkOrder(value);
            }
            byte[] bytes = BitConverter.GetBytes(value);
            writeBuffer(bytes);
        }
        #endregion

        #region 写string 字符串，文本以UTF-8格式
        public void writeString(string value)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            writeBuffer(bytes);
        }
        #endregion
    }
}
