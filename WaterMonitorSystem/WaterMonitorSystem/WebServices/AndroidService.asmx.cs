using Common;
using Maticsoft.Model;
using Module;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Services;
using WaterMonitorSystem.Src;

namespace WaterMonitorSystem.WebServices
{
    /// <summary>
    /// AndroidService 的摘要说明
    /// </summary>
    [WebService(Description = "安卓用户服务", Name = "安卓网络服务", Namespace = "http://www.data86.net/")]
    [Serializable, WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class AndroidService : System.Web.Services.WebService
    {
        log4net.ILog myLogger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private HttpContext context = HttpContext.Current;

        public AndroidService()
        {
            this.context.Response.Buffer = true;
            this.context.Response.ExpiresAbsolute = DateTime.Now.AddSeconds(-1.0);
            this.context.Response.Expires = 0;
            this.context.Response.CacheControl = "no-cache";
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>测试连接</span><br/><p>返回数据：'连接成功'</p>")]
        public string TestConnection()
        {
            return "连接成功";
        }

        [WebMethod(EnableSession = true, Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>用户登录方法</span><br/><p style='text-indent:15px'>输入参数：LoginName=用户名；LoginPwd=密码；返回数据：{'Results':bool,'Message':string}</p>")]
        public string Login(string LoginName, string LoginPwd, string LoginAddress)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("Guid", "");

            if (SysInfo.IsReg)
            {
                string path = context.Server.MapPath("~/");

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
                    obj2["Message"] = "系统未注册";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                else
                {
                    SysInfo.IsRegSuccess = true;
                }
            }

            long userID = 0;
            try
            {
                string str3 = SysUserModule.Login(LoginName, LoginPwd, ToolsWeb.GetIP(context.Request), LoginAddress, ref userID);
                if (str3 == "登陆成功")
                {
                    SysUserModule.OperatorLogin(userID, ToolsWeb.GetIP(context.Request), "");
                    LoginUser loginUser = new LoginUser
                    {
                        UserId = userID,
                        LoginName = LoginName,
                        LoginIdentifer = Guid.NewGuid().ToString()
                    };
                    GlobalAppModule.AddLoginUser(loginUser);

                    obj2["Result"] = true;
                    obj2["Guid"] = loginUser.LoginIdentifer;
                }
                obj2["Message"] = str3;
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
            }

            try
            {
                SysLog log = new SysLog();
                log.LogUserId = userID;
                log.LogUserName = LoginName;
                log.LogAddress = ToolsWeb.GetIP(context.Request) + " | " + LoginAddress;
                log.LogTime = DateTime.Now;
                log.LogType = "android登录";
                log.LogContent = obj2["Message"].ToString();
                SysLogModule.Add(log);
            }
            catch { }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(EnableSession = true, Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>用户重新登录方法</span><br/><p style='text-indent:15px'>输入参数：LoginName=用户名；LoginPwd=密码；返回数据：{'Results':bool,'Message':string}</p>")]
        public string ReLogin(string LoginName, string LoginPwd, string LoginAddress, string loginIdentifer)
        {
            myLogger.Info("loginIdentifer！" + loginIdentifer);

            JavaScriptObject obj2 = new JavaScriptObject();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("Guid", "");

            LoginUser loginUser = GlobalAppModule.GetLoginUser(loginIdentifer);
            if (loginUser != null && !loginUser.LoginTimeout)
            {
                myLogger.Info("无需重新登录！");

                loginUser.LastOperateTime = DateTime.Now;
                obj2["Result"] = true;
                obj2["Guid"] = loginIdentifer;
                return JavaScriptConvert.SerializeObject(obj2);
            }
            else
            {
                myLogger.Info("需重新登录！" + loginIdentifer);
            }

            GlobalAppModule.RemoveLoginUser(loginIdentifer);

            if (SysInfo.IsReg)
            {
                string path = context.Server.MapPath("~/");

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
                    obj2["Message"] = "系统未注册";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                else
                {
                    SysInfo.IsRegSuccess = true;
                }
            }

            long userID = 0;
            try
            {
                string str3 = SysUserModule.Login(LoginName, LoginPwd, ToolsWeb.GetIP(context.Request), LoginAddress, ref userID);
                if (str3 == "登陆成功")
                {
                    SysUserModule.OperatorLogin(userID, ToolsWeb.GetIP(context.Request), "");
                    LoginUser loginUserNew = new LoginUser
                    {
                        UserId = userID,
                        LoginName = LoginName,
                        LoginIdentifer = Guid.NewGuid().ToString()
                    };
                    GlobalAppModule.AddLoginUser(loginUserNew);

                    obj2["Result"] = true;
                    obj2["Guid"] = loginUserNew.LoginIdentifer;
                    myLogger.Info("重新登录成功！" + loginIdentifer);
                }
                obj2["Message"] = str3;
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
            }

            try
            {
                SysLog log = new SysLog();
                log.LogUserId = userID;
                log.LogUserName = LoginName;
                log.LogAddress = ToolsWeb.GetIP(context.Request) + " | " + LoginAddress;
                log.LogTime = DateTime.Now;
                log.LogType = "android登录";
                log.LogContent = obj2["Message"].ToString();
                SysLogModule.Add(log);
            }
            catch { }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(EnableSession = true, Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>心跳</span><br/><p style='text-indent:15px'>返回数据：{'Results':bool,'Message':string}</p>")]
        public string Heartbeat(string loginIdentifer, string LoginAddress)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            obj2.Add("Result", false);
            obj2.Add("Message", "");

            JavaScriptArray objList = new JavaScriptArray();
            obj2.Add("List", objList);

            LoginUser loginUser = GlobalAppModule.GetLoginUser(loginIdentifer);
            if (loginUser == null)
            {
                obj2["Message"] = "未登录";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            if (loginUser.LoginTimeout)
            {
                obj2["Message"] = "登录超时";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            loginUser.LastOperateTime = DateTime.Now;

            obj2["Result"] = true;
            obj2["Message"] = "";
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(EnableSession = true, Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>获取可下载文件列表</span><br/><p style='text-indent:15px'>返回数据：{'Results':bool,'Message':string,'List':object}</p>")]
        public string GetFileList(string loginIdentifer, string LoginAddress)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            obj2.Add("Result", false);
            obj2.Add("Message", "");

            JavaScriptArray objList = new JavaScriptArray();
            obj2.Add("List", objList);

            LoginUser loginUser = GlobalAppModule.GetLoginUser(loginIdentifer);
            if (loginUser == null)
            {
                obj2["Message"] = "未登录";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            if (loginUser.LoginTimeout)
            {
                obj2["Message"] = "登录超时";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            loginUser.LastOperateTime = DateTime.Now;

            try
            {
                string rootpath = "md/DownFiles/";
                string path = Server.MapPath("~/" + rootpath);
                DirectoryInfo TheFolder = new DirectoryInfo(path);
                if (TheFolder.Exists)
                {
                    foreach (FileInfo fi in TheFolder.GetFiles())
                    {
                        JavaScriptObject obj7 = new JavaScriptObject();
                        obj7.Add("fn", fi.Name);
                        obj7.Add("fp", rootpath + fi.Name);
                        obj7.Add("fd", fi.LastWriteTime.ToString("yyyyMMddHHmmssfff"));
                        obj7.Add("fz", fi.Length);
                        //obj7.Add("fm", Encode.GetMD5HashFromFile(fi.FullName));
                        obj7.Add("fm", "");
                        objList.Add(obj7);
                    }

                    obj2["Result"] = true;
                    obj2["Message"] = "";
                }
                else
                {
                    obj2["Result"] = true;
                    obj2["Message"] = "";
                }
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
            }

            try
            {
                SysLog log = new SysLog();
                log.LogUserId = loginUser.UserId;
                log.LogUserName = loginUser.LoginName;
                log.LogAddress = ToolsWeb.GetIP(context.Request) + " | " + LoginAddress;
                log.LogTime = DateTime.Now;
                log.LogType = "获取可下载文件列表";
                log.LogContent = obj2["Message"].ToString();
                SysLogModule.Add(log);
            }
            catch { }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(EnableSession = true, Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>上传文件</span><br/><p style='text-indent:15px'>返回数据：{'Results':bool,'Message':string,'List':object}</p>")]
        public string UploadFile(string loginIdentifer, string LoginAddress, string filename, string sum, string curr)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            obj2.Add("Result", false);
            obj2.Add("Message", "");

            LoginUser loginUser = GlobalAppModule.GetLoginUser(loginIdentifer);
            if (loginUser == null)
            {
                obj2["Message"] = "未登录";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            if (loginUser.LoginTimeout)
            {
                obj2["Message"] = "登录超时";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            loginUser.LastOperateTime = DateTime.Now;

            try
            {
                string fileStr = context.Request["fileStr"];

                //myLogger.Info("sum：" + sum + "，curr：" + curr + "，filename：" + filename + "，length：" + HexStringUtility.HexStringToByteArray(fileStr).Length);
                string appName = loginUser.LoginName + "_fileStrs";

                string[] fileStrs;
                if (int.Parse(curr) == 1)
                {
                    try
                    {
                        SysLog log = new SysLog();
                        log.LogUserId = loginUser.UserId;
                        log.LogUserName = loginUser.LoginName;
                        log.LogAddress = ToolsWeb.GetIP(context.Request) + " | " + LoginAddress;
                        log.LogTime = DateTime.Now;
                        log.LogType = "android上传文件";
                        log.LogContent = "sum：" + sum + "，curr：" + curr + "，filename：" + filename;
                        SysLogModule.Add(log);
                    }
                    catch { }

                    if (Application[appName] != null)
                    {
                        //myLogger.Info("上传文件移除已存在的记录fileStrs");
                        Application.Remove(appName);
                    }
                }

                if (Application[appName] != null)
                {
                    fileStrs = (string[])Application[appName];
                }
                else
                {
                    //myLogger.Info("初始化fileStrs，长度：" + int.Parse(sum));
                    fileStrs = new string[int.Parse(sum)];
                    Application.Add(appName, fileStrs);
                }

                fileStrs[int.Parse(curr) - 1] = fileStr;

                obj2["Result"] = true;
                if (sum == curr)
                {
                    obj2["Message"] = "1";//接收完成
                    SaveFile(fileStrs, loginUser.LoginName, filename);
                    if (Application[appName] != null)
                    {
                        //myLogger.Info("接收完成移除已存在的记录fileStrs");
                        Application.Remove(appName);
                    }
                }
                else
                {
                    obj2["Message"] = "0";//继续上传
                }
            }
            catch (Exception ex)
            {
                myLogger.Error(ex.Message);
                obj2["Message"] = "上传文件出错！";
            }

            return JavaScriptConvert.SerializeObject(obj2);
        }

        private void SaveFile(string[] fileStrs, string LoginName, string filename)
        {
            DateTime dateNew = DateTime.Now;
            //myLogger.Info("SaveFile fileStrs：" + fileStrs.Length + "，LoginName：" + LoginName + "，filename：" + filename);
            string rootpath = "md/UpFiles/" + LoginName + "/" + dateNew.ToString("yyyy-MM-dd") + "/";
            string path = Server.MapPath("~/" + rootpath);
            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch
                {
                }
            }
            StringBuilder fileStrFull = new StringBuilder();
            foreach (string fileStr in fileStrs)
            {
                fileStrFull.Append(fileStr);
            }
            byte[] bsFile = HexStringUtility.HexStringToByteArray(fileStrFull.ToString());

            try
            {
                System.IO.File.WriteAllBytes(path + dateNew.ToString("HHmmssfff") + "_" + filename, bsFile);
                myLogger.Info("上传文件" + filename + "保存成功！");
            }
            catch (Exception ex)
            {
                myLogger.Info("上传文件" + filename + "保存失败！" + Environment.NewLine + ex.Message);
            }
        }

        [WebMethod(EnableSession = true, Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>获取设备列表</span><br/><p style='text-indent:15px'>返回数据：{'Results':bool,'Message':string,'List':object}</p>")]
        public string GetDeviceList(string loginIdentifer, string LoginAddress, string deviceNo)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            obj2.Add("Result", false);
            obj2.Add("Message", "");

            JavaScriptArray objList = new JavaScriptArray();
            obj2.Add("List", objList);

            LoginUser loginUser = GlobalAppModule.GetLoginUser(loginIdentifer);
            if (loginUser == null)
            {
                obj2["Message"] = "未登录";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            if (loginUser.LoginTimeout)
            {
                obj2["Message"] = "登录超时";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            loginUser.LastOperateTime = DateTime.Now;

            SysUser user = SysUserModule.GetUser(loginUser.UserId);
            if (user != null)
            {
                List<long> list_DistrictID = DistrictModule.GetAllDistrictID(user.DistrictId);
                List<Device> list_Device = DeviceModule.GetAllDevice();
                foreach (Device device in list_Device)
                {
                    if (list_DistrictID.Contains(device.DistrictId))
                    {
                        if (deviceNo == "" || DeviceModule.GetFullDeviceNoByID(device.Id).Contains(deviceNo))
                            objList.Add(DeviceNodeToJson(device));
                    }
                }
                obj2["Result"] = true;
                obj2["Message"] = "";
            }
            else
            {
                obj2["Message"] = "未查找到用户";
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        private JavaScriptObject DeviceNodeToJson(Device device)
        {
            JavaScriptObject obj = new JavaScriptObject();
            obj["Id"] = device.Id;
            obj["SimNo"] = device.SimNo;
            obj["Name"] = device.DeviceName;
            obj["SDate"] = device.SetupDate.ToString("yyyy-MM-dd HH:mm");
            obj["Add"] = device.SetupAddress;
            obj["LON"] = device.LON / 1000000.0;
            obj["LAT"] = device.LAT / 1000000.0;

            string districtName = DistrictModule.GetDistrictName(device.DistrictId);
            if (districtName != null)
                obj.Add("Dist", districtName);
            else
                obj.Add("Dist", "未知");
            obj["DevNo"] = device.DeviceNo;
            obj["DevNoF"] = DeviceModule.GetFullDeviceNoByID(device.Id);
            obj["OL"] = device.Online;
            obj["OLT"] = device.OnlineTime.ToString("yyyy-MM-dd HH:mm:ss");
            switch (device.StationType)
            {
                case 0: obj["STT"] = "单站"; break;
                case 1: obj["STT"] = "主站"; break;
                case 2: obj["STT"] = "从站"; break;
                default: obj["STT"] = "单站"; break;
            }
            obj["DevT"] = device.DeviceType;
            obj["RSN"] = device.RemoteStation;

            return obj;
        }

        [WebMethod(EnableSession = true, Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>获取可下载文件列表</span><br/><p style='text-indent:15px'>返回数据：{'Results':bool,'Message':string,'List':object}</p>")]
        public string GetFileListDevice(string loginIdentifer, string LoginAddress, string RemoteStation)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            obj2.Add("Result", false);
            obj2.Add("Message", "");

            JavaScriptArray objList = new JavaScriptArray();
            obj2.Add("List", objList);

            LoginUser loginUser = GlobalAppModule.GetLoginUser(loginIdentifer);
            if (loginUser == null)
            {
                obj2["Message"] = "未登录";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            if (loginUser.LoginTimeout)
            {
                obj2["Message"] = "登录超时";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            loginUser.LastOperateTime = DateTime.Now;

            try
            {
                string rootpath = "UploadImg/" + RemoteStation + "/";
                string path = Server.MapPath("~/" + rootpath);
                DirectoryInfo TheFolder = new DirectoryInfo(path);
                if (TheFolder.Exists)
                {
                    foreach (FileInfo fi in TheFolder.GetFiles())
                    {
                        JavaScriptObject obj7 = new JavaScriptObject();
                        obj7.Add("fn", fi.Name);
                        obj7.Add("fp", rootpath + fi.Name);
                        obj7.Add("fd", fi.LastWriteTime.ToString("yyyyMMddHHmmssfff"));
                        obj7.Add("fz", fi.Length);
                        //obj7.Add("fm", Encode.GetMD5HashFromFile(fi.FullName));
                        obj7.Add("fm", "");
                        objList.Add(obj7);
                    }

                    obj2["Result"] = true;
                    obj2["Message"] = "";
                }
                else
                {
                    obj2["Result"] = true;
                    obj2["Message"] = "";
                }
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
            }

            try
            {
                SysLog log = new SysLog();
                log.LogUserId = loginUser.UserId;
                log.LogUserName = loginUser.LoginName;
                log.LogAddress = ToolsWeb.GetIP(context.Request) + " | " + LoginAddress;
                log.LogTime = DateTime.Now;
                log.LogType = "获取水文设备可下载文件列表";
                log.LogContent = obj2["Message"].ToString();
                SysLogModule.Add(log);
            }
            catch { }
            return JavaScriptConvert.SerializeObject(obj2);
        }
    }
}
