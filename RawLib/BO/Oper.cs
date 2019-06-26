using Devart.Data.Oracle;
using OraTables;

namespace BO
{
    public class Oper : IdentificatedBO
    {
        public Oper(OracleConnection connection, int id) : base(connection, id)
        {
        }

        public string Text => Tables.Oper.QueryString(connection, "oper_text", Id);
        public string Npp=> Tables.Oper.QueryString(connection, "npp", Id);
    }
}