using Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Utils;

namespace CardOperationSystem
{
    public partial class frmMain : Form
    {
        static log4net.ILog myLogger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public frmMain()
        {
            InitializeComponent();

            lbResult.Text = InfoSys.StrUnknown;
            lbHardVer.Text = InfoSys.StrUnknown;
            lbSoftVer.Text = InfoSys.StrUnknown;
            lbIcdev.Text = InfoSys.StrUnknown;

            this.textBox1.Text = "";
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            this.timerIC.Enabled = true;
            this.timerIC.Start();

            this.dataGridView1.AutoGenerateColumns = false;

            this.lbTrueName.Text = InfoSys.UserTrueName;
            this.lbGroupName.Text = InfoSys.UserGroupName;
            this.lbDistrictName.Text = InfoSys.UserDistrictName;

            if (InfoSys.LoginUserName == "admin")
            {
                btnInit.Visible = true;
                btnReadIC.Visible = true;
            }

            this.lbDeviceCount.Text = "数量：终端 " + InfoSys.ListDevices.Count.ToString() + "，行政区 " + InfoSys.ListDistricts.Count.ToString();
        }

        int GroupNo = 0; //用户级别
        public bool connectIC = false;
        public int icdev; // 通讯设备标识符
        UInt16 tagtype = 0;
        byte size = 0;
        uint snr = 0;

        private void timerIC_Tick(object sender, EventArgs e)
        {
            if (!connectIC)
            {
                if (Connect())
                {
                    connectIC = true;
                }
            }
        }

        private bool Connect()
        {
            byte[] ver = new byte[30];

            common.lib_ver(ver);
            string sver = System.Text.Encoding.ASCII.GetString(ver);
            lbSoftVer.Text = sver;

            icdev = common.rf_init(0, 9600);
            this.lbIcdev.Text = icdev.ToString();

            if (icdev > 0)
            {
                lbResult.Text = "打开串口成功！";
                byte[] status = new byte[30];
                common.rf_get_status(icdev, status);
                lbHardVer.Text = System.Text.Encoding.ASCII.GetString(status);
                common.rf_beep(icdev, 5);
                return true;
            }
            else
            {
                lbResult.Text = "打开串口失败！";
                return false;
            }
        }

        public string getKeyA()
        {
            return InfoSys.KeyA;
        }

        public string getKeyB()
        {
            return InfoSys.KeyB;
        }

        public uint getSnr()
        {
            return snr;
        }

        public byte getSize()
        {
            return size;
        }

        public void clearLog()
        {
            this.textBox1.Text = "";
        }

        public void AddLog(string str, bool isNewLine)
        {
            ToolsForm.AddLog(str, true, false, this.textBox1);
        }

        public void Log(string s)
        {
            AddLog(s, true);
        }

        public void BeginLog(string CardTypeStr, string s)
        {
            Log(getDateStr() + CardTypeStr + s + "开始");
        }

        public void EndLog(string CardTypeStr, string s)
        {
            Log(getDateStr() + CardTypeStr + s + "结束");
        }

        public void AuthLog(string CardTypeStr, string s, int sec, string v)
        {
            Log(CardTypeStr + s + " 扇区：" + sec + " 认证状态：" + v);
        }

        public string getDateStr()
        {
            return "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "]";
        }

        public bool SeedIC()
        {
            mifareone.rf_reset(icdev, 3);
            if (mifareone.rf_request(icdev, 1, out tagtype) != 0)
            {
                Log("卡类型获取失败");
                return false;
            }

            Log("tagtype：" + tagtype);

            if (mifareone.rf_anticoll(icdev, 0, out snr) != 0)
            {
                Log("卡序列号获取失败");
                return false;
            }

            Log("snr：" + snr.ToString("X"));

            if (mifareone.rf_select(icdev, snr, out size) != 0)
            {
                Log("卡容量获取失败");
                return false;
            }

            Log("size：" + size);

            Log(InfoSys.StrSeekSuccess);
            return true;
        }

