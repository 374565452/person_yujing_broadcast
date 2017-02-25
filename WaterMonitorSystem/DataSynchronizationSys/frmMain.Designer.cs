namespace DataSynchronizationSys
{
    partial class frmMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.ckbSOILMOISTURE = new System.Windows.Forms.CheckBox();
            this.ckbMeteorological = new System.Windows.Forms.CheckBox();
            this.ckbGroundWater = new System.Windows.Forms.CheckBox();
            this.lblSOILMOISTURE_No = new System.Windows.Forms.Label();
            this.lblMeteorological_No = new System.Windows.Forms.Label();
            this.lblGroundWater_No = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblConn2State = new System.Windows.Forms.Label();
            this.lblConn1State = new System.Windows.Forms.Label();
            this.lblRunTime = new System.Windows.Forms.Label();
            this.lblCurrentTime = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txtStrConn2 = new System.Windows.Forms.TextBox();
            this.txtStrConn1 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.label8 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(684, 461);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.label15);
            this.tabPage3.Controls.Add(this.label14);
            this.tabPage3.Controls.Add(this.label13);
            this.tabPage3.Controls.Add(this.textBox2);
            this.tabPage3.Controls.Add(this.textBox1);
            this.tabPage3.Controls.Add(this.label8);
            this.tabPage3.Controls.Add(this.ckbSOILMOISTURE);
            this.tabPage3.Controls.Add(this.ckbMeteorological);
            this.tabPage3.Controls.Add(this.ckbGroundWater);
            this.tabPage3.Controls.Add(this.lblSOILMOISTURE_No);
            this.tabPage3.Controls.Add(this.lblMeteorological_No);
            this.tabPage3.Controls.Add(this.lblGroundWater_No);
            this.tabPage3.Controls.Add(this.label5);
            this.tabPage3.Controls.Add(this.label4);
            this.tabPage3.Controls.Add(this.label3);
            this.tabPage3.Controls.Add(this.lblConn2State);
            this.tabPage3.Controls.Add(this.lblConn1State);
            this.tabPage3.Controls.Add(this.lblRunTime);
            this.tabPage3.Controls.Add(this.lblCurrentTime);
            this.tabPage3.Controls.Add(this.label12);
            this.tabPage3.Controls.Add(this.label11);
            this.tabPage3.Controls.Add(this.label10);
            this.tabPage3.Controls.Add(this.label9);
            this.tabPage3.Controls.Add(this.btnStop);
            this.tabPage3.Controls.Add(this.btnStart);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(676, 435);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "启动";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // ckbSOILMOISTURE
            // 
            this.ckbSOILMOISTURE.AutoSize = true;
            this.ckbSOILMOISTURE.Location = new System.Drawing.Point(152, 361);
            this.ckbSOILMOISTURE.Name = "ckbSOILMOISTURE";
            this.ckbSOILMOISTURE.Size = new System.Drawing.Size(120, 16);
            this.ckbSOILMOISTURE.TabIndex = 24;
            this.ckbSOILMOISTURE.Text = "同步SOILMOISTURE";
            this.ckbSOILMOISTURE.UseVisualStyleBackColor = true;
            // 
            // ckbMeteorological
            // 
            this.ckbMeteorological.AutoSize = true;
            this.ckbMeteorological.Location = new System.Drawing.Point(152, 297);
            this.ckbMeteorological.Name = "ckbMeteorological";
            this.ckbMeteorological.Size = new System.Drawing.Size(132, 16);
            this.ckbMeteorological.TabIndex = 23;
            this.ckbMeteorological.Text = "同步Meteorological";
            this.ckbMeteorological.UseVisualStyleBackColor = true;
            // 
            // ckbGroundWater
            // 
            this.ckbGroundWater.AutoSize = true;
            this.ckbGroundWater.Location = new System.Drawing.Point(152, 234);
            this.ckbGroundWater.Name = "ckbGroundWater";
            this.ckbGroundWater.Size = new System.Drawing.Size(114, 16);
            this.ckbGroundWater.TabIndex = 22;
            this.ckbGroundWater.Text = "同步GroundWater";
            this.ckbGroundWater.UseVisualStyleBackColor = true;
            // 
            // lblSOILMOISTURE_No
            // 
            this.lblSOILMOISTURE_No.AutoSize = true;
            this.lblSOILMOISTURE_No.Location = new System.Drawing.Point(150, 331);
            this.lblSOILMOISTURE_No.Name = "lblSOILMOISTURE_No";
            this.lblSOILMOISTURE_No.Size = new System.Drawing.Size(113, 12);
            this.lblSOILMOISTURE_No.TabIndex = 21;
            this.lblSOILMOISTURE_No.Text = "lblSOILMOISTURE_No";
            // 
            // lblMeteorological_No
            // 
            this.lblMeteorological_No.AutoSize = true;
            this.lblMeteorological_No.Location = new System.Drawing.Point(150, 269);
            this.lblMeteorological_No.Name = "lblMeteorological_No";
            this.lblMeteorological_No.Size = new System.Drawing.Size(125, 12);
            this.lblMeteorological_No.TabIndex = 19;
            this.lblMeteorological_No.Text = "lblMeteorological_No";
            // 
            // lblGroundWater_No
            // 
            this.lblGroundWater_No.AutoSize = true;
            this.lblGroundWater_No.Location = new System.Drawing.Point(150, 208);
            this.lblGroundWater_No.Name = "lblGroundWater_No";
            this.lblGroundWater_No.Size = new System.Drawing.Size(107, 12);
            this.lblGroundWater_No.TabIndex = 17;
            this.lblGroundWater_No.Text = "lblGroundWater_No";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(34, 331);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(101, 12);
            this.label5.TabIndex = 16;
            this.label5.Text = "T_SOILMOISTURE：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(22, 269);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(113, 12);
            this.label4.TabIndex = 15;
            this.label4.Text = "T_Meteorological：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(40, 208);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(95, 12);
            this.label3.TabIndex = 14;
            this.label3.Text = "T_GroundWater：";
            // 
            // lblConn2State
            // 
            this.lblConn2State.AutoSize = true;
            this.lblConn2State.Location = new System.Drawing.Point(150, 167);
            this.lblConn2State.Name = "lblConn2State";
            this.lblConn2State.Size = new System.Drawing.Size(83, 12);
            this.lblConn2State.TabIndex = 13;
            this.lblConn2State.Text = "lblConn2State";
            // 
            // lblConn1State
            // 
            this.lblConn1State.AutoSize = true;
            this.lblConn1State.Location = new System.Drawing.Point(150, 140);
            this.lblConn1State.Name = "lblConn1State";
            this.lblConn1State.Size = new System.Drawing.Size(83, 12);
            this.lblConn1State.TabIndex = 12;
            this.lblConn1State.Text = "lblConn1State";
            // 
            // lblRunTime
            // 
            this.lblRunTime.AutoSize = true;
            this.lblRunTime.Location = new System.Drawing.Point(448, 103);
            this.lblRunTime.Name = "lblRunTime";
            this.lblRunTime.Size = new System.Drawing.Size(65, 12);
            this.lblRunTime.TabIndex = 11;
            this.lblRunTime.Text = "lblRunTime";
            // 
            // lblCurrentTime
            // 
            this.lblCurrentTime.AutoSize = true;
            this.lblCurrentTime.Location = new System.Drawing.Point(150, 103);
            this.lblCurrentTime.Name = "lblCurrentTime";
            this.lblCurrentTime.Size = new System.Drawing.Size(89, 12);
            this.lblCurrentTime.TabIndex = 10;
            this.lblCurrentTime.Text = "lblCurrentTime";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(52, 167);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(83, 12);
            this.label12.TabIndex = 9;
            this.label12.Text = "数据库2状态：";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(52, 140);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(83, 12);
            this.label11.TabIndex = 8;
            this.label11.Text = "数据库1状态：";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(368, 103);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(65, 12);
            this.label10.TabIndex = 7;
            this.label10.Text = "运行时间：";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(70, 103);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(65, 12);
            this.label9.TabIndex = 6;
            this.label9.Text = "当前时间：";
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(164, 52);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 1;
            this.btnStop.Text = "停止";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(60, 52);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "运行";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.label7);
            this.tabPage2.Controls.Add(this.label6);
            this.tabPage2.Controls.Add(this.txtStrConn2);
            this.tabPage2.Controls.Add(this.txtStrConn1);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(676, 435);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "数据库配置";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(67, 86);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(41, 12);
            this.label7.TabIndex = 5;
            this.label7.Text = "备份至";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(91, 25);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(17, 12);
            this.label6.TabIndex = 4;
            this.label6.Text = "从";
            // 
            // txtStrConn2
            // 
            this.txtStrConn2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtStrConn2.Location = new System.Drawing.Point(114, 52);
            this.txtStrConn2.Name = "txtStrConn2";
            this.txtStrConn2.Size = new System.Drawing.Size(518, 21);
            this.txtStrConn2.TabIndex = 3;
            this.txtStrConn2.Text = "Data Source=.;Initial Catalog=WR_IC;User ID=sa;Password=Jssl2016";
            // 
            // txtStrConn1
            // 
            this.txtStrConn1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtStrConn1.Location = new System.Drawing.Point(114, 113);
            this.txtStrConn1.Name = "txtStrConn1";
            this.txtStrConn1.Size = new System.Drawing.Size(518, 21);
            this.txtStrConn1.TabIndex = 2;
            this.txtStrConn1.Text = "Data Source=.;Initial Catalog=WaterMonitorSystemDB;User ID=sa;Password=Jssl2016";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(55, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "数据库：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(55, 116);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "数据库：";
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // timer2
            // 
            this.timer2.Enabled = true;
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(380, 205);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 12);
            this.label8.TabIndex = 25;
            this.label8.Text = "上下限：";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(439, 199);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 21);
            this.textBox1.TabIndex = 26;
            this.textBox1.Text = "-10";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(439, 226);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(100, 21);
            this.textBox2.TabIndex = 27;
            this.textBox2.Text = "10";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(545, 205);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(107, 12);
            this.label13.TabIndex = 28;
            this.label13.Text = "下限整数，默认-10";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(545, 229);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(101, 12);
            this.label14.TabIndex = 29;
            this.label14.Text = "上限整数，默认10";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(437, 259);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(137, 12);
            this.label15.TabIndex = 30;
            this.label15.Text = "地下水位比上次浮动数据";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 461);
            this.Controls.Add(this.tabControl1);
            this.MinimumSize = new System.Drawing.Size(700, 500);
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "数据同步";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMain_FormClosed);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TextBox txtStrConn2;
        private System.Windows.Forms.TextBox txtStrConn1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label lblConn2State;
        private System.Windows.Forms.Label lblConn1State;
        private System.Windows.Forms.Label lblRunTime;
        private System.Windows.Forms.Label lblCurrentTime;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox ckbSOILMOISTURE;
        private System.Windows.Forms.CheckBox ckbMeteorological;
        private System.Windows.Forms.CheckBox ckbGroundWater;
        private System.Windows.Forms.Label lblSOILMOISTURE_No;
        private System.Windows.Forms.Label lblMeteorological_No;
        private System.Windows.Forms.Label lblGroundWater_No;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label8;
    }
}

