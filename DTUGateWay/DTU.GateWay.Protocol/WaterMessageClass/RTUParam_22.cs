using System;
using System.Text;

namespace DTU.GateWay.Protocol.WaterMessageClass
{
    public class RTUParam_22 : RTUParam
    {
        /// <summary>
        /// 标识符引导符
        /// </summary>
        public byte Key = 0x22;

        /// <summary>
        /// 标识符数据定义
        /// </summary>
        public byte Length = 0x08;

        public override byte GetKey()
        {
            return Key;
        }

        public override byte GetLength()
        {
            return Length;
        }

        public int? Val
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
            builder.Append(Val.HasValue ? Val + "小时，" : "-，");
            return builder.ToString();
        }

        public override string GetHexStr()
        {
            if (!Val.HasValue || (Val < 0 || Val > 23))
            {
                return "";
            }

            StringBuilder builderC = new StringBuilder();
            builderC.Append(Val.Value.ToString().PadLeft(2, '0'));

            StringBuilder builder = new StringBuilder();
            builder.Append(Key.ToString("X").PadLeft(2, '0'));
            Length = Convert.ToByte(WaterBaseProtocol.GetLengthHexStr(builderC.ToString().Length / 2, 0), 16);
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
            try
            {
                this.Val = Convert.ToInt32(hexStr);
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
                this.Val = Convert.ToInt32(val);
            }
            catch
            {
                this.Val = null;
            }
        }
    }
}
