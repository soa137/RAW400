using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RawLib
{

    public interface ISourceDirectory
    {
        IList<string> GetFiles();
        string GetFileText(string filename);
        Stream GetReport(string reportPath);
    }

    public interface IHandledStorage
    {
        void Add(string filename, string text);
        IList<string> GetFiles();
        IList<string> GetErrorFiles();
        void Error(string filename, string message);
    }

    public interface IPasportSaver
    {
        void Save(ProcessDTO dto);
    }


    public interface IFileStorage
    {
        string Upload(string path, Stream stream);
    }

    public class Catcher
    {

        public readonly ISourceDirectory Source;
        public readonly IHandledStorage Handled;
        public readonly IFileStorage FileStorage;
        public readonly IPasportSaver Saver;

        public Catcher(ISourceDirectory source, IHandledStorage handled, IPasportSaver saver, IFileStorage fs)
        {

            this.Source = source;
            this.Saver = saver;
            this.Handled = handled;
            this.FileStorage = fs;
        }


        public IList<string> GetNewFiles()
        {
            var srclist = Source.GetFiles();
            var dstlist = Handled.GetFiles();
            var errList = Handled.GetErrorFiles();


            return new List<string>(srclist.Except(dstlist).Except(errList));

        }

        public void Go()
        {

            foreach (var fname in GetNewFiles())
                try
                {
                    var json = Source.GetFileText(fname);

                    if (string.IsNullOrEmpty(json)) throw new Exception($"{fname} is empty");

                    var dto = JsonParser.Parse(json);


                    if (!string.IsNullOrEmpty(dto.ReportPath) && dto.ReportPath.ToLower().EndsWith(".pdf"))
                        dto.ReportURL = FileStorage.Upload(dto.ReportPath, Source.GetReport(dto.ReportPath));


                    Saver.Save(dto);

                    Handled.Add(fname, json);


                }
                catch (Exception e)
                {
                    Handled.Error(fname, e.Message);
                }
            
        }

        public void Establish()
        {
            foreach (var fname in GetNewFiles())
                try
                {
                    var json = Source.GetFileText(fname);

                    //if (string.IsNullOrEmpty(json)) throw new Exception($"{fname} is empty");

                    //var dto = JsonParser.Parse(json);


                    //if (!string.IsNullOrEmpty(dto.ReportPath) && dto.ReportPath.ToLower().EndsWith(".pdf"))
                    //    dto.ReportURL = FileStorage.Upload(dto.ReportPath, Source.GetReport(dto.ReportPath));


                    //Saver.Save(dto);

                    Handled.Add(fname, json);


                }
                catch (Exception e)
                {
                    Handled.Error(fname, e.Message);
                }
        }
    }
}
