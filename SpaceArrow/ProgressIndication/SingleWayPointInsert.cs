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
    public partial class SingleWayPointInsert :Form
    {
        public SingleWayPointInsert()
        {
            InitializeComponent();

            this.TransparencyKey = Color.Red;
            this.ControlBox = false;
            this.textBox1.Text = "30";
            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;

            this.Opacity =0.6;           
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

        private void dmButtonClose1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void dmButtonMin1_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        #endregion


        private void button_10meter_Click(object sender, EventArgs e)
        {
            this.textBox1.Text = "10";
        }

        private void button_30meter_Click(object sender, EventArgs e)
        {
            this.textBox1.Text = "30";
        }

        private void button_200meter_Click(object sender, EventArgs e)
        {
            this.textBox1.Text = "200";
        }

        private void button_100meter_Click(object sender, EventArgs e)
        {
            this.textBox1.Text = "100";

        }

        private void button_50meter_Click(object sender, EventArgs e)
        {
            this.textBox1.Text = "50";
        }

        private void button_20meter_Click(object sender, EventArgs e)
        {
            this.textBox1.Text = "20";
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button_80meter_Click(object sender, EventArgs e)
        {
            textBox1.Text = "80";
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                double height = double.Parse(this.textBox1.Text);
                this.textBox1.Text = (height + 10).ToString();
            }
            catch {
                MessageBox.Show("高度数据不正确");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                double height = double.Parse(this.textBox1.Text);
                this.textBox1.Text = (height - 10).ToString();
            }
            catch
            {
                MessageBox.Show("高度数据不正确");
            }
        }
    }
}
