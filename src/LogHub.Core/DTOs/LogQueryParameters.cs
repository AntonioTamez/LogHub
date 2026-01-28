using LogHub.Core.Enums;

namespace LogHub.Core.DTOs;

public class LogQueryParameters
{
    public Guid? ApplicationId { get; set; }
    public LogLevel? MinLevel { get; set; }
    public LogLevel? MaxLevel { get; set; }
    public string? SearchText { get; set; }
    public string? CorrelationId { get; set; }
    public DateTimeOffset? From { get; set; }
    public DateTimeOffset? To { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public string SortBy { get; set; } = "Timestamp";
    public bool SortDescending { get; set; } = true;
}
