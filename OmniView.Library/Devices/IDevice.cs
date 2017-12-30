using OmniView.Library.Network;
using System.Threading.Tasks;

namespace OmniView.Library.Devices
{
    public interface IDevice
    {
        IDeviceDescription Description {get;}
        DeviceCapabilities Capabilities {get;}


        // TODO: Change format to a picture format enumeration
        UrlBuilder GetPictureUrl(string format);

        // TODO: Change format to a video format enumeration
        UrlBuilder GetVideoUrl(string format);

        Task SetPtzDirectionAsync(PtzDirection direction);
    }
}
