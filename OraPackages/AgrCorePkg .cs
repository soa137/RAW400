using Devart.Data.Oracle;
using System;
using System.Collections.Generic;
using System.Text;

namespace OraPackages
{
    public class AgrCorePkg
    {

        static string packageName = "agr_core_pkg";
        static OraclePackage GetPackage(OracleConnection connection) => new OraclePackage { Connection = connection, PackageName = packageName };

        public static bool UserHasRole(OracleConnection connection, int userId, int roleId)
        {
            object[] args = { userId, roleId };
            var value = GetPackage(connection).ExecuteProcedure("user_has_role", typeof(int), args);
            return value.Equals(1);
        }

        public static void GrantRoleToUser(OracleConnection connection, int roleId, int userId, bool skipProcs)
        {

            object[] args = { roleId, userId, skipProcs ? 1 : 0 };
            GetPackage(connection).ExecuteProcedure("grant_role_to_user",  args);
            
        }
    }
}
