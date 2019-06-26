using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using RawLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace UnitTestProject1
{

    [TestClass]
    public class ParserTests
    {
        [TestMethod]
        public void Parse_Exceptions()
        {            
            Assert.ThrowsException<JsonSerializationException>(() => JsonParser.Parse("12345"));
            Assert.ThrowsException<ArgumentNullException>(() => JsonParser.Parse(""));            
        }

        [TestMethod]
        public void Parse_NoExceptions()
        {
            JsonParser.Parse("{}");
        }

        

        [TestMethod]
        public void Parse_1()
        {            
            
            var dto = JsonParser.Parse("{\"DateTimeStart\":\"28.11.2018 15:30:23\"}");

            
            Assert.AreEqual(new DateTime(2018, 11, 28, 15, 30, 23), dto.DateTimeStart);
        }


        [TestMethod]
        public void Parse_Report_Path()
        {
            

            var dto = JsonParser.Parse("{\"ReportPath\":\"APATH\"}");


            Assert.AreEqual("APATH", dto.ReportPath);
        }

        [TestMethod]
        public void Parse_BF1()
        {

            string json = "{\"ReportPath\":\"d:\\\\_vsmpo_report_\\\\BF1_2018-12-18_11-56-39.pdf\"}";
            
            JsonParser.Parse(json);

            // Ensure no exceptions
        }


        [TestMethod]
        public void Parse_Untn()
        {


            var dto = JsonParser.Parse("{\"PersonUntn\":1}");


            Assert.AreEqual(1, dto.PersonUntn);
        }



    }
}
