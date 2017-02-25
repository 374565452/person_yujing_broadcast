using Common;
using Server.Util.Log;
using Server.Web.ServerCore;
using System;
using System.Diagnostics;

namespace Server.Web.ProtocolProcess
{
    public class WebInvokeElement : AsyncProtocolInvokeElement
    {
        log4net.ILog LogHelper = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public WebInvokeElement(ServerSocket server, AsyncSocketUserToken userToken)
            : base(server, userToken)
        {
        }
        public void printLog(string str)
        {
            Debug.Print("hello world");
        }
        public override bool processCommand(DTU.GateWay.Protocol.Command command)
        {
            string receive = HexStringUtility.ByteArrayToHexString(command.Data);
            LogHelper.Info("收到的数据为：[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]" + receive);
            ShowLogData.add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":[" + userToken.ConnectedSocket.RemoteEndPoint.ToString() + "]" + receive);

            byte[] data = HexStringUtility.HexStringToByteArray(receive);
            //data[data.Length - 1] = FormatHelper.CheckSum(data, 0, data.Length - 1);
            send(data, 0, data.Length);

            return true;
        }
    }
}
