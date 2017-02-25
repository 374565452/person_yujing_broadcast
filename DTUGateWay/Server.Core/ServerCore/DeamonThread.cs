using Server.Util.Cache;
using System;
using System.Threading;

namespace Server.Core.ServerCore
{
    public class DeamonThread
    {
        log4net.ILog LogHelper = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private Thread deamonThread;

        private ServerSocket serverSocket;

        private bool isStart;

        //超时时间，以秒来计算
        private int timeout;

        //public long RestartTime =  12*60 * 60 ;
        //private long count = 0;

        public DeamonThread(ServerSocket serverSocket, int timeout)
        {
            this.serverSocket = serverSocket;
            this.timeout = timeout;
            deamonThread = new Thread(deamonThreadStart);
            isStart = true;
            deamonThread.Start();
        }

        /// <summary>
        /// 关闭守护进程
        /// </summary>
        public void close()
        {
            isStart = false;
        }

        /// <summary>
        /// 判定DTU终端设备连接超时情况
        /// 在通信链路上一定时间内(timeout)没有数据往来,即认为超时
        /// 关闭此连接
        /// DTU定时发送一个心跳，timeout应该设置的长度大于心跳间隔时间
        /// </summary>
        public void deamonThreadStart()
        {
            while (deamonThread.IsAlive && isStart)
            {
                AsyncSocketUserToken[] userTokenArray = null;
                serverSocket.AsyncSocketUserTokenList.copyList(ref userTokenArray);
                for (int i = 0; i < userTokenArray.Length; i++)
                {
                    if (!deamonThread.IsAlive)
                        break;
                    try
                    {
                        if ((DateTime.Now - userTokenArray[i].ActiveDateTime).TotalSeconds > timeout)
                        {
                            lock (userTokenArray[i])
                            {
                                serverSocket.closeClientSocket(userTokenArray[i], true);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        if (SysCache.ShowErrorLog)
                        {
                            string error = string.Format("程序在守护进程中出错，错误信息{0}", e.Message);
                            LogHelper.Error(error);
                        }
                    }
                }
                for (int i = 0; i < 100/*2000 20 * 1000 / 10*/; i++) //20s检测一次
                {
                    if (!deamonThread.IsAlive)
                        break;
                    Thread.Sleep(10);
                }
            }
        }
    }
}
