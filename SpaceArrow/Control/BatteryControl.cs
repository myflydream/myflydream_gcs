using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SpaceArrow.Control
{
    public partial class BatteryControl : UserControl
    {
        Bitmap      ControlBitmap = null;
        Graphics    ControlGraphics = null;

        int percent = 0;
        Color colorpen = Color.FromArgb(255,119,183,96);





        public BatteryControl()
        {
            InitializeComponent();
            this.Paint += BatteryControl_Paint;
            this.Resize += BatteryControl_Resize;
            ControlBitmap = new Bitmap(Width, Height);
            ControlGraphics = Graphics.FromImage(ControlBitmap);

            this.BackColor = Color.Transparent;

            //使用默认双缓冲
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.ResizeRedraw |
                    ControlStyles.AllPaintingInWmPaint, true);
        }


        public void SetBattery(int per)
        {
            if (per > 100) per = 100;
            if (per < 0) per = 0;
            this.percent = per;
            this.Invalidate();

        }


        void DrawbBorderFrame(Graphics g) {
            Bitmap frame = new Bitmap(global::SpaceArrow.Properties.Resources.battery_space);

            ControlGraphics.Clear(Color.Transparent);

            RectangleF rect_dis = new RectangleF(0, 0, frame.Width, frame.Height);
            RectangleF rect = new RectangleF(0,0, this.Width,  this.Height);
            ControlGraphics.DrawImage(frame, rect, rect_dis, System.Drawing.GraphicsUnit.Pixel);




        //    ControlGraphics.DrawRectangle(new Pen(Color.Red, 2), new Rectangle((int)(0.14 * ControlBitmap.Width), (int)(0.20 * ControlBitmap.Height), (int)(0.77 * ControlBitmap.Width), (int)(0.62 * ControlBitmap.Height)));


            int width = (int)((0.77 * ControlBitmap.Width) * percent / 100.0f);

            SolidBrush b;

            if (percent < 20)
            {
                b = new SolidBrush(Color.Red);
            }
            else {
                b = new SolidBrush(colorpen);
            }


            ControlGraphics.FillRectangle(b, new Rectangle((int)(0.91f*ControlBitmap.Width-width),(int)(0.20 * ControlBitmap.Height),width,(int)(0.62 * ControlBitmap.Height)));

            //  Font f = new System.Drawing.Font("宋体",(int)50);
            Font f = new System.Drawing.Font("宋体", (int)(ControlBitmap.Height * 0.5));
            string str = percent.ToString() + "%";

            SizeF size = ControlGraphics.MeasureString(str, f);

            ControlGraphics.DrawString(str, f, new SolidBrush(Color.Black), new PointF((ControlBitmap.Width - size.Width) / 2, (ControlBitmap.Height - size.Height) / 2));













            RectangleF rect_dis1 = new RectangleF(0, 0, ControlBitmap.Width, ControlBitmap.Height);
            RectangleF rect1 = new RectangleF(0, 0, this.Width, this.Height);
            g.DrawImage(ControlBitmap, rect1, rect_dis1, System.Drawing.GraphicsUnit.Pixel);

        }


        void BatteryControl_Paint(object sender, PaintEventArgs e)
        {
            DrawbBorderFrame(e.Graphics);
        }
        void BatteryControl_Resize(object sender, EventArgs e)
        {
            ControlBitmap = new Bitmap(Width, Height);
            ControlGraphics = Graphics.FromImage(ControlBitmap);
            this.Invalidate();
        }
 }
}
