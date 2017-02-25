using System.Collections.Generic;

namespace Server.Util.DataProcess
{
    public class SendData
    {
        private List<string> sendDataList;

        public SendData()
        {
            sendDataList = new List<string>();
        }

        public string getSendData()
        {
            string temp = "";
            if (sendDataList.Count > 0)
            {
                temp = sendDataList[0];
                for (int i = 1; i < sendDataList.Count; i++)
                {
                    temp = temp + sendDataList[i];
                }
            }
            return temp;
        }

        public void clear()
        {
            sendDataList.Clear();
        }

        public void addData(string data)
        {
            this.sendDataList.Add(data);
        }
    }
}
