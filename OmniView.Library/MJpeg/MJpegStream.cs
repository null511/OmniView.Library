using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace OmniView.Library.MJpeg
{
    public class MJpegStream : IDisposable
    {
        private const int BUFFER_SIZE = 4096;
        private const int HEADER_SIZE = 256;
        private const string tag_length = "Content-Length:";
        private const string stamp_format = "yyyyMMddHHmmssfff";

        public event EventHandler<RenderEventArgs> Render;

        private bool isHead, isSetup;
        private byte[] buffer, newline, newline_src;
        private int imgBufferStart;
        private Stream data_stream;
        private readonly SwapBuffer swapBuffer;
        private BufferWriter writeBuffer;
        private int headStart, headStop;
        private int imgSize, imgSizeTgt;
        private string boundary_tag;
        private bool tagReadStarted;

        public Encoding Encoding {get; set;}


        public MJpegStream()
        {
            Encoding = Encoding.UTF8;

            isSetup = false;
            buffer = new byte[BUFFER_SIZE];
            newline_src = new byte[] { 13, 10 };

            swapBuffer = new SwapBuffer();
            swapBuffer.Release += SwapBuffer_OnRelease;
        }

        public void Dispose()
        {
            swapBuffer?.Dispose();
            data_stream?.Dispose();
        }

        public void Read(Stream stream, CancellationToken token)
        {
            data_stream = stream;

            StartHeader(0);

            while (!token.IsCancellationRequested) {
                try {
                    ProcessTask(token);
                }
                catch (EndOfStreamException) {
                    break;
                }
            }
        }

        private bool ProcessTask(CancellationToken token)
        {
            try {
                if (isHead) ProcessHeader();
                else ProcessImageBoundary();
                return true;
            }
            catch (IOException error) {
                if (error.InnerException != null) {
                    var socketException = error.InnerException as SocketException;
                    if (socketException.SocketErrorCode == SocketError.Interrupted) return false;
                }
                throw;
            }
            catch (ObjectDisposedException) {
                return false;
            }
            catch (Exception) {
                throw;
            }
        }

        private void SwapBuffer_OnRelease(object sender, EventArgs e)
        {
            var stream = swapBuffer.GetReadBuffer();

            OnRender(stream);
        }

        protected void OnRender(Stream imageData)
        {
            try {
                Render?.Invoke(this, new RenderEventArgs(imageData));
            }
            catch {}
        }

        //-----------------------------
        #region Header

        private void StartHeader(int remaining_bytes)
        {
            isHead = true;
            headStart = 0;
            headStop = remaining_bytes;
            imgSizeTgt = 0;
            tagReadStarted = false;
        }

        private void ProcessHeader()
        {
            int nl, t = Math.Min(BUFFER_SIZE - headStop, HEADER_SIZE);
            var readSize = data_stream.Read(buffer, headStop, t);
            if (readSize == 0) throw new EndOfStreamException();
            headStop += readSize;

            if (!isSetup) {
                if ((nl = FindNewline(headStart, headStop, out byte[] new_newline)) >= 0) {
                    var tag = Encoding.GetString(buffer, headStart, nl - headStart);
                    if (tag.StartsWith("--")) boundary_tag = tag;
                    headStart = nl + new_newline.Length;
                    newline = new_newline;
                    isSetup = true;
                    return;
                }
            } else {
                while ((nl = FindData(ref newline, headStart, headStop)) >= 0) {
                    var tag = Encoding.GetString(buffer, headStart, nl - headStart);
                    if (!tagReadStarted && tag.Length > 0) tagReadStarted = true;
                    headStart = nl+newline.Length;

                    if (!ProcessHeaderData(tag, nl)) return;
                }
            }

            if (headStop >= BUFFER_SIZE) {
                //var data = encoder.GetString(buffer, headStart, headStop - headStart);
                throw new ApplicationException("Invalid Header!");
            }
        }

        private bool ProcessHeaderData(string tag, int index)
        {
            if (tag.StartsWith(tag_length, StringComparison.OrdinalIgnoreCase)) {
                var val = tag.Substring(tag_length.Length);
                imgSizeTgt = int.Parse(val);
                return true;
            }

            if (tag.Length == 0 && tagReadStarted) {
                if (imgSizeTgt > 0) {
                    writeBuffer = swapBuffer.GetWriteBuffer();

                    var remainingLength = headStop - headStart;
                    if (remainingLength > 0)
                        writeBuffer.Stream.Write(buffer, headStart, remainingLength);

                    CopyStream(data_stream, writeBuffer.Stream, imgSizeTgt - remainingLength);

                    ProcessImageData(0);
                    StartHeader(0);
                    return false;
                }
                if (boundary_tag != null) {
                    writeBuffer = swapBuffer.GetWriteBuffer();

                    var remainingLength = headStop - headStart;
                    writeBuffer.Stream.Write(buffer, headStart, remainingLength);

                    imgBufferStart = remainingLength;
                    imgSize = 0;

                    isHead = false;
                    return false;
                }
            }

            return true;
        }

        #endregion
        //-----------------------------
        #region Image

        private void CopyStream(Stream source, Stream dest, int length, int bufferSize = 4096) {
            var buffer = new byte[bufferSize];
            int readLength, pos = 0;
            while (pos < length) {
                var remainingLength = length - pos;
                var targetReadLength = Math.Min(remainingLength, bufferSize);
                readLength = source.Read(buffer, 0, targetReadLength);
                if (readLength == 0) throw new EndOfStreamException();

                dest.Write(buffer, 0, readLength);
                pos += readLength;
            }
        }

        private void ProcessImageBoundary() {
            var targetReadLength = Math.Max(BUFFER_SIZE - imgBufferStart, 0);
            var readLength = data_stream.Read(buffer, imgBufferStart, targetReadLength);

            int tag_length = boundary_tag.Length;
            int nl_length = newline.Length;
            int nl, start = 0;
            int end = imgBufferStart + readLength;
            while ((nl = FindData(ref newline, start, end)) >= 0) {
                if (nl+nl_length+tag_length > BUFFER_SIZE) {
                    AppendImageData(start, nl+nl_length - start);
                    start = nl+nl_length;
                    continue;
                }

                // Try decoding the boundary_tag as byte[] for quicker comparisons?
                string v = Encoding.GetString(buffer, nl+nl_length, tag_length);
                if (v == boundary_tag) {
                    AppendImageData(start, nl - start);
                    int xstart = nl+nl_length + tag_length;
                    int xsize = ShiftBytes(xstart, end);
                    ProcessImageData(xsize);
                    return;
                } else {
                    AppendImageData(start, nl+nl_length - start);
                }
                start = nl+nl_length;
            }

            if (start < end) {
                int end_x = end - nl_length;
                if (start < end_x) {
                    AppendImageData(start, end_x - start);
                }

                ShiftBytes(end - nl_length, end);
                imgBufferStart = nl_length;
            }
        }

        private void ProcessImageData(int remaining_bytes) {
            writeBuffer.Release();

            StartHeader(remaining_bytes);
        }

        private void AppendImageData(int index, int length) {
            writeBuffer.Stream.Write(buffer, index, length);
            imgSize += (length - index);
        }

        #endregion
        //-----------------------------
        #region Utilities

        private int FindNewline(int start, int stop, out byte[] data) {
            for (int i = start; i < stop; i++) {
                if (i < stop-1 && buffer[i] == newline_src[0] && buffer[i+1] == newline_src[1]) {
                    data = newline_src;
                    return i;
                } else if (buffer[i] == newline_src[1]) {
                    data = new byte[] {newline_src[1]};
                    return i;
                }
            }
            data = null;
            return -1;
        }

        private int FindData(ref byte[] data, int start, int stop) {
            int data_size = data.Length;
            for (int i = start; i < stop-data_size; i++) {
                if (FindInnerData(ref data, i)) return i;
            }
            return -1;
        }

        private bool FindInnerData(ref byte[] data, int buffer_index) {
            int count = data.Length;
            for (int i = 0; i < count; i++) {
                if (data[i] != buffer[buffer_index+i]) return false;
            }
            return true;
        }

        private int ShiftBytes(int start, int end) {
            int c = end - start;
            for (int i = 0; i < c; i++) {
                buffer[i] = buffer[end-c+i];
            }
            return c;
        }

        #endregion
    }
}
