using BO;
using Devart.Data.Oracle;
using RawLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{


    class Program
    {

        //static string json = "{\"DateTimeStart\":\"26.12.2018 11:11:06\",\"DateTimeFinish\":\"26.12.2018 11:12:42\",\"HeightSm\":-1,\"DiameterOuter\":1485,\"DiameterInner\":1081.7,\"Height\":375,\"OperatorName\":\"-\",\"UserName\":\"Харьковский Н.А.\",\"EquipmentId\":3460,\"Npart\":\"1060\",\"Nplav\":\"8-41-10504\",\"Pnum\":\"2\",\"Snum\":\"-\",\"Npp\":\"21\\u0027 СО\\u0027\",\"OperationName\":\"1 РАСКАТКА\",\"Tek\":\"ТЭК-22-50-090-1424-2018 рев. 1\",\"Weight\":1310,\"BrigadeName\":\"Смена Б\",\"OperationId\":595463659,\"ZagotId\":594505736,\"PassportId\":2689338,\"BrigadaId\":536,\"PersonUntn\":151788,\"ReportPath\":\"d:\\\\_vsmpo_report_\\\\RRM_2018-12-26_11-12-42.pdf\"}";
        //static string json = "{\"DateTimeStart\":\"05.12.2018 19:26:17\",\"DateTimeFinish\":\"05.12.2018 19:28:33\",\"HeightSm\":-1,\"DiameterOuter\":1332.6,\"DiameterInner\":1266.9,\"Height\":375.4,\"OperatorName\":\"-\",\"UserName\":\"-\",\"EquipmentId\":3460,\"Npart\":\"-\",\"Nplav\":\"-\",\"Pnum\":\"-\",\"Snum\":\"-\",\"Npp\":\"-\",\"OperationName\":\"-\",\"Tek\":\"-\",\"Weight\":-1,\"BrigadeName\":\"-\",\"OperationId\":-1,\"ZagotId\":-1,\"PassportId\":-1,\"BrigadaId\":-1,\"PersonUntn\":-1,\"ReportPath\":\"d:\\\\_vsmpo_report_\\\\RRM_2018-12-05_19-28-33.pdf\"}";

        static string json = "{\"DateTimeStart\":\"20.12.2018 16:42:19\",\"DateTimeFinish\":\"20.12.2018 16:43:56\",\"HeightSm\":-1,\"DiameterOuter\":825,\"DiameterInner\":630.5,\"Height\":142.9,\"OperatorName\":\"-\",\"UserName\":\"Гусак К.М.\",\"EquipmentId\":3460,\"Npart\":\"1170\",\"Nplav\":\"0-21-07467\",\"Pnum\":\"2\",\"Snum\":\"-\",\"Npp\":\"81\",\"OperationName\":\"2 РАСКАТКА\",\"Tek\":\"ТЭК-22-50-090-1547-2018 рев. 0\",\"Weight\":139,\"BrigadeName\":\"Смена А\",\"OperationId\":606574001,\"ZagotId\":607981877,\"PassportId\":2723933,\"BrigadaId\":535,\"PersonUntn\":67813,\"ReportPath\":\"d:\\\\_vsmpo_report_\\\\RRM_2018-12-20_16-43-56.pdf\"}";



        static void Main(string[] args)
        {
            var dto = JsonParser.Parse(json);

            //var connection = new OracleConnection("Direct=True;User Id = bruhno; Password=pi124 ;Host=testfin;SID=B;Port=1521;Pooling=false");
            var connection = new OracleConnection("Direct=True;User Id = terminal_54_1; Password=te ;Host=testfin;SID=B;Port=1521;Pooling=false");
            connection.Open();


            dto.ReportURL = new NFileStorage().Upload(dto.ReportPath, StringToStream(json));
            
            PrintProperties(dto);

            new FinSaver(connection).Save(dto);

            var oz = new OperZagot(connection, (int)dto.OperationId, (int)dto.ZagotId);

            Console.WriteLine("--------------------------");
            Console.WriteLine(oz);

            Console.ReadKey();
        }

        private static void PrintProperties(ProcessDTO obj)
        {
            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(obj))
            {
                string name = descriptor.Name;
                object value = descriptor.GetValue(obj);
                Console.WriteLine("{0}={1}", name, value);
            }
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
