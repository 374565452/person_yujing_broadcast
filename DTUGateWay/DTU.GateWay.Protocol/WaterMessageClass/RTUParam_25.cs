using System;
using System.Text;

namespace DTU.GateWay.Protocol.WaterMessageClass
{
    public class RTUParam_25 : RTUParam
    {
        /// <summary>
        /// 标识符引导符
        /// </summary>
        public byte Key = 0x25;

        /// <summary>
        /// 标识符数据定义
        /// </summary>
        public byte Length = 0x09;

        public override byte GetKey()
        {
            return Key;
        }

        public override byte GetLength()
        {
            return Length;
        }

        public decimal? Val
        {
            get;
            set;
        }

        public override object GetVal()
        {
            return Val;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("【" + EnumUtils.GetDescription(typeof(RTUParam.RTUParamKey), Key) + "】：");
            builder.Append(Val.HasValue ? Val + "毫米，" : "-，");
            return builder.ToString();
        }

        public override string GetHexStr()
        {
            int ByteCount = 1;
            int Digits = 1;

            if (!Val.HasValue || (double)Val.Value * Math.Pow(10, Digits) >= Math.Pow(10, ByteCount * 2))
            {
                return "";
            }

            StringBuilder builderC = new StringBuilder();

            builderC.Append(((double)Val.Value * Math.Pow(10, Digits)).ToString().PadLeft(ByteCount * 2, '0'));

            StringBuilder builder = new StringBuilder();
            builder.Append(Key.ToString("X").PadLeft(2, '0'));
            Length = Convert.ToByte(WaterBaseProtocol.GetLengthHexStr(ByteCount, Digits), 16);
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
            if (ByteCount <= 0 && Digits <= 0)
            {
                ByteCount = 1;
                Digits = 1;
            }

            this.Length = Convert.ToByte(WaterBaseProtocol.GetLengthHexStr(ByteCount, Digits), 16);

            try
            {
                this.Val = (decimal)(Convert.ToInt32(hexStr) * 1.0 / Math.Pow(10, Digits));
            }
            catch
            {
                this.Val = null;
            }
        }

        public override void SetVal(object val)
        {
            try
            {
                this.Val = Convert.ToDecimal(val);
            }
            catch
            {
                this.Val = null;
            }
        }
    }
}
