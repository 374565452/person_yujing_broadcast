using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Common
{
    public class Encode
    {

        public Encode() { }
        /// <summary>
        /// 返回字串的MD5散列 16位
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        /// 
        public static string GetMd5(string code)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            string t = BitConverter.ToString(md5.ComputeHash(UTF8Encoding.Default.GetBytes(code)), 4, 8);
            t = t.Replace("-", "");
            return t;
        }
        /// <summary>
        /// 返回MD5 32位
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string Md5(string code)
        {
            MD5 md5 = MD5.Create();
            string pwd = "";
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(code));
            for (int i = 0; i < s.Length; i++)
            {
                pwd += s[i].ToString("X");
            }
            return pwd;
        }
        /// <summary>
        /// Escape加密
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static String Escape(String s)
        {
            return Uri.EscapeDataString(s);
        }
        /// <summary>
        /// Base64 编码
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string EncodeBase64(string value)
        {
            return Convert.ToBase64String(System.Text.ASCIIEncoding.Default.GetBytes(value));
        }
        /// <summary>
        /// Base64 解码
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string DecodeBase64(string value)
        {
            try
            {
                return System.Text.ASCIIEncoding.Default.GetString(Convert.FromBase64String(value));
            }
            catch
            {
            }
            return "";
        }
        /// <summary>
        /// Eascape 解码
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static String Unescape(String s)
        {
            return Uri.UnescapeDataString(s);
        }

        /// <summary>
        /// 将输入转换为安全HTML
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string HTMLEncode(string str)
        {

            if (str == null || str == "")
                return "";
            str = str.Replace(">", "&gt;");
            str = str.Replace("<", "&lt;");
            str = str.Replace(" ", "&nbsp;");       // HttpUtility.HtmlEncode( 并不支持这个替换 
            str = str.Replace("  ", " &nbsp;");     // HttpUtility.HtmlEncode( 并不支持这个替换 
            str = str.Replace("\"", "&quot;");
            str = str.Replace("\'", "&#39;");
            str = str.Replace("\n", "<br/>");     // HttpUtility.HtmlEncode( 并不支持这个替换 
            return str;
        }
        public static string UnHTMLEncode(string str)
        {
            if (str == null || str == "")
                return "";
            str = str.Replace("&gt;", ">");
            str = str.Replace("&lt;", "<");
            str = str.Replace("&nbsp;", " ");       // HttpUtility.HtmlEncode( 并不支持这个替换 
            str = str.Replace(" &nbsp;", "  ");     // HttpUtility.HtmlEncode( 并不支持这个替换 
            str = str.Replace("&quot;", "\"");
            str = str.Replace("&#39;", "\'");
            str = str.Replace("<br/>", "\n");     // HttpUtility.HtmlEncode( 并不支持这个替换 
            return str;

            /*
            string ret = "";
            if (obj != null)
            {
                ret = obj.ToString();
                ret = ret.Replace("&amp;", "&");
                ret = ret.Replace("&nbsp;", " ").Replace("&lt;", "<");
                ret = ret.Replace("&gt;", ">");
                ret = ret.Replace("&quote;", "\"");
                ret = ret.Replace("<br>", "\r\n");
                ret = System.Web.HttpContext.Current.Server.HtmlDecode(ret);
            }
            return ret;
             * */
        }
        //获取截取后的字串
        public static string GetStr(object s, int l)
        {
            if (s == null) return "";

            return GetStr(s.ToString(), l);
        }
        public static string GetStr(string temp, int l)
        {

            if (Regex.Replace(temp, "[\u4e00-\u9fa5]", "zz", RegexOptions.IgnoreCase).Length <= l)
            {
                return temp;
            }
            for (int i = temp.Length; i >= 0; i--)
            {
                temp = temp.Substring(0, i);
                if (Regex.Replace(temp, "[\u4e00-\u9fa5]", "zz", RegexOptions.IgnoreCase).Length <= l - 3)
                {
                    return temp + "";
                }
            }
            return "";
        }

        /// <summary>
        /// 字符串截断工具
        /// </summary>
        /// <param name="str">需要截断的字串</param>
        /// <param name="Count">中英文混排时候显示的字符数值</param>
        /// <returns></returns>
        public static string StrLength(string str, int Count)
        {
            String Temp = "";
            byte[] b = Encoding.GetEncoding("gb2312").GetBytes(str);

            int index = 0;
            List<byte> nByte = new List<byte>();
            for (int i = 0; i < b.Length; )
            {
                if (i < Count)
                {
                    //GB2312编码从A1A0开始 由于一个汉字用双直接标识 所以每两个大于160（A0）的BYTE为一个字
                    if (b[i] >= 160)
                    {
                        nByte.Add(b[i]);
                        nByte.Add(b[i + 1]);
                        i += 2;
                        index++;
                    }
                    else
                    {
                        nByte.Add(b[i]);
                        i++;
                        index++;
                        //这里是英文以及半角符号判断区域
                    }
                }
                else
                {
                    break;
                }
            }
            Temp = Encoding.GetEncoding("gb2312").GetString(nByte.ToArray());

            if (nByte.Count < b.Length)
            {
                Temp += "...";
            }
            nByte.Clear();
            return Temp;
        }

        /// <summary>  
        /// 获取文件的MD5码  
        /// </summary>  
        /// <param name="fileName">传入的文件名（含路径及后缀名）</param>  
        /// <returns></returns>  
        public static string GetMD5HashFromFile(string fileName)
        {
            try
            {
                FileStream file = new FileStream(fileName, System.IO.FileMode.Open);
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
            }
        }  
    }
}
