using System;
using System.Text;

namespace DTU.GateWay.Protocol.WaterMessageClass
{
    public class RTUParam_01 : RTUParam
    {
        /// <summary>
        /// 标识符引导符
        /// </summary>
        public byte Key = 0x01;

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

        public int[] CenterStations
        {
            get;
            set;
        }

        public override object GetVal()
        {
            if (CenterStations != null)
            {
                return "" + CenterStations[0] + ";" + CenterStations[1] + ";" + CenterStations[2] + ";" + CenterStations[3] + "";
            }
            else
            {
                return "0;0;0;0";
            }
           
        
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("【" + EnumUtils.GetDescription(typeof(RTUParam.RTUParamKey), Key) + "】：");
            if (CenterStations != null)
            {
                builder.Append(string.Join("、", CenterStations) + "，");
            }
            else
            {
                builder.Append("无，");
            }
            return builder.ToString();
        }

        public override string GetHexStr()
        {
            StringBuilder builderC = new StringBuilder();
            for (int i = 0; i < 4; i++)
            {
                if (CenterStations.Length > i)
                {
                    if (CenterStations[i] > 0 && CenterStations[i] <= 255)
                    {
                        builderC.Append(CenterStations[i].ToString("X").PadLeft(2, '0'));
                    }
                    else
                    {
                        builderC.Append("00");
                    }
                }
            }

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
            this.CenterStations = new int[4];
            for (int i = 0; i < 4; i++)
            {
                try
                {
                    string val = hexStr.Substring(2 * i, 2);
                    if (val != "00")
                    {
                        this.CenterStations[i] = Convert.ToInt32(val, 16);
                    }
                    else
                    {
                        this.CenterStations[i] = 0;
                    }
                }
                catch
                {
                    this.CenterStations[i] = 0;
                }
            }
        }

        public override void SetVal(object val)
        {
            
        }
    }
}
