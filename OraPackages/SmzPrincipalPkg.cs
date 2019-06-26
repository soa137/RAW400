using Devart.Data.Oracle;
using System;
using System.Collections.Generic;
using System.Text;

namespace OraPackages
{

    public class SmzPrincipalPkg
    {

        static string packageName = "smz_principal_pkg";
        static OraclePackage GetPackage(OracleConnection connection) => new OraclePackage { Connection = connection, PackageName = packageName };

        public static int GetPrincipalIdUntn(OracleConnection connection, int untn)
        {
            object[] args = { untn };
            var value = GetPackage(connection).ExecuteProcedure("get_principal_id_untn", typeof(int), args);
            return (int)value;
        }


    }
}
