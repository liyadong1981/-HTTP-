namespace HttpPostTest
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
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.txtFilePath = new System.Windows.Forms.TextBox();
			this.txtUrl = new System.Windows.Forms.TextBox();
			this.numTrys = new System.Windows.Forms.NumericUpDown();
			this.numThreads = new System.Windows.Forms.NumericUpDown();
			this.txtSpac = new System.Windows.Forms.TextBox();
			this.msg = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.successC = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.label7 = new System.Windows.Forms.Label();
			this.responseTime = new System.Windows.Forms.TextBox();
			((System.ComponentModel.ISupportInitialize)(this.numTrys)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numThreads)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button1.Location = new System.Drawing.Point(427, 65);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(68, 23);
			this.button1.TabIndex = 0;
			this.button1.Text = "浏览";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// button2
			// 
			this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.button2.Location = new System.Drawing.Point(78, 216);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(97, 34);
			this.button2.TabIndex = 1;
			this.button2.Text = "开始";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click_1);
			// 
			// openFileDialog1
			// 
			this.openFileDialog1.FileName = "openFileDialog1";
			// 
			// txtFilePath
			// 
			this.txtFilePath.Location = new System.Drawing.Point(212, 67);
			this.txtFilePath.Name = "txtFilePath";
			this.txtFilePath.Size = new System.Drawing.Size(193, 21);
			this.txtFilePath.TabIndex = 2;
			this.txtFilePath.Text = "C:\\Users\\LiYaDong\\Desktop\\新建文本文档 (2).txt";
			// 
			// txtUrl
			// 
			this.txtUrl.Location = new System.Drawing.Point(168, 24);
			this.txtUrl.Name = "txtUrl";
			this.txtUrl.Size = new System.Drawing.Size(297, 21);
			this.txtUrl.TabIndex = 3;
			this.txtUrl.Text = "http://localhost:8080";
			// 
			// numTrys
			// 
			this.numTrys.Location = new System.Drawing.Point(427, 110);
			this.numTrys.Name = "numTrys";
			this.numTrys.Size = new System.Drawing.Size(120, 21);
			this.numTrys.TabIndex = 4;
			this.numTrys.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// numThreads
			// 
			this.numThreads.Location = new System.Drawing.Point(168, 110);
			this.numThreads.Name = "numThreads";
			this.numThreads.Size = new System.Drawing.Size(120, 21);
			this.numThreads.TabIndex = 5;
			this.numThreads.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
			// 
			// txtSpac
			// 
			this.txtSpac.Location = new System.Drawing.Point(168, 157);
			this.txtSpac.Name = "txtSpac";
			this.txtSpac.Size = new System.Drawing.Size(120, 21);
			this.txtSpac.TabIndex = 6;
			this.txtSpac.Text = "10";
			// 
			// msg
			// 
			this.msg.Dock = System.Windows.Forms.DockStyle.Fill;
			this.msg.Location = new System.Drawing.Point(0, 0);
			this.msg.Multiline = true;
			this.msg.Name = "msg";
			this.msg.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.msg.Size = new System.Drawing.Size(908, 515);
			this.msg.TabIndex = 7;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(88, 27);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(65, 12);
			this.label1.TabIndex = 8;
			this.label1.Text = "测试的地址";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(69, 70);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(137, 12);
			this.label2.TabIndex = 8;
			this.label2.Text = "post的数据(从文件读取)";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(76, 119);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(77, 12);
			this.label3.TabIndex = 8;
			this.label3.Text = "模拟用户数量";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(313, 112);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(101, 12);
			this.label4.TabIndex = 8;
			this.label4.Text = "用户重复发送次数";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(76, 160);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(77, 12);
			this.label5.TabIndex = 8;
			this.label5.Text = "用户发送间隔";
			// 
			// successC
			// 
			this.successC.Location = new System.Drawing.Point(427, 157);
			this.successC.Name = "successC";
			this.successC.Size = new System.Drawing.Size(100, 21);
			this.successC.TabIndex = 9;
			this.successC.Text = "0";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(349, 160);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(65, 12);
			this.label6.TabIndex = 8;
			this.label6.Text = "响应返回数";
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.label7);
			this.splitContainer1.Panel1.Controls.Add(this.responseTime);
			this.splitContainer1.Panel1.Controls.Add(this.button2);
			this.splitContainer1.Panel1.Controls.Add(this.successC);
			this.splitContainer1.Panel1.Controls.Add(this.button1);
			this.splitContainer1.Panel1.Controls.Add(this.label6);
			this.splitContainer1.Panel1.Controls.Add(this.txtFilePath);
			this.splitContainer1.Panel1.Controls.Add(this.label5);
			this.splitContainer1.Panel1.Controls.Add(this.txtUrl);
			this.splitContainer1.Panel1.Controls.Add(this.label4);
			this.splitContainer1.Panel1.Controls.Add(this.numTrys);
			this.splitContainer1.Panel1.Controls.Add(this.label3);
			this.splitContainer1.Panel1.Controls.Add(this.numThreads);
			this.splitContainer1.Panel1.Controls.Add(this.label2);
			this.splitContainer1.Panel1.Controls.Add(this.txtSpac);
			this.splitContainer1.Panel1.Controls.Add(this.label1);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.msg);
			this.splitContainer1.Size = new System.Drawing.Size(908, 782);
			this.splitContainer1.SplitterDistance = 259;
			this.splitContainer1.SplitterWidth = 8;
			this.splitContainer1.TabIndex = 10;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(569, 15);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(89, 12);
			this.label7.TabIndex = 11;
			this.label7.Text = "响应时间(毫秒)";
			// 
			// responseTime
			// 
			this.responseTime.Location = new System.Drawing.Point(681, 12);
			this.responseTime.Multiline = true;
			this.responseTime.Name = "responseTime";
			this.responseTime.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.responseTime.Size = new System.Drawing.Size(128, 222);
			this.responseTime.TabIndex = 10;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(908, 782);
			this.Controls.Add(this.splitContainer1);
			this.Name = "Form1";
			this.Text = "HTTPPost测试";
			((System.ComponentModel.ISupportInitialize)(this.numTrys)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numThreads)).EndInit();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel1.PerformLayout();
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.Panel2.PerformLayout();
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TextBox txtFilePath;
        private System.Windows.Forms.TextBox txtUrl;
        private System.Windows.Forms.NumericUpDown numTrys;
        private System.Windows.Forms.NumericUpDown numThreads;
        private System.Windows.Forms.TextBox txtSpac;
        private System.Windows.Forms.TextBox msg;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox successC;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox responseTime;
    }
}

