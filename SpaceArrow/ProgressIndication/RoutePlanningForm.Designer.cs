namespace SpaceArrow.ProgressIndication
{
    partial class RoutePlanningForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RoutePlanningForm));
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_distance = new System.Windows.Forms.TextBox();
            this.textBox_showdirection = new System.Windows.Forms.TextBox();
            this.textBox_totaldistance = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.button_startplan = new System.Windows.Forms.Button();
            this.button_close = new System.Windows.Forms.Button();
            this.button_Replan = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox_height = new System.Windows.Forms.TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage_waypoint = new System.Windows.Forms.TabPage();
            this.dataGridView_waypointlist = new System.Windows.Forms.DataGridView();
            this.number = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.height = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.isStart = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.tabpage_pointline = new System.Windows.Forms.TabPage();
            this.dataGridView_linepointlist = new System.Windows.Forms.DataGridView();
            this.linenumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.linedirection = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.linetwodot = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.isDirection = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.textBox_Coincidencerate_height = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.textBox_Coincidencerate_width = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textBox_camerafield_height = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.radioButton_camerafieldset = new System.Windows.Forms.RadioButton();
            this.radioButton_distanceset = new System.Windows.Forms.RadioButton();
            this.textBox_camerafiled_width = new System.Windows.Forms.TextBox();
            this.checkBox_startphoto = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBox_photodistance = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.TrackBar_Coincidencerate_height = new SpaceArrow.Control.CustomTrackBar();
            this.TrackBar_Coincidencerate_width = new SpaceArrow.Control.CustomTrackBar();
            this.TrackBar_camerafield_height = new SpaceArrow.Control.CustomTrackBar();
            this.TrackBar_camerafield_width = new SpaceArrow.Control.CustomTrackBar();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage_waypoint.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_waypointlist)).BeginInit();
            this.tabpage_pointline.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_linepointlist)).BeginInit();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(16, 281);
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(297, 45);
            this.trackBar1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(157, 329);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "航线间距：";
            // 
            // textBox_distance
            // 
            this.textBox_distance.Location = new System.Drawing.Point(228, 323);
            this.textBox_distance.Name = "textBox_distance";
            this.textBox_distance.Size = new System.Drawing.Size(76, 21);
            this.textBox_distance.TabIndex = 2;
            // 
            // textBox_showdirection
            // 
            this.textBox_showdirection.Location = new System.Drawing.Point(85, 323);
            this.textBox_showdirection.Name = "textBox_showdirection";
            this.textBox_showdirection.Size = new System.Drawing.Size(66, 21);
            this.textBox_showdirection.TabIndex = 3;
            this.textBox_showdirection.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_showDirection_keypress);
            // 
            // textBox_totaldistance
            // 
            this.textBox_totaldistance.Location = new System.Drawing.Point(228, 363);
            this.textBox_totaldistance.Name = "textBox_totaldistance";
            this.textBox_totaldistance.ReadOnly = true;
            this.textBox_totaldistance.Size = new System.Drawing.Size(75, 21);
            this.textBox_totaldistance.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 326);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 5;
            this.label2.Text = "飞行方向：";
            // 
            // button_startplan
            // 
            this.button_startplan.Location = new System.Drawing.Point(136, 397);
            this.button_startplan.Name = "button_startplan";
            this.button_startplan.Size = new System.Drawing.Size(75, 23);
            this.button_startplan.TabIndex = 6;
            this.button_startplan.Text = "开始规划";
            this.button_startplan.UseVisualStyleBackColor = true;
            this.button_startplan.Click += new System.EventHandler(this.button_startplan_Click);
            // 
            // button_close
            // 
            this.button_close.Location = new System.Drawing.Point(228, 397);
            this.button_close.Name = "button_close";
            this.button_close.Size = new System.Drawing.Size(75, 23);
            this.button_close.TabIndex = 7;
            this.button_close.Text = "关闭";
            this.button_close.UseVisualStyleBackColor = true;
            this.button_close.Click += new System.EventHandler(this.button_close_Click);
            // 
            // button_Replan
            // 
            this.button_Replan.Location = new System.Drawing.Point(38, 397);
            this.button_Replan.Name = "button_Replan";
            this.button_Replan.Size = new System.Drawing.Size(75, 23);
            this.button_Replan.TabIndex = 8;
            this.button_Replan.Text = "重新规划";
            this.button_Replan.UseVisualStyleBackColor = true;
            this.button_Replan.Click += new System.EventHandler(this.button_Replan_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(158, 363);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 9;
            this.label3.Text = "总航程：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(24, 363);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 12);
            this.label4.TabIndex = 10;
            this.label4.Text = "巡航高度:";
            // 
            // textBox_height
            // 
            this.textBox_height.Location = new System.Drawing.Point(84, 360);
            this.textBox_height.Name = "textBox_height";
            this.textBox_height.Size = new System.Drawing.Size(67, 21);
            this.textBox_height.TabIndex = 11;
            this.textBox_height.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textbox_height_keypress);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage_waypoint);
            this.tabControl1.Controls.Add(this.tabpage_pointline);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Location = new System.Drawing.Point(16, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(297, 250);
            this.tabControl1.TabIndex = 12;
            // 
            // tabPage_waypoint
            // 
            this.tabPage_waypoint.Controls.Add(this.dataGridView_waypointlist);
            this.tabPage_waypoint.Location = new System.Drawing.Point(4, 22);
            this.tabPage_waypoint.Name = "tabPage_waypoint";
            this.tabPage_waypoint.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_waypoint.Size = new System.Drawing.Size(289, 224);
            this.tabPage_waypoint.TabIndex = 0;
            this.tabPage_waypoint.Text = "航点";
            this.tabPage_waypoint.UseVisualStyleBackColor = true;
            // 
            // dataGridView_waypointlist
            // 
            this.dataGridView_waypointlist.AllowUserToAddRows = false;
            this.dataGridView_waypointlist.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_waypointlist.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.number,
            this.height,
            this.isStart});
            this.dataGridView_waypointlist.Location = new System.Drawing.Point(6, 6);
            this.dataGridView_waypointlist.Name = "dataGridView_waypointlist";
            this.dataGridView_waypointlist.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dataGridView_waypointlist.RowHeadersVisible = false;
            this.dataGridView_waypointlist.RowTemplate.Height = 23;
            this.dataGridView_waypointlist.Size = new System.Drawing.Size(277, 212);
            this.dataGridView_waypointlist.TabIndex = 0;
            // 
            // number
            // 
            this.number.HeaderText = "编号";
            this.number.Name = "number";
            this.number.ReadOnly = true;
            this.number.Width = 80;
            // 
            // height
            // 
            this.height.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.height.HeaderText = "巡航高度";
            this.height.Name = "height";
            this.height.ToolTipText = "巡航时，飞行器飞行高度";
            this.height.Width = 78;
            // 
            // isStart
            // 
            this.isStart.HeaderText = "起始航点";
            this.isStart.Name = "isStart";
            this.isStart.ToolTipText = "确定是否将该点设置为巡航时的起始点";
            // 
            // tabpage_pointline
            // 
            this.tabpage_pointline.Controls.Add(this.dataGridView_linepointlist);
            this.tabpage_pointline.Location = new System.Drawing.Point(4, 22);
            this.tabpage_pointline.Name = "tabpage_pointline";
            this.tabpage_pointline.Padding = new System.Windows.Forms.Padding(3);
            this.tabpage_pointline.Size = new System.Drawing.Size(289, 224);
            this.tabpage_pointline.TabIndex = 1;
            this.tabpage_pointline.Text = "航线";
            this.tabpage_pointline.UseVisualStyleBackColor = true;
            // 
            // dataGridView_linepointlist
            // 
            this.dataGridView_linepointlist.AllowUserToAddRows = false;
            this.dataGridView_linepointlist.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_linepointlist.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.linenumber,
            this.linedirection,
            this.linetwodot,
            this.isDirection});
            this.dataGridView_linepointlist.Location = new System.Drawing.Point(9, 6);
            this.dataGridView_linepointlist.Name = "dataGridView_linepointlist";
            this.dataGridView_linepointlist.RowHeadersVisible = false;
            this.dataGridView_linepointlist.RowTemplate.Height = 23;
            this.dataGridView_linepointlist.Size = new System.Drawing.Size(277, 197);
            this.dataGridView_linepointlist.TabIndex = 1;
            // 
            // linenumber
            // 
            this.linenumber.HeaderText = "线段";
            this.linenumber.Name = "linenumber";
            this.linenumber.Width = 60;
            // 
            // linedirection
            // 
            this.linedirection.HeaderText = "方向";
            this.linedirection.Name = "linedirection";
            this.linedirection.ReadOnly = true;
            this.linedirection.Width = 90;
            // 
            // linetwodot
            // 
            this.linetwodot.HeaderText = "端点";
            this.linetwodot.Name = "linetwodot";
            this.linetwodot.Width = 60;
            // 
            // isDirection
            // 
            this.isDirection.HeaderText = "巡航方向";
            this.isDirection.Name = "isDirection";
            this.isDirection.Width = 60;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.textBox_Coincidencerate_height);
            this.tabPage1.Controls.Add(this.label9);
            this.tabPage1.Controls.Add(this.textBox_Coincidencerate_width);
            this.tabPage1.Controls.Add(this.label8);
            this.tabPage1.Controls.Add(this.textBox_camerafield_height);
            this.tabPage1.Controls.Add(this.label7);
            this.tabPage1.Controls.Add(this.radioButton_camerafieldset);
            this.tabPage1.Controls.Add(this.radioButton_distanceset);
            this.tabPage1.Controls.Add(this.textBox_camerafiled_width);
            this.tabPage1.Controls.Add(this.checkBox_startphoto);
            this.tabPage1.Controls.Add(this.label6);
            this.tabPage1.Controls.Add(this.textBox_photodistance);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.TrackBar_Coincidencerate_height);
            this.tabPage1.Controls.Add(this.TrackBar_Coincidencerate_width);
            this.tabPage1.Controls.Add(this.TrackBar_camerafield_height);
            this.tabPage1.Controls.Add(this.TrackBar_camerafield_width);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(289, 224);
            this.tabPage1.TabIndex = 2;
            this.tabPage1.Text = "拍照";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // textBox_Coincidencerate_height
            // 
            this.textBox_Coincidencerate_height.Location = new System.Drawing.Point(235, 147);
            this.textBox_Coincidencerate_height.Name = "textBox_Coincidencerate_height";
            this.textBox_Coincidencerate_height.Size = new System.Drawing.Size(35, 21);
            this.textBox_Coincidencerate_height.TabIndex = 27;
            this.textBox_Coincidencerate_height.Text = "0";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 155);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(71, 12);
            this.label9.TabIndex = 26;
            this.label9.Text = "长度重合率:";
            // 
            // textBox_Coincidencerate_width
            // 
            this.textBox_Coincidencerate_width.Location = new System.Drawing.Point(235, 120);
            this.textBox_Coincidencerate_width.Name = "textBox_Coincidencerate_width";
            this.textBox_Coincidencerate_width.Size = new System.Drawing.Size(35, 21);
            this.textBox_Coincidencerate_width.TabIndex = 21;
            this.textBox_Coincidencerate_width.Text = "0";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(4, 123);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(71, 12);
            this.label8.TabIndex = 19;
            this.label8.Text = "宽度重合率:";
            // 
            // textBox_camerafield_height
            // 
            this.textBox_camerafield_height.Location = new System.Drawing.Point(235, 93);
            this.textBox_camerafield_height.Name = "textBox_camerafield_height";
            this.textBox_camerafield_height.Size = new System.Drawing.Size(35, 21);
            this.textBox_camerafield_height.TabIndex = 18;
            this.textBox_camerafield_height.Text = "94";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(15, 96);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(59, 12);
            this.label7.TabIndex = 16;
            this.label7.Text = "视野长度:";
            // 
            // radioButton_camerafieldset
            // 
            this.radioButton_camerafieldset.AutoSize = true;
            this.radioButton_camerafieldset.Location = new System.Drawing.Point(175, 6);
            this.radioButton_camerafieldset.Name = "radioButton_camerafieldset";
            this.radioButton_camerafieldset.Size = new System.Drawing.Size(71, 16);
            this.radioButton_camerafieldset.TabIndex = 14;
            this.radioButton_camerafieldset.TabStop = true;
            this.radioButton_camerafieldset.Text = "视野设定";
            this.radioButton_camerafieldset.UseVisualStyleBackColor = true;
            this.radioButton_camerafieldset.CheckedChanged += new System.EventHandler(this.radioButton_camerafieldset_CheckedChanged);
            // 
            // radioButton_distanceset
            // 
            this.radioButton_distanceset.AutoSize = true;
            this.radioButton_distanceset.Location = new System.Drawing.Point(97, 6);
            this.radioButton_distanceset.Name = "radioButton_distanceset";
            this.radioButton_distanceset.Size = new System.Drawing.Size(71, 16);
            this.radioButton_distanceset.TabIndex = 13;
            this.radioButton_distanceset.TabStop = true;
            this.radioButton_distanceset.Text = "距离设定";
            this.radioButton_distanceset.UseVisualStyleBackColor = true;
            this.radioButton_distanceset.CheckedChanged += new System.EventHandler(this.radioButton_distanceset_CheckedChanged);
            // 
            // textBox_camerafiled_width
            // 
            this.textBox_camerafiled_width.Location = new System.Drawing.Point(235, 64);
            this.textBox_camerafiled_width.Name = "textBox_camerafiled_width";
            this.textBox_camerafiled_width.Size = new System.Drawing.Size(35, 21);
            this.textBox_camerafiled_width.TabIndex = 12;
            this.textBox_camerafiled_width.Text = "90";
            this.textBox_camerafiled_width.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_camerafield_keypress);
            // 
            // checkBox_startphoto
            // 
            this.checkBox_startphoto.AutoSize = true;
            this.checkBox_startphoto.Location = new System.Drawing.Point(18, 6);
            this.checkBox_startphoto.Name = "checkBox_startphoto";
            this.checkBox_startphoto.Size = new System.Drawing.Size(72, 16);
            this.checkBox_startphoto.TabIndex = 11;
            this.checkBox_startphoto.Text = "启动拍照";
            this.checkBox_startphoto.UseVisualStyleBackColor = true;
            this.checkBox_startphoto.CheckedChanged += new System.EventHandler(this.checkBox_startphoto_CheckedChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(15, 67);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(59, 12);
            this.label6.TabIndex = 9;
            this.label6.Text = "视野宽度:";
            // 
            // textBox_photodistance
            // 
            this.textBox_photodistance.Location = new System.Drawing.Point(81, 34);
            this.textBox_photodistance.Name = "textBox_photodistance";
            this.textBox_photodistance.Size = new System.Drawing.Size(100, 21);
            this.textBox_photodistance.TabIndex = 8;
            this.textBox_photodistance.Text = "40m";
            this.textBox_photodistance.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textbox_takephoto_keypress);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 34);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 12);
            this.label5.TabIndex = 7;
            this.label5.Text = "拍照间距:";
            // 
            // TrackBar_Coincidencerate_height
            // 
            this.TrackBar_Coincidencerate_height.Location = new System.Drawing.Point(74, 147);
            this.TrackBar_Coincidencerate_height.MaxValue = 80;
            this.TrackBar_Coincidencerate_height.MinValue = 0;
            this.TrackBar_Coincidencerate_height.Name = "TrackBar_Coincidencerate_height";
            this.TrackBar_Coincidencerate_height.Size = new System.Drawing.Size(155, 21);
            this.TrackBar_Coincidencerate_height.TabIndex = 25;
            this.TrackBar_Coincidencerate_height.Value = 0;
            this.TrackBar_Coincidencerate_height.ValueChanged += new System.EventHandler(this.TrackBar_Coincidencerate_height_ValueChanged);
            // 
            // TrackBar_Coincidencerate_width
            // 
            this.TrackBar_Coincidencerate_width.Location = new System.Drawing.Point(74, 120);
            this.TrackBar_Coincidencerate_width.MaxValue = 80;
            this.TrackBar_Coincidencerate_width.MinValue = 0;
            this.TrackBar_Coincidencerate_width.Name = "TrackBar_Coincidencerate_width";
            this.TrackBar_Coincidencerate_width.Size = new System.Drawing.Size(155, 21);
            this.TrackBar_Coincidencerate_width.TabIndex = 24;
            this.TrackBar_Coincidencerate_width.Value = 0;
            this.TrackBar_Coincidencerate_width.ValueChanged += new System.EventHandler(this.TrackBar_Coincidencerate_width_ValueChanged);
            // 
            // TrackBar_camerafield_height
            // 
            this.TrackBar_camerafield_height.Location = new System.Drawing.Point(74, 93);
            this.TrackBar_camerafield_height.MaxValue = 150;
            this.TrackBar_camerafield_height.MinValue = 60;
            this.TrackBar_camerafield_height.Name = "TrackBar_camerafield_height";
            this.TrackBar_camerafield_height.Size = new System.Drawing.Size(155, 21);
            this.TrackBar_camerafield_height.TabIndex = 23;
            this.TrackBar_camerafield_height.Value = 94;
            this.TrackBar_camerafield_height.ValueChanged += new System.EventHandler(this.trackBar_camerafield_height_valuechange);
            // 
            // TrackBar_camerafield_width
            // 
            this.TrackBar_camerafield_width.Location = new System.Drawing.Point(74, 64);
            this.TrackBar_camerafield_width.MaxValue = 120;
            this.TrackBar_camerafield_width.MinValue = 60;
            this.TrackBar_camerafield_width.Name = "TrackBar_camerafield_width";
            this.TrackBar_camerafield_width.Size = new System.Drawing.Size(155, 21);
            this.TrackBar_camerafield_width.TabIndex = 22;
            this.TrackBar_camerafield_width.Value = 90;
            this.TrackBar_camerafield_width.ValueChanged += new System.EventHandler(this.trackbar_camerafield_width_valuechange);
            // 
            // RoutePlanningForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(329, 428);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.textBox_height);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.button_Replan);
            this.Controls.Add(this.button_close);
            this.Controls.Add(this.button_startplan);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox_totaldistance);
            this.Controls.Add(this.textBox_showdirection);
            this.Controls.Add(this.textBox_distance);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.trackBar1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "RoutePlanningForm";
            this.Text = "RoutePlanningForm";
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage_waypoint.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_waypointlist)).EndInit();
            this.tabpage_pointline.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_linepointlist)).EndInit();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_distance;
        private System.Windows.Forms.TextBox textBox_showdirection;
        private System.Windows.Forms.TextBox textBox_totaldistance;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button_startplan;
        private System.Windows.Forms.Button button_close;
        private System.Windows.Forms.Button button_Replan;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox_height;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage_waypoint;
        private System.Windows.Forms.TabPage tabpage_pointline;
        private System.Windows.Forms.DataGridView dataGridView_waypointlist;
        private System.Windows.Forms.DataGridView dataGridView_linepointlist;
        private System.Windows.Forms.DataGridViewTextBoxColumn number;
        private System.Windows.Forms.DataGridViewTextBoxColumn height;
        private System.Windows.Forms.DataGridViewCheckBoxColumn isStart;
        private System.Windows.Forms.DataGridViewTextBoxColumn linenumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn linedirection;
        private System.Windows.Forms.DataGridViewTextBoxColumn linetwodot;
        private System.Windows.Forms.DataGridViewCheckBoxColumn isDirection;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TextBox textBox_camerafiled_width;
        private System.Windows.Forms.CheckBox checkBox_startphoto;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBox_photodistance;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RadioButton radioButton_camerafieldset;
        private System.Windows.Forms.RadioButton radioButton_distanceset;
        private System.Windows.Forms.TextBox textBox_camerafield_height;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBox_Coincidencerate_width;
        private System.Windows.Forms.Label label8;
        private Control.CustomTrackBar TrackBar_camerafield_width;
        private System.Windows.Forms.Label label9;
        private Control.CustomTrackBar TrackBar_Coincidencerate_height;
        private Control.CustomTrackBar TrackBar_Coincidencerate_width;
        private Control.CustomTrackBar TrackBar_camerafield_height;
        private System.Windows.Forms.TextBox textBox_Coincidencerate_height;

    }
}