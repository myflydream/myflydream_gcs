using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace SpaceArrow.ProgressIndication
{
    public partial class FilePathManager : Form
    {
        string cachepath = System.Environment.CurrentDirectory + "\\GMapCache\\";
        string datafilepath = System.Environment.CurrentDirectory + "\\AllFlightdataSave\\";
        string logfilepath = System.Environment.CurrentDirectory + "\\logFile\\";

        public delegate void SetFilePathDele(string filepath);
        public SetFilePathDele SetDataFilePath = null;
        public SetFilePathDele SetLogFilePath = null;
       


        public FilePathManager()
        {
            InitializeComponent();

            this.Text = "GCS-文件管理器";

            this.StartPosition = FormStartPosition.CenterScreen;
            this.ShowInTaskbar = false;

            this.Icon = global::SpaceArrow.Properties.Resources.mpdesktop;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            this.textBox_Cachepath.Text = cachepath;
            this.textBox_datafilepath.Text = datafilepath;
            this.textBox_logfilepath.Text = logfilepath;

            this.comboBox_datafilesize.SelectedIndex = 0;
            this.comboBox_logfilesize.SelectedIndex = 0;

            this.checkBox_autocleardatafile.Visible = false;
            this.checkBox_autologfile.Visible = false;

        }

        public int GetFileSize(string comboxfilesize) {
            int size = 0;

            if (comboxfilesize.EndsWith("KB") || comboxfilesize.EndsWith("Kb")||
                comboxfilesize.EndsWith("kB") || comboxfilesize.EndsWith("kb"))
            {
                comboxfilesize = comboxfilesize.Substring(0, comboxfilesize.Length - 2);
            }
            else if (comboxfilesize.EndsWith("K") || comboxfilesize.EndsWith("k")) {
                comboxfilesize = comboxfilesize.Substring(0, comboxfilesize.Length - 1);
            }

            try
            {
                size = int.Parse(comboxfilesize);
            }
            catch (Exception e){
                MessageBox.Show(e.ToString());
                return 0;
            }



            return size;
        }
        public void SetCachePath(string filepath) {
            cachepath = filepath;
            this.textBox_Cachepath.Text = cachepath;
        }
        public void SetDataPath(string filepath)
        {
            datafilepath = filepath;
            this.textBox_datafilepath.Text = datafilepath;
        }
        public void SetLogPath(string filepath)
        {
            logfilepath = filepath;
            this.textBox_logfilepath.Text = logfilepath;
        }

        private void button_cleardatafile_Click(object sender, EventArgs e)
        {
            string str = this.comboBox_datafilesize.Text;
            str = "将会清理数据文件存储目录下文件大小小于" + str + "的数据文件!";
            if (MessageBox.Show(str, "清理数据文件确认", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                DirectoryInfo MyFolder = new DirectoryInfo(this.textBox_datafilepath.Text);
                FileInfo[] files = MyFolder.GetFiles();


                int size = GetFileSize(this.comboBox_datafilesize.Text);
                int i = 0;
                foreach (FileInfo n in files)
                {
                    if (n.Length < size * 1024) {
                        n.Delete();
                        i++;
                    }
                }
                MessageBox.Show("清理成功,共清理:"+i+"个文件");

            }        

        }
        private void button_changedatafilepath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "请选择文件路径";
            fbd.SelectedPath = this.datafilepath;
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                if (SetDataFilePath != null) {
                    SetDataFilePath(fbd.SelectedPath);
                    this.textBox_datafilepath.Text = fbd.SelectedPath;
                    MessageBox.Show("设置成功，重启软件生效.");
                }
            }
        }
        private void button_clearlogfile_Click(object sender, EventArgs e)
        {
            string str = this.comboBox_logfilesize.Text;
            str = "将会清理数据文件存储目录下文件大小小于" + str + "的日志文件!";
            if (MessageBox.Show(str, "清理日志文件确认", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                DirectoryInfo MyFolder = new DirectoryInfo(this.textBox_logfilepath.Text);
                FileInfo[] files = MyFolder.GetFiles();


                int size = GetFileSize(this.comboBox_logfilesize.Text);
                int i = 0;
                foreach (FileInfo n in files)
                {
                    if (n.Length < size * 1024)
                    {
                        n.Delete();
                        i++;
                    }
                }
                MessageBox.Show("清理成功,共清理:" + i + "个文件");

            }  
        }
        private void button_changelogfilepath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "请选择文件路径";
            fbd.SelectedPath = this.logfilepath;
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                if (SetLogFilePath != null)
                {
                    SetLogFilePath(fbd.SelectedPath);
                    this.textBox_logfilepath.Text = fbd.SelectedPath;
                    MessageBox.Show("设置成功，重启软件生效.");
                }
            }
        }
    
    }
}
