using DTU.GateWay.Protocol;
using System.Collections.Generic;

namespace Server.Util.Service
{
    public class WaterDeviceService
    {
        private static Dictionary<string, WaterBaseMessage[]> Conllection_Message = new Dictionary<string, WaterBaseMessage[]>();

        public static void Add(string deviceNo, WaterBaseMessage[] msg)
        {
            if (!Conllection_Message.ContainsKey(deviceNo))
            {
                Conllection_Message.Add(deviceNo, msg);
            }
            else
            {
                Conllection_Message[deviceNo] = msg;
            }
        }

        public static WaterBaseMessage[] Get(string deviceNo)
        {
            if (Conllection_Message.ContainsKey(deviceNo))
            {
                return Conllection_Message[deviceNo];
            }
            return null;
        }

        public static void Remove(string deviceNo)
        {
            if (Conllection_Message.ContainsKey(deviceNo))
            {
                Conllection_Message.Remove(deviceNo);
            }
        }

        public static int GetCount()
        {
            return Conllection_Message.Count;
        }
    }
}
