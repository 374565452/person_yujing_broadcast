using Maticsoft.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Util.Service
{
    public class OnlineDeviceService
    {
        private static Dictionary<string, Device> onlineDeviceConllection = new Dictionary<string, Device>();
        private static Dictionary<long, List<string>> onlineMSDeviceConllection = new Dictionary<long, List<string>>();

        public static void AddOnline(string deviceNo, Device device)
        {
            if (!onlineDeviceConllection.ContainsKey(deviceNo))
            {
                if (device.MainId > 0)
                {
                    if (onlineMSDeviceConllection.ContainsKey(device.MainId))
                    {
                        List<string> list = onlineMSDeviceConllection[device.MainId];
                        if (!list.Contains(deviceNo))
                            list.Add(deviceNo);
                    }
                    else
                    {
                        List<string> list = new List<string>();
                        list.Add(deviceNo);
                        onlineMSDeviceConllection.Add(device.MainId, list);
                    }
                }
                onlineDeviceConllection.Add(deviceNo, device);
            }
            else
            {
                onlineDeviceConllection[deviceNo] = device;
            }
        }

        public static Device GetOnline(string deviceNo){
            if (onlineDeviceConllection.ContainsKey(deviceNo))
            {
                return onlineDeviceConllection[deviceNo];
            }
            return null;
        }

        public static bool IsOnline(string deviceNo)
        {
            if (onlineDeviceConllection.ContainsKey(deviceNo))
            {
                return true;
            }
            return false;
        }

        public static void RemoveOnline(string deviceNo)
        {
            if (onlineDeviceConllection.ContainsKey(deviceNo))
            {
                Device device = onlineDeviceConllection[deviceNo];
                if (onlineMSDeviceConllection.ContainsKey(device.Id))
                {
                    List<string> list = onlineMSDeviceConllection[device.Id];
                    foreach (string deviceNoSub in list)
                    {
                        RemoveOnline(deviceNoSub);
                    }
                }
                onlineDeviceConllection.Remove(deviceNo);
            }
        }

        public static long GetOnlineCount()
        {
            return onlineDeviceConllection.Count;
        }

        public static List<string> GetDeviceNoSubList(long Id)
        {
            if (onlineMSDeviceConllection.ContainsKey(Id))
            {
                return onlineMSDeviceConllection[Id];
            }
            return null;
        }
    }
}
