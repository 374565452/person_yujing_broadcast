using System;
using System.Text;

namespace DTU.GateWay.Protocol.WaterMessageClass
{
    public class Identifier_39 : Identifier
    {
        /// <summary>
        /// 标识符引导符
        /// </summary>
        public byte Key = 0x39;

        /// <summary>
        /// 标识符数据定义
        /// </summary>
        public byte Length = 0x23;

        public override byte GetKey()
        {
            return Key;
        }

        public override byte GetLength()
        {
            return Length;
        }

        public decimal? WaterLevel
        {
            get;
            set;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("【" + EnumUtils.GetDescription(typeof(Identifier_Standard), Key).Split('，')[0] + "】：");
            builder.Append(WaterLevel.HasValue ? WaterLevel + "米，" : "-，");
            return builder.ToString();
        }

        public override string GetHexStr()
        {
            if (!WaterLevel.HasValue)
            {
                return "";
            }

            try
            {
                int ByteCount = 4;
                int Digits = 3;

                if ((double)WaterLevel.Value * Math.Pow(10, Digits) >= Math.Pow(10, ByteCount * 2))
                {
                    return "";
                }

                StringBuilder builderC = new StringBuilder();
                builderC.Append(((double)WaterLevel.Value * Math.Pow(10, Digits)).ToString().PadLeft(ByteCount * 2, '0'));

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
                ByteCount = 4;
                Digits = 3;
            }

            this.Length = Convert.ToByte(WaterBaseProtocol.GetLengthHexStr(ByteCount, Digits), 16);

            try
            {
                this.WaterLevel = (decimal)(Convert.ToInt32(hexStr) * 1.0 / Math.Pow(10, Digits));
            }
            catch
            {
                this.WaterLevel = null;
            }
        }
    }
}
