using System;
using System.Text;

namespace DTU.GateWay.Protocol.WaterMessageClass
{
    public class Identifier_04 : Identifier
    {
        /// <summary>
        /// 标识符引导符
        /// </summary>
        public byte Key = 0x04;

        /// <summary>
        /// 标识符数据定义
        /// </summary>
        public byte Length = 0x18;

        public override byte GetKey()
        {
            return Key;
        }

        public override byte GetLength()
        {
            return Length;
        }

        public int dd
        {
            get;
            set;
        }

        public int HH
        {
            get;
            set;
        }

        public int mm
        {
            get;
            set;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("【" + EnumUtils.GetDescription(typeof(Identifier_Standard), Key).Split('，')[0] + "】：");
            if (dd > 0)
            {
                builder.Append(dd + "天，");
            }
            else if (HH > 0)
            {
                builder.Append(HH + "小时，");
            }
            else if (mm > 0)
            {
                builder.Append(mm + "分钟，");
            }
            else
            {
                builder.Append("1小时时间段，");
            }
            return builder.ToString();
        }

        public override string GetHexStr()
        {
            try
            {
                int ByteCount = 3;
                int Digits = 0;

                StringBuilder builderC = new StringBuilder();
                builderC.Append(dd.ToString().PadLeft(2, '0'));
                builderC.Append(HH.ToString().PadLeft(2, '0'));
                builderC.Append(mm.ToString().PadLeft(2, '0'));

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
                Digits = 0;
            }

            this.Length = Convert.ToByte(WaterBaseProtocol.GetLengthHexStr(ByteCount, Digits), 16);

            try
            {
                this.dd = Convert.ToInt32(hexStr.Substring(0, 2));
            }
            catch
            {
                this.dd = 0;
            }
            try
            {
                this.HH = Convert.ToInt32(hexStr.Substring(2, 2));
            }
            catch
            {
                this.HH = 0;
            }
            try
            {
                this.mm = Convert.ToInt32(hexStr.Substring(4, 2));
            }
            catch
            {
                this.mm = 0;
            }
        }
    }
}
