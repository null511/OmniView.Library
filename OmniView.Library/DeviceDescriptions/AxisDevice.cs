using OmniView.Library.Devices;
using OmniView.Library.Network;
using System.Threading.Tasks;

namespace OmniView.Library.DeviceDescriptions
{
    [Device("axis", "Axis")]
    public class AxisDevice : IDevice
    {
        private readonly DeviceClient client;

        public IDeviceDescription Description {get;}
        public DeviceCapabilities Capabilities {get;}


        public AxisDevice(IDeviceDescription description)
        {
            this.Description = description;

            client = new DeviceClient();

            Capabilities = new DeviceCapabilities {
                SupportsPicture = true,
                SupportsVideo = true,
                SupportsPanTilt = false,
                SupportsZoom = false,
            };
        }

        public UrlBuilder GetPictureUrl(string format)
        {
            return GetUrlBuilder("axis-cgi/jpg/image.cgi");
        }

        public UrlBuilder GetVideoUrl(string format)
        {
            return GetUrlBuilder("axis-cgi/mjpg/video.cgi");
        }

        private int currentPanSpeed;
        private int currentTiltSpeed;

        public async Task SetPtzDirectionAsync(PtzDirection direction)
        {
            var panSpeed = 0;
            var tiltSpeed = 0;

            if ((direction & PtzDirection.Left) == PtzDirection.Left)
                panSpeed--;

            if ((direction & PtzDirection.Right) == PtzDirection.Right)
                panSpeed++;

            if ((direction & PtzDirection.Up) == PtzDirection.Up)
                tiltSpeed++;

            if ((direction & PtzDirection.Down) == PtzDirection.Down)
                tiltSpeed--;

            if (panSpeed != currentPanSpeed || tiltSpeed != currentTiltSpeed) {
                await Ptz(panSpeed, tiltSpeed);
                currentPanSpeed = panSpeed;
                currentTiltSpeed = tiltSpeed;
            }
        }

        private async Task Ptz(int panSpeed, int tiltSpeed)
        {
            var url = GetUrlBuilder("axis-cgi/com/ptz.cgi");
            url.Query["continuouspantiltmove"] = $"{panSpeed},{tiltSpeed}";

            await client.GetAsync(url.ToString());
        }

        private UrlBuilder GetUrlBuilder(string path)
        {
            var url = new UrlBuilder(Description.Url, path);

            //if (!string.IsNullOrEmpty(Description.Username))
            //    url.Query["user"] = Description.Username;

            //if (!string.IsNullOrEmpty(Description.Password))
            //    url.Query["pwd"] = Description.Password;

            return url;
        }
    }
}
