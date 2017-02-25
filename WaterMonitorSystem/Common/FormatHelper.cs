using System;
using System.Text;

namespace Common
{
    public class FormatHelper
    {

        #region 格式化字符串
        /// <summary>
        /// 将字符数组装换成字符串
        /// </summary>
        /// <param name="array">数组</param>
        /// <param name="startIndex">开始位置</param>
        /// <param name="count">长度</param>
        /// <returns>拼接字符串</returns>
        public static string GetStringByArray(string[] array, int startIndex, int count)
        {
            int index = array.Length;

            if (startIndex < 0 || startIndex > index)
            {
                return string.Empty;
            }

            int endIndex = startIndex + count;
            if (endIndex < 0 || endIndex > index)
            {
                return string.Empty;
            }

            if (startIndex > endIndex)
            {
                return string.Empty;
            }

            string result = string.Empty;
            result = string.Join(" ", array, startIndex, count);
            return result;
        }

        /// <summary>
        /// 将BCDto日期
        /// </summary>
        /// <param name="now"></param>
        /// <returns></returns>
        public static DateTime GetDateTimeByBCD(string now)
        {
            string[] tempNow = now.TrimEnd(' ').Split(' ');

            string dt = string.Format("{6}{0}-{1}-{2} {3}:{4}:{5}", tempNow[0], tempNow[1], tempNow[2], tempNow[3], tempNow[4], tempNow[5], DateTime.Now.Year.ToString().Substring(0, 2));

            DateTime result;

            bool done = DateTime.TryParse(dt, out result);
            if (done)
            {
                return result;
            }
            return DateTime.Now;
        }


        ///// <summary>
        ///// 返回格式化的日期时间字符串
        ///// </summary>
        ///// <param name="current"></param>
        ///// <returns></returns>
        //public static string GetBCDDateTime(DateTime current)
        //{
        //    string result = current.ToString("yy MM dd HH mm ss");
        //    return result;
        //}
        /// <summary>
        /// 返回格式化的日期时间字符串
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static byte[] GetBCDDateTime(DateTime dateTime)
        {
            byte[] ret = new byte[6];


            ret[0] = (byte)ToBCD(dateTime.Year - 2000);
            ret[1] = (byte)ToBCD(dateTime.Month);
            ret[2] = (byte)ToBCD(dateTime.Day);

            ret[3] = (byte)ToBCD(dateTime.Hour);
            ret[4] = (byte)ToBCD(dateTime.Minute);
            ret[5] = (byte)ToBCD(dateTime.Second);


            return ret;
        }

        /// <summary>
        /// 将字符串转换成对应的数值/100
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static decimal GetValueByString(string value)
        {
            string tempValue = value.Replace(" ", "");
            return decimal.Divide(decimal.Parse(tempValue), 100);
        }

        /// <summary>
        /// 将字串转换为Unicode字串
        /// </summary>
        /// <param name="value">需要转换的字串</param>
        /// <returns>转换后的字串</returns>
        public static string StringToUnicodeString(string value)
        {
            string unicode = "";

            for (int i = 0; i < value.Length; i++)
            {
                int code = (int)value[i];
                // Console.Write("{0:x} ", (int)value[i]);
                if (code < 255)
                {
                    unicode += String.Format("{0:x2} 00", code);
                }
                else
                {
                    unicode += string.Format("{0:x2} {1:x2} ", (byte)(code & 0x000000FF), (byte)((code & 0x0000FF00) >> 8));
                }
            }
            return unicode;
        }
        /// <summary>
        /// 将字串转换为Unicode数组
        /// </summary>
        /// <param name="value">需要转换的字串</param>
        /// <returns>转换后的字串数组</returns>
        public static byte[] StringToUnicodeByte(string value)
        {

            return HexStringToByteArray(StringToUnicodeString(value));
        }

        /// <summary>
        /// 返回对应的整型字符串
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static string BinaryToIntString(int i)
        {
            string result = string.Empty;
            if (i < 0 || i > 999999)
            {
                return "00 00 00";
            }
            else if (i < 100)
            {
                return string.Format("00 00 {0}", i.ToString("D2"));
            }
            else if (i >= 100 && i < 1000)
            {
                return string.Format("00 0{0} {1}", i.ToString().Substring(0, 1), i.ToString().Substring(1, 2));
            }
            else if (i >= 1000 && i < 10000)
            {
                return string.Format("00 {0} {1}", i.ToString().Substring(0, 2), i.ToString().Substring(2, 2));
            }
            else if (i >= 10000 && i < 100000)
            {
                return string.Format("0{0} {1} {2}", i.ToString().Substring(0, 1), i.ToString().Substring(1, 2), i.ToString().Substring(3, 2));
            }
            else if (i >= 100000 && i < 1000000)
            {
                return string.Format("{0} {1} {2}", i.ToString().Substring(0, 2), i.ToString().Substring(2, 2), i.ToString().Substring(4, 2));
            }
            return "00 00";
        }

        /// <summary>
        /// 整形格式化为两位小数
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string IntFormatString(int value)
        {
            return value.ToString("D2");
        }

