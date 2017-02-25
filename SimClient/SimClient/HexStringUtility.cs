using System;
using System.Text;

namespace SimClient
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
    }
}
