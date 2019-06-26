using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Text;
using Devart.Data.Oracle;

namespace OraTables
{
    public class QM
    {
        

        public static string GetPkName(OracleConnection connection, string table)
        {
            var arr = table.Split('.');
            var tname = arr[arr.Length - 1].ToUpper();

            return QueryString(connection, "select s.COLUMN_NAME  from all_constraints c  inner join all_cons_columns s on s.CONSTRAINT_NAME=c.CONSTRAINT_NAME where c.TABLE_NAME=:0 and c.CONSTRAINT_TYPE='P'", new object[] { tname });
        }

        
        private static object QueryValue(OracleConnection connection, string sql, object[] args)
        {
            var command = new OracleCommand(sql, connection);

            if (args != null)
                for (int i = 0; i < args.Length; i++)
                    command.Parameters.Add(i.ToString(), args[i]);

            using (var reader = command.ExecuteReader())
                if (reader.Read()) return reader.GetValue(0);


            return null;

        }

        

        //[Obsolete]
        //internal static T Query<T>(OracleConnection connection, string column, string table, int id)
        //{
        //    var pkname = GetPkName(connection, table);
        //    var sql = $"select {column} from {table} where {pkname}=:0";

        //    var command = new OracleCommand(sql, connection);
            
        //    command.Parameters.Add("0",id);

        //    using (var reader = command.ExecuteReader())
        //        if (reader.Read()) return Read<T>(reader, 0);


        //    return default(T);
        //}

        public static IEnumerable<int> QueryIntegerArray(OracleConnection connection, string sql, object[] args)
        {
            var command = new OracleCommand(sql, connection);

            if (args != null)
                for (int i = 0; i < args.Length; i++)
                    command.Parameters.Add(i.ToString(), args[i]);


            var list = new List<int>();

            using (var reader = command.ExecuteReader())
                while (reader.Read())                    
                    yield return reader.GetInt32(0);            

        }

        

        public static IEnumerable<Tuple<T1,T2>> QueryArray<T1,T2>(OracleConnection connection, string sql, object[] args)
        {
            var command = new OracleCommand(sql, connection);

            if (args != null)
                for (int i = 0; i < args.Length; i++)
                    command.Parameters.Add(i.ToString(), args[i]);


            var list = new List<int>();

            using (var reader = command.ExecuteReader())
                while (reader.Read())
                    yield return new Tuple<T1,T2>(Read<T1>(reader, 0), Read<T2>(reader, 1));
                    
                


            
        }

        private static T Read<T>(OracleDataReader reader, int idx)
        {


            switch (Type.GetTypeCode(typeof(T)))
            {
                case TypeCode.Int32:
                    return (T)(object)reader.GetInt32(idx);

                case TypeCode.String:
                    return (T)(object)reader.GetString(idx);

                default:
                    throw new NotImplementedException(Type.GetTypeCode(typeof(T)).ToString());

            }
        }

        public static IEnumerable<int> QueryIntegerArray(OracleConnection connection, string sql, int arg0) => QueryIntegerArray(connection, sql, new object[] { arg0 });


        
        public static int? QueryInteger(OracleConnection connection, string column, string table, int id)
        {
            var pkname = GetPkName(connection, table);
            var sql = $"select {column} from {table} where {pkname}=:0";
            return QueryInteger(connection, sql, new object[] { id });
        }


        public static string QueryString(OracleConnection connection, string column, string table, int id)
        {
            var pkname = GetPkName(connection, table);
            var sql = $"select {column} from {table} where {pkname}=:0";
            return QueryValue(connection, sql, new object[] { id }).ToString();
        }


        public static string QueryString(OracleConnection connection, string column, string table, object[] conditions)
        {
            switch (conditions.Length)
            {
                case 2:
                    var sql2 = $"select {column} from {table} where {conditions[0].ToString()}=:0";
                    return QueryString(connection, sql2, new object[] { conditions[1] });
                case 4:
                    var sql4 = $"select {column} from {table} where {conditions[0].ToString()}=:0 and {conditions[2].ToString()}=:1";
                    return QueryString(connection, sql4, new object[] { conditions[1], conditions[3] });
                default:
                    throw new NotImplementedException($"length = {conditions.Length}");
            }
        }


        public static int? QueryInteger(OracleConnection connection, string column, string table, object[] conditions)
        {
            switch (conditions.Length)
            {
                case 2:
                    var sql2 = $"select {column} from {table} where {conditions[0].ToString()}=:0";
                    return QueryInteger(connection, sql2, new object[] { conditions[1] });
                case 4:
                    var sql4 = $"select {column} from {table} where {conditions[0].ToString()}=:0 and {conditions[2].ToString()}=:1";
                    return QueryInteger(connection, sql4, new object[] { conditions[1], conditions[3] });
                default:
                    throw new NotImplementedException($"length = {conditions.Length}");
            }
        }

        //internal static string QueryDateTime(OracleConnection connection, string column, string table, int id)
        //{
        //    var pkname = GetPkName(connection, table);
        //    var sql = $"select {column} from {table} where {pkname}=:0";
        //    return QueryValue(connection, sql, new object[] { id }).ToString();
        //}
        public static DateTime? QueryDateTime(OracleConnection connection, string column, string table, int id)
        {
            var pkname = GetPkName(connection, table);
            var sql = $"select {column} from {table} where {pkname}=:0";
            return QueryDateTime(connection, sql, new object[] { id });
        }



        public static int? QueryInteger(OracleConnection connection, string sql, object[] args)
        {
            var value = QueryValue(connection, sql, args);


            if (value == DBNull.Value) return null;
            if (value == null) return null;

            return Convert.ToInt32(value);
        }


        public static DateTime? QueryDateTime(OracleConnection connection, string sql, object[] args)
        {
            var value = QueryValue(connection, sql, args);


            if (value == DBNull.Value) return null;
            if (value == null) return null;

            return Convert.ToDateTime(value);
        }

        public static int? QueryInteger(OracleConnection connection, string sql, object arg0) => QueryInteger(connection, sql, new object[] { arg0 });

        public static string QueryString(OracleConnection connection, string sql, object[] args)
        {
            var value = QueryValue(connection, sql, args);

            if (value == DBNull.Value) return string.Empty;

            if (value == null) return string.Empty;

            return value.ToString();
        }

        public static void Update(OracleConnection connection, string table, int id, string column, object value)
        {
            var pkname = GetPkName(connection, table);
            var sql = $"update {table} set {column}=:value where {pkname}=:id";
            var command = new OracleCommand(sql, connection);

            command.Parameters.Add("id", id);
            command.Parameters.Add("value", value);

            command.ExecuteNonQuery();
        }

        public static int Insert(OracleConnection connection, string statement, object[] args)
        {
            statement = statement.ToUpper();
            Contract.Requires(statement.Contains("INSERT"));


            if (!statement.Contains("RETURNING"))
            {
                var tableName = FindTableName(statement);
                Debug.Assert(!string.IsNullOrEmpty(tableName));
                var pkName = GetPkName(connection, tableName);

                if (!string.IsNullOrEmpty(pkName))
                    statement = $"{statement} returning {pkName} into :id".ToUpper();

            }

            if (statement.Contains("RETURNING"))
            {

                var command = new OracleCommand(statement, connection);
                command.Parameters
                            .Add("id", OracleDbType.Integer)
                            .Direction = System.Data.ParameterDirection.Output;

                for (int i = 0; i < args.Length; i++)
                    command.Parameters.Add(i.ToString(), args[i]);

                command.ExecuteNonQuery();

                return (int)command.Parameters["id"].Value;
            } else
            {
                var command = new OracleCommand(statement, connection);

                for (int i = 0; i < args.Length; i++)
                    command.Parameters.Add(i.ToString(), args[i]);

                command.ExecuteNonQuery();

                return 0;
            }


        }

        private static string FindTableName(string statement)
        {
            var strings = statement
                            .Replace('(', ' ')
                            .Replace('(', ' ')
                            .ToUpper()
                            .Split(' ');


            if (strings[0] != "INSERT") new ArgumentException("must contain INSERT");
            if (strings[1] != "INTO") new ArgumentException("must contain INTO");

            return strings[2];


        }

        public static int Nextval(OracleConnection connection, string sequence)
        {
            var sql = $"select {sequence}.nextval from dual";

            return (int)QueryInteger(connection, sql, null);

        }

        public static void DML(OracleConnection connection, string statement, object[] args)
        {
            var command = new OracleCommand(statement, connection);

            for (int i = 0; i < args.Length; i++)
                command.Parameters.Add(i.ToString(), args[i]);

            command.ExecuteNonQuery();
        }

        public static void DML(OracleConnection connection, string statement, object arg0, object arg1) => DML(connection, statement, new object[] { arg0, arg1 });
        public static void DML(OracleConnection connection, string statement) => DML(connection, statement, new object[0]);
        

    }
}
