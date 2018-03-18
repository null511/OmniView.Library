using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OmniView.Library.Logging
{
    public class LogManager : IDisposable
    {
        public static LogManager Global {get;} = new LogManager("Global");

        public event EventHandler Error;

        private readonly ConcurrentDictionary<string, Logger> loggerList;
        private readonly ManualResetEventSlim appendEvent;

        private Thread thread;
        private CancellationTokenSource tokenSource;

        public string Name {get;}


        public LogManager(string name)
        {
            this.Name = name;

            loggerList = new ConcurrentDictionary<string, Logger>(StringComparer.OrdinalIgnoreCase);
            appendEvent = new ManualResetEventSlim(false);
        }

        public void Dispose()
        {
            try {
                Stop();
            }
            catch {}

            tokenSource?.Dispose();
            appendEvent?.Dispose();
        }

        public void Start()
        {
            tokenSource = new CancellationTokenSource();

            thread = new Thread(ThreadProcess) {
                Name = "LogManager",
                IsBackground = true,
                Priority = ThreadPriority.BelowNormal,
            };

            thread.Start(tokenSource.Token);
        }

        public void Stop()
        {
            try {
                Complete();

                tokenSource.Cancel();
                thread.Join();
            }
            finally {
                tokenSource.Dispose();
            }
        }

        public void Complete()
        {
            var list = loggerList.Values.ToArray();

            foreach (var logger in list)
                logger.Complete();
        }

        public void Add(Logger logger)
        {
            logger.MessageAppended += OnLoggerAppend;
            loggerList[logger.Name] = logger;
        }

        public bool Remove(Logger logger)
        {
            logger.MessageAppended -= OnLoggerAppend;
            return loggerList.TryRemove(logger.Name, out _);
        }

        private void ThreadProcess(object arguments)
        {
            var token = (CancellationToken)arguments;

            var taskList = new List<Task>();

            while (!token.IsCancellationRequested) {
                var anyUpdates = false;
                taskList.Clear();

                try {
                    var list = loggerList.Values.ToArray();

                    foreach (var logger in list) {
                        if (logger. QueueCount > 0) {
                            anyUpdates = true;
                            taskList.Add(logger.Update());
                        }
                    }

                    if (anyUpdates) {
                        Task.WaitAll(taskList.ToArray());
                    }
                    else {
                        try {
                            appendEvent.Reset();
                            appendEvent.Wait(token);
                        }
                        catch (OperationCanceledException) {}
                    }
                }
                catch (Exception error) {
                    OnError(error);
                }
            }
        }

        protected void OnLoggerAppend(object sender, EventArgs e)
        {
            appendEvent.Set();
        }

        protected void OnError(Exception error)
        {
            // TODO: Create an actual error event!
            Error?.Invoke(this, EventArgs.Empty);
        }
    }
}
