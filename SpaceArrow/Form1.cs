
using Dongzr.MidiLite;//引用的定时器
using SpaceArrow.ProgressIndication;
using EarthStationMap;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using JoyKeys.DirectInputJoy;
using map;
using MAVLinkWP;
using Serialize;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using System.Threading;
namespace SpaceArrow
{

    public partial class Form1 : Form
    {
        private const ushort AP_DATA_STATUS_ARMED = 0x0020;
        private const ushort AP_DATA_STATUS_TAKEOFF=0x0008;
        private const ushort AP_DATA_STATUS_NEED_HOVER_THR=0x0010;
        private const ushort AP_DATA_STATUS_WPT_CIRCLE =0x0001;
        private const ushort AP_DATA_STATUS_WPT_ARRIVED_AND_CLOSE= 0x0002;
        private const ushort AP_DATA_STATUS_WPT_SINGLE_WPT_VALID = 0x0004;

        private enum FLY_MODE_t
        {
            FM_MANUAL,
            FM_STABLIZER,
            FM_RTH,
            FM_ALT,
            FM_CIRCLE,
            FM_WAY_POINT,
            FM_HOVER,
            FM_SINGLE_WAY_POINT
        };
        private string[] modestring = { "手动", "增稳", "返航", "定高", "盘旋", "多点", "悬停", "单点" };   //模式字符串，
        private int indexmodestring = 0;                                                                    //模式字符串下标，用以确定当前模式。

        private SerializationClass serialize = null;    //序列化记录当前配置信息
        private SerializeDataClass systemdata;          //序列化记录当前飞行回传数据
        //串口数据
        private string serial_com_str;   //代表串口号的字符串
        private string serial_dataBits;  //代表串口数据位的字符串
        private string serial_stopBits;  //代表串口停止位的字符串
        private string serial_baud;      //代表串口波特率
        private string serial_verify;    //代表校验字节的字符串
        private SerialPort serial = null;           //定义串口对象
        private bool isComOpen = false;             //标志串口是否打开

        //有关连接选择的操作
        private string[] connecttype_array = { "Serial Port", "TCP", "UDP", "Bluetooth" };//定义连接类型常量

        //委托
        private delegate void NoParametersNoReturnDele();                                                //串口连接的委托  用于串口打开连接时进行相关操作
        private NoParametersNoReturnDele StartConnect = null;                                            //开启串口连接的委托对象
        private delegate void SetLocation(double lat, double lng,double alt,float yaw);     //声明用于绘制当前飞行器图标的委托
        private delegate void ListBoxMessgeDele(string message);
        private delegate void DrawtailDele(PointLatLngAlt p);
        private delegate void SetMapPlaneDele(PointLatLngAlt location, float yaw);
        private delegate void SetDataViewDele(MAVLink.mavlink_sys_status_t sys_status_t);

        //相关变量的声明
        private MapShow map;                                            //定义地图对象用于显示地图
        private MAVLink_wps mavlink_wps = null;                         //MAVLINK协议相关操作
        private PointLatLngAlt MarkerClickPoint = new PointLatLngAlt(); //点击Maker时，记录Maker的坐标  以地球坐标方式存储
        private PointLatLngAlt MapClickPoint = new PointLatLngAlt();    //点击地图时，记录点击点的坐标  以地球坐标方式存储
        private List<PointLatLngAlt> BookMark = null;                   //记录书签的列表

        private bool isClickMarker = false;                     //用以区别Marker点击事件和Map点击事件,Marker鼠标点击事件先于地图点击事件生成，这里主要是为了防止鼠标右键点击弹出的Menustrip被覆盖。
        private byte[] dataBuffer = new byte[0];    //用以存储从串口接收到的数据
        private byte[] data_packet = new byte[0];   //用以存储解析得到的一个完整数据包。

        private List<PointLatLngAlt> CenterList = new List<PointLatLngAlt>();   //存储飞机尾线的数据点

        //对话框对象
        private Serial_Form serial_form = null;                 //定义串口窗体，用于指定串口号，波特率等。
        private PlayBackForm playback_from = null;              //用于数据回放的对话框
        private SingleWayPointInsert singlewaypoint = null;     //用于单点模式下设置单点高度。
        private BatteryWarning systembatterywarning = null;     //电池电压过低，提示返航界面
        private LoiterTimeForm lf;
        private RoutePlanningForm routeplanningform = null;     //用于航点规划

        private MmTimer timerD = new MmTimer();//多媒体定时器,用于处理，接收到的数据串口数据包对象
        private string datafilepath = System.Windows.Forms.Application.StartupPath + "\\AllFlightdataSave\\";//用于指定数据文件存储路径

        private bool isArmStart = false;    //标识飞行器电机是否启动。

        [DllImport("User32.dll")]
        private static extern Int32 SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        private const int WM_VSCROLL = 0x0115;  //垂直滚动条消息
        private const int SB_LINEDOWN = 1;      //向下滚动一行
        private const int SB_PAGEDOWN = 3;      //向下滚动一页
        private const int SB_BOTTOM = 7;        //滚动到最底部

        private short satellitenumber = 0;                  //存储当前GPS卫星数量
        private DateTime startTime = DateTime.Now;          //记录最后一次解析到完整数据包的时间，用于判断是否断开连接
        private DateTime planeCenterPoint = DateTime.Now;   //标识最后一次添加飞机中心位置的时间，用于绘制飞行器航迹

        private DateTime HeartbeatTime = DateTime.Now;  //心跳时间计算，记录上一次发送心跳包的时间
        private bool connectstate = false;              //连接状态标志，表示GCS是否与飞行器建立了连接
        private float lastpercent = 0;                  //记录上一次的电池百分比，用于防止电池电量上下跳动

        private struct attitude
        {//飞行姿态结构体
            public float yaw;
            public float roll;
            public float pitch;
        }
        private attitude ang;                                   //声明姿态结构体对象，存储实时结构体对象
        private FLY_MODE_t oldmode;                             //枚举飞行器模式，用于日志判断是否有模式切换
        private PointLatLngAlt LatLngAlt = new PointLatLngAlt();//声明坐标点对象，实时存储飞行器经度，纬度以及高度信息
        private bool isGPSarrived = false;                      //是否有GPS信息即经纬度位置信息到达，用于开始绘制飞行器轨迹。
        private int BackGroundOperation = 0;            //系统BackGround控件执行任务标识
        private bool isOperationWhenPowerlow = false;   //标识系统是否检测到电池电压低
        private List<PointLatLngAlt> drawPoint;         //用于在上传航点时，获得来自DataView的航点数据
        private bool CameraStart = true;                //开始和停止摄像标识

        private List<PointLatLng> pointroutes = new List<PointLatLng>();             //航点规划后生成的飞行器航点点集
        private double threshold = 0.0005f;                                         //航线间的距离(单位不是m)
        private double xMove = 0;                                                   //在进行航点规划时，相邻两条航线之间在经度上的偏移
        private double yMove = 0;                                                   //在进行航点规划时，相邻两条航线之间在纬度上的偏移
        private double RouteDirection_K;                                            //进行航点规划时，飞行器飞行线路的方向
        private double RouteIntercept_B;                                            //进行航点规划时，飞行器在垂直于飞行方向上的偏移量
        private List<double> point_K = new List<double>();                          //用于规划航点的点集中相邻两点之间的直线方向
        private List<double> point_B = new List<double>();                          //用于规划航点的点集中相邻两点之间的直线在直线垂直方向上的便宜
        private List<PointLatLngAlt> backupdrawpoint = new List<PointLatLngAlt>();   //用于备份进行航点规划之前的航点点集
        private bool isInplanning = false;                                           //标识是否进行了航点规划
        private int startIndex = 0;                                                  //在进行航点规划时，第一个航点的位置
        private PointLatLngAlt TargetPoint = new PointLatLngAlt();  //在进行云台目标点计算时，标识目标点的位置
        private static int hKeyboardHook = 0;
        private static int hMouseHook = 0;
        private bool InverseMouse = false;
        private bool isCtrlPress = false;   //检测键盘Ctrl键是否被按下
        private bool isShiftPress = false;  //检测键盘Shift键是否被按下
        private bool isKeyBPress = false;   //检测键盘B键是否被按下
        private int bookmarkerindex = 0;    //当前bookmarkerindex的下标，用于快捷键实现书签跳转
      
