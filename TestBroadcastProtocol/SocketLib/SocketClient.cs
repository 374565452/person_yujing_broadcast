using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketLib
{
    public class SocketClient : SocketInvokeElement
    {
        private string ip;
        private int port;

        public SocketClient(string ip, int port)
        {
            this.ip = ip;
            this.port = port;
            showInfoLog = true;
            showErrorLog = true;
        }

        public bool  connectToServer()
        {
            try
            {
                this.connect(this.ip, this.port);
                if (showInfoLog)
                {
                    string error = string.Format("成功连接服务器{0},端口号为{1}", ip, port);
                    //LogHelper.log(error, this, null, LogLevel.Info);
                }
                return true;
            }
            catch (Exception e)
            {
               
                if (showErrorLog)
                {
                    string msg = string.Format("连接服务器{0}，端口号{1}！", ip, port);
                    //LogHelper.log(msg, this, e, LogLevel.Error);
                }
            }
            return false;
        }

        public void sendCommand(byte[]datas)
        {
            doSendCommand(datas);
        }

        public override bool receiveProcess(DynamicBufferManager bufferManager)
        {
            //FormatHelper.GetStringByArray(bufferManager.Buffer, 0, bufferManager.Buffer.Length);
           // StringBuilder sb = new StringBuilder();
            string str = "";
            //for (int i = 0; i < bufferManager.getCount(); i++)
           // {
                //sb.Append(bufferManager.Buffer[i]);
              //  str += string.Format("{0:X2}", bufferManager.Buffer[i]);
                
            //}
            Debug.Print("the recv len is " +bufferManager.getCount()+  "\r\n");
           // string str = sb.ToString();
            //Console.WriteLine(str);

            /*byte[] buff = bufferManager.Buffer;
            int len = bufferManager.getCount();
            UInt16 crc = FormatHelper.CRC16(buff, 0, len-2);

            byte low = (byte)(crc & 0x00FF);
            
            byte high = (byte)((crc & 0xFF00) >> 8);

            //Console.WriteLine("{0:X2}", low);
            //Console.WriteLine("{0:X2}", high);
            //Console.WriteLine("the len is {0}", len);
            //Console.WriteLine("{0:X2}", buff[len - 1]);
            //Console.WriteLine("{0:X2}", buff[len - 2]);
            if (low == buff[len - 1] && high == buff[len - 2])
            {
                //Console.WriteLine("校验成功----------------------！！！！！！");
                if (buff[0] == 0x68 && buff[1] == 0x00)
                {
                    //Console.WriteLine("校验成功！！！！！！");
                    MountainCommand command = new MountainCommand();
                    command.action = buff[2];
                    Array.Copy(buff, 3, command.address, 0, command.address.Length);

                    command.frameNum = buff[3 + command.address.Length];
                    command.frameNo = buff[3 + command.address.Length + 1];
                    command.functionCode = buff[3 + command.address.Length + 2];
                    command.dataLen = buff[3 + command.address.Length + 3];
                    if (command.dataLen != 0)
                    {
                        int count = Int32.Parse(string.Format("{0:X2}", command.dataLen), System.Globalization.NumberStyles.HexNumber);
                        //Console.WriteLine(count);
                        command.data = new byte[count];
                        Array.Copy(buff, 3 + command.address.Length + 4, command.data, 0, count);
                    }

                    string deviceId = BuildMountainCommand.getStringFromUnicode(command.address);
                    //Console.WriteLine("111111111111111---"+deviceId);
                    proxySend(deviceId, command);

                }
            }*/
            //如果不清除，会一直重复收
            int len = bufferManager.getCount();
            bufferManager.clearBuffer(len);
            return true;
        }

       
    }
}
