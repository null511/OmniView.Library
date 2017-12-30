using System.IO;
using System.Threading.Tasks;

namespace OmniView.Library.Network
{
    public class DeviceClient
    {
        public DeviceClientResponse Get(string url)
        {
            return new DeviceClientRequest(url).Send();
        }

        public async Task<DeviceClientResponse> GetAsync(string url)
        {
            return await new DeviceClientRequest(url).SendAsync();
        }

        public async Task<Stream> GetBufferedStreamAsync(string url)
        {
            return await Get(url).As.BufferedStreamAsync();
        }
    }
}
