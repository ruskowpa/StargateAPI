using MediatR;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Business.Services;
using StargateAPI.Controllers;
using System.Text.Json;

namespace StargateAPI.Business.Queries
{
    public class GetLogs : IRequest<GetLogsResult>
    {
        public int? Limit { get; set; } = 50;
        public string? LogLevel { get; set; }
        public string? Source { get; set; }
    }

    public class GetLogsHandler : IRequestHandler<GetLogs, GetLogsResult>
    {
        private readonly StargateContext _context;
        private readonly ILoggingService _loggingService;

        public GetLogsHandler(StargateContext context, ILoggingService loggingService)
        {
            _context = context;
            _loggingService = loggingService;
        }

        public async Task<GetLogsResult> Handle(GetLogs request, CancellationToken cancellationToken)
        {
            var requestData = JsonSerializer.Serialize(new { request.Limit, request.LogLevel, request.Source });
            await _loggingService.LogInfoAsync($"Retrieving logs with filters", "GetLogsHandler", "Handle", requestData);

            try
            {
                var query = _context.LogEntries.AsQueryable();

                if (!string.IsNullOrEmpty(request.LogLevel))
                {
                    query = query.Where(l => l.LogLevel == request.LogLevel.ToUpper());
                }

                if (!string.IsNullOrEmpty(request.Source))
                {
                    query = query.Where(l => l.Source != null && l.Source.Contains(request.Source));
                }

                var logEntries = await query
                    .OrderByDescending(l => l.Timestamp)
                    .Take(request.Limit ?? 50)
                    .ToListAsync();

                var logs = logEntries.Select(l => new LogEntryDto
                {
                    Id = l.Id,
                    LogLevel = l.LogLevel,
                    Message = l.Message,
                    Exception = l.Exception,
                    Source = l.Source,
                    Method = l.Method,
                    Timestamp = l.Timestamp,
                    MachineName = l.MachineName,
                    Environment = l.Environment,
                    AdditionalData = l.AdditionalData
                }).ToList();

                await _loggingService.LogInfoAsync($"Successfully retrieved {logs.Count} log entries", "GetLogsHandler", "Handle", requestData);

                return new GetLogsResult
                {
                    Logs = logs,
                    Success = true,
                    Message = $"Retrieved {logs.Count} log entries"
                };
            }
            catch (Exception ex)
            {
                await _loggingService.LogErrorAsync($"Error retrieving logs", ex, "GetLogsHandler", "Handle", requestData);
                return new GetLogsResult
                {
                    Logs = new List<LogEntryDto>(),
                    Success = false,
                    Message = $"An error occurred while retrieving logs: {ex.Message}",
                    ResponseCode = 500
                };
            }
        }
    }

    public class GetLogsResult : BaseResponse
    {
        public List<LogEntryDto> Logs { get; set; } = new List<LogEntryDto>();
    }

    public class LogEntryDto
    {
        public int Id { get; set; }
        public string LogLevel { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? Exception { get; set; }
        public string? Source { get; set; }
        public string? Method { get; set; }
        public DateTime Timestamp { get; set; }
        public string? MachineName { get; set; }
        public string? Environment { get; set; }
        public string? AdditionalData { get; set; }
    }
}
