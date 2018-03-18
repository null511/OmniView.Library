using OmniView.Library.Common;
using System;
using System.Text;

namespace OmniView.Library.Logging
{
    public interface ILog
    {
        void Add(string message);
    }

    public static class LogExtensions
    {
        public static void Debug(this ILog logger, string message)
        {
            WriteMessage(logger, "DEBUG", message);
        }

        public static void Info(this ILog logger, string message)
        {
            WriteMessage(logger, "INFO", message);
        }

        public static void Warning(this ILog logger, string message)
        {
            WriteMessage(logger, "WARN", message);
        }

        public static void Error(this ILog logger, string message, Exception error = null)
        {
            WriteMessage(logger, "ERROR", message, error);
        }

        public static void Fatal(this ILog logger, string message, Exception error = null)
        {
            WriteMessage(logger, "FATAL", message, error);
        }

        private static void WriteMessage(ILog logger, string level, string message, Exception error = null)
        {
            var now = DateTime.Now.ToString();

            var line = new StringBuilder()
                .Append($"[{level.PadLeft(5, ' ')}] {now}");

            if (!string.IsNullOrEmpty(message))
                line.Append($" - {message}");

            if (error != null)
                line.Append($" > {error.UnfoldMessageString()}");

            logger.Add(line.ToString());
        }
    }
}
