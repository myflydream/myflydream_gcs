using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;

namespace JoyKeys.DirectInputJoy
{
    /// <summary>
    /// 游戏手柄类
    /// </summary>
    public class Joystick_V : IDisposable
    {
        /// <summary>
        /// 根据游戏手柄的Id实例化
        /// </summary>
        /// <param name="joystickId"></param>
        private Joystick_V(int joystickId)
        {
            this.Id = joystickId;
            this.JoystickCAPS = new JoystickAPI.JOYCAPS();

            //取得游戏手柄的参数信息
            if (JoystickAPI.joyGetDevCaps(joystickId, ref this.JoystickCAPS, Marshal.SizeOf(typeof(JoystickAPI.JOYCAPS)))
                == JoystickAPI.JOYERR_NOERROR)
            {
                this.IsConnected = true;
                this.Name = this.JoystickCAPS.szPname;
            }
            else
            {
                this.IsConnected = false;
            }

        }

        public static Joystick_V ReturnJoystick(int joystickId)
        {
            Joystick_V joystick = new Joystick_V(joystickId);

            return joystick;
        }

        /// <summary>
        /// 返回当前游戏手柄的Id
        /// </summary>
        public int Id { get; private set; }
        /// <summary>
        /// X方向坐标
        /// </summary>
        public int X { get; private set; }
        /// <summary>
        /// Y方向坐标
        /// </summary>
        public int Y { get; private set; }
        /// <summary>
        /// Z方向坐标
        /// </summary>
        public int Z { get; private set; }

