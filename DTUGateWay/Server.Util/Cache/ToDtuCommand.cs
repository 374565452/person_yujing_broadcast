using DTU.GateWay.Protocol;
using System;
using System.Collections.Generic;

namespace Server.Util.Cache
{
    public class ToDtuCommand
    {
        private static Dictionary<string, BaseMessage> BaseMessageConllection_ToDtu = new Dictionary<string, BaseMessage>();
        private static Dictionary<string, BaseMessage> BaseMessageConllection_FromDtu = new Dictionary<string, BaseMessage>();

        public static DateTime dateNew;
        public static bool isClear = true;

        public static void Clear()
        {
            BaseMessageConllection_ToDtu = new Dictionary<string, BaseMessage>();
            BaseMessageConllection_FromDtu = new Dictionary<string, BaseMessage>();
            isClear = true;
        }

        public static BaseMessage GetBaseMessageToDtuByKey(string Key)
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

        public static void AddBaseMessageToDtu(string Key, BaseMessage bm)
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

        public static BaseMessage GetBaseMessageFromDtuByKey(string Key)
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

        public static void AddBaseMessageFromDtu(string Key, BaseMessage bm)
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
