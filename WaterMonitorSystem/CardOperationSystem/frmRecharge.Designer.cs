namespace CardOperationSystem
{
    partial class frmRecharge
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
            this.label19 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.lbState = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.txtRemark = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.txtTotalPriceElectric = new System.Windows.Forms.TextBox();
            this.txtNumberElectric = new System.Windows.Forms.TextBox();
            this.txtUnitPriceElectric = new System.Windows.Forms.TextBox();
            this.txtTotalPriceWater = new System.Windows.Forms.TextBox();
            this.txtNumberWater = new System.Windows.Forms.TextBox();
            this.txtUnitPriceWater = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.txtTotalElectric = new System.Windows.Forms.Label();
            this.txtTotalWater = new System.Windows.Forms.Label();
            this.txtResidualElectric = new System.Windows.Forms.Label();
            this.txtResidualWater = new System.Windows.Forms.Label();
            this.txtTelephone = new System.Windows.Forms.Label();
            this.txtIdentityNumber = new System.Windows.Forms.Label();
            this.txtUserName = new System.Windows.Forms.Label();
            this.txtUserNo = new System.Windows.Forms.Label();
            this.lbSerialNumber = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.lbCardType = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.txtWaterQuota = new System.Windows.Forms.Label();
            this.txtElectricQuota = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.txtWaterUsed = new System.Windows.Forms.Label();
            this.txtElectricUsed = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.txtWaterLevel = new System.Windows.Forms.Label();
            this.txtElectricLevel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(120, 289);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(251, 12);
            this.label19.TabIndex = 123;
            this.label19.Text = "充值后剩余可用水电量取值范围为0-9999999.9";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(122, 358);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(84, 16);
            this.checkBox1.TabIndex = 122;
            this.checkBox1.Text = "减少水电量";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.Visible = false;
            // 
            // lbState
            // 
            this.lbState.AutoSize = true;
            this.lbState.Location = new System.Drawing.Point(304, 385);
            this.lbState.Name = "lbState";
            this.lbState.Size = new System.Drawing.Size(29, 12);
            this.lbState.TabIndex = 119;
            this.lbState.Text = "状态";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(120, 268);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(305, 12);
            this.label18.TabIndex = 118;
            this.label18.Text = "充值单价金额保留两位小数，充值水量电量保留一位小数";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(212, 380);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 117;
            this.button2.Text = "关闭";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(122, 380);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 116;
            this.button1.Text = "充值";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtRemark
            // 
            this.txtRemark.Location = new System.Drawing.Point(122, 305);
            this.txtRemark.Multiline = true;
            this.txtRemark.Name = "txtRemark";
            this.txtRemark.Size = new System.Drawing.Size(315, 46);
            this.txtRemark.TabIndex = 115;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(51, 308);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(65, 12);
            this.label17.TabIndex = 114;
            this.label17.Text = "充值备注：";
            // 
            // txtTotalPriceElectric
            // 
            this.txtTotalPriceElectric.Location = new System.Drawing.Point(357, 244);
            this.txtTotalPriceElectric.Name = "txtTotalPriceElectric";
            this.txtTotalPriceElectric.Size = new System.Drawing.Size(80, 21);
            this.txtTotalPriceElectric.TabIndex = 113;
            this.txtTotalPriceElectric.Text = "0";
            // 
            // txtNumberElectric
            // 
            this.txtNumberElectric.Location = new System.Drawing.Point(357, 217);
            this.txtNumberElectric.Name = "txtNumberElectric";
            this.txtNumberElectric.Size = new System.Drawing.Size(80, 21);
            this.txtNumberElectric.TabIndex = 112;
            this.txtNumberElectric.Text = "0";
            this.txtNumberElectric.TextChanged += new System.EventHandler(this.txtNumberElectric_TextChanged);
            // 
            // txtUnitPriceElectric
            // 
            this.txtUnitPriceElectric.Location = new System.Drawing.Point(357, 192);
            this.txtUnitPriceElectric.Name = "txtUnitPriceElectric";
            this.txtUnitPriceElectric.ReadOnly = true;
            this.txtUnitPriceElectric.Size = new System.Drawing.Size(80, 21);
            this.txtUnitPriceElectric.TabIndex = 111;
            this.txtUnitPriceElectric.Text = "0";
            // 
            // txtTotalPriceWater
            // 
            this.txtTotalPriceWater.Location = new System.Drawing.Point(122, 244);
            this.txtTotalPriceWater.Name = "txtTotalPriceWater";
            this.txtTotalPriceWater.Size = new System.Drawing.Size(80, 21);
            this.txtTotalPriceWater.TabIndex = 110;
            this.txtTotalPriceWater.Text = "0";
            // 
            // txtNumberWater
            // 
            this.txtNumberWater.Location = new System.Drawing.Point(122, 217);
            this.txtNumberWater.Name = "txtNumberWater";
            this.txtNumberWater.Size = new System.Drawing.Size(80, 21);
            this.txtNumberWater.TabIndex = 109;
            this.txtNumberWater.Text = "0";
            this.txtNumberWater.TextChanged += new System.EventHandler(this.txtNumberWater_TextChanged);
            // 
            // txtUnitPriceWater
            // 
            this.txtUnitPriceWater.Location = new System.Drawing.Point(122, 192);
            this.txtUnitPriceWater.Name = "txtUnitPriceWater";
            this.txtUnitPriceWater.ReadOnly = true;
            this.txtUnitPriceWater.Size = new System.Drawing.Size(80, 21);
            this.txtUnitPriceWater.TabIndex = 108;
            this.txtUnitPriceWater.Text = "0";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(274, 247);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(77, 12);
            this.label14.TabIndex = 107;
            this.label14.Text = "充值电金额：";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(286, 220);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(65, 12);
            this.label15.TabIndex = 106;
            this.label15.Text = "充值电量：";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(286, 195);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(65, 12);
            this.label16.TabIndex = 105;
            this.label16.Text = "充值电价：";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(39, 247);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(77, 12);
            this.label13.TabIndex = 104;
            this.label13.Text = "充值水金额：";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(51, 220);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(65, 12);
            this.label10.TabIndex = 103;
            this.label10.Text = "充值水量：";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(51, 195);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(65, 12);
            this.label9.TabIndex = 102;
            this.label9.Text = "充值水价：";
            // 
            // txtTotalElectric
            // 
            this.txtTotalElectric.AutoSize = true;
            this.txtTotalElectric.Location = new System.Drawing.Point(357, 99);
            this.txtTotalElectric.Name = "txtTotalElectric";
            this.txtTotalElectric.Size = new System.Drawing.Size(29, 12);
            this.txtTotalElectric.TabIndex = 101;
            this.txtTotalElectric.Text = "未知";
            // 
            // txtTotalWater
            // 
            this.txtTotalWater.AutoSize = true;
            this.txtTotalWater.Location = new System.Drawing.Point(128, 99);
            this.txtTotalWater.Name = "txtTotalWater";
            this.txtTotalWater.Size = new System.Drawing.Size(29, 12);
            this.txtTotalWater.TabIndex = 100;
            this.txtTotalWater.Text = "未知";
            // 
            // txtResidualElectric
            // 
            this.txtResidualElectric.AutoSize = true;
            this.txtResidualElectric.Location = new System.Drawing.Point(357, 77);
            this.txtResidualElectric.Name = "txtResidualElectric";
            this.txtResidualElectric.Size = new System.Drawing.Size(29, 12);
            this.txtResidualElectric.TabIndex = 99;
            this.txtResidualElectric.Text = "未知";
            // 
            // txtResidualWater
            // 
            this.txtResidualWater.AutoSize = true;
            this.txtResidualWater.Location = new System.Drawing.Point(128, 77);
            this.txtResidualWater.Name = "txtResidualWater";
            this.txtResidualWater.Size = new System.Drawing.Size(29, 12);
            this.txtResidualWater.TabIndex = 98;
            this.txtResidualWater.Text = "未知";
            // 
            // txtTelephone
            // 
            this.txtTelephone.AutoSize = true;
            this.txtTelephone.Location = new System.Drawing.Point(357, 56);
            this.txtTelephone.Name = "txtTelephone";
            this.txtTelephone.Size = new System.Drawing.Size(29, 12);
            this.txtTelephone.TabIndex = 97;
            this.txtTelephone.Text = "未知";
            // 
            // txtIdentityNumber
            // 
            this.txtIdentityNumber.AutoSize = true;
            this.txtIdentityNumber.Location = new System.Drawing.Point(128, 56);
            this.txtIdentityNumber.Name = "txtIdentityNumber";
            this.txtIdentityNumber.Size = new System.Drawing.Size(29, 12);
            this.txtIdentityNumber.TabIndex = 96;
            this.txtIdentityNumber.Text = "未知";
            // 
            // txtUserName
            // 
            this.txtUserName.AutoSize = true;
            this.txtUserName.Location = new System.Drawing.Point(357, 34);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(29, 12);
            this.txtUserName.TabIndex = 95;
            this.txtUserName.Text = "未知";
            // 
            // txtUserNo
            // 
            this.txtUserNo.AutoSize = true;
            this.txtUserNo.Location = new System.Drawing.Point(128, 34);
            this.txtUserNo.Name = "txtUserNo";
            this.txtUserNo.Size = new System.Drawing.Size(29, 12);
            this.txtUserNo.TabIndex = 94;
            this.txtUserNo.Text = "未知";
            // 
            // lbSerialNumber
            // 
            this.lbSerialNumber.AutoSize = true;
            this.lbSerialNumber.Location = new System.Drawing.Point(357, 14);
            this.lbSerialNumber.Name = "lbSerialNumber";
            this.lbSerialNumber.Size = new System.Drawing.Size(29, 12);
            this.lbSerialNumber.TabIndex = 93;
            this.lbSerialNumber.Text = "未知";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(286, 14);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(65, 12);
            this.label12.TabIndex = 92;
            this.label12.Text = "卡序列号：";
            // 
            // lbCardType
            // 
            this.lbCardType.AutoSize = true;
            this.lbCardType.ForeColor = System.Drawing.Color.Red;
            this.lbCardType.Location = new System.Drawing.Point(128, 14);
            this.lbCardType.Name = "lbCardType";
            this.lbCardType.Size = new System.Drawing.Size(29, 12);
            this.lbCardType.TabIndex = 91;
            this.lbCardType.Text = "未知";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(63, 14);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(53, 12);
            this.label11.TabIndex = 90;
            this.label11.Text = "卡类型：";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(274, 99);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(77, 12);
            this.label8.TabIndex = 89;
            this.label8.Text = "累计用电量：";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(39, 99);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(77, 12);
            this.label7.TabIndex = 88;
            this.label7.Text = "累计用水量：";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(262, 77);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(89, 12);
            this.label6.TabIndex = 87;
            this.label6.Text = "剩余可用电量：";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(27, 77);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(89, 12);
            this.label5.TabIndex = 86;
            this.label5.Text = "剩余可用水量：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(286, 56);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 85;
            this.label4.Text = "联系电话：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(51, 56);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 84;
            this.label3.Text = "身份证号：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(298, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 83;
            this.label2.Text = "用户名：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(51, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 82;
            this.label1.Text = "用户卡号：";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(51, 119);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(65, 12);
            this.label20.TabIndex = 124;
            this.label20.Text = "水量限额：";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(286, 119);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(65, 12);
            this.label21.TabIndex = 125;
            this.label21.Text = "电量限额：";
            // 
            // txtWaterQuota
            // 
            this.txtWaterQuota.AutoSize = true;
            this.txtWaterQuota.Location = new System.Drawing.Point(128, 119);
            this.txtWaterQuota.Name = "txtWaterQuota";
            this.txtWaterQuota.Size = new System.Drawing.Size(29, 12);
            this.txtWaterQuota.TabIndex = 126;
            this.txtWaterQuota.Text = "未知";
            // 
            // txtElectricQuota
            // 
            this.txtElectricQuota.AutoSize = true;
            this.txtElectricQuota.Location = new System.Drawing.Point(357, 119);
            this.txtElectricQuota.Name = "txtElectricQuota";
            this.txtElectricQuota.Size = new System.Drawing.Size(29, 12);
            this.txtElectricQuota.TabIndex = 127;
            this.txtElectricQuota.Text = "未知";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(15, 140);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(101, 12);
            this.label22.TabIndex = 128;
            this.label22.Text = "年累计充值水量：";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(250, 140);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(101, 12);
            this.label23.TabIndex = 129;
            this.label23.Text = "年累计充值电量：";
            // 
            // txtWaterUsed
            // 
            this.txtWaterUsed.AutoSize = true;
            this.txtWaterUsed.Location = new System.Drawing.Point(128, 140);
            this.txtWaterUsed.Name = "txtWaterUsed";
            this.txtWaterUsed.Size = new System.Drawing.Size(29, 12);
            this.txtWaterUsed.TabIndex = 130;
            this.txtWaterUsed.Text = "未知";
            // 
            // txtElectricUsed
            // 
            this.txtElectricUsed.AutoSize = true;
            this.txtElectricUsed.Location = new System.Drawing.Point(357, 140);
            this.txtElectricUsed.Name = "txtElectricUsed";
            this.txtElectricUsed.Size = new System.Drawing.Size(29, 12);
            this.txtElectricUsed.TabIndex = 131;
            this.txtElectricUsed.Text = "未知";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(51, 163);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(65, 12);
            this.label24.TabIndex = 132;
            this.label24.Text = "充值限水：";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(286, 163);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(65, 12);
            this.label25.TabIndex = 133;
            this.label25.Text = "充值限电：";
            // 
            // txtWaterLevel
            // 
            this.txtWaterLevel.AutoSize = true;
            this.txtWaterLevel.Location = new System.Drawing.Point(128, 163);
            this.txtWaterLevel.Name = "txtWaterLevel";
            this.txtWaterLevel.Size = new System.Drawing.Size(29, 12);
            this.txtWaterLevel.TabIndex = 134;
            this.txtWaterLevel.Text = "未知";
            // 
            // txtElectricLevel
            // 
            this.txtElectricLevel.AutoSize = true;
            this.txtElectricLevel.Location = new System.Drawing.Point(357, 163);
            this.txtElectricLevel.Name = "txtElectricLevel";
            this.txtElectricLevel.Size = new System.Drawing.Size(29, 12);
            this.txtElectricLevel.TabIndex = 135;
            this.txtElectricLevel.Text = "未知";
            // 
            // frmRecharge
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 411);
            this.Controls.Add(this.txtElectricLevel);
            this.Controls.Add(this.txtWaterLevel);
            this.Controls.Add(this.label25);
            this.Controls.Add(this.label24);
            this.Controls.Add(this.txtElectricUsed);
            this.Controls.Add(this.txtWaterUsed);
            this.Controls.Add(this.label23);
            this.Controls.Add(this.label22);
            this.Controls.Add(this.txtElectricQuota);
            this.Controls.Add(this.txtWaterQuota);
            this.Controls.Add(this.label21);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.lbState);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txtRemark);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.txtTotalPriceElectric);
            this.Controls.Add(this.txtNumberElectric);
            this.Controls.Add(this.txtUnitPriceElectric);
            this.Controls.Add(this.txtTotalPriceWater);
            this.Controls.Add(this.txtNumberWater);
            this.Controls.Add(this.txtUnitPriceWater);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.txtTotalElectric);
            this.Controls.Add(this.txtTotalWater);
            this.Controls.Add(this.txtResidualElectric);
            this.Controls.Add(this.txtResidualWater);
            this.Controls.Add(this.txtTelephone);
            this.Controls.Add(this.txtIdentityNumber);
            this.Controls.Add(this.txtUserName);
            this.Controls.Add(this.txtUserNo);
            this.Controls.Add(this.lbSerialNumber);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.lbCardType);
            this.Controls.Add(this.label11);
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
            this.Name = "frmRecharge";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "用户卡充值";
            this.Load += new System.EventHandler(this.frmRecharge_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label lbState;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox txtRemark;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox txtTotalPriceElectric;
        private System.Windows.Forms.TextBox txtNumberElectric;
        private System.Windows.Forms.TextBox txtUnitPriceElectric;
        private System.Windows.Forms.TextBox txtTotalPriceWater;
        private System.Windows.Forms.TextBox txtNumberWater;
        private System.Windows.Forms.TextBox txtUnitPriceWater;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label txtTotalElectric;
        private System.Windows.Forms.Label txtTotalWater;
        private System.Windows.Forms.Label txtResidualElectric;
        private System.Windows.Forms.Label txtResidualWater;
        private System.Windows.Forms.Label txtTelephone;
        private System.Windows.Forms.Label txtIdentityNumber;
        private System.Windows.Forms.Label txtUserName;
        private System.Windows.Forms.Label txtUserNo;
        private System.Windows.Forms.Label lbSerialNumber;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label lbCardType;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label txtWaterQuota;
        private System.Windows.Forms.Label txtElectricQuota;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label txtWaterUsed;
        private System.Windows.Forms.Label txtElectricUsed;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label txtWaterLevel;
        private System.Windows.Forms.Label txtElectricLevel;
    }
}