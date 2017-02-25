using Common;
using System;
using System.Text;

namespace DTU.GateWay.Protocol.WaterMessageClass
{
    public class Identifier_45 : Identifier
    {
        /// <summary>
        /// 标识符引导符
        /// </summary>
        public byte Key = 0x45;

        /// <summary>
        /// 标识符数据定义
        /// </summary>
        public byte Length = 0x20;

        public override byte GetKey()
        {
            return Key;
        }

        public override byte GetLength()
        {
            return Length;
        }

        public string AlarmStateV
        {
            get;
            set;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("【" + EnumUtils.GetDescription(typeof(Identifier_Standard), Key).Split('，')[0] + "】：");
            for (int i = 32 - 1; i >= 0; i--)
            {
                if (Enum.IsDefined(typeof(AlarmState), 32 - i))
                {
                    builder.Append(EnumUtils.GetDescription(typeof(AlarmState), 32 - i) + "：");
                    if (i == 32 - 1)
                    {
                        if (AlarmStateV[i] == '1')
                            builder.Append("停电、");
                        else
                            builder.Append("正常、");
                    }
                    else if (i == 32 - 2)
                    {
                        if (AlarmStateV[i] == '1')
                            builder.Append("电压低、");
                        else
                            builder.Append("正常、");
                    }
                    else if (i == 32 - 3)
                    {
                        if (AlarmStateV[i] == '1')
                            builder.Append("报警、");
                        else
                            builder.Append("正常、");
                    }
                    else if (i == 32 - 4)
                    {
                        if (AlarmStateV[i] == '1')
                            builder.Append("报警、");
                        else
                            builder.Append("正常、");
                    }
                    else if (i == 32 - 5)
                    {
                        if (AlarmStateV[i] == '1')
                            builder.Append("报警、");
                        else
                            builder.Append("正常、");
                    }
                    else if (i == 32 - 6)
                    {
                        if (AlarmStateV[i] == '1')
                            builder.Append("故障、");
                        else
                            builder.Append("正常、");
                    }
                    else if (i == 32 - 7)
                    {
                        if (AlarmStateV[i] == '1')
                            builder.Append("故障、");
                        else
                            builder.Append("正常、");
                    }
                    else if (i == 32 - 8)
                    {
                        if (AlarmStateV[i] == '1')
                            builder.Append("关闭、");
                        else
                            builder.Append("开启、");
                    }
                    else if (i == 32 - 9)
                    {
                        if (AlarmStateV[i] == '1')
                            builder.Append("异常、");
                        else
                            builder.Append("正常、");
                    }
                    else if (i == 32 - 10)
                    {
                        if (AlarmStateV[i] == '1')
                            builder.Append("IC卡有效、");
                        else
                            builder.Append("关闭、");
                    }
                    else if (i == 32 - 11)
                    {
                        if (AlarmStateV[i] == '1')
                            builder.Append("水泵停机、");
                        else
                            builder.Append("水泵工作、");
                    }
                    else if (i == 32 - 12)
                    {
                        if (AlarmStateV[i] == '1')
                            builder.Append("水量超限、");
                        else
                            builder.Append("未超限、");
                    }
                }
            }
            builder.Remove(builder.Length - 1, 1).Append("，");
            return builder.ToString();
        }

        public override string GetHexStr()
        {
            int ByteCount = 4;
            int Digits = 0;

            StringBuilder builderC = new StringBuilder();
            if (AlarmStateV.Length <= 32)
                builderC.Append(HexStringUtility.BinStringToHexString(AlarmStateV).PadLeft(8,'0'));
            else
                builderC.Append(HexStringUtility.BinStringToHexString(AlarmStateV).Substring(0, 8));

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
                ByteCount = 4;
                Digits = 0;
            }

            this.Length = Convert.ToByte(WaterBaseProtocol.GetLengthHexStr(ByteCount, Digits), 16);

            this.AlarmStateV = HexStringUtility.HexStringToBinString(hexStr).PadLeft(32, '0');
        }
    }
}
