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
    public partial class IconForm : Form
    {
        const int DEFAULT_WPBOOK_WIDTHHEIGHT = 70;//20
        const int DEFAULT_PLANE_WIDTHHEIGHT = 20;//68
        const int DEFAULT_HOME_WIDTHHEIGHT = 70;//32

        enum SizeType{
            DefaultSize=0,
            OriginSize,
            CustomSize,
        }
        public delegate void SetBitmapDele(Bitmap bitmap,bool isUseFile,string filepath);
        public SetBitmapDele SetHomeBitmap = null;
        public SetBitmapDele SetPlaneBitmap = null;
        public SetBitmapDele SetWpBitmap = null;
        public SetBitmapDele SetBookBitmap = null;

        Rectangle rect = new Rectangle(0, 0, 350, 227);

        //默认原始图像
        Bitmap bitmapOriginhome;
        Bitmap bitmapOriginplane;
        Bitmap bitmapOriginwpmarker;
        Bitmap bitmapOriginbookmarker;

        //绘制图像
        Bitmap bitmapPainthome;
        Bitmap bitmapPaintplane;
        Bitmap bitmapPaintwpmarker;
        Bitmap bitmapPaintbookmarker;

        //文件读取图像
        Bitmap bitmapFilehome;
        Bitmap bitmapFileplane;
        Bitmap bitmapFilewpmarker;
        Bitmap bitmapFilebookmarker;

        //文件读取图像路径
        string filepathhome = "";
        string filepathplane = "";
        string filepathwpmarker = "";
        string filepathbookmarker = "";

        //是否采用文件图像
        bool boolusefilehome=false;
        bool boolusefileplane = false;
        bool boolusefilewpmarker = false;
        bool boolusefilebookmarker = false;
     


        public IconForm()
        {
            InitializeComponent();
            this.Icon = global::SpaceArrow.Properties.Resources.mpdesktop;
            this.Text = "GCS-图标管理器";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ShowInTaskbar = false;

            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            //this.radioButton_bookmarker_defaulticon.Checked = true;
            //this.radioButton_home_defauleicon.Checked = true;
            //this.radioButton_plane_defaulticon.Checked = true;
            //this.radioButton_wpmarker_defaulticon.Checked = true;

            this.tabPage_home.BackColor = this.BackColor;
            this.tabPage_plane.BackColor = this.BackColor;
            this.tabPage_bookmarker.BackColor = this.BackColor;
            this.tabPage_wpmarker.BackColor = this.BackColor;

            this.tabPage_home.Paint += tabPage_home_Paint;
            this.tabPage_wpmarker.Paint += tabPage_wpmarker_Paint;
            this.tabPage_plane.Paint += tabPage_plane_Paint;
            this.tabPage_bookmarker.Paint += tabPage_bookmarker_Paint;
        }


        void tabPage_bookmarker_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(this.BackColor);
            SolidBrush b = new SolidBrush(Color.White);
            Rectangle rect = new Rectangle(0, 0, 350, 227);
            g.FillRectangle(b, rect);
            if (bitmapPaintbookmarker == null)
            {
                if (bitmapOriginbookmarker != null)
                {
                    bitmapPaintbookmarker = bitmapOriginbookmarker;
                }
            }
            if (bitmapPaintbookmarker != null)
            {
                Rectangle rect_src = new Rectangle(0, 0, bitmapPaintbookmarker.Width, bitmapPaintbookmarker.Height);
                Rectangle rect_dest = new Rectangle(rect.X + rect.Width / 2 - bitmapPaintbookmarker.Width / 2,
                    rect.Y + rect.Height / 2 - bitmapPaintbookmarker.Height / 2,
                    bitmapPaintbookmarker.Width, bitmapPaintbookmarker.Height
                    );
                g.DrawImage(bitmapPaintbookmarker, rect_dest, rect_src, GraphicsUnit.Pixel);
            }

        }
        void tabPage_plane_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(this.BackColor);
            SolidBrush b = new SolidBrush(Color.White);
            g.FillRectangle(b, rect);

            if (bitmapPaintplane == null)
            {
                if (bitmapOriginplane != null)
                {
                    bitmapPaintplane = bitmapOriginplane;
                }
            }
            if (bitmapPaintplane != null)
            {
                Rectangle rect_src = new Rectangle(0, 0, bitmapPaintplane.Width, bitmapPaintplane.Height);
                Rectangle rect_dest = new Rectangle(rect.X + rect.Width / 2 - bitmapPaintplane.Width / 2,
                    rect.Y + rect.Height / 2 - bitmapPaintplane.Height / 2,
                    bitmapPaintplane.Width, bitmapPaintplane.Height
                    );
                g.DrawImage(bitmapPaintplane, rect_dest, rect_src, GraphicsUnit.Pixel);
            }
        }
        void tabPage_wpmarker_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(this.BackColor);
            SolidBrush b = new SolidBrush(Color.White);
            g.FillRectangle(b, rect);

            if (bitmapPaintwpmarker == null)
            {
                if (bitmapOriginwpmarker != null)
                {
                    bitmapPaintwpmarker = bitmapOriginwpmarker;
                }
            }
            if (bitmapPaintwpmarker != null)
            {
                Rectangle rect_src = new Rectangle(0, 0, bitmapPaintwpmarker.Width, bitmapPaintwpmarker.Height);
                Rectangle rect_dest = new Rectangle(rect.X + rect.Width / 2 - bitmapPaintwpmarker.Width / 2,
                    rect.Y + rect.Height / 2 - bitmapPaintwpmarker.Height / 2,
                    bitmapPaintwpmarker.Width, bitmapPaintwpmarker.Height
                    );

                g.DrawImage(bitmapPaintwpmarker, rect_dest, rect_src, GraphicsUnit.Pixel);
            }
        }
        void tabPage_home_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(this.BackColor);
            SolidBrush b = new SolidBrush(Color.White);
            g.FillRectangle(b,rect);

            if (bitmapPainthome == null) {
                if (bitmapOriginhome != null) {
                    bitmapPainthome = bitmapOriginhome;
                }
            }
            if (bitmapPainthome != null) {
                Rectangle rect_src = new Rectangle(0, 0, bitmapPainthome.Width, bitmapPainthome.Height);
                Rectangle rect_dest = new Rectangle(rect.X + rect.Width / 2 - bitmapPainthome.Width/2, 
                    rect.Y + rect.Height / 2 - bitmapPainthome.Height / 2,
                    bitmapPainthome.Width, bitmapPainthome.Height
                    );

                g.DrawImage(bitmapPainthome, rect_dest, rect_src, GraphicsUnit.Pixel);
                
            }
        }

        public void SetParaHomeBitmap(Bitmap bitmap, Bitmap bitmapnow)
        {
            bitmapFilehome = bitmapnow;
            bitmapOriginhome = bitmap;
            this.bitmapPainthome = bitmapnow;
            this.textBox_home_iconheight.Text = bitmap.Height.ToString();
            this.textBox_home_iconwidth.Text = bitmap.Width.ToString();
            this.tabPage_home.Invalidate();
        }
        public void SetParaPlaneBitmap(Bitmap bitmap, Bitmap bitmapnow)
        {
            bitmapOriginplane = bitmap;
            bitmapFileplane = bitmapnow;
            this.bitmapPaintplane = bitmapnow;
            this.textBox_plane_iconheight.Text = bitmap.Height.ToString();
            this.textBox_plane_iconwidth.Text = bitmap.Width.ToString();
            this.tabPage_plane.Invalidate();
        }
        public void SetParaWpBitmap(Bitmap bitmap, Bitmap bitmapnow)
        {
            bitmapOriginwpmarker = bitmap;
            bitmapFilewpmarker = bitmapnow;
            this.bitmapPaintwpmarker= bitmapnow;
            this.textBox_wpmarker_iconheight.Text = bitmap.Height.ToString();
            this.textBox_wpmarker_iconwidth.Text = bitmap.Width.ToString();
            this.tabPage_wpmarker.Invalidate();
        }
        public void SetParaBookBitmap(Bitmap bitmap, Bitmap bitmapnow)
        {
            bitmapOriginbookmarker = bitmap;
            bitmapFilebookmarker = bitmapnow;
            this.bitmapPaintbookmarker= bitmapnow;
            this.textBox_bookmarker_iconheight.Text = bitmap.Height.ToString();
            this.textBox_bookmarker_iconwidth.Text = bitmap.Width.ToString();
            this.tabPage_bookmarker.Invalidate();
        }

        private void radioButton_bookmarker_defaultsize_CheckedChanged(object sender, EventArgs e)
        {
            Bitmap bit = new Bitmap(bitmapPaintbookmarker.Width, bitmapPaintbookmarker.Height);
            Graphics gbook = Graphics.FromImage(bit);
            gbook.DrawImage(bitmapPaintbookmarker,new Point(0,0));

            bitmapPaintbookmarker = new Bitmap(DEFAULT_WPBOOK_WIDTHHEIGHT, DEFAULT_WPBOOK_WIDTHHEIGHT);
            Graphics gpaint = Graphics.FromImage(bitmapPaintbookmarker);
            Rectangle rect_src = new Rectangle(0,0,bit.Width,bit.Height);
            Rectangle rect_dest = new Rectangle(0, 0, bitmapPaintbookmarker.Width, bitmapPaintbookmarker.Height);
            gpaint.DrawImage(bit, rect_dest, rect_src,GraphicsUnit.Pixel);

            this.textBox_bookmarker_iconheight.Text = bitmapPaintbookmarker.Height.ToString();
            this.textBox_bookmarker_iconwidth.Text = bitmapPaintbookmarker.Width.ToString();

            this.tabPage_bookmarker.Invalidate();
        }
        private void radioButton_bookmarker_originicon_CheckedChanged(object sender, EventArgs e)
        {
            if (bitmapFilebookmarker != null)
            {
                bitmapPaintbookmarker = bitmapFilebookmarker;
            }
            else {
                bitmapPaintbookmarker = bitmapOriginbookmarker;
            }                
            this.textBox_bookmarker_iconheight.Text = bitmapPaintbookmarker.Height.ToString();
            this.textBox_bookmarker_iconwidth.Text = bitmapPaintbookmarker.Width.ToString();
            this.tabPage_bookmarker.Invalidate();
            boolusefilebookmarker = true;
        }
        private void radioButton_bookmarker_defaulticon_CheckedChanged(object sender, EventArgs e)
        {
            if (bitmapOriginbookmarker == null) return;
            bitmapPaintbookmarker = bitmapOriginbookmarker;
            this.textBox_bookmarker_iconheight.Text = bitmapPaintbookmarker.Height.ToString();
            this.textBox_bookmarker_iconwidth.Text = bitmapPaintbookmarker.Width.ToString();
            this.tabPage_bookmarker.Invalidate();
            boolusefilebookmarker = false;
        }
        private void bookmarker_icon_keypress(object sender, KeyPressEventArgs e)
        {
     //       MessageBox.Show(e.KeyChar.ToString());

            if (e.KeyChar == '\r' ) {
                try
                {
                    int w = int.Parse(this.textBox_bookmarker_iconwidth.Text);
                    int h = int.Parse(this.textBox_bookmarker_iconheight.Text);

                    Bitmap bit = new Bitmap(bitmapPaintbookmarker.Width, bitmapPaintbookmarker.Height);
                    Graphics gbook = Graphics.FromImage(bit);
                    gbook.DrawImage(bitmapPaintbookmarker, new Point(0, 0));

                    bitmapPaintbookmarker = new Bitmap(w, h);
                    Graphics gpaint = Graphics.FromImage(bitmapPaintbookmarker);
                    Rectangle rect_src = new Rectangle(0, 0, bit.Width, bit.Height);
                    Rectangle rect_dest = new Rectangle(0, 0, bitmapPaintbookmarker.Width, bitmapPaintbookmarker.Height);
                    gpaint.DrawImage(bit, rect_dest, rect_src, GraphicsUnit.Pixel);

                    this.tabPage_bookmarker.Invalidate();

                }
                catch { 
                
                }
            }

        }
        private void button_bookmarker_changeicon_Click(object sender, EventArgs e)
        {
            OpenFileDialog loOpenFile = new OpenFileDialog();

            loOpenFile.Filter = "(*.png)|*.png|*.*|(*.jpg)|*.jpg|(*.icon)|*.icon|所有文件(*.*)";// "PDF文件(*.pdf)|*.pdf";
            loOpenFile.Title = "更改书签图标";
            loOpenFile.FilterIndex = 1;
            loOpenFile.RestoreDirectory = true;
            if (loOpenFile.ShowDialog() == DialogResult.OK)
            {
                filepathbookmarker = loOpenFile.FileName.ToString(); //获得文件路径 
                bitmapFilebookmarker = new Bitmap(filepathbookmarker);
                this.radioButton_bookmarker_defaultsize.Checked = true;

                Rectangle rect_src = new Rectangle(0, 0, bitmapFilebookmarker.Width, bitmapFilebookmarker.Height);
                Rectangle rect_dest = new Rectangle(0, 0, DEFAULT_WPBOOK_WIDTHHEIGHT, DEFAULT_WPBOOK_WIDTHHEIGHT);
                bitmapPaintbookmarker= new Bitmap(DEFAULT_WPBOOK_WIDTHHEIGHT, DEFAULT_WPBOOK_WIDTHHEIGHT);
                Graphics g = Graphics.FromImage(bitmapPaintbookmarker);
                g.DrawImage(bitmapFilebookmarker, rect_dest, rect_src, GraphicsUnit.Pixel);

                this.textBox_bookmarker_iconheight.Text = bitmapPaintbookmarker.Height.ToString();
                this.textBox_bookmarker_iconwidth.Text = bitmapPaintbookmarker.Width.ToString();
                this.tabPage_bookmarker.Invalidate();
                boolusefilebookmarker = true;
            }
        }
        private void button_bookmarker_confirmchange_Click(object sender, EventArgs e)
        {
            if (SetBookBitmap != null)
            {
                SetBookBitmap(bitmapPaintbookmarker, this.boolusefilebookmarker, filepathbookmarker);
            }
        }

        private void radioButton_home_defaultsize_CheckedChanged(object sender, EventArgs e)
        {
            Bitmap bit = new Bitmap(bitmapPainthome.Width, bitmapPainthome.Height);
            Graphics gbook = Graphics.FromImage(bit);
            gbook.DrawImage(bitmapPainthome, new Point(0, 0));

            bitmapPainthome = new Bitmap(DEFAULT_HOME_WIDTHHEIGHT, DEFAULT_HOME_WIDTHHEIGHT);
            Graphics gpaint = Graphics.FromImage(bitmapPainthome);
            Rectangle rect_src = new Rectangle(0, 0, bit.Width, bit.Height);
            Rectangle rect_dest = new Rectangle(0, 0, bitmapPainthome.Width, bitmapPainthome.Height);
            gpaint.DrawImage(bit, rect_dest, rect_src, GraphicsUnit.Pixel);

            this.textBox_home_iconheight.Text = bitmapPainthome.Height.ToString();
            this.textBox_home_iconwidth.Text = bitmapPainthome.Width.ToString();

            this.tabPage_home.Invalidate();
        }
        private void radioButton_home_originicon_CheckedChanged(object sender, EventArgs e)
        {
            if (bitmapFilehome != null)
            {
                bitmapPainthome = bitmapFilehome;
            }
            else
            {
                bitmapPainthome = bitmapOriginhome;
            }
            this.textBox_home_iconheight.Text = bitmapPainthome.Height.ToString();
            this.textBox_home_iconwidth.Text = bitmapPainthome.Width.ToString();
            this.tabPage_home.Invalidate();

            boolusefilehome =true;
        }
        private void radioButton_home_defauleicon_CheckedChanged(object sender, EventArgs e)
        {
            if (bitmapOriginhome == null) return;
            bitmapPainthome = bitmapOriginhome;
            this.textBox_home_iconheight.Text = bitmapPainthome.Height.ToString();
            this.textBox_home_iconwidth.Text = bitmapPainthome.Width.ToString();
            this.tabPage_home.Invalidate();
            boolusefilehome = false;
        }
        private void home_icon_keypress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                try
                {
                    int w = int.Parse(this.textBox_home_iconwidth.Text);
                    int h = int.Parse(this.textBox_home_iconheight.Text);

                    Bitmap bit = new Bitmap(bitmapPainthome.Width, bitmapPainthome.Height);
                    Graphics gbook = Graphics.FromImage(bit);
                    gbook.DrawImage(bitmapPainthome, new Point(0, 0));

                    bitmapPainthome = new Bitmap(w, h);
                    Graphics gpaint = Graphics.FromImage(bitmapPainthome);
                    Rectangle rect_src = new Rectangle(0, 0, bit.Width, bit.Height);
                    Rectangle rect_dest = new Rectangle(0, 0, bitmapPainthome.Width, bitmapPainthome.Height);
                    gpaint.DrawImage(bit, rect_dest, rect_src, GraphicsUnit.Pixel);

                    this.tabPage_home.Invalidate();

                }
                catch
                {

                }
            }

        }
        private void button_home_changeicon_Click(object sender, EventArgs e)
        {
            OpenFileDialog loOpenFile = new OpenFileDialog();

            loOpenFile.Filter = "(*.png)|*.png|*.*|(*.jpg)|*.jpg|(*.icon)|*.icon|所有文件(*.*)";// "PDF文件(*.pdf)|*.pdf";
            loOpenFile.Title = "更改HOME图标";
            loOpenFile.FilterIndex = 1;
            loOpenFile.RestoreDirectory = true;
            if (loOpenFile.ShowDialog() == DialogResult.OK)
            {
                filepathhome = loOpenFile.FileName.ToString(); //获得文件路径 
                bitmapFilehome = new Bitmap(filepathhome);
                this.radioButton_home_defaultsize.Checked = true;

                Rectangle rect_src = new Rectangle(0, 0, bitmapFilehome.Width, bitmapFilehome.Height);
                Rectangle rect_dest = new Rectangle(0, 0, DEFAULT_HOME_WIDTHHEIGHT, DEFAULT_HOME_WIDTHHEIGHT);
                bitmapPainthome = new Bitmap(DEFAULT_HOME_WIDTHHEIGHT, DEFAULT_HOME_WIDTHHEIGHT);
                Graphics g = Graphics.FromImage(bitmapPainthome);
                g.DrawImage(bitmapFilehome, rect_dest, rect_src, GraphicsUnit.Pixel);

                this.textBox_home_iconheight.Text = bitmapPainthome.Height.ToString();
                this.textBox_home_iconwidth.Text = bitmapPainthome.Width.ToString();
                this.tabPage_home.Invalidate();

                boolusefilehome = true;
            }
        }
        private void button_home_confirmchange_Click(object sender, EventArgs e)
        {
            if (SetHomeBitmap != null) {
                SetHomeBitmap(bitmapPainthome, this.boolusefilehome, filepathhome);
            }
        }

        private void radioButton_plane_defaultsize_CheckedChanged(object sender, EventArgs e)
        {
            Bitmap bit = new Bitmap(bitmapPaintplane.Width, bitmapPaintplane.Height);
            Graphics gbook = Graphics.FromImage(bit);
            gbook.DrawImage(bitmapPaintplane, new Point(0, 0));
            bitmapPaintplane = new Bitmap(DEFAULT_PLANE_WIDTHHEIGHT, DEFAULT_PLANE_WIDTHHEIGHT);
            Graphics gpaint = Graphics.FromImage(bitmapPaintplane);
            Rectangle rect_src = new Rectangle(0, 0, bit.Width, bit.Height);
            Rectangle rect_dest = new Rectangle(0, 0, bitmapPaintplane.Width, bitmapPaintplane.Height);
            gpaint.DrawImage(bit, rect_dest, rect_src, GraphicsUnit.Pixel);
            this.textBox_plane_iconheight.Text = bitmapPaintplane.Height.ToString();
            this.textBox_plane_iconwidth.Text = bitmapPaintplane.Width.ToString();
            this.tabPage_plane.Invalidate();


        }
        private void radioButton_plane_originicon_CheckedChanged(object sender, EventArgs e)
        {
            if (bitmapFileplane != null)
            {
                bitmapPaintplane = bitmapFileplane;
            }
            else
            {
                bitmapPaintplane = bitmapOriginplane;
            }
            this.textBox_plane_iconheight.Text = bitmapPaintplane.Height.ToString();
            this.textBox_plane_iconwidth.Text = bitmapPaintplane.Width.ToString();
            this.tabPage_plane.Invalidate();
            boolusefileplane = true;
        }
        private void radioButton_plane_defaulticon_CheckedChanged(object sender, EventArgs e)
        {
            if (bitmapOriginplane== null) return;
            bitmapPaintplane = bitmapOriginplane;
            this.textBox_plane_iconheight.Text = bitmapPaintplane.Height.ToString();
            this.textBox_plane_iconwidth.Text = bitmapPaintplane.Width.ToString();
            this.tabPage_plane.Invalidate();
            boolusefileplane = false;
        }
        private void plane_icon_keypress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                try
                {
                    int w = int.Parse(this.textBox_plane_iconwidth.Text);
                    int h = int.Parse(this.textBox_plane_iconheight.Text);

                    Bitmap bit = new Bitmap(bitmapPaintplane.Width, bitmapPaintplane.Height);
                    Graphics gbook = Graphics.FromImage(bit);
                    gbook.DrawImage(bitmapPaintplane, new Point(0, 0));

                    bitmapPaintplane = new Bitmap(w, h);
                    Graphics gpaint = Graphics.FromImage(bitmapPaintplane);
                    Rectangle rect_src = new Rectangle(0, 0, bit.Width, bit.Height);
                    Rectangle rect_dest = new Rectangle(0, 0, bitmapPaintplane.Width, bitmapPaintplane.Height);
                    gpaint.DrawImage(bit, rect_dest, rect_src, GraphicsUnit.Pixel);
                    this.tabPage_plane.Invalidate();
                }
                catch
                {

                }
            }
        }
        private void button_plane_changeicon_Click(object sender, EventArgs e)
        {
            OpenFileDialog loOpenFile = new OpenFileDialog();

            loOpenFile.Filter = "(*.png)|*.png|*.*|(*.jpg)|*.jpg|(*.icon)|*.icon|所有文件(*.*)";// "PDF文件(*.pdf)|*.pdf";
            loOpenFile.Title = "更改PLANE图标";
            loOpenFile.FilterIndex = 1;
            loOpenFile.RestoreDirectory = true;
            if (loOpenFile.ShowDialog() == DialogResult.OK)
            {
                filepathplane = loOpenFile.FileName.ToString(); //获得文件路径 
                bitmapFileplane = new Bitmap(filepathplane);
                this.radioButton_plane_defaultsize.Checked = true;

                Rectangle rect_src = new Rectangle(0, 0, bitmapFileplane.Width, bitmapFileplane.Height);
                Rectangle rect_dest = new Rectangle(0, 0, DEFAULT_PLANE_WIDTHHEIGHT, DEFAULT_PLANE_WIDTHHEIGHT);
                bitmapPaintplane = new Bitmap(DEFAULT_PLANE_WIDTHHEIGHT, DEFAULT_PLANE_WIDTHHEIGHT);
                Graphics g = Graphics.FromImage(bitmapPaintplane);
                g.DrawImage(bitmapFileplane, rect_dest, rect_src, GraphicsUnit.Pixel);

                this.textBox_plane_iconheight.Text = bitmapPaintplane.Height.ToString();
                this.textBox_plane_iconwidth.Text  = bitmapPaintplane.Width.ToString();
                this.tabPage_plane.Invalidate();
                boolusefileplane = true;
            }
        }
        private void button_plane_confirmchange_Click(object sender, EventArgs e)
        {
            if (SetPlaneBitmap != null)
            {
                SetPlaneBitmap(bitmapPaintplane,this.boolusefileplane,this.filepathplane);
            }
        }

        private void radioButton_wpmarker_defaultsize_CheckedChanged(object sender, EventArgs e)
        {
            Bitmap bit = new Bitmap(bitmapPaintwpmarker.Width, bitmapPaintwpmarker.Height);
            Graphics gbook = Graphics.FromImage(bit);
            gbook.DrawImage(bitmapPaintwpmarker, new Point(0, 0));
            bitmapPaintwpmarker = new Bitmap(DEFAULT_WPBOOK_WIDTHHEIGHT, DEFAULT_WPBOOK_WIDTHHEIGHT);
            Graphics gpaint = Graphics.FromImage(bitmapPaintwpmarker);
            Rectangle rect_src = new Rectangle(0, 0, bit.Width, bit.Height);
            Rectangle rect_dest = new Rectangle(0, 0, bitmapPaintwpmarker.Width, bitmapPaintwpmarker.Height);
            gpaint.DrawImage(bit, rect_dest, rect_src, GraphicsUnit.Pixel);
            this.textBox_wpmarker_iconheight.Text = bitmapPaintwpmarker.Height.ToString();
            this.textBox_wpmarker_iconwidth.Text = bitmapPaintwpmarker.Width.ToString();
            this.tabPage_wpmarker.Invalidate();
        }
        private void radioButton_wpmarker_originicon_CheckedChanged(object sender, EventArgs e)
        {
            if (bitmapFilewpmarker != null)
            {
                bitmapPaintwpmarker = bitmapFilewpmarker;
            }
            else
            {
                bitmapPaintwpmarker = bitmapOriginwpmarker;
            }
            this.textBox_wpmarker_iconheight.Text = bitmapPaintwpmarker.Height.ToString();
            this.textBox_wpmarker_iconwidth.Text = bitmapPaintwpmarker.Width.ToString();
            this.tabPage_wpmarker.Invalidate();
            boolusefilewpmarker = true;
        }
        private void radioButton_wpmarker_defaulticon_CheckedChanged(object sender, EventArgs e)
        {
            if (bitmapOriginwpmarker == null) return;
            bitmapPaintwpmarker = bitmapOriginwpmarker;
            this.textBox_wpmarker_iconheight.Text = bitmapPaintwpmarker.Height.ToString();
            this.textBox_wpmarker_iconwidth.Text = bitmapPaintwpmarker.Width.ToString();
            this.tabPage_wpmarker.Invalidate();
            boolusefilewpmarker = false;
        }
        private void wpmarker_icon_keypress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                try
                {
                    int w = int.Parse(this.textBox_wpmarker_iconwidth.Text);
                    int h = int.Parse(this.textBox_wpmarker_iconheight.Text);

                    Bitmap bit = new Bitmap(bitmapPaintwpmarker.Width, bitmapPaintwpmarker.Height);
                    Graphics gbook = Graphics.FromImage(bit);
                    gbook.DrawImage(bitmapPaintwpmarker, new Point(0, 0));

                    bitmapPaintwpmarker = new Bitmap(w, h);
                    Graphics gpaint = Graphics.FromImage(bitmapPaintwpmarker);
                    Rectangle rect_src = new Rectangle(0, 0, bit.Width, bit.Height);
                    Rectangle rect_dest = new Rectangle(0, 0, bitmapPaintwpmarker.Width, bitmapPaintwpmarker.Height);
                    gpaint.DrawImage(bit, rect_dest, rect_src, GraphicsUnit.Pixel);
                    this.tabPage_wpmarker.Invalidate();
                }
                catch
                {

                }
            }
        }
        private void button_wpmarker_changeicon_Click(object sender, EventArgs e)
        {
            OpenFileDialog loOpenFile = new OpenFileDialog();

            loOpenFile.Filter = "(*.png)|*.png|*.*|(*.jpg)|*.jpg|(*.icon)|*.icon|所有文件(*.*)";// "PDF文件(*.pdf)|*.pdf";
            loOpenFile.Title = "更改航点图标";
            loOpenFile.FilterIndex = 1;
            loOpenFile.RestoreDirectory = true;
            if (loOpenFile.ShowDialog() == DialogResult.OK)
            {
                filepathwpmarker = loOpenFile.FileName.ToString(); //获得文件路径 
                bitmapFilewpmarker = new Bitmap(filepathwpmarker);
                this.radioButton_wpmarker_defaultsize.Checked = true;

                Rectangle rect_src = new Rectangle(0, 0, bitmapFilewpmarker.Width, bitmapFilewpmarker.Height);
                Rectangle rect_dest = new Rectangle(0, 0, DEFAULT_WPBOOK_WIDTHHEIGHT, DEFAULT_WPBOOK_WIDTHHEIGHT);
                bitmapPaintwpmarker = new Bitmap(DEFAULT_WPBOOK_WIDTHHEIGHT, DEFAULT_WPBOOK_WIDTHHEIGHT);
                Graphics g = Graphics.FromImage(bitmapPaintwpmarker);
                g.DrawImage(bitmapFilewpmarker, rect_dest, rect_src, GraphicsUnit.Pixel);

                this.textBox_wpmarker_iconheight.Text = bitmapPaintwpmarker.Height.ToString();
                this.textBox_wpmarker_iconwidth.Text = bitmapPaintwpmarker.Width.ToString();
                this.tabPage_wpmarker.Invalidate();
                boolusefilewpmarker = true;
            }
        }
        private void button_wpmarker_confirmchange_Click(object sender, EventArgs e)
        {
            if (SetWpBitmap != null)
            {
                SetWpBitmap(bitmapPaintwpmarker,this.boolusefilewpmarker,this.filepathwpmarker);
            }
        }





    }
}
