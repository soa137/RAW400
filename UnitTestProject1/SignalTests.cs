using BO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OraTables;
using System.Linq;

namespace UnitTestProject1
{
    [TestClass]
    public class SignalTests
    {

        private TSTDAO A = new TSTDAO();

        [TestMethod]
        public void Write_0()
        {
            var psp = A.PasportRandom(1);
            var res = A.NewResource();


            Triad triad = new Triad(A.Connection, psp.Oper[0], psp.Zagot[0], res.Id);

            var cnt = Tables.RppSignal.Count(A.Connection);

            Signal.Write(triad);

            Assert.AreEqual(cnt + 1, Tables.RppSignal.Count(A.Connection));
        }



        [TestMethod]
        public void Write_1()
        {
            

            var psp = A.PasportRandom(1);
            var res = A.NewResource();


            Triad triad = new Triad(A.Connection, psp.Oper[0], psp.Zagot[0], res.Id);

            Signal.Write(triad);

            Assert.AreEqual(1, Signal.Find(triad).Count());
        }

        [TestMethod]
        public void Write_DateAsutp_0()
        {
            var res = A.NewResource();

            var signal = Signal.WriteEmpty(A.Connection, res.Id);

            
            Assert.IsNotNull(signal.DateAsutp);
        }


        [TestMethod]
        public void Write_DateAsutp_1()
        {


            var psp = A.PasportRandom(1);
            var res = A.NewResource();


            Triad triad = new Triad(A.Connection, psp.Oper[0], psp.Zagot[0], res.Id);

            var signal = Signal.Write(triad);

            Assert.IsNotNull(signal.DateAsutp);
        }


        [TestMethod]
        public void Write_Info()
        {


            var psp = A.PasportRandom(1);
            var res = A.NewResource();



            Triad triad = new Triad(A.Connection, psp.Oper[0], psp.Zagot[0], res.Id);

            var signal = Signal.Write(triad);

            var oper = new Oper(A.Connection, psp.Oper[0]);


            StringAssert.Contains(signal.TextInfo, psp.Nplav);
            StringAssert.Contains(signal.TextInfo, psp.Npart);
            StringAssert.Contains(signal.TextInfo, oper.Text);
            StringAssert.Contains(signal.TextInfo, oper.Npp);


        }


        [TestMethod]
        public void WriteError()
        {

            var signal = Signal.WriteError(A.Connection, null, A.NewResource().Id, null, null );

            var f = Tables.RppSignal.QueryInteger(A.Connection, "pr_error", signal.Id);

            Assert.AreEqual(1, f);

        }

        [TestMethod]
        public void WriteError_1()
        {

            var signal = Signal.WriteError(A.Connection, null, A.NewResource().Id, null, null);

            var f = Tables.RppSignal.QueryInteger(A.Connection, "pr_error", signal.Id);

            Assert.AreEqual(1, f);

        }
    }




}
