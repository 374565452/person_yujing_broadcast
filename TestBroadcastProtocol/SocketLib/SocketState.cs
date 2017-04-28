using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketLib
{
    public class SocketState
    {
        private Socket socket;
        public Socket Socket
        {
            get
            {
                return socket;
            }
            set
            {
                socket = value;
            }
        }

        private byte[] receiveBuffer;

        public SocketState(int receiveSize)
        {
            receiveBuffer = new byte[receiveSize];
        }

        public byte[] ReceiveBuffer
        {
            get
            {
                return receiveBuffer;
            }
        }
    }
}
