using Common;
using DBUtility;
using DTU.GateWay.Protocol;
using Maticsoft.Model;
using Module;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web;
using Utils;
using WaterMonitorSystem.Src;

namespace WaterMonitorSystem
{
    /// <summary>
    /// Ajaxjson 的摘要说明
    /// </summary>
    public class Ajaxjson : IHttpHandler
    {
        static log4net.ILog myLogger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            //context.Response.Write("Hello World");
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;

            JavaScriptObject result = new JavaScriptObject();
            result.Add("Result", false);
            result.Add("Message", "");

            try
            {
                string method = request["Method"] ?? "";
                switch (method)
                {
                    case "HelloWorld": result = HelloWorld(request); break;
                    case "UserLogin": result = UserLogin(request); break;
                    case "GetDistrictList": result = GetDistrictList(request); break;
                    case "GetDeviceList": result = GetDeviceList(request); break;
                    case "GetTypeCodeList": result = GetTypeCodeList(request); break;
                    case "OpenCardClear": result = OpenCardClear(request); break;
                    case "CancelCardClear": result = CancelCardClear(request); break;
                    case "OpenCardNetSet": result = OpenCardNetSet(request); break;
                    case "ModifyCardNetSet": result = ModifyCardNetSet(request); break;
                    case "CancelCardNetSet": result = CancelCardNetSet(request); break;
                    case "OpenCardRead": result = OpenCardRead(request); break;
                    case "ModifyCardRead": result = ModifyCardRead(request); break;
                    case "CancelCardRead": result = CancelCardRead(request); break;
                    case "OpenCardDevice": result = OpenCardDevice(request); break;
                    case "ModifyCardDevice": result = ModifyCardDevice(request); break;
                    case "CancelCardDevice": result = CancelCardDevice(request); break;
                    case "OpenCardUser": result = OpenCardUser(request); break;
                    case "ModifyCardUser": result = ModifyCardUser(request); break;
                    case "CancelCardUser": result = CancelCardUser(request); break;
                    case "RechargeCardUser": result = RechargeCardUser(request); break;
                    case "CountermandCardUser": result = CountermandCardUser(request); break;
                    case "CountermandCancelCardUser": result = CountermandCancelCardUser(request); break;
                    case "ReOpenCardUser": result = ReOpenCardUser(request); break;
                    case "GetCardUserList": result = GetCardUserList(request); break;
                    case "GetWaterUserInfo": result = GetWaterUserInfo(request); break;
                    case "GetCardUserRechargeInfo": result = GetCardUserRechargeInfo(request); break;
                    case "GetCardUserRechargeLogList": result = GetCardUserRechargeLogList(request); break;
                    case "GetCardUserWaterLogList": result = GetCardUserWaterLogList(request); break;
                    default:
                        result["Message"] = "缺少参数！";
                        break;
                }
            }
            catch (Exception ex)
            {
                result["Message"] = "错误！" + ex.Message;
            }

            string str = JavaScriptConvert.SerializeObject(result);
            response.Write(str);
            response.End();
        }

        private JavaScriptObject HelloWorld(HttpRequest request)
        {
            string NetCardMAC = request["NetCardMAC"] ?? "";

            JavaScriptObject result = new JavaScriptObject();
            result.Add("Result", true);
            result.Add("Message", "HelloWorld");

            return result;
        }

        #region 方法
        //用户登录
        private JavaScriptObject UserLogin(HttpRequest request)
        {
            string UserName = request["UserName"] ?? "";
            string Password = request["Password"] ?? "";
            string NetCardMAC = request["NetCardMAC"] ?? "";

            JavaScriptObject result = new JavaScriptObject();
            result.Add("Result", false);
            result.Add("Message", "");

            if (SysInfo.IsReg)
            {
                string path = request.MapPath("~/");

                SysInfo.SetFilePath(path);

                SysInfo.IsRegSuccess = false;
                string regStr = "";
                if (FileHelper.IsExists(SysInfo.fileName))
                {
                    regStr = FileHelper.ReadFile(SysInfo.fileName);
                }
                else
                {
                    regStr = "00000000000000000000000000000000";
                    FileHelper.writeFile(SysInfo.fileName, regStr);
                }

                if (regStr != SysInfo.GetRegStr2())
                {
                    myLogger.Info("注册码不对！序列号为：" + SysInfo.GetRegStr1());
                    result["Message"] = "系统未注册！";
                    return result;
                }
                else
                {
                    SysInfo.IsRegSuccess = true;
                }
            }

            try
            {
                long userID = 0;
                string str = SysUserModule.Login(UserName, Password, ToolsWeb.GetIP(request), NetCardMAC, ref userID);

                if (str == "登陆成功")
                {
                    SysUserModule.OperatorLogin(userID, ToolsWeb.GetIP(request), NetCardMAC);

                    JsonSysUser user = new JsonSysUser();
                    SysUser model = SysUserModule.GetUser(userID);
                    if (model.IsAllow == 0)
                    {
                        result["Message"] = "用户已被禁止登陆";
                    }
                    else
                    {
                        user.UserId = model.ID;
                        user.TrueName = model.TrueName;
                        user.GroupId = model.RoleId;
                        Role role = RoleModule.GetRole(model.RoleId);
                        if (role != null)
                        {
                            user.GroupName = role.RoleName;
                        }
                        else
                        {
                            user.GroupName = "未知";
                        }
                        user.DistrictId = model.DistrictId;
                        District district = DistrictModule.ReturnDistrictInfo(model.DistrictId);
                        if (district != null)
                        {
                            user.DistrictName = district.DistrictName;
                        }
                        else
                        {
                            user.DistrictName = "未知";
                        }
                        result["Result"] = true;
                        result["Message"] = JavaScriptConvert.SerializeObject(user);
                    }
                }
                else
                {
                    result["Message"] = str;
                }
            }
            catch (Exception exception)
            {
                result["Message"] = exception.Message;
                myLogger.Error(exception.Message);
            }

            return result;
        }

        //获取地区列表
        private JavaScriptObject GetDistrictList(HttpRequest request)
        {
            string UserName = request["opUserName"] ?? "";
            string Password = request["opPassword"] ?? "";
            string NetCardMAC = request["NetCardMAC"] ?? "";

            JavaScriptObject result = new JavaScriptObject();
            result.Add("Result", false);
            result.Add("Message", "");

            try
            {
                long userID = 0;
                string str = SysUserModule.Login(UserName, Password, ToolsWeb.GetIP(request), NetCardMAC, ref userID);

                if (str == "登陆成功")
                {
                    JsonSysUser user = new JsonSysUser();
                    SysUser model = SysUserModule.GetUser(userID);
                    if (model.IsAllow == 0)
                    {
                        result["Message"] = "用户已被禁止登陆";
                    }
                    else
                    {
                        //List<District> list = DistrictModule.GetAllDistrict();
                        List<District> list = DistrictModule.GetAllDistrict(model.DistrictId);
                        result["Result"] = true;
                        result["Message"] = JavaScriptConvert.SerializeObject(list);
                    }
                }
            }
            catch (Exception exception)
            {
                result["Message"] = exception.Message;
                myLogger.Error(exception.Message);
            }

            return result;
        }

        //获取终端列表
        private JavaScriptObject GetDeviceList(HttpRequest request)
        {
            string UserName = request["opUserName"] ?? "";
            string Password = request["opPassword"] ?? "";
            string NetCardMAC = request["NetCardMAC"] ?? "";

            JavaScriptObject result = new JavaScriptObject();
            result.Add("Result", false);
            result.Add("Message", "");

            try
            {
                long userID = 0;
                string str = SysUserModule.Login(UserName, Password, ToolsWeb.GetIP(request), NetCardMAC, ref userID);

                JsonSysUser user = new JsonSysUser();
                SysUser model = SysUserModule.GetUser(userID);
                if (model.IsAllow == 0)
                {
                    result["Message"] = "用户已被禁止登陆";
                }
                else
                {
                    List<long> list_DistrictID = DistrictModule.GetAllDistrictID(model.DistrictId);
                    List<Device> list_Device = DeviceModule.GetAllDevice();
                    List<Device> list = new List<Device>();
                    foreach (Device device in list_Device)
                    {
                        if (list_DistrictID.Contains(device.DistrictId))
                        {
                            list.Add(device);
                        }
                    }
                    result["Result"] = true;
                    result["Message"] = JavaScriptConvert.SerializeObject(list);
                }
            }
            catch (Exception exception)
            {
                result["Message"] = exception.Message;
                myLogger.Error(exception.Message);
            }

            return result;
        }

        //获取流量计类型
        private JavaScriptObject GetTypeCodeList(HttpRequest request)
        {
            string UserName = request["opUserName"] ?? "";
            string Password = request["opPassword"] ?? "";
            string NetCardMAC = request["NetCardMAC"] ?? "";

            JavaScriptObject result = new JavaScriptObject();
            result.Add("Result", false);
            result.Add("Message", "");

            try
            {
                long userID = 0;
                string str = SysUserModule.Login(UserName, Password, ToolsWeb.GetIP(request), NetCardMAC, ref userID);

                JsonSysUser user = new JsonSysUser();
                SysUser model = SysUserModule.GetUser(userID);
                if (model.IsAllow == 0)
                {
                    result["Message"] = "用户已被禁止登陆";
                }
                else
                {
                    List<DeviceTypeCode> list = DeviceTypeCodeModule.GetAll();
                    result["Result"] = true;
                    result["Message"] = JavaScriptConvert.SerializeObject(list);
                }
            }
            catch (Exception exception)
            {
                result["Message"] = exception.Message;
                myLogger.Error(exception.Message);
            }

            return result;
        }
        #endregion

        #region 清零卡
        //开卡
        private JavaScriptObject OpenCardClear(HttpRequest request)
        {
            string opUserId = request["opUserId"] ?? "0";
            string opUserName = request["opUserName"] ?? "";
            string opPassword = request["opPassword"] ?? "";
            string NetCardMAC = request["NetCardMAC"] ?? "";
            string SerialNumber = request["SerialNumber"] ?? "";

            JavaScriptObject result = new JavaScriptObject();
            result.Add("Result", false);
            result.Add("Message", "");
            DateTime dateNow = DateTime.Now;
            CardClear obj = null;
            try
            {
                if (CardClearModule.ExistsSerialNumber(SerialNumber))
                {
                    result["Message"] = "已存在相同序列号卡，无法开卡！";
                }
                else
                {
                    obj = new CardClear();
                    obj.SerialNumber = SerialNumber;
                    obj.OpenUserId = long.Parse(opUserId);
                    obj.OpenUserName = opUserName;
                    obj.OpenAddress = NetCardMAC;
                    obj.OpenTime = dateNow;

                    long id = CardClearModule.AddCardClear(obj);

                    if (id > 0)
                    {
                        obj.Id = id;
                        result["Result"] = true;
                        result["Message"] = id;
                    }
                    else
                    {
                        result["Message"] = "开卡保存出错！";
                    }
                }
            }
            catch (Exception ex)
            {
                result["Message"] = "开卡出错！";
                myLogger.Error(ex.Message);
            }

            try
            {
                CardClearLog objLog = new CardClearLog();
                objLog.SerialNumber = SerialNumber;
                objLog.LogUserId = long.Parse(opUserId);
                objLog.LogUserName = opUserName;
                objLog.LogAddress = NetCardMAC;
                objLog.LogTime = dateNow;
                objLog.LogType = "开卡";
                objLog.LogContent = result["Result"] + "|" + result["Message"];

                CardClearLogModule.AddCardClearLog(objLog);
            }
            catch (Exception ex)
            {
                myLogger.Error(ex.Message);
            }

            return result;
        }

