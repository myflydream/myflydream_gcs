using System;
using System.Collections.Generic;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using map;
using System.Windows.Forms;
using System.Drawing;
using SpaceArrow;

/*
 * 1、map control（GMapControl） :用于图形显示
 * 2、Overlay（GMapOverlay）：地图上的一层，用于记录站点，商店等信息
 * 3、Marker（GMapMarker）:
 * 4、route(GMapRoute):
 * 5、
 * 
 * 1. What is the map control (GMapControl)? This is the control which renders the map. 
 * 2. What is an Overlay (GMapOverlay)? This is a layer on top of the map control. You can have several layers on top of a map, each layer representing, say, a route with stops, a list of stores etc.
 * 3. What are Markers (GMapMarker)? These are the points on a layer, each representing a specific geo location (Lat,Lon) e.g. each drop point on a route.
 * 4. What is a route (GMapRoute)? This is the path or direction between two or more points.
 * 5. WTF are tiles? – well here is something to read
 * 
 */

namespace EarthStationMap
{
    class MapShow
    {
        public GMapControl mapControl;//地图控件

        public GMapOverlay markersOverlay = new GMapOverlay("WPMarker");         //用于绘制航点  marker routes
        public GMapOverlay MapCenterTail = new GMapOverlay("飞机飞行");          //用于绘制无人机尾线    marker routes
        public GMapOverlay MapBookMark = new GMapOverlay("绘制地图书签层");      //用于绘制地图书签标志  marker
        public GMapOverlay MapTargetPoint = new GMapOverlay("直达航点层");       //用于绘制直达航点的Marker  marker
        public GMapOverlay markersRouteOverlay = new GMapOverlay("航点规划线路层"); //marker routes
        public GMapOverlay measureOverlay = new GMapOverlay("距离测量层");//
        public GMapOverlay photopointoverlay = new GMapOverlay("拍照点层");//用于Marker显示拍照点


        public List<PointLatLngAlt> drawingPoints = new List<PointLatLngAlt>();  //绘制好的航点的坐标点集,地球坐标系
        //委托 
        public delegate void mapControlMouseClickDele(object sender, MouseEventArgs e);
        public mapControlMouseClickDele MouseRightClick = null;//鼠标右键点击地图
        public delegate void SetModify(int colindex, int rowindex);
        public delegate void MarkerMouseClick(GMapMarker item, MouseEventArgs e);
        public MarkerMouseClick WPMarkerRightClick=null;
        public MarkerMouseClick BookMarkMarkerRightClick = null;
        public MarkerMouseClick MarkerLeftClick = null;
        public delegate void MouseDragMarkerDele(int index, PointLatLng point);
        public delegate void DirectPointProcessDele(PointLatLng directPoint);
        public DirectPointProcessDele DirectPointProcess = null;
        public DirectPointProcessDele LeftClick = null;
        public MouseDragMarkerDele MouseDragBookMarker = null;
        public delegate void showMessageDele(string str);

        public bool isMeasureDistance = false;  //尺子工具标识
        public List<PointLatLng> measurepoint = new List<PointLatLng>();//用于存储使用尺子工具时的点

        private bool isMouseDown = false;//是否在地图控件上按下了鼠标
        private bool isCanDragMarker = false;//是否可以拖拽Marker
        private bool isMouseInMarker = false;//是否鼠标在Marker范围内

        public Color colorScale = Color.LightYellow;      //地图比例尺颜色
        public Color colorTargetLocation = Color.White;   //目标位置颜色
        public Color colorMouseLocation = Color.White;    //鼠标位置颜色
        public Color colorZoom = Color.Red;               //缩放比例颜色
        public Color colorTrack = Color.Blue;             //航迹颜色
        public Color colorRoutes = Color.Red;             //航线颜色

        public Bitmap BitmapWpMarker;//可更改航点图标中转
        public Bitmap BitmapWpOrigin = new Bitmap(20, 20); //原始航点图标
        public Bitmap BitmapHomeOrigin = new Bitmap(global::SpaceArrow.Properties.Resources.Home);  //原始家图标
        public Bitmap BitmapHome;
        public Bitmap BitmapPlaneOrgin;
        public Bitmap BitmapPlane;
        public Bitmap BitmapBookMarkerOrigin;
        public Bitmap BitmapBookMarker;

        public PointLatLng mapCenterPoint = new PointLatLng(23.1693223959252, 113.449287414551); //地图中心标识
        public PointLatLng mapPlanePoint = new PointLatLng();
        public PointLatLng homeLocation = new PointLatLng();//家地址
        private GMarkerGoogle mapPlaneMarker;//飞行器Marker
        public float planeInmap_yaw;//飞行器在地图上的航线角

        private DateTime timemousedown;//鼠标按下时事件记录，用于区分鼠标点击和鼠标拖动两个动作。
        private int timemillSecond = 200;//如果鼠标按下事件超过这个值(单位:毫秒),鼠标点击事件将无效
        private int distancebe = 30;//如果鼠标按下时坐标和点击事件产生时坐标之间的距离小于这个值，认为是鼠标点击事件
        private bool isDrawWayPoint = true;//是否可以进行地图操作
        private int mousepress_x = 0, mousepress_y = 0;//鼠标按下时的坐标值
        private System.Windows.Forms.DataGridView dataGridView = null;//存储航点的列表

        public PointLatLng MouseLocation = new PointLatLng();//鼠标移动时在地图上的经纬度信息
        public int waypoint_count = 0;//指向航点的编号

        private GMapMarker currentEnterMarker;//当前鼠标进入的地图Marker标识
        private int planemarker_radius;//飞行器图标的半径
        public byte SystemModeID = 1;//用以区别单点模式和多点模式
        public bool isRemoveToPosition = false;//地图跟随标识

