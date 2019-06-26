using Devart.Data.Oracle;
using System;
using System.Collections.Generic;
using System.Text;

namespace OraPackages
{
    public class NTekAppPkg
    {
        static string packageName="n_tek_app_pkg";


        static OraclePackage GetPackage(OracleConnection connection) => new OraclePackage { Connection = connection, PackageName = packageName};
                           

        public static int CreateTek(OracleConnection connection, string nc, string obozn)
        {
            object[] args = { nc, obozn };
            var value = GetPackage(connection).ExecuteProcedure("create_tek", typeof(int), args);
            return (int)value;
        }

        public static void SetWorkTek(OracleConnection connection, int tekId)
        {
            object[] args = { tekId };
            GetPackage(connection).ExecuteProcedure("set_work_tek", args);            
        }

        public static int AddMarshObor(OracleConnection connection, int marshId, int sprOborId, bool isMain)
        {
            object[] args = { marshId, sprOborId, isMain ? 1 : 0 };
            var value = GetPackage(connection).ExecuteProcedure("add_marsh_obor", typeof(int), args);
            return (int)value;
        }
    }
}