        frmCardUser f = null;
        private void btnOpenIC_Click(object sender, EventArgs e)
        {
            if (!connectIC)
            {
                MessageBox.Show("请等待串口打开成功！");
                return;
            }

            f = new frmCardUser(this);
            f.ShowDialog();
            /*
            if (f == null) //如果子窗体为空则创造实例 并显示
            {
                f = new frmCardUser(this);
                f.ShowDialog();
            }
            else
            {
                if (f.IsDisposed)//若子窗体关闭 则打开新子窗体 并显示
                {
                    f = new frmCardUser(this);
                    f.ShowDialog();
                }
                else
                {
                    f.Activate(); //使子窗体获得焦点
                }
            }
             * */
        }

        frmCardDevice fCardDevice = null;
        private void btnDevice_Click(object sender, EventArgs e)
        {
            if (!connectIC)
            {
                MessageBox.Show("请等待串口打开成功！");
                return;
            }

            fCardDevice = new frmCardDevice(this);
            fCardDevice.ShowDialog();
            /*
            if (fCardDevice == null) //如果子窗体为空则创造实例 并显示
            {
                fCardDevice = new frmCardDevice(this);
                fCardDevice.ShowDialog();
            }
            else
            {
                if (fCardDevice.IsDisposed)//若子窗体关闭 则打开新子窗体 并显示
                {
                    fCardDevice = new frmCardDevice(this);
                    fCardDevice.ShowDialog();
                }
                else
                {
                    fCardDevice.Activate(); //使子窗体获得焦点
                }
            }
             * */
        }

        frmCardRead fCardRead = null;
        private void button1_Click(object sender, EventArgs e)
        {
            if (!connectIC)
            {
                MessageBox.Show("请等待串口打开成功！");
                return;
            }
            fCardRead = new frmCardRead(this);
            fCardRead.ShowDialog();
            /*
            if (fCardRead == null) //如果子窗体为空则创造实例 并显示
            {
                fCardRead = new frmCardRead(this);
                fCardRead.ShowDialog();
            }
            else
            {
                if (fCardRead.IsDisposed)//若子窗体关闭 则打开新子窗体 并显示
                {
                    fCardRead = new frmCardRead(this);
                    fCardRead.ShowDialog();
                }
                else
                {
                    fCardRead.Activate(); //使子窗体获得焦点
                }
            }
             * */
        }

        frmCardClear fCardClear = null;
        private void button2_Click(object sender, EventArgs e)
        {
            if (!connectIC)
            {
                MessageBox.Show("请等待串口打开成功！");
                return;
            }

            fCardClear = new frmCardClear(this);
            fCardClear.ShowDialog();
            /*
            if (fCardClear == null) //如果子窗体为空则创造实例 并显示
            {
                fCardClear = new frmCardClear(this);
                fCardClear.ShowDialog();
            }
            else
            {
                if (fCardClear.IsDisposed)//若子窗体关闭 则打开新子窗体 并显示
                {
                    fCardClear = new frmCardClear(this);
                    fCardClear.ShowDialog();
                }
                else
                {
                    fCardClear.Activate(); //使子窗体获得焦点
                }
            }
             * */
        }

