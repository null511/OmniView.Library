using OmniView.Library.Network;
using System;
using System.Threading.Tasks;

namespace OmniView.Library.Devices
{
    public abstract class DeviceDriverBase
    {
        protected readonly DeviceClient Client;

        public IDevice Device {get;}


        public DeviceDriverBase(IDevice device)
        {
            this.Device = device ?? throw new ArgumentNullException(nameof(device));

            Client = new DeviceClient();
        }

        public VideoFrame TakePicture()
        {
            if (!Device.Capabilities.SupportsPicture)
                throw new ApplicationException("Device does not support pictures!");

            // Use request timestamp
            var time = DateTime.Now;
            var url = Device.GetPictureUrl();
            var result = Client.Get(url.ToString());

            return new VideoFrame {
                Time = time,
                ImageData = result.As.BufferedStream(),
            };
        }

        public async Task<VideoFrame> TakePictureAsync()
        {
            if (!Device.Capabilities.SupportsPicture)
                throw new ApplicationException("Device does not support pictures!");

            // Use request timestamp
            var time = DateTime.Now;

            var request = Client.GetRequest();
            request.AllowAutoRedirect = false;
            request.Url = Device.GetPictureUrl();
            request.KeepAlive = false;

            using (var result = await request.SendAsync())
            return new VideoFrame {
                Time = time,
                ImageData = await result.As.BufferedStreamAsync()
            };
        }
    }
}
