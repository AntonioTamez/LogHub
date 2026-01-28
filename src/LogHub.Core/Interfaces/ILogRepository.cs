using LogHub.Core.DTOs;
using LogHub.Core.Entities;
using LogHub.Core.Enums;

namespace LogHub.Core.Interfaces;

public interface ILogRepository : IRepository<LogEntry>
{
    Task<PagedResult<LogEntry>> GetLogsPagedAsync(
        LogQueryParameters parameters,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<LogEntry>> GetLogsByApplicationAsync(
        Guid applicationId,
        DateTimeOffset? from = null,
        DateTimeOffset? to = null,
        LogLevel? minLevel = null,
        CancellationToken cancellationToken = default);

    Task<LogStats> GetStatsAsync(
        Guid? applicationId = null,
        DateTimeOffset? from = null,
        DateTimeOffset? to = null,
        CancellationToken cancellationToken = default);

    Task<int> DeleteOldLogsAsync(
        int retentionDays,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<LogEntry>> GetRecentLogsAsync(
        Guid applicationId,
        int count = 100,
        CancellationToken cancellationToken = default);
}
