using LogHub.Core.Enums;

namespace LogHub.Core.DTOs;

public class LogMessage
{
    public Guid ApplicationId { get; set; }
    public LogLevel Level { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Exception { get; set; }
    public string? StackTrace { get; set; }
    public Dictionary<string, object>? Properties { get; set; }
    public string? CorrelationId { get; set; }
    public string? Source { get; set; }
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
}
