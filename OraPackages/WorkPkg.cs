using Devart.Data.Oracle;
using System;
using System.Collections.Generic;
using System.Text;

namespace OraPackages
{
    public class CollectionPkg
    {

        static string packageName = "collection_pkg";
        static OraclePackage GetPackage(OracleConnection connection) => new OraclePackage { Connection = connection, PackageName = packageName };

        public static int CreateCollection(OracleConnection connection, string docType)
        {
            object[] args = { docType};
            var value = GetPackage(connection).ExecuteProcedure("CreateCollection", typeof(int), args);
            return (int)value;
        }
        

    }
}
