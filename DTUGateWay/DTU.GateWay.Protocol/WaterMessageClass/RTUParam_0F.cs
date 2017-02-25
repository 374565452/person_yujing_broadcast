using Common;
using System;
using System.Text;

namespace DTU.GateWay.Protocol.WaterMessageClass
{
    public class RTUParam_0F : RTUParam
    {
        /// <summary>
        /// 标识符引导符
        /// </summary>
        public byte Key = 0x0F;

        /// <summary>
        /// 标识符数据定义
        /// </summary>
        public byte Length = 0x00;

        public override byte GetKey()
        {
            return Key;
        }

        public override byte GetLength()
        {
            return Length;
        }

        public int SimNoTypeV
        {
            get;
            set;
        }

        public string SimNo
        {
            get;
            set;
        }

        public override object GetVal()
        {
            return SimNoTypeV + ";" + SimNo;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("【" + EnumUtils.GetDescription(typeof(RTUParam.RTUParamKey), Key) + "】：");
            builder.Append("卡类型：" + EnumUtils.GetDescription(typeof(SimNoType), SimNoTypeV) + "，");
            builder.Append("卡识别号：" + SimNo + "，");
            return builder.ToString();
        }

        public override string GetHexStr()
        {
            StringBuilder builderC = new StringBuilder();
            builderC.Append(HexStringUtility.StrToHexString(SimNoTypeV.ToString()));
            builderC.Append(HexStringUtility.StrToHexString(SimNo));

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
                this.SimNoTypeV = Convert.ToInt32(HexStringUtility.HexStringToStr(hexStr.Substring(0, 2)));
                this.SimNo = HexStringUtility.HexStringToStr(hexStr.Substring(2));
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
