using System;
using System.Drawing;
using System.Windows.Forms;

using JoyKeys.DirectInputJoy;
namespace SpaceArrow
{
    public partial class JoyForm : Form
    {
        public JoyForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 主动式手柄对象
        /// </summary>
        Joystick_V _joystick_V = null;
        /// <summary>
        /// 被动式手柄对象
        /// </summary>
        Joystick_P _joystick_P = null;

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
         //   JoystickInit();
        }

        protected override void OnClosed(EventArgs e)
        {
          //  JoystickDispose();
            base.OnClosed(e);
        }

        /// <summary>
        /// 初始化Joystick
        /// </summary>
        private void JoystickInit()
        {
            _joystick_P = new Joystick_P();
            _joystick_P.Move += new EventHandler<JoystickEventArgs>(_joystick_P_Click);
            
            _joystick_P.Register(this.Handle, API.JOYSTICKID1);
            _joystick_V = Joystick_V.ReturnJoystick(API.JOYSTICKID1);
            _joystick_V.Capture();
        }
        /// <summary>
        /// 卸载Joystick
        /// </summary>
        private void JoystickDispose()
        {
            _joystick_P.Move -= new EventHandler<JoystickEventArgs>(_joystick_P_Click);
            _joystick_P.UnRegister(API.JOYSTICKID1);
            _joystick_V.ReleaseCapture();
            _joystick_V.Dispose();
        }

        void _joystick_P_Click(object sender, JoystickEventArgs e)
        {
                int x = 0, y = 0;
                if ((e.Buttons & JoystickButtons.UP) == JoystickButtons.UP) y--;
                if ((e.Buttons & JoystickButtons.Down) == JoystickButtons.Down) y++;
                if ((e.Buttons & JoystickButtons.Left) == JoystickButtons.Left) x--;
                if ((e.Buttons & JoystickButtons.Right) == JoystickButtons.Right) x++;

                if (x == -1 && y == -1) this.lbl_Dirt.TextAlign = ContentAlignment.TopLeft;
                if (x == 0 && y == -1) this.lbl_Dirt.TextAlign = ContentAlignment.TopCenter;
                if (x == 1 && y == -1) this.lbl_Dirt.TextAlign = ContentAlignment.TopRight;

                if (x == -1 && y == 0) this.lbl_Dirt.TextAlign = ContentAlignment.MiddleLeft;
                if (x == 0 && y == 0) this.lbl_Dirt.TextAlign = ContentAlignment.MiddleCenter;
                if (x == 1 && y == 0) this.lbl_Dirt.TextAlign = ContentAlignment.MiddleRight;

                if (x == -1 && y == 1) this.lbl_Dirt.TextAlign = ContentAlignment.BottomLeft;
                if (x == 0 && y == 1) this.lbl_Dirt.TextAlign = ContentAlignment.BottomCenter;
                if (x == 1 && y == 1) this.lbl_Dirt.TextAlign = ContentAlignment.BottomRight;

                this.lbl_1.BackColor = ((e.Buttons & JoystickButtons.B1) == JoystickButtons.B1) ? Color.Red : SystemColors.Control;
                this.lbl_2.BackColor = ((e.Buttons & JoystickButtons.B2) == JoystickButtons.B2) ? Color.Red : SystemColors.Control;
                this.lbl_3.BackColor = ((e.Buttons & JoystickButtons.B3) == JoystickButtons.B3) ? Color.Red : SystemColors.Control;
                this.lbl_4.BackColor = ((e.Buttons & JoystickButtons.B4) == JoystickButtons.B4) ? Color.Red : SystemColors.Control;
                this.lbl_5.BackColor = ((e.Buttons & JoystickButtons.B5) == JoystickButtons.B5) ? Color.Red : SystemColors.Control;
                this.lbl_6.BackColor = ((e.Buttons & JoystickButtons.B6) == JoystickButtons.B6) ? Color.Red : SystemColors.Control;
                this.lbl_7.BackColor = ((e.Buttons & JoystickButtons.B7) == JoystickButtons.B7) ? Color.Red : SystemColors.Control;
                this.lbl_8.BackColor = ((e.Buttons & JoystickButtons.B8) == JoystickButtons.B8) ? Color.Red : SystemColors.Control;
                this.lbl_9.BackColor = ((e.Buttons & JoystickButtons.B9) == JoystickButtons.B9) ? Color.Red : SystemColors.Control;
        }

        private void t_Refresh_Tick(object sender, EventArgs e)
        {
            /*
        
                lbl_X.Text = "X:" + _joystick_V.X;
                lbl_Y.Text = "Y:" + _joystick_V.Y;
                lbl_Z.Text = "Z:" + _joystick_V.Z;

                this.lbl_1.BackColor = ((_joystick_V.CurButtonsState & JoystickButtons.B1) == JoystickButtons.B1) ? Color.Red : SystemColors.Control;
                this.lbl_2.BackColor = ((_joystick_V.CurButtonsState & JoystickButtons.B2) == JoystickButtons.B2) ? Color.Red : SystemColors.Control;
                this.lbl_3.BackColor = ((_joystick_V.CurButtonsState & JoystickButtons.B3) == JoystickButtons.B3) ? Color.Red : SystemColors.Control;
                this.lbl_4.BackColor = ((_joystick_V.CurButtonsState & JoystickButtons.B4) == JoystickButtons.B4) ? Color.Red : SystemColors.Control;
                this.lbl_5.BackColor = ((_joystick_V.CurButtonsState & JoystickButtons.B5) == JoystickButtons.B5) ? Color.Red : SystemColors.Control;
                this.lbl_6.BackColor = ((_joystick_V.CurButtonsState & JoystickButtons.B6) == JoystickButtons.B6) ? Color.Red : SystemColors.Control;
                this.lbl_7.BackColor = ((_joystick_V.CurButtonsState & JoystickButtons.B7) == JoystickButtons.B7) ? Color.Red : SystemColors.Control;
                this.lbl_8.BackColor = ((_joystick_V.CurButtonsState & JoystickButtons.B8) == JoystickButtons.B8) ? Color.Red : SystemColors.Control;
                this.lbl_9.BackColor = ((_joystick_V.CurButtonsState & JoystickButtons.B9) == JoystickButtons.B9) ? Color.Red : SystemColors.Control;
            */
        }
    }
}
