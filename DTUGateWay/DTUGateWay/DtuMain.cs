using DBUtility;
using DTU.GateWay.Protocol;
using Maticsoft.Model;
using Module;
using Server.Data.Bridge;
using Server.Util.Cache;
using Server.Util.Log;
using Server.Util.Service;
using Server.Util.Transfer;
using System;
using System.Collections.Generic;
using System.Net;
using System.Windows.Forms;

namespace DTUGateWay
{
    public partial class DtuMain : Form
    {
        log4net.ILog logHelper = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string xmlConfig = Application.StartupPath + "\\setup.xml";

        //dtu服务端口号
        private int dtuPort = 0;

        //web服务端口号
        private int webPort = 0;

        //超时时间
        private int timeout = 0;

        //所能承受的最大连接数
        private int connectedCount = 0;

        //是否开启信息日志
        private bool showInfoLog = true;
        //错误日志开启
        private bool showErrorLog = true;

        private int timeCount = 0;

        //是否允许进行事件记录功能，供查询、检索使用
        private bool recordEventEnable = true;

        public IList<Device> deviceLists;

        private bool loadConfigFlag = true;
        private XmlHelper xmlHelper;

        private DataBridge bridge;

        private Server.Core.ServerCore.ServerSocket dtuServer;//引用的资源  Server.Core  等等

        private Server.Web.ServerCore.ServerSocket webServer;

        Timer timer_Sentinel = new Timer();
        public DtuMain()
        {
            InitializeComponent();

            this.skinEngine12.SkinFile = Application.StartupPath + "/Skins/SportsBlue.ssk";

            FilePathCache.Path = "";

            ShowLogData.isShow = true;

            if (SysInfo.IsReg)
            {
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
                    this.Hide();
                    frmReg fReg = new frmReg();
                    fReg.ShowDialog();

                    if (frmReg.isClose)
                    {
                        this.Close();
                        Environment.Exit(0);
                        return;
                    }
                }
            }
            //MessageBox.Show(Common.HexStringUtility.StrToHexString("c").Length.ToString());
            setUpTime();
            CheckForIllegalCrossThreadCalls = false;// 新创建的线程可以访问窗体中的控件
            xmlHelper = new XmlHelper(xmlConfig);//xml配置文件路径实例化
            //ShowLogData.add("【" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "】加载配置参数！");
            loadConfigFlag = loadConfig();// 获取xml中的配置参数

            if (loadConfigFlag)//bool型的变量，判断是否获取到了配置参数包括ip  port  超时间等参数
            {
                //ShowLogData.add("【" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "】数据库测试连接！");
                bool flag = false;
                try
                {
                    DateTime dateSql = Convert.ToDateTime(DbHelperSQL.GetSingle("select GETDATE()"));
                    this.SSLDateStart.Text = dateSql.ToString("yyyy-MM-dd HH:mm:ss");
                    flag = true;
                }
                catch (Exception e)
                {
                    string error = string.Format("数据库在初始化时出错，错误信息为{0}", e.Message);
                    //MessageBox.Show(error);
                    logHelper.Error(e.Message);
                    Environment.Exit(0);
                }

                if (flag)
                {
                    //ShowLogData.add("【"+DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "】数据库连接成功！");

                    //ShowLogData.add("【" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "】初始化设备状态和缓存开始！");
                    installDeviceState();
                    //ShowLogData.add("【" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "】初始化设备状态和缓存完成！");


                    fLog = new frmLog();
                    fConfig = new frmConfig();
                    //ShowLogData.add("【" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "】日志和配置页面初始化成功！");
                    dtuServer = new Server.Core.ServerCore.ServerSocket(connectedCount);
                    dtuServer.Timeout = timeout;
                    dtuServer.ShowErrorLog = showErrorLog;
                    dtuServer.ShowInfoLog = showInfoLog;
                    dtuServer.RecordEevent = recordEventEnable;
                    dtuServer.OnServerDataSocketHandler += dtuServer_OnServerDataSocketHandler;
                    dtuServer.OnServerConnectedHandler += dtuServer_OnServerConnectedHandler;
                    IPEndPoint dtuPoint = new IPEndPoint(IPAddress.Any, dtuPort);
                    //DtuServer参数配置（以上）
                    dtuServer.start(dtuPoint);
                    //ShowLogData.add("【" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "】dtuServer网关启动成功！");
                    webServer = new Server.Web.ServerCore.ServerSocket(connectedCount);
                    webServer.ShowErrorLog = showErrorLog;
                    webServer.ShowInfoLog = showInfoLog;
                    webServer.OnServerDataSocketHandler += webServer_OnServerDataSocketHandler;
                    webServer.OnServerStartHandler += webServer_OnServerStartHandler;
                    IPEndPoint webPoint = new IPEndPoint(IPAddress.Any, webPort);//服务端会自动检测到自己的Ip，any
                    //web数据服务参数配置（以上）
                    webServer.start(webPoint);
                    //ShowLogData.add("【" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "】webServer网关启动成功！");
                    bridge = new DataBridge(dtuServer, webServer);

                    //setUpTime();//时间显示
                    //增加2015-5-6 kqz
                    string msg = string.Format("服务在端口{0}启动成功！", dtuPort);
                    this.SSLState.Text = msg;


                    this.Show();
                }
                else
                {
                }
            }
        }

