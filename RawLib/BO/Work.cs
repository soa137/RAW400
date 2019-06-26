using Devart.Data.Oracle;
using OraTables;

namespace BO
{
    public class Work:IdentificatedBO
    {
        

        public Work(OracleConnection connection, int id) : base(connection, id)
        {
        }

        private int? Untn => Table.QueryInteger(connection, "untn", Id);

        public override Table Table => Tables.VVFactWorkers;

        public Person Person => Untn == null ? null : new Person(connection, (int)Untn);
    }
}