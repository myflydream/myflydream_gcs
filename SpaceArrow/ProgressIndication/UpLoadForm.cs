using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpaceArrow
{
    public partial class UpLoadForm : Form
    {
        Bitmap bit = new Bitmap(global::SpaceArrow.Properties.Resources.logo);
        string title = "Uploading way point...";
        string message = "Getting WP 30000";
        Graphics g;

        public UpLoadForm()
        {
            InitializeComponent();

            this.Text = title;
            this.Icon = Icon.FromHandle(global::SpaceArrow.Properties.Resources.logo.GetHicon());

            this.BringToFront();

            this.StartPosition = FormStartPosition.CenterScreen;
            g = this.CreateGraphics();

            this.dmButton_Cancel.BackColor = this.BackColor;
          //  this.ShowInTaskbar = false;
            this.TopMost = true;

            this.ProgressBar1.DM_ProgressBarStyle = ProgressBarStyle.Continuous;

          }
       
        public void SetCount(int index,int count) {
            this.ProgressBar1.Value = (index+1) * 100 / count;
            SetMessage(2,index);
        }
        public void SetCount1(int index, int count)
        {
            this.ProgressBar1.Value = (index + 1) * 100 / count;
            SetMessage(6, index);
        }
        int sendCount = 0;
        int GetCount = 0;
        public void SetMessage(int type,int num) {
            switch (type) {
                case 0: //发送总的航点数
                    message = "Setting total wps["+num.ToString()+"]";
                    sendCount = num-1;
                    break;
                case 1:
                    message = "Setting Home.....";
                    break;
                case 2:
                    message = "Setting WP " + num.ToString() + "/" + sendCount.ToString();
                    break;
                case 3:
                    message = "Set done.";
                    break;
                case 4:
                    message = "Getting WP Count...";
                    break;
                case 5:
                    message = "Getting WP Count[" + num.ToString() + "]";
                    GetCount = num-1;
                    break;
                case 6:
                    message = "Getting WP " + num.ToString() + "/" + GetCount.ToString();
                    break;
                case 7:
                    message = "Get Done ";
                    break;
                case 8:
                    message = "Get Fail... ";  
                    break;
                default: break;
            }
            this.Invalidate();
          //  Application.DoEvents();
        }
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
            switch (m.Msg)
            {
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

        private void UpLoadForm_Paint(object sender, PaintEventArgs e)
        {
            Font font = new Font("宋体", 10);
            SolidBrush b = new SolidBrush(Color.Black);

            PointF p = new PointF();
            SizeF size = g.MeasureString(this.message, font);
            p.X = this.Width / 2 - size.Width / 2;
            p.Y = 0.6f * (this.Height/2);
            g.DrawString(this.message, font, b, p);

 
        
        }
      //  public delegate void CancelWayPoint();

        public bool isCancel = false;
        
        private void dmButton_Cancel_Click(object sender, EventArgs e)
        {
            this.isCancel = true;
        }

    }
}
