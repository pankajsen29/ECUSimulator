using Serilog;
using Serilog.Events;
using System;

namespace LoggerLib
{
    /// <summary>
    /// https://github.com/serilog/serilog/blob/dev/LICENSE
    /// https://github.com/serilog/serilog-sinks-file/blob/dev/LICENSE
    /// </summary>
    public class Logger : IDisposable
    {
        private ILogger _loggerObj;

        public Logger(string logFilePath)
        {
            _loggerObj = GetLogger(logFilePath);
        }

        ~Logger()
        {
            Dispose(false);
        }

        public ILogger GetLogger(string logFilePath)
        {
            return new LoggerConfiguration()
                    .MinimumLevel.Is(LogEventLevel.Information)
                    .WriteTo.File(
                        path: logFilePath,
                        outputTemplate: "[{Level:u3}] {Message:lj}{NewLine}{Exception}",
                        fileSizeLimitBytes: 10000000,
                        rollOnFileSizeLimit: true,
                        retainedFileCountLimit: 10)
                    .CreateLogger();
        }

        public void WriteToLog(string message, bool addtimestamp = true)
        {
            if (addtimestamp)
            {
                message = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {message}";
            }
            _loggerObj.Information(message);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (m_isDisposed)
                return;

            if (disposing)
            {
                (_loggerObj as Logger)?.Dispose();
            }
            m_isDisposed = true;
        }
        private bool m_isDisposed = false;
    }
}
