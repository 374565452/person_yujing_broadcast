namespace CardOperationSystem
{
    partial class frmQuery
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.dateTimePicker2 = new System.Windows.Forms.DateTimePicker();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtTelephone = new System.Windows.Forms.TextBox();
            this.txtIdentityNumber = new System.Windows.Forms.TextBox();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txtUserNo = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.dg2_卡号 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dg2_用户名 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dg2_身份证 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dg2_电话 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dg2_设备序号 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dg2_开泵时间 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dg2_开泵剩余水量 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dg2_开泵剩余电量 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dg2_关泵时间 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dg2_关泵剩余水量 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dg2_关泵剩余电量 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dg2_灌溉时长 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dg2_本次用水 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dg2_本次用电 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dg1_卡号 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dg1_用户名 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dg1_身份证 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dg1_电话 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dg1_充值时间 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dg1_充水金额 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dg1_充水数量 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dg1_充电金额 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dg1_充电数量 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dg1_总金额 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dg1_操作人 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.dateTimePicker2);
            this.groupBox1.Controls.Add(this.dateTimePicker1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtTelephone);
            this.groupBox1.Controls.Add(this.txtIdentityNumber);
            this.groupBox1.Controls.Add(this.txtUserName);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.txtUserNo);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(660, 100);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "信息";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(183, 71);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 22;
            this.button2.Text = "按用户查询";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(86, 71);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 21;
            this.button1.Text = "按卡查询";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // dateTimePicker2
            // 
            this.dateTimePicker2.CustomFormat = "yyyy-MM-dd";
            this.dateTimePicker2.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker2.Location = new System.Drawing.Point(521, 14);
            this.dateTimePicker2.Name = "dateTimePicker2";
            this.dateTimePicker2.Size = new System.Drawing.Size(100, 21);
            this.dateTimePicker2.TabIndex = 20;
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.CustomFormat = "yyyy-MM-dd";
            this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker1.Location = new System.Drawing.Point(327, 14);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(100, 21);
            this.dateTimePicker1.TabIndex = 19;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(450, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 18;
            this.label2.Text = "结束时间：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(256, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 17;
            this.label1.Text = "开始时间：";
            // 
            // txtTelephone
            // 
            this.txtTelephone.Location = new System.Drawing.Point(521, 41);
            this.txtTelephone.Name = "txtTelephone";
            this.txtTelephone.ReadOnly = true;
            this.txtTelephone.Size = new System.Drawing.Size(100, 21);
            this.txtTelephone.TabIndex = 16;
            // 
            // txtIdentityNumber
            // 
            this.txtIdentityNumber.Location = new System.Drawing.Point(86, 41);
            this.txtIdentityNumber.Name = "txtIdentityNumber";
            this.txtIdentityNumber.ReadOnly = true;
            this.txtIdentityNumber.Size = new System.Drawing.Size(140, 21);
            this.txtIdentityNumber.TabIndex = 15;
            // 
            // txtUserName
            // 
            this.txtUserName.Location = new System.Drawing.Point(327, 41);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.ReadOnly = true;
            this.txtUserName.Size = new System.Drawing.Size(100, 21);
            this.txtUserName.TabIndex = 14;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(474, 44);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(41, 12);
            this.label10.TabIndex = 13;
            this.label10.Text = "电话：";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(15, 44);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(65, 12);
            this.label9.TabIndex = 12;
            this.label9.Text = "身份证号：";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(268, 44);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 12);
            this.label8.TabIndex = 11;
            this.label8.Text = "用户名：";
            // 
            // txtUserNo
            // 
            this.txtUserNo.Location = new System.Drawing.Point(86, 14);
            this.txtUserNo.Name = "txtUserNo";
            this.txtUserNo.ReadOnly = true;
            this.txtUserNo.Size = new System.Drawing.Size(140, 21);
            this.txtUserNo.TabIndex = 10;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(39, 17);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(41, 12);
            this.label7.TabIndex = 9;
            this.label7.Text = "卡号：";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.dataGridView1);
            this.groupBox2.Location = new System.Drawing.Point(12, 118);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(660, 160);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "充值记录";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dg1_卡号,
            this.dg1_用户名,
            this.dg1_身份证,
            this.dg1_电话,
            this.dg1_充值时间,
            this.dg1_充水金额,
            this.dg1_充水数量,
            this.dg1_充电金额,
            this.dg1_充电数量,
            this.dg1_总金额,
            this.dg1_操作人});
            this.dataGridView1.Location = new System.Drawing.Point(6, 20);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(648, 134);
            this.dataGridView1.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.dataGridView2);
            this.groupBox3.Location = new System.Drawing.Point(12, 284);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(660, 165);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "用水记录";
            // 
            // dataGridView2
            // 
            this.dataGridView2.AllowUserToAddRows = false;
            this.dataGridView2.AllowUserToDeleteRows = false;
            this.dataGridView2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dg2_卡号,
            this.dg2_用户名,
            this.dg2_身份证,
            this.dg2_电话,
            this.dg2_设备序号,
            this.dg2_开泵时间,
            this.dg2_开泵剩余水量,
            this.dg2_开泵剩余电量,
            this.dg2_关泵时间,
            this.dg2_关泵剩余水量,
            this.dg2_关泵剩余电量,
            this.dg2_灌溉时长,
            this.dg2_本次用水,
            this.dg2_本次用电});
            this.dataGridView2.Location = new System.Drawing.Point(6, 20);
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.ReadOnly = true;
            this.dataGridView2.RowTemplate.Height = 23;
            this.dataGridView2.Size = new System.Drawing.Size(648, 139);
            this.dataGridView2.TabIndex = 1;
            // 
            // dg2_卡号
            // 
            this.dg2_卡号.DataPropertyName = "UserNo";
            this.dg2_卡号.Frozen = true;
            this.dg2_卡号.HeaderText = "卡号";
            this.dg2_卡号.Name = "dg2_卡号";
            this.dg2_卡号.ReadOnly = true;
            this.dg2_卡号.Width = 65;
            // 
            // dg2_用户名
            // 
            this.dg2_用户名.DataPropertyName = "UserName";
            this.dg2_用户名.Frozen = true;
            this.dg2_用户名.HeaderText = "用户名";
            this.dg2_用户名.Name = "dg2_用户名";
            this.dg2_用户名.ReadOnly = true;
            this.dg2_用户名.Width = 65;
            // 
            // dg2_身份证
            // 
            this.dg2_身份证.DataPropertyName = "IdentityNumber";
            this.dg2_身份证.HeaderText = "身份证";
            this.dg2_身份证.Name = "dg2_身份证";
            this.dg2_身份证.ReadOnly = true;
            this.dg2_身份证.Width = 120;
            // 
            // dg2_电话
            // 
            this.dg2_电话.DataPropertyName = "Telephone";
            this.dg2_电话.HeaderText = "电话";
            this.dg2_电话.Name = "dg2_电话";
            this.dg2_电话.ReadOnly = true;
            this.dg2_电话.Width = 80;
            // 
            // dg2_设备序号
            // 
            this.dg2_设备序号.DataPropertyName = "DeviceNo";
            this.dg2_设备序号.HeaderText = "设备序号";
            this.dg2_设备序号.Name = "dg2_设备序号";
            this.dg2_设备序号.ReadOnly = true;
            // 
            // dg2_开泵时间
            // 
            this.dg2_开泵时间.DataPropertyName = "StartTime";
            this.dg2_开泵时间.HeaderText = "开泵时间";
            this.dg2_开泵时间.Name = "dg2_开泵时间";
            this.dg2_开泵时间.ReadOnly = true;
            this.dg2_开泵时间.Width = 130;
            // 
            // dg2_开泵剩余水量
            // 
            this.dg2_开泵剩余水量.DataPropertyName = "StartResidualWater";
            this.dg2_开泵剩余水量.HeaderText = "开泵剩余水量";
            this.dg2_开泵剩余水量.Name = "dg2_开泵剩余水量";
            this.dg2_开泵剩余水量.ReadOnly = true;
            this.dg2_开泵剩余水量.Visible = false;
            // 
            // dg2_开泵剩余电量
            // 
            this.dg2_开泵剩余电量.DataPropertyName = "StartResidualElectric";
            this.dg2_开泵剩余电量.HeaderText = "开泵剩余电量";
            this.dg2_开泵剩余电量.Name = "dg2_开泵剩余电量";
            this.dg2_开泵剩余电量.ReadOnly = true;
            this.dg2_开泵剩余电量.Visible = false;
            // 
            // dg2_关泵时间
            // 
            this.dg2_关泵时间.DataPropertyName = "EndTime";
            this.dg2_关泵时间.HeaderText = "关泵时间";
            this.dg2_关泵时间.Name = "dg2_关泵时间";
            this.dg2_关泵时间.ReadOnly = true;
            this.dg2_关泵时间.Width = 130;
            // 
            // dg2_关泵剩余水量
            // 
            this.dg2_关泵剩余水量.DataPropertyName = "EndResidualWater";
            this.dg2_关泵剩余水量.HeaderText = "关泵剩余水量";
            this.dg2_关泵剩余水量.Name = "dg2_关泵剩余水量";
            this.dg2_关泵剩余水量.ReadOnly = true;
            this.dg2_关泵剩余水量.Visible = false;
            // 
            // dg2_关泵剩余电量
            // 
            this.dg2_关泵剩余电量.DataPropertyName = "EndResidualElectric";
            this.dg2_关泵剩余电量.HeaderText = "关泵剩余电量";
            this.dg2_关泵剩余电量.Name = "dg2_关泵剩余电量";
            this.dg2_关泵剩余电量.ReadOnly = true;
            this.dg2_关泵剩余电量.Visible = false;
            // 
            // dg2_灌溉时长
            // 
            this.dg2_灌溉时长.DataPropertyName = "Duration";
            this.dg2_灌溉时长.HeaderText = "灌溉时长";
            this.dg2_灌溉时长.Name = "dg2_灌溉时长";
            this.dg2_灌溉时长.ReadOnly = true;
            this.dg2_灌溉时长.Width = 80;
            // 
            // dg2_本次用水
            // 
            this.dg2_本次用水.DataPropertyName = "WaterUsed";
            this.dg2_本次用水.HeaderText = "本次用水";
            this.dg2_本次用水.Name = "dg2_本次用水";
            this.dg2_本次用水.ReadOnly = true;
            this.dg2_本次用水.Width = 80;
            // 
            // dg2_本次用电
            // 
            this.dg2_本次用电.DataPropertyName = "ElectricUsed";
            this.dg2_本次用电.HeaderText = "本次用电";
            this.dg2_本次用电.Name = "dg2_本次用电";
            this.dg2_本次用电.ReadOnly = true;
            this.dg2_本次用电.Width = 80;
            // 
            // dg1_卡号
            // 
            this.dg1_卡号.DataPropertyName = "UserNo";
            this.dg1_卡号.Frozen = true;
            this.dg1_卡号.HeaderText = "卡号";
            this.dg1_卡号.Name = "dg1_卡号";
            this.dg1_卡号.ReadOnly = true;
            this.dg1_卡号.Width = 65;
            // 
            // dg1_用户名
            // 
            this.dg1_用户名.DataPropertyName = "UserName";
            this.dg1_用户名.Frozen = true;
            this.dg1_用户名.HeaderText = "用户名";
            this.dg1_用户名.Name = "dg1_用户名";
            this.dg1_用户名.ReadOnly = true;
            this.dg1_用户名.Width = 65;
            // 
            // dg1_身份证
            // 
            this.dg1_身份证.DataPropertyName = "IdentityNumber";
            this.dg1_身份证.HeaderText = "身份证";
            this.dg1_身份证.Name = "dg1_身份证";
            this.dg1_身份证.ReadOnly = true;
            this.dg1_身份证.Width = 120;
            // 
            // dg1_电话
            // 
            this.dg1_电话.DataPropertyName = "Telephone";
            this.dg1_电话.HeaderText = "电话";
            this.dg1_电话.Name = "dg1_电话";
            this.dg1_电话.ReadOnly = true;
            this.dg1_电话.Width = 80;
            // 
            // dg1_充值时间
            // 
            this.dg1_充值时间.DataPropertyName = "LogTime";
            this.dg1_充值时间.HeaderText = "充值时间";
            this.dg1_充值时间.Name = "dg1_充值时间";
            this.dg1_充值时间.ReadOnly = true;
            this.dg1_充值时间.Width = 130;
            // 
            // dg1_充水金额
            // 
            this.dg1_充水金额.DataPropertyName = "WaterPrice";
            this.dg1_充水金额.HeaderText = "充水金额";
            this.dg1_充水金额.Name = "dg1_充水金额";
            this.dg1_充水金额.ReadOnly = true;
            this.dg1_充水金额.Width = 80;
            // 
            // dg1_充水数量
            // 
            this.dg1_充水数量.DataPropertyName = "WaterNum";
            this.dg1_充水数量.HeaderText = "充水数量";
            this.dg1_充水数量.Name = "dg1_充水数量";
            this.dg1_充水数量.ReadOnly = true;
            this.dg1_充水数量.Width = 80;
            // 
            // dg1_充电金额
            // 
            this.dg1_充电金额.DataPropertyName = "ElectricPrice";
            this.dg1_充电金额.HeaderText = "充电金额";
            this.dg1_充电金额.Name = "dg1_充电金额";
            this.dg1_充电金额.ReadOnly = true;
            this.dg1_充电金额.Width = 80;
            // 
            // dg1_充电数量
            // 
            this.dg1_充电数量.DataPropertyName = "ElectricNum";
            this.dg1_充电数量.HeaderText = "充电数量";
            this.dg1_充电数量.Name = "dg1_充电数量";
            this.dg1_充电数量.ReadOnly = true;
            this.dg1_充电数量.Width = 80;
            // 
            // dg1_总金额
            // 
            this.dg1_总金额.DataPropertyName = "TotalPrice";
            this.dg1_总金额.HeaderText = "总金额";
            this.dg1_总金额.Name = "dg1_总金额";
            this.dg1_总金额.ReadOnly = true;
            this.dg1_总金额.Width = 80;
            // 
            // dg1_操作人
            // 
            this.dg1_操作人.DataPropertyName = "LogUserName";
            this.dg1_操作人.HeaderText = "操作人";
            this.dg1_操作人.Name = "dg1_操作人";
            this.dg1_操作人.ReadOnly = true;
            this.dg1_操作人.Width = 80;
            // 
            // frmQuery
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 461);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.MinimumSize = new System.Drawing.Size(700, 500);
            this.Name = "frmQuery";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "充值用水查询";
            this.Load += new System.EventHandler(this.frmQuery_Load);
            this.SizeChanged += new System.EventHandler(this.frmQuery_SizeChanged);
            this.Resize += new System.EventHandler(this.frmQuery_Resize);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtTelephone;
        private System.Windows.Forms.TextBox txtIdentityNumber;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtUserNo;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DateTimePicker dateTimePicker2;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridView dataGridView2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dg1_卡号;
        private System.Windows.Forms.DataGridViewTextBoxColumn dg1_用户名;
        private System.Windows.Forms.DataGridViewTextBoxColumn dg1_身份证;
        private System.Windows.Forms.DataGridViewTextBoxColumn dg1_电话;
        private System.Windows.Forms.DataGridViewTextBoxColumn dg1_充值时间;
        private System.Windows.Forms.DataGridViewTextBoxColumn dg1_充水金额;
        private System.Windows.Forms.DataGridViewTextBoxColumn dg1_充水数量;
        private System.Windows.Forms.DataGridViewTextBoxColumn dg1_充电金额;
        private System.Windows.Forms.DataGridViewTextBoxColumn dg1_充电数量;
        private System.Windows.Forms.DataGridViewTextBoxColumn dg1_总金额;
        private System.Windows.Forms.DataGridViewTextBoxColumn dg1_操作人;
        private System.Windows.Forms.DataGridViewTextBoxColumn dg2_卡号;
        private System.Windows.Forms.DataGridViewTextBoxColumn dg2_用户名;
        private System.Windows.Forms.DataGridViewTextBoxColumn dg2_身份证;
        private System.Windows.Forms.DataGridViewTextBoxColumn dg2_电话;
        private System.Windows.Forms.DataGridViewTextBoxColumn dg2_设备序号;
        private System.Windows.Forms.DataGridViewTextBoxColumn dg2_开泵时间;
        private System.Windows.Forms.DataGridViewTextBoxColumn dg2_开泵剩余水量;
        private System.Windows.Forms.DataGridViewTextBoxColumn dg2_开泵剩余电量;
        private System.Windows.Forms.DataGridViewTextBoxColumn dg2_关泵时间;
        private System.Windows.Forms.DataGridViewTextBoxColumn dg2_关泵剩余水量;
        private System.Windows.Forms.DataGridViewTextBoxColumn dg2_关泵剩余电量;
        private System.Windows.Forms.DataGridViewTextBoxColumn dg2_灌溉时长;
        private System.Windows.Forms.DataGridViewTextBoxColumn dg2_本次用水;
        private System.Windows.Forms.DataGridViewTextBoxColumn dg2_本次用电;
    }
}