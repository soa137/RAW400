using Devart.Data.Oracle;
using System;
using System.Collections.Generic;
using System.Text;

namespace OraPackages
{
    public class SmzResourcePkg
    {

        static string packageName = "smz_resource_pkg";
        static OraclePackage GetPackage(OracleConnection connection) => new OraclePackage { Connection = connection, PackageName = packageName };

        public static int GetOborResourceId(OracleConnection connection, Int64? globalid, int? oborid)
        {
            object[] args = { globalid, oborid };
            var value = GetPackage(connection).ExecuteProcedure("get_obor_resource_id", typeof(int), args);                        
            return (int)value;
        }

        public static void Extract(OracleConnection connection, int resourceId,  out Int64? globalId, out int? sprOborId)
        {

            var parameters = new OracleParameterCollection();
            parameters.Add("p_resource_id", resourceId);
            parameters.Add("p_id_global",OracleDbType.VarChar).Direction = System.Data.ParameterDirection.Output;
            parameters.Add("p_obor_id", OracleDbType.Integer).Direction = System.Data.ParameterDirection.Output;

            GetPackage(connection).ExecuteProcedure("extract", parameters);

            var o = parameters["p_id_global"].Value;
            if (o == DBNull.Value) globalId = null; else globalId = Convert.ToInt64(o) ;


            o = parameters["p_obor_id"].Value;
            if (o == DBNull.Value) sprOborId = null; else sprOborId = (int)o;

        }


    }
}