        //注销卡
        private JavaScriptObject CancelCardClear(HttpRequest request)
        {
            string opUserId = request["opUserId"] ?? "0";
            string opUserName = request["opUserName"] ?? "";
            string opPassword = request["opPassword"] ?? "";
            string NetCardMAC = request["NetCardMAC"] ?? "";
            string SerialNumber = request["SerialNumber"] ?? "";

            JavaScriptObject result = new JavaScriptObject();
            result.Add("Result", false);
            result.Add("Message", "");
            DateTime dateNow = DateTime.Now;
            CardClear obj = null;
            try
            {
                obj = CardClearModule.GetCardClearBySerialNumber(SerialNumber);
                if (obj != null)
                {
                    if (CardClearModule.DeleteCardClear(obj.Id) == "删除成功")
                    {
                        result["Result"] = true;
                        result["Message"] = "注销成功";
                    }
                    else
                    {
                        result["Message"] = "注销保存失败";
                    }
                }
                else
                {
                    result["Message"] = "卡不存在，无法注销！";
                }
            }
            catch (Exception ex)
            {
                result["Message"] = "注销出错！";
                myLogger.Error(ex.Message);
            }

            try
            {
                CardClearLog objLog = new CardClearLog();
                objLog.SerialNumber = SerialNumber;
                objLog.LogUserId = long.Parse(opUserId);
                objLog.LogUserName = opUserName;
                objLog.LogAddress = NetCardMAC;
                objLog.LogTime = dateNow;
                objLog.LogType = "注销";
                objLog.LogContent = result["Result"] + "|" + result["Message"];

                CardClearLogModule.AddCardClearLog(objLog);
            }
            catch (Exception ex)
            {
                myLogger.Error(ex.Message);
            }

            return result;
        }
        #endregion

        #region 网络设置卡
        //开卡
        private JavaScriptObject OpenCardNetSet(HttpRequest request)
        {
            string opUserId = request["opUserId"] ?? "0";
            string opUserName = request["opUserName"] ?? "";
            string opPassword = request["opPassword"] ?? "";
            string NetCardMAC = request["NetCardMAC"] ?? "";
            string SerialNumber = request["SerialNumber"] ?? "";

            string IP = request["IP"] ?? "";
            string Port = request["Port"] ?? "0";
            string IsDomain = request["IsDomain"] ?? "";
            string APNName = request["APNName"] ?? "";
            string APNUserName = request["APNUserName"] ?? "";
            string APNPassword = request["APNPassword"] ?? "";

            JavaScriptObject result = new JavaScriptObject();
            result.Add("Result", false);
            result.Add("Message", "");
            DateTime dateNow = DateTime.Now;
            CardNetSet obj = null;
            try
            {
                if (CardNetSetModule.ExistsSerialNumber(SerialNumber))
                {
                    result["Message"] = "已存在相同序列号卡，无法开卡！";
                }
                else
                {
                    obj = new CardNetSet();
                    obj.SerialNumber = SerialNumber;
                    obj.OpenUserId = long.Parse(opUserId);
                    obj.OpenUserName = opUserName;
                    obj.OpenAddress = NetCardMAC;
                    obj.OpenTime = dateNow;
                    obj.LastUpdateUserId = 0;
                    obj.LastUpdateUserName = "";
                    obj.LastUpdateAddress = "";
                    obj.LastUpdateTime = DateTime.Parse("1900-1-1");

                    obj.IP = IP;
                    obj.Port = int.Parse(Port);
                    obj.IsDomain = IsDomain;
                    obj.APNName = APNName;
                    obj.APNUserName = APNUserName;
                    obj.APNPassword = APNPassword;

                    long id = CardNetSetModule.AddCardNetSet(obj);
                    if (id > 0)
                    {
                        obj.Id = id;
                        result["Result"] = true;
                        result["Message"] = id;
                    }
                    else
                    {
                        result["Message"] = "开卡保存出错！";
                    }
                }
            }
            catch (Exception ex)
            {
                result["Message"] = "开卡出错！";
                myLogger.Error(ex.Message);
            }

            try
            {
                CardNetSetLog objLog = new CardNetSetLog();
                objLog.SerialNumber = SerialNumber;
                objLog.LogUserId = long.Parse(opUserId);
                objLog.LogUserName = opUserName;
                objLog.LogAddress = NetCardMAC;
                objLog.LogTime = dateNow;
                objLog.LogType = "开卡";
                objLog.LogContent = result["Result"] + "|" + result["Message"];
                objLog.IP = IP;
                objLog.Port = int.Parse(Port);
                objLog.IsDomain = IsDomain;
                objLog.APNName = APNName;
                objLog.APNUserName = APNUserName;
                objLog.APNPassword = APNPassword;
                CardNetSetLogModule.AddCardNetSetLog(objLog);
            }
            catch (Exception ex)
            {
                myLogger.Error(ex.Message);
            }

            return result;
        }

        //修改卡
        private JavaScriptObject ModifyCardNetSet(HttpRequest request)
        {
            string opUserId = request["opUserId"] ?? "0";
            string opUserName = request["opUserName"] ?? "";
            string opPassword = request["opPassword"] ?? "";
            string NetCardMAC = request["NetCardMAC"] ?? "";
            string SerialNumber = request["SerialNumber"] ?? "";

            string IP = request["IP"] ?? "";
            string Port = request["Port"] ?? "0";
            string IsDomain = request["IsDomain"] ?? "";
            string APNName = request["APNName"] ?? "";
            string APNUserName = request["APNUserName"] ?? "";
            string APNPassword = request["APNPassword"] ?? "";

            JavaScriptObject result = new JavaScriptObject();
            result.Add("Result", false);
            result.Add("Message", "");
            DateTime dateNow = DateTime.Now;
            CardNetSet obj = null;
            try
            {
                obj = CardNetSetModule.GetCardNetSetBySerialNumber(SerialNumber);
                if (obj != null)
                {
                    obj.SerialNumber = SerialNumber;
                    obj.LastUpdateUserId = long.Parse(opUserId);
                    obj.LastUpdateUserName = opUserName;
                    obj.LastUpdateAddress = NetCardMAC;
                    obj.LastUpdateTime = dateNow;

                    obj.IP = IP;
                    obj.Port = int.Parse(Port);
                    obj.IsDomain = IsDomain;
                    obj.APNName = APNName;
                    obj.APNUserName = APNUserName;
                    obj.APNPassword = APNPassword;

                    if (CardNetSetModule.ModifyCardNetSet(obj) == "修改成功")
                    {
                        result["Result"] = true;
                        result["Message"] = "修改成功";
                    }
                    else
                    {
                        result["Message"] = "修改保存出错！";
                    }
                }
                else
                {
                    result["Message"] = "卡不存在，无法修改！";
                }
            }
            catch (Exception ex)
            {
                result["Message"] = "修改出错！";
                myLogger.Error(ex.Message);
            }

            try
            {
                CardNetSetLog objLog = new CardNetSetLog();
                objLog.SerialNumber = SerialNumber;
                objLog.LogUserId = long.Parse(opUserId);
                objLog.LogUserName = opUserName;
                objLog.LogAddress = NetCardMAC;
                objLog.LogTime = dateNow;
                objLog.LogType = "修改";
                objLog.LogContent = result["Result"] + "|" + result["Message"];
                objLog.IP = IP;
                objLog.Port = int.Parse(Port);
                objLog.IsDomain = IsDomain;
                objLog.APNName = APNName;
                objLog.APNUserName = APNUserName;
                objLog.APNPassword = APNPassword;
                CardNetSetLogModule.AddCardNetSetLog(objLog);
            }
            catch (Exception ex)
            {
                myLogger.Error(ex.Message);
            }

            return result;
        }

        //注销卡
        private JavaScriptObject CancelCardNetSet(HttpRequest request)
        {
            string opUserId = request["opUserId"] ?? "0";
            string opUserName = request["opUserName"] ?? "";
            string opPassword = request["opPassword"] ?? "";
            string NetCardMAC = request["NetCardMAC"] ?? "";
            string SerialNumber = request["SerialNumber"] ?? "";

            JavaScriptObject result = new JavaScriptObject();
            result.Add("Result", false);
            result.Add("Message", "");
            DateTime dateNow = DateTime.Now;
            CardNetSet obj = null;
            try
            {
                obj = CardNetSetModule.GetCardNetSetBySerialNumber(SerialNumber);
                if (obj != null)
                {
                    if (CardNetSetModule.DeleteCardNetSet(obj.Id) == "删除成功")
                    {
                        result["Result"] = true;
                        result["Message"] = "注销成功";
                    }
                    else
                    {
                        result["Message"] = "注销保存失败";
                    }
                }
                else
                {
                    result["Message"] = "卡不存在，无法注销！";
                }
            }
            catch (Exception ex)
            {
                result["Message"] = "注销出错！";
                myLogger.Error(ex.Message);
            }

            try
            {
                CardNetSetLog objLog = new CardNetSetLog();
                objLog.SerialNumber = SerialNumber;
                objLog.LogUserId = long.Parse(opUserId);
                objLog.LogUserName = opUserName;
                objLog.LogAddress = NetCardMAC;
                objLog.LogTime = dateNow;
                objLog.LogType = "注销";
                objLog.LogContent = result["Result"] + "|" + result["Message"];
                if (obj != null)
                {
                    objLog.IP = obj.IP;
                    objLog.Port = obj.Port;
                    objLog.IsDomain = obj.IsDomain;
                    objLog.APNName = obj.APNName;
                    objLog.APNUserName = obj.APNUserName;
                    objLog.APNPassword = obj.APNPassword;
                }
                CardNetSetLogModule.AddCardNetSetLog(objLog);
            }
            catch (Exception ex)
            {
                myLogger.Error(ex.Message);
            }

            return result;
        }
        #endregion

        #region 读取卡
        //开卡
        private JavaScriptObject OpenCardRead(HttpRequest request)
        {
            string opUserId = request["opUserId"] ?? "0";
            string opUserName = request["opUserName"] ?? "";
            string opPassword = request["opPassword"] ?? "";
            string NetCardMAC = request["NetCardMAC"] ?? "";
            string SerialNumber = request["SerialNumber"] ?? "";

            string AddressCode1 = request["AddressCode1"] ?? "";
            string AddressCode2 = request["AddressCode2"] ?? "";
            string AddressCode3 = request["AddressCode3"] ?? "0";

            JavaScriptObject result = new JavaScriptObject();
            result.Add("Result", false);
            result.Add("Message", "");
            DateTime dateNow = DateTime.Now;
            CardRead obj = null;
            try
            {
                if (CardReadModule.ExistsSerialNumber(SerialNumber))
                {
                    result["Message"] = "已存在相同序列号卡，无法开卡！";
                }
                else
                {
                    obj = new CardRead();
                    obj.SerialNumber = SerialNumber;
                    obj.OpenUserId = long.Parse(opUserId);
                    obj.OpenUserName = opUserName;
                    obj.OpenAddress = NetCardMAC;
                    obj.OpenTime = dateNow;
                    obj.LastUpdateUserId = 0;
                    obj.LastUpdateUserName = "";
                    obj.LastUpdateAddress = "";
                    obj.LastUpdateTime = DateTime.Parse("1900-1-1");

                    obj.AddressCode1 = AddressCode1;
                    obj.AddressCode2 = AddressCode2;
                    obj.AddressCode3 = int.Parse(AddressCode3);

                    long id = CardReadModule.AddCardRead(obj);
                    if (id > 0)
                    {
                        obj.Id = id;
                        result["Result"] = true;
                        result["Message"] = id;
                    }
                    else
                    {
                        result["Message"] = "开卡保存出错！";
                    }
                }
            }
            catch (Exception ex)
            {
                result["Message"] = "开卡出错！";
                myLogger.Error(ex.Message);
            }

            try
            {
                CardReadLog objLog = new CardReadLog();
                objLog.SerialNumber = SerialNumber;
                objLog.LogUserId = long.Parse(opUserId);
                objLog.LogUserName = opUserName;
                objLog.LogAddress = NetCardMAC;
                objLog.LogTime = dateNow;
                objLog.LogType = "开卡";
                objLog.LogContent = result["Result"] + "|" + result["Message"];
                objLog.AddressCode1 = AddressCode1;
                objLog.AddressCode2 = AddressCode2;
                objLog.AddressCode3 = int.Parse(AddressCode3);
                CardReadLogModule.AddCardReadLog(objLog);
            }
            catch (Exception ex)
            {
                myLogger.Error(ex.Message);
            }

            return result;
        }

