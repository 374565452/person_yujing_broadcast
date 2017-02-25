using Common;
using Maticsoft.Model;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace CardOperationSystem
{
    class InfoSys
    {
        static log4net.ILog myLogger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static string LoginUserName = "";
        public static string LoginPassword = "";
        public static bool LoginIsLogin = false;

        public static long UserId = 0;
        public static string UserTrueName = "";
        public static string UserGroupName = "";
        public static string UserDistrictName = "";

        public static void GetBaseInfo()
        {
            if (LoginUserName == "" || LoginPassword == "" || !LoginIsLogin)
            {
                myLogger.Error("无法获取基本信息");
                return;
            }

            try
            {
                string str = DataTransfer.GetDistrictList();

                JavaScriptObject result = (JavaScriptObject)JavaScriptConvert.DeserializeObject(str);
                if (bool.Parse(result["Result"].ToString()))
                {
                    ListDistricts = JavaScriptConvert.DeserializeObject<List<District>>(result["Message"].ToString());
                }
                else
                {
                    myLogger.Error("获取ListDistricts失败");
                }
            }
            catch
            {
                myLogger.Error("无法获取ListDistricts");
            }

            lock (_DistrictCollection)
            {
                _DistrictCollection.Clear();
                foreach (District district in ListDistricts)
                {
                    _DistrictCollection.Add(district.Id, district);
                }
            }

            try
            {
                string str = DataTransfer.GetDeviceList();

                JavaScriptObject result = (JavaScriptObject)JavaScriptConvert.DeserializeObject(str);
                if (bool.Parse(result["Result"].ToString()))
                {
                    ListDevices = JavaScriptConvert.DeserializeObject<List<Device>>(result["Message"].ToString());
                }
                else
                {
                    myLogger.Error("获取ListDevices失败");
                }
            }
            catch
            {
                myLogger.Error("无法获取ListDevices");
            }

            try
            {
                string str = DataTransfer.GetTypeCodeList();

                JavaScriptObject result = (JavaScriptObject)JavaScriptConvert.DeserializeObject(str);
                if (bool.Parse(result["Result"].ToString()))
                {
                    ListDeviceTypeCodes = JavaScriptConvert.DeserializeObject<List<DeviceTypeCode>>(result["Message"].ToString());
                }
                else
                {
                    myLogger.Error("获取ListDeviceTypeCodes失败");
                }
            }
            catch
            {
                myLogger.Error("无法获取ListDeviceTypeCodes");
            }
        }

        public static List<District> ListDistricts;
        public static Dictionary<long, District> _DistrictCollection = new Dictionary<long, District>();
        public static List<Device> ListDevices;
        public static List<DeviceTypeCode> ListDeviceTypeCodes;

        public static District GetDistrictById(long Id)
        {
            lock (_DistrictCollection)
            {
                if (_DistrictCollection.ContainsKey(Id))
                    return _DistrictCollection[Id];
            }
            return null;
        }

        public static string KeyControl = "FF078069";
        public static string KeyA = "f".PadRight(12, 'f');
        public static string KeyB = "e".PadRight(12, 'e');

        public static string NetCardIP = "";
        public static string NetCardMAC = "";
        public static string HostName = "";

        public static string CardTypeUser = "01";
        public static string CardTypeDevice = "02";
        public static string CardTypeRead = "03";
        public static string CardTypeClear = "04";
        public static string CardTypeNetSet = "05";

        public static string CardTypeStrUser = "用户卡";
        public static string CardTypeStrDevice = "设置卡";
        public static string CardTypeStrRead = "读取卡";
        public static string CardTypeStrClear = "清零卡";
        public static string CardTypeStrNetSet = "网络设置卡";

        public static string MethodOpenCard = "开卡";
        public static string MethodReadCard = "读卡";
        public static string MethodModifyCard = "修改卡";
        public static string MethodCancelCard = "注销卡";

        public static string StrSeekSuccess = "寻卡成功";
        public static string StrSeekFailure = "寻卡失败";
        public static string StrAuthSuccess = "认证成功";
        public static string StrAuthFailure = "认证失败";
        public static string StrOpenSuccess = "开卡成功";
        public static string StrOpenFailure = "开卡失败";
        public static string StrReadSuccess = "读卡成功";
        public static string StrReadFailure = "读卡失败";
        public static string StrModifySuccess = "修改成功";
        public static string StrModifyFailure = "修改失败";
        public static string StrCancelSuccess = "注销成功";
        public static string StrCancelFailure = "注销失败";
        public static string StrUnknown = "未知";
        public static string StrState = "状态";
        public static string StrCannotOpen = "非新卡，无法开卡！";
        public static string StrCannotRead = "无法读卡！";

        public static string InfoWriteFailure(int sec, int block, string CardTypeStr, string Str)
        {
            return CardTypeStrClear + "扇区" + sec + "数据块" + block + "写入失败！" + Str;
        }

        public static string InfoReadFailure(int sec, int block, string CardTypeStr, string Str)
        {
            return CardTypeStrClear + "扇区" + sec + "数据块" + block + "读取失败！" + Str;
        }
    }
}
