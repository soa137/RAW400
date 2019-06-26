using BO;
using Devart.Data.Oracle;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OraPackages;
using OraTables;
using RawLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace UnitTestProject1
{

    

    internal class TSTDAO:IDisposable
    {
        private static Random random = new Random();
        public readonly OracleConnection Connection;
        public readonly OracleConnection GRPOConnection;

        public int? SessionUntn => new SessionContext(Connection).Untn;

        public Person SessionPerson => new Person(Connection, (int)SessionUntn);

        public TSTDAO()
        {
            GRPOConnection = new OracleConnection("Direct=True;User Id = grpo; Password=oprga; Host=testfin;SID=B;Port=1521;Pooling=false");
            GRPOConnection.Open();
          


            Connection = new OracleConnection("Direct=True;User Id = terminal_54_1; Password=te ;Host=testfin;SID=B;Port=1521;Pooling=false");
            Connection.Open();

        }

        public int NewUserId(bool isPerson = true)
        {
            int? untn = isPerson ? QM.QueryInteger(Connection, "select untn from v_kadr_fin k where not exists(select null from user_name u where u.untn = k.untn) and k.r_u = 'Работает' and rownum < 2", null) : null;

            var username = RandomString(10);

            using (var fsis = new OracleConnection("Direct=True;User Id = fsis;Password=fs;Host=testfin;SID=B;Port=1521;Pooling=false"))
            {
                fsis.Open();

                var command = new OracleCommand
                {
                    Connection = fsis,
                    CommandType = System.Data.CommandType.StoredProcedure,
                    CommandText = "create_username"
                };

                command.Parameters.Add("p_username", username);
                command.Parameters.Add("p_untn", untn);

                command.ExecuteNonQuery();

                fsis.Commit();
               
            }

            Commit();

            return NUserPack.GetUserId(Connection, username);
            
        }

        internal void NewUser() => new SessionContext(Connection).SetSessionUser(NewUserId());

        internal Resource NewResource()
        {
            var s = QM.QueryString(Connection, "select g.id_global from v_global_obor g minus select r.global_id from smz_obor_resource r", null);
            var globalId = Convert.ToInt64(s);
            var resourceId = SmzResourcePkg.GetOborResourceId(Connection, globalId, null);

            
            var r = new Resource(GRPOConnection, resourceId) { PdrId = NewPdrId() };

            return new Resource(Connection, r.Id);


            
        }

        private int NewPdrId()
        {
            var id = QM.Nextval(Connection, "seq1");
            QM.DML(GRPOConnection,
                    "insert into lns_pdr_vinov(lns_pdr_vinov_id, nc, naim) values(:0,:1,:2)",
                    new object[] { id, "22", RandomString(20) });
            return id;
        }

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public void Dispose()
        {
            Connection.Close();
        }

        internal Pasport PasportRandom(int zagotCount)
        {
            new IdentityManager(GRPOConnection).AddPermission("grpo", Permission.PRO_PASPORT);
            new IdentityManager(GRPOConnection).AddPermission("grpo", Permission.AGR_TRANS_TO);

            var builder = new PasportBuilder(GRPOConnection)
            {
                Nc = "22",
                NPart = RandomString(5),
                NPlav = RandomString(5),
                BshId = 60033,
                VidProizvId = 1,


                ZagotCount = zagotCount,
                OperCount = 5,
                TekId = NewRandomTek().Id
            };
            

            var psp = builder.GetResult();

            Commit();

            return psp;



        }

        public OperZagot NewOperZagot()
        {
            var psp = PasportRandom(1);

            return new OperZagot(Connection, psp.Oper[0], psp.Zagot[0]);            
        }

        public Tek NewRandomTek()
        {
            var builder = new TekBuilder(GRPOConnection)
            {
                Nc = "22",
                Obozn = RandomString(5),
                NaimId = 135,
                SprOperId = 437,
                SprOborId = 437
            };

            return builder.GetResult();
        }

        internal void Commit()
        {
            Connection.Commit();
            GRPOConnection.Commit();
        }

        internal void Monitor()
        {
            new OracleMonitor().IsActive = true;
        }

        internal int RandomUntn()
        {
            var max = (int)QM.QueryInteger(Connection, "select max(untn) from v_kadr_fin", new object[0]);

            max = random.Next(max);

            return (int)QM.QueryInteger(Connection, "select max(untn) from v_kadr_fin where untn<:0", new object[] { max });


        }
    }



    [TestClass]
    public class TestDAOTests
    {

        private TSTDAO A = new TSTDAO();

        [TestMethod]
        public void Random_Pasport_0()
        {            
            var psp = A.PasportRandom(1);

            Assert.IsTrue(psp.Tek.IsNotNull);            
            Assert.AreEqual(1, psp.Oper.Count);
            Assert.AreEqual(1, psp.Zagot.Count);
        }


        [TestMethod]
        public void Random_Pasport_Rank()
        {
            var psp = A.PasportRandom(1);

            A.Commit();

            Assert.AreEqual(200, psp.Document.Rank);
        }


        [TestMethod]
        public void Random_Pasport_Npart()
        {
            var psp = A.PasportRandom(1);


            Assert.IsFalse(string.IsNullOrEmpty(psp.Nplav));
            Assert.IsFalse(string.IsNullOrEmpty(psp.Npart));
        }


        [TestMethod]
        public void Random_Tek_0()
        {
            var tek = A.NewRandomTek();

            Assert.AreEqual(1, tek.Marsh.Count);
            Assert.AreNotEqual(null, tek.Marsh[0].SprOborId);

        }



        [TestMethod]
        public void Random_Resource_0()
        {
            

            var res = A.NewResource();

            Assert.AreNotEqual(null, res.PdrId);

        }
    }

    internal class TekBuilder
    {
        private OracleConnection connection;

        public TekBuilder(OracleConnection connection)
        {
            this.connection = connection;
        }

        public string Nc { get; internal set; }
        public string Obozn { get; internal set; }
        public int NaimId { get; internal set; }
        public int SprOperId { get; internal set; }
        public int SprOborId { get; internal set; }        

        internal Tek GetResult()
        {
            var tekId = NTekAppPkg.CreateTek(connection, Nc, Obozn);

            NTekAppPkg.SetWorkTek(connection, tekId);

            QM.Update(connection, "N_TEK", tekId, "NAIM_ID", NaimId);

            var marshid = QM.Insert(connection, "insert into tpp.n_marsh(n_tek_id, spr_oper_id, npp) values (:0, :1, :2)  returning n_marsh_id into :id", new object[] { tekId, SprOperId, 1 });

            var marsh = new Marsh(connection, marshid);

            marsh.AddObor(SprOborId,true);


            return new Tek(connection, tekId);
        }
    }

    internal class PasportBuilder
    {
        private OracleConnection connection;

        public PasportBuilder(OracleConnection connection)
        {            
            this.connection = connection;
        }

        public string Nc { get; internal set; }
        public string NPart { get; internal set; }
        public int BshId { get; internal set; }
        public int VidProizvId { get; internal set; }
        public string NPlav { get; internal set; }
        public int ZagotCount { get; internal set; }
        public int OperCount { get; internal set; }
        public int TekId { get; internal set; }
        public int ShifrId { get; internal set; }


        internal Pasport GetResult()
        {
            

            int pspId = QM.Nextval(connection, "n_pasport_seq");

            QM.DML(connection, "insert into pasport(pasport_id,nc,npart,nplav,bsh_id, vid_proizv_id,date_zap) values (:0,:1,:2,:3,:4, :5, sysdate)", new object[] { pspId, Nc, NPart, NPlav, BshId, VidProizvId });
            QM.DML(connection, "insert into n_pasport(n_pasport_id,pasport_id) values (:0,:0)", new object[] { pspId });

            if (ShifrId != 0)
                QM.DML(connection, "insert into n_pasport_shifr(n_pasport_id,uslov_shifr_id) values (:0,:1)", new object[] { pspId, ShifrId });



            var psp = new Pasport(connection, pspId);

            for (var i = 0; i < ZagotCount; i++) psp.AddZagot($"T{i}");

            psp.LinkTek(TekId);
            psp.GenerateOpers();
            
            psp.Document.TransTo("22P_EL_PASP");
            psp.Document.Denorm();


            

            return psp;

        }
    }

    internal class IdentityManager
    {
        private OracleConnection connection;

        public IdentityManager(OracleConnection connection)
        {
            this.connection = connection;
        }

        internal void AddPermission(string username, Permission permission)
        {
            var roleName = GetRoleName(permission);

            var roleId = QM.QueryInteger(connection, "select agr_role_id from agr_role where agr_role=:0", roleName);
            if (roleId == null) throw new Exception($"role not found {roleName}");

            var userId = NUserPack.GetUserId(connection, username.ToUpper());

            if (AgrAppPkg.UserHasRole(connection, userId, (int)roleId)) return;

            AgrCorePkg.GrantRoleToUser(connection, (int)roleId, userId, true);
            
        }

        private string ReplaceFirst(string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }
        private object GetRoleName(Permission permission)
        {
            var role = $"#{permission.ToString()}";            

            return ReplaceFirst(role, "_", ".");

        }
    }

   

    public enum Permission { PRO_PASPORT, AGR_TRANS_TO };
}