        //修改卡
        private JavaScriptObject ModifyCardRead(HttpRequest request)
        {
            string opUserId = request["opUserId"] ?? "0";
            string opUserName = request["opUserName"] ?? "";
            string opPassword = request["opPassword"] ?? "";
            string NetCardMAC = request["NetCardMAC"] ?? "";
            string SerialNumber = request["SerialNumber"] ?? "";

            string AddressCode1 = request["AddressCode1"] ?? "";
            string AddressCode2 = request["AddressCode2"] ?? "";
            string AddressCode3 = request["AddressCode3"] ?? "0";

            JavaScriptObject result = new JavaScriptObject();
            result.Add("Result", false);
            result.Add("Message", "");
            DateTime dateNow = DateTime.Now;
            CardRead obj = null;
            try
            {
                obj = CardReadModule.GetCardReadBySerialNumber(SerialNumber);
                if (obj != null)
                {
                    obj.SerialNumber = SerialNumber;
                    obj.LastUpdateUserId = long.Parse(opUserId);
                    obj.LastUpdateUserName = opUserName;
                    obj.LastUpdateAddress = NetCardMAC;
                    obj.LastUpdateTime = dateNow;

                    obj.AddressCode1 = AddressCode1;
                    obj.AddressCode2 = AddressCode2;
                    obj.AddressCode3 = int.Parse(AddressCode3);

                    if (CardReadModule.ModifyCardRead(obj) == "修改成功")
                    {
                        result["Result"] = true;
                        result["Message"] = "修改成功";
                    }
                    else
                    {
                        result["Message"] = "修改保存出错！";
                    }
                }
                else
                {
                    result["Message"] = "卡不存在，无法修改！";
                }
            }
            catch (Exception ex)
            {
                result["Message"] = "修改出错！";
                myLogger.Error(ex.Message);
            }

            try
            {
                CardReadLog objLog = new CardReadLog();
                objLog.SerialNumber = SerialNumber;
                objLog.LogUserId = long.Parse(opUserId);
                objLog.LogUserName = opUserName;
                objLog.LogAddress = NetCardMAC;
                objLog.LogTime = dateNow;
                objLog.LogType = "修改";
                objLog.LogContent = result["Result"] + "|" + result["Message"];
                objLog.AddressCode1 = AddressCode1;
                objLog.AddressCode2 = AddressCode2;
                objLog.AddressCode3 = int.Parse(AddressCode3);
                CardReadLogModule.AddCardReadLog(objLog);
            }
            catch (Exception ex)
            {
                myLogger.Error(ex.Message);
            }

            return result;
        }

        //注销卡
        private JavaScriptObject CancelCardRead(HttpRequest request)
        {
            string opUserId = request["opUserId"] ?? "0";
            string opUserName = request["opUserName"] ?? "";
            string opPassword = request["opPassword"] ?? "";
            string NetCardMAC = request["NetCardMAC"] ?? "";
            string SerialNumber = request["SerialNumber"] ?? "";

            JavaScriptObject result = new JavaScriptObject();
            result.Add("Result", false);
            result.Add("Message", "");
            DateTime dateNow = DateTime.Now;
            CardRead obj = null;
            try
            {
                obj = CardReadModule.GetCardReadBySerialNumber(SerialNumber);
                if (obj != null)
                {
                    if (CardReadModule.DeleteCardRead(obj.Id) == "删除成功")
                    {
                        result["Result"] = true;
                        result["Message"] = "注销成功";
                    }
                    else
                    {
                        result["Message"] = "注销保存失败";
                    }
                }
                else
                {
                    result["Message"] = "卡不存在，无法注销！";
                }
            }
            catch (Exception ex)
            {
                result["Message"] = "注销出错！";
                myLogger.Error(ex.Message);
            }

            try
            {
                CardReadLog objLog = new CardReadLog();
                objLog.SerialNumber = SerialNumber;
                objLog.LogUserId = long.Parse(opUserId);
                objLog.LogUserName = opUserName;
                objLog.LogAddress = NetCardMAC;
                objLog.LogTime = dateNow;
                objLog.LogType = "注销";
                objLog.LogContent = result["Result"] + "|" + result["Message"];
                if (obj != null)
                {
                    objLog.AddressCode1 = obj.AddressCode1;
                    objLog.AddressCode2 = obj.AddressCode2;
                    objLog.AddressCode3 = obj.AddressCode3;
                }
                CardReadLogModule.AddCardReadLog(objLog);
            }
            catch (Exception ex)
            {
                myLogger.Error(ex.Message);
            }

            return result;
        }
        #endregion

        #region 设置卡
        //开卡
        private JavaScriptObject OpenCardDevice(HttpRequest request)
        {
            string opUserId = request["opUserId"] ?? "0";
            string opUserName = request["opUserName"] ?? "";
            string opPassword = request["opPassword"] ?? "";
            string NetCardMAC = request["NetCardMAC"] ?? "";
            string SerialNumber = request["SerialNumber"] ?? "";

            string AddressCode1 = request["AddressCode1"] ?? "";
            string AddressCode2 = request["AddressCode2"] ?? "";
            string AddressCode3 = request["AddressCode3"] ?? "0";
            string YearExploitation = request["YearExploitation"] ?? "0";
            string AlertAvailableWater = request["AlertAvailableWater"] ?? "0";
            string AlertAvailableElectric = request["AlertAvailableElectric"] ?? "0";
            string TypeCode = request["TypeCode"] ?? "0";
            string MeterPulse = request["MeterPulse"] ?? "0";
            string AlertWaterLevel = request["AlertWaterLevel"] ?? "0";
            string StationType = request["StationType"] ?? "0";
            string StationCode = request["StationCode"] ?? "0";
            string Frequency = request["Frequency"] ?? "0";

            JavaScriptObject result = new JavaScriptObject();
            result.Add("Result", false);
            result.Add("Message", "");
            DateTime dateNow = DateTime.Now;
            CardDevice obj = null;
            try
            {
                if (CardDeviceModule.ExistsSerialNumber(SerialNumber))
                {
                    result["Message"] = "已存在相同序列号卡，无法开卡！";
                }
                else
                {
                    obj = new CardDevice();
                    obj.SerialNumber = SerialNumber;
                    obj.OpenUserId = long.Parse(opUserId);
                    obj.OpenUserName = opUserName;
                    obj.OpenAddress = NetCardMAC;
                    obj.OpenTime = dateNow;
                    obj.LastUpdateUserId = 0;
                    obj.LastUpdateUserName = "";
                    obj.LastUpdateAddress = "";
                    obj.LastUpdateTime = DateTime.Parse("1900-1-1");

                    obj.AddressCode1 = AddressCode1;
                    obj.AddressCode2 = AddressCode2;
                    obj.AddressCode3 = int.Parse(AddressCode3);
                    obj.YearExploitation = decimal.Parse(YearExploitation);
                    obj.AlertAvailableWater = int.Parse(AlertAvailableWater);
                    obj.AlertAvailableElectric = int.Parse(AlertAvailableElectric);
                    obj.TypeCode = int.Parse(TypeCode);
                    obj.MeterPulse = int.Parse(MeterPulse);
                    obj.AlertWaterLevel = decimal.Parse(AlertWaterLevel);
                    obj.StationType = int.Parse(StationType);
                    obj.StationCode = int.Parse(StationCode);
                    obj.Frequency = int.Parse(Frequency);

                    long id = CardDeviceModule.AddCardDevice(obj);
                    if (id > 0)
                    {
                        obj.Id = id;
                        result["Result"] = true;
                        result["Message"] = id;
                    }
                    else
                    {
                        result["Message"] = "开卡保存出错！";
                    }
                }
            }
            catch (Exception ex)
            {
                result["Message"] = "开卡出错！";
                myLogger.Error(ex.Message);
            }

            try
            {
                CardDeviceLog objLog = new CardDeviceLog();
                objLog.SerialNumber = SerialNumber;
                objLog.LogUserId = long.Parse(opUserId);
                objLog.LogUserName = opUserName;
                objLog.LogAddress = NetCardMAC;
                objLog.LogTime = dateNow;
                objLog.LogType = "开卡";
                objLog.LogContent = result["Result"] + "|" + result["Message"];
                objLog.AddressCode1 = AddressCode1;
                objLog.AddressCode2 = AddressCode2;
                objLog.AddressCode3 = int.Parse(AddressCode3);
                objLog.YearExploitation = decimal.Parse(YearExploitation);
                objLog.AlertAvailableWater = int.Parse(AlertAvailableWater);
                objLog.AlertAvailableElectric = int.Parse(AlertAvailableElectric);
                objLog.TypeCode = int.Parse(TypeCode);
                objLog.MeterPulse = int.Parse(MeterPulse);
                objLog.AlertWaterLevel = decimal.Parse(AlertWaterLevel);
                objLog.StationType = int.Parse(StationType);
                objLog.StationCode = int.Parse(StationCode);
                objLog.Frequency = int.Parse(Frequency);
                CardDeviceLogModule.AddCardDeviceLog(objLog);
            }
            catch (Exception ex)
            {
                myLogger.Error(ex.Message);
            }

            return result;
        }

