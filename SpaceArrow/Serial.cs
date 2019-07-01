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
    public partial class Serial_Form : Form
    {
        Bitmap bit = new Bitmap(global::SpaceArrow.Properties.Resources.plane);
        string title = "Serial port settings";
        Graphics g;

        private int[] baud_array ={ 1200,              //串口可选择的波特率选项
                                    4800,       
                                    9600,
                                    14400,
                                    19200,
                                    28800,
                                    38400,
                                    57600,
                                    115200,
                                    128000,
                                    256000,
                                  };

        public int dataBits = 0;
        public int stopBits = 0;
        public int baud=38400;
        public int verify = 0;
        public string com ;

        public delegate void setPort();
        public setPort PortPara = null;

        public Serial_Form()
        {
            InitializeComponent();

            this.Text = title;
            //this.Icon = new Icon("mpdesktop.ico");
            this.Icon = Icon.FromHandle(global::SpaceArrow.Properties.Resources.logo.GetHicon());
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ShowInTaskbar = false;

            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

           // this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            g = this.CreateGraphics();

            this.comboBox_databits.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBox4_stopbit.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBox5_verify.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBox_com.DropDownStyle = ComboBoxStyle.DropDownList;

            //数据位
            this.comboBox_databits.Items.Add(8);
            this.comboBox_databits.Items.Add(7);
            this.comboBox_databits.Items.Add(6);
            this.comboBox_databits.SelectedIndex = 0;

            //停止位
            this.comboBox4_stopbit.Items.Add(1);
            this.comboBox4_stopbit.Items.Add(2);
            this.comboBox4_stopbit.SelectedIndex = 0;

            //校验位
            this.comboBox5_verify.Items.Add("无");
            this.comboBox5_verify.Items.Add("奇");
            this.comboBox5_verify.Items.Add("偶");
            this.comboBox5_verify.Items.Add("M");
            this.comboBox5_verify.Items.Add("S");
            this.comboBox5_verify.SelectedIndex = 0;

            for (int i = 0; i < baud_array.Length; i++)
            {
                this.comboBox_baud.Items.Add(this.baud_array[i].ToString());
            }
            this.comboBox_baud.Text = "115200";
            ScanAddCom();


            this.comboBox_databits.Enabled = false;
            this.comboBox4_stopbit.Enabled = false;
            this.comboBox5_verify.Enabled = false;

        }
        private void ScanAddCom() {
            this.comboBox_com.Items.Clear();
            foreach (string str in System.IO.Ports.SerialPort.GetPortNames())
                this.comboBox_com.Items.Add(str);
            if (this.comboBox_com.Items.Count > 0)
                this.comboBox_com.SelectedIndex = 0;
            else {
               // MessageBox.Show("当前不存在可用串口...");
            }
        }

        public void SetControlEnable(bool enable) {
            this.comboBox_com.Enabled = enable;
            this.comboBox_baud.Enabled = enable;
          //  this.comboBox_databits.Enabled = enable;
          //  this.comboBox4_stopbit.Enabled = enable;
          //  this.comboBox5_verify.Enabled = enable;
            this.button1.Enabled = enable;
            this.button2.Enabled = enable;
            this.button3.Enabled = enable;

        }

        private void dmButtonClose1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void dmButtonMin1_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        private void MaxWin_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
        }
        private void NorWin_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
        }
        private void From_Resize(object sender, EventArgs e)
        {
            int From_width = this.Width;
            int From_height = this.Height;
        }
        private void From_Paint(object sender, PaintEventArgs e)
        {

        }
        private void button1_Click(object sender, EventArgs e)
        {
            ScanAddCom();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            this.comboBox_databits.SelectedIndex = 0;
            this.comboBox4_stopbit.SelectedIndex = 0;
            this.comboBox5_verify.SelectedIndex = 0;
           // this.comboBox_baud.Text = "38400";
        }
        private void button2_Click(object sender, EventArgs e)
        {
            this.com = this.comboBox_com.Text;

            try
            {
                this.baud = int.Parse(this.comboBox_baud.Text);
            }
            catch {
                MessageBox.Show("波特率输入错误...");
                return;
            }

           // this.dataBits = int.Parse(this.comboBox_databits.Text);
           // this.stopBits = this.comboBox4_stopbit.SelectedIndex;
            //this.verify = this.comboBox5_verify.SelectedIndex;
            if (PortPara != null)
            {
                PortPara();
            }
         
                this.Close();
       
        }
        public void SetCom(string str) {
            if (str == null) {
              //  MessageBox.Show("串口号为空...");
                return;
            }

            if (this.comboBox_com.Items.Contains(str)) {
                this.comboBox_com.Text = str;
            }
        }

        /*
        private const int WM_NCHITTEST = 0x84;
        private const int HTCLIENT = 0x1;
        private const int HTCAPTION = 0x2;
        //首先必须了解Windows的消息传递机制，当有鼠标活动消息时， 
        //系统发送WM_NCHITTEST 消息给窗体作为判断消息发生地的根据。 nchittest
        //假如你点击的是标题栏，窗体收到的消息值就是 HTCAPTION ， 
        //同样地，若接受到的消息是 HTCLIENT，说明用户点击的是客户区，也就是鼠标消息发生在客户区。 

        //重写窗体，使窗体可以不通过自带标题栏实现移动
        protected override void WndProc(ref Message m)
        {
            //当重载窗体的 WndProc 方法时，可以截获 WM_NCHITTEST 消息并改些该消息， 
            //当判断鼠标事件发生在客户区时，改写改消息，发送 HTCAPTION 给窗体， 
            //这样，窗体收到的消息就时 HTCAPTION ，在客户区通过鼠标来拖动窗体就如同通过标题栏来拖动一样。 
            //注意：当你重载 WndProc 并改写鼠标事件后，整个窗体的鼠标事件也就随之改变了。 
            switch (m.Msg)
            {
                case WM_NCHITTEST:
                    base.WndProc(ref m);
                    if ((int)m.Result == HTCLIENT)
                        m.Result = (IntPtr)HTCAPTION;
                    return;
            }
            base.WndProc(ref m);
        }

        */

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

    }
}
