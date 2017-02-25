namespace CardOperationSystem
{
    partial class frmCardDevice
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnCancel = new System.Windows.Forms.Button();
            this.cboDeviceList = new System.Windows.Forms.ComboBox();
            this.label45 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.lbState = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.label18 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.cboTypeCode = new System.Windows.Forms.ComboBox();
            this.txtAlertWaterLevel = new System.Windows.Forms.TextBox();
            this.txtMeterPulse = new System.Windows.Forms.TextBox();
            this.txtAlertAvailableWater = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.txtYearExploitation = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.txtAddressCode3 = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtAddressCode2 = new System.Windows.Forms.TextBox();
            this.txtAddressCode1 = new System.Windows.Forms.TextBox();
            this.lbSerialNumber = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.lbCardType = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label22 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.txtAlertAvailableElectric = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.txtStationCode = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(380, 299);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 132;
            this.btnCancel.Text = "注销卡";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // cboDeviceList
            // 
            this.cboDeviceList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDeviceList.FormattingEnabled = true;
            this.cboDeviceList.IntegralHeight = false;
            this.cboDeviceList.Location = new System.Drawing.Point(139, 36);
            this.cboDeviceList.MaxDropDownItems = 20;
            this.cboDeviceList.Name = "cboDeviceList";
            this.cboDeviceList.Size = new System.Drawing.Size(358, 20);
            this.cboDeviceList.TabIndex = 131;
            this.cboDeviceList.SelectedIndexChanged += new System.EventHandler(this.cboDeviceList_SelectedIndexChanged);
            // 
            // label45
            // 
            this.label45.AutoSize = true;
            this.label45.Location = new System.Drawing.Point(66, 39);
            this.label45.Name = "label45";
            this.label45.Size = new System.Drawing.Size(65, 12);
            this.label45.TabIndex = 130;
            this.label45.Text = "选择终端：";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(218, 299);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 129;
            this.button3.Text = "修改卡";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // lbState
            // 
            this.lbState.AutoSize = true;
            this.lbState.Location = new System.Drawing.Point(137, 335);
            this.lbState.Name = "lbState";
            this.lbState.Size = new System.Drawing.Size(29, 12);
            this.lbState.TabIndex = 128;
            this.lbState.Text = "状态";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(299, 299);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 127;
            this.button2.Text = "开卡";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(137, 299);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 126;
            this.button1.Text = "读卡";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(395, 218);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(71, 12);
            this.label18.TabIndex = 125;
            this.label18.Text = "0-9999999.9";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(135, 218);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(41, 12);
            this.label17.TabIndex = 124;
            this.label17.Text = "0-9999";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(135, 177);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(47, 12);
            this.label15.TabIndex = 122;
            this.label15.Text = "0-65535";
            // 
            // cboTypeCode
            // 
            this.cboTypeCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTypeCode.FormattingEnabled = true;
            this.cboTypeCode.IntegralHeight = false;
            this.cboTypeCode.Location = new System.Drawing.Point(139, 237);
            this.cboTypeCode.MaxDropDownItems = 10;
            this.cboTypeCode.Name = "cboTypeCode";
            this.cboTypeCode.Size = new System.Drawing.Size(121, 20);
            this.cboTypeCode.TabIndex = 121;
            // 
            // txtAlertWaterLevel
            // 
            this.txtAlertWaterLevel.Location = new System.Drawing.Point(397, 194);
            this.txtAlertWaterLevel.Name = "txtAlertWaterLevel";
            this.txtAlertWaterLevel.Size = new System.Drawing.Size(100, 21);
            this.txtAlertWaterLevel.TabIndex = 120;
            // 
            // txtMeterPulse
            // 
            this.txtMeterPulse.Location = new System.Drawing.Point(137, 194);
            this.txtMeterPulse.Name = "txtMeterPulse";
            this.txtMeterPulse.Size = new System.Drawing.Size(100, 21);
            this.txtMeterPulse.TabIndex = 119;
            // 
            // txtAlertAvailableWater
            // 
            this.txtAlertAvailableWater.Location = new System.Drawing.Point(137, 153);
            this.txtAlertAvailableWater.Name = "txtAlertAvailableWater";
            this.txtAlertAvailableWater.Size = new System.Drawing.Size(100, 21);
            this.txtAlertAvailableWater.TabIndex = 118;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(395, 133);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(71, 12);
            this.label14.TabIndex = 117;
            this.label14.Text = "0-9999999.9";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(135, 133);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(125, 12);
            this.label13.TabIndex = 116;
            this.label13.Text = "测站选址范围为 1-254";
            // 
            // txtYearExploitation
            // 
            this.txtYearExploitation.Location = new System.Drawing.Point(397, 109);
            this.txtYearExploitation.Name = "txtYearExploitation";
            this.txtYearExploitation.Size = new System.Drawing.Size(100, 21);
            this.txtYearExploitation.TabIndex = 115;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(395, 89);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(71, 12);
            this.label11.TabIndex = 114;
            this.label11.Text = "必须6位数字";
            // 
            // txtAddressCode3
            // 
            this.txtAddressCode3.Location = new System.Drawing.Point(137, 109);
            this.txtAddressCode3.Name = "txtAddressCode3";
            this.txtAddressCode3.Size = new System.Drawing.Size(100, 21);
            this.txtAddressCode3.TabIndex = 113;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(135, 89);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(71, 12);
            this.label10.TabIndex = 112;
            this.label10.Text = "必须6位数字";
            // 
            // txtAddressCode2
            // 
            this.txtAddressCode2.Location = new System.Drawing.Point(397, 65);
            this.txtAddressCode2.Name = "txtAddressCode2";
            this.txtAddressCode2.Size = new System.Drawing.Size(100, 21);
            this.txtAddressCode2.TabIndex = 111;
            // 
            // txtAddressCode1
            // 
            this.txtAddressCode1.Location = new System.Drawing.Point(137, 65);
            this.txtAddressCode1.Name = "txtAddressCode1";
            this.txtAddressCode1.Size = new System.Drawing.Size(100, 21);
            this.txtAddressCode1.TabIndex = 110;
            // 
            // lbSerialNumber
            // 
            this.lbSerialNumber.AutoSize = true;
            this.lbSerialNumber.Location = new System.Drawing.Point(395, 13);
            this.lbSerialNumber.Name = "lbSerialNumber";
            this.lbSerialNumber.Size = new System.Drawing.Size(29, 12);
            this.lbSerialNumber.TabIndex = 109;
            this.lbSerialNumber.Text = "未知";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(324, 13);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(65, 12);
            this.label12.TabIndex = 108;
            this.label12.Text = "卡序列号：";
            // 
            // lbCardType
            // 
            this.lbCardType.AutoSize = true;
            this.lbCardType.BackColor = System.Drawing.SystemColors.Control;
            this.lbCardType.ForeColor = System.Drawing.Color.Red;
            this.lbCardType.Location = new System.Drawing.Point(135, 13);
            this.lbCardType.Name = "lbCardType";
            this.lbCardType.Size = new System.Drawing.Size(29, 12);
            this.lbCardType.TabIndex = 107;
            this.lbCardType.Text = "未知";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(312, 197);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(77, 12);
            this.label9.TabIndex = 106;
            this.label9.Text = "水位报警值：";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(54, 197);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(77, 12);
            this.label8.TabIndex = 105;
            this.label8.Text = "电表脉冲数：";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(54, 240);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(77, 12);
            this.label7.TabIndex = 104;
            this.label7.Text = "流量计类型：";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(30, 156);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(101, 12);
            this.label6.TabIndex = 103;
            this.label6.Text = "可用水量提醒值：";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(288, 112);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(101, 12);
            this.label5.TabIndex = 102;
            this.label5.Text = "年度可开采水量：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(66, 112);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 101;
            this.label4.Text = "测站编码：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(288, 68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(101, 12);
            this.label3.TabIndex = 100;
            this.label3.Text = "镇（乡）村编码：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(54, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 12);
            this.label2.TabIndex = 99;
            this.label2.Text = "行政区划码：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(78, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 98;
            this.label1.Text = "卡类型：";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(336, 240);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(53, 12);
            this.label19.TabIndex = 133;
            this.label19.Text = "站类型：";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(78, 270);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(53, 12);
            this.label20.TabIndex = 134;
            this.label20.Text = "地址码：";
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(397, 237);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 20);
            this.comboBox1.TabIndex = 135;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(324, 270);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(65, 12);
            this.label22.TabIndex = 139;
            this.label22.Text = "主站编码：";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(397, 267);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 21);
            this.textBox1.TabIndex = 140;
            // 
            // txtAlertAvailableElectric
            // 
            this.txtAlertAvailableElectric.Location = new System.Drawing.Point(395, 153);
            this.txtAlertAvailableElectric.Name = "txtAlertAvailableElectric";
            this.txtAlertAvailableElectric.Size = new System.Drawing.Size(100, 21);
            this.txtAlertAvailableElectric.TabIndex = 142;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(288, 156);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(101, 12);
            this.label21.TabIndex = 141;
            this.label21.Text = "可用电量提醒值：";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(395, 177);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(47, 12);
            this.label23.TabIndex = 143;
            this.label23.Text = "0-65535";
            // 
            // txtStationCode
            // 
            this.txtStationCode.Location = new System.Drawing.Point(139, 267);
            this.txtStationCode.Name = "txtStationCode";
            this.txtStationCode.Size = new System.Drawing.Size(100, 21);
            this.txtStationCode.TabIndex = 144;
            // 
            // frmCardDevice
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 361);
            this.Controls.Add(this.txtStationCode);
            this.Controls.Add(this.label23);
            this.Controls.Add(this.txtAlertAvailableElectric);
            this.Controls.Add(this.label21);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label22);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.cboDeviceList);
            this.Controls.Add(this.label45);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.lbState);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.cboTypeCode);
            this.Controls.Add(this.txtAlertWaterLevel);
            this.Controls.Add(this.txtMeterPulse);
            this.Controls.Add(this.txtAlertAvailableWater);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.txtYearExploitation);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.txtAddressCode3);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.txtAddressCode2);
            this.Controls.Add(this.txtAddressCode1);
            this.Controls.Add(this.lbSerialNumber);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.lbCardType);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmCardDevice";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "设置卡操作";
            this.Load += new System.EventHandler(this.frmCardDevice_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ComboBox cboDeviceList;
        private System.Windows.Forms.Label label45;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label lbState;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.ComboBox cboTypeCode;
        private System.Windows.Forms.TextBox txtAlertWaterLevel;
        private System.Windows.Forms.TextBox txtMeterPulse;
        private System.Windows.Forms.TextBox txtAlertAvailableWater;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txtYearExploitation;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtAddressCode3;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtAddressCode2;
        private System.Windows.Forms.TextBox txtAddressCode1;
        private System.Windows.Forms.Label lbSerialNumber;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label lbCardType;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox txtAlertAvailableElectric;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.TextBox txtStationCode;
    }
}