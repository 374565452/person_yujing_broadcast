using System;
using System.Text;

namespace DTU.GateWay.Protocol.WaterMessageClass
{
    public class Identifier_26 : Identifier
    {
        /// <summary>
        /// 标识符引导符
        /// </summary>
        public byte Key = 0x26;

        /// <summary>
        /// 标识符数据定义
        /// </summary>
        public byte Length = 0x19;

        public override byte GetKey()
        {
            return Key;
        }

        public override byte GetLength()
        {
            return Length;
        }

        public decimal? CumulativePrecipitation
        {
            get;
            set;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("【" + EnumUtils.GetDescription(typeof(Identifier_Standard), Key).Split('，')[0] + "】：");
            builder.Append(CumulativePrecipitation.HasValue ? CumulativePrecipitation + "毫米，" : "-，");
            return builder.ToString();
        }

        public override string GetHexStr()
        {
            if (!CumulativePrecipitation.HasValue)
            {
                return "";
            }

            try
            {
                int ByteCount = 3;
                int Digits = 1;

                if ((double)CumulativePrecipitation.Value * Math.Pow(10, Digits) >= Math.Pow(10, ByteCount * 2))
                {
                    return "";
                }

                StringBuilder builderC = new StringBuilder();
                builderC.Append(((double)CumulativePrecipitation.Value * Math.Pow(10, Digits)).ToString().PadLeft(ByteCount * 2, '0'));

                StringBuilder builder = new StringBuilder();
                builder.Append(Key.ToString("X").PadLeft(2, '0'));
                Length = Convert.ToByte(WaterBaseProtocol.GetLengthHexStr(ByteCount, Digits), 16);
                builder.Append(Length.ToString("X").PadLeft(2, '0'));
                builder.Append(builderC.ToString());
                return builder.ToString();
            }
            catch
            {
                return "";
            }
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
            if (ByteCount <= 0 && Digits <= 0)
            {
                ByteCount = 3;
                Digits = 1;
            }

            this.Length = Convert.ToByte(WaterBaseProtocol.GetLengthHexStr(ByteCount, Digits), 16);

            try
            {
                this.CumulativePrecipitation = (decimal)(Convert.ToInt32(hexStr) * 1.0 / Math.Pow(10, Digits));
            }
            catch
            {
                this.CumulativePrecipitation = null;
            }
        }
    }
}
