using Devart.Data.Oracle;
using System;
using System.Collections.Generic;
using System.Text;

namespace OraPackages
{
    public class AgrAppPkg
    {

        static string packageName = "agr_app_pkg";
        static OraclePackage GetPackage(OracleConnection connection) => new OraclePackage { Connection = connection, PackageName = packageName };

        public static bool UserHasRole(OracleConnection connection, int userid, int roleid)
        {
            object[] args = { userid, roleid };
            var value = GetPackage(connection).ExecuteProcedure("user_has_role", typeof(int), args);
            return value.Equals(1);
        }

        public static void SetSessionUser(OracleConnection connection, string username)
        {
            object[] args = { username};
            GetPackage(connection).ExecuteProcedure("set_session_user", args);
        }


        public static string GetSessionUser(OracleConnection connection)
        {            
            var value = GetPackage(connection).ExecuteProcedure("get_session_user", typeof(string));
            return Convert.ToString(value);
        }

        public static int GetDocRank(OracleConnection connection, string docType, int docId)
        {
            object[] args = { docType, docId };
            var value = GetPackage(connection).ExecuteProcedure("get_doc_rank", typeof(int), args);
            return (int)value;
        }
    }
}
