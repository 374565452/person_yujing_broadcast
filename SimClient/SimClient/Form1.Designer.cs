namespace SimClient
{
    partial class Form1
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
            if (clientSocket != null)
            {
                clientSocket.Dispose();
            }
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
            this.button6 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.button7 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtIP = new System.Windows.Forms.TextBox();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtCenterStation = new System.Windows.Forms.TextBox();
            this.txtRemoteStation = new System.Windows.Forms.TextBox();
            this.txtPW = new System.Windows.Forms.TextBox();
            this.button9 = new System.Windows.Forms.Button();
            this.button10 = new System.Windows.Forms.Button();
            this.txtAlarm_01_1 = new System.Windows.Forms.TextBox();
            this.txtAlarm_01_2 = new System.Windows.Forms.TextBox();
            this.txtAlarm_01_3 = new System.Windows.Forms.TextBox();
            this.txtAlarm_03_1 = new System.Windows.Forms.TextBox();
            this.txtAlarm_03_2 = new System.Windows.Forms.TextBox();
            this.txtAlarm_03_3 = new System.Windows.Forms.TextBox();
            this.txtAlarm_06_1 = new System.Windows.Forms.TextBox();
            this.txtAlarm_06_2 = new System.Windows.Forms.TextBox();
            this.txtAlarm_06_3 = new System.Windows.Forms.TextBox();
            this.txtAlarm_12_1 = new System.Windows.Forms.TextBox();
            this.txtAlarm_12_2 = new System.Windows.Forms.TextBox();
            this.txtAlarm_12_3 = new System.Windows.Forms.TextBox();
            this.txtPhone_01 = new System.Windows.Forms.TextBox();
            this.txtPhone_02 = new System.Windows.Forms.TextBox();
            this.txtPhone_03 = new System.Windows.Forms.TextBox();
            this.txtPhone_04 = new System.Windows.Forms.TextBox();
            this.txtPhone_05 = new System.Windows.Forms.TextBox();
            this.txtPhone_10 = new System.Windows.Forms.TextBox();
            this.txtPhone_09 = new System.Windows.Forms.TextBox();
            this.txtPhone_08 = new System.Windows.Forms.TextBox();
            this.txtPhone_07 = new System.Windows.Forms.TextBox();
            this.txtPhone_06 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(696, 138);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(75, 23);
            this.button6.TabIndex = 60;
            this.button6.Text = "ClearLog";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(615, 138);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(75, 23);
            this.button5.TabIndex = 59;
            this.button5.Text = "ClearMsg";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(255, 138);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 58;
            this.button4.Text = "校验";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(174, 138);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 57;
            this.button3.Text = "发送";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(93, 138);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 56;
            this.button2.Text = "断开连接";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 138);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 55;
            this.button1.Text = "连接";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(12, 167);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(760, 106);
            this.textBox1.TabIndex = 62;
            // 
            // txtLog
            // 
            this.txtLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLog.Location = new System.Drawing.Point(12, 279);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(760, 370);
            this.txtLog.TabIndex = 63;
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(336, 138);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(75, 23);
            this.button7.TabIndex = 64;
            this.button7.Text = "链路维持报";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(417, 138);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(75, 23);
            this.button8.TabIndex = 65;
            this.button8.Text = "图片代码";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 35;
            this.label1.Text = "IP：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(185, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 36;
            this.label2.Text = "端口：";
            // 
            // txtIP
            // 
            this.txtIP.Location = new System.Drawing.Point(52, 12);
            this.txtIP.Name = "txtIP";
            this.txtIP.Size = new System.Drawing.Size(111, 21);
            this.txtIP.TabIndex = 37;
            this.txtIP.Text = "127.0.0.1";
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(232, 12);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(51, 21);
            this.txtPort.TabIndex = 38;
            this.txtPort.Text = "6001";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(307, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 49;
            this.label3.Text = "中心站：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(427, 15);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 50;
            this.label4.Text = "遥测站：";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(591, 15);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 12);
            this.label5.TabIndex = 51;
            this.label5.Text = "密码：";
            // 
            // txtCenterStation
            // 
            this.txtCenterStation.Location = new System.Drawing.Point(363, 12);
            this.txtCenterStation.Name = "txtCenterStation";
            this.txtCenterStation.Size = new System.Drawing.Size(43, 21);
            this.txtCenterStation.TabIndex = 52;
            this.txtCenterStation.Text = "16";
            // 
            // txtRemoteStation
            // 
            this.txtRemoteStation.Location = new System.Drawing.Point(486, 12);
            this.txtRemoteStation.Name = "txtRemoteStation";
            this.txtRemoteStation.Size = new System.Drawing.Size(84, 21);
            this.txtRemoteStation.TabIndex = 53;
            this.txtRemoteStation.Text = "0000000001";
            // 
            // txtPW
            // 
            this.txtPW.Location = new System.Drawing.Point(638, 12);
            this.txtPW.Name = "txtPW";
            this.txtPW.Size = new System.Drawing.Size(41, 21);
            this.txtPW.TabIndex = 54;
            this.txtPW.Text = "1234";
            // 
            // button9
            // 
            this.button9.Location = new System.Drawing.Point(615, 109);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(75, 23);
            this.button9.TabIndex = 66;
            this.button9.Text = "基本参数";
            this.button9.UseVisualStyleBackColor = true;
            this.button9.Click += new System.EventHandler(this.button9_Click);
            // 
            // button10
            // 
            this.button10.Location = new System.Drawing.Point(696, 109);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(75, 23);
            this.button10.TabIndex = 67;
            this.button10.Text = "运行参数";
            this.button10.UseVisualStyleBackColor = true;
            this.button10.Click += new System.EventHandler(this.button10_Click);
            // 
            // txtAlarm_01_1
            // 
            this.txtAlarm_01_1.Location = new System.Drawing.Point(19, 44);
            this.txtAlarm_01_1.Name = "txtAlarm_01_1";
            this.txtAlarm_01_1.Size = new System.Drawing.Size(40, 21);
            this.txtAlarm_01_1.TabIndex = 68;
            this.txtAlarm_01_1.Text = "2";
            // 
            // txtAlarm_01_2
            // 
            this.txtAlarm_01_2.Location = new System.Drawing.Point(65, 44);
            this.txtAlarm_01_2.Name = "txtAlarm_01_2";
            this.txtAlarm_01_2.Size = new System.Drawing.Size(40, 21);
            this.txtAlarm_01_2.TabIndex = 69;
            this.txtAlarm_01_2.Text = "5";
            // 
            // txtAlarm_01_3
            // 
            this.txtAlarm_01_3.Location = new System.Drawing.Point(111, 44);
            this.txtAlarm_01_3.Name = "txtAlarm_01_3";
            this.txtAlarm_01_3.Size = new System.Drawing.Size(40, 21);
            this.txtAlarm_01_3.TabIndex = 70;
            this.txtAlarm_01_3.Text = "8";
            // 
            // txtAlarm_03_1
            // 
            this.txtAlarm_03_1.Location = new System.Drawing.Point(157, 44);
            this.txtAlarm_03_1.Name = "txtAlarm_03_1";
            this.txtAlarm_03_1.Size = new System.Drawing.Size(40, 21);
            this.txtAlarm_03_1.TabIndex = 71;
            this.txtAlarm_03_1.Text = "3";
            // 
            // txtAlarm_03_2
            // 
            this.txtAlarm_03_2.Location = new System.Drawing.Point(203, 44);
            this.txtAlarm_03_2.Name = "txtAlarm_03_2";
            this.txtAlarm_03_2.Size = new System.Drawing.Size(40, 21);
            this.txtAlarm_03_2.TabIndex = 72;
            this.txtAlarm_03_2.Text = "6";
            // 
            // txtAlarm_03_3
            // 
            this.txtAlarm_03_3.Location = new System.Drawing.Point(249, 44);
            this.txtAlarm_03_3.Name = "txtAlarm_03_3";
            this.txtAlarm_03_3.Size = new System.Drawing.Size(40, 21);
            this.txtAlarm_03_3.TabIndex = 73;
            this.txtAlarm_03_3.Text = "9";
            // 
            // txtAlarm_06_1
            // 
            this.txtAlarm_06_1.Location = new System.Drawing.Point(295, 44);
            this.txtAlarm_06_1.Name = "txtAlarm_06_1";
            this.txtAlarm_06_1.Size = new System.Drawing.Size(40, 21);
            this.txtAlarm_06_1.TabIndex = 74;
            this.txtAlarm_06_1.Text = "4";
            // 
            // txtAlarm_06_2
            // 
            this.txtAlarm_06_2.Location = new System.Drawing.Point(341, 44);
            this.txtAlarm_06_2.Name = "txtAlarm_06_2";
            this.txtAlarm_06_2.Size = new System.Drawing.Size(40, 21);
            this.txtAlarm_06_2.TabIndex = 75;
            this.txtAlarm_06_2.Text = "7";
            // 
            // txtAlarm_06_3
            // 
            this.txtAlarm_06_3.Location = new System.Drawing.Point(387, 44);
            this.txtAlarm_06_3.Name = "txtAlarm_06_3";
            this.txtAlarm_06_3.Size = new System.Drawing.Size(40, 21);
            this.txtAlarm_06_3.TabIndex = 76;
            this.txtAlarm_06_3.Text = "10";
            // 
            // txtAlarm_12_1
            // 
            this.txtAlarm_12_1.Location = new System.Drawing.Point(433, 44);
            this.txtAlarm_12_1.Name = "txtAlarm_12_1";
            this.txtAlarm_12_1.Size = new System.Drawing.Size(40, 21);
            this.txtAlarm_12_1.TabIndex = 77;
            this.txtAlarm_12_1.Text = "5";
            // 
            // txtAlarm_12_2
            // 
            this.txtAlarm_12_2.Location = new System.Drawing.Point(479, 44);
            this.txtAlarm_12_2.Name = "txtAlarm_12_2";
            this.txtAlarm_12_2.Size = new System.Drawing.Size(40, 21);
            this.txtAlarm_12_2.TabIndex = 78;
            this.txtAlarm_12_2.Text = "8";
            // 
            // txtAlarm_12_3
            // 
            this.txtAlarm_12_3.Location = new System.Drawing.Point(525, 44);
            this.txtAlarm_12_3.Name = "txtAlarm_12_3";
            this.txtAlarm_12_3.Size = new System.Drawing.Size(40, 21);
            this.txtAlarm_12_3.TabIndex = 79;
            this.txtAlarm_12_3.Text = "11";
            // 
            // txtPhone_01
            // 
            this.txtPhone_01.Location = new System.Drawing.Point(19, 71);
            this.txtPhone_01.Name = "txtPhone_01";
            this.txtPhone_01.Size = new System.Drawing.Size(100, 21);
            this.txtPhone_01.TabIndex = 80;
            this.txtPhone_01.Text = "13112345678";
            // 
            // txtPhone_02
            // 
            this.txtPhone_02.Location = new System.Drawing.Point(126, 71);
            this.txtPhone_02.Name = "txtPhone_02";
            this.txtPhone_02.Size = new System.Drawing.Size(100, 21);
            this.txtPhone_02.TabIndex = 81;
            // 
            // txtPhone_03
            // 
            this.txtPhone_03.Location = new System.Drawing.Point(232, 71);
            this.txtPhone_03.Name = "txtPhone_03";
            this.txtPhone_03.Size = new System.Drawing.Size(100, 21);
            this.txtPhone_03.TabIndex = 82;
            this.txtPhone_03.Text = "13187654321";
            // 
            // txtPhone_04
            // 
            this.txtPhone_04.Location = new System.Drawing.Point(338, 71);
            this.txtPhone_04.Name = "txtPhone_04";
            this.txtPhone_04.Size = new System.Drawing.Size(100, 21);
            this.txtPhone_04.TabIndex = 83;
            this.txtPhone_04.Text = "2345";
            // 
            // txtPhone_05
            // 
            this.txtPhone_05.Location = new System.Drawing.Point(444, 71);
            this.txtPhone_05.Name = "txtPhone_05";
            this.txtPhone_05.Size = new System.Drawing.Size(100, 21);
            this.txtPhone_05.TabIndex = 84;
            this.txtPhone_05.Text = "13685130000";
            // 
            // txtPhone_10
            // 
            this.txtPhone_10.Location = new System.Drawing.Point(445, 98);
            this.txtPhone_10.Name = "txtPhone_10";
            this.txtPhone_10.Size = new System.Drawing.Size(100, 21);
            this.txtPhone_10.TabIndex = 89;
            // 
            // txtPhone_09
            // 
            this.txtPhone_09.Location = new System.Drawing.Point(339, 98);
            this.txtPhone_09.Name = "txtPhone_09";
            this.txtPhone_09.Size = new System.Drawing.Size(100, 21);
            this.txtPhone_09.TabIndex = 88;
            // 
            // txtPhone_08
            // 
            this.txtPhone_08.Location = new System.Drawing.Point(233, 98);
            this.txtPhone_08.Name = "txtPhone_08";
            this.txtPhone_08.Size = new System.Drawing.Size(100, 21);
            this.txtPhone_08.TabIndex = 87;
            // 
            // txtPhone_07
            // 
            this.txtPhone_07.Location = new System.Drawing.Point(127, 98);
            this.txtPhone_07.Name = "txtPhone_07";
            this.txtPhone_07.Size = new System.Drawing.Size(100, 21);
            this.txtPhone_07.TabIndex = 86;
            // 
            // txtPhone_06
            // 
            this.txtPhone_06.Location = new System.Drawing.Point(20, 98);
            this.txtPhone_06.Name = "txtPhone_06";
            this.txtPhone_06.Size = new System.Drawing.Size(100, 21);
            this.txtPhone_06.TabIndex = 85;
            this.txtPhone_06.Text = "18111223344";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(614, 71);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(65, 21);
            this.textBox2.TabIndex = 90;
            this.textBox2.Text = "1";
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(685, 71);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(65, 21);
            this.textBox3.TabIndex = 91;
            this.textBox3.Text = "1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 661);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.txtPhone_10);
            this.Controls.Add(this.txtPhone_09);
            this.Controls.Add(this.txtPhone_08);
            this.Controls.Add(this.txtPhone_07);
            this.Controls.Add(this.txtPhone_06);
            this.Controls.Add(this.txtPhone_05);
            this.Controls.Add(this.txtPhone_04);
            this.Controls.Add(this.txtPhone_03);
            this.Controls.Add(this.txtPhone_02);
            this.Controls.Add(this.txtPhone_01);
            this.Controls.Add(this.txtAlarm_12_3);
            this.Controls.Add(this.txtAlarm_12_2);
            this.Controls.Add(this.txtAlarm_12_1);
            this.Controls.Add(this.txtAlarm_06_3);
            this.Controls.Add(this.txtAlarm_06_2);
            this.Controls.Add(this.txtAlarm_06_1);
            this.Controls.Add(this.txtAlarm_03_3);
            this.Controls.Add(this.txtAlarm_03_2);
            this.Controls.Add(this.txtAlarm_03_1);
            this.Controls.Add(this.txtAlarm_01_3);
            this.Controls.Add(this.txtAlarm_01_2);
            this.Controls.Add(this.txtAlarm_01_1);
            this.Controls.Add(this.button10);
            this.Controls.Add(this.button9);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txtPW);
            this.Controls.Add(this.txtRemoteStation);
            this.Controls.Add(this.txtCenterStation);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtPort);
            this.Controls.Add(this.txtIP);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "模拟终端";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtIP;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtCenterStation;
        private System.Windows.Forms.TextBox txtRemoteStation;
        private System.Windows.Forms.TextBox txtPW;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.Button button10;
        private System.Windows.Forms.TextBox txtAlarm_01_1;
        private System.Windows.Forms.TextBox txtAlarm_01_2;
        private System.Windows.Forms.TextBox txtAlarm_01_3;
        private System.Windows.Forms.TextBox txtAlarm_03_1;
        private System.Windows.Forms.TextBox txtAlarm_03_2;
        private System.Windows.Forms.TextBox txtAlarm_03_3;
        private System.Windows.Forms.TextBox txtAlarm_06_1;
        private System.Windows.Forms.TextBox txtAlarm_06_2;
        private System.Windows.Forms.TextBox txtAlarm_06_3;
        private System.Windows.Forms.TextBox txtAlarm_12_1;
        private System.Windows.Forms.TextBox txtAlarm_12_2;
        private System.Windows.Forms.TextBox txtAlarm_12_3;
        private System.Windows.Forms.TextBox txtPhone_01;
        private System.Windows.Forms.TextBox txtPhone_02;
        private System.Windows.Forms.TextBox txtPhone_03;
        private System.Windows.Forms.TextBox txtPhone_04;
        private System.Windows.Forms.TextBox txtPhone_05;
        private System.Windows.Forms.TextBox txtPhone_10;
        private System.Windows.Forms.TextBox txtPhone_09;
        private System.Windows.Forms.TextBox txtPhone_08;
        private System.Windows.Forms.TextBox txtPhone_07;
        private System.Windows.Forms.TextBox txtPhone_06;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
    }
}

