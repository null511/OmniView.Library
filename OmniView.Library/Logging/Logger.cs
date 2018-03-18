using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace OmniView.Library.Logging
{
    public abstract class Logger : ILog
    {
        public event EventHandler MessageAppended;

        private readonly BlockingCollection<string> messageQueue;

        public string Name {get;}
        public Encoding Encoding {get; set;}
        public int QueueCount => messageQueue.Count;


        public Logger(string name)
        {
            this.Name = name;

            Encoding = Encoding.UTF8;
            messageQueue = new BlockingCollection<string>();
        }

        public void Add(string message)
        {
            var indentedMessage = IndentMessage(message);

            messageQueue.Add(indentedMessage);
            OnMessageAppended();
        }

        internal void Complete()
        {
            messageQueue.CompleteAdding();
        }

        internal async Task Update()
        {
            if (messageQueue.Count == 0) return;

            using (var stream = GetStream())
            using (var writer = new StreamWriter(stream, Encoding)) {
                writer.AutoFlush = false;

                while (messageQueue.TryTake(out var message)) {
                    await writer.WriteLineAsync(message);
                }

                await writer.FlushAsync();
            }
        }

        protected abstract Stream GetStream();

        protected void OnMessageAppended()
        {
            MessageAppended?.Invoke(this, EventArgs.Empty);
        }

        private static string IndentMessage(string message, int indent = 4, string newline = "\r\n")
        {
            if (indent < 0) throw new ApplicationException("'indent' must be greater than or equal to zero!");
            if (indent == 0 || string.IsNullOrEmpty(message)) return message;

            var indentString = new string(' ', indent);
            var lineList = message.Split(new[] {newline}, StringSplitOptions.None);

            for (var i = 1; i < lineList.Length; i++) {
                lineList[i] = indentString + lineList[i];
            }

            return string.Join(newline, lineList);
        }
    }
}
