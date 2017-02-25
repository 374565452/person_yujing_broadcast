using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Util.Cache
{
    public class FilePathCache
    {
        public static string Path;

        public static Dictionary<string, object[]> FileCache_FromDTU;

        public static object[] GetFileCacheFromDTU(string key)
        {
            if (FileCache_FromDTU == null) FileCache_FromDTU = new Dictionary<string, object[]>();
            if (FileCache_FromDTU.ContainsKey(key))
            {
                return FileCache_FromDTU[key];
            }
            return null;
        }

        public static void AddFileCacheFromDTU(string key, object[] objs)
        {
            if (FileCache_FromDTU == null) FileCache_FromDTU = new Dictionary<string, object[]>();
            if (!FileCache_FromDTU.ContainsKey(key))
            {
                FileCache_FromDTU.Add(key, objs);
            }
            else
            {
                FileCache_FromDTU[key] = objs;
            }
        }

        public static bool RemoveFileCacheFromDTU(string key)
        {
            if (FileCache_FromDTU.ContainsKey(key))
            {
                return FileCache_FromDTU.Remove(key);
            }
            return false;
        }
    }
}
