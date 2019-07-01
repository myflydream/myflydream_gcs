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
    public partial class NorWin : UserControl
    {
        int Control_width = 30;
        int Control_height = 27;
        Color Color_back;
        Pen p = new Pen(Color.DarkGray, 2);
        int edge = 5;
        int offset = 3;

        public NorWin()
        {
            InitializeComponent();
            Color_back = this.BackColor;
        }

        private void NorWin_Load(object sender, EventArgs e)
        {
            this.Width = this.Control_width;
            this.Height = this.Control_height;
        }

        private void NorWin_Enter(object sender, EventArgs e)
        {
            this.BackColor = Color.RoyalBlue;
            p.Color = Color.White;
        }

        private void NorWin_Leave(object sender, EventArgs e)
        {
            this.BackColor = Color_back;
            p.Color = Color.DarkGray;
        }

        private void NorWin_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            SolidBrush b=new SolidBrush(this.BackColor);
            g.DrawRectangle(p, edge + offset, edge + offset, this.Control_width - 3 * edge - 2*offset, this.Control_height - 3 * edge - 2*offset);
            g.DrawRectangle(p, edge * 2 + offset, edge * 2 + offset, this.Control_width - 3 * edge - 2*offset, this.Control_height - 3 * edge - 2*offset);
            g.FillRectangle(b, edge * 2 + offset+1, edge * 2 + offset+1, this.Control_width - 3 * edge - 2 * offset-2, this.Control_height - 3 * edge - 2 * offset-2);


        }



    }
}
