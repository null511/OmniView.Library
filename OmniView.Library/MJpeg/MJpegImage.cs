using System;
using System.IO;

namespace OmniView.Library.MJpeg
{
    public class MJpegImage : IDisposable
    {
        public Stream Buffer {get; set;}
        public DateTime Time {get; set;}
        public long Size {get; set;}


        public MJpegImage()
        {
            Buffer = new MemoryStream();
        }

        public void Dispose()
        {
            Buffer?.Dispose();
        }

        public void Clear()
        {
            Buffer.SetLength(0);
        }
    }
}