        /// <summary>
        /// 将国标定义的GBK 字节流转换成字串
        /// </summary>
        /// <param name="data">需要转换的字串</param>
        /// <param name="start">起始位置</param>
        /// <param name="length">取的字串长度</param>
        /// <returns></returns>
        public static string GetGBKStringFromBytes(byte[] data, int start, int length)
        {
            string gbk = "";

            try
            {
                gbk = Encoding.GetEncoding("GBK").GetString(data, start, length);
            }
            catch
            {
            }
            return gbk;
        }

        /// <summary>
        /// 将国标定义的GBK 字节流转换成字串
        /// </summary>
        /// <param name="data">需要转换的字串</param>
        /// <param name="start">起始位置</param>
        /// <param name="bytesLength">包函gbk字串的字节流长度</param>
        /// <returns>转换后的字串</returns>
        public static string GetGBKStringFromBytes(byte[] data, int start, out int bytesLength)
        {
            int length = 0;
            byte[] _tmp = new byte[2048];
            string _str = "";

            while (((length + start) < data.Length) && (data[length + start] != '\0') && (length < _tmp.Length))
            {
                _tmp[length] = data[length + start];
                length++;
            }
            _str = Encoding.GetEncoding("GBK").GetString(_tmp, 0, length);


            bytesLength = length + 1;

            return _str;
        }
        /// <summary>
        /// 从指定位置开始到\0结束的字串
        /// </summary>
        /// <param name="data">需要转换的数组</param>
        /// <param name="start">起始位置</param>
        /// <param name="bytesLength">数据长度</param>
        /// <returns>转换后的字串</returns>
        public static string GetAsciiStringFromBytes(byte[] data, int start, out int bytesLength)
        {
            int length = 0;
            byte[] _tmp = new byte[data.Length];
            string _str = "";

            while (((length + start) < data.Length) && (data[length + start] != '\0'))
            {
                _tmp[length] = data[length + start];
                length++;
            }
            _str = Encoding.ASCII.GetString(_tmp, 0, length);

            bytesLength = length;
            return _str;
        }

        /// <summary>
        /// 将国标定义的GBK 字节流转换成字串
        /// </summary>
        /// <param name="data">需要转换的字串</param>
        /// <param name="start">起始位置</param>
        /// <returns>转换后的字串</returns>
        public static string GetGBKStringFromBytes(byte[] data, int start)
        {
            int temp = 0;

            return GetGBKStringFromBytes(data, start, out temp);
        }

        /// <summary>
        /// 将GBK字串转换成字或流
        /// </summary>
        /// <param name="gbkString">GBK字串</param>
        /// <returns>转换后的数据</returns>
        public static byte[] GetBytesFromGBKString(string gbkString)
        {

            //byte[] ret;
            //byte[] gbk = Encoding.GetEncoding("GBK").GetBytes(gbkString);

            //ret = new byte[gbk.Length + 1];

            //Array.Copy(gbk, 0, ret, 0, gbk.Length);

            //return ret;
            return Encoding.GetEncoding("GBK").GetBytes(gbkString);
        }
        /// <summary>
        /// 将ascii字串转换成字或流
        /// </summary>
        /// <param name="asciiString">GBK字串</param>
        /// <returns>转换后的数据</returns>
        public static byte[] GetBytesFromAsciiString(string asciiString)
        {
            return ASCIIEncoding.Default.GetBytes(asciiString);
        }

        #endregion

        #region 校验码计算的方法

        /// <summary>
        /// 计算指定16进字命令的异或值
        /// </summary>
        /// <param name="cmd">被计算的命令</param>
        /// <param name="start">起始位置</param>
        /// <param name="count">长度</param>
        /// <returns>异或值</returns>
        public static byte GetXorByte(byte[] cmd, int start, int count)
        {
            byte ret = 0x00;
            int i = 1;

            if (cmd.Length > start)
            {
                ret = cmd[start];

                //校验长度合法性
                if ((start + count) <= cmd.Length)
                {
                    while (i < count)
                    {
                        ret = (byte)(ret ^ cmd[start + i]);
                        i++;
                    }
                }

            }


            return ret;
        }


        /// <summary>
        /// 求和
        /// </summary>
        /// <param name="src"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static byte CheckSum(byte[] src, int startIndex, int length)
        {
            byte ret = 0x00;
            int i = 0;

            while ((i < length) && ((startIndex + i) < src.Length))
            {
                ret = (byte)(ret + src[startIndex + i]);
                i++;
            }


            return ret;
        }

