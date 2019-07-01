using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SpaceArrow.ProgressIndication
{
    public partial class RoutePlanningForm : Form
    {
        public delegate void PlanningDirectionDele(int flightangle,float routessistance );

        public PlanningDirectionDele PlanningDirection = null;

        public delegate void RePlanDele();
        public RePlanDele Replan = null;
        public RePlanDele FormClose = null;

        public delegate void UpdateHeightDele(int height);
        public UpdateHeightDele UpdateHeight = null;

        public UpdateHeightDele WaypointClick = null;
        public UpdateHeightDele LinepointClick = null;
        bool checkboxin = false;
        List<DataGridViewCheckBoxCell> waypointcheckbox = new List<DataGridViewCheckBoxCell>();
        List<DataGridViewCheckBoxCell> linecheckbox = new List<DataGridViewCheckBoxCell>(); 
        public RoutePlanningForm()
        {
            InitializeComponent();

            this.Text = "航点规划窗体";
            trackBar1.Minimum = 0;
            trackBar1.Maximum = 180;
            trackBar1.Value = 0;
            trackBar1.ValueChanged += trackBar1_ValueChanged;
            this.ControlBox = false;
            this.FormBorderStyle = FormBorderStyle.None;

            this.ShowInTaskbar = false;
      //      this.Opacity = 0.6;  
            trackBar1.Value = 30;
            textBox_showdirection.Text = trackBar1.Value.ToString();
            textBox_distance.Text = "10";
            this.textBox_height.Text = "30";

            this.dataGridView_waypointlist.CellMouseDown += dataGridView_waypointlist_CellMouseDown;
            this.dataGridView_waypointlist.CellMouseUp+=dataGridView_waypointlist_CellMouseUp;

            this.dataGridView_linepointlist.CellMouseDown += dataGridView_linepointlist_CellMouseDown;
            this.dataGridView_linepointlist.CellMouseUp += dataGridView_linepointlist_CellMouseUp;


            this.radioButton_distanceset.Enabled = false;
            this.radioButton_camerafieldset.Enabled = false;
            this.textBox_camerafield_height.Enabled = false;
            this.textBox_camerafiled_width.Enabled = false;
            this.textBox_Coincidencerate_height.Enabled = false;
            this.textBox_Coincidencerate_width.Enabled = false;
            this.textBox_photodistance.Enabled = false;
            this.TrackBar_camerafield_height.Enabled = false;
            this.TrackBar_camerafield_width.Enabled = false;
            this.TrackBar_Coincidencerate_height.Enabled = false;
            this.TrackBar_Coincidencerate_width.Enabled = false;

        }
        void dataGridView_linepointlist_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            //throw new NotImplementedException();
            int colIndex = e.ColumnIndex;
            int rowIndex = e.RowIndex;
            if (colIndex == -1 || rowIndex == -1) return;
            DataGridView d = (DataGridView)sender;
            DataGridViewRow row = d.Rows[rowIndex];
            DataGridViewCell c = row.Cells[colIndex];
            if (c.GetType().ToString().EndsWith("CheckBoxCell"))
            {
                DataGridViewCheckBoxCell check = (DataGridViewCheckBoxCell)c;
                bool val = (bool)check.EditedFormattedValue;
                bool val1 = (bool)check.Value;

                if (checkboxin != val)
                {
                    if (val == true)
                    {
                        //MessageBox.Show("选中了Check");
                        for (int i = 0; i < linecheckbox.Count; i++)
                        {
                            if (linecheckbox[i].GetHashCode() == check.GetHashCode())
                            {
                                if (LinepointClick != null) {
                                    LinepointClick(i);
                                }
                            }
                            else
                            {
                                if ((bool)linecheckbox[i].EditedFormattedValue)
                                {
                                    linecheckbox[i].Value = false;
                                }
                            }
                        }
                    }
                    else
                    {
                        return;

                    }

                }
                else
                {

                }
            }
            else
            {


            }
        }
        void dataGridView_linepointlist_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            int colIndex = e.ColumnIndex;
            int rowIndex = e.RowIndex;

            if (colIndex == -1 || rowIndex == -1) return;

            DataGridView d = (DataGridView)sender;
            DataGridViewRow row = d.Rows[rowIndex];
            DataGridViewCell c = row.Cells[colIndex];


            if (c.GetType().ToString().EndsWith("CheckBoxCell"))
            {
                DataGridViewCheckBoxCell check = (DataGridViewCheckBoxCell)c;
                bool val = (bool)check.EditedFormattedValue;
                checkboxin = val;
            }
            else
            {

            }
        }
        void dataGridView_waypointlist_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            int colIndex = e.ColumnIndex;
            int rowIndex = e.RowIndex;

            if (colIndex == -1 || rowIndex == -1) return;

            DataGridView d = (DataGridView)sender;
            DataGridViewRow row = d.Rows[rowIndex];
            DataGridViewCell c = row.Cells[colIndex];


            if (c.GetType().ToString().EndsWith("CheckBoxCell"))
            {
                DataGridViewCheckBoxCell check = (DataGridViewCheckBoxCell)c;

                bool val = (bool)check.EditedFormattedValue;
                bool val1 = (bool)check.Value;

                checkboxin = val;
            }
            else
            {

            }
            
        }
        void dataGridView_waypointlist_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            //throw new NotImplementedException();
            int colIndex = e.ColumnIndex;
            int rowIndex = e.RowIndex;

            if (colIndex == -1 || rowIndex == -1) return;

            DataGridView d = (DataGridView)sender;
            DataGridViewRow row = d.Rows[rowIndex];
            DataGridViewCell c = row.Cells[colIndex];


            if (c.GetType().ToString().EndsWith("CheckBoxCell"))
            {
                DataGridViewCheckBoxCell check = (DataGridViewCheckBoxCell)c;

                bool val = (bool)check.EditedFormattedValue;
                bool val1 = (bool)check.Value;

                if (checkboxin != val)
                {
                    if (val == true)
                    {
                        //MessageBox.Show("选中了Check");

                        for (int i = 0; i < waypointcheckbox.Count; i++) {
                            if (waypointcheckbox[i].GetHashCode() == check.GetHashCode())
                            {
                                if (WaypointClick != null) {
                                    WaypointClick(i);
                                }
                            }
                            else
                            {
                                if ((bool)waypointcheckbox[i].EditedFormattedValue)
                                {
                                    waypointcheckbox[i].Value = false;
                                }
                            }
                        }
                    }
                    else {
                        MessageBox.Show("必须有一个起始点作为起始航点...");
                        

                        return;

                    }

                }
                else
                {
                   
                }
            }
            else
            {
               

            }

        }
        #region 无边框窗体


        const int Guying_HTLEFT = 10;
        const int Guying_HTRIGHT = 11;
        const int Guying_HTTOP = 12;
        const int Guying_HTTOPLEFT = 13;
        const int Guying_HTTOPRIGHT = 14;
        const int Guying_HTBOTTOM = 15;
        const int Guying_HTBOTTOMLEFT = 0x10;
        const int Guying_HTBOTTOMRIGHT = 17;
        protected override void WndProc(ref Message m)
        {
            try
            {
                switch (m.Msg)
                {

                    //case 0x0084:
                    //    base.WndProc(ref m);
                    //    Point vPoint = new Point((int)m.LParam & 0xFFFF,
                    //    (int)m.LParam >> 16 & 0xFFFF);
                    //    vPoint = PointToClient(vPoint);
                    //    if (vPoint.X <= 5)
                    //        if (vPoint.Y <= 5)
                    //            m.Result = (IntPtr)Guying_HTTOPLEFT;
                    //        else if (vPoint.Y >= ClientSize.Height - 5)
                    //            m.Result = (IntPtr)Guying_HTBOTTOMLEFT;
                    //        else
                    //            m.Result = (IntPtr)Guying_HTLEFT;
                    //    else if (vPoint.X >= ClientSize.Width - 5)
                    //        if (vPoint.Y <= 5)
                    //            m.Result = (IntPtr)Guying_HTTOPRIGHT;
                    //        else if (vPoint.Y >= ClientSize.Height - 5)
                    //            m.Result = (IntPtr)Guying_HTBOTTOMRIGHT;
                    //        else
                    //            m.Result = (IntPtr)Guying_HTRIGHT;
                    //    else if (vPoint.Y <= 5)
                    //        m.Result = (IntPtr)Guying_HTTOP;
                    //    else if (vPoint.Y >= ClientSize.Height - 5)
                    //        m.Result = (IntPtr)Guying_HTBOTTOM;
                    //    break;

                    case 0x0201:                //鼠标左键按下的消息 
                        m.Msg = 0x00A1;         //更改消息为非客户区按下鼠标 
                        m.LParam = IntPtr.Zero; //默认值 
                        m.WParam = new IntPtr(2);//鼠标放在标题栏内 
                        base.WndProc(ref m);
                        break;
                    default:
                        base.WndProc(ref m);
                        break;
                }
            }
            catch (ArgumentException ae)
            {
                System.Diagnostics.Debug.WriteLine(ae.Message);
            }
        }
        #endregion
        void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            //throw new NotImplementedException();

            textBox_showdirection.Text = trackBar1.Value.ToString();
        }
        private void button_close_Click(object sender, EventArgs e)
        {
            if (FormClose!=null) {
                FormClose();
            }

            this.Close();
        }
        private void button_startplan_Click(object sender, EventArgs e)
        {
            if (PlanningDirection!=null)
            {
                int angle = trackBar1.Value;
                float distance = 0;
                try
                {
                    distance = float.Parse(this.textBox_distance.Text);
                }
                catch {
                    MessageBox.Show("航线间间距无效.");
                    return;
                }
                routesangle = angle;
                PlanningDirection(angle, distance);
            }
        }
        private void button_Replan_Click(object sender, EventArgs e)
        {
            if (Replan != null) {
                Replan();
            }
        }
        private void textbox_height_keypress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar) { 
                case 'A':
                    break;
                case 'C':
                    break;
                case '\r':
                case '\n':
                    if (UpdateHeight != null) {
                        try
                        {
                            int height = int.Parse(this.textBox_height.Text);
                            UpdateHeight(height);
                        }   
                        catch {
                            MessageBox.Show("请输入有效高度数据...");
                        }
                    }


                    break;
                default:
                   // MessageBox.Show(((int)e.KeyChar).ToString());
                    break;
            }
        }
        private void textBox_showDirection_keypress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case 'A':
                    break;
                case 'C':
                    break;
                case '\r':
                case '\n':
                    try
                    {
                        int height = int.Parse(this.textBox_showdirection.Text);
                        if (height < trackBar1.Minimum || height > trackBar1.Maximum)
                        {
                            MessageBox.Show("请输入" + trackBar1.Minimum + "到" + trackBar1.Maximum + "之间的整数数据");
                        }
                        else {
                            this.trackBar1.Value = height;
                        }
                    }
                    catch
                    {
                        MessageBox.Show("请输入一个整数...");
                    }
                    break;
                default:
                    // MessageBox.Show(((int)e.KeyChar).ToString());
                    break;
            }
        }
        public void SetDirection(int angle) {
            textBox_showdirection.Text = angle.ToString();
            this.trackBar1.Value = angle;
        }
        public void WriteTotalDistance(int s) {
            if (this.textBox_totaldistance.InvokeRequired)
            {

                this.BeginInvoke(new UpdateHeightDele(WriteTotalDistance), new Object[] { s });
            }
            else {

                if (s < 1000)
                {
                    this.textBox_totaldistance.Text = s.ToString()+"m";
                }
                else {
                    this.textBox_totaldistance.Text = (s/1000.0f).ToString("0.00") + "KM";
                }



            }
        }
        public int  GetHeight() {
            try
            {
                return int.Parse(this.textBox_height.Text);
                
            }
            catch
            {
                MessageBox.Show("飞行高度无效...");
                return 0;
            }
        }
        public void ClearWayPointDataView() {
            this.dataGridView_waypointlist.Rows.Clear();
            waypointcheckbox.Clear();
        }
        public void ClearLinePointDataView()
        {
            this.dataGridView_linepointlist.Rows.Clear();
            this.linecheckbox.Clear();
        }
        public void SetWayPointPara(int num, int height, bool isStart)
        {
            DataGridViewRow row = new DataGridViewRow();
            DataGridViewTextBoxCell textboxcell1 = new DataGridViewTextBoxCell();
            DataGridViewTextBoxCell textboxcell2 = new DataGridViewTextBoxCell();
            DataGridViewCheckBoxCell checkboxcell = new DataGridViewCheckBoxCell();

            
           

            textboxcell1.Value = num.ToString();
            textboxcell2.Value = height.ToString();
            checkboxcell.Value = isStart;

            row.Cells.Add(textboxcell1);
            row.Cells.Add(textboxcell2);
            row.Cells.Add(checkboxcell);


            textboxcell1.ReadOnly = true;
            textboxcell2.ReadOnly = true;

            waypointcheckbox.Add(checkboxcell);

            this.dataGridView_waypointlist.Rows.Add(row);
        }
        public void UpdateNumendpoints(int num) {
            for (int i = num; i < this.dataGridView_linepointlist.Rows.Count; i++) {
                DataGridViewTextBoxCell textboxcell = (DataGridViewTextBoxCell)this.dataGridView_linepointlist.Rows[i].Cells[0];
                DataGridViewTextBoxCell textboxcell1 = (DataGridViewTextBoxCell)this.dataGridView_linepointlist.Rows[i].Cells[2];

                if (i == this.dataGridView_linepointlist.Rows.Count - 1)
                {
                    textboxcell.Value = (i + 1).ToString();
                    textboxcell1.Value = (i + 1).ToString() + ",1"; 
                }
                else { 
                    textboxcell.Value = (i + 1).ToString();
                    textboxcell1.Value = (i+1).ToString() +","+ (i + 2).ToString();                
                }


            }
        }
        public void UpdateNumwaypoints(int num) {
            for (int i = num; i < this.dataGridView_waypointlist.Rows.Count; i++)
            {
                DataGridViewTextBoxCell textboxcell = (DataGridViewTextBoxCell)this.dataGridView_waypointlist.Rows[i].Cells[0];

                textboxcell.Value = (i + 1).ToString();
            }
        }
        public void SetLinePointPara(int num, int angle,string direction,string str, bool isDirection)
        {
            DataGridViewRow row = new DataGridViewRow();
            DataGridViewTextBoxCell textboxcell1 = new DataGridViewTextBoxCell();
            DataGridViewTextBoxCell textboxcell2 = new DataGridViewTextBoxCell();
            DataGridViewTextBoxCell textboxcell3 = new DataGridViewTextBoxCell();
            DataGridViewCheckBoxCell checkboxcell = new DataGridViewCheckBoxCell();

            string directionstr = direction + ":" + angle.ToString();
            //if(angle==0){
            //    directionstr = direction;
            //}else{
            //    directionstr = direction + ":" + angle.ToString();
          
            //}

            textboxcell1.Value = num.ToString();
            textboxcell2.Value = directionstr;
            textboxcell3.Value = str;
            checkboxcell.Value = isDirection;

            this.linecheckbox.Add(checkboxcell);

            row.Cells.Add(textboxcell1);
            row.Cells.Add(textboxcell2);
            row.Cells.Add(textboxcell3);
            row.Cells.Add(checkboxcell);


            textboxcell1.ReadOnly = true;

            this.dataGridView_linepointlist.Rows.Insert(num-1,row);
        }
        public void DeleteLineRow(int rowsIndex) {
            this.linecheckbox.RemoveAt(rowsIndex);
            this.dataGridView_linepointlist.Rows.RemoveAt(rowsIndex);
        }
        public void DeleWayRow(int rowIndex) {
            this.waypointcheckbox.RemoveAt(rowIndex);
            this.dataGridView_waypointlist.Rows.RemoveAt(rowIndex);
        }
        public void UpdateLineAngle(int index,int angle,string str) {
            DataGridViewTextBoxCell textboxcell = (DataGridViewTextBoxCell)this.dataGridView_linepointlist.Rows[index].Cells[1];
            string directionstr = "";

            if (angle == 0)
            {
                directionstr = str;

            }
            else {
                directionstr = str + ":" + angle.ToString();
            }
            textboxcell.Value = directionstr;
        }
        public void UpdateWayPointHeight(int index,int height) {
            DataGridViewTextBoxCell textboxcell = (DataGridViewTextBoxCell)this.dataGridView_waypointlist.Rows[index].Cells[1];

            textboxcell.Value = height.ToString();


    
        }
        public bool isStartphoto = false;
        public int distance=40;
        public int cameraangle = 90;
        public int routesangle;
        private void checkBox_startphoto_CheckedChanged(object sender, EventArgs e)
        {
            isStartphoto = this.checkBox_startphoto.Checked;

            if (this.checkBox_startphoto.Checked)
            {
                this.radioButton_camerafieldset.Enabled = true;
                this.radioButton_distanceset.Enabled = true;

                if (radioButton_distanceset.Checked)
                {
                    this.textBox_photodistance.Enabled = true;
                }
                if(radioButton_camerafieldset.Checked) {
                    this.textBox_camerafield_height.Enabled = true;
                    this.textBox_camerafiled_width.Enabled = true;
                    this.textBox_Coincidencerate_height.Enabled = true;
                    this.textBox_Coincidencerate_width.Enabled = true;
                    this.TrackBar_camerafield_height.Enabled = true;
                    this.TrackBar_camerafield_width.Enabled = true;
                    this.TrackBar_Coincidencerate_height.Enabled = true;
                    this.TrackBar_Coincidencerate_width.Enabled = true;
                }
            }
            else {
                this.radioButton_distanceset.Enabled = false;
                this.radioButton_camerafieldset.Enabled = false;

                this.textBox_camerafield_height.Enabled = false;
                this.textBox_camerafiled_width.Enabled = false;
                this.textBox_Coincidencerate_height.Enabled = false;
                this.textBox_Coincidencerate_width.Enabled = false;
                this.textBox_photodistance.Enabled = false;

                this.TrackBar_camerafield_height.Enabled = false;
                this.TrackBar_camerafield_width.Enabled = false;
                this.TrackBar_Coincidencerate_height.Enabled = false;
                this.TrackBar_Coincidencerate_width.Enabled = false;

                textBox_distance.Enabled = true;
            }
        }

        public int GettTakePhotoDistance() {
            string textboxInput = this.textBox_photodistance.Text;
            if (textboxInput.EndsWith("KM") || textboxInput.EndsWith("km") ||
                textboxInput.EndsWith("Km") || textboxInput.EndsWith("kM"))
            {
                textboxInput = textboxInput.Substring(0, textboxInput.Length - 2);
            }
            else if (textboxInput.EndsWith("CM") || textboxInput.EndsWith("cm") ||
               textboxInput.EndsWith("Cm") || textboxInput.EndsWith("cM"))
            {
                textboxInput = textboxInput.Substring(0, textboxInput.Length - 1);
            }
            else if (textboxInput.EndsWith("M") || textboxInput.EndsWith("m"))
            {
                textboxInput = textboxInput.Substring(0, textboxInput.Length - 1);
            }

            try
            {
                distance = int.Parse(textboxInput);
            }
            catch
            {
                MessageBox.Show("照相点距离设置错误,请重新设置!!!");

                return -1;
            }
            return distance;
        }
        
        private void textbox_takephoto_keypress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                string textboxInput = this.textBox_photodistance.Text;

                if (textboxInput.EndsWith("KM") || textboxInput.EndsWith("km") ||
                    textboxInput.EndsWith("Km") || textboxInput.EndsWith("kM"))
                {
                    textboxInput = textboxInput.Substring(0, textboxInput.Length - 2);
                }
                else if (textboxInput.EndsWith("CM") || textboxInput.EndsWith("cm") ||
                   textboxInput.EndsWith("Cm") || textboxInput.EndsWith("cM"))
                {
                    textboxInput = textboxInput.Substring(0, textboxInput.Length - 1);
                }
                else if (textboxInput.EndsWith("M") || textboxInput.EndsWith("m"))
                {
                    textboxInput = textboxInput.Substring(0, textboxInput.Length - 1);
                }

                try
                {
                    distance = int.Parse(textboxInput);
                }
                catch
                {
                    MessageBox.Show("请输入正确的距离数据!!!");
                }
            }
        }
        private void textBox_camerafield_keypress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r'){
                try
                {
                    int ang = int.Parse(textBox_camerafiled_width.Text);
                    if (ang < 0 || ang > 180)
                    {
                        MessageBox.Show("角度不在范围内!");
                        return;
                    }
                    cameraangle = ang;
                    textBox_camerafiled_width.Text = ((int)cameraangle).ToString();
                    //trackBar_cameraField_width.Value = cameraangle;
                }
                catch
                {
                    MessageBox.Show("请输入正确的视野角度数据!");
                    textBox_camerafiled_width.Text = ((int)cameraangle).ToString();
                }
            }else{

            }
        }

        public bool distanceset = false;
        public bool camerafield = false;
        
        private void radioButton_distanceset_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButton_distanceset.Checked)
            {
                this.textBox_camerafield_height.Enabled = false;
                this.textBox_camerafiled_width.Enabled = false;
                this.textBox_Coincidencerate_height.Enabled = false;
                this.textBox_Coincidencerate_width.Enabled = false;
                this.TrackBar_camerafield_height.Enabled = false;
                this.TrackBar_camerafield_width.Enabled = false;
                this.TrackBar_Coincidencerate_height.Enabled = false;
                this.TrackBar_Coincidencerate_width.Enabled = false;

                this.textBox_photodistance.Enabled = true;
                textBox_distance.Enabled = true;

                distanceset = true;
            }
            else {
                distanceset = false;
            }

        }
        private void radioButton_camerafieldset_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_camerafieldset.Checked)
            {
                this.textBox_camerafield_height.Enabled = true;
                this.textBox_camerafiled_width.Enabled = true;
                this.textBox_Coincidencerate_height.Enabled = true;
                this.textBox_Coincidencerate_width.Enabled = true;
                this.TrackBar_camerafield_height.Enabled = true;
                this.TrackBar_camerafield_width.Enabled = true;
                this.TrackBar_Coincidencerate_height.Enabled = true;
                this.TrackBar_Coincidencerate_width.Enabled = true;

                this.textBox_photodistance.Enabled = false;
                textBox_distance.Enabled = false;

                camerafield = true;

            }
            else {
                camerafield = false;
                    
            }

        }
        private void trackbar_camerafield_width_valuechange(object sender, EventArgs e)
        {
            this.textBox_camerafiled_width.Text = this.TrackBar_camerafield_width.Value.ToString();
        }
        private void trackBar_camerafield_height_valuechange(object sender, EventArgs e)
        {
            this.textBox_camerafield_height.Text = this.TrackBar_camerafield_height.Value.ToString();
        }
        private void TrackBar_Coincidencerate_width_ValueChanged(object sender, EventArgs e)
        {
            this.textBox_Coincidencerate_width.Text = this.TrackBar_Coincidencerate_width.Value.ToString();
        }
        private void TrackBar_Coincidencerate_height_ValueChanged(object sender, EventArgs e)
        {
            this.textBox_Coincidencerate_height.Text = this.TrackBar_Coincidencerate_height.Value.ToString();
        }

        public int GetCoincidenceratewidth() {
            return this.TrackBar_Coincidencerate_width.Value;
        }
        public int GetCoincidencerateheight() {
            return this.TrackBar_Coincidencerate_height.Value;
        }
        public int GetCamerawidth() {
            return this.TrackBar_camerafield_width.Value;
        }
        public int GetCameraheight() {
            return this.TrackBar_camerafield_height.Value;
        }
        public void SetTextBoxDistance(int distance) {
            this.textBox_distance.Text = distance.ToString();
        }
        public void SetPhotoDistance(int distance) {
            this.textBox_photodistance.Text = distance.ToString();
        }
    }
}
