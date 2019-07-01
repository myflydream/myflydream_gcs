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
    public partial class ImageButton : UserControl
    {
        Image image ;

        Image imagedown;
        Image imageup;

        public void SetImage(Image value) {
            imageup = value;
            image = value;
        }

        public void SetImageDown(Image value) {
            imagedown = value;

        }
        public ImageButton()
        {
            InitializeComponent();
            this.BackColor = Color.Transparent;
            this.Paint += ImageButton_Paint;

            this.MouseDown += ImageButton_MouseDown;
            this.MouseUp += ImageButton_MouseUp;
            //使用默认双缓冲
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.ResizeRedraw |
                    ControlStyles.AllPaintingInWmPaint, true);
            SetImage(global::SpaceArrow.Properties.Resources.mapzoomplus);
            SetImageDown(global::SpaceArrow.Properties.Resources.mapzomplusdown);
            image = imageup;
        }

        void ImageButton_MouseUp(object sender, MouseEventArgs e)
        {
            //throw new NotImplementedException();
           // this.BackColor = Color.Transparent;

            image = imageup;
            this.Invalidate();
        }

        void ImageButton_MouseDown(object sender, MouseEventArgs e)
        {
            //throw new NotImplementedException();
            //this.BackColor = Color.DarkTurquoise;
            image = imagedown;
            this.Invalidate();

        }
        void ImageButton_Paint(object sender, PaintEventArgs e)
        {
            if (image != null)
            {
                Graphics g = e.Graphics;
                RectangleF rect_dis = new RectangleF(0, 0, image.Width, image.Height);
                RectangleF rect = new RectangleF(0, 0, this.Width, this.Height);
                g.DrawImage(image, rect, rect_dis, System.Drawing.GraphicsUnit.Pixel);
            }
        }
    }
}
