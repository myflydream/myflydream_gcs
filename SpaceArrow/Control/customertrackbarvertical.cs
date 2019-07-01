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
    public partial class customertrackbarvertical : UserControl
    {

        float Edge_Scale_Width = 0.3f;//自定义滑块组件边界矩形左右两边宽度与控件宽度的比例
        float Edge_Scale_height = 0.03f;//自定义滑块组件边界矩形左右两边高度与控件高度的比例

        float Block_Scale_Start_Hieght = 0.03f;
        float Block_Scale_Width = 0.02f;

        Pen PenControl = new Pen(Color.Transparent, 1);
        SolidBrush BrushControl = new SolidBrush(Color.Red);
        Rectangle block_rect = new Rectangle();
        bool tracing = false;
        Bitmap ControlBitmap;

        private int MaxValueBar = 100;
        private int MinValueBar = 0;

        public int MaxValue
        {
            get
            {
                return MaxValueBar;
            }
            set
            {
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
            set
            {
                nowValue = value;
               // if (ValueChanged != null) ValueChanged(this, new EventArgs());
            }
        }

        public customertrackbarvertical()
        {
            InitializeComponent();
            ControlBitmap = new Bitmap(Width, Height);
            this.Paint += customertrackbarvertical_Paint;
            this.MouseDown += customertrackbarvertical_MouseDown;
            this.MouseUp += customertrackbarvertical_MouseUp;
            this.MouseMove += customertrackbarvertical_MouseMove;
            ////使用默认双缓冲
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.ResizeRedraw |
                    ControlStyles.AllPaintingInWmPaint, true);
        }
        void customertrackbarvertical_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && tracing)
            {
                Point mousePos = e.Location;
                int block_start = (int)((1-Edge_Scale_height) * Height);
                int block_end = (int)((Edge_Scale_height) *Height);

                if (mousePos.Y >= block_start) Value = MinValue;
                else if (mousePos.Y < block_end) Value = MaxValue;
                else
                {
                    float percent = (float)(mousePos.Y - block_end) / (float)(block_start - block_end);
                    Value = (int)(MinValue + (1-percent) * (MaxValue - MinValue));
                }
                this.Invalidate();
            }
        }
        void customertrackbarvertical_MouseUp(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Default;
            tracing = false;
            Invalidate();
        }
        void customertrackbarvertical_MouseDown(object sender, MouseEventArgs e)
        {
            if (block_rect.Contains(e.Location))
            {
                Cursor = Cursors.Hand;
                tracing = true;
                Invalidate();
            }
        }
        public void DrawBitmap()
        {
            Graphics bitmapGraphics = Graphics.FromImage(ControlBitmap);
            bitmapGraphics.Clear(Color.White);
            PenControl.Color = Color.Blue;

            bitmapGraphics.DrawRectangle(PenControl, new Rectangle((int)(Edge_Scale_Width * Width),
                (int)(Edge_Scale_height * Height),
                (int)((1 - 2 * Edge_Scale_Width) * Width),
                (int)((1 - 2 * Edge_Scale_height) * Height)));

            float percent = (float)(Value - MinValue) / (float)(MaxValue - MinValue);

            block_rect.Width = (int)((1-Block_Scale_Width )* Width) + 1;
            block_rect.Height = (int)((Block_Scale_Start_Hieght) * Height)+1;
            block_rect.X = (int)((Width-block_rect.Width)/2);

            float height = (float)(((1.0f - 2.0f * Edge_Scale_height) * Height - (float)block_rect.Height)) * (1 - percent);
            block_rect.Y = (int)(Edge_Scale_height * Height + height);

            BrushControl.Color = Color.Blue;
            bitmapGraphics.FillRectangle(BrushControl, block_rect);
            bitmapGraphics.FillRectangle(BrushControl, new Rectangle((int)(Edge_Scale_Width * Width),
                (int)(block_rect.Y + ((Block_Scale_Start_Hieght) * Height) ),
                (int)((1 - 2 * Edge_Scale_Width) * Width+1),
                (int)((1 - 2 * Edge_Scale_height) * Height - height - block_rect.Height+2)
                ));
        }

        void customertrackbarvertical_Paint(object sender, PaintEventArgs e)
        {
            DrawBitmap();
            Graphics g = e.Graphics;
            g.DrawImage(ControlBitmap, new Point(0, 0));
        }
    }
}