        /* 
        
        Data:需要计算的数据指针
        Len: 需要计算的长度
        */
        /// <summary>
        /// 计算一组数据的CRC16校验码
        /// </summary>
        /// <param name="Data">需要校验的数据</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="Len">计算长度</param>
        /// <returns>CRC16值</returns>
        public static UInt16 CRC16(byte[] Data, int startIndex, int Len)
        {
            int b = startIndex;

            UInt16 crc_initial_value = 0xFFFF;
            byte i = 0;

            UInt16[] crc_ta = {
                0, 0xc1c0, 0x81c1, 0x4001, 0x1c3, 0xc003, 0x8002, 0x41c2, 0x1c6, 0xc006, 0x8007, 0x41c7, 5, 0xc1c5, 0x81c4, 0x4004,
                460, 0xc00c, 0x800d, 0x41cd, 15, 0xc1cf, 0x81ce, 0x400e, 10, 0xc1ca, 0x81cb, 0x400b, 0x1c9, 0xc009, 0x8008, 0x41c8,
                0x1d8, 0xc018, 0x8019, 0x41d9, 0x1b, 0xc1db, 0x81da, 0x401a, 30, 0xc1de, 0x81df, 0x401f, 0x1dd, 0xc01d, 0x801c, 0x41dc,
                20, 0xc1d4, 0x81d5, 0x4015, 0x1d7, 0xc017, 0x8016, 0x41d6, 0x1d2, 0xc012, 0x8013, 0x41d3, 0x11, 0xc1d1, 0x81d0, 0x4010,
                0x1f0, 0xc030, 0x8031, 0x41f1, 0x33, 0xc1f3, 0x81f2, 0x4032, 0x36, 0xc1f6, 0x81f7, 0x4037, 0x1f5, 0xc035, 0x8034, 0x41f4,
                60, 0xc1fc, 0x81fd, 0x403d, 0x1ff, 0xc03f, 0x803e, 0x41fe, 0x1fa, 0xc03a, 0x803b, 0x41fb, 0x39, 0xc1f9, 0x81f8, 0x4038,
                40, 0xc1e8, 0x81e9, 0x4029, 0x1eb, 0xc02b, 0x802a, 0x41ea, 0x1ee, 0xc02e, 0x802f, 0x41ef, 0x2d, 0xc1ed, 0x81ec, 0x402c,
                0x1e4, 0xc024, 0x8025, 0x41e5, 0x27, 0xc1e7, 0x81e6, 0x4026, 0x22, 0xc1e2, 0x81e3, 0x4023, 0x1e1, 0xc021, 0x8020, 0x41e0,
                0x1a0, 0xc060, 0x8061, 0x41a1, 0x63, 0xc1a3, 0x81a2, 0x4062, 0x66, 0xc1a6, 0x81a7, 0x4067, 0x1a5, 0xc065, 0x8064, 0x41a4,
                0x6c, 0xc1ac, 0x81ad, 0x406d, 0x1af, 0xc06f, 0x806e, 0x41ae, 0x1aa, 0xc06a, 0x806b, 0x41ab, 0x69, 0xc1a9, 0x81a8, 0x4068,
                120, 0xc1b8, 0x81b9, 0x4079, 0x1bb, 0xc07b, 0x807a, 0x41ba, 0x1be, 0xc07e, 0x807f, 0x41bf, 0x7d, 0xc1bd, 0x81bc, 0x407c,
                0x1b4, 0xc074, 0x8075, 0x41b5, 0x77, 0xc1b7, 0x81b6, 0x4076, 0x72, 0xc1b2, 0x81b3, 0x4073, 0x1b1, 0xc071, 0x8070, 0x41b0,
                80, 0xc190, 0x8191, 0x4051, 0x193, 0xc053, 0x8052, 0x4192, 0x196, 0xc056, 0x8057, 0x4197, 0x55, 0xc195, 0x8194, 0x4054,
                0x19c, 0xc05c, 0x805d, 0x419d, 0x5f, 0xc19f, 0x819e, 0x405e, 90, 0xc19a, 0x819b, 0x405b, 0x199, 0xc059, 0x8058, 0x4198,
                0x188, 0xc048, 0x8049, 0x4189, 0x4b, 0xc18b, 0x818a, 0x404a, 0x4e, 0xc18e, 0x818f, 0x404f, 0x18d, 0xc04d, 0x804c, 0x418c,
                0x44, 0xc184, 0x8185, 0x4045, 0x187, 0xc047, 0x8046, 0x4186, 0x182, 0xc042, 0x8043, 0x4183, 0x41, 0xc181, 0x8180, 0x4040
             };

            //        UInt16[] crc_ta ={
            //                            0x0000, 0x1021, 0x2042, 0x3063, 0x4084, 0x50a5, 0x60c6, 0x70e7,
            //0x8108, 0x9129, 0xa14a, 0xb16b, 0xc18c, 0xd1ad, 0xe1ce, 0xf1ef,
            //0x1231, 0x0210, 0x3273, 0x2252, 0x52b5, 0x4294, 0x72f7, 0x62d6,
            //0x9339, 0x8318, 0xb37b, 0xa35a, 0xd3bd, 0xc39c, 0xf3ff, 0xe3de,
            //0x2462, 0x3443, 0x0420, 0x1401, 0x64e6, 0x74c7, 0x44a4, 0x5485,
            //0xa56a, 0xb54b, 0x8528, 0x9509, 0xe5ee, 0xf5cf, 0xc5ac, 0xd58d,
            //0x3653, 0x2672, 0x1611, 0x0630, 0x76d7, 0x66f6, 0x5695, 0x46b4,
            //0xb75b, 0xa77a, 0x9719, 0x8738, 0xf7df, 0xe7fe, 0xd79d, 0xc7bc,
            //0x48c4, 0x58e5, 0x6886, 0x78a7, 0x0840, 0x1861, 0x2802, 0x3823,
            //0xc9cc, 0xd9ed, 0xe98e, 0xf9af, 0x8948, 0x9969, 0xa90a, 0xb92b,
            //0x5af5, 0x4ad4, 0x7ab7, 0x6a96, 0x1a71, 0x0a50, 0x3a33, 0x2a12,
            //0xdbfd, 0xcbdc, 0xfbbf, 0xeb9e, 0x9b79, 0x8b58, 0xbb3b, 0xab1a,
            //0x6ca6, 0x7c87, 0x4ce4, 0x5cc5, 0x2c22, 0x3c03, 0x0c60, 0x1c41,
            //0xedae, 0xfd8f, 0xcdec, 0xddcd, 0xad2a, 0xbd0b, 0x8d68, 0x9d49,
            //0x7e97, 0x6eb6, 0x5ed5, 0x4ef4, 0x3e13, 0x2e32, 0x1e51, 0x0e70,
            //0xff9f, 0xefbe, 0xdfdd, 0xcffc, 0xbf1b, 0xaf3a, 0x9f59, 0x8f78,
            //0x9188, 0x81a9, 0xb1ca, 0xa1eb, 0xd10c, 0xc12d, 0xf14e, 0xe16f,
            //0x1080, 0x00a1, 0x30c2, 0x20e3, 0x5004, 0x4025, 0x7046, 0x6067,
            //0x83b9, 0x9398, 0xa3fb, 0xb3da, 0xc33d, 0xd31c, 0xe37f, 0xf35e,
            //0x02b1, 0x1290, 0x22f3, 0x32d2, 0x4235, 0x5214, 0x6277, 0x7256,
            //0xb5ea, 0xa5cb, 0x95a8, 0x8589, 0xf56e, 0xe54f, 0xd52c, 0xc50d,
            //0x34e2, 0x24c3, 0x14a0, 0x0481, 0x7466, 0x6447, 0x5424, 0x4405,
            //0xa7db, 0xb7fa, 0x8799, 0x97b8, 0xe75f, 0xf77e, 0xc71d, 0xd73c,
            //0x26d3, 0x36f2, 0x0691, 0x16b0, 0x6657, 0x7676, 0x4615, 0x5634,
            //0xd94c, 0xc96d, 0xf90e, 0xe92f, 0x99c8, 0x89e9, 0xb98a, 0xa9ab,
            //0x5844, 0x4865, 0x7806, 0x6827, 0x18c0, 0x08e1, 0x3882, 0x28a3,
            //0xcb7d, 0xdb5c, 0xeb3f, 0xfb1e, 0x8bf9, 0x9bd8, 0xabbb, 0xbb9a,
            //0x4a75, 0x5a54, 0x6a37, 0x7a16, 0x0af1, 0x1ad0, 0x2ab3, 0x3a92,
            //0xfd2e, 0xed0f, 0xdd6c, 0xcd4d, 0xbdaa, 0xad8b, 0x9de8, 0x8dc9,
            //0x7c26, 0x6c07, 0x5c64, 0x4c45, 0x3ca2, 0x2c83, 0x1ce0, 0x0cc1,
            //0xef1f, 0xff3e, 0xcf5d, 0xdf7c, 0xaf9b, 0xbfba, 0x8fd9, 0x9ff8,
            //0x6e17, 0x7e36, 0x4e55, 0x5e74, 0x2e93, 0x3eb2, 0x0ed1, 0x1ef0
            //                      };


            while (Len-- != startIndex)
            {
                i = (byte)(crc_initial_value / 0x100);
                crc_initial_value = (UInt16)(crc_initial_value << 8);
                crc_initial_value = (UInt16)(crc_initial_value ^ crc_ta[i ^ Data[b]]);
                b++;
            }

            return crc_initial_value;

        }




