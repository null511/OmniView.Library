using System;
using System.Net;

namespace OmniView.Library.Network
{
    public class DeviceClientResponse : IDisposable
    {
        public HttpWebResponse HttpResponse {get;}
        public DeviceClientResponseParser As {get;}


        internal DeviceClientResponse(HttpWebResponse httpResponse)
        {
            this.HttpResponse = httpResponse ?? throw new ArgumentNullException(nameof(httpResponse));

            As = new DeviceClientResponseParser(httpResponse);
        }

        public void Dispose()
        {
            HttpResponse?.Dispose();
        }
    }
}
