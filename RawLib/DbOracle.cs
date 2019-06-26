using Devart.Data.Oracle;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RawLib
{
    class DbOracle
    {
        private OracleConnection conn;
        private string connectionString;
        public DbOracle()
        {
        }
        public bool Open(string userID, string password, string host)
        {
            connectionString = "User id=grpo; password=oprga; Host=testfin; direct=True; SID=B; Port=1521;";

            conn = new OracleConnection(connectionString);
            try
            {
                conn.Open();
                
            }
            catch (OracleException err)
            {
                throw new Exception(String.Format("Ошибка Open DbOracle {0} {1}", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), err.ToString()));
            }
            return conn.State == System.Data.ConnectionState.Open;
        }

        public bool Close()
        {
           
            try
            {
                conn.Close();
            }
            catch (OracleException err)
            {
                throw new Exception(String.Format("Ошибка Close DbOracle {0} {1}", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), err.ToString()));
            }
            return conn.State == System.Data.ConnectionState.Closed;
        }

        

        public void InsertDirFile(string fname, string ftp, string sjson)
        {

            OracleCommand cmd = new OracleCommand("insert into GRPO.DIR_FILE( FILE_NAME, FTP_NAME, TEXT_JSON ) values ( :P_FILE_NAME, :P_FTP_NAME, :P_TEXT_JSON )", conn);
            cmd.Parameters.Add("P_FILE_NAME", fname);
            cmd.Parameters.Add("P_FTP_NAME", ftp);
            cmd.Parameters.Add("P_TEXT_JSON", sjson);
            
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                throw new Exception(String.Format("Ошибка insert GRPO.DIR_FILE DbOracle {0} {1}", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), err.ToString()));
            }
        }

        public List<string> GetFilesJ(string ftp)
        {
            OracleDataReader readerFile;
            List<string> tmp = new List<string>();
            string sql = "select DIR_FILE_ID, FILE_NAME from GRPO.DIR_FILE where upper( FTP_NAME ) = upper( :PFTP )";
            OracleCommand cmd = new OracleCommand(sql, conn);
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
                throw new Exception(String.Format("Ошибка select GRPO.DIR_FILE DbOracle {0} {1}", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), err.ToString()));
            }
             return tmp;
        }

        
    }
}
