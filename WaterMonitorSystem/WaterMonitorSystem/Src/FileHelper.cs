using System.IO;

namespace WaterMonitorSystem.Src
{
    public class FileHelper
    {
        public static bool IsExists(string fileName)
        {
            return File.Exists(fileName);
        }

        public static void writeFile(string fileName, string content)
        {
            byte[] myByte = System.Text.Encoding.UTF8.GetBytes(content);
            using (FileStream fsWrite = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite))
            {
                fsWrite.SetLength(0);
                fsWrite.Write(myByte, 0, myByte.Length);
            };
        }

        public static string ReadFile(string fileName)
        {
            string myStr = "";
            using (FileStream fsRead = new FileStream(fileName, FileMode.Open))
            {
                int fsLen = (int)fsRead.Length;
                byte[] heByte = new byte[fsLen];
                int r = fsRead.Read(heByte, 0, heByte.Length);
                myStr = System.Text.Encoding.UTF8.GetString(heByte);
            }
            return myStr;
        }
    }
}