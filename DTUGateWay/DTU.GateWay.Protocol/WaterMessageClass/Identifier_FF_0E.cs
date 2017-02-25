using System;
using System.Text;

namespace DTU.GateWay.Protocol.WaterMessageClass
{
    public class Identifier_FF_0E : Identifier_FF
    {
        public byte KeySub = 0x0E;

        public byte LengthSub = 0x12;

        public override byte GetKey()
        {
            return Key;
        }

        public override byte GetKeySub()
        {
            return KeySub;
        }

        public override byte GetLength()
        {
            return LengthSub;
        }

        public decimal?[] DepthDay
        {
            get;
            set;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("【" + EnumUtils.GetDescription(typeof(Identifier_Custom), Key).Split('，')[0] + "】：");
            if (DepthDay != null)
            {
                if (DepthDay.Length > 0)
                {
                    builder.Append(string.Join("、", DepthDay) + "米，");
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

        public override string GetHexStrHead()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(Key.ToString("X").PadLeft(2, '0'));
            builder.Append(KeySub.ToString("X").PadLeft(2, '0'));
            builder.Append(LengthSub.ToString("X").PadLeft(2, '0'));
            return builder.ToString();
        }

        public override string GetHexStr()
        {
            StringBuilder builderC = new StringBuilder();
            for (int i = 0; i < 6; i++)
            {
                if (DepthDay.Length > i && DepthDay[i].HasValue)
                {
                    if (Convert.ToInt32(DepthDay[i].Value * 100) > 0xFFFFFF)
                    {
                        builderC.Append("FFFFFF");
                    }
                    else
                    {
                        builderC.Append(Convert.ToInt32(DepthDay[i] * 100).ToString().PadLeft(6, '0'));
                    }
                }
                else
                {
                    builderC.Append("FFFFFF");
                }
            }

            StringBuilder builder = new StringBuilder();
            builder.Append(Key.ToString("X").PadLeft(2, '0'));
            builder.Append(KeySub.ToString("X").PadLeft(2, '0'));
            builder.Append(LengthSub.ToString("X").PadLeft(2, '0'));
            builder.Append(builderC.ToString());
            return builder.ToString();
        }

        public override void SetVal(int ByteCount, int Digits, string hexStr)
        {
            this.DepthDay = new decimal?[6];
            for (int i = 0; i < 6; i++)
            {
                try
                {
                    string val = hexStr.Substring(6 * i, 6);
                    if (val != "FFFFFF")
                        this.DepthDay[i] = (decimal)(int.Parse(val) / 100.0);
                    else
                        this.DepthDay[i] = null;
                }
                catch
                {
                    this.DepthDay[i] = null;
                }
            }
        }
    }
}
