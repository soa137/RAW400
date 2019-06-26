using Devart.Data.Oracle;
using OraPackages;

namespace BO
{
    public class SessionContext
    {
        private OracleConnection connection;

        public SessionContext(OracleConnection connection)
        {
            this.connection = connection;
        }

        public void SetSessionUser(int userid)
        {
            var username = NUserPack.GetUsernameById(connection, userid);
            AgrAppPkg.SetSessionUser(connection, username);
        }

        


        public int? Untn => NUserPack.GetUserUntn(connection, Username);

        public string Username => AgrAppPkg.GetSessionUser(connection);
    }
}