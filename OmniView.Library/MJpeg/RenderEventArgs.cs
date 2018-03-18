using System;

namespace OmniView.Library.MJpeg
{
    public class RenderEventArgs : EventArgs
    {
        public MJpegImage Image {get;}

        public RenderEventArgs(MJpegImage image)
        {
            this.Image = image;
        }
    }
}
