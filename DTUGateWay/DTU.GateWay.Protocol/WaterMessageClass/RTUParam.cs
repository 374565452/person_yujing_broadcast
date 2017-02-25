using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace DTU.GateWay.Protocol.WaterMessageClass
{
    public abstract class RTUParam
    {
        public abstract byte GetKey();

        public abstract byte GetLength();

        public abstract object GetVal();

        public abstract string GetHexStr();

        public abstract string GetHexStrHead();

        public abstract void SetVal(int ByteCount, int Digits, string hexStr);

        public abstract void SetVal(object val);

        public static List<RTUParam> analyse(string Remain, byte AFN, out string msg)
        {
            List<RTUParam> List_RTUParam = new List<RTUParam>();

            try
            {
                while (Remain.Length >= 4)
                {
                    byte Key = Convert.ToByte(Remain.Substring(0, 2), 16);
                    string LengthStr = Remain.Substring(2, 2);
                    byte Length = Convert.ToByte(LengthStr, 16);

                    int[] LS = WaterBaseProtocol.GetLengthList(LengthStr);
                    int ByteCount = LS[0];
                    int Digits = LS[1];
                    if (Remain.Length < 2 + 2 + ByteCount * 2)
                    {
                        msg = "长度不足，无法获取参数" + Key + "数据" + " " + Remain;
                        return null;
                    }

                    string Val = Remain.Substring(4, ByteCount * 2);
                    RTUParam rp;
                    if (Key == (byte)RTUParamKey._01)
                    {
                        rp = new RTUParam_01();
                    }
                    else if (Key == (byte)RTUParamKey._02)
                    {
                        rp = new RTUParam_02();
                    }
                    else if (Key == (byte)RTUParamKey._03)
                    {
                        rp = new RTUParam_03();
                    }
                    else if (Key == (byte)RTUParamKey._04)
                    {
                        rp = new RTUParam_04();
                    }
                    else if (Key == (byte)RTUParamKey._05)
                    {
                        rp = new RTUParam_05();
                    }
                    else if (Key == (byte)RTUParamKey._06)
                    {
                        rp = new RTUParam_06();
                    }
                    else if (Key == (byte)RTUParamKey._07)
                    {
                        rp = new RTUParam_07();
                    }
                    else if (Key == (byte)RTUParamKey._08)
                    {
                        rp = new RTUParam_08();
                    }
                    else if (Key == (byte)RTUParamKey._09)
                    {
                        rp = new RTUParam_09();
                    }
                    else if (Key == (byte)RTUParamKey._0A)
                    {
                        rp = new RTUParam_0A();
                    }
                    else if (Key == (byte)RTUParamKey._0B)
                    {
                        rp = new RTUParam_0B();
                    }
                    else if (Key == (byte)RTUParamKey._0C)
                    {
                        rp = new RTUParam_0C();
                    }
                    else if (Key == (byte)RTUParamKey._0D)
                    {
                        rp = new RTUParam_0D();
                    }
                    else if (Key == (byte)RTUParamKey._0E)
                    {
                        rp = new RTUParam_0E();
                    }
                    else if (Key == (byte)RTUParamKey._0F)
                    {
                        rp = new RTUParam_0F();
                    }
                    else if (Key == (byte)RTUParamKey._20)
                    {
                        rp = new RTUParam_20();
                    }
                    else if (Key == (byte)RTUParamKey._21)
                    {
                        rp = new RTUParam_21();
                    }
                    else if (Key == (byte)RTUParamKey._22)
                    {
                        rp = new RTUParam_22();
                    }
                    else if (Key == (byte)RTUParamKey._23)
                    {
                        rp = new RTUParam_23();
                    }
                    else if (Key == (byte)RTUParamKey._24)
                    {
                        rp = new RTUParam_24();
                    }
                    else if (Key == (byte)RTUParamKey._25)
                    {
                        rp = new RTUParam_25();
                    }
                    else if (Key == (byte)RTUParamKey._26)
                    {
                        rp = new RTUParam_26();
                    }
                    else if (Key == (byte)RTUParamKey._27)
                    {
                        rp = new RTUParam_27();
                    }
                    else if (Key == (byte)RTUParamKey._28)
                    {
                        rp = new RTUParam_28();
                    }
                    else if (Key == (byte)RTUParamKey._29)
                    {
                        rp = new RTUParam_29();
                    }
                    else if (Key == (byte)RTUParamKey._2A)
                    {
                        rp = new RTUParam_2A();
                    }
                    else if (Key == (byte)RTUParamKey._2B)
                    {
                        rp = new RTUParam_2B();
                    }
                    else if (Key == (byte)RTUParamKey._2C)
                    {
                        rp = new RTUParam_2C();
                    }
                    else if (Key == (byte)RTUParamKey._2D)
                    {
                        rp = new RTUParam_2D();
                    }
                    else if (Key == (byte)RTUParamKey._2E)
                    {
                        rp = new RTUParam_2E();
                    }
                    else if (Key == (byte)RTUParamKey._2F)
                    {
                        rp = new RTUParam_2F();
                    }
                    else if (Key == (byte)RTUParamKey._30)
                    {
                        rp = new RTUParam_30();
                    }
                    else if (Key == (byte)RTUParamKey._31)
                    {
                        rp = new RTUParam_31();
                    }
                    else if (Key == (byte)RTUParamKey._32)
                    {
                        rp = new RTUParam_32();
                    }
                    else if (Key == (byte)RTUParamKey._33)
                    {
                        rp = new RTUParam_33();
                    }
                    else if (Key == (byte)RTUParamKey._34)
                    {
                        rp = new RTUParam_34();
                    }
                    else if (Key == (byte)RTUParamKey._35)
                    {
                        rp = new RTUParam_35();
                    }
                    else if (Key == (byte)RTUParamKey._36)
                    {
                        rp = new RTUParam_36();
                    }
                    else if (Key == (byte)RTUParamKey._37)
                    {
                        rp = new RTUParam_37();
                    }
                    else if (Key == (byte)RTUParamKey._38)
                    {
                        rp = new RTUParam_38();
                    }
                    else if (Key == (byte)RTUParamKey._39)
                    {
                        rp = new RTUParam_39();
                    }
                    else if (Key == (byte)RTUParamKey._3A)
                    {
                        rp = new RTUParam_3A();
                    }
                    else if (Key == (byte)RTUParamKey._3B)
                    {
                        rp = new RTUParam_3B();
                    }
                    else if (Key == (byte)RTUParamKey._3C)
                    {
                        rp = new RTUParam_3C();
                    }
                    else if (Key == (byte)RTUParamKey._3D)
                    {
                        rp = new RTUParam_3D();
                    }
                    else if (Key == (byte)RTUParamKey._3E)
                    {
                        rp = new RTUParam_3E();
                    }
                    else if (Key == (byte)RTUParamKey._3F)
                    {
                        rp = new RTUParam_3F();
                    }
                    else if (Key == (byte)RTUParamKey._40)
                    {
                        rp = new RTUParam_40();
                    }
                    else if (Key == (byte)RTUParamKey._41)
                    {
                        rp = new RTUParam_41();
                    }
                    else if (Key == (byte)RTUParamKey._97)
                    {
                        rp = new RTUParam_97();
                    }
                    else if (Key == (byte)RTUParamKey._98)
                    {
                        rp = new RTUParam_98();
                    }
                    else
                    {
                        rp = new RTUParam_00(Key);
                    }

                    if(Val!="")
                        rp.SetVal(ByteCount, Digits, Val);
                    List_RTUParam.Add(rp);

                    Remain = Remain.Substring(2 + 2 + ByteCount * 2);
                }
                msg = "";
                return List_RTUParam;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return null;
            }
        }

        public static List<RTUParam> analyse2(string Remain, out string msg)
        {
            List<RTUParam> List_RTUParam = new List<RTUParam>();

            try
            {
                while (Remain.Length >= 4)
                {
                    byte Key = Convert.ToByte(Remain.Substring(0, 2), 16);
                    string LengthStr = Remain.Substring(2, 2);
                    byte Length = Convert.ToByte(LengthStr, 16);

                    int[] LS = WaterBaseProtocol.GetLengthList(LengthStr);
                    int ByteCount = LS[0];
                    int Digits = LS[1];

                    RTUParam rp;
                    if (Key == (byte)RTUParamKey._01)
                    {
                        rp = new RTUParam_01();
                    }
                    else if (Key == (byte)RTUParamKey._02)
                    {
                        rp = new RTUParam_02();
                    }
                    else if (Key == (byte)RTUParamKey._03)
                    {
                        rp = new RTUParam_03();
                    }
                    else if (Key == (byte)RTUParamKey._04)
                    {
                        rp = new RTUParam_04();
                    }
                    else if (Key == (byte)RTUParamKey._05)
                    {
                        rp = new RTUParam_05();
                    }
                    else if (Key == (byte)RTUParamKey._06)
                    {
                        rp = new RTUParam_06();
                    }
                    else if (Key == (byte)RTUParamKey._07)
                    {
                        rp = new RTUParam_07();
                    }
                    else if (Key == (byte)RTUParamKey._08)
                    {
                        rp = new RTUParam_08();
                    }
                    else if (Key == (byte)RTUParamKey._09)
                    {
                        rp = new RTUParam_09();
                    }
                    else if (Key == (byte)RTUParamKey._0A)
                    {
                        rp = new RTUParam_0A();
                    }
                    else if (Key == (byte)RTUParamKey._0B)
                    {
                        rp = new RTUParam_0B();
                    }
                    else if (Key == (byte)RTUParamKey._0C)
                    {
                        rp = new RTUParam_0C();
                    }
                    else if (Key == (byte)RTUParamKey._0D)
                    {
                        rp = new RTUParam_0D();
                    }
                    else if (Key == (byte)RTUParamKey._0E)
                    {
                        rp = new RTUParam_0E();
                    }
                    else if (Key == (byte)RTUParamKey._0F)
                    {
                        rp = new RTUParam_0F();
                    }
                    else if (Key == (byte)RTUParamKey._20)
                    {
                        rp = new RTUParam_20();
                    }
                    else if (Key == (byte)RTUParamKey._21)
                    {
                        rp = new RTUParam_21();
                    }
                    else if (Key == (byte)RTUParamKey._22)
                    {
                        rp = new RTUParam_22();
                    }
                    else if (Key == (byte)RTUParamKey._23)
                    {
                        rp = new RTUParam_23();
                    }
                    else if (Key == (byte)RTUParamKey._24)
                    {
                        rp = new RTUParam_24();
                    }
                    else if (Key == (byte)RTUParamKey._25)
                    {
                        rp = new RTUParam_25();
                    }
                    else if (Key == (byte)RTUParamKey._26)
                    {
                        rp = new RTUParam_26();
                    }
                    else if (Key == (byte)RTUParamKey._27)
                    {
                        rp = new RTUParam_27();
                    }
                    else if (Key == (byte)RTUParamKey._28)
                    {
                        rp = new RTUParam_28();
                    }
                    else if (Key == (byte)RTUParamKey._29)
                    {
                        rp = new RTUParam_29();
                    }
                    else if (Key == (byte)RTUParamKey._2A)
                    {
                        rp = new RTUParam_2A();
                    }
                    else if (Key == (byte)RTUParamKey._2B)
                    {
                        rp = new RTUParam_2B();
                    }
                    else if (Key == (byte)RTUParamKey._2C)
                    {
                        rp = new RTUParam_2C();
                    }
                    else if (Key == (byte)RTUParamKey._2D)
                    {
                        rp = new RTUParam_2D();
                    }
                    else if (Key == (byte)RTUParamKey._2E)
                    {
                        rp = new RTUParam_2E();
                    }
                    else if (Key == (byte)RTUParamKey._2F)
                    {
                        rp = new RTUParam_2F();
                    }
                    else if (Key == (byte)RTUParamKey._30)
                    {
                        rp = new RTUParam_30();
                    }
                    else if (Key == (byte)RTUParamKey._31)
                    {
                        rp = new RTUParam_31();
                    }
                    else if (Key == (byte)RTUParamKey._32)
                    {
                        rp = new RTUParam_32();
                    }
                    else if (Key == (byte)RTUParamKey._33)
                    {
                        rp = new RTUParam_33();
                    }
                    else if (Key == (byte)RTUParamKey._34)
                    {
                        rp = new RTUParam_34();
                    }
                    else if (Key == (byte)RTUParamKey._35)
                    {
                        rp = new RTUParam_35();
                    }
                    else if (Key == (byte)RTUParamKey._36)
                    {
                        rp = new RTUParam_36();
                    }
                    else if (Key == (byte)RTUParamKey._37)
                    {
                        rp = new RTUParam_37();
                    }
                    else if (Key == (byte)RTUParamKey._38)
                    {
                        rp = new RTUParam_38();
                    }
                    else if (Key == (byte)RTUParamKey._39)
                    {
                        rp = new RTUParam_39();
                    }
                    else if (Key == (byte)RTUParamKey._3A)
                    {
                        rp = new RTUParam_3A();
                    }
                    else if (Key == (byte)RTUParamKey._3B)
                    {
                        rp = new RTUParam_3B();
                    }
                    else if (Key == (byte)RTUParamKey._3C)
                    {
                        rp = new RTUParam_3C();
                    }
                    else if (Key == (byte)RTUParamKey._3D)
                    {
                        rp = new RTUParam_3D();
                    }
                    else if (Key == (byte)RTUParamKey._3E)
                    {
                        rp = new RTUParam_3E();
                    }
                    else if (Key == (byte)RTUParamKey._3F)
                    {
                        rp = new RTUParam_3F();
                    }
                    else if (Key == (byte)RTUParamKey._40)
                    {
                        rp = new RTUParam_40();
                    }
                    else if (Key == (byte)RTUParamKey._41)
                    {
                        rp = new RTUParam_41();
                    }
                    else if (Key == (byte)RTUParamKey._97)
                    {
                        rp = new RTUParam_97();
                    }
                    else if (Key == (byte)RTUParamKey._98)
                    {
                        rp = new RTUParam_98();
                    }
                    else
                    {
                        rp = new RTUParam_00(Key);
                    }

                    List_RTUParam.Add(rp);

                    Remain = Remain.Substring(4);
                }
                msg = "";
                return List_RTUParam;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return null;
            }
        }

        public static RTUParam GetRTUParam(List<RTUParam> list, byte key)
        {
            if (list == null) return null;
            foreach (RTUParam rp in list)
            {
                if (rp.GetKey() == key)
                    return rp;
            }
            return null;
        }

        public enum RTUParamKey
        {
            /// <summary>
            /// 中心站地址
            /// </summary>
            [Description("中心站地址")]
            _01 = 0x01,
            /// <summary>
            /// 遥测站地址
            /// </summary>
            [Description("遥测站地址")]
            _02 = 0x02,
            /// <summary>
            /// 密码
            /// </summary>
            [Description("密码")]
            _03 = 0x03,
            /// <summary>
            /// 中心站1主信道类型及地址
            /// </summary>
            [Description("中心站1主信道类型及地址")]
            _04 = 0x04,
            /// <summary>
            /// 中心站1备用信道类型及地址
            /// </summary>
            [Description("中心站1备用信道类型及地址")]
            _05 = 0x05,
            /// <summary>
            /// 中心站2主信道类型及地址
            /// </summary>
            [Description("中心站2主信道类型及地址")]
            _06 = 0x06,
            /// <summary>
            /// 中心站2备用信道类型及地址
            /// </summary>
            [Description("中心站2备用信道类型及地址")]
            _07 = 0x07,
            /// <summary>
            /// 中心站3主信道类型及地址
            /// </summary>
            [Description("中心站3主信道类型及地址")]
            _08 = 0x08,
            /// <summary>
            /// 中心站3备用信道类型及地址
            /// </summary>
            [Description("中心站3备用信道类型及地址")]
            _09 = 0x09,
            /// <summary>
            /// 中心站4主信道类型及地址
            /// </summary>
            [Description("中心站4主信道类型及地址")]
            _0A = 0x0A,
            /// <summary>
            /// 中心站4备用信道类型及地址
            /// </summary>
            [Description("中心站4备用信道类型及地址")]
            _0B = 0x0B,
            /// <summary>
            /// 工作方式
            /// </summary>
            [Description("工作方式")]
            _0C = 0x0C,
            /// <summary>
            /// 遥测站采集要素设置
            /// </summary>
            [Description("遥测站采集要素设置")]
            _0D = 0x0D,
            /// <summary>
            /// 中继站（集合转发站）服务地址范围
            /// </summary>
            [Description("中继站（集合转发站）服务地址范围")]
            _0E = 0x0E,
            /// <summary>
            /// 遥测站通信设备识别号
            /// </summary>
            [Description("遥测站通信设备识别号")]
            _0F = 0x0F,
            /// <summary>
            /// 定时报时间间隔
            /// </summary>
            [Description("定时报时间间隔")]
            _20 = 0x20,
            /// <summary>
            /// 加报时间间隔
            /// </summary>
            [Description("加报时间间隔")]
            _21 = 0x21,
            /// <summary>
            /// 降水量日起始时间
            /// </summary>
            [Description("降水量日起始时间")]
            _22 = 0x22,
            /// <summary>
            /// 采样间隔
            /// </summary>
            [Description("采样间隔")]
            _23 = 0x23,
            /// <summary>
            /// 水位数据存储间隔
            /// </summary>
            [Description("水位数据存储间隔")]
            _24 = 0x24,
            /// <summary>
            /// 雨量计分辨力
            /// </summary>
            [Description("雨量计分辨力")]
            _25 = 0x25,
            /// <summary>
            /// 水位计分辨力
            /// </summary>
            [Description("水位计分辨力")]
            _26 = 0x26,
            /// <summary>
            /// 雨量加报阈值
            /// </summary>
            [Description("雨量加报阈值")]
            _27 = 0x27,
            /// <summary>
            /// 水位基值1
            /// </summary>
            [Description("水位基值1")]
            _28 = 0x28,
            /// <summary>
            /// 水位基值2
            /// </summary>
            [Description("水位基值2")]
            _29 = 0x29,
            /// <summary>
            /// 水位基值3
            /// </summary>
            [Description("水位基值3")]
            _2A = 0x2A,
            /// <summary>
            /// 水位基值4
            /// </summary>
            [Description("水位基值4")]
            _2B = 0x2B,
            /// <summary>
            /// 水位基值5
            /// </summary>
            [Description("水位基值5")]
            _2C = 0x2C,
            /// <summary>
            /// 水位基值6
            /// </summary>
            [Description("水位基值6")]
            _2D = 0x2D,
            /// <summary>
            /// 水位基值7
            /// </summary>
            [Description("水位基值7")]
            _2E = 0x2E,
            /// <summary>
            /// 水位基值8
            /// </summary>
            [Description("水位基值8")]
            _2F = 0x2F,
            /// <summary>
            /// 水位修正基值1
            /// </summary>
            [Description("水位修正基值1")]
            _30 = 0x30,
            /// <summary>
            /// 水位修正基值2
            /// </summary>
            [Description("水位修正基值2")]
            _31 = 0x31,
            /// <summary>
            /// 水位修正基值3
            /// </summary>
            [Description("水位修正基值3")]
            _32 = 0x32,
            /// <summary>
            /// 水位修正基值4
            /// </summary>
            [Description("水位修正基值4")]
            _33 = 0x33,
            /// <summary>
            /// 水位修正基值5
            /// </summary>
            [Description("水位修正基值5")]
            _34 = 0x34,
            /// <summary>
            /// 水位修正基值6
            /// </summary>
            [Description("水位修正基值6")]
            _35 = 0x35,
            /// <summary>
            /// 水位修正基值7
            /// </summary>
            [Description("水位修正基值7")]
            _36 = 0x36,
            /// <summary>
            /// 水位修正基值8
            /// </summary>
            [Description("水位修正基值8")]
            _37 = 0x37,
            /// <summary>
            /// 加报水位1
            /// </summary>
            [Description("加报水位1")]
            _38 = 0x38,
            /// <summary>
            /// 加报水位2
            /// </summary>
            [Description("加报水位2")]
            _39 = 0x39,
            /// <summary>
            /// 加报水位3
            /// </summary>
            [Description("加报水位3")]
            _3A = 0x3A,
            /// <summary>
            /// 加报水位4
            /// </summary>
            [Description("加报水位4")]
            _3B = 0x3B,
            /// <summary>
            /// 加报水位5
            /// </summary>
            [Description("加报水位5")]
            _3C = 0x3C,
            /// <summary>
            /// 加报水位6
            /// </summary>
            [Description("加报水位6")]
            _3D = 0x3D,
            /// <summary>
            /// 加报水位7
            /// </summary>
            [Description("加报水位7")]
            _3E = 0x3E,
            /// <summary>
            /// 加报水位8
            /// </summary>
            [Description("加报水位8")]
            _3F = 0x3F,
            /// <summary>
            /// 加报水位以上加报阈值
            /// </summary>
            [Description("加报水位以上加报阈值")]
            _40 = 0x40,
            /// <summary>
            /// 加报水位以下加报阈值
            /// </summary>
            [Description("加报水位以下加报阈值")]
            _41 = 0x41,
            /// <summary>
            /// 固态存储数据初始化
            /// </summary>
            [Description("固态存储数据初始化")]
            _97 = 0x97,
            /// <summary>
            /// 遥测终端参数恢复出厂设置
            /// </summary>
            [Description("遥测终端参数恢复出厂设置")]
            _98 = 0x98
        }

        public enum ChannelType
        {
            /// <summary>
            /// 短信
            /// </summary>
            [Description("短信")]
            _01 = 0x01,
            /// <summary>
            /// IPV4
            /// </summary>
            [Description("IPV4")]
            _02 = 0x02,
            /// <summary>
            /// 北斗
            /// </summary>
            [Description("北斗")]
            _03 = 0x03,
            /// <summary>
            /// 海事卫星
            /// </summary>
            [Description("海事卫星")]
            _04 = 0x04,
            /// <summary>
            /// PSTN
            /// </summary>
            [Description("PSTN")]
            _05 = 0x05,
            /// <summary>
            /// 超短波
            /// </summary>
            [Description("超短波")]
            _06 = 0x06
        }

        public enum WorkType
        {
            /// <summary>
            /// 自报工作状态
            /// </summary>
            [Description("自报工作状态")]
            _01 = 0x01,
            /// <summary>
            /// 自报确认工作状态
            /// </summary>
            [Description("自报确认工作状态")]
            _02 = 0x02,
            /// <summary>
            /// 查询/应答工作状态
            /// </summary>
            [Description("查询/应答工作状态")]
            _03 = 0x03,
            /// <summary>
            /// 调试或维修状态
            /// </summary>
            [Description("调试或维修状态")]
            _04 = 0x04
        }

        public enum IdenCollection
        {
            /// <summary>
            /// 降水量
            /// </summary>
            [Description("降水量")]
            _A1D7 = 0x00,
            /// <summary>
            /// 蒸发量
            /// </summary>
            [Description("蒸发量")]
            _A1D6 = 0x01,
            /// <summary>
            /// 风向
            /// </summary>
            [Description("风向")]
            _A1D5 = 0x02,
            /// <summary>
            /// 风速
            /// </summary>
            [Description("风速")]
            _A1D4 = 0x03,
            /// <summary>
            /// 气温
            /// </summary>
            [Description("气温")]
            _A1D3 = 0x04,
            /// <summary>
            /// 湿度
            /// </summary>
            [Description("湿度")]
            _A1D2 = 0x05,
            /// <summary>
            /// 地温
            /// </summary>
            [Description("地温")]
            _A1D1 = 0x06,
            /// <summary>
            /// 气压
            /// </summary>
            [Description("气压")]
            _A1D0 = 0x07,
            /// <summary>
            /// 水位8
            /// </summary>
            [Description("水位8")]
            _A2D7 = 0x08,
            /// <summary>
            /// 水位7
            /// </summary>
            [Description("水位7")]
            _A2D6 = 0x09,
            /// <summary>
            /// 水位6
            /// </summary>
            [Description("水位6")]
            _A2D5 = 0x0A,
            /// <summary>
            /// 水位5
            /// </summary>
            [Description("水位5")]
            _A2D4 = 0x0B,
            /// <summary>
            /// 水位4
            /// </summary>
            [Description("水位4")]
            _A2D3 = 0x0C,
            /// <summary>
            /// 水位3
            /// </summary>
            [Description("水位3")]
            _A2D2 = 0x0D,
            /// <summary>
            /// 水位2
            /// </summary>
            [Description("水位2")]
            _A2D1 = 0x0E,
            /// <summary>
            /// 水位1
            /// </summary>
            [Description("水位1")]
            _A2D0 = 0x0F,
            /// <summary>
            /// 地下水埋深
            /// </summary>
            [Description("地下水埋深")]
            _A3D7 = 0x10,
            /// <summary>
            /// 图片
            /// </summary>
            [Description("图片")]
            _A3D6 = 0x11,
            /// <summary>
            /// 波浪
            /// </summary>
            [Description("波浪")]
            _A3D5 = 0x12,
            /// <summary>
            /// 闸门开度
            /// </summary>
            [Description("闸门开度")]
            _A3D4 = 0x13,
            /// <summary>
            /// 水量
            /// </summary>
            [Description("水量")]
            _A3D3 = 0x14,
            /// <summary>
            /// 流速
            /// </summary>
            [Description("流速")]
            _A3D2 = 0x15,
            /// <summary>
            /// 流量
            /// </summary>
            [Description("流量")]
            _A3D1 = 0x16,
            /// <summary>
            /// 水压
            /// </summary>
            [Description("水压")]
            _A3D0 = 0x17,
            /// <summary>
            /// 水表8
            /// </summary>
            [Description("水表8")]
            _A4D7 = 0x18,
            /// <summary>
            /// 水表7
            /// </summary>
            [Description("水表7")]
            _A4D6 = 0x19,
            /// <summary>
            /// 水表6
            /// </summary>
            [Description("水表6")]
            _A4D5 = 0x1A,
            /// <summary>
            /// 水表5
            /// </summary>
            [Description("水表5")]
            _A4D4 = 0x1B,
            /// <summary>
            /// 水表4
            /// </summary>
            [Description("水表4")]
            _A4D3 = 0x1C,
            /// <summary>
            /// 水表3
            /// </summary>
            [Description("水表3")]
            _A4D2 = 0x1D,
            /// <summary>
            /// 水表2
            /// </summary>
            [Description("水表2")]
            _A4D1 = 0x1E,
            /// <summary>
            /// 水表1
            /// </summary>
            [Description("水表1")]
            _A4D0 = 0x1F,
            /// <summary>
            /// 100CM墒情
            /// </summary>
            [Description("100CM墒情")]
            _A5D7 = 0x20,
            /// <summary>
            /// 80CM墒情
            /// </summary>
            [Description("80CM墒情")]
            _A5D6 = 0x21,
            /// <summary>
            /// 60CM墒情
            /// </summary>
            [Description("60CM墒情")]
            _A5D5 = 0x22,
            /// <summary>
            /// 50CM墒情
            /// </summary>
            [Description("50CM墒情")]
            _A5D4 = 0x23,
            /// <summary>
            /// 40CM墒情
            /// </summary>
            [Description("40CM墒情")]
            _A5D3 = 0x24,
            /// <summary>
            /// 30CM墒情
            /// </summary>
            [Description("30CM墒情")]
            _A5D2 = 0x25,
            /// <summary>
            /// 20CM墒情
            /// </summary>
            [Description("20CM墒情")]
            _A5D1 = 0x26,
            /// <summary>
            /// 10CM墒情
            /// </summary>
            [Description("10CM墒情")]
            _A5D0 = 0x27,
            /// <summary>
            /// pH值
            /// </summary>
            [Description("pH值")]
            _A6D7 = 0x28,
            /// <summary>
            /// 溶解氧
            /// </summary>
            [Description("溶解氧")]
            _A6D6 = 0x29,
            /// <summary>
            /// 电导率
            /// </summary>
            [Description("电导率")]
            _A6D5 = 0x2A,
            /// <summary>
            /// 浊度
            /// </summary>
            [Description("浊度")]
            _A6D4 = 0x2B,
            /// <summary>
            /// 氧化还原电位
            /// </summary>
            [Description("氧化还原电位")]
            _A6D3 = 0x2C,
            /// <summary>
            /// 高锰酸盐指数
            /// </summary>
            [Description("高锰酸盐指数")]
            _A6D2 = 0x2D,
            /// <summary>
            /// 氨氮
            /// </summary>
            [Description("氨氮")]
            _A6D1 = 0x2E,
            /// <summary>
            /// 水温
            /// </summary>
            [Description("水温")]
            _A6D0 = 0x2F,
            /// <summary>
            /// 总有机碳
            /// </summary>
            [Description("总有机碳")]
            _A7D7 = 0x30,
            /// <summary>
            /// 总氮
            /// </summary>
            [Description("总氮")]
            _A7D6 = 0x31,
            /// <summary>
            /// 总磷
            /// </summary>
            [Description("总磷")]
            _A7D5 = 0x32,
            /// <summary>
            /// 锌
            /// </summary>
            [Description("锌")]
            _A7D4 = 0x33,
            /// <summary>
            /// 硒
            /// </summary>
            [Description("硒")]
            _A7D3 = 0x34,
            /// <summary>
            /// 砷
            /// </summary>
            [Description("砷")]
            _A7D2 = 0x35,
            /// <summary>
            /// 总汞
            /// </summary>
            [Description("总汞")]
            _A7D1 = 0x36,
            /// <summary>
            /// 镉
            /// </summary>
            [Description("镉")]
            _A7D0 = 0x37,
            /// <summary>
            /// A8D7
            /// </summary>
            [Description("A8D7")]
            _A8D7 = 0x38,
            /// <summary>
            /// A8D6
            /// </summary>
            [Description("A8D6")]
            _A8D6 = 0x39,
            /// <summary>
            /// A8D5
            /// </summary>
            [Description("A8D5")]
            _A8D5 = 0x3A,
            /// <summary>
            /// A8D4
            /// </summary>
            [Description("A8D4")]
            _A8D4 = 0x3B,
            /// <summary>
            /// A8D3
            /// </summary>
            [Description("A8D3")]
            _A8D3 = 0x3C,
            /// <summary>
            /// 叶绿素a
            /// </summary>
            [Description("叶绿素a")]
            _A8D2 = 0x3D,
            /// <summary>
            /// 铜
            /// </summary>
            [Description("铜")]
            _A8D1 = 0x3E,
            /// <summary>
            /// 铅
            /// </summary>
            [Description("铅")]
            _A8D0 = 0x3F
        }

        public enum SimNoType
        {
            /// <summary>
            /// 移动通信卡
            /// </summary>
            [Description("移动通信卡")]
            _01 = 0x01,
            /// <summary>
            /// 北斗卫星通信卡
            /// </summary>
            [Description("北斗卫星通信卡")]
            _02 = 0x02
        }
    }
}