        #endregion

        #region 进制转换
        /// <summary>
        /// 整形转化成字节
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static byte IntToByte(int i)
        {
            return Convert.ToByte(i % 0x100);
        }

        /// <summary>
        /// 将一个字节流转换成SQL数据库的BIGINT数据类型
        /// </summary>
        /// <param name="src"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static long ByteArrayToBigInt_BigMode(byte[] src, int startIndex, int length)
        {
            long ret = 0;

            ret = src[startIndex];

            for (int i = 1; i < length; i++)
            {
                ret = (ret << 8) | src[startIndex + i];
            }

            return ret;
        }

        /// <summary>
        /// 将手机的BCD转换成手机号
        /// </summary>
        /// <param name="PhoneBCD">6Byte手机bcd</param>
        /// <returns></returns>
        public static string BCDToPhone(byte[] PhoneBCD)
        {
            int i = 0;
            StringBuilder sb = new StringBuilder();



            //转换数据
            while ((i < 6) && (i < PhoneBCD.Length))
            {
                if ((i < PhoneBCD.Length))
                {
                    sb.Append(String.Format("{0:d2}", ((PhoneBCD[i] & 0xF0) >> 4) | (PhoneBCD[i] & 0x0F)));

                }

                i++;
            }

            return sb.ToString();
        }