        private void btnInit_Click(object sender, EventArgs e)
        {
            if (!(MessageBox.Show("确定注销卡？", "注销卡", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes))
            {
                return;
            }

            Log(getDateStr() + "开始注销");
            if (connectIC)
            {
                if (!SeedIC())
                {
                    Log("寻卡失败");
                    Log(getDateStr() + "结束注销");
                    return;
                }

                bool flag = false;
                for (int j = 0; j < size; j++)
                {
                    string result_AuthIC = CardCommon.AuthIC(icdev, 0, j, getKeyB());
                    Log("注销卡扇区：" + j + " 认证状态：" + result_AuthIC);
                    if (result_AuthIC != InfoSys.StrAuthSuccess)
                    {
                        continue;
                    }
                    else
                    {
                        if(!flag) flag = true;
                        int begin = 0;
                        if (j == 0)
                        {
                            begin = 1;
                        }

                        for (int k = begin; k < 3; k++)
                        {
                            string result = CardCommon.WriteIC(icdev, j, k, "".PadRight(32, '0'));
                            Log("注销卡扇区：" + j + " 数据块：" + k + " 结果：" + (result == "" ? "成功" : result));
                        }

                        {
                            int block = 3;
                            string result = CardCommon.WriteIC(icdev, j, block, getKeyA() + InfoSys.KeyControl + getKeyA());
                            Log("注销卡写密码扇区：" + j + " 数据块：" + block + " 结果：" + (result == "" ? "成功" : result));
                        }
                    }
                }

                if (!flag)
                {
                    Log("第一次注销失败，开始第二次注销");
                    if (!SeedIC())
                    {
                        Log("寻卡失败");
                        Log(getDateStr() + "结束注销");
                        return;
                    }

                    for (int j = 0; j < size; j++)
                    {
                        string result_AuthIC = CardCommon.AuthIC(icdev, 0, j, getKeyA());
                        Log("注销卡扇区：" + j + " 认证状态：" + result_AuthIC);
                        if (result_AuthIC != InfoSys.StrAuthSuccess)
                        {
                            continue;
                        }
                        else
                        {
                            if (!flag) flag = true;
                            int begin = 0;
                            if (j == 0)
                            {
                                begin = 1;
                            }

                            for (int k = begin; k < 3; k++)
                            {
                                string result = CardCommon.WriteIC(icdev, j, k, "".PadRight(32, '0'));
                                Log("注销卡扇区：" + j + " 数据块：" + k + " 结果：" + (result == "" ? "成功" : result));
                            }

                            {
                                int block = 3;
                                string result = CardCommon.WriteIC(icdev, j, block, getKeyA() + InfoSys.KeyControl + getKeyA());
                                Log("注销卡写密码扇区：" + j + " 数据块：" + block + " 结果：" + (result == "" ? "成功" : result));
                            }
                        }
                    }
                }
            }
            else
            {
                Log("无法注销！确保打开串口成功！");
            }

            Log(getDateStr() + "结束注销");
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            clearLog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Log(getDateStr() + "检测卡开始");
            if (connectIC)
            {
                if (!SeedIC())
                {
                    Log("寻卡失败");
                    MessageBox.Show("寻卡失败！");
                    Log(getDateStr() + "检测卡结束");
                    return;
                }

                int mode = 4; //以B密码认证
                int sec = 1; //扇区
                int block = 0;
                string keyA = getKeyA(); //读卡密码
                string keyB = getKeyB(); //读卡密码
                string result_ReadIC = "";
                string result_AuthIC = "";

                //读取扇区1内容
                sec = 1;
                //认证卡密码B，认证成功为非新卡
                result_AuthIC = CardCommon.AuthIC(icdev, mode, sec, keyB);
                if (result_AuthIC == InfoSys.StrAuthSuccess)
                {
                    //读取数据块0
                    block = 0;
                    result_ReadIC = CardCommon.ReadIC(icdev, sec, block);
                    if (result_ReadIC.Length == 32)
                    {
                        string CardType = result_ReadIC.Substring(0, 2);
                        if (CardType == InfoSys.CardTypeUser)
                        {
                            Log("此卡为" + InfoSys.CardTypeStrUser);
                            MessageBox.Show("此卡为" + InfoSys.CardTypeStrUser);
                        }
                        else if (CardType == InfoSys.CardTypeDevice)
                        {
                            Log("此卡为" + InfoSys.CardTypeStrDevice);
                            MessageBox.Show("此卡为" + InfoSys.CardTypeStrDevice);
                        }
                        else if (CardType == InfoSys.CardTypeRead)
                        {
                            Log("此卡为" + InfoSys.CardTypeStrRead);
                            MessageBox.Show("此卡为" + InfoSys.CardTypeStrRead);
                        }
                        else if (CardType == InfoSys.CardTypeClear)
                        {
                            Log("此卡为" + InfoSys.CardTypeStrClear);
                            MessageBox.Show("此卡为" + InfoSys.CardTypeStrClear);
                        }
                        else if (CardType == InfoSys.CardTypeNetSet)
                        {
                            Log("此卡为" + InfoSys.CardTypeStrNetSet);
                            MessageBox.Show("此卡为" + InfoSys.CardTypeStrNetSet);
                        }
                        else
                        {
                            Log("卡类型未知：" + CardType);
                            MessageBox.Show("卡类型未知：" + CardType);
                        }
                    }
                    else
                    {
                        Log("卡类型数据获取失败！");
                        MessageBox.Show("卡类型数据获取失败！");
                    }
                }
                else
                {
                    Log("重新寻卡进行新卡认证");
                    if (!SeedIC())
                    {
                        Log("寻卡失败");
                        Log(getDateStr() + "检测卡结束");
                        return;
                    }

                    //新卡检测
                    result_AuthIC = CardCommon.AuthIC(icdev, mode, sec, keyA);
                    if (result_AuthIC == InfoSys.StrAuthSuccess)
                    {
                        block = 0;
                        result_ReadIC = CardCommon.ReadIC(icdev, sec, block);
                        if (result_ReadIC.Length == 32)
                        {
                            Log("此卡为新卡");
                            MessageBox.Show("此卡为新卡！");
                        }
                        else
                        {
                            Log("新卡检测失败！");
                            MessageBox.Show("新卡检测失败！");
                        }
                    }
                    else
                    {
                        Log("新卡认证失败！");
                        MessageBox.Show("新卡认证失败！");
                    }

                }
            }
            else
            {
                Log("无法检测卡，确保打开串口成功！");
                MessageBox.Show("无法检测卡，确保打开串口成功！");
            }
            Log(getDateStr() + "检测卡结束");
        }

        frmConfig fConfig = null;
        private void button5_Click(object sender, EventArgs e)
        {
            if (fConfig == null) //如果子窗体为空则创造实例 并显示
            {
                fConfig = new frmConfig(true);
                fConfig.ShowDialog();
            }
            else
            {
                if (fConfig.IsDisposed)//若子窗体关闭 则打开新子窗体 并显示
                {
                    fConfig = new frmConfig(true);
                    fConfig.ShowDialog();
                }
                else
                {
                    fConfig.Activate(); //使子窗体获得焦点
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定退出程序？", "退出", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                this.timerIC.Stop();
                //Application.Exit();
                Application.ExitThread();
            }
        }

        private void btnReadIC_Click(object sender, EventArgs e)
        {
            clearLog();
            this.groupBox6.Visible = false;
            this.groupBox4.Visible = true;
            Log(getDateStr() + "开始读卡");
            if (connectIC)
            {
                Log("可以读卡");

                if (!SeedIC())
                {
                    Log("1 寻卡失败");
                    Log(getDateStr() + "结束读卡");
                    return;
                }

                bool flag = false;
                for (int j = 0; j < size; j++)
                {
                    string result_AuthIC = CardCommon.AuthIC(icdev, 0, j, getKeyB());
                    Log("1 读卡扇区：" + j + " 认证状态：" + result_AuthIC);
                    if (result_AuthIC == InfoSys.StrAuthSuccess)
                    {
                        if (!flag) flag = true;
                        for (int k = 0; k < 4; k++)
                        {
                            Log("1 读卡扇区：" + j + " 数据块：" + k + " 结果：" + CardCommon.ReadIC(icdev, j, k));
                        }
                    }
                }

                if (!flag)
                {
                    Log("第一次读取失败，开始第二次读取");
                    if (!SeedIC())
                    {
                        Log("2 寻卡失败");
                        Log(getDateStr() + "结束读卡");
                        return;
                    }

                    for (int j = 0; j < size; j++)
                    {
                        string result_AuthIC = CardCommon.AuthIC(icdev, 0, j, getKeyA());
                        Log("2 读卡扇区：" + j + " 认证状态：" + result_AuthIC);
                        if (result_AuthIC == InfoSys.StrAuthSuccess)
                        {
                            for (int k = 0; k < 4; k++)
                            {
                                Log("2 读卡扇区：" + j + " 数据块：" + k + " 结果：" + CardCommon.ReadIC(icdev, j, k));
                            }
                        }
                    }
                }
            }
            else
            {
                Log("无法读卡，确保打开串口成功！");
            }

            Log(getDateStr() + "结束读卡");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            InfoSys.GetBaseInfo();
            this.lbDeviceCount.Text = "数量：终端 " + InfoSys.ListDevices.Count.ToString() + "，行政区 " + InfoSys.ListDistricts.Count.ToString();

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            this.groupBox4.Visible = false;
            this.groupBox6.Visible = true;
            SearchCardUser();
        }

        private void SearchCardUser()
        {
            string UserNo = this.txtUserNo.Text.Trim();
            string UserName = this.txtUserName.Text.Trim();
            string IdentityNumber = this.txtIdentityNumber.Text.Trim();
            string Telephone = this.txtTelephone.Text.Trim();
            /*
            if (UserNo.Trim() == "" && UserName.Trim() == "" && IdentityNumber.Trim() == "" && Telephone.Trim() == "")
            {
                MessageBox.Show("至少要有一个查询条件不为空");
                return;
            }
             * */
            string str = DataTransfer.GetCardUserList(UserNo, UserName, IdentityNumber, Telephone);

            JavaScriptObject result = (JavaScriptObject)JavaScriptConvert.DeserializeObject(str);
            if (bool.Parse(result["Result"].ToString()))
            {
                List<JsonCardUser> list = JavaScriptConvert.DeserializeObject<List<JsonCardUser>>(result["Message"].ToString());
                if (list.Count > 0)
                {
                    ModelHandler<JsonCardUser> mh = new ModelHandler<JsonCardUser>();
                    DataTable dt = mh.FillDataTable(list);
                    this.dataGridView1.DataSource = dt.DefaultView;
                    this.dataGridView1.ClearSelection();
                }
                else
                {
                    this.dataGridView1.DataSource = null;
                    MessageBox.Show("未查询到记录");
                }
            }
            else
            {
                MessageBox.Show(result["Message"].ToString());
                myLogger.Error(result["Message"].ToString());
            }
        }

        private void btnCountermand_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedRows.Count < 1)
            {
                MessageBox.Show("请选择一行记录！");
                return;
            }
            string SerialNumber = this.dataGridView1.SelectedRows[0].Cells["SerialNumber"].Value.ToString();
            string UserNo = this.dataGridView1.SelectedRows[0].Cells["UserNo"].Value.ToString();
            string UserName = this.dataGridView1.SelectedRows[0].Cells["UserName"].Value.ToString();
            string IdentityNumber = this.dataGridView1.SelectedRows[0].Cells["IdentityNumber"].Value.ToString();
            if (MessageBox.Show("确定挂失当前选择用户卡？" + Environment.NewLine + "卡号：" + UserNo + "，用户名：" + UserName +
                Environment.NewLine + "身份证号：" + IdentityNumber,
                "挂失", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                string str = DataTransfer.CountermandCardUser(SerialNumber);
                JavaScriptObject result = (JavaScriptObject)JavaScriptConvert.DeserializeObject(str);
                if (bool.Parse(result["Result"].ToString()))
                {
                    MessageBox.Show("挂失成功！");
                    SearchCardUser();
                }
                else
                {
                    MessageBox.Show(result["Message"].ToString());
                    myLogger.Error(result["Message"].ToString());
                }
            }
        }

        private void btnCountermandCancel_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedRows.Count < 1)
            {
                MessageBox.Show("请选择一行记录！");
                return;
            }
            string SerialNumber = this.dataGridView1.SelectedRows[0].Cells["SerialNumber"].Value.ToString();
            string UserNo = this.dataGridView1.SelectedRows[0].Cells["UserNo"].Value.ToString();
            string UserName = this.dataGridView1.SelectedRows[0].Cells["UserName"].Value.ToString();
            string IdentityNumber = this.dataGridView1.SelectedRows[0].Cells["IdentityNumber"].Value.ToString();
            if (MessageBox.Show("确定取消挂失当前选择用户卡？" + Environment.NewLine + "卡号：" + UserNo + "，用户名：" + UserName +
                Environment.NewLine + "身份证号：" + IdentityNumber,
                "取消挂失", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                string str = DataTransfer.CountermandCancelCardUser(SerialNumber);
                JavaScriptObject result = (JavaScriptObject)JavaScriptConvert.DeserializeObject(str);
                if (bool.Parse(result["Result"].ToString()))
                {
                    MessageBox.Show("取消挂失成功！");
                    SearchCardUser();
                }
                else
                {
                    MessageBox.Show(result["Message"].ToString());
                    myLogger.Error(result["Message"].ToString());
                }
            }
        }

