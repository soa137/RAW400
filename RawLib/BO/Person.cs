using Devart.Data.Oracle;
using OraPackages;

namespace BO
{
    public class Person : IdentificatedBO
    {
        public Person(OracleConnection connection, int untn) : base(connection, untn)
        {
        }

        public int Untn => Id;

        public string Fio => NUserPack.GetFioUntn(connection, Untn);

        public override string ToString()
        {
            return $"{Fio} [{Untn}]";
        }
    }
}