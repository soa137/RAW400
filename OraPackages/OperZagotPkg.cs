using Devart.Data.Oracle;
using System;
using System.Collections.Generic;
using System.Text;

namespace OraPackages
{
    public class OperZagotPkg
    {

        static string packageName = "oper_zagot_pkg";
        static OraclePackage GetPackage(OracleConnection connection) => new OraclePackage { Connection = connection, PackageName = packageName };

        public static int? GetVVFactId(OracleConnection connection, int operid, int zagotid)
        {
            object[] args = { operid, zagotid };
            var value = GetPackage(connection).ExecuteProcedure("get_vvfact_id", typeof(int), args);
            if (value == DBNull.Value) return null;
            var id = (int)value;
            return id;
        }


        public static int? GetOperRashodId(OracleConnection connection, int operid, int zagotid)
        {
            object[] args = { operid, zagotid };
            var value = GetPackage(connection).ExecuteProcedure("get_oper_rashod_id", typeof(int), args);
            if (value == DBNull.Value) return null;            
            return (int)value;
        }

        public static void CreateOperRashod(OracleConnection connection, int operid, int zagotid, int pdrId)
        {
            object[] args = { operid, zagotid, pdrId };
            GetPackage(connection).ExecuteProcedure("create_oper_rashod",  args);
        }

    }
}
