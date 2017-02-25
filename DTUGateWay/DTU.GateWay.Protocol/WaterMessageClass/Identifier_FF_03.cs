using System;
using System.Text;

namespace DTU.GateWay.Protocol.WaterMessageClass
{
    public class Identifier_FF_03 : Identifier_FF
    {
        public byte KeySub = 0x03;

        public byte LengthSub = 0x0C;

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

        public decimal?[] WTDay
        {
            get;
            set;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("【" + EnumUtils.GetDescription(typeof(Identifier_Custom), Key).Split('，')[0] + "】：");
            if (WTDay != null)
            {
                if (WTDay.Length > 0)
                {
                    builder.Append(string.Join("、", WTDay) + "摄氏度，");
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
            for (int i = 0; i < 6; i++)
            {
                if (WTDay.Length > i && WTDay[i].HasValue)
                {
                    if (Convert.ToInt32(WTDay[i].Value * 10) > 0xFFFF)
                    {
                        builderC.Append("FFFF");
                    }
                    else
                    {
                        builderC.Append(Convert.ToInt32(WTDay[i].Value * 10).ToString().PadLeft(4, '0'));
                    }
                }
                else
                {
                    builderC.Append("FFFF");
                }
            }

            StringBuilder builder = new StringBuilder();
            builder.Append(Key.ToString("X").PadLeft(2, '0'));
            builder.Append(KeySub.ToString("X").PadLeft(2, '0'));
            builder.Append(LengthSub.ToString("X").PadLeft(2, '0'));
            builder.Append(builderC.ToString());
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

        public override void SetVal(int ByteCount, int Digits, string hexStr)
        {
            this.WTDay = new decimal?[6];
            for (int i = 0; i < 6; i++)
            {
                try
                {
                    string val = hexStr.Substring(4 * i, 4);
                    if (val != "FFFF")
                        this.WTDay[i] = (decimal)(int.Parse(val) / 10.0);
                    else
                        this.WTDay[i] = null;
                }
                catch
                {
                    this.WTDay[i] = null;
                }
            }
        }
    }
}
