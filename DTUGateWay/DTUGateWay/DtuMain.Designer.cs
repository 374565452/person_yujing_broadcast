namespace DTUGateWay
{
    partial class DtuMain
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.SSLDateNow = new System.Windows.Forms.ToolStripStatusLabel();
            this.SSLDateStart = new System.Windows.Forms.ToolStripStatusLabel();
            this.copyrightToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.SSLState = new System.Windows.Forms.ToolStripStatusLabel();
            this.connectedStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.connectedStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolTripTime = new System.Windows.Forms.Timer(this.components);
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.districtTreeView = new System.Windows.Forms.TreeView();
            this.deviceListsDataGridView = new System.Windows.Forms.DataGridView();
            this.在线 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.设备序号 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.设备名称 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.设备类型 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.水文编号 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.所属村庄 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.主站编号 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.主站名称 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.数据时间 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.事件动作 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.报文描述 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.事件原始报文 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.查看日志ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.服务器配置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.测试ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.数据缓存ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.水位计测试ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.skinEngine12 = new Sunisoft.IrisSkin.SkinEngine();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.deviceListsDataGridView)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SSLDateNow,
            this.SSLDateStart,
            this.copyrightToolStripStatusLabel,
            this.SSLState,
            this.connectedStripStatusLabel1,
            this.connectedStripStatusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 703);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1008, 26);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // SSLDateNow
            // 
            this.SSLDateNow.AutoSize = false;
            this.SSLDateNow.ForeColor = System.Drawing.Color.Blue;
            this.SSLDateNow.Name = "SSLDateNow";
            this.SSLDateNow.Size = new System.Drawing.Size(180, 21);
            this.SSLDateNow.Text = "当前时间";
            // 
            // SSLDateStart
            // 
            this.SSLDateStart.AutoSize = false;
            this.SSLDateStart.ForeColor = System.Drawing.Color.Red;
            this.SSLDateStart.Name = "SSLDateStart";
            this.SSLDateStart.Size = new System.Drawing.Size(180, 21);
            this.SSLDateStart.Text = "启动时间";
            // 
            // copyrightToolStripStatusLabel
            // 
            this.copyrightToolStripStatusLabel.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.copyrightToolStripStatusLabel.Enabled = false;
            this.copyrightToolStripStatusLabel.Name = "copyrightToolStripStatusLabel";
            this.copyrightToolStripStatusLabel.Size = new System.Drawing.Size(402, 21);
            this.copyrightToolStripStatusLabel.Spring = true;
            this.copyrightToolStripStatusLabel.Text = "控制平台";
            // 
            // SSLState
            // 
            this.SSLState.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.SSLState.ForeColor = System.Drawing.Color.ForestGreen;
            this.SSLState.Name = "SSLState";
            this.SSLState.Size = new System.Drawing.Size(60, 21);
            this.SSLState.Text = "状态信息";
            // 
            // connectedStripStatusLabel1
            // 
            this.connectedStripStatusLabel1.AutoSize = false;
            this.connectedStripStatusLabel1.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.connectedStripStatusLabel1.Name = "connectedStripStatusLabel1";
            this.connectedStripStatusLabel1.Size = new System.Drawing.Size(80, 21);
            this.connectedStripStatusLabel1.Text = "在线总数：";
            // 
            // connectedStripStatusLabel
            // 
            this.connectedStripStatusLabel.AutoSize = false;
            this.connectedStripStatusLabel.ForeColor = System.Drawing.Color.Red;
            this.connectedStripStatusLabel.Name = "connectedStripStatusLabel";
            this.connectedStripStatusLabel.Size = new System.Drawing.Size(60, 21);
            this.connectedStripStatusLabel.Text = "1234";
            // 
            // toolTripTime
            // 
            this.toolTripTime.Enabled = true;
            this.toolTripTime.Interval = 1000;
            this.toolTripTime.Tick += new System.EventHandler(this.toolTripTime_Tick);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.districtTreeView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.deviceListsDataGridView);
            this.splitContainer1.Size = new System.Drawing.Size(1008, 678);
            this.splitContainer1.SplitterDistance = 215;
            this.splitContainer1.TabIndex = 3;
            // 
            // districtTreeView
            // 
            this.districtTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.districtTreeView.Location = new System.Drawing.Point(0, 0);
            this.districtTreeView.Name = "districtTreeView";
            this.districtTreeView.Size = new System.Drawing.Size(215, 678);
            this.districtTreeView.TabIndex = 0;
            // 
            // deviceListsDataGridView
            // 
            this.deviceListsDataGridView.AllowUserToAddRows = false;
            this.deviceListsDataGridView.AllowUserToDeleteRows = false;
            this.deviceListsDataGridView.BackgroundColor = System.Drawing.Color.White;
            this.deviceListsDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.deviceListsDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.deviceListsDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.在线,
            this.设备序号,
            this.设备名称,
            this.设备类型,
            this.水文编号,
            this.所属村庄,
            this.主站编号,
            this.主站名称,
            this.数据时间,
            this.事件动作,
            this.报文描述,
            this.事件原始报文});
            this.deviceListsDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.deviceListsDataGridView.Location = new System.Drawing.Point(0, 0);
            this.deviceListsDataGridView.MultiSelect = false;
            this.deviceListsDataGridView.Name = "deviceListsDataGridView";
            this.deviceListsDataGridView.ReadOnly = true;
            this.deviceListsDataGridView.RowHeadersWidth = 45;
            this.deviceListsDataGridView.RowTemplate.Height = 23;
            this.deviceListsDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.deviceListsDataGridView.Size = new System.Drawing.Size(789, 678);
            this.deviceListsDataGridView.TabIndex = 0;
            this.deviceListsDataGridView.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.deviceListsDataGridView_RowPostPaint);
            // 
            // 在线
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.在线.DefaultCellStyle = dataGridViewCellStyle2;
            this.在线.Frozen = true;
            this.在线.HeaderText = "在线";
            this.在线.Name = "在线";
            this.在线.ReadOnly = true;
            this.在线.Width = 55;
            // 
            // 设备序号
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.设备序号.DefaultCellStyle = dataGridViewCellStyle3;
            this.设备序号.Frozen = true;
            this.设备序号.HeaderText = "设备序号";
            this.设备序号.Name = "设备序号";
            this.设备序号.ReadOnly = true;
            // 
            // 设备名称
            // 
            this.设备名称.Frozen = true;
            this.设备名称.HeaderText = "设备名称";
            this.设备名称.Name = "设备名称";
            this.设备名称.ReadOnly = true;
            this.设备名称.Width = 80;
            // 
            // 设备类型
            // 
            this.设备类型.Frozen = true;
            this.设备类型.HeaderText = "设备类型";
            this.设备类型.Name = "设备类型";
            this.设备类型.ReadOnly = true;
            this.设备类型.Width = 80;
            // 
            // 水文编号
            // 
            this.水文编号.Frozen = true;
            this.水文编号.HeaderText = "水文编号";
            this.水文编号.Name = "水文编号";
            this.水文编号.ReadOnly = true;
            // 
            // 所属村庄
            // 
            this.所属村庄.HeaderText = "所属村庄";
            this.所属村庄.Name = "所属村庄";
            this.所属村庄.ReadOnly = true;
            this.所属村庄.Width = 80;
            // 
            // 主站编号
            // 
            this.主站编号.HeaderText = "主站编号";
            this.主站编号.Name = "主站编号";
            this.主站编号.ReadOnly = true;
            // 
            // 主站名称
            // 
            this.主站名称.HeaderText = "主站名称";
            this.主站名称.Name = "主站名称";
            this.主站名称.ReadOnly = true;
            this.主站名称.Width = 80;
            // 
            // 数据时间
            // 
            this.数据时间.HeaderText = "数据时间";
            this.数据时间.Name = "数据时间";
            this.数据时间.ReadOnly = true;
            this.数据时间.Width = 120;
            // 
            // 事件动作
            // 
            this.事件动作.HeaderText = "事件动作";
            this.事件动作.Name = "事件动作";
            this.事件动作.ReadOnly = true;
            this.事件动作.Width = 80;
            // 
            // 报文描述
            // 
            this.报文描述.HeaderText = "报文描述";
            this.报文描述.Name = "报文描述";
            this.报文描述.ReadOnly = true;
            this.报文描述.Width = 220;
            // 
            // 事件原始报文
            // 
            this.事件原始报文.HeaderText = "事件原始报文";
            this.事件原始报文.Name = "事件原始报文";
            this.事件原始报文.ReadOnly = true;
            this.事件原始报文.Visible = false;
            this.事件原始报文.Width = 200;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.查看日志ToolStripMenuItem,
            this.服务器配置ToolStripMenuItem,
            this.测试ToolStripMenuItem,
            this.数据缓存ToolStripMenuItem,
            this.水位计测试ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1008, 25);
            this.menuStrip1.TabIndex = 4;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 查看日志ToolStripMenuItem
            // 
            this.查看日志ToolStripMenuItem.Name = "查看日志ToolStripMenuItem";
            this.查看日志ToolStripMenuItem.Size = new System.Drawing.Size(68, 21);
            this.查看日志ToolStripMenuItem.Text = "查看日志";
            this.查看日志ToolStripMenuItem.Click += new System.EventHandler(this.查看日志ToolStripMenuItem_Click);
            // 
            // 服务器配置ToolStripMenuItem
            // 
            this.服务器配置ToolStripMenuItem.Name = "服务器配置ToolStripMenuItem";
            this.服务器配置ToolStripMenuItem.Size = new System.Drawing.Size(80, 21);
            this.服务器配置ToolStripMenuItem.Text = "服务器配置";
            this.服务器配置ToolStripMenuItem.Click += new System.EventHandler(this.服务器配置ToolStripMenuItem_Click);
            // 
            // 测试ToolStripMenuItem
            // 
            this.测试ToolStripMenuItem.Name = "测试ToolStripMenuItem";
            this.测试ToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.测试ToolStripMenuItem.Text = "测试";
            this.测试ToolStripMenuItem.Click += new System.EventHandler(this.测试ToolStripMenuItem_Click);
            // 
            // 数据缓存ToolStripMenuItem
            // 
            this.数据缓存ToolStripMenuItem.Name = "数据缓存ToolStripMenuItem";
            this.数据缓存ToolStripMenuItem.Size = new System.Drawing.Size(68, 21);
            this.数据缓存ToolStripMenuItem.Text = "数据缓存";
            this.数据缓存ToolStripMenuItem.Click += new System.EventHandler(this.数据缓存ToolStripMenuItem_Click);
            // 
            // 水位计测试ToolStripMenuItem
            // 
            this.水位计测试ToolStripMenuItem.Name = "水位计测试ToolStripMenuItem";
            this.水位计测试ToolStripMenuItem.Size = new System.Drawing.Size(80, 21);
            this.水位计测试ToolStripMenuItem.Text = "水位计测试";
            this.水位计测试ToolStripMenuItem.Click += new System.EventHandler(this.水位计测试ToolStripMenuItem_Click);
            // 
            // skinEngine12
            // 
            this.skinEngine12.@__DrawButtonFocusRectangle = true;
            this.skinEngine12.DisabledButtonTextColor = System.Drawing.Color.Gray;
            this.skinEngine12.DisabledMenuFontColor = System.Drawing.SystemColors.GrayText;
            this.skinEngine12.InactiveCaptionColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.skinEngine12.SerialNumber = "";
            this.skinEngine12.SkinFile = null;
            // 
            // DtuMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1008, 729);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "DtuMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "数据网关";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DtuMain_FormClosing);
            this.Load += new System.EventHandler(this.DtuMain_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.deviceListsDataGridView)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel SSLDateNow;
        private System.Windows.Forms.ToolStripStatusLabel copyrightToolStripStatusLabel;
        private System.Windows.Forms.ToolStripStatusLabel SSLState;
        private System.Windows.Forms.ToolStripStatusLabel connectedStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel connectedStripStatusLabel;
        private System.Windows.Forms.Timer toolTripTime;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView districtTreeView;
        private System.Windows.Forms.DataGridView deviceListsDataGridView;
        private System.Windows.Forms.ToolStripStatusLabel SSLDateStart;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 查看日志ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 服务器配置ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 测试ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 数据缓存ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 水位计测试ToolStripMenuItem;
        private Sunisoft.IrisSkin.SkinEngine skinEngine12;
        private System.Windows.Forms.DataGridViewTextBoxColumn 在线;
        private System.Windows.Forms.DataGridViewTextBoxColumn 设备序号;
        private System.Windows.Forms.DataGridViewTextBoxColumn 设备名称;
        private System.Windows.Forms.DataGridViewTextBoxColumn 设备类型;
        private System.Windows.Forms.DataGridViewTextBoxColumn 水文编号;
        private System.Windows.Forms.DataGridViewTextBoxColumn 所属村庄;
        private System.Windows.Forms.DataGridViewTextBoxColumn 主站编号;
        private System.Windows.Forms.DataGridViewTextBoxColumn 主站名称;
        private System.Windows.Forms.DataGridViewTextBoxColumn 数据时间;
        private System.Windows.Forms.DataGridViewTextBoxColumn 事件动作;
        private System.Windows.Forms.DataGridViewTextBoxColumn 报文描述;
        private System.Windows.Forms.DataGridViewTextBoxColumn 事件原始报文;
    }
}

