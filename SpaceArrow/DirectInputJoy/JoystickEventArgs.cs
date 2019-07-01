using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace JoyKeys.DirectInputJoy
{
    /// <summary>
    /// 游戏手柄的事件参数
    /// </summary>
    public class JoystickEventArgs : EventArgs
    {
        public JoystickEventArgs()
        {

        }
        /// <summary>
        /// 游戏手柄的事件参数
        /// </summary>
        /// <param name="joystickId">手柄Id</param>
        /// <param name="buttons">按钮</param>
        public JoystickEventArgs(int joystickId, JoystickButtons buttons)
        {
            this.JoystickId = joystickId;
            this.Buttons = buttons;
        }

        public JoystickEventArgs(int joystickId, JoystickButtons buttons, Point move)
        {
            this.JoystickId = joystickId;
            this.Buttons = buttons;
            this.Move = move;
        }
        /// <summary>
        /// 手柄Id
        /// </summary>
        public int JoystickId { get; set; }
        /// <summary>
        /// 按钮
        /// </summary>
        public JoystickButtons Buttons { get; set; }

        public Point Move;// { get; set; }
    }

    /// <summary>
    /// 游戏手柄的按钮定义
    /// </summary>
    [Flags]
    public enum JoystickButtons
    {
        //没有任何按钮
        None = 0x0,
        UP = 0x01,
        Down = 0x02,
        Left = 0x04,
        Right = 0x08,
        B1 = 0x10,
        B2 = 0x20,
        B3 = 0x40,
        B4 = 0x80,
        B5 = 0x100,
        B6 = 0x200,
        B7 = 0x400,
        B8 = 0x800,
        B9 = 0x1000,
        B10 = 0x2000,
        B11 = 0x4000,
        B12 = 0x8000,
        POV_UP = 0x10000,
        POV_Down = 0x20000,
        POV_Left = 0x40000,
        POV_Right = 0x80000,
        LeftUp = 0x100000,
        RightUp = 0x200000,
        LeftDown = 0x400000,
        RightDown = 0x800000
    }
}
