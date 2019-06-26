using Microsoft.VisualStudio.TestTools.UnitTesting;
using RawLib;
using System.IO;

namespace UnitTestProject1
{
    [TestClass]
    public class NFileStorageTests
    {



        [TestMethod]
        public void Storage_0()
        {
            var DATA = "A_DATA";
            var PATH = "A_PATH";


            var stream = StringToStream(DATA);

            var fs = new NFileStorage();

            var url = fs.Upload(PATH, stream);

            
            Assert.IsFalse(string.IsNullOrEmpty(url));
        }



        private static Stream StringToStream(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
