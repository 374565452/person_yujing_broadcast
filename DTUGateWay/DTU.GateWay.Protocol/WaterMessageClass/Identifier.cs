using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace DTU.GateWay.Protocol.WaterMessageClass
{
    /// <summary>
    /// 要素标识符
    /// </summary>
    public abstract class Identifier
    {
        public abstract byte GetKey();

        public abstract byte GetLength();

        public abstract string GetHexStr();

        public abstract string GetHexStrHead();

        public abstract void SetVal(int ByteCount, int Digits, string hexStr);

        public static List<Identifier> analyse(string Remain, byte AFN, out string msg)
        {
            List<Identifier> List_Identifier = new List<Identifier>();

            try
            {
                while (Remain.Length >= 4)
                {
                    byte Key = Convert.ToByte(Remain.Substring(0, 2), 16);
                    string LengthStr = Remain.Substring(2, 2);
                    byte Length = Convert.ToByte(LengthStr, 16);

                    if (Key == (byte)Identifier_Standard._FF)
                    {
                        byte IdentifierSub = Convert.ToByte(Remain.Substring(2, 2), 16);
                        if (IdentifierSub == (byte)Identifier_Custom._03)
                        {
                            if (Remain.Length < 30)
                            {
                                msg = "长度不足，无法获取日水温" + " " + Remain;
                                return null;
                            }
                            Identifier_FF_03 iden = new Identifier_FF_03();
                            iden.SetVal(0, 0, Remain.Substring(6, 24));
                            List_Identifier.Add(iden);
                            Remain = Remain.Substring(30);
                        }
                        else if (IdentifierSub == (byte)Identifier_Custom._0E)
                        {
                            if (Remain.Length < 42)
                            {
                                msg = "长度不足，无法获取日埋深" + " " + Remain;
                                return null;
                            }
                            Identifier_FF_0E iden = new Identifier_FF_0E();
                            iden.SetVal(0, 0, Remain.Substring(6, 36));
                            List_Identifier.Add(iden);
                            Remain = Remain.Substring(42);
                        }
                        else
                        {
                            msg = "非法自定义标识符" + IdentifierSub + "" + " " + Remain;
                            return null;
                        }
                    }
                    else if (Key == (byte)Identifier_Standard._F0)
                    {
                        int ByteCount = 5;
                        if (Remain.Length < 2 + 2 + ByteCount * 2)
                        {
                            msg = "长度不足，无法获取标识符" + Key + "数据" + " " + Remain;
                            return null;
                        }
                        Identifier_F0 iden = new Identifier_F0();
                        iden.SetVal(0, 0, Remain.Substring(4, 10));
                        List_Identifier.Add(iden);
                        Remain = Remain.Substring(2 + 2 + ByteCount * 2);
                    }
                    else if (Key == (byte)Identifier_Standard._F1)
                    {
                        int ByteCount = 5;
                        if (Enum.IsDefined(typeof(Identifier_F1.HaveKey), (int)AFN))
                        {
                            ByteCount = 6;
                        }
                        if (Remain.Length < 2 + 2 + ByteCount * 2)
                        {
                            msg = "长度不足，无法获取标识符" + Key + "数据" + " " + Remain;
                            return null;
                        }
                        Identifier_F1 iden = new Identifier_F1();
                        iden.SetVal(0, 0, Remain.Substring(4, ByteCount * 2));
                        List_Identifier.Add(iden);
                        Remain = Remain.Substring(2 + 2 + ByteCount * 2);
                    }
                    else if (Key == (byte)Identifier_Standard._F2)
                    {
                        Identifier_F2 iden = new Identifier_F2();
                        iden.SetVal(0, 0, Remain.Substring(4));
                        List_Identifier.Add(iden);
                        msg = "";
                        return List_Identifier;
                    }
                    else if (Key == (byte)Identifier_Standard._F3)
                    {
                        Identifier_F3 iden = new Identifier_F3();
                        iden.SetVal(0, 0, Remain.Substring(4));
                        List_Identifier.Add(iden);
                        msg = "";
                        return List_Identifier;
                    }
                    else if (Key == (byte)Identifier_Standard._F4)
                    {
                        int ByteCount = 12;
                        if (Remain.Length < 2 + 2 + ByteCount * 2)
                        {
                            msg = "长度不足，无法获取标识符" + Key + "数据" + " " + Remain;
                            return null;
                        }
                        string ValStr = Remain.Substring(4, ByteCount * 2);
                        Identifier_F4 iden = new Identifier_F4();
                        iden.SetVal(0, 0, ValStr);
                        List_Identifier.Add(iden);
                        Remain = Remain.Substring(2 + 2 + ByteCount * 2);
                    }
                    else if (Key == (byte)Identifier_Standard._F5)
                    {
                        int ByteCount = 24;
                        if (Remain.Length < 2 + 2 + ByteCount * 2)
                        {
                            msg = "长度不足，无法获取标识符" + Key + "数据" + " " + Remain;
                            return null;
                        }
                        string ValStr = Remain.Substring(4, ByteCount * 2);
                        Identifier_F5 iden = new Identifier_F5();
                        iden.SetVal(0, 0, ValStr);
                        List_Identifier.Add(iden);
                        Remain = Remain.Substring(2 + 2 + ByteCount * 2);
                    }
                    else if (Key == (byte)Identifier_Standard._F6)
                    {
                        int ByteCount = 24;
                        if (Remain.Length < 2 + 2 + ByteCount * 2)
                        {
                            msg = "长度不足，无法获取标识符" + Key + "数据" + " " + Remain;
                            return null;
                        }
                        string ValStr = Remain.Substring(4, ByteCount * 2);
                        Identifier_F6 iden = new Identifier_F6();
                        iden.SetVal(0, 0, ValStr);
                        List_Identifier.Add(iden);
                        Remain = Remain.Substring(2 + 2 + ByteCount * 2);
                    }
                    else if (Key == (byte)Identifier_Standard._F7)
                    {
                        int ByteCount = 24;
                        if (Remain.Length < 2 + 2 + ByteCount * 2)
                        {
                            msg = "长度不足，无法获取标识符" + Key + "数据" + " " + Remain;
                            return null;
                        }
                        string ValStr = Remain.Substring(4, ByteCount * 2);
                        Identifier_F7 iden = new Identifier_F7();
                        iden.SetVal(0, 0, ValStr);
                        List_Identifier.Add(iden);
                        Remain = Remain.Substring(2 + 2 + ByteCount * 2);
                    }
                    else if (Key == (byte)Identifier_Standard._F8)
                    {
                        int ByteCount = 24;
                        if (Remain.Length < 2 + 2 + ByteCount * 2)
                        {
                            msg = "长度不足，无法获取标识符" + Key + "数据" + " " + Remain;
                            return null;
                        }
                        string ValStr = Remain.Substring(4, ByteCount * 2);
                        Identifier_F8 iden = new Identifier_F8();
                        iden.SetVal(0, 0, ValStr);
                        List_Identifier.Add(iden);
                        Remain = Remain.Substring(2 + 2 + ByteCount * 2);
                    }
                    else if (Key == (byte)Identifier_Standard._F9)
                    {
                        int ByteCount = 24;
                        if (Remain.Length < 2 + 2 + ByteCount * 2)
                        {
                            msg = "长度不足，无法获取标识符" + Key + "数据" + " " + Remain;
                            return null;
                        }
                        string ValStr = Remain.Substring(4, ByteCount * 2);
                        Identifier_F9 iden = new Identifier_F9();
                        iden.SetVal(0, 0, ValStr);
                        List_Identifier.Add(iden);
                        Remain = Remain.Substring(2 + 2 + ByteCount * 2);
                    }
                    else if (Key == (byte)Identifier_Standard._FA)
                    {
                        int ByteCount = 24;
                        if (Remain.Length < 2 + 2 + ByteCount * 2)
                        {
                            msg = "长度不足，无法获取标识符" + Key + "数据" + " " + Remain;
                            return null;
                        }
                        string ValStr = Remain.Substring(4, ByteCount * 2);
                        Identifier_FA iden = new Identifier_FA();
                        iden.SetVal(0, 0, ValStr);
                        List_Identifier.Add(iden);
                        Remain = Remain.Substring(2 + 2 + ByteCount * 2);
                    }
                    else if (Key == (byte)Identifier_Standard._FB)
                    {
                        int ByteCount = 24;
                        if (Remain.Length < 2 + 2 + ByteCount * 2)
                        {
                            msg = "长度不足，无法获取标识符" + Key + "数据" + " " + Remain;
                            return null;
                        }
                        string ValStr = Remain.Substring(4, ByteCount * 2);
                        Identifier_FB iden = new Identifier_FB();
                        iden.SetVal(0, 0, ValStr);
                        List_Identifier.Add(iden);
                        Remain = Remain.Substring(2 + 2 + ByteCount * 2);
                    }
                    else if (Key == (byte)Identifier_Standard._FC)
                    {
                        int ByteCount = 24;
                        if (Remain.Length < 2 + 2 + ByteCount * 2)
                        {
                            msg = "长度不足，无法获取标识符" + Key + "数据" + " " + Remain;
                            return null;
                        }
                        string ValStr = Remain.Substring(4, ByteCount * 2);
                        Identifier_FC iden = new Identifier_FC();
                        iden.SetVal(0, 0, ValStr);
                        List_Identifier.Add(iden);
                        Remain = Remain.Substring(2 + 2 + ByteCount * 2);
                    }
                    else if (Key == (byte)Identifier_Standard._FD)
                    {
                        Identifier_FD iden = new Identifier_FD();
                        iden.SetVal(0, 0, Remain.Substring(4));
                        List_Identifier.Add(iden);
                        msg = "";
                        return List_Identifier;
                    }
                    else
                    {
                        int[] LS = WaterBaseProtocol.GetLengthList(LengthStr);
                        int ByteCount = LS[0];
                        int Digits = LS[1];
                        if (Remain.Length < 2 + 2 + ByteCount * 2)
                        {
                            msg = "长度不足，无法获取标识符" + Key + "数据" + " " + Remain;
                            return null;
                        }

                        string ValStr = Remain.Substring(4, ByteCount * 2);
                        Identifier iden;
                        if (Key == (byte)Identifier_Standard._03)
                        {
                            iden = new Identifier_03();
                        }
                        else if (Key == (byte)Identifier_Standard._04)
                        {
                            iden = new Identifier_04();
                        }
                        else if (Key == (byte)Identifier_Standard._0E)
                        {
                            iden = new Identifier_0E();
                        }
                        else if (Key == (byte)Identifier_Standard._1A)
                        {
                            iden = new Identifier_1A();
                        }
                        else if (Key == (byte)Identifier_Standard._1F)
                        {
                            iden = new Identifier_1F();
                        }
                        else if (Key == (byte)Identifier_Standard._20)
                        {
                            iden = new Identifier_20();
                        }
                        else if (Key == (byte)Identifier_Standard._26)
                        {
                            iden = new Identifier_26();
                        }
                        else if (Key == (byte)Identifier_Standard._38)
                        {
                            iden = new Identifier_38();
                        }
                        else if (Key == (byte)Identifier_Standard._39)
                        {
                            iden = new Identifier_39();
                        }
                        else if (Key == (byte)Identifier_Standard._45)
                        {
                            iden = new Identifier_45();
                        }
                        else if (Key == (byte)Identifier_Standard._70)
                        {
                            iden = new Identifier_70();
                        }
                        else if (Key == (byte)Identifier_Standard._71)
                        {
                            iden = new Identifier_71();
                        }
                        else if (Key == (byte)Identifier_Standard._72)
                        {
                            iden = new Identifier_72();
                        }
                        else if (Key == (byte)Identifier_Standard._73)
                        {
                            iden = new Identifier_73();
                        }
                        else if (Key == (byte)Identifier_Standard._74)
                        {
                            iden = new Identifier_74();
                        }
                        else if (Key == (byte)Identifier_Standard._75)
                        {
                            iden = new Identifier_75();
                        }
                        else
                        {
                            iden = new Identifier_00(Key);
                        }

                        iden.SetVal(ByteCount, Digits, ValStr);
                        List_Identifier.Add(iden);
                        Remain = Remain.Substring(2 + 2 + ByteCount * 2);
                    }
                }

                msg = "";
                return List_Identifier;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return null;
            }
        }

        public static List<Identifier> analyse2(string Remain, out string msg)
        { 
            List<Identifier> List_Identifier = new List<Identifier>();

            try
            {
                while (Remain.Length >= 4)
                {
                    byte Key = Convert.ToByte(Remain.Substring(0, 2), 16);
                    if (Key == (byte)Identifier_Standard._FF)
                    {
                        byte IdentifierSub = Convert.ToByte(Remain.Substring(2, 2), 16);
                        if (IdentifierSub == (byte)Identifier_Custom._03)
                        {
                            Identifier_FF_03 iden = new Identifier_FF_03();
                            List_Identifier.Add(iden);
                            Remain = Remain.Substring(6);
                        }
                        else if (IdentifierSub == (byte)Identifier_Custom._0E)
                        {
                            Identifier_FF_0E iden = new Identifier_FF_0E();
                            List_Identifier.Add(iden);
                            Remain = Remain.Substring(6);
                        }
                        else
                        {
                            msg = "非法自定义标识符" + IdentifierSub + "" + " " + Remain;
                            return null;
                        }
                    }
                    else if (Key == (byte)Identifier_Standard._F0)
                    {
                        Identifier_F0 iden = new Identifier_F0();
                        List_Identifier.Add(iden);
                        Remain = Remain.Substring(4);
                    }
                    else if (Key == (byte)Identifier_Standard._F1)
                    {
                        Identifier_F1 iden = new Identifier_F1();
                        List_Identifier.Add(iden);
                        Remain = Remain.Substring(4);
                    }
                    else if (Key == (byte)Identifier_Standard._F2)
                    {
                        Identifier_F2 iden = new Identifier_F2();
                        List_Identifier.Add(iden);
                        Remain = Remain.Substring(4);
                    }
                    else if (Key == (byte)Identifier_Standard._F3)
                    {
                        Identifier_F3 iden = new Identifier_F3();
                        List_Identifier.Add(iden);
                        Remain = Remain.Substring(4);
                    }
                    else if (Key == (byte)Identifier_Standard._F4)
                    {
                        Identifier_F4 iden = new Identifier_F4();
                        List_Identifier.Add(iden);
                        Remain = Remain.Substring(4);
                    }
                    else if (Key == (byte)Identifier_Standard._F5)
                    {
                        Identifier_F5 iden = new Identifier_F5();
                        List_Identifier.Add(iden);
                        Remain = Remain.Substring(4);
                    }
                    else if (Key == (byte)Identifier_Standard._F6)
                    {
                        Identifier_F6 iden = new Identifier_F6();
                        List_Identifier.Add(iden);
                        Remain = Remain.Substring(4);
                    }
                    else if (Key == (byte)Identifier_Standard._F7)
                    {
                        Identifier_F7 iden = new Identifier_F7();
                        List_Identifier.Add(iden);
                        Remain = Remain.Substring(4);
                    }
                    else if (Key == (byte)Identifier_Standard._F8)
                    {
                        Identifier_F8 iden = new Identifier_F8();
                        List_Identifier.Add(iden);
                        Remain = Remain.Substring(4);
                    }
                    else if (Key == (byte)Identifier_Standard._F9)
                    {
                        Identifier_F9 iden = new Identifier_F9();
                        List_Identifier.Add(iden);
                        Remain = Remain.Substring(4);
                    }
                    else if (Key == (byte)Identifier_Standard._FA)
                    {
                        Identifier_FA iden = new Identifier_FA();
                        List_Identifier.Add(iden);
                        Remain = Remain.Substring(4);
                    }
                    else if (Key == (byte)Identifier_Standard._FB)
                    {
                        Identifier_FB iden = new Identifier_FB();
                        List_Identifier.Add(iden);
                        Remain = Remain.Substring(4);
                    }
                    else if (Key == (byte)Identifier_Standard._FC)
                    {
                        Identifier_FC iden = new Identifier_FC();
                        List_Identifier.Add(iden);
                        Remain = Remain.Substring(4);
                    }
                    else if (Key == (byte)Identifier_Standard._FD)
                    {
                        Identifier_FD iden = new Identifier_FD();
                        List_Identifier.Add(iden);
                        Remain = Remain.Substring(4);
                    }
                    else
                    {
                        Identifier iden;
                        if (Key == (byte)Identifier_Standard._03)
                        {
                            iden = new Identifier_03();
                        }
                        else if (Key == (byte)Identifier_Standard._04)
                        {
                            iden = new Identifier_04();
                        }
                        else if (Key == (byte)Identifier_Standard._0E)
                        {
                            iden = new Identifier_0E();
                        }
                        else if (Key == (byte)Identifier_Standard._1A)
                        {
                            iden = new Identifier_1A();
                        }
                        else if (Key == (byte)Identifier_Standard._1F)
                        {
                            iden = new Identifier_1F();
                        }
                        else if (Key == (byte)Identifier_Standard._20)
                        {
                            iden = new Identifier_20();
                        }
                        else if (Key == (byte)Identifier_Standard._26)
                        {
                            iden = new Identifier_26();
                        }
                        else if (Key == (byte)Identifier_Standard._38)
                        {
                            iden = new Identifier_38();
                        }
                        else if (Key == (byte)Identifier_Standard._39)
                        {
                            iden = new Identifier_39();
                        }
                        else if (Key == (byte)Identifier_Standard._45)
                        {
                            iden = new Identifier_45();
                        }
                        else if (Key == (byte)Identifier_Standard._70)
                        {
                            iden = new Identifier_70();
                        }
                        else if (Key == (byte)Identifier_Standard._71)
                        {
                            iden = new Identifier_71();
                        }
                        else if (Key == (byte)Identifier_Standard._72)
                        {
                            iden = new Identifier_72();
                        }
                        else if (Key == (byte)Identifier_Standard._73)
                        {
                            iden = new Identifier_73();
                        }
                        else if (Key == (byte)Identifier_Standard._74)
                        {
                            iden = new Identifier_74();
                        }
                        else if (Key == (byte)Identifier_Standard._75)
                        {
                            iden = new Identifier_75();
                        }
                        else
                        {
                            iden = new Identifier_00(Key);
                        }
                        List_Identifier.Add(iden);
                    }
                }

                msg = "";
                return List_Identifier;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return null;
            }
        }
    }

    /// <summary>
    /// 标准要素标识符与数据定义
    /// </summary>
    public enum Identifier_Standard
    {
        /// <summary>
        /// 瞬时气温，摄氏度，N(3,1)
        /// </summary>
        [Description("瞬时气温，摄氏度，N(3,1)")]
        _02 = 0x02,
        /// <summary>
        /// 瞬时水温，摄氏度，N(3,1)
        /// </summary>
        [Description("瞬时水温，摄氏度，N(3,1)")]
        _03 = 0x03,
        /// <summary>
        /// 时间步长码，，N(3)
        /// </summary>
        [Description("时间步长码，，N(3)")]
        _04 = 0x04,
        /// <summary>
        /// 气压，百帕，N(5)
        /// </summary>
        [Description("气压，百帕，N(5)")]
        _08 = 0x08,
        /// <summary>
        /// 地下水埋深，米，N(6,2)
        /// </summary>
        [Description("地下水瞬时埋深，米，N(6,2)")]
        _0E = 0x0E,
        /// <summary>
        /// 1小时时段降水量，毫米，N(5,1)
        /// </summary>
        [Description("1小时时段降水量，毫米，N(5,1)")]
        _1A = 0x1A,
        /// <summary>
        /// 日降水量，毫米，N(5,1)
        /// </summary>
        [Description("日降水量，毫米，N(5,1)")]
        _1F = 0x1F,
        /// <summary>
        /// 当前降水量，毫米，N(5,1)
        /// </summary>
        [Description("当前降水量，毫米，N(5,1)")]
        _20 = 0x20,
        /// <summary>
        /// 降水量累计值，毫米，N(6,1)
        /// </summary>
        [Description("降水量累计值，毫米，N(6,1)")]
        _26 = 0x26,
        /// <summary>
        /// 电源电压，伏特，N(4,2)
        /// </summary>
        [Description("电源电压，伏特，N(4,2)")]
        _38 = 0x38,
        /// <summary>
        /// 瞬时河道水位、潮位，米，N(7,3)
        /// </summary>
        [Description("瞬时河道水位、潮位，米，N(7,3)")]
        _39 = 0x39,
        /// <summary>
        /// 遥测站状态及报警信息，，4 字节 HEX
        /// </summary>
        [Description("遥测站状态及报警信息，，4 字节 HEX")]
        _45 = 0x45,
        /// <summary>
        /// pH，，N(4,2)
        /// </summary>
        [Description("pH，，N(4,2)")]
        _46 = 0x46,
        /// <summary>
        /// 电导率COND，微西门/厘米，N(5)
        /// </summary>
        [Description("电导率COND，微西门/厘米，N(5)")]
        _48 = 0x48,
        /// <summary>
        /// 浊度TURB，度，N(3)
        /// </summary>
        [Description("浊度TURB，度，N(3)")]
        _49 = 0x49,
        /// <summary>
        /// 氨氮NH3N，毫克/升，N(6,3)
        /// </summary>
        [Description("氨氮NH3N，毫克/升，N(6,3)")]
        _4C = 0x4C,
        /// <summary>
        /// 交流A相电压，伏特，N(4,1)
        /// </summary>
        [Description("交流A相电压，伏特，N(4,1)")]
        _70 = 0x70,
        /// <summary>
        /// 交流B相电压，伏特，N(4,1)
        /// </summary>
        [Description("交流B相电压，伏特，N(4,1)")]
        _71 = 0x71,
        /// <summary>
        /// 交流C相电压，伏特，N(4,1)
        /// </summary>
        [Description("交流C相电压，伏特，N(4,1)")]
        _72 = 0x72,
        /// <summary>
        /// 交流A相电压，伏特，N(4,1)
        /// </summary>
        [Description("交流A相电流，安培，N(4,1)")]
        _73 = 0x73,
        /// <summary>
        /// 交流A相电压，伏特，N(4,1)
        /// </summary>
        [Description("交流B相电流，安培，N(4,1)")]
        _74 = 0x74,
        /// <summary>
        /// 交流A相电压，伏特，N(4,1)
        /// </summary>
        [Description("交流C相电流，安培，N(4,1)")]
        _75 = 0x75,
        /// <summary>
        /// 泉流量水位，米，N(5,3)
        /// </summary>
        [Description("泉流量水位，米，N(5,3)")]
        _76 = 0x76,
        /// <summary>
        /// 硝酸盐氮NO3，毫克/升，N(5,3)
        /// </summary>
        [Description("硝酸盐氮NO3，毫克/升，N(5,3)")]
        _77 = 0x77,
        /// <summary>
        /// 氟化物F，毫克/升，N(5,2)
        /// </summary>
        [Description("氟化物F，毫克/升，N(5,2)")]
        _78 = 0x78,
        /// <summary>
        /// 氯离子，毫克/升，N(5,2)
        /// </summary>
        [Description("氯离子，毫克/升，N(5,2)")]
        _79 = 0x79,
        /// <summary>
        /// 信号强度，，N(2)
        /// </summary>
        [Description("信号强度，，N(2)")]
        _7A = 0x7A,
        /// <summary>
        /// 观测时间引导符，，N(10)
        /// </summary>
        [Description("观测时间引导符，，N(10)")]
        _F0 = 0xF0,
        /// <summary>
        /// 测站编码引导符，，N(10)
        /// </summary>
        [Description("测站编码引导符，，N(10)")]
        _F1 = 0xF1,
        /// <summary>
        /// 人工置数，d 字节，C(d)1
        /// </summary>
        [Description("人工置数，d 字节，C(d)1")]
        _F2 = 0xF2,
        /// <summary>
        /// 图片信息，KB，JPG格式
        /// </summary>
        [Description("图片信息，KB，JPG格式")]
        _F3 = 0xF3,
        /// <summary>
        /// 1小时内每5分钟时段雨量，0.1 毫米，12 字节
        /// </summary>
        [Description("1小时内每5分钟时段雨量，0.1毫米，12字节")]
        _F4 = 0xF4,
        /// <summary>
        /// 1小时内5分钟间隔相对水位1，0.1 毫米，12 字节
        /// </summary>
        [Description("1小时内5分钟间隔相对水位1，0.01米，24字节")]
        _F5 = 0xF5,
        /// <summary>
        /// 1小时内5分钟间隔相对水位2，0.1 毫米，12 字节
        /// </summary>
        [Description("1小时内5分钟间隔相对水位2，0.01米，24字节")]
        _F6 = 0xF6,
        /// <summary>
        /// 1小时内5分钟间隔相对水位3，0.1 毫米，12 字节
        /// </summary>
        [Description("1小时内5分钟间隔相对水位3，0.01米，24字节")]
        _F7 = 0xF7,
        /// <summary>
        /// 1小时内5分钟间隔相对水位4，0.1 毫米，12 字节
        /// </summary>
        [Description("1小时内5分钟间隔相对水位4，0.01米，24字节")]
        _F8 = 0xF8,
        /// <summary>
        /// 1小时内5分钟间隔相对水位5，0.1 毫米，12 字节
        /// </summary>
        [Description("1小时内5分钟间隔相对水位5，0.01米，24字节")]
        _F9 = 0xF9,
        /// <summary>
        /// 1小时内5分钟间隔相对水位6，0.1 毫米，12 字节
        /// </summary>
        [Description("1小时内5分钟间隔相对水位6，0.01米，24字节")]
        _FA = 0xFA,
        /// <summary>
        /// 1小时内5分钟间隔相对水位7，0.1 毫米，12 字节
        /// </summary>
        [Description("1小时内5分钟间隔相对水位7，0.01米，24字节")]
        _FB = 0xFB,
        /// <summary>
        /// 1小时内5分钟间隔相对水位8，0.1 毫米，12 字节
        /// </summary>
        [Description("1小时内5分钟间隔相对水位8，0.01米，24字节")]
        _FC = 0xFC,
        /// <summary>
        /// 流速批量数据传输，0.1 毫米，12 字节
        /// </summary>
        [Description("流速批量数据传输，，")]
        _FD = 0xFD,
        /// <summary>
        /// 自定义，，
        /// </summary>
        [Description("自定义，，")]
        _FF = 0xFF
    }

    /// <summary>
    /// 自定义要素标识符与数据定义
    /// </summary>
    public enum Identifier_Custom
    {
        /// <summary>
        /// 日水温，单位摄氏度，六组N(3,1)
        /// </summary>
        [Description("日水温，单位摄氏度，六组N(3,1)")]
        _03 = 0x03,
        /// <summary>
        /// 日埋深，单位米，六组N(6,2)
        /// </summary>
        [Description("日埋深，单位米，六组N(6,2)")]
        _0E = 0x0E
    }

    public enum AlarmState
    {
        /// <summary>
        /// 交流电充电状态
        /// </summary>
        [Description("交流电充电状态")]
        _01 = 0x01,
        /// <summary>
        /// 蓄电池电压状态
        /// </summary>
        [Description("蓄电池电压状态")]
        _02 = 0x02,
        /// <summary>
        /// 水位超限报警状态
        /// </summary>
        [Description("水位超限报警状态")]
        _03 = 0x03,
        /// <summary>
        /// 流量超限报警状态
        /// </summary>
        [Description("流量超限报警状态")]
        _04 = 0x04,
        /// <summary>
        /// 水质超限报警状态
        /// </summary>
        [Description("水质超限报警状态")]
        _05 = 0x05,
        /// <summary>
        /// 流量仪表状态
        /// </summary>
        [Description("流量仪表状态")]
        _06 = 0x06,
        /// <summary>
        /// 水位仪表状态
        /// </summary>
        [Description("水位仪表状态")]
        _07 = 0x07,
        /// <summary>
        /// 终端箱门状态
        /// </summary>
        [Description("终端箱门状态")]
        _08 = 0x08,
        /// <summary>
        /// 存储器状态
        /// </summary>
        [Description("存储器状态")]
        _09 = 0x09,
        /// <summary>
        /// IC卡功能有效
        /// </summary>
        [Description("IC卡功能有效")]
        _10 = 0x0A,
        /// <summary>
        /// 水泵工作状态
        /// </summary>
        [Description("水泵工作状态")]
        _11 = 0x0B,
        /// <summary>
        /// 剩余水量报警
        /// </summary>
        [Description("剩余水量报警")]
        _12 = 0x0C
    }
}
