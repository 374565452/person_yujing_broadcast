using System;
using System.Text;

namespace DTU.GateWay.Protocol.WaterMessageClass
{
    public class Identifier_FB : Identifier
    {
        /// <summary>
        /// 标识符引导符
        /// </summary>
        public byte Key = 0xFB;

        /// <summary>
        /// 标识符数据定义
        /// </summary>
        public byte Length = 0xC0;

        public override byte GetKey()
        {
            return Key;
        }

        public override byte GetLength()
        {
            return Length;
        }

        public decimal?[] WaterLevel
        {
            get;
            set;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("【" + EnumUtils.GetDescription(typeof(Identifier_Standard), Key).Split('，')[0] + "】：");
            if (WaterLevel != null)
            {
                if (WaterLevel.Length > 0)
                {
                    builder.Append(string.Join("、", WaterLevel) + "米，");
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
            if (WaterLevel == null)
            {
                for (int i = 0; i < 12; i++)
                    builderC.Append("FFFF");
            }
            else
            {
                for (int i = 0; i < 12; i++)
                {
                    if (this.WaterLevel.Length > i && this.WaterLevel[i].HasValue)
                    {
                        if (this.WaterLevel[i].Value * 100 > 0xFFFF)
                        {
                            builderC.Append("FFFF");
                        }
                        else
                        {
                            builderC.Append(((int)(this.WaterLevel[i].Value * 100)).ToString("X").PadLeft(4, '0'));
                        }
                    }
                    else
                        builderC.Append("FFFF");
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
            this.WaterLevel = new decimal?[12];
            for (int i = 0; i < 12; i++)
            {
                try
                {
                    string val = hexStr.Substring(4 * i, 4);
                    if (val != "FFFF")
                    {
                        this.WaterLevel[i] = (decimal)(Convert.ToInt32(val, 16) / 100.0);
                    }
                    else
                    {
                        this.WaterLevel[i] = null;
                    }
                }
                catch
                {
                    this.WaterLevel[i] = null;
                }
            }
        }
    }
}
