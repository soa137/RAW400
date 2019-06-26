using Devart.Data.Oracle;
using OraPackages;
using OraTables;
using System.Collections.Generic;

namespace BO
{
    internal class OperRashod:IdentificatedBO
    {
        public OperRashod(OracleConnection connection, int id) : base(connection, id)
        {
        }

        public IEnumerable<Work> Work
        {
            get
            {
                foreach (var workid in Tables.VVFactWorkers.QueryIntegerArray(connection, "vv_fact_workers_id", new object[] { "oper_rashod_id", Id })) 
                    yield return new Work(connection, workid);
            }
        }

        internal void CompleteWork(Resource resource)
        {
            var ctx = new SessionContext(connection);

            CompleteWork((int)ctx.Untn, resource);
        }

        internal void CompleteWork(int untn, Resource resource)
        {
            var principal = new Principal(connection, untn);
            var smen = Smen.Current(connection);
            NewWork(principal, resource, 1, smen);
        }



        private void NewWork(Principal principal, Resource resource, double ken, Smen smen)
        {
            //if (resource.GlobalId == null) throw new NotImplementedException();

            WorkPkg.InsertWork(connection, Id, null, null, principal.Untn, null, ken, resource.SprOborId, resource.GlobalId, smen.Date, smen.Num, principal.BrigadaId, null);
        }
    }
}