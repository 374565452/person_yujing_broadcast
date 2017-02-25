using System;
using System.Globalization;
using System.Text;

namespace DTU.GateWay.Protocol.WaterMessageClass
{
    public class Identifier_F0 : Identifier
    {
        /// <summary>
        /// 标识符引导符
        /// </summary>
        public byte Key = 0xF0;

        /// <summary>
        /// 标识符数据定义
        /// </summary>
        public byte Length = 0xF0;

        public override byte GetKey()
        {
            return Key;
        }

        public override byte GetLength()
        {
            return Length;
        }

        public DateTime? ObsTime
        {
            get;
            set;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("【观测时间】：");
            builder.Append(ObsTime.HasValue ? ObsTime.Value.ToString("yyyy-MM-dd HH:mm:ss") + "，" : "无观测时间");
            return builder.ToString();
        }

        public override string GetHexStr()
        {
            if (!ObsTime.HasValue)
            {
                return "";
            }

            StringBuilder builderC = new StringBuilder();
            builderC.Append(ObsTime.Value.ToString("yyyyMMddHHmm").Substring(2));

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
            try
            {
                this.ObsTime = DateTime.ParseExact("20" + hexStr + "00", "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            }
            catch
            {
                this.ObsTime = null;
            }
        }
    }
}
