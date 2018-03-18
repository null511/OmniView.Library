using OmniView.Library.Network;
using System.Threading.Tasks;

namespace OmniView.Library.Devices
{
    public interface IDevice
    {
        DeviceClient Client {get;}
        IDeviceDescription Description {get;}
        DeviceCapabilities Capabilities {get;}
        DeviceResolution? Resolution {get;}


        UrlBuilder GetPictureUrl();

        UrlBuilder GetVideoUrl();

        Task SetPtzDirectionAsync(PtzDirection direction);

        Task SetResolution(DeviceResolution resolution);
    }
}
