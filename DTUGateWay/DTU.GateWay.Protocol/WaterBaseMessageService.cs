using Common;
using System;
using System.Globalization;

namespace DTU.GateWay.Protocol
{
    public class WaterBaseMessageService
    {
        public static string GetMsgList_WriteMsg(WaterBaseMessage wbm, string UserDataAll, out byte[] UserDataBytesAll, out WaterBaseMessage[] MsgList)
        {
            UserDataBytesAll = HexStringUtility.HexStringToByteArray(UserDataAll);

            wbm.DataBeginChar = (byte)WaterBaseProtocol.DataBeginChar.STX;
            if (UserDataBytesAll.Length > WaterBaseProtocol.PocketSize)
            {
                wbm.TotalPackage = (UserDataBytesAll.Length - 1) / WaterBaseProtocol.PocketSize + 1;
                wbm.DataBeginChar = (byte)WaterBaseProtocol.DataBeginChar.SYN;
            }
            else
            {
                wbm.TotalPackage = 1;
            }
            MsgList = new WaterBaseMessage[wbm.TotalPackage];

            for (int i = 0; i < wbm.TotalPackage; i++)
            {
                WaterBaseMessage bm = new WaterBaseMessage();
                bm.CenterStation = wbm.CenterStation;
                bm.RemoteStation = wbm.RemoteStation;
                bm.PW = wbm.PW;
                bm.AFN = wbm.AFN;
                bm.UpOrDown = wbm.UpOrDown;
                bm.TotalPackage = wbm.TotalPackage;
                bm.CurrentPackage = i + 1;
                bm.DataBeginChar = wbm.DataBeginChar;
                byte[] bs = null;
                if (i == wbm.TotalPackage - 1)
                {
                    bs = new byte[UserDataBytesAll.Length - WaterBaseProtocol.PocketSize * i];
                    wbm.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Up.ETX;
                }
                else
                {
                    bs = new byte[WaterBaseProtocol.PocketSize];
                    wbm.DataEndChar = (byte)WaterBaseProtocol.DataEndChar_Up.ETB;
                }
                bm.DataEndChar = wbm.DataEndChar;
                Array.Copy(UserDataBytesAll, WaterBaseProtocol.PocketSize * i, bs, 0, bs.Length);
                bm.UserDataBytes = bs;

                string msg = bm.WriteMsgBase();
                if (msg == "")
                {
                    MsgList[i] = bm;
                }
                else
                {
                    MsgList = null;
                    return (i + 1) + "：" + msg;
                }
            }

            return "";
        }

        public static string ReadMsg(WaterBaseMessage[] MsgList, out string UserDataAll, out byte[] UserDataBytesAll)
        {
            UserDataAll = "";
            UserDataBytesAll = null;

            if (MsgList == null || MsgList.Length == 0)
            {
                return "无信息，无法分析！";
            }

            UserDataAll = "";
            int c = 0;
            foreach (WaterBaseMessage bm in MsgList)
            {
                c++;
                if (bm != null && bm.UserData != null)
                {
                    UserDataAll += bm.UserData;
                }
                else
                {
                    return "第" + c + "包无信息，无法分析！";
                }
            }

            UserDataBytesAll = HexStringUtility.HexStringToByteArray(UserDataAll);

            return "";
        }

        public static string GetSerialNumberAndSendTime(string UserDataAll, out short SerialNumber, out DateTime SendTime)
        {
            SerialNumber = 0;
            SendTime = DateTime.Now;
            try
            {
                SerialNumber = Convert.ToInt16(UserDataAll.Substring(0, 4), 16);
            }
            catch (Exception ex)
            {
                return "获取流水号出错";
            }

            try
            {
                SendTime = DateTime.ParseExact("20" + UserDataAll.Substring(4, 12), "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                return "获取发报时间出错";
            }

            return "";
        }
    }
}
