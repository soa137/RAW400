using Devart.Data.Oracle;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RawLib
{
    public class FileStorage : IHandledStorage
    {
        List<string> files = new List<string>();
        List<string> fileserr = new List<string>();
        string ftp_str = "";
        public readonly OracleConnection connection;

        public FileStorage()
        {
            //string ftp_str = "ftp://s203/test/";
                     
        }

        public FileStorage(string pf, OracleConnection conn)
        {
            ftp_str = pf;
            connection = conn;

        }
        public void Add(string filename, string text)
        {
            if (!String.IsNullOrWhiteSpace(filename) & !String.IsNullOrWhiteSpace(text))
            {
                //DbOracle db = new DbOracle();
                try
                {
                    //if (db.Open("", "", ""))
                    //{
                       
                            InsertDirFile(filename, ftp_str, text);
                            //db.Close();
                            
                            
                       
                    //}
                }
                catch (Exception err)
                {
                    //db.Close();

                    throw new Exception(String.Format("Ошибка InsertDirFile FileStorage {0} {1}", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), err.ToString()));
                }
                
            }
        }

        //public void AddEstab(string filename, string text)
        //{
        //    if (!String.IsNullOrWhiteSpace(filename))
        //    {
        //        //DbOracle db = new DbOracle();
        //        try
        //        {
        //            //if (db.Open("", "", ""))
        //            //{

        //            InsertDirFileEstab(filename, ftp_str, text);
        //            //db.Close();



        //            //}
        //        }
        //        catch (Exception err)
        //        {
        //            //db.Close();

        //            throw new Exception(String.Format("Ошибка AddEstab FileStorage {0} {1}", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), err.ToString()));
        //        }

        //    }
        //}

        public void Error(string filename, string message)
        {
            if (!String.IsNullOrWhiteSpace(filename) & !String.IsNullOrWhiteSpace(message))
            {
               
                try
                {
                  
                    InsertDirFileErr(filename, ftp_str, message);
                               
                }
                catch (Exception err)
                {

                    throw new Exception(String.Format("Ошибка Error FileStorage {0} {1}", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), err.ToString()));
                }

            }
        }

        public IList<string> GetFiles()
        {
            //DbOracle db = new DbOracle();
            try
            {
                //if (db.Open("", "", ""))
                //{
                    files = GetFilesJ(ftp_str);
                //    db.Close();
                //}
            }
            catch (Exception err)
            {
                //db.Close();
                throw new Exception(String.Format("Ошибка GetFilesJ FileStorage {0} {1}", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), err.ToString()));
            }
            return files;
        }

        public void InsertDirFile(string fname, string ftp, string sjson)
        {

            OracleCommand cmd = new OracleCommand("insert into GRPO.DIR_FILE( FILE_NAME, FTP_NAME, TEXT_JSON ) values ( :P_FILE_NAME, :P_FTP_NAME, :P_TEXT_JSON )", connection);
            cmd.Parameters.Add("P_FILE_NAME", fname);
            cmd.Parameters.Add("P_FTP_NAME", ftp);
            cmd.Parameters.Add("P_TEXT_JSON", sjson);

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                throw new Exception(String.Format("Ошибка insert GRPO.DIR_FILE FileStorage {0} {1}", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), err.ToString()));
            }
        }


        public void InsertDirFileErr(string fname, string ftp, string sjson)
        {

            OracleCommand cmd = new OracleCommand("insert into GRPO.DIR_FILE_ERR( FILE_NAME, FTP_NAME, TEXT_JSON ) values ( :P_FILE_NAME, :P_FTP_NAME, :P_TEXT_JSON )", connection);
            cmd.Parameters.Add("P_FILE_NAME", fname);
            cmd.Parameters.Add("P_FTP_NAME", ftp);
            cmd.Parameters.Add("P_TEXT_JSON", sjson);

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                throw new Exception(String.Format("Ошибка insert GRPO.DIR_FILE_ERR FileStorage {0} {1}", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), err.ToString()));
            }
        }

        //public void InsertDirFileEstab(string fname, string ftp, string sjson)
        //{

        //    OracleCommand cmd = new OracleCommand("insert into GRPO.DIR_FILE( FILE_NAME, FTP_NAME, TEXT_JSON, PR_UNUSED ) values ( :P_FILE_NAME, :P_FTP_NAME, :P_TEXT_JSON, 1 )", connection);
        //    cmd.Parameters.Add("P_FILE_NAME", fname);
        //    cmd.Parameters.Add("P_FTP_NAME", ftp);
        //    cmd.Parameters.Add("P_TEXT_JSON", sjson);

        //    try
        //    {
        //        cmd.ExecuteNonQuery();
        //    }
        //    catch (Exception err)
        //    {
        //        throw new Exception(String.Format("Ошибка insert GRPO.DIR_FILE InsertDirFileEstab FileStorage {0} {1}", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), err.ToString()));
        //    }
        //}

        public List<string> GetFilesJ(string ftp)
        {
            OracleDataReader readerFile;
            List<string> tmp = new List<string>();
            string sql = "select DIR_FILE_ID, FILE_NAME from GRPO.DIR_FILE where upper( FTP_NAME ) = upper( :PFTP )";
            OracleCommand cmd = new OracleCommand(sql, connection);
            cmd.Parameters.Add("PFTP", ftp);
            try
            {
                readerFile = cmd.ExecuteReader();
                while (readerFile.Read())
                {
                    tmp.Add(readerFile.GetString(1));
                }
            }
            catch (Exception err)
            {
                throw new Exception(String.Format("Ошибка select GRPO.DIR_FILE FileStorage {0} {1}", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), err.ToString()));
            }
            return tmp;
        }

        public List<string> GetFilesJErr(string ftp)
        {
            OracleDataReader readerFile;
            List<string> tmp = new List<string>();
            string sql = "select DIR_FILE_ERR_ID, FILE_NAME from GRPO.DIR_FILE_ERR where upper( FTP_NAME ) = upper( :PFTP )";
            OracleCommand cmd = new OracleCommand(sql, connection);
            cmd.Parameters.Add("PFTP", ftp);
            try
            {
                readerFile = cmd.ExecuteReader();
                while (readerFile.Read())
                {
                    tmp.Add(readerFile.GetString(1));
                }
            }
            catch (Exception err)
            {
                throw new Exception(String.Format("Ошибка select GRPO.DIR_FILE_ERR FileStorage {0} {1}", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), err.ToString()));
            }
            return tmp;
        }

        public IList<string> GetErrorFiles()
        {
            try
            {   
                fileserr = GetFilesJErr(ftp_str);
            }
            catch (Exception err)
            {
                throw new Exception(String.Format("Ошибка GetErrorFiles FileStorage {0} {1}", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), err.ToString()));
            }
            return fileserr;
        }
    }
}
