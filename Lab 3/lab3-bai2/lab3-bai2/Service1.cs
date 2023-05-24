using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace lab3_bai2
{
    public partial class Service1 : ServiceBase
    {
        //Tiến trình được xử dụng ở đây là notepad
        static public string ProcessName = "notepad";
        Timer timer = new Timer();
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            WriteToFile("Service is started at " + DateTime.Now);
            WriteToFile(GetProcessStatus(ProcessName));
            //Xử lý việc bật/tắt tiến trình theo chu kì 5 giây
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = 5000; //number in milisecinds
            timer.Enabled = true;
        }

        protected override void OnStop()
        {
            WriteToFile("Service is stopped at " + DateTime.Now);
        }
        //Hàm kiểm tra tiến trình đang được mở hay tắt
        public string GetProcessStatus(string pname)
        {
            Process[] name = Process.GetProcessesByName(pname);
            if (name.Length == 0)
                return "Process is not running";
            else
                return "Process is running";
        }
        //Hàm mở tiến trình
        public void startProcess(string pname)
        {
            Process.Start(pname);
        }
        //Hàm ngưng tiến trình
        public void stopProcess(string pname)
        {
            Process[] name = Process.GetProcessesByName(pname);
            foreach (Process process in name)
            {
                process.Kill();
            }
        }
        //Hàm xử lý vòng lặp khi service onStart 
        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            WriteToFile(GetProcessStatus(ProcessName));
            if (GetProcessStatus(ProcessName) == "Process is not running")
            {
                WriteToFile("Stopped At" + DateTime.Now);
                startProcess(ProcessName);
            }
            else
            {
                WriteToFile("Started At" + DateTime.Now);
                stopProcess(ProcessName);
            }
        }
        public void WriteToFile(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory +
           "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') +
           ".txt";
            if (!File.Exists(filepath))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
        }
    }
}