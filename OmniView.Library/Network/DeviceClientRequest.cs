using System.Net;
using System.Threading.Tasks;

namespace OmniView.Library.Network
{
    class DeviceClientRequest
    {
        public HttpWebRequest HttpRequest {get;}

        public string Url {get; set;}


        public DeviceClientRequest(string url)
        {
            HttpRequest = WebRequest.CreateHttp(url);
        }

        public DeviceClientResponse Send()
        {
            HttpWebResponse httpResponse = null;
            try {
                httpResponse = (HttpWebResponse)HttpRequest.GetResponse();
                return new DeviceClientResponse(httpResponse);
            }
            catch {
                httpResponse?.Dispose();
                throw;
            }
        }

        public async Task<DeviceClientResponse> SendAsync()
        {
            HttpWebResponse httpResponse = null;
            try {
                httpResponse = (HttpWebResponse)await HttpRequest.GetResponseAsync();
                return new DeviceClientResponse(httpResponse);
            }
            catch {
                httpResponse?.Dispose();
                throw;
            }
        }
    }
}
