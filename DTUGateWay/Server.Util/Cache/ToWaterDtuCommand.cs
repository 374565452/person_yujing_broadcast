using DTU.GateWay.Protocol;
using System;
using System.Collections.Generic;

namespace Server.Util.Cache
{
    public class ToWaterDtuCommand
    {
        private static Dictionary<string, WaterBaseMessage> BaseMessageConllection_ToDtu = new Dictionary<string, WaterBaseMessage>();
        private static Dictionary<string, WaterBaseMessage> BaseMessageConllection_FromDtu = new Dictionary<string, WaterBaseMessage>();

        public static DateTime dateNew;
        public static bool isClear = true;

        public static void Clear()
        {
            BaseMessageConllection_ToDtu = new Dictionary<string, WaterBaseMessage>();
            BaseMessageConllection_FromDtu = new Dictionary<string, WaterBaseMessage>();
            isClear = true;
        }

        public static WaterBaseMessage GetBaseMessageToDtuByKey(string Key)
        {
            lock (BaseMessageConllection_ToDtu)
            {
                if (BaseMessageConllection_ToDtu.ContainsKey(Key))
                {
                    return BaseMessageConllection_ToDtu[Key];
                }
            }
            return null;
        }

        public static void AddBaseMessageToDtu(string Key, WaterBaseMessage bm)
        {
            dateNew = DateTime.Now;
            isClear = false;
            RemoveBaseMessageFromDtu(Key);
            lock (BaseMessageConllection_ToDtu)
            {
                if (!BaseMessageConllection_ToDtu.ContainsKey(Key))
                {
                    BaseMessageConllection_ToDtu.Add(Key, bm);
                }
                else
                {
                    BaseMessageConllection_ToDtu[Key] = bm;
                }
            }
        }

        public static bool RemoveBaseMessageToDtu(string Key)
        {
            lock (BaseMessageConllection_ToDtu)
            {
                if (BaseMessageConllection_ToDtu.ContainsKey(Key))
                {
                    return BaseMessageConllection_ToDtu.Remove(Key);
                }
            }
            return false;
        }

        public static WaterBaseMessage GetBaseMessageFromDtuByKey(string Key)
        {
            lock (BaseMessageConllection_FromDtu)
            {
                if (BaseMessageConllection_FromDtu.ContainsKey(Key))
                {
                    RemoveBaseMessageToDtu(Key);
                    return BaseMessageConllection_FromDtu[Key];
                }
            }
            return null;
        }

        public static void AddBaseMessageFromDtu(string Key, WaterBaseMessage bm)
        {
            lock (BaseMessageConllection_FromDtu)
            {
                if (!BaseMessageConllection_FromDtu.ContainsKey(Key))
                {
                    BaseMessageConllection_FromDtu.Add(Key, bm);
                }
                else
                {
                    BaseMessageConllection_FromDtu[Key] = bm;
                }
            }
        }

        public static bool RemoveBaseMessageFromDtu(string Key)
        {
            lock (BaseMessageConllection_FromDtu)
            {
                if (BaseMessageConllection_FromDtu.ContainsKey(Key))
                {
                    return BaseMessageConllection_FromDtu.Remove(Key);
                }
            }
            return false;
        }
    }
}