        /// <summary>
        /// BCD转成字串
        /// </summary>
        /// <param name="bcd"></param>
        /// <returns></returns>
        public static string BCDToString(byte[] bcd)
        {
            return BCDToString(bcd, 0, bcd.Length);
        }

        /// <summary>
        /// phone to bcd
        /// </summary>
        /// <param name="deviceNO"></param>
        /// <returns></returns>
        public static byte[] GetPhoneBCD(string phoneNo)
        {
            byte[] bcd = new byte[6];
            int i = 0;
            string s;
            byte temp;
            int index = 0;

            while (phoneNo.Length < 12)
            {
                phoneNo = "0" + phoneNo;
            }

            while (i < 6)
            {
                s = phoneNo.Substring(index, 1);
                temp = byte.Parse(s);

                bcd[i] = temp;

                s = phoneNo.Substring(index + 1, 1);
                temp = byte.Parse(s);

                bcd[i] = (byte)((bcd[i] << 4) | temp);

                i = i + 1;
                index = index + 2;
            }


            return bcd;
        }


        /// <summary>
        /// 将设备ID从字符转换为长整型
        /// </summary>
        /// <param name="deviceNO"></param>
        /// <returns></returns>
        public static long GetDeviceIDFromString(string deviceNO)
        {
            long ret = 0;

            long.TryParse(deviceNO, out ret);

            return ret;
        }

        /// <summary>
        /// BCD转成字串
        /// </summary>
        /// <param name="bcd"></param>
        /// <param name="count">长度</param>
        /// <param name="start">起始位置</param>
        /// <returns></returns>
        public static string BCDToString(byte[] bcd, int start, int count)
        {
            int i = 0;
            string _ret = "";

            //超标
            if ((start + count) > bcd.Length)
            {
                return "";
            }


            //转换数据
            while (i < count)
            {
                _ret = _ret + String.Format("{0:X2}", bcd[i + start]);

                i++;
            }

            return _ret;
        }

        /// <summary>
        /// BCD字串到BCD数组
        /// </summary>
        /// <param name="bcd"></param>
        /// <returns></returns>
        public static byte[] BCDStringToBCDByteArray(string bcd)
        {
            if ((bcd.Length % 2) != 0) return new byte[0];

            return HexStringToByteArray(bcd);
        }


        /// <summary>
        /// 将用BCD表示的字节直接转成10进制数
        /// </summary>
        /// <param name="bcd"></param>
        /// <returns></returns>
        public static int BCDByteToInt(byte bcd)
        {
            string str = string.Format("{0:X2}", bcd);
            int ret = 0;

            int.TryParse(str, out ret);
            return ret;


        }

        /// <summary>
        /// 将BCD字串转换为BCD长整型
        /// </summary>
        /// <param name="bcdString"></param>
        /// <returns></returns>
        public static long BCDStringToLong(string bcdString)
        {
            long ret = 0;

            long.TryParse(bcdString, out ret);

            return ret;
        }

        /// <summary>
        /// 将BCD数组转换为长整型
        /// </summary>
        /// <param name="bcd"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static long BCDByteArrayToLong(byte[] bcd, int start, int length)
        {
            return BCDStringToLong(BCDToString(bcd, start, length));
        }
        /// <summary>
        /// 将BCD的时间转成日期时间格式
        /// </summary>
        /// <param name="bcd"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public static DateTime BCDToDateTime(byte[] bcd, int start)
        {
            DateTime d;

            d = DateTime.Parse(string.Format("20{0:d2}-{1:d2}-{2:d2} {3:d2}:{4:d2}:{5:d2}",
                BCDByteToInt(bcd[start]),
                BCDByteToInt(bcd[start + 1]),
                BCDByteToInt(bcd[start + 2]),
                BCDByteToInt(bcd[start + 3]),
                BCDByteToInt(bcd[start + 4]),
                BCDByteToInt(bcd[start + 5])
                ));




            return d;
        }

        /// <summary>
        /// 16进制转化成BCD
        /// </summary>
        /// <param name="b">byte</param>
        /// <returns>返回BCD码</returns>
        public static byte HexToBCD(byte b)
        {
            int r1 = b % 0x10;     // b = 0x12 -> r1 = 0x02
            int r2 = b / 0x10;     // b = 0x12 -> r2 = 0x01
            if (r1 > 9)             //r1 = 0x0A -> r1 = 0x00
                r1 = 0;
            if (r2 > 9)             //r2 = 0x0A -> r2 = 0x00
                r2 = 0;
            return Convert.ToByte(r1 + 10 * r2);    //0x12 -> 12
        }

        /// <summary>
        /// 16进制转化成BCD
        /// </summary>
        /// <param name="i">int</param>
        /// <returns>返回BCD码</returns>
        public static byte HexToBCD(int i)
        {
            return HexToBCD(IntToByte(i));
        }

