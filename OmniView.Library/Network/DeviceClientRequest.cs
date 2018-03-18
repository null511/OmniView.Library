using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OmniView.Library.Network
{
    public class DeviceClientRequest
    {
        public UrlBuilder Url {get; set;}
        public string Username {get; set;}
        public string Password {get; set;}
        public AuthMethods AuthMethod {get; set;}
        public bool AllowAutoRedirect {get; set;}
        public bool KeepAlive {get; set;}


        public DeviceClientRequest(string url = null)
        {
            Url = new UrlBuilder();

            if (!string.IsNullOrEmpty(url))
                Url.AppendPath(url);

            AllowAutoRedirect = true;
            KeepAlive = true;
        }

        public DeviceClientResponse Send()
        {
            var httpRequest = BuildRequest();

            HttpWebResponse httpResponse = null;
            try {
                httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                return new DeviceClientResponse(httpResponse);
            }
            catch {
                httpResponse?.Dispose();
                throw;
            }
        }

        public async Task<DeviceClientResponse> SendAsync()
        {
            var httpRequest = BuildRequest();

            HttpWebResponse httpResponse = null;
            try {
                httpResponse = (HttpWebResponse)await httpRequest.GetResponseAsync();
                return new DeviceClientResponse(httpResponse);
            }
            catch {
                httpResponse?.Dispose();
                throw;
            }
        }

        public HttpWebRequest BuildRequest()
        {
            var httpRequest = WebRequest.CreateHttp(Url.ToString());

            httpRequest.AllowAutoRedirect = AllowAutoRedirect;
            httpRequest.KeepAlive = KeepAlive;

            //...

            var hasUser = !string.IsNullOrEmpty(Username);
            var hasPass = !string.IsNullOrEmpty(Password);

            if (hasUser || hasPass) {
                switch (AuthMethod) {
                    case AuthMethods.Basic:
                        AddBasicAuthHeader(httpRequest);
                        break;
                }
            }

            return httpRequest;
        }

        private void AddBasicAuthHeader(HttpWebRequest httpRequest)
        {
            var authString = $"{Username}:{Password}";
            var authBytes = Encoding.GetEncoding("ISO-8859-1").GetBytes(authString);
            var authStringEncoded = Convert.ToBase64String(authBytes);
            httpRequest.Headers.Add("Authorization", $"Basic {authStringEncoded}");
        }
    }
}