        /// <summary>
        /// 返回当前游戏手柄的名称
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 返回当前游戏手柄是否已连接
        /// </summary>
        public bool IsConnected { get; private set; }
        /// <summary>
        /// 当前按键状态
        /// </summary>
        public JoystickButtons CurButtonsState
        {
            get
            {
                return PreviousButtons;
            }
        }
        /// <summary>
        /// 获取按钮按下状态
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        public bool GetButtonPressedState(JoystickButtons button)
        {
            if ((PreviousButtons & button) == button)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 是否已捕捉
        /// </summary>
        private bool IsCapture { get; set; }

        /// <summary>
        /// 游戏手柄的参数信息
        /// </summary>
        private JoystickAPI.JOYCAPS JoystickCAPS;

        /// <summary>
        /// 定时器
        /// </summary>
        private Timer CaptureTimer;

        #region 事件定义
        /// <summary>
        /// 按钮被单击
        /// </summary>
        public event EventHandler<JoystickEventArgs> Click;
        /// <summary>
        /// 按钮被按下
        /// </summary>
        public event EventHandler<JoystickEventArgs> ButtonDown;
        /// <summary>
        /// 按钮已弹起
        /// </summary>
        public event EventHandler<JoystickEventArgs> ButtonUp;
        /// <summary>
        /// 触发单击事件
        /// </summary>
        /// <param name="e"></param>
        protected void OnClick(JoystickEventArgs e)
        {
            EventHandler<JoystickEventArgs> h = this.Click;
            if (h != null) h(this, e);
        }
        /// <summary>
        /// 触发按钮弹起事件
        /// </summary>
        /// <param name="e"></param>
        protected void OnButtonUp(JoystickEventArgs e)
        {
            EventHandler<JoystickEventArgs> h = this.ButtonUp;
            if (h != null) h(this, e);
        }
        /// <summary>
        /// 触发按钮按下事件
        /// </summary>
        /// <param name="e"></param>
        protected void OnButtonDown(JoystickEventArgs e)
        {
            EventHandler<JoystickEventArgs> h = this.ButtonDown;
            if (h != null) h(this, e);
        }
        #endregion

        /// <summary>
        /// 捕捉游戏手柄
        /// </summary>
        /// <returns></returns>
        public void Capture()
        {
            if (this.IsConnected && !this.IsCapture)
            {
                //手柄已连接
                this.IsCapture = true;
                this.CaptureTimer = new Timer(this.OnTimerCallback, null, 0, 50);
            }
        }

        /// <summary>
        /// 释放捕捉
        /// </summary>
        public void ReleaseCapture()
        {
            if (this.IsCapture)
            {
                this.CaptureTimer.Dispose();
                this.CaptureTimer = null;
                this.IsCapture = false;
            }
        }

        /// <summary>
        /// 前一次的处于按下状态的按钮
        /// </summary>
        private JoystickButtons PreviousButtons = JoystickButtons.None;

        /// <summary>
        /// 定时器的回调方法
        /// </summary>
        /// <param name="state"></param>
        private void OnTimerCallback(object state)
        {
            JoystickAPI.JOYINFOEX infoEx = new JoystickAPI.JOYINFOEX();
            infoEx.dwSize = Marshal.SizeOf(typeof(JoystickAPI.JOYINFOEX));
            infoEx.dwFlags = (int)(JoystickAPI.JOY_RETURNBUTTONS | JoystickAPI.JOY_RETURNPOV);

            int result = JoystickAPI.joyGetPosEx(this.Id, ref infoEx);
            if (result == JoystickAPI.JOYERR_NOERROR)
            {
                JoystickButtons buttons = GetButtons(infoEx.dwButtons);
                GetXYZButtons(infoEx.dwXpos, infoEx.dwYpos, infoEx.dwZpos, ref buttons);
                GetPOVButtons(infoEx.dwPOV, ref buttons);

                if (PreviousButtons != buttons && PreviousButtons != JoystickButtons.None)
                {
                    //按钮状态有更改,则发出ButtonUp事件

                    //取得前一次按下的按钮与本次按下的按钮的差(即不再处于按下状态的按钮)
                    JoystickButtons b = (JoystickButtons)(PreviousButtons - (PreviousButtons & buttons));
                    if (b != JoystickButtons.None)
                    {
                        this.OnButtonUp(new JoystickEventArgs(this.Id, b));

                        //记录还处于按下状态的按钮
                        PreviousButtons = PreviousButtons & buttons;
                    }
                }

                if (buttons != JoystickButtons.None)
                {
                    //本次有按下按钮

                    //取得当前按下的按钮与前一次按下的按钮的差(即本次是新按下的按钮)
                    JoystickButtons b = (JoystickButtons)(buttons - (PreviousButtons & buttons));
                    if (b != PreviousButtons && b != JoystickButtons.None)
                    {
                        this.OnButtonDown(new JoystickEventArgs(this.Id, b));
                        this.OnClick(new JoystickEventArgs(this.Id, b));

                        //记录当前处于按下状态的按钮
                        PreviousButtons = buttons;
                    }
                }
            }
            else
            {
                //其它值无法工作.则停止获取
                //this.ReleaseCapture();
            }
        }

        /// <summary>
        /// 根据按钮码获取当前按下的按钮
        /// </summary>
        /// <param name="dwButtons"></param>
        /// <returns></returns>
        private JoystickButtons GetButtons(int dwButtons)
        {
            JoystickButtons buttons = JoystickButtons.None;
            if ((dwButtons & JoystickAPI.JOY_BUTTON1) == JoystickAPI.JOY_BUTTON1)
            {
                buttons |= JoystickButtons.B1;
            }
            if ((dwButtons & JoystickAPI.JOY_BUTTON2) == JoystickAPI.JOY_BUTTON2)
            {
                buttons |= JoystickButtons.B2;
            }
            if ((dwButtons & JoystickAPI.JOY_BUTTON3) == JoystickAPI.JOY_BUTTON3)
            {
                buttons |= JoystickButtons.B3;
            }
            if ((dwButtons & JoystickAPI.JOY_BUTTON4) == JoystickAPI.JOY_BUTTON4)
            {
                buttons |= JoystickButtons.B4;
            }
            if ((dwButtons & JoystickAPI.JOY_BUTTON5) == JoystickAPI.JOY_BUTTON5)
            {
                buttons |= JoystickButtons.B5;
            }
            if ((dwButtons & JoystickAPI.JOY_BUTTON6) == JoystickAPI.JOY_BUTTON6)
            {
                buttons |= JoystickButtons.B6;
            }
            if ((dwButtons & JoystickAPI.JOY_BUTTON7) == JoystickAPI.JOY_BUTTON7)
            {
                buttons |= JoystickButtons.B7;
            }
            if ((dwButtons & JoystickAPI.JOY_BUTTON8) == JoystickAPI.JOY_BUTTON8)
            {
                buttons |= JoystickButtons.B8;
            }
            if ((dwButtons & JoystickAPI.JOY_BUTTON9) == JoystickAPI.JOY_BUTTON9)
            {
                buttons |= JoystickButtons.B9;
            }
            if ((dwButtons & JoystickAPI.JOY_BUTTON10) == JoystickAPI.JOY_BUTTON10)
            {
                buttons |= JoystickButtons.B10;
            }

            return buttons;
        }

        /// <summary>
        /// 获取X,Y,Z轴的状态
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="buttons"></param>
        private void GetXYZButtons(int x, int y, int z, ref JoystickButtons buttons)
        {
            //处理X,Y轴
            int m = 0xFFFF / 2;                             //中心点的值,偏差0x100
            if ((x - m) > 0x100)
            {
                buttons |= JoystickButtons.Right;
            }
            else if ((m - x) > 0x100)
            {
                buttons |= JoystickButtons.Left;
            }
            if ((y - m) > 0x100)
            {
                buttons |= JoystickButtons.Down;
            }
            else if ((m - y) > 0x100)
            {
                buttons |= JoystickButtons.UP;
            }

            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// 获取POV的状态
        /// </summary>
        /// <param name="pov"></param>
        /// <param name="buttons"></param>
        private void GetPOVButtons(int pov, ref JoystickButtons buttons)
        {
            //处理POV
            int none = 0xFFFF;
            int value = pov / 100;

            if (value == none) return;

            if (value > 359) return;
            else if (value > 45 && value < 135) buttons |= JoystickButtons.POV_Right;
            else if (value >= 135 && value <= 225) buttons |= JoystickButtons.POV_Down;
            else if (value > 225 && value < 315) buttons |= JoystickButtons.POV_Left;
            else buttons |= JoystickButtons.POV_UP;
        }

        #region IDisposable 成员

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            this.ReleaseCapture();
            this.CaptureTimer = null;
        }

        #endregion
    }
}