        /// <summary>
        /// Double转换为压缩BCD
        /// </summary>
        /// <param name="d">0-100内的双精度浮点数字</param>
        /// <returns>返回压缩BCD码</returns>
        public static string DoubleToBCD(double d)
        {
            string[] strs = d.ToString("F2").Split('.');
            string temp1 = int.Parse(strs[0]).ToString("D2");
            string temp2 = int.Parse(strs[1]).ToString("D2");
            return string.Format("{0} {1}", temp1, temp2);
        }

        /// <summary>
        /// long转换为BCD
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static long ToBCD(long val)
        {
            long res = 0;
            int bit = 0;

            while (val >= 10)
            {
                res |= (val % 10 << bit);
                val /= 10;
                bit += 4;
            }
            res |= val << bit;
            return res;
        }

        /// <summary>
        /// BCD转换为long
        /// </summary>
        /// <param name="vals"></param>
        /// <returns></returns>
        public static long FromBCD(long vals)
        {
            long c = 1;
            long b = 0;
            while (vals > 0)
            {
                b += ((vals & 0xf) * c);
                c *= 10;
                vals >>= 4;
            }
            return b;
        }

        /// <summary>
        /// BCD转化成16进制
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static byte BCDToHex(byte b)
        {
            int r2 = b % 100;      // b = 12 -> r2 = 12        //123 -> r2 = 23
            int r1 = r2 % 10;      //r2 = 12 -> r1 = 2     
            r2 = r2 / 10;           //r2 = 12 -> r2 =1
            return Convert.ToByte(r1 + 0x10 * r2);
        }

        /// <summary>
        /// BCD转化成16进制
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static byte BCDToHex(int i)
        {
            return BCDToHex(IntToByte(i));
        }

        /// <summary>
        /// btye转化成16进制字符
        /// </summary>
        /// <param name="b">byte</param>
        /// <returns></returns>
        public static string ToHexString(byte b)
        {
            return Convert.ToString(b, 16);
        }

        /// <summary>
        /// int转化成16进制字符
        /// </summary>
        /// <param name="num">int</param>
        /// <returns></returns>
        public static string ToHexString(int num)
        {
            return Convert.ToString(num, 16);
        }

        /// <summary>
        /// 16进制字符串转换成字节数组
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static byte[] HexStringToByteArray(string s)
        {
            s = s.Replace(" ", "");
            byte[] buffer = new byte[s.Length / 2];
            for (int i = 0; i < s.Length; i += 2)
            {
                buffer[i / 2] = (byte)Convert.ToByte(s.Substring(i, 2), 16);
            }
            return buffer;
        }

        /// <summary>
        /// 16进制字串转成32位无符号整型 大端
        /// </summary>
        /// <param name="s"></param>
        /// <param name="value"></param>
        /// <returns>true:成功 false:失败</returns>
        public static bool HexStringToUint32_Big(string s, out UInt32 value)
        {
            byte[] temp;
            int length = 4;

            value = 0x00;


            //转换成字节
            try
            {
                temp = HexStringToByteArray(s);
            }
            catch
            {
                return false;
            }

            length = temp.Length;

            if (length <= 0)
            {
                return false;
            }
            if (length > 4) length = 4;

            for (int i = 0; i < length; i++)
            {
                value = (value << (i * 8)) | temp[i];
            }



            return true;
        }

        /// <summary> 
        /// 字节数组转换成16进制字符串
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ByteArrayToHexString(byte[] data)
        {
            int i = 0;
            if (data == null) return "";
            StringBuilder sb = new StringBuilder(data.Length * 3);
            foreach (byte b in data)
            {
                i++;
                sb.Append(Convert.ToString(b, 16).PadLeft(2, '0').PadRight(3, ' '));

                if (i > 10240) break;
            }
            return sb.ToString().Trim().ToUpper();
        }
        /// <summary> 
        /// 字节数组转换成16进制字符串
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ByteArrayToHexString(byte[] data, int length)
        {
            StringBuilder sb = new StringBuilder();

            if (length > 10240) length = 10240;

            foreach (byte b in data)
            {

                length--;
                sb.Append(Convert.ToString(b, 16).PadLeft(2, '0').PadRight(3, ' '));

                if (length <= 0)
                {
                    break;
                }
            }
            return sb.ToString().Trim().ToUpper();
        }

        /// <summary> 
        /// 字节数组转换成16进制字符串
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ByteArrayToHexString(byte[] data, int startIndex, int length)
        {
            StringBuilder sb = new StringBuilder();

            if (length > 10240) length = 10240;

            for (int i = 0; i < length; i++)
            {
                sb.Append(Convert.ToString(data[startIndex + i], 16).PadLeft(2, '0').PadRight(3, ' '));

            }
            return sb.ToString().Trim().ToUpper();
        }


        /// <summary>
        /// ASCII转16进制字串
        /// </summary>
        /// <param name="ascii">ASCII</param>
        /// <returns></returns>
        public static string ASCIIToHexString(string ascii)
        {
            char[] cs = ascii.ToCharArray();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < cs.Length; i++)
            {
                string hex = ((int)cs[i]).ToString("X");
                sb.AppendFormat("{0} ", hex);
            }
            return sb.ToString().TrimEnd(' ');
        }

