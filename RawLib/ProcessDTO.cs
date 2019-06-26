using System;

namespace RawLib
{
    public class ProcessDTO
    {

        public int? BrigadaId { get; set; }


        public int? PassportId { get; set; }


        public int? ZagotId { get; set; }

        public int? OperationId { get; set; }

        public int? Height { get; set; }

        public double? DiameterInner { get; set; }

        public double? DiameterOuter { get; set; }

        [Obsolete]
        public double? HeightSm { get; set; }

        public double? EquipmentId { get; set; }

        public double? Weight { get; set; }


        //string json;

        public string Json { get; set; }

        public string ReportPath { get; set; }



        public string BrigadeName { get; set; }

        public string Tek { get; set; }

        public string OperationName { get; set; }

        public string Npp { get; set; }

        public string Snum { get; set; }

        public string Pnum { get; set; }

        public string Nplav { get; set; }

        public string Npart { get; set; }

        public string UserName { get; set; }

        public string OperatorName { get; set; }

        public DateTime? DateTimeStart { get; set; }

        public DateTime? DateTimeFinish { get; set; }

        public int? PersonUntn { get; set; }
        public string ReportURL { get; set; }
    }
}