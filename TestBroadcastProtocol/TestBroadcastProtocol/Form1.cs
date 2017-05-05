using SocketLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestBroadcastProtocol
{
    public partial class Form1 : Form
    {
        private SocketClient client;
        public Form1()
        {
            InitializeComponent();
            client = new SocketClient("127.0.0.1", 7002);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            client.connectToServer();
        }

        private void test_Click(object sender, EventArgs e)
        {
            for (int j = 0; j < 20; j++)
            {
                byte[] buffer = new byte[8 * 1024];

                int len = buffer.Length;
                for (int i = 0; i < len; i++)
                {
                    buffer[i] = (byte)(i & 0xFF);
                }
                byte[] datas = new byte[len + 8];
                datas[0] = 0xaa;
                datas[1] = 0x15;
                datas[2] = 0x02;
                datas[3] = (byte)((len) >> 8 & 0xFF);
                datas[4] = (byte)(len & 0xFF);
                Array.Copy(buffer, 0, datas, 5, len);
                datas[5 + len] = 0x13;
                datas[6 + len] = 0x14;
                datas[7 + len] = 0x55;
                client.sendCommand(datas);
                //Thread.Sleep(1000 * 3);
            }
        }
    }
}
