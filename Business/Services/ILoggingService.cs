namespace StargateAPI.Business.Services
{
    public interface ILoggingService
    {
        Task LogInfoAsync(string message, string? source = null, string? method = null, string? additionalData = null);
        Task LogWarningAsync(string message, string? source = null, string? method = null, string? additionalData = null);
        Task LogErrorAsync(string message, Exception? exception = null, string? source = null, string? method = null, string? additionalData = null);
        Task LogDebugAsync(string message, string? source = null, string? method = null, string? additionalData = null);
        Task LogTraceAsync(string message, string? source = null, string? method = null, string? additionalData = null);
    }
}

