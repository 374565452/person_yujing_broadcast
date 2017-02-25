using Common;
using System;
using System.Text;

namespace DTU.GateWay.Protocol
{
    public class DeviceInfo
    {
        /// <summary>
        /// 设备序列，根据协议规定，长度为7
        /// 协议格式
        /// </summary>
        public byte[] SerialNum
        {
            get;
            set;
        }

        /// <summary>
        /// 设备序列字符串 7字节 16进制
        /// </summary>
        public string SerialString
        {
            get;
            set;
        }

        /// <summary>
        /// 设备序列字符串 前6字节 + 3位数字
        /// </summary>
        public string DeviceNo
        {
            get;
            set;
        }

        /// <summary>
        /// 设备序列长整型
        /// </summary>
        public long SerialLong
        {
            get;
            set;
        }

        public DeviceInfo()
        {
            SerialNum = new byte[7];
        }

        public DeviceInfo(string serialString)
        {
            this.SerialString = serialString;
            this.SerialNum = GetDeviceSerialBytes(serialString);
            this.SerialLong = FormatHelper.ByteArrayToBigInt_BigMode(this.SerialNum, 0, this.SerialNum.Length);
            this.DeviceNo = SerialString.Substring(0, 12) + Convert.ToInt32(SerialString.Substring(12, 2), 16).ToString().PadLeft(3, '0');
        }

        /// <summary>
        /// 将字节填充至类
        /// </summary>
        /// <param name="data"></param>
        public void Parse(byte[] data)
        {
            int length = 0;

            length = data.Length;
            if (data.Length > SerialNum.Length)
            {
                length = SerialNum.Length;
            }

            Array.Copy(data, SerialNum, length);
            this.SerialLong = FormatHelper.ByteArrayToBigInt_BigMode(SerialNum, 0, SerialNum.Length);
            this.SerialString = string.Format("{0:X14}", SerialLong);
            this.DeviceNo = SerialString.Substring(0, 12) + Convert.ToInt32(SerialString.Substring(12, 2), 16).ToString().PadLeft(3, '0');

        }
        /// <summary>
        /// 转换为协议
        /// </summary>
        /// <param name="deviceID"></param>
        public void Parse(long deviceID)
        {
            /*
            this.SerialLong = FormatHelper.ToBCD(deviceID);
            FormatHelper.LongToByteArray_BigMode(SerialNum, this.SerialLong, 0);            
            this.SerialString = string.Format("{0:X14}", SerialLong);
             * */

            this.SerialLong = deviceID;
            this.SerialString = deviceID.ToString("X").PadLeft(14,'0');
            this.SerialNum = GetDeviceSerialBytes(SerialString);
            this.DeviceNo = SerialString.Substring(0, 12) + Convert.ToInt32(SerialString.Substring(12, 2), 16).ToString().PadLeft(3, '0');
        }

        public byte[] GetDeviceSerialBytes(string serialString)
        {
            byte[] device = new byte[7];
            
            //string t;
            int index = 0;
            byte vv = 0;

            if (serialString.Length < 14) return device;
            device = HexStringUtility.HexStringToByteArray(serialString.Substring(0, 14));
            /*
            for (int i = 0; i < 14; i += 2)
            {
                vv = byte.Parse(serialString.Substring(i, 1));
                vv = (byte)((vv << 4) | (byte.Parse(serialString.Substring(i + 1, 1))));


                device[index] = vv;
                index = index + 1;
            }
            */
            return device;
        }

        public string GetDeviceSerialString(byte[] serial)
        {
            StringBuilder sb = new StringBuilder();
            int i = 0;

            if (serial.Length < 7) return "";

            while (i < 7)
            {
                sb.Append(string.Format("{0:X2}", serial[i]));
                i++;
            }

            return sb.ToString();

        }

        public string GetDeviceSerialString(byte[] serial, int startIndex)
        {
            byte[] param = new byte[7];

            Array.Copy(serial, startIndex, param, 0, 7);

            return GetDeviceSerialString(param);
        }
    }
}
