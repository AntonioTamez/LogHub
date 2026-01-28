using LogHub.Core.Enums;

namespace LogHub.Core.Entities;

public class LogEntry : BaseEntity
{
    public Guid ApplicationId { get; set; }
    public LogLevel Level { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Exception { get; set; }
    public string? StackTrace { get; set; }
    public string? Properties { get; set; } // JSON
    public string? CorrelationId { get; set; }
    public string? Source { get; set; }
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;

    // Navigation property
    public Application Application { get; set; } = null!;
}
