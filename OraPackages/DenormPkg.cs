using Devart.Data.Oracle;
using System;
using System.Collections.Generic;
using System.Text;

namespace OraPackages
{
    public class DenormPkg
    {

        static string packageName = "denorm_pkg";
        static OraclePackage GetPackage(OracleConnection connection) => new OraclePackage { Connection = connection, PackageName = packageName };


        public static void DenormDoc(OracleConnection connection, string doctype, int docid)
        {

            object[] args = { doctype, docid };
            GetPackage(connection).ExecuteProcedure("denorm_doc",  args);
            
        }
    }
}
