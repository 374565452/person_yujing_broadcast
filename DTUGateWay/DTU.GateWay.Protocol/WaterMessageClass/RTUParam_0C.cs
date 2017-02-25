using System;
using System.Text;

namespace DTU.GateWay.Protocol.WaterMessageClass
{
    public class RTUParam_0C : RTUParam
    {
        /// <summary>
        /// 标识符引导符
        /// </summary>
        public byte Key = 0x0C;

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

        public int WorkTypeV
        {
            get;
            set;
        }

        public override object GetVal()
        {
            return WorkTypeV;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("【" + EnumUtils.GetDescription(typeof(RTUParam.RTUParamKey), Key) + "】：");
            builder.Append(EnumUtils.GetDescription(typeof(WorkType), WorkTypeV) + "，");           
            return builder.ToString();
        }

        public override string GetHexStr()
        {
            if (!Enum.IsDefined(typeof(WorkType), WorkTypeV))
                return "";

            StringBuilder builderC = new StringBuilder();
            builderC.Append(WorkTypeV.ToString("X").PadLeft(2, '0'));

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
                this.WorkTypeV = Convert.ToInt32(hexStr.Substring(0, 2), 16);
            }
            catch
            {
            }
        }

        public override void SetVal(object val)
        {

        }
    }
}
