using System;
using System.Windows.Forms;
using MAVLinkWP;
//引用的定时器
using System.IO;
using Serialize;
using SpaceArrow.ProgressIndication;
namespace SpaceArrow
{
    static class Program
    {
        public static bool isMarsCoordinatesInChina = true;            //to flag the different map use World Geodetic System or Mars Geodetic System within the territory of China
        public static MavLinkHandler mav_msg_handler;                   //the MAVLink message class
        public static CoordinateTransformation PointConversion;         //the coordinate transformation of map
        public static bool boolhidden=true;
        public static string currentpath = System.Windows.Forms.Application.StartupPath+"\\logFile";//to use for record the software path.
        public static string logFileName = System.Windows.Forms.Application.StartupPath+"\\logFile";//程序log文件绝对路径
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            try{
                //处理未捕获的异常
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                //处理UI异常
                Application.ThreadException+=Application_ThreadException;
                //处理非UI异常
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

                mav_msg_handler = new MavLinkHandler();
                PointConversion = new CoordinateTransformation();

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                string procName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
                if ((System.Diagnostics.Process.GetProcessesByName(procName)).GetUpperBound(0) > 0)
                {
                    MessageBox.Show("已经检测到有实例在运行!!!");
                    return;
                }

                SerializationClass serialize = XMLSerializer.DeSerialize<SerializationClass>(System.Windows.Forms.Application.StartupPath+"\\config.txt");
                if (serialize != null)
                {
                    if (serialize.Language == "en")
                    {
                        System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en");
                    }
                }
 
                Application.Run(new Form1());
            }catch(Exception ex){
                string str = "";
                string strDateInfo = "出现应用程序未处理的异常：" + DateTime.Now.ToString() + "\r\n";
                if (ex != null)
                {
                    str = string.Format(strDateInfo + "异常类型：{0}\r\n异常消息：{1}\r\n异常信息：{2}\r\n",
                    ex.GetType().Name, ex.Message, ex.StackTrace);
                }
                else
                {
                    str = string.Format("应用程序线程错误:{0}", ex);
                }
                MessageBox.Show(str);
            }
        }
        public static bool WriteLog(string logcontent)
        {
            try
            {
                System.IO.File.AppendAllText(
                    logFileName, // 日志文件名
                    string.Format("{0}\t{1}\r\n", DateTime.Now.ToString(), logcontent), // 用制表符 \t 分隔字段
                    System.Text.Encoding.Default);
            }
            catch {

                WriteLog(logcontent);

            }


            return true;


        }
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            string str = "";
            Exception error = e.ExceptionObject as Exception;
            string strDateInfo = "出现应用程序未处理的异常：" + DateTime.Now.ToString() + "\r\n";
            if (error != null)
            {
                str = string.Format(strDateInfo + "Application UnhandledException:{0};\n\r堆栈信息:{1}", error.Message, error.StackTrace);
            }
            else
            {
                str = string.Format("Application UnhandledError:{0}", e);
            }

            MessageBox.Show(str, "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }
        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            //throw new NotImplementedException();

            string str = "";
            string strDateInfo = "出现应用程序未处理的异常：" + DateTime.Now.ToString() + "\r\n";
            Exception error = e.Exception as Exception;
            if (error != null)
            {
                str = string.Format(strDateInfo + "异常类型：{0}\r\n异常消息：{1}\r\n异常信息：{2}\r\n",
                error.GetType().Name, error.Message, error.StackTrace);
            }
            else
            {
                str = string.Format("应用程序线程错误:{0}", e);
            }

           MessageBox.Show(str, "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
