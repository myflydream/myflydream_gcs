namespace SpaceArrow
{
    partial class UpLoadForm
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
            this.ProgressBar1 = new DMSkin.Metro.Controls.MetroProgressBar();
            this.dmButton_Cancel = new DMSkin.Controls.DMButton();
            this.SuspendLayout();
            // 
            // ProgressBar1
            // 
            this.ProgressBar1.Location = new System.Drawing.Point(60, 131);
            this.ProgressBar1.Name = "ProgressBar1";
            this.ProgressBar1.Size = new System.Drawing.Size(183, 29);
            this.ProgressBar1.TabIndex = 0;
            // 
            // dmButton_Cancel
            // 
            this.dmButton_Cancel.BackColor = System.Drawing.Color.Transparent;
            this.dmButton_Cancel.DM_DisabledColor = System.Drawing.Color.Empty;
            this.dmButton_Cancel.DM_DownColor = System.Drawing.Color.FromArgb(((int)(((byte)(9)))), ((int)(((byte)(140)))), ((int)(((byte)(188)))));
            this.dmButton_Cancel.DM_MoveColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(195)))), ((int)(((byte)(245)))));
            this.dmButton_Cancel.DM_NormalColor = System.Drawing.Color.FromArgb(((int)(((byte)(9)))), ((int)(((byte)(163)))), ((int)(((byte)(220)))));
            this.dmButton_Cancel.DM_Radius = 5;
            this.dmButton_Cancel.Image = null;
            this.dmButton_Cancel.Location = new System.Drawing.Point(117, 205);
            this.dmButton_Cancel.Name = "dmButton_Cancel";
            this.dmButton_Cancel.Size = new System.Drawing.Size(125, 45);
            this.dmButton_Cancel.TabIndex = 1;
            this.dmButton_Cancel.Text = "取消";
            this.dmButton_Cancel.UseVisualStyleBackColor = false;
            this.dmButton_Cancel.Click += new System.EventHandler(this.dmButton_Cancel_Click);
            // 
            // UpLoadForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(320, 291);
            this.Controls.Add(this.dmButton_Cancel);
            this.Controls.Add(this.ProgressBar1);
            this.Name = "UpLoadForm";
            this.Text = "UpLoadForm";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.UpLoadForm_Paint);
            this.ResumeLayout(false);

        }

        #endregion

        private DMSkin.Metro.Controls.MetroProgressBar ProgressBar1;
        private DMSkin.Controls.DMButton dmButton_Cancel;
    }
}