namespace SpaceArrow.ProgressIndication
{
    partial class FilePathManager
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
            this.label3 = new System.Windows.Forms.Label();
            this.textBox_Cachepath = new System.Windows.Forms.TextBox();
            this.textBox_datafilepath = new System.Windows.Forms.TextBox();
            this.textBox_logfilepath = new System.Windows.Forms.TextBox();
            this.button_changedatafilepath = new System.Windows.Forms.Button();
            this.checkBox_autocleardatafile = new System.Windows.Forms.CheckBox();
            this.comboBox_datafilesize = new System.Windows.Forms.ComboBox();
            this.button_cleardatafile = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.button_changelogfilepath = new System.Windows.Forms.Button();
            this.checkBox_autologfile = new System.Windows.Forms.CheckBox();
            this.comboBox_logfilesize = new System.Windows.Forms.ComboBox();
            this.button_clearlogfile = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(34, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "地图缓存目录:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "数据文件目录:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 17);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "日志文件目录:";
            // 
            // textBox_Cachepath
            // 
            this.textBox_Cachepath.Location = new System.Drawing.Point(123, 30);
            this.textBox_Cachepath.Name = "textBox_Cachepath";
            this.textBox_Cachepath.ReadOnly = true;
            this.textBox_Cachepath.Size = new System.Drawing.Size(317, 21);
            this.textBox_Cachepath.TabIndex = 3;
            // 
            // textBox_datafilepath
            // 
            this.textBox_datafilepath.Location = new System.Drawing.Point(100, 20);
            this.textBox_datafilepath.Name = "textBox_datafilepath";
            this.textBox_datafilepath.ReadOnly = true;
            this.textBox_datafilepath.Size = new System.Drawing.Size(317, 21);
            this.textBox_datafilepath.TabIndex = 4;
            // 
            // textBox_logfilepath
            // 
            this.textBox_logfilepath.Location = new System.Drawing.Point(100, 17);
            this.textBox_logfilepath.Name = "textBox_logfilepath";
            this.textBox_logfilepath.ReadOnly = true;
            this.textBox_logfilepath.Size = new System.Drawing.Size(317, 21);
            this.textBox_logfilepath.TabIndex = 5;
            // 
            // button_changedatafilepath
            // 
            this.button_changedatafilepath.Location = new System.Drawing.Point(423, 20);
            this.button_changedatafilepath.Name = "button_changedatafilepath";
            this.button_changedatafilepath.Size = new System.Drawing.Size(95, 23);
            this.button_changedatafilepath.TabIndex = 6;
            this.button_changedatafilepath.Text = "更改路径";
            this.button_changedatafilepath.UseVisualStyleBackColor = true;
            this.button_changedatafilepath.Click += new System.EventHandler(this.button_changedatafilepath_Click);
            // 
            // checkBox_autocleardatafile
            // 
            this.checkBox_autocleardatafile.AutoSize = true;
            this.checkBox_autocleardatafile.Location = new System.Drawing.Point(305, 53);
            this.checkBox_autocleardatafile.Name = "checkBox_autocleardatafile";
            this.checkBox_autocleardatafile.Size = new System.Drawing.Size(72, 16);
            this.checkBox_autocleardatafile.TabIndex = 8;
            this.checkBox_autocleardatafile.Text = "自动清理";
            this.checkBox_autocleardatafile.UseVisualStyleBackColor = true;
            // 
            // comboBox_datafilesize
            // 
            this.comboBox_datafilesize.FormattingEnabled = true;
            this.comboBox_datafilesize.Items.AddRange(new object[] {
            "3KB",
            "5KB",
            "10KB",
            "15KB",
            "20KB",
            "30KB",
            "50KB",
            "100KB"});
            this.comboBox_datafilesize.Location = new System.Drawing.Point(100, 46);
            this.comboBox_datafilesize.Name = "comboBox_datafilesize";
            this.comboBox_datafilesize.Size = new System.Drawing.Size(101, 20);
            this.comboBox_datafilesize.TabIndex = 9;
            // 
            // button_cleardatafile
            // 
            this.button_cleardatafile.Location = new System.Drawing.Point(207, 46);
            this.button_cleardatafile.Name = "button_cleardatafile";
            this.button_cleardatafile.Size = new System.Drawing.Size(75, 23);
            this.button_cleardatafile.TabIndex = 10;
            this.button_cleardatafile.Text = "清理文件";
            this.button_cleardatafile.UseVisualStyleBackColor = true;
            this.button_cleardatafile.Click += new System.EventHandler(this.button_cleardatafile_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.checkBox_autocleardatafile);
            this.groupBox1.Controls.Add(this.button_cleardatafile);
            this.groupBox1.Controls.Add(this.textBox_datafilepath);
            this.groupBox1.Controls.Add(this.comboBox_datafilesize);
            this.groupBox1.Controls.Add(this.button_changedatafilepath);
            this.groupBox1.Location = new System.Drawing.Point(23, 57);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(524, 77);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(35, 50);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 12);
            this.label4.TabIndex = 12;
            this.label4.Text = "文件阈值:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.button_changelogfilepath);
            this.groupBox2.Controls.Add(this.checkBox_autologfile);
            this.groupBox2.Controls.Add(this.comboBox_logfilesize);
            this.groupBox2.Controls.Add(this.button_clearlogfile);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.textBox_logfilepath);
            this.groupBox2.Location = new System.Drawing.Point(23, 140);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(524, 72);
            this.groupBox2.TabIndex = 12;
            this.groupBox2.TabStop = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(35, 46);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 12);
            this.label5.TabIndex = 14;
            this.label5.Text = "文件阈值:";
            // 
            // button_changelogfilepath
            // 
            this.button_changelogfilepath.Location = new System.Drawing.Point(423, 15);
            this.button_changelogfilepath.Name = "button_changelogfilepath";
            this.button_changelogfilepath.Size = new System.Drawing.Size(95, 23);
            this.button_changelogfilepath.TabIndex = 12;
            this.button_changelogfilepath.Text = "更改路径";
            this.button_changelogfilepath.UseVisualStyleBackColor = true;
            this.button_changelogfilepath.Click += new System.EventHandler(this.button_changelogfilepath_Click);
            // 
            // checkBox_autologfile
            // 
            this.checkBox_autologfile.AutoSize = true;
            this.checkBox_autologfile.Location = new System.Drawing.Point(305, 45);
            this.checkBox_autologfile.Name = "checkBox_autologfile";
            this.checkBox_autologfile.Size = new System.Drawing.Size(72, 16);
            this.checkBox_autologfile.TabIndex = 11;
            this.checkBox_autologfile.Text = "自动清理";
            this.checkBox_autologfile.UseVisualStyleBackColor = true;
            // 
            // comboBox_logfilesize
            // 
            this.comboBox_logfilesize.FormattingEnabled = true;
            this.comboBox_logfilesize.Items.AddRange(new object[] {
            "3KB",
            "5KB",
            "10KB",
            "15KB",
            "20KB",
            "30KB",
            "50KB",
            "100KB"});
            this.comboBox_logfilesize.Location = new System.Drawing.Point(100, 44);
            this.comboBox_logfilesize.Name = "comboBox_logfilesize";
            this.comboBox_logfilesize.Size = new System.Drawing.Size(101, 20);
            this.comboBox_logfilesize.TabIndex = 10;
            // 
            // button_clearlogfile
            // 
            this.button_clearlogfile.Location = new System.Drawing.Point(207, 41);
            this.button_clearlogfile.Name = "button_clearlogfile";
            this.button_clearlogfile.Size = new System.Drawing.Size(75, 23);
            this.button_clearlogfile.TabIndex = 9;
            this.button_clearlogfile.Text = "清理文件";
            this.button_clearlogfile.UseVisualStyleBackColor = true;
            this.button_clearlogfile.Click += new System.EventHandler(this.button_clearlogfile_Click);
            // 
            // FilePathManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(559, 230);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.textBox_Cachepath);
            this.Controls.Add(this.label1);
            this.Name = "FilePathManager";
            this.Text = "FilePathManager";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox_Cachepath;
        private System.Windows.Forms.TextBox textBox_datafilepath;
        private System.Windows.Forms.TextBox textBox_logfilepath;
        private System.Windows.Forms.Button button_changedatafilepath;
        private System.Windows.Forms.CheckBox checkBox_autocleardatafile;
        private System.Windows.Forms.ComboBox comboBox_datafilesize;
        private System.Windows.Forms.Button button_cleardatafile;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox checkBox_autologfile;
        private System.Windows.Forms.ComboBox comboBox_logfilesize;
        private System.Windows.Forms.Button button_clearlogfile;
        private System.Windows.Forms.Button button_changelogfilepath;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
    }
}