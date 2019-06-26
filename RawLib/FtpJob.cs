using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace RawLib
{
    public class FtpJob:ISourceDirectory
    {
        string d = "";
        public readonly string ftp_str = "";
        List<string> files = new List<string>();
        

        public FtpJob()
        {
            string d = @"C:\test2";
            //string ftp_str = "ftp://s203/test/";
            if (Directory.Exists(d)) Directory.Delete(d, true);
            if (!Directory.Exists(d)) Directory.CreateDirectory(d);
            
        }

        public FtpJob(string pf)
        {
            ftp_str = pf;
            d = @"C:\tmp_ftp";
            if (Directory.Exists(d)) Directory.Delete(d, true);
            if (!Directory.Exists(d)) Directory.CreateDirectory(d);
        }

        public IList<string> GetFiles()
        {
            try
            {
                NetworkCredential credentials = new NetworkCredential("", "");
                GetFtpDirectoryFile(ftp_str, credentials);
            }
            catch (Exception err)
            {
                throw new Exception(String.Format("Ошибка GetFiles FtpJob {0} {1}", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), err.ToString()));
            }
            return files;
        }

        public string GetFileText(string filename)
        {
            string sjson = "";
            try
            {
                if (!File.Exists(Path.Combine(d, filename)))
                {
                
                    NetworkCredential credentials = new NetworkCredential("", "");

                    DownloadFtpDirectory(ftp_str, credentials, d, filename);
                }
            
                using (StreamReader sr = new StreamReader(File.Open( Path.Combine(d, filename), FileMode.Open)))
                {
              
                    sjson = sr.ReadLine();
                }
            }
            catch (Exception err)
            {
                throw new Exception(String.Format("Ошибка GetFileText FtpJob {0} {1}", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), err.ToString()));
            }
            return sjson;
        }

        public Stream GetReport(string reportPath)
        {
            Stream stream = new MemoryStream();

            try
            {
                string ReporName = reportPath.Remove(0, reportPath.LastIndexOf(@"\") + 1);

                if (!String.IsNullOrWhiteSpace(ReporName))
                {
                    NetworkCredential credentials = new NetworkCredential("", "");

                    DownloadFtpDirectory(ftp_str, credentials, d, ReporName);
                    using (var fs = new FileStream(Path.Combine(d, ReporName), FileMode.Open))
                    {
                        fs.CopyTo(stream);
                    }
                }
            }
            catch (Exception ex)
            {
                if (Directory.Exists(d)) Directory.Delete(d, true);

                throw new Exception(String.Format("Ошибка FtpJob метод GetReport {0} {1}", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), ex.ToString()));


            }
            
            return stream;
        }

        void GetFtpDirectoryFile(string url, NetworkCredential credentials)
        {
            FtpWebRequest listRequest = (FtpWebRequest)WebRequest.Create(url);
            listRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            listRequest.Credentials = credentials;

            List<string> lines = new List<string>();

            using (FtpWebResponse listResponse = (FtpWebResponse)listRequest.GetResponse())
            using (Stream listStream = listResponse.GetResponseStream())
            using (StreamReader listReader = new StreamReader(listStream))
            {
                while (!listReader.EndOfStream)
                {
                    lines.Add(listReader.ReadLine());
                }
            }

            foreach (string line in lines)
            {
                
                    string[] tokens =
                        line.Split(new[] { ' ' }, 9, StringSplitOptions.RemoveEmptyEntries);
                    string name = tokens[tokens.Length - 1];
                    string permissions = tokens[0];

                    
                    string fileUrl = url + name;

                    if (permissions[0] == 'd')
                    {
                        

                        GetFtpDirectoryFile(fileUrl + "/", credentials);
                    }
                    else
                    {
                        if (name.EndsWith(".json"))
                            files.Add(name);
                        
                    }
                }
            
        }


        void DownloadFtpDirectory(string url, NetworkCredential credentials, string localPath, string fname)
        {
            FtpWebRequest listRequest = (FtpWebRequest)WebRequest.Create(url);
            listRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            listRequest.Credentials = credentials;

            List<string> lines = new List<string>();

            using (FtpWebResponse listResponse = (FtpWebResponse)listRequest.GetResponse())
            using (Stream listStream = listResponse.GetResponseStream())
            using (StreamReader listReader = new StreamReader(listStream))
            {
                while (!listReader.EndOfStream)
                {
                    lines.Add(listReader.ReadLine());
                }
            }

            foreach (string line in lines)
            {
                string[] tokens =
                    line.Split(new[] { ' ' }, 9, StringSplitOptions.RemoveEmptyEntries);
                string name = tokens[tokens.Length - 1];
                string permissions = tokens[0];

                string localFilePath = Path.Combine(localPath, name);
                string fileUrl = url + name;

                if (permissions[0] == 'd')
                {

                    DownloadFtpDirectory(fileUrl + "/", credentials, localPath, fname);
                }
                else
                {
                    if (name == fname)
                    {
                        FtpWebRequest downloadRequest = (FtpWebRequest)WebRequest.Create(fileUrl);
                        downloadRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                        downloadRequest.Credentials = credentials;

                        using (FtpWebResponse downloadResponse =
                                  (FtpWebResponse)downloadRequest.GetResponse())
                        using (Stream sourceStream = downloadResponse.GetResponseStream())
                        using (Stream targetStream = File.Create(localFilePath))
                        {
                            byte[] buffer = new byte[10240];
                            int read;
                            while ((read = sourceStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                targetStream.Write(buffer, 0, read);
                            }
                        }
                    }
                }
            }
        }




    }
}
