using Common;
using Module;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Xml;

namespace WaterMonitorSystem.Src
{
    public static class GlobalAppModule
    {
        private static int commandTimeOut = 12;
        private static List<DeviceAppGroup> deviceAppGroups = null;
        private static bool isInitMainLib = false;
        private static bool isUseMainLib = false;
        private static int loginTimeOut = 0x708;
        private static Dictionary<string, LoginUser> loginUsers = new Dictionary<string, LoginUser>();
        private static int operateTimeOut = 0x708;
        private static Dictionary<string, PageInfo> pageInfos = new Dictionary<string, PageInfo>();
        private static int showLevelCount = 2;

        static GlobalAppModule()
        {
            deviceAppGroups = new List<DeviceAppGroup>();
            XmlDocument document = new XmlDocument();
            FileInfo info = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + @"App_Config\GlobalAppConfig.config");
            if (info.Exists)
            {
                try
                {
                    document.Load(info.FullName);
                    XmlNodeList list = document.SelectNodes("/GlobalAppConfig/应用分类列表/分类配置");
                    if ((list != null) && (list.Count != 0))
                    {
                        foreach (XmlNode node in list)
                        {
                            if ((node["名称"] != null) && (node["名称"].InnerText.Trim() != ""))
                            {
                                DeviceAppGroup item = new DeviceAppGroup
                                {
                                    GroupName = node["名称"].InnerText.Trim()
                                };
                                if ((node["实时监测入口URL"] != null) && (node["实时监测入口URL"].InnerText.Trim() != ""))
                                {
                                    item.MonitorUrl = node["实时监测入口URL"].InnerText.Trim();
                                }
                                if ((node["电子地图入口URL"] != null) && (node["电子地图入口URL"].InnerText.Trim() != ""))
                                {
                                    item.MapURL = node["电子地图入口URL"].InnerText.Trim();
                                }
                                if (node["用户站参数"] != null)
                                {
                                    string[] collection = node["用户站参数"].InnerText.Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                    item.UserStationParms.AddRange(collection);
                                }
                                if (node["子类列表"] != null)
                                {
                                    foreach (XmlNode node2 in node["子类列表"].ChildNodes)
                                    {
                                        if ((node2["名称"] != null) && (node2["名称"].InnerText.Trim() != ""))
                                        {
                                            DeviceAppGroup group2 = new DeviceAppGroup
                                            {
                                                GroupName = node2["名称"].InnerText.Trim()
                                            };
                                            if (node2["用户站参数"] != null)
                                            {
                                                string[] strArray2 = node2["用户站参数"].InnerText.Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                                group2.UserStationParms.AddRange(strArray2);
                                                foreach (string str2 in strArray2)
                                                {
                                                    if (!item.UserStationParms.Contains(str2))
                                                    {
                                                        item.UserStationParms.Add(str2);
                                                    }
                                                }
                                            }
                                            item.ChildDeviceGroup.Add(group2);
                                        }
                                    }
                                }
                                deviceAppGroups.Add(item);
                            }
                        }
                    }
                    XmlNode node3 = document.SelectSingleNode("/GlobalAppConfig/级别显示个数");
                    if (node3 != null)
                    {
                        try
                        {
                            showLevelCount = Convert.ToInt32(node3.InnerText.Trim());
                            if (showLevelCount < 2)
                            {
                                showLevelCount = 2;
                            }
                        }
                        catch
                        {
                        }
                    }
                    XmlNode node4 = document.SelectSingleNode("/GlobalAppConfig/命令超时");
                    if (node4 != null)
                    {
                        try
                        {
                            commandTimeOut = Convert.ToInt32(node4.InnerText.Trim());
                        }
                        catch
                        {
                        }
                    }
                    node4 = document.SelectSingleNode("/GlobalAppConfig/登录超时");
                    if (node4 != null)
                    {
                        try
                        {
                            loginTimeOut = Convert.ToInt32(node4.InnerText.Trim());
                        }
                        catch
                        {
                        }
                    }
                    node4 = document.SelectSingleNode("/GlobalAppConfig/分页操作超时");
                    if (node4 != null)
                    {
                        try
                        {
                            operateTimeOut = Convert.ToInt32(node4.InnerText.Trim());
                        }
                        catch
                        {
                        }
                    }
                }
                catch
                {
                }
            }
            new Thread(new ThreadStart(GlobalAppModule.GlobalAppThread)) { IsBackground = true }.Start();
        }

        public static void AddLoginUser(LoginUser loginUser)
        {
            lock (loginUsers)
            {
                if (loginUsers.ContainsKey(loginUser.LoginIdentifer))
                {
                    loginUsers[loginUser.LoginIdentifer].LastOperateTime = DateTime.Now;
                }
                else
                {
                    loginUsers.Add(loginUser.LoginIdentifer, loginUser);
                }
            }
        }

