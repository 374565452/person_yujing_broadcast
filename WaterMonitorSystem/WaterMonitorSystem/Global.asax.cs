using DBUtility;
using Module;
using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Threading;
using WaterMonitorSystem.Src;
using WaterMonitorSystem.WebServices;

namespace WaterMonitorSystem
{
    public class Global : System.Web.HttpApplication
    {
        log4net.ILog myLogger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public bool isClose = false;
        protected void Application_Start(object sender, EventArgs e)
        {
            string path = Server.MapPath("~/");
            SysInfo.IsReg = false;
            SysInfo.DRegStr = "jssl160721";
            SysInfo.RegStr = SysInfo.DRegStr;

            if (SysInfo.IsReg)
            {
                myLogger.Info("判断注册！");

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
                }
                else
                {
                    myLogger.Info("注册码正确");
                    SysInfo.IsRegSuccess = true;
                }
            }

            isClose = false;
            myLogger.Info("网站启动");

            DbHelperSQL.SetConnectionString(ConfigurationManager.AppSettings["ConnectionString"]);

            GlobalAppModule.IsInitMainLib = true;
            BaseModule.LoadBaseInfo();
            MenuModule.MenuInit();
            RoleModule.UpdateRoleInfo();
            SysUserModule.UpdateUserInfo();
            //SysUserConfigModule.LoadSysUserConfig();
            DeviceTypeCodeModule.LoadDeviceTypeCodeInfos();
            PriceModule.LoadPriceTypes();
            PriceModule.LoadPriceInfos();
            CropModule.LoadUnitQuotaInfos();
            DistrictModule.UpdateLevelInfo();
            DistrictModule.UpdateDistrictInfo();
            WaterUserModule.LoadWaterUsers();
            DeviceModule.LoadDevices();
            //Device_GroundWaterModule.LoadDevices();
            CardUserModule.LoadCardUsers();

            GlobalAppModule.IsInitMainLib = false;

            Thread thread = new Thread(new ThreadStart(testConnect));
            thread.Start();

            Thread threadDeleteXLS = new Thread(new ThreadStart(DeleteXLS));
            threadDeleteXLS.Start();

            Thread threadRefresh = new Thread(new ThreadStart(Refresh));
            threadRefresh.Start();

            myLogger.Info("网站启动完成！");
        }

        private void Refresh()
        {
            int i = 0;
            while (!isClose)
            {
                i++;
                if (i == 2 * 10)
                {
                    WaterUserModule.LoadWaterUsers();
                    CardUserModule.LoadCardUsers();
                    i = 0;
                }
                DeviceModule.LoadDevices();

                //间隔30秒
                Thread.Sleep(30 * 1000);
            }
        }

        private void DeleteXLS()
        {
            while (!isClose)
            {
                try
                {
                    string path = Server.MapPath("~/");
                    DirectoryInfo TheFolder = new DirectoryInfo(path);
                    foreach (DirectoryInfo NextFolder in TheFolder.GetDirectories())
                    {
                        if (NextFolder.FullName == (path + "DataMonitor") || NextFolder.FullName == (path + "DataQuery")
                            || NextFolder.FullName == (path + "DataReport") || NextFolder.FullName == (path + "GlobalView")
                            || NextFolder.FullName == (path + "SystemAdmin") || NextFolder.FullName == (path + "WaterRight")
                            || NextFolder.FullName == (path + "WebAdmin"))
                        {
                            FileInfo[] files = NextFolder.GetFiles();
                            foreach (FileInfo file in files)
                            {
                                if (file.Extension == ".xls")
                                {
                                    string fn = file.Directory.Name + "/" + file.Name;
                                    try
                                    {
                                        file.Delete();
                                        myLogger.Info("删除文件：" + fn + "，成功！");
                                    }
                                    catch
                                    {
                                        myLogger.Info("删除文件：" + fn + "，失败！");
                                    }
                                }

                            }
                        }
                    }
                }
                catch
                {
                }

                //间隔2小时
                Thread.Sleep(2 * 60 * 60 * 1000);
            }
        }

        private void testConnect()
        {
            while (!isClose)
            {
                TcpCommunication tcpService = new TcpCommunication();

                int timeDelay = 0;
                //待socket准备好
                while (timeDelay < tcpService.TcpWait)
                {
                    if ((tcpService.SOCKET_STATE == TcpCommunication.TCP_SocketState.SOCKET_CONNECTED)
                        || (tcpService.SOCKET_STATE == TcpCommunication.TCP_SocketState.SOCKET_CLOSED))
                    {
                        break;
                    }

                    Thread.Sleep(100);
                    timeDelay = timeDelay + 1;
                }

                if (tcpService.SOCKET_STATE != TcpCommunication.TCP_SocketState.SOCKET_CONNECTED)
                {
                    SystemService.isConnect = false;
                }
                else
                {
                    SystemService.isConnect = true;
                }
                myLogger.Info("网关服务器连接状态：" + (SystemService.isConnect ? "正常" : "中断"));
                tcpService.Close();

                //间隔5分钟
                Thread.Sleep(5 * 60 * 1000);
            }
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            //myLogger.Info("网站出错");
        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {
            isClose = true;
            myLogger.Info("网站关闭");
        }
    }
}