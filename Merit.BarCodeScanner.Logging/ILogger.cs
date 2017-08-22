using System;

namespace Merit.BarCodeScanner.Logging
{
    public interface ILogger
    {
        void LogFatal(string message);
        void LogFatal(string message, Exception ex);

        void LogError(string message);
        void LogError(string message, Exception ex);

        void LogWarn(string message);
        void LogWarn(string message, Exception ex);

        void LogInfo(string message);
        void LogInfo(string message, Exception ex);

        void LogDebug(string message);
        void LogDebug(string message, Exception ex);
    }
}
