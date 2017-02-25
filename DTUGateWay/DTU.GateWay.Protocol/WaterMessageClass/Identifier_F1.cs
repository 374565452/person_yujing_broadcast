using System;
using System.Text;

namespace DTU.GateWay.Protocol.WaterMessageClass
{
    public class Identifier_F1 : Identifier
    {
        /// <summary>
        /// 标识符引导符
        /// </summary>
        public byte Key = 0xF1;

        /// <summary>
        /// 标识符数据定义
        /// </summary>
        public byte Length = 0xF1;

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

        //测站类型
        //40H、41H、45H、46H、47H、48H、49H、4AH、4BH、4CH、4DH、4EH、4FH、50H、51H不包含
        //30H、31H、32H、33H、34H、36H、37H、38H、44H包含
        public byte StationType
        {
            get;
            set;
        }

        public enum HaveKey
        {
            _30 = 0x30,
            _31 = 0x31,
            _32 = 0x32,
            _33 = 0x33,
            _34 = 0x34,
            _36 = 0x36,
            _37 = 0x37,
            _38 = 0x38,
            _3A = 0x3A,
            _44 = 0x44
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("【测站编码】：");
            builder.Append(RemoteStation + "，");
            if (StationType > 0)
            {
                builder.Append("【测站类型】：");
                builder.Append(EnumUtils.GetDescription(typeof(WaterBaseProtocol.StationType), StationType) + "，");
            }
            return builder.ToString();
        }

        public override string GetHexStr()
        {
            StringBuilder builderC = new StringBuilder();
            builderC.Append(RemoteStation.PadLeft(10, '0'));
            if (StationType != 0x00)
                builderC.Append(StationType.ToString("X").PadLeft(2, '0'));

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
            this.RemoteStation = hexStr.Substring(0, 10);
            if (hexStr.Length >= 12)
                this.StationType = Convert.ToByte(hexStr.Substring(10, 2), 16);
            else
                this.StationType = 0x00;
        }
    }
}
