using OmniView.Library.Common;
using System.IO;
using System.Threading.Tasks;

namespace OmniView.Library.Network
{
    public enum AuthMethods
    {
        Basic,
    }

    public class DeviceClient
    {
        public string BaseUrl {get; set;}
        public string Username {get; set;}
        public string Password {get; set;}
        public AuthMethods AuthMethod {get; set;}


        public DeviceClientRequest GetRequest(string path = null)
        {
            var url = NetPath.Combine(BaseUrl, path);

            var request = new DeviceClientRequest(url) {
                Username = Username,
                Password = Password,
                AuthMethod = AuthMethod,
            };

            return request;
        }

        public DeviceClientResponse Get(string url)
        {
            return GetRequest(url).Send();
        }

        public async Task<DeviceClientResponse> GetAsync(string url)
        {
            return await GetRequest(url).SendAsync();
        }

        public async Task<Stream> GetBufferedStreamAsync(string url)
        {
            return await Get(url).As.BufferedStreamAsync();
        }
    }
}
