using System;
using System.Collections.Generic;
using System.IO;

namespace OmniView.Library.Devices
{
    public class VideoFrame : IDisposable
    {
        public DateTime Time {get; set;}
        public Dictionary<string, string> Metadata {get;}
        public Stream ImageData {get; set;}

        public long Size => ImageData?.Length ?? 0;


        public VideoFrame()
        {
            ImageData = new MemoryStream();
            Metadata = new Dictionary<string, string>();
        }

        public void Dispose()
        {
            ImageData?.Dispose();
        }

        //public bool Equals(VideoFrame frame)
        //{
        //    if (frame == null) return false;
        //    return DateTime.Equals(Time, frame.Time);
        //}

        public void Clear()
        {
            Metadata.Clear();
            ImageData.SetLength(0);
        }
    }
}
