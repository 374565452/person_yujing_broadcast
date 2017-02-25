using Common;
using DBUtility;
using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Threading;
using System.Web;

namespace WaterMonitorSystem.Src
{
    public class CommonUtil
    {
        /// <summary>
        /// 读取一个配置
        /// </summary>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public static string ReadAppSetting(string keyName)
        {
            return ConfigurationManager.AppSettings[keyName];
        }

        public static ResMsg CheckLoginState(string loginIdentifer, bool isWaitMainLibInit)
        {
            if (loginIdentifer == null)
            {
                return new ResMsg(false, "未登录");
            }
            LoginUser loginUser = GlobalAppModule.GetLoginUser(loginIdentifer);
            if (loginUser == null)
            {
                return new ResMsg(false, "未登录");
            }
            if (loginUser.LoginTimeout)
            {
                return new ResMsg(false, "登录超时");
            }
            loginUser.LastOperateTime = DateTime.Now;
            if (isWaitMainLibInit)
            {
                WaitMainLibInit();
            }
            return new ResMsg(true, loginUser.UserId.ToString());
        }

        public static void WaitMainLibInit()
        {
            int num = 0;
            while (GlobalAppModule.IsInitMainLib)
            {
                if (num == 4)
                {
                    return;
                }
                num++;
                Thread.Sleep(500);
            }
        }

        public static void RemoveFiles(string strPath, string strFilter)
        {

            DirectoryInfo info = new DirectoryInfo(strPath);
            if (info.Exists)
            {
                foreach (FileInfo info2 in info.GetFiles())
                {
                    try
                    {
                        if (info2.Extension.ToString() == strFilter)
                        {
                            TimeSpan span = new TimeSpan(0, 0, 20, 0, 0);
                            if (info2.CreationTime < DateTime.Now.Subtract(span))
                            {
                                info2.Delete();
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }

        }

        public static void RemoveFiles(string strPath, string strFilter, string userName)
        {
            DirectoryInfo info = new DirectoryInfo(strPath);
            if (info.Exists)
            {
                foreach (FileInfo info2 in info.GetFiles())
                {
                    try
                    {
                        if ((info2.Extension.ToString() == strFilter) && info2.Name.Contains(userName))
                        {
                            TimeSpan span = new TimeSpan(0, 0, 20, 0, 0);
                            if (info2.CreationTime < DateTime.Now.Subtract(span))
                            {
                                info2.Delete();
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }

        public static string GetTimeFromSecond(string second)
        {
            string str = "";
            string str2 = "";
            string str3 = "";
            TimeSpan span = TimeSpan.FromSeconds(Convert.ToDouble(second));
            str = ((int)span.TotalHours).ToString();
            str2 = span.Minutes.ToString();
            str3 = span.Seconds.ToString();
            string str4 = str3 + "秒";
            if (str != "0")
            {
                return (str + "时" + str2 + "分" + str3 + "秒");
            }
            if (str2 != "0")
            {
                str4 = str2 + "分" + str3 + "秒";
            }
            return str4;
        }

        public static string GetCookieName(HttpRequest request, string name)
        {
            if (request == null)
            {
                return "";
            }
            string authority = request.Url.Authority;
            string applicationPath = request.ApplicationPath;
            return (authority + ((applicationPath == "/") ? "" : ChinesePY.GetPinYin(applicationPath.Replace("/", "_")).Replace(" ", "")) + "_" + name);
        }

        public static bool CheckTableExiste(string strTableName)
        {
            DataTable table = DbHelperSQL.QueryDataTable("select * from sysobjects where name='" + strTableName + "'");
            return ((table != null) && (table.Rows.Count != 0));
        }
    }
}