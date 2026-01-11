using Microsoft.Extensions.Logging;

namespace EntryIt.Services
{
    public interface ILoggerService 
    {
        void LogInfo(string message);
        void LogWarning(string message);
        void LogError(string message, Exception? ex = null);
    }

    public class LoggerService : ILoggerService
    {
        private readonly ILogger<LoggerService> _logger;

        public LoggerService(ILogger<LoggerService> logger)
        {
            _logger = logger;
        }

        public void LogInfo(string message)
        {
            _logger.LogInformation(message);
        }

        public void LogWarning(string message)
        {
            _logger.LogWarning(message);
        }

        public void LogError(string message, Exception? ex = null)
        {
            if (ex != null)
                _logger.LogError(ex, message);
            else
                _logger.LogError(message);
        }
    }
}
