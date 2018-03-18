using System;
using System.IO;

namespace OmniView.Library.MJpeg
{
    internal class BufferWriter
    {
        private readonly Action releaseAction;

        public MJpegImage Image {get;}


        internal BufferWriter(MJpegImage image, Action releaseAction)
        {
            this.Image = image;
            this.releaseAction = releaseAction;
        }

        public void Release()
        {
            Image.Buffer.Seek(0, SeekOrigin.Begin);
            releaseAction?.Invoke();
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            Image.Buffer.Write(buffer, offset, count);
        }
    }
}
