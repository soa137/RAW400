using Devart.Data.Oracle;
using System;
using System.Collections.Generic;
using System.Text;

namespace OraPackages
{
    public class NPasportPkg
    {
        static string packageName = "n_pasport_pkg";
        static OraclePackage GetPackage(OracleConnection connection) => new OraclePackage { Connection = connection, PackageName = packageName };


        public static void LinkPasportTek(OracleConnection connection, int pasportId, int tekId)
        {
            var command = new OracleCommand {
                Connection = connection,
                CommandType = System.Data.CommandType.StoredProcedure,
                CommandText = "n_link_pasport_tek"                
            };

            command.Parameters.Add("p_pasport_id", pasportId);
            command.Parameters.Add("p_ntek_id", tekId);
            command.Parameters.Add("p_tek_id", null);
            command.Parameters.Add("p_id", OracleDbType.Integer, System.Data.ParameterDirection.ReturnValue);

            command.ExecuteNonQuery();

            
            
            


            //object[] args = { pasportId, tekId, 0 };
            
            //new OraclePackage{Connection=connection}.ExecuteProcedure("n_link_pasport_tek", args);
        }

        public static void GeneratePasport(OracleConnection connection, int pasportId, int prgroup, int begin, int end)
        {
            object[] args = { pasportId, prgroup, begin, end };
            GetPackage(connection).ExecuteProcedure("generate_pasport", args);
        }
    }
}
