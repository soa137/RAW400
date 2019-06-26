using Devart.Data.Oracle;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OraTables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestProject1
{
    [TestClass]
    public class QMTests
    {
        private TSTDAO A = new TSTDAO();

        private readonly string TABLE_NAME = "A_TEST_23421";
        private readonly string INT_1 = "INT_1";

        [TestMethod]
        public void Insert()
        {
            var cmd1 = new OracleCommand($"drop table {TABLE_NAME}", A.GRPOConnection);
            var cmd2 = new OracleCommand($"create table {TABLE_NAME} ({INT_1} int) ", A.GRPOConnection);

            try { cmd1.ExecuteNonQuery(); }
            catch (OracleException e) { if (e.Code != 942) throw; }                          

            cmd2.ExecuteNonQuery();

            QM.Insert(A.GRPOConnection, $"insert into {TABLE_NAME} ({INT_1}) values (1)", new object[0]);

            Assert.AreEqual(1, new Table(TABLE_NAME).Count(A.GRPOConnection));

        }
    }
}
