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
    public partial class InsertWPNum : Form
    {
        Bitmap bit = new Bitmap(global::SpaceArrow.Properties.Resources.logo);
        string title = "Insert way point...";
        Graphics g;

        public int count = 5;

        public delegate void InsertWPNumDone(int num);
        public InsertWPNumDone GetNum = null;
        public InsertWPNum()
        {
            InitializeComponent();
            this.ShowInTaskbar = false;

            this.Text = title;
            this.Icon = Icon.FromHandle(global::SpaceArrow.Properties.Resources.logo.GetHicon());

    
            this.StartPosition = FormStartPosition.CenterScreen;
            //this.FormBorderStyle = FormBorderStyle.None;
            g = this.CreateGraphics();

        }

        public int flag = 0;



        private void button1_Click(object sender, EventArgs e)
        {
            string str = this.textBox_wp.Text;
            int num = 0;
            try
            {
                num = int.Parse(str);
            }
            catch {
                MessageBox.Show("输入非法...请重新输入...");
                return;
            }
            if (flag == 0)
            {
                if (num > count || num < 1)
                {
                    MessageBox.Show("航点编号必须小于当前航点总数(非零)...");
                    return;
                }
            }
            else { 
            
            }
            if (GetNum!=null)
                GetNum(num);
            this.Close();
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

        private void Form_Paint(object sender, PaintEventArgs e)
        {
            //if (!Program.FORM_BORDER_NONE) return;
            //Rectangle rect = new Rectangle(0, 0, bit.Width, bit.Height);
            //Rectangle disrect = new Rectangle(0, 0, 40, 30);
            //g.DrawImage(bit, disrect, rect, System.Drawing.GraphicsUnit.Pixel);

            //SolidBrush b = new SolidBrush(Color.Black);
            //Font font = new Font("宋体", 10);
            //g.DrawString(this.title, font, b, new PointF(45, 7));
        }

        private void dmButtonClose1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
