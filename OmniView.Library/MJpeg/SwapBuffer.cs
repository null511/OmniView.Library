using System;
using System.Threading;

namespace OmniView.Library.MJpeg
{
    internal class SwapBuffer : IDisposable
    {
        private readonly ManualResetEventSlim writeLock_a;
        private readonly ManualResetEventSlim writeLock_b;

        public event EventHandler Release;

        private readonly MJpegImage buffer_a, buffer_b;
        private BufferWriter currentWriter;
        private bool write_a;


        public SwapBuffer()
        {
            write_a = true;
            buffer_a = new MJpegImage();
            buffer_b = new MJpegImage();

            writeLock_a = new ManualResetEventSlim(true);
            writeLock_b = new ManualResetEventSlim(true);
        }

        public void Dispose()
        {
            buffer_a?.Dispose();
            buffer_b?.Dispose();
            writeLock_a?.Dispose();
            writeLock_b?.Dispose();
        }

        public BufferWriter GetWriteBuffer()
        {
            if (write_a) {
                writeLock_a.Wait();

                buffer_a.Clear();
                writeLock_a.Reset();

                return currentWriter = new BufferWriter(buffer_a, () => {
                    writeLock_a.Set();

                    Swap();
                    OnRelease();
                });
            }
            else {
                writeLock_b.Wait();

                buffer_b.Clear();
                writeLock_b.Reset();

                return currentWriter = new BufferWriter(buffer_b, () => {
                    writeLock_b.Set();

                    Swap();
                    OnRelease();
                });
            }
        }

        public MJpegImage GetReadBuffer()
        {
            return write_a ? buffer_b : buffer_a;
        }

        public void Swap()
        {
            write_a = !write_a;
        }

        public void OnRelease()
        {
            try {
                Release?.Invoke(this, EventArgs.Empty);
            }
            catch {}
        }
    }
}
