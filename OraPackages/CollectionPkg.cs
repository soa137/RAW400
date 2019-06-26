using Devart.Data.Oracle;
using System;
using System.Collections.Generic;
using System.Text;

namespace OraPackages
{
    public class WorkPkg
    {

        static string packageName = "work_pkg";
        static OraclePackage GetPackage(OracleConnection connection) => new OraclePackage { Connection = connection, PackageName = packageName };

        public static int CreateWorkR(OracleConnection connection, int operRashodId, int principalId, double ken, int resourceId, DateTime date, int smenNum)
        {
            object[] args = { operRashodId, null, null, principalId, null, ken, resourceId, date, smenNum};
            var value = GetPackage(connection).ExecuteProcedure("create_work_r", typeof(int), args);
            return (int)value;
        }

        public static void SetOkonDef(OracleConnection connection, int vvfactid, DateTime date)
        {
            object[] args = { vvfactid, date };
            GetPackage(connection).ExecuteProcedure("set_okon_def", args);
        }


        public static int InsertWork(OracleConnection connection, int? operRashodId, string docType, int? docId, int? untn, int? normStateId, double ken, int? sprOborId, Int64? globalId, DateTime date, int smenNum, int? brigadaId, int? operSubId)
        {
            object[] args = { operRashodId, docType, docId, untn, normStateId, ken, sprOborId, globalId, date, smenNum, brigadaId, operSubId };
            var value = GetPackage(connection).ExecuteProcedure("insert_work", typeof(int), args);
            return (int)value;
        }

    }
}
