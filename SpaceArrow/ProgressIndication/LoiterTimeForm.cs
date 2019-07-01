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
    public partial class LoiterTimeForm : Form
    {
        Bitmap bit = new Bitmap(global::SpaceArrow.Properties.Resources.logo);  //引用窗体logo
        string title = "Hovering Mode";               //定义窗体标题
        Graphics g;    
        public delegate void GetDataDele(MAVLink.mavlink_command_long_t dd);
        public GetDataDele GetData = null;

        public LoiterTimeForm()
        {
            InitializeComponent();


            this.ShowInTaskbar = false;

            this.Text = title;
            this.Icon = Icon.FromHandle(global::SpaceArrow.Properties.Resources.logo.GetHicon());


            this.StartPosition = FormStartPosition.CenterScreen;
           // this.FormBorderStyle = FormBorderStyle.None;
            g = this.CreateGraphics();

            this.Paint += LoiterTimeForm_Paint;

            this.comboBox_missionType.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBox_missionType.Items.Add(0);
            this.comboBox_missionType.Items.Add(1);
            this.comboBox_missionType.Items.Add(2);
            this.comboBox_missionType.Items.Add(3);
            comboBox_missionType.SelectedIndex = 0;
            EnableControl(0);

            this.textBox_missionalt.Text = "100";
            this.textBox_missionlat.Text = "23";
            this.textBox_missionlng.Text= "113";
            this.textBox_missionSeconds.Text = "10";
            this.textBox_missionyaw.Text = "0";
            this.textBox_missonRadius.Text = "10";
        }

        void LoiterTimeForm_Paint(object sender, PaintEventArgs e)
        {
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
        public void EnableControl(int type) {
            switch (type) {
                case 0:
                    this.textBox_missionalt.Enabled = false;
                    this.textBox_missionlat.Enabled = false;
                    this.textBox_missionlng.Enabled = false;
                    this.textBox_missionSeconds.Enabled = false;
                    this.textBox_missionyaw.Enabled = false;
                    this.textBox_missonRadius.Enabled = false;
                    break;
                case 1: 
                    this.textBox_missionalt.Enabled = true;
                    this.textBox_missionlat.Enabled = true;
                    this.textBox_missionlng.Enabled = true;
                    this.textBox_missionSeconds.Enabled = true;
                    this.textBox_missionyaw.Enabled = true;
                    this.textBox_missonRadius.Enabled = true;
                    break;
                case 2: 
                    this.textBox_missionalt.Enabled = false;
                    this.textBox_missionlat.Enabled = false;
                    this.textBox_missionlng.Enabled = false;
                    this.textBox_missionSeconds.Enabled = false;
                    this.textBox_missionyaw.Enabled = true;
                    this.textBox_missonRadius.Enabled = false;
                    break;
                case 3:
                    this.textBox_missionalt.Enabled = true;
                    this.textBox_missionlat.Enabled = false;
                    this.textBox_missionlng.Enabled = false;
                    this.textBox_missionSeconds.Enabled = false;
                    this.textBox_missionyaw.Enabled = false;
                    this.textBox_missonRadius.Enabled = false;
                    break;
                default: break;
            }
        }
        private void comboBox_missionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            int type = int.Parse(this.comboBox_missionType.Text);
            EnableControl(type);
        }

        private void button_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void button_OK_Click(object sender, EventArgs e)
        {

            MAVLink.mavlink_command_long_t dd = new MAVLink.mavlink_command_long_t();

            try
            {
                dd.param1 = float.Parse(this.textBox_missionSeconds.Text);
                dd.param2 = float.Parse(this.comboBox_missionType.Text);
                dd.param3 = float.Parse(this.textBox_missonRadius.Text);
                dd.param4 = float.Parse(this.textBox_missionyaw.Text);
                dd.param5 = float.Parse(this.textBox_missionlat.Text);
                dd.param6 = float.Parse(this.textBox_missionlng.Text);
                dd.param7 = float.Parse(this.textBox_missionalt.Text);

                if (GetData != null)
                {
                    GetData(dd);
                }

            }
            catch {
                MessageBox.Show("数据出错...");
                return;
            }







            this.Close();
        }





    }
}
