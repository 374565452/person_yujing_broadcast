using Common;
using System.Text;

namespace CardOperationSystem
{
    class CardCommon
    {
        /// <summary>
        /// 读指定扇区数据块内容
        /// </summary>
        /// <param name="icdev">通讯设备标识符</param>
        /// <param name="sec">扇区</param>
        /// <param name="block">数据块</param>
        /// <returns></returns>
        public static string ReadIC(int icdev, int sec, int block)
        {
            string str = "";

            try
            {
                int i = 0;
                byte[] data = new byte[16];
                byte[] buff = new byte[32];

                for (i = 0; i < 16; i++)
                    data[i] = 0;
                for (i = 0; i < 32; i++)
                    buff[i] = 0;

                if (mifareone.rf_read(icdev, sec * 4 + block, data) == 0)
                {
                    common.hex_a(data, buff, 16);
                    str = System.Text.Encoding.ASCII.GetString(buff);
                }
                else
                {
                    str = "读数据失败！";
                }
            }
            catch
            {
                str = "读数据出错！";
            }

            return str;
        }

        /// <summary>
        /// 写指定扇区数据块内容
        /// </summary>
        /// <param name="icdev">通讯设备标识符</param>
        /// <param name="sec">扇区</param>
        /// <param name="block">数据块</param>
        /// <param name="data">要写入的数据</param>
        /// <returns></returns>
        public static string WriteIC(int icdev, int sec, int block, string data)
        {
            string temp = data.Trim();

            int i = 0;
            byte[] databuff = new byte[16];
            byte[] buff = new byte[32];

            if (temp.Length < 32)
            {
                return "请正确输入数据，数据长度不对！至少32位，多的位数自动截断！";
            }

            for (i = 0; i < data.Length; i++)
            {
                if (data[i] >= '0' && data[i] <= '9')
                    continue;
                if (data[i] <= 'a' && data[i] <= 'f')
                    continue;
                if (data[i] <= 'A' && data[i] <= 'F')
                    continue;
            }
            if (i != temp.Length)
            {
                return "数据必须为十六进制数！";
            }

            buff = Encoding.ASCII.GetBytes(data);
            common.a_hex(buff, databuff, 32);
            if (mifareone.rf_write(icdev, sec * 4 + block, databuff) == 0)
            {
                return "";
            }
            else
            {
                return "写数据失败！";
            }
        }

        /// <summary>
        /// 认证扇区密码
        /// </summary>
        /// <param name="icdev">通讯设备标识符</param>
        /// <param name="mode">模式</param>
        /// <param name="sec">扇区</param>
        /// <param name="key">密码</param>
        /// <returns></returns>
        public static string AuthIC(int icdev, int mode, int sec, string key)
        {
            byte[] key2 = new byte[7];
            common.a_hex(Encoding.ASCII.GetBytes(key), key2, 12);

            int i1 = common.rf_load_key(icdev, mode, sec, key2);
            int i2 = mifareone.rf_authentication(icdev, mode, sec);

            if (i1 == 0 && i2 == 0)
            {
                return InfoSys.StrAuthSuccess;
            }
            else
            {
                return InfoSys.StrAuthFailure + " " + i1 + "|" + i2;
            }
        }

        public static string WritePWD(int icdev, int sec, int block, string keyA, string keyControl, string keyB)
        {
            return CardCommon.WriteIC(icdev, sec, 3, keyA + keyControl + keyB);
        }
    }
}
