using System;
using System.Drawing;
using System.Windows.Forms;

namespace SpaceArrow
{
    public partial class PlayBackForm : Form
    {
        Bitmap bit = new Bitmap(global::SpaceArrow.Properties.Resources.logo);
        string title = "Play back the flight data...";
        Graphics g;

        public DateTime StatrtTime = DateTime.Now;
        public DateTime EndTime = DateTime.Now.AddSeconds(5);

        public delegate string GetFilenameDele();
        public GetFilenameDele getFilename = null;

        public delegate void playstatusdele(int status);
        public playstatusdele playstatus;

        bool isChoiceData = false;
        public PlayBackForm()
        {
            InitializeComponent();

            this.ShowInTaskbar = false;

            this.Text = title;
           // this.Icon = new Icon("mpdesktop.ico");
            this.Icon = Icon.FromHandle(global::SpaceArrow.Properties.Resources.logo.GetHicon());
            this.StartPosition = FormStartPosition.CenterScreen;
           // this.FormBorderStyle = FormBorderStyle.None;
            g = this.CreateGraphics();
            this.Paint += PlayBackForm_Paint;

            this.label1.Text = StatrtTime.ToString("HH:mm:ss");
            this.label2.Text = EndTime.ToString("HH:mm:ss");

            this.dmProgressBar1.MouseDown += dmProgressBar1_MouseDown;
            this.dmProgressBar1.MouseMove += dmProgressBar1_MouseMove;
            this.dmProgressBar1.MouseUp += dmProgressBar1_MouseUp;
            this.dmProgressBar1.MouseLeave += dmProgressBar1_MouseLeave;
        }

        void dmProgressBar1_MouseLeave(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            isMousedown = false;
        }

        void dmProgressBar1_MouseUp(object sender, MouseEventArgs e)
        {
            //throw new NotImplementedException();

            if (!isChoiceData) return;

            if (isMousedown)
            {
                double secondsall = ((EndTime - StatrtTime)).TotalSeconds;
                double sec = secondsall * this.dmProgressBar1.DM_Value / 100;
                this.label1.Text = this.StatrtTime.AddSeconds(sec).ToString("HH:mm:ss");
                if (SetPlayBackProcess != null)
                {
                    SetPlayBackProcess(sec);
                }
            }
                isMousedown = false;
        }
        bool isMousedown;
        public delegate void SetPlayBackProcessDele(double Second);
       public  SetPlayBackProcessDele SetPlayBackProcess = null;
        void dmProgressBar1_MouseMove(object sender, MouseEventArgs e)
        {
            //throw new NotImplementedException();
            if (!isChoiceData) return;

            if (isMousedown) { 
                double secondsall = ((EndTime - StatrtTime)).TotalSeconds;
                double sec = secondsall * this.dmProgressBar1.DM_Value/100;
                this.label1.Text = this.StatrtTime.AddSeconds(sec).ToString("HH:mm:ss");
                if (SetPlayBackProcess != null) {
                    SetPlayBackProcess(sec);
                }
            }
        }

        void dmProgressBar1_MouseDown(object sender, MouseEventArgs e)
        {
            isMousedown = true;
        }
        void PlayBackForm_Paint(object sender, PaintEventArgs e)
        {
            //if (!Program.FORM_BORDER_NONE) return;
            //Rectangle rect = new Rectangle(0, 0, bit.Width, bit.Height);
            //Rectangle disrect = new Rectangle(0, 0, 40, 30);
            //g.DrawImage(bit, disrect, rect, System.Drawing.GraphicsUnit.Pixel);

            //SolidBrush b = new SolidBrush(Color.Black);
            //Font font = new Font("宋体", 10);
            //g.DrawString(this.title, font, b, new PointF(45, 7));
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

        private void dmButtonClose1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public bool isPlayPause= false;

        private void button_playpause_Click(object sender, EventArgs e)
        {
            switch (this.button_playpause.Text) { 
                case "播放":
                    button_playpause.Text = "暂停";
                    this.label_playstatus.Text = "正在播放";
                    break;
                case "暂停":
                    button_playpause.Text = "播放";
                    this.label_playstatus.Text = "暂停";
                    break;
            }
            isPlayPause = !isPlayPause;

            if (playstatus != null) {
                playstatus(1);
            }

        }

        private void button_ChoiceDataFile_Click(object sender, EventArgs e)
        {
            if (getFilename != null) { 
                string str = getFilename();
                isChoiceData = true;

                if (str == null)
                {

                    this.button_ChoiceDataFile.Text = "选择数据文件";
                }
                else { 
                    this.button_ChoiceDataFile.Text = "选择数据文件(" + str + ")";
                //    this.label1.Text = StatrtTime.ToString("HH:mm:ss");
                //    this.label2.Text = EndTime.ToString("HH:mm:ss");

                    this.label1.Text = "0";
                    this.label2.Text = (EndTime - StatrtTime).ToString(@"hh\:mm\:ss");

                    this.label_playstatus.Text = "正在播放";

                    isPlayPause = true;
                    button_playpause.Text = "暂停";
                    this.label_playstatus.Text = "正在播放";
                }

            }

        }
        public delegate  void labeldele(string str);
        void labelShow(string str) {
            if (this.label1.InvokeRequired) {
                try
                {
                    this.BeginInvoke(new labeldele(labelShow), new Object[1] { str });
                }
                catch (ObjectDisposedException) {
                    return;
                }
            }else{
                this.label1.Text=str;
            }
        }
        public double processBarValue = 0;
        public void SetProcessBar(DateTime star,DateTime Now) {
            double secondsnow = ((Now - star)).TotalSeconds;
            double secondsall = ((EndTime - StatrtTime)).TotalSeconds;

            if (secondsnow > secondsall)
            {
                this.dmProgressBar1.DM_Value = 100;
            }
            else {
                this.dmProgressBar1.DM_Value =secondsnow/secondsall* 100;
                labelShow((StatrtTime.AddSeconds(secondsnow) - StatrtTime).ToString(@"hh\:mm\:ss"));
            }
            processBarValue = this.dmProgressBar1.DM_Value;

        }

        private void button_Retreat_Click(object sender, EventArgs e)
        {
            if (playstatus != null)
            {
                playstatus(3);
            }
        }

        private void button_Forward_Click(object sender, EventArgs e)
        {
            if (playstatus != null)
            {
                playstatus(2);
            }
        }
    }
}
