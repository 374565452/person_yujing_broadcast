namespace AudioRecording
{
    partial class ConfigFrm
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.ipTxt = new System.Windows.Forms.TextBox();
            this.portTxt = new System.Windows.Forms.TextBox();
            this.sureBtn = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.bitRate = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.bits = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "服务器IP：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(50, 74);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "端口：";
            // 
            // ipTxt
            // 
            this.ipTxt.Location = new System.Drawing.Point(97, 29);
            this.ipTxt.Name = "ipTxt";
            this.ipTxt.Size = new System.Drawing.Size(165, 21);
            this.ipTxt.TabIndex = 2;
            // 
            // portTxt
            // 
            this.portTxt.Location = new System.Drawing.Point(97, 71);
            this.portTxt.Name = "portTxt";
            this.portTxt.Size = new System.Drawing.Size(165, 21);
            this.portTxt.TabIndex = 3;
            // 
            // sureBtn
            // 
            this.sureBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.sureBtn.Location = new System.Drawing.Point(108, 221);
            this.sureBtn.Name = "sureBtn";
            this.sureBtn.Size = new System.Drawing.Size(75, 23);
            this.sureBtn.TabIndex = 4;
            this.sureBtn.Text = "确认";
            this.sureBtn.UseVisualStyleBackColor = true;
            this.sureBtn.Click += new System.EventHandler(this.sureBtn_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(38, 125);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "采样率：";
            // 
            // bitRate
            // 
            this.bitRate.FormattingEnabled = true;
            this.bitRate.Items.AddRange(new object[] {
            "8000",
            "11025",
            "22050",
            "32000",
            "44100",
            "47250",
            "48000"});
            this.bitRate.Location = new System.Drawing.Point(97, 120);
            this.bitRate.Name = "bitRate";
            this.bitRate.Size = new System.Drawing.Size(165, 20);
            this.bitRate.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(38, 168);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 5;
            this.label4.Text = "采样位数：";
            // 
            // bits
            // 
            this.bits.FormattingEnabled = true;
            this.bits.Items.AddRange(new object[] {
            "8",
            "16",
            "24"});
            this.bits.Location = new System.Drawing.Point(97, 165);
            this.bits.Name = "bits";
            this.bits.Size = new System.Drawing.Size(165, 20);
            this.bits.TabIndex = 7;
            // 
            // ConfigFrm
            // 
            this.AcceptButton = this.sureBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(306, 279);
            this.Controls.Add(this.bits);
            this.Controls.Add(this.bitRate);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.sureBtn);
            this.Controls.Add(this.portTxt);
            this.Controls.Add(this.ipTxt);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConfigFrm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "参数配置";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox ipTxt;
        private System.Windows.Forms.TextBox portTxt;
        private System.Windows.Forms.Button sureBtn;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox bitRate;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox bits;
    }
}