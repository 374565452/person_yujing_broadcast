using Common;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace CardOperationSystem
{
    class DataTransfer
    {
        static log4net.ILog myLogger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static string ServerIP { set; get; }
        public static int ServerPort { set; get; }

        public static string getLoginUrl()
        {
            return "http://" + ServerIP + ":" + ServerPort + "/Ajaxjson.ashx";
        }

        //连接测试
        public static string HelloWorld(string ip, int port)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("Method", "HelloWorld");
            parameters.Add("NetCardMAC", InfoSys.NetCardMAC);

            JavaScriptObject result = new JavaScriptObject();
            result.Add("Result", false);
            result.Add("Message", "连接出错！");

            string url = "http://" + ip + ":" + port + "/Ajaxjson.ashx";

            string retString = HttpWebResponseUtility.getPostResponse(url, "HelloWorld", parameters, "", 
                JavaScriptConvert.SerializeObject(result));

            return retString;
        }

        //用户登录
        public static string UserLogin()
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("Method", "UserLogin");
            parameters.Add("NetCardMAC", InfoSys.NetCardMAC);
            parameters.Add("UserName", InfoSys.LoginUserName);
            parameters.Add("Password", InfoSys.LoginPassword);

            JavaScriptObject result = new JavaScriptObject();
            result.Add("Result", false);
            result.Add("Message", "登录出错！");

            string retString = HttpWebResponseUtility.getPostResponse(getLoginUrl(), "UserLogin", parameters, "", 
                JavaScriptConvert.SerializeObject(result));

            return retString;
        }

        //获取地区列表
        public static string GetDistrictList()
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("Method", "GetDistrictList");
            parameters.Add("NetCardMAC", InfoSys.NetCardMAC);
            parameters.Add("opUserName", InfoSys.LoginUserName);
            parameters.Add("opPassword", InfoSys.LoginPassword);

            JavaScriptObject result = new JavaScriptObject();
            result.Add("Result", false);
            result.Add("Message", "获取地址列表出错！");

            string retString = HttpWebResponseUtility.getPostResponse(getLoginUrl(), "GetDistrictList", parameters, "",
                JavaScriptConvert.SerializeObject(result));

            return retString;
        }

        //获取终端列表
        public static string GetDeviceList()
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("Method", "GetDeviceList");
            parameters.Add("NetCardMAC", InfoSys.NetCardMAC);
            parameters.Add("opUserName", InfoSys.LoginUserName);
            parameters.Add("opPassword", InfoSys.LoginPassword);

            JavaScriptObject result = new JavaScriptObject();
            result.Add("Result", false);
            result.Add("Message", "获取终端列表出错！");

            string retString = HttpWebResponseUtility.getPostResponse(getLoginUrl(), "GetDeviceList", parameters, "",
                JavaScriptConvert.SerializeObject(result));

            return retString;
        }

        //获取流量计类型
        public static string GetTypeCodeList()
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("Method", "GetTypeCodeList");
            parameters.Add("NetCardMAC", InfoSys.NetCardMAC);
            parameters.Add("opUserName", InfoSys.LoginUserName);
            parameters.Add("opPassword", InfoSys.LoginPassword);

            JavaScriptObject result = new JavaScriptObject();
            result.Add("Result", false);
            result.Add("Message", "获取流量计类型列表出错！");

            string retString = HttpWebResponseUtility.getPostResponse(getLoginUrl(), "GetTypeCodeList", parameters, "",
                JavaScriptConvert.SerializeObject(result));

            return retString;
        }

        //清零卡开卡
        public static string OpenCardClear(string SerialNumber)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("Method", "OpenCardClear");
            parameters.Add("NetCardMAC", InfoSys.NetCardMAC);
            parameters.Add("opUserId", InfoSys.UserId.ToString());
            parameters.Add("opUserName", InfoSys.LoginUserName);
            parameters.Add("opPassword", InfoSys.LoginPassword);
            parameters.Add("SerialNumber", SerialNumber);

            JavaScriptObject result = new JavaScriptObject();
            result.Add("Result", false);
            result.Add("Message", "清零卡开卡保存出错！");

            string retString = HttpWebResponseUtility.getPostResponse(getLoginUrl(), "OpenCardClear", parameters, "",
                JavaScriptConvert.SerializeObject(result));

            return retString;
        }

        //清零卡注销
        public static string CancelCardClear(string SerialNumber)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("Method", "CancelCardClear");
            parameters.Add("NetCardMAC", InfoSys.NetCardMAC);
            parameters.Add("opUserId", InfoSys.UserId.ToString());
            parameters.Add("opUserName", InfoSys.LoginUserName);
            parameters.Add("opPassword", InfoSys.LoginPassword);
            parameters.Add("SerialNumber", SerialNumber);

            JavaScriptObject result = new JavaScriptObject();
            result.Add("Result", false);
            result.Add("Message", "清零卡注销卡保存出错！");

            string retString = HttpWebResponseUtility.getPostResponse(getLoginUrl(), "CancelCardClear", parameters, "",
                JavaScriptConvert.SerializeObject(result));

            return retString;
        }

        //网络设置卡开卡
        public static string OpenCardNetSet(string SerialNumber, string IP, string Port, string IsDomain,
            string APNName, string APNUserName, string APNPassword)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("Method", "OpenCardNetSet");
            parameters.Add("NetCardMAC", InfoSys.NetCardMAC);
            parameters.Add("opUserId", InfoSys.UserId.ToString());
            parameters.Add("opUserName", InfoSys.LoginUserName);
            parameters.Add("opPassword", InfoSys.LoginPassword);
            parameters.Add("SerialNumber", SerialNumber);
            parameters.Add("IP", IP);
            parameters.Add("Port", Port);
            parameters.Add("IsDomain", IsDomain);
            parameters.Add("APNName", APNName);
            parameters.Add("APNUserName", APNUserName);
            parameters.Add("APNPassword", APNPassword);

            JavaScriptObject result = new JavaScriptObject();
            result.Add("Result", false);
            result.Add("Message", "网络设置卡开卡保存出错！");

            string retString = HttpWebResponseUtility.getPostResponse(getLoginUrl(), "OpenCardNetSet", parameters, "",
                JavaScriptConvert.SerializeObject(result));

            return retString;
        }

        //网络设置卡修改
        public static string ModifyCardNetSet(string SerialNumber, string IP, string Port, string IsDomain,
            string APNName, string APNUserName, string APNPassword)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("Method", "ModifyCardNetSet");
            parameters.Add("NetCardMAC", InfoSys.NetCardMAC);
            parameters.Add("opUserId", InfoSys.UserId.ToString());
            parameters.Add("opUserName", InfoSys.LoginUserName);
            parameters.Add("opPassword", InfoSys.LoginPassword);
            parameters.Add("SerialNumber", SerialNumber);
            parameters.Add("IP", IP);
            parameters.Add("Port", Port);
            parameters.Add("IsDomain", IsDomain);
            parameters.Add("APNName", APNName);
            parameters.Add("APNUserName", APNUserName);
            parameters.Add("APNPassword", APNPassword);

            JavaScriptObject result = new JavaScriptObject();
            result.Add("Result", false);
            result.Add("Message", "网络设置卡修改保存出错！");

            string retString = HttpWebResponseUtility.getPostResponse(getLoginUrl(), "ModifyCardNetSet", parameters, "",
                JavaScriptConvert.SerializeObject(result));

            return retString;
        }

        //网络设置卡注销
        public static string CancelCardNetSet(string SerialNumber)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("Method", "CancelCardNetSet");
            parameters.Add("NetCardMAC", InfoSys.NetCardMAC);
            parameters.Add("opUserId", InfoSys.UserId.ToString());
            parameters.Add("opUserName", InfoSys.LoginUserName);
            parameters.Add("opPassword", InfoSys.LoginPassword);
            parameters.Add("SerialNumber", SerialNumber);

            JavaScriptObject result = new JavaScriptObject();
            result.Add("Result", false);
            result.Add("Message", "网络设置卡注销保存出错！");

            string retString = HttpWebResponseUtility.getPostResponse(getLoginUrl(), "CancelCardNetSet", parameters, "",
                JavaScriptConvert.SerializeObject(result));

            return retString;
        }


        //读取卡开卡
        public static string OpenCardRead(string SerialNumber,
            string AddressCode1, string AddressCode2, string AddressCode3)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("Method", "OpenCardRead");
            parameters.Add("NetCardMAC", InfoSys.NetCardMAC);
            parameters.Add("opUserId", InfoSys.UserId.ToString());
            parameters.Add("opUserName", InfoSys.LoginUserName);
            parameters.Add("opPassword", InfoSys.LoginPassword);
            parameters.Add("SerialNumber", SerialNumber);
            parameters.Add("AddressCode1", AddressCode1);
            parameters.Add("AddressCode2", AddressCode2);
            parameters.Add("AddressCode3", AddressCode3);

            JavaScriptObject result = new JavaScriptObject();
            result.Add("Result", false);
            result.Add("Message", "读取卡开卡保存出错！");

            string retString = HttpWebResponseUtility.getPostResponse(getLoginUrl(), "OpenCardRead", parameters, "",
                JavaScriptConvert.SerializeObject(result));

            return retString;
        }

        //读取卡修改
        public static string ModifyCardRead(string SerialNumber,
            string AddressCode1, string AddressCode2, string AddressCode3)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("Method", "ModifyCardRead");
            parameters.Add("NetCardMAC", InfoSys.NetCardMAC);
            parameters.Add("opUserId", InfoSys.UserId.ToString());
            parameters.Add("opUserName", InfoSys.LoginUserName);
            parameters.Add("opPassword", InfoSys.LoginPassword);
            parameters.Add("SerialNumber", SerialNumber);
            parameters.Add("AddressCode1", AddressCode1);
            parameters.Add("AddressCode2", AddressCode2);
            parameters.Add("AddressCode3", AddressCode3);

            JavaScriptObject result = new JavaScriptObject();
            result.Add("Result", false);
            result.Add("Message", "读取卡修改保存出错！");

            string retString = HttpWebResponseUtility.getPostResponse(getLoginUrl(), "ModifyCardRead", parameters, "",
                JavaScriptConvert.SerializeObject(result));

            return retString;
        }

        //读取卡注销
        public static string CancelCardRead(string SerialNumber)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("Method", "CancelCardRead");
            parameters.Add("NetCardMAC", InfoSys.NetCardMAC);
            parameters.Add("opUserId", InfoSys.UserId.ToString());
            parameters.Add("opUserName", InfoSys.LoginUserName);
            parameters.Add("opPassword", InfoSys.LoginPassword);
            parameters.Add("SerialNumber", SerialNumber);

            JavaScriptObject result = new JavaScriptObject();
            result.Add("Result", false);
            result.Add("Message", "读取卡注销保存出错！");

            string retString = HttpWebResponseUtility.getPostResponse(getLoginUrl(), "CancelCardRead", parameters, "",
                JavaScriptConvert.SerializeObject(result));

            return retString;
        }

        //设置卡开卡
        public static string OpenCardDevice(string SerialNumber,
            string AddressCode1, string AddressCode2, string AddressCode3, string YearExploitation,
            string AlertAvailableWater, string AlertAvailableElectric, string TypeCode, string MeterPulse, string AlertWaterLevel,
            string StationType, string StationCode, string Frequency)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("Method", "OpenCardDevice");
            parameters.Add("NetCardMAC", InfoSys.NetCardMAC);
            parameters.Add("opUserId", InfoSys.UserId.ToString());
            parameters.Add("opUserName", InfoSys.LoginUserName);
            parameters.Add("opPassword", InfoSys.LoginPassword);
            parameters.Add("SerialNumber", SerialNumber);
            parameters.Add("AddressCode1", AddressCode1);
            parameters.Add("AddressCode2", AddressCode2);
            parameters.Add("AddressCode3", AddressCode3);
            parameters.Add("YearExploitation", YearExploitation);
            parameters.Add("AlertAvailableWater", AlertAvailableWater);
            parameters.Add("AlertAvailableElectric", AlertAvailableElectric);
            parameters.Add("TypeCode", TypeCode);
            parameters.Add("MeterPulse", MeterPulse);
            parameters.Add("AlertWaterLevel", AlertWaterLevel);
            parameters.Add("StationType", StationType);
            parameters.Add("StationCode", StationCode);
            parameters.Add("Frequency", Frequency);

            JavaScriptObject result = new JavaScriptObject();
            result.Add("Result", false);
            result.Add("Message", "设置卡开卡保存出错！");

            string retString = HttpWebResponseUtility.getPostResponse(getLoginUrl(), "OpenCardDevice", parameters, "",
                JavaScriptConvert.SerializeObject(result));

            return retString;
        }

        //设置卡修改
        public static string ModifyCardDevice(string SerialNumber,
            string AddressCode1, string AddressCode2, string AddressCode3, string YearExploitation,
            string AlertAvailableWater, string AlertAvailableElectric, string TypeCode, string MeterPulse, string AlertWaterLevel,
            string StationType, string StationCode, string Frequency)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("Method", "ModifyCardDevice");
            parameters.Add("NetCardMAC", InfoSys.NetCardMAC);
            parameters.Add("opUserId", InfoSys.UserId.ToString());
            parameters.Add("opUserName", InfoSys.LoginUserName);
            parameters.Add("opPassword", InfoSys.LoginPassword);
            parameters.Add("SerialNumber", SerialNumber);
            parameters.Add("AddressCode1", AddressCode1);
            parameters.Add("AddressCode2", AddressCode2);
            parameters.Add("AddressCode3", AddressCode3);
            parameters.Add("YearExploitation", YearExploitation);
            parameters.Add("AlertAvailableWater", AlertAvailableWater);
            parameters.Add("AlertAvailableElectric", AlertAvailableElectric);
            parameters.Add("TypeCode", TypeCode);
            parameters.Add("MeterPulse", MeterPulse);
            parameters.Add("AlertWaterLevel", AlertWaterLevel);
            parameters.Add("StationType", StationType);
            parameters.Add("StationCode", StationCode);
            parameters.Add("Frequency", Frequency);

            JavaScriptObject result = new JavaScriptObject();
            result.Add("Result", false);
            result.Add("Message", "设置卡修改保存出错！");

            string retString = HttpWebResponseUtility.getPostResponse(getLoginUrl(), "ModifyCardDevice", parameters, "",
                JavaScriptConvert.SerializeObject(result));

            return retString;
        }

        //设置卡注销
        public static string CancelCardDevice(string SerialNumber)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("Method", "CancelCardDevice");
            parameters.Add("NetCardMAC", InfoSys.NetCardMAC);
            parameters.Add("opUserId", InfoSys.UserId.ToString());
            parameters.Add("opUserName", InfoSys.LoginUserName);
            parameters.Add("opPassword", InfoSys.LoginPassword);
            parameters.Add("SerialNumber", SerialNumber);

            JavaScriptObject result = new JavaScriptObject();
            result.Add("Result", false);
            result.Add("Message", "设置卡注销保存出错！");

            string retString = HttpWebResponseUtility.getPostResponse(getLoginUrl(), "CancelCardDevice", parameters, "",
                JavaScriptConvert.SerializeObject(result));

            return retString;
        }

        //用户卡开卡
        public static string OpenCardUser(string SerialNumber,
            string UserNo, string UserName, string IdentityNumber, string Telephone, string DeviceList)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("Method", "OpenCardUser");
            parameters.Add("NetCardMAC", InfoSys.NetCardMAC);
            parameters.Add("opUserId", InfoSys.UserId.ToString());
            parameters.Add("opUserName", InfoSys.LoginUserName);
            parameters.Add("opPassword", InfoSys.LoginPassword);
            parameters.Add("SerialNumber", SerialNumber);
            parameters.Add("UserNo", UserNo);
            parameters.Add("UserName", UserName);
            parameters.Add("IdentityNumber", IdentityNumber);
            parameters.Add("Telephone", Telephone);
            parameters.Add("DeviceList", DeviceList);

            myLogger.Info("用户卡开卡：" + SerialNumber + "|" + UserNo + "|" +
                UserName + "|" + IdentityNumber + "|" + Telephone + "|" + DeviceList);

            JavaScriptObject result = new JavaScriptObject();
            result.Add("Result", false);
            result.Add("Message", "用户卡开卡保存出错！");

            string retString = HttpWebResponseUtility.getPostResponse(getLoginUrl(), "OpenCardUser", parameters, "",
                JavaScriptConvert.SerializeObject(result));

            return retString;
        }

        //用户卡修改
        public static string ModifyCardUser(string SerialNumber,
            string UserNo, string UserName, string IdentityNumber, string Telephone, string DeviceList)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("Method", "ModifyCardUser");
            parameters.Add("NetCardMAC", InfoSys.NetCardMAC);
            parameters.Add("opUserId", InfoSys.UserId.ToString());
            parameters.Add("opUserName", InfoSys.LoginUserName);
            parameters.Add("opPassword", InfoSys.LoginPassword);
            parameters.Add("SerialNumber", SerialNumber);
            parameters.Add("UserNo", UserNo);
            parameters.Add("UserName", UserName);
            parameters.Add("IdentityNumber", IdentityNumber);
            parameters.Add("Telephone", Telephone);
            parameters.Add("DeviceList", DeviceList);

            myLogger.Info("用户卡修改：" + SerialNumber + "|" + UserNo + "|" +
                UserName + "|" + IdentityNumber + "|" + Telephone + "|" + DeviceList);

            JavaScriptObject result = new JavaScriptObject();
            result.Add("Result", false);
            result.Add("Message", "用户卡修改保存出错！");

            string retString = HttpWebResponseUtility.getPostResponse(getLoginUrl(), "ModifyCardUser", parameters, "",
                JavaScriptConvert.SerializeObject(result));

            return retString;
        }

        //用户卡注销
        public static string CancelCardUser(string SerialNumber)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("Method", "CancelCardUser");
            parameters.Add("NetCardMAC", InfoSys.NetCardMAC);
            parameters.Add("opUserId", InfoSys.UserId.ToString());
            parameters.Add("opUserName", InfoSys.LoginUserName);
            parameters.Add("opPassword", InfoSys.LoginPassword);
            parameters.Add("SerialNumber", SerialNumber);

            myLogger.Info("用户卡注销：" + SerialNumber);

            JavaScriptObject result = new JavaScriptObject();
            result.Add("Result", false);
            result.Add("Message", "用户卡注销保存出错！");

            string retString = HttpWebResponseUtility.getPostResponse(getLoginUrl(), "CancelCardUser", parameters, "",
                JavaScriptConvert.SerializeObject(result));

            return retString;
        }

        //用户卡充值
        public static string RechargeCardUser(string SerialNumber,
            string UserNo, string UserName, string IdentityNumber, string Telephone,
            string WaterPrice, string WaterNum, string ElectricPrice, string ElectricNum,
            string WaterUsed, string ElectricUsed, string Remark)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("Method", "RechargeCardUser");
            parameters.Add("NetCardMAC", InfoSys.NetCardMAC);
            parameters.Add("opUserId", InfoSys.UserId.ToString());
            parameters.Add("opUserName", InfoSys.LoginUserName);
            parameters.Add("opPassword", InfoSys.LoginPassword);
            parameters.Add("SerialNumber", SerialNumber);

            parameters.Add("UserNo", UserNo);
            parameters.Add("UserName", UserName);
            parameters.Add("IdentityNumber", IdentityNumber);
            parameters.Add("Telephone", Telephone);
            parameters.Add("WaterPrice", WaterPrice);
            parameters.Add("WaterNum", WaterNum);
            parameters.Add("ElectricPrice", ElectricPrice);
            parameters.Add("ElectricNum", ElectricNum);
            parameters.Add("WaterUsed", WaterUsed);
            parameters.Add("ElectricUsed", ElectricUsed);
            parameters.Add("Remark", Remark);

            myLogger.Info("用户卡充值：" + SerialNumber + "|" + UserNo + "|" +
                UserName + "|" + IdentityNumber + "|" + Telephone);

            JavaScriptObject result = new JavaScriptObject();
            result.Add("Result", false);
            result.Add("Message", "用户卡充值保存出错！");

            string retString = HttpWebResponseUtility.getPostResponse(getLoginUrl(), "RechargeCardUser", parameters, "",
                JavaScriptConvert.SerializeObject(result));

            return retString;
        }

        //用户卡挂失
        public static string CountermandCardUser(string SerialNumber)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("Method", "CountermandCardUser");
            parameters.Add("NetCardMAC", InfoSys.NetCardMAC);
            parameters.Add("opUserId", InfoSys.UserId.ToString());
            parameters.Add("opUserName", InfoSys.LoginUserName);
            parameters.Add("opPassword", InfoSys.LoginPassword);
            parameters.Add("SerialNumber", SerialNumber);

            JavaScriptObject result = new JavaScriptObject();
            result.Add("Result", false);
            result.Add("Message", "用户卡挂失保存出错！");

            string retString = HttpWebResponseUtility.getPostResponse(getLoginUrl(), "CountermandCardUser", parameters, "",
                JavaScriptConvert.SerializeObject(result));

            return retString;
        }

        //用户卡取消挂失
        public static string CountermandCancelCardUser(string SerialNumber)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("Method", "CountermandCancelCardUser");
            parameters.Add("NetCardMAC", InfoSys.NetCardMAC);
            parameters.Add("opUserId", InfoSys.UserId.ToString());
            parameters.Add("opUserName", InfoSys.LoginUserName);
            parameters.Add("opPassword", InfoSys.LoginPassword);
            parameters.Add("SerialNumber", SerialNumber);

            JavaScriptObject result = new JavaScriptObject();
            result.Add("Result", false);
            result.Add("Message", "用户卡取消挂失保存出错！");

            string retString = HttpWebResponseUtility.getPostResponse(getLoginUrl(), "CountermandCancelCardUser", parameters, "",
                JavaScriptConvert.SerializeObject(result));

            return retString;
        }

        //用户卡重新开卡
        public static string ReOpenCardUser(string SerialNumberOld, string SerialNumberNew)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("Method", "ReOpenCardUser");
            parameters.Add("NetCardMAC", InfoSys.NetCardMAC);
            parameters.Add("opUserId", InfoSys.UserId.ToString());
            parameters.Add("opUserName", InfoSys.LoginUserName);
            parameters.Add("opPassword", InfoSys.LoginPassword);
            parameters.Add("SerialNumberOld", SerialNumberOld);
            parameters.Add("SerialNumberNew", SerialNumberNew);

            JavaScriptObject result = new JavaScriptObject();
            result.Add("Result", false);
            result.Add("Message", "用户卡重新开卡保存出错！");

            string retString = HttpWebResponseUtility.getPostResponse(getLoginUrl(), "ReOpenCardUser", parameters, "",
                JavaScriptConvert.SerializeObject(result));

            return retString;
        }

        //查询用户卡列表
        public static string GetCardUserList(string UserNo, string UserName, string IdentityNumber, string Telephone)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("Method", "GetCardUserList");
            parameters.Add("NetCardMAC", InfoSys.NetCardMAC);
            parameters.Add("opUserId", InfoSys.UserId.ToString());
            parameters.Add("opUserName", InfoSys.LoginUserName);
            parameters.Add("opPassword", InfoSys.LoginPassword);

            parameters.Add("UserNo", UserNo);
            parameters.Add("UserName", UserName);
            parameters.Add("IdentityNumber", IdentityNumber);
            parameters.Add("Telephone", Telephone);

            JavaScriptObject result = new JavaScriptObject();
            result.Add("Result", false);
            result.Add("Message", "查询用户卡列表出错！");

            string retString = HttpWebResponseUtility.getPostResponse(getLoginUrl(), "GetCardUserList", parameters, "",
                JavaScriptConvert.SerializeObject(result));

            return retString;
        }

        //根据用户名获取用户信息
        public static string GetWaterUserInfo(string UserName, string IdentityNumber, string Telephone)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("Method", "GetWaterUserInfo");
            parameters.Add("NetCardMAC", InfoSys.NetCardMAC);
            parameters.Add("opUserId", InfoSys.UserId.ToString());
            parameters.Add("opUserName", InfoSys.LoginUserName);
            parameters.Add("opPassword", InfoSys.LoginPassword);

            parameters.Add("UserName", UserName);
            parameters.Add("IdentityNumber", IdentityNumber);
            parameters.Add("Telephone", Telephone);

            JavaScriptObject result = new JavaScriptObject();
            result.Add("Result", false);
            result.Add("Message", "查询用户信息出错！");

            string retString = HttpWebResponseUtility.getPostResponse(getLoginUrl(), "GetWaterUserInfo", parameters, "",
                JavaScriptConvert.SerializeObject(result));

            return retString;
        }

        //根据用户Id获取用户信息
        public static string GetCardUserRechargeInfo(long WaterUserId)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("Method", "GetCardUserRechargeInfo");
            parameters.Add("NetCardMAC", InfoSys.NetCardMAC);
            parameters.Add("opUserId", InfoSys.UserId.ToString());
            parameters.Add("opUserName", InfoSys.LoginUserName);
            parameters.Add("opPassword", InfoSys.LoginPassword);

            parameters.Add("WaterUserId", WaterUserId.ToString());

            JavaScriptObject result = new JavaScriptObject();
            result.Add("Result", false);
            result.Add("Message", "查询用户充值信息出错！");

            string retString = HttpWebResponseUtility.getPostResponse(getLoginUrl(), "GetCardUserRechargeInfo", parameters, "",
                JavaScriptConvert.SerializeObject(result));

            return retString;
        }

        //查询用户充值记录
        public static string GetCardUserRechargeLogList(string UserNo, string UserName, string IdentityNumber, string Telephone)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("Method", "GetCardUserRechargeLogList");
            parameters.Add("NetCardMAC", InfoSys.NetCardMAC);
            parameters.Add("opUserId", InfoSys.UserId.ToString());
            parameters.Add("opUserName", InfoSys.LoginUserName);
            parameters.Add("opPassword", InfoSys.LoginPassword);

            parameters.Add("UserNo", UserNo);
            parameters.Add("UserName", UserName);
            parameters.Add("IdentityNumber", IdentityNumber);
            parameters.Add("Telephone", Telephone);

            JavaScriptObject result = new JavaScriptObject();
            result.Add("Result", false);
            result.Add("Message", "查询用户充值记录列表出错！");

            string retString = HttpWebResponseUtility.getPostResponse(getLoginUrl(), "GetCardUserRechargeLogList", parameters, "",
                JavaScriptConvert.SerializeObject(result));

            return retString;
        }

        //查询用户用水记录
        public static string GetCardUserWaterLogList(string UserNo, string UserName, string IdentityNumber, string Telephone)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("Method", "GetCardUserWaterLogList");
            parameters.Add("NetCardMAC", InfoSys.NetCardMAC);
            parameters.Add("opUserId", InfoSys.UserId.ToString());
            parameters.Add("opUserName", InfoSys.LoginUserName);
            parameters.Add("opPassword", InfoSys.LoginPassword);

            parameters.Add("UserNo", UserNo);
            parameters.Add("UserName", UserName);
            parameters.Add("IdentityNumber", IdentityNumber);
            parameters.Add("Telephone", Telephone);

            JavaScriptObject result = new JavaScriptObject();
            result.Add("Result", false);
            result.Add("Message", "查询用户用水记录列表出错！");

            string retString = HttpWebResponseUtility.getPostResponse(getLoginUrl(), "GetCardUserWaterLogList", parameters, "",
                JavaScriptConvert.SerializeObject(result));

            return retString;
        }
    }
}
