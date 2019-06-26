using Devart.Data.Oracle;
using OraTables;

namespace BO
{
    public class Report : IdentificatedBO
    {
        public Report(OracleConnection connection, int id) : base(connection, id)
        {
        }

        public override Table Table => Tables.AsutpProtPress;

        public string Url => QueryString("file_url");
        public string Filename => QueryString("source_file_name");
    }
}