using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RawLib;

namespace UnitTestProject1
{
    [TestClass]
    public class DirectoryMonitorTests
    {
        string _basedir = "c:/temp/ftest/";
        string TestDirectory;
        ISourceDirectory SourceDirectory;
        IHandledStorage DestinationRepository;


        #region INNER LOGIC
        private static Random random = new Random();

        public IHandledStorage DestinationRepository1 { get => DestinationRepository; set => DestinationRepository = value; }
        public IHandledStorage DestinationRepository2 { get => DestinationRepository; set => DestinationRepository = value; }

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        

        private string CreateEmptyFile()
        {
            var fname = Path.Combine(TestDirectory, $"{RandomString(5)}.tmp");
#pragma warning disable CS0642 // Possible mistaken empty statement
            using (File.Create(fname)) ;
#pragma warning restore CS0642 // Possible mistaken empty statement
            return fname;
        }
        #endregion

        [TestInitialize]
        public void Setup()
        {
            TestDirectory = Path.Combine(_basedir,RandomString(5));
            Directory.CreateDirectory(TestDirectory);

            SourceDirectory = new LocalSourceDirectory(TestDirectory);
            DestinationRepository = new MemoryHandledStorage();
        }

        [TestCleanup]
        public void TearDown()
        {
            DirectoryInfo di = new DirectoryInfo(_basedir);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
        }


        [TestMethod]
        public void TestMethod1()
        {
            var f = new MemoryFileStorage();

            var c = new Catcher(SourceDirectory, DestinationRepository, null, null);

            var list = c.GetNewFiles();

            Assert.AreEqual(0, list.Count);

        }



        [TestMethod]
        public void Add_File_Test()
        {            

            var c = new Catcher(SourceDirectory, DestinationRepository, null, null);

            CreateEmptyFile();

            Assert.AreEqual(1, c.GetNewFiles().Count);

            CreateEmptyFile();

            Assert.AreEqual(2, c.GetNewFiles().Count);

        }

        //[TestMethod]

        //public void Add_File_Test_1()
        //{            

        //    var c = new Catcher(SourceDirectory, DestinationRepository, null);


        //    var fname = CreateEmptyFile();
           

        //    Assert.AreEqual(1, c.GetNewFiles().Count);


            
        //    m.MarkCompleted(fname);

        //    Assert.AreEqual(0, c.GetNewFiles().Count);

        //}



    }
}
