using System.Text;

namespace DTU.GateWay.Protocol.WaterMessageClass
{
    public class Identifier_FD : Identifier
    {
        /// <summary>
        /// 标识符引导符
        /// </summary>
        public byte Key = 0xFD;

        /// <summary>
        /// 标识符数据定义
        /// </summary>
        public byte Length = 0xF6;

        public override byte GetKey()
        {
            return Key;
        }

        public override byte GetLength()
        {
            return Length;
        }

        public string Content
        {
            get;
            set;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("【" + EnumUtils.GetDescription(typeof(Identifier_Standard), Key).Split('，')[0] + "】：");
            builder.Append(Content + "，");
            return builder.ToString();
        }

        public override string GetHexStr()
        {
            StringBuilder builderC = new StringBuilder();
            if (Content.Length % 2 == 1)
            {
                builderC.Append("0");
            }
            builderC.Append(Content);

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
            this.Content = hexStr;
        }
    }
}
