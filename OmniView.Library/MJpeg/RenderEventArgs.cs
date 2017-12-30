using System;
using System.IO;

namespace OmniView.Library.MJpeg
{
    public class RenderEventArgs : EventArgs
    {
        public Stream ImageData {get;}

        public RenderEventArgs(Stream imageData)
        {
            this.ImageData = imageData;
        }
    }
}
