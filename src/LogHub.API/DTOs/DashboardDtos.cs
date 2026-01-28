namespace LogHub.API.DTOs;

public record DashboardStatsDto(
    int TotalLogs,
    int TraceCount,
    int DebugCount,
    int InformationCount,
    int WarningCount,
    int ErrorCount,
    int CriticalCount,
    Dictionary<string, int> LogsByApplication,
    Dictionary<string, int> LogsByHour,
    Dictionary<string, int> LogsByDay
);

public record DashboardQueryRequest
{
    public Guid? ApplicationId { get; init; }
    public DateTimeOffset? From { get; init; }
    public DateTimeOffset? To { get; init; }
}
