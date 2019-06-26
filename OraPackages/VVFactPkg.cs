using Devart.Data.Oracle;
using System;
using System.Collections.Generic;
using System.Text;

namespace OraPackages
{
    public class VVFactPkg
    {

        static string packageName = "vvfact_pkg";
        static OraclePackage GetPackage(OracleConnection connection) => new OraclePackage { Connection = connection, PackageName = packageName };

        public static void SetNachDef(OracleConnection connection, int vvfactid, DateTime? date)
        {
            object[] args = { vvfactid, date };
            GetPackage(connection).ExecuteProcedure("set_nach_def", args);
        }

        public static void SetOkonDef(OracleConnection connection, int vvfactid, DateTime? date)
        {
            object[] args = { vvfactid, date };
            GetPackage(connection).ExecuteProcedure("set_okon_def", args);
        }

    }
}
