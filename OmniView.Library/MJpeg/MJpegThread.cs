﻿using System;
using System.IO;
using System.Net;
using System.Threading;

namespace OmniView.Library.MJpeg
{
    public class MJpegThread : IDisposable
    {
        private const int StopWaitTime = 3000;

        public event EventHandler<RenderEventArgs> Render;
        public event UnhandledExceptionEventHandler RenderError;

        private readonly MJpegStream mjpegStream;
        private volatile bool isRunning;
        private CancellationTokenSource tokenSource;
        private WebRequest request;
        private Thread thread;


        public MJpegThread()
        {
            mjpegStream = new MJpegStream();
            mjpegStream.Render += MjpegStream_OnRender;
        }

        public void Dispose()
        {
            if (isRunning)
                Stop();

            tokenSource?.Dispose();
            mjpegStream?.Dispose();
        }

        public void Start(WebRequest request)
        {
            this.request = request;

            tokenSource?.Dispose();
            tokenSource = new CancellationTokenSource();

            thread = new Thread(ThreadProcess) {
                Name = $"MJPEG",
                Priority = ThreadPriority.AboveNormal,
            };

            isRunning = true;
            thread.Start();
        }

        public void Stop()
        {
            tokenSource.Cancel();
            isRunning = false;

            try {
                thread.Join(StopWaitTime);
                thread.Abort();
            }
            catch {}
        }

        private void ThreadProcess()
        {
            Stream responseStream = null;
            HttpWebResponse response = null;

            try {
                response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode != HttpStatusCode.OK)
                    throw new ApplicationException($"Failed to connect! [{response.StatusCode}: {response.StatusDescription}]");

                responseStream = response.GetResponseStream();

                mjpegStream.Read(responseStream, tokenSource.Token);
            }
            catch (Exception error) {
                OnRenderError(error);
            }
            finally {
                responseStream?.Dispose();
                response?.Dispose();
            }
        }

        protected void OnRenderError(object exception)
        {
            try {
                RenderError?.Invoke(this, new UnhandledExceptionEventArgs(exception, false));
            }
            catch {}
        }

        private void MjpegStream_OnRender(object sender, RenderEventArgs e)
        {
            try {
                Render?.Invoke(this, e);
            }
            catch (Exception error) {
                OnRenderError(error);
            }
        }
    }
}