        private void btnReOpen_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedRows.Count < 1)
            {
                MessageBox.Show("请选择一行记录！");
                return;
            }
            string SerialNumber = this.dataGridView1.SelectedRows[0].Cells["SerialNumber"].Value.ToString();
            string UserNo = this.dataGridView1.SelectedRows[0].Cells["UserNo"].Value.ToString();
            string UserName = this.dataGridView1.SelectedRows[0].Cells["UserName"].Value.ToString();
            string IdentityNumber = this.dataGridView1.SelectedRows[0].Cells["IdentityNumber"].Value.ToString();
            byte[] newUserName = null;
            newUserName = HexStringUtility.StrToByteArray("".PadRight(16, ' '));

            string DeviceList = this.dataGridView1.SelectedRows[0].Cells["DeviceList"].Value.ToString();

            string ResidualWater = this.dataGridView1.SelectedRows[0].Cells["ResidualWater"].Value.ToString();
            string ResidualElectric = this.dataGridView1.SelectedRows[0].Cells["ResidualElectric"].Value.ToString();
            string TotalWater = this.dataGridView1.SelectedRows[0].Cells["TotalWater"].Value.ToString();
            string TotalElectric = this.dataGridView1.SelectedRows[0].Cells["TotalElectric"].Value.ToString();

            if (MessageBox.Show("确定重新开卡当前选择用户卡？" + Environment.NewLine + "卡号：" + UserNo + "，用户名：" + UserName +
                Environment.NewLine + "身份证号：" + IdentityNumber,
                "重新开卡", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                Log(getDateStr() + "开始重新开卡");

                if (!SeedIC())
                {
                    Log("寻卡失败");
                    Log(getDateStr() + "结束重新开卡");
                    return;
                }

                string SerialNumberNew = getSnr().ToString("X");
                
                //保存远程服务器数据库
                string str = DataTransfer.ReOpenCardUser(SerialNumber, SerialNumberNew);
                JavaScriptObject result = (JavaScriptObject)JavaScriptConvert.DeserializeObject(str);
                if (bool.Parse(result["Result"].ToString()))
                {
                    MessageBox.Show("取消挂失成功！");
                    SearchCardUser();
                }
                else
                {
                    string txt = result["Message"].ToString();
                    MessageBox.Show(txt);
                    myLogger.Error(txt);
                    Log(txt);
                    Log(getDateStr() + "结束重新开卡");
                    return;
                }
                
                int mode = 4; //以B密码认证
                int sec = 1; //扇区
                int block = 0;
                string key = getKeyA(); //读卡密码
                string result_WriteIC = "";
                string result_AuthIC = "";

                //设置扇区1内容
                sec = 1;
                //认证卡密码B
                result_AuthIC = CardCommon.AuthIC(icdev, mode, sec, key);
                if (result_AuthIC == InfoSys.StrAuthSuccess)
                {
                    //写数据块0，数据块0为卡类型（1字节）、用户卡号（4字节）
                    block = 0;
                    result_WriteIC = CardCommon.WriteIC(icdev, sec, block, (InfoSys.CardTypeUser + UserNo.PadLeft(8, '0')).PadRight(32, '0'));
                    if (result_WriteIC != "")
                    {
                        Log("重新写卡扇区" + sec + "数据块" + block + "写入失败");
                        Log(getDateStr() + "结束重新开卡");
                        return;
                    }

                    //写数据块1，数据块1为用户名（16字节）
                    block = 1;
                    result_WriteIC = CardCommon.WriteIC(icdev, sec, block, HexStringUtility.ByteArrayToHexString(newUserName));
                    if (result_WriteIC != "")
                    {
                        Log("重新写卡扇区" + sec + "数据块" + block + "写入失败");
                        Log(getDateStr() + "结束重新开卡");
                        return;
                    }

                    //写数据块2，数据块2为身份证号（9字节）、联系电话（6字节）
                    block = 2;
                    result_WriteIC = CardCommon.WriteIC(icdev, sec, block, (IdentityNumber + "0" + Telephone).PadRight(32, '0'));
                    if (result_WriteIC != "")
                    {
                        Log("重新写卡扇区" + sec + "数据块" + block + "写入失败");
                        Log(getDateStr() + "结束重新开卡");
                        return;
                    }

                    //写数据块3，密码eeeeeeeeeeee
                    block = 3;
                    CardCommon.WritePWD(icdev, sec, block, getKeyA(), InfoSys.KeyControl, getKeyB());
                }
                else
                {
                    Log("重新写卡扇区" + sec + "数据块" + block + "写入失败");
                    Log(getDateStr() + "结束重新开卡");
                    return;
                }

                //设置扇区2内容
                sec = 2;
                //认证卡密码B
                result_AuthIC = CardCommon.AuthIC(icdev, mode, sec, key);
                AuthLog(InfoSys.CardTypeStrUser, InfoSys.MethodOpenCard, sec, result_AuthIC);
                if (result_AuthIC == InfoSys.StrAuthSuccess)
                {
                    //写数据块0，剩余可用水量（4字节）剩余可用电量（4字节）累计用水量（4字节）累计用电量（4字节）
                    block = 0;
                    double d1 = Tools.StringToDouble(ResidualWater, 0);
                    double d2 = Tools.StringToDouble(ResidualElectric, 0);
                    double d3 = Tools.StringToDouble(TotalWater, 0);
                    double d4 = Tools.StringToDouble(TotalElectric, 0);
                    result_WriteIC = CardCommon.WriteIC(icdev, sec, block,
                        d1.ToString().PadLeft(8, '0') +
                        d2.ToString().PadLeft(8, '0') +
                        d3.ToString().PadLeft(8, '0') +
                        d4.ToString().PadLeft(8, '0'));
                    if (result_WriteIC != "")
                    {
                        Log("重新写卡扇区" + sec + "数据块" + block + "写入失败");
                        Log(getDateStr() + "结束重新开卡");
                        return;
                    }

                    string[] s = { "", "", "", "" };
                    string[] DeviceLists = DeviceList.Split(',');

                    for (int i = 0; i < DeviceLists.Length; i++)
                    {
                        s[i] = DeviceLists[i];
                    }

                    string data = "";
                    //写数据块1，地址码1（7字节）地址码2（7字节）
                    block = 1;
                    data = (s[0] + s[1]).PadRight(32, '0');
                    result_WriteIC = CardCommon.WriteIC(icdev, sec, block, data);
                    if (result_WriteIC != "")
                    {
                        Log("重新写卡扇区" + sec + "数据块" + block + "写入失败");
                        Log(getDateStr() + "结束重新开卡");
                        return;
                    }

                    //写数据块2，地址码1（7字节）地址码2（7字节）
                    block = 2;
                    data = (s[2] + s[3]).PadRight(32, '0');
                    result_WriteIC = CardCommon.WriteIC(icdev, sec, block, data);
                    if (result_WriteIC != "")
                    {
                        Log("重新写卡扇区" + sec + "数据块" + block + "写入失败");
                        Log(getDateStr() + "结束重新开卡");
                        return;
                    }

                    //写数据块3，密码eeeeeeeeeeee
                    block = 3;
                    CardCommon.WritePWD(icdev, sec, block, getKeyA(), InfoSys.KeyControl, getKeyB());
                }

                Log("重新开卡完成");
                Log(getDateStr() + "结束重新开卡");
            }
        }



        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("确定退出程序？", "退出", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                this.timerIC.Stop();
                //Application.Exit();
                Application.ExitThread();
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            //this.timerIC.Stop();
        }

        frmCardNetSet fCardNetSet = null;
        private void button7_Click(object sender, EventArgs e)
        {
            if (!connectIC)
            {
                MessageBox.Show("请等待串口打开成功！");
                return;
            }

            fCardNetSet = new frmCardNetSet(this);
            fCardNetSet.ShowDialog();
            /*
            if (fCardNetSet == null) //如果子窗体为空则创造实例 并显示
            {
                fCardNetSet = new frmCardNetSet(this);
                fCardNetSet.ShowDialog();
            }
            else
            {
                if (fCardNetSet.IsDisposed)//若子窗体关闭 则打开新子窗体 并显示
                {
                    fCardNetSet = new frmCardNetSet(this);
                    fCardNetSet.ShowDialog();
                }
                else
                {
                    fCardNetSet.Activate(); //使子窗体获得焦点
                }
            }
             * */
        }

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            //MessageBox.Show("数据绑定完成！");
            DataGridView curDgv = (DataGridView)sender;
            foreach (DataGridViewRow Row in curDgv.Rows)
            {
                if (Row != null)
                {
                    foreach (DataGridViewCell cell in Row.Cells)
                    {
                        if (curDgv.Columns[cell.ColumnIndex].HeaderText.Equals("是否挂失"))
                        {
                            if (cell.Value.ToString().Equals("1"))
                            {
                                Row.DefaultCellStyle.ForeColor = Color.Blue;
                                Row.DefaultCellStyle.BackColor = Color.OrangeRed;
                            }
                            else
                            {
                            }
                        }
                        
                    }
                }
            }
        }

        frmQuery fQuery = null;
        private void button8_Click(object sender, EventArgs e)
        {
            string SerialNumber = "";
            string UserNo = "";
            string UserName = "";
            string IdentityNumber = "";
            string Telephone = "";

            if (this.dataGridView1.SelectedRows.Count > 0)
            {
                SerialNumber = this.dataGridView1.SelectedRows[0].Cells["SerialNumber"].Value.ToString();
                UserNo = this.dataGridView1.SelectedRows[0].Cells["UserNo"].Value.ToString();
                UserName = this.dataGridView1.SelectedRows[0].Cells["UserName"].Value.ToString();
                IdentityNumber = this.dataGridView1.SelectedRows[0].Cells["IdentityNumber"].Value.ToString();
                Telephone = this.dataGridView1.SelectedRows[0].Cells["Telephone"].Value.ToString();
            }
            else
            {
                MessageBox.Show("请选择一行记录！");
                return;
            }

            fQuery = new frmQuery(UserNo, UserName, IdentityNumber, Telephone);
            fQuery.ShowDialog();
        }
    }
}
