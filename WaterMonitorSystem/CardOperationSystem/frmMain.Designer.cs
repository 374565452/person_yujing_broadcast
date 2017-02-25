namespace CardOperationSystem
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
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.SerialNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UserNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UserName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IdentityNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Telephone = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ResidualWater = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ResidualElectric = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TotalWater = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TotalElectric = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OpenTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LastConsumptionTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LastChargeTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LastUpdateTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IsCountermand = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CountermandTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CountermandCancelTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DeviceList = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.button8 = new System.Windows.Forms.Button();
            this.btnReOpen = new System.Windows.Forms.Button();
            this.btnCountermandCancel = new System.Windows.Forms.Button();
            this.btnCountermand = new System.Windows.Forms.Button();
            this.txtTelephone = new System.Windows.Forms.TextBox();
            this.txtIdentityNumber = new System.Windows.Forms.TextBox();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txtUserNo = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btnSearch = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.button7 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.btnReadIC = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.btnInit = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.btnOpenIC = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btnDevice = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lbDeviceCount = new System.Windows.Forms.Label();
            this.lbDistrictName = new System.Windows.Forms.Label();
            this.lbGroupName = new System.Windows.Forms.Label();
            this.lbTrueName = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lbIcdev = new System.Windows.Forms.Label();
            this.lbSoftVer = new System.Windows.Forms.Label();
            this.lbHardVer = new System.Windows.Forms.Label();
            this.lbResult = new System.Windows.Forms.Label();
            this.timerIC = new System.Windows.Forms.Timer(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.groupBox6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox6
            // 
            this.groupBox6.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox6.Controls.Add(this.dataGridView1);
            this.groupBox6.Location = new System.Drawing.Point(264, 127);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(824, 594);
            this.groupBox6.TabIndex = 11;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "用户卡列表";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.SerialNumber,
            this.UserNo,
            this.UserName,
            this.IdentityNumber,
            this.Telephone,
            this.ResidualWater,
            this.ResidualElectric,
            this.TotalWater,
            this.TotalElectric,
            this.OpenTime,
            this.LastConsumptionTime,
            this.LastChargeTime,
            this.LastUpdateTime,
            this.IsCountermand,
            this.CountermandTime,
            this.CountermandCancelTime,
            this.Id,
            this.DeviceList});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(3, 17);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersWidth = 30;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(818, 574);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.dataGridView1_DataBindingComplete);
            // 
            // SerialNumber
            // 
            this.SerialNumber.DataPropertyName = "SerialNumber";
            this.SerialNumber.Frozen = true;
            this.SerialNumber.HeaderText = "序列号";
            this.SerialNumber.Name = "SerialNumber";
            this.SerialNumber.ReadOnly = true;
            this.SerialNumber.Width = 65;
            // 
            // UserNo
            // 
            this.UserNo.DataPropertyName = "UserNo";
            this.UserNo.Frozen = true;
            this.UserNo.HeaderText = "卡号";
            this.UserNo.Name = "UserNo";
            this.UserNo.ReadOnly = true;
            this.UserNo.Width = 65;
            // 
            // UserName
            // 
            this.UserName.DataPropertyName = "UserName";
            this.UserName.Frozen = true;
            this.UserName.HeaderText = "用户名";
            this.UserName.Name = "UserName";
            this.UserName.ReadOnly = true;
            this.UserName.Width = 65;
            // 
            // IdentityNumber
            // 
            this.IdentityNumber.DataPropertyName = "IdentityNumber";
            this.IdentityNumber.HeaderText = "身份证号";
            this.IdentityNumber.Name = "IdentityNumber";
            this.IdentityNumber.ReadOnly = true;
            this.IdentityNumber.Width = 120;
            // 
            // Telephone
            // 
            this.Telephone.DataPropertyName = "Telephone";
            this.Telephone.HeaderText = "电话";
            this.Telephone.Name = "Telephone";
            this.Telephone.ReadOnly = true;
            this.Telephone.Width = 80;
            // 
            // ResidualWater
            // 
            this.ResidualWater.DataPropertyName = "ResidualWater";
            this.ResidualWater.HeaderText = "剩余水量";
            this.ResidualWater.Name = "ResidualWater";
            this.ResidualWater.ReadOnly = true;
            this.ResidualWater.Width = 80;
            // 
            // ResidualElectric
            // 
            this.ResidualElectric.DataPropertyName = "ResidualElectric";
            this.ResidualElectric.HeaderText = "剩余电量";
            this.ResidualElectric.Name = "ResidualElectric";
            this.ResidualElectric.ReadOnly = true;
            this.ResidualElectric.Width = 80;
            // 
            // TotalWater
            // 
            this.TotalWater.DataPropertyName = "TotalWater";
            this.TotalWater.HeaderText = "总计水量";
            this.TotalWater.Name = "TotalWater";
            this.TotalWater.ReadOnly = true;
            this.TotalWater.Width = 80;
            // 
            // TotalElectric
            // 
            this.TotalElectric.DataPropertyName = "TotalElectric";
            this.TotalElectric.HeaderText = "总计电量";
            this.TotalElectric.Name = "TotalElectric";
            this.TotalElectric.ReadOnly = true;
            this.TotalElectric.Width = 80;
            // 
            // OpenTime
            // 
            this.OpenTime.DataPropertyName = "OpenTime";
            this.OpenTime.HeaderText = "开卡时间";
            this.OpenTime.Name = "OpenTime";
            this.OpenTime.ReadOnly = true;
            this.OpenTime.Width = 120;
            // 
            // LastConsumptionTime
            // 
            this.LastConsumptionTime.DataPropertyName = "LastConsumptionTime";
            this.LastConsumptionTime.HeaderText = "最后消费时间";
            this.LastConsumptionTime.Name = "LastConsumptionTime";
            this.LastConsumptionTime.ReadOnly = true;
            this.LastConsumptionTime.Width = 120;
            // 
            // LastChargeTime
            // 
            this.LastChargeTime.DataPropertyName = "LastChargeTime";
            this.LastChargeTime.HeaderText = "最后充值时间";
            this.LastChargeTime.Name = "LastChargeTime";
            this.LastChargeTime.ReadOnly = true;
            this.LastChargeTime.Width = 120;
            // 
            // LastUpdateTime
            // 
            this.LastUpdateTime.DataPropertyName = "LastUpdateTime";
            this.LastUpdateTime.HeaderText = "最后修改时间";
            this.LastUpdateTime.Name = "LastUpdateTime";
            this.LastUpdateTime.ReadOnly = true;
            this.LastUpdateTime.Width = 120;
            // 
            // IsCountermand
            // 
            this.IsCountermand.DataPropertyName = "IsCountermand";
            this.IsCountermand.HeaderText = "是否挂失";
            this.IsCountermand.Name = "IsCountermand";
            this.IsCountermand.ReadOnly = true;
            this.IsCountermand.Width = 80;
            // 
            // CountermandTime
            // 
            this.CountermandTime.DataPropertyName = "CountermandTime";
            this.CountermandTime.HeaderText = "挂失时间";
            this.CountermandTime.Name = "CountermandTime";
            this.CountermandTime.ReadOnly = true;
            this.CountermandTime.Width = 120;
            // 
            // CountermandCancelTime
            // 
            this.CountermandCancelTime.DataPropertyName = "CountermandCancelTime";
            this.CountermandCancelTime.HeaderText = "取消挂失时间";
            this.CountermandCancelTime.Name = "CountermandCancelTime";
            this.CountermandCancelTime.ReadOnly = true;
            this.CountermandCancelTime.Width = 120;
            // 
            // Id
            // 
            this.Id.DataPropertyName = "Id";
            this.Id.HeaderText = "Id";
            this.Id.Name = "Id";
            this.Id.ReadOnly = true;
            this.Id.Visible = false;
            // 
            // DeviceList
            // 
            this.DeviceList.DataPropertyName = "DeviceList";
            this.DeviceList.HeaderText = "DeviceList";
            this.DeviceList.Name = "DeviceList";
            this.DeviceList.ReadOnly = true;
            this.DeviceList.Visible = false;
            // 
            // groupBox5
            // 
            this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox5.Controls.Add(this.label12);
            this.groupBox5.Controls.Add(this.label11);
            this.groupBox5.Controls.Add(this.button8);
            this.groupBox5.Controls.Add(this.btnReOpen);
            this.groupBox5.Controls.Add(this.btnCountermandCancel);
            this.groupBox5.Controls.Add(this.btnCountermand);
            this.groupBox5.Controls.Add(this.txtTelephone);
            this.groupBox5.Controls.Add(this.txtIdentityNumber);
            this.groupBox5.Controls.Add(this.txtUserName);
            this.groupBox5.Controls.Add(this.label10);
            this.groupBox5.Controls.Add(this.label9);
            this.groupBox5.Controls.Add(this.label8);
            this.groupBox5.Controls.Add(this.txtUserNo);
            this.groupBox5.Controls.Add(this.label7);
            this.groupBox5.Controls.Add(this.btnSearch);
            this.groupBox5.Location = new System.Drawing.Point(261, 9);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(824, 109);
            this.groupBox5.TabIndex = 10;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "用户卡查询";
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(339, 73);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(90, 23);
            this.button8.TabIndex = 12;
            this.button8.Text = "充值用水查询";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // btnReOpen
            // 
            this.btnReOpen.Location = new System.Drawing.Point(258, 73);
            this.btnReOpen.Name = "btnReOpen";
            this.btnReOpen.Size = new System.Drawing.Size(75, 23);
            this.btnReOpen.TabIndex = 11;
            this.btnReOpen.Text = "重新开卡";
            this.btnReOpen.UseVisualStyleBackColor = true;
            this.btnReOpen.Click += new System.EventHandler(this.btnReOpen_Click);
            // 
            // btnCountermandCancel
            // 
            this.btnCountermandCancel.Location = new System.Drawing.Point(177, 73);
            this.btnCountermandCancel.Name = "btnCountermandCancel";
            this.btnCountermandCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCountermandCancel.TabIndex = 10;
            this.btnCountermandCancel.Text = "取消挂失";
            this.btnCountermandCancel.UseVisualStyleBackColor = true;
            this.btnCountermandCancel.Click += new System.EventHandler(this.btnCountermandCancel_Click);
            // 
            // btnCountermand
            // 
            this.btnCountermand.Location = new System.Drawing.Point(96, 73);
            this.btnCountermand.Name = "btnCountermand";
            this.btnCountermand.Size = new System.Drawing.Size(75, 23);
            this.btnCountermand.TabIndex = 9;
            this.btnCountermand.Text = "挂失";
            this.btnCountermand.UseVisualStyleBackColor = true;
            this.btnCountermand.Click += new System.EventHandler(this.btnCountermand_Click);
            // 
            // txtTelephone
            // 
            this.txtTelephone.Location = new System.Drawing.Point(366, 46);
            this.txtTelephone.Name = "txtTelephone";
            this.txtTelephone.Size = new System.Drawing.Size(100, 21);
            this.txtTelephone.TabIndex = 8;
            // 
            // txtIdentityNumber
            // 
            this.txtIdentityNumber.Location = new System.Drawing.Point(95, 46);
            this.txtIdentityNumber.Name = "txtIdentityNumber";
            this.txtIdentityNumber.Size = new System.Drawing.Size(180, 21);
            this.txtIdentityNumber.TabIndex = 7;
            // 
            // txtUserName
            // 
            this.txtUserName.Location = new System.Drawing.Point(366, 19);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(100, 21);
            this.txtUserName.TabIndex = 6;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(319, 49);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(41, 12);
            this.label10.TabIndex = 5;
            this.label10.Text = "电话：";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(24, 49);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(65, 12);
            this.label9.TabIndex = 4;
            this.label9.Text = "身份证号：";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(307, 22);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 12);
            this.label8.TabIndex = 3;
            this.label8.Text = "用户名：";
            // 
            // txtUserNo
            // 
            this.txtUserNo.Location = new System.Drawing.Point(95, 19);
            this.txtUserNo.Name = "txtUserNo";
            this.txtUserNo.Size = new System.Drawing.Size(180, 21);
            this.txtUserNo.TabIndex = 2;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(48, 22);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(41, 12);
            this.label7.TabIndex = 1;
            this.label7.Text = "卡号：";
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(15, 73);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 0;
            this.btnSearch.Text = "查询";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.textBox1);
            this.groupBox4.Location = new System.Drawing.Point(264, 127);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(824, 594);
            this.groupBox4.TabIndex = 9;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "日志";
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(6, 20);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox1.Size = new System.Drawing.Size(812, 568);
            this.textBox1.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox3.Controls.Add(this.button7);
            this.groupBox3.Controls.Add(this.button3);
            this.groupBox3.Controls.Add(this.btnReadIC);
            this.groupBox3.Controls.Add(this.button6);
            this.groupBox3.Controls.Add(this.button5);
            this.groupBox3.Controls.Add(this.button4);
            this.groupBox3.Controls.Add(this.btnInit);
            this.groupBox3.Controls.Add(this.button2);
            this.groupBox3.Controls.Add(this.btnOpenIC);
            this.groupBox3.Controls.Add(this.btnClear);
            this.groupBox3.Controls.Add(this.button1);
            this.groupBox3.Controls.Add(this.btnDevice);
            this.groupBox3.Location = new System.Drawing.Point(12, 300);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(240, 421);
            this.groupBox3.TabIndex = 8;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "操作";
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(20, 86);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(75, 23);
            this.button7.TabIndex = 46;
            this.button7.Text = "网络设置卡";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // button3
            // 
            this.button3.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button3.Location = new System.Drawing.Point(20, 115);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 45;
            this.button3.Text = "基础信息";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // btnReadIC
            // 
            this.btnReadIC.Location = new System.Drawing.Point(20, 201);
            this.btnReadIC.Name = "btnReadIC";
            this.btnReadIC.Size = new System.Drawing.Size(75, 23);
            this.btnReadIC.TabIndex = 44;
            this.btnReadIC.Text = "读卡信息";
            this.btnReadIC.UseVisualStyleBackColor = true;
            this.btnReadIC.Visible = false;
            this.btnReadIC.Click += new System.EventHandler(this.btnReadIC_Click);
            // 
            // button6
            // 
            this.button6.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button6.Location = new System.Drawing.Point(114, 144);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(75, 23);
            this.button6.TabIndex = 43;
            this.button6.Text = "退出";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(20, 144);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(75, 23);
            this.button5.TabIndex = 42;
            this.button5.Text = "系统配置";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(114, 87);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 41;
            this.button4.Text = "检测卡状态";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // btnInit
            // 
            this.btnInit.Location = new System.Drawing.Point(114, 201);
            this.btnInit.Name = "btnInit";
            this.btnInit.Size = new System.Drawing.Size(75, 23);
            this.btnInit.TabIndex = 35;
            this.btnInit.Text = "注销卡";
            this.btnInit.UseVisualStyleBackColor = true;
            this.btnInit.Visible = false;
            this.btnInit.Click += new System.EventHandler(this.btnInit_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(114, 58);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 38;
            this.button2.Text = "清零卡";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // btnOpenIC
            // 
            this.btnOpenIC.Location = new System.Drawing.Point(20, 29);
            this.btnOpenIC.Name = "btnOpenIC";
            this.btnOpenIC.Size = new System.Drawing.Size(75, 23);
            this.btnOpenIC.TabIndex = 34;
            this.btnOpenIC.Text = "用户卡";
            this.btnOpenIC.UseVisualStyleBackColor = true;
            this.btnOpenIC.Click += new System.EventHandler(this.btnOpenIC_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(114, 115);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 33;
            this.btnClear.Text = "清除日志";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Visible = false;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(20, 58);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 37;
            this.button1.Text = "读取卡";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnDevice
            // 
            this.btnDevice.Location = new System.Drawing.Point(114, 29);
            this.btnDevice.Name = "btnDevice";
            this.btnDevice.Size = new System.Drawing.Size(75, 23);
            this.btnDevice.TabIndex = 36;
            this.btnDevice.Text = "设置卡";
            this.btnDevice.UseVisualStyleBackColor = true;
            this.btnDevice.Click += new System.EventHandler(this.btnDevice_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lbDeviceCount);
            this.groupBox2.Controls.Add(this.lbDistrictName);
            this.groupBox2.Controls.Add(this.lbGroupName);
            this.groupBox2.Controls.Add(this.lbTrueName);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(12, 127);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(240, 167);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "登录用户信息";
            // 
            // lbDeviceCount
            // 
            this.lbDeviceCount.AutoSize = true;
            this.lbDeviceCount.Location = new System.Drawing.Point(18, 119);
            this.lbDeviceCount.Name = "lbDeviceCount";
            this.lbDeviceCount.Size = new System.Drawing.Size(29, 12);
            this.lbDeviceCount.TabIndex = 11;
            this.lbDeviceCount.Text = "数量";
            // 
            // lbDistrictName
            // 
            this.lbDistrictName.AutoSize = true;
            this.lbDistrictName.Location = new System.Drawing.Point(77, 85);
            this.lbDistrictName.Name = "lbDistrictName";
            this.lbDistrictName.Size = new System.Drawing.Size(89, 12);
            this.lbDistrictName.TabIndex = 10;
            this.lbDistrictName.Text = "lbDistrictName";
            // 
            // lbGroupName
            // 
            this.lbGroupName.AutoSize = true;
            this.lbGroupName.Location = new System.Drawing.Point(77, 56);
            this.lbGroupName.Name = "lbGroupName";
            this.lbGroupName.Size = new System.Drawing.Size(71, 12);
            this.lbGroupName.TabIndex = 9;
            this.lbGroupName.Text = "lbGroupName";
            // 
            // lbTrueName
            // 
            this.lbTrueName.AutoSize = true;
            this.lbTrueName.Location = new System.Drawing.Point(77, 28);
            this.lbTrueName.Name = "lbTrueName";
            this.lbTrueName.Size = new System.Drawing.Size(65, 12);
            this.lbTrueName.TabIndex = 8;
            this.lbTrueName.Text = "lbTrueName";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(18, 85);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 12);
            this.label6.TabIndex = 7;
            this.label6.Text = "行政区：";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(18, 56);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 6;
            this.label5.Text = "用户组：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(18, 28);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 5;
            this.label4.Text = "用户名：";
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.Transparent;
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.lbIcdev);
            this.groupBox1.Controls.Add(this.lbSoftVer);
            this.groupBox1.Controls.Add(this.lbHardVer);
            this.groupBox1.Controls.Add(this.lbResult);
            this.groupBox1.Location = new System.Drawing.Point(12, 9);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(240, 109);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "读卡器状态";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 80);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 10;
            this.label3.Text = "软件版本：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 9;
            this.label2.Text = "硬件版本：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "串口状态：";
            // 
            // lbIcdev
            // 
            this.lbIcdev.AutoSize = true;
            this.lbIcdev.Location = new System.Drawing.Point(187, 20);
            this.lbIcdev.Name = "lbIcdev";
            this.lbIcdev.Size = new System.Drawing.Size(47, 12);
            this.lbIcdev.TabIndex = 8;
            this.lbIcdev.Text = "lbIcdev";
            this.lbIcdev.Visible = false;
            // 
            // lbSoftVer
            // 
            this.lbSoftVer.AutoSize = true;
            this.lbSoftVer.Location = new System.Drawing.Point(77, 80);
            this.lbSoftVer.Name = "lbSoftVer";
            this.lbSoftVer.Size = new System.Drawing.Size(59, 12);
            this.lbSoftVer.TabIndex = 5;
            this.lbSoftVer.Text = "lbSoftVer";
            // 
            // lbHardVer
            // 
            this.lbHardVer.AutoSize = true;
            this.lbHardVer.Location = new System.Drawing.Point(77, 54);
            this.lbHardVer.Name = "lbHardVer";
            this.lbHardVer.Size = new System.Drawing.Size(59, 12);
            this.lbHardVer.TabIndex = 4;
            this.lbHardVer.Text = "lbHardVer";
            // 
            // lbResult
            // 
            this.lbResult.AutoSize = true;
            this.lbResult.Location = new System.Drawing.Point(77, 27);
            this.lbResult.Name = "lbResult";
            this.lbResult.Size = new System.Drawing.Size(53, 12);
            this.lbResult.TabIndex = 3;
            this.lbResult.Text = "lbResult";
            // 
            // timerIC
            // 
            this.timerIC.Interval = 2000;
            this.timerIC.Tick += new System.EventHandler(this.timerIC_Tick);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.DodgerBlue;
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.groupBox5);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1100, 121);
            this.panel1.TabIndex = 13;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("华文楷体", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label11.ForeColor = System.Drawing.Color.White;
            this.label11.Location = new System.Drawing.Point(554, 12);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(255, 33);
            this.label11.TabIndex = 13;
            this.label11.Text = "水资源管理平台";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("华文楷体", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label12.ForeColor = System.Drawing.Color.White;
            this.label12.Location = new System.Drawing.Point(505, 54);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(255, 33);
            this.label12.TabIndex = 14;
            this.label12.Text = "IC卡网络维护终端";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1100, 733);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.panel1);
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "IC卡管理";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMain_FormClosed);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.groupBox6.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button btnReOpen;
        private System.Windows.Forms.Button btnCountermandCancel;
        private System.Windows.Forms.Button btnCountermand;
        private System.Windows.Forms.TextBox txtTelephone;
        private System.Windows.Forms.TextBox txtIdentityNumber;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtUserNo;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button btnReadIC;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button btnInit;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button btnOpenIC;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnDevice;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lbDeviceCount;
        private System.Windows.Forms.Label lbDistrictName;
        private System.Windows.Forms.Label lbGroupName;
        private System.Windows.Forms.Label lbTrueName;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbIcdev;
        private System.Windows.Forms.Label lbSoftVer;
        private System.Windows.Forms.Label lbHardVer;
        private System.Windows.Forms.Label lbResult;
        private System.Windows.Forms.Timer timerIC;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.DataGridViewTextBoxColumn SerialNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn UserNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn UserName;
        private System.Windows.Forms.DataGridViewTextBoxColumn IdentityNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn Telephone;
        private System.Windows.Forms.DataGridViewTextBoxColumn ResidualWater;
        private System.Windows.Forms.DataGridViewTextBoxColumn ResidualElectric;
        private System.Windows.Forms.DataGridViewTextBoxColumn TotalWater;
        private System.Windows.Forms.DataGridViewTextBoxColumn TotalElectric;
        private System.Windows.Forms.DataGridViewTextBoxColumn OpenTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn LastConsumptionTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn LastChargeTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn LastUpdateTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn IsCountermand;
        private System.Windows.Forms.DataGridViewTextBoxColumn CountermandTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn CountermandCancelTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn Id;
        private System.Windows.Forms.DataGridViewTextBoxColumn DeviceList;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Panel panel1;
    }
}