        /// <summary>
        /// ASCII 转16进制数组
        /// </summary>
        /// <param name="ascii">需要转换的ascii</param>
        /// <returns>转换后的byte数组</returns>
        public static byte[] ASCIIToHex(string ascii)
        {
            return ASCIIEncoding.ASCII.GetBytes(ascii);
        }

        /// <summary>
        /// HEX to ASCII
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static string HexToASCII(string hex)
        {
            if (hex.Length % 2 == 0)
            {
                int iValue;
                byte[] bs;
                string sValue = string.Empty;
                string result = string.Empty;
                int length = hex.Length / 2;

                for (int i = 0; i < length; i++)
                {
                    iValue = Convert.ToInt32(hex.Substring(i * 2, 2), 16); // 16进制->10进制
                    bs = System.BitConverter.GetBytes(iValue); //int->byte[]
                    sValue = System.Text.Encoding.ASCII.GetString(bs, 0, 1);  //byte[]-> ASCII
                    result += char.Parse(sValue).ToString();
                }
                return result.PadLeft(length, '0');
            }
            return string.Empty;
        }

        /// <summary>
        /// ASCII To Float
        /// </summary>
        /// <param name="ascii"></param>
        /// <returns></returns>
        public static float ASCIIToFloat(string ascii)
        {
            if (ascii.Length == 8)
            {
                byte[] arr = new byte[4];
                for (int i = 0; i < 4; i++)
                {

                    arr[i] = Convert.ToByte(ascii.Substring((3 - i) * 2, 2), 16);
                }
                float f = BitConverter.ToSingle(arr, 0);
                return f;
            }
            return 0f;
        }

        /// <summary>
        /// Hex to Float
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static float HexToFloat(string hex)
        {
            string ascii = HexToASCII(hex);
            float f = ASCIIToFloat(ascii);
            return f;
        }
        #endregion

        #region 其它辅助函数

        /// <summary>
        /// 将设备id转换为符合要求的设备字串
        /// </summary>
        /// <param name="deviceNO"></param>
        /// <returns></returns>
        public static long StringToDeviceID(string deviceNO)
        {


            return long.Parse(deviceNO);
        }

        /// <summary>
        /// 将设备ID的整型转换为字符型
        /// </summary>
        /// <param name="deviceNO"></param>
        /// <returns></returns>
        public static string GetDeviceID(long deviceNO)
        {
            return string.Format("{0:d14}", deviceNO);
        }





        /// <summary>
        /// 将16位无符号整数转换为字节数组，大端模式
        /// </summary>
        /// <param name="dest">存放数据的数组</param>
        /// <param name="id">被转换的值</param>
        /// <param name="startIndex">起存放起始位置</param>
        public static void UINT16ToByteArray_BigMode(byte[] dest, ushort id, int startIndex)
        {
            dest[startIndex] = (byte)((id & 0xFF00) >> 8);
            dest[startIndex + 1] = (byte)(id & 0x00FF);
        }
        /// <summary>
        /// 将16位有符号整数转换为字节数组，大端模式
        /// </summary>
        /// <param name="dest">存放数据的数组</param>
        /// <param name="id">被转换的值</param>
        /// <param name="startIndex">起存放起始位置</param>
        public static void INT16ToByteArray_BigMode(byte[] dest, int id, int startIndex)
        {
            dest[startIndex] = (byte)((id & 0xFF00) >> 8);
            dest[startIndex + 1] = (byte)(id & 0x00FF);
        }


        /// <summary>
        /// 将32位无符号整数转换成字节流
        /// </summary>
        /// <param name="dest"></param>
        /// <param name="value"></param>
        /// <param name="startIndex"></param>
        public static void UINT32ToByteArray_BigMode(byte[] dest, uint value, int startIndex)
        {
            if (dest.Length < (startIndex + 4)) return;

            dest[startIndex] = (byte)((value & 0xFF000000) >> 24);
            dest[startIndex + 1] = (byte)((value & 0x00FF0000) >> 16);
            dest[startIndex + 2] = (byte)((value & 0x0000FF00) >> 8);
            dest[startIndex + 3] = (byte)(value & 0x000000FF);
        }
        /// <summary>
        /// 将长整型转换成字节流,32位
        /// </summary>
        /// <param name="dest"></param>
        /// <param name="value"></param>
        /// <param name="startIndex"></param>
        public static void LongToByteArray_BigMode(byte[] dest, long value, int startIndex)
        {
            int i = dest.Length;

            //64位
            if (i > 8) i = 8;

            if (i < 1) return;

            i = i - 1;

            while (i >= 0)
            {
                dest[dest.Length - i - 1] = (byte)(value >> (i * 8));

                i--;
            }

            //dest[startIndex] = (byte)((value & 0xFF000000) >> 24);
            //dest[startIndex + 1] = (byte)((value & 0x00FF0000) >> 16);
            //dest[startIndex + 2] = (byte)((value & 0x0000FF00) >> 8);
            //dest[startIndex + 3] = (byte)(value & 0x000000FF);
        }