        public PointLatLngAlt MouseClickPoint = new PointLatLngAlt();
        public  DateTime dateArm = DateTime.Now;      //起始时间
        public bool isArm = false;//电机是否转动
        public DateTime dataarmNow = DateTime.Now;   //当前时间
        public DateTime ArmStart = DateTime.Now;
        public bool isFirstSet = false;
        public bool isStartTime=false;
        public TimeSpan d = new TimeSpan();
        public TimeSpan dStart = new TimeSpan();
        public void SetisArm(bool arm) {
            if (isArm == false && arm == true && isFirstSet==false)
            { 
                dateArm = DateTime.Now;
                dataarmNow = DateTime.Now;
                isFirstSet = true;
                isStartTime = true;
            }
            if (arm == false)
            {
                isStartTime = false;
                dStart = d;
            }
            else {
                dateArm = ArmStart;
                isStartTime = true;
                isFirstSet = true;
            }
            isArm = arm;
        }
        public bool isWayPointplan = false;
        public GMapMarker markerTarget;
        PointLatLng targetLocation = new PointLatLng();
        Bitmap BitmapTarget = new Bitmap(20, 20);//单点模式下，全局目标航点自定义图标
        public MapShow(GMapControl map)
        {
            //获取相关对象
            this.mapControl = map;
            mapControl.CacheLocation = System.Windows.Forms.Application.StartupPath + "\\GMapCache\\高德平面地图\\"; //缓存位置
            mapControl.MapProvider = AMapProvider.Instance;                             //默认高德平面地图

            mapControl.Manager.Mode = AccessMode.ServerAndCache;

            mapControl.MinZoom = 1;                                                     //最小比例
            mapControl.MaxZoom = 23;                                                    //最大比例
            mapControl.Zoom = 15;                                                       //当前比例
            mapControl.ShowCenter = false;                                              //不显示中心十字点
            mapControl.DragButton = System.Windows.Forms.MouseButtons.Left;             //左键拖拽地图

            mapControl.Overlays.Add(photopointoverlay);
            mapControl.Overlays.Add(markersRouteOverlay);
            mapControl.Overlays.Add(MapBookMark);
            mapControl.Overlays.Add(MapTargetPoint);
            mapControl.Overlays.Add(markersOverlay);
            mapControl.Overlays.Add(MapCenterTail);
            mapControl.Overlays.Add(measureOverlay);

            mapControl.MouseClick += new MouseEventHandler(mapControl_MouseClick);
            mapControl.MouseMove +=  new MouseEventHandler(mapControl_MouseMove);
            mapControl.MouseDown +=  new MouseEventHandler(mapControl_MouseDown);
            mapControl.MouseUp +=    new MouseEventHandler(mapControl_MouseUp);

            mapControl.OnMarkerEnter += new MarkerEnter(mapControl_OnMarkerEnter);
            mapControl.OnMarkerLeave += new MarkerLeave(mapControl_OnMarkerLeave);
            mapControl.OnMarkerClick += new MarkerClick(mapControl_MarkerClick);
            mapControl.Paint += mapControl_Paint;
            mapControl.OnMapZoomChanged += mapControl_OnMapZoomChanged;

            mapControl.Position = Program.PointConversion.GetEarth2Mars(mapCenterPoint);

            InitMapOriginMarker();//初始化GCS内部自带的Marker图标
            DrawTargetPoint();//初始化单点模式下，目标航点的自定义Bitmap

            mapControl.ShowCenter = true;//地图显示中间十字
            mapControl_SetPlanePosition(mapCenterPoint.Lng, mapCenterPoint.Lat, 100, 0);
            SetHome(this.homeLocation);
       }

        public void ClearPhotoPoint()
        {
            photopointoverlay.Markers.Clear();
        }

        public void DrawPhotoPoint(int photopoint_count, PointLatLng p)
        {
            PointLatLng p1 = Program.PointConversion.GetEarth2Mars(p);
            GMapMarker photomarker = new GMarkerGoogle(p1, GMarkerGoogleType.black_small);
            photomarker.ToolTipText = photopoint_count.ToString();
            photomarker.ToolTipMode = MarkerTooltipMode.Always;
            //marker1.ToolTipText = "经度：" + p.Lng.ToString("0.0000000") + "\r\n纬度：" +p.Lat.ToString());
            lock (markersOverlay)
            {
                photopointoverlay.Markers.Add(photomarker);
            }

        }
        
        private void InitMapOriginMarker() {
            Graphics GraphicsMarker = Graphics.FromImage(BitmapWpOrigin);
            GraphicsMarker.Clear(Color.Transparent);

            Pen p = new Pen(Color.Black, 1);
            GraphicsMarker.DrawRectangle(p, 2, 2, 16, 12);
            SolidBrush b = new SolidBrush(Color.DeepSkyBlue);
            GraphicsMarker.FillRectangle(b, 3, 3, 15, 11);
            GraphicsMarker.DrawLine(p, 10, 14, 10, 20);
            Bitmap Center = new Bitmap(global::SpaceArrow.Properties.Resources.plane);  //
            planemarker_radius = (int)(System.Math.Sqrt(Center.Width * Center.Width + Center.Height * Center.Height) / 2) + 1;
            BitmapPlaneOrgin = new Bitmap(2 * planemarker_radius, 2 * planemarker_radius);
            Graphics bitGraphics = Graphics.FromImage(BitmapPlaneOrgin);
            System.Drawing.Drawing2D.Matrix m = bitGraphics.Transform;
            m.RotateAt(-45, new PointF(planemarker_radius, planemarker_radius));
            bitGraphics.Transform = m;
            bitGraphics.Clear(Color.Transparent);
            Rectangle rect = new Rectangle(planemarker_radius - Center.Width / 2, planemarker_radius - Center.Height / 2, Center.Width, Center.Height);//目标区域
            Rectangle srcrect = new Rectangle(0, 0, Center.Width, Center.Height);
            bitGraphics.DrawImage(Center, rect, srcrect, System.Drawing.GraphicsUnit.Pixel);
            BitmapBookMarkerOrigin = new Bitmap(20, 20);
            Graphics gMarker = Graphics.FromImage(BitmapBookMarkerOrigin);
            gMarker.Clear(Color.Transparent);
            b.Color = Color.Red;
            PointF[] pp = new PointF[5];
            pp[0].X = 0;
            pp[0].Y = 0;
            pp[1].X = BitmapBookMarkerOrigin.Width;
            pp[1].Y = 0;
            pp[2].X = BitmapBookMarkerOrigin.Width;
            pp[2].Y = BitmapBookMarkerOrigin.Height;
            pp[3].X = 0.5f * BitmapBookMarkerOrigin.Width;
            pp[3].Y = 0.7f * BitmapBookMarkerOrigin.Height;
            pp[4].X = 0;
            pp[4].Y = pp[2].Y;
            gMarker.FillPolygon(b, pp);
            BitmapWpMarker = BitmapWpOrigin;
            BitmapHome = BitmapHomeOrigin;
            BitmapPlane = BitmapPlaneOrgin;
            BitmapBookMarker = BitmapBookMarkerOrigin;            
        }
        #region 地图控件事件响应
        void mapControl_OnMarkerEnter(GMapMarker item)
        {
            this.mapControl.CanDragMap = false;
            isMouseInMarker = true;
            currentEnterMarker = item;
        }
        void mapControl_OnMarkerLeave(GMapMarker item)
        {
            isMouseInMarker = false;
            this.mapControl.CanDragMap = true;
        }
        void mapControl_MouseDown(object sender, MouseEventArgs e)
        {
            timemousedown = DateTime.Now;
            mousepress_x = e.X;
            mousepress_y = e.Y;
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                isMouseDown = true;
        }
        void mapControl_MouseUp(object sender, MouseEventArgs e)
        {
            //  MessageBox.Show("up");
            isMouseDown = false;
            // MessageBox.Show("map up");
        }
        //ToolTip ToolTip_MouseLocation = new ToolTip();
        
