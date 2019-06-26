using Devart.Data.Oracle;
using RawLib;
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

namespace WinServiceObor
{

    public partial class Service1 : ServiceBase
    {
        private int incId = 1;
        private volatile bool _executing;
        public Service1()
        {
            InitializeComponent();
            this.CanStop = true; // службу можно остановить
            this.CanPauseAndContinue = true; // службу можно приостановить и затем продолжить
            this.AutoLog = true; // служба может вести запись в лог
            
        }

        protected override void OnStart(string[] args)
        {
            //using (StreamWriter writer = new StreamWriter("C:\\WinServiceObor.log", true))
            //{
            //    writer.WriteLine(String.Format("{0} OnStart {1}",
            //        DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"), incId++));
            //    writer.Flush();
            //}
            Timer timer = new Timer
            {
                Interval = 60000 // 60 seconds
            };
            timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimer);
            timer.Start();
        }

        private void OnTimer(object sender, ElapsedEventArgs e)
        {
            if (_executing)
                return;

            _executing = true;
            try
            {
                OracleConnection conn = new OracleConnection("User id=raw400; password=raw0099; Host=testfin; direct=True; SID=B; Port=1521;");
                conn.Open();
                //CatcherFactory.SetupCatcher("ftp://s203/test/", conn).Go();
                CatcherFactory.SetupCatcher("ftp://194.226.128.71/Bf1/", conn).Go();
                CatcherFactory.SetupCatcher("ftp://194.226.128.71/Rrm/", conn).Go();
                CatcherFactory.SetupCatcher("ftp://194.226.128.71/Exp/", conn).Go();
                conn.Close();
            }
            finally
            {
                _executing = false;
                if (Directory.Exists(@"C:\tmp_ftp")) Directory.Delete(@"C:\tmp_ftp", true);
            }

        }

        protected override void OnStop()
        {
            //using (StreamWriter writer = new StreamWriter("C:\\WinServiceObor.log", true))
            //{
            //    writer.WriteLine(String.Format("{0} OnStop {1}",
            //        DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"), incId));
            //    writer.Flush();
            //}
        }
    }
}