        /// <summary>
        /// 将字节流转换成32位无符号整数
        /// </summary>
        /// <param name="data"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public static uint ByteArrayToUINT32_BigMode(byte[] data, int startIndex)
        {
            uint value = 0;

            if (data.Length < (startIndex + 4)) return 0;


            value = data[startIndex];

            value = (value << 8) | data[startIndex + 1];
            value = (value << 8) | data[startIndex + 2];
            value = (value << 8) | data[startIndex + 3];

            return value;

        }


        /// <summary>
        /// 将字节转换成16位无符号整数
        /// </summary>
        /// <param name="data"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public static ushort ByteArrayToUINT16_BigMode(byte[] data, int startIndex)
        {
            ushort tmp = 0;

            if ((data.Length - startIndex) < 2)
            {
                return 0;
            }

            tmp = data[startIndex];

            tmp = (ushort)((tmp << 8) | data[startIndex + 1]);

            return tmp;
        }

        /// <summary>
        /// 将字节转换成16位有符号整数
        /// </summary>
        /// <param name="data"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public static int ByteArrayToINT16_BigMode(byte[] data, int startIndex)
        {
            int tmp = 0;

            if ((data.Length - startIndex) < 2)
            {
                return 0;
            }

            tmp = data[startIndex];

            tmp = (ushort)((tmp << 8) | data[startIndex + 1]);

            return tmp;
        }

        /// <summary>
        /// 将4字节无符号整数转换为小端对齐的数据
        /// </summary>
        /// <param name="dest">存储数组</param>
        /// <param name="id">需要转换的4字节整数</param>
        /// <param name="startIndex">存储的起始位置</param>
        public static void Int32ToByteArray_LitMode(ref byte[] dest, int id, int startIndex)
        {
            dest[startIndex] = (byte)(id & 0x000000FF);
            dest[startIndex + 1] = (byte)((id & 0x0000FF00) >> 8);
            dest[startIndex + 2] = (byte)((id & 0x00FF0000) >> 16);
            dest[startIndex + 3] = (byte)((id & 0xFF000000) >> 24);

        }

        /// <summary>
        /// 将字节数组转换为 Int32
        /// </summary>
        /// <param name="source"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public static int ByteArrayToInt32(byte[] source, int start)
        {
            int ret = 0;

            ret = (0x000000FF & source[3 + start]) << 24;
            ret |= (0x000000FF & source[2 + start]) << 16;
            ret |= (0x000000FF & source[1 + start]) << 8;
            ret |= (0x000000FF & source[start]);

            return ret;
        }
        #endregion

        #region 进制转换整形相关
        //long类型转成byte数组 
        public static byte[] longToByte(long number)
        {
            BitConverter.GetBytes(1000);

            long temp = number;
            byte[] b = new byte[8];
            for (int i = 0; i < b.Length; i++)
            {
                b[i] = (byte)(temp & 0xff);//将最低位保存在最低位
                temp = temp >> 8; // 向右移8位 
            }
            Array.Reverse(b);
            return b;
        }

        //byte数组转成long 
        public static long byteToLong(byte[] b)
        {
            long s = 0;
            long s0 = b[0] & 0xff;// 最低位 
            long s1 = b[1] & 0xff;
            long s2 = b[2] & 0xff;
            long s3 = b[3] & 0xff;
            long s4 = b[4] & 0xff;
            long s5 = b[5] & 0xff;
            long s6 = b[6] & 0xff;
            long s7 = b[7] & 0xff;

            // s0不变 
            s1 <<= 8;
            s2 <<= 16;
            s3 <<= 24;
            s4 <<= 8 * 4;
            s5 <<= 8 * 5;
            s6 <<= 8 * 6;
            s7 <<= 8 * 7;
            s = s0 | s1 | s2 | s3 | s4 | s5 | s6 | s7;
            return s;
        }


        public static byte[] intToByte(int number)
        {
            int temp = number;
            byte[] b = new byte[4];
            for (int i = 0; i < b.Length; i++)
            {
                b[i] = (byte)(temp & 0xff);//将最低位保存在最低位
                temp = temp >> 8; // 向右移8位 
            }
            Array.Reverse(b);
            return b;
        }


        public static int byteToInt(byte[] b)
        {
            int s = 0;
            int s0 = b[0] & 0xff;// 最低位 
            int s1 = b[1] & 0xff;
            int s2 = b[2] & 0xff;
            int s3 = b[3] & 0xff;
            s3 <<= 24;
            s2 <<= 16;
            s1 <<= 8;
            s = s0 | s1 | s2 | s3;
            return s;
        }


        public static byte[] shortToByte(short number)
        {
            int temp = number;
            byte[] b = new byte[2];
            for (int i = 0; i < b.Length; i++)
            {
                b[i] = (byte)(temp & 0xff);//将最低位保存在最低位
                temp = temp >> 8; // 向右移8位 
            }
            Array.Reverse(b);
            return b;
        }


        public static short byteToShort(byte[] b)
        {
            short s = 0;
            short s0 = (short)(b[0] & 0xff);// 最低位 
            short s1 = (short)(b[1] & 0xff);
            s1 <<= 8;
            s = (short)(s0 | s1);
            return s;
        }
        #endregion
    }
}
