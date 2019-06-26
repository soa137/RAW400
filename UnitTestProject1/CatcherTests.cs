using Devart.Data.Oracle;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RawLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestProject1
{

    class StringSaver : IPasportSaver
    {        
        internal string Json;

        public ProcessDTO DTO { get; private set; }

        public void Save(ProcessDTO dto)
        {
            
            Json += dto.Json;
            DTO = dto;
        }
    }

    
    class ErrorSaver : IPasportSaver
    {
        private readonly string message;

        public ErrorSaver(string message = null)
        {
            this.message = message;
        }

        public void Save(ProcessDTO dto)
        {
            throw new Exception(message);
            
        }
    }


    class MemoryFileStorage : IFileStorage
    {
        public Dictionary<string, string> Dict = new Dictionary<string, string>();


        private string StreamToString(MemoryStream stream)
        {
            string myString;  // outside using

            stream.Position = 0;
            using (var reader = new StreamReader(stream))
            {
                myString = reader.ReadToEnd();
            }

            return myString;
        }

        public string Upload(string path, Stream stream)
        {            
            Dict[path] = StreamToString((MemoryStream)stream);

            return path;
        }
    }





    [TestClass]
    public class CatcherTests
    {


        [TestMethod]
        public void Catcher_Test_0()
        {

            var src = new Mock<ISourceDirectory>();
            src.Setup(a => a.GetFiles()).Returns(new List<string> { "F1", "F2", "F3", "F4", "F5" });
            src.Setup(a => a.GetFileText(It.IsAny<string>())).Returns("{}");

            var saver = new Mock<IPasportSaver>();
            var fs = new MemoryFileStorage();

            var catcher = new Catcher(src.Object, new MemoryHandledStorage(), saver.Object, fs);


            catcher.Go();

            saver.Verify(a => a.Save(It.IsAny<ProcessDTO>()), Times.Exactly(5));


        }

        [TestMethod]
        public void Catcher_Test_Files()
        {


            var src1 = new StringSource("F1,F2,F3,F4,F5", "{},{},{},{},{}" );
            var saver = new StringSaver();
            var fs = new MemoryFileStorage();

            var catcher = new Catcher(src1, new MemoryHandledStorage(), saver, fs);

            catcher.Go();

            Assert.AreEqual("{}{}{}{}{}", saver.Json);

            catcher.Go();

            Assert.AreEqual("{}{}{}{}{}", saver.Json);

        }


        [TestMethod]
        public void Catcher_Report()
        {

            var src = new StringSource($"F1", "{\"ReportPath\":\"APATH.pdf\"}");
            src.AddFile("APATH.pdf", "12345");

            var hnd = new MemoryHandledStorage();
            var saver = new StringSaver();
            var f = new MemoryFileStorage();

            var catcher = new Catcher(src, hnd, saver, f);

            catcher.Go();

            Assert.AreEqual("12345", f.Dict["APATH.pdf"]);

        }


        [TestMethod]
        public void Catcher_Report_1()
        {

            var src = new StringSource($"F1", "{\"ReportPath\":\"APATH.PDF\"}");
            src.AddFile("APATH.PDF", "12345");

            var hnd = new MemoryHandledStorage();
            var saver = new StringSaver();
            var f = new MemoryFileStorage();

            var catcher = new Catcher(src, hnd, saver, f);

            catcher.Go();

            Assert.AreEqual("12345", f.Dict["APATH.PDF"]);

        }


        [TestMethod]
        public void Catcher_Report_2()
        {
            var APATH = "APATH";

            var src = new StringSource($"F1", "{\"ReportPath\":\""+APATH+"\"}");
            src.AddFile(APATH, "12345");

            var hnd = new MemoryHandledStorage();
            var saver = new StringSaver();
            var f = new MemoryFileStorage();

            var catcher = new Catcher(src, hnd, saver, f);

            catcher.Go();



            Assert.IsFalse(f.Dict.Keys.Contains(APATH));
            
            
            

        }


        [TestMethod]
        public void Catcher_Report_3()
        {
            var APATH = "APATH.PDF";

            var src = new StringSource($"F1", "{\"ReportPath\":\"" + APATH + "\"}");
            src.AddFile(APATH, "FILE_CONTENT_HERE");

            var hnd = new MemoryHandledStorage();
            var saver = new StringSaver();
            var f = new MemoryFileStorage();

            var catcher = new Catcher(src, hnd, saver, f);

            catcher.Go();

            var url = APATH;

            Assert.IsNotNull(saver.DTO.ReportURL);
            Assert.AreEqual(url, saver.DTO.ReportURL);
        }


        [TestMethod]
        public void Catcher_Error()
        {
            var ERRM = "ERRM";

            var src1 = new StringSource("F1", "{ }");
            var saver = new ErrorSaver(ERRM);
            var fs = new MemoryFileStorage();
            var hs = new MemoryHandledStorage();

            var catcher = new Catcher(src1, hs, saver, fs);

            catcher.Go();

            Assert.IsTrue(hs.ErrorTable.ContainsKey("F1"));
            Assert.AreEqual(ERRM, hs.ErrorTable["F1"]);
            

            

        }


        [TestMethod]
        public void Catcher_Queue_0()
        {
            var JSON_A = "{a:1}";
            var JSON_B = "{b:1}";

            var src = new StringSource("F1", JSON_A);
            var saver = new StringSaver();
            var fs = new MemoryFileStorage();

            var catcher = new Catcher(src, new MemoryHandledStorage(), saver, fs);

            catcher.Go();

            Assert.AreEqual(JSON_A, saver.Json);

            src.Append("F2", JSON_B);

            catcher.Go();

            Assert.AreEqual($"{JSON_A}{JSON_B}", saver.Json);
        }

        [TestMethod]
        public void Catcher_Queue_Err()
        {
            var JSON_OK = "{a:1}";
            var JSON_ERR = "ERRR";

            var src = new StringSource("F1", JSON_OK);
            src.Append("F2", JSON_ERR);

            var saver = new StringSaver();
            var fs = new MemoryFileStorage();

            var catcher = new Catcher(src, new MemoryHandledStorage(), saver, fs);

            catcher.Go();

            Assert.AreEqual(0, catcher.GetNewFiles().Count);

            
        }


        [TestMethod]
        public void Catcher_Queue_Establish()
        {
            var JSON_OK = "{a:1}";
            var JSON_ERR = "ERRR";

            var src = new StringSource("F1", JSON_OK);
            src.Append("F2", JSON_ERR);

            var saver = new StringSaver();
            var fs = new MemoryFileStorage();
            var hnd = new MemoryHandledStorage();

            var catcher = new Catcher(src, hnd, saver, fs);

            catcher.Establish();

            Assert.AreEqual(0, fs.Dict.Count);
            Assert.IsTrue(string.IsNullOrEmpty(saver.Json));
            Assert.AreEqual(0, catcher.GetNewFiles().Count);
            Assert.AreEqual(0, hnd.ErrorTable.Count);
            Assert.AreEqual(2, hnd.Table.Count);


        }
    }


    [TestClass]
    public class CatcherFactoryTests
    {
        [TestMethod]
        public void CatcheFactoty_0()
        {
            var CONNECTION = new OracleConnection();
            var PATH = "A_PATH";

            var catcher = CatcherFactory.SetupCatcher(PATH, CONNECTION);

            
            Assert.IsInstanceOfType(catcher.Source, typeof(FtpJob));
            Assert.AreEqual(PATH, ((FtpJob)catcher.Source).ftp_str);

            Assert.IsInstanceOfType(catcher.Handled, typeof(FileStorage));            
            Assert.AreEqual(CONNECTION, ((FileStorage)catcher.Handled).connection); 


            Assert.IsInstanceOfType(catcher.Saver, typeof(FinSaver));
            Assert.AreEqual(CONNECTION, ((FinSaver)catcher.Saver).Connection);

            Assert.IsInstanceOfType(catcher.FileStorage, typeof(NFileStorage));


        }
    }
}
