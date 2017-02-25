using DTU.GateWay.Protocol.WaterMessageClass;
using System;

namespace SimClient.src
{
    public class ParamBase
    {
        public static RTUParam_01 RTUParam_01;
        public static RTUParam_02 RTUParam_02;
        public static RTUParam_03 RTUParam_03;
        public static RTUParam_04 RTUParam_04;
        public static RTUParam_05 RTUParam_05;
        public static RTUParam_06 RTUParam_06;
        public static RTUParam_07 RTUParam_07;
        public static RTUParam_08 RTUParam_08;
        public static RTUParam_09 RTUParam_09;
        public static RTUParam_0A RTUParam_0A;
        public static RTUParam_0B RTUParam_0B;
        public static RTUParam_0C RTUParam_0C;
        public static RTUParam_0D RTUParam_0D;
        public static RTUParam_0E RTUParam_0E;
        public static RTUParam_0F RTUParam_0F;

        public static void Init()
        {
            string CenterStation1 = ConfigHelper.GetAppConfig("CenterStation1");
            string CenterStation2 = ConfigHelper.GetAppConfig("CenterStation2");
            string CenterStation3 = ConfigHelper.GetAppConfig("CenterStation3");
            string CenterStation4 = ConfigHelper.GetAppConfig("CenterStation4");
            RTUParam_01 = new RTUParam_01();
            RTUParam_01.CenterStations = new int[] { 0, 0, 0, 0 };
            try
            {
                RTUParam_01.CenterStations[0] = Convert.ToInt32(CenterStation1 == null ? "0" : CenterStation1);
            }
            catch { }
            try
            {
                RTUParam_01.CenterStations[1] = Convert.ToInt32(CenterStation2 == null ? "0" : CenterStation2);
            }
            catch { }

            try
            {
                RTUParam_01.CenterStations[2] = Convert.ToInt32(CenterStation3 == null ? "0" : CenterStation3);
            }
            catch { }
            try
            {
                RTUParam_01.CenterStations[3] = Convert.ToInt32(CenterStation4 == null ? "0" : CenterStation4);
            }
            catch { }

            string RemoteStation = ConfigHelper.GetAppConfig("RemoteStation");
            RTUParam_02 = new RTUParam_02();
            RTUParam_02.RemoteStation = RemoteStation == null ? "" : RemoteStation;

            string Password = ConfigHelper.GetAppConfig("Password");
            RTUParam_03 = new RTUParam_03();
            RTUParam_03.Password = Password == null ? "0000" : Password;

            string ChannelType1_M = ConfigHelper.GetAppConfig("ChannelType1_M");
            string Add1_M = ConfigHelper.GetAppConfig("Add1_M");
            RTUParam_04 = new RTUParam_04();
            RTUParam_04.ChannelTypeV = 01;
            try
            {
                RTUParam_04.ChannelTypeV = Convert.ToInt32(ChannelType1_M == null ? "0" : ChannelType1_M);
            }
            catch { }
            RTUParam_04.IP = "";
            RTUParam_04.Port = 0;
            RTUParam_04.Add = "";
            if (RTUParam_04.ChannelTypeV == (int)RTUParam.ChannelType._02)
            {
                try
                {
                    RTUParam_04.IP = Add1_M == null ? "" : Add1_M.Split(':')[0];
                    RTUParam_04.Port = Convert.ToInt32(Add1_M == null ? "0" : Add1_M.Split(':')[1]);
                }
                catch { }
            }
            else
            {
                RTUParam_04.Add = Add1_M == null ? "" : Add1_M;
            }

            string ChannelType1_B = ConfigHelper.GetAppConfig("ChannelType1_B");
            string Add1_B = ConfigHelper.GetAppConfig("Add1_B");
            RTUParam_05 = new RTUParam_05();
            RTUParam_05.ChannelTypeV = 01;
            RTUParam_05.IP = "";
            RTUParam_05.Port = 0;
            RTUParam_05.Add = "";

            string ChannelType2_M = ConfigHelper.GetAppConfig("ChannelType2_M");
            string Add2_M = ConfigHelper.GetAppConfig("Add2_M");
            RTUParam_06 = new RTUParam_06();
            RTUParam_06.ChannelTypeV = 01;
            RTUParam_06.IP = "";
            RTUParam_06.Port = 0;
            RTUParam_06.Add = "";

            string ChannelType2_B = ConfigHelper.GetAppConfig("ChannelType2_B");
            string Add2_B = ConfigHelper.GetAppConfig("Add2_B");
            RTUParam_07 = new RTUParam_07();
            RTUParam_07.ChannelTypeV = 01;
            RTUParam_07.IP = "";
            RTUParam_07.Port = 0;
            RTUParam_07.Add = "";

            string ChannelType3_M = ConfigHelper.GetAppConfig("ChannelType3_M");
            string Add3_M = ConfigHelper.GetAppConfig("Add3_M");
            RTUParam_08 = new RTUParam_08();
            RTUParam_08.ChannelTypeV = 01;
            RTUParam_08.IP = "";
            RTUParam_08.Port = 0;
            RTUParam_08.Add = "";

            string ChannelType3_B = ConfigHelper.GetAppConfig("ChannelType3_B");
            string Add3_B = ConfigHelper.GetAppConfig("Add3_B");
            RTUParam_09 = new RTUParam_09();
            RTUParam_09.ChannelTypeV = 01;
            RTUParam_09.IP = "";
            RTUParam_09.Port = 0;
            RTUParam_09.Add = "";

            string ChannelType4_M = ConfigHelper.GetAppConfig("ChannelType4_M");
            string Add4_M = ConfigHelper.GetAppConfig("Add4_M");
            RTUParam_0A = new RTUParam_0A();
            RTUParam_0A.ChannelTypeV = 01;
            RTUParam_0A.IP = "";
            RTUParam_0A.Port = 0;
            RTUParam_0A.Add = "";

            string ChannelType4_B = ConfigHelper.GetAppConfig("ChannelType4_B");
            string Add4_B = ConfigHelper.GetAppConfig("Add4_B");
            RTUParam_0B = new RTUParam_0B();
            RTUParam_0B.ChannelTypeV = 01;
            RTUParam_0B.IP = "";
            RTUParam_0B.Port = 0;
            RTUParam_0B.Add = "";

            string WorkType = ConfigHelper.GetAppConfig("WorkType");
            RTUParam_0C = new RTUParam_0C();
            RTUParam_0C.WorkTypeV = 00;

            string BitStr = ConfigHelper.GetAppConfig("BitStr");
            RTUParam_0D = new RTUParam_0D();
            RTUParam_0D.BitStr = "";

            RTUParam_0E = new RTUParam_0E();

            string SimNoType = ConfigHelper.GetAppConfig("SimNoType");
            string SimNo = ConfigHelper.GetAppConfig("SimNo");
            RTUParam_0F = new RTUParam_0F();
            RTUParam_0F.SimNoTypeV = 00;
            RTUParam_0F.SimNo = "";
        }
    }
}
