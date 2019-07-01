using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyControl
{
    public partial class AltitudeCon : UserControl
    {
        int ControlWidth = 0;
        int ControlHeight = 0;

        float yaw = 180;
        float alt = 0;

        float vertical = 1;//垂直
        float Horizontal = 1;//水平

        float speed = 0;
    //    float 

        public AltitudeCon()
        {
            InitializeComponent();
            ControlWidth = this.Width;
            ControlHeight = this.Height;

           // this.BackColor = Color.Transparent;

            //使用默认双缓冲
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.ResizeRedraw |
                    ControlStyles.AllPaintingInWmPaint, true);
        }

        public void Setyaw(float ya) {
            this.yaw = ya;
            this.Invalidate();
        }

        void DrawScale(Graphics ControlGraphics)
        {
            Bitmap BitmapScale = new Bitmap(2*ControlWidth, ControlHeight);
            Graphics BitmapGraphics = Graphics.FromImage(BitmapScale);

            Pen p=new Pen(Color.Black);
            SolidBrush fontBrush = new SolidBrush(Color.Black);
            SolidBrush backBrush = new SolidBrush(Color.FromArgb(80,Color.White));

            Font font = new Font("宋体", (int)(0.03 * this.ControlWidth));

            float everyscale = 30;//每个刻度代表的度数
            float middlewidth = 0.1f * ControlWidth;
            //width == 0--360 

            float resolution = ((float)this.ControlWidth) / 360.0f;//每一度需要的像素点

            //计算开始画线的位置以及角度
 
            //画线
            for (int i = 0; i * resolution * everyscale <= 2*ControlWidth; i++)
            {
                float ang =  (i ) * everyscale >= 360 ?  (i ) * everyscale - 360 :  (i ) * everyscale;
                string str = (ang).ToString();
                SizeF size = BitmapGraphics.MeasureString(str, font);
                BitmapGraphics.DrawString(str, font, fontBrush,  i * resolution * everyscale - size.Width / 2, 0.2f * ControlHeight);
                BitmapGraphics.DrawLine(p,  i * resolution * everyscale, 0.8f * ControlHeight, i * resolution * everyscale, ControlHeight);
            }

          //  float startangle = ;
            int startloc = (int)(resolution * (yaw > 180 ? yaw - 180 : yaw + 180));

            Rectangle dis = new Rectangle(0, 0, ControlWidth, ControlHeight);
            Rectangle res = new Rectangle(startloc, 0, ControlWidth, ControlHeight);

            ControlGraphics.DrawImage(BitmapScale, dis, res, System.Drawing.GraphicsUnit.Pixel);
            ControlGraphics.FillRectangle(backBrush, (ControlWidth - middlewidth) / 2, 0, middlewidth, ControlHeight);
       }
        void DrawAltitude(Graphics ControlGraphics)
        {
            int x_offset = 0;
            int y_offset = 0;

            int altWidth = this.ControlWidth;
            int altHeigh = this.ControlHeight;

            float resolution = 0.5f;//每个像素点表示的高度
            float startalt = alt - altHeigh / 2 * resolution;
            float endalt = alt + altHeigh / 2 * resolution;
            float everyscale = 3;//每个刻度表示的高度
            float shortlength = 0.1f * altWidth;
            float longlenght = 0.2f * ControlWidth;

            Font font = new System.Drawing.Font("宋体", 8);

            Bitmap BitmapAlt = new Bitmap(altWidth, altHeigh);
            Graphics AltGraphics = Graphics.FromImage(BitmapAlt);

            Pen PenLine = new Pen(Color.Black,8);

            SolidBrush backBrush = new SolidBrush(Color.FromArgb(120, Color.White));
            SolidBrush lineBrush = new SolidBrush(Color.Black);
            AltGraphics.FillRectangle(backBrush, 0, 0, altWidth, altHeigh);

           //AltGraphics.DrawRectangle(PenLine, 0, 0, altWidth, altHeigh);

            AltGraphics.DrawLine(PenLine, 0, 0, 0, altHeigh);


         //   float curalt = alt - resolution * altHeigh / 2;

            Pen p = new Pen(Color.Black);
            float sizey=0;
            //向上画线
            for (int i = 0; altHeigh/2 > (everyscale / resolution) * i; i++)
            {
                if (i % 5 == 0)
                {
                    AltGraphics.DrawLine(p, 0, altHeigh/2 - (everyscale / resolution) * i, longlenght, altHeigh/2 - (everyscale / resolution) * i);

                    string str = (alt + i * (everyscale / resolution)).ToString();
                    SizeF size = AltGraphics.MeasureString(str, font);
                    AltGraphics.DrawString(str, font, lineBrush, new PointF(longlenght + 5, altHeigh/2 - (everyscale / resolution) * i - size.Height / 2));
                    sizey=sizey>size.Height?sizey:size.Height;
                }
                else
                {
                    AltGraphics.DrawLine(p, 0, altHeigh/2 - (everyscale / resolution) * i, shortlength, altHeigh/2 - (everyscale / resolution) * i);

                }
            }
            //向下画线
            for (int i = 0; altHeigh / 2 > (everyscale / resolution) * i; i++)
            {
                if (i % 5 == 0)
                {
                    AltGraphics.DrawLine(p, 0, altHeigh / 2 + (everyscale / resolution) * i, longlenght, altHeigh / 2 + (everyscale / resolution) * i);

                    string str = (alt - i * (everyscale / resolution)).ToString();
                    SizeF size = AltGraphics.MeasureString(str, font);
                    AltGraphics.DrawString(str, font, lineBrush, new PointF(longlenght + 5, altHeigh / 2 + (everyscale / resolution) * i - size.Height / 2));
                    sizey=sizey>size.Height?sizey:size.Height;
                }
                else
                {
                    AltGraphics.DrawLine(p, 0, altHeigh / 2 + (everyscale / resolution) * i, shortlength, altHeigh / 2 + (everyscale / resolution) * i);

                }
            }

            PointF[] pp = new PointF[5];

            pp[0].X = 0;
            pp[0].Y = altHeigh / 2;
            pp[1].X = longlenght;
            pp[1].Y = pp[0].Y - sizey / 2.0f * 1.2f;
            pp[2].X = altWidth;
            pp[2].Y = pp[1].Y;
            pp[3].X = pp[2].X;
            pp[3].Y = pp[0].Y + sizey / 2.0f * 1.2f;
            pp[4].X = pp[1].X;
            pp[4].Y = pp[3].Y;



            SolidBrush b1 = new SolidBrush(Color.FromArgb(50, Color.Red));
            AltGraphics.FillPolygon(b1, pp);

            Rectangle dis = new Rectangle(x_offset, y_offset, altWidth, altHeigh);
            Rectangle res = new Rectangle(0, 0, altWidth, altHeigh);

            ControlGraphics.DrawImage(BitmapAlt, dis, res, System.Drawing.GraphicsUnit.Pixel);
        
        }
        void DrawSpeed(Graphics ControlGraphics)
        {
            int x_offset = 0;
            int y_offset = 0;

            int speedWidth = this.ControlWidth;
            int speedHeigh = this.ControlHeight;


            Font font = new System.Drawing.Font("宋体", 8);

            Bitmap BitmapSpeed = new Bitmap(speedWidth, speedHeigh);
            Graphics SpeedGraphics = Graphics.FromImage(BitmapSpeed);

            Pen PenLine = new Pen(Color.Black, 8);
            SolidBrush backBrush = new SolidBrush(Color.FromArgb(120, Color.White));
            Pen p = new Pen(Color.Black);
            SolidBrush lineBrush = new SolidBrush(Color.Black);

            SpeedGraphics.FillRectangle(backBrush, 0, 0, speedWidth, speedHeigh);
            SpeedGraphics.DrawLine(PenLine, speedWidth, 0, speedWidth, speedHeigh);


            float resolution = 0.5f;//每个像素点表示的速度
            float startalt = speed - speedHeigh / 2 * resolution;
            float endalt = speed + speedHeigh / 2 * resolution;
            float everyscale = 3;//每个刻度表示的速度
            float shortlength = 0.1f * speedWidth;
            float longlenght = 0.2f * speedWidth;

            float sizey = 0;
            //向上画线
            for (int i = 0; speedHeigh / 2 > (everyscale / resolution) * i; i++)
            {
                if (i % 5 == 0)
                {
                    SpeedGraphics.DrawLine(p, speedWidth - longlenght, speedHeigh / 2 - (everyscale / resolution) * i, speedWidth, speedHeigh / 2 - (everyscale / resolution) * i);

                    string str = (speed + i * (everyscale / resolution)).ToString();
                    SizeF size = SpeedGraphics.MeasureString(str, font);
                    SpeedGraphics.DrawString(str, font, lineBrush, new PointF(speedWidth - longlenght- size.Width*1.2f, speedHeigh / 2 - (everyscale / resolution) * i - size.Height / 2));
                    sizey = sizey > size.Height ? sizey : size.Height;
                }
                else
                {
                    SpeedGraphics.DrawLine(p, speedWidth - shortlength, speedHeigh / 2 - (everyscale / resolution) * i, speedWidth, speedHeigh / 2 - (everyscale / resolution) * i);

                }
            }
            //向下画线
            for (int i = 0; speedHeigh / 2 > (everyscale / resolution) * i; i++)
            {
                if (i % 5 == 0)
                {
                    SpeedGraphics.DrawLine(p, speedWidth - longlenght, speedHeigh / 2 + (everyscale / resolution) * i, speedWidth, speedHeigh / 2 + (everyscale / resolution) * i);

                    string str = (speed - i * (everyscale / resolution)).ToString();
                    SizeF size = SpeedGraphics.MeasureString(str, font);
                    SpeedGraphics.DrawString(str, font, lineBrush, new PointF(speedWidth - longlenght - size.Width * 1.2f, speedHeigh / 2 + (everyscale / resolution) * i - size.Height / 2));
                    sizey = sizey > size.Height ? sizey : size.Height;
                }
                else
                {
                    SpeedGraphics.DrawLine(p, speedWidth - shortlength, speedHeigh / 2 + (everyscale / resolution) * i, speedWidth, speedHeigh / 2 + (everyscale / resolution) * i);

                }
            }

            PointF[] pp = new PointF[5];
            pp[0].X = speedWidth;
            pp[0].Y = speedHeigh / 2;

            pp[1].X = speedWidth-longlenght;
            pp[1].Y = pp[0].Y - sizey / 2.0f * 1.2f;
            pp[2].X = 0;
            pp[2].Y = pp[1].Y;
            pp[3].X = pp[2].X;
            pp[3].Y = pp[0].Y + sizey / 2.0f * 1.2f;
            pp[4].X = pp[1].X;
            pp[4].Y = pp[3].Y;



            SolidBrush b1 = new SolidBrush(Color.FromArgb(50, Color.Red));
            SpeedGraphics.FillPolygon(b1, pp);




















            Rectangle dis = new Rectangle(x_offset, y_offset, speedWidth, speedHeigh);
            Rectangle res = new Rectangle(0, 0, speedWidth, speedHeigh);

            ControlGraphics.DrawImage(BitmapSpeed, dis, res, System.Drawing.GraphicsUnit.Pixel);
        

        }

        void DrawRemoteSensing(Graphics ControlGraphics)
        {
            int x_offset = 0;
            int y_offset = 0;

            int RemoteWidth = this.ControlWidth;
            int RemoteHeight = this.ControlHeight;

            float RodWidthHeight = (float)(0.05f * (RemoteWidth > RemoteHeight ? RemoteHeight : RemoteWidth));
            float SquareWidth = (float)(2.3f * RodWidthHeight);
            float SquareHeight = (float)(1.3f * RodWidthHeight);

            Bitmap BitmapRemote = new Bitmap(RemoteWidth, RemoteHeight);
            Graphics RemoteGraphics = Graphics.FromImage(BitmapRemote);

            RemoteGraphics.Clear(Color.White);

            SolidBrush Rodbrush = new SolidBrush(Color.Yellow);
            SolidBrush Squarebrush = new SolidBrush(Color.Black);

            RemoteGraphics.FillRectangle(Rodbrush, 0, (RemoteHeight - RodWidthHeight  ) / 2, RemoteWidth, RodWidthHeight);
            RemoteGraphics.FillRectangle(Rodbrush, (RemoteWidth - RodWidthHeight) / 2, 0, RodWidthHeight, RemoteHeight);

            //垂直
            RemoteGraphics.FillRectangle(Squarebrush, (RemoteWidth - SquareWidth) / 2, (1-vertical) * (RemoteHeight-SquareHeight), SquareWidth, SquareHeight);
        
            
            //水平
            RemoteGraphics.FillRectangle(Squarebrush, (RemoteWidth - SquareHeight) * Horizontal, (RemoteHeight - SquareWidth) / 2, SquareHeight, SquareWidth); 

            Rectangle dis = new Rectangle(x_offset, y_offset, RemoteWidth, RemoteHeight);
            Rectangle res = new Rectangle((int)(SquareHeight / 2), (int)SquareHeight / 2, (int)(RemoteWidth - SquareHeight), (int)(RemoteHeight - SquareHeight));

            ControlGraphics.DrawImage(BitmapRemote, dis, res, System.Drawing.GraphicsUnit.Pixel);
        }

        private void YaControl_Paint(object sender, PaintEventArgs e)
        {
           // DrawAltitude(e.Graphics);
          //  DrawSpeed(e.Graphics);
            DrawRemoteSensing(e.Graphics);
        }
        private void YaControl_Resize(object sender, EventArgs e)
        {
            ControlWidth = this.Width;
            ControlHeight = this.Height;
            this.Invalidate();
        }

        private void YaControl_Load(object sender, EventArgs e)
        {
            ControlWidth = this.Width;
            ControlHeight = this.Height;

            this.Invalidate();
        }

        public void SetAlt(float alt) {
            this.alt = alt;
            this.Invalidate();
        }
        public void SetVertical(float ver) {
            this.vertical = ver;
            this.Invalidate();
        }
        public void SetHorizontal(float hor)
        {
            this.Horizontal = hor;
            this.Invalidate();
        }
    }
}
