using System;
using System.IO;
using log4net;
using log4net.Config;

namespace Merit.BarCodeScanner.Logging
{
    public class FileLogManager: ILogger
    {
        private readonly ILog _log4net;

        static FileLogManager()
        {
            string logConfigDirection = AppDomain.CurrentDomain.RelativeSearchPath ??
                                        AppDomain.CurrentDomain.BaseDirectory;
            string logConfigFilePath = Path.Combine(logConfigDirection, "log4net.config");
            XmlConfigurator.ConfigureAndWatch(new FileInfo(logConfigFilePath));
        }

        public FileLogManager(Type logClass)
        {
            _log4net = LogManager.GetLogger(logClass);
        }

        public void LogFatal(string message)
        {
            if (_log4net.IsFatalEnabled)
                _log4net.Fatal(message);
        }

        public void LogFatal(string message, Exception ex)
        {
            if (_log4net.IsFatalEnabled)
                _log4net.Fatal(message, ex);
        }

        public void LogError(string message)
        {
            if (_log4net.IsErrorEnabled)
                _log4net.Error(message);
        }

        public void LogError(string message, Exception ex)
        {
            if (_log4net.IsErrorEnabled)
                _log4net.Error(message, ex);
        }

        public void LogWarn(string message)
        {
            if (_log4net.IsWarnEnabled)
                _log4net.Warn(message);
        }

        public void LogWarn(string message, Exception ex)
        {
            if (_log4net.IsWarnEnabled)
                _log4net.Warn(message, ex);
        }

        public void LogInfo(string message)
        {
            if (_log4net.IsInfoEnabled)
                _log4net.Info(message);
        }

        public void LogInfo(string message, Exception ex)
        {
            if (_log4net.IsInfoEnabled)
                _log4net.Info(message, ex);
        }

        public void LogDebug(string message)
        {
            if (_log4net.IsDebugEnabled)
                _log4net.Debug(message);
        }

        public void LogDebug(string message, Exception ex)
        {
            if (_log4net.IsDebugEnabled)
                _log4net.Debug(message, ex);
        }
    }
}
