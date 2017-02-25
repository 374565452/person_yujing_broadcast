using Common;
using DTU.GateWay.Protocol;
using Maticsoft.Model;
using Module;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WaterMonitorSystem.Src;
using WaterMonitorSystem.WebServices;

namespace WaterMonitorSystem
{
    public partial class accept : System.Web.UI.Page
    {
        log4net.ILog myLogger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string uperr = "no|未知！";
                try
                {
                    if (HttpContext.Current.Request.QueryString["loginIdentifer"] == null)
                    {
                        uperr = "no|非法操作！";
                    }
                    else
                    {
                        string loginIdentifer = HttpContext.Current.Request.QueryString["loginIdentifer"];

                        LoginUser loginUser = GlobalAppModule.GetLoginUser(loginIdentifer);
                        if (loginUser == null)
                        {
                            uperr = "no|未登录！";
                        }
                        else if (loginUser.LoginTimeout)
                        {
                            uperr = "no|登录超时！";
                        }
                        else
                        {
                            loginUser.LastOperateTime = DateTime.Now;
                            CommonUtil.WaitMainLibInit();

                            byte typeStr = Convert.ToByte(HttpContext.Current.Request.QueryString["a"]);
                            long realDevId = Convert.ToInt64(HttpContext.Current.Request.QueryString["realDevId"]);

                            HttpFileCollection files = HttpContext.Current.Request.Files;
                            if (files.Count > 0)
                            {
                                uperr = upSingleFile(files[0], typeStr, loginUser, realDevId);
                            }
                            else
                            {
                                uperr = "no|未选择文件！";
                            }
                        }
                    }
                }
                catch
                {
                    uperr = "no|类型转换错误！";
                }
                Response.Write(uperr);
                Response.End();
            }
        }

        //上传文件
        private string upSingleFile(HttpPostedFile file, byte typeStr, LoginUser loginUser, long DevId)
        {
            if (!SystemService.isConnect)
            {
                return "no|与通讯服务器连接中断！";
            }
            Device device = DeviceModule.GetDeviceByID(DevId);
            if (device == null)
            {
                return "no|要操作的设备不存在！";
            }

            string DeviceNo = DeviceModule.GetFullDeviceNoByID(device.Id);           

            string infos = "";
            bool fileOK = false;
            string fileExtension;
            string fileName = System.IO.Path.GetFileName(file.FileName);
            if (fileName != "")
            {
                if (file.ContentLength >= 1024 * 1024 * 1)
                {
                    infos = "no|文件太大，目前仅支持1M以内的文档！";
                }
                else
                {
                    string theFileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + DeviceNo + "_" + System.IO.Path.GetFileNameWithoutExtension(fileName);
                    if (typeStr == 0x01)
                    {
                        theFileName += ".jpg";
                    }
                    else if (typeStr == 0x02)
                    {
                        theFileName += ".bin";
                    }

                    fileExtension = System.IO.Path.GetExtension(fileName).ToLower();
                    //String[] allowedExtensions = { ".jpg", ".jpeg", ".gif", ".bmp", ".png", ".icon" };
                    String[] allowedExtensions = { };
                    if (allowedExtensions.Length > 0)
                    {
                        for (int i = 0; i < allowedExtensions.Length; i++)
                        {
                            if (fileExtension == allowedExtensions[i])
                            {
                                fileOK = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        fileOK = true;
                    }

                    if (!fileOK)
                    {
                        infos = "no|不支持上传此类型文件！目前支持的图片格式有：" + String.Join("|", allowedExtensions);
                    }
                    else
                    {
                        int packetSize = 230;
                        string path = System.Web.HttpContext.Current.Request.MapPath("~/UploadFiles/User/" + loginUser.LoginName + "/");
                        DirectoryInfo dir = new DirectoryInfo(path);
                        if (!dir.Exists)
                            dir.Create();
                        file.SaveAs(path + theFileName);
                        
                        byte[] bsOld = new byte[file.ContentLength];
                        file.InputStream.Read(bsOld, 0, bsOld.Length);
                        file.InputStream.Seek(0, SeekOrigin.Begin);
                        int count = file.ContentLength / packetSize + 1;
                        //byte[] bsNew = new byte[file.ContentLength];
                        for (int i = 0; i < count; i++)
                        {
                            try
                            {
                                byte[] bs;
                                if (i != count - 1)
                                {
                                    bs = new byte[packetSize];
                                }
                                else
                                {
                                    bs = new byte[file.ContentLength + 1 - packetSize * i];
                                }

                                if (i == 0)
                                {
                                    bs[0] = typeStr;
                                    Array.Copy(bsOld, 0, bs, 1, bs.Length - 1);
                                    //Array.Copy(bs, 1, bsNew, 0, bs.Length - 1);
                                }
                                else
                                {
                                    Array.Copy(bsOld, packetSize * i - 1, bs, 0, bs.Length);
                                    //Array.Copy(bs, 0, bsNew, packetSize * i - 1, bs.Length);
                                }

                                CmdToDtuSendFile cmd = new CmdToDtuSendFile();
                                cmd.AddressField = DeviceNo.Substring(0, 12) + Convert.ToInt32(DeviceNo.Substring(12, 3)).ToString("X").PadLeft(2, '0');
                                cmd.StationType = (byte)device.StationType;
                                cmd.StationCode = device.StationType == 2 ? device.StationCode : 0;
                                cmd.Sum = (short)count;
                                cmd.Curr = (short)(i + 1);
                                cmd.Content = bs;
                                cmd.RawDataChar = cmd.WriteMsg();
                                cmd.RawDataStr = HexStringUtility.ByteArrayToHexString(cmd.RawDataChar);

                                DeviceOperation op = new DeviceOperation();
                                op.DeviceNo = DeviceNo;
                                op.DeviceName = device.DeviceName;
                                op.OperationTime = DateTime.Now;
                                op.OperationType = "中心下发文件";
                                op.RawData = cmd.RawDataStr;
                                op.Remark = cmd.Sum + "|" + cmd.Curr;
                                op.UserId = loginUser.UserId;
                                op.UserName = SysUserModule.GetUser(loginUser.UserId).UserName;
                                op.State = "等待发送";
                                
                                myLogger.Info(op.OperationType + "：" + cmd.RawDataStr);
                                byte[] cmd_send = cmd.RawDataChar;
                                myLogger.Error((i + 1).ToString().PadLeft(count.ToString().Length, '0') + "：" + cmd.RawDataStr);

                                ResMsg resMsg = SendCmd(cmd_send);
                                if (resMsg.Result)
                                {
                                    if (resMsg.Message == BaseProtocol.DeviceOffline)
                                    {
                                        infos = "no|查询终端不在线！";
                                    }
                                    else
                                    {
                                        byte[] cmd_receive = HexStringUtility.HexStringToByteArray(resMsg.Message);
                                        BaseMessage message = new BaseMessage();
                                        message.RawDataChar = cmd_receive;
                                        string msg = message.ReadMsg();
                                        if (msg == "")
                                        {
                                            CmdResponseToDtuSendFile res = new CmdResponseToDtuSendFile(message);
                                            string msg1 = res.ReadMsg();
                                            if (msg1 == "")
                                            {
                                                if (res.Result == 1)
                                                {
                                                    op.State = "发送成功";
                                                    //infos = "yes|结果为1，继续";
                                                }
                                                else
                                                {
                                                    op.State = "发送成功";
                                                    infos = "no|结果为0，退出";
                                                }
                                            }
                                            else
                                            {
                                                op.State = "发送失败";
                                                infos = "no|" + msg1;
                                            }
                                        }
                                        else
                                        {
                                            op.State = "发送失败";
                                            infos = "no|" + msg;
                                        }
                                    }
                                }

                                if (infos.StartsWith("no|"))
                                {
                                    op.State += "|" + infos.Split('|')[1];
                                }
                                DeviceOperationModule.AddDeviceOperation(op);
                                if (op.State.Contains("发送失败"))
                                {
                                    myLogger.Error((i + 1).ToString().PadLeft(count.ToString().Length, '0') + "-" + count + "：" + infos);
                                    return infos;
                                }
                            }
                            catch
                            {
                                myLogger.Error((i + 1).ToString().PadLeft(count.ToString().Length, '0') + "-" + count + "：出错，发送失败");
                                return "no|" + (i + 1);
                            }
                        }
                        
                        infos = "yes|" + theFileName.Split('_')[2];
                    }
                }
            }
            else
            {
                infos = "no|未选择文件！";
            }
            return infos;
        }

        private ResMsg SendCmd(byte[] cmd_send)
        {
            ResMsg msg = new ResMsg(false, "");

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
                msg.Message = "与网关通讯失败！";
                return msg;
            }

            tcpService.SendData(cmd_send, 0, cmd_send.Length);

            bool waitRsp = false;
            timeDelay = 0;
            while (timeDelay < tcpService.TcpWait)
            {
                if (tcpService.socketData.Buffer.Length >= CommandCommon.CMD_MIN_LENGTH)
                {
                    byte[] re = tcpService.socketData.Buffer.Buffer;
                    byte[] buffer_new = new byte[tcpService.socketData.Buffer.Length];
                    Array.Copy(re, buffer_new, tcpService.socketData.Buffer.Length);
                    waitRsp = true;
                    msg.Result = true;
                    msg.Message = HexStringUtility.ByteArrayToHexString(buffer_new);

                    myLogger.Info("收到数据：" + msg.Message);
                }

                if (waitRsp == true)
                {
                    //myLogger.Info("获取响应结束");
                    break;
                }

                if (tcpService.SOCKET_STATE == TcpCommunication.TCP_SocketState.SOCKET_CLOSED)
                {
                    //myLogger.Info("Socket关闭结束");
                    break;
                }

                Thread.Sleep(100);
                timeDelay = timeDelay + 1;
            }

            tcpService.Close();

            if (waitRsp == false)
            {
                msg.Message = "等待设备回复超时！";
            }
            return msg;
        }
    }
}