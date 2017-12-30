using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OmniView.Library.Network
{
    public class DeviceClientResponseParser
    {
        private readonly HttpWebResponse response;


        internal DeviceClientResponseParser(HttpWebResponse response)
        {
            this.response = response;
        }

        public Stream RawStream()
        {
            return response.GetResponseStream();
        }

        public Stream BufferedStream(int bufferSize = 4096)
        {
            MemoryStream stream = null;

            try {
                stream = new MemoryStream();

                using (var inputStream = response.GetResponseStream()) {
                    int size;
                    var buffer = new byte[bufferSize];
                    while ((size = inputStream.Read(buffer, 0, bufferSize)) > 0) {
                        stream.Write(buffer, 0, size);
                    }
                }

                stream.Flush();
                stream.Seek(0, SeekOrigin.Begin);
                return stream;
            }
            catch {
                stream?.Dispose();
                throw;
            }
        }

        public async Task<Stream> BufferedStreamAsync(int bufferSize = 4096)
        {
            MemoryStream stream = null;

            try {
                stream = new MemoryStream();

                using (var inputStream = response.GetResponseStream()) {
                    int size;
                    var buffer = new byte[bufferSize];
                    while ((size = await inputStream.ReadAsync(buffer, 0, bufferSize)) > 0) {
                        await stream.WriteAsync(buffer, 0, size);
                    }
                }

                await stream.FlushAsync();
                stream.Seek(0, SeekOrigin.Begin);
                return stream;
            }
            catch {
                stream?.Dispose();
                throw;
            }
        }

        public string Text()
        {
            var encoding = Encoding.UTF8;

            if (!string.IsNullOrEmpty(response.ContentEncoding))
                encoding = Encoding.GetEncoding(response.ContentEncoding);

            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream, encoding)) {
                return reader.ReadToEnd();
            }
        }

        public async Task<string> TextAsync()
        {
            var encoding = Encoding.UTF8;

            if (!string.IsNullOrEmpty(response.ContentEncoding))
                encoding = Encoding.GetEncoding(response.ContentEncoding);

            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream, encoding)) {
                return await reader.ReadToEndAsync();
            }
        }
    }
}
