using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace RawLib
{
    public class NFileStorage : IFileStorage
    {
        //string d = "";

        public NFileStorage()
        {
           

        }

       
        public string Upload(string path, Stream stream)
        {
            string url = "";
            string ReporName = path.Remove(0, path.LastIndexOf(@"\") + 1);
            try
            {
                if (stream.Length != 0)
            {
                var request = (HttpWebRequest)WebRequest.Create("http://nfile.vm.vsmpo.ru/FileServer/api/file/3/" + ReporName);



                var data = ReadToEnd(stream);

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;

                using (var strm = request.GetRequestStream())
                {
                    strm.Write(data, 0, data.Length);
                }

                var response = (HttpWebResponse)request.GetResponse();

                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                responseString = responseString.Trim('"');
                request = (HttpWebRequest)WebRequest.Create("http://nfile.vm.vsmpo.ru/FileServer/api/file/3/" + responseString);

                response = (HttpWebResponse)request.GetResponse();




               
                byte[] tmpFtpHash;
                
                tmpFtpHash = new MD5CryptoServiceProvider().ComputeHash(stream);

                byte[] tmpNFILEHash;
                using (var m = new MemoryStream())
                {
                     response.GetResponseStream().CopyTo(m);
                     tmpNFILEHash = new MD5CryptoServiceProvider().ComputeHash(m);
                }
                bool bEqual = false;
                if (tmpNFILEHash.Length == tmpFtpHash.Length)
                {
                    int i = 0;
                    while ((i < tmpNFILEHash.Length) && (tmpNFILEHash[i] == tmpFtpHash[i]))
                    {
                        i += 1;
                    }
                    if (i == tmpNFILEHash.Length)
                    {
                        bEqual = true;
                    }
                }


                if (!bEqual)
                {
                    request = (HttpWebRequest)WebRequest.Create("http://nfile.vm.vsmpo.ru/FileServer/api/file/3/" + responseString);

                    request.Method = "PUT";
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.ContentLength = 0;
                    response = (HttpWebResponse)request.GetResponse();
                    string returnString = response.StatusCode.ToString();

                    throw new Exception(String.Format("Ошибка NFileStorage загрузки отчета в NFILE {0} {1}", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), ReporName));
                }
                url = responseString;
            }
            }
            catch (Exception ex)
            {
                

                throw new Exception(String.Format("Ошибка NFileStorage метод Upload {0} {1}", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), ex.ToString()));


            }
            return url;
        }
        public static byte[] ReadToEnd(Stream input)
        {
            using (var memoryStream = new MemoryStream())
            {
                input.Position = 0;
                input.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
    
    }