        //地图控件鼠标移动事件
        void mapControl_MouseMove(object sender, MouseEventArgs e)
        {
            PointLatLng latLng = this.mapControl.FromLocalToLatLng(e.X, e.Y);
            MouseLocation = Program.PointConversion.Mar2Earth(latLng);
            if (isMouseDown && isMouseInMarker)
            {
                isCanDragMarker = true;//此时可以拖拽
            }
            if (!isMouseDown)
            {
                isCanDragMarker = false;
            }

            if (isMeasureDistance)
            {
                if (measurepoint.Count == 0) {

                }
                else if (measurepoint.Count == 1) {
                    measurepoint.Add(new PointLatLng(MouseLocation.Lat, MouseLocation.Lng));

                    List<PointLatLng> list = new List<PointLatLng>();
                    foreach (PointLatLng p1 in measurepoint) {
                        PointLatLng p2 = Program.PointConversion.GetEarth2Mars(p1);
                        list.Add(p2);
                    }

                    GMapRoute measure = new GMapRoute(list, "linewp");
                    measure.Stroke.Color = Color.Red;
                    measure.Stroke.Width = 2;  //设置画

                    this.measureOverlay.Routes.Clear();
                    this.measureOverlay.Routes.Add(measure);
                }
                else if (measurepoint.Count == 2) {
                    measurepoint[1] = new PointLatLng(MouseLocation.Lat, MouseLocation.Lng);

                    List<PointLatLng> list = new List<PointLatLng>();
                    foreach (PointLatLng p1 in measurepoint)
                    {
                        PointLatLng p2 = Program.PointConversion.GetEarth2Mars(p1);
                        list.Add(p2);
                    }

                    GMapRoute measure = new GMapRoute(list, "linewp");
                    measure.Stroke.Color = Color.Red;
                    measure.Stroke.Width = 2;  //设置画
                    this.measureOverlay.Routes.Clear();

                    this.measureOverlay.Routes.Add(measure);
                }
                return;
            }
            if (this.drawingPoints.Count == 0 && this.isWayPointplan) {
                this.drawingPoints.Add(new PointLatLngAlt(latLng));
               // markersOverlay.Markers[markersOverlay.Markers.Count - 1].Position = latLng;
                mapAddClickMarker(new PointLatLngAlt(MouseLocation.Lat, MouseLocation.Lng)); //point_count++;
                mapDrawWP(drawingPoints);
            }
            if (isCanDragMarker)
            {
                if (markersOverlay.Markers.Contains(currentEnterMarker))
                {
                    currentEnterMarker.Position = latLng;

                  //  currentEnterMarker.ToolTipText = "高度30，离家：30";

                    int n = markersOverlay.Markers.IndexOf(currentEnterMarker);
                    MouseDargChangeWPValue(n + 1, latLng);

                    MouseDargChangWPPlanRoutes(n+1,latLng);

                }
                else if (MapBookMark.Markers.Contains(currentEnterMarker))
                {
                    currentEnterMarker.Position = latLng;
                    int n = MapBookMark.Markers.IndexOf(currentEnterMarker);
                    MouseDragBookMarker(n + 1, latLng);
                }
            }
            else
            {

            }
            this.mapControl.Invalidate();

        }
        void mapControl_MarkerClick(GMapMarker item, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {

            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (markersOverlay.Markers.Contains(item))
                {
                    WPMarkerRightClick(item, e);
                }
                else if (MapBookMark.Markers.Contains(item))
                {
                    BookMarkMarkerRightClick(item, e);
                }

            }

        }
        //地图控件鼠标点击事件
        void mapControl_MouseClick(object sender, MouseEventArgs e)
        {
            if (timemousedown.AddMilliseconds(timemillSecond) > DateTime.Now)
            {
                isDrawWayPoint = true;
            }
            else
            {
                isDrawWayPoint = false;
            }
            int x_dis = (mousepress_x > e.X) ? mousepress_x - e.X : e.X - mousepress_x;
            int y_dis = (mousepress_y > e.Y) ? mousepress_y - e.Y : e.Y - mousepress_y;
            if (distancebe < System.Math.Sqrt(x_dis * x_dis + y_dis * x_dis))
            {
                isDrawWayPoint = false;
            }
            if (isDrawWayPoint && e.Button == System.Windows.Forms.MouseButtons.Left)
            {//鼠标左键按下
                PointLatLng p = mapControl.FromLocalToLatLng(e.X, e.Y);
                PointLatLng pp = Program.PointConversion.Mar2Earth(p);
                MouseClickPoint.Lat = pp.Lat;
                MouseClickPoint.Lng = pp.Lng;

                if (isMeasureDistance) {
                    if (measurepoint.Count == 0)
                    {
                        measurepoint.Add(pp);
                    }
                    else
                    {
                        double s = this.Distance(pp.Lat, pp.Lng, measurepoint[0].Lat, measurepoint[0].Lng);
                        MessageBox.Show("距离："+s.ToString("0.00"));
                        measurepoint.Clear();
                        this.measureOverlay.Routes.Clear();
                    }
                    return;
                }
                if (this.SystemModeID == 0)//单点模式准备
                {
                    DirectPointProcess(pp);
                    return;
                }
                drawingPoints.Add(new PointLatLngAlt(pp.Lat, pp.Lng));//得到当前地点
                mapDrawWP(drawingPoints);
                mapAddClickMarker(new PointLatLngAlt(pp.Lat, pp.Lng)); //point_count++;
                mapAdddataGridView(this.waypoint_count, new PointLatLngAlt(pp.Lat, pp.Lng));
                if (LeftClick != null) {
                    LeftClick(pp);
                }
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                MouseRightClick(sender, e);
            }
        }
        void mapControl_OnMapZoomChanged()
        {
            GPoint gp = this.mapControl.FromLatLngToLocal(Program.PointConversion.GetEarth2Mars(this.mapPlanePoint));
            gp.Y += planemarker_radius;
            this.mapPlaneMarker.Position = this.mapControl.FromLocalToLatLng((int)gp.X, (int)gp.Y);

           if (markerTarget == null) return;
           gp = this.mapControl.FromLatLngToLocal(targetLocation);
           gp.Y += BitmapTarget.Height / 2;
           this.markerTarget.Position = this.mapControl.FromLocalToLatLng((int)gp.X, (int)gp.Y);
        }
        public string YuntaiTargetLocation = "无目标";
        void mapControl_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            SolidBrush b = new SolidBrush(colorZoom);
            Font font = new Font(FontFamily.GenericSansSerif, 18, FontStyle.Regular);
            string str = string.Format("ZOOM:{0}", this.mapControl.Zoom);
            SizeF size = g.MeasureString(str, font);
            g.DrawString(str, font, b, new PointF(this.mapControl.Width - size.Width * 1.2f, this.mapControl.Height - size.Height * 2));
            Font font1 = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Regular);
            string str1 = "经度:" + MouseLocation.Lng.ToString("0.0000000") + "，纬度：" + MouseLocation.Lat.ToString("0.0000000");
            SizeF size1 = g.MeasureString(str1, font1);
            b.Color = colorMouseLocation;
            g.DrawString(str1, font1, b, new PointF(10, this.mapControl.Height - size1.Height * 3));
            #region 绘制目标经纬度
            b.Color = colorTargetLocation;
            SizeF size3 = g.MeasureString(YuntaiTargetLocation, font1);
            g.DrawString(YuntaiTargetLocation, font1, b, new PointF((this.mapControl.Width - size3.Width) / 2, this.mapControl.Height - size3.Height * 3));
            #endregion
            Pen p = new Pen(colorScale, 3);
            PointLatLng pp = this.mapControl.FromLocalToLatLng(0, 0);
            PointLatLng pp1 = this.mapControl.FromLocalToLatLng((int)(0.15f * this.mapControl.Width), 0);
            double a = pp1.Lat - pp.Lat;
            double bb = pp1.Lng - pp.Lng;
            double s = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) +
            Math.Cos(pp1.Lat) * Math.Cos(this.homeLocation.Lat) * Math.Pow(Math.Sin(bb / 2), 2)));
            s = s * 6371.004;//0.000037749460859188845
            s = Math.Round(s * 10000.0) / 500.0;

            s = Distance(pp.Lat,pp.Lng,pp1.Lat,pp1.Lng);


        g.DrawLine(p, new PointF(0.8f*this.mapControl.Width,0.03f*this.mapControl.Height), new PointF(0.95f*this.mapControl.Width,0.03f*this.mapControl.Height));
        g.DrawLine(p,new PointF(0.8f*this.mapControl.Width,0.02f*this.mapControl.Height+2),new PointF(0.8f*this.mapControl.Width,0.03f*this.mapControl.Height));
        g.DrawLine(p, new PointF(0.95f * this.mapControl.Width, 0.02f * this.mapControl.Height), new PointF(0.95f * this.mapControl.Width, 0.03f * this.mapControl.Height+2));

        SizeF size2 = g.MeasureString("0m",font);
        b.Color = colorScale;
        g.DrawString("0m", font, b, new PointF(0.8f * this.mapControl.Width - size2.Width / 2, 0.03f * this.mapControl.Height+5));

        size2 = g.MeasureString(((int)s).ToString()+"m",font);
        g.DrawString(((int)s).ToString() + "m", font, b, new PointF(0.95f * this.mapControl.Width-size2.Width/2, 0.03f * this.mapControl.Height+5));

            if ( isFirstSet)
            {
                d = dataarmNow - dateArm;
                 string datestring =(this.d.Hours*60+d.Minutes).ToString("00") +  ":" + d.Seconds.ToString("00");
                 g.DrawString(datestring, new Font("宋体", 20), new SolidBrush(Color.White),new PointF(10,10));                
            }

        }
        #endregion
        public void ClearTime() {
            dateArm = dataarmNow;
            this.mapControl.Invalidate();
        }
        public void SetHome(PointLatLng pointhome) {
            homeLocation.Lat = pointhome.Lat;
            homeLocation.Lng = pointhome.Lng;
            PointLatLng p1 = Program.PointConversion.GetEarth2Mars(new PointLatLng(pointhome.Lat, pointhome.Lng));
            measureOverlay.Markers.Clear();

            Bitmap bit = new Bitmap(BitmapHome.Width, BitmapHome.Height);  //引用窗体logo
            Graphics g = Graphics.FromImage(bit);
            g.DrawImage(BitmapHome,new Point(0,0));
            GMapMarker markerHome = new GMarkerGoogle(p1, bit);
            markerHome.ToolTipText = "经度：" + pointhome.Lng.ToString("0.0000000") + "\r\n纬度：" + pointhome.Lat.ToString("0.0000000");
            measureOverlay.Markers.Add(markerHome);
            UpdateWayPointHomeDistance();
        }
        /// <summary>
        /// 用于在更新家地址之后， 更新航点家距 距离 （map Marker , dataview）
        /// </summary>
        void UpdateWayPointHomeDistance() {
            if (markersOverlay.Markers.Count != 0) { 
                markersOverlay.Markers.Clear();
                waypoint_count = 0;
                if (this.dataGridView != null)
                {
                    this.dataGridView.Rows.Clear();
                }
                foreach (PointLatLngAlt p in this.drawingPoints) { //地球坐标
                    mapAddClickMarker(p);
                    mapAdddataGridView(this.waypoint_count, new PointLatLngAlt(p.Lat, p.Lng));
                }            
            }

        }
     
        /// <summary>
        /// 在地图上展示飞行器的位置，包括飞行器所对准的航向
        /// </summary>
        /// <param name="lng">当前飞行器的经度值</param>
        /// <param name="lat">当前飞行器的纬度值</param>
        /// <param name="alt">这个参数基本没用</param>
        /// <param name="yaw"></param>
        public void mapControl_SetPlanePosition(double lng,double lat,double alt,float yaw){
            mapPlanePoint.Lat = lat;
            mapPlanePoint.Lng = lng;
            this.planeInmap_yaw = yaw;
            PointLatLng MarPostionCenter = Program.PointConversion.GetEarth2Mars(new PointLatLng(lat, lng));
           //是否选中了地图跟随
            if (isRemoveToPosition) { 
                mapControl.Position = MarPostionCenter;
            }
            SolidBrush b = new SolidBrush(Color.Black);
            MapCenterTail.Markers.Clear();
            planemarker_radius = (int)(System.Math.Sqrt(BitmapPlane.Width * BitmapPlane.Width + BitmapPlane.Height * BitmapPlane.Height) / 2) + 1;
            Bitmap bitmap = new Bitmap(2 * planemarker_radius, 2 * planemarker_radius);
            Graphics bitGraphics = Graphics.FromImage(bitmap);
            System.Drawing.Drawing2D.Matrix m = bitGraphics.Transform;
            m.RotateAt(yaw, new PointF(planemarker_radius, planemarker_radius));
            bitGraphics.Transform = m;
            bitGraphics.Clear(Color.Transparent);
            Rectangle rect = new Rectangle(planemarker_radius - BitmapPlane.Width / 2, planemarker_radius - BitmapPlane.Height / 2,
                BitmapPlane.Width, BitmapPlane.Height);//目标区域
            Rectangle srcrect = new Rectangle(0, 0, BitmapPlane.Width, BitmapPlane.Height);
            bitGraphics.DrawImage(BitmapPlane, rect, srcrect, System.Drawing.GraphicsUnit.Pixel);
            
            GPoint gp=this.mapControl.FromLatLngToLocal(MarPostionCenter);
            gp.Y += planemarker_radius;
            mapPlaneMarker = new GMarkerGoogle(this.mapControl.FromLocalToLatLng((int)gp.X, (int)gp.Y), bitmap);
            MapCenterTail.Markers.Add(mapPlaneMarker);
        }
        public bool isShowTrack = false;
        public void DrawCenterTail(List<PointLatLngAlt> points)
        {     
             MapCenterTail.Routes.Clear();
             if (!isShowTrack) {
                 return;
             }

             List<PointLatLng> point = new List<PointLatLng>();

             if (Program.isMarsCoordinatesInChina)
             { //在中国境内是火星坐标来的
                 foreach (PointLatLngAlt p in points)
                 {
                     if (p == null) continue;
                     PointLatLng pp = new PointLatLng(p.Lat,p.Lng);
                     PointLatLng p1 = Program.PointConversion.GetEarth2Mars(pp);
                     point.Add(p1);
                 }
             }
             else {
                 foreach (PointLatLngAlt p in points)
                 {
                     PointLatLng p1 = new PointLatLng(p.Lat, p.Lng);
                     point.Add(p1);
                 }
             }
             GMapRoute route_flying_path = new GMapRoute(point, "efwefw");
            route_flying_path.Stroke.Color = colorTrack;
            route_flying_path.Stroke.Width = 2;  //设置画
            lock (MapCenterTail) {
                if (this.mapControl.Overlays.Contains(MapCenterTail))
                {
                    MapCenterTail.Routes.Add(route_flying_path);
                }
            }
        }
        public delegate void MouseDargChangWPPlanRoutesDele(int index,PointLatLng point);
        public MouseDargChangWPPlanRoutesDele MouseDargChangWPPlanRoutes = null;
        void MouseDargChangeWPValue(int index,PointLatLng point) {
            PointLatLng pp = Program.PointConversion.Mar2Earth(point);

            PointLatLngAlt p =this.drawingPoints[index-1];
            p.Lat = pp.Lat;
            p.Lng = pp.Lng;

            this.drawingPoints.RemoveAt(index - 1);
            this.drawingPoints.Insert(index - 1, p);

            DataGridViewRow row = new DataGridViewRow();
            DataGridViewTextBoxCell textboxcell1 = new DataGridViewTextBoxCell();
            DataGridViewTextBoxCell textboxcell2 = new DataGridViewTextBoxCell();
            DataGridViewTextBoxCell textboxcell3 = new DataGridViewTextBoxCell();
            DataGridViewTextBoxCell textboxcell4 = new DataGridViewTextBoxCell();
            DataGridViewTextBoxCell textboxcell5 = new DataGridViewTextBoxCell();

            textboxcell1.Value = index.ToString();
            textboxcell2.Value = pp.Lng.ToString("0.0000000");
            textboxcell3.Value = pp.Lat.ToString("0.0000000");
            textboxcell4.Value = p.Alt.ToString();
            double s = Distance(pp.Lat, pp.Lng, homeLocation.Lat, homeLocation.Lng);
            textboxcell5.Value = s.ToString("0.00");

            currentEnterMarker.ToolTipText = "高度:" +( (int)p.Alt).ToString()+"\r\n离家:"+((int)s).ToString();

            DataGridViewButtonCell buttoncell = new DataGridViewButtonCell();
            buttoncell.Value = "删除行";
            row.Cells.Add(textboxcell1);
            row.Cells.Add(textboxcell2);
            row.Cells.Add(textboxcell3);
            row.Cells.Add(textboxcell4);
            row.Cells.Add(textboxcell5);
            row.Cells.Add(buttoncell);
            this.dataGridView.Rows.RemoveAt(index - 1);
            this.dataGridView.Rows.Insert(index - 1, row);
            mapDrawWP(this.drawingPoints);
            
        }

        private  Bitmap DrawWPBitmapMarker(int count)
        {
            Bitmap bitmarker = new Bitmap(BitmapWpMarker.Width, BitmapWpMarker.Height);
            Graphics gramarker = Graphics.FromImage(bitmarker);
            gramarker.DrawImage(BitmapWpMarker, new Point(0, 0));
            SolidBrush b = new SolidBrush(Color.Black);
            string str = count.ToString();
            Font font = new System.Drawing.Font("宋体", 8);
            SizeF sizeF = gramarker.MeasureString(str, font);
            gramarker.DrawString(str, font, b, new PointF(bitmarker.Width / 2 - sizeF.Width / 2, bitmarker.Height/2 - sizeF.Height / 2));
            return bitmarker;
        }
       public void mapAdddataGridView(int count,PointLatLngAlt p) {

           if (this.dataGridView != null)
           {
               DataGridViewRow row = new DataGridViewRow();
               DataGridViewTextBoxCell textboxcell1 = new DataGridViewTextBoxCell();
               DataGridViewTextBoxCell textboxcell2 = new DataGridViewTextBoxCell();
               DataGridViewTextBoxCell textboxcell3 = new DataGridViewTextBoxCell();
               DataGridViewTextBoxCell textboxcell4 = new DataGridViewTextBoxCell();
               DataGridViewTextBoxCell textboxcell5 = new DataGridViewTextBoxCell();

                textboxcell1.Value = count.ToString();
                textboxcell4.Value = p.Alt.ToString();




                double s = Distance(p.Lat, p.Lng, homeLocation.Lat, homeLocation.Lng);

               // PointLatLng point = Program.PointConversion.Mar2Earth(new PointLatLng(p.Lat, p.Lng));
                textboxcell2.Value = p.Lng.ToString("0.0000000");
                textboxcell3.Value = p.Lat.ToString("0.0000000");

                textboxcell5.Value = s.ToString("0.00");

               DataGridViewButtonCell buttoncell = new DataGridViewButtonCell();
               buttoncell.Value = "删除行";
               row.Cells.Add(textboxcell1);
               row.Cells.Add(textboxcell2);
               row.Cells.Add(textboxcell3);
               row.Cells.Add(textboxcell4);
               row.Cells.Add(textboxcell5);

               row.Cells.Add(buttoncell);


               textboxcell1.ReadOnly = true;
               textboxcell5.ReadOnly = true;

               this.dataGridView.Rows.Add(row);
           }
       }
       /// <summary>
       /// 向地图上添加航点Marker
       /// </summary>
       /// <param name="p">航点所在的位置，地球坐标</param>
        public void mapAddClickMarker(PointLatLngAlt p)
        {
            waypoint_count++;
            PointLatLng p1;
            p1 = Program.PointConversion.GetEarth2Mars(new PointLatLng(p.Lat, p.Lng));
            GMapMarker marker1 ;

            marker1 = new GMarkerGoogle(p1, DrawWPBitmapMarker(waypoint_count));


            int s= (int)Distance(p.Lat, p.Lng, homeLocation.Lat, homeLocation.Lng);

            marker1.ToolTipText = "高度:" + ((int)p.Alt).ToString() + "m\r\n离家：" + s.ToString() + "m";

            marker1.ToolTipMode = MarkerTooltipMode.OnMouseOver;


            //marker1.ToolTipText = "经度：" + p.Lng.ToString("0.0000000") + "\r\n纬度：" +p.Lat.ToString());
            lock (markersOverlay) { 
                markersOverlay.Markers.Add(marker1);
            }
        }
        List<PointLatLng> draw = new List<PointLatLng>();
        /// <summary>
        /// 在地图添加航点线路上
        /// </summary>
        /// <param name="drawpoint">线路上的航点位置</param>
       public void mapDrawWP(List<PointLatLngAlt> drawpoint)
       {
           lock (draw) { 
               draw.Clear();
               foreach (PointLatLngAlt p in drawpoint)
               {
                   PointLatLngAlt p1 = new PointLatLngAlt(p);//获得的P坐标必须是地球坐标系中的值

                   PointLatLng pp;      //将地球坐标转换为地图使用的坐标
                   if (Program.isMarsCoordinatesInChina)
                   {
                       pp = Program.PointConversion.GetEarth2Mars(new PointLatLng(p1.Lat, p1.Lng));
                   }
                   else {
                       pp = new PointLatLng(p1.Lat, p1.Lng);
                   }
                   draw.Add(new PointLatLng(pp.Lat,pp.Lng));           
                }
           }
               markersOverlay.Routes.Clear();
               GMapRoute route_way_points = new GMapRoute(draw, "linewp");
               route_way_points.Stroke.Color = colorRoutes;
               route_way_points.Stroke.Width = 2;  //设置画

               markersOverlay.Routes.Add(route_way_points);
        }
        public void mapClearWP() {
            lock (markersOverlay) {
                markersOverlay.Markers.Clear();
            }
            markersOverlay.Routes.Clear();
            drawingPoints.Clear();
            waypoint_count = 0;

            if (this.dataGridView != null) {
                this.dataGridView.Rows.Clear();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
       public  void DrawPolygons(List<PointLatLngAlt> points) {
            List<PointLatLng> list = new List<PointLatLng>();

            foreach (PointLatLngAlt p in points) {
                PointLatLng pp = Program.PointConversion.GetEarth2Mars(new PointLatLng(p.Lat,p.Lng));
                list.Add(pp);
            }


            this.markersOverlay.Polygons.Clear();

            GMapPolygon polygon = new GMapPolygon(list, "多边形");
            polygon.Fill = new SolidBrush(Color.FromArgb(50, Color.Red));
            polygon.Stroke = new Pen(Color.Blue, 2);
            polygon.IsHitTestVisible = true;
            this.markersOverlay.Polygons.Add(polygon);

        }
        private delegate List<PointLatLngAlt>  getwpDataDele();
        
        public List<PointLatLngAlt> getWPsFromDataView()
        {
            List<PointLatLngAlt> drawPoint = new List<PointLatLngAlt>();

            if (this.dataGridView.InvokeRequired)
            {
                this.dataGridView.Invoke(new getwpDataDele(getWPsFromDataView));
            }
            else { 
                if (this.dataGridView == null)
                {
                    return null;
                }
                else {
                    this.dataGridView.CurrentCell = this.dataGridView[0, 0];
                    int count = this.dataGridView.RowCount;
                    for (int i = 0; i < count; ) {
                        PointLatLngAlt p = new PointLatLngAlt();

                        p.Lng = double.Parse(this.dataGridView.CurrentRow.Cells[1].Value.ToString());
                        p.Lat = double.Parse(this.dataGridView.CurrentRow.Cells[2].Value.ToString());
                        p.Alt = float.Parse(this.dataGridView.CurrentRow.Cells[3].Value.ToString());
                        drawPoint.Add(p);

                        i++;
                        if(i>=count){
                            break;
                        }


                        this.dataGridView.CurrentCell = this.dataGridView[0, i];

                    }
                }       
            }
            return drawPoint;   
        }

        public void mapDrawGetWPs(List<PointLatLngAlt> drawpoint)
        {
            PointLatLngAlt Point_Max = new PointLatLngAlt(-90, -180);
            PointLatLngAlt Point_Min = new PointLatLngAlt(90, 180);
            mapClearWP();
            int i = 0;
            foreach (PointLatLngAlt p in drawpoint)
            {
                drawingPoints.Add(p);//得到当前地点
                mapAddClickMarker(p); //point_count++;
                mapAdddataGridView(++i,p);
                Point_Max.Lat = (Point_Max.Lat > p.Lat) ? Point_Max.Lat : p.Lat;
                Point_Max.Lng = (Point_Max.Lng > p.Lng) ? Point_Max.Lng : p.Lng;
                Point_Min.Lat = (Point_Min.Lat < p.Lat) ? Point_Min.Lat : p.Lat;
                Point_Min.Lng = (Point_Min.Lng < p.Lng) ? Point_Min.Lng : p.Lng;
            }
            mapDrawWP(drawingPoints);
            PointLatLng point;
            if (Program.isMarsCoordinatesInChina)
            {
                point = Program.PointConversion.GetEarth2Mars(new PointLatLng((Point_Max.Lat + Point_Min.Lat) / 2, (Point_Max.Lng + Point_Min.Lng) / 2));
            }
            else {
                point = new PointLatLng((Point_Max.Lat + Point_Min.Lat) / 2, (Point_Max.Lng + Point_Min.Lng) / 2);
            }
            mapControl.Position = point;  
        }
        public void WPReWrite() {
            int i = 1;
            this.dataGridView.Rows.Clear();
            foreach (PointLatLngAlt p in drawingPoints) {
                mapAdddataGridView(i++,p);
            }
        }

        public void DeleteWP(PointLatLngAlt point)
        {
            int n=0;
            if (drawingPoints.Contains(point))
            {
                n=  drawingPoints.IndexOf(point);
                drawingPoints.Remove(point);
            }
            else {
                return;
            }
            this.dataGridView.Rows.Clear();
            markersOverlay.Routes.Clear();
            markersOverlay.Markers.Clear();
            waypoint_count = 0;
            if (this.dataGridView != null)
            {
                WPReWrite();
            }

            foreach (PointLatLngAlt p in drawingPoints)
            {
                mapAddClickMarker(p);
            }     
            mapDrawWP(drawingPoints);

        }
        public void InsertWP(PointLatLngAlt point, int num)
        {
            PointLatLngAlt pp = new PointLatLngAlt(point);
            this.drawingPoints.Insert(num-1, pp);
             markersOverlay.Markers.Clear();
            markersOverlay.Routes.Clear();
            if (this.dataGridView != null)
            {
                WPReWrite();
            }

            waypoint_count = 0;
            foreach (PointLatLngAlt p in drawingPoints)
            {
                mapAddClickMarker(p);
            }
            mapDrawWP(drawingPoints);

        }

        private Bitmap DrawBookMarkBitmapMarker(int count)
        {
            Bitmap BitmapMarker = new Bitmap(BitmapBookMarker.Width, BitmapBookMarker.Height);
            Graphics GraphicsMarker = Graphics.FromImage(BitmapMarker);

            GraphicsMarker.DrawImage(BitmapBookMarker, new Point(0, 0));
            SolidBrush b = new SolidBrush(Color.White);
            string str = count.ToString();
            Font font = new System.Drawing.Font("宋体", 8);
            SizeF sizeF = GraphicsMarker.MeasureString(str, font);
            GraphicsMarker.DrawString(str, font, b, new PointF(BitmapBookMarker.Width / 2 - sizeF.Width / 2, BitmapBookMarker.Height/2 - sizeF.Height / 2));
            return BitmapMarker;
        }

        public void DrawBookMark(PointLatLngAlt drawpoint, int MarkCount)
        {
            PointLatLng p =Program.PointConversion.GetEarth2Mars( new PointLatLng(drawpoint.Lat, drawpoint.Lng));
            GMapMarker marker = new GMarkerGoogle(p, DrawBookMarkBitmapMarker(MarkCount));

            marker.ToolTipText = "经度：" + drawpoint.Lng.ToString("0.0000000") + "\r\n纬度：" + drawpoint.Lat.ToString("0.0000000");
            lock (MapBookMark) { 
            MapBookMark.Markers.Add(marker);
            
            }
        }
        public void RemoveMark(GMapMarker marker)
        {
            if (MapBookMark.Markers.Contains(marker))
            {
                lock (MapBookMark)
                {

                    MapBookMark.Markers.Remove(marker);
                }
            }
        }
        public void ClearBookMark(){
            lock (MapBookMark)
            {
                MapBookMark.Routes.Clear();
                MapBookMark.Polygons.Clear();
                MapBookMark.Markers.Clear();
            }
        }



        /// <summary>
        /// 在目标航点Marker上绘制自定义航点图标
        /// </summary>
        /// <returns></returns>
        public Bitmap DrawTargetPoint() {
            Graphics GraphicsTarget = Graphics.FromImage(BitmapTarget);
            GraphicsTarget.Clear(Color.Transparent);
            SolidBrush b = new SolidBrush(Color.Red);
            Pen p = new Pen(Color.Red,2);

            GraphicsTarget.DrawEllipse(p, 3, 3, 14, 14);
            GraphicsTarget.DrawEllipse(p, 5, 5, 10, 10);
            GraphicsTarget.DrawEllipse(p, 8, 8, 4, 4);
            GraphicsTarget.DrawLine(p, 10, 17, 10, 20);
            return BitmapTarget;
        }


        public void DrawTargetPoint(PointLatLngAlt drawpoint)
        {
            PointLatLng p =Program.PointConversion.GetEarth2Mars( new PointLatLng(drawpoint.Lat, drawpoint.Lng));
            PointLatLngAlt pp = new PointLatLngAlt(drawpoint);
            if(markerTarget==null)
                markerTarget = new GMarkerGoogle(p, BitmapTarget);

            targetLocation = p;
            GPoint gp = this.mapControl.FromLatLngToLocal(p);
            gp.Y += BitmapTarget.Height/2;
            this.markerTarget.Position = this.mapControl.FromLocalToLatLng((int)gp.X, (int)gp.Y);
            markerTarget.ToolTipText = "经度：" + drawpoint.Lng.ToString() + "\r\n纬度：" + drawpoint.Lat.ToString();
            lock (MapTargetPoint) { 
                  MapTargetPoint.Markers.Clear();
                  MapTargetPoint.Markers.Add(markerTarget);           
            }

        }
        public void MapWPHeightUpdate(int index, int height)
        {
            DataGridViewTextBoxCell textboxcell = (DataGridViewTextBoxCell)this.dataGridView.Rows[index].Cells[3];
            textboxcell.Value = height.ToString();
        }
        public void ShowMapUpdate() {
            mapControl_SetPlanePosition(this.mapCenterPoint.Lng, this.mapCenterPoint.Lat, 100, planeInmap_yaw);//飞行器
            markersOverlay.Markers.Clear();
            markersOverlay.Routes.Clear();
            markersOverlay.Polygons.Clear();
            if (this.SystemModeID == 0)//单点模式准备
            {
                MapTargetPoint.Markers.Clear();
                this.DrawTargetPoint(this.MouseClickPoint);
                mapControl.Position = Program.PointConversion.GetEarth2Mars(new PointLatLng(this.MouseClickPoint.Lat,this.MouseClickPoint.Lng));
            }
            else if (this.SystemModeID == 1) {
                if (drawingPoints.Count >= 1) { 
                    lock (drawingPoints) {
                        waypoint_count = 0;
                        PointLatLng Point_Max = new PointLatLng(-90,-180);
                        PointLatLng Point_Min = new PointLatLng(90,180);
                        foreach (PointLatLngAlt p in drawingPoints)//航点
                        {
                            mapAddClickMarker(p);

                            Point_Max.Lat = Point_Max.Lat > p.Lat ? Point_Max.Lat : p.Lat;
                            Point_Max.Lng = Point_Max.Lng > p.Lng ? Point_Max.Lng : p.Lng;
                            Point_Min.Lat = Point_Min.Lat < p.Lat ? Point_Min.Lat : p.Lat;
                            Point_Min.Lng = Point_Min.Lng < p.Lng ? Point_Min.Lng : p.Lng;

                        }
                        mapDrawWP(drawingPoints);
                        Point_Max.Lat = (Point_Max.Lat + Point_Min.Lat) / 2;
                        Point_Max.Lng = (Point_Max.Lng + Point_Min.Lng) / 2;
                        this.mapControl.Position = Program.PointConversion.GetEarth2Mars(Point_Max);
                    }                
                }
            }
            if (this.measurepoint.Count > 1) {
                this.measureOverlay.Routes.Clear();

                List<PointLatLng> list = new List<PointLatLng>();
                foreach (PointLatLng p1 in measurepoint)
                {
                    PointLatLng p2 = Program.PointConversion.GetEarth2Mars(p1);
                    list.Add(p2);
                }
                GMapRoute measure = new GMapRoute(list, "linewp");
                measure.Stroke.Color = Color.Red;
                measure.Stroke.Width = 2;  //设置画

                this.measureOverlay.Routes.Add(measure);
            }
            ClearBookMark();
            SetHome(this.homeLocation);
        }

        public void SetDataView(System.Windows.Forms.DataGridView dataGridView1)
        {
            this.dataGridView = dataGridView1;
            this.dataGridView.CurrentCellChanged += DataGridViewDataChange;
            this.dataGridView.CellBeginEdit += DataGridViewCellBeginEdit;
            this.dataGridView.CellEndEdit += DataGridViewCellEndEdit;
            this.dataGridView.CellClick += DataGridViewCellClick;
        }
        void DataGridViewCellClick(object sender, DataGridViewCellEventArgs e)
        {


            int colIndex = e.ColumnIndex;
            int rowIndex = e.RowIndex;

            if (e.GetType().ToString() == "System.Windows.Forms.DataGridViewCellEventArgs")
            {
                if (colIndex == -1 || rowIndex == -1) return;

                DataGridView d = (DataGridView)sender;
                DataGridViewRow row = d.Rows[rowIndex];
                DataGridViewCell c=row.Cells[colIndex];
                if(c.GetType().ToString().EndsWith("ButtonCell")){
                
                }else{
                    return;                
                }
                PointLatLngAlt p = new PointLatLngAlt();
                try
                {
                    p.Lng = double.Parse(this.dataGridView.CurrentRow.Cells[1].Value.ToString());
                    p.Lat = double.Parse(this.dataGridView.CurrentRow.Cells[2].Value.ToString());
                    p.Alt = float.Parse(this.dataGridView.CurrentRow.Cells[3].Value.ToString());
                }
                catch
                {
                    return;
                }
                DeleteWP(p);
            }
        }
        void DataGridViewDataChange(object sender, EventArgs e)
        {

        }
        void DataGridViewCellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            //  MessageBox.Show("ewefwefw");
        }
        void DataGridViewCellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            int num = e.ColumnIndex;
            int nu = e.RowIndex;
            
            PointLatLngAlt p = new PointLatLngAlt();
            float s = 0;
            try
            {
                num = int.Parse(this.dataGridView.CurrentRow.Cells[0].Value.ToString());
                p.Lng = double.Parse(this.dataGridView.CurrentRow.Cells[1].Value.ToString());
                p.Lat = double.Parse(this.dataGridView.CurrentRow.Cells[2].Value.ToString());
                p.Alt = float.Parse(this.dataGridView.CurrentRow.Cells[3].Value.ToString());
                s = float.Parse(this.dataGridView.CurrentRow.Cells[4].Value.ToString());
            }
            catch
            {
                return;
            }
            this.drawingPoints.RemoveAt(num - 1);
            this.drawingPoints.Insert(num - 1, p);
            markersOverlay.Markers.Clear();
       
            markersOverlay.Routes.Clear();
            waypoint_count = 0;
            foreach (PointLatLngAlt p1 in drawingPoints)
            {
                mapAddClickMarker(p1);
            }
            mapDrawWP(drawingPoints);
        }
        public Bitmap DrawHome()
        {

            Bitmap BitmapHome = new Bitmap(30, 30);
            Graphics GraphicsHome = Graphics.FromImage(BitmapHome);

            GraphicsHome.Clear(Color.Transparent);

            SolidBrush b = new SolidBrush(Color.Red);
            Pen p = new Pen(Color.DodgerBlue, 2);


            GraphicsHome.DrawLine(p, 0,0.2f*BitmapHome.Width, BitmapHome.Width / 2, 0);
            GraphicsHome.DrawLine(p, BitmapHome.Width, 0.2f*BitmapHome.Width, BitmapHome.Width / 2, 0);
            b.Color = Color.DodgerBlue;
            GraphicsHome.FillRectangle(b, new RectangleF(0, 0.2f * BitmapHome.Width, BitmapHome.Width, 0.8f * BitmapHome.Width));

            b.Color = Color.Black;
            string str = "HOME";
            Font f=new Font("宋体",10);
            SizeF size=GraphicsHome.MeasureString(str,f);

            GraphicsHome.DrawString(str, f, b, new PointF(0,BitmapHome.Height-size.Height*1.2f));


            /*
            GraphicsHome.DrawLine(p, 0,BitmapHome.Height-BitmapHome.Width, BitmapHome.Width/2, 0);
            GraphicsHome.DrawLine(p, BitmapHome.Width, BitmapHome.Height - BitmapHome.Width, BitmapHome.Width / 2, 0);
        //    GraphicsHome.DrawLine(p, 20, 10, 0, 10);

            b.Color = Color.DodgerBlue;
            GraphicsHome.FillRectangle(b, new Rectangle(0, BitmapHome.Height - BitmapHome.Width, BitmapHome.Width, BitmapHome.Width));
            */

            return BitmapHome;
        }
        public double GetDistance(double lat1, double lng1)
        {
            //double a = lat1 - this.homeLocation.Lat;
            //double b = lng1 - this.homeLocation.Lng;
            //double s = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) +
            //Math.Cos(lat1) * Math.Cos(this.homeLocation.Lat) * Math.Pow(Math.Sin(b / 2), 2)));
            //s = s * 6371.004;//0.000037749460859188845
            //s = Math.Round(s * 10000.0) / 500.0;
            //return s;
            
            return Distance(lat1, lng1, homeLocation.Lat, homeLocation.Lng);

           
        }

        public float multipleflag = 1;
        
      public  double Distance(double lat1, double lon1, double lat2, double lon2)
        {
            double latitude1, longitude1, latitude2, longitude2;
            double dlat, dlon;
            latitude1 = lat1;
            longitude1 = lon1;
            latitude2 = lat2;
            longitude2 = lon2;
            //computing procedure  
            double a, c, distance;
            dlon = System.Math.Abs((longitude2 - longitude1)) * System.Math.PI / 180;
            dlat = System.Math.Abs((latitude2 - latitude1)) * System.Math.PI / 180;
            a = (System.Math.Sin(dlat / 2) * System.Math.Sin(dlat / 2)) + System.Math.Cos(latitude1 * System.Math.PI / 180) * System.Math.Cos(latitude2 * System.Math.PI / 180) * (System.Math.Sin(dlon / 2) * System.Math.Sin(dlon / 2));
            if (a == 1.0)
                c = System.Math.PI;
            else
                c = 2 * System.Math.Atan(System.Math.Sqrt(a) / System.Math.Sqrt(1 - a));
            distance = 6378137.0 * c;

            return multipleflag*distance;
        }

      public double GetDistance1(double Alat, double Alng, double Blat, double Blng)
      {
          double SinAlat = Math.Sin(Alat / 180 * Math.PI);
          double SinBlat = Math.Sin(Blat / 180 * Math.PI);
          double CosAlng = Math.Cos(Alng / 180 * Math.PI);
          double CosBlng = Math.Cos(Blng / 180 * Math.PI);
          double CosDlng = Math.Cos(Math.Abs(Alng - Blng) / 180 * Math.PI);
          double CosThta = SinAlat * SinBlat + CosAlng * CosBlng * CosDlng;
          return Math.Acos(CosThta) * 6731000;
      }
    }
}
