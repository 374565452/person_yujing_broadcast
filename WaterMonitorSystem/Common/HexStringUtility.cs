using System;
using System.Text;

namespace Common
{
    public class HexStringUtility
    {
        /// <summary>
        /// 中文转十六进制编码字符串
        /// </summary>
        /// <param name="s">中文字符串</param>
        /// <returns></returns>
        public static string StrToHexString(string s)
        {
            string result = string.Empty;

            byte[] arrByte = StrToByteArray(s);
            for (int i = 0; i < arrByte.Length; i++)
            {
                result += System.Convert.ToString(arrByte[i], 16).PadLeft(2, '0');
            }

            return result;
        }

        /// <summary>
        /// 十六进制编码字符串转中文
        /// </summary>
        /// <param name="s">十六进制编码字符串</param>
        /// <returns></returns>
        public static string HexStringToStr(string s)
        {
            return Encoding.GetEncoding("GB2312").GetString(HexStringToByteArray(s));
        }

        /// <summary>
        /// 十六进制编码字符串转字符数组
        /// </summary>
        /// <param name="s">十六进制编码字符串</param>
        /// <returns></returns>
        public static byte[] HexStringToByteArray(string s)
        {
            s = s.Replace(" ", "").Trim().ToUpper();
            byte[] buffer = new byte[s.Length / 2];
            for (int i = 0; i < s.Length; i += 2)
                buffer[i / 2] = (byte)Convert.ToByte(s.Substring(i, 2), 16);
            return buffer;
        }

        /// <summary>
        /// 字符数组转中文字符串
        /// </summary>
        /// <param name="bs">字符数组</param>
        /// <returns></returns>
        public static string ByteArrayToStr(byte[] bs)
        {
            return Encoding.GetEncoding("GB2312").GetString(bs);
        }

        /// <summary>
        /// 字符数组转十六进制格式字符串
        /// </summary>
        /// <param name="bs">字符数组</param>
        /// <returns></returns>
        public static string ByteArrayToHexString(byte[] bs)
        {
            string result = string.Empty;

            byte[] arrByte = bs;
            for (int i = 0; i < arrByte.Length; i++)
            {
                result += System.Convert.ToString(arrByte[i], 16).PadLeft(2, '0');
            }

            return result;
        }

        /// <summary>
        /// 中文字符串转字符数组
        /// </summary>
        /// <param name="s">中文字符串</param>
        /// <returns></returns>
        public static byte[] StrToByteArray(string s)
        {
            return System.Text.Encoding.GetEncoding("GB2312").GetBytes(s);
        }

        /// <summary>
        /// 十六进制字符串转二进制字符串
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static string HexStringToBinString(string hexString)
        {
            string result = string.Empty;
            foreach (char c in hexString)
            {
                int v = Convert.ToInt32(c.ToString(), 16);
                int v2 = int.Parse(Convert.ToString(v, 2));
                // 去掉格式串中的空格，即可去掉每个4位二进制数之间的空格，
                result += string.Format("{0:d4}", v2);
            }
            return result;
        }

        /// <summary>
        /// 二进制字符串转十六进制字符串
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string BinStringToHexString(string binString)
        {
            string result = string.Empty;
            while (binString.Length >= 4)
            {
                string c = binString.Substring(binString.Length - 4, 4);
                int v = Convert.ToInt32(c.ToString(), 2);
                int v2 = int.Parse(Convert.ToString(v, 16));
                result = v2.ToString("X") + result;

                binString = binString.Substring(0, binString.Length - 4);
            }

            if (binString != "")
            {
                string c = binString.Substring(binString.Length - 4, 4);
                int v = Convert.ToInt32(c.ToString(), 2);
                int v2 = int.Parse(Convert.ToString(v, 16));
                result = v2.ToString("X") + result;
            }
            return result;
        }
    }
}
