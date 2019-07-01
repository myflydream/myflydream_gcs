namespace SpaceArrow
{
    partial class PlayBackForm
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
            this.dmProgressBar1 = new DMSkin.Controls.DMProgressBar();
            this.button_playpause = new System.Windows.Forms.Button();
            this.button_Forward = new System.Windows.Forms.Button();
            this.button_Retreat = new System.Windows.Forms.Button();
            this.button_ChoiceDataFile = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label_playstatus = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // dmProgressBar1
            // 
            this.dmProgressBar1.BackColor = System.Drawing.Color.Transparent;
            this.dmProgressBar1.DM_BackColor = System.Drawing.Color.Silver;
            this.dmProgressBar1.DM_BlockColor = System.Drawing.Color.Aqua;
            this.dmProgressBar1.DM_BufferColor = System.Drawing.Color.Empty;
            this.dmProgressBar1.DM_BufferValue = 0D;
            this.dmProgressBar1.DM_DrawRound = true;
            this.dmProgressBar1.Dm_OperationModel = DMSkin.Controls.DMProgressBar.OperationModel.Slide;
            this.dmProgressBar1.DM_RoundColor = System.Drawing.Color.Silver;
            this.dmProgressBar1.DM_RoundSize = 10;
            this.dmProgressBar1.DM_RoundX = 2;
            this.dmProgressBar1.DM_RoundY = 7;
            this.dmProgressBar1.DM_Value = 0D;
            this.dmProgressBar1.Location = new System.Drawing.Point(23, 83);
            this.dmProgressBar1.Name = "dmProgressBar1";
            this.dmProgressBar1.Size = new System.Drawing.Size(320, 25);
            this.dmProgressBar1.TabIndex = 2;
            this.dmProgressBar1.Text = "dmProgressBar1";
            // 
            // button_playpause
            // 
            this.button_playpause.Location = new System.Drawing.Point(149, 114);
            this.button_playpause.Name = "button_playpause";
            this.button_playpause.Size = new System.Drawing.Size(44, 23);
            this.button_playpause.TabIndex = 3;
            this.button_playpause.Text = "播放";
            this.button_playpause.UseVisualStyleBackColor = true;
            this.button_playpause.Click += new System.EventHandler(this.button_playpause_Click);
            // 
            // button_Forward
            // 
            this.button_Forward.Location = new System.Drawing.Point(199, 114);
            this.button_Forward.Name = "button_Forward";
            this.button_Forward.Size = new System.Drawing.Size(59, 23);
            this.button_Forward.TabIndex = 4;
            this.button_Forward.Text = "前进5秒";
            this.button_Forward.UseVisualStyleBackColor = true;
            this.button_Forward.Click += new System.EventHandler(this.button_Forward_Click);
            // 
            // button_Retreat
            // 
            this.button_Retreat.Location = new System.Drawing.Point(84, 114);
            this.button_Retreat.Name = "button_Retreat";
            this.button_Retreat.Size = new System.Drawing.Size(59, 23);
            this.button_Retreat.TabIndex = 5;
            this.button_Retreat.Text = "后退5秒";
            this.button_Retreat.UseVisualStyleBackColor = true;
            this.button_Retreat.Click += new System.EventHandler(this.button_Retreat_Click);
            // 
            // button_ChoiceDataFile
            // 
            this.button_ChoiceDataFile.Location = new System.Drawing.Point(23, 54);
            this.button_ChoiceDataFile.Name = "button_ChoiceDataFile";
            this.button_ChoiceDataFile.Size = new System.Drawing.Size(309, 23);
            this.button_ChoiceDataFile.TabIndex = 6;
            this.button_ChoiceDataFile.Text = "选择数据文件";
            this.button_ChoiceDataFile.UseVisualStyleBackColor = true;
            this.button_ChoiceDataFile.Click += new System.EventHandler(this.button_ChoiceDataFile_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 111);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 7;
            this.label1.Text = "label1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(277, 111);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 8;
            this.label2.Text = "label2";
            // 
            // label_playstatus
            // 
            this.label_playstatus.AutoSize = true;
            this.label_playstatus.Location = new System.Drawing.Point(147, 15);
            this.label_playstatus.Name = "label_playstatus";
            this.label_playstatus.Size = new System.Drawing.Size(29, 12);
            this.label_playstatus.TabIndex = 9;
            this.label_playstatus.Text = "暂停";
            // 
            // PlayBackForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(355, 159);
            this.Controls.Add(this.label_playstatus);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button_ChoiceDataFile);
            this.Controls.Add(this.button_Retreat);
            this.Controls.Add(this.button_Forward);
            this.Controls.Add(this.button_playpause);
            this.Controls.Add(this.dmProgressBar1);
            this.Name = "PlayBackForm";
            this.Text = "PlayBackForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DMSkin.Controls.DMProgressBar dmProgressBar1;
        private System.Windows.Forms.Button button_playpause;
        private System.Windows.Forms.Button button_Forward;
        private System.Windows.Forms.Button button_Retreat;
        private System.Windows.Forms.Button button_ChoiceDataFile;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label_playstatus;
    }
}