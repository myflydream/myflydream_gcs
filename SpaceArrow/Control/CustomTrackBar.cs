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
    public partial class CustomTrackBar : UserControl
    {
        Color ColorBack = Color.White;
        Color ColorRect = Color.Red;
        Color ColorFill = Color.Red;

        Pen PenControl = new Pen(Color.Transparent, 1);
        SolidBrush BrushControl = new SolidBrush(Color.Red);

        float Edge_Scale_Width = 0.03f;//自定义滑块组件边界矩形左右两边宽度与控件宽度的比例
        float Edge_Scale_height = 0.3f;//自定义滑块组件边界矩形左右两边高度与控件高度的比例

        float Block_Scale_Start_Hieght = 0.02f;
        float Block_Scale_Width = 0.05f;

       private int MaxValueBar = 100;
       private int MinValueBar = 0;

       public int MaxValue {
           get {
               return MaxValueBar;
           }
           set {
               MaxValueBar = value;
               if (Value > MaxValueBar) Value = MaxValueBar;

           }
       }
       public int MinValue
       {
           get
           {
               return MinValueBar;
           }
           set
           {
               MinValueBar = value;
               if (Value < MinValueBar) Value = MinValueBar;
           }
       }

       private int nowValue;
       public int Value
       {
           get { return nowValue; }
           set { nowValue = value;
           if (ValueChanged != null) ValueChanged(this, new EventArgs());
           }
       }




        Rectangle block_rect = new Rectangle();
        bool tracing = false;

        Bitmap ControlBitmap;
        public event EventHandler ValueChanged;

        public CustomTrackBar()
        {
            InitializeComponent();
            ControlBitmap = new Bitmap(this.Width, this.Height);
            ////使用默认双缓冲
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.ResizeRedraw |
                    ControlStyles.AllPaintingInWmPaint, true);
        }

        private void Customtrackbar_paint(object sender, PaintEventArgs e)
        {
            DrawBitmap();
            Graphics g = e.Graphics;
            g.DrawImage(ControlBitmap, new Point(0, 0));
        }
        private void Customtrackbar_mousedown(object sender, MouseEventArgs e)
        {
            if (block_rect.Contains(e.Location))
            {
                Cursor = Cursors.Hand;
                tracing = true;
                Invalidate();
            }
        }
        private void Customtrackbar_mouseup(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Default;
            tracing = false;
            Invalidate();
        }
        private void CustomTrackBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && tracing)
            {
                Point mousePos = e.Location;
                int block_start = (int)(Edge_Scale_Width * Width);
                int block_end = (int)((1 - 2 * Edge_Scale_Width) * Width - Block_Scale_Width * Width);

                if (mousePos.X <= block_start) Value = MinValue;
                else if (mousePos.X >= block_end) Value = MaxValue;
                else
                {
                    float percent = (float)(mousePos.X - block_start) / (float)(block_end - block_start);
                    Value = (int)(MinValue + percent * (MaxValue - MinValue));
                }
                this.Invalidate();
            }
        }
        public void DrawBitmap()
        {
            Graphics bitmapGraphics = Graphics.FromImage(ControlBitmap);
            bitmapGraphics.Clear(ColorBack);
            PenControl.Color = ColorRect;
            bitmapGraphics.DrawRectangle(PenControl, new Rectangle((int)(Edge_Scale_Width * Width),
                (int)(Edge_Scale_height * Height),
                (int)((1 - 2 * Edge_Scale_Width) * Width),
                (int)((1 - 2 * Edge_Scale_height) * Height)));

            float percent = (float)(Value-MinValue) / (float)(MaxValue - MinValue);


            block_rect.X = (int)(Edge_Scale_Width * Width + percent * ((1 - 2 * Edge_Scale_Width) * Width -
                Block_Scale_Width * Width));
            block_rect.Y = (int)(Block_Scale_Start_Hieght * Height);
            block_rect.Width = (int)(Block_Scale_Width * Width) + 1;
            block_rect.Height = (int)((1 - Block_Scale_Start_Hieght) * Height);

            BrushControl.Color = ColorFill;
            bitmapGraphics.FillRectangle(BrushControl, block_rect);

            bitmapGraphics.FillRectangle(BrushControl, new Rectangle((int)(Edge_Scale_Width * Width) + 1,
                (int)(Edge_Scale_height * Height + 1),
                block_rect.X - (int)(Edge_Scale_Width * Width),
                (int)((1 - 2 * Edge_Scale_height) * Height) - 1
                ));

        }
        private void CustomTrackBar_Resize(object sender, EventArgs e)
        {
            if (ControlBitmap != null)
                ControlBitmap.Dispose();
            ControlBitmap = new Bitmap(this.Width, this.Height);
        }
        
    
    
    }
}
