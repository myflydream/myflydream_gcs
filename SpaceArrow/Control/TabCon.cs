using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace MyControl
{
    public partial class MyTabCon : UserControl, IDisposable
    {
        int ControlWidth = 0;       //控件宽
        int ControlHeight = 0;      //控件高
        int ControlPenWidth = 1;    //画笔粗
        int LeftEdge ;
        int RightEdge ;
        int BottomEdge;
        int TopEdge ;

        float roll = 0.0f;
        float pitch = 0.0f;
        public  float yaw = 0.0f;
        float speed = 0;
        float alt = 0.0f;
        bool isConnect = false;
        float distance =0f;
        int SatellitesNumber = 0;
        float votage = 0f;
        float current = 0f;
        int count = 0;
        int rssi = 0;
        float targetheight = 0f;

        SolidBrush b;
        private Font font = new Font("宋体", 10);          //字符的字体和大小
        Pen p = null;
        //new PointLatLng(23.1693223959252, 113.449287414551);
        double lat = 0;
        double lng = 0;


        Color ControlBackColor      = Color.Gray;//控件背景颜色
        Color ControlPenColor       = Color.White;
        Color ControlBrushColor     = Color.Red;
        Color ControlMiddleFocus    = Color.Black;
        Color ControlSkyColor       = Color.RoyalBlue;
        Color ControlGroundColor    = Color.Peru;
        Color ControlLineColor      = Color.White;
        Color ControlCharColor      = Color.Black;
        Color ControlTriangle       = Color.Red;
        Color ControlYawBack        = Color.White;
        Color ColorDisConnect       = Color.Red;
        Color ColorConnect          = Color.GreenYellow;


        Bitmap ControlBitamp        = null;     //在内存中声明Bitmap用作缓冲
        Graphics ControlGraphics    = null;     //内存bitmap的画布

        Bitmap RollPitchBitmap      = null;     //用于俯仰横滚的角度缓存
        Graphics RollPitchGraphics  = null;      //角度缓存的画布


        bool isMultiaxial = false;       //飞行器是否为多轴

        private bool isDrawLine = false;

        public MyTabCon()
        {
            InitializeComponent();

            this.BackColor = this.ControlBackColor;
            this.ControlWidth = this.Width;
            this.ControlHeight = this.Height;

            LeftEdge = (int)(0.15 * ControlWidth);
            RightEdge = (int)(0.15 * ControlWidth);
            BottomEdge = (int)(0.0 * ControlHeight);
            TopEdge = (int)(0.13 * ControlHeight);

            //初始化 内存控件画布
            ControlBitamp = new Bitmap(ControlWidth, ControlHeight);
            ControlGraphics = Graphics.FromImage(ControlBitamp);

            p = new Pen(ControlPenColor,ControlPenWidth);
            b = new SolidBrush(ControlBrushColor);

            //使用默认双缓冲
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.ResizeRedraw |
                    ControlStyles.AllPaintingInWmPaint, true);
        }

        private void draw_plane_focus()
        {
            int RollPoitchWidth = ControlWidth - LeftEdge - RightEdge;
            int RollPoitchHeight = ControlHeight - BottomEdge - TopEdge;

            int MiddleSquareWidth = (int)(0.025 * RollPoitchWidth);
            int MiddleSquareHeight = (int)(0.025 * RollPoitchHeight);
            int IntervalSquare=(int)(0.08*RollPoitchWidth);
            int SideSquareWidth = (int)(0.2 * RollPoitchWidth);

            int VerticalWidth = (int)(1.2 * MiddleSquareWidth);
            int VerticalHeight = (int)(2.2f * MiddleSquareHeight);

          //  ControlGraphics.DrawRectangle(p, LeftEdge,TopEdge,RollPoitchWidth,RollPoitchHeight);

            Rectangle rect = new Rectangle();

            if (isDrawLine)
            {
                rect.X = LeftEdge + (RollPoitchWidth - MiddleSquareWidth) / 2;
                rect.Y = TopEdge + (RollPoitchHeight - MiddleSquareHeight) / 2;
                rect.Width = MiddleSquareWidth;
                rect.Height = MiddleSquareHeight;

                ControlGraphics.DrawRectangle(p, rect);

                rect.X = LeftEdge + (RollPoitchWidth - MiddleSquareWidth) / 2 - IntervalSquare - SideSquareWidth;
                rect.Width = SideSquareWidth; 
                ControlGraphics.DrawRectangle(p, rect);

                rect.X = LeftEdge + (RollPoitchWidth + MiddleSquareWidth) / 2 + IntervalSquare;
                ControlGraphics.DrawRectangle(p, rect);

                rect.X = LeftEdge + (RollPoitchWidth - MiddleSquareWidth) / 2 - IntervalSquare - VerticalWidth;
                rect.Width = VerticalWidth;
                rect.Height = VerticalHeight;
                ControlGraphics.DrawRectangle(p, rect);

                rect.X = LeftEdge + (RollPoitchWidth + MiddleSquareWidth) / 2 + IntervalSquare ;
                ControlGraphics.DrawRectangle(p, rect);
            }
            else {
                ControlPenWidth = 0;
            }



            b.Color = ControlMiddleFocus;

            rect.X = LeftEdge + (RollPoitchWidth - MiddleSquareWidth + ControlPenWidth) / 2;
            rect.Y = TopEdge + (RollPoitchHeight - MiddleSquareHeight + ControlPenWidth) / 2;
            rect.Width = MiddleSquareWidth - ControlPenWidth;
            rect.Height = MiddleSquareHeight - ControlPenWidth;
            ControlGraphics.FillRectangle(b,rect);

            rect.X = LeftEdge + (RollPoitchWidth - MiddleSquareWidth + ControlPenWidth) / 2 - IntervalSquare - SideSquareWidth;
            rect.Width = SideSquareWidth - ControlPenWidth;
            ControlGraphics.FillRectangle(b, rect);

            rect.X = LeftEdge + (RollPoitchWidth + MiddleSquareWidth + ControlPenWidth) / 2 + IntervalSquare ;
            rect.Width = SideSquareWidth - ControlPenWidth;
            ControlGraphics.FillRectangle(b, rect);

            rect.X = LeftEdge + (RollPoitchWidth - MiddleSquareWidth + ControlPenWidth) / 2 - IntervalSquare - VerticalWidth;
            rect.Width = VerticalWidth - ControlPenWidth;
            rect.Height = VerticalHeight - ControlPenWidth;
            ControlGraphics.FillRectangle(b, rect);

            rect.X = LeftEdge + (RollPoitchWidth + MiddleSquareWidth + ControlPenWidth) / 2 + IntervalSquare ;
            ControlGraphics.FillRectangle(b, rect);

        }


        void DrawNumberOfSatellites() {
            Bitmap Satellitesbitmap;
            Graphics SatellitesGraphics;
            SolidBrush b = new SolidBrush(Color.FloralWhite);

            try
            {
                //Satellitesbitmap = new Bitmap(Program.currentpath + "\\satellite.png");
                //SatellitesGraphics = Graphics.FromImage(Satellitesbitmap);
            }
            catch
            {
                //Satellitesbitmap 
                //String str = "卫星数:0";
                //Font font = new Font("宋体", 8);
                //SizeF size = ControlGraphics.MeasureString(str, font);

                //Satellitesbitmap = new Bitmap((int)size.Width, (int)size.Height);
                //SatellitesGraphics = Graphics.FromImage(Satellitesbitmap);

                //SatellitesGraphics.DrawString(str, font, b, new PointF(0, 0));
            }

            Satellitesbitmap = new Bitmap(60,30);
            SatellitesGraphics = Graphics.FromImage(Satellitesbitmap);

            String str1 = "GPS:" + SatellitesNumber.ToString();
            Font font1 = new Font("宋体", 12,FontStyle.Bold);
            SizeF size1 = SatellitesGraphics.MeasureString(str1, font1);

            SatellitesGraphics.DrawString(str1, font1, b, new PointF((Satellitesbitmap.Width - size1.Width) / 2, (Satellitesbitmap.Height - size1.Height) / 2));

            RectangleF rect_dis = new RectangleF(0, 0, Satellitesbitmap.Width, Satellitesbitmap.Height);
            RectangleF rect = new RectangleF(this.LeftEdge * 1.02f, this.TopEdge, 0.1F * this.Width, 0.1F * this.Height);
            ControlGraphics.DrawImage(Satellitesbitmap, rect, rect_dis, System.Drawing.GraphicsUnit.Pixel);
        }
        private void DrawRollPitchState() {

            p.Color = ControlLineColor;

            int RollPitchWidth = ControlWidth - LeftEdge - RightEdge;
            int RollPitchHeight = ControlHeight - BottomEdge - TopEdge;
            float r = (float)System.Math.Sqrt(RollPitchWidth * RollPitchWidth + RollPitchHeight * RollPitchHeight) / 2+1;

            float LongLineLenght = (float)(0.25f * RollPitchWidth);
            float ShortLineLenght = (float)(0.17f * RollPitchWidth);
            float AngleDistance = 5.0f;
            float LineDistance = (float)(r / 90.0f * AngleDistance);

            RollPitchBitmap = new Bitmap((int)(2 * r), (int)(2 * r));
            RollPitchGraphics = Graphics.FromImage(RollPitchBitmap);
            RollPitchGraphics.Clear(this.ControlSkyColor);


            Matrix m = RollPitchGraphics.Transform;
            m.RotateAt(-1*roll, new PointF(r, r));
            RollPitchGraphics.Transform = m;

            float PitchDistance = (this.pitch) / 90.0f * r;
            float RadianPitch   = (float)(System.Math.Asin(PitchDistance / r) * 180 / System.Math.PI);

            b.Color = this.ControlGroundColor;
             
            RollPitchGraphics.FillPie(b, 0, 0, 2 * r, 2 * r, RadianPitch, 2 * (90-RadianPitch));//填充圆弧

            if (RadianPitch >= 0)
            {
                b.Color = this.ControlSkyColor;
            }
            else
            {
                b.Color = this.ControlGroundColor;
            }                
            RadianPitch = (RadianPitch) * (float)System.Math.PI / 180.0f;
            PointF[] PP = new PointF[3];
            float Y=(float)(r + (r * System.Math.Sin(RadianPitch)));
            PP[0].X = r;
            PP[0].Y = r;
            PP[1].X = (float)(r + (r * System.Math.Cos(RadianPitch)));
            PP[1].Y = Y;
            PP[2].X = (float)(r - (r * System.Math.Cos(RadianPitch)));
            PP[2].Y = Y;
            RollPitchGraphics.FillPolygon(b, PP);
            b.Color = ControlCharColor;
            //向上画线
            for (int i = 0; (0.75f * r < Y - i * LineDistance) && (AngleDistance * i<=90) ; i++)
            {
                if (Y - i * LineDistance > 1.65 * r)
                {
                    continue;
                }
                if (i % 3 == 0)
                {
                    RollPitchGraphics.DrawLine(p, r - LongLineLenght / 2, Y - i * LineDistance, r + LongLineLenght / 2, Y - i * LineDistance);
                    string str = ((int)(AngleDistance * i)).ToString();
                    RollPitchGraphics.DrawString(str, font, b, new PointF(r + LongLineLenght / 2 + 5, Y - LineDistance * i - 4));

                }
                else {
                    RollPitchGraphics.DrawLine(p, r - ShortLineLenght / 2, Y - i * LineDistance, r + ShortLineLenght / 2, Y - i * LineDistance);
                }
            }
            //向下画线
            for (int i = 0; (1.65 * r > Y + i * LineDistance) && (AngleDistance * i <= 90) ; i++)
            {
                if (Y + i * LineDistance < 0.75 * r)
                {
                    continue;
                }

                if (i % 3 == 0)
                {
                    RollPitchGraphics.DrawLine(p, r - LongLineLenght / 2, Y + i * LineDistance, r + LongLineLenght / 2, Y + i * LineDistance);
                    string str = ((int)(-1*AngleDistance * i)).ToString();
                    RollPitchGraphics.DrawString(str, font, b, new PointF(r + LongLineLenght / 2 + 5, Y + LineDistance * i - 4));
                }
                else
                {
                    RollPitchGraphics.DrawLine(p, r - ShortLineLenght / 2, Y + i * LineDistance, r + ShortLineLenght / 2, Y + i * LineDistance);
                }
            }
            int RWidth = (RollPitchWidth > RollPitchHeight) ? RollPitchHeight : RollPitchWidth;

            m.RotateAt(roll, new PointF(r, r));



            RollPitchGraphics.Transform = m;

            RollPitchGraphics.DrawArc(p, (int)r - (int)(RWidth / 2), (int)r - (int)(RWidth / 2), RWidth, RWidth, 180 + 30, 120);
            
            m.RotateAt(-70, new PointF(r, r));
            RollPitchGraphics.Transform = m;
            float angle = 10;
            b.Color = this.ControlPenColor;
            SizeF size = RollPitchGraphics.MeasureString(((int)(angle * 2) * 1.5f - 180).ToString(), font);

            int a = 0;
            //
            if (!this.isMultiaxial)
            {
                a = 2;
            }
            else {
                a = 1;
            }
            for (int i = 0; i < 13; i++)
            {
                m.RotateAt(angle, new PointF(r, r));
                RollPitchGraphics.Transform = m;
                if (i % 3 == 0)
                {
                    RollPitchGraphics.DrawLine(p, (int)r, (int)r - (int)(RWidth / 2),
                        (int)r - (int)(RWidth / 2) + RWidth / 2, (int)r - (int)(RWidth / 2) + 10
                        );

                    int an=((int)((angle * i * a) * 1.5f - 90*a));
                    size = RollPitchGraphics.MeasureString(an.ToString(), font);
                    RollPitchGraphics.DrawString((an).ToString(), font, b, (int)r - size.Width / 2.0f, (int)r - (int)(RWidth / 2) + 10 + 5);
                }
                else
                {
                    RollPitchGraphics.DrawLine(p, (int)r - (int)(RWidth / 2) + RWidth / 2, (int)r - (int)(RWidth / 2),
                        (int)r - (int)(RWidth / 2) + RWidth / 2, (int)r - (int)(RWidth / 2) + 5
                        );
                }
            }

            m.RotateAt(-60, new PointF(r, r));
            RollPitchGraphics.Transform = m;

            PointF[] PP1 = new PointF[3];
            PP1[0].X = r;
            PP1[0].Y = r - (RWidth / 2);
            PP1[1].X = r - 10;
            PP1[1].Y = PP1[0].Y + 10;
            PP1[2].X = r + 10;
            PP1[2].Y = PP1[0].Y + 10;
            if (roll > 90 * a)
            {
                roll = 90 * a;
            }
            else if (roll < -90 * a)
            {
                roll = -90 * a;
            }
            m.RotateAt(roll / (3.0f)*(3-a), new PointF(r, r));
            RollPitchGraphics.Transform = m;

            p.Color = ControlTriangle;
            RollPitchGraphics.DrawLine(p, PP1[0].X, PP1[0].Y, PP1[1].X, PP1[1].Y);
            RollPitchGraphics.DrawLine(p, PP1[2].X, PP1[2].Y, PP1[1].X, PP1[1].Y);
            RollPitchGraphics.DrawLine(p, PP1[0].X, PP1[0].Y, PP1[2].X, PP1[2].Y);

            m.RotateAt(-1 * roll / (3.0f)*(3-a), new PointF(r, r));
            RollPitchGraphics.Transform = m;


            SolidBrush b111;
            //连接判断
            if (isConnect)
            {
                b111 = new SolidBrush(ColorConnect);
            }
            else
            {
                b111 = new SolidBrush(ColorDisConnect);
            }
            int r1 = (int)(0.03f * (RollPitchWidth > RollPitchHeight ? RollPitchHeight : RollPitchWidth));
            int topedge = (int)(0.02f * RollPitchHeight);
            int rightedge = (int)(0.02f * RollPitchWidth);
            //电机启动判断
            if (isArmed){
                RollPitchGraphics.FillEllipse(b111, new Rectangle(
                  (int)r - (int)(RollPitchWidth / 2) + RollPitchWidth - 2 * r1 - rightedge,
                  topedge + (int)r - (int)(RollPitchHeight / 2) + (int)(0.03F * this.Height),
                 2* r1, 2*r1));
            }else { 
              RollPitchGraphics.FillEllipse(b111, new Rectangle(
                (int)r - (int)(RollPitchWidth / 2) + RollPitchWidth - 2 * r1 - rightedge,
                topedge + (int)r - (int)(RollPitchHeight / 2) + (int)(0.03F * this.Height),
                r1, r1));          
            }
            Rectangle rect_dis = new Rectangle((int)r - (int)(RollPitchWidth / 2), (int)r - (int)(RollPitchHeight / 2), RollPitchWidth, RollPitchHeight);
            Rectangle rect = new Rectangle(this.LeftEdge, this.TopEdge, RollPitchWidth, RollPitchHeight);
            ControlGraphics.DrawImage(RollPitchBitmap, rect, rect_dis, System.Drawing.GraphicsUnit.Pixel);
  
        }
        bool isArmed = false;
        bool isShowWarinning = false;
        string warinning = "油门低";
        Color ColorWarinning = Color.Yellow;
        private void DrawWarnning() {
            if (!isShowWarinning) return;

            int RollPitchWidth = ControlWidth - LeftEdge - RightEdge;
            int RollPitchHeight = ControlHeight - BottomEdge - TopEdge;
            SolidBrush b = new SolidBrush(ColorWarinning);

            Font f = new Font("宋体", 0.1f * (RollPitchWidth > RollPitchHeight ? RollPitchHeight : RollPitchWidth));

            SizeF size = ControlGraphics.MeasureString(warinning, f);

            ControlGraphics.DrawString(warinning, f, b, new PointF(LeftEdge + (RollPitchWidth - size.Width) / 2, TopEdge + RollPitchHeight*0.18f));
        }

        public void SetWarnning(bool visiablewarining,string warn,Color color) {
            isShowWarinning = visiablewarining;
            warinning = warn;
            ColorWarinning = color;
            this.Invalidate();
        }
        public void SetArmed(bool isArm) {
            isArmed = isArm;
            this.Invalidate();
        }

        public bool GetArmed()
        {
            return isArmed;
        }

        float YawHight;
        void DrawYawState()
        {
            YawHight = 1f * (TopEdge);

            Bitmap BitmapScale = new Bitmap(2 * ControlWidth, (int)YawHight+1);
            Graphics BitmapGraphics = Graphics.FromImage(BitmapScale);

            Pen p = new Pen(Color.Black);
            SolidBrush fontBrush = new SolidBrush(Color.Black);
            SolidBrush backBrush = new SolidBrush(Color.FromArgb(130, Color.White));
            SolidBrush b = new SolidBrush(Color.FromArgb(255,Color.NavajoWhite));
            //this.BackColor = Color.RoyalBlue;
            BitmapGraphics.FillRectangle(b, new Rectangle(0, 0, 2 * ControlWidth, (int)YawHight + 1));

           // Font font = new Font("宋体", (int)(0.03 * this.ControlWidth));

         //   Font font = new Font(fCollection.Families[0].Name, (int)(0.03 * this.ControlWidth));
            Font font = new Font(".Lock Clock", (int)(0.03 * this.ControlWidth));

            float everyscale = 30;//每个刻度代表的度数
            float middlewidth = 0.1f * ControlWidth;
            //width == 0--360 

            float resolution = ((float)this.ControlWidth) / 360.0f;//每一度需要的像素点

            //计算开始画线的位置以及角度

            //画线
            for (int i = 0; i * resolution * everyscale <= 2 * ControlWidth; i++)
            {
                float ang = (i) * everyscale >= 360 ? (i) * everyscale - 360 : (i) * everyscale;
                string str = (ang).ToString();
                SizeF size = BitmapGraphics.MeasureString(str, font);
                BitmapGraphics.DrawString(str, font, fontBrush, i * resolution * everyscale - size.Width / 2, 0.2f * YawHight);
                BitmapGraphics.DrawLine(p, i * resolution * everyscale, 0.8f * YawHight, i * resolution * everyscale, YawHight);
            }

            //  float startangle = ;
            int startloc = (int)(resolution * (yaw > 180 ? yaw - 180 : yaw + 180));

            Rectangle dis = new Rectangle(0, 0, ControlWidth, (int)YawHight);
            Rectangle res = new Rectangle(startloc, 0, ControlWidth, (int)YawHight);

            ControlGraphics.DrawImage(BitmapScale, dis, res, System.Drawing.GraphicsUnit.Pixel);

            ControlGraphics.FillRectangle(b, (ControlWidth - middlewidth) / 2, 0, middlewidth, YawHight);

            yaw =(float) System.Math.Round((double)yaw, 1);

            Font f = new System.Drawing.Font("叶根友毛笔行书3.0版", 0.14f * (middlewidth > YawHight ? YawHight : middlewidth) * (6 - 0.8f * yaw.ToString().Length), FontStyle.Bold);
            
            SizeF s = ControlGraphics.MeasureString(yaw.ToString("0"),f);
            
            ControlGraphics.FillRectangle(backBrush, (ControlWidth - middlewidth) / 2, 0, middlewidth, YawHight);
            ControlGraphics.DrawString(yaw.ToString("0"), f, fontBrush, new PointF((ControlWidth - s.Width) / 2, (YawHight - s.Height) / 2));
        }

        void DrawSpeed()
        {
            int x_offset = 0;
            int y_offset = this.TopEdge;

            int speedWidth = this.LeftEdge;
            int speedHeigh = this.ControlHeight-this.TopEdge;


            Font font = new System.Drawing.Font("宋体", 8,FontStyle.Bold);

            Bitmap BitmapSpeed = new Bitmap(speedWidth, speedHeigh);
            Graphics SpeedGraphics = Graphics.FromImage(BitmapSpeed);

            Pen PenLine = new Pen(Color.Black, 8);
            SolidBrush backBrush = new SolidBrush(Color.FromArgb(255, Color.White));
            Pen p = new Pen(Color.Black);
            SolidBrush lineBrush = new SolidBrush(Color.Black);

            SpeedGraphics.FillRectangle(backBrush, 0, 0, speedWidth, speedHeigh);
            SpeedGraphics.DrawLine(PenLine, speedWidth, 0, speedWidth, speedHeigh);




            float resolution = 0.25f;//每个像素点表示的速度
            float startalt = speed - speedHeigh / 2 * resolution;
            float endalt = speed + speedHeigh / 2 * resolution;
            float everyscale = 2;//每个刻度表示的速度
            float shortlength = 0.2f * speedWidth;
            float longlenght = 0.3f * speedWidth;

            float sizey = 0;
            //向上画线
            for (int i = 0; speedHeigh / 2 > (everyscale / resolution) * i; i++)
            {
                if (i % 5 == 0)
                {
                    SpeedGraphics.DrawLine(p, speedWidth - longlenght, speedHeigh / 2 - (everyscale / resolution) * i, speedWidth, speedHeigh / 2 - (everyscale / resolution) * i);

                    string str = Convert.ToDouble(speed + i * (everyscale)).ToString("0.00");



                    SizeF size = SpeedGraphics.MeasureString(str, font);
                    SpeedGraphics.DrawString(str, font, lineBrush, new PointF(speedWidth - longlenght - size.Width * 1.2f, speedHeigh / 2 - (everyscale / resolution) * i - size.Height / 2));
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

                    //string str = (speed - i * (everyscale )).ToString();
                    string str = Convert.ToDouble(speed - i * (everyscale)).ToString("0.00");

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

            pp[1].X = speedWidth - longlenght;
            pp[1].Y = pp[0].Y - sizey / 2.0f * 1.5f;
            pp[2].X = 0;
            pp[2].Y = pp[1].Y;
            pp[3].X = pp[2].X;
            pp[3].Y = pp[0].Y + sizey / 2.0f * 1.5f;
            pp[4].X = pp[1].X;
            pp[4].Y = pp[3].Y;



            SolidBrush b1 = new SolidBrush(Color.FromArgb(50, Color.Red));
            SpeedGraphics.FillPolygon(b1, pp);

            Rectangle dis = new Rectangle(x_offset, y_offset, speedWidth, speedHeigh);
            Rectangle res = new Rectangle(0, 0, speedWidth, speedHeigh);

            ControlGraphics.DrawImage(BitmapSpeed, dis, res, System.Drawing.GraphicsUnit.Pixel);


        }

       public  void Draw_ConnectRound(Graphics RoundGraphics){
           int r = (int)(0.05f * (ControlWidth > ControlHeight ? ControlHeight : ControlWidth));
           int topedge = (int)(0.2f * ControlHeight);
           int rightedge = (int)(0.2f * ControlWidth);

           SolidBrush b = new SolidBrush(Color.Red);

           RoundGraphics.FillEllipse(b, new Rectangle(this.Width-2*r-rightedge,topedge,2*r,2*r));


       }
       void DrawGPS()
       {
           Font font = new Font("宋体", 0.028f * (this.Width > this.Height ? this.Height : this.Width));
           SizeF sizelat = ControlGraphics.MeasureString(lat.ToString("0.0000000"), font);
           SizeF sizelng = ControlGraphics.MeasureString(lng.ToString("0.0000000"), font);
           SolidBrush b = new SolidBrush(Color.NavajoWhite);

           ControlGraphics.DrawString(string.Format("LAT:{0}", lat.ToString("0.0000000")), font, b, new PointF(LeftEdge, this.Height - 1.2f * (sizelat.Height + sizelng.Height)));
           ControlGraphics.DrawString(string.Format("LNG:{0}", lng.ToString("0.0000000")), font, b, new PointF(LeftEdge, this.Height - 1.2f * (sizelat.Height)));


           string str;
           if (distance > 1000)
           {
               str = (this.distance / 1000).ToString() + "KM";
           }
           else
           {
               str = this.distance.ToString() + "m";
           }
           SizeF size = ControlGraphics.MeasureString(str, font);
           ControlGraphics.DrawString(str, font, b, new PointF(LeftEdge, Height - 1.2f * (sizelat.Height + sizelng.Height + size.Height)));
       }
        
        void DrawAltitude()
        {
            int x_offset = this.ControlWidth-this.RightEdge;
            int y_offset = this.TopEdge;

            int altWidth = this.RightEdge;
            int altHeigh = this.ControlHeight-this.TopEdge-this.BottomEdge;

            float resolution = 0.1f;//每个像素点表示的高度
            float startalt = alt - altHeigh / 2 * resolution;
            float endalt = alt + altHeigh / 2 * resolution;
            float everyscale = 1;//每个刻度表示的高度
            float shortlength = 0.2f * altWidth;
            float longlenght = 0.3f * altWidth;

            Font font = new System.Drawing.Font("宋体", 8,FontStyle.Bold);

            Bitmap BitmapAlt = new Bitmap(altWidth, altHeigh);
            Graphics AltGraphics = Graphics.FromImage(BitmapAlt);

            Pen PenLine = new Pen(Color.Black, 8);

            SolidBrush backBrush = new SolidBrush(Color.FromArgb( 255,Color.White));
            SolidBrush lineBrush = new SolidBrush(Color.Black);
            AltGraphics.FillRectangle(backBrush, 0, 0, altWidth, altHeigh);

            //AltGraphics.DrawRectangle(PenLine, 0, 0, altWidth, altHeigh);

            AltGraphics.DrawLine(PenLine, 0, 0, 0, altHeigh);


            //   float curalt = alt - resolution * altHeigh / 2;

            Pen p = new Pen(Color.Black);
            float sizey = 0;
            //向上画线
            for (int i = 0; altHeigh / 2 > (everyscale / resolution) * i; i++)
            {
                if (i % 5 == 0)
                {
                    AltGraphics.DrawLine(p, 0, altHeigh / 2 - (everyscale / resolution) * i, longlenght, altHeigh / 2 - (everyscale / resolution) * i);

                 //   string str = (alt + i * (everyscale)).ToString();
                    string str = Convert.ToDouble(alt + i * (everyscale)).ToString("0.00");

                    SizeF size = AltGraphics.MeasureString(str, font);
                    AltGraphics.DrawString(str, font, lineBrush, new PointF(longlenght + 5, altHeigh / 2 - (everyscale / resolution) * i - size.Height / 2));
                    sizey = sizey > size.Height ? sizey : size.Height;
                }
                else
                {
                    AltGraphics.DrawLine(p, 0, altHeigh / 2 - (everyscale / resolution) * i, shortlength, altHeigh / 2 - (everyscale / resolution) * i);

                }
            }
            //向下画线
            for (int i = 0; altHeigh / 2 > (everyscale / resolution) * i; i++)
            {
                if (i % 5 == 0)
                {
                    AltGraphics.DrawLine(p, 0, altHeigh / 2 + (everyscale / resolution) * i, longlenght, altHeigh / 2 + (everyscale / resolution) * i);

                  //  string str = (alt - i * (everyscale)).ToString();
                    string str = Convert.ToDouble(alt - i * (everyscale)).ToString("0.00");

                    SizeF size = AltGraphics.MeasureString(str, font);
                    AltGraphics.DrawString(str, font, lineBrush, new PointF(longlenght + 5, altHeigh / 2 + (everyscale / resolution) * i - size.Height / 2));
                    sizey = sizey > size.Height ? sizey : size.Height;
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
            pp[1].Y = pp[0].Y - sizey / 2.0f * 1.5f;
            pp[2].X = altWidth;
            pp[2].Y = pp[1].Y;
            pp[3].X = pp[2].X;
            pp[3].Y = pp[0].Y + sizey / 2.0f * 1.5f;
            pp[4].X = pp[1].X;
            pp[4].Y = pp[3].Y;



            SolidBrush b1 = new SolidBrush(Color.FromArgb(50, Color.Red));
            // b1.Color = Color.FromArgb(20,);
            // b1.Color = Color.Olive;
            AltGraphics.FillPolygon(b1, pp);





            Rectangle dis = new Rectangle(x_offset, y_offset, altWidth, altHeigh);
            Rectangle res = new Rectangle(0, 0, altWidth, altHeigh);

            ControlGraphics.DrawImage(BitmapAlt, dis, res, System.Drawing.GraphicsUnit.Pixel);

        }

        void DrawVoltageCurrent() {
            string votagestr;
            string currentstr;
            string targetheightstr;

            votagestr =  (votage/1000.0f).ToString("0.00") + "V";
            currentstr = (current/1000.0f).ToString("0.00") + "A";
            targetheightstr = targetheight.ToString() + "m";

            Font f = new System.Drawing.Font("宋体", 0.03f * (this.Width - this.LeftEdge - RightEdge));
            SizeF size = ControlGraphics.MeasureString(votagestr, f);
            SizeF size1 = ControlGraphics.MeasureString(currentstr, f);
            SizeF size2 = ControlGraphics.MeasureString(targetheightstr, f);

            SolidBrush b = new SolidBrush(Color.NavajoWhite);

            ControlGraphics.DrawString(votagestr, f, b, new PointF(0.98f * (this.Width - this.RightEdge) -(size.Width>size1.Width?size.Width:size1.Width), (this.Height) - 2.2f * size.Height));
            ControlGraphics.DrawString(currentstr, f, b, new PointF(0.98f * (this.Width - this.RightEdge) - (size.Width > size1.Width ? size.Width : size1.Width), (this.Height) - 3.2f * size1.Height));
           // ControlGraphics.DrawString(targetheightstr, f, b, new PointF(0.96f * (this.Width - this.RightEdge) - (size.Width > size1.Width ? size.Width : size1.Width), (this.Height) - 4.2f * size1.Height));
            ControlGraphics.DrawString(targetheightstr, f, b, new PointF((0.98f * (this.Width - this.RightEdge) -size2.Width*1.2f), (this.Height) - 4.2f * size.Height));
        }
        private void Control_paint(object sender, PaintEventArgs e)
        {

            DrawSpeed();
            DrawAltitude();
            DrawYawState();
            DrawRollPitchState();

            draw_plane_focus();
            DrawGPS();
            DrawNumberOfSatellites();
            Font f=new System.Drawing.Font("宋体",0.03f*(this.Width-this.LeftEdge-RightEdge));
            //string rssi_str = "RSSI " + rssi.ToString();
            string rssi_str = "RSSI " + (rssi/10).ToString();
            SizeF size = ControlGraphics.MeasureString(rssi_str, f);
            SolidBrush b = new SolidBrush(Color.NavajoWhite);
            //ControlGraphics.DrawString(this.count.ToString(), f, b, new PointF(0.95f * (this.Width - this.RightEdge) - size.Width, (this.Height) - 1.2f * size.Height));
            ControlGraphics.DrawString(rssi_str, f, b, new PointF(0.97f * (this.Width - this.RightEdge) - size.Width, (this.Height) - 1.2f * size.Height));

            DrawWarnning();

            DrawVoltageCurrent();

            Graphics g = e.Graphics;
            g.DrawImage(ControlBitamp, 0, 0);
        }
        private void ControlResize(object sender, EventArgs e)
        {
            if (this.Width < 10 || this.Height<10) {
                return;
            }
            this.ControlWidth = this.Width;
            this.ControlHeight = this.Height;
            LeftEdge = (int)(0.15 * ControlWidth);
            RightEdge = (int)(0.15 * ControlWidth);
            BottomEdge = (int)(0.00 * ControlHeight);
            TopEdge = (int)(0.13 * ControlHeight);

          //  ControlBitamp.Dispose();
          //  ControlGraphics.Dispose();

            //初始化 内存控件画布
            ControlBitamp = new Bitmap(ControlWidth, ControlHeight);
            ControlGraphics = Graphics.FromImage(ControlBitamp);
        }

        public void SetTargetHeight(float height) {
            this.targetheight = height;
            this.Invalidate();
        }

        public void SetAlt(float alt) {
            this.alt = alt;
            this.Invalidate();
        }
        public void Set_Roll(float r) {
            this.roll = (float)(r);
            this.Invalidate();
        }
        public void Set_Pitch(float p)
        {
            this.pitch = (float)(p);
            this.Invalidate();
        }
        public void Set_Yaw(float y)
        {
            this.yaw = (float)(y );
            this.Invalidate();
        }
        public void SetConnect(bool connect) {
            this.isConnect = connect;
            this.Invalidate();
        }
        public void Set_Speed(float speed) {
            this.speed = speed;
            this.Invalidate();
        }
        public void SetLat(double la)
        {
            this.lat = la;
            this.Invalidate();
        }
        public void SetSatelliteNumber(int number)
        {
            this.SatellitesNumber = number;
            this.Invalidate();
        }

        public void SetVotage(float vol) {
            this.votage = vol;
            this.Invalidate();
        }
        public void SetCurrent(float cur)
        {
            this.current = cur;
            this.Invalidate();
        }
        public void SetLng(double ln)
        {
            this.lng = ln;
            this.Invalidate();
        }
        public void SetCount(int cou) {
            this.count = cou;
            this.Invalidate();
        }
        public void PlusOneCount() {
            this.count ++;
            this.Invalidate();
        }
        public void SetRSSI(int rssi_value){
            rssi = rssi_value;
        }
        //以米为单位
        public void setDistance(int dista) {
            this.distance = dista;
            this.Invalidate();
        }
        /// <summary>
        /// 设置 当前飞行器为多轴还是固定翼，系统默认多轴
        /// </summary>
        /// <param name="isMu">true表示系统为多轴，false表示为固定翼</param>
        public void SetPlaneType(bool isMu) {
            this.isMultiaxial = isMu;
            this.Invalidate();
        }
  
    
    }
}