        //修改卡
        private JavaScriptObject ModifyCardDevice(HttpRequest request)
        {
            string opUserId = request["opUserId"] ?? "0";
            string opUserName = request["opUserName"] ?? "";
            string opPassword = request["opPassword"] ?? "";
            string NetCardMAC = request["NetCardMAC"] ?? "";
            string SerialNumber = request["SerialNumber"] ?? "";

            string AddressCode1 = request["AddressCode1"] ?? "";
            string AddressCode2 = request["AddressCode2"] ?? "";
            string AddressCode3 = request["AddressCode3"] ?? "0";
            string YearExploitation = request["YearExploitation"] ?? "0";
            string AlertAvailableWater = request["AlertAvailableWater"] ?? "0";
            string AlertAvailableElectric = request["AlertAvailableElectric"] ?? "0";
            string TypeCode = request["TypeCode"] ?? "0";
            string MeterPulse = request["MeterPulse"] ?? "0";
            string AlertWaterLevel = request["AlertWaterLevel"] ?? "0";
            string StationType = request["StationType"] ?? "0";
            string StationCode = request["StationCode"] ?? "0";
            string Frequency = request["Frequency"] ?? "0";

            JavaScriptObject result = new JavaScriptObject();
            result.Add("Result", false);
            result.Add("Message", "");
            DateTime dateNow = DateTime.Now;
            CardDevice obj = null;
            try
            {
                obj = CardDeviceModule.GetCardDeviceBySerialNumber(SerialNumber);
                if (obj != null)
                {
                    obj.SerialNumber = SerialNumber;
                    obj.LastUpdateUserId = long.Parse(opUserId);
                    obj.LastUpdateUserName = opUserName;
                    obj.LastUpdateAddress = NetCardMAC;
                    obj.LastUpdateTime = dateNow;

                    obj.AddressCode1 = AddressCode1;
                    obj.AddressCode2 = AddressCode2;
                    obj.AddressCode3 = int.Parse(AddressCode3);
                    obj.YearExploitation = decimal.Parse(YearExploitation);
                    obj.AlertAvailableWater = int.Parse(AlertAvailableWater);
                    obj.AlertAvailableElectric = int.Parse(AlertAvailableElectric);
                    obj.TypeCode = int.Parse(TypeCode);
                    obj.MeterPulse = int.Parse(MeterPulse);
                    obj.AlertWaterLevel = decimal.Parse(AlertWaterLevel);
                    obj.StationType = int.Parse(StationType);
                    obj.StationCode = int.Parse(StationCode);
                    obj.Frequency = int.Parse(Frequency);

                    if (CardDeviceModule.ModifyCardDevice(obj) == "修改成功")
                    {
                        result["Result"] = true;
                        result["Message"] = "修改成功";
                    }
                    else
                    {
                        result["Message"] = "修改保存出错！";
                    }
                }
                else
                {
                    result["Message"] = "卡不存在，无法修改！";
                }
            }
            catch (Exception ex)
            {
                result["Message"] = "修改出错！";
                myLogger.Error(ex.Message);
            }

            try
            {
                CardDeviceLog objLog = new CardDeviceLog();
                objLog.SerialNumber = SerialNumber;
                objLog.LogUserId = long.Parse(opUserId);
                objLog.LogUserName = opUserName;
                objLog.LogAddress = NetCardMAC;
                objLog.LogTime = dateNow;
                objLog.LogType = "修改";
                objLog.LogContent = result["Result"] + "|" + result["Message"];
                objLog.AddressCode1 = AddressCode1;
                objLog.AddressCode2 = AddressCode2;
                objLog.AddressCode3 = int.Parse(AddressCode3);
                objLog.YearExploitation = decimal.Parse(YearExploitation);
                objLog.AlertAvailableWater = int.Parse(AlertAvailableWater);
                objLog.AlertAvailableElectric = int.Parse(AlertAvailableElectric);
                objLog.TypeCode = int.Parse(TypeCode);
                objLog.MeterPulse = int.Parse(MeterPulse);
                objLog.AlertWaterLevel = decimal.Parse(AlertWaterLevel);
                objLog.StationType = int.Parse(StationType);
                objLog.StationCode = int.Parse(StationCode);
                objLog.Frequency = int.Parse(Frequency);
                CardDeviceLogModule.AddCardDeviceLog(objLog);
            }
            catch (Exception ex)
            {
                myLogger.Error(ex.Message);
            }

            return result;
        }

        //注销卡
        private JavaScriptObject CancelCardDevice(HttpRequest request)
        {
            string opUserId = request["opUserId"] ?? "0";
            string opUserName = request["opUserName"] ?? "";
            string opPassword = request["opPassword"] ?? "";
            string NetCardMAC = request["NetCardMAC"] ?? "";
            string SerialNumber = request["SerialNumber"] ?? "";

            JavaScriptObject result = new JavaScriptObject();
            result.Add("Result", false);
            result.Add("Message", "");
            DateTime dateNow = DateTime.Now;
            CardDevice obj = null;
            try
            {
                obj = CardDeviceModule.GetCardDeviceBySerialNumber(SerialNumber);
                if (obj != null)
                {
                    if (CardDeviceModule.DeleteCardDevice(obj.Id) == "删除成功")
                    {
                        result["Result"] = true;
                        result["Message"] = "注销成功";
                    }
                    else
                    {
                        result["Message"] = "注销保存失败";
                    }
                }
                else
                {
                    result["Message"] = "卡不存在，无法注销！";
                }
            }
            catch (Exception ex)
            {
                result["Message"] = "注销出错！";
                myLogger.Error(ex.Message);
            }

            try
            {
                CardDeviceLog objLog = new CardDeviceLog();
                objLog.SerialNumber = SerialNumber;
                objLog.LogUserId = long.Parse(opUserId);
                objLog.LogUserName = opUserName;
                objLog.LogAddress = NetCardMAC;
                objLog.LogTime = dateNow;
                objLog.LogType = "注销";
                objLog.LogContent = result["Result"] + "|" + result["Message"];
                if (obj != null)
                {
                    objLog.AddressCode1 = obj.AddressCode1;
                    objLog.AddressCode2 = obj.AddressCode2;
                    objLog.AddressCode3 = obj.AddressCode3;
                    objLog.YearExploitation = obj.YearExploitation;
                    objLog.AlertAvailableWater = obj.AlertAvailableWater;
                    objLog.AlertAvailableElectric = obj.AlertAvailableElectric;
                    objLog.TypeCode = obj.TypeCode;
                    objLog.MeterPulse = obj.MeterPulse;
                    objLog.AlertWaterLevel = obj.AlertWaterLevel;
                    objLog.StationType = obj.StationType;
                    objLog.StationCode = obj.StationCode;
                    objLog.Frequency = obj.Frequency;
                }
                CardDeviceLogModule.AddCardDeviceLog(objLog);
            }
            catch (Exception ex)
            {
                myLogger.Error(ex.Message);
            }

            return result;
        }
        #endregion

