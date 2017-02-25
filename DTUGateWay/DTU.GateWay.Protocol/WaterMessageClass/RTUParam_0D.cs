using Common;
using System;
using System.Text;

namespace DTU.GateWay.Protocol.WaterMessageClass
{
    public class RTUParam_0D : RTUParam
    {
        /// <summary>
        /// 标识符引导符
        /// </summary>
        public byte Key = 0x0D;

        /// <summary>
        /// 标识符数据定义
        /// </summary>
        public byte Length = 0x40;

        public override byte GetKey()
        {
            return Key;
        }

        public override byte GetLength()
        {
            return Length;
        }

        public string BitStr
        {
            get;
            set;
        }

        public override object GetVal()
        {
            return BitStr;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("【" + EnumUtils.GetDescription(typeof(RTUParam.RTUParamKey), Key) + "】：");
            for (int i = 0; i < 64; i++)
            {
                if (BitStr[i] == '1')
                {
                    builder.Append(EnumUtils.GetDescription(typeof(IdenCollection), i) + "、");
                }
            }
            builder.Remove(builder.Length - 1, 1).Append("，");
            return builder.ToString();
        }

        public override string GetHexStr()
        {
            StringBuilder builderC = new StringBuilder();
            if (BitStr.Length <= 64)
                builderC.Append(HexStringUtility.BinStringToHexString(BitStr.PadLeft(64, '0')));
            else
                builderC.Append(HexStringUtility.BinStringToHexString(BitStr.Substring(0, 64)));

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
            this.BitStr = HexStringUtility.HexStringToBinString(hexStr);
        }

        public override void SetVal(object val)
        {

        }
    }
}
