using System;
using System.Text;

namespace DTU.GateWay.Protocol.WaterMessageClass
{
    public class RTUParam_07 : RTUParam
    {
        /// <summary>
        /// 标识符引导符
        /// </summary>
        public byte Key = 0x07;

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


        public int ChannelTypeV
        {
            get;
            set;
        }

        public string IP
        {
            get;
            set;
        }

        public int Port
        {
            get;
            set;
        }

        public string Add
        {
            get;
            set;
        }

        public override object GetVal()
        {
            return ChannelTypeV + ";" + IP + ";" + Port + ";" + Add;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("【" + EnumUtils.GetDescription(typeof(RTUParam.RTUParamKey), Key) + "】：");
            builder.Append("信道类型：" + EnumUtils.GetDescription(typeof(ChannelType), ChannelTypeV) + "，");
            if (ChannelTypeV == (int)ChannelType._02)
            {
                builder.Append("目的地地址：" + IP + "，端口：" + Port + "，");
            }
            else
            {
                builder.Append("目的地地址：" + Add + "，");
            }
            return builder.ToString();
        }

        public override string GetHexStr()
        {
            //if (!Enum.IsDefined(typeof(ChannelType), ChannelTypeV))
            //    return "";

            StringBuilder builderC = new StringBuilder();
            if (Enum.IsDefined(typeof(ChannelType), ChannelTypeV))
            {
                builderC.Append(ChannelTypeV.ToString("X").PadLeft(2, '0'));
                if (ChannelTypeV == (int)ChannelType._02)
                {
                    string[] ips = IP.Split('.');
                    for (int i = 0; i < 4; i++)
                    {
                        if (ips.Length > i)
                            builderC.Append(ips[i].PadLeft(3, '0'));
                        else
                            builderC.Append("000");
                    }
                    builderC.Append(Port.ToString().PadLeft(6, '0'));
                }
                else
                {
                    if (Add.Length % 2 == 1)
                    {
                        builderC.Append("0");
                    }
                    builderC.Append(Add);
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
            try
            {
                this.ChannelTypeV = Convert.ToInt32(hexStr.Substring(0, 2), 16);
                if (this.ChannelTypeV == (int)ChannelType._02)
                {
                    string IP1 = hexStr.Substring(2, 3).TrimStart('0');
                    string IP2 = hexStr.Substring(5, 3).TrimStart('0');
                    string IP3 = hexStr.Substring(8, 3).TrimStart('0');
                    string IP4 = hexStr.Substring(11, 3).TrimStart('0');
                    this.IP = IP1 + "." + IP2 + "." + IP3 + "." + IP4;
                    this.Port = Convert.ToInt32(hexStr.Substring(14));
                }
                else
                {
                    this.Add = hexStr.Substring(2);
                }
            }
            catch
            {
                this.Add = hexStr.Substring(2);
            }
        }

        public override void SetVal(object val)
        {

        }
    }
}
