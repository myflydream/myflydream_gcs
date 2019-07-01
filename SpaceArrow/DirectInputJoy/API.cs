﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace JoyKeys.DirectInputJoy
{
    public class API
    {
        #region 消息定义
        public const int MM_JOY1MOVE = 0x3A0;
        public const int MM_JOY2MOVE = 0x3A1;
        public const int MM_JOY1BUTTONDOWN = 0x3B5;
        public const int MM_JOY2BUTTONDOWN = 0x3B6;
        public const int MM_JOY1BUTTONUP = 0x3B7;
        public const int MM_JOY2BUTTONUP = 0x3B8;
        #endregion

        #region 按钮定义
        public const int JOY_BUTTON1 = 0x0001;

        public const int JOY_BUTTON2 = 0x0002;

        public const int JOY_BUTTON3 = 0x0004;

        public const int JOY_BUTTON4 = 0x0008;

        public const int JOY_BUTTON5 = 0x0010;

        public const int JOY_BUTTON6 = 0x0020;

        public const int JOY_BUTTON7 = 0x0040;

        public const int JOY_BUTTON8 = 0x0080;

        public const int JOY_BUTTON9 = 0x0100;

        public const int JOY_BUTTON10 = 0x0200;

        public const int JOY_BUTTON11 = 0x0400;

        public const int JOY_BUTTON12 = 0x0800;

        //Button up/down
        public const int JOY_BUTTON1CHG = 0x0100;

        public const int JOY_BUTTON2CHG = 0x0200;

        public const int JOY_BUTTON3CHG = 0x0400;

        public const int JOY_BUTTON4CHG = 0x0800;
        #endregion

        #region 手柄Id定义
        /// <summary>
        /// 主游戏手柄Id
        /// </summary>
        public const int JOYSTICKID1 = 0;
        /// <summary>
        /// 副游戏手柄Id
        /// </summary>
        public const int JOYSTICKID2 = 1;
        #endregion

        #region 错误号定义
        /// <summary>
        /// 没有错误
        /// </summary>
        public const int JOYERR_NOERROR = 0;
        /// <summary>
        /// 参数错误
        /// </summary>
        public const int JOYERR_PARMS = 165;
        /// <summary>
        /// 无法正常工作
        /// </summary>
        public const int JOYERR_NOCANDO = 166;
        /// <summary>
        /// 操纵杆未连接 
        /// </summary>
        public const int JOYERR_UNPLUGGED = 167;
        #endregion

        /// <summary>
        /// 游戏手柄的参数信息JoystickEventArgs
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct JOYCAPS
        {
            public ushort wMid;
            public ushort wPid;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string szPname;
            public int wXmin;
            public int wXmax;
            public int wYmin;
            public int wYmax;
            public int wZmin;
            public int wZmax;
            public int wNumButtons;
            public int wPeriodMin;
            public int wPeriodMax;
            public int wRmin;
            public int wRmax;
            public int wUmin;
            public int wUmax;
            public int wVmin;
            public int wVmax;
            public int wCaps;
            public int wMaxAxes;
            public int wNumAxes;
            public int wMaxButtons;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string szRegKey;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szOEMVxD;
        }

        /// <summary>
        /// 游戏手柄的位置与按钮状态
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct JOYINFO
        {
            public int wXpos;
            public int wYpos;
            public int wZpos;
            public int wButtons;
        }

        /// <summary>
        /// 检查系统是否配置了游戏端口和驱动程序。如果返回值为零，表明不支持操纵杆功能。如果返回值不为零，则说明系统支持游戏操纵杆功能。
        /// </summary>
        /// <returns></returns>
        [DllImport("winmm.dll")]
        public static extern int joyGetNumDevs();

        /// <summary>
        /// 获取某个游戏手柄的参数信息
        /// </summary>
        /// <param name="uJoyID">指定游戏杆(0-15)，它可以是JOYSTICKID1或JOYSTICKID2</param>
        /// <param name="pjc"></param>
        /// <param name="cbjc">JOYCAPS结构的大小</param>
        /// <returns></returns>
        [DllImport("winmm.dll")]
        public static extern int joyGetDevCaps(int uJoyID, ref JOYCAPS pjc, int cbjc);

        /// <summary>
        /// 向系统申请捕获某个游戏杆并定时将该设备的状态值通过消息发送到某个窗口 
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <param name="uJoyID">指定游戏杆(0-15)，它可以是JOYSTICKID1或JOYSTICKID2</param>
        /// <param name="uPeriod">每隔给定的轮询间隔就给应用程序发送有关游戏杆的信息。这个参数是以毫妙为单位的轮询频率。</param>
        /// <param name="fChanged">是否允许程序当操纵杆移动一定的距离后才接受消息</param>
        /// <returns></returns>
        [DllImport("winmm.dll")]
        public static extern int joySetCapture(IntPtr hWnd, int uJoyID, int uPeriod, bool fChanged);

        /// <summary>
        /// 释放操纵杆的捕获
        /// </summary>
        /// <param name="uJoyID">指定游戏杆(0-15)，它可以是JOYSTICKID1或JOYSTICKID2</param>
        /// <returns></returns>
        [DllImport("winmm.dll")]
        public static extern int joyReleaseCapture(int uJoyID);

        /// <summary>
        /// 获取操纵杆位置和按钮状态
        /// </summary>
        /// <param name="uJoyID"></param>
        /// <param name="pji"></param>
        /// <returns></returns>
        [DllImport("winmm.dll")]
        public static extern int joyGetPos(int uJoyID, ref JOYINFO pji);

        /// <summary>
        /// 获取操纵杆位置和按钮状态
        /// </summary>
        /// <param name="uJoyID"></param>
        /// <param name="pji"></param>
        /// <returns></returns>
        //[DllImport("winmm.dll")]
        //public static extern int joyGetPosEx(int uJoyID, ref JOYINFOEX pji);
    }
}
