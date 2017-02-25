using System.Collections.Generic;

namespace Server.Util.BufferManager
{
    struct SendBufferPacket
    {
        //偏移量
        public int offset;
        //总数
        public int count;
    }
    public class AsyncSendBufferManager
    {
        private DynamicBufferManager dynamicBufferManager;
        public DynamicBufferManager DynamicBufferManager
        {
            get
            {
                return dynamicBufferManager;
            }
        }
        //由于是异步发送，有可能接收到两次命令，写入了两次发送，需要等待下一次回调后再发一次的响应
        //所有需要一个集合来存放发送的数据
        private IList<SendBufferPacket> sendBufferPackets;
        private SendBufferPacket sendBufferPacket;

        public AsyncSendBufferManager(int bufferSize)
        {
            dynamicBufferManager = new DynamicBufferManager(bufferSize);
            sendBufferPackets = new List<SendBufferPacket>();
            sendBufferPacket.offset = 0;
            sendBufferPacket.count = 0;
        }

        #region 开始打包发送数据
        public void startPacket()
        {
            sendBufferPacket.offset = dynamicBufferManager.DataCount;
            sendBufferPacket.count = 0;
        }
        #endregion

        #region 结束打包发送数据
        public void endPacket()
        {
            sendBufferPacket.count = dynamicBufferManager.getCount() - sendBufferPacket.offset;
            sendBufferPackets.Add(sendBufferPacket);
        }
        #endregion

        #region 取出第一个数据包
        public bool getFirstPacket(ref int offset, ref int count)
        {
            if (sendBufferPackets.Count <= 0)
            {
                return false;
            }
            offset = sendBufferPackets[0].offset;
            count = sendBufferPackets[0].count;
            return true;
        }
        #endregion

        #region 清除第一个数据包，第一个数据包发送完成后，要清除下第一个数据包，再接着发送下一个
        public bool clearFirstPacket()
        {
            if (sendBufferPackets.Count <= 0)
            {
                return false;
            }
            int count = sendBufferPackets[0].count;
            dynamicBufferManager.clearBuffer(count);
            sendBufferPackets.RemoveAt(0);
            return true;
        }
        #endregion

        #region 清包
        public void clearPacket()
        {
            sendBufferPackets.Clear();
            dynamicBufferManager.clearAll();
        }
        #endregion
    }
}
