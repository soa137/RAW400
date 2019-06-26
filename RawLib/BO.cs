using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Devart.Data.Oracle;
using OraPackages;
using OraTables;

namespace BO
{


    public class Principal 
    {
        private readonly OracleConnection connection;

        public int Id { get;}
        public int? Untn { get; }

        public int? BrigadaId => null; 

        public Principal(OracleConnection connection, int untn)
        {            
            this.connection = connection;
            this.Id=  SmzPrincipalPkg.GetPrincipalIdUntn(connection, untn);
            Untn = untn;
        }

        
    }

    public class Smen : IdentificatedBO
    {
        public Smen(OracleConnection connection, int id) : base(connection, id)
        {
        }


        public static Smen Current(OracleConnection connection)
        {
            var id = SmzPeriodPkg.CurrentSmenId(connection);
            return new Smen(connection, id);
        }

        public DateTime Date => SmzPeriodPkg.GetSmenDate(connection, Id);
        public int Num => SmzPeriodPkg.GetSmenNum(connection, Id);



    }

    public class Resource : IdentificatedBO
    {
        private long? _globalId;
        private bool Initialized;
        private int? _sprOborId;

        public Resource(OracleConnection connection, int id) : base(connection, id)
        {
        }
        

        public int? PdrId
        {
            get
            {
                if (GlobalId != null) return QM.QueryInteger(connection, "select t.lns_pdr_vinov_id from plft4.uch_obor t where t.id_global=:0", GlobalId);
                else return QM.QueryInteger(connection, "select t.lns_pdr_vinov_id from plft4.uch_obor t where t.spr_obor_id=:0", SprOborId);
            }
            set
            {
                if (PdrId == null)
                {
                    if (GlobalId != null) QM.DML(connection, "insert into uch_obor(lns_pdr_vinov_id, id_global) values (:0, :1)", value, GlobalId);
                    else throw new NotImplementedException();
                }
            }
        }

        public Int64? GlobalId
        {
            get
            {
                if (Initialized) return _globalId;

                SmzResourcePkg.Extract(connection, Id, out _globalId, out _sprOborId);

                Initialized = true;

                return _globalId;

            }            
        }
        public int? SprOborId
        {
            get
            {
                if (Initialized) return _sprOborId;

                SmzResourcePkg.Extract(connection, Id, out _globalId, out _sprOborId);

                Initialized = true;

                return _sprOborId;

            }
        }


}

    internal class VVFact : IdentificatedBO
    {
        public VVFact(OracleConnection connection, int id) : base(connection, id)
        {
        }

        public DateTime? DeformStart
        {
            get => QM.QueryDateTime(connection, "nach_def", "vv_fact_zag", Id);
            internal set => VVFactPkg.SetNachDef(connection, Id, value);
        }

        public DateTime? DeformFinish
        {
            get => QM.QueryDateTime(connection, "okon_def", "vv_fact_zag", Id);
            internal set => VVFactPkg.SetOkonDef(connection, Id, value);
        }
       
        public int? Height
        {
            get
            {
                var s = QM.QueryString(connection, "h", "vv_fact_zag", Id);
                if (s == "") { return null; }
                { return int.Parse(s); }
            }

            internal set => QM.Update(connection, "vv_fact_zag", Id, "H", value);
        }

        public double? DiameterInner
        {
            get
            {
                var s = QM.QueryString(connection, "d_vn", "vv_fact_zag", Id);
                if (s == "") { return null; }
                { return int.Parse(s); }
            }
            internal set => QM.Update(connection, "vv_fact_zag", Id, "d_vn", value);
        }

        public double? DiameterOuter
        {
            get
            {
                var s = QM.QueryString(connection, "d", "vv_fact_zag", Id);
                if (s == "") { return null; }
                { return int.Parse(s); }
            }
            internal set => QM.Update(connection, "vv_fact_zag", Id, "d", value);
        }
    }

    public abstract class IdentificatedBO
    {
        protected readonly OracleConnection connection;

        public int Id { get; private set; }



        public IdentificatedBO(OracleConnection connection, int id)
        {
            this.connection = connection;
            Id = id;
        }

        public bool IsNotNull => Id != 0;
        public bool IsNull => !IsNotNull;

        public Document Document
        {
            get
            {
                if (string.IsNullOrEmpty(DocType)) return null;

                return new Document(connection, DocType, Id);
            }
        }

        public virtual string DocType => string.Empty;
        public virtual Table Table { get; }

        public override bool Equals(object obj)
        {
            var bO = obj as IdentificatedBO;
            return bO != null &&
                   EqualityComparer<OracleConnection>.Default.Equals(connection, bO.connection) &&
                   Id == bO.Id;
        }

        public override int GetHashCode()
        {
            var hashCode = -1989265727;
            hashCode = hashCode * -1521134295 + EqualityComparer<OracleConnection>.Default.GetHashCode(connection);
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            return hashCode;
        }

        