        #region 用户卡
        //开卡
        private JavaScriptObject OpenCardUser(HttpRequest request)
        {
            string opUserId = request["opUserId"] ?? "0";
            string opUserName = request["opUserName"] ?? "";
            string opPassword = request["opPassword"] ?? "";
            string NetCardMAC = request["NetCardMAC"] ?? "";
            string SerialNumber = request["SerialNumber"] ?? "";

            string UserNo = request["UserNo"] ?? "";
            string UserName = request["UserName"] ?? "";
            string IdentityNumber = request["IdentityNumber"] ?? "";
            string Telephone = request["Telephone"] ?? "";
            string DeviceList = request["DeviceList"] ?? "";

            JavaScriptObject result = new JavaScriptObject();
            result.Add("Result", false);
            result.Add("Message", "");
            DateTime dateNow = DateTime.Now;
            CardUser obj = null;
            try
            {
                if (CardUserModule.ExistsSerialNumber(SerialNumber))
                {
                    result["Message"] = "已存在相同序列号卡，无法开卡！";
                }
                else if (CardUserModule.ExistsUserNo(UserNo))
                {
                    result["Message"] = "已存在相同用户卡号，无法开卡！";
                }
                else
                {
                    obj = new CardUser();
                    obj.SerialNumber = SerialNumber;
                    obj.OpenUserId = long.Parse(opUserId);
                    obj.OpenUserName = opUserName;
                    obj.OpenAddress = NetCardMAC;
                    obj.OpenTime = dateNow;
                    obj.LastUpdateUserId = 0;
                    obj.LastUpdateUserName = "";
                    obj.LastUpdateAddress = "";
                    obj.LastUpdateTime = DateTime.Parse("1900-1-1");
                    obj.LastChargeUserId = 0;
                    obj.LastChargeUserName = "";
                    obj.LastChargeAddress = "";
                    obj.LastChargeTime = DateTime.Parse("1900-1-1");
                    obj.LastConsumptionDeviceId = 0;
                    obj.LastConsumptionDeviceNo = "";
                    obj.LastConsumptionTime = DateTime.Parse("1900-1-1");

                    obj.IsCountermand = 0;
                    obj.CountermandContent = "";
                    obj.CountermandUserId = 0;
                    obj.CountermandUserName = "";
                    obj.CountermandAddress = "";
                    obj.CountermandTime = DateTime.Parse("1900-1-1");
                    obj.CountermandCancelContent = "";
                    obj.CountermandCancelUserId = 0;
                    obj.CountermandCancelUserName = "";
                    obj.CountermandCancelAddress = "";
                    obj.CountermandCancelTime = DateTime.Parse("1900-1-1");

                    obj.UserNo = UserNo.Trim().TrimStart('0');
                    obj.DeviceList = DeviceList;

                    List<WaterUser> list1 = WaterUserModule.GetWaterUserByIdentityNumber(IdentityNumber);
                    List<WaterUser> list2 = WaterUserModule.GetWaterUserByTelephone(Telephone);
                    List<WaterUser> list3 = WaterUserModule.GetWaterUserByUserName(UserName);
                    bool flag = false;
                    WaterUser waterUser = null;
                    if (list1.Count == 1 && list2.Count == 1 && list3.Count > 0)
                    {
                        foreach (WaterUser wu in list3)
                        {
                            if (list1[0].id == wu.id && list2[0].id == wu.id)
                            {
                                flag = true;
                                waterUser = wu;
                                break;
                            }
                        }
                    }
                    if (!flag)
                    {
                        waterUser = null;
                    }

                    if (waterUser == null)
                    {
                        result["Message"] = "此用户不存在，无法开卡";
                    }
                    else if (waterUser.State != "正常")
                    {
                        result["Message"] = "此用户状态不正常，无法开卡";
                    }
                    else
                    {
                        obj.WaterUserId = waterUser.id;
                        long id = CardUserModule.AddCardUser(obj);
                        if (id > 0)
                        {
                            Thread parameterThread = new Thread(new ParameterizedThreadStart(TcpRunThread.ParameterRun));
                            parameterThread.Start(ProtocolConst.WebToGateUpdateCache + ProtocolConst.UpdateCache_CardUser + "01" + obj.SerialNumber.PadLeft(16, '0'));
                            obj.Id = id;
                            result["Result"] = true;
                            result["Message"] = id;
                        }
                        else
                        {
                            result["Message"] = "开卡保存出错！";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result["Message"] = "开卡出错！";
                myLogger.Error(ex.Message);
            }

            try
            {
                CardUserLog objLog = new CardUserLog();
                objLog.SerialNumber = SerialNumber;
                objLog.LogUserId = long.Parse(opUserId);
                objLog.LogUserName = opUserName;
                objLog.LogAddress = NetCardMAC;
                objLog.LogTime = dateNow;
                objLog.LogType = "开卡";
                objLog.LogContent = result["Result"] + "|" + result["Message"] + "|" + UserNo + "|" + UserName + "|" + IdentityNumber + "|" + Telephone;
                if (obj != null)
                {
                    objLog.UserNo = obj.UserNo.Trim().TrimStart('0');
                    objLog.WaterUserId = obj.WaterUserId;
                    objLog.DeviceList = obj.DeviceList;
                    objLog.ResidualWater = obj.ResidualWater;
                    objLog.ResidualElectric = obj.ResidualElectric;
                    objLog.TotalWater = obj.TotalWater;
                    objLog.TotalElectric = obj.TotalElectric;
                    objLog.Remark = obj.Remark;
                }
                CardUserLogModule.AddCardUserLog(objLog);
            }
            catch (Exception ex)
            {
                myLogger.Error(ex.Message);
            }

            return result;
        }

        //修改卡
        private JavaScriptObject ModifyCardUser(HttpRequest request)
        {
            string opUserId = request["opUserId"] ?? "0";
            string opUserName = request["opUserName"] ?? "";
            string opPassword = request["opPassword"] ?? "";
            string NetCardMAC = request["NetCardMAC"] ?? "";
            string SerialNumber = request["SerialNumber"] ?? "";

            string UserNo = request["UserNo"] ?? "";
            string UserName = request["UserName"] ?? "";
            string IdentityNumber = request["IdentityNumber"] ?? "";
            string Telephone = request["Telephone"] ?? "";
            string DeviceList = request["DeviceList"] ?? "";

            JavaScriptObject result = new JavaScriptObject();
            result.Add("Result", false);
            result.Add("Message", "");
            DateTime dateNow = DateTime.Now;
            CardUser obj = null;
            try
            {
                obj = CardUserModule.GetCardUserBySerialNumber(SerialNumber);
                if (obj != null)
                {
                    if (CardUserModule.ExistsUserNo(UserNo, obj.Id))
                    {
                        result["Message"] = "已存在相同用户卡号，无法修改！";
                    }
                    else
                    {
                        obj.SerialNumber = SerialNumber;
                        obj.LastUpdateUserId = long.Parse(opUserId);
                        obj.LastUpdateUserName = opUserName;
                        obj.LastUpdateAddress = NetCardMAC;
                        obj.LastUpdateTime = dateNow;

                        obj.UserNo = UserNo.Trim().TrimStart('0');
                        obj.DeviceList = DeviceList;

                        List<WaterUser> list1 = WaterUserModule.GetWaterUserByIdentityNumber(IdentityNumber);
                        List<WaterUser> list2 = WaterUserModule.GetWaterUserByTelephone(Telephone);
                        List<WaterUser> list3 = WaterUserModule.GetWaterUserByUserName(UserName);
                        bool flag = false;
                        WaterUser waterUser = null;
                        if (list1.Count == 1 && list2.Count == 1 && list3.Count > 0)
                        {
                            foreach (WaterUser wu in list3)
                            {
                                waterUser = wu;
                                if (list1[0].id == wu.id && list2[0].id == wu.id)
                                {
                                    flag = true;
                                    break;
                                }
                            }
                        }
                        if (!flag)
                        {
                            waterUser = null;
                        }

                        if (waterUser == null)
                        {
                            result["Message"] = "此用户不存在，无法修改";
                        }
                        else if (waterUser.State != "正常")
                        {
                            result["Message"] = "此用户状态不正常，无法修改";
                        }
                        else
                        {
                            obj.WaterUserId = waterUser.id;

                            if (CardUserModule.ModifyCardUser(obj) == "修改成功")
                            {
                                Thread parameterThread = new Thread(new ParameterizedThreadStart(TcpRunThread.ParameterRun));
                                parameterThread.Start(ProtocolConst.WebToGateUpdateCache + ProtocolConst.UpdateCache_CardUser + "01" + obj.SerialNumber.PadLeft(16, '0'));  

                                result["Result"] = true;
                                result["Message"] = "修改成功";
                            }
                            else
                            {
                                result["Message"] = "修改保存出错！";
                            }
                        }
                    }
                }
                else
                {
                    result["Message"] = "卡不存在，无法修改！";
                }
            }
            catch (Exception ex)
            {
                result["Message"] = "修改出错！";
                myLogger.Error(ex.Message);
            }

            try
            {
                CardUserLog objLog = new CardUserLog();
                objLog.SerialNumber = SerialNumber;
                objLog.LogUserId = long.Parse(opUserId);
                objLog.LogUserName = opUserName;
                objLog.LogAddress = NetCardMAC;
                objLog.LogTime = dateNow;
                objLog.LogType = "修改";
                objLog.LogContent = result["Result"] + "|" + result["Message"];
                if (obj != null)
                {
                    objLog.UserNo = obj.UserNo.Trim().TrimStart('0');
                    objLog.WaterUserId = obj.WaterUserId;
                    objLog.DeviceList = obj.DeviceList;
                    objLog.ResidualWater = obj.ResidualWater;
                    objLog.ResidualElectric = obj.ResidualElectric;
                    objLog.TotalWater = obj.TotalWater;
                    objLog.TotalElectric = obj.TotalElectric;
                    objLog.Remark = obj.Remark;
                }
                CardUserLogModule.AddCardUserLog(objLog);
            }
            catch (Exception ex)
            {
                myLogger.Error(ex.Message);
            }

            return result;
        }

        //注销卡
        private JavaScriptObject CancelCardUser(HttpRequest request)
        {
            string opUserId = request["opUserId"] ?? "0";
            string opUserName = request["opUserName"] ?? "";
            string opPassword = request["opPassword"] ?? "";
            string NetCardMAC = request["NetCardMAC"] ?? "";
            string SerialNumber = request["SerialNumber"] ?? "";

            JavaScriptObject result = new JavaScriptObject();
            result.Add("Result", false);
            result.Add("Message", "");
            DateTime dateNow = DateTime.Now;
            CardUser obj = null;
            try
            {
                obj = CardUserModule.GetCardUserBySerialNumber(SerialNumber);
                if (obj != null)
                {
                    string str = CardUserModule.DeleteCardUser(obj.Id);
                    if (str == "删除成功")
                    {
                        Thread parameterThread = new Thread(new ParameterizedThreadStart(TcpRunThread.ParameterRun));
                        parameterThread.Start(ProtocolConst.WebToGateUpdateCache + ProtocolConst.UpdateCache_CardUser + "02" + obj.SerialNumber.PadLeft(16, '0'));  

                        result["Result"] = true;
                        result["Message"] = "注销成功";
                    }
                    else
                    {
                        result["Message"] = str;
                    }
                }
                else
                {
                    result["Message"] = "卡不存在，无法注销！";
                }
            }
            catch (Exception ex)
            {
                result["Message"] = "注销出错！";
                myLogger.Error(ex.Message);
            }

            try
            {
                CardUserLog objLog = new CardUserLog();
                objLog.SerialNumber = SerialNumber;
                objLog.LogUserId = long.Parse(opUserId);
                objLog.LogUserName = opUserName;
                objLog.LogAddress = NetCardMAC;
                objLog.LogTime = dateNow;
                objLog.LogType = "注销";
                objLog.LogContent = result["Result"] + "|" + result["Message"];
                if (obj != null)
                {
                    objLog.UserNo = obj.UserNo.Trim().TrimStart('0');
                    objLog.WaterUserId = obj.WaterUserId;
                    objLog.DeviceList = obj.DeviceList;
                    objLog.ResidualWater = obj.ResidualWater;
                    objLog.ResidualElectric = obj.ResidualElectric;
                    objLog.TotalWater = obj.TotalWater;
                    objLog.TotalElectric = obj.TotalElectric;
                    objLog.Remark = obj.Remark;
                }
                CardUserLogModule.AddCardUserLog(objLog);
            }
            catch (Exception ex)
            {
                myLogger.Error(ex.Message);
            }

            return result;
        }

        //充值
        private JavaScriptObject RechargeCardUser(HttpRequest request)
        {
            string opUserId = request["opUserId"] ?? "0";
            string opUserName = request["opUserName"] ?? "";
            string opPassword = request["opPassword"] ?? "";
            string NetCardMAC = request["NetCardMAC"] ?? "";
            string SerialNumber = request["SerialNumber"] ?? "";

            string UserNo = request["UserNo"] ?? "";
            string UserName = request["UserName"] ?? "";
            string IdentityNumber = request["IdentityNumber"] ?? "";
            string Telephone = request["Telephone"] ?? "";

            string WaterPrice = request["WaterPrice"] ?? "";
            string WaterNum = request["WaterNum"] ?? "";
            string ElectricPrice = request["ElectricPrice"] ?? "";
            string ElectricNum = request["ElectricNum"] ?? "";
            string WaterUsed = request["WaterUsed"] ?? "";
            string ElectricUsed = request["ElectricUsed"] ?? "";
            string Remark = request["Remark"] ?? "";

            JavaScriptObject result = new JavaScriptObject();
            result.Add("Result", false);
            result.Add("Message", "");

            decimal d_WaterPrice = 0;
            try
            {
                d_WaterPrice = decimal.Parse(WaterPrice);
            }
            catch { result["Message"] = "充水金额格式不对"; return result; }
            decimal d_WaterNum = 0;
            try
            {
                d_WaterNum = decimal.Parse(WaterNum);
            }
            catch { result["Message"] = "充水数量格式不对"; return result; }
            decimal d_ElectricPrice = 0;
            try
            {
                d_ElectricPrice = decimal.Parse(ElectricPrice);
            }
            catch { result["Message"] = "充电金额格式不对"; return result; }
            decimal d_ElectricNum = 0;
            try
            {
                d_ElectricNum = decimal.Parse(ElectricNum);
            }
            catch { result["Message"] = "充电数量格式不对"; return result; }
            decimal d_WaterUsed = 0;
            try
            {
                d_WaterUsed = decimal.Parse(WaterUsed);
            }
            catch { result["Message"] = "年累计用水量格式不对"; return result; }
            decimal d_ElectricUsed = 0;
            try
            {
                d_ElectricUsed = decimal.Parse(ElectricUsed);
            }
            catch { result["Message"] = "年累计用电量格式不对"; return result; }

            DateTime dateNow = DateTime.Now;
            CardUser obj = null;
            try
            {
                obj = CardUserModule.GetCardUserBySerialNumber(SerialNumber);
                if (obj != null)
                {
                    List<WaterUser> list1 = WaterUserModule.GetWaterUserByIdentityNumber(IdentityNumber);
                    List<WaterUser> list2 = WaterUserModule.GetWaterUserByTelephone(Telephone);
                    List<WaterUser> list3 = WaterUserModule.GetWaterUserByUserName(UserName);
                    bool flag = false;
                    WaterUser waterUser = null;
                    if (list1.Count == 1 && list2.Count == 1 && list3.Count > 0)
                    {
                        foreach (WaterUser wu in list3)
                        {
                            waterUser = wu;
                            if (list1[0].id == wu.id && list2[0].id == wu.id)
                            {
                                flag = true;
                                break;
                            }
                        }
                    }
                    if (!flag)
                    {
                        waterUser = null;
                    }

                    if (waterUser == null)
                    {
                        result["Message"] = "此用户不存在，无法充值";
                    }
                    else if (waterUser.State != "正常")
                    {
                        result["Message"] = "此用户状态不正常，无法充值";
                    }
                    else
                    {
                        CardUserRechargeLog objRechargeLog = new CardUserRechargeLog();

                        objRechargeLog.SerialNumber = SerialNumber;
                        objRechargeLog.UserNo = UserNo;
                        objRechargeLog.WateUserId = waterUser.id;

                        objRechargeLog.LogUserId = long.Parse(opUserId);
                        objRechargeLog.LogUserName = opUserName;
                        objRechargeLog.LogAddress = NetCardMAC;
                        objRechargeLog.LogTime = dateNow;
                        objRechargeLog.LogType = "充值";

                        objRechargeLog.RechargeType = "充值";
                        objRechargeLog.WaterPrice = d_WaterPrice;
                        objRechargeLog.WaterNum = d_WaterNum;
                        objRechargeLog.ElectricPrice = d_ElectricPrice;
                        objRechargeLog.ElectricNum = d_ElectricNum;
                        objRechargeLog.TotalPrice = objRechargeLog.WaterPrice + objRechargeLog.ElectricPrice;
                        objRechargeLog.Remark = Remark;

                        objRechargeLog.WaterQuota = waterUser.WaterQuota;
                        objRechargeLog.ElectricQuota = waterUser.ElectricQuota;

                        objRechargeLog.WaterPriceId = waterUser.水价ID;
                        objRechargeLog.ElectricPriceId = waterUser.电价ID;
                        objRechargeLog.WaterUsed = d_WaterUsed;
                        objRechargeLog.ElectricUsed = d_ElectricUsed;

                        long id = CardUserRechargeLogModule.AddCardUserRechargeLog(objRechargeLog);
                        if (id > 0)
                        {
                            try
                            {
                                obj.LastChargeAddress = objRechargeLog.LogAddress;
                                obj.LastChargeTime = objRechargeLog.LogTime;
                                obj.LastChargeUserId = objRechargeLog.LogUserId;
                                obj.LastChargeUserName = objRechargeLog.LogUserName;

                                obj.ResidualWater += d_WaterNum;
                                obj.ResidualElectric += d_ElectricNum;
                                obj.TotalWater = d_WaterUsed;
                                obj.TotalElectric = d_ElectricUsed;
                                CardUserModule.ModifyCardUser(obj);
                            }
                            catch(Exception ex) {
                                myLogger.Error("充值保存用户卡信息出错！！" + ex.Message);
                            }
                            result["Result"] = true;
                            result["Message"] = "充值成功";
                        }
                        else
                        {
                            result["Message"] = "充值记录保存失败！";
                        }
                    }

                }
                else
                {
                    result["Message"] = "卡不存在，无法充值！";
                }
            }
            catch (Exception ex)
            {
                result["Message"] = "充值出错";
                myLogger.Error(ex.Message);
            }

            try
            {
                CardUserLog objLog = new CardUserLog();
                objLog.SerialNumber = SerialNumber;
                objLog.LogUserId = long.Parse(opUserId);
                objLog.LogUserName = opUserName;
                objLog.LogAddress = NetCardMAC;
                objLog.LogTime = dateNow;
                objLog.LogType = "充值";
                objLog.LogContent = result["Result"] + "|" + result["Message"];
                if (obj != null)
                {
                    objLog.UserNo = obj.UserNo.Trim().TrimStart('0');
                    objLog.WaterUserId = obj.WaterUserId;
                    objLog.DeviceList = obj.DeviceList;
                    objLog.ResidualWater = obj.ResidualWater;
                    objLog.ResidualElectric = obj.ResidualElectric;
                    objLog.TotalWater = obj.TotalWater;
                    objLog.TotalElectric = obj.TotalElectric;
                    objLog.Remark = obj.Remark;
                }
                CardUserLogModule.AddCardUserLog(objLog);
            }
            catch (Exception ex)
            {
                myLogger.Error(ex.Message);
            }

            return result;
        }

        //挂失卡
        private JavaScriptObject CountermandCardUser(HttpRequest request)
        {
            string opUserId = request["opUserId"] ?? "0";
            string opUserName = request["opUserName"] ?? "";
            string opPassword = request["opPassword"] ?? "";
            string NetCardMAC = request["NetCardMAC"] ?? "";
            string SerialNumber = request["SerialNumber"] ?? "";
            string loginIdentifer = request["loginIdentifer"];

            JavaScriptObject result = new JavaScriptObject();
            result.Add("Result", false);
            result.Add("Message", "");

            if (loginIdentifer != null)
            {
                LoginUser loginUser = GlobalAppModule.GetLoginUser(loginIdentifer);
                if (loginUser == null)
                {
                    result["Message"] = "未登录";
                    return result;
                }
                if (loginUser.LoginTimeout)
                {
                    result["Message"] = "登录超时";
                    return result;
                }
                loginUser.LastOperateTime = DateTime.Now;
                opUserId = loginUser.UserId.ToString();
                opUserName = loginUser.LoginName;
                NetCardMAC = ToolsWeb.GetIP(request);
            }

            DateTime dateNow = DateTime.Now;

            CardUser obj = null;
            try
            {
                obj = CardUserModule.GetCardUserBySerialNumber(SerialNumber);
                if (obj == null)
                {
                    result["Message"] = "卡不存在，无法挂失！";
                }
                else if (obj.IsCountermand == 0)
                {
                    try
                    {
                        obj.IsCountermand = 1;
                        obj.CountermandContent = "";
                        obj.CountermandUserId = long.Parse(opUserId);
                        obj.CountermandUserName = opUserName;
                        obj.CountermandAddress = NetCardMAC;
                        obj.CountermandTime = dateNow;

                        CardUserModule.ModifyCardUser(obj);

                        result["Result"] = true;
                    }
                    catch
                    {
                        result["Message"] = "挂失保存失败";
                    }
                }
                else
                {
                    result["Message"] = "卡已挂失，无法重复挂失！";
                }
            }
            catch (Exception ex)
            {
                result["Message"] = "挂失失败";
                myLogger.Error(ex.Message);
            }

            try
            {
                CardUserLog objLog = new CardUserLog();
                objLog.SerialNumber = SerialNumber;
                objLog.LogUserId = long.Parse(opUserId);
                objLog.LogUserName = opUserName;
                objLog.LogAddress = NetCardMAC;
                objLog.LogTime = dateNow;
                objLog.LogType = "挂失";
                objLog.LogContent = result["Result"] + "|" + result["Message"];
                if (obj != null)
                {
                    objLog.UserNo = obj.UserNo.Trim().TrimStart('0');
                    objLog.WaterUserId = obj.WaterUserId;
                    objLog.DeviceList = obj.DeviceList;
                    objLog.ResidualWater = obj.ResidualWater;
                    objLog.ResidualElectric = obj.ResidualElectric;
                    objLog.TotalWater = obj.TotalWater;
                    objLog.TotalElectric = obj.TotalElectric;
                    objLog.Remark = obj.Remark;
                }
                CardUserLogModule.AddCardUserLog(objLog);
            }
            catch (Exception ex)
            {
                myLogger.Error(ex.Message);
            }

            return result;
        }

        //取消挂失卡
        private JavaScriptObject CountermandCancelCardUser(HttpRequest request)
        {
            string opUserId = request["opUserId"] ?? "0";
            string opUserName = request["opUserName"] ?? "";
            string opPassword = request["opPassword"] ?? "";
            string NetCardMAC = request["NetCardMAC"] ?? "";
            string SerialNumber = request["SerialNumber"] ?? "";
            string loginIdentifer = request["loginIdentifer"];

            JavaScriptObject result = new JavaScriptObject();
            result.Add("Result", false);
            result.Add("Message", "");

            if (loginIdentifer != null)
            {
                LoginUser loginUser = GlobalAppModule.GetLoginUser(loginIdentifer);
                if (loginUser == null)
                {
                    result["Message"] = "未登录";
                    return result;
                }
                if (loginUser.LoginTimeout)
                {
                    result["Message"] = "登录超时";
                    return result;
                }
                loginUser.LastOperateTime = DateTime.Now;
                opUserId = loginUser.UserId.ToString();
                opUserName = loginUser.LoginName;
                NetCardMAC = ToolsWeb.GetIP(request);
            }

            DateTime dateNow = DateTime.Now;

            CardUser obj = null;
            try
            {
                obj = CardUserModule.GetCardUserBySerialNumber(SerialNumber);
                if (obj == null)
                {
                    result["Message"] = "卡不存在，无法取消挂失！";
                }
                else if (obj.IsCountermand == 1)
                {
                    try
                    {
                        obj.IsCountermand = 0;
                        obj.CountermandCancelContent = "";
                        obj.CountermandCancelUserId = long.Parse(opUserId);
                        obj.CountermandCancelUserName = opUserName;
                        obj.CountermandCancelAddress = NetCardMAC;
                        obj.CountermandCancelTime = dateNow;

                        CardUserModule.ModifyCardUser(obj);

                        result["Result"] = true;
                    }
                    catch
                    {
                        result["Message"] = "取消挂失保存失败";
                    }
                }
                else
                {
                    result["Message"] = "卡未挂失，无法重复取消挂失！";
                }
            }
            catch (Exception ex)
            {
                result["Message"] = "取消挂失失败";
                myLogger.Error(ex.Message);
            }

            try
            {
                CardUserLog objLog = new CardUserLog();
                objLog.SerialNumber = SerialNumber;
                objLog.LogUserId = long.Parse(opUserId);
                objLog.LogUserName = opUserName;
                objLog.LogAddress = NetCardMAC;
                objLog.LogTime = dateNow;
                objLog.LogType = "取消挂失";
                objLog.LogContent = result["Result"] + "|" + result["Message"];
                if (obj != null)
                {
                    objLog.UserNo = obj.UserNo.Trim().TrimStart('0');
                    objLog.WaterUserId = obj.WaterUserId;
                    objLog.DeviceList = obj.DeviceList;
                    objLog.ResidualWater = obj.ResidualWater;
                    objLog.ResidualElectric = obj.ResidualElectric;
                    objLog.TotalWater = obj.TotalWater;
                    objLog.TotalElectric = obj.TotalElectric;
                    objLog.Remark = obj.Remark;
                }
                CardUserLogModule.AddCardUserLog(objLog);
            }
            catch (Exception ex)
            {
                myLogger.Error(ex.Message);
            }

            return result;
        }

        //重新开卡
        private JavaScriptObject ReOpenCardUser(HttpRequest request)
        {
            string opUserId = request["opUserId"] ?? "0";
            string opUserName = request["opUserName"] ?? "";
            string opPassword = request["opPassword"] ?? "";
            string NetCardMAC = request["NetCardMAC"] ?? "";
            string SerialNumberOld = request["SerialNumberOld"] ?? "";
            string SerialNumberNew = request["SerialNumberNew"] ?? "";

            JavaScriptObject result = new JavaScriptObject();
            result.Add("Result", false);
            result.Add("Message", "");

            DateTime dateNow = DateTime.Now;
            CardUser objOld = null;
            try
            {
                objOld = CardUserModule.GetCardUserBySerialNumber(SerialNumberOld);
                CardUser objNew = CardUserModule.GetCardUserBySerialNumber(SerialNumberNew);

                if (objOld == null)
                {
                    result["Message"] = "卡不存在，无法重新开卡！";
                }
                else if (objNew != null)
                {
                    result["Message"] = "新卡已存在，无法重新开卡！";
                }
                else if (objOld.IsCountermand == 1)
                {
                    objNew = new CardUser();
                    objNew.SerialNumber = SerialNumberNew;
                    objNew.OpenUserId = long.Parse(opUserId);
                    objNew.OpenUserName = opUserName;
                    objNew.OpenAddress = NetCardMAC;
                    objNew.OpenTime = dateNow;
                    objNew.LastUpdateUserId = 0;
                    objNew.LastUpdateUserName = "";
                    objNew.LastUpdateAddress = "";
                    objNew.LastUpdateTime = DateTime.Parse("1900-1-1");
                    objNew.LastChargeUserId = 0;
                    objNew.LastChargeUserName = "";
                    objNew.LastChargeAddress = "";
                    objNew.LastChargeTime = dateNow;
                    objNew.LastConsumptionDeviceId = 0;
                    objNew.LastConsumptionDeviceNo = "";
                    objNew.LastConsumptionTime = DateTime.Parse("1900-1-1");

                    objNew.IsCountermand = 0;
                    objNew.CountermandContent = "";
                    objNew.CountermandUserId = 0;
                    objNew.CountermandUserName = "";
                    objNew.CountermandAddress = "";
                    objNew.CountermandTime = DateTime.Parse("1900-1-1");
                    objNew.CountermandCancelContent = "";
                    objNew.CountermandCancelUserId = 0;
                    objNew.CountermandCancelUserName = "";
                    objNew.CountermandCancelAddress = "";
                    objNew.CountermandCancelTime = DateTime.Parse("1900-1-1");

                    objNew.UserNo = objOld.UserNo.Trim().TrimStart('0');
                    objNew.WaterUserId = objOld.WaterUserId;
                    objNew.DeviceList = objOld.DeviceList;
                    objNew.ResidualWater = objOld.ResidualWater;
                    objNew.ResidualElectric = objOld.ResidualElectric;
                    objNew.TotalWater = objOld.TotalWater;
                    objNew.TotalElectric = objOld.TotalElectric;

                    CardUserModule.DeleteCardUser(objOld.Id);
                    long id = CardUserModule.AddCardUser(objNew);
                    if (id > 0)
                    {
                        result["Result"] = true;
                        result["Message"] = id;
                    }
                    else
                    {
                        CardUserModule.AddCardUser(objOld);
                        result["Message"] = "重新开卡保存出错！";
                    }

                }
                else
                {
                    result["Message"] = "卡未挂失，无法重新开卡！";
                }
            }
            catch (Exception ex)
            {
                result["Message"] = "重新开卡失败";
                myLogger.Error(ex.Message);
            }

            try
            {
                CardUserLog objLog = new CardUserLog();
                objLog.SerialNumber = SerialNumberOld;
                objLog.LogUserId = long.Parse(opUserId);
                objLog.LogUserName = opUserName;
                objLog.LogAddress = NetCardMAC;
                objLog.LogTime = dateNow;
                objLog.LogType = "重新开卡";
                objLog.LogContent = result["Result"] + "|" + result["Message"];
                if (objOld != null)
                {
                    objLog.UserNo = objOld.UserNo.Trim().TrimStart('0');
                    objLog.WaterUserId = objOld.WaterUserId;
                    objLog.DeviceList = objOld.DeviceList;
                    objLog.ResidualWater = objOld.ResidualWater;
                    objLog.ResidualElectric = objOld.ResidualElectric;
                    objLog.TotalWater = objOld.TotalWater;
                    objLog.TotalElectric = objOld.TotalElectric;
                }
                objLog.Remark = SerialNumberNew;
                CardUserLogModule.AddCardUserLog(objLog);
            }
            catch (Exception ex)
            {
                myLogger.Error(ex.Message);
            }

            return result;
        }
        #endregion

        #region 查询用户卡列表
        public JavaScriptObject GetCardUserList(HttpRequest request)
        {
            string opUserId = request["opUserId"] ?? "0";
            string opUserName = request["opUserName"] ?? "";
            string opPassword = request["opPassword"] ?? "";
            string NetCardMAC = request["NetCardMAC"] ?? "";

            string UserNo = request["UserNo"] ?? "";
            string UserName = request["UserName"] ?? "";
            string IdentityNumber = request["IdentityNumber"] ?? "";
            string Telephone = request["Telephone"] ?? "";

            JavaScriptObject result = new JavaScriptObject();
            result.Add("Result", false);
            result.Add("Message", "");

            /*
            if (UserNo.Trim() == "" && UserName.Trim() == "" && IdentityNumber.Trim() == "" && Telephone.Trim() == "")
            {
                result["Message"] = "至少要有一个查询条件不为空";
                return result;
            }
            */
            try
            {
                long userID = 0;
                string str = SysUserModule.Login(opUserName, opPassword, ToolsWeb.GetIP(request), NetCardMAC, ref userID);

                if (str == "登陆成功")
                {
                    JsonSysUser user = new JsonSysUser();
                    SysUser model = SysUserModule.GetUser(userID);
                    if (model.IsAllow == 0)
                    {
                        result["Message"] = "用户已被禁止登陆";
                    }
                    else
                    {
                        List<long> list_DistrictID = DistrictModule.GetAllDistrictID(model.DistrictId);

                        List<JsonCardUser> list = new List<JsonCardUser>();

                        string strSql = "select cu.[Id],[SerialNumber],[UserNo],[WaterUserId],[ResidualWater],[ResidualElectric],[TotalWater],[TotalElectric],[ResidualMoney],[TotallMoney],[DeviceList],cu.[Remark],[OpenUserId],[OpenUserName],[OpenAddress],case when [OpenTime]>'2011-1-1' then CONVERT(varchar(20),[OpenTime],120) else '' end [OpenTime],[LastConsumptionDeviceId],[LastConsumptionDeviceNo],case when [LastConsumptionTime]>'2011-1-1' then CONVERT(varchar(20),[LastConsumptionTime],120) else '' end [LastConsumptionTime],[LastChargeUserId],[LastChargeUserName],[LastChargeAddress],case when [LastChargeTime]>'2011-1-1' then CONVERT(varchar(20),[LastChargeTime],120) else '' end [LastChargeTime],[LastUpdateUserId],[LastUpdateUserName],[LastUpdateAddress],case when [LastUpdateTime]>'2011-1-1' then CONVERT(varchar(20),[LastUpdateTime],120) else '' end [LastUpdateTime],[IsCountermand],[CountermandContent],[CountermandUserId],[CountermandUserName],[CountermandAddress],case when [CountermandTime]>'2011-1-1' then CONVERT(varchar(20),[CountermandTime],120) else '' end [CountermandTime],[CountermandCancelContent],[CountermandCancelUserId],[CountermandCancelUserName],[CountermandCancelAddress],case when [CountermandCancelTime]>'2011-1-1' then CONVERT(varchar(20),[CountermandCancelTime],120) else '' end [CountermandCancelTime],wu.UserName,wu.Password,wu.DistrictId,wu.IdentityNumber,wu.Telephone from CardUser cu,WaterUser wu where cu.WaterUserId=wu.id";
                        List<SqlParameter> parameters = new List<SqlParameter>();
                        if (UserNo.Trim() != "")
                        {
                            strSql += " and cu.UserNo like @UserNo";
                            SqlParameter par = new SqlParameter("@UserNo", SqlDbType.VarChar, 50);
                            par.Value = "%" + UserNo.Trim() + "%";
                            parameters.Add(par);
                        }
                        if (UserName.Trim() != "")
                        {
                            strSql += " and wu.UserName like @UserName";
                            SqlParameter par = new SqlParameter("@UserName", SqlDbType.VarChar, 50);
                            par.Value = "%" + UserName.Trim() + "%";
                            parameters.Add(par);
                        }
                        if (IdentityNumber.Trim() != "")
                        {
                            strSql += " and wu.IdentityNumber like @IdentityNumber";
                            SqlParameter par = new SqlParameter("@IdentityNumber", SqlDbType.VarChar, 50);
                            par.Value = "%" + IdentityNumber.Trim() + "%";
                            parameters.Add(par);
                        }
                        if (Telephone.Trim() != "")
                        {
                            strSql += " and wu.Telephone like @Telephone";
                            SqlParameter par = new SqlParameter("@Telephone", SqlDbType.VarChar, 50);
                            par.Value = "%" + Telephone.Trim() + "%";
                            parameters.Add(par);
                        }
                        try
                        {
                            DataTable table = DbHelperSQL.Query(strSql, parameters.ToArray()).Tables[0];
                            if (table.Rows.Count != 0)
                            {
                                ModelHandler<JsonCardUser> modelHandler = new ModelHandler<JsonCardUser>();
                                for (int i = 0; i < table.Rows.Count; i++)
                                {
                                    DataRow dataRow = table.Rows[i];
                                    JsonCardUser jcu = modelHandler.FillModel(dataRow);
                                    if (list_DistrictID.Contains(jcu.DistrictId))
                                    {
                                        list.Add(jcu);
                                    }
                                }

                            }
                        }
                        catch (Exception ex) { myLogger.Error(ex.Message); }

                        result["Result"] = true;
                        result["Message"] = JavaScriptConvert.SerializeObject(list);
                    }
                }
            }
            catch (Exception exception)
            {
                result["Message"] = exception.Message;
                myLogger.Error(exception.Message);
            }

            return result;
        }
        #endregion

        #region 获取用户信息
        public JavaScriptObject GetWaterUserInfo(HttpRequest request)
        {
            string opUserId = request["opUserId"] ?? "0";
            string opUserName = request["opUserName"] ?? "";
            string opPassword = request["opPassword"] ?? "";
            string NetCardMAC = request["NetCardMAC"] ?? "";

            string UserName = request["UserName"] ?? "";
            string IdentityNumber = request["IdentityNumber"] ?? "";
            string Telephone = request["Telephone"] ?? "";

            JavaScriptObject result = new JavaScriptObject();
            result.Add("Result", false);
            result.Add("Message", "");
            List<WaterUser> list_WaterUser = null;
            
            long userID = 0;
            string str = SysUserModule.Login(opUserName, opPassword, ToolsWeb.GetIP(request), NetCardMAC, ref userID);

            if (str == "登陆成功")
            {
                JsonSysUser user = new JsonSysUser();
                SysUser model = SysUserModule.GetUser(userID);
                if (model.IsAllow == 0)
                {
                    result["Message"] = "用户已被禁止登陆";
                }
                else
                {
                    List<long> list_DistrictID = DistrictModule.GetAllDistrictID(model.DistrictId);
                    if (IdentityNumber.Trim() != "")
                    {
                        try
                        {
                            list_WaterUser = WaterUserModule.GetWaterUserByIdentityNumberLike(IdentityNumber);
                            
                        }
                        catch { }
                    }
                    else if (Telephone.Trim() != "")
                    {
                        try
                        {
                            list_WaterUser = WaterUserModule.GetWaterUserByTelephoneLike(Telephone);
                        }
                        catch { }
                    }

                    else if (UserName.Trim() != "")
                    {
                        try
                        {
                            list_WaterUser = WaterUserModule.GetWaterUserByUserNameLike(UserName);
                        }
                        catch { }
                    }
                    else
                    {
                        //result["Message"] = "查询条件不全无法查询";
                        try
                        {
                            list_WaterUser = WaterUserModule.GetAllWaterUser();
                        }
                        catch { }
                    }

                    if (list_WaterUser != null)
                    {
                        List<WaterUser> list = new List<WaterUser>();
                        foreach (WaterUser wu in list_WaterUser)
                        {
                            if (list_DistrictID.Contains(wu.DistrictId))
                            {
                                list.Add(wu);
                            }
                        }
                        result["Result"] = true;
                        result["Message"] = JavaScriptConvert.SerializeObject(list);
                    }
                    else
                    {
                        result["Message"] = "查询失败！";
                    }
                }
            }

            return result;
        }

        public JavaScriptObject GetCardUserRechargeInfo(HttpRequest request)
        {
            string opUserId = request["opUserId"] ?? "0";
            string opUserName = request["opUserName"] ?? "";
            string opPassword = request["opPassword"] ?? "";
            string NetCardMAC = request["NetCardMAC"] ?? "";

            string WaterUserId = request["WaterUserId"] ?? "0";

            JavaScriptObject result = new JavaScriptObject();
            result.Add("Result", false);
            result.Add("Message", "");

            try
            {
                WaterUser waterUser = WaterUserModule.GetWaterUserById(long.Parse(WaterUserId));

                int 水价ID = waterUser.水价ID;
                int 电价ID = waterUser.电价ID;
                decimal WaterQuota = waterUser.WaterQuota;
                decimal ElectricQuota = waterUser.ElectricQuota;
                decimal UnitPriceWater = -1;
                decimal UnitPriceElectric = -1;
                decimal WaterLevel = -1;
                decimal ElectricLevel = -1;
                
                PriceInfo pi1 = PriceModule.GetPriceInfoById(水价ID.ToString());
                PriceInfo pi2 = PriceModule.GetPriceInfoById(电价ID.ToString());

                DataTable dt1 = CardUserRechargeLogModule.GetCardUserRechargeYear(waterUser.id, DateTime.Now.Year);

                decimal WaterUsed = decimal.Parse(dt1.Rows[0][0].ToString());
                decimal ElectricUsed = decimal.Parse(dt1.Rows[0][1].ToString());

                if (pi1.LadderType == PriceInfo.LaddersType.百分比)
                {
                    if (pi1.LaddersCount > 1)
                    {
                        decimal d1 = pi1.FirstVolume / 100 * WaterQuota;
                        decimal d2 = pi1.SecondVolume / 100 * WaterQuota;
                        decimal d3 = pi1.ThirdVolume / 100 * WaterQuota;
                        decimal d4 = pi1.FourthVolume / 100 * WaterQuota;

                        if (WaterUsed >= d3 && d3 > 0) { UnitPriceWater = pi1.FourthPrice; WaterLevel = -1; }
                        else if (WaterUsed >= d2 && d2 > 0) { UnitPriceWater = pi1.ThirdPrice; WaterLevel = d3 - WaterUsed; }
                        else if (WaterUsed >= d1 && d1 > 0) { UnitPriceWater = pi1.SecondPrice; WaterLevel = d2 - WaterUsed; }
                        else { UnitPriceWater = pi1.FirstPrice; WaterLevel = d1 - WaterUsed; }
                    }
                    else
                    {
                        UnitPriceWater = pi1.FirstPrice;
                    }
                }
                else
                {
                    if (pi1.LaddersCount > 1)
                    {
                        decimal d1 = pi1.FirstVolume;
                        decimal d2 = pi1.SecondVolume;
                        decimal d3 = pi1.ThirdVolume;
                        decimal d4 = pi1.FourthVolume;

                        if (WaterUsed >= d3 && d3 > 0) { UnitPriceWater = pi1.FourthPrice; WaterLevel = -1; }
                        else if (WaterUsed >= d2 && d2 > 0) { UnitPriceWater = pi1.ThirdPrice; WaterLevel = d3 - WaterUsed; }
                        else if (WaterUsed >= d1 && d1 > 0) { UnitPriceWater = pi1.SecondPrice; WaterLevel = d2 - WaterUsed; }
                        else { UnitPriceWater = pi1.FirstPrice; WaterLevel = d1 - WaterUsed; }
                    }
                    else
                    {
                        UnitPriceWater = pi1.FirstPrice;
                    }
                }

                if (pi2.LadderType == PriceInfo.LaddersType.百分比)
                {
                    if (pi2.LaddersCount > 1)
                    {
                        decimal d1 = pi2.FirstVolume / 100 * ElectricQuota;
                        decimal d2 = pi2.SecondVolume / 100 * ElectricQuota;
                        decimal d3 = pi2.ThirdVolume / 100 * ElectricQuota;
                        decimal d4 = pi2.FourthVolume / 100 * ElectricQuota;

                        if (ElectricUsed >= d3 && d3 > 0) { UnitPriceElectric = pi2.FourthPrice; ElectricLevel = -1; }
                        else if (ElectricUsed >= d2 && d2 > 0) { UnitPriceElectric = pi2.ThirdPrice; ElectricLevel = d3 - ElectricUsed; }
                        else if (ElectricUsed >= d1 && d1 > 0) { UnitPriceElectric = pi2.SecondPrice; ElectricLevel = d2 - ElectricUsed; }
                        else { UnitPriceElectric = pi1.FirstPrice; ElectricLevel = d1 - ElectricUsed; }
                    }
                    else
                    {
                        UnitPriceElectric = pi2.FirstPrice;
                    }
                }
                else
                {
                    if (pi2.LaddersCount > 1)
                    {
                        decimal d1 = pi2.FirstVolume;
                        decimal d2 = pi2.SecondVolume;
                        decimal d3 = pi2.ThirdVolume;
                        decimal d4 = pi2.FourthVolume;

                        if (ElectricUsed >= d3 && d3 > 0) { UnitPriceElectric = pi2.FourthPrice; ElectricLevel = -1; }
                        else if (ElectricUsed >= d2 && d2 > 0) { UnitPriceElectric = pi2.ThirdPrice; ElectricLevel = d3 - ElectricUsed; }
                        else if (ElectricUsed >= d1 && d1 > 0) { UnitPriceElectric = pi2.SecondPrice; ElectricLevel = d2 - ElectricUsed; }
                        else { UnitPriceElectric = pi1.FirstPrice; ElectricLevel = d1 - ElectricUsed; }
                    }
                    else
                    {
                        UnitPriceElectric = pi2.FirstPrice;
                    }
                }
                result["Result"] = true;
                result["Message"] = WaterUsed + "|" + ElectricUsed + "|" + WaterLevel + "|" + ElectricLevel + "|" + UnitPriceWater + "|" + UnitPriceElectric;
            }
            catch
            {
                result["Message"] = "查询年充值信息出错";
            }
            return result;
        }
        #endregion

        #region 查询用户充值记录列表
        public JavaScriptObject GetCardUserRechargeLogList(HttpRequest request)
        {
            string opUserId = request["opUserId"] ?? "0";
            string opUserName = request["opUserName"] ?? "";
            string opPassword = request["opPassword"] ?? "";
            string NetCardMAC = request["NetCardMAC"] ?? "";

            string UserNo = request["UserNo"] ?? "";
            string UserName = request["UserName"] ?? "";
            string IdentityNumber = request["IdentityNumber"] ?? "";
            string Telephone = request["Telephone"] ?? "";

            JavaScriptObject result = new JavaScriptObject();
            result.Add("Result", false);
            result.Add("Message", "");

            if (UserNo.Trim() == "" && UserName.Trim() == "" && IdentityNumber.Trim() == "" && Telephone.Trim() == "")
            {
                result["Message"] = "至少要有一个查询条件不为空";
            }
            else
            {
                try
                {
                    List<JsonCardUserRechargeLog> list = new List<JsonCardUserRechargeLog>();

                    string strSql = "select cu.*,wu.UserName,wu.Password,wu.DistrictId,wu.IdentityNumber,wu.Telephone from CardUserRechargeLog cu,WaterUser wu where cu.WateUserId=cu.Id";
                    List<SqlParameter> parameters = new List<SqlParameter>();
                    if (UserNo.Trim() != "")
                    {
                        strSql += " and cu.UserNo = @UserNo";
                        SqlParameter par = new SqlParameter("@UserNo", SqlDbType.VarChar, 50);
                        par.Value = UserNo.Trim();
                        parameters.Add(par);
                    }
                    if (UserName.Trim() != "")
                    {
                        strSql += " and wu.UserName = @UserName";
                        SqlParameter par = new SqlParameter("@UserName", SqlDbType.VarChar, 50);
                        par.Value = UserName.Trim();
                        parameters.Add(par);
                    }
                    if (IdentityNumber.Trim() != "")
                    {
                        strSql += " and wu.IdentityNumber = @IdentityNumber";
                        SqlParameter par = new SqlParameter("@IdentityNumber", SqlDbType.VarChar, 50);
                        par.Value = IdentityNumber.Trim();
                        parameters.Add(par);
                    }
                    if (Telephone.Trim() != "")
                    {
                        strSql += " and wu.Telephone = @Telephone";
                        SqlParameter par = new SqlParameter("@Telephone", SqlDbType.VarChar, 50);
                        par.Value = Telephone.Trim();
                        parameters.Add(par);
                    }

                    try
                    {
                        DataTable table = DbHelperSQL.Query(strSql, parameters.ToArray()).Tables[0];
                        if (table.Rows.Count != 0)
                        {
                            ModelHandler<JsonCardUserRechargeLog> modelHandler = new ModelHandler<JsonCardUserRechargeLog>();
                            for (int i = 0; i < table.Rows.Count; i++)
                            {
                                DataRow dataRow = table.Rows[i];
                                JsonCardUserRechargeLog jcu = modelHandler.FillModel(dataRow);
                                list.Add(jcu);
                            }

                        }
                    }
                    catch (Exception ex) { myLogger.Error(ex.Message); }

                    result["Result"] = true;
                    result["Message"] = JavaScriptConvert.SerializeObject(list);
                }
                catch (Exception ex)
                {
                    result["Message"] = ex.Message;
                    myLogger.Error(ex.Message);
                }
            }

            return result;
        }
        #endregion

        #region 查询用户用水记录列表
        public JavaScriptObject GetCardUserWaterLogList(HttpRequest request)
        {
            string opUserId = request["opUserId"] ?? "0";
            string opUserName = request["opUserName"] ?? "";
            string opPassword = request["opPassword"] ?? "";
            string NetCardMAC = request["NetCardMAC"] ?? "";

            string UserNo = request["UserNo"] ?? "";
            string UserName = request["UserName"] ?? "";
            string IdentityNumber = request["IdentityNumber"] ?? "";
            string Telephone = request["Telephone"] ?? "";

            JavaScriptObject result = new JavaScriptObject();
            result.Add("Result", false);
            result.Add("Message", "");

            if (UserNo.Trim() == "" && UserName.Trim() == "" && IdentityNumber.Trim() == "" && Telephone.Trim() == "")
            {
                result["Message"] = "至少要有一个查询条件不为空";
            }
            else
            {
                try
                {
                    List<JsonCardUserWaterLog> list = new List<JsonCardUserWaterLog>();

                    string strSql = "select cu.*,wu.UserName,wu.Password,wu.DistrictId,wu.IdentityNumber,wu.Telephone from CardUserWaterLog cu,WaterUser wu where cu.WateUserId=cu.Id";
                    List<SqlParameter> parameters = new List<SqlParameter>();
                    if (UserNo.Trim() != "")
                    {
                        strSql += " and cu.UserNo = @UserNo";
                        SqlParameter par = new SqlParameter("@UserNo", SqlDbType.VarChar, 50);
                        par.Value = UserNo.Trim();
                        parameters.Add(par);
                    }
                    if (UserName.Trim() != "")
                    {
                        strSql += " and wu.UserName = @UserName";
                        SqlParameter par = new SqlParameter("@UserName", SqlDbType.VarChar, 50);
                        par.Value = UserName.Trim();
                        parameters.Add(par);
                    }
                    if (IdentityNumber.Trim() != "")
                    {
                        strSql += " and wu.IdentityNumber = @IdentityNumber";
                        SqlParameter par = new SqlParameter("@IdentityNumber", SqlDbType.VarChar, 50);
                        par.Value = IdentityNumber.Trim();
                        parameters.Add(par);
                    }
                    if (Telephone.Trim() != "")
                    {
                        strSql += " and wu.Telephone = @Telephone";
                        SqlParameter par = new SqlParameter("@Telephone", SqlDbType.VarChar, 50);
                        par.Value = Telephone.Trim();
                        parameters.Add(par);
                    }

                    try
                    {
                        DataTable table = DbHelperSQL.Query(strSql, parameters.ToArray()).Tables[0];
                        if (table.Rows.Count != 0)
                        {
                            ModelHandler<JsonCardUserWaterLog> modelHandler = new ModelHandler<JsonCardUserWaterLog>();
                            for (int i = 0; i < table.Rows.Count; i++)
                            {
                                DataRow dataRow = table.Rows[i];
                                JsonCardUserWaterLog jcu = modelHandler.FillModel(dataRow);
                                list.Add(jcu);
                            }

                        }
                    }
                    catch (Exception ex) { myLogger.Error(ex.Message); }

                    result["Result"] = true;
                    result["Message"] = JavaScriptConvert.SerializeObject(list);
                }
                catch (Exception ex)
                {
                    result["Message"] = ex.Message;
                    myLogger.Error(ex.Message);
                }
            }

            return result;
        }
        #endregion

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}