        /// <summary>
        /// 初始化参数  port   timeout   xmal.config 里面的配置   获取成功返回ture
        /// </summary>
        /// <returns></returns>
        private bool loadConfig()
        {
            try
            {
                dtuPort = int.Parse(xmlHelper.getValue("dtuServerPort"));
                webPort = int.Parse(xmlHelper.getValue("webServerPort"));
                timeout = int.Parse(xmlHelper.getValue("socketTimeout"));
                connectedCount = int.Parse(xmlHelper.getValue("connectedCount"));
                int infoLog = int.Parse(xmlHelper.getValue("infoLogEnable"));
                int errorLog = int.Parse(xmlHelper.getValue("errorLogEnable"));
                int recordEvent = int.Parse(xmlHelper.getValue("recordEventEnable"));
                if (infoLog == 1)
                {
                    showInfoLog = true;
                }
                else
                {
                    showInfoLog = false;
                }
                if (errorLog == 1)
                {
                    showErrorLog = true;
                }
                else
                {
                    showErrorLog = false;
                }
                if (recordEvent == 1)
                {
                    this.recordEventEnable = true;
                }
                else
                {
                    this.recordEventEnable = false;
                }

                SysCache.ShowInfoLog = showInfoLog;
                SysCache.ShowErrorLog = showErrorLog;
                SysCache.RecordEevent = recordEventEnable;

                BaseMessage.ShowLog = SysCache.ShowInfoLog;
                WaterBaseMessage.ShowLog = SysCache.ShowInfoLog;

                FilePathCache.Path = xmlHelper.getValue("imgSavePath").Trim('/') + "/";
                return true;
            }
            catch (Exception)
            {
                MessageBox.Show("读取配置信息出错，程序退出！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Environment.Exit(0);
                return false;
            }
        }

        void webServer_OnServerStartHandler(Server.Web.ServerCore.ServerSocket server, string msg)
        {
            MessageBox.Show(msg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Environment.Exit(0);
        }

        void dtuServer_OnServerConnectedHandler(Server.Core.ServerCore.ServerSocket server, string msg)
        {
            MessageBox.Show(msg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Environment.Exit(0);
        }

        //将数据传递给处理方法proxyProcess  （以下）
        void webServer_OnServerDataSocketHandler(Server.Web.ServerCore.ServerSocket server, Server.Web.ServerCore.AsyncSocketUserToken userToken, object data)
        {
            DataTransfer transfer = (DataTransfer)data;
            proxyProcess(transfer);
        }

        void dtuServer_OnServerDataSocketHandler(Server.Core.ServerCore.ServerSocket server, Server.Core.ServerCore.AsyncSocketUserToken userToken, object data)
        {
            DataTransfer transfer = (DataTransfer)data;
            proxyProcess(transfer);
        }

        //根据信息的种类 ，进行处理：1.数据信息，发送给DTU  2.控制命令，发送给WEB  3.设备列表，更新显示表格
        private void proxyProcess(DataTransfer transfer)
        {
            switch (transfer.TransferType)
            {
                case DataTransferType.DataTransferDataInfo:
                    {
                        DataTransferInfo info = transfer.TransferDataInfo;
                        DeviceInfo di = new DeviceInfo();
                        di.Parse(info.DeviceNo);
                        string DeviceFullNo = DeviceModule.DeviceNo_Hex2Normal(di.SerialString);
                        string DeviceFullNoMain = DeviceModule.GetDeviceNoMain(DeviceFullNo);
                        bridge.sendToDtu(DeviceFullNo, DeviceFullNoMain, info.Data, info.Offset, info.Length);
                        break;
                    }
                case DataTransferType.DataTransferCommand:
                    {
                        DTU.GateWay.Protocol.Command command = transfer.TransferCommand;
                        bridge.sendToWeb(command.DeviceInfo.SerialLong, command);
                        break;
                    }
                case DataTransferType.DataTransferDeviceList:
                    {
                        Device deviceList = transfer.DeviceList;
                        DeviceEvent deviceEvent = transfer.DeviceEvent;
                        updateDeviceListInGridView(deviceList, deviceEvent);
                        break;
                    }
                case DataTransferType.DataTransferLog:
                    {
                        break;
                    }
                case DataTransferType.DataTransferUpdateDevice:
                    {
                        string kt = transfer.KeyType;
                        string key = transfer.Key;
                        if (SysCache.ShowInfoLog)
                        {
                            ShowLogData.add("更新设备缓存：" + kt + "，" + key);
                            logHelper.Info("更新设备缓存：" + kt + "，" + key);
                        }
                        if (kt == "01")
                        {
                            Device device = DeviceModule.GetDeviceByFullDeviceNo_DB(key);
                            if (device != null)
                            {
                                DeviceModule.UpdateDeviceInfo(device);
                                if (dtuServer != null)
                                    dtuServer.updateDevice(device);

                                updateDeviceListInGridView(device, null);
                            }
                            else
                            {
                                if (SysCache.ShowInfoLog)
                                {
                                    ShowLogData.add("更新设备缓存：" + kt + "，" + key + " 无法更新设备不存在");
                                    logHelper.Info("更新设备缓存：" + kt + "，" + key + " 无法更新设备不存在");
                                }
                            }
                        }
                        else if (kt == "02")
                        {
                            DeviceModule.RemoveDeviceInfo(key);
                            lock (this.deviceListsDataGridView)
                            {
                                int rows = this.deviceListsDataGridView.Rows.Count;
                                for (int i = 0; i < rows; i++)
                                {
                                    object obj = this.deviceListsDataGridView.Rows[i].Cells[1].Value;
                                    if (obj != null)
                                    {
                                        string val = obj.ToString();
                                        if (val.Equals(key))
                                        {
                                            this.deviceListsDataGridView.Rows.Remove(this.deviceListsDataGridView.Rows[i]);
                                            return;
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    }
                case DataTransferType.DataTransferUpdateCardUser:
                    {
                        string kt = transfer.KeyType;
                        string key = transfer.Key;
                        ShowLogData.add("更新用户卡缓存：" + kt + "，" + key);
                        if (kt == "01")
                        {
                            CardUser cardUser = CardUserModule.GetCardUserBySerialNumber_DB(key);
                            if (cardUser != null)
                            {
                                CardUserModule.UpdateCardUserInfo(cardUser);
                            }
                        }
                        else if (kt == "02")
                        {
                            CardUserModule.RemoveCardUserInfo(key);
                        }
                        break;
                    }
                case DataTransferType.DataTransferUpdateDistrict:
                    {
                        ShowLogData.add("更新地区缓存");
                        initDistrictTree();
                        break;
                    }
            }
        }

        /// <summary>
        /// 更新大数据表的显示
        /// </summary>
        /// <param name="deviceList"></param>
        private void updateDeviceListInGridView(Device deviceList, DeviceEvent deviceEvent)
        {
            lock (this.deviceListsDataGridView)
            {
                int rows = this.deviceListsDataGridView.Rows.Count;
                for (int i = 0; i < rows; i++)
                {
                    object obj = this.deviceListsDataGridView.Rows[i].Cells[1].Value;
                    if (obj != null)
                    {
                        string val = obj.ToString();
                        if (val.Equals(DeviceModule.GetFullDeviceNoByID(deviceList.Id)))
                        {
                            //存在修改行
                            this.deviceListsDataGridView.Rows[i].Cells[0].Value = (deviceList.Online == 0 ? "离线" : "在线");
                            this.deviceListsDataGridView.Rows[i].Cells[2].Value = deviceList.DeviceName;
                            this.deviceListsDataGridView.Rows[i].Cells[3].Value = deviceList.DeviceType;
                            this.deviceListsDataGridView.Rows[i].Cells[4].Value = deviceList.RemoteStation;
                            District district = DistrictModule.ReturnDistrictInfo(deviceList.DistrictId);
                            if (district != null)
                            {
                                this.deviceListsDataGridView.Rows[i].Cells[5].Value = district.DistrictName;
                            }
                            else
                            {
                                this.deviceListsDataGridView.Rows[i].Cells[5].Value = deviceList.DistrictId;
                            }
                            if (deviceList.StationType == 0)
                            {
                                this.deviceListsDataGridView.Rows[i].Cells[6].Value = "单站";
                                this.deviceListsDataGridView.Rows[i].Cells[7].Value = "";
                            }
                            else if (deviceList.StationType == 1)
                            {
                                this.deviceListsDataGridView.Rows[i].Cells[6].Value = "主站";
                                this.deviceListsDataGridView.Rows[i].Cells[7].Value = "";
                            }
                            else if (deviceList.StationType == 2)
                            {
                                Device deviceMain = DeviceModule.GetDeviceByID(deviceList.MainId);
                                if (deviceMain != null)
                                {
                                    this.deviceListsDataGridView.Rows[i].Cells[6].Value = DeviceModule.GetFullDeviceNoByID(deviceMain.Id);
                                    this.deviceListsDataGridView.Rows[i].Cells[7].Value = deviceMain.DeviceName;
                                }
                                else
                                {
                                    this.deviceListsDataGridView.Rows[i].Cells[6].Value = "从站";
                                    this.deviceListsDataGridView.Rows[i].Cells[7].Value = "";
                                }
                            }
                            if (deviceList.Online == 1)
                            {
                                if (deviceEvent != null)
                                {
                                    this.deviceListsDataGridView.Rows[i].Cells[8].Value = deviceEvent.EventTime.ToString("yyyy-MM-dd HH:mm:ss");
                                    this.deviceListsDataGridView.Rows[i].Cells[9].Value = deviceEvent.EventType;
                                    this.deviceListsDataGridView.Rows[i].Cells[10].Value = deviceEvent.Remark;
                                    this.deviceListsDataGridView.Rows[i].Cells[11].Value = deviceEvent.RawData;
                                }
                            }
                            else
                            {
                                this.deviceListsDataGridView.Rows[i].Cells[8].Value = deviceList.LastUpdate.ToString("yyyy-MM-dd HH:mm:ss");
                                this.deviceListsDataGridView.Rows[i].Cells[9].Value = "";
                                this.deviceListsDataGridView.Rows[i].Cells[10].Value = "";
                                this.deviceListsDataGridView.Rows[i].Cells[11].Value = "";
                            }
                            return;
                        }
                    }
                }

                //不存在添加新行
                this.deviceListsDataGridView.Rows.Add(DeviceViewRow(deviceList));
            }
        }

        //启动时，设备全部显示离线
        #region 2015-8-7 每次启动时，将登陆状态置为0
        void installDeviceState()
        {
            try
            {
                BaseModule.LoadBaseInfo();
                CardUserModule.LoadCardUsers();
                DistrictModule.UpdateLevelInfo();
                DistrictModule.UpdateDistrictInfo();

                //将在线状态置为0
                DeviceModule.SetOnline0();

                DeviceModule.LoadDevices();
                /*
                IList<Device> deviceList = DeviceModule.GetAllDevice();

                foreach (Device d in deviceList)
                {
                    if (d.Online == 1)
                    {
                        d.Online = 0;
                        DeviceModule.ModifyDevice(d);
                    }
                }
                 * */

                AlarmService.Init();
                int alarmCount = AlarmService.GetCount();
                //logHelper.Info("程序启动报警数量为：" + alarmCount);
                ShowLogData.add("程序启动报警数量为：" + alarmCount);
            }
            catch (Exception e)
            {
                if (showErrorLog)
                {
                    string error = string.Format("在将设备登陆状态置为0时，读取数据库时出错，错误信息{0}", e.Message);
                    logHelper.Error(error);
                }
            }
            finally
            {
            }
        }
        #endregion

        private void DtuMain_Load(object sender, EventArgs e)
        {
            if (loadConfigFlag)
            {
                this.connectedStripStatusLabel.Text = "0";

                //ShowLogData.add("【" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "】加载地区树！");
                initDistrictTree();
                //ShowLogData.add("【" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "】加载地区树完成！");
                //ShowLogData.add("【" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "】加载终端列表！");
                initDataGridView();
                //ShowLogData.add("【" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "】加载终端列表完成！");
            }
        }

        void DataSyn_OnLogHandler(string msg)
        {
            ShowLogData.add("【" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "】数据同步信息：" + msg);
        }

        public void setUpTime()
        {
            this.SSLDateNow.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        bool isRefresh = false;
        /// <summary>
        /// 时钟，用来更显下边栏状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolTripTime_Tick(object sender, EventArgs e)
        {
            setUpTime();//   更新时间显示

            //if (DataSynLib.DataSyn.isRun) 数据同步ToolStripMenuItem.ForeColor = System.Drawing.Color.Green;
            //else 数据同步ToolStripMenuItem.ForeColor = System.Drawing.Color.Red;

            if (!isRefresh)
            {
                isRefresh = true;
                /*
                if (dtuServer != null)
                {
                    int count = dtuServer.AsyncSocketUserTokenList.count();//计算socket连接数量
                    //this.connectedStripStatusLabel.Text = count.ToString();//显示在线设备数量
                }
                */

                this.connectedStripStatusLabel.Text = OnlineDeviceService.GetOnlineCount().ToString();//显示在线设备数量

                DateTime dateNow = DateTime.Now;
                if (!ToDtuCommand.isClear && ToDtuCommand.dateNew < dateNow.AddHours(-2))
                {
                    ShowLogData.add("【" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "】清理ToDtuCommand信息");
                    ToDtuCommand.Clear();
                }
                if (!ToWaterDtuCommand.isClear && ToWaterDtuCommand.dateNew < dateNow.AddHours(-2))
                {
                    ShowLogData.add("【" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "】清理ToWaterDtuCommand信息");
                    ToWaterDtuCommand.Clear();
                }
                /*
                timeCount = timeCount + 1;
                if (this.timeCount == 5)//5秒初始化一次大表格
                {
                    this.timeCount = 0;
                    lock (this.deviceListsDataGridView)
                    {
                        initDataGridView();
                    }
                }
                 * */

                isRefresh = false;
            }
        }

        #region 初始化gridView
        public void initDataGridView()
        {
            try
            {
                IList<Device> deviceList = DeviceModule.GetAllDevice();
                if (this.deviceLists == null || this.deviceLists.Count != deviceList.Count)
                {
                    this.deviceLists = deviceList;
                    //防止界面表格重复添加数据
                    if (this.deviceListsDataGridView != null && this.deviceListsDataGridView.Rows != null)
                    {
                        this.deviceListsDataGridView.Rows.Clear();
                    }
                    initData(deviceList);
                }
            }
            catch (Exception e)
            {
                if (showErrorLog)
                {
                    string error = string.Format("在初始化datagride，读取数据库时出错，错误信息{0}", e.Message);
                    logHelper.Error(error);
                }
            }
            finally
            {
            }

        }

        /// <summary>
        /// 表格中的数据，判断设备的状态，通过键值对  DeviceLists 的接口实现
        /// </summary>
        /// <param name="deviceLists"></param>
        public void initData(IList<Device> deviceLists)
        {
            if (null != deviceLists && deviceLists.Count > 0)
            {
                for (int i = 0; i < deviceLists.Count; i++)
                {
                    Device device = deviceLists[i];

                    this.deviceListsDataGridView.Rows.Add(DeviceViewRow(device));
                }
            }
            else
            {
                if (this.deviceListsDataGridView != null && this.deviceListsDataGridView.Rows != null)
                {
                    this.deviceListsDataGridView.Rows.Clear();
                }
            }
        }

        private DataGridViewRow DeviceViewRow(Device device)
        {
            DataGridViewRow viewRow = new DataGridViewRow();

            DataGridViewTextBoxCell deviceStateCell = new DataGridViewTextBoxCell();
            deviceStateCell.Value = (device.Online == 0 ? "离线" : "在线");
            viewRow.Cells.Add(deviceStateCell);

            DataGridViewTextBoxCell deviceNoCell = new DataGridViewTextBoxCell();
            deviceNoCell.Value = DeviceModule.GetFullDeviceNoByID(device.Id);
            viewRow.Cells.Add(deviceNoCell);

            DataGridViewTextBoxCell deviceDeviceNameCell = new DataGridViewTextBoxCell();
            deviceDeviceNameCell.Value = device.DeviceName;
            viewRow.Cells.Add(deviceDeviceNameCell);

            DataGridViewTextBoxCell deviceDeviceTypeCell = new DataGridViewTextBoxCell();
            deviceDeviceTypeCell.Value = device.DeviceType;
            viewRow.Cells.Add(deviceDeviceTypeCell);

            DataGridViewTextBoxCell deviceRemoteStationCell = new DataGridViewTextBoxCell();
            deviceRemoteStationCell.Value = device.RemoteStation;
            viewRow.Cells.Add(deviceRemoteStationCell);

            DataGridViewTextBoxCell deviceDistrictNameCell = new DataGridViewTextBoxCell();
            District district = DistrictModule.ReturnDistrictInfo(device.DistrictId);
            if (district != null)
            {
                deviceDistrictNameCell.Value = district.DistrictName;
            }
            else
            {
                deviceDistrictNameCell.Value = device.DistrictId;
            }
            viewRow.Cells.Add(deviceDistrictNameCell);

            if (device.StationType == 0)
            {
                DataGridViewTextBoxCell deviceMainCell = new DataGridViewTextBoxCell();
                deviceMainCell.Value = "单站";
                viewRow.Cells.Add(deviceMainCell);

                DataGridViewTextBoxCell deviceNameMainCell = new DataGridViewTextBoxCell();
                deviceNameMainCell.Value = "";
                viewRow.Cells.Add(deviceNameMainCell);
            }
            else if (device.StationType == 1)
            {
                DataGridViewTextBoxCell deviceMainCell = new DataGridViewTextBoxCell();
                deviceMainCell.Value = "主站";
                viewRow.Cells.Add(deviceMainCell);

                DataGridViewTextBoxCell deviceNameMainCell = new DataGridViewTextBoxCell();
                deviceNameMainCell.Value = "";
                viewRow.Cells.Add(deviceNameMainCell);
            }
            else if (device.StationType == 2)
            {
                Device deviceMain = DeviceModule.GetDeviceByID(device.MainId);
                if (deviceMain != null)
                {
                    DataGridViewTextBoxCell deviceMainCell = new DataGridViewTextBoxCell();
                    deviceMainCell.Value = DeviceModule.GetFullDeviceNoByID(deviceMain.Id);
                    viewRow.Cells.Add(deviceMainCell);

                    DataGridViewTextBoxCell deviceNameMainCell = new DataGridViewTextBoxCell();
                    deviceNameMainCell.Value = deviceMain.DeviceName;
                    viewRow.Cells.Add(deviceNameMainCell);
                }
            }

            DataGridViewTextBoxCell dataEventTimeCell = new DataGridViewTextBoxCell();
            dataEventTimeCell.Value = "";
            viewRow.Cells.Add(dataEventTimeCell);

            DataGridViewTextBoxCell deviceEventTypeCell = new DataGridViewTextBoxCell();
            deviceEventTypeCell.Value = "";
            viewRow.Cells.Add(deviceEventTypeCell);

            DataGridViewTextBoxCell deviceRawDataDescCell = new DataGridViewTextBoxCell();
            deviceRawDataDescCell.Value = "";
            viewRow.Cells.Add(deviceRawDataDescCell);

            DataGridViewTextBoxCell deviceRawDataCell = new DataGridViewTextBoxCell();
            deviceRawDataCell.Value = "";
            viewRow.Cells.Add(deviceRawDataCell);

            return viewRow;
        }

        #endregion

        #region 初始化区域树
        public void initDistrictTree()
        {
            ThreadFunction();
        }

        private delegate void FlushClient();
        private void ThreadFunction()
        {
            if (this.districtTreeView.InvokeRequired)//等待异步
            {
                FlushClient fc = new FlushClient(ThreadFunction);
                this.Invoke(fc);//通过代理调用刷新方法
            }
            else
            {
                initDistrictTreeRoot();
            }
        }

        public void initDistrictTreeRoot()//左侧侧边栏
        {
            DistrictModule.UpdateDistrictInfo();
            DistrictModule.UpdateLevelInfo();
            this.districtTreeView.Nodes.Clear();
            try
            {
                IList<District> districts = DistrictModule.GetAllDistrict();
                if (null != districts && districts.Count > 0)
                {
                    for (int i = 0; i < districts.Count; i++)
                    {
                        District district = districts[i];
                        District Parent = DistrictModule.ReturnDistrictInfo(district.ParentId);
                        IList<District> Children = DistrictModule.GetChildrenDistrict(district.Id);
                        if (Parent == null)
                        {
                            TreeNode root = new TreeNode();
                            root.Text = district.DistrictName;
                            root.Tag = district;

                            if (null != Children && Children.Count >= 0)
                            {
                                initDistrictTreeChild(root, Children);
                            }

                            this.districtTreeView.Nodes.Add(root);
                            root.Expand();
                        }
                    }
                }
                //如果为0，清除掉所有节点
                else
                {
                    //this.districtTreeView.Nodes.Clear();
                }
            }
            catch (Exception)
            {
            }
            finally
            {
            }
        }
        //地区树形图
        public void initDistrictTreeChild(TreeNode parent, IList<District> childs)
        {
            for (int i = 0; i < childs.Count; i++)
            {
                District district = childs[i];
                TreeNode child = new TreeNode();
                child.Tag = district;
                child.Text = district.DistrictName;
                IList<District> Children = DistrictModule.GetChildrenDistrict(district.Id);

                if (null != Children && Children.Count >= 0)
                {
                    initDistrictTreeChild(child, Children);
                }
                parent.Nodes.Add(child);

            }
        }
        #endregion

        /// <summary>
        /// 窗体关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DtuMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            ShowLogData.isShow = false;

            this.toolTripTime.Enabled = false;
            this.toolTripTime.Stop();
            if (dtuServer != null && dtuServer.DeamonThread != null)
            {
                dtuServer.DeamonThread.close();
            }
        }

        /// <summary>
        /// 显示行号？
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deviceListsDataGridView_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            System.Drawing.Rectangle rectangle = new System.Drawing.Rectangle(e.RowBounds.Location.X, e.RowBounds.Location.Y,
               this.deviceListsDataGridView.RowHeadersWidth - 4, e.RowBounds.Height);

            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
                this.deviceListsDataGridView.RowHeadersDefaultCellStyle.Font,
                rectangle,
                this.deviceListsDataGridView.RowHeadersDefaultCellStyle.ForeColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }

        frmLog fLog = null;
        private void 查看日志ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fLog.Show();
            fLog.Activate();
        }

        frmConfig fConfig = null;
        private void 服务器配置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fConfig.Show();
            fConfig.Activate();
        }

        frmCS fCS = null;
        private void 测试ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fCS == null) //如果子窗体为空则创造实例 并显示
            {
                fCS = new frmCS(bridge);
                fCS.Show();
            }
            else
            {
                if (fCS.IsDisposed)//若子窗体关闭 则打开新子窗体 并显示
                {
                    fCS = new frmCS(bridge);
                    fCS.Show();
                }
                else
                {
                    fCS.Activate(); //使子窗体获得焦点
                }
            }
        }

        frmCache fCache = null;
        private void 数据缓存ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fCache == null) //如果子窗体为空则创造实例 并显示
            {
                fCache = new frmCache(dtuServer);
                fCache.Show();
            }
            else
            {
                if (fCache.IsDisposed)//若子窗体关闭 则打开新子窗体 并显示
                {
                    fCache = new frmCache(dtuServer);
                    fCache.Show();
                }
                else
                {
                    fCache.Activate(); //使子窗体获得焦点
                }
            }
        }

        frmCS1 fCS1 = null;
        private void 水位计测试ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fCS1 == null) //如果子窗体为空则创造实例 并显示
            {
                fCS1 = new frmCS1(bridge);
                fCS1.Show();
            }
            else
            {
                if (fCS1.IsDisposed)//若子窗体关闭 则打开新子窗体 并显示
                {
                    fCS1 = new frmCS1(bridge);
                    fCS1.Show();
                }
                else
                {
                    fCS1.Activate(); //使子窗体获得焦点
                }
            }
        }

        private void 数据同步ToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }
    }
}
