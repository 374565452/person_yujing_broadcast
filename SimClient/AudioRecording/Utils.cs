using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AudioRecording
{
    public class Utils
    {

        public static string uploadFile(string fileName,string filePath, string url)
        {
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            byte[] byteFile = new byte[fs.Length];

            fs.Read(byteFile, 0, Convert.ToInt32(fs.Length));

            fs.Close();

            string postData = "f="+fileName+"&u=" + HttpUtility.UrlEncode(Convert.ToBase64String(byteFile));

            WebClient webclient = new WebClient();

            webclient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            byte[] buffer = webclient.UploadData(url, "POST", byteArray);

            string msg = Encoding.UTF8.GetString(buffer);
            return msg;
        }

        public static string random_str(int length)
        {
            System.Threading.Thread.Sleep(1);
            int number;
            string checkCode = String.Empty;
           // int iSeed = 10;
            Random ro = new Random(10);
            long tick = DateTime.Now.Ticks;
            Random random = new Random((int)(tick & 0xffffffffL) | (int)(tick >> 32));
            for (int i = 0; i < length; i++)
            {
                number = random.Next();
                number = number % 36;
                if (number < 10)
                {
                    number += 48;
                }
                else
                {
                    number += 55;
                }
                checkCode += ((char)number).ToString();
            }
            return checkCode;
        }

    }
}
