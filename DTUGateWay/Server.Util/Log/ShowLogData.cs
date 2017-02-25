using System;
using System.Collections.Concurrent;

namespace Server.Util.Log
{
    public class ShowLogData
    {
        private static ConcurrentQueue<string> ShowLogDataList;

        public static bool isShow = false;

        public static void add(string s)
        {
            if (ShowLogDataList == null)
                ShowLogDataList = new ConcurrentQueue<string>();
            if (isShow)
            {
                if (ShowLogDataList.Count < 1000)
                {
                    ShowLogDataList.Enqueue(s);
                }
            }
        }

        public static string get()
        {
            string str = "";
            try
            {
                if (ShowLogDataList.Count > 0)
                    ShowLogDataList.TryDequeue(out str);
            }
            catch { }
            return str;
        }
    }
}
