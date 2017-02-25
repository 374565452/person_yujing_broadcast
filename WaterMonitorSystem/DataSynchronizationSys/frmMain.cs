using DBUtility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace DataSynchronizationSys
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        #region 修改控件状态

        public delegate void setButtonEnabledHandler(Button obj, bool flag);
        private void setButtonEnabled(Button obj, bool flag)
        {
            if (obj.InvokeRequired == true)
            {
                setButtonEnabledHandler set = new setButtonEnabledHandler(setButtonEnabled);//委托的方法参数应和SetCalResult一致  
                obj.Invoke(set, new object[] { obj, flag }); //此方法第二参数用于传入方法,代替形参result  
            }
            else
            {
                obj.Enabled = flag;
            }
        }

        public delegate void setTextBoxEnabledHandler(TextBox obj, bool flag);
        private void setTextBoxEnabled(TextBox obj, bool flag)
        {
            if (obj.InvokeRequired == true)
            {
                setTextBoxEnabledHandler set = new setTextBoxEnabledHandler(setTextBoxEnabled);//委托的方法参数应和SetCalResult一致  
                obj.Invoke(set, new object[] { obj, flag }); //此方法第二参数用于传入方法,代替形参result  
            }
            else
            {
                obj.Enabled = flag;
            }
        }

        public delegate void setTextBoxTextHandler(TextBox obj, string str);
        private void setTextBoxText(TextBox obj, string str)
        {
            if (obj.InvokeRequired == true)
            {
                setTextBoxTextHandler set = new setTextBoxTextHandler(setTextBoxText);//委托的方法参数应和SetCalResult一致  
                obj.Invoke(set, new object[] { obj, str }); //此方法第二参数用于传入方法,代替形参result  
            }
            else
            {
                obj.Text = str;
            }
        }

        public delegate void setLabelTextHandler(Label obj, string str);
        private void setLabelText(Label obj, string str)
        {
            if (obj.InvokeRequired == true)
            {
                setLabelTextHandler set = new setLabelTextHandler(setLabelText);//委托的方法参数应和SetCalResult一致  
                obj.Invoke(set, new object[] { obj, str }); //此方法第二参数用于传入方法,代替形参result  
            }
            else
            {
                obj.Text = str;
            }
        }

        public delegate void setLabelForeColorHandler(Label obj, Color c);
        private void setLabelForeColor(Label obj, Color c)
        {
            if (obj.InvokeRequired == true)
            {
                setLabelForeColorHandler set = new setLabelForeColorHandler(setLabelForeColor);//委托的方法参数应和SetCalResult一致  
                obj.Invoke(set, new object[] { obj, c }); //此方法第二参数用于传入方法,代替形参result  
            }
            else
            {
                obj.ForeColor = c;
            }
        }

        public delegate void setCheckBoxCheckedHandler(CheckBox obj, bool flag);
        private void setCheckBoxChecked(CheckBox obj, bool flag)
        {
            if (obj.InvokeRequired == true)
            {
                setCheckBoxCheckedHandler set = new setCheckBoxCheckedHandler(setCheckBoxChecked);//委托的方法参数应和SetCalResult一致  
                obj.Invoke(set, new object[] { obj, flag }); //此方法第二参数用于传入方法,代替形参result  
            }
            else
            {
                obj.Checked = flag;
            }
        }

        public delegate void setCheckBoxEnabledHandler(CheckBox obj, bool flag);
        private void setCheckBoxEnabled(CheckBox obj, bool flag)
        {
            if (obj.InvokeRequired == true)
            {
                setCheckBoxEnabledHandler set = new setCheckBoxEnabledHandler(setCheckBoxEnabled);//委托的方法参数应和SetCalResult一致  
                obj.Invoke(set, new object[] { obj, flag }); //此方法第二参数用于传入方法,代替形参result  
            }
            else
            {
                obj.Enabled = flag;
            }
        }

        public void setBtnEnabled(bool isRun)
        {
            setButtonEnabled(btnStart, !isRun);
            setButtonEnabled(btnStop, isRun);

            setTextBoxEnabled(txtStrConn1, !isRun);
            setTextBoxEnabled(txtStrConn2, !isRun);

            //setCheckBoxEnabled(ckbGroundWater, !isRun);
            //setCheckBoxEnabled(ckbMeteorological, !isRun);
            //setCheckBoxEnabled(ckbSOILMOISTURE, !isRun);
        }

        #endregion

        private void ReadXml()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFullPath + xmlFileName);

            try
            {
                XmlNode NodeNo = doc.SelectSingleNode("/Tables/GroundWater/No");
                XmlNode NodeIs = doc.SelectSingleNode("/Tables/GroundWater/Is");
                GroundWater_No = int.Parse(NodeNo.InnerText.Trim());
                IsGroundWater = NodeIs.InnerText.Trim() == "1";
            }
            catch (Exception ex)
            {
                MessageBox.Show("读 GroundWater 出错！" + Environment.NewLine + ex.Message);
            }

            try
            {
                XmlNode NodeNo = doc.SelectSingleNode("/Tables/Meteorological/No");
                XmlNode NodeIs = doc.SelectSingleNode("/Tables/Meteorological/Is");
                Meteorological_No = int.Parse(NodeNo.InnerText.Trim());
                IsMeteorological = NodeIs.InnerText.Trim() == "1";
            }
            catch (Exception ex)
            {
                MessageBox.Show("读 GroundWater 出错！" + Environment.NewLine + ex.Message);
            }

            try
            {
                XmlNode NodeNo = doc.SelectSingleNode("/Tables/SOILMOISTURE/No");
                XmlNode NodeIs = doc.SelectSingleNode("/Tables/SOILMOISTURE/Is");
                SOILMOISTURE_No = int.Parse(NodeNo.InnerText.Trim());
                IsSOILMOISTURE = NodeIs.InnerText.Trim() == "1";
            }
            catch (Exception ex)
            {
                MessageBox.Show("读 GroundWater 出错！" + Environment.NewLine + ex.Message);
            }
        }

        bool isWrite = false;
        private void WriteXml()
        {
            if (isWrite) return;
            try
            {
                isWrite = true;
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlFullPath + xmlFileName);
                try
                {
                    XmlNode NodeNo = doc.SelectSingleNode("/Tables/GroundWater/No");
                    XmlNode NodeIs = doc.SelectSingleNode("/Tables/GroundWater/Is");
                    NodeNo.InnerText = GroundWater_No.ToString();
                    NodeIs.InnerText = IsGroundWater ? "1" : "0";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("写 GroundWater 出错！" + Environment.NewLine + ex.Message);
                }

                try
                {
                    XmlNode NodeNo = doc.SelectSingleNode("/Tables/Meteorological/No");
                    XmlNode NodeIs = doc.SelectSingleNode("/Tables/Meteorological/Is");
                    NodeNo.InnerText = Meteorological_No.ToString();
                    NodeIs.InnerText = IsMeteorological ? "1" : "0";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("写 GroundWater 出错！" + Environment.NewLine + ex.Message);
                }

                try
                {
                    XmlNode NodeNo = doc.SelectSingleNode("/Tables/SOILMOISTURE/No");
                    XmlNode NodeIs = doc.SelectSingleNode("/Tables/SOILMOISTURE/Is");
                    NodeNo.InnerText = SOILMOISTURE_No.ToString();
                    NodeIs.InnerText = IsSOILMOISTURE ? "1" : "0";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("写 GroundWater 出错！" + Environment.NewLine + ex.Message);
                }

                doc.Save(xmlFullPath + xmlFileName);//保存。
            }
            catch
            {
            }
            finally {
                isWrite = false; ;
            }
        }

        private void CheckXml()
        {
            if (!File.Exists(xmlFullPath + xmlFileName))
            {
                try
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    //创建Xml声明部分，即<?xml version="1.0" encoding="utf-8" ?>
                    xmlDoc.CreateXmlDeclaration("1.0", "utf-8", "yes");

                    //创建根节点
                    XmlNode rootNode = xmlDoc.CreateElement("Tables");

                    try
                    {
                        //创建GroundWater子节点
                        XmlNode Node_GroundWater = xmlDoc.CreateElement("GroundWater");
                        //创建No子节点
                        XmlNode Node_GroundWaterNode_No = xmlDoc.CreateElement("No");
                        Node_GroundWaterNode_No.InnerText = "0";
                        Node_GroundWater.AppendChild(Node_GroundWaterNode_No);
                        //创建Is子节点
                        XmlNode Node_GroundWaterNode_Is = xmlDoc.CreateElement("Is");
                        Node_GroundWaterNode_Is.InnerText = "0";
                        Node_GroundWater.AppendChild(Node_GroundWaterNode_Is);

                        rootNode.AppendChild(Node_GroundWater);
                    }
                    catch { }

                    try
                    {
                        //创建Meteorological子节点
                        XmlNode Node_Meteorological = xmlDoc.CreateElement("Meteorological");
                        //创建No子节点
                        XmlNode Node_Meteorological_No = xmlDoc.CreateElement("No");
                        Node_Meteorological_No.InnerText = "0";
                        Node_Meteorological.AppendChild(Node_Meteorological_No);
                        //创建Is子节点
                        XmlNode Node_Meteorological_Is = xmlDoc.CreateElement("Is");
                        Node_Meteorological_Is.InnerText = "0";
                        Node_Meteorological.AppendChild(Node_Meteorological_Is);

                        rootNode.AppendChild(Node_Meteorological);
                    }
                    catch { }

                    try
                    {
                        //创建SOILMOISTURE子节点
                        XmlNode Node_SOILMOISTURE = xmlDoc.CreateElement("SOILMOISTURE");
                        //创建No子节点
                        XmlNode Node_SOILMOISTURE_No = xmlDoc.CreateElement("No");
                        Node_SOILMOISTURE_No.InnerText = "0";
                        Node_SOILMOISTURE.AppendChild(Node_SOILMOISTURE_No);
                        //创建Is子节点
                        XmlNode Node_SOILMOISTURE_Is = xmlDoc.CreateElement("Is");
                        Node_SOILMOISTURE_Is.InnerText = "0";
                        Node_SOILMOISTURE.AppendChild(Node_SOILMOISTURE_Is);

                        rootNode.AppendChild(Node_SOILMOISTURE);
                    }
                    catch { }

                    //附加根节点
                    xmlDoc.AppendChild(rootNode);

                    //保存Xml文档
                    xmlDoc.Save(xmlFullPath + xmlFileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        bool isRun = false;
        string xmlFileName = "table.xml";
        string xmlFullPath = System.Windows.Forms.Application.StartupPath + "\\";
        int GroundWater_No = 0;
        bool IsGroundWater = false;
        int Meteorological_No = 0;
        bool IsMeteorological = false;
        int SOILMOISTURE_No = 0;
        bool IsSOILMOISTURE = false;

        private void frmMain_Load(object sender, EventArgs e)
        {
            CheckXml();
            ReadXml();

            this.txtStrConn1.Text = "Data Source=.;Initial Catalog=WaterMonitorSystemDB;User ID=sa;Password=Jssl2016";
            this.txtStrConn2.Text = "Data Source=.;Initial Catalog=WR_IC;User ID=sa;Password=Jssl2016";

            setLabelText(this.lblConn1State, "");
            setLabelText(this.lblConn2State, "");
            setLabelText(this.lblRunTime, "");

            setLabelText(this.lblGroundWater_No, GroundWater_No.ToString());
            setCheckBoxChecked(this.ckbGroundWater, IsGroundWater);
            setLabelText(this.lblMeteorological_No, Meteorological_No.ToString());
            setCheckBoxChecked(this.ckbMeteorological, IsMeteorological);
            setLabelText(this.lblSOILMOISTURE_No, SOILMOISTURE_No.ToString());
            setCheckBoxChecked(this.ckbSOILMOISTURE, IsSOILMOISTURE);

            timer1.Start();
           
            isRun = false;
            setBtnEnabled(isRun);
        }

        Thread tMain;
        private void btnStart_Click(object sender, EventArgs e)
        {
            setLabelForeColor(this.lblConn1State, Color.Black);
            setLabelText(this.lblConn1State, "");
            setLabelForeColor(this.lblConn2State, Color.Black);
            setLabelText(this.lblConn2State, "");

            tMain = null;
            tMain = new Thread(new ThreadStart(Start));
            tMain.Start();
        }

        private void Start()
        {
            bool flag1 = false;
            bool flag2 = false;

            setLabelText(this.lblConn1State, "数据库1正在连接。。。");
            try
            {
                DataTable dt = DbHelperSQL.QueryDataTable("select CONVERT(varchar(100), GETDATE(), 121)", this.txtStrConn1.Text.Trim());
                setLabelForeColor(this.lblConn1State, Color.Green);
                setLabelText(this.lblConn1State, "数据库1连接成功！" + dt.Rows[0][0].ToString().Trim());
                flag1 = true;
            }
            catch
            {
                setLabelForeColor(this.lblConn1State, Color.Red);
                setLabelText(this.lblConn1State, "数据库1连接失败！");
            }

            setLabelText(this.lblConn2State, "数据库2正在连接。。。");
            try
            {
                DataTable dt = DbHelperSQL.QueryDataTable("select CONVERT(varchar(100), GETDATE(), 121)", this.txtStrConn2.Text.Trim());
                setLabelForeColor(this.lblConn2State, Color.Green);
                setLabelText(this.lblConn2State, "数据库2连接成功！" + dt.Rows[0][0].ToString().Trim());
                flag2 = true;
            }
            catch
            {
                setLabelForeColor(this.lblConn2State, Color.Red);
                setLabelText(this.lblConn2State, "数据库2连接失败！");
            }

            if (flag1 && flag2)
            {
                isRun = true;

                setBtnEnabled(isRun);

                setLabelText(this.lblRunTime, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                IsGroundWater = this.ckbGroundWater.Checked;
                
                IsMeteorological = this.ckbMeteorological.Checked;
                
                IsSOILMOISTURE = this.ckbSOILMOISTURE.Checked;

                WriteXml();

                int num = 0;
                while (isRun)
                {
                    if (num % (60 * 30) == 0)
                    {
                        if (num > 0) num = 1;

                        IsGroundWater = this.ckbGroundWater.Checked;
                        IsMeteorological = this.ckbMeteorological.Checked;
                        IsSOILMOISTURE = this.ckbSOILMOISTURE.Checked;

                        if (IsGroundWater)
                        {
                            try
                            {
                                DataTable dt = DbHelperSQL.QueryDataTable("SELECT * FROM T_GroundWater where 1=1 and No>'" + GroundWater_No + "' order by No", this.txtStrConn2.Text.Trim());

                                if (dt.Rows.Count > 0)
                                {
                                    DbHelperSQL.SqlBulkCopyByDatatable("T_GroundWater", dt, this.txtStrConn1.Text.Trim());
                                    GroundWater_No = int.Parse(dt.Rows[dt.Rows.Count - 1]["No"].ToString());
                                }

                                setLabelText(this.lblGroundWater_No, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "，" + GroundWater_No.ToString() + "，" + dt.Rows.Count);
                            }
                            catch (Exception ex)
                            {
                                setLabelText(this.lblGroundWater_No, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "，" + ex.Message);
                            }
                        }
                        else
                        {
                            setLabelText(this.lblGroundWater_No, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "，" + "停止同步");
                        }

                        if (IsMeteorological)
                        {
                            try
                            {
                                DataTable dt = DbHelperSQL.QueryDataTable("SELECT * FROM T_Meteorological where 1=1 and No>'" + Meteorological_No + "' order by No", this.txtStrConn2.Text.Trim());

                                if (dt.Rows.Count > 0)
                                {
                                    DbHelperSQL.SqlBulkCopyByDatatable("T_Meteorological", dt, this.txtStrConn1.Text.Trim());
                                    Meteorological_No = int.Parse(dt.Rows[dt.Rows.Count - 1]["No"].ToString());
                                }

                                setLabelText(this.lblMeteorological_No, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "，" + Meteorological_No.ToString() + "，" + dt.Rows.Count);
                            }
                            catch (Exception ex)
                            {
                                setLabelText(this.lblMeteorological_No, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "，" + ex.Message);
                            }
                        }
                        else
                        {
                            setLabelText(this.lblMeteorological_No, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "，" + "停止同步");
                        }

                        if (IsSOILMOISTURE)
                        {
                            try
                            {
                                DataTable dt = DbHelperSQL.QueryDataTable("SELECT * FROM T_SOILMOISTURE where 1=1 and No>'" + SOILMOISTURE_No + "' order by No", this.txtStrConn2.Text.Trim());

                                if (dt.Rows.Count > 0)
                                {
                                    DbHelperSQL.SqlBulkCopyByDatatable("T_SOILMOISTURE", dt, this.txtStrConn1.Text.Trim());
                                    SOILMOISTURE_No = int.Parse(dt.Rows[dt.Rows.Count - 1]["No"].ToString());
                                }

                                setLabelText(this.lblSOILMOISTURE_No, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "，" + SOILMOISTURE_No.ToString() + "，" + dt.Rows.Count);
                            }
                            catch (Exception ex)
                            {
                                setLabelText(this.lblSOILMOISTURE_No, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "，" + ex.Message);
                            }
                        }
                        else
                        {
                            setLabelText(this.lblSOILMOISTURE_No, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "，" + "停止同步");
                        }

                        WriteXml();
                    }

                    num++;
                    Thread.Sleep(1000);
                }
            }
        }


        private void btnStop_Click(object sender, EventArgs e)
        {
            isRun = false;
            setBtnEnabled(isRun);

            setLabelText(this.lblRunTime, "--");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            setLabelText(this.lblCurrentTime, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            isRun = false;
            timer1.Stop();
            
        }

        Random rd = new Random();
        private void timer2_Tick(object sender, EventArgs e)
        {
            
            try
            {
                DataTable dt = DbHelperSQL.QueryDataTable("SELECT top 1 * FROM T_GroundWater order by Acq_Time desc", this.txtStrConn1.Text.Trim());

                DateTime Acq_Time = DateTime.Parse(dt.Rows[0]["Acq_Time"].ToString());
                double GroundWaterLevel = double.Parse(dt.Rows[0]["GroundWaterLevel"].ToString());
                double GroundWaterTempture = double.Parse(dt.Rows[0]["GroundWaterLevel"].ToString());
                double BV = double.Parse(dt.Rows[0]["BV"].ToString());
                DateTime CREATE_TIME = DateTime.Now;

                if (Acq_Time.AddHours(5) < DateTime.Now)
                {
                    if (Acq_Time.Hour == 8)
                    {
                        BV = rd.Next(1030, 1070) / 100.0;
                    }

                    int i1 = -10;
                    try
                    {
                        i1 = int.Parse(this.textBox1.Text.Trim());
                    }
                    catch (Exception ex)
                    { }
                    int i2 = 10;
                    try
                    {
                        i2 = int.Parse(this.textBox2.Text.Trim());
                    }
                    catch (Exception ex)
                    { }
                    GroundWaterTempture = rd.Next(1010, 1050) / 100.0;
                    GroundWaterLevel += rd.Next(i1, i2) / 100.0;

                    string sql = "INSERT INTO [T_GroundWater] VALUES('0010000001','" + GroundWaterLevel + "','0','" + GroundWaterTempture + 
                        "','" + BV + "','" + Acq_Time.AddHours(4).ToString("yyyy-MM-dd HH:mm:ss") +
                        "','" + CREATE_TIME.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                    DbHelperSQL.ExecuteSql(sql, this.txtStrConn1.Text.Trim());
                }
            }
            catch { }

        }

    }
}
