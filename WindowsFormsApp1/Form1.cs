using Devart.Data.Oracle;
using RawLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private BackgroundWorker worker;
        private volatile bool _executing;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
                OracleConnection conn = new OracleConnection("User id=raw400; password=raw0099; Host=testfin; direct=True; SID=B; Port=1521;");
            try
            {
                conn.Open();
                CatcherFactory.SetupCatcher("ftp://s203/test/", conn).Go();
                //CatcherFactory.SetupCatcher("ftp://194.226.128.71/Bf1/", conn).Go();
                //CatcherFactory.SetupCatcher("ftp://194.226.128.71/Rrm/", conn).Go();
                //CatcherFactory.SetupCatcher("ftp://194.226.128.71/Exp/", conn).Go();
                //    ISourceDirectory sourceRrm = new FtpJob("ftp://194.226.128.71/Rrm/");
                //IHandledStorage storageRrm = new FileStorage("ftp://194.226.128.71/Rrm/", conn);
                //IFileStorage fsRrm = new NFileStorage();
                //IPasportSaver pasportRrm = new FinSaver(conn);
                //List<Catcher> catList = new List<Catcher> { new Catcher(sourceRrm, storageRrm, pasportRrm, fsRrm) };
                //catList.ForEach(delegate (Catcher cat)
                //{
                //    cat.Go();
                //});
                conn.Close();

            }
            catch (Exception ex)
            {
                conn.Close();
                using (StreamWriter writer = new StreamWriter("C:\\log.txt", true))
                {
                    writer.WriteLine(String.Format("Exception {0} {1}",
                        DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), ex.ToString()));
                    writer.Flush();
                }
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            OracleConnection conn = new OracleConnection("User id=raw400; password=raw0099; Host=testfin; direct=True; SID=B; Port=1521;");
            try
            {
                conn.Open();
                //CatcherFactory.SetupCatcher("ftp://s203/test/", conn).Establish();
                CatcherFactory.SetupCatcher("ftp://194.226.128.71/Bf1/", conn).Establish();
                CatcherFactory.SetupCatcher("ftp://194.226.128.71/Rrm/", conn).Establish();
                CatcherFactory.SetupCatcher("ftp://194.226.128.71/Exp/", conn).Establish();
                //    ISourceDirectory sourceRrm = new FtpJob("ftp://194.226.128.71/Rrm/");
                //IHandledStorage storageRrm = new FileStorage("ftp://194.226.128.71/Rrm/", conn);
                //IFileStorage fsRrm = new NFileStorage();
                //IPasportSaver pasportRrm = new FinSaver(conn);
                //List<Catcher> catList = new List<Catcher> { new Catcher(sourceRrm, storageRrm, pasportRrm, fsRrm) };
                //catList.ForEach(delegate (Catcher cat)
                //{
                //    cat.Go();
                //});
                conn.Close();

            }
            catch (Exception ex)
            {
                conn.Close();
                using (StreamWriter writer = new StreamWriter("C:\\log.txt", true))
                {
                    writer.WriteLine(String.Format("Exception {0} {1}",
                        DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), ex.ToString()));
                    writer.Flush();
                }
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            worker = new BackgroundWorker();
            worker.DoWork += worker_DoWork;
            System.Timers.Timer timer = new System.Timers.Timer
            {
                Interval = 60000 // 60 seconds
            };
            timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimer);
            timer.Start();
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            OracleConnection conn = new OracleConnection("User id=raw400; password=raw0099; Host=testfin; direct=True; SID=B; Port=1521;");
            conn.Open();
            //CatcherFactory.SetupCatcher("ftp://s203/test/", conn).Go();
            CatcherFactory.SetupCatcher("ftp://194.226.128.71/Bf1/", conn).Go();
            CatcherFactory.SetupCatcher("ftp://194.226.128.71/Rrm/", conn).Go();
            CatcherFactory.SetupCatcher("ftp://194.226.128.71/Exp/", conn).Go();
            conn.Close();
            if (Directory.Exists(@"C:\tmp_ftp")) Directory.Delete(@"C:\tmp_ftp", true);
        }

        private void OnTimer(object sender, ElapsedEventArgs e)
        {
            if (!worker.IsBusy)
                worker.RunWorkerAsync();

        }

        private void button4_Click(object sender, EventArgs e)
        {
            System.Timers.Timer timer = new System.Timers.Timer
            {
                Interval = 60000 // 60 seconds
            };
            timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimer2);
            timer.Start();
        }

        private void OnTimer2(object sender, ElapsedEventArgs e)
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
    }
}
