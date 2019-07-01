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
    public partial class MaxWin : UserControl
    {
        int Control_width = 30;
        int Control_height = 27;
        int Control_edge = 10;
        Pen p = new Pen(Color.DarkGray, 2);
        Color Color_back;

        public MaxWin()
        {
            this.Width = Control_width;
            this.Height = Control_height;
            Color_back = this.BackColor;
            InitializeComponent();
        }

    

        private void MaxWin_Paint(object sender, PaintEventArgs e)
        {
            Graphics g=e.Graphics;
            g.DrawRectangle(p, Control_edge, Control_edge, Control_width - 2 * Control_edge, Control_height - 2 * Control_edge);
        }

        private void MaxWin_Load(object sender, EventArgs e)
        {
            this.Width = Control_width;
            this.Height = Control_height;
        }
        private void MaxWin_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void MaxWin_MouseEnter(object sender, EventArgs e)
        {
           // isMouseIn = true;
            this.BackColor = Color.RoyalBlue;
            p.Color = Color.White;
            
        }
        private void MaxWin_MouseLeave(object sender, EventArgs e)
        {
         //   isMouseIn = false;
            this.BackColor = Color_back;
            p.Color = Color.DarkGray;
        }



    }
}
