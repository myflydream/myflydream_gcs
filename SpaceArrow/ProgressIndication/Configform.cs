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
    public partial class Configform : Form
    {
        Color colorTrack            = Color.Red;
        Color colorRoutes           = Color.Red;
        Color colorTargetLocation    = Color.Red;
        Color colorMouseLocation    = Color.Red;
        Color colorZoom             = Color.Red;
        Color colorScale            = Color.Red;

        public delegate void SetColorDele(Color color);
        public SetColorDele SetColorTrack           = null;
        public SetColorDele SetColorRoutes          = null;
        public SetColorDele SetColorTargetLocation  = null;
        public SetColorDele SetColorMouseLocation   = null;
        public SetColorDele SetColorZoom            = null;
        public SetColorDele SetColorScale           = null;

        public Configform()
        {
            InitializeComponent();

            this.Text = "GCS-颜色管理器";

            this.StartPosition = FormStartPosition.CenterScreen;
            this.ShowInTaskbar = false;

            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            this.Icon = global::SpaceArrow.Properties.Resources.mpdesktop;
            this.label_colortrack.BackColor = colorTrack;
            this.textBox_colortrack_red.Text = colorTrack.R.ToString();
            this.textBox_colortrack_green.Text = colorTrack.G.ToString();
            this.textBox_colortrack_blue.Text = colorTrack.B.ToString();

            this.label_colorroutes.BackColor = colorRoutes;
            this.textBox_colorroutes_red.Text = colorRoutes.R.ToString();
            this.textBox_colorroutes_green.Text = colorRoutes.G.ToString();
            this.textBox_colorroutes_blue.Text = colorRoutes.B.ToString();

            this.label_colormouselocation.BackColor = colorMouseLocation;
            this.textBox_colormouselocation_red.Text = colorMouseLocation.R.ToString();
            this.textBox_colormouselocation_green.Text = colorMouseLocation.G.ToString();
            this.textBox_colormouselocation_blue.Text = colorMouseLocation.B.ToString();

            this.label_colorscale.BackColor = colorScale;
            this.textBox_colorscale_red.Text = colorScale.R.ToString();
            this.textBox_colorscale_green.Text = colorScale.G.ToString();
            this.textBox_colorscale_blue.Text = colorScale.B.ToString();

            this.label_colortargetlocation.BackColor = colorTargetLocation;
            this.textBox_colortargetlocation_red.Text = colorTargetLocation.R.ToString();
            this.textBox_colortargetlocation_green.Text = colorTargetLocation.G.ToString();
            this.textBox_colortargetlocation_blue.Text = colorTargetLocation.B.ToString();

            this.label_colorzoom.BackColor = colorZoom;
            this.textBox_colorzoom_red.Text = colorZoom.R.ToString();
            this.textBox_colorzoom_green.Text = colorZoom.G.ToString();
            this.textBox_colorzoom_blue.Text = colorZoom.B.ToString();

        }

        private void button_colortrack_Click(object sender, EventArgs e)
        {
             ColorDialog colordialog = new ColorDialog();
             if (colordialog.ShowDialog() == DialogResult.OK)
             {
                 colorTrack = colordialog.Color;
                 this.textBox_colortrack_red.Text   = colorTrack.R.ToString();
                 this.textBox_colortrack_green.Text = colorTrack.G.ToString();
                 this.textBox_colortrack_blue.Text  = colorTrack.B.ToString();
                 this.label_colortrack.BackColor    = colorTrack;
                 if (SetColorTrack!=null)
                 SetColorTrack(colorTrack);
             }
        }
        private void button_colorroutes_Click(object sender, EventArgs e)
        {
            ColorDialog colordialog = new ColorDialog();
            if (colordialog.ShowDialog() == DialogResult.OK)
            {
                colorRoutes = colordialog.Color;
                this.textBox_colorroutes_red.Text = colorRoutes.R.ToString();
                this.textBox_colorroutes_green.Text = colorRoutes.G.ToString();
                this.textBox_colorroutes_blue.Text = colorRoutes.B.ToString();
                this.label_colorroutes.BackColor = colorRoutes;
                if (SetColorRoutes != null)
                SetColorRoutes(colorRoutes);
            }
        }
        private void button_colortargetlocation_Click(object sender, EventArgs e)
        {
            ColorDialog colordialog = new ColorDialog();
            if (colordialog.ShowDialog() == DialogResult.OK)
            {
                colorTargetLocation = colordialog.Color;
                this.textBox_colortargetlocation_red.Text = colorTargetLocation.R.ToString();
                this.textBox_colortargetlocation_green.Text = colorTargetLocation.G.ToString();
                this.textBox_colortargetlocation_blue.Text = colorTargetLocation.B.ToString();
                this.label_colortargetlocation.BackColor = colorTargetLocation;
                if (SetColorTargetLocation != null)
                SetColorTargetLocation(colorTargetLocation);
            }
        }
        private void button_colormouselocation_Click(object sender, EventArgs e)
        {
            ColorDialog colordialog = new ColorDialog();
            if (colordialog.ShowDialog() == DialogResult.OK)
            {
                colorMouseLocation = colordialog.Color;
                this.textBox_colormouselocation_red.Text = colorMouseLocation.R.ToString();
                this.textBox_colormouselocation_green.Text = colorMouseLocation.G.ToString();
                this.textBox_colormouselocation_blue.Text = colorMouseLocation.B.ToString();
                this.label_colormouselocation.BackColor = colorMouseLocation;
                if (SetColorMouseLocation != null)
                SetColorMouseLocation(colorMouseLocation);
            }
        }
        private void button_colorzoom_Click(object sender, EventArgs e)
        {
            ColorDialog colordialog = new ColorDialog();
            if (colordialog.ShowDialog() == DialogResult.OK)
            {
                colorZoom = colordialog.Color;
                this.textBox_colorzoom_red.Text = colorZoom.R.ToString();
                this.textBox_colorzoom_green.Text = colorZoom.G.ToString();
                this.textBox_colorzoom_blue.Text = colorZoom.B.ToString();
                this.label_colorzoom.BackColor = colorZoom;
                if (SetColorZoom != null)
                SetColorZoom(colorZoom);
            }
        }
        private void button_colorscale_Click(object sender, EventArgs e)
        {
            ColorDialog colordialog = new ColorDialog();
            if (colordialog.ShowDialog() == DialogResult.OK)
            {
                colorScale = colordialog.Color;
                this.textBox_colorscale_red.Text = colorScale.R.ToString();
                this.textBox_colorscale_green.Text = colorScale.G.ToString();
                this.textBox_colorscale_blue.Text = colorScale.B.ToString();
                this.label_colorscale.BackColor = colorScale;
                if (SetColorScale != null)
                SetColorScale(colorScale);
            }
        }
        private void textBox_colortrack_keypress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r') {
                try
                {
                    int r = int.Parse(this.textBox_colortrack_red.Text);
                    int g = int.Parse(this.textBox_colortrack_green.Text);
                    int b = int.Parse(this.textBox_colortrack_blue.Text);
                    colorTrack = Color.FromArgb(255, r, g, b);
                    this.label_colortrack.BackColor = colorTrack;
                    if (SetColorTrack != null)
                    SetColorTrack(colorTrack);
                }
                catch
                {
                    MessageBox.Show("您输入的RGB变量有错!!!");
                }
            }
    

        }
        private void textBox_colorroutes_keypress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                try
                {
                    int r = int.Parse(this.textBox_colorroutes_red.Text);
                    int g = int.Parse(this.textBox_colorroutes_green.Text);
                    int b = int.Parse(this.textBox_colorroutes_blue.Text);
                    colorRoutes = Color.FromArgb(255, r, g, b);
                    this.label_colorroutes.BackColor = colorRoutes;
                    if (SetColorRoutes != null)
                    SetColorRoutes(colorRoutes);
                }
                catch
                {
                    MessageBox.Show("您输入的RGB变量有错!!!");
                }
            }
        }
        private void textBox_colortargetlocation_keypress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                try
                {
                    int r = int.Parse(this.textBox_colortargetlocation_red.Text);
                    int g = int.Parse(this.textBox_colortargetlocation_green.Text);
                    int b = int.Parse(this.textBox_colortargetlocation_blue.Text);
                    colorTargetLocation = Color.FromArgb(255, r, g, b);
                    this.label_colortargetlocation.BackColor = colorTargetLocation;
                    if (SetColorTargetLocation != null)
                    SetColorTargetLocation(colorTargetLocation);
                }
                catch
                {
                    MessageBox.Show("您输入的RGB变量有错!!!");
                }
            }
        }
        private void textBox_colormouselocation_keypress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                try
                {
                    int r = int.Parse(this.textBox_colormouselocation_red.Text);
                    int g = int.Parse(this.textBox_colormouselocation_green.Text);
                    int b = int.Parse(this.textBox_colormouselocation_blue.Text);
                    colorMouseLocation = Color.FromArgb(255, r, g, b);
                    this.label_colormouselocation.BackColor = colorMouseLocation;
                    if (SetColorMouseLocation != null)
                    SetColorMouseLocation(colorMouseLocation);
                }
                catch
                {
                    MessageBox.Show("您输入的RGB变量有错!!!");
                }
            }
        }
        private void textBox_colorzoom_keypress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                try
                {
                    int r = int.Parse(this.textBox_colorzoom_red.Text);
                    int g = int.Parse(this.textBox_colorzoom_green.Text);
                    int b = int.Parse(this.textBox_colorzoom_blue.Text);
                    colorZoom = Color.FromArgb(255, r, g, b);
                    this.label_colorzoom.BackColor = colorZoom;
                    if (SetColorZoom != null)
                    SetColorZoom(colorZoom);
                }
                catch
                {
                    MessageBox.Show("您输入的RGB变量有错!!!");
                }
            }
        }
        private void textBox_colorscale_keypress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                try
                {
                    int r = int.Parse(this.textBox_colorscale_red.Text);
                    int g = int.Parse(this.textBox_colorscale_green.Text);
                    int b = int.Parse(this.textBox_colorscale_blue.Text);
                    colorScale = Color.FromArgb(255, r, g, b);
                    this.label_colorscale.BackColor = colorScale;
                    if (SetColorScale != null)
                    SetColorScale(colorScale);
                }
                catch
                {
                    MessageBox.Show("您输入的RGB变量有错!!!");
                }
            }
        }

        public void SetParaColorTrack(Color color) {
            colorTrack = color;
            this.label_colortrack.BackColor = colorTrack;
            this.textBox_colortrack_red.Text = colorTrack.R.ToString();
            this.textBox_colortrack_green.Text = colorTrack.G.ToString();
            this.textBox_colortrack_blue.Text = colorTrack.B.ToString();
        }
        public void SetParaColorRoutes(Color color)
        {
            colorRoutes = color;
            this.label_colorroutes.BackColor = colorRoutes;
            this.textBox_colorroutes_red.Text = colorRoutes.R.ToString();
            this.textBox_colorroutes_green.Text = colorRoutes.G.ToString();
            this.textBox_colorroutes_blue.Text = colorRoutes.B.ToString();
        }
        public void SetParaColorTargetLocation(Color color)
        {
            colorTargetLocation = color;
            this.label_colortargetlocation.BackColor    = colorTargetLocation;
            this.textBox_colortargetlocation_red.Text   = colorTargetLocation.R.ToString();
            this.textBox_colortargetlocation_green.Text = colorTargetLocation.G.ToString();
            this.textBox_colortargetlocation_blue.Text  = colorTargetLocation.B.ToString();
        }
        public void SetParaColorMouseLocation(Color color)
        {
            colorMouseLocation = color;
            this.label_colormouselocation.BackColor =       colorMouseLocation;
            this.textBox_colormouselocation_red.Text =      colorMouseLocation.R.ToString();
            this.textBox_colormouselocation_green.Text =    colorMouseLocation.G.ToString();
            this.textBox_colormouselocation_blue.Text =     colorMouseLocation.B.ToString();
        }
        public void SetParaColorZoom(Color color)
        {
            colorZoom = color;
            this.label_colorzoom.BackColor = colorZoom;
            this.textBox_colorzoom_red.Text = colorZoom.R.ToString();
            this.textBox_colorzoom_green.Text = colorZoom.G.ToString();
            this.textBox_colorzoom_blue.Text = colorZoom.B.ToString();
        }
        public void SetParaColorScale(Color color)
        {
            colorScale = color;
            this.label_colorscale.BackColor = colorScale;
            this.textBox_colorscale_red.Text = colorScale.R.ToString();
            this.textBox_colorscale_green.Text = colorScale.G.ToString();
            this.textBox_colorscale_blue.Text = colorScale.B.ToString();
        }
    
    }
}
