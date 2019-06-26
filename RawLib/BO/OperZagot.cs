using System;
using System.Collections.Generic;
using System.Linq;
using Devart.Data.Oracle;
using OraPackages;
using OraTables;

namespace BO
{
    public class OperZagot
    {
        internal readonly OracleConnection connection;
        public int OperId;
        public int ZagotId;
        private OperRashod _operRashod;

        public OperZagot(OracleConnection connection, int operId, int zagotId)
        {
            OperId = operId;
            ZagotId = zagotId;
            this.connection = connection;
        }

        public DateTime? DeformStart
        {
            get => GetVVFact().DeformStart;
            set
            {
                var f = GetVVFact();
                f.DeformStart = value;
            }
        }

        public DateTime? DeformFinish
        {
            get => GetVVFact().DeformFinish;
            set
            {
                var f = GetVVFact();
                f.DeformFinish = value;
            }
        }

        public int? Height
        {
            get => GetVVFact().Height;
            set
            {
                var f = GetVVFact();
                f.Height = value;
            }
        }

        public double? DiameterInner
        {
            get => GetVVFact().DiameterInner;
            set
            {
                var f = GetVVFact();
                f.DiameterInner = value;
            }
        }

        public double? DiameterOuter
        {
            get => GetVVFact().DiameterOuter;
            set
            {
                var f = GetVVFact();
                f.DiameterOuter = value;
            } 
        }

        public void AddReport(string url, string filename)
        {
            if (string.IsNullOrEmpty(url)) throw new ArgumentNullException("url is null");
            if (string.IsNullOrEmpty(filename)) throw new ArgumentNullException("filename is null");

            var zvvid = GetZagotVVFactZagId();

            if (zvvid == null) throw new InvalidOperationException($"zagot_vv_fact_zag is null: zagot[{ZagotId}] oper[{OperId}] ");

            var id = QM.Nextval(connection, "seq1");

            Tables.AsutpProtPress.Insert(connection, "file_url", url, "source_file_name", filename, "zagot_vv_fact_zag_id", zvvid, "asutp_prot_press_id", id);
        }

        public IEnumerable<Work> Work { get {

                if (OperRashod == null) yield break;


                foreach (var w in OperRashod.Work)
                    yield return w;
                               

            } }

        public void CompleteWork(Resource resource)
        {
            var pdrid = resource.PdrId;
            if (pdrid == null) throw new Exception($"Pdr is null for resource [{resource}]");
            var r = GetOrCreateOperRashod((int)pdrid);
            r.CompleteWork(resource);
        }

        [Obsolete("Use CompleteWork(Resource, Person)")]
        public void CompleteWork(int untn, Resource resource)
        {

            var pdrid = resource.PdrId;
            if (pdrid == null) throw new Exception($"Pdr is null for resource [{resource}]");
            var r = GetOrCreateOperRashod((int)pdrid);
            r.CompleteWork(untn, resource);
        }

        

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var oz = (OperZagot)obj;
            return OperId == oz.OperId && ZagotId == oz.ZagotId;
        }

        public override int GetHashCode()
        {
            var hashCode = -1208511241;
            hashCode = hashCode * -1521134295 + OperId.GetHashCode();
            hashCode = hashCode * -1521134295 + ZagotId.GetHashCode();
            return hashCode;
        }

        public override string ToString() => $"OperZagot[{OperId}:{ZagotId}]";


        private OperRashod OperRashod
        {
            get
            {

                if (_operRashod != null) return _operRashod;

                var id = OperZagotPkg.GetOperRashodId(connection, OperId, ZagotId);

                if (id != null) _operRashod = new OperRashod(connection, (int)id);

                return _operRashod;
            }
        }

        //private int? ZagotVVFactZagId
        //{
        //    get
        //    {

        //    }
        //}

        public IEnumerable<Report> Reports
        {
            get
            {
                var zvvid = GetZagotVVFactZagId();

                if (zvvid == null) yield break;

                foreach (var id in Tables.AsutpProtPress.QueryPrimaryKeyArray(connection, "zagot_vv_fact_zag_id", zvvid))
                    yield return new Report(connection, id);
            }
        }

        private int? GetZagotVVFactZagId()
        {
            var id = Tables.ZagotVVFactZag
                                .QueryPrimaryKeyArray(connection, "oper_id", OperId, "zagot_id", ZagotId)
                                .FirstOrDefault();

            if (id == 0) return null;

            return id;
        }

        private OperRashod GetOrCreateOperRashod(int pdrId)
        {
            

            if (OperRashod != null) return OperRashod;

            //var ctx = new SessionContext(connection);
            //if (ctx.Untn == null) throw new InvalidOperationException($"Session Untn is null [{ctx.Username}]");

            OperZagotPkg.CreateOperRashod(connection, OperId, ZagotId, pdrId);
            

            return OperRashod;

        }

        private VVFact GetVVFact()
        {
            var id = OperZagotPkg.GetVVFactId(connection, OperId, ZagotId);
            if (id == null)  throw new Exception("vvfact is null");
            return new VVFact(connection, (int)id);
        }

        public void CompleteWork(Resource resource, Person person)
        {
            var pdrid = resource.PdrId;
            if (pdrid == null) throw new Exception($"Pdr is null for resource [{resource}]");
            var r = GetOrCreateOperRashod((int)pdrid);
            r.CompleteWork(person.Untn, resource);
        }
    }
}