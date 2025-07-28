using Serilog;
using System;

namespace LoggerLib
{
    /// <summary>
    /// https://github.com/serilog/serilog/blob/dev/LICENSE
    /// https://github.com/serilog/serilog-sinks-file/blob/dev/LICENSE
    /// https://github.com/serilog/serilog-settings-appsettings/blob/dev/LICENSE
    /// </summary>
    public class Logger : IDisposable
    {
        private LoggerConfiguration _loggerConfig;

        public Logger(string logfile)
        {
            ConfigureLogger(logfile);
        }

        ~Logger()
        {
            Dispose(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private void ConfigureLogger(string logfile)
        {
            _loggerConfig = new LoggerConfiguration();            

            if (!String.IsNullOrWhiteSpace(logfile))
            {
                _loggerConfig.WriteTo.File(logfile,
                    rollOnFileSizeLimit: true,
                    fileSizeLimitBytes: 10000000,
                    retainedFileCountLimit: 10,
                    outputTemplate: "[{Level:u3}] {Message:lj}{NewLine}{Exception}");
                Log.Logger = _loggerConfig.CreateLogger();
            }
        }

        public void WriteToLog(string message, bool addtimestamp = true)
        {
            if (addtimestamp)
            {
                message = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {message}";
            }
            Log.Information(message);
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
                Log.CloseAndFlush();
            }
            m_isDisposed = true;
        }
        private bool m_isDisposed = false;
    }
}
