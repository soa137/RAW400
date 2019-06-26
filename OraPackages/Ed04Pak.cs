using Devart.Data.Oracle;
using System;
using System.Collections.Generic;
using System.Text;

namespace OraPackages
{
    public class Ed04Pak
    {

        static string packageName = "ed_04_pak";
        static OraclePackage GetPackage(OracleConnection connection) => new OraclePackage { Connection = connection, PackageName = packageName };



        internal static void CreateDoc(OracleConnection connection, int zagotId)
        {
            object[] args = { zagotId};
            GetPackage(connection).ExecuteProcedure("create_doc", args);
        }
    }
}
