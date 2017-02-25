using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace WaterMonitorSystem.Src
{
    public class TcpRunThread
    {
        static log4net.ILog myLogger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void ParameterRun(object o)
        {
            TcpCommunication tcpService = new TcpCommunication();

            int timeDelay = 0;
            //待socket准备好
            while (timeDelay < tcpService.TcpWait)
            {
                if ((tcpService.SOCKET_STATE == TcpCommunication.TCP_SocketState.SOCKET_CONNECTED)
                    || (tcpService.SOCKET_STATE == TcpCommunication.TCP_SocketState.SOCKET_CLOSED))
                {
                    break;
                }

                Thread.Sleep(100);
                timeDelay = timeDelay + 1;
            }

            if (tcpService.SOCKET_STATE != TcpCommunication.TCP_SocketState.SOCKET_CONNECTED)
            {
                myLogger.Info("与网关通讯失败！");
                return;
            }

            try
            {
                byte[] cmd_send = HexStringUtility.HexStringToByteArray(o.ToString());

                tcpService.SendData(cmd_send, 0, cmd_send.Length);
            }
            catch (Exception ex)
            {
                myLogger.Info("数据发送失败！" + ex.Message);
            }
        }
    }
}