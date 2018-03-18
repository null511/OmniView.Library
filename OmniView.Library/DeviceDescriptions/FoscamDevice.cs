using OmniView.Library.Devices;
using OmniView.Library.Network;
using System;
using System.Net;
using System.Threading.Tasks;

namespace OmniView.Library.DeviceDescriptions
{
    [Device("foscam", "Foscam")]
    public class FoscamDevice : IDevice
    {
        private bool isLookUp;
        private bool isLookDown;
        private bool isLookLeft;
        private bool isLookRight;

        public DeviceClient Client {get;}
        public IDeviceDescription Description {get;}
        public DeviceCapabilities Capabilities {get;}
        public DeviceResolution? Resolution {get; private set;}


        public FoscamDevice(IDeviceDescription description)
        {
            this.Description = description;

            Client = new DeviceClient();

            Capabilities = new DeviceCapabilities {
                SupportsPicture = true,
                SupportsVideo = true,
                SupportsPanTilt = true,
                SupportsZoom = false,
            };
        }

        public UrlBuilder GetPictureUrl()
        {
            return GetUrlBuilder("snapshot.cgi");
        }

        public UrlBuilder GetVideoUrl()
        {
            return GetUrlBuilder("videostream.cgi");
        }

        public async Task SetResolution(DeviceResolution resolution)
        {
            await Task.Run(() => throw new NotImplementedException());
        }

        public async Task SetPtzDirectionAsync(PtzDirection direction)
        {
            var _lookUp = (direction & PtzDirection.Up) == PtzDirection.Up;
            var _lookDown = (direction & PtzDirection.Down) == PtzDirection.Down;
            var _lookLeft = (direction & PtzDirection.Left) == PtzDirection.Left;
            var _lookRight = (direction & PtzDirection.Right) == PtzDirection.Right;

            if (_lookUp != isLookUp) {
                await DecoderControlAsync(_lookUp ? 0 : 1);
                isLookUp = _lookUp;
            }

            if (_lookDown != isLookDown) {
                await DecoderControlAsync(_lookDown ? 2 : 3);
                isLookDown = _lookDown;
            }

            if (_lookLeft != isLookLeft) {
                await DecoderControlAsync(_lookLeft ? 4 : 5);
                isLookLeft = _lookLeft;
            }

            if (_lookRight != isLookRight) {
                await DecoderControlAsync(_lookRight ? 6 : 7);
                isLookRight = _lookRight;
            }
        }

        private async Task DecoderControlAsync(int value)
        {
            var url = GetUrlBuilder("decoder_control.cgi");
            url.Query["command"] = value;

            using (var response = await Client.GetAsync(url.ToString())) {
                if (response.HttpResponse.StatusCode != HttpStatusCode.OK)
                    throw new ApplicationException("DecoderControl Error!");
            }
        }

        private UrlBuilder GetUrlBuilder(string path)
        {
            var url = new UrlBuilder(Description.Url, path);

            if (!string.IsNullOrEmpty(Description.Username))
                url.Query["user"] = Description.Username;

            if (!string.IsNullOrEmpty(Description.Password))
                url.Query["pwd"] = Description.Password;

            return url;
        }
    }
}
