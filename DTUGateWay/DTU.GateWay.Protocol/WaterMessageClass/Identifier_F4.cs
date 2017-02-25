using System;
using System.Text;

namespace DTU.GateWay.Protocol.WaterMessageClass
{
    public class Identifier_F4 : Identifier
    {
        /// <summary>
        /// 标识符引导符
        /// </summary>
        public byte Key = 0xF4;

        /// <summary>
        /// 标识符数据定义
        /// </summary>
        public byte Length = 0x60;

        public override byte GetKey()
        {
            return Key;
        }

        public override byte GetLength()
        {
            return Length;
        }

        public decimal?[] Precipitation
        {
            get;
            set;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("【" + EnumUtils.GetDescription(typeof(Identifier_Standard), Key).Split('，')[0] + "】：");
            if (Precipitation != null)
            {
                if (Precipitation.Length > 0)
                {
                    builder.Append(string.Join("、", Precipitation) + "毫米，");
                }
                else
                {
                    builder.Append("长度为0，");
                }
            }
            else
            {
                builder.Append("不存在，");
            }
            return builder.ToString();
        }

        public override string GetHexStr()
        {

            StringBuilder builderC = new StringBuilder();
            if (Precipitation == null)
            {
                for (int i = 0; i < 12; i++)
                    builderC.Append("FF");
            }
            else
            {
                for (int i = 0; i < 12; i++)
                {
                    if (this.Precipitation.Length > i && this.Precipitation[i].HasValue)
                    {
                        if (this.Precipitation[i].Value * 10 > 0xFF)
                        {
                            builderC.Append("FF");
                        }
                        else
                        {
                            builderC.Append(((int)(this.Precipitation[i].Value * 10)).ToString("X").PadLeft(2, '0'));
                        }
                    }
                    else
                        builderC.Append("FF");
                }
            }

            StringBuilder builder = new StringBuilder();
            builder.Append(Key.ToString("X").PadLeft(2, '0'));
            builder.Append(Length.ToString("X").PadLeft(2, '0'));
            builder.Append(builderC.ToString());
            return builder.ToString();
        }

        public override string GetHexStrHead()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(Key.ToString("X").PadLeft(2, '0'));
            builder.Append(Length.ToString("X").PadLeft(2, '0'));
            return builder.ToString();
        }

        public override void SetVal(int ByteCount, int Digits, string hexStr)
        {
            this.Precipitation = new decimal?[12];
            for (int i = 0; i < 12; i++)
            {
                try
                {
                    string val = hexStr.Substring(2 * i, 2);
                    if (val != "FF")
                    {
                        this.Precipitation[i] = (decimal)(Convert.ToInt32(val, 16) / 10.0);
                    }
                    else
                    {
                        this.Precipitation[i] = null;
                    }
                }
                catch
                {
                    this.Precipitation[i] = null;
                }
            }
        }
    }
}
