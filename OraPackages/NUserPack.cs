using Devart.Data.Oracle;
using System;
using System.Collections.Generic;
using System.Text;

namespace OraPackages
{
    public class NUserPack
    {

        static string packageName = "n_user_pack";
        static OraclePackage GetPackage(OracleConnection connection) => new OraclePackage { Connection = connection, PackageName = packageName };

        internal static string GetFioUntn(OracleConnection connection, int untn)
        {
            object[] args = { untn };
            var value = GetPackage(connection).ExecuteProcedure("get_fio_untn", typeof(string), args);
            return Convert.ToString(value);
        }

        public static int GetUserId(OracleConnection connection, string username)
        {
            object[] args = { username };
            var value = GetPackage(connection).ExecuteProcedure("get_user_id", typeof(int), args);
            return (int)value;
        }


        public static string GetUsernameById(OracleConnection connection, int userid)
        {
            object[] args = { userid };
            var value = GetPackage(connection).ExecuteProcedure("get_username_by_id", typeof(string), args);
            return Convert.ToString(value);
        }


        public static int? GetUserUntn(OracleConnection connection, string username)
        {
            var parameters = new OracleParameterCollection
            {
                { "p_username", username }
            };
            parameters.Add("untn", OracleDbType.Integer).Direction = System.Data.ParameterDirection.ReturnValue;

            GetPackage(connection).ExecuteProcedure("get_user_untn", parameters);

            var value = parameters["untn"].Value;

            if (value == DBNull.Value) return null; else return (int)value;

        }


    }
}
