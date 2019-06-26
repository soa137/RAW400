using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using RawLib;

namespace UnitTestProject1
{
    internal class LocalSourceDirectory : ISourceDirectory
    {
        private string directory;

        public LocalSourceDirectory(string directory)
        {
            this.directory = directory;
        }

        public IList<string> GetFiles()
        {
            var files = Directory.GetFiles(directory);

            return new List<string>(files);
        }

        public string GetFileText(string filename)
        {
            return string.Empty;
        }

        public Stream GetReport(string reportPath)
        {
            throw new NotImplementedException();
        }
    }


    internal class MemoryHandledStorage : IHandledStorage
    {
        public Dictionary<string, string> Table = new Dictionary<string, string>();

        public Dictionary<string, string> ErrorTable = new Dictionary<string, string>();

        public IList<string> GetFiles()
        {            
            return new List<string>(Table.Keys);
        }



        public void Add(string filename, string text)
        {
            Table[filename] = text;
        }


        public void Error(string filename, string message)
        {
            ErrorTable[filename] = message;
        }

        public IList<string> GetErrorFiles()
        {
            return new List<string>(ErrorTable.Keys);
        }
    }


    class StringSource : ISourceDirectory
    {
        private List<string> fnames = new List<string>();
        private List<string> contents = new List<string>();

        private Dictionary<string, string> reports = new Dictionary<string, string>();

        public StringSource(string filenames, string contents)
        {

            var arrf = new string[0];
            var arrc = new string[0];

            if (!string.IsNullOrEmpty(filenames)) arrf = filenames.Replace(" ",string.Empty).Split(',');
            if (!string.IsNullOrEmpty(contents)) arrc = contents.Replace(" ", string.Empty).Split(',');


            Debug.Assert(arrf.Length == arrc.Length);

            this.fnames.AddRange(arrf);
            this.contents.AddRange(arrc);
        }

        public void Append(string filename, string content)
        {
            fnames.Add(filename);
            contents.Add(content);
        }

        public IList<string> GetFiles()
        {
            return new List<string>(fnames);
        }

        public string GetFileText(string filename)
        {
            var idx = fnames.IndexOf(filename);
            return contents[idx];

            //var idx = Array.IndexOf(fnames, filename);

            //return contents[idx];
        }

        private static Stream StreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public Stream GetReport(string reportPath)
        {
            return StreamFromString( reports[reportPath]);
        }

        internal void AddFile(string path, string file)
        {
            reports[path] = file;
        }
    }
}