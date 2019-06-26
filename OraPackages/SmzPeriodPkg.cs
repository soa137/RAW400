using Devart.Data.Oracle;
using System;
using System.Collections.Generic;
using System.Text;

namespace OraPackages
{

    public class SmzPeriodPkg
    {

        static string packageName = "smz_period_pkg";
        static OraclePackage GetPackage(OracleConnection connection) => new OraclePackage { Connection = connection, PackageName = packageName };

        public static int CurrentSmenId(OracleConnection connection)
        {            
            var value = GetPackage(connection).ExecuteProcedure("current_smen_id", typeof(int), null);
            return (int)value;
        }


        public static DateTime GetSmenDate(OracleConnection connection, int smenId)
        {
            object[] args = { smenId };
            var value = GetPackage(connection).ExecuteProcedure("get_smen_date", typeof(DateTime), args);
            return (DateTime)value;
        }


        public static int GetSmenNum(OracleConnection connection, int smenId)
        {
            object[] args = { smenId };
            var value = GetPackage(connection).ExecuteProcedure("get_smen_num", typeof(int), args);
            return (int)value;
        }

    }
}
