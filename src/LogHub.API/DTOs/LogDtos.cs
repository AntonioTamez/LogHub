using System.ComponentModel.DataAnnotations;
using LogLevel = LogHub.Core.Enums.LogLevel;

namespace LogHub.API.DTOs;

public record CreateLogRequest(
    [Required] LogLevel Level,
    [Required] string Message,
    string? Exception,
    string? StackTrace,
    Dictionary<string, object>? Properties,
    string? CorrelationId,
    string? Source,
    DateTimeOffset? Timestamp
);

public record BatchLogRequest(
    [Required][MinLength(1)] List<CreateLogRequest> Logs
);

public record LogEntryDto(
    Guid Id,
    Guid ApplicationId,
    string ApplicationName,
    LogLevel Level,
    string Message,
    string? Exception,
    string? StackTrace,
    string? Properties,
    string? CorrelationId,
    string? Source,
    DateTimeOffset Timestamp
);

public record LogQueryRequest
{
    public Guid? ApplicationId { get; init; }
    public LogLevel? MinLevel { get; init; }
    public LogLevel? MaxLevel { get; init; }
    public string? SearchText { get; init; }
    public string? CorrelationId { get; init; }
    public DateTimeOffset? From { get; init; }
    public DateTimeOffset? To { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 50;
    public string SortBy { get; init; } = "Timestamp";
    public bool SortDescending { get; init; } = true;
}

public record PagedResponse<T>(
    IEnumerable<T> Items,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages,
    bool HasPreviousPage,
    bool HasNextPage
);