        public string QueryString(string column)
        { 
            if (Table == null) throw new InvalidOperationException("property Table is null");
            return Table.QueryString(connection, column, Id);
        }

        public int? QueryInteger(string column)
        {
            if (Table == null) throw new InvalidOperationException("property Table is null");
            return Table.QueryInteger(connection, column, Id);
        }

        public void Update(string column, object value)
        {
            if (Table == null) throw new InvalidOperationException("property Table is null");
            Table.Update(connection, Id, column, value);
        }


    }

    public class Document
    {
        private OracleConnection connection;
        private string docType;
        private int docId;

        public int Rank => AgrAppPkg.GetDocRank(connection, docType, docId);

        public Document(OracleConnection connection, string docType, int docId)
        {
            this.connection = connection;
            this.docType = docType;
            this.docId = docId;
        }

        public void TransTo(string state)
        {
            AgrLifePkg.TransTo(connection, docType, docId, state, null);
        }

        public void Denorm()
        {
            DenormPkg.DenormDoc(connection, docType, docId);
        }
    }

    public class Pasport : IdentificatedBO
    {
        public Pasport(OracleConnection connection, int id) : base(connection, id)
        {
        }

        public List<int> Oper => new List<int>(QM.QueryIntegerArray(connection, "select oper_id from oper where pasport_id=:0 order by npp_sort", new object[] { Id }));
        public List<int> Zagot => new List<int>(QM.QueryIntegerArray(connection, "select zagot_id from zagot where pasport_id =:0", Id));

        public Tek Tek { get {
                var tekid = QM.QueryInteger(connection, "n_tek_id", "n_pasport", Id);
                return new Tek(connection, Id);
            } }

        public override string DocType => "N_PASPORT";

        public string Nplav => Tables.Pasport.QueryString(connection, "nplav", Id);

        public string Npart => Tables.Pasport.QueryString(connection, "npart", Id);

        public void LinkTek(int tekId)
        {
            NPasportPkg.LinkPasportTek(connection, Id, tekId);
        }

        public void GenerateOpers()
        {
            NPasportPkg.GeneratePasport(connection, Id, 1, 0, 2000);
        }

        public void AddZagot(string number)
        {
            var zagotid = QM.Nextval(connection, "seq1");

            QM.DML(connection,
                "insert into zagot(ZAGOT_ID, PASPORT_ID, ZAG, START_VV_ZAG_STATUS_ID, VV_ZAG_STATUS_ID) values(:0, :1, :2, :3, :3)",
                new object[] { zagotid, Id, number, ZagotType.PRODUCT }
                );

            Ed04Pak.CreateDoc(connection, zagotid);
        }
    }

    public enum ZagotType {SPECIMEN = 4, PRODUCT = 7};

    public class Tek : IdentificatedBO
    {
        public Tek(OracleConnection connection, int id) : base(connection, id)
        {
        }

        public List<Marsh> Marsh
        {
            get
            {
                var list = new List<Marsh>();

                foreach (var marshid in QM.QueryIntegerArray(connection, "select n_marsh_id from n_marsh where n_tek_id=:0 order by npp_sort", Id))
                    list.Add(new Marsh(connection, marshid));

                return list;
            }
        }
    }

    public class Marsh : IdentificatedBO
    {
        public Marsh(OracleConnection connection, int id) : base(connection, id)
        {
        }

        public int? SprOborId => QM.QueryInteger(connection, "select mo.spr_obor_id from n_marsh_obor mo where mo.n_marsh_id=:0 and mo.pr_main=1", Id);

        public void AddObor(int sprOborId, bool isMain)
        {
            NTekAppPkg.AddMarshObor(connection, Id, sprOborId, isMain);
        }
    }

    public class Signal : IdentificatedBO
    {
        public Signal(OracleConnection connection, int id) : base(connection, id)
        {
        }

        public DateTime? DateAsutp {
            get
            {
                return Tables.RppSignal.QueryDateTime(connection, "DATE_ASUTP", Id);
            }
            
        }

        public string TextInfo
        {
            get => Tables.RppSignal.QueryString(connection, "text_info", Id);
            set => Tables.RppSignal.Update(connection, Id, "text_info", value);
        }
        public bool IsError
        {
            get => 1 == QueryInteger("pr_error");

            set => Update("pr_error", value ? 1 : (object)null);
        }

        public string ReportFilename
        {
            get => Tables.RppSignalLob.QueryString(connection, "naim", "signal_id", Id);
            set
            {
                if (value == ReportFilename) return;

                var id = Tables.RppSignalLob.QueryPrimaryKeyArray(connection, "signal_id", Id).FirstOrDefault();

                if (id == 0) id = Tables.RppSignalLob.Insert(connection, "signal_id", Id);

                Tables.RppSignalLob.Update(connection, id, "naim", value);
            }
        }
        public string ReportUrl
        {
            get => Tables.RppSignalLob.QueryString(connection, "url", "signal_id", Id);
            set {
                if (value == ReportUrl) return;

                var id = Tables.RppSignalLob.QueryPrimaryKeyArray(connection, "signal_id", Id).FirstOrDefault();

                if (id == 0) id = Tables.RppSignalLob.Insert(connection, "signal_id", Id);

                Tables.RppSignalLob.Update(connection, id, "url", value);
            }
        }

