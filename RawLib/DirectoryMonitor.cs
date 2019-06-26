using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RawLib
{



    [Obsolete]
    internal class DirectoryMonitor
    {
        private ISourceDirectory source;
        private IHandledStorage handled;

        public DirectoryMonitor(ISourceDirectory source, IHandledStorage handled)
        {
            this.source = source;
            this.handled = handled;
        }

        
        public IList<string> GetNewFiles()
        {
            var srclist = source.GetFiles();
            var dstlist = handled.GetFiles();


            return new List<string>(srclist.Except(dstlist));

            
        }

        internal string GetFileText(string filename) => source.GetFileText(filename);
        

        
        public void MarkCompleted(string filename)
        {
            handled.Add(filename, source.GetFileText(filename));
        }
    }


}
