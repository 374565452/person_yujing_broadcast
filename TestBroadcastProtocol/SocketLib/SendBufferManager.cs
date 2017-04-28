using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketLib
{
    struct SendPacket
    {
        public int offset;
        public int count;
    }
    public class SendBufferManager
    {
        private DynamicBufferManager dynamicBufferManager;
        public DynamicBufferManager DynamicBufferManager
        {
            get
            {
                return dynamicBufferManager;
            }
        }
        private SendPacket sendPacket;
        private List<SendPacket> sendPackets;

        public SendBufferManager(int bufferSize)
        {
            dynamicBufferManager = new DynamicBufferManager(bufferSize);
            sendPackets = new List<SendPacket>();
            sendPacket.offset = 0;
            sendPacket.count = 0;
        }

        public void startPacket()
        {
            sendPacket.offset = dynamicBufferManager.getCount();
            sendPacket.count = 0;
        }
        public void endPacket()
        {
            sendPacket.count = dynamicBufferManager.getCount() - sendPacket.offset;
            sendPackets.Add(sendPacket);
        }

        public bool getFirstPacket(ref int offset, ref int count)
        {
            if (sendPackets.Count <= 0)
            {
                return false;
            }
            offset = sendPackets[0].offset;
            count = sendPackets[0].count;
            return true;
        }

        public bool clearFirstPacket()
        {
            if (sendPackets.Count <= 0)
            {
                return false;
            }
            int count = sendPackets[0].count;
            dynamicBufferManager.clearBuffer(count);
            sendPackets.RemoveAt(0);
            return true;
        }

        public void clearPacket()
        {
            dynamicBufferManager.clearAll();
            sendPackets.Clear();
        }
    }
}
