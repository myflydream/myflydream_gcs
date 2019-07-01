namespace SpaceArrow
{
    partial class LoiterTimeForm
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
            this.comboBox_missionType = new System.Windows.Forms.ComboBox();
            this.textBox_missionSeconds = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox_missonRadius = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox_missionyaw = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox_missionlat = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBox_missionlng = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBox_missionalt = new System.Windows.Forms.TextBox();
            this.button_Cancel = new System.Windows.Forms.Button();
            this.button_OK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // comboBox_missionType
            // 
            this.comboBox_missionType.FormattingEnabled = true;
            this.comboBox_missionType.Location = new System.Drawing.Point(101, 48);
            this.comboBox_missionType.Name = "comboBox_missionType";
            this.comboBox_missionType.Size = new System.Drawing.Size(121, 20);
            this.comboBox_missionType.TabIndex = 0;
            this.comboBox_missionType.SelectedIndexChanged += new System.EventHandler(this.comboBox_missionType_SelectedIndexChanged);
            // 
            // textBox_missionSeconds
            // 
            this.textBox_missionSeconds.Location = new System.Drawing.Point(101, 74);
            this.textBox_missionSeconds.Name = "textBox_missionSeconds";
            this.textBox_missionSeconds.Size = new System.Drawing.Size(100, 21);
            this.textBox_missionSeconds.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 51);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "mission_type:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 82);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "Seconds:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(24, 108);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "Radius:";
            // 
            // textBox_missonRadius
            // 
            this.textBox_missonRadius.Location = new System.Drawing.Point(101, 105);
            this.textBox_missonRadius.Name = "textBox_missonRadius";
            this.textBox_missonRadius.Size = new System.Drawing.Size(100, 21);
            this.textBox_missonRadius.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(24, 135);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 12);
            this.label4.TabIndex = 7;
            this.label4.Text = "Yaw:";
            // 
            // textBox_missionyaw
            // 
            this.textBox_missionyaw.Location = new System.Drawing.Point(101, 132);
            this.textBox_missionyaw.Name = "textBox_missionyaw";
            this.textBox_missionyaw.Size = new System.Drawing.Size(100, 21);
            this.textBox_missionyaw.TabIndex = 6;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(24, 168);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 12);
            this.label5.TabIndex = 9;
            this.label5.Text = "Lat:";
            // 
            // textBox_missionlat
            // 
            this.textBox_missionlat.Location = new System.Drawing.Point(101, 159);
            this.textBox_missionlat.Name = "textBox_missionlat";
            this.textBox_missionlat.Size = new System.Drawing.Size(100, 21);
            this.textBox_missionlat.TabIndex = 8;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(24, 195);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(29, 12);
            this.label6.TabIndex = 11;
            this.label6.Text = "Lng:";
            // 
            // textBox_missionlng
            // 
            this.textBox_missionlng.Location = new System.Drawing.Point(101, 186);
            this.textBox_missionlng.Name = "textBox_missionlng";
            this.textBox_missionlng.Size = new System.Drawing.Size(100, 21);
            this.textBox_missionlng.TabIndex = 10;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(24, 222);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(29, 12);
            this.label7.TabIndex = 13;
            this.label7.Text = "ALT:";
            // 
            // textBox_missionalt
            // 
            this.textBox_missionalt.Location = new System.Drawing.Point(101, 213);
            this.textBox_missionalt.Name = "textBox_missionalt";
            this.textBox_missionalt.Size = new System.Drawing.Size(100, 21);
            this.textBox_missionalt.TabIndex = 12;
            // 
            // button_Cancel
            // 
            this.button_Cancel.Location = new System.Drawing.Point(48, 262);
            this.button_Cancel.Name = "button_Cancel";
            this.button_Cancel.Size = new System.Drawing.Size(75, 23);
            this.button_Cancel.TabIndex = 14;
            this.button_Cancel.Text = "取消";
            this.button_Cancel.UseVisualStyleBackColor = true;
            this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
            // 
            // button_OK
            // 
            this.button_OK.Location = new System.Drawing.Point(147, 262);
            this.button_OK.Name = "button_OK";
            this.button_OK.Size = new System.Drawing.Size(75, 23);
            this.button_OK.TabIndex = 15;
            this.button_OK.Text = "确定";
            this.button_OK.UseVisualStyleBackColor = true;
            this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
            // 
            // LoiterTimeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(277, 310);
            this.Controls.Add(this.button_OK);
            this.Controls.Add(this.button_Cancel);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.textBox_missionalt);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.textBox_missionlng);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBox_missionlat);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBox_missionyaw);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBox_missonRadius);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox_missionSeconds);
            this.Controls.Add(this.comboBox_missionType);
            this.Name = "LoiterTimeForm";
            this.Text = "LoiterTimeForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBox_missionType;
        private System.Windows.Forms.TextBox textBox_missionSeconds;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox_missonRadius;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox_missionyaw;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox_missionlat;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBox_missionlng;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBox_missionalt;
        private System.Windows.Forms.Button button_Cancel;
        private System.Windows.Forms.Button button_OK;
    }
}