using Devart.Data.Oracle;
using System;
using System.Collections.Generic;
using System.Text;

namespace OraPackages
{
    public class AgrLifePkg
    {
        static string packageName = "agr_life_pkg";
        static OraclePackage GetPackage(OracleConnection connection) => new OraclePackage { Connection = connection, PackageName = packageName };

       
        public static void TransTo(OracleConnection connection, string doctype, int docid, string state, DateTime? dateSnd)
        {
            var args = new OracleParameterCollection
            {
                { "p_doc_type", doctype },
                { "p_doc_id", docid },
                { "p_state", state }
            };

            if (dateSnd != null)
            {
                args.Add("p_rollback_limit", null);
                args.Add("p_date_snd", dateSnd);
            }

            GetPackage(connection).ExecuteProcedure("trans_to", args);
        }
    }
}
