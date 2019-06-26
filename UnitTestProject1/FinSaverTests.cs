using BO;
using Devart.Data.Linq.Monitoring;
using Devart.Data.Oracle;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OraTables;
using RawLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestProject1
{


    [TestClass]
    public class FinSaverTests
    {
        private TSTDAO A = new TSTDAO();
        
        
        


        [TestMethod]
        public void Save_DateTimeStart()
        {
            

            A.NewUser();

            

            var psp = A.PasportRandom(1);

            
            Assert.AreEqual(200, psp.Document.Rank);

            A.Commit();

            


            var dto = new ProcessDTO {                
                OperationId=psp.Oper[0],
                ZagotId=psp.Zagot[0],
                EquipmentId = A.NewResource().Id,
                DateTimeStart = new DateTime(2018, 2, 1),
                PersonUntn = A.SessionUntn,
            };            

            new FinSaver(A.Connection).Save(dto);


            var oz = new OperZagot(A.Connection, psp.Oper[0], psp.Zagot[0]);

            Assert.IsNotNull(oz.DeformStart);
            Assert.AreEqual(dto.DateTimeStart, oz.DeformStart);
        }


        [TestMethod]
        public void Save_DateTimeFinish()
        {

            A.NewUser();

            var psp = A.PasportRandom(1);



            var dto = new ProcessDTO
            {
                OperationId = psp.Oper[0],
                ZagotId = psp.Zagot[0],
                EquipmentId = A.NewResource().Id,
                DateTimeFinish = new DateTime(2018, 2, 2),
                PersonUntn = A.SessionUntn,
            };

            new FinSaver(A.Connection).Save(dto);

            var oz = new OperZagot(A.Connection, psp.Oper[0], psp.Zagot[0]);

            Assert.AreEqual(dto.DateTimeFinish, oz.DeformFinish);
        }

        [TestMethod]
        public void Save_Height_IsNull()
        {
            A.NewUser();

            var psp = A.PasportRandom(1);

            var dto = new ProcessDTO
            {
                OperationId = psp.Oper[0],
                ZagotId = psp.Zagot[0],
                EquipmentId = A.NewResource().Id,
                PersonUntn = A.SessionUntn,
                Height = -1,
            };

            new FinSaver(A.Connection).Save(dto);

            var oz = new OperZagot(A.Connection, psp.Oper[0], psp.Zagot[0]);

            Assert.IsNull(oz.Height);
        }

        [TestMethod]
        public void Save_DiameterInner_IsNull()
        {
            A.NewUser();

            var psp = A.PasportRandom(1);

            var dto = new ProcessDTO
            {
                OperationId = psp.Oper[0],
                ZagotId = psp.Zagot[0],
                EquipmentId = A.NewResource().Id,
                PersonUntn = A.SessionUntn,
                DiameterInner = -1,
            };

            new FinSaver(A.Connection).Save(dto);

            var oz = new OperZagot(A.Connection, psp.Oper[0], psp.Zagot[0]);

            Assert.IsNull(oz.DiameterInner);
        }

        [TestMethod]
        public void Save_DiameterOuter_IsNull()
        {
            A.NewUser();

            var psp = A.PasportRandom(1);

            var dto = new ProcessDTO
            {
                OperationId = psp.Oper[0],
                ZagotId = psp.Zagot[0],
                EquipmentId = A.NewResource().Id,
                PersonUntn = A.SessionUntn,
                DiameterOuter = -1,
            };

            new FinSaver(A.Connection).Save(dto);

            var oz = new OperZagot(A.Connection, psp.Oper[0], psp.Zagot[0]);

           // "Just fot commit test_0"

            Assert.IsNull(oz.DiameterOuter);
        }

        [TestMethod]
        public void Save_Height()
        {
            A.NewUser();

            var psp = A.PasportRandom(1);

            var dto = new ProcessDTO
            {
                OperationId = psp.Oper[0],
                ZagotId = psp.Zagot[0],
                EquipmentId = A.NewResource().Id,
                Height = 150,
                PersonUntn = A.SessionUntn,
            };

            new FinSaver(A.Connection).Save(dto);

            var oz = new OperZagot(A.Connection, psp.Oper[0], psp.Zagot[0]);


            Assert.AreEqual(dto.Height, oz.Height);

        }

        [TestMethod]
        public void Save_DiameterInner()
        {
            A.NewUser();

            var psp = A.PasportRandom(1);

            var dto = new ProcessDTO
            {
                OperationId = psp.Oper[0],
                ZagotId = psp.Zagot[0],
                EquipmentId = A.NewResource().Id,
                DiameterInner = 10,
                PersonUntn = A.SessionUntn,
            };

            new FinSaver(A.Connection).Save(dto);

            var oz = new OperZagot(A.Connection, psp.Oper[0], psp.Zagot[0]);

            Assert.AreEqual(dto.DiameterInner, oz.DiameterInner);

        }

        [TestMethod]
        public void Save_DiameterOuter()
        {
            A.NewUser();

            var psp = A.PasportRandom(1);

            var dto = new ProcessDTO
            {
                OperationId = psp.Oper[0],
                ZagotId = psp.Zagot[0],
                EquipmentId = A.NewResource().Id,
                DiameterOuter = 20,
                PersonUntn = A.SessionUntn,
            };

            new FinSaver(A.Connection).Save(dto);

            var oz = new OperZagot(A.Connection, psp.Oper[0], psp.Zagot[0]);

            Assert.AreEqual(oz.DiameterOuter, dto.DiameterOuter);
        }

        [TestMethod]
        public void Exception_0()
        {
            var dto = new ProcessDTO ();

            Assert.ThrowsException<ArgumentException>(()=> new FinSaver(A.Connection).Save(dto));
        }


        [TestMethod]
        public void Exception_1()
        {
            // no OperId & ZagotId
            var dto = new ProcessDTO { PassportId = 1 };

            Assert.ThrowsException<ArgumentException>(() => new FinSaver(A.Connection).Save(dto));
        }


        [TestMethod]
        public void Exception_2()
        {
            // no ZagotId
            var dto = new ProcessDTO { OperationId = 1 };

            Assert.ThrowsException<ArgumentException>(() => new FinSaver(A.Connection).Save(dto));
        }

        [TestMethod]
        public void Exception_3()
        {
            //  OperId & ZagotId does not exist
            var dto = new ProcessDTO { OperationId = 3, ZagotId = 3 };

            Assert.ThrowsException<ArgumentException>(() => new FinSaver(A.Connection).Save(dto));
        }

        [TestMethod]
        public void Exception_4()
        {
            

            var psp = A.PasportRandom(1);
            //  OperId  does not exist
            var dto = new ProcessDTO { OperationId = 3, ZagotId = psp.Zagot[0] };

            Assert.ThrowsException<ArgumentException>(() => new FinSaver(A.Connection).Save(dto));
        }




        [TestMethod]
        public void Exception_Invalid_Resource()
        {
            A.NewUser();
            var psp = A.PasportRandom(1);            
            var dto = new ProcessDTO { OperationId = psp.Oper[0], ZagotId = psp.Zagot[0], PersonUntn = A.SessionUntn };
            var saver = new FinSaver(A.Connection);

            Assert.ThrowsException<ArgumentException>(() => saver.Save(dto));

            dto.EquipmentId = 1; // non-existent Equipment

            Assert.ThrowsException<ArgumentException>(() => saver.Save(dto));


            dto.EquipmentId = A.NewResource().Id;

            try { saver.Save(dto); }
            catch (ArgumentException) { throw; }
            catch (Exception) { };



        }      
      
        [TestMethod]
        public void Exception_Invalid_Untn()
        {
            A.NewUser();
            var psp = A.PasportRandom(1);
            var dto = new ProcessDTO { OperationId = psp.Oper[0], ZagotId = psp.Zagot[0], EquipmentId = A.NewResource().Id};
            var saver = new FinSaver(A.Connection);

            Assert.ThrowsException<ArgumentException>(() => saver.Save(dto));
       
            dto.PersonUntn = A.SessionUntn;

            try { saver.Save(dto); }
            catch (ArgumentException) { throw; }
            catch (Exception) { };
          
        }

        [TestMethod]
        public void Save_Signal()
        {
            A.NewUser();

            A.Commit();

            

            var psp = A.PasportRandom(1);
            var res = A.NewResource();

            A.Commit();

            var dto = new ProcessDTO { OperationId = psp.Oper[0], ZagotId = psp.Zagot[0], EquipmentId=res.Id, PersonUntn = A.SessionUntn };

            var saver = new FinSaver(A.Connection);

            saver.Save(dto);

            var signals = Signal.Find(A.Connection, (int)dto.OperationId, (int)dto.ZagotId, (int)dto.EquipmentId);




            Assert.AreEqual(1, signals.Count());
        }



        [TestMethod]
        public void Oper_Zagot_Are_Empty()
        {
            var dto = new ProcessDTO { OperationId = -1, ZagotId = -1,  EquipmentId = A.NewResource().Id };
            var saver = new FinSaver(A.Connection);

            var cnt0 = Tables.RppSignal.Count(A.Connection);

            saver.Save(dto);

            var cnt1 = Tables.RppSignal.Count(A.Connection);

            Assert.AreEqual(1, cnt1 - cnt0);

        }


        [TestMethod]
        public void Zagot_Is_Empty()
        {
            A.NewUser();
            var oz = A.NewOperZagot();


            var dto = new ProcessDTO { OperationId = oz.OperId, ZagotId = -1, EquipmentId = A.NewResource().Id, PersonUntn=A.SessionUntn };
            var saver = new FinSaver(A.Connection);

            var cnt0 = Tables.RppSignal.Count(A.Connection);

            saver.Save(dto);

            var cnt1 = Tables.RppSignal.Count(A.Connection);

            Assert.AreEqual(1, cnt1 - cnt0);

        }


        [TestMethod]
        public void Oper_Is_Empty()
        {
            A.NewUser();
            var oz = A.NewOperZagot();


            var dto = new ProcessDTO { OperationId = -1, ZagotId = oz.ZagotId, EquipmentId = A.NewResource().Id, PersonUntn = A.SessionUntn };
            var saver = new FinSaver(A.Connection);

            var cnt0 = Tables.RppSignal.Count(A.Connection);

            saver.Save(dto);

            var cnt1 = Tables.RppSignal.Count(A.Connection);

            Assert.AreEqual(1, cnt1 - cnt0);

        }




        [TestMethod]
        public void Write_Error_0()
        {
            var FILE_NAME = "A_FILENAME";
            var FILE_PATH = "\\AAA\\BBB\\" + FILE_NAME;
            var URL = "A_URL";

            var dto = new ProcessDTO { EquipmentId = A.NewResource().Id, ReportPath=FILE_PATH, ReportURL=URL };

            var saver = new FinSaver(A.Connection);

            var signal = saver.SaveErrorSignal(dto);

            Assert.AreEqual(FILE_NAME, signal.ReportFilename);
            Assert.AreEqual(URL, signal.ReportUrl);


        }

        [TestMethod]
        public void Save_Person_0()
        {
            A.NewUser();

            var psp = A.PasportRandom(1);
            var res = A.NewResource();

            A.Commit();

            var Person = new Person(A.Connection, A.RandomUntn());

            var oz = new OperZagot(A.Connection, psp.Oper[0], psp.Zagot[0]);

            var dto = new ProcessDTO { OperationId = oz.OperId, ZagotId = oz.ZagotId, EquipmentId = res.Id, PersonUntn = Person.Untn };

            var saver = new FinSaver(A.Connection);

            saver.Save(dto);

            Assert.AreEqual(1, oz.Work.Count());
            Assert.AreEqual(Person, oz.Work.First().Person);


        }



        [TestMethod]
        public void Save_Person_1()
        {           
            A.NewUser();

            var psp = A.PasportRandom(1);
            var res = A.NewResource();

            A.Commit();

            var Person = new Person(A.Connection, A.RandomUntn());

            var oz = new OperZagot(A.Connection, psp.Oper[0], psp.Zagot[0]);

            oz.CompleteWork(res,Person);

            var dto = new ProcessDTO { OperationId = oz.OperId, ZagotId = oz.ZagotId, EquipmentId = res.Id, PersonUntn = Person.Untn };

            var saver = new FinSaver(A.Connection);

            saver.Save(dto);

            Assert.AreEqual(1, oz.Work.Count());
            Assert.AreEqual(Person, oz.Work.First().Person);


        }


        [TestMethod]
        public void Save_Report_0()        
        {
            A.NewUser();
            var psp = A.PasportRandom(1);
            var res = A.NewResource();

            var OZ = new OperZagot(A.Connection, psp.Oper[0], psp.Zagot[0]);

            var dto = new ProcessDTO { OperationId = OZ.OperId, ZagotId = OZ.ZagotId, EquipmentId = res.Id, PersonUntn = A.SessionUntn };

            var saver = new FinSaver(A.Connection);

            saver.Save(dto);

            Assert.AreEqual(0, OZ.Reports.Count());
        }


        [TestMethod]
        public void Save_Report_1()
        {
            var REPORT_URL = "IT_IS_FILE_URL";
            var FILE_NAME = "IT_IS_FILE_NAME";

            A.NewUser();
            var psp = A.PasportRandom(1);
            var res = A.NewResource();

            var OZ = new OperZagot(A.Connection, psp.Oper[0], psp.Zagot[0]);

            var dto = new ProcessDTO { ReportURL = REPORT_URL, ReportPath= FILE_NAME, OperationId = OZ.OperId, ZagotId = OZ.ZagotId, EquipmentId = res.Id, PersonUntn = A.SessionUntn };

            var saver = new FinSaver(A.Connection);

            saver.Save(dto);

            Assert.AreEqual(1, OZ.Reports.Count());
            Assert.AreEqual(REPORT_URL, OZ.Reports.First().Url);
            Assert.AreEqual(FILE_NAME, OZ.Reports.First().Filename);

        }


        [TestMethod]
        public void Save_Report_2()
        {
            var REPORT_URL = "IT_IS_FILE_URL";
            var FILE_NAME = "IT_IS_FILE_NAME";
            var FILE_PATH = $"\\DIR1\\Dir2\\{FILE_NAME}";

            A.NewUser();
            var psp = A.PasportRandom(1);
            var res = A.NewResource();

            var OZ = new OperZagot(A.Connection, psp.Oper[0], psp.Zagot[0]);

            var dto = new ProcessDTO { ReportURL = REPORT_URL, ReportPath = FILE_PATH, OperationId = OZ.OperId, ZagotId = OZ.ZagotId, EquipmentId = res.Id, PersonUntn = A.SessionUntn };

            var saver = new FinSaver(A.Connection);

            saver.Save(dto);

            Assert.AreEqual(1, OZ.Reports.Count());
            Assert.AreEqual(REPORT_URL, OZ.Reports.First().Url);
            Assert.AreEqual(FILE_NAME, OZ.Reports.First().Filename);

        }

        [TestMethod]
        public void Save_Without_Report()
        {

            A.NewUser();            

            var OZ = A.NewOperZagot();

            

            var dto = new ProcessDTO {OperationId = OZ.OperId, ZagotId = OZ.ZagotId, EquipmentId = A.NewResource().Id, PersonUntn = A.SessionUntn };

            var saver = new FinSaver(A.Connection);

            saver.Save(dto);

            Assert.AreEqual(0, OZ.Reports.Count());

        }






    }
    
}
