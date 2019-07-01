using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpaceArrow.Control
{
    public partial class MyImageButton : UserControl
    {
      //  Bitmap bitmap = new Bitmap("E:\\workspace\\VS2013_workspace\\SpaceArrow\\SpaceArrow\\bin\\Debug\\Connect.png");

        string filename =null;
        Graphics ControlGraphicsOn;

        Color corlor_back;

        public Color colorNor;
        Bitmap bitPicture;




        public MyImageButton()
        {
            InitializeComponent();
            this.Paint += MyImageButton_Paint;
            this.MouseEnter += MyImageButton_MouseEnter;
            this.MouseLeave += MyImageButton_MouseLeave;


            ControlGraphicsOn = this.CreateGraphics();
            ControlGraphicsOn.Clear(Color.Transparent);

            colorNor = Color.Transparent;
            corlor_back = colorNor;

            try
            {
                button_img[0] = new Bitmap(this.filename + ".png");
                button_img[1] = new Bitmap(this.filename + "y.png");
                button_img[2] = new Bitmap(this.filename + "g.png");
            }
            catch
            {
                bitPicture = new Bitmap(this.Width, this.Height);
                Graphics gg = Graphics.FromImage(bitPicture);
                gg.Clear(Color.Transparent);

                SolidBrush bb111 = new SolidBrush(Color.Black);

                Font f = new Font("宋体", 8);

                gg.DrawString("ImageButton", f, bb111, new PointF(0, 0));

                button_img[0] = bitPicture;
                button_img[1] = bitPicture;
                button_img[2] = bitPicture;
            }
            this.Resize += MyImageButton_Resize;

        }

        void MyImageButton_Resize(object sender, EventArgs e)
        {
            //throw new NotImplementedException();

            this.Invalidate();
        }

        void MyImageButton_MouseLeave(object sender, EventArgs e)
        {
            corlor_back = colorNor;
            this.Invalidate();
        }


        public static Bitmap ToGray(Bitmap bmp)
        {
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    //获取该点的像素的RGB的颜色
                    Color color = bmp.GetPixel(i, j);
                    //利用公式计算灰度值
                    int gray = (int)(color.R * 0.3 + color.G * 0.59 + color.B * 0.11);
                    Color newColor = Color.FromArgb(gray, gray, gray);
                    bmp.SetPixel(i, j, newColor);
                }
            }
            return bmp;
        }

        void MyImageButton_MouseEnter(object sender, EventArgs e)
        {

            corlor_back = Color.DarkTurquoise;
            this.Invalidate();

        }

        public Color colorround = Color.Gray;
        public bool isShowRound = false;

        public void SetColor(Color color) {
            this.colorround = color;
            this.Invalidate();
        }

        Bitmap[] button_img = new Bitmap[3];
        
        void DrawRoundRectangle(Graphics g) {
            Bitmap ControlBitMap = new Bitmap(this.Width, this.Height);
            Graphics ControlGraphics = Graphics.FromImage(ControlBitMap);



            ControlGraphics.Clear(corlor_back);

            Rectangle rect = new Rectangle(0, 0, button_img[2].Width, button_img[2].Height);
            Rectangle dist = new Rectangle(0, 0, this.Width, this.Height);
            if (colorround == Color.LightGreen)
            {
                ControlGraphics.DrawImage(button_img[2], dist, rect, System.Drawing.GraphicsUnit.Pixel);
            }
            else if(colorround == Color.Red)
            {
                ControlGraphics.DrawImage(button_img[1], dist, rect, System.Drawing.GraphicsUnit.Pixel);
            }
            else
            {
                ControlGraphics.DrawImage(button_img[0], dist, rect, System.Drawing.GraphicsUnit.Pixel);
            }
            g.DrawImage(ControlBitMap,0,0);


        }
        void MyImageButton_Paint(object sender, PaintEventArgs e)
        {
            DrawRoundRectangle(e.Graphics);
        }

        public void SetImage(string fileName)
        {
            if (fileName.EndsWith("1"))
            {
                button_img[0] = button_img[1] = button_img[2] = global::SpaceArrow.Properties.Resources._1;
            }
            else if (fileName.EndsWith("2"))
            {
                button_img[0] = button_img[1] = button_img[2] = global::SpaceArrow.Properties.Resources._2;
            }
            else if (fileName.EndsWith("3"))
            {
                button_img[0] = button_img[1] = button_img[2] = global::SpaceArrow.Properties.Resources._3;
            }
            else if (fileName.EndsWith("4"))
            {
                button_img[0] = global::SpaceArrow.Properties.Resources._4;
                button_img[1] = global::SpaceArrow.Properties.Resources._4y;
                button_img[2] = global::SpaceArrow.Properties.Resources._4g;
            } else if (fileName.EndsWith("5"))
            {
                button_img[0] = global::SpaceArrow.Properties.Resources._5;
                button_img[1] = global::SpaceArrow.Properties.Resources._5y;
                button_img[2] = global::SpaceArrow.Properties.Resources._5g;
            } else if (fileName.EndsWith("6"))
            {
                button_img[0] = global::SpaceArrow.Properties.Resources._6;
                button_img[1] = global::SpaceArrow.Properties.Resources._6y;
                button_img[2] = global::SpaceArrow.Properties.Resources._6g;
            } else if (fileName.EndsWith("7"))
            {
                button_img[0] = global::SpaceArrow.Properties.Resources._7;
                button_img[1] = global::SpaceArrow.Properties.Resources._7y;
                button_img[2] = global::SpaceArrow.Properties.Resources._7g;
            }
            else if (fileName.EndsWith("8"))
            {
                button_img[0] = global::SpaceArrow.Properties.Resources._8;
                button_img[1] = global::SpaceArrow.Properties.Resources._8y;
                button_img[2] = global::SpaceArrow.Properties.Resources._8g;
            }
            else if (fileName.EndsWith("9"))
            {
                button_img[0] = global::SpaceArrow.Properties.Resources._9;
                button_img[1] = global::SpaceArrow.Properties.Resources._9y;
                button_img[2] = global::SpaceArrow.Properties.Resources._9g;
            }
            this.Invalidate();
        }
        
        //public void SetImage(string fileName) {
        //    try
        //    {
        //        this.filename = fileName;
        //     //   bitPicture = new Bitmap(this.filename);

        //        if (filename.EndsWith("1") || filename.EndsWith("2") || filename.EndsWith("3"))
        //        {
        //            button_img[0] = button_img[1] = button_img[2] = new Bitmap(this.filename + ".png");
        //        }
        //        else
        //        {
        //            button_img[0] = new Bitmap(this.filename + ".png");
        //            button_img[1] = new Bitmap(this.filename + "y.png");
        //            button_img[2] = new Bitmap(this.filename + "g.png");
        //        }
        //    }
        //    catch
        //    { 
            
        //    }

        //    this.Invalidate();
        //}
        public void SetNorcolor(Color color)
        {
            this.colorNor = color;
            this.Invalidate();
        }

        public void SetEnable(bool en) {
         //   this.Enabled = en;
           // this.Invalidate();

        }
    }
}
