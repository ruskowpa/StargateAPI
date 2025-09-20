using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using System.Diagnostics;
using System.Text.Json;

namespace StargateAPI.Business.Services
{
    public class DatabaseLoggingService : ILoggingService
    {
        private readonly StargateContext _context;
        private readonly ILogger<DatabaseLoggingService> _logger;

        public DatabaseLoggingService(StargateContext context, ILogger<DatabaseLoggingService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task LogInfoAsync(string message, string? source = null, string? method = null, string? additionalData = null)
        {
            await LogAsync("INFO", message, null, source, method, additionalData);
        }

        public async Task LogWarningAsync(string message, string? source = null, string? method = null, string? additionalData = null)
        {
            await LogAsync("WARN", message, null, source, method, additionalData);
        }

        public async Task LogErrorAsync(string message, Exception? exception = null, string? source = null, string? method = null, string? additionalData = null)
        {
            string? exceptionDetails = null;
            if (exception != null)
            {
                exceptionDetails = $"{exception.GetType().Name}: {exception.Message}\nStack Trace:\n{exception.StackTrace}";
                
                // Include inner exceptions
                var innerException = exception.InnerException;
                while (innerException != null)
                {
                    exceptionDetails += $"\n\nInner Exception:\n{innerException.GetType().Name}: {innerException.Message}\nStack Trace:\n{innerException.StackTrace}";
                    innerException = innerException.InnerException;
                }
            }

            await LogAsync("ERROR", message, exceptionDetails, source, method, additionalData);
        }

        public async Task LogDebugAsync(string message, string? source = null, string? method = null, string? additionalData = null)
        {
            await LogAsync("DEBUG", message, null, source, method, additionalData);
        }

        public async Task LogTraceAsync(string message, string? source = null, string? method = null, string? additionalData = null)
        {
            await LogAsync("TRACE", message, null, source, method, additionalData);
        }

        private async Task LogAsync(string logLevel, string message, string? exception = null, string? source = null, string? method = null, string? additionalData = null)
        {
            try
            {
                var logEntry = new LogEntry
                {
                    LogLevel = logLevel,
                    Message = message,
                    Exception = exception,
                    Source = source ?? GetCallingSource(),
                    Method = method ?? GetCallingMethod(),
                    AdditionalData = additionalData,
                    Timestamp = DateTime.UtcNow,
                    MachineName = Environment.MachineName,
                    Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown"
                };

                await _context.LogEntries.AddAsync(logEntry);
                await _context.SaveChangesAsync();

                // Also log to standard .NET logging for immediate visibility
                switch (logLevel)
                {
                    case "INFO":
                        _logger.LogInformation("DB_LOG: {Message}", message);
                        break;
                    case "WARN":
                        _logger.LogWarning("DB_LOG: {Message}", message);
                        break;
                    case "ERROR":
                        _logger.LogError("DB_LOG: {Message}", message);
                        break;
                    case "DEBUG":
                        _logger.LogDebug("DB_LOG: {Message}", message);
                        break;
                    case "TRACE":
                        _logger.LogTrace("DB_LOG: {Message}", message);
                        break;
                }
            }
            catch (Exception ex)
            {
                // Fallback to standard logging if database logging fails
                _logger.LogError(ex, "Failed to write log entry to database. Original message: {Message}", message);
            }
        }

        private string GetCallingSource()
        {
            var stackTrace = new StackTrace(true);
            var frame = stackTrace.GetFrame(3); // Skip LogAsync, public method, and calling method
            return frame?.GetMethod()?.DeclaringType?.Name ?? "Unknown";
        }

        private string GetCallingMethod()
        {
            var stackTrace = new StackTrace(true);
            var frame = stackTrace.GetFrame(3); // Skip LogAsync, public method, and calling method
            return frame?.GetMethod()?.Name ?? "Unknown";
        }
    }
}

