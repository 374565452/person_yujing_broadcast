using System;
using System.Text;

namespace DTU.GateWay.Protocol.WaterMessageClass
{
    public class RTUParam_02 : RTUParam
    {
        /// <summary>
        /// 标识符引导符
        /// </summary>
        public byte Key = 0x02;

        /// <summary>
        /// 标识符数据定义
        /// </summary>
        public byte Length = 0x28;

        public override byte GetKey()
        {
            return Key;
        }

        public override byte GetLength()
        {
            return Length;
        }

        public string RemoteStation
        {
            get;
            set;
        }

        public override object GetVal()
        {
            return RemoteStation;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("【" + EnumUtils.GetDescription(typeof(RTUParam.RTUParamKey), Key) + "】：");
            builder.Append(RemoteStation + "，");
            return builder.ToString();
        }

        public override string GetHexStr()
        {
            StringBuilder builderC = new StringBuilder();
            if (RemoteStation.Length <= 10)
                builderC.Append(RemoteStation.PadLeft(10, '0'));
            else
                builderC.Append(RemoteStation.Substring(0, 10));

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
            this.RemoteStation = hexStr;
        }

        public override void SetVal(object val)
        {

        }
    }
}
