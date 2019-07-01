using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SpaceArrow
{
    public partial class BatteryWarning : Form
    {
        public bool isClickFalse = false;
        public bool isClickRight = false;

        public BatteryWarning()
        {
            InitializeComponent();
            this.ControlBox = false;

            this.StartPosition = FormStartPosition.CenterScreen;
            this.TransparencyKey = Color.Red;
            this.FormBorderStyle = FormBorderStyle.None;

        //    this.TopMost = true;
        //    this.TopLevel = true;
            //SetTimeNum(8);
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




     public void SetTimeNum(int num) {
            switch (num) {
                case 0: this.pictureBox1.Image = global::SpaceArrow.Properties.Resources.number_0; break;
                case 1: this.pictureBox1.Image = global::SpaceArrow.Properties.Resources.number_1; break;
                case 2: this.pictureBox1.Image = global::SpaceArrow.Properties.Resources.number_2; break;
                case 3: this.pictureBox1.Image = global::SpaceArrow.Properties.Resources.number_3; break;
                case 4: this.pictureBox1.Image = global::SpaceArrow.Properties.Resources.number_4; break;
                case 5: this.pictureBox1.Image = global::SpaceArrow.Properties.Resources.number_5; break;
                case 6: this.pictureBox1.Image = global::SpaceArrow.Properties.Resources.number_6; break;
                case 7: this.pictureBox1.Image = global::SpaceArrow.Properties.Resources.number_7; break;
                case 8: this.pictureBox1.Image = global::SpaceArrow.Properties.Resources.number_8; break;
                case 9: this.pictureBox1.Image = global::SpaceArrow.Properties.Resources.number_9; break;
                default: this.pictureBox1.Image = global::SpaceArrow.Properties.Resources.number_0; break;
            }
        }

     private void button2_Click(object sender, EventArgs e)
     {
         isClickFalse = true;
     }

     private void button1_Click(object sender, EventArgs e)
     {
         isClickRight = true;
     }


    }
}
