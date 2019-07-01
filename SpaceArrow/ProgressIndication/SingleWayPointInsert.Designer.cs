namespace SpaceArrow.ProgressIndication
{
    partial class SingleWayPointInsert
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
            this.button_10meter = new System.Windows.Forms.Button();
            this.button_20meter = new System.Windows.Forms.Button();
            this.button_50meter = new System.Windows.Forms.Button();
            this.button_100meter = new System.Windows.Forms.Button();
            this.button_30meter = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "高度：";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // button_10meter
            // 
            this.button_10meter.Location = new System.Drawing.Point(12, 52);
            this.button_10meter.Name = "button_10meter";
            this.button_10meter.Size = new System.Drawing.Size(50, 23);
            this.button_10meter.TabIndex = 2;
            this.button_10meter.Text = "10米";
            this.button_10meter.UseVisualStyleBackColor = true;
            this.button_10meter.Click += new System.EventHandler(this.button_10meter_Click);
            // 
            // button_20meter
            // 
            this.button_20meter.Location = new System.Drawing.Point(68, 52);
            this.button_20meter.Name = "button_20meter";
            this.button_20meter.Size = new System.Drawing.Size(53, 23);
            this.button_20meter.TabIndex = 3;
            this.button_20meter.Text = "20米";
            this.button_20meter.UseVisualStyleBackColor = true;
            this.button_20meter.Click += new System.EventHandler(this.button_20meter_Click);
            // 
            // button_50meter
            // 
            this.button_50meter.Location = new System.Drawing.Point(68, 81);
            this.button_50meter.Name = "button_50meter";
            this.button_50meter.Size = new System.Drawing.Size(53, 23);
            this.button_50meter.TabIndex = 4;
            this.button_50meter.Text = "50米";
            this.button_50meter.UseVisualStyleBackColor = true;
            this.button_50meter.Click += new System.EventHandler(this.button_50meter_Click);
            // 
            // button_100meter
            // 
            this.button_100meter.Location = new System.Drawing.Point(127, 83);
            this.button_100meter.Name = "button_100meter";
            this.button_100meter.Size = new System.Drawing.Size(53, 23);
            this.button_100meter.TabIndex = 5;
            this.button_100meter.Text = "100米";
            this.button_100meter.UseVisualStyleBackColor = true;
            this.button_100meter.Click += new System.EventHandler(this.button_100meter_Click);
            // 
            // button_30meter
            // 
            this.button_30meter.Location = new System.Drawing.Point(12, 81);
            this.button_30meter.Name = "button_30meter";
            this.button_30meter.Size = new System.Drawing.Size(50, 23);
            this.button_30meter.TabIndex = 7;
            this.button_30meter.Text = "30米";
            this.button_30meter.UseVisualStyleBackColor = true;
            this.button_30meter.Click += new System.EventHandler(this.button_30meter_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(59, 25);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(62, 21);
            this.textBox1.TabIndex = 1;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(127, 25);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(59, 23);
            this.button1.TabIndex = 9;
            this.button1.Text = "高度加";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(127, 54);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(59, 23);
            this.button2.TabIndex = 10;
            this.button2.Text = "高度减";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // SingleWayPointInsert
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(199, 114);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.button_30meter);
            this.Controls.Add(this.button_100meter);
            this.Controls.Add(this.button_50meter);
            this.Controls.Add(this.button_20meter);
            this.Controls.Add(this.button_10meter);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "SingleWayPointInsert";
            this.Text = "SingleWayPointInsert";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button_10meter;
        private System.Windows.Forms.Button button_20meter;
        private System.Windows.Forms.Button button_50meter;
        private System.Windows.Forms.Button button_100meter;
        private System.Windows.Forms.Button button_30meter;
        public System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}