        #region 遥控手柄
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            JoystickInit();
        }
        /// <summary>
        /// 主动式手柄对象
        /// </summary>
        Joystick_V _joystick_V = null;
        /// <summary>
        /// 被动式手柄对象
        /// </summary>
        Joystick_P _joystick_P = null;
        /// <summary>
        /// 初始化Joystick
        /// </summary>
        private void JoystickInit()
        {
            _joystick_P = new Joystick_P();
            _joystick_P.Move += new EventHandler<JoystickEventArgs>(_joystick_P_Move);

            _joystick_P.Register(this.Handle, API.JOYSTICKID1);
            _joystick_V = Joystick_V.ReturnJoystick(API.JOYSTICKID1);
            _joystick_V.Capture();
        }
        /// <summary>
        /// 卸载Joystick
        /// </summary>
        private void JoystickDispose()
        {
            _joystick_P.Move -= new EventHandler<JoystickEventArgs>(_joystick_P_Move);
            _joystick_P.UnRegister(API.JOYSTICKID1);
            _joystick_V.ReleaseCapture();
            _joystick_V.Dispose();
        }
        void _joystick_P_Move(object sender, JoystickEventArgs e)
        {

            gmTrackBar_pitch.Value =(int)((1 - _joystick_V.Z / 65535.0f) * gmTrackBar_pitch.Maximum);


          //  float pitch = ((gmTrackBar_pitch.Maximum - gmTrackBar_pitch.Value) / (float)gmTrackBar_pitch.Maximum);
          //  label_pitch.Text = "Pitch:" + (180 * pitch - 30).ToString();
        }
        #endregion
        #region 键盘钩子
        // 安装钩子
        [DllImport("user32.dll", CharSet = CharSet.Auto,CallingConvention = CallingConvention.StdCall)]
        public static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);
        // 卸载钩子
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(int idHook);
        // 继续下一个钩子
        [DllImport("user32.dll", CharSet = CharSet.Auto,CallingConvention = CallingConvention.StdCall)]
        public static extern int CallNextHookEx(int idHook, int nCode, Int32 wParam,IntPtr lParam);
        // 取得当前线程编号
        [DllImport("kernel32.dll")]
        static extern int GetCurrentThreadId();
        /*
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        private static extern int SetCursorPos(int x, int y);
        */

        [StructLayout(LayoutKind.Explicit)]
        public struct Input
        {
            [FieldOffset(0)]
            public Int32 type;
            [FieldOffset(4)]
            public MouseInput mi;
            [FieldOffset(4)]
            public tagKEYBDINPUT ki;
            [FieldOffset(4)]
            public tagHARDWAREINPUT hi;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct MouseInput
        {
            public Int32 dx;
            public Int32 dy;
            public Int32 Mousedata;
            public Int32 dwFlag;
            public Int32 time;
            public IntPtr dwExtraInfo;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct tagKEYBDINPUT
        {
            Int16 wVk;
            Int16 wScan;
            Int32 dwFlags;
            Int32 time;
            IntPtr dwExtraInfo;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct tagHARDWAREINPUT
        {
            Int32 uMsg;
            Int16 wParamL;
            Int16 wParamH;
        }
        const int MouseEvent_Absolute = 0x8000;
        const int MouserEvent_Hwheel = 0x01000;
        const int MouseEvent_Move = 0x0001;
        const int MouseEvent_Move_noCoalesce = 0x2000;
        const int MouseEvent_LeftDown = 0x0002;
        const int MouseEvent_LeftUp = 0x0004;
        const int MouseEvent_MiddleDown = 0x0020;
        const int MouseEvent_MiddleUp = 0x0040;
        const int MouseEvent_RightDown = 0x0008;
        const int MouseEvent_RightUp = 0x0010;
        const int MouseEvent_Wheel = 0x0800;
        const int MousseEvent_XUp = 0x0100;
        const int MousseEvent_XDown = 0x0080;

        [DllImport("user32.dll")]
        public static extern UInt32 SendInput(UInt32 nInputs, Input[] pInputs, int cbSize);

        public delegate int HookProc(int nCode, Int32 wParam, IntPtr lParam);

        private HookProc KeyboardHookProcedure;
        private HookProc MouseHookProcedure;

        [StructLayout(LayoutKind.Sequential)]
        public class POINT
        {
            public int x;
            public int y;
        }
        [StructLayout(LayoutKind.Sequential)]
        public class MouseHookStruct
        {
            public POINT pt;
            public int hwnd;
            public int wHitTestCode;
            public int dwExtraInfo;
        }

        Point old_pt = new Point();
        Point screen_size = new Point();

        public void FormKeyDown(Keys key) {
             switch (key)
             {
                 case Keys.ControlKey:
                     isCtrlPress = true;
                     break;
                 case Keys.ShiftKey:
                     isShiftPress = true;
                     break;
                 case Keys.B:
                     isKeyBPress = true;
                     break;
                 default: break;
             }
         }
        public void FormKeyUp(Keys key)
         {
             PointLatLng pp = this.map.mapControl.FromLocalToLatLng(0, 0);
             PointLatLng pp1 = this.map.mapControl.FromLocalToLatLng((int)(0.1f * this.map.mapControl.Width), (int)(0.1f * this.map.mapControl.Height));
             pp1.Lat -= pp.Lat;
             pp1.Lng -= pp.Lng;
             PointLatLng point = this.map.mapControl.Position;
             switch (key) {
                 case Keys.Up:
                     if (!isCtrlPress)
                         point.Lat -= 8 * pp1.Lat;
                     else{
                         this.Yuntailookedup();
                     }
                     break;
                 case Keys.Down:
                     if (!isCtrlPress)
                         point.Lat += 8 * pp1.Lat;
                     else{
                         this.Yuntailookeddown();
                     }
                     break;
                 case Keys.Left:
                     if (!isCtrlPress)
                         point.Lng -= pp1.Lng;
                     else{
                         this.CameraZoomOut();
                     }
                     break;
                 case Keys.Right:
                     if (!isCtrlPress)
                         point.Lng += pp1.Lng;
                     else{
                         this.CameraZoomIn();
                     }
                     break;
                 case Keys.Oemplus:
                     if (this.map.mapControl.MaxZoom > this.map.mapControl.Zoom)
                         this.map.mapControl.Zoom++;
                     break;
                 case Keys.OemMinus:
                     if (this.map.mapControl.MinZoom < this.map.mapControl.Zoom)
                         this.map.mapControl.Zoom--;
                     break;
                 case Keys.ControlKey:
                     isCtrlPress = false;
                     break;
                 case Keys.ShiftKey:
                     isShiftPress = false;
                     break;
                 case Keys.B:
                     isKeyBPress = false;
                     break;
                 case Keys.U:
                     if (isCtrlPress && isKeyBPress==false)
                     {//上传航点
                         UploadWayPoint();
                     }
                     else if (isCtrlPress && isKeyBPress)
                     {
                         if (this.BookMark.Count != 0)
                         {
                             if (bookmarkerindex >= this.BookMark.Count) { bookmarkerindex = 0; }
                             point =Program.PointConversion.GetEarth2Mars(new PointLatLng( this.BookMark[bookmarkerindex].Lat, this.BookMark[bookmarkerindex].Lng));
                             bookmarkerindex++;
                         }
                     }
                     break;
                 case Keys.D:
                     if (isCtrlPress && isKeyBPress == false)
                     {//下载航点
                         DownloadWayPoint();
                     }
                     else if (isCtrlPress && isKeyBPress)
                     {
                         if (this.BookMark.Count != 0)
                         {
                             if (bookmarkerindex >= this.BookMark.Count) { bookmarkerindex = 0; }
                             point = Program.PointConversion.GetEarth2Mars(new PointLatLng(this.BookMark[bookmarkerindex].Lat, this.BookMark[bookmarkerindex].Lng));
                              bookmarkerindex--;
                             if (bookmarkerindex < 0) bookmarkerindex = this.BookMark.Count - 1;
                         }
                     }
                     break;
                 case Keys.R:
                     if (isCtrlPress && isShiftPress==false){//读取任务
                         ReadMission();
                     }else if(isShiftPress && isCtrlPress==false){
                         MissionToLand();
                     }
                     break;
                 case Keys.S:
                     if (isCtrlPress && isShiftPress==false){//保存任务
                         SaveMission();
                     }
                     else if (isShiftPress && isCtrlPress == false) {
                         SingleMode();
                     }
                     break;
                 case Keys.T:
                     if (isCtrlPress){//标记云台目标点
                         YuntaiTargetMarker();
                     }
                     break;
                 case Keys.M:
                     if (isShiftPress && isShiftPress==false) { 
                         MulMode();
                     }
                     else if (isShiftPress && isCtrlPress == false) {
                         MissionTakeOff();
                     }
                     break;
                 case Keys.H:
                     if (isShiftPress)
                     {
                         HoverMode();
                     }
                     break;
                 case Keys.G:
                     if (isShiftPress) {
                         MissionGo();
                     }
                     break;
                 case Keys.Q:
                     if (isCtrlPress)
                     {
                         InverseMouse = !InverseMouse;
                     }
                     break;
                 default: break;
             }
             this.map.mapControl.Position = point;
         }
        private int KeyboardHookProc(int nCode, Int32 wParam, IntPtr lParam)
        {
            Int64 a = lParam.ToInt64();
            Keys key = (Keys)wParam;
            if (this.ActiveControl.GetType().Name.Equals("TextBox") ||
                this.ActiveControl.GetType().Name.Equals("DataGridViewTextBoxEditingControl") ||
                this.ActiveControl.GetType().Name.Equals("DataGridView"))
            {
                return CallNextHookEx(hKeyboardHook, nCode, wParam, lParam);
            }
            if ((nCode > 0 && a > 0) ) {//KeyDown 
                FormKeyDown(key);
            }else if((nCode == 0 && a > 0)){
            
            }else if ((nCode > 0 && a < 0) )
            {
                FormKeyUp(key);
            }else if((nCode == 0 && a < 0)){
            
            }
            return CallNextHookEx(hKeyboardHook, nCode, wParam, lParam);
        }

        private int MouseHookProc(int nCode, Int32 wParam, IntPtr lParam)
        {
            if (nCode < 0)
            {
                return CallNextHookEx(hMouseHook, nCode, wParam, lParam);
            }


            if (nCode == 0 /*HC_ACTION*/ && wParam == 0x0200)
            {
                MouseHookStruct mhs = (MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseHookStruct));

                if (old_pt.X == 0xFFFF && old_pt.Y == 0xFFFF)
                {
                    old_pt.X = mhs.pt.x;
                    old_pt.Y = mhs.pt.y;
                }

                if ( (old_pt.X != mhs.pt.x || old_pt.Y != mhs.pt.y) && InverseMouse)
                {
                    //old_pt.X = old_pt.X - ((int)(mhs.pt.x / 1.5) - old_pt.X);
                    //old_pt.Y = old_pt.Y - ((int)(mhs.pt.y / 1.5) - old_pt.Y);
                    old_pt.X = old_pt.X - ((int)(mhs.pt.x) - old_pt.X);
                    old_pt.Y = old_pt.Y - ((int)(mhs.pt.y) - old_pt.Y);
                    
                    if (old_pt.X < 0) old_pt.X = 0;
                    if (old_pt.Y < 0) old_pt.Y = 0;
                    if (old_pt.X >= Screen.PrimaryScreen.Bounds.Width) old_pt.X = Screen.PrimaryScreen.Bounds.Width - 1;
                    if (old_pt.Y >= Screen.PrimaryScreen.Bounds.Height) old_pt.Y = Screen.PrimaryScreen.Bounds.Height - 1;

                    System.Windows.Forms.Cursor.Position = new  Point(old_pt.X, old_pt.Y);

                    return 1;
                }

                return 0;
            }

            System.Diagnostics.Debug.WriteLine("SHIT SHIT SHIT");
            
            return 0;
            
        }

        public void HookStart()
        {        // 安装钩子
            if (hKeyboardHook == 0)
            {
                // 创建HookProc实例
                KeyboardHookProcedure = new HookProc(KeyboardHookProc);
                // 设置线程钩子
                hKeyboardHook = SetWindowsHookEx(2, KeyboardHookProcedure, IntPtr.Zero,
                                              GetCurrentThreadId());
                // 如果设置钩子失败
                if (hKeyboardHook == 0)
                {
                    HookStop();
                    throw new Exception("HookKeyboard failed.");
                }

                MouseHookProcedure = new HookProc(MouseHookProc);
                hMouseHook = SetWindowsHookEx(14 /*WH_MOUSE_LL*/, MouseHookProcedure, IntPtr.Zero, 0);
                // 如果设置钩子失败
                if (hMouseHook == 0)
                {
                    HookStop();
                    throw new Exception("HookMouse failed.");
                }
                old_pt.X = 0xFFFF;
                old_pt.Y = 0xFFFF;

            }
        }
        public void HookStop()
        {        // 卸载钩子
            bool ret = true;
            if (hKeyboardHook != 0)
            {
                ret = UnhookWindowsHookEx(hKeyboardHook);
                hKeyboardHook = 0;
            }
            if (hMouseHook != 0)
            {
                ret = UnhookWindowsHookEx(hMouseHook);
                hKeyboardHook = 0;
            }
        }
        #endregion
        #region 声明用于数据回放的变量

        struct dataIndex {
            public int positionindex;   //位置下标
            public int postureindex;    //姿态下标
            public int sysstatusindex;  //系统下标
            public int connectindex;    //连接下标
        }
        private dataIndex sysplaybackindex;         //播放下标记录
        private DateTime sysplaybackstartTime;      //数据记录起始时间
        private DateTime sysplaybackendtime;        //数据记录结束时间
        private SerializeDataClass sysplaybackdata; //系统数据保存结构
        private DateTime sysstartplaytime;          //系统开始播放时间
        private bool sysplaybackflag = false;       //是否开始播放数据
        private bool sysplaybackendflag = true;     //播放是否结束标志

        private TimeSpan sysplaybackprogress;       //记录播放进度

        private float sysplaybackyaw = 0;

        #endregion
        public Form1()
        {
            InitializeComponent();

            //地图类初始化   
            this.map = new MapShow(this.gMapControl1);          //给地图对象指定地图控件
            this.map.DirectPointProcess = DirectPointProcess;
            this.map.LeftClick = MapLeftMouseClick;
            this.map.MouseDargChangWPPlanRoutes = MouseDargChangWPPlanRoutes;
            this.map.MouseDragBookMarker = MouseDragChangeBookMarkValue;

            map.MouseRightClick += mapControlMouseRightClick;
            map.WPMarkerRightClick = mapControl_WPMarkerRightClick;
            map.BookMarkMarkerRightClick = mapControl_BookMarkRightClick;
            imageButton_mapzoomminus = new Control.ImageButton();
            imageButton_mapzoomminus.SetImage(global::SpaceArrow.Properties.Resources.mapzoomminus);
            imageButton_mapzoomminus.SetImageDown(global::SpaceArrow.Properties.Resources.mapzoomminusdown);
            this.imageButton_mapzoomminus.BackColor = System.Drawing.Color.Transparent;
            this.gMapControl1.Controls.Add(imageButton_mapzoomminus);
            this.imageButton_mapzoomplus.BackColor = System.Drawing.Color.Transparent;
            this.gMapControl1.Controls.Add(imageButton_mapzoomplus);
            imageButton_mapzoomplus.MouseClick +=   imageButton_mapzoomplus_MouseClick;
            imageButton_mapzoomminus.MouseClick += imageButton_mapzoomminus_MouseClick;

            this.Text = "Ground Control Station 广州长天航空科技有限公司";

            //将窗体设置为无边框窗体，并设置初始显示位置
            this.StartPosition = FormStartPosition.CenterScreen;

            //启动键盘钩子
            HookStart();

            //设置窗体最大化，最小化等 处于右上角的控件
            //最大化按钮  正常显示按钮  属于自定义控件，无需进一步初始化

            //设置 系统模式选择以及操作件
            this.myImageButton_Connect.SetImage("2");
            this.myImageButton_SerialSet.SetImage("3");

            this.myImageButton_Takeoff.SetImage("7");
            this.myImageButton_WayPointGo.SetImage("9");
            this.myImageButton_Toland.SetImage("8");

            this.myImageButton_Multipoint.SetImage("6");
            this.myImageButton_Singlepoint.SetImage("5");
            this.myImageButton_Iremote.SetImage("4");

            //设置消息显示区域
            this.listBox_message.BorderStyle = BorderStyle.None;    //无边框
            this.listBox_message.BackColor = Color.Black;           //背景黑
            this.listBox_message.ForeColor = Color.FromArgb(0, 0, 255, 0);//字体绿
            this.listBox_message.HorizontalScrollbar = true;            //竖直方向滚动条

            //设置连接类型选择comboBox   此时用不到选择故将其隐藏
            this.comboBox_connecttype.DropDownStyle = ComboBoxStyle.DropDownList;
            for (int i = 0; i < connecttype_array.Length; i++)
            {
                this.comboBox_connecttype.Items.Add(connecttype_array[i]);
            }
            this.comboBox_connecttype.SelectedIndex = 0;
            this.comboBox_connecttype.Visible = false;

            //设置窗体显示地图的 TABCON  tabCon_showandset
      //      this.tabcon_mapshow.BackColor = this.BackColor;
            this.tabcon_mapshow.SelectedIndex = 0;//默认选择地图页

            #region comboBox设置
            //设置航点数据显示的DataViewGrid
            this.dataGridView_wp.RowHeadersVisible = false;
            this.dataGridView_wp.ColumnCount = 6;
            this.dataGridView_wp.Columns[0].HeaderText = "编号";
            this.dataGridView_wp.Columns[1].HeaderText = "经度";
            this.dataGridView_wp.Columns[2].HeaderText = "纬度";
            this.dataGridView_wp.Columns[3].HeaderText = "高度";
            this.dataGridView_wp.Columns[4].HeaderText = "离家距离";
            this.dataGridView_wp.Columns[5].HeaderText = "删除";


            //设置书签数据的DataViewGrid
            this.dataView_bookMark.RowHeadersVisible = false;
            this.dataView_bookMark.ColumnCount = 5;
            this.dataView_bookMark.Columns[0].HeaderText = "编号";
            this.dataView_bookMark.Columns[1].HeaderText = "经度";
            this.dataView_bookMark.Columns[2].HeaderText = "纬度";
            this.dataView_bookMark.Columns[3].HeaderText = "距离";
            this.dataView_bookMark.Columns[4].HeaderText = "删除";
            this.dataView_bookMark.CellClick += dataView_bookMark_CellClick;

            //设置地图选择ComboBox
            this.comboBox_mapname.IntegralHeight = false;
            this.comboBox_mapname.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBox_mapname.Items.Add("高德平面地图");
            this.comboBox_mapname.Items.Add("高德卫星地图");
            this.comboBox_mapname.Items.Add("ArcGIS_Imagery_World_2D_Map");
            this.comboBox_mapname.Items.Add("ArcGIS_StreetMap_World_2D_Map");
            this.comboBox_mapname.Items.Add("ArcGIS_Topo_US_2D_Map");
            this.comboBox_mapname.Items.Add("ArcGIS_World_Street_Map");
            this.comboBox_mapname.Items.Add("ArcGIS_World_Terrain_Base_Map");
            this.comboBox_mapname.Items.Add("ArcGIS_World_Topo_Map");
            this.comboBox_mapname.Items.Add("BingHybridMap");
            this.comboBox_mapname.Items.Add("BingMap");
            this.comboBox_mapname.Items.Add("BingSatelliteMap");
            this.comboBox_mapname.Items.Add("CloudMadeMap");
            this.comboBox_mapname.Items.Add("CzechHistoryMap");
            this.comboBox_mapname.Items.Add("CzechHybridMap");
            this.comboBox_mapname.Items.Add("CzechMap");
            this.comboBox_mapname.Items.Add("CzechSatelliteMap");
            this.comboBox_mapname.Items.Add("CzechTuristMap");
            this.comboBox_mapname.Items.Add("GoogleChinaHybridMap");
            this.comboBox_mapname.Items.Add("GoogleChinaMap");
            this.comboBox_mapname.Items.Add("GoogleChinaSatelliteMap");
            this.comboBox_mapname.Items.Add("GoogleChinaTerrainMap");
            this.comboBox_mapname.Items.Add("GoogleHybridMap");
            this.comboBox_mapname.Items.Add("GoogleKoreaHybridMap");
            this.comboBox_mapname.Items.Add("GoogleKoreaMap");
            this.comboBox_mapname.Items.Add("GoogleKoreaSatelliteMap");
            this.comboBox_mapname.Items.Add("GoogleMap");
            this.comboBox_mapname.Items.Add("GoogleSatelliteMap");
            this.comboBox_mapname.Items.Add("GoogleTerrainMap");
            #endregion
            //设置数据显示的TabCon
            this.pagging_MianOpration.BackColor = this.BackColor;
            this.pagging_MianOpration.SelectedIndex = 0;

            //设置分页控件  页面的背景
            this.TabPage_SysOpration.BackColor = this.BackColor;
            this.TabPage_bookMark.BackColor = this.BackColor;
            this.TabPage_WayPoint.BackColor = this.BackColor;

            this.tabPage_map.BackColor = this.BackColor;
            this.tabPage_set.BackColor = this.BackColor;
          //  this.tabcon_mapshow.BackColor = this.BackColor;
            tabPage_datashow.BackColor = this.BackColor;
            tabcon_mapshow.BackColor = Color.Transparent;
            //云台俯仰角控件设置
            gmTrackBar_pitch.Minimum = 0;
            gmTrackBar_pitch.Maximum = 9000;
            gmTrackBar_pitch.Value = gmTrackBar_pitch.Maximum - 0;

            #region 读取配置文件

            this.serialize = XMLSerializer.DeSerialize<SerializationClass>(System.Windows.Forms.Application.StartupPath+"\\config.txt");
            if (serialize != null)
            {
                serial_com_str = this.serialize.ComPortName;
                serial_dataBits = this.serialize.ComDataBits;
                serial_stopBits = this.serialize.ComStopBit;
                serial_baud = this.serialize.ComBaud;
                serial_verify = this.serialize.ComVerify;

                this.map.colorMouseLocation = this.serialize.GetColorMouseLocation();
                this.map.colorRoutes = this.serialize.GetColorRoutes();
                this.map.colorScale = this.serialize.GetColorScale();
                this.map.colorTargetLocation = this.serialize.GetColorTargetLocation();
                this.map.colorTrack = this.serialize.GetColorTrack();
                this.map.colorZoom = this.serialize.GetColorZoom();

                if (this.map.colorMouseLocation == Color.Transparent) this.map.colorMouseLocation = Color.White;
                if (this.map.colorRoutes == Color.Transparent) this.map.colorRoutes = Color.Red;
                if (this.map.colorScale == Color.Transparent) this.map.colorScale = Color.Yellow;
                if (this.map.colorTargetLocation == Color.Transparent) this.map.colorTargetLocation = Color.White;
                if (this.map.colorTrack == Color.Transparent) this.map.colorTrack = Color.Blue;
                if (this.map.colorZoom == Color.Transparent) this.map.colorZoom = Color.Red;

                this.datafilepath = this.serialize.datafilepath;
                if (this.datafilepath == null) this.datafilepath = System.Windows.Forms.Application.StartupPath + "\\AllFlightdataSave\\";
                Program.currentpath = this.serialize.logfilepath;
                if (Program.currentpath == null) Program.currentpath = System.Windows.Forms.Application.StartupPath + "\\logFile";
                Program.logFileName = Program.currentpath + "\\log" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".txt";
                if (!Directory.Exists(Program.currentpath))
                {
                    Directory.CreateDirectory(Program.currentpath);
                }
                this.comboBox_mapname.Text = serialize.MapName;
                map_choice(serialize.MapName);

                #region 设置图标
                if (this.serialize.isIconHomeUseFile)
                {
                    try
                    {
                        Bitmap bit = new Bitmap(this.serialize.iconhome.filepath);
                        Bitmap bit1 = new Bitmap(this.serialize.iconhome.iconwidth, this.serialize.iconhome.iconheight);
                        Rectangle rect_src = new Rectangle(0, 0, bit.Width, bit.Height);
                        Rectangle rect_dest = new Rectangle(0, 0, bit1.Width, bit1.Height);
                        Graphics g = Graphics.FromImage(bit1);
                        g.DrawImage(bit, rect_dest, rect_src, GraphicsUnit.Pixel);

                        this.map.BitmapHome = bit1;
                        this.map.SetHome(this.map.homeLocation);
                    }
                    catch
                    {

                    }
                }
                if (this.serialize.isIconPlaneUseFile)
                {
                    try
                    {
                        Bitmap bit = new Bitmap(this.serialize.iconplane.filepath);
                        Bitmap bit1 = new Bitmap(this.serialize.iconplane.iconwidth, this.serialize.iconplane.iconheight);
                        Rectangle rect_src = new Rectangle(0, 0, bit.Width, bit.Height);
                        Rectangle rect_dest = new Rectangle(0, 0, bit1.Width, bit1.Height);
                        Graphics g = Graphics.FromImage(bit1);
                        g.DrawImage(bit, rect_dest, rect_src, GraphicsUnit.Pixel);

                        this.map.BitmapPlane = bit1;
                        this.map.mapControl_SetPlanePosition(this.map.mapPlanePoint.Lng, this.map.mapPlanePoint.Lat, 30, this.map.planeInmap_yaw);
                    }
                    catch
                    {

                    }
                }
                if (this.serialize.isIconWpMarkerUseFile)
                {
                    Bitmap bit = new Bitmap(this.serialize.iconwpmarker.filepath);
                    Bitmap bit1 = new Bitmap(this.serialize.iconwpmarker.iconwidth, this.serialize.iconwpmarker.iconheight);
                    Rectangle rect_src = new Rectangle(0, 0, bit.Width, bit.Height);
                    Rectangle rect_dest = new Rectangle(0, 0, bit1.Width, bit1.Height);
                    Graphics g = Graphics.FromImage(bit1);
                    g.DrawImage(bit, rect_dest, rect_src, GraphicsUnit.Pixel);

                    this.map.BitmapWpMarker = bit1;
                }
                if (this.serialize.isIconBookMarkerUseFile)
                {
                    Bitmap bit = new Bitmap(this.serialize.iconbookmarker.filepath);
                    Bitmap bit1 = new Bitmap(this.serialize.iconbookmarker.iconwidth, this.serialize.iconbookmarker.iconheight);
                    Rectangle rect_src = new Rectangle(0, 0, bit.Width, bit.Height);
                    Rectangle rect_dest = new Rectangle(0, 0, bit1.Width, bit1.Height);
                    Graphics g = Graphics.FromImage(bit1);
                    g.DrawImage(bit, rect_dest, rect_src, GraphicsUnit.Pixel);

                    this.map.BitmapBookMarker = bit1;
                }
                #endregion
            
                
                //this.map.multipleflag =serialize.DebugFlag;
                this.map.multipleflag = 1.0f;
                this.map.homeLocation = this.serialize.HomeLocation;
                this.map.mapCenterPoint = this.serialize.mapCenter;
                this.map.mapPlanePoint = this.serialize.planeLocation;
                this.map.mapControl_SetPlanePosition(this.map.mapPlanePoint.Lng, this.map.mapPlanePoint.Lat, 0, 0);

                if (this.serialize.BookMark != null)
                {
                    int i = 1;
                    BookMark = new List<PointLatLngAlt>();
                    foreach (PointLatLngAlt p in this.serialize.BookMark)
                    {
                        this.addBookMark(p);
                        i++;
                    }
                }
            }
            else
            {
               if (!Directory.Exists(Program.currentpath))
               {
                   Directory.CreateDirectory(Program.currentpath);
               }
               Program.logFileName = Program.currentpath + "\\log" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".txt";
               this.comboBox_mapname.SelectedIndex = 0;
            }
            #endregion


            this.LatLngAlt.Lat = this.map.mapPlanePoint.Lat;
            this.LatLngAlt.Lng = this.map.mapPlanePoint.Lng;
            this.map.SetDataView(this.dataGridView_wp);
            this.map.SetHome(this.map.homeLocation);
            this.checkBox_showtrack.Checked = true;
            this.MinimumSize = new System.Drawing.Size(800, 800);
            gmTrackBar_pitch.Value =(int) (5.0 / 6.0 * gmTrackBar_pitch.Maximum);
            if (Program.boolhidden) {
                HiddenControl();
            }

            /*
            //程序运行位置
            string R_startPath = Application.ExecutablePath;
            //对应于HKEY_LOCAL_MACHINE主键
            Microsoft.Win32.RegistryKey R_local = Microsoft.Win32.Registry.LocalMachine;
            //开机自动运行
            Microsoft.Win32.RegistryKey Rkey = R_local.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
            // Microsoft.Win32.RegistryKey Rkey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (Rkey == null)
                Rkey = R_local.CreateSubKey("SOFTWARE\\Microsofdt\\Windows\\CurrentVersion\\Run");
            Rkey.SetValue("SpaceArrowGCS", R_startPath);//修改注册表，使程序开机时自动执行。  
            //  MessageBox.Show("程序设置完成，重新启动计算机后即可生效！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Rkey.Close();
             */


            //初始化启动为最大化，窗体不占用任务栏
            this.MaximumSize = new Size(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height);
            this.WindowState = FormWindowState.Maximized;
            this.MinimumSize = new System.Drawing.Size(600, 600);
            this.WindowState = FormWindowState.Maximized;

            this.Resize += new System.EventHandler(this.Form_Resize);
            this.FormClosing += Form1_FormClosing;
            this.FormClosed += Form1_FormClosed;

            this.imageButton_Marker.SetImage(global::SpaceArrow.Properties.Resources.marker);
            this.imageButton_Marker.SetImageDown(global::SpaceArrow.Properties.Resources.markerdown);

            imageButton_Marker.MouseClick += imageButton_Marker_MouseClick;          
        }
        /// <summary>
        /// 地图上，比例缩放加按钮点击事件,实现点击按钮地图ZOOM增大,视角高度增大。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void imageButton_mapzoomminus_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left) { 
                if (this.map.mapControl.Zoom > this.map.mapControl.MinZoom) {
                    this.map.mapControl.Zoom--;
                }            
            }
        }
        /// <summary>
        /// 地图上，比例缩放减按钮点击事件,实现点击按钮地图ZOOM减小,视角高度降低。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void imageButton_mapzoomplus_MouseClick(object sender, MouseEventArgs e)
        {
            //throw new NotImplementedException();
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (this.map.mapControl.Zoom < this.map.mapControl.MaxZoom)
                {
                    this.map.mapControl.Zoom++;
                }
            }

        }
        /// <summary>
        /// 单点模式执行函数，绘制单点标识，发送单点指令，串口关闭时函数无效
        /// </summary>
        /// <param name="directPoint">单点经纬度坐标</param>
        private void DirectPointProcess(PointLatLng directPoint){
            if (!this.isComOpen)
            {
              MessageBox.Show("当前无连接");
                return;
            }
            if (this.singlewaypoint.Visible) {
                try
                {
                    this.map.MouseClickPoint.Alt = float.Parse(this.singlewaypoint.textBox1.Text);
                    this.map.DrawTargetPoint(this.map.MouseClickPoint);
                    this.mavlink_wps.GoToTargetPoint(this.map.MouseClickPoint);
                }
                catch { 
                
                }
            }
        }
        /// <summary>
        /// 书签列表，Cell点击事件，这里只对ButtonCell有效，
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataView_bookMark_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int colIndex = e.ColumnIndex;
            int rowIndex = e.RowIndex;
            if (colIndex == -1 || rowIndex == -1) return;
            DataGridViewCell c = (((DataGridView)sender).Rows[rowIndex]).Cells[colIndex];
            if (!c.GetType().ToString().EndsWith("ButtonCell"))
                return;

            PointLatLngAlt p = new PointLatLngAlt();
            try
            {
                p.Lng = double.Parse(this.dataView_bookMark.CurrentRow.Cells[1].Value.ToString());
                p.Lat = double.Parse(this.dataView_bookMark.CurrentRow.Cells[2].Value.ToString());
            }
            catch
            {
                return;
            }

            this.BookMark.RemoveAt(e.RowIndex);
            this.map.ClearBookMark();
            this.dataView_bookMark.Rows.Clear();

            List<PointLatLngAlt> books = new List<PointLatLngAlt>();

            foreach(PointLatLngAlt pp in BookMark){
                PointLatLngAlt ppp = new PointLatLngAlt(pp);
                books.Add(ppp);
            }

            BookMark.Clear();
            this.totaldistance = 0;
            foreach (PointLatLngAlt p1 in books)
            {
                this.addBookMark(p1);
            }
        }
        void mapControlMouseRightClick(object sender, MouseEventArgs e) {//地图控件鼠标右键点击地图事件，MapRightMouseClick
            if (isClickMarker) { isClickMarker = false; return;}
            PointLatLng p = map.mapControl.FromLocalToLatLng(e.X, e.Y);
            PointLatLng p1 = Program.PointConversion.Mar2Earth(p);
            MapClickPoint.Lat = p1.Lat;
            MapClickPoint.Lng = p1.Lng;
            this.map.MouseClickPoint.Lat = p1.Lat;
            this.map.MouseClickPoint.Lng = p1.Lng;
            this.MenuStrip_Map.Show(Cursor.Position);
        }
        void mapControl_BookMarkRightClick(GMapMarker item, MouseEventArgs e) {//存储BookMarker的地图层，鼠标右键点击Marker事件
            isClickMarker = true;
            MarkerClickPoint.Lat = item.Position.Lat;
            MarkerClickPoint.Lng = item.Position.Lng;
            PointLatLng p = Program.PointConversion.Mar2Earth(new PointLatLng(this.MarkerClickPoint.Lat, this.MarkerClickPoint.Lng));
            MarkerClickPoint.Lat = p.Lat;
            MarkerClickPoint.Lng = p.Lng;
            this.MenuStrip_BookMark.Show(Cursor.Position);
        }
        void mapControl_WPMarkerRightClick(GMapMarker item, MouseEventArgs e)
        {//存储航点的地图层，鼠标右键点击Marker事件。
            isClickMarker = true;
            MarkerClickPoint.Lat = item.Position.Lat;
            MarkerClickPoint.Lng = item.Position.Lng;


            PointLatLng p = Program.PointConversion.Mar2Earth(new PointLatLng(this.MarkerClickPoint.Lat, this.MarkerClickPoint.Lng));
            MarkerClickPoint.Lat = p.Lat;
            MarkerClickPoint.Lng = p.Lng;
            this.MenuStrip_WP.Show(Cursor.Position);
        }
        private void Form_Resize(object sender, EventArgs e)
        {
            FResize();
        }      
        void FromClose() {
            timerD.Stop();
            HookStop();
            System.Threading.Thread.Sleep(10);

            if (this.serialize == null)
            {
                this.serialize = new SerializationClass();
            }
            this.serialize.BookMark =       this.BookMark;
            this.serialize.ComPortName =    this.serial_com_str;
            this.serialize.ComDataBits =    this.serial_dataBits;
            this.serialize.ComBaud =        this.serial_baud;
            this.serialize.ComStopBit =     this.serial_stopBits;
            this.serialize.ComVerify =      this.serial_verify;

            this.serialize.SetColorMouseLocation(this.map.colorMouseLocation);
            this.serialize.SetColorRoutes(this.map.colorRoutes);
            this.serialize.SetColorScale(this.map.colorScale);
            this.serialize.SetColorTargetLocation(this.map.colorTargetLocation);
            this.serialize.SetColorTrack(this.map.colorTrack);
            this.serialize.SetColorZoom(this.map.colorZoom);

            this.serialize.MapName = this.comboBox_mapname.Text;
            this.serialize.HomeLocation = this.map.homeLocation;
            this.serialize.mapCenter = this.map.mapCenterPoint;
            this.serialize.planeLocation = this.map.mapPlanePoint;

            if (serialize != null)
                XMLSerializer.Serialize<SerializationClass>(this.serialize, "config.txt");
            if (this.isComOpen)
            {
                this.serial.Close();
                this.serial.Dispose();
                string filename1=this.datafilepath+"FlightData" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".txt";
              //  string filename1 = Program.currentpath + "\\AllFlightdataSave\\FlightData" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".txt";
                if (!Directory.Exists(datafilepath))
                {
                    Directory.CreateDirectory(datafilepath);
                }
                XMLSerializer.Serialize<SerializeDataClass>(this.systemdata, filename1);
            }
            Program.WriteLog("GCS正常关闭...");
        }
        #region 窗体消息响应
        private void Form_Paint(object sender, PaintEventArgs e)
        {
            //if (!Program.FORM_BORDER_NONE) {
            //    return;
            //}
            //Graphics g = e.Graphics;
            //SolidBrush b = new SolidBrush(Color.Black);
            //Rectangle rect = new Rectangle(0, 0, bit.Width, bit.Height);
            //Rectangle disrect = new Rectangle(0, 0, 40, 30);
            //g.DrawImage(bit, disrect, rect, System.Drawing.GraphicsUnit.Pixel);

            //Font font = new Font("宋体",10);
            //g.DrawString(this.title, font, b, new PointF(45,7));
        }
        void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            FromClose();
        }
        void ClosePlayBackForm() {
            if (playback_from != null) { 
                if (this.playback_from.InvokeRequired)
                {
                    this.playback_from.Invoke(new NoParametersNoReturnDele(ClosePlayBackForm));
                }
                else {
                    if(this.playback_from!=null)
                    this.playback_from.Close();
                }            
            }

        }
        void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            timerD.Stop();
            timerD.Dispose();
            ClosePlayBackForm();
            this.dataGridView_wp.Dispose();
            this.dataView_bookMark.Dispose();
            this.listBox_message.Dispose();
            this.gMapControl1.Dispose();
            this.myTabCon1.Dispose();
            this.Controls.Clear();
            this.Dispose();
            System.Environment.Exit(0);
        }
        private void SendHeartBeatToCopter()
        {
            if (this.InvokeRequired)
            {
                try
                {
                    NoParametersNoReturnDele d = new NoParametersNoReturnDele(SendHeartBeatToCopter);
                this.BeginInvoke(d, new Object[0] {} );
                }
                catch {
                    return;
                }
            }
            else
            {
                mavlink_wps.SendHeartBeat();
            }
        }
        private void ListBoxAddMessage(string message) {
            if(this.listBox_message.InvokeRequired){
                ListBoxMessgeDele l=new ListBoxMessgeDele(ListBoxAddMessage);
                this.BeginInvoke(l,new Object[1]{message});
            }else{
                this.listBox_message.Items.Add(message);
                SendMessage(listBox_message.Handle, WM_VSCROLL, SB_LINEDOWN, 0);
            }

        }
        private void Form1_Load(object sender, EventArgs e)
        {
            timerD.Mode = MmTimerMode.Periodic;//循环执行
            timerD.Interval = 50;
            timerD.Tick += timerD_Tick;
            timerD.Start();
            FResize();
        }
        void timerD_Tick(object sender, System.EventArgs e)
        {
            //throw new System.NotImplementedException();
            DataProcess();
        }
        public void FResize()
        {
            this.MaximumSize = new Size(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height);
            int btnCloseFrom_width = (int)(0.006*this.Width);
            int btnCloseFrom_height = (int)(0.03 * this.Height);
             this.MaximumSize = new Size(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height);
            this.myImageButton_Connect.SetBounds(Width - (btnCloseFrom_width + 5) * 6, 
                (int)(((Height - btnCloseFrom_height) * 0.06f)*0.2f),
                (int)((Width - (btnCloseFrom_width + 5) * 6) * 0.03f), 
                (int)((Height - btnCloseFrom_height) * 0.05f));
            this.myImageButton_SerialSet.SetBounds(Width - (btnCloseFrom_width + 5) * 6 - (int)((Width - (btnCloseFrom_width + 5) * 6) * 0.03f) * 1,
                (int)(((Height - btnCloseFrom_height) * 0.06f) * 0.2f), 
                (int)((Width - (btnCloseFrom_width + 5) * 6) * 0.03f), 
                (int)((Height-btnCloseFrom_height)*0.05f));

            this.myImageButton_Takeoff.SetBounds(Width - (btnCloseFrom_width + 5) * 6 - (int)(((Width - (btnCloseFrom_width + 5) * 6) * 0.03f) * 2.5f),
                (int)(((Height - btnCloseFrom_height) * 0.06f) * 0.2f),
                (int)((Width - (btnCloseFrom_width + 5) * 6) * 0.03f),
                (int)((Height - btnCloseFrom_height) * 0.05f));
            this.myImageButton_WayPointGo.SetBounds(Width - (btnCloseFrom_width + 5) * 6 - (int)(((Width - (btnCloseFrom_width + 5) * 6) * 0.03f) * 3.5f),
                (int)(((Height - btnCloseFrom_height) * 0.06f) * 0.2f),
                (int)((Width - (btnCloseFrom_width + 5) * 6) * 0.03f),
                (int)((Height - btnCloseFrom_height) * 0.05f));
            this.myImageButton_Toland.SetBounds(Width - (btnCloseFrom_width + 5) * 6 - (int)(((Width - (btnCloseFrom_width + 5) * 6) * 0.03f) * 4.5f),
                (int)(((Height - btnCloseFrom_height) * 0.06f) * 0.2f),
                (int)((Width - (btnCloseFrom_width + 5) * 6) * 0.03f),
                (int)((Height - btnCloseFrom_height) * 0.05f));

            this.myImageButton_Multipoint.SetBounds(Width - (btnCloseFrom_width + 5) * 6 - (int)(((Width - (btnCloseFrom_width + 5) * 6) * 0.03f) * 6f),
                 (int)(((Height - btnCloseFrom_height) * 0.06f) * 0.2f),
                 (int)((Width - (btnCloseFrom_width + 5) * 6) * 0.03f),
                 (int)((Height - btnCloseFrom_height) * 0.05f));
            this.myImageButton_Singlepoint.SetBounds(Width - (btnCloseFrom_width + 5) * 6 - (int)(((Width - (btnCloseFrom_width + 5) * 6) * 0.03f) * 7f),
                (int)(((Height - btnCloseFrom_height) * 0.06f) * 0.2f),
                (int)((Width - (btnCloseFrom_width + 5) * 6) * 0.03f),
                (int)((Height - btnCloseFrom_height) * 0.05f));
            this.myImageButton_Iremote.SetBounds(Width - (btnCloseFrom_width + 5) * 6 - (int)(((Width - (btnCloseFrom_width + 5) * 6) * 0.03f) * 8f),
                (int)(((Height - btnCloseFrom_height) * 0.06f) * 0.2f),
                (int)((Width - (btnCloseFrom_width + 5) * 6) * 0.03f),
                (int)((Height - btnCloseFrom_height) * 0.05f));

            this.battery.SetBounds(Width - (btnCloseFrom_width + 5) * 6 - (int)(((Width - (btnCloseFrom_width + 5) * 6) * 0.03f) * 10.0f),
                (int)(((Height - btnCloseFrom_height) * 0.06f) * 0.2f),
                (int)((Width - (btnCloseFrom_width + 5) * 6) * 0.05f),
                (int)((Height - btnCloseFrom_height) * 0.05f));


            string modestr = this.modestring[indexmodestring];
            Font f = new System.Drawing.Font("宋体",0.018f*Width,FontStyle.Bold);
            Graphics g = this.CreateGraphics();
            SizeF size = g.MeasureString(modestr,f);
            this.label_mode.Font = f;
            this.label_mode.BackColor=Color.Transparent;


            this.label_mode.SetBounds(Width - (btnCloseFrom_width + 5) * 6 - (int)(((Width - (btnCloseFrom_width + 5) * 6) * 0.03f) * 12.5f),
                (int)(((Height - btnCloseFrom_height) * 0.06f) * 0.2f),
                (int)((Width - (btnCloseFrom_width + 5) * 6) * 0.1f),
                (int)((Height - btnCloseFrom_height) * 0.05f));

            this.label_mode.Text = modestr;

            this.imageButton_Marker.SetBounds(Width - (btnCloseFrom_width + 5) * 6 - (int)(((Width - (btnCloseFrom_width + 5) * 6) * 0.03f) * 14f),
                (int)(((Height - btnCloseFrom_height) * 0.06f) * 0.2f),
                (int)((Width - (btnCloseFrom_width + 5) * 6) * 0.03f),
                (int)((Height - btnCloseFrom_height) * 0.05f));
            int startheiht = btnCloseFrom_height + 5;
            float mapscale = 0.7f;
            this.tabcon_mapshow.SetBounds((int)((1 - mapscale) * this.Width), startheiht, (int)(mapscale * this.Width), this.Height - startheiht);
            int leftedge = 10;

            this.myTabCon1.SetBounds(leftedge, startheiht, (int)((1 - mapscale) * this.Width) - 5 - leftedge, (int)((this.Height - startheiht) * 0.4f));

            this.comboBox_mapname.SetBounds(leftedge, (int)(startheiht*1.3f) + (int)((this.Height - startheiht) * 0.4f) ,(int)( 0.4 * this.myTabCon1.Width), (int)((this.Height - startheiht) * 0.05f));
            checkBox1.SetBounds(comboBox_mapname.Location.X + comboBox_mapname.Width+2, comboBox_mapname.Location.Y, (int)(0.2 * this.myTabCon1.Width), (int)((this.Height - startheiht) * 0.03f));
            checkBox_showtrack.SetBounds(checkBox1.Location.X + checkBox1.Width, comboBox_mapname.Location.Y, (int)(0.2 * this.myTabCon1.Width), (int)((this.Height - startheiht) * 0.03f));

            label_pitch.SetBounds(checkBox_showtrack.Location.X + checkBox_showtrack.Width, comboBox_mapname.Location.Y, (int)(0.2 * this.myTabCon1.Width), (int)((this.Height - startheiht) * 0.03f));
          
            
          
            this.pagging_MianOpration.SetBounds(leftedge, comboBox_mapname.Location.Y + comboBox_mapname.Height, (int)((0.9f)*(1 - mapscale) * this.Width) - 5 - leftedge, (int)((this.Height - startheiht) * 0.36f));


            TabPage_SysOpration.SetBounds(pagging_MianOpration.Location.X, pagging_MianOpration.Location.Y, pagging_MianOpration.Width, pagging_MianOpration.Height);

            checkBox1.AutoSize = false;
            checkBox_showtrack.AutoSize = false;

            /*
            gmTrackBar_pitch.SetBounds(
                tabcon_mapshow.Location.X - (int)((0.1f) * (1 - mapscale) * this.Width),
                comboBox_mapname.Location.Y + label_pitch.Height, 
                (int)((0.1f) * (1 - mapscale) * this.Width) - 5 - leftedge,
               (int)((this.Height - startheiht) * 0.4f) - comboBox_mapname.Height-2);
             */
            gmTrackBar_pitch.SetBounds(
                tabcon_mapshow.Location.X - (int)((0.1f) * (1 - mapscale) * this.Width) + 10,
                comboBox_mapname.Location.Y + label_pitch.Height + 60,
                (int)((0.1f) * (1 - mapscale) * this.Width) - 5 - leftedge,
               (int)((this.Height - startheiht) * 0.4f) - comboBox_mapname.Height - 2 - 100);

            button_yaw_left.SetBounds(
                tabcon_mapshow.Location.X - (int)((0.1f) * (1 - mapscale) * this.Width),
                comboBox_mapname.Location.Y + label_pitch.Height + 60 - 30,
                22,30);
            button_yaw_right.SetBounds(
                button_yaw_left.Right,
                button_yaw_left.Top,
                22, 30);

            btn_ClearWP.SetBounds(0, 0, (int)(0.2F * TabPage_SysOpration.Width), (int)(0.15F * this.TabPage_SysOpration.Height));

            SetButtonPosition(button_Cleartrack,0,1);
            SetButtonPosition(button_playback, 4, 0);

            SetButtonPosition(btn_getWP, 1, 0);
            SetButtonPosition(btn_SendWP, 1, 1);

            SetButtonPosition(button_export, 3, 1);
            SetButtonPosition(button_import, 3, 0);

            SetButtonPosition(button_savewp, 2, 1);
            SetButtonPosition(button_LoadwayPoint, 2, 0);

            SetButtonPosition(button_Read,0,2);
          
            SetButtonPosition(but_DownMapPart, 4, 1);

            textBox_SearchPlace.SetBounds(this.button_missionStart.Location.X, this.btn_ClearWP.Location.Y + this.btn_ClearWP.Height * 3, this.btn_ClearWP.Width, this.btn_ClearWP.Height);

            SetButtonPositionf(this.button_toGpx, 0, 5.0f);
            SetButtonPositionf(this.button_Read, 1, 5.0f);
            SetButtonPositionf(this.button_SetHome, 2, 5.0f);
            SetButtonPositionf(button_pitchplus,3,5.0f);
            SetButtonPositionf(button_pitchminus, 4, 5.0f);

            SetButtonPositionf(button_camera, 0, 3.0f);
            SetButtonPositionf(button_toawmfile, 1, 3.0f);
            SetButtonPositionf(button_importawmfile, 2, 3.0f);
            SetButtonPositionf(button_missionStart, 4, 3.0f);   //level gimbal
            SetButtonPositionf(button_LocHold, 3, 3.0f);



            this.dataGridView_wp.SetBounds(0, 0, this.TabPage_SysOpration.Width, this.TabPage_SysOpration.Height);
            this.dataGridView_wp.ColumnCount = 6;
            this.dataGridView_wp.Columns[0].Width = (int)(0.12f * this.dataGridView_wp.Width);
            this.dataGridView_wp.Columns[1].Width = (int)(0.17f * this.dataGridView_wp.Width);
            this.dataGridView_wp.Columns[2].Width = (int)(0.17f * this.dataGridView_wp.Width);
            this.dataGridView_wp.Columns[3].Width = (int)(0.17f * this.dataGridView_wp.Width);
            this.dataGridView_wp.Columns[4].Width = (int)(0.17f * this.dataGridView_wp.Width);
            this.dataGridView_wp.Columns[5].Width = (int)(0.15f * this.dataGridView_wp.Width);

            label_totaldistance.SetBounds((int)(0.7 * this.TabPage_WayPoint.Width),
                this.TabPage_WayPoint.Height - label_totaldistance.Height,
                (int)(0.3 * this.TabPage_WayPoint.Width),
                label_totaldistance.Height
                );

            this.dataView_bookMark.SetBounds(0, 0, this.TabPage_WayPoint.Width, this.TabPage_WayPoint.Height - label_totaldistance.Height -2);
            this.dataView_bookMark.ColumnCount = 5;
            this.dataView_bookMark.Columns[0].Width = (int)(0.13f * this.dataView_bookMark.Width);
            this.dataView_bookMark.Columns[1].Width = (int)(0.26f * this.dataView_bookMark.Width);
            this.dataView_bookMark.Columns[2].Width = (int)(0.26f * this.dataView_bookMark.Width);
            this.dataView_bookMark.Columns[3].Width = (int)(0.2f * this.dataView_bookMark.Width);
            this.dataView_bookMark.Columns[4].Width = (int)(0.2f * this.dataView_bookMark.Width);
            this.listBox_message.SetBounds(leftedge, startheiht + (int)((this.Height - startheiht) * 0.8f), (int)((1 - mapscale) * this.Width) - 5 - leftedge, (int)((this.Height - startheiht) * 0.16f));
            this.gMapControl1.SetBounds(0, 0, this.tabPage_map.Width, this.tabPage_map.Height);
            this.imageButton_mapzoomminus.Size = new System.Drawing.Size((int)(0.05 * gMapControl1.Width), (int)(0.06 * gMapControl1.Height));
            this.imageButton_mapzoomminus.Location = new System.Drawing.Point((int)(gMapControl1.Width - imageButton_mapzoomminus.Width * 1.5f), (int)(gMapControl1.Height - imageButton_mapzoomminus.Height * 2.5f));
            this.imageButton_mapzoomplus.Size = new System.Drawing.Size((int)(0.05 * gMapControl1.Width), (int)(0.06 * gMapControl1.Height));
            this.imageButton_mapzoomplus.Location = new System.Drawing.Point((int)(gMapControl1.Width - imageButton_mapzoomplus.Width*1.5f), (int)(gMapControl1.Height -imageButton_mapzoomminus.Height- imageButton_mapzoomplus.Height*2.5f));
            if (WindowState == FormWindowState.Minimized)
            {
                if (routeplanningform != null) routeplanningform.Visible = false;
                if (singlewaypoint != null) singlewaypoint.Visible = false;

                return;
            }
            else {
                if (routeplanningform != null) routeplanningform.Visible = true;
                if (singlewaypoint != null) singlewaypoint.Visible = true;

            }
            if (routeplanningform != null) { 
                routeplanningform.Location = new System.Drawing.Point((int)(this.Width - routeplanningform.Width * 1.08f), (this.myImageButton_Connect.Height * 4));
            }
            if (this.singlewaypoint != null) {
                singlewaypoint.Location = new System.Drawing.Point(this.Width - singlewaypoint.Width, (this.myImageButton_Connect.Height * 4));
            }
        }
        void SetButtonPosition(System.Windows.Forms.Button btn, int x, int y)
        {
            int org_x = btn_ClearWP.Location.X;
            int org_y = btn_ClearWP.Location.Y;

            btn.SetBounds(org_x + x * btn_ClearWP.Width, org_y + y * btn_ClearWP.Height, btn_ClearWP.Width, btn_ClearWP.Height);
        }
        void SetButtonPositionf(System.Windows.Forms.Button btn, float x, float y)
        {
            int org_x = btn_ClearWP.Location.X;
            int org_y = btn_ClearWP.Location.Y;

            btn.SetBounds(org_x + (int)(x * btn_ClearWP.Width), org_y + (int)(y * btn_ClearWP.Height), btn_ClearWP.Width, btn_ClearWP.Height);
        }
        void SetMapPlane(PointLatLngAlt location, float yaw)
        {

            if (this.map.mapControl.InvokeRequired)
            {
                this.BeginInvoke(new SetMapPlaneDele(SetMapPlane), new Object[] { location ,yaw});
            }
            else {
                this.map.mapControl_SetPlanePosition(location.Lng, location.Lat, location.Alt, yaw);
            }
        }
        void DrawTail(PointLatLngAlt pp) 
        {
           if (CenterList.Count > 100)
                CenterList.RemoveAt(0);
           PointLatLngAlt p;
           p = new PointLatLngAlt(pp.Lat, pp.Lng,pp.Alt);
           lock (CenterList) {this.CenterList.Add(p); this.map.DrawCenterTail(CenterList); }
        }
        void UpdateLabelMode() {
            new Thread((ThreadStart)(delegate()
            {
                label_mode.Invoke((MethodInvoker)delegate()
                {
                    string modestr = this.modestring[indexmodestring];
                    Font f = new System.Drawing.Font("宋体", 0.018f * Width, FontStyle.Bold);
                    Graphics g = this.CreateGraphics();
                    SizeF size = g.MeasureString(modestr, f);
                    this.label_mode.Font = f;
                    this.label_mode.BackColor = Color.Transparent;
                    this.label_mode.Text = modestr;
                });
            })).Start();
        }
        void SetDataView(MAVLink.mavlink_sys_status_t sys_status_t) {
            if (this.textBox_nextpoint.InvokeRequired)
            {
                try
                {
                    this.BeginInvoke(new SetDataViewDele(SetDataView), new Object[] { sys_status_t });
                }
                catch (ObjectDisposedException) {
                    return;
                }
            }
            else {
                this.textBox_way2way.Text = sys_status_t.errors_count4.ToString();

                this.textBox_nextpoint.Text = sys_status_t.battery_remaining.ToString();
                this.textBox_crosstrackdistance.Text = sys_status_t.errors_count3.ToString();//目标高度

                this.myTabCon1.SetTargetHeight(sys_status_t.errors_count3/100.0f);

                ushort status = Program.mav_msg_handler.mavlink_mission_sys_status_t.drop_rate_comm;
                if ((status & AP_DATA_STATUS_WPT_ARRIVED_AND_CLOSE) == 0)
                {
                    this.button_isArrive.BackgroundImage = global::SpaceArrow.Properties.Resources.error;
                }
                else
                {
                    this.button_isArrive.BackgroundImage = global::SpaceArrow.Properties.Resources.right;
                }
                textBox_targetdistance.Text = sys_status_t.errors_comm.ToString();
            }
        }
        void sysplaybackfun() {
            if (this.sysplaybackflag)
            { //已选择文件 开始回放
                if (playback_from.isPlayPause)
                {
                    if (!this.sysplaybackendflag) { 
                        if (this.map.isArm)
                        {
                           this.map.dateArm = this.map.ArmStart;
                            this.map.dataarmNow = DateTime.Now;
                            this.map.mapControl.Invalidate(new Rectangle(10, 10, 150, 80));
                        }
                        else
                        {
                            this.map.ArmStart = DateTime.Now - this.map.dStart;
                        }                    
                    }

                    #region 正常开始回放

                    this.playback_from.SetProcessBar(this.sysstartplaytime, DateTime.Now);


                    if (this.sysplaybackindex.connectindex >= this.sysplaybackdata.listconnect.Count &&
                        this.sysplaybackindex.positionindex >= this.sysplaybackdata.listposition.Count &&
                        this.sysplaybackindex.postureindex >= this.sysplaybackdata.listposture.Count &&
                        this.sysplaybackindex.sysstatusindex >= this.sysplaybackdata.listsysstatus.Count
                        )
                    {
                        this.sysplaybackendflag = true;
                    }
                    else {
                        this.sysplaybackendflag = false;
                    }


                    if (this.sysplaybackdata != null)
                    { //读到的数据不为空
                        if (this.sysplaybackdata.listconnect != null &&
                            this.sysplaybackdata.listconnect.Count != 0 &&
                            this.sysplaybackindex.connectindex < this.sysplaybackdata.listconnect.Count)
                        { //如果连接状态列表有正常数据
                            //判断连接状态
                            if (DateTime.Now - this.sysstartplaytime > //获取从开始播放到当前时间的时间差
                                this.sysplaybackdata.listconnect[this.sysplaybackindex.connectindex].time - this.sysplaybackstartTime)//计算当前播放位置的时间差
                            {
                                this.myTabCon1.SetConnect(this.sysplaybackdata.listconnect[this.sysplaybackindex.connectindex].conn);//更新连接信息
                                this.sysplaybackindex.connectindex++;
                            }
                        }
                        if (this.sysplaybackdata.listposture != null &&
                            this.sysplaybackdata.listposture.Count != 0 &&
                            this.sysplaybackindex.postureindex < this.sysplaybackdata.listposture.Count
                            )
                        { //进行姿态列表数据回放
                            //判断连接状态
                            if (DateTime.Now - this.sysstartplaytime > //获取从开始播放到当前时间的时间差
                                this.sysplaybackdata.listposture[this.sysplaybackindex.postureindex].time - this.sysplaybackstartTime)//计算当前播放位置的时间差
                            {
                                sysplaybackyaw = (float)(this.sysplaybackdata.listposture[this.sysplaybackindex.postureindex].mission_attitude.yaw * 180.0f / System.Math.PI);
                                this.myTabCon1.Set_Roll((float)(this.sysplaybackdata.listposture[this.sysplaybackindex.postureindex].mission_attitude.roll * 180.0f / System.Math.PI));
                                this.myTabCon1.Set_Pitch((float)(this.sysplaybackdata.listposture[this.sysplaybackindex.postureindex].mission_attitude.pitch * 180.0f / System.Math.PI));
                                this.myTabCon1.Set_Yaw(sysplaybackyaw);
                                this.sysplaybackindex.postureindex++;
                            }
                        }

                        if (this.sysplaybackdata.listposition != null &&
                        this.sysplaybackdata.listposition.Count != 0 &&
                        this.sysplaybackindex.positionindex < this.sysplaybackdata.listposition.Count
                        )
                        { //进行位置数据回放
                            if (DateTime.Now - this.sysstartplaytime > //获取从开始播放到当前时间的时间差
                                    this.sysplaybackdata.listposition[this.sysplaybackindex.positionindex].time - this.sysplaybackstartTime)//计算当前播放位置的时间差
                            {
                                PointLatLngAlt latlngaltplayback = new PointLatLngAlt();
                                latlngaltplayback.Alt = this.sysplaybackdata.listposition[this.sysplaybackindex.positionindex].mission_position.relative_alt / 1000.0f;
                                latlngaltplayback.Lat = this.sysplaybackdata.listposition[this.sysplaybackindex.positionindex].mission_position.lat / 10000000.0f;
                                latlngaltplayback.Lng = this.sysplaybackdata.listposition[this.sysplaybackindex.positionindex].mission_position.lon / 10000000.0f;

                                this.myTabCon1.SetAlt(latlngaltplayback.Alt);
                                this.myTabCon1.SetLat(latlngaltplayback.Lat);
                                this.myTabCon1.SetLng(latlngaltplayback.Lng);
                                if (this.sysplaybackdata.listposition[this.sysplaybackindex.positionindex].mission_position.vz == 1)
                                {
                                    this.map.SetHome(new PointLatLng(LatLngAlt.Lat, LatLngAlt.Lng));
                                }
                                this.myTabCon1.Set_Speed(this.sysplaybackdata.listposition[this.sysplaybackindex.positionindex].mission_position.vy);
                                this.myTabCon1.SetSatelliteNumber(this.sysplaybackdata.listposition[this.sysplaybackindex.positionindex].mission_position.vx);
                                double dis = this.map.GetDistance(latlngaltplayback.Lat, latlngaltplayback.Lng);
                                this.myTabCon1.setDistance((int)(dis));
                                SetMapPlane(latlngaltplayback, sysplaybackyaw);
                                if (this.map.mapControl.InvokeRequired)
                                {
                                    DrawtailDele d = new DrawtailDele(DrawTail);
                                    this.BeginInvoke(d, new object[] { latlngaltplayback });
                                }
                                this.sysplaybackindex.positionindex++;
                            }
                        }
                        if (this.sysplaybackdata.listsysstatus != null &&
                        this.sysplaybackdata.listsysstatus.Count != 0 &&
                        this.sysplaybackindex.sysstatusindex < this.sysplaybackdata.listsysstatus.Count)
                        { //如果连接状态列表有正常数据
                            //判断连接状态
                            if (DateTime.Now - this.sysstartplaytime > //获取从开始播放到当前时间的时间差
                                this.sysplaybackdata.listsysstatus[this.sysplaybackindex.sysstatusindex].time - this.sysplaybackstartTime)//计算当前播放位置的时间差
                            {
                                float votage = this.sysplaybackdata.listsysstatus[this.sysplaybackindex.sysstatusindex].mission_sysstatus.voltage_battery * 10.0f;
                                float current = this.sysplaybackdata.listsysstatus[this.sysplaybackindex.sysstatusindex].mission_sysstatus.current_battery * 10.0f;

                                this.myTabCon1.SetVotage(votage);
                                this.myTabCon1.SetCurrent(current);
                            
                                float percent = (votage/1000f * 100f / 4.2f - 500);

                                if (percent < lastpercent) {
                                    lastpercent = percent;
                                    battery.SetBattery((int)(percent));
                                }

                                if (percent > lastpercent && percent - lastpercent > 15) {
                                    lastpercent = percent;
                                    battery.SetBattery((int)(percent));
                                }


                                

                                ushort status = this.sysplaybackdata.listsysstatus[this.sysplaybackindex.sysstatusindex].mission_sysstatus.drop_rate_comm;
                                ushort target_dist = this.sysplaybackdata.listsysstatus[this.sysplaybackindex.sysstatusindex].mission_sysstatus.errors_comm;
                                ushort rssi = this.sysplaybackdata.listsysstatus[this.sysplaybackindex.sysstatusindex].mission_sysstatus.errors_count1;
                                SetDataView(this.sysplaybackdata.listsysstatus[this.sysplaybackindex.sysstatusindex].mission_sysstatus);
                                myTabCon1.SetRSSI(rssi);



                                if ((status & AP_DATA_STATUS_ARMED) == 0)
                                {//螺旋桨
                                    this.myTabCon1.SetArmed(false);
                                    this.map.SetisArm(false);
                                }else{
                                    this.myTabCon1.SetArmed(true);
                                    this.map.SetisArm(true);
                                }
                                if ((status & AP_DATA_STATUS_TAKEOFF) == 0){//自动起飞
                                    this.myTabCon1.SetWarnning(false, "", Color.Yellow);
                                }else{
                                    this.myTabCon1.SetWarnning(true, "一键起飞", Color.Yellow);
                                }
                                if ((status & AP_DATA_STATUS_NEED_HOVER_THR) == 0)
                                {//油门低
                                }
                                else
                                {
                                    this.myTabCon1.SetWarnning(true, "油门低警告", Color.Red);
                                }
                                FLY_MODE_t flymode = (FLY_MODE_t)Program.mav_msg_handler.mavlink_mission_sys_status_t.load;
                                switch (flymode)
                                {
                                    case FLY_MODE_t.FM_ALT:
                                        indexmodestring = 3;
                                        break;
                                    case FLY_MODE_t.FM_CIRCLE:
                                        indexmodestring = 4;
                                        break;
                                    case FLY_MODE_t.FM_HOVER:
                                        indexmodestring = 6;
                                        break;
                                    case FLY_MODE_t.FM_MANUAL:
                                        indexmodestring = 0;
                                        break;
                                    case FLY_MODE_t.FM_RTH:
                                        indexmodestring = 2;
                                        break;
                                    case FLY_MODE_t.FM_STABLIZER:
                                        indexmodestring = 1;
                                        break;
                                    case FLY_MODE_t.FM_WAY_POINT:
                                        indexmodestring = 5;
                                        break;
                                    case FLY_MODE_t.FM_SINGLE_WAY_POINT:
                                        indexmodestring = 7;
                                        break;
                                    default: break;
                                }
                                UpdateLabelMode();

                                this.sysplaybackindex.sysstatusindex++;
                            }
                        }
                    }
                    #endregion
                }
                else
                {

                    this.map.dStart = this.map.dataarmNow - this.map.dateArm;
                    this.map.ArmStart = DateTime.Now - this.map.dStart;

                    #region 暂停
                    sysstartplaytime = DateTime.Now - sysplaybackprogress;
                    #endregion
                }
            }
            else { 
            
            }
        }
        void DataProcess() { 

        if (this.isComOpen)
        {
            #region 串口打开 进行数据处理

            if (this.map.isArm)
                {
                    this.map.dataarmNow = DateTime.Now;
                    this.map.mapControl.Invalidate(new Rectangle(10, 10, 150, 80));
                }
                else {
                    this.map.ArmStart = DateTime.Now - this.map.dStart;
                }
                //发送心跳包
                if (HeartbeatTime.AddSeconds(1) < DateTime.Now)
                {
                    SendHeartBeatToCopter();
                    HeartbeatTime = DateTime.Now;
                }
                #region MAVLINK 消息处理
                if (Program.mav_msg_handler.mission_attitude)
                {
                    ang.yaw = (float)(Program.mav_msg_handler.mavlink_mission_attitude.yaw * 180 / System.Math.PI);
                    ang.roll = (float)(Program.mav_msg_handler.mavlink_mission_attitude.roll * 180.0f / System.Math.PI);
                    ang.pitch = (float)(Program.mav_msg_handler.mavlink_mission_attitude.pitch * 180 / System.Math.PI);
                    this.myTabCon1.Set_Roll(ang.roll);
                    myTabCon1.Set_Pitch(ang.pitch);
                    this.myTabCon1.Set_Yaw(ang.yaw);
                    Program.mav_msg_handler.mission_attitude = false;
                    this.systemdata.listposture.Add(new posture(Program.mav_msg_handler.mavlink_mission_attitude));
                    CalcYuntaiTarget();

                }
                if (Program.mav_msg_handler.mission_sys_status_t) {
                    this.systemdata.listsysstatus.Add(new sysstatus(Program.mav_msg_handler.mavlink_mission_sys_status_t));
                    float votage = Program.mav_msg_handler.mavlink_mission_sys_status_t.voltage_battery*10.0f;   //convert to mv
                    float current = Program.mav_msg_handler.mavlink_mission_sys_status_t.current_battery*10.0f;  //convert to ma
                    this.myTabCon1.SetVotage(votage);
                    this.myTabCon1.SetCurrent(current);
                    float percent = (votage /42.0f - 500);
                    if (percent < lastpercent)
                    {
                        lastpercent = percent;
                        battery.SetBattery((int)(percent));
                    }
                    if (percent > lastpercent && percent - lastpercent > 15)
                    {
                        lastpercent = percent;
                        battery.SetBattery((int)(percent));
                    }
                    ushort status = Program.mav_msg_handler.mavlink_mission_sys_status_t.drop_rate_comm;
                    ushort target_dist = Program.mav_msg_handler.mavlink_mission_sys_status_t.errors_comm;
                    ushort rssi = Program.mav_msg_handler.mavlink_mission_sys_status_t.errors_count1;
                    SetDataView(Program.mav_msg_handler.mavlink_mission_sys_status_t);
                    myTabCon1.SetRSSI(rssi);
                    if ((status & AP_DATA_STATUS_ARMED) == 0){//螺旋桨
                        this.myTabCon1.SetArmed(false);
                        this.map.SetisArm(false);
                        isArmStart = false;
                    } else {
                        this.myTabCon1.SetArmed(true);
                        this.map.SetisArm(true);
                        isArmStart = true;
                    }

                    if (percent > 30) { //如果电量大于百分之30 电量充足
                        isOperationWhenPowerlow = false;
                    }
                    if (percent < 20 && isArmStart)//检测到飞行器螺旋桨转动，并且电量少于20%
                    {
                        if ((systembatterywarning == null || systembatterywarning.IsDisposed) && !isOperationWhenPowerlow)
                        {// 
                            BackGroundOperation = 2;

                            System.ComponentModel.BackgroundWorker m_BackgroundWorker = new System.ComponentModel.BackgroundWorker();
                            InitializeBackgoundWorker(m_BackgroundWorker);
                            m_BackgroundWorker.RunWorkerAsync();
                        }
                    }
                    if ((status & AP_DATA_STATUS_TAKEOFF) == 0){//自动起飞
                        this.myTabCon1.SetWarnning(false, "",Color.Yellow);
                    }else{
                        this.myTabCon1.SetWarnning(true, "一键起飞", Color.Yellow);
                        Program.WriteLog("收到报告：正在起飞...");
                    }
                    if ((status & AP_DATA_STATUS_NEED_HOVER_THR) == 0){//油门低
                    }else {
                        this.myTabCon1.SetWarnning(true, "油门低警告", Color.Red);
                        Program.WriteLog("收到报告:油门低警告...");
                    }
                    FLY_MODE_t flymode = (FLY_MODE_t)Program.mav_msg_handler.mavlink_mission_sys_status_t.load;
                    switch (flymode){
                        case FLY_MODE_t.FM_ALT: 
                            indexmodestring = 3; 
                            break;
                        case FLY_MODE_t.FM_CIRCLE: 
                            indexmodestring = 4; 
                            break;
                        case FLY_MODE_t.FM_HOVER: 
                            indexmodestring = 6; 
                            break;
                        case FLY_MODE_t.FM_MANUAL: 
                            indexmodestring = 0; 
                            break;
                        case FLY_MODE_t.FM_RTH:
                            this.myTabCon1.SetWarnning(true, "返航", Color.Yellow);
                            indexmodestring = 2; 
                            break;
                        case FLY_MODE_t.FM_STABLIZER: 
                            indexmodestring = 1; 
                            break;
                        case FLY_MODE_t.FM_WAY_POINT:
                            indexmodestring=5;
                            break;
                        case FLY_MODE_t.FM_SINGLE_WAY_POINT: 
                            indexmodestring = 7; 
                            break;
                        default: break;
                    }
                    if (oldmode != flymode) {
                        Program.WriteLog("切换进入了:"+modestring[indexmodestring]+"模式");
                    }
                    oldmode = flymode;
                    UpdateLabelMode();
                    Program.mav_msg_handler.mission_sys_status_t = false;
                }
                if (Program.mav_msg_handler.mission_global_position_int) {
                    this.systemdata.listposition.Add(new position(Program.mav_msg_handler.mavlink_mission_global_position_int));
                    isGPSarrived = true;
                    LatLngAlt.Alt = Program.mav_msg_handler.mavlink_mission_global_position_int.relative_alt / 1000.0f;
                    LatLngAlt.Lat = Program.mav_msg_handler.mavlink_mission_global_position_int.lat / 10000000.0f;
                    LatLngAlt.Lng = Program.mav_msg_handler.mavlink_mission_global_position_int.lon / 10000000.0f;
                    this.myTabCon1.SetAlt(LatLngAlt.Alt);
                    this.myTabCon1.SetLat(LatLngAlt.Lat);
                    this.myTabCon1.SetLng(LatLngAlt.Lng);
                    if (Program.mav_msg_handler.mavlink_mission_global_position_int.vz == 1){
                        this.map.SetHome(new PointLatLng(LatLngAlt.Lat,LatLngAlt.Lng));
                    }
                    this.myTabCon1.Set_Speed(this.map.multipleflag*Program.mav_msg_handler.mavlink_mission_global_position_int.vy);

                    CalcYuntaiTarget();

                    satellitenumber = Program.mav_msg_handler.mavlink_mission_global_position_int.vx;

                    this.myTabCon1.SetSatelliteNumber(Program.mav_msg_handler.mavlink_mission_global_position_int.vx);
                    double dis = this.map.GetDistance(LatLngAlt.Lat, LatLngAlt.Lng);
                    this.myTabCon1.setDistance((int)(dis));
                    SetMapPlane(LatLngAlt,ang.yaw);
                    Program.mav_msg_handler.mission_global_position_int = false;
                }
                if (Program.mav_msg_handler.heart_beat_count > 2){
                    this.myTabCon1.SetConnect(true);
                    if (!connectstate)
                        this.systemdata.listconnect.Add(new connect(true));
                    connectstate = true;
                }
                #endregion
                if (isGPSarrived) {
                    if (planeCenterPoint.AddMilliseconds(500) < DateTime.Now) {
                        if (this.map.mapControl.InvokeRequired) {
                            DrawtailDele d = new DrawtailDele(DrawTail);
                            this.BeginInvoke(d,new object[]{LatLngAlt} );
                        }
                        planeCenterPoint = DateTime.Now;
                    }
                }
            #endregion

                if (startTime.AddSeconds(3) < DateTime.Now || isComOpen == false)
                {
                    this.myTabCon1.SetConnect(false);
                    this.map.SetisArm(false);
                    Program.mav_msg_handler.heart_beat_count = 0;
                    if (systemdata != null && connectstate == true)
                        this.systemdata.listconnect.Add(new connect(false));
                    connectstate = false;
                }
            }
          else
            {
                sysplaybackfun();
            }
            GC.Collect();
        }
        #endregion
        #region  串口连接配置
        private void GetPara()
        {
            if (serialize == null)
            {
                serialize = new SerializationClass();
            }
            serialize.ComPortName = this.serial_form.comboBox_com.Text;
            serialize.ComBaud = this.serial_form.comboBox_baud.Text;
        }
        private bool SerialPortConnect()
        {
            if (playback_from != null) {
                MessageBox.Show("正在回放,不能打开连接!!!");
                return false;
            }

            string message_str = "";
            if (serial_form == null || serial_form.IsDisposed )
            {
                if (serialize == null)
                {
                    MessageBox.Show("第一次使用,请设置连接..");
                    return false;
                }
                else
                {

                }

            }
            else
            {
                if (this.serial_form.com != null) { 
                    serial_com_str = this.serial_form.com;
                  //  this.serial_dataBits = this.serial_form.comboBox_databits.Text;
                  //  this.serial_stopBits = this.serial_form.comboBox4_stopbit.Text;
                    this.serial_baud = this.serial_form.comboBox_baud.Text;
                  //  this.serial_verify = this.serial_form.comboBox5_verify.Text;                
                }


            }
            if (serial == null)
            {
                serial = new SerialPort();
            }

            if (!isComOpen)
            {
                try
                {
                this.serial.BaudRate = int.Parse(this.serial_baud);


                }
                catch {
                    MessageBox.Show("第一次使用串口连接，请设置参数...");
                    return false;
                }

                if (this.serial_com_str == "") {
                    MessageBox.Show("当前不存在可用串口...");
                    return false;
                }

                this.serial.PortName = this.serial_com_str;

                message_str += ("端口：" + this.serial.PortName + "波特率:" + this.serial.BaudRate.ToString());

                //数据位
                //this.serial.DataBits = int.Parse(this.serial_dataBits);
                serial.DataBits = 8;
                serial.StopBits = System.IO.Ports.StopBits.One;
                //message_str += "数据位：" + this.serial_dataBits.ToString();
                /*
                switch (int.Parse(this.serial_stopBits))
                {
                    case 1:
                        this.serial.StopBits = System.IO.Ports.StopBits.One;
                        message_str += "停止位:1";
                        break;
                    case 2:
                        this.serial.StopBits = System.IO.Ports.StopBits.Two;
                        message_str += "停止位:2";

                        break;
                    default:
                        return false;
                }
                 */
                //校验位
                serial.Parity = System.IO.Ports.Parity.None;
                /*
                switch (this.serial_verify)
                {
                    case "无":
                        serial.Parity = System.IO.Ports.Parity.None;//无校验
                        message_str += " 无校验";
                        break;
                    case "奇":
                        serial.Parity = System.IO.Ports.Parity.Odd;//奇校验
                        message_str += " 奇校验";
                        break;
                    case "偶":
                        serial.Parity = System.IO.Ports.Parity.Even;//偶校验
                        message_str += " 偶校验";
                        break;
                    case "M":
                        serial.Parity = System.IO.Ports.Parity.Mark;//1
                        message_str += " 校验位保留为1";
                        break;
                    case "S":
                        serial.Parity = System.IO.Ports.Parity.Space;//0
                        message_str += " 校验位保留为0";
                        break;
                    default://出错
                        return false;
                }
                 */
                //打开串口
                try
               {
                    serial.Open();

                    Program.WriteLog(message_str+"串口打开成功...");
               }
               catch (System.UnauthorizedAccessException e)
               {
                   Program.WriteLog(e.ToString());
                   MessageBox.Show(e.ToString());

                   return false;
               }catch(System.IO.IOException e){
                   Program.WriteLog(e.ToString());
                   MessageBox.Show( e.ToString());
                   return false;
               }
                this.serial.DataReceived += Com_DataReceived;
                this.comboBox_connecttype.Enabled = false;


                StartConnect();

                message_str += " 串口连接打开成功";

                if (systemdata == null)
                    systemdata = new SerializeDataClass();
                else {
                    systemdata.Clear();
                }

                myImageButton_Connect.SetImage( "1\\1");
            }
            else
            {
                isGPSarrived = false;

                myImageButton_Connect.SetImage( "1\\2");
                this.serial.Close();

                Program.mav_msg_handler.heart_beat_count = 0;
                this.serial.DataReceived -= Com_DataReceived;
                this.map.SetisArm(false);
                this.myTabCon1.SetConnect(false);
                this.comboBox_connecttype.Enabled = true;
                message_str += "已断开串口连接";

                string filename1 = this.datafilepath + "\\FlightData" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".txt";

                if (!Directory.Exists(datafilepath))
                {
                    Directory.CreateDirectory(datafilepath);
                }
                XMLSerializer.Serialize<SerializeDataClass>(this.systemdata, filename1);
                systemdata = null;

                Program.WriteLog(message_str);
            }
            isComOpen = !isComOpen;
            this.ListBoxAddMessage(message_str);
            return true;
        }
        void ConnectPane()
        {
            if (mavlink_wps == null)
            {
                mavlink_wps = new MAVLink_wps(this.serial,this);
                mavlink_wps.ListMessage = ListBoxAddMessage;

            }
            else
            {
                mavlink_wps.serial = this.serial;
            }

            mavlink_wps.getParamListBG();

        }
        void Com_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            byte[] buf = new byte[128];
            int r = 0;
            try
            {
                r = this.serial.Read(buf, 0, buf.Length);
            }
            catch(Exception )
            {
               // MessageBox.Show(ee.ToString());

                return;
            }

                Array.Resize(ref dataBuffer, dataBuffer.Length + r);//重新设置缓冲区大小
                Array.ConstrainedCopy(buf, 0, dataBuffer, dataBuffer.Length - r, r);//接收到的数据添加到原有数据后面   
                do
                {
                    buf = ComReadPacket();
                    

                    if (buf != null)
                    {
                        MAVLink.MAVLINK_MSG_ID message_id = (MAVLink.MAVLINK_MSG_ID)buf[5];

                        startTime = DateTime.Now;

                        this.myTabCon1.PlusOneCount();

                        //排除数传包对心跳检测的干扰
                        if (message_id != MAVLink.MAVLINK_MSG_ID.RADIO && message_id !=MAVLink.MAVLINK_MSG_ID.RADIO_STATUS) { 
                            Program.mav_msg_handler.heart_beat_count++;                        
                        }

                        //
                        switch (message_id)
                        {
                            case MAVLink.MAVLINK_MSG_ID.HEARTBEAT:
                                break;
                            case MAVLink.MAVLINK_MSG_ID.MISSION_COUNT:
                                MAVLink.mavlink_mission_count_t msg_mission_count = buf.ByteArrayToStructure<MAVLink.mavlink_mission_count_t>();
                                Program.mav_msg_handler.mission_count = msg_mission_count.count;
                                Program.mav_msg_handler.mission_count_update = true;
                                break;
                            case MAVLink.MAVLINK_MSG_ID.MISSION_ITEM:
                                MAVLink.mavlink_mission_item_t msg_mission_item = buf.ByteArrayToStructure<MAVLink.mavlink_mission_item_t>();
                                try {
                                    Program.mav_msg_handler.mission_items.Add(msg_mission_item.seq, msg_mission_item);
                                }
                               catch {
                                }
                                break;
                            case MAVLink.MAVLINK_MSG_ID.SYS_STATUS:
                                Program.mav_msg_handler.mavlink_mission_sys_status_t = buf.ByteArrayToStructure<MAVLink.mavlink_sys_status_t>();
                                Program.mav_msg_handler.mission_sys_status_t = true;
                                break;
                            case MAVLink.MAVLINK_MSG_ID.MISSION_REQUEST:
                                MAVLink.mavlink_mission_request_t request = buf.ByteArrayToStructure<MAVLink.mavlink_mission_request_t>();
                                Program.mav_msg_handler.mavlink_mission_request = request;
                                Program.mav_msg_handler.mission_request = true;
                               // MAV_CMD_NAV_LOITER_TIME
                                break;
                            case MAVLink.MAVLINK_MSG_ID.MISSION_ACK:
                                MAVLink.mavlink_mission_ack_t ack = buf.ByteArrayToStructure<MAVLink.mavlink_mission_ack_t>();
                                Program.mav_msg_handler.mavlink_mission_ack = ack;
                                Program.mav_msg_handler.mission_ack = true;
                                break;
                            case MAVLink.MAVLINK_MSG_ID.ATTITUDE:
                                MAVLink.mavlink_attitude_t att = buf.ByteArrayToStructure<MAVLink.mavlink_attitude_t>(6);
                                Program.mav_msg_handler.mavlink_mission_attitude = att;
                                Program.mav_msg_handler.mission_attitude = true;
                                break;
                            case MAVLink.MAVLINK_MSG_ID.GLOBAL_POSITION_INT:
                                Program.mav_msg_handler.mavlink_mission_global_position_int = buf.ByteArrayToStructure<MAVLink.mavlink_global_position_int_t>(6);
                                Program.mav_msg_handler.mission_global_position_int = true;
                                break;
                            case MAVLink.MAVLINK_MSG_ID.PARAM_VALUE:
                                Program.mav_msg_handler.mavlink_mission_param_value_t = buf.ByteArrayToStructure<MAVLink.mavlink_param_value_t>(6);
                                break;
                            case MAVLink.MAVLINK_MSG_ID.COMMAND_ACK:
                                //Program.mav_msg_handler.mavlink_mission_command_ack_t = buf.ByteArrayToStructure<MAVLink.mavlink_command_ack_t>(6);
                                //Program.mav_msg_handler.mission_command_ack_t = true;
                                MAVLink.mavlink_command_ack_t cmd_ack = buf.ByteArrayToStructure<MAVLink.mavlink_command_ack_t>(6);
                                string cmd_name="";
                                switch (cmd_ack.command)
                                {
                                    case (ushort)MAVLink.MAV_CMD.LOITER_TIME:
                                        cmd_name = "发送悬停命令";
                                        if (cmd_ack.result == 1)
                                        {
                                            this.map.SystemModeID = 0xFF;
                                          //  Program.WriteLog("悬停模式进入失败...");
                                        }
                                        else { 
                                            this.myImageButton_Iremote.SetColor(Color.LightGreen);
                                         //   Program.WriteLog("悬停模式进入成功...");
                                        
                                        }
                                        break;
                                    case (ushort)MAVLink.MAV_CMD.MISSION_START:
                                        cmd_name = "发送执行航线命令";
                                        break;
                                    case (ushort)MAVLink.MAV_CMD.TAKEOFF:
                                        cmd_name = "发送起飞命令";
    
                                        break;
                                    case (ushort)MAVLink.MAV_CMD.RETURN_TO_LAUNCH:
                                        cmd_name = "发送返航命令";
                                        //if (cmd_ack.result == 0)
                                        //{
                                        //    indexmodestring = 2;
                                        //    UpdateLabelMode();
                                        //}

                                        break;
                                    case (ushort)MAVLink.MAV_CMD.DO_SET_MODE:
                                        cmd_name = "设置模式";
                                        if (cmd_ack.result == 0x80) {
                                            lock (myImageButton_Singlepoint) { 
                                                this.myImageButton_Singlepoint.SetColor(Color.LightGreen);
                                           
                                            }
                                            cmd_ack.result = 0;
                                         //   this.map.SystemModeID = 0;
                                        }
                                        else if (cmd_ack.result == 0x81)
                                        {
                                            lock (myImageButton_Multipoint) { 
                                            this.myImageButton_Multipoint.SetColor(Color.LightGreen);
                                            }
                                            cmd_ack.result = 0;
                                         //   this.map.SystemModeID = 1;
                                        }
                                        else {
                                            cmd_ack.result = 1;
                                         //   this.map.SystemModeID = 0xFF;
                                        } 
                                        break;
                                    case (ushort)MAVLink.MAV_CMD.DO_SET_HOME:
                                        cmd_name = "设置起飞坐标";
                                        break;
                                    case (ushort)MAVLink.MAV_CMD.DO_MOUNT_CONTROL:
                                        cmd_name = "设置云台俯仰";
                                        break;
                                    case (ushort)MAVLink.MAV_CMD.DO_DIGICAM_CONTROL:
                                        cmd_name = "相机操作命令";
                                        break;
                                    case 40000:
                                        cmd_name = "校准命令";
                                        break;
                                    default:
                                        cmd_name = "接收到未知命令的返回";
                                        break;
                                }

                                if (cmd_ack.result == 0)
                                {
                                    cmd_name = cmd_name + " 完成!";
                                }
                                else
                                {
                                    cmd_name = cmd_name + " 错误 " + cmd_ack.result.ToString();
                                }

                                this.ListBoxAddMessage(cmd_name);
                                Program.WriteLog(cmd_name);
                                break;
                            default:
                                break;
                        }
                    }
                } while (buf != null);
  
        }
        public byte[] ComReadPacket()
        {
            int packet_length = 0;
            int index = 0;
            //循环找到数据包头
            if (dataBuffer.Length > 0)
            {
                if (dataBuffer[0] != 0xFE)
                {
                }
            }

            while (index+1 < dataBuffer.Length && dataBuffer.Length > 1)
            {
                if (dataBuffer[index] == 0xFE)
                {
                    packet_length = dataBuffer[index + 1] + 6 + 2;// data + header + checksum// - U - length
                    int available_data_len = dataBuffer.Length - index;
                    Array.ConstrainedCopy(dataBuffer, index, dataBuffer, 0, available_data_len);
                    Array.Resize(ref dataBuffer, available_data_len);
                    if (packet_length > available_data_len)
                    {
                       // this.ListBoxAddMessage("Packet length is not enough...."+index.ToString());
                        return null;
                    }
                    break;
                }
                else
                {
                    index++;
                }
            }

            if (index +1== dataBuffer.Length || dataBuffer.Length < 2)
            {
                return null;
            }
            if (packet_length < 6) {
                return null;
            }

            //循环找寻数据包体
            byte[] buffer = new byte[packet_length];
            Array.ConstrainedCopy(dataBuffer, 0, buffer, 0, packet_length);
            ushort crc = MAVLink.MavlinkCRC.crc_calculate(buffer, buffer[1]+6);
            crc = MAVLink.MavlinkCRC.crc_accumulate(MAVLink.MAVLINK_MESSAGE_CRCS[buffer[5]], crc);
            byte a = (byte)(crc >> 8);
            byte b = (byte)(crc & 0xff);
            if (buffer.Length < 5 || buffer[buffer.Length - 1] != (crc >> 8) || buffer[buffer.Length - 2] != (crc & 0xff))
            {
                Array.ConstrainedCopy(dataBuffer, 1, dataBuffer, 0, dataBuffer.Length-1);
                Array.Resize(ref dataBuffer, dataBuffer.Length-1);
                ListBoxAddMessage("CRC is not correct!!!");
                return null;
            }
            //找到数据包之后

            Array.Resize(ref data_packet, packet_length);
            Array.ConstrainedCopy(dataBuffer, 0, this.data_packet, 0, packet_length);
            Array.ConstrainedCopy(dataBuffer, packet_length, this.dataBuffer, 0, this.dataBuffer.Length - packet_length);
            Array.Resize(ref dataBuffer, dataBuffer.Length - packet_length);

            return this.data_packet;
        }
        #endregion
        private void button_Connect_Click(object sender, EventArgs e)
        {

        }
        #region 航点操作
        private void InitializeBackgoundWorker(System.ComponentModel.BackgroundWorker m_BackgroundWorker)
        {
          //  m_BackgroundWorker = new System.ComponentModel.BackgroundWorker(); // 实例化后台对象
            //添加委托   
            m_BackgroundWorker.DoWork += m_BackgroundWorker_DoWork;
            //委托完成后执行的事件   
            m_BackgroundWorker.RunWorkerCompleted += m_BackgroundWorker_RunWorkerCompleted;
            //执行委托过程中报告进度信息   
            m_BackgroundWorker.ProgressChanged += m_BackgroundWorker_ProgressChanged;
        }
        void m_BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }
        void m_BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (BackGroundOperation==1)
            { 
                PointLatLngAlt[] p = mavlink_wps.drawPoints.ToArray();
                if (p.Length > 0)
                {
                    Array.ConstrainedCopy(p, 1, p, 0, p.Length - 1);
                    Array.Resize<PointLatLngAlt>(ref p, p.Length - 1);
                    map.mapDrawGetWPs(p.ToList<PointLatLngAlt>());
                }
            }            
        }
        void m_BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (BackGroundOperation==1)
            {
                mavlink_wps.getWPs();
            }
            else if (BackGroundOperation==0)
                mavlink_wps.sendWPS(drawPoint);
            else if (BackGroundOperation==2)
            {
                systembatterywarning = new BatteryWarning();

                systembatterywarning.StartPosition = FormStartPosition.Manual;
                systembatterywarning.Location = new System.Drawing.Point((this.Width - systembatterywarning.Width)/2, (this.Height-systembatterywarning.Height)/2);
                systembatterywarning.TopMost = true;
                systembatterywarning.Show();
                DateTime start = DateTime.Now;

                while (systembatterywarning != null || systembatterywarning.IsDisposed) {
                    systembatterywarning.SetTimeNum(9-(DateTime.Now-start).Seconds);
                    if (systembatterywarning.isClickFalse)
                    {
                        batterywarning();
                        isOperationWhenPowerlow = true;
                        break;
                    }
                    if (systembatterywarning.isClickRight) {
                        batterywarning();
                        this.mavlink_wps.SendToLand();
                        isOperationWhenPowerlow = true;
                        break;
                    }
                    if (start.AddSeconds(10) <= DateTime.Now) {
                        batterywarning();
                        isOperationWhenPowerlow = true;
                        this.mavlink_wps.SendToLand();
                        break;
                    }
                    Application.DoEvents();
                }

            }
        }
        void batterywarning(){
            if (this.systembatterywarning.InvokeRequired)
            {
                this.Invoke(new NoParametersNoReturnDele(batterywarning));
            }
            else {
                systembatterywarning.Close();
            }
        }
        private void btn_ClearWP_Click(object sender, EventArgs e)
        {
            map.mapClearWP();
            this.ListBoxAddMessage("已清除地图上所有航点坐标");
            Program.WriteLog("点击了清除航点按钮，已清除所有航点信息...");
        }
        public void UploadWayPoint() {
            if (!this.isComOpen)
            {
                MessageBox.Show("当前无连接");
                return;
            }
            if (map.drawingPoints.Count > 0)
            {
                this.ListBoxAddMessage("正在发送航点....");

                Program.WriteLog("点击了上传航点按钮，正准备发送航点...");

                drawPoint = map.getWPsFromDataView();
                BackGroundOperation = 0;
                System.ComponentModel.BackgroundWorker m_BackgroundWorker = new System.ComponentModel.BackgroundWorker();
                InitializeBackgoundWorker(m_BackgroundWorker);
                m_BackgroundWorker.RunWorkerAsync();
            }
            else
            {
                MessageBox.Show("航点数量必须大于1个");
            }
        }
        public void DownloadWayPoint() {
            if (!this.isComOpen)
            {
                MessageBox.Show("当前无连接");
                return;
            }
            Program.WriteLog("点击了下载航点按钮，正准备下载航点...");

            BackGroundOperation = 1;

            System.ComponentModel.BackgroundWorker m_BackgroundWorker = new System.ComponentModel.BackgroundWorker();
            InitializeBackgoundWorker(m_BackgroundWorker);
            m_BackgroundWorker.RunWorkerAsync();
        }
        private void btn_SendWP_Click(object sender, EventArgs e)
        {
            UploadWayPoint();
        }
        private void btn_getWP_Click(object sender, EventArgs e)
        {
            DownloadWayPoint();
        }
        public void getwpnum(int num)
        {
            this.map.InsertWP(MapClickPoint, num);

            if (this.routeplanningform != null) {
                if (this.isInplanning) return;

               // this.routeplanningform.SetWayPointPara(num,(int) MapClickPoint.Alt, false);

                if (point_K.Count >= 3)
                {
                    PointLatLngAlt p1;
                    PointLatLngAlt p2;
                    PointLatLngAlt p3;

                    if (num - 1 == 0)//第一个点
                    {
                        p1 = this.map.drawingPoints[this.map.drawingPoints.Count - 1];
                        p2 = this.map.drawingPoints[0];
                        p3 = this.map.drawingPoints[1];
                    }
                    else if (num == this.map.drawingPoints.Count)//最后一个点
                    {
                        p1 = this.map.drawingPoints[num - 2];
                        p2 = this.map.drawingPoints[num - 1];
                        p3 = this.map.drawingPoints[0];

                    }
                    else
                    {
                        p1 = this.map.drawingPoints[num - 2];
                        p2 = this.map.drawingPoints[num - 1];
                        p3 = this.map.drawingPoints[num];
                    }


                    double x0 = p1.Lng;
                    double y0 = p1.Lat;
                    double x1 = p2.Lng;
                    double y1 = p2.Lat;
                    double k1 = (y0 - y1) / (x0 - x1);
                    double b1 = (x1 * y0 - x0 * y1) / (x1 - x0);

                    double x10 = p2.Lng;
                    double y10 = p2.Lat;
                    double x11 = p3.Lng;
                    double y11 = p3.Lat;
                    double k2 = (y10 - y11) / (x10 - x11);
                    double b2 = (x11 * y10 - x10 * y11) / (x11 - x10);

                    if (num - 1 == 0)
                    {
                        point_K.RemoveAt(this.point_K.Count - 1);
                        point_B.RemoveAt(this.point_B.Count - 1);

                        point_K.Add(k1);
                        point_K.Insert(0, k2);

                        point_B.Add(b1);
                        point_B.Insert(0, b2);

                        this.routeplanningform.SetWayPointPara(1, (int)MapClickPoint.Alt, false);
                        this.routeplanningform.UpdateNumwaypoints(0);

                        double ang = (Math.Atan(k1) / Math.PI * 180);
                        string str = "";
                        this.updateanglestr(ref str, ref ang, this.map.drawingPoints.Count-1);

                        double ang1 = (Math.Atan(k2) / Math.PI * 180);
                        string str1 = "";
                        this.updateanglestr(ref str1, ref ang1, 0);
                        this.routeplanningform.DeleteLineRow(this.point_K.Count - 2);

                        this.routeplanningform.SetLinePointPara(this.point_K.Count-1 , (int)ang, str, "1,2", false);
                        this.routeplanningform.SetLinePointPara(1, (int)ang1, str1, "2,3", false);
                        this.routeplanningform.UpdateNumendpoints(0);

                    }
                    else {
                        point_K.RemoveAt(num - 2);
                        point_B.RemoveAt(num - 2);

                        point_K.Insert(num - 1, k2);
                        point_K.Insert(num - 1, k1);
                        point_B.Insert(num - 1, b2);
                        point_B.Insert(num - 1, b1);

                        this.routeplanningform.SetWayPointPara(num, (int)MapClickPoint.Alt, false);
                        this.routeplanningform.UpdateNumwaypoints(num-1);

                        double ang = (Math.Atan(k1) / Math.PI * 180);
                        string str = "";
                        this.updateanglestr(ref str, ref ang, num-2);

                        double ang1 = (Math.Atan(k2) / Math.PI * 180);
                        string str1 = "";
                        this.updateanglestr(ref str1, ref ang1, num-1);
                        this.routeplanningform.DeleteLineRow(num - 2);


                        this.routeplanningform.SetLinePointPara(num-1, (int)ang1, str1, "mmmmm", false);

                        this.routeplanningform.SetLinePointPara(num-1, (int)ang, str, "nnnn", false);

                        this.routeplanningform.UpdateNumendpoints(num-2);
                    
                    }

                }



                //this.UpdatePlanForm();
            }
        }
        #endregion 
        #region ToolStripMenuItcem
        private void 删除航点ToolStripMenuItem_Click(object sender, EventArgs e)
        {
          //如果打开了，规划航点对话框
            if (this.routeplanningform != null)
            {

                if (this.map.drawingPoints.Contains(MarkerClickPoint))
                {
                    int n = this.map.drawingPoints.IndexOf(MarkerClickPoint);

                    PointLatLngAlt p1=new PointLatLngAlt();
                    PointLatLngAlt p2=new PointLatLngAlt();

                    if (this.map.drawingPoints.Count > 3)
                    {
                        if (!this.isInplanning) { 
                            if (n == 0)
                            {
                                p1 = this.map.drawingPoints[this.map.drawingPoints.Count - 1];
                                p2 = this.map.drawingPoints[1];

                                this.routeplanningform.DeleteLineRow(0);
                                this.routeplanningform.DeleteLineRow(this.point_K.Count-2);

                                this.point_K.RemoveAt(0);
                                this.point_K.RemoveAt(this.point_K.Count - 1);

                                this.point_B.RemoveAt(0);
                                this.point_B.RemoveAt(this.point_K.Count - 1);
                            }
                            else if (n == this.map.drawingPoints.Count - 1)
                            {
                                p1 = this.map.drawingPoints[this.map.drawingPoints.Count - 2];
                                p2 = this.map.drawingPoints[0];

                                this.routeplanningform.DeleteLineRow(this.point_K.Count-2);
                                this.routeplanningform.DeleteLineRow(this.point_K.Count-2);

                                this.point_K.RemoveAt(this.point_K.Count - 2);
                                this.point_K.RemoveAt(this.point_K.Count - 2);

                                this.point_B.RemoveAt(this.point_K.Count - 2);
                                this.point_B.RemoveAt(this.point_K.Count - 2);
                            }
                            else {
                                p1 = this.map.drawingPoints[n - 1];
                                p2 = this.map.drawingPoints[n + 1];

                                this.routeplanningform.DeleteLineRow(n);
                                this.routeplanningform.DeleteLineRow(n-1);

                                this.point_K.RemoveAt(n);
                                this.point_K.RemoveAt(n-1);

                                this.point_B.RemoveAt(n);
                                this.point_B.RemoveAt(n-1);
                            }                        
                        }
                    }
                    this.map.DeleteWP(MarkerClickPoint);


                if (this.isInplanning) return;
                this.routeplanningform.DeleWayRow(n);
                this.routeplanningform.UpdateNumwaypoints(n);

                if (this.map.drawingPoints.Count >= 3)
                {
                    double x0 = p1.Lng;
                    double y0 = p1.Lat;
                    double x1 = p2.Lng;
                    double y1 = p2.Lat;
                    double k1 = (y0 - y1) / (x0 - x1);
                    double b1 = (x1 * y0 - x0 * y1) / (x1 - x0);

                    point_K.Insert(n, k1);
                    point_B.Insert(n,b1);

                    double ang1 = (Math.Atan(k1) / Math.PI * 180);
                    string str1 = "";
                    this.updateanglestr(ref str1, ref ang1, (n - 1)>=0 ? n-1 : this.map.drawingPoints.Count-1);
                    this.routeplanningform.SetLinePointPara(((n - 1) >= 0 ? n - 1 : this.map.drawingPoints.Count - 1)+1,
                        (int)ang1, str1, (n ).ToString() + "," + ((n+1).ToString()), false);

                    this.routeplanningform.UpdateNumendpoints(n);
                }
                else {
                    this.routeplanningform.ClearLinePointDataView();
                }
                }
                else
                {
                    return;
                }
           
            }
            else
            {
                this.map.DeleteWP(MarkerClickPoint);
            }


            //this.map.DeleteWP(MarkerClickPoint);
  

        }
      
        private void 插入航点ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertWPNum insertwp = new InsertWPNum();
            insertwp.GetNum = getwpnum;
            insertwp.count = this.map.drawingPoints.Count;
            insertwp.TopMost = true;
            insertwp.Show();

        }
        private void 清除所有航点ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.map.mapClearWP();
            if (this.routeplanningform != null) {
                this.UpdatePlanForm();
            }

        }
        private void 直到此处ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!this.isComOpen)
            {
               MessageBox.Show("当前无连接");
               return;
            }
            if (this.map.SystemModeID != 0)
            {
                MessageBox.Show("当前不在单航点模式");
                return;
            }
            InsertWPNum insert = new InsertWPNum();
            insert.flag = 1;
            insert.textBox_wp.Text = "100";

            insert.label1.Text = "请设置高度:";
            insert.label2.Text = "单位:m";
            insert.GetNum = GetNum;
            insert.ShowDialog();
        }
      
        void GetNum(int n) {
            this.map.MouseClickPoint.Alt = n;
           this.map.DrawTargetPoint(this.map.MouseClickPoint);
            this.mavlink_wps.GoToTargetPoint(this.map.MouseClickPoint);
        }

        private void 飞行至此ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!this.isComOpen)
            {
                MessageBox.Show("当前无连接");
                return;
            }
            if (this.map.SystemModeID != 0)
            {
                MessageBox.Show("当前不在单航点模式");
                return;
            }


            InsertWPNum insert = new InsertWPNum();
            insert.flag = 1;
            insert.textBox_wp.Text = "30";
            insert.label1.Text = "请设置高度:";
            insert.label2.Text = "单位:m";
            insert.GetNum = GetNum;
            insert.ShowDialog();
        }
        private void 飞行至此ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (!this.isComOpen)
            {
                MessageBox.Show("当前无连接");
                return;
            }
            if (this.map.SystemModeID != 0)
            {
                MessageBox.Show("当前不在单航点模式");
                return;
            }
            InsertWPNum insert = new InsertWPNum();
            insert.flag = 1;
            insert.textBox_wp.Text = "100";

            insert.label1.Text = "请设置高度:";
            insert.label2.Text = "单位:m";
            insert.GetNum = GetNum;
            insert.ShowDialog();
        }
        private void 清除消息ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.listBox_message.Items.Clear();
        }
        public void addBookMark(PointLatLngAlt p)
        {
            int count = this.BookMark.Count + 1;
            if (BookMark == null)
            {
                BookMark = new List<PointLatLngAlt>();
            }

            this.map.DrawBookMark(p, count);

            BookMark.Add(p);

            addDataView_BookMark(count, new PointLatLngAlt(p));

        }

        double totaldistance = 0;

        private void 添加书签ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.addBookMark(new PointLatLngAlt(MapClickPoint));
        }
        private void 添加书签ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.addBookMark(new PointLatLngAlt(MapClickPoint));
        }
        private void ClearBookMark()
        {
            this.map.ClearBookMark();
            this.dataView_bookMark.Rows.Clear();
            this.BookMark.Clear();

            this.totaldistance = 0;
            this.label_totaldistance.Text = "总里程数:" + totaldistance.ToString("0.000m");

        }
        private void 清除书签ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearBookMark();
        }
        private void 清除标签ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearBookMark();
        }
        private void 删除书签ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.BookMark.Contains(this.MarkerClickPoint))
            {
                this.BookMark.Remove(this.MarkerClickPoint);
                this.map.ClearBookMark();
                this.dataView_bookMark.Rows.Clear();
                int i = 1;
                foreach (PointLatLngAlt p in this.BookMark)
                {
                    if (this.dataView_bookMark != null)
                    {
                        this.addDataView_BookMark(i, p);
                    }
                    this.map.DrawBookMark(p, i);
                    i++;
                }
            }
            else
            {
                MessageBox.Show("该坐标点没有被标记为书签...");
            }

        }
        private void 清除书签ALLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearBookMark();
        }
        #endregion
        private void map_choice(string str){
            string mapLoc = this.map.mapControl.CacheLocation;
            this.map.mapControl.CacheLocation = System.Windows.Forms.Application.StartupPath + "\\GMapCache\\" + str + "\\";

            Program.WriteLog("选择了地图:"+str);

            switch (str)
            {
                case "高德平面地图":
                    Program.isMarsCoordinatesInChina = true;
                    this.map.mapControl.MapProvider = AMapProvider.Instance;
                    break;
                case "高德卫星地图":
                    Program.isMarsCoordinatesInChina = true;
                    this.map.mapControl.MapProvider = AMapSateliteProvider.Instance;

                    //
                    break;
                case "ArcGIS_Imagery_World_2D_Map":
                    Program.isMarsCoordinatesInChina = false;
                    this.map.mapControl.MapProvider = GMapProviders.ArcGIS_Imagery_World_2D_Map;
                    break;
                case "ArcGIS_StreetMap_World_2D_Map":
                    Program.isMarsCoordinatesInChina = false;
                    this.map.mapControl.MapProvider = GMapProviders.ArcGIS_StreetMap_World_2D_Map;
                    break;
                case "ArcGIS_Topo_US_2D_Map":
                    Program.isMarsCoordinatesInChina = false;
                    this.map.mapControl.MapProvider = GMapProviders.ArcGIS_Topo_US_2D_Map;
                    break;
                case "ArcGIS_World_Street_Map":
                    Program.isMarsCoordinatesInChina = false;
                    this.map.mapControl.MapProvider = GMapProviders.ArcGIS_World_Street_Map;
                    break;
                case "ArcGIS_World_Terrain_Base_Map":
                    Program.isMarsCoordinatesInChina = false;
                    this.map.mapControl.MapProvider = GMapProviders.ArcGIS_World_Terrain_Base_Map;
                    break;
                case "ArcGIS_World_Topo_Map":
                    Program.isMarsCoordinatesInChina = false;
                    this.map.mapControl.MapProvider = GMapProviders.ArcGIS_World_Topo_Map;
                    break;
                case "BingHybridMap":
                    Program.isMarsCoordinatesInChina = false;
                    this.map.mapControl.MapProvider = GMapProviders.BingHybridMap;
                    break;
                case "BingMap":
                    Program.isMarsCoordinatesInChina = false;
                    this.map.mapControl.MapProvider = GMapProviders.BingMap;
                    break;
                case "BingSatelliteMap":
                    this.map.mapControl.MapProvider = GMapProviders.BingSatelliteMap;
                    Program.isMarsCoordinatesInChina = false;
                    break;
                case "CloudMadeMap":
                    Program.isMarsCoordinatesInChina = false;
                    this.map.mapControl.MapProvider = GMapProviders.CloudMadeMap;
                    break;
                case "CzechHistoryMap":
                    Program.isMarsCoordinatesInChina = false;
                    this.map.mapControl.MapProvider = GMapProviders.CzechHistoryMap;
                    break;
                case "CzechHybridMap":
                    Program.isMarsCoordinatesInChina = false;
                    this.map.mapControl.MapProvider = GMapProviders.CzechHybridMap;
                    break;
                case "CzechMap":
                    Program.isMarsCoordinatesInChina = false;
                    this.map.mapControl.MapProvider = GMapProviders.CzechMap;
                    break;
                case "CzechSatelliteMap":
                    Program.isMarsCoordinatesInChina = true;
                    this.map.mapControl.MapProvider = GMapProviders.CzechSatelliteMap;
                    break;
                case "CzechTuristMap":
                    Program.isMarsCoordinatesInChina = false;
                    this.map.mapControl.MapProvider = GMapProviders.CzechTuristMap;
                    break;
                case "GoogleChinaHybridMap":
                    Program.isMarsCoordinatesInChina = true;
                    this.map.mapControl.MapProvider = GMapProviders.GoogleChinaHybridMap;
                    break;
                case "GoogleChinaMap":
                    Program.isMarsCoordinatesInChina = false;
                    this.map.mapControl.MapProvider = GMapProviders.GoogleChinaMap;
                    break;
                case "GoogleChinaSatelliteMap":
                    Program.isMarsCoordinatesInChina = false;
                    this.map.mapControl.MapProvider = GMapProviders.GoogleChinaSatelliteMap;
                    break;
                case "GoogleChinaTerrainMap":
                    Program.isMarsCoordinatesInChina = false;
                    this.map.mapControl.MapProvider = GMapProviders.GoogleChinaTerrainMap;
                    break;
                case "GoogleHybridMap":
                    Program.isMarsCoordinatesInChina = true;
                    this.map.mapControl.MapProvider = GMapProviders.GoogleHybridMap;
                    break;
                case "GoogleKoreaHybridMap":
                    Program.isMarsCoordinatesInChina = false;
                    this.map.mapControl.MapProvider = GMapProviders.GoogleKoreaHybridMap;
                    break;
                case "GoogleKoreaMap":
                    Program.isMarsCoordinatesInChina = false;
                    this.map.mapControl.MapProvider = GMapProviders.GoogleKoreaMap;
                    break;
                case "GoogleKoreaSatelliteMap":
                    Program.isMarsCoordinatesInChina = false;
                    this.map.mapControl.MapProvider = GMapProviders.GoogleKoreaSatelliteMap;
                    break;
                case "GoogleMap":
                    Program.isMarsCoordinatesInChina = false;
                    this.map.mapControl.MapProvider = GMapProviders.GoogleMap;
                    break;
                case "GoogleSatelliteMap":
                    Program.isMarsCoordinatesInChina = false;
                    this.map.mapControl.MapProvider = GMapProviders.GoogleSatelliteMap;
                    break;
                case "GoogleTerrainMap":
                    Program.isMarsCoordinatesInChina = false;
                    this.map.mapControl.MapProvider = GMapProviders.GoogleTerrainMap;
                    break;
                default:
                    this.map.mapControl.CacheLocation = mapLoc;
                    break;
            }
           this.map.ShowMapUpdate();
           if (this.BookMark != null)
           {
               int i = 1;
               foreach (PointLatLngAlt p in this.BookMark)
               {
                   this.map.DrawBookMark(p, i);
                   i++;
               }
           }
        }
        private void comboBox_mapname_SelectedIndexChanged(object sender, EventArgs e)
        {
            map_choice(this.comboBox_mapname.Text);
           
        }
        void SetMapLocation(double lat, double lng,double alt,float yaw)
        {
            if (this.map.mapControl.InvokeRequired)
            {
                SetLocation t = new SetLocation(SetMapLocation);
               this.map.mapControl.BeginInvoke(t, new object[] { lat, lng ,alt,yaw});
            }
            else
            {
                this.map.mapControl_SetPlanePosition(lng, lat, alt, yaw);
            }
        }
        private void but_DownMapPart_Click(object sender, EventArgs e)
        {
            RectLatLng area = this.map.mapControl.SelectedArea;
            if (!area.IsEmpty)
            {
                //for (int i = (int)this.map.mapControl.Zoom; i <= this.map.mapControl.MaxZoom; i++)
                for (int i = (int)this.map.mapControl.Zoom; i <= Math.Min(map.mapControl.MaxZoom, 19); i++)
                {
                    TilePrefetcher obj = new TilePrefetcher();
                    obj.Text = "正在下载地图...";
                    obj.Start(area, i, this.map.mapControl.MapProvider, 100,3);
                }
            }
            else
            {
                MessageBox.Show("Select map area holding ALT", "GMap.NET", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            this.map.isRemoveToPosition = !this.map.isRemoveToPosition;

            if (this.map.isRemoveToPosition)
            {
                Program.WriteLog("选择了地图跟随...");
            }
            else {
                Program.WriteLog("取消了地图跟随...");
            }

        }
        public void addDataView_BookMark(int count,PointLatLngAlt p)
        {
            PointLatLngAlt point=new PointLatLngAlt();

           // PointLatLng p1 = Program.PointConversion.Mar2Earth(new PointLatLng(p.Lat, p.Lng));
            point.Lat = p.Lat;
            point.Lng = p.Lng;
  
            point.Alt = p.Alt;
            if (this.dataView_bookMark == null)
            {
                return;
            }


            DataGridViewRow row = new DataGridViewRow();
            DataGridViewTextBoxCell textboxcell1 = new DataGridViewTextBoxCell();
            DataGridViewTextBoxCell textboxcell2 = new DataGridViewTextBoxCell();
            DataGridViewTextBoxCell textboxcell3 = new DataGridViewTextBoxCell();
            DataGridViewTextBoxCell textboxcell4 = new DataGridViewTextBoxCell();

            textboxcell1.Value = count.ToString();
            textboxcell2.Value = point.Lng.ToString("0.0000000");
            textboxcell3.Value = point.Lat.ToString("0.0000000");

            if (this.BookMark.Count == 1)
            {
                textboxcell4.Value = 0;
            }
            else if(this.BookMark.Count>1){
                double s = this.map.Distance(BookMark[BookMark.Count - 2].Lat, BookMark[BookMark.Count - 2].Lng, BookMark[BookMark.Count - 1].Lat, BookMark[BookMark.Count - 1].Lng);
                textboxcell4.Value = s.ToString("0.000");
                totaldistance += s;

            }

                this.label_totaldistance.Text = "总里程数:" + totaldistance.ToString("0.000m");


            DataGridViewButtonCell buttoncell = new DataGridViewButtonCell();
            buttoncell.Value = "删除行";
            row.Cells.Add(textboxcell1);
            row.Cells.Add(textboxcell2);
            row.Cells.Add(textboxcell3);
            row.Cells.Add(textboxcell4);
            row.Cells.Add(buttoncell);
            this.dataView_bookMark.Rows.Add(row);


        }
        private void MouseDragChangeBookMarkValue(int index, PointLatLng point) {

            point = Program.PointConversion.Mar2Earth(point);

            PointLatLngAlt p = this.BookMark[index - 1];
            p.Lat = point.Lat;
            p.Lng = point.Lng;
            this.BookMark.RemoveAt(index-1);
            this.BookMark.Insert(index-1,p);
            DataGridViewRow row = new DataGridViewRow();
            DataGridViewTextBoxCell textboxcell1 = new DataGridViewTextBoxCell();
            DataGridViewTextBoxCell textboxcell2 = new DataGridViewTextBoxCell();
            DataGridViewTextBoxCell textboxcell3 = new DataGridViewTextBoxCell();
            DataGridViewTextBoxCell textboxcell4 = new DataGridViewTextBoxCell();

            if (index == 1)
            {
                textboxcell4.Value = 0;
                DataGridViewTextBoxCell textbox = (DataGridViewTextBoxCell)this.dataView_bookMark.Rows[index].Cells[3];
                double ss = double.Parse((string)textbox.Value);

                double s1 = this.map.Distance(BookMark[index].Lat, BookMark[index].Lng,
                    BookMark[index - 1].Lat, BookMark[index - 1].Lng);
                textbox.Value = s1.ToString("0.000");

                totaldistance = totaldistance - ss + s1;

            } else if (index == this.BookMark.Count) {
                double s = this.map.Distance(BookMark[index - 2].Lat, BookMark[index - 2].Lng,
                    BookMark[index - 1].Lat, BookMark[index - 1].Lng);
                textboxcell4.Value = s.ToString("0.000");


                DataGridViewTextBoxCell textbox1 = (DataGridViewTextBoxCell)this.dataView_bookMark.Rows[index-1].Cells[3];
                double sss = double.Parse((string)textbox1.Value);
                totaldistance += (s-sss);
   

            }
            else
            {
                double s = this.map.Distance(BookMark[index - 2].Lat, BookMark[index - 2].Lng,
                    BookMark[index - 1].Lat, BookMark[index - 1].Lng);
                textboxcell4.Value = s.ToString("0.000");

                DataGridViewTextBoxCell textbox = (DataGridViewTextBoxCell)this.dataView_bookMark.Rows[index].Cells[3];
                DataGridViewTextBoxCell textbox1 = (DataGridViewTextBoxCell)this.dataView_bookMark.Rows[index - 1].Cells[3];
                double sss = double.Parse((string)textbox1.Value);

                double ss = double.Parse((string)textbox.Value);

                double s1 = this.map.Distance(BookMark[index ].Lat, BookMark[index ].Lng,
                    BookMark[index - 1].Lat, BookMark[index - 1].Lng);
                textbox.Value = s1.ToString("0.000");


                totaldistance = totaldistance - ss + s1 + s-sss;

            }
            this.label_totaldistance.Text = "总里程数:" + this.totaldistance.ToString("0.000m");
            textboxcell1.Value = index.ToString();
            textboxcell2.Value = point.Lng.ToString("0.0000000");
            textboxcell3.Value = point.Lat.ToString("0.0000000");
            DataGridViewButtonCell buttoncell = new DataGridViewButtonCell();
            buttoncell.Value = "删除行";
            row.Cells.Add(textboxcell1);
            row.Cells.Add(textboxcell2);
            row.Cells.Add(textboxcell3);
            row.Cells.Add(textboxcell4);
            row.Cells.Add(buttoncell);
            this.dataView_bookMark.Rows.RemoveAt(index - 1);
            this.dataView_bookMark.Rows.Insert(index - 1, row);
        }
        public void SaveMission() {
            SaveFileDialog loSaveFile = new SaveFileDialog();
            //string str = sfd.OpenFile();

            loSaveFile.Filter = "(*.txt)|*.txt|所有文件(*.*)|*.*|(*.bin)|*.bin|(*.dat)|*.dat";// "PDF文件(*.pdf)|*.pdf";
            loSaveFile.Title = "保存";
            loSaveFile.FilterIndex = 1;
            loSaveFile.RestoreDirectory = true;
            DateTime time = DateTime.Now;
            string str = DateTime.Now.ToString("yyyyMMddhhmmss");
            loSaveFile.FileName = str;
            if (loSaveFile.ShowDialog() == DialogResult.OK)
            {
                string localFilePath = loSaveFile.FileName.ToString(); //获得文件路径 
                string fileNameExt = localFilePath.Substring(localFilePath.LastIndexOf("\\") + 1); //获取文件名，不带路径
                FileStream fs = new FileStream(localFilePath, FileMode.OpenOrCreate, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);
                foreach (PointLatLngAlt p in this.map.drawingPoints)
                {
                    sw.WriteLine(p.ToString());
                }
                sw.Dispose();
                fs.Dispose();
                Program.WriteLog("保存任务:" + localFilePath);
            }
        }
        private void button_savewp_Click(object sender, EventArgs e)
        {
            SaveMission();

        }
        private void ListBox_up(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right) {
                this.MenuStrip_ListBox.Show(Cursor.Position);
            }
        }
        public void ReadMission() {
            OpenFileDialog loOpenFile = new OpenFileDialog();
            //string str = sfd.OpenFile();

            loOpenFile.Filter = "(*.txt)|*.txt|所有文件(*.*)|*.*|(*.bin)|*.bin|(*.dat)|*.dat";// "PDF文件(*.pdf)|*.pdf";
            loOpenFile.Title = "读取";
            loOpenFile.FilterIndex = 1;
            loOpenFile.RestoreDirectory = true;


            if (loOpenFile.ShowDialog() == DialogResult.OK)
            {
                string localFilePath = loOpenFile.FileName.ToString(); //获得文件路径 
                string fileNameExt = localFilePath.Substring(localFilePath.LastIndexOf("\\") + 1); //获取文件名，不带路径
                //  MessageBox.Show(loOpenFile.FileName.ToString());
                FileStream fs = new FileStream(localFilePath, FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(fs);
                //StreamWriter sw = new StreamWriter(fs);

                Program.WriteLog("读取任务：" + localFilePath);

                string str = sr.ReadLine();
                List<PointLatLngAlt> draw = new List<PointLatLngAlt>();
                int i = 0;
                while (str != null)
                {
                    try
                    {
                        string[] str1 = Regex.Split(str, " ");
                        double lng = double.Parse(str1[0]);
                        double lat = double.Parse(str1[1]);
                        float alt = float.Parse(str1[2]);
                        PointLatLngAlt p = new PointLatLngAlt(lat, lng, alt);
                        this.ListBoxAddMessage("经度：" + lng.ToString() + "\t纬度：" + lat.ToString() + "\t高度：" + alt.ToString());
                        draw.Add(p);
                    }
                    catch
                    {
                        MessageBox.Show("该文件不是标准格式的数据文件...");
                        fs.Dispose();
                        sr.Dispose();
                        return;
                    }
                    str = sr.ReadLine();
                }
                this.map.mapClearWP();
                this.map.drawingPoints = draw;
                PointLatLngAlt Point_Max = new PointLatLngAlt(-90, -180);
                PointLatLngAlt Point_Min = new PointLatLngAlt(90, 180);

                foreach (PointLatLngAlt p in draw)
                {
                    if (this.map.SystemModeID == 1)
                    {
                        this.map.mapAddClickMarker(new PointLatLngAlt(p.Lat, p.Lng)); //point_count++;
                        this.map.mapAdddataGridView(++i, new PointLatLngAlt(p.Lat, p.Lng, p.Alt));
                    }
                    Point_Max.Lat = (Point_Max.Lat > p.Lat) ? Point_Max.Lat : p.Lat;
                    Point_Max.Lng = (Point_Max.Lng > p.Lng) ? Point_Max.Lng : p.Lng;
                    Point_Min.Lat = (Point_Min.Lat < p.Lat) ? Point_Min.Lat : p.Lat;
                    Point_Min.Lng = (Point_Min.Lng < p.Lng) ? Point_Min.Lng : p.Lng;
                }

                PointLatLng point;
                if (Program.isMarsCoordinatesInChina)
                {
                    point = Program.PointConversion.GetEarth2Mars(new PointLatLng((Point_Max.Lat + Point_Min.Lat) / 2, (Point_Max.Lng + Point_Min.Lng) / 2));
                }
                else
                {
                    point = new PointLatLng((Point_Max.Lat + Point_Min.Lat) / 2, (Point_Max.Lng + Point_Min.Lng) / 2);
                }
                if (this.map.SystemModeID == 1)
                {
                    this.map.mapControl.Position = point;
                    this.map.mapDrawWP(map.drawingPoints);
                }

                fs.Dispose();
                sr.Dispose();
                //如果当前正在规划航点
                if (this.routeplanningform != null)
                {
                    UpdatePlanForm();
                }

            }
        }
        private void button_LoadwayPoint_Click(object sender, EventArgs e)
        {
            ReadMission();
        }
        //将所有鼠标点击出来的信息进行全面更新
        void UpdatePlanForm() {
            int i = 0;
            this.routeplanningform.ClearLinePointDataView();
            this.routeplanningform.ClearWayPointDataView();

            if (this.map.drawingPoints.Count == 0) return;

            CalcStraightSlope();


            for (i = 0; i < point_K.Count-1; i++) {
                if (i == 0)
                {
                    this.routeplanningform.SetWayPointPara(i + 1, (int)this.map.drawingPoints[i].Alt, true);
                }
                else {
                    this.routeplanningform.SetWayPointPara(i + 1, (int)this.map.drawingPoints[i].Alt, false);
                }
                #region 计算并填充航线段
                if (point_K.Count >= 3) { 
                    double ang=Math.Atan(point_K[i])/Math.PI*180;
                    string direction = "";
                    updateanglestr(ref direction, ref ang, i);
                    this.routeplanningform.SetLinePointPara(i + 1, (int)ang,direction,(i+1).ToString()+","+(i+2).ToString() , false);
                #endregion
                }
            }
            this.routeplanningform.SetWayPointPara(i + 1, (int)this.map.drawingPoints[i].Alt, false);
            if (point_K.Count >= 3) { 
                 double ang1 = Math.Atan(point_K[i]) / Math.PI * 180;

                 string direction1 = "";

                 updateanglestr(ref direction1, ref ang1, i);
                 this.routeplanningform.SetLinePointPara(i + 1, (int)ang1, direction1,(i + 1).ToString() + ",1", false);           
            }
        }
        private void button_import_Click(object sender, EventArgs e)
        {
            if (!this.map.mapControl.ShowImportDialog()) {
                MessageBox.Show("Import Failed！！！");
            }
        }
        private void button_export_Click(object sender, EventArgs e)
        {
            this.map.mapControl.ShowExportDialog();
        }
        void ThreadMethod()
        {
            if (playback_from == null || playback_from.IsDisposed)
            {
                playback_from = new PlayBackForm();
                playback_from.getFilename = ChiocePlayBackFile;
                playback_from.FormClosing += playback_from_FormClosing;
                playback_from.SetPlayBackProcess = SetPlayBackProcess;
                playback_from.playstatus = playstatus;

                playback_from.StartPosition = FormStartPosition.Manual;
                playback_from.Location = new System.Drawing.Point((this.Width - playback_from.Width) / 2, (this.Height - playback_from.Height) / 2);
                playback_from.TopMost = true;
            }
            playback_from.ShowDialog();
            while (playback_from != null && !playback_from.IsDisposed)
            {
                Application.DoEvents();
            }
        }
        void playstatus(int status) {
            switch (status)
            {
                case 1: //按下了播放暂停键
                    if (!playback_from.isPlayPause) { //回放处于暂停状态
                        this.sysplaybackprogress = DateTime.Now - sysstartplaytime;
                    }
                    break;
                case 2: //按下了前进键
                   sysstartplaytime= this.sysstartplaytime.AddSeconds(-5);
                   if (DateTime.Now - sysstartplaytime > sysplaybackendtime - sysplaybackstartTime) {
                       sysstartplaytime = this.sysstartplaytime.AddSeconds(5);
                   }
                   SysPlayBackUpdateStatus();
                    break;
                case 3://按下了后退键
                    sysstartplaytime = this.sysstartplaytime.AddSeconds(5);
                    if (DateTime.Now < sysstartplaytime )
                    {
                        sysstartplaytime = this.sysstartplaytime.AddSeconds(-5);

                    }
                    SysPlayBackUpdateStatus();
                    break;
                case 4: 
                    
                    break;
                default: break;
            }
        }
        void SysPlayBackUpdateStatus() {
            if (this.sysplaybackflag)
            {
                lock (sysplaybackdata) { 
                    sysplaybackindex.connectindex = 0;
                    sysplaybackindex.positionindex = 0;
                    sysplaybackindex.postureindex = 0;
                    sysplaybackindex.sysstatusindex = 0;

                    lock (CenterList)
                    {
                        CenterList.Clear();
                    }
               

                    if (sysplaybackdata != null)
                    {
                        #region 连接列表
                        if (sysplaybackdata.listconnect != null) {
                            for (;sysplaybackindex.connectindex < sysplaybackdata.listconnect.Count; sysplaybackindex.connectindex++) {
                                if (DateTime.Now - sysstartplaytime >=
                                    this.sysplaybackdata.listconnect[this.sysplaybackindex.connectindex].time - this.sysplaybackstartTime)//计算当前播放位置的时间差
                                {
                                    this.myTabCon1.SetConnect(this.sysplaybackdata.listconnect[this.sysplaybackindex.connectindex].conn);//更新连接信息
                                }
                                else {
                                    break;
                                }
                            }
                        }
                        #endregion 
                        #region 姿态信息
                        if (sysplaybackdata.listposture != null)
                        {
                            for (; sysplaybackindex.postureindex < sysplaybackdata.listposture.Count; sysplaybackindex.postureindex++)
                            {
                                if (DateTime.Now - sysstartplaytime >=
                                    this.sysplaybackdata.listposture[this.sysplaybackindex.postureindex].time - this.sysplaybackstartTime)//计算当前播放位置的时间差
                                {

                                }
                                else
                                {
                                    sysplaybackyaw = (float)(this.sysplaybackdata.listposture[this.sysplaybackindex.postureindex].mission_attitude.yaw * 180.0f / System.Math.PI);
                                    this.myTabCon1.Set_Roll((float)(this.sysplaybackdata.listposture[this.sysplaybackindex.postureindex].mission_attitude.roll * 180.0f / System.Math.PI));
                                    this.myTabCon1.Set_Pitch((float)(this.sysplaybackdata.listposture[this.sysplaybackindex.postureindex].mission_attitude.pitch * 180.0f / System.Math.PI));
                                    this.myTabCon1.Set_Yaw(sysplaybackyaw);
                                    break;
                                }
                            }
                        }
                        #endregion 
                        #region 位置信息
                        if (sysplaybackdata.listposition!=null) {
                            for (; sysplaybackindex.positionindex < sysplaybackdata.listposition.Count; sysplaybackindex.positionindex++)
                            {
                                if (DateTime.Now - sysstartplaytime >=
                                    this.sysplaybackdata.listposition[this.sysplaybackindex.positionindex].time - this.sysplaybackstartTime)//计算当前播放位置的时间差
                                {
                                    PointLatLngAlt latlngaltplayback1 = new PointLatLngAlt();
                                    latlngaltplayback1.Alt = this.sysplaybackdata.listposition[this.sysplaybackindex.positionindex].mission_position.relative_alt / 100.0f;
                                    latlngaltplayback1.Lat = this.sysplaybackdata.listposition[this.sysplaybackindex.positionindex].mission_position.lat / 10000000.0f;
                                    latlngaltplayback1.Lng = this.sysplaybackdata.listposition[this.sysplaybackindex.positionindex].mission_position.lon / 10000000.0f;
                                    lock (CenterList) {CenterList.Add(latlngaltplayback1); }
                                }
                                else
                                {
                                    PointLatLngAlt latlngaltplayback = new PointLatLngAlt();
                                    latlngaltplayback.Alt = this.sysplaybackdata.listposition[this.sysplaybackindex.positionindex].mission_position.relative_alt / 100.0f;
                                    latlngaltplayback.Lat = this.sysplaybackdata.listposition[this.sysplaybackindex.positionindex].mission_position.lat / 10000000.0f;
                                    latlngaltplayback.Lng = this.sysplaybackdata.listposition[this.sysplaybackindex.positionindex].mission_position.lon / 10000000.0f;
                                    this.myTabCon1.SetAlt(latlngaltplayback.Alt);
                                    this.myTabCon1.SetLat(latlngaltplayback.Lat);
                                    this.myTabCon1.SetLng(latlngaltplayback.Lng);
                                    if (this.sysplaybackdata.listposition[this.sysplaybackindex.positionindex].mission_position.vz == 1)
                                    {
                                        this.map.SetHome(new PointLatLng(LatLngAlt.Lat, LatLngAlt.Lng));
                                    }
                                    this.myTabCon1.Set_Speed(this.sysplaybackdata.listposition[this.sysplaybackindex.positionindex].mission_position.vy);
                                    this.myTabCon1.SetSatelliteNumber(this.sysplaybackdata.listposition[this.sysplaybackindex.positionindex].mission_position.vx);
                                    double dis = this.map.GetDistance(latlngaltplayback.Lat, latlngaltplayback.Lng);
                                    this.myTabCon1.setDistance((int)(dis));
                                    SetMapPlane(latlngaltplayback, sysplaybackyaw);
                                    if (this.map.mapControl.InvokeRequired)
                                    {
                                        DrawtailDele d = new DrawtailDele(DrawTail);
                                        this.BeginInvoke(d, new object[] { latlngaltplayback });
                                    }
                                    break;
                                }
                            }



                        }
                        #endregion
                        #region 系统状态信息
                        if (sysplaybackdata.listsysstatus != null)
                        {

                            DateTime timestart = new DateTime();
                            TimeSpan Ts = new TimeSpan();
                            TimeSpan Ts1 = new TimeSpan();

                            bool armStartflag = false;
                            bool armEndflag = false;

                            for (; sysplaybackindex.sysstatusindex < sysplaybackdata.listsysstatus.Count; sysplaybackindex.sysstatusindex++)
                            {
                                if (DateTime.Now - sysstartplaytime >=
                                    this.sysplaybackdata.listsysstatus[this.sysplaybackindex.sysstatusindex].time - this.sysplaybackstartTime)//计算当前播放位置的时间差
                                {

                                    ushort status = this.sysplaybackdata.listsysstatus[this.sysplaybackindex.sysstatusindex].mission_sysstatus.drop_rate_comm;

                                    string str = this.sysplaybackdata.listsysstatus[this.sysplaybackindex.sysstatusindex].time.ToString("hh:mm:ss");

                                //    string sss = "时间:" + str +" Index:"+this.sysplaybackindex.sysstatusindex+ " Value:" + (status & AP_DATA_STATUS_ARMED).ToString();

                                 //  System.Diagnostics.Debug.WriteLine("时间:" + str + " Value:" + (status & AP_DATA_STATUS_ARMED).ToString());

                                   if ((status & AP_DATA_STATUS_ARMED) == 0)
                                   {//此时电机停转
                                       armEndflag = true;
                                       if (armStartflag) { //如果上一个时刻电机处于转动状态
                                           armStartflag = false;
                                           Ts = Ts + Ts1;
                                      //     this.ListBoxAddMessage("FALSE");

                                  //         sss += " FALSE";

                                           Ts1 = new TimeSpan();
                                       }
                                   }
                                   else { //此时电机开始转动
                                       if (armEndflag) {//表示上一个时刻电机处于停转状态
                                           armEndflag = false;
                                           timestart = this.sysplaybackdata.listsysstatus[this.sysplaybackindex.sysstatusindex].time;//记录电机开始转动时间
                                       }
                                       Ts1 = this.sysplaybackdata.listsysstatus[this.sysplaybackindex.sysstatusindex].time - timestart;//更新电机本次转动的时间
                                       armStartflag = true;

                            //           sss += (" TS1:" + Ts1.Hours + ":" + Ts1.Minutes + ":" + Ts1.Seconds);

                                   //    this.ListBoxAddMessage("TS1:" + Ts1.Hours + ":" + Ts1.Minutes + ":" + Ts1.Seconds);

                                   }

                               //    System.Diagnostics.Debug.WriteLine(sss);
                                }
                                else
                                {
                                    this.map.dStart = Ts+Ts1;
                                   this.map.ArmStart = DateTime.Now - this.map.dStart;
                                   this.map.dateArm = this.map.ArmStart;
                                   this.map.dataarmNow = DateTime.Now;
                             //     this.ListBoxAddMessage("TS:"+Ts.Hours+":"+Ts.Minutes+":"+Ts.Seconds);

                                    float votage = this.sysplaybackdata.listsysstatus[this.sysplaybackindex.sysstatusindex].mission_sysstatus.voltage_battery * 10.0f;
                                    float current = this.sysplaybackdata.listsysstatus[this.sysplaybackindex.sysstatusindex].mission_sysstatus.current_battery * 10.0f;

                                    this.myTabCon1.SetVotage(votage);
                                    this.myTabCon1.SetCurrent(current);

                                    float percent = (votage/1000f *100f / 4.20f - 500);
                                    battery.SetBattery((int)(percent));


                                    ushort status = this.sysplaybackdata.listsysstatus[this.sysplaybackindex.sysstatusindex].mission_sysstatus.drop_rate_comm;
                                    ushort target_dist = this.sysplaybackdata.listsysstatus[this.sysplaybackindex.sysstatusindex].mission_sysstatus.errors_comm;
                                    ushort rssi = this.sysplaybackdata.listsysstatus[this.sysplaybackindex.sysstatusindex].mission_sysstatus.errors_count1;
                                    SetDataView(this.sysplaybackdata.listsysstatus[this.sysplaybackindex.sysstatusindex].mission_sysstatus);
                                    myTabCon1.SetRSSI(rssi);
                                    if ((status & AP_DATA_STATUS_ARMED) == 0)
                                    {//螺旋桨
                                        this.myTabCon1.SetArmed(false);
                                        this.map.SetisArm(false);
    
                                    }
                                    else
                                    {
                                        this.myTabCon1.SetArmed(true);
                                        this.map.SetisArm(true);

                                    }
                                    if ((status & AP_DATA_STATUS_TAKEOFF) == 0)
                                    {//自动起飞
                                        this.myTabCon1.SetWarnning(false, "", Color.Yellow);
                                    }
                                    else
                                    {
                                        this.myTabCon1.SetWarnning(true, "一键起飞", Color.Yellow);
                                    }
                                    if ((status & AP_DATA_STATUS_NEED_HOVER_THR) == 0)
                                    {//油门低
                                    }
                                    else
                                    {
                                        this.myTabCon1.SetWarnning(true, "油门低警告", Color.Red);
                                    }
                                    FLY_MODE_t flymode = (FLY_MODE_t)this.sysplaybackdata.listsysstatus[this.sysplaybackindex.sysstatusindex].mission_sysstatus.load;
                                    switch (flymode)
                                    {
                                        case FLY_MODE_t.FM_ALT:
                                            indexmodestring = 3;
                                            break;
                                        case FLY_MODE_t.FM_CIRCLE:
                                            indexmodestring = 4;
                                            break;
                                        case FLY_MODE_t.FM_HOVER:
                                            indexmodestring = 6;
                                            break;
                                        case FLY_MODE_t.FM_MANUAL:
                                            indexmodestring = 0;
                                            break;
                                        case FLY_MODE_t.FM_RTH:
                                            indexmodestring = 2;
                                            break;
                                        case FLY_MODE_t.FM_STABLIZER:
                                            indexmodestring = 1;
                                            break;
                                        case FLY_MODE_t.FM_WAY_POINT:
                                            indexmodestring = 5;
                                            break;
                                        case FLY_MODE_t.FM_SINGLE_WAY_POINT:
                                            indexmodestring = 7;
                                            break;
                                        default: break;
                                    }
                                    UpdateLabelMode();
                                    break;
                                }
                            }
                        }
                        #endregion
                    }
                
                }
            }
            else { 
            
            }
        }
        void SetPlayBackProcess(double sec)
        {
            this.sysstartplaytime = DateTime.Now.AddSeconds(-1*sec);
         
            SysPlayBackUpdateStatus();

        }
        void playback_from_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
           // DateStart = DateTime.Now;
        }
        string ChiocePlayBackFile()
        { 
            OpenFileDialog loOpenFile = new OpenFileDialog();
            loOpenFile.Filter = "(*.txt)|*.txt|所有文件(*.*)|*.*|(*.bin)|*.bin|(*.dat)|*.dat";// "PDF文件(*.pdf)|*.pdf";
            loOpenFile.Title = "保存文档";
            loOpenFile.Multiselect = false;
            loOpenFile.FilterIndex = 1;
            loOpenFile.RestoreDirectory = true;
            if (loOpenFile.ShowDialog() == DialogResult.OK)
            {
                this.sysplaybackdata = XMLSerializer.DeSerialize<SerializeDataClass>(loOpenFile.FileName.ToString());

                if (sysplaybackdata == null)
                {
                    MessageBox.Show("数据读取失败");
                    return null;
                }

                if (sysplaybackdata.listconnect == null) return null;
                DateTime time = this.sysplaybackdata.listconnect[0].time;
                if (sysplaybackdata.listposition != null && sysplaybackdata.listposition.Count > 1)
                {
                    time = sysplaybackdata.listposition[0].time < time ? sysplaybackdata.listposition[0].time : time;
                }
                if (sysplaybackdata.listposture != null && sysplaybackdata.listposture.Count > 1)
                {
                    time = sysplaybackdata.listposture[0].time < time ? sysplaybackdata.listposture[0].time : time;
                }
                if (sysplaybackdata.listsysstatus != null && sysplaybackdata.listsysstatus.Count > 1)
                {
                    time = sysplaybackdata.listsysstatus[0].time < time ? sysplaybackdata.listsysstatus[0].time : time;
                }

                this.sysplaybackstartTime = time;

                DateTime time1 = this.sysplaybackdata.listconnect[this.sysplaybackdata.listconnect.Count - 1].time;

                if (sysplaybackdata.listposition != null && sysplaybackdata.listposition.Count > 1)
                {
                    time1 = sysplaybackdata.listposition[sysplaybackdata.listposition.Count - 1].time > time1 ? sysplaybackdata.listposition[sysplaybackdata.listposition.Count - 1].time : time1;
                }

                if (sysplaybackdata.listposture != null && sysplaybackdata.listposture.Count > 1)
                {
                    time1 = sysplaybackdata.listposture[sysplaybackdata.listposture.Count - 1].time > time1 ? sysplaybackdata.listposture[sysplaybackdata.listposition.Count - 1].time : time1;
                }

                if (sysplaybackdata.listsysstatus != null && sysplaybackdata.listsysstatus.Count > 1)
                {
                    time1 = sysplaybackdata.listsysstatus[sysplaybackdata.listsysstatus.Count - 1].time > time1 ? sysplaybackdata.listsysstatus[sysplaybackdata.listsysstatus.Count - 1].time : time1;
                }

                this.sysplaybackendtime = time1;

                sysplaybackindex.connectindex = 0;
                sysplaybackindex.positionindex = 0;
                sysplaybackindex.postureindex = 0;
                sysplaybackindex.sysstatusindex = 0;
                sysstartplaytime = DateTime.Now;
                sysplaybackflag = true;
                sysplaybackendflag = false;
                playback_from.StatrtTime = this.sysplaybackstartTime;
                playback_from.EndTime = this.sysplaybackendtime;
                lock (CenterList) {CenterList.Clear(); }

                
                return Path.GetFileName(loOpenFile.FileName);  
            }
            else { 
            
            }
            return null;
        }
        private void button_playback_Click_1(object sender, EventArgs e)
        {
            if (this.isComOpen)
            {
                MessageBox.Show("串口连接已开启,无法进行数据回放...");
                return;
            }
            else
            {
               
                System.Threading.Thread newThread = new System.Threading.Thread(new System.Threading.ThreadStart(ThreadMethod));
                newThread.SetApartmentState(System.Threading.ApartmentState.STA);
                newThread.Start();
            }
        }
        private void button_toawmfile_Click(object sender, EventArgs e) //zoom in
        {
            CameraZoomIn();
        }
        void CameraZoomIn() { 
            if (!this.isComOpen)
            {
                MessageBox.Show("当前无连接");
                return;
            }

            mavlink_wps.SendCamZoom(0);

            ListBoxAddMessage("Zoom in...");


            Program.WriteLog("镜头拉近...");        
        }
        void CameraZoomOut() { 
            if (!this.isComOpen)
            {
                MessageBox.Show("当前无连接");
                return;
            }

            mavlink_wps.SendCamZoom(1);

            ListBoxAddMessage("Zoom out...");

            Program.WriteLog("镜头推远...");        
        }
        private void button_importawmfile_Click(object sender, EventArgs e)     //zoom out
        {
            CameraZoomOut();
        }
        private void MyImageButtonClick(object sender, EventArgs e)
        {
            string str = this.comboBox_connecttype.Text;
            switch (str)
            {
                case "Serial Port":
                    StartConnect = ConnectPane;
                    SerialPortConnect();
                    break;
                case "TCP":
                    break;
                case "UDP":
                    break;
                case "Bluetooth":
                    break;
                default: break;

            }
        }
        private void MyImageButtonSerialset_Click(object sender, EventArgs e)
        {
            string str = this.comboBox_connecttype.Text;
            switch (str)
            {
                case "Serial Port":
                    serial_form = new Serial_Form();
                    if (serialize == null)
                    {
                        //  MessageBox.Show("第一次使用...");
                    }
                    else
                    {
                        this.serial_form.SetCom(serialize.ComPortName);
                        this.serial_form.comboBox_baud.Text = serialize.ComBaud;
                      //  this.serial_form.comboBox_databits.Text = serialize.ComDataBits;
                      //  this.serial_form.comboBox4_stopbit.Text = serialize.ComStopBit;
                      //  this.serial_form.comboBox5_verify.Text = serialize.ComVerify;
                    }
                    this.serial_form.PortPara = GetPara;

                    this.serial_form.SetControlEnable(!this.isComOpen);

                    serial_form.ShowDialog();
                    break;
                case "TCP":
                    break;
                case "UDP":
                    break;
                case "Bluetooth":
                    break;
                default: break;

            }
        }
        private void button_toGpx_Click(object sender, System.EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "GPX (*.gpx)|*.gpx";
            DateTime time = DateTime.Now;
            string str = DateTime.Now.ToString("yyyyMMddhhmmss");
            sfd.FileName = str;


            if (sfd.ShowDialog() == DialogResult.OK)
            {
                // var log = Stuff.GetRoutesFromMobileLog(mobileGpsLog, date, dateEnd, 3.3);
                Serialize.gpxType r = new Serialize.gpxType();
                int index = 0;
                foreach (PointLatLngAlt p in this.map.drawingPoints)
                {
                    if (r.wpt == null)
                    {
                        r.wpt = new Serialize.wptType[this.map.drawingPoints.Count];
                    }
                    Serialize.wptType w = new Serialize.wptType();
                    w.lat = (decimal)p.Lat;
                    w.lon = (decimal)p.Lng;

                    w.ele = (decimal)p.Alt;


                    if (p.date == DateTime.Parse("0001-01-01T00:00:00"))
                    {
                        w.time = DateTime.UtcNow;
                    }
                    else
                    {
                        w.time = p.date;
                    }



                    r.wpt[index] = w;

                    index++;


                }
                index = 0;
                if (r.trk == null)
                {
                    r.trk = new Serialize.trkType[1];
                    r.trk[0] = new Serialize.trkType();
                }

                lock (CenterList) { 
                    foreach (PointLatLngAlt p in this.CenterList)
                    {
                        if (r.trk[0].trkseg == null)
                        {
                            r.trk[0].trkseg = new Serialize.trksegType[1];
                            r.trk[0].trkseg[0] = new Serialize.trksegType();
                            r.trk[0].trkseg[0].trkpt = new Serialize.wptType[this.CenterList.Count];
                        }
                        Serialize.wptType w = new Serialize.wptType();
                        w.lat = (decimal)p.Lat;
                        w.lon = (decimal)p.Lng;
                        w.ele = 30;
                        w.ele = (decimal)p.Alt;
                        if (p.date == DateTime.Parse("0001-01-01T00:00:00"))
                        {
                            w.time = DateTime.UtcNow;
                        }
                        else
                        {
                            w.time = p.date;
                        }
                        r.trk[0].trkseg[0].trkpt[index] = w;
                        index++;
                    }                
                }




                XMLSerializer.Serialize<Serialize.gpxType>(r, sfd.FileName);


                Program.WriteLog(sfd.FileName);
                // File.WriteAllText(sfd.FileName, xmlstring);
            }

        }
        public GMap.NET.gpxType DeserializeGPX(string objectXml)
        {
            object retVal = null;
            XmlSerializer serializer = new XmlSerializer(typeof(GMap.NET.gpxType));
            StringReader stringReader = new StringReader(objectXml);
            XmlTextReader xmlReader = new XmlTextReader(stringReader);
            retVal = serializer.Deserialize(xmlReader);
            return retVal as GMap.NET.gpxType;
        }
        private void button_Read_Click(object sender, EventArgs e)
        {
  
            OpenFileDialog loOpenFile = new OpenFileDialog();
            loOpenFile.Filter = "(*.gpx)|*.gpx|所有文件(*.*)|*.*|(*.bin)|*.bin|(*.dat)|*.dat";// "PDF文件(*.pdf)|*.pdf";
            loOpenFile.Title = "读取gpx文件";
            loOpenFile.Multiselect = false;
            loOpenFile.FilterIndex = 1;
            loOpenFile.RestoreDirectory = true;

            Serialize.gpxType gpx = null;

            if (loOpenFile.ShowDialog() == DialogResult.OK)
            {




                string xmlstring = File.ReadAllText(loOpenFile.FileName.ToString());

 

                gpx = XMLSerializer.DeSerializeGPX(loOpenFile.FileName.ToString());

                if (gpx != null) {
                    if (gpx.wpt != null) {

                        if (gpx.wpt.Length > 0) {
                            List<PointLatLngAlt> point = new System.Collections.Generic.List<PointLatLngAlt>();

                            foreach (Serialize.wptType wpt in gpx.wpt) {
                                PointLatLngAlt p = new PointLatLngAlt();
                                p.Lat = (double)wpt.lat;
                                p.Lng = (double)wpt.lon;
                                p.Alt = (float)wpt.ele;

                                point.Add(p);                 
                            }
                            map.mapDrawGetWPs(point);
                        }
                    }
                    if (gpx.trk != null) {
                        if (gpx.trk.Length > 0) {

                            PointLatLng pMax = new PointLatLng(-90, -180);
                            PointLatLng pMin = new PointLatLng(90, 180);

                            foreach (var trk in gpx.trk)
                            {
                                List<PointLatLngAlt> points = new List<PointLatLngAlt>();

                                foreach (var seg in trk.trkseg)
                                {
                                    if (seg != null) { 
                                        foreach (var p in seg.trkpt)
                                        {
                                            points.Add(new PointLatLngAlt((double)p.lat, (double)p.lon,(float)p.ele));

                                            pMax.Lat = pMax.Lat > (double)p.lat ? pMax.Lat : (double)p.lat;
                                            pMax.Lng = pMax.Lng > (double)p.lon ? pMax.Lng : (double)p.lon;
                                            pMin.Lat = pMin.Lat < (double)p.lat ? pMin.Lat : (double)p.lat;
                                            pMin.Lng = pMin.Lng < (double)p.lon ? pMin.Lng : (double)p.lon;
                                        }                                    
                                    }

                                }
                                lock (CenterList) {this.CenterList.Clear();
                                this.CenterList = points;this.map.DrawCenterTail(CenterList); }

                                
                                

                                this.map.mapControl.Position = new PointLatLng((pMax.Lat+pMin.Lat)/2,(pMin.Lng+pMax.Lng)/2);
                            }
                        }
                    }
                }


                Program.WriteLog("读取GPX文件：" + loOpenFile.FileName.ToString());
            }
        }
        private void checkBox_showtrack_CheckedChanged(object sender, EventArgs e)
        {
           // dd./

                if (this.checkBox_showtrack.Checked)
                {
                    this.map.isShowTrack = true;
                    Program.WriteLog("显示航迹...");
                }
                else { 
                    this.map.isShowTrack = false;
                    Program.WriteLog("不显示航迹...");
                }

        }
        public void showform() {
            if (lf.InvokeRequired)
            {
                this.Invoke(new NoParametersNoReturnDele(showform));
            }
            else {
                if (lf == null || lf.IsDisposed)
                {

                }
                else { 
                      lf.Visible = false;
                  lf.Visible = true;                
                }

            }
        
        }
        void locationHold()
        {

            if(true)
            {
                lf = new LoiterTimeForm();
                lf.GetData = this.mavlink_wps.SendLotiter;
                lf.ShowDialog();
            }
            else {

            }
            while (lf != null && !lf.IsDisposed)
            {
                Application.DoEvents();
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (!this.isComOpen) {
                MessageBox.Show("当前无连接");    
                return; 
            }
            mavlink_wps.SendGimbal((float)( 120.0f / 180.0));
            gmTrackBar_pitch.Value = (int)(9000 - 9000 * 120.0f / 180.0);
            ListBoxAddMessage("云台置垂直角度...");
            Program.WriteLog("设置云台垂直角度");
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (!this.isComOpen)
            {
                MessageBox.Show("当前无连接");
                return;
            }
            mavlink_wps.SendGimbal( (float)( 30.0f/180.0 ) );
            gmTrackBar_pitch.Value =  (int)(9000 - 9000 * 30.0f/180.0);
            ListBoxAddMessage("云台置水平角度...");
            Program.WriteLog("设置云台水平角度...");
        }
        private void button_Cleartrack_Click(object sender, EventArgs e)
        {
            lock (CenterList) { CenterList.Clear(); }

           
            lock (this.map.MapCenterTail) { 
            this.map.MapCenterTail.Routes.Clear();
            this.map.MapCenterTail.Polygons.Clear();            
            }

            Program.WriteLog("清除航迹...");

        }
        public void MulMode() {
            this.map.SystemModeID = 1;
            if (!this.isComOpen)
            {
                ListBoxAddMessage("当前无连接");
            }
            else
            {
                this.mavlink_wps.SendToMode(2);
                this.ListBoxAddMessage("正在发送多点模式指令......");
                Program.WriteLog("点击按钮准备进入多点模式...");
            }
            if (this.map.markerTarget != null)
            {
                if (this.map.MapTargetPoint.Markers.Contains(this.map.markerTarget))
                {
                    this.map.MapTargetPoint.Markers.Remove(this.map.markerTarget);

                    PointLatLng p = this.map.markerTarget.Position;
                    this.map.markerTarget.Position = Program.PointConversion.Mar2Earth(p);
                }
            }
            this.myImageButton_Singlepoint.SetColor(Color.Gray);
            this.myImageButton_Multipoint.SetColor(Color.Red);
            this.myImageButton_Iremote.SetColor(Color.Gray);
            if (singlewaypoint == null || singlewaypoint.IsDisposed)
            {
            }
            else
            {
                singlewaypoint.Visible = false;
            }
            if (this.map.markersOverlay.Markers.Count == 0)
            {
                if (this.map.drawingPoints.Count == 0) return;
                this.map.markersOverlay.Markers.Clear();
                this.map.markersOverlay.Routes.Clear();
                PointLatLngAlt Point_Max = new PointLatLngAlt(-90, -180);
                PointLatLngAlt Point_Min = new PointLatLngAlt(90, 180);
                foreach (PointLatLngAlt p in this.map.drawingPoints)
                {
                    this.map.mapAddClickMarker(new PointLatLngAlt(p.Lat, p.Lng)); //point_count++;
                    Point_Max.Lat = (Point_Max.Lat > p.Lat) ? Point_Max.Lat : p.Lat;
                    Point_Max.Lng = (Point_Max.Lng > p.Lng) ? Point_Max.Lng : p.Lng;
                    Point_Min.Lat = (Point_Min.Lat < p.Lat) ? Point_Min.Lat : p.Lat;
                    Point_Min.Lng = (Point_Min.Lng < p.Lng) ? Point_Min.Lng : p.Lng;
                }
                PointLatLng point;
                if (Program.isMarsCoordinatesInChina)
                {
                    point = Program.PointConversion.GetEarth2Mars(new PointLatLng((Point_Max.Lat + Point_Min.Lat) / 2, (Point_Max.Lng + Point_Min.Lng) / 2));
                }
                else
                {
                    point = new PointLatLng((Point_Max.Lat + Point_Min.Lat) / 2, (Point_Max.Lng + Point_Min.Lng) / 2);
                }
                this.map.mapControl.Position = point;
                this.map.mapDrawWP(map.drawingPoints);
            }
        }
        private void Multipoint_Click(object sender, EventArgs e)
        {
            MulMode();
        }
        public void SingleMode() {
            if (singlewaypoint == null || singlewaypoint.IsDisposed)
            {
                singlewaypoint = new SingleWayPointInsert();
                singlewaypoint.Text = "单点模式";
                singlewaypoint.StartPosition = FormStartPosition.Manual;
                singlewaypoint.Location = new System.Drawing.Point(this.Width - singlewaypoint.Width, (this.myImageButton_Connect.Height * 3));
                singlewaypoint.TopMost = true;
                singlewaypoint.Show();
            }
            else
            {
                singlewaypoint.Visible = true;
            }
            this.map.SystemModeID = 0;
            if (this.map.markerTarget != null)
            {
                if (!this.map.MapTargetPoint.Markers.Contains(this.map.markerTarget))
                {
                    PointLatLng p = this.map.markerTarget.Position;
                    this.map.markerTarget.Position = Program.PointConversion.GetEarth2Mars(p);

                    this.map.MapTargetPoint.Markers.Add(this.map.markerTarget);
                    this.map.mapControl.Position = this.map.markerTarget.Position;
                }
            }
            if (this.map.markersOverlay.Markers.Count != 0)
            {
                this.map.waypoint_count = 0;
                this.map.markersOverlay.Markers.Clear();
                this.map.markersOverlay.Routes.Clear();
            }
            if (!this.isComOpen)
            {
                ListBoxAddMessage("当前无连接....");
            }
            else
            {
                this.mavlink_wps.SendToMode(1);
                Program.WriteLog("准备进入单点模式...");
                this.ListBoxAddMessage("正在发送单点模式指令....");
            }
            this.myImageButton_Singlepoint.SetColor(Color.Red);
            this.myImageButton_Multipoint.SetColor(Color.Gray);
            this.myImageButton_Iremote.SetColor(Color.Gray);
        }
        private void Singlepoint_Click(object sender, EventArgs e)
        {
            SingleMode();
        }
        private void MissionGo(){
            if (!this.isComOpen)
            {
                MessageBox.Show("当前无连接...");
                return;
            }
            this.ListBoxAddMessage("正在发送启动任务指令....");

            Program.WriteLog("发起WayPointGo命令");

            if (this.map.SystemModeID != 0)//如果不是单点模式
            {
                this.mavlink_wps.SendMissionStart(0, new PointLatLngAlt(), 0, 5);
            }
            else
            {
                try
                {
                    int height = int.Parse(this.singlewaypoint.textBox1.Text);
                    this.mavlink_wps.SendMissionStart(height, new PointLatLngAlt(), 0, 5);
                }
                catch
                {

                }
            }
        }
        private void WayPointGo(object sender, EventArgs e)
        {
            MissionGo();
        }
        public void MissionTakeOff() { 
            if (!this.isComOpen)
            {
               MessageBox.Show("当前无连接...");
                return;
            }
            if (satellitenumber < 6) {
                MessageBox.Show("GPS卫星数量小于6颗，无法起飞");
                return;
            }
            if(MessageBox.Show("是否确定要执行自动起飞?","自动起飞确认",MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                ListBoxAddMessage("正在发送起飞指令....");
                mavlink_wps.SendTakeOff();
                Program.WriteLog("发起起飞指令...");
            }        
        }
        private void Button_TakeOff(object sender, EventArgs e)
        {
            MissionTakeOff();
        }
        public void MissionToLand() { 
            if (!this.isComOpen){
                MessageBox.Show("当前无连接...");
                return;
            }
            this.ListBoxAddMessage("正在发送降落指令....");
            Program.WriteLog("用户发起降落指令...");
            this.mavlink_wps.SendToLand();        
        }
        private void Button_ToLand(object sender, EventArgs e)
        {
            MissionToLand();
        }
        public void HoverMode() {
            if (!this.isComOpen)
            {
                ListBoxAddMessage("当前无连接...");
            }
            else
            {
                this.mavlink_wps.SendLotiter(30, new PointLatLngAlt(), 0, 5);
                Program.WriteLog("准备进入悬停模式...");
            }
            this.myImageButton_Singlepoint.SetColor(Color.Gray);
            this.myImageButton_Multipoint.SetColor(Color.Gray);
            this.myImageButton_Iremote.SetColor(Color.Red);

            this.map.SystemModeID = 2;

            if (singlewaypoint == null || singlewaypoint.IsDisposed)
            { }
            else
            {
                singlewaypoint.Visible = false;
            }
            if (this.map.markerTarget != null)
            {
                if (this.map.MapTargetPoint.Markers.Contains(this.map.markerTarget))
                {
                    this.map.MapTargetPoint.Markers.Remove(this.map.markerTarget);
                }
            }
            if (this.map.markersOverlay.Markers.Count == 0)
            {
                if (this.map.drawingPoints.Count == 0) return;
                this.map.markersOverlay.Markers.Clear();
                this.map.markersOverlay.Routes.Clear();
                PointLatLngAlt Point_Max = new PointLatLngAlt(-90, -180);
                PointLatLngAlt Point_Min = new PointLatLngAlt(90, 180);
                int i = 0;
                foreach (PointLatLngAlt p in this.map.drawingPoints)
                {
                    this.map.mapAddClickMarker(new PointLatLngAlt(p.Lat, p.Lng)); //point_count++;
                    this.map.mapAdddataGridView(++i, new PointLatLngAlt(p.Lat, p.Lng, p.Alt));
                    Point_Max.Lat = (Point_Max.Lat > p.Lat) ? Point_Max.Lat : p.Lat;
                    Point_Max.Lng = (Point_Max.Lng > p.Lng) ? Point_Max.Lng : p.Lng;
                    Point_Min.Lat = (Point_Min.Lat < p.Lat) ? Point_Min.Lat : p.Lat;
                    Point_Min.Lng = (Point_Min.Lng < p.Lng) ? Point_Min.Lng : p.Lng;
                }
                PointLatLng point;
                if (Program.isMarsCoordinatesInChina)
                {
                    point = Program.PointConversion.GetEarth2Mars(new PointLatLng((Point_Max.Lat + Point_Min.Lat) / 2, (Point_Max.Lng + Point_Min.Lng) / 2));
                }
                else
                {
                    point = new PointLatLng((Point_Max.Lat + Point_Min.Lat) / 2, (Point_Max.Lng + Point_Min.Lng) / 2);
                }
                this.map.mapControl.Position = point;
                this.map.mapDrawWP(map.drawingPoints);
            }
        }
        private void myImageButton_Iremote_Click_1(object sender, System.EventArgs e)   //hover
        {
            HoverMode();
        }
        private void TextBox_SearchPlace_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if ((Keys)e.KeyChar == Keys.Enter)
            {
                GeoCoderStatusCode status = GeoCoderStatusCode.Unknow;
                GeocodingProvider gp = this.map.mapControl.MapProvider as GeocodingProvider;
                if (gp == null)
                {
                    gp = GMapProviders.OpenStreetMap as GeocodingProvider;
                }
                if (gp != null)
                {
                    var pt = gp.GetPoint(this.textBox_SearchPlace.Text, out status);
                    if (status == GeoCoderStatusCode.G_GEO_SUCCESS && pt.HasValue)
                    {
                        this.map.mapControl.Position = pt.Value;
                    }
                }
                if (status != GeoCoderStatusCode.G_GEO_SUCCESS)
                {
                    MessageBox.Show("Geocoder can't find: '" + textBox_SearchPlace.Text + "', reason: " + status.ToString(), "GMap.NET", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {

                }
            }
        }
        private void button_SetHome_Click(object sender, System.EventArgs e)
        {
            if (!this.isComOpen)
            {
                MessageBox.Show("当前无连接...");
                return;
            }

            if (this.myTabCon1.GetArmed())
            {
                ListBoxAddMessage("飞行器已经在启动，无法设置家");
            }
            else
            {
                this.map.SetHome(new PointLatLng(LatLngAlt.Lat,LatLngAlt.Lng));
                this.mavlink_wps.SendHome();

                Program.WriteLog("设置家:LAT:" + LatLngAlt.Lat.ToString("0.0000000") + " LNG:" + LatLngAlt.Lng.ToString("0.0000000"));
            }
        }
        private void button_camera_Click(object sender, System.EventArgs e)
        {
            if (!this.isComOpen)
            {
                MessageBox.Show("当前无连接...");
                return;
            }

            if (CameraStart)
            {
                this.mavlink_wps.SendToCamera(1);
                this.button_camera.Text = "停止摄像";

                Program.WriteLog("开始摄像...");
            }
            else {
                this.mavlink_wps.SendToCamera(0);
                this.button_camera.Text = "开始摄像";
                Program.WriteLog("停止摄像...");
            }
            CameraStart = !CameraStart;
        }
        private void gmTrackBar_pitch_ValueChange(object sender, System.EventArgs e)
        {
            {
                float pitch = ((gmTrackBar_pitch.Maximum - gmTrackBar_pitch.Value) / (float)gmTrackBar_pitch.Maximum);
                label_pitch.Text = "Pitch:" + (180 * pitch - 30).ToString("0");
            }

        }
        private void gmTrackBar_pitch_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)//左键
            {
                float pitch = ((gmTrackBar_pitch.Maximum - gmTrackBar_pitch.Value) / (float)gmTrackBar_pitch.Maximum);
                label_pitch.Text = "Pitch:" + (180 * pitch-30).ToString("0");
                if (!isComOpen)
                {
                 //   return;
                }else
                this.mavlink_wps.SendGimbal(pitch);

               // this.LatLngAlt.Lat = this.map.Center_point.Lat;
               // this.LatLngAlt.Lng = this.map.Center_point.Lng;
                CalcYuntaiTarget();
                this.map.mapControl.Invalidate();

            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (!isComOpen)
                {
                    return;
                }
                this.MenuStrip_Gimbal.Show(Cursor.Position);
            }
        }
        private void 水平云台ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            float angle = 1.0f / 6.0f;
            gmTrackBar_pitch.Value = (int)((gmTrackBar_pitch.Maximum - gmTrackBar_pitch.Minimum) * (1 - angle)) + gmTrackBar_pitch.Minimum;


            //0度
            if (this.isComOpen)

            this.mavlink_wps.SendGimbal(angle);
        }
        private void 竖直云台ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            float angle = (30.0f+30.0f)/180.0f;
            gmTrackBar_pitch.Value = (int)((gmTrackBar_pitch.Maximum - gmTrackBar_pitch.Minimum) * (1 - angle)) + gmTrackBar_pitch.Minimum;

            //x=(y+30)/180;
            //0度
            if (this.isComOpen)

            this.mavlink_wps.SendGimbal(angle);
        }
        private void 度ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            float angle = (45.0f + 30.0f) / 180.0f;
            gmTrackBar_pitch.Value = (int)((gmTrackBar_pitch.Maximum - gmTrackBar_pitch.Minimum) * (1 - angle)) + gmTrackBar_pitch.Minimum;
            if (this.isComOpen)

            this.mavlink_wps.SendGimbal(angle);

        }
        private void 度ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            float angle = (60.0f + 30.0f) / 180.0f;
            gmTrackBar_pitch.Value = (int)((gmTrackBar_pitch.Maximum - gmTrackBar_pitch.Minimum) * (1 - angle)) + gmTrackBar_pitch.Minimum;
            if (this.isComOpen)

            this.mavlink_wps.SendGimbal(angle);


        }
        private void 度ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            float angle = (90.0f + 30.0f) / 180.0f;
            gmTrackBar_pitch.Value = (int)((gmTrackBar_pitch.Maximum - gmTrackBar_pitch.Minimum) * (1 - angle)) + gmTrackBar_pitch.Minimum;
            
            if(this.isComOpen)
            this.mavlink_wps.SendGimbal(angle);

        }
        private void 度ToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            float angle = (120.0f + 30.0f) / 180.0f;
            gmTrackBar_pitch.Value = (int)((gmTrackBar_pitch.Maximum - gmTrackBar_pitch.Minimum) * (1 - angle)) + gmTrackBar_pitch.Minimum;
            if (this.isComOpen)

            this.mavlink_wps.SendGimbal(angle);

        }
        //MFD AP Compass
        private void CalCompass1_Click(object sender, System.EventArgs e)
        {
            if (!this.isComOpen)
            {
                MessageBox.Show("当前无连接...");
                return;
            }

            if (this.myTabCon1.GetArmed())
            {
                ListBoxAddMessage("飞行器已经在启动，无法校准");
            }
            else
            {
                ListBoxAddMessage("发送校准磁罗盘1命令");
                mavlink_wps.SendCalCompass1();
                Program.WriteLog("正在校准磁罗盘1...");
            }
        }
        //naza compass
        private void CalCompass2_Click(object sender, System.EventArgs e)
        {
            if (!this.isComOpen)
            {
                MessageBox.Show("当前无连接...");
                return;
            }

            if (this.myTabCon1.GetArmed())
            {
                ListBoxAddMessage("飞行器已经在启动，无法校准");
            }
            else
            {
                ListBoxAddMessage("发送校准磁罗盘2命令");
                mavlink_wps.SendCalCompass2();
                Program.WriteLog("正在校准磁罗盘2...");
            }
        }
        private void button1_Click_1(object sender, EventArgs e)
        {
            this.map.ClearTime();
            Program.WriteLog("清除电机转动时间...");
        }
        private void HiddenControl()
        {
            if (this.tabcon_mapshow.Controls.Contains(this.tabPage_datashow)) {
                this.tabcon_mapshow.Controls.Remove(this.tabPage_datashow);
            }

            CalCompass2.Text = "校准磁罗盘";

            this.CalCompass1.Visible = false;
            label1.Visible = false;
            label2.Visible = false;
            label3.Visible = false;
            label4.Visible = false;
            label5.Visible = false;

            this.textBox_crosstrackdistance.Visible = false;
            this.textBox_nextpoint.Visible = false;
            this.textBox_targetdistance.Visible = false;
            this.textBox_way2way.Visible = false;
            this.button_isArrive.Visible = false;

        }
        #region 有关航点规划的代码
        private void 生成航线ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (routeplanningform == null || routeplanningform.IsDisposed) {
                routeplanningform = new ProgressIndication.RoutePlanningForm();
                routeplanningform.TopMost = true;
                routeplanningform.PlanningDirection = SetRoutesPara;
                routeplanningform.UpdateHeight = PlanFormUpdateHeight;

                routeplanningform.StartPosition = FormStartPosition.Manual;
                routeplanningform.Location = new System.Drawing.Point((int)(this.Width - routeplanningform.Width * 1.08f), (this.myImageButton_Connect.Height * 4));
                routeplanningform.Replan = SetRePlan;
                routeplanningform.WaypointClick = waypointclick;
                routeplanningform.LinepointClick = linepointclick;
                routeplanningform.FormClose = RoutePlanningFromClose;

                UpdatePlanForm();

                routeplanningform.Show();
            }
        }

        private void 生成航线ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (routeplanningform == null || routeplanningform.IsDisposed)
            {
                routeplanningform = new ProgressIndication.RoutePlanningForm();
                routeplanningform.TopMost = true;
                routeplanningform.PlanningDirection = SetRoutesPara;
                routeplanningform.UpdateHeight = PlanFormUpdateHeight;
                routeplanningform.WaypointClick = waypointclick;
                routeplanningform.LinepointClick = linepointclick;

                routeplanningform.StartPosition = FormStartPosition.Manual;
                routeplanningform.Location = new System.Drawing.Point((int)(this.Width - routeplanningform.Width*1.08f), (this.myImageButton_Connect.Height *4));
                routeplanningform.Replan = SetRePlan;
                routeplanningform.FormClose = RoutePlanningFromClose;

                UpdatePlanForm();
                routeplanningform.Show();
            }
        }

        private void 生成航线ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (routeplanningform == null || routeplanningform.IsDisposed)
            {
                routeplanningform = new ProgressIndication.RoutePlanningForm();
                routeplanningform.TopMost = true;
                routeplanningform.PlanningDirection = SetRoutesPara;
                routeplanningform.Replan = SetRePlan;
                routeplanningform.UpdateHeight = PlanFormUpdateHeight;
                routeplanningform.StartPosition = FormStartPosition.Manual;
                routeplanningform.FormClose = RoutePlanningFromClose;
                routeplanningform.WaypointClick = waypointclick;
                routeplanningform.LinepointClick = linepointclick;

                UpdatePlanForm();
                routeplanningform.Location = new System.Drawing.Point((int)(this.Width - routeplanningform.Width * 1.08f), (this.myImageButton_Connect.Height * 4));
                routeplanningform.Show();
            }
        }

        void RoutePlanningFromClose() {
            this.map.LeftClick = null;
            this.routeplanningform = null;
            this.map.markersOverlay.Polygons.Clear();

        }
        void linepointclick(int linepointindex) {
            double ang = Math.Atan(this.point_K[linepointindex])/Math.PI*180;

            if (ang < 0) {
                ang += 180;
            }

            this.routeplanningform.SetDirection((int)ang);
        }
        void waypointclick(int waypointindex) {
            //this.ListBoxAddMessage("点击了Check..."+waypointindex);
            this.startIndex = waypointindex;
        }

        #endregion
        private void button_pitchplus_Click(object sender, EventArgs e)
        {
            Yuntailookeddown();
        }
        void Yuntailookeddown()
        {
            float pitch = ((gmTrackBar_pitch.Maximum - gmTrackBar_pitch.Value) / (float)gmTrackBar_pitch.Maximum);


            if (180 * pitch - 30 < 4 && 180 * pitch - 30 > 0) pitch = 0.195f;

            label_pitch.Text = "Pitch:" + (180 * pitch - 30).ToString("0"); 

            if (!isComOpen)
            {
                return;
            }
            pitch += 0.01f;
            gmTrackBar_pitch.Value = (int)(gmTrackBar_pitch.Maximum-pitch * gmTrackBar_pitch.Maximum);
            this.mavlink_wps.SendGimbal(pitch);
            label_pitch.Text = "Pitch:" + (180 * pitch - 30).ToString("0");
            Program.WriteLog("云台低头:" + (180 * pitch - 30).ToString("0"));
        }
        void Yuntailookedup() { 
            float pitch = ((gmTrackBar_pitch.Maximum - gmTrackBar_pitch.Value) / (float)gmTrackBar_pitch.Maximum);
            label_pitch.Text = "Pitch:" + (180 * pitch - 30).ToString("0"); 
            if (!isComOpen)
            {
                return;
            }
            pitch -= 0.01f;
            gmTrackBar_pitch.Value = (int)(gmTrackBar_pitch.Maximum - pitch * gmTrackBar_pitch.Maximum);
            label_pitch.Text = "Pitch:" + (180 * pitch - 30).ToString("0"); 
            this.mavlink_wps.SendGimbal(pitch);
            Program.WriteLog("云台抬头:" + (180 * pitch - 30).ToString("0"));        
        }
        private void button_pitchminus_Click(object sender, EventArgs e)
        {
            Yuntailookedup();
        }
        void PlanFormUpdateHeight(int height) {
            for (int i = 0; i < this.backupdrawpoint.Count; i++)
            {
                this.backupdrawpoint[i].Alt = height;
                this.routeplanningform.UpdateWayPointHeight(i, height);
                this.map.MapWPHeightUpdate(i, height);
            }
            foreach (PointLatLngAlt p in this.map.drawingPoints)
            {
                p.Alt = height;
            }
        }
        void SetRoutesPara(int angle,float distance) {
            if (this.map.drawingPoints.Count <4)
            {
                MessageBox.Show("必须有至少4个航点在地图上被设置...");
                return;
            }


            double R=6371000;//地球半径
            double r=R*Math.Cos(this.map.drawingPoints[0].Lat/180.0*Math.PI);//计算当前位置的纬度圆半径
            double perimeter_r=2*Math.PI*r;//计算当前纬度圆周长
            double linescale = 360000.0 / perimeter_r;//计算当前纬度上每一米代表的度数，扩大1000倍

            if (this.routeplanningform.camerafield && this.routeplanningform.isStartphoto)
            {
                int height = this.routeplanningform.GetHeight();
                int camerawidth = this.routeplanningform.GetCamerawidth();
                int cameracoincidencerate = this.routeplanningform.GetCoincidenceratewidth();

                double tan=Math.Tan((camerawidth / 2.0) / 180.0 * Math.PI);

                float cameramapwidth = (float)(height * tan);
                distance = cameramapwidth * (1 - cameracoincidencerate / 100.0f);

                this.routeplanningform.SetTextBoxDistance((int)distance);
            }

            double Coordinateslen = linescale * distance/1000.0;//计算线间距对应的坐标差，     

            if (angle == 180) return;

            if (isInplanning)
            {
                if (MessageBox.Show("当前已经进行了航线规划，是否继续进行规划?", "航线规划确认", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
                {
                    this.UpdatePlanForm();
                }
                else
                {
                    return;
                }
            }
            else
                isInplanning = true;//记录标志是否当前已经进行了航点规划




            threshold = Coordinateslen;
            xMove = threshold * (Math.Sin(angle / 180.0 * Math.PI));
            yMove = threshold * (Math.Cos(angle / 180.0 * Math.PI));
            RouteDirection_K = Math.Tan(angle / 180.0f * Math.PI);
            WayPointRoutesPlanMul();
        }
        void SetRePlan() {
            this.map.mapClearWP();
            this.map.markersOverlay.Polygons.Clear();
            isInplanning = false;

            this.map.ClearPhotoPoint();
            //this.map.drawingPoints = pointroutes;
            PointLatLngAlt Point_Max = new PointLatLngAlt(-90, -180);
            PointLatLngAlt Point_Min = new PointLatLngAlt(90, 180);
            int jj = 0;
            foreach (PointLatLngAlt p in backupdrawpoint)
            {

                this.map.drawingPoints.Add(p);
                if (this.map.SystemModeID == 1)
                {
                    this.map.mapAddClickMarker(new PointLatLngAlt(p.Lat, p.Lng)); //point_count++;
                    this.map.mapAdddataGridView(++jj, new PointLatLngAlt(p.Lat, p.Lng, p.Alt));
                }
                Point_Max.Lat = (Point_Max.Lat > p.Lat) ? Point_Max.Lat : p.Lat;
                Point_Max.Lng = (Point_Max.Lng > p.Lng) ? Point_Max.Lng : p.Lng;
                Point_Min.Lat = (Point_Min.Lat < p.Lat) ? Point_Min.Lat : p.Lat;
                Point_Min.Lng = (Point_Min.Lng < p.Lng) ? Point_Min.Lng : p.Lng;
            }
            PointLatLng point;
            if (Program.isMarsCoordinatesInChina)
            {
                point = Program.PointConversion.GetEarth2Mars(new PointLatLng((Point_Max.Lat + Point_Min.Lat) / 2, (Point_Max.Lng + Point_Min.Lng) / 2));
            }
            else
            {
                point = new PointLatLng((Point_Max.Lat + Point_Min.Lat) / 2, (Point_Max.Lng + Point_Min.Lng) / 2);
            }
            if (this.map.SystemModeID == 1)
            {
                this.map.mapControl.Position = point;
                this.map.mapDrawWP(map.drawingPoints);
            }
        }
        /// <summary>
        /// 更新航点规划点时使用
        /// </summary>
        /// <param name="anglestr">指明角度的字符串，东南西北区分</param>
        /// <param name="ang">具体的角度值</param>
        /// <param name="index">点的坐标从0开始</param>
        void updateanglestr(ref string anglestr,ref double ang,int index) {

            int index1 = (index + 1) >= this.map.drawingPoints.Count ? 0 : index + 1;

            if (this.map.drawingPoints[index].CompareLatGreater(this.map.drawingPoints[index1]) &&
                this.map.drawingPoints[index].CompareLngGreater(this.map.drawingPoints[index1])
                ) {//如果起始点坐标比结束点坐标经纬度都高
                if (ang > 45) {
                    anglestr = "南偏西";
                    ang = 90 - ang;
                }else{
                    anglestr = "西偏南";                    
                }    
            }else if (this.map.drawingPoints[index].CompareLatGreater(this.map.drawingPoints[index1]) &&
                this.map.drawingPoints[index].CompareLngLess(this.map.drawingPoints[index1])
            ){//起始点坐标纬度比结束点纬度高，但经度低
                if (-1*ang > 45)
                {
                    anglestr = "南偏东";
                    ang =90- Math.Abs(ang);
                }
                else { 
                    anglestr = "东偏南";
                    ang = -1*ang;                
                }
            }else if (this.map.drawingPoints[index].CompareLatLess(this.map.drawingPoints[index1]) &&
               this.map.drawingPoints[index].CompareLngLess(this.map.drawingPoints[index1])
                ){//起始点坐标比结束点坐标经纬度都低
                    if (ang > 45)
                    {
                        anglestr = "北偏东";
                        ang = 90 - ang;
                    }
                    else {
                        anglestr = "东偏北";
                    }
            }else if (this.map.drawingPoints[index].CompareLatLess(this.map.drawingPoints[index1]) &&
              this.map.drawingPoints[index].CompareLngGreater(this.map.drawingPoints[index1])
               ){//起始点纬度比目标点纬度低，但经度高
                   if ( -1*ang > 45)
                   {
                       anglestr = "北偏西";
                       ang = 90 + ang;
                   }
                   else {
                       anglestr = "西偏北";
                       ang = -1 * ang;
                   }
               }else if (this.map.drawingPoints[index].CompareLatEquals(this.map.drawingPoints[index1]) &&
             this.map.drawingPoints[index].CompareLngGreater(this.map.drawingPoints[index1])
                ){//在同一个纬度上,起始点经度大于结束点经度
                    anglestr = "正西方向";
                    ang = 0;

                }else if (this.map.drawingPoints[index].CompareLatEquals(this.map.drawingPoints[index1]) &&
                    this.map.drawingPoints[index].CompareLngLess(this.map.drawingPoints[index1])
               )
            {//在同一个纬度上,起始点经度小于结束点经度
                anglestr = "正东方向";
                ang = 0;
            }
            else if (this.map.drawingPoints[index].CompareLatGreater(this.map.drawingPoints[index1]) &&
            this.map.drawingPoints[index].CompareLngEquals(this.map.drawingPoints[index1])
               )
            {//在同一个经度上,起始点纬度大于结束点经度
                anglestr = "正南方向";
                ang = 0;
            }
            else if (this.map.drawingPoints[index].CompareLatLess(this.map.drawingPoints[index1]) &&
           this.map.drawingPoints[index].CompareLngEquals(this.map.drawingPoints[index1])
              )
            {//在同一个经度上,起始点纬度小于结束点经度
                anglestr = "正北方向";
                ang = 0;

            }
        }
        void MapLeftMouseClick(PointLatLng p)
        {
            if (this.routeplanningform != null) {
                //点击按钮添加
                if (this.map.drawingPoints.Count == 1)
                {
                    this.routeplanningform.SetWayPointPara(this.map.drawingPoints.Count,
                        (int)this.map.drawingPoints[this.map.drawingPoints.Count - 1].Alt, true);
                }
                else {
                    this.routeplanningform.SetWayPointPara(this.map.drawingPoints.Count,
                       (int)this.map.drawingPoints[this.map.drawingPoints.Count - 1].Alt, false);   
                }

                if (this.map.drawingPoints.Count == 3) {
                    CalcStraightSlope();

                    for (int i = 0; i < point_K.Count ; i++) {
                        double ang = Math.Atan(point_K[i]) / Math.PI * 180;
                        string direction = "";
                        int index = (i + 1) >= point_K.Count  ? 0 : i+1;

                        updateanglestr(ref direction, ref ang, i);

                        this.routeplanningform.SetLinePointPara(i + 1, (int)ang, direction, (i + 1).ToString() + "," + (index+1).ToString(), false);
                    }
                }
                else if (this.map.drawingPoints.Count > 3) {
                    int count = this.map.drawingPoints.Count;
                    //点击事件需要增加和更改最后的斜率事件
                    double x0 = this.map.drawingPoints[count-2].Lng;
                    double y0 = this.map.drawingPoints[count - 2].Lat;
                    double x1 = this.map.drawingPoints[count - 2 + 1].Lng;
                    double y1 = this.map.drawingPoints[count - 2 + 1].Lat;
                    double k = (y0 - y1) / (x0 - x1);
                    double b = (x1 * y0 - x0 * y1) / (x1 - x0);

                    this.routeplanningform.DeleteLineRow(this.point_K.Count-1);
                    this.point_B.RemoveAt(this.point_B.Count - 1);
                    this.point_K.RemoveAt(this.point_K.Count-1);
                    this.point_B.Add(b);
                    this.point_K.Add(k);

                    double ang = Math.Atan(k) / Math.PI * 180;
                    string direction = "";
                    updateanglestr(ref direction, ref ang, this.map.drawingPoints.Count-2);
                    this.routeplanningform.SetLinePointPara(this.point_K.Count,
                        (int)ang,direction,
                        this.point_K.Count.ToString()+","+(this.point_K.Count+1).ToString(),
                        false);

                    double x10 = this.map.drawingPoints[count - 1].Lng;
                    double y10 = this.map.drawingPoints[count - 1].Lat;
                    double x11 = this.map.drawingPoints[0].Lng;
                    double y11 = this.map.drawingPoints[0].Lat;
                    double k1 = (y10 - y11) / (x10 - x11);
                    double b1 = (x11 * y10 - x10 * y11) / (x11 - x10);

                    this.point_B.Add(b1);
                    this.point_K.Add(k1);

                    double ang1 = Math.Atan(k1) / Math.PI * 180;
                    string direction1 = "";
                    updateanglestr(ref direction1, ref ang1, this.map.drawingPoints.Count - 1);
                    this.routeplanningform.SetLinePointPara(this.point_K.Count,
                        (int)ang1, direction1,
                       ( this.point_K.Count).ToString() + ",1" ,
                        false);
                }
            }
        }
        /// <summary>
        /// 计算直线斜率
        /// </summary>
        void CalcStraightSlope() {
            point_K.Clear();
            point_B.Clear();

            if (this.map.drawingPoints.Count == 0) return;

            int i = 0;
            backupdrawpoint.Clear();
            for (i = 0; i < this.map.drawingPoints.Count - 1; i++)
            {
                double x0 = this.map.drawingPoints[i].Lng;
                double y0 = this.map.drawingPoints[i].Lat;
                double x1 = this.map.drawingPoints[i + 1].Lng;
                double y1 = this.map.drawingPoints[i + 1].Lat;
                double k = (y0 - y1) / (x0 - x1);
                double b = (x1 * y0 - x0 * y1) / (x1 - x0);

                point_K.Add(k);
                point_B.Add(b);

                backupdrawpoint.Add(new PointLatLngAlt(this.map.drawingPoints[i]));
            }

            backupdrawpoint.Add(new PointLatLngAlt(this.map.drawingPoints[i]));
            double x10 = this.map.drawingPoints[i].Lng;
            double y10 = this.map.drawingPoints[i].Lat;
            double x11 = this.map.drawingPoints[0].Lng;
            double y11 = this.map.drawingPoints[0].Lat;
            double k1 = (y10 - y11) / (x10 - x11);
            double b1 = (x11 * y10 - x10 * y11) / (x11 - x10);
            point_K.Add(k1);
            point_B.Add(b1);
        }
        /// <summary>
        /// 航点拖拽时执行此函数，
        /// </summary>
        /// <param name="index">航点在列表中的下标，从1开始</param>
        /// <param name="poiint">点在地图上的位置</param>
        void MouseDargChangWPPlanRoutes(int index, PointLatLng poiint) {
            if (this.routeplanningform != null) {
                if (this.isInplanning) {
                    return;
                }

                if (point_K.Count >= 3) {
                    PointLatLngAlt p1;
                    PointLatLngAlt p2;
                    PointLatLngAlt p3;

                    if (index - 1 == 0)//第一个点
                    {
                        p1 = this.map.drawingPoints[this.map.drawingPoints.Count-1];
                        p2 = this.map.drawingPoints[0];
                        p3 = this.map.drawingPoints[1];
                    }
                    else if (index == this.map.drawingPoints.Count)//最后一个点
                    {
                        p1 = this.map.drawingPoints[index - 2];
                        p2 = this.map.drawingPoints[index - 1];
                        p3 = this.map.drawingPoints[0]; 

                    }
                    else { 
                        p1 = this.map.drawingPoints[index - 2];
                        p2 = this.map.drawingPoints[index - 1];
                        p3 = this.map.drawingPoints[index]; 
                    }


                    double x0 = p1.Lng;
                    double y0 = p1.Lat;
                    double x1 = p2.Lng;
                    double y1 = p2.Lat;
                    double k1 = (y0 - y1) / (x0 - x1);
                    double b1 = (x1 * y0 - x0 * y1) / (x1 - x0);

                    double x10 = p2.Lng;
                    double y10 = p2.Lat;
                    double x11 = p3.Lng;
                    double y11 = p3.Lat;
                    double k2 = (y10 - y11) / (x10 - x11);
                    double b2 = (x11 * y10 - x10 * y11) / (x11 - x10);


                    if (index - 1 == 0)
                    {
                        point_K[point_K.Count - 1] = k1;
                        point_K[0] = k2;
                        point_B[point_B.Count - 1] = b1;
                        point_B[0] = b2;

                        double ang = (Math.Atan(k1) / Math.PI * 180);
                        string str = "";
                        this.updateanglestr(ref str, ref ang, this.map.drawingPoints.Count - 1);

                        double ang1 = (Math.Atan(k2) / Math.PI * 180);
                        string str1 = "";
                        this.updateanglestr(ref str1, ref ang1, 0);


                        //System.Diagnostics.Debug.WriteLine("P1.LAT:" + p1.Lat.ToString("0.0000000") + "P1.LNG:" + p1.Lng.ToString("0.0000000") +
                        //    "P2.LAT:" + p2.Lat.ToString("0.0000000") + "P2.LNG:" + p2.Lng.ToString("0.0000000") +
                        //    "P3.LAT:" + p3.Lat.ToString("0.0000000") + "P3.LNG:" + p3.Lat.ToString("0.0000000"));

                      //  System.Diagnostics.Debug.WriteLine("K1:"+k1.ToString("0.0000000")+"   ang:"+ang.ToString("0.00")+"\tK2:"+k2.ToString("0.0000000")+"   ang:"+ang1.ToString("0.00"));

                        if (ang < 0 || ang1 < 0) {
                            System.Diagnostics.Debug.WriteLine("小于零");
                        }

                        //更新界面
                        this.routeplanningform.UpdateLineAngle(this.point_K.Count-1, (int)ang,str);
                        this.routeplanningform.UpdateLineAngle(index - 1, (int)(ang1),str1);                         
                    }
                    else { 
                         point_K[index - 2] = k1;
                         point_K[index - 1] = k2;
                         point_B[index - 2] = b1;
                         point_B[index - 1] = b2;

                         double ang = (Math.Atan(k1) / Math.PI * 180);
                         string str = "";
                         this.updateanglestr(ref str, ref ang, index-2);

                         double ang1 = (Math.Atan(k2) / Math.PI * 180);
                         string str1 = "";
                         this.updateanglestr(ref str1, ref ang1, index - 1);
                        //更新界面
                        this.routeplanningform.UpdateLineAngle(index - 2, (int)(ang),str);
                        this.routeplanningform.UpdateLineAngle(index - 1, (int)(ang1),str1);                         

                    }
                }
            }
        }
        private void WayPointRoutesPlanMul() {
            if (this.map.drawingPoints.Count >= 4)
            {
                List<PointLatLngAlt> offsetwaypoints = new List<PointLatLngAlt>();
                List<double> offsetpoint_k = new List<double>();
                List<double> offsetpoint_b = new List<double>();

                double maxb = 0;
                double minb = 0;

                List<PointLatLngAlt> points = new List<PointLatLngAlt>();
                CalcStraightSlope();//计算直线斜率

                for (int j = startIndex; j < this.map.drawingPoints.Count; j++)
                {
                    offsetwaypoints.Add(this.map.drawingPoints[j]);
                    offsetpoint_k.Add(this.point_K[j]);
                    offsetpoint_b.Add(this.point_B[j]);

                    double b = this.map.drawingPoints[j].Lat-RouteDirection_K * this.map.drawingPoints[j].Lng;

                    if (j == startIndex)
                    {
                        maxb = b;
                        minb = b;
                    }
                    else {
                        maxb = maxb > b ? maxb : b;
                        minb = minb < b ? minb : b;
                    }
                }
                for(int j=0;j<startIndex;j++){
                    offsetwaypoints.Add(this.map.drawingPoints[j]);
                    offsetpoint_k.Add(this.point_K[j]);
                    offsetpoint_b.Add(this.point_B[j]);


                    double b = this.map.drawingPoints[j].Lat - RouteDirection_K * this.map.drawingPoints[j].Lng;
                    maxb = maxb > b ? maxb : b;
                    minb = minb < b ? minb : b;
                }
                int i;
                RouteIntercept_B = offsetwaypoints[0].Lat - RouteDirection_K * offsetwaypoints[0].Lng;

                //清除之前的规划路径
                pointroutes.Clear();
                this.map.markersRouteOverlay.Markers.Clear();
                this.map.markersRouteOverlay.Routes.Clear();

                int n = offsetwaypoints.Count;
                int leftlineIndex = n-1;
                int rightlineIndex = 0;
                int thresholdnum = 0;

                double AmountOfTranslation = 0;

                double b1 = offsetwaypoints[0].Lat - RouteDirection_K * offsetwaypoints[0].Lng;

                if (Math.Abs(maxb - b1) > Math.Abs(minb - b1))
                {//向上扫描
                    if (RouteDirection_K > 0)//0到90度
                    {
                        AmountOfTranslation = -1 * yMove - RouteDirection_K * xMove;
                    }
                    else {
                        AmountOfTranslation = yMove+RouteDirection_K * xMove;
                    }
                }
                else
                {//向下扫描
                    if (RouteDirection_K > 0)//0到90度
                    {
                        AmountOfTranslation = yMove + RouteDirection_K * xMove;
                    }
                    else {
                        AmountOfTranslation = -1 * yMove - RouteDirection_K * xMove;
                    }
                }

                //开始规划   以开始两个点线斜率为线方向
                for (i = 0; ; ) {
                    if (rightlineIndex == leftlineIndex) break;

                    double bb = RouteIntercept_B - thresholdnum * (AmountOfTranslation);


                    if (i % 4 == 1 || i % 4 == 0)
                    {

                        double K1 = RouteDirection_K;
                        double B1 = bb;
                        double K2 = offsetpoint_k[leftlineIndex];
                        double B2 = offsetpoint_b[leftlineIndex];

                        double x0 = (B2 - B1) / (K1 - K2);
                        double y0 = (B1 * K2 - B2 * K1) / (K2 - K1);

                        //线段端点坐标值
                        PointLatLngAlt pointdotA;
                        PointLatLngAlt pointdotB;

                        if (leftlineIndex >= n - 1)
                        {
                            pointdotA = offsetwaypoints[n - 1];
                            pointdotB = offsetwaypoints[0];
                        }
                        else {
                            pointdotA = offsetwaypoints[leftlineIndex];
                            pointdotB = offsetwaypoints[leftlineIndex + 1];
                        }

                        if (x0 - pointdotA.Lng < 0.0000001 && x0 - pointdotA.Lng > -0.0000001) {
                            x0 = pointdotA.Lng;
                            y0 = pointdotA.Lat;
                        }
                        if (x0 - pointdotB.Lng < 0.0000001 && x0 - pointdotB.Lng > -0.0000001)
                        {
                            x0 = pointdotB.Lng;
                            y0 = pointdotB.Lat;
                        }
                        if (((y0 < pointdotA.Lat && y0 < pointdotB.Lat)||
                            (y0 > pointdotA.Lat && y0 > pointdotB.Lat)) && 
                            leftlineIndex>=1) {
                            leftlineIndex--;
                            continue;
                        }
                        pointroutes.Add( new PointLatLng(y0, x0));
                    }
                    else {
                      //  double bb = RouteIntercept_B - thresholdnum * yMove - thresholdnum*RouteDirection_K * xMove;
                        double K1 = RouteDirection_K;
                        double B1 = bb;
                        double K2 = offsetpoint_k[rightlineIndex];
                        double B2 = offsetpoint_b[rightlineIndex];
                        double x0 = (B2 - B1) / (K1 - K2);
                        double y0 = (B1 * K2 - B2 * K1) / (K2 - K1);
                        //线段端点坐标值
                        PointLatLngAlt pointdotA = offsetwaypoints[rightlineIndex];
                        PointLatLngAlt pointdotB = offsetwaypoints[rightlineIndex + 1];
                        if (x0 - pointdotA.Lng < 0.0000001 && x0 - pointdotA.Lng > -0.0000001)
                        {
                            x0 = pointdotA.Lng;
                            y0 = pointdotA.Lat;
                        }
                        if (x0 - pointdotB.Lng < 0.0000001 && x0 - pointdotB.Lng > -0.0000001)
                        {
                            x0 = pointdotB.Lng;
                            y0 = pointdotB.Lat;
                        }
                        if (((y0 < pointdotA.Lat && y0 < pointdotB.Lat) ||
                            (y0 > pointdotA.Lat && y0 > pointdotB.Lat)) && 
                            rightlineIndex < n - 1)
                        {
                            rightlineIndex++;
                            continue;
                        }
                        pointroutes.Add(new PointLatLng(y0, x0));
                    }
                    if (i > 5000) {
                        MessageBox.Show("您所选择的区域无法生成航点规划线路图");
                        return;
                    }
                    if (i % 2 == 0) { thresholdnum++; }

                    i++;
                }

                this.map.markersRouteOverlay.Routes.Clear();
                this.map.mapClearWP();
                //this.map.drawingPoints = pointroutes;
                PointLatLngAlt Point_Max = new PointLatLngAlt(-90, -180);
                PointLatLngAlt Point_Min = new PointLatLngAlt(90, 180);
                int jj = 0;
                double sdistance = 0;
                int heig = this.routeplanningform.GetHeight();
                foreach (PointLatLng p in pointroutes)
                {
                    PointLatLngAlt pp = new PointLatLngAlt(p);
                    if (heig != 0) pp.Alt = heig;

                    if (jj == pointroutes.Count - 1)
                    {
                        double s = this.map.Distance(pointroutes[jj].Lat, pointroutes[jj].Lng, pointroutes[0].Lat, pointroutes[0].Lng);
                        sdistance += s;
                    }
                    else {
                        double s = this.map.Distance(pointroutes[jj].Lat, pointroutes[jj].Lng, pointroutes[jj + 1].Lat, pointroutes[jj+1].Lng);
                        sdistance += s;              
                    }
                    this.map.drawingPoints.Add(pp);
                    if (this.map.SystemModeID == 1)
                    {
                        this.map.mapAddClickMarker(new PointLatLngAlt(p.Lat, p.Lng)); //point_count++;
                        this.map.mapAdddataGridView(++jj, new PointLatLngAlt(p.Lat, p.Lng, pp.Alt));
                    }
                    Point_Max.Lat = (Point_Max.Lat > p.Lat) ? Point_Max.Lat : p.Lat;
                    Point_Max.Lng = (Point_Max.Lng > p.Lng) ? Point_Max.Lng : p.Lng;
                    Point_Min.Lat = (Point_Min.Lat < p.Lat) ? Point_Min.Lat : p.Lat;
                    Point_Min.Lng = (Point_Min.Lng < p.Lng) ? Point_Min.Lng : p.Lng;
                }

                this.map.DrawPolygons(this.backupdrawpoint);

                this.routeplanningform.WriteTotalDistance((int)sdistance);
                PointLatLng point;
                if (Program.isMarsCoordinatesInChina)
                {
                    point = Program.PointConversion.GetEarth2Mars(new PointLatLng((Point_Max.Lat + Point_Min.Lat) / 2, (Point_Max.Lng + Point_Min.Lng) / 2));
                }
                else
                {
                    point = new PointLatLng((Point_Max.Lat + Point_Min.Lat) / 2, (Point_Max.Lng + Point_Min.Lng) / 2);
                }
                if (this.map.SystemModeID == 1)
                {
                    this.map.mapControl.Position = point;
                    this.map.mapDrawWP(map.drawingPoints);
                }
                if (this.routeplanningform.isStartphoto)
                {
                    if (this.routeplanningform.distanceset) {
                        int height = this.routeplanningform.GetHeight();//飞行器高度
                        int distan = this.routeplanningform.GettTakePhotoDistance();//间距点
                        int routesangle = this.routeplanningform.routesangle;//飞行器巡航时的航向角度
                        if (distan == -1)
                        {
                            SetRePlan();
                            return;
                        }
                        CalcTakePhotoPoint(height, distan, routesangle);                    
                    }
                    if (this.routeplanningform.camerafield) {
                        int height = this.routeplanningform.GetHeight();//飞行器高度
                        int routesangle = this.routeplanningform.routesangle;//飞行器巡航时的航向角度

                        double length = (this.routeplanningform.GetHeight() * Math.Tan((this.routeplanningform.GetCameraheight() / 2.0) / 180.0 * Math.PI));
                        int distan = (int)(length * (1 - this.routeplanningform.GetCoincidencerateheight() / 100.0f));

                        this.routeplanningform.SetPhotoDistance(distan);

                        CalcTakePhotoPoint(height, distan, routesangle);  
                    }
                }
            }
            else {
                MessageBox.Show("航点规划生成航点必须保证有四个以上航点被设置!!!");
            }
        }


        void CalcTakePhotoPoint(int flyheight,int distance,int flyangle)
        {
//            int cameraange = this.routeplanningform.cameraangle;//相机视野角度
            int height = flyheight;//飞行器高度
            int distan = distance;//间距点
            int routesangle = flyangle;//飞行器巡航时的航向角度


            double scalelng = distan * Math.Cos(routesangle / 180 * Math.PI) * 180 / (Math.PI * 6371000 * Math.Cos(pointroutes[0].Lat / 180 * Math.PI));
            this.map.ClearPhotoPoint();
            int index = 0;
            for (int i = 0; i < this.pointroutes.Count - 1; i++)
            {
                double distancetwopoint = this.map.Distance(pointroutes[i].Lat, pointroutes[i].Lng, pointroutes[i + 1].Lat, pointroutes[i + 1].Lng);
                if (distancetwopoint > distan)
                {

                    double line_k = (pointroutes[i].Lat - pointroutes[i + 1].Lat) / (pointroutes[i].Lng - pointroutes[i + 1].Lng);
                    double line_b = (pointroutes[i + 1].Lng * pointroutes[i].Lat - pointroutes[i + 1].Lat * pointroutes[i].Lng) /
                        (pointroutes[i + 1].Lng - pointroutes[i].Lng);

                    if (pointroutes[i].Lng >= pointroutes[i + 1].Lng)
                    {
                        for (double starlng = pointroutes[i].Lng; starlng >= pointroutes[i + 1].Lng; starlng -= scalelng)
                        {
                            PointLatLng p = new PointLatLng();
                            p.Lng = starlng;
                            p.Lat = starlng * line_k + line_b;

                            index++;
                            this.map.DrawPhotoPoint(index, p);
                        }
                    }
                    else
                    {
                        for (double starlng = pointroutes[i].Lng; starlng <= pointroutes[i + 1].Lng; starlng += scalelng)
                        {
                            PointLatLng p = new PointLatLng();
                            p.Lng = starlng;
                            p.Lat = starlng * line_k + line_b;

                            index++;
                            this.map.DrawPhotoPoint(index, p);
                        }
                    }
                }
            }
        }



        private void CalcYuntaiTarget() {
            double yaw = ang.yaw;   //飞行器偏航角度
            //double pitch = 0;     //飞行器俯仰角度
            //double roll = 0 ;     //飞行器横滚角度

            double lat = LatLngAlt.Lat;//飞行器纬度
            double lng = LatLngAlt.Lng;//飞行器经度
            double alt = LatLngAlt.Alt;//飞行器高度

            double yuntaiangle = 180*((gmTrackBar_pitch.Maximum - gmTrackBar_pitch.Value) / (float)gmTrackBar_pitch.Maximum)-30;
            double cont = Math.PI / 180;

            if (yuntaiangle < 1 && yuntaiangle >-1) {
                this.map.YuntaiTargetLocation = "云台水平，无目标";

                TargetPoint.Lat = 4000;
                TargetPoint.Lng = 4000;

                return;
            }
            else if (yuntaiangle == 90) {
                this.map.YuntaiTargetLocation = "目标处在正下方";
                TargetPoint.Lat = lat;
                TargetPoint.Lng = lng;
                return;
            }


            //云台俯仰角度
            ///假设只有偏航会对云台角度产生影响，计算目标点
            ///
            double yawTargetlat=lat ;
            double yawTagretlng=lng;

            double HD = alt / Math.Tan(yuntaiangle * cont);
            double XD = HD;
            double YD = HD;

            double SinYaw = Math.Sin(yaw * cont);
            double CosYaw = Math.Cos(yaw * cont);
            double ScaleLng = 0.000008993216 / Math.Cos(lat * cont);   // 180 / (Math.PI * 6371000) / Math.Cos(lat);
            double ScaleLat = 0.000008993216;                   //180 / (Math.PI * 6371000);

            double Dlat = 0;
            double Dlng = 0;

            XD = HD * SinYaw;
            YD = HD * CosYaw;

            Dlat = ScaleLat * YD;
            Dlng = ScaleLng * XD;

            yawTargetlat = lat + Dlat;
            yawTagretlng = lng + Dlng;

            TargetPoint.Lat = yawTargetlat;
            TargetPoint.Lng = yawTagretlng;

            if (yuntaiangle > -1 && yuntaiangle<1)
            {
                this.map.YuntaiTargetLocation = "云台水平，无目标";

                TargetPoint.Lat = 4000;
                TargetPoint.Lng = 4000;
            }
            else if (yuntaiangle<0 )
            {
                this.map.YuntaiTargetLocation = "无目标";

                TargetPoint.Lat = 4000;
                TargetPoint.Lng = 4000;
            }
            else if (yuntaiangle < 91 && yuntaiangle > 89) {
                this.map.YuntaiTargetLocation = "目标处在正下方";
                TargetPoint.Lat = lat;
                TargetPoint.Lng= lng;
            }
            else
            {
                this.map.YuntaiTargetLocation = "目标经度:" + yawTagretlng.ToString("0.0000000") + " 目标纬度:" + yawTargetlat.ToString("0.0000000");
            }
        }
        public void YuntaiTargetMarker() {
            if (!this.isComOpen) {
                MessageBox.Show("未开启连接，无法获取目标位置");
            }
            try
            {
                if (TargetPoint.Lat > 360 && TargetPoint.Lng > 360)
                {
                    MessageBox.Show("当前云台俯仰角度无效,无目标角度");
                    return;
                }
                PointLatLngAlt p = new PointLatLngAlt(TargetPoint);
                this.addBookMark(p);
            }
            catch
            {

            }
        }
        void imageButton_Marker_MouseClick(object sender, MouseEventArgs e)
        {
            YuntaiTargetMarker();
        }
        private void 插入航点ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (!this.isComOpen)
            {
                MessageBox.Show("当前无连接");
                return;
            }
            if (map.drawingPoints.Count > 0)
            {
                this.ListBoxAddMessage("正在发送航点....");

                Program.WriteLog("点击了上传航点按钮，正准备发送航点...");

                drawPoint = map.getWPsFromDataView();
                BackGroundOperation = 0;
                System.ComponentModel.BackgroundWorker m_BackgroundWorker = new System.ComponentModel.BackgroundWorker();
                InitializeBackgoundWorker(m_BackgroundWorker);
                m_BackgroundWorker.RunWorkerAsync();
            }
            else
            {
                MessageBox.Show("航点数量必须大于1个");
            }
        }
        private void 下载航点ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!this.isComOpen)
            {
                MessageBox.Show("当前无连接");
                return;
            }
            Program.WriteLog("点击了下载航点按钮，正准备下载航点...");

            BackGroundOperation = 1;

            System.ComponentModel.BackgroundWorker m_BackgroundWorker = new System.ComponentModel.BackgroundWorker();
            InitializeBackgoundWorker(m_BackgroundWorker);
            m_BackgroundWorker.RunWorkerAsync();
        }
        private void 颜色ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Configform configform = new Configform();
            configform.SetParaColorMouseLocation(this.map.colorMouseLocation);
            configform.SetParaColorTargetLocation(this.map.colorTargetLocation);
            configform.SetParaColorZoom(this.map.colorZoom);
            configform.SetParaColorScale(this.map.colorScale);
            configform.SetParaColorTrack(this.map.colorTrack);
            configform.SetParaColorRoutes(this.map.colorRoutes);

            configform.SetColorMouseLocation = SetColorMouseLocation;
            configform.SetColorRoutes = SetColorRoutes;
            configform.SetColorScale = SetColorScale;
            configform.SetColorTargetLocation = SetColorTargetLocation;
            configform.SetColorTrack = SetColorTrack;
            configform.SetColorZoom = SetColorZoom;

            configform.TopMost = true;
            configform.Show();
        }
        #region 颜色管理器
        void SetColorMouseLocation(Color colorMouseLocation)
        {
            this.map.colorMouseLocation = colorMouseLocation;
            this.map.mapControl.Invalidate();
        }
        void SetColorRoutes(Color colorRoutes)
        {
            this.map.colorRoutes = colorRoutes;
            foreach (GMapRoute route in this.map.markersOverlay.Routes)
            {
                route.Stroke.Color = colorRoutes;
            }
            this.map.mapControl.Invalidate();
        }
        void SetColorScale(Color colorScale)
        {
            this.map.colorScale = colorScale;
            this.map.mapControl.Invalidate();
        }
        void SetColorTargetLocation(Color colorTargetLocation)
        {
            this.map.colorTargetLocation = colorTargetLocation;
            this.map.mapControl.Invalidate();
        }
        void SetColorTrack(Color colorTrack)
        {
            this.map.colorTrack = colorTrack;
            foreach (GMapRoute route in this.map.MapCenterTail.Routes)
            {
                route.Stroke.Color = colorTrack;
            }
            this.map.mapControl.Invalidate();
        }
        void SetColorZoom(Color colorZoom)
        {
            this.map.colorZoom = colorZoom;
            this.map.mapControl.Invalidate();
        }
        #endregion
        private void 关于ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.isComOpen) return;
            MessageBox.Show("本产品由广州长天航空科技有限公司独立研究开发完成,\r\n 公司致力于生产专业级多功能航空无人飞行器。如在使\r\n用中有任何问题请及时联系我们!");
        }
        private void 联系我们ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.isComOpen) return;
            MessageBox.Show("网址:http://spacearrow.com \r\n邮箱:yuanqz@spacearrow.com");

        }
        #region 图标管理器
        private void 图标ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IconForm form = new IconForm();
            form.SetParaWpBitmap(this.map.BitmapWpOrigin, this.map.BitmapWpMarker);
            form.SetParaPlaneBitmap(this.map.BitmapPlaneOrgin,this.map.BitmapPlane);
            form.SetParaBookBitmap(this.map.BitmapBookMarkerOrigin,this.map.BitmapBookMarker);
            form.SetParaHomeBitmap(this.map.BitmapHomeOrigin,this.map.BitmapHome);
            form.SetHomeBitmap = SetHomeBitmap;
            form.SetPlaneBitmap = SetPlaneBitmap;
            form.SetWpBitmap = SetWpMarkerBitmap;
            form.SetBookBitmap = SetBookMarkerBitmap;
            form.TopMost = true;
            form.Show();
        }
        public void SetHomeBitmap(Bitmap bit, bool isUseFile, string filepath)
        {
            this.map.BitmapHome = bit;
            this.map.SetHome(this.map.homeLocation);

            this.serialize.isIconHomeUseFile = isUseFile;
            if (isUseFile) {
                this.serialize.iconhome.filepath = filepath;
                this.serialize.iconhome.iconwidth = bit.Width;
                this.serialize.iconhome.iconheight = bit.Height;
            }

        }
        public void SetPlaneBitmap(Bitmap bit, bool isUseFile, string filepath)
        {
            this.map.BitmapPlane = bit;
            this.map.mapControl_SetPlanePosition(this.map.mapPlanePoint.Lng, this.map.mapPlanePoint.Lat, 30, this.map.planeInmap_yaw);

            this.serialize.isIconPlaneUseFile = isUseFile;
            if (isUseFile)
            {
                this.serialize.iconplane.filepath = filepath;
                this.serialize.iconplane.iconwidth = bit.Width;
                this.serialize.iconplane.iconheight = bit.Height;
            }
        }
        public void SetWpMarkerBitmap(Bitmap bit, bool isUseFile, string filepath)
        {
            this.map.BitmapWpMarker = bit;
            this.map.waypoint_count = 0;
            this.map.markersOverlay.Markers.Clear();
            foreach(PointLatLngAlt p in this.map.drawingPoints){
                this.map.mapAddClickMarker(new PointLatLngAlt(p.Lat, p.Lng)); //point_count++;
            }

            this.serialize.isIconWpMarkerUseFile = isUseFile;
            if (isUseFile)
            {
                this.serialize.iconwpmarker.filepath = filepath;
                this.serialize.iconwpmarker.iconwidth = bit.Width;
                this.serialize.iconwpmarker.iconheight = bit.Height;
            }
        }
        public void SetBookMarkerBitmap(Bitmap bit, bool isUseFile, string filepath)
        {
            this.map.BitmapBookMarker = bit;
            this.map.MapBookMark.Markers.Clear();
            for (int i = 0; i < this.BookMark.Count; i++) {
                this.map.DrawBookMark(BookMark[i],i+1);
            }

            this.serialize.isIconBookMarkerUseFile = isUseFile;
            if (isUseFile)
            {
                this.serialize.iconbookmarker.filepath = filepath;
                this.serialize.iconbookmarker.iconwidth = bit.Width;
                this.serialize.iconbookmarker.iconheight = bit.Height;
            }
        }
        #endregion
        private void 尺子ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.map.isMeasureDistance)
            {
                this.map.isMeasureDistance = false;
                this.尺子ToolStripMenuItem.Checked = false;
            }
            else {
                this.map.isMeasureDistance = true;
                this.尺子ToolStripMenuItem.Checked = true;
            }
            this.map.measureOverlay.Routes.Clear();
            this.map.measurepoint.Clear();
        }
        private void 缓存路径ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FilePathManager filepathform = new FilePathManager();
            filepathform.SetCachePath(System.Windows.Forms.Application.StartupPath);
            filepathform.SetDataPath(datafilepath);
            filepathform.SetLogPath(Program.currentpath);
            filepathform.SetDataFilePath = SetDataFilePath;
            filepathform.SetLogFilePath = SetLogFilePath;
            filepathform.ShowDialog();
        }
        void SetLogFilePath(string filepath)
        {
            if (serialize == null) return;
            this.serialize.logfilepath = filepath;
        }
        void SetDataFilePath(string filepath){
            if (serialize == null) return;
            this.serialize.datafilepath = filepath;
        }

        private void englishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo("en");
            ApplyResource();
        }

        private void 中文ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo("zh-CN");
            ApplyResource();
        }

        private void 语言ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void ApplyResource()
        {
            System.ComponentModel.ComponentResourceManager res = new ComponentResourceManager(typeof(Form1));
            foreach (System.Windows.Forms.Control ctl in Controls)
            {
                res.ApplyResources(ctl, ctl.Name);
            }

            //菜单
            foreach (ToolStripMenuItem item in this.menuStrip1.Items)
            {
                res.ApplyResources(item, item.Name);
                foreach (ToolStripMenuItem subItem in item.DropDownItems)
                {
                    res.ApplyResources(subItem, subItem.Name);
                }
            }

            //Caption
            res.ApplyResources(this, "$this");
        }

        private void button_yaw_left_Click(object sender, EventArgs e)
        {
            if (!this.isComOpen)
            {
                MessageBox.Show("当前无连接");
                return;
            }
            mavlink_wps.SendYaw(1);
            ListBoxAddMessage("左旋转微调...");
            Program.WriteLog("左旋转微调...");
        }

        private void button_yaw_right_Click(object sender, EventArgs e)
        {
            if (!this.isComOpen)
            {
                MessageBox.Show("当前无连接");
                return;
            }
            mavlink_wps.SendYaw(0);
            ListBoxAddMessage("右旋转微调...");
            Program.WriteLog("右旋转微调...");
        }

    }
}