        public override Table Table => Tables.RppSignal;

        public static IEnumerable<Signal> Find(OracleConnection connection, int operId, int zagotId, int resourceId)
        {
            var sql =
                 @"select *
              from rpp_signal s
                   inner join rpp_event e on s.signal_id = e.signal_id
                   inner join znr_zakaz_group g on e.doc_type = g.doc_type and e.doc_id = g.doc_id
                   inner join znr_zagot z on z.znr_group_id = g.znr_group_id
                   inner join znr_oper o on o.znr_group_id = g.znr_group_id
             where s.smz_resource_id = :0
               and z.zagot_id = :1
               and o.oper_id = :2";

            var arr = QM.QueryIntegerArray(connection, sql, new object[] { resourceId, zagotId, operId  });

            //var list = new List<Signal>();

            foreach (var i in arr)
                yield return new Signal(connection, i);


            
        }

        public static IEnumerable<Signal> Find(Triad triad) => Find(triad.Connection, triad.OperId, triad.ZagotId, triad.ResourceId);

        public static Signal Write(OracleConnection connection, int operId, int zagotId, int resourceId)
        {            
            var signal = WriteEmpty(connection, resourceId);

            signal.TextInfo = GetInfo(connection, zagotId, operId); 

            var zag = OperZagotCollection.Make(connection, operId, zagotId);
            Tables.RppEvent.Insert(connection,"signal_id", signal.Id, "doc_type", zag.DocType, "doc_id", zag.Id);            

            return signal;
        }


        public static Signal WriteError(OracleConnection connection, string info, int resourceId, string reportPath, string reportUrl)
        {
            var signal = WriteEmpty(connection, resourceId);

            
            signal.IsError = true;
            signal.ReportFilename = Path.GetFileName(reportPath);
            signal.ReportUrl = reportUrl;
            signal.TextInfo = info;

            return signal;
            
            
        }

        private static string GetInfo(OracleConnection connection, int zagotId, int operId)
        {
            var zagot = new Zagot(connection, zagotId);
            var oper = new Oper(connection, operId);

            return $"{zagot.Pasport.Npart} / {zagot.Pasport.Nplav} / {oper.Npp} {oper.Text}";
        }

        public static Signal Write(Triad triad) => Write(triad.Connection, triad.OperId, triad.ZagotId, triad.ResourceId);

        public static Signal WriteEmpty(OracleConnection connection, int resourceId)
        {
            var signalId = Tables.RppSignal.Insert(connection, "smz_resource_id", resourceId, "date_asutp", DateTime.Now);


            return new Signal(connection, signalId);
        }

        
    }

    public class Zagot : IdentificatedBO
    {
        public Zagot(OracleConnection connection, int id) : base(connection, id)
        {
        }

        public Pasport Pasport => new Pasport(connection, PasportId);

        private int PasportId => (int)Tables.Zagot.QueryInteger(connection, "pasport_id", Id);
    }

    public class OperZagotCollection : IdentificatedBO
    {
        public OperZagotCollection(OracleConnection connection, int id) : base(connection, id)
        {
        }

        public override string DocType => "ZNR_COLLECTION";

        public IEnumerable<OperZagot> OperZagot {
            get
            {
                var sql = @"select o.oper_id, z.zagot_id
                              from znr_zakaz_group g 
                                   inner join znr_zagot z on z.znr_group_id = g.znr_group_id
                                   inner join znr_oper o on o.znr_group_id = g.znr_group_id
                             where g.doc_type = :0
                               and g.doc_id = :1                   
                            ";
                var arr = QM.QueryArray<int, int>(connection, sql,new object[] { DocType, Id });
                foreach (var t in arr)
                    yield return new OperZagot(connection, t.Item1, t.Item2);
            }
        }

        public static OperZagotCollection Make(OracleConnection connection, int operId, int zagotId)
        {
            Tables.InArrZagot.DeleteAll(connection);
            Tables.InArrZagot.Insert(connection, "oper_id", operId, "zagot_id", zagotId);

            var id = CollectionPkg.CreateCollection(connection, "ZNR_COLLECTION");

            return new OperZagotCollection(connection, id);

        }

        public static OperZagotCollection Make(OperZagot oz) => Make(oz.connection, oz.OperId, oz.ZagotId);
    }


    public class Triad
    {
        internal readonly OracleConnection Connection;
        internal readonly int OperId;
        internal readonly int ZagotId;
        internal readonly int ResourceId;

        public Triad(OracleConnection connection, int operId, int zagotId, int resourceId)
        {
            Connection = connection;
            OperId = operId;
            ZagotId = zagotId;
            ResourceId = resourceId;
        }
    }
}