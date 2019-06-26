using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestProject1
{
    [TestClass]
    public class StringSourceTest
    {

        private string StreamToString(Stream stream)
        {
            if (stream == null) return null;

            string myString;  // outside using

            stream.Position = 0;
            using (var reader = new StreamReader(stream))
            {
                myString = reader.ReadToEnd();
            }

            return myString;
        }

        [TestMethod]
        public void AddFile_0()
        {
            var src = new StringSource(null, null);

            src.AddFile("APATH", "12345");

            var stream = src.GetReport("APATH");

            Assert.AreEqual("12345", StreamToString(stream));

        }


        [TestMethod]
        public void String_0()
        {
            var src = new StringSource("f1,f2", "c1,c2");

            Assert.AreEqual(2, src.GetFiles().Count);
        }

        [TestMethod]
        public void Append()
        {
            var src = new StringSource("f1,f2", "c1,c2");

            src.Append("f3", "c3");

            Assert.AreEqual(3, src.GetFiles().Count);
        }
    }
}
