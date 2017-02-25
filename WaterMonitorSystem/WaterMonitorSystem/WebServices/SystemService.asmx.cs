using Common;
using Maticsoft.Model;
using Module;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Web;
using System.Web.Services;
using WaterMonitorSystem.Src;

namespace WaterMonitorSystem.WebServices
{
    /// <summary>
    /// SystemService 的摘要说明
    /// </summary>
    [Serializable, WebService(Description = "获取系统状态信息，重启网站，重启通讯服务器", Name = "系统服务", Namespace = "http://www.data86.net/"), WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class SystemService : System.Web.Services.WebService
    {
        private HttpContext context = HttpContext.Current;

        public SystemService()
        {
            this.context.Response.Buffer = true;
            this.context.Response.ExpiresAbsolute = DateTime.Now.AddSeconds(-1.0);
            this.context.Response.Expires = 0;
            this.context.Response.CacheControl = "no-cache";

            
        }

        public static bool isConnect = false;

        [WebMethod(EnableSession = true, Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>获取系统信息</span><br/><p style='text-indent:15px'>返回数据格式：{'Result':false,'Message':'',SysStateInfo:{}}</p>")]
        public string GetSystemStateInfo(string loginIdentifer)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            JavaScriptObject obj3 = new JavaScriptObject();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("SysStateInfo", obj3);
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
            CommonUtil.WaitMainLibInit();
            SysUser user = SysUserModule.GetUser(loginUser.UserId);
            if (user == null)
            {
                obj2["Message"] = "ID为" + loginUser.UserId + "的用户不存在";
            }
            else
            {
                obj2["Result"] = true;
                obj3.Add("通讯服务器连接状态", isConnect ? "正常" : "中断");
                obj3.Add("当前登录操作员名称", user.UserName);
                obj3.Add("当前登录操作员管理ID", user.DistrictId);
                obj3.Add("当前登录操作员角色ID", user.RoleId);
                obj3.Add("远程客户端的IP主机地址", this.context.Request.UserHostAddress);
                obj3.Add("监测点级别名称", "设备");
                obj3.Add("监测点管理名称", "设备");
                obj3.Add("系统名称", "水资源管理平台");
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(EnableSession = true, Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>获取系统信息</span><br/><p style='text-indent:15px'>返回数据格式：{'Result':false,'Message':'',SysStateInfo:{}}</p>")]
        public string GetDeviceTypeCode(string loginIdentifer)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            obj2["Result"] = false;
            obj2["Message"] = "";
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
            CommonUtil.WaitMainLibInit();
            List<DeviceTypeCode> deviceTypeCodeList = DeviceTypeCodeModule.GetAll();
            JavaScriptArray array = new JavaScriptArray();
            foreach (DeviceTypeCode info in deviceTypeCodeList)
            {
                JavaScriptObject item = this.DeviceTypeCodeInfoToJson(info);
                array.Add(item);
            }
            obj2["Result"] = true;
            obj2["DeviceTypeCode"] = array;
            return JavaScriptConvert.SerializeObject(obj2);
        }

        private JavaScriptObject DeviceTypeCodeInfoToJson(DeviceTypeCode uni)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            obj2["ID"] = uni.Id;
            obj2["K"] = uni.k;
            //obj2["定额方式ID"] = uni.ModeId;
            obj2["V"] = uni.v;
            return obj2;
        }

        [WebMethod(EnableSession = true, Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>刷新缓存</span><br/><p style='text-indent:15px'>返回数据格式：{'Result':false,'Message':''}</p>")]
        public string RefreshCache(string loginIdentifer, string CacheName)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            obj2["Result"] = false;
            obj2["Message"] = "";
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
            CommonUtil.WaitMainLibInit();

            GlobalAppModule.IsInitMainLib = true;
            try
            {
                if (CacheName == "基础信息")
                {
                    BaseModule.LoadBaseInfo();
                    MenuModule.MenuInit();
                    RoleModule.UpdateRoleInfo();
                    SysUserModule.UpdateUserInfo();
                    DeviceTypeCodeModule.LoadDeviceTypeCodeInfos();
                    PriceModule.LoadPriceTypes();
                    PriceModule.LoadPriceInfos();
                    CropModule.LoadUnitQuotaInfos();
                    obj2["Result"] = true;
                }
                else if (CacheName == "用水户")
                {
                    WaterUserModule.LoadWaterUsers();
                    obj2["Result"] = true;
                }
                else if (CacheName == "用户卡")
                {
                    CardUserModule.LoadCardUsers();
                    obj2["Result"] = true;
                }
                else if (CacheName == "设备")
                {
                    DeviceModule.LoadDevices();
                    obj2["Result"] = true;
                }
                else if (CacheName == "区域")
                {
                    DistrictModule.UpdateLevelInfo();
                    DistrictModule.UpdateDistrictInfo();
                    obj2["Result"] = true;
                }
                else
                {
                    obj2["Message"] = "参数错误！【" + CacheName + "】";
                }
            }
            catch (Exception ex)
            {
                obj2["Message"] = "刷新缓存【" + CacheName + "】出错！" + ex.Message;
            }

            GlobalAppModule.IsInitMainLib = false;
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(EnableSession = true, Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>获取上传文件列表</span><br/><p style='text-indent:15px'>返回数据格式：{'Result':false,'Message':''}</p>")]
        public string UploadFile(string loginIdentifer, string deviceFullNo)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            obj2["Result"] = false;
            obj2["Message"] = "";
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
            CommonUtil.WaitMainLibInit();

            JavaScriptArray array = new JavaScriptArray();
            obj2.Add("DevDatas", array);

            string rootpath = "UploadFiles/Device/";
            string path = Server.MapPath("~/" + rootpath);
            DirectoryInfo TheFolder = new DirectoryInfo(path);
            if (!TheFolder.Exists)
            {
                TheFolder.Create();
            }
            DirectoryInfo[] dis = TheFolder.GetDirectories("*" + deviceFullNo + "*");
            if (dis != null && dis.Length > 0)
            {
                foreach (DirectoryInfo di in dis)
                {
                    foreach (FileInfo fi in di.GetFiles())
                    {
                        try
                        {
                            Device device = DeviceModule.GetDeviceByFullDeviceNo(di.Name);
                            District district = null;
                            if (device != null)
                                district = DistrictModule.ReturnDistrictInfo(device.DistrictId);
                            JavaScriptObject obj3 = new JavaScriptObject();
                            obj3["UploadTime"] = DateTime.ParseExact(fi.Name.Split('.')[0], "yyyyMMddHHmmss", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd HH:mm:ss");
                            obj3["village"] = district != null ? district.DistrictName : "-未知-";
                            obj3["deviceName"] = device != null ? device.DeviceName : "-未知-";
                            obj3["deviceType"] = device != null ? device.DeviceType : "-未知-";
                            obj3["deviceFullNo"] = di.Name;
                            obj3["FileName"] = fi.Name;
                            array.Add(obj3);
                        }
                        catch { }
                    }
                }
            }
            obj2["Result"] = true;
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(EnableSession = true, Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>获取下发文件列表</span><br/><p style='text-indent:15px'>返回数据格式：{'Result':false,'Message':''}</p>")]
        public string SendFile(string loginIdentifer, string UserName, string deviceFullNo)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            obj2["Result"] = false;
            obj2["Message"] = "";
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
            CommonUtil.WaitMainLibInit();

            JavaScriptArray array = new JavaScriptArray();
            obj2.Add("DevDatas", array);

            string rootpath = "UploadFiles/User/";
            string path = Server.MapPath("~/" + rootpath);
            DirectoryInfo TheFolder = new DirectoryInfo(path);
            if (!TheFolder.Exists)
            {
                TheFolder.Create();
            }
            DirectoryInfo[] dis = TheFolder.GetDirectories("*" + UserName + "*");
            foreach (DirectoryInfo di in dis)
            {
                foreach (FileInfo fi in di.GetFiles())
                {
                    try
                    {
                        string[] filenames = fi.Name.Split('_');
                        if (filenames.Length > 2)
                        {
                            if (filenames[1].Contains(deviceFullNo) || deviceFullNo == "")
                            {
                                Device device = DeviceModule.GetDeviceByFullDeviceNo(filenames[1]);
                                District district = null;
                                if (device != null)
                                    district = DistrictModule.ReturnDistrictInfo(device.DistrictId);

                                JavaScriptObject obj3 = new JavaScriptObject();
                                obj3["UploadTime"] = DateTime.ParseExact(filenames[0], "yyyyMMddHHmmss", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd HH:mm:ss");
                                obj3["UserName"] = di.Name;
                                obj3["village"] = district != null ? district.DistrictName : "-未知-";
                                obj3["deviceName"] = device != null ? device.DeviceName : "-未知-";
                                obj3["deviceType"] = device != null ? device.DeviceType : "-未知-";
                                obj3["deviceFullNo"] = filenames[1];
                                obj3["FileName"] = fi.Name.Substring(filenames[0].Length + filenames[1].Length + 2);
                                array.Add(obj3);
                            }
                        }
                    }
                    catch { }
                }
            }
            obj2["Result"] = true;
            return JavaScriptConvert.SerializeObject(obj2);
        }
    }
}
