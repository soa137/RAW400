using BO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OraTables;
using System;
using System.Linq;


namespace UnitTestProject1
{


    [TestClass]
    public class OperZagotTests
    {

        private TSTDAO A = new TSTDAO();

        [TestMethod]
        public void Prop_0()
        {
            A.NewUser();

            var psp = A.PasportRandom(1);

            A.Commit();

            var oz = new OperZagot(A.Connection, psp.Oper[0], psp.Zagot[0]);

            oz.CompleteWork(A.NewResource());

            var d = new DateTime(2018, 05, 07, 11, 30, 00);

            oz.DeformStart = d;

            Assert.AreEqual(d, oz.DeformStart);
        }


        [TestMethod]
        public void Work_Complete_Untn()
        {
            var psp = A.PasportRandom(1);
            var res = A.NewResource();

            A.Commit();

            var oz = new OperZagot(A.Connection, psp.Oper[0], psp.Zagot[0]);

            Assert.IsNull(new SessionContext(A.Connection).Untn);
            Assert.ThrowsException<InvalidOperationException>(() => oz.CompleteWork(res));

            A.NewUser();
            Assert.IsNotNull(new SessionContext(A.Connection).Untn);
            oz.CompleteWork(res);




        }

        [TestMethod]
        public void Work_Complete_Untn_1()
        {
            var psp = A.PasportRandom(1);
            var res = A.NewResource();

            A.Commit();

            var oz = new OperZagot(A.Connection, psp.Oper[0], psp.Zagot[0]);

            Assert.IsNull(A.SessionUntn);
            Assert.ThrowsException<InvalidOperationException>(() => oz.CompleteWork( res, A.SessionPerson));

        }


        [TestMethod]
        public void Add_Report_Exceptions()
        {
            A.NewUser();

            var psp = A.PasportRandom(1);            

            var oz = new OperZagot(A.Connection, psp.Oper[0], psp.Zagot[0]);

            Assert.ThrowsException<ArgumentNullException>(() => oz.AddReport(string.Empty, "x"));
            Assert.ThrowsException<ArgumentNullException>(() => oz.AddReport("x", string.Empty));
            Assert.ThrowsException<InvalidOperationException>(() => oz.AddReport("x", "x"));

            oz.CompleteWork(A.NewResource());

            oz.AddReport("x", "x"); // Ensure no exceptions
        }

        [TestMethod]
        public void Add_Report_0()
        {
            var FILENAME = "A_FILENAME";
            var URL = "A_URL";

            A.NewUser();

            var psp = A.PasportRandom(1);

            var oz = new OperZagot(A.Connection, psp.Oper[0], psp.Zagot[0]);            

            oz.CompleteWork(A.NewResource());

            oz.AddReport(URL, FILENAME);

            Assert.AreEqual(1, oz.Reports.Count());
            Assert.AreEqual(URL, oz.Reports.First().Url);
            Assert.AreEqual(FILENAME, oz.Reports.First().Filename);
        }

    }

    

    [TestClass]
    public class OperZagotCollectionTests
    {

        private TSTDAO A = new TSTDAO();

        [TestMethod]
        public void Make_0()
        {
            var psp = A.PasportRandom(1);

            var oz = new OperZagot(A.Connection, psp.Oper[0], psp.Zagot[0]);

            var ozc = OperZagotCollection.Make(oz);

            Assert.AreEqual(1, ozc.OperZagot.Count());            
            Assert.AreEqual(oz, ozc.OperZagot.First());
        }

        [TestMethod]
        public void InArr_0()
        {
            var operId = 1;
            var zagotId = 2;

            Tables.InArrZagot.DeleteAll(A.Connection);
            Tables.InArrZagot.Insert(A.Connection, "oper_id", operId, "zagot_id", zagotId);

            var arr = Tables.InArrZagot.QueryAll<int, int>(A.Connection,  "oper_id", "zagot_id" );

            Assert.AreEqual(1, arr.Count());
            Assert.AreEqual(operId, arr.First().Item1);
            Assert.AreEqual(zagotId, arr.First().Item2);


        }

        [TestMethod]
        public void OperRashod_Null_Untn()
        {
            

            var userid= A.NewUserId(false);
            new SessionContext(A.Connection).SetSessionUser(userid);

            Assert.IsNull(A.SessionUntn);

            var oz = A.NewOperZagot();

            var resource = A.NewResource();
            var person = new Person(A.Connection, A.RandomUntn());

            oz.CompleteWork(resource, person); // Ensure no exceptions
            

        }
    }






}
