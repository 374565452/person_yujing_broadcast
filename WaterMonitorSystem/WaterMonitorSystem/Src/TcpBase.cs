using Common;
using System;
using System.Threading;

namespace WaterMonitorSystem.Src
{
    public class TcpBase
    {
        public class TcpResponse
        {
            public string Description;
            public bool Status;
            public byte[] Parameter;

            public TcpResponse()
            {
                Parameter = new byte[0];
            }
        }

        /// <summary>
        /// 基本执行tcp操作
        /// </summary>
        /// <param name="cmdTag">指令</param>
        /// <returns></returns>
        public static TcpResponse TcpSendCmdBase(CmdTag cmdTag)
        {
            int timeDelay = 0;
            bool waitRsp = false;
            TcpResponse rsp = new TcpResponse();
            MemQueue<CmdTag> cmdlist = new MemQueue<CmdTag>();
            TcpCommunication tcpService;
            CmdTag cmdRspTag;

            tcpService = new TcpCommunication();

            timeDelay = 0;
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
                rsp.Description = "与网关通信失败!";
                rsp.Status = false;
                return rsp;
            }

            timeDelay = 0;

            byte[] cmd_send = Command.BuildPlatformCmd(cmdTag);

            tcpService.SendData(cmd_send, 0, cmd_send.Length);
            cmdRspTag = new CmdTag();

            timeDelay = 0;
            while (timeDelay < tcpService.TcpWait)
            {

                if (tcpService.socketData.Buffer.Length >= CommandCommon.CMD_MIN_LENGTH)
                {
                    Command.ParsePlatFormCmd(ref tcpService.socketData.Buffer, ref cmdlist, "");

                    while (cmdlist.Count > 0)
                    {
                        cmdRspTag = cmdlist.Get();

                        //非本设备的回复
                        if ((cmdRspTag.DeviceInfo.SerialLong != cmdTag.DeviceInfo.SerialLong))
                        {
                            continue;
                        }

                        if ((cmdRspTag.CmdType == cmdTag.CmdType) && (cmdRspTag.CmdCode == cmdTag.CmdCode))
                        {
                            waitRsp = true;
                            break;
                        }

                    }
                }

                if (waitRsp == true)
                {
                    break;
                }

                if (tcpService.SOCKET_STATE == TcpCommunication.TCP_SocketState.SOCKET_CLOSED)
                {
                    break;
                }
                Thread.Sleep(100);
                timeDelay = timeDelay + 1;
            }

            tcpService.Close();

            if (waitRsp == false)
            {
                rsp.Status = false;
                rsp.Description = "等待设备回复超时!";

                return rsp;
            }

            rsp.Status = false;
            rsp.Parameter = new byte[cmdRspTag.Param.Length];
            if (rsp.Parameter.Length > 0)
            {
                Array.Copy(cmdRspTag.Param, rsp.Parameter, cmdRspTag.Param.Length);
            }

            switch (cmdRspTag.CmdState)
            {
                case CommandState.CMD_STATE_SUCCESS:
                    {
                        rsp.Status = true;
                        rsp.Description = "设备回复成功";
                        break;
                    }
                case CommandState.CMD_STATE_FAIL:
                    {
                        rsp.Description = "设备回复失败";
                        break;
                    }
                case CommandState.CMD_STATE_NOT_SUPPORT:
                    {
                        rsp.Description = "设备回复不支持";
                        break;
                    }
                default:
                    {
                        rsp.Description = "未知回复";
                        break;
                    }
            }

            return rsp;
        }
    }
}