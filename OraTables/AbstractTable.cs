using Devart.Data.Oracle;
using System;
using System.Collections.Generic;
using System.Text;

namespace OraTables
{
    public class Table
    {
        private string table;

        public Table(string table)
        {
            this.table = table;
        }

        public bool Exists(OracleConnection connection, int id)
        {           
            var pkname = QM.GetPkName(connection, table);
            return QueryInteger(connection, pkname, id) != null; ;
        }

        

        public int? QueryInteger(OracleConnection connection, string column, int id) => QM.QueryInteger(connection, column, table, id);
        public int? QueryInteger(OracleConnection connection, string column, object[] conditions) => QM.QueryInteger(connection, column, table, conditions);
        public int? QueryInteger(OracleConnection connection, string column, string where0, object value0) => QueryInteger(connection, column, new object[] { where0, value0 });
        public string QueryString(OracleConnection connection, string column, int id) => QM.QueryString(connection, column, table, id);
        public string QueryString(OracleConnection connection, string column,  object[] conditions) => QM.QueryString(connection, column, table, conditions);
        public string QueryString(OracleConnection connection, string column, string where0, object value0) => QueryString(connection, column, new object[] { where0, value0 });

        public DateTime? QueryDateTime(OracleConnection connection, string column, int id) => QM.QueryDateTime(connection, column, table, id);

        internal IEnumerable<int> QueryIntegerArray(OracleConnection connection, string column, object[] conditions) {
            switch (conditions.Length)
            {
                case 2:
                    var sql2 = $"select {column} from {table} where {conditions[0].ToString()}=:0";
                    return QM.QueryIntegerArray(connection, sql2, new object[] { conditions[1] });
                case 4:
                    var sql4 = $"select {column} from {table} where {conditions[0].ToString()}=:0 and {conditions[2].ToString()}=:1";
                    return QM.QueryIntegerArray(connection, sql4, new object[] { conditions[1], conditions[3] });
                default:
                    throw new NotImplementedException($"length = {conditions.Length}");
            }
        }

        public IEnumerable<int> QueryIntegerArray(OracleConnection connection, string column, string where0, object value0) => QueryIntegerArray(connection, column, new object[] { where0, value0 });
        public IEnumerable<int> QueryIntegerArray(OracleConnection connection, string column, string where0, object value0, string where1, object value1) => QueryIntegerArray(connection, column, new object[] { where0, value0, where1, value1 });
        public IEnumerable<int> QueryPrimaryKeyArray(OracleConnection connection, string where0, object value0)
        {
            var column = QM.GetPkName(connection, table);

            return QueryIntegerArray(connection, column, where0, value0);
        }


        public IEnumerable<int> QueryPrimaryKeyArray(OracleConnection connection, string where0, object value0, string where1, object value1)
        {
            var column = QM.GetPkName(connection, table);

            return QueryIntegerArray(connection, column, where0, value0, where1, value1);
        }


        public int Insert(OracleConnection connection, object[] args)
        {
            

            if (args.Length % 2 != 0) throw new ArgumentException("number of args must be even");

            int M = args.Length / 2;

            switch(M)
            {
                case 1:
                    var statement1 = $"insert into {table} ({args[0]}) values (:0)";
                    return QM.Insert(connection, statement1, new object[] { args[1] });                    

                case 2:
                    var statement2 = $"insert into {table} ({args[0]},{args[2]}) values (:0, :1)";
                    return QM.Insert(connection, statement2, new object[] { args[1], args[3] });

                case 3:
                    var statement3 = $"insert into {table} ({args[0]},{args[2]},{args[4]}) values (:0, :1, :2)";
                    return QM.Insert(connection, statement3, new object[] { args[1], args[3], args[5] });

                case 4:
                    var statement4 = $"insert into {table} ({args[0]},{args[2]},{args[4]},{args[6]}) values (:0, :1, :2, :3)";
                    return QM.Insert(connection, statement4, new object[] { args[1], args[3], args[5], args[7] });

                default:
                    throw new NotImplementedException($"M = {M}");

            }
        }

        public int Insert(OracleConnection connection, object column, object value) => Insert(connection, new object[] { column, value });
        public int Insert(OracleConnection connection, string column0, object value0, string column1, object value1) => Insert(connection, new object[] { column0, value0, column1, value1 });
        public int Insert(OracleConnection connection, string column0, object value0, string column1, object value1, string column2, object value2) => Insert(connection, new object[] { column0, value0, column1, value1, column2, value2 });
        public int Insert(OracleConnection connection, string column0, object value0, string column1, object value1, string column2, object value2, string column3, object value3) => Insert(connection, new object[] { column0, value0, column1, value1, column2, value2, column3, value3 });


        public void Update(OracleConnection connection, int Id, string column, object value)
        {
            QM.Update(connection, table, Id, column, value);
        }

        public int Count(OracleConnection connection)
        {            
            return (int)QM.QueryInteger(connection, $"select count(*) from {table}", new object[0]);
        }

        

        public IEnumerable<Tuple<T1,T2>> QueryAll<T1, T2>(OracleConnection connection, string column0, string column1)
        {
            var sql = $"select {column0}, {column1} from {table}";

            return QM.QueryArray<T1, T2>(connection, sql, new object[0]);
        }

        

        public void DeleteAll(OracleConnection connection)
        {
            QM.DML(connection, $"delete {table}");
        }

        
    }
}
