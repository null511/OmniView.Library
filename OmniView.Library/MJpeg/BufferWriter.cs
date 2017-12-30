using System;
using System.IO;

namespace OmniView.Library.MJpeg
{
    internal class BufferWriter
    {
        private readonly Action releaseAction;

        public Stream Stream {get;}


        internal BufferWriter(Stream stream, Action releaseAction)
        {
            this.Stream = stream;
            this.releaseAction = releaseAction;
        }

        public void Release()
        {
            Stream.Seek(0, SeekOrigin.Begin);
            releaseAction?.Invoke();
        }
    }
}