        public static void AddPageInfo(PageInfo pageInfo)
        {
            lock (pageInfos)
            {
                if (pageInfos.ContainsKey(pageInfo.OperateIdentifer))
                {
                    pageInfos[pageInfo.OperateIdentifer].LastOperateTime = DateTime.Now;
                }
                else
                {
                    pageInfos.Add(pageInfo.OperateIdentifer, pageInfo);
                }
            }
        }

        public static List<DeviceAppGroup> GetDeviceAppGroups()
        {
            return deviceAppGroups;
        }

        public static LoginUser GetLoginUser(string loginIdentifer)
        {
            lock (loginUsers)
            {
                if (loginUsers.ContainsKey(loginIdentifer))
                {
                    return loginUsers[loginIdentifer];
                }
                return null;
            }
        }

        public static PageInfo GetPageInfo(string operateIdentifer)
        {
            lock (pageInfos)
            {
                if (pageInfos.ContainsKey(operateIdentifer))
                {
                    return pageInfos[operateIdentifer];
                }
                return null;
            }
        }

        private static void GlobalAppThread()
        {
            while (true)
            {
                try
                {
                    lock (loginUsers)
                    {
                        List<string> list = new List<string>();
                        foreach (KeyValuePair<string, LoginUser> pair in loginUsers)
                        {
                            if (pair.Value.LoginTimeout)
                            {
                                list.Add(pair.Key);
                            }
                        }
                        foreach (string str in list)
                        {
                            loginUsers.Remove(str);
                        }
                    }
                    lock (pageInfos)
                    {
                        List<string> list2 = new List<string>();
                        foreach (KeyValuePair<string, PageInfo> pair2 in pageInfos)
                        {
                            if (pair2.Value.OperateTimeout)
                            {
                                list2.Add(pair2.Key);
                            }
                        }
                        foreach (string str2 in list2)
                        {
                            list2.Remove(str2);
                        }
                    }
                }
                catch
                {
                }
                Thread.Sleep(0xea60);
            }
        }

        public static bool IsLoginByIdentifer(string identifer)
        {
            lock (loginUsers)
            {
                if (loginUsers.ContainsKey(identifer))
                {
                    if (loginUsers[identifer].LoginTimeout)
                    {
                        return false;
                    }
                    return true;
                }
                return false;
            }
        }

        public static void RemoveLoginUser(string identifer)
        {
            lock (loginUsers)
            {
                if (loginUsers.ContainsKey(identifer))
                {
                    loginUsers.Remove(identifer);
                }
            }
        }

        public static void ReStartCTS(long userId, string loginIP)
        {
            /*
            try
            {
                RemotoControl.RestartServer(userId);
                FileStream stream = new FileStream(HttpContext.Current.Server.MapPath("~") + @"\ctsrestart.log", FileMode.OpenOrCreate, FileAccess.Write);
                StreamWriter writer = new StreamWriter(stream);
                writer.Flush();
                writer.BaseStream.Seek(0L, SeekOrigin.End);
                writer.Write(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " | 通讯服务器重启 | 登录用户:" + SysUserModule.GetUserName(long.Parse(userId)) + " | 登录IP:" + loginIP + Environment.NewLine);
                writer.Flush();
                writer.Close();
            }
            catch
            {
            }
             * */
        }

        public static ResMsg ReStartWebApp(long userId, string loginIP)
        {
            try
            {
                FileStream stream = new FileStream(HttpContext.Current.Server.MapPath("~") + @"\bin\webrestart.log", FileMode.OpenOrCreate, FileAccess.Write);
                StreamWriter writer = new StreamWriter(stream);
                writer.Flush();
                writer.BaseStream.Seek(0L, SeekOrigin.End);
                writer.Write(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " | 网站重启 | 登录用户:" + SysUserModule.GetUserName(userId) + " | 登录IP:" + loginIP + Environment.NewLine);
                writer.Flush();
                writer.Close();
                return new ResMsg(true, "");
            }
            catch (Exception exception)
            {
                return new ResMsg(false, exception.Message);
            }
        }

        public static int CommandTimeOut
        {
            get
            {
                return commandTimeOut;
            }
            set
            {
                commandTimeOut = value;
            }
        }

        public static bool IsInitMainLib
        {
            get
            {
                return isInitMainLib;
            }
            set
            {
                isInitMainLib = value;
            }
        }

        public static bool IsUseMainLib
        {
            get
            {
                return isUseMainLib;
            }
            set
            {
                isUseMainLib = value;
            }
        }

        public static int LoginTimeOut
        {
            get
            {
                return loginTimeOut;
            }
            set
            {
                loginTimeOut = value;
            }
        }

        public static Dictionary<string, LoginUser> LoginUsers
        {
            get
            {
                return loginUsers;
            }
        }

        public static int OperateTimeOut
        {
            get
            {
                return operateTimeOut;
            }
            set
            {
                operateTimeOut = value;
            }
        }

        public static Dictionary<string, PageInfo> PageInfos
        {
            get
            {
                return pageInfos;
            }
        }

        public static int ShowLevelCount
        {
            get
            {
                return showLevelCount;
            }
        }
    }
}