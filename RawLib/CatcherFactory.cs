using System;
using System.Collections.Generic;
using System.Text;
using Devart.Data.Oracle;

namespace RawLib
{
    public class CatcherFactory
    {
        public static Catcher SetupCatcher(string ftpFolder, OracleConnection connection)
        {
            ISourceDirectory source = new FtpJob(ftpFolder);
            IHandledStorage storage = new FileStorage(ftpFolder, connection);
            IFileStorage fs = new NFileStorage();
            IPasportSaver pasport = new FinSaver(connection);
            return new Catcher(source, storage, pasport, fs);
        }
    }
}
