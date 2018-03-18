using System;
using System.IO;

namespace OmniView.Library.Logging
{
    public class FileLogger : Logger
    {
        public string Filename {get; set;}


        public FileLogger(string name) : base(name) {}

        protected override Stream GetStream()
        {
            if (string.IsNullOrEmpty(Filename)) throw new ApplicationException("Filename is undefined!");

            var path = Path.GetDirectoryName(Filename);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return File.Open(Filename, FileMode.Append, FileAccess.Write, FileShare.Read);
        }
    }
}
