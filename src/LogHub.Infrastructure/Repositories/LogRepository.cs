using LogHub.Core.DTOs;
using LogHub.Core.Entities;
using LogHub.Core.Enums;
using LogHub.Core.Interfaces;
using LogHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LogHub.Infrastructure.Repositories;

public class LogRepository : Repository<LogEntry>, ILogRepository
{
    public LogRepository(LogHubDbContext context) : base(context)
    {
    }

    public async Task<PagedResult<LogEntry>> GetLogsPagedAsync(
        LogQueryParameters parameters,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();

        // Apply filters
        if (parameters.ApplicationId.HasValue)
            query = query.Where(l => l.ApplicationId == parameters.ApplicationId.Value);

        if (parameters.MinLevel.HasValue)
            query = query.Where(l => l.Level >= parameters.MinLevel.Value);

        if (parameters.MaxLevel.HasValue)
            query = query.Where(l => l.Level <= parameters.MaxLevel.Value);

        if (!string.IsNullOrWhiteSpace(parameters.SearchText))
            query = query.Where(l => l.Message.Contains(parameters.SearchText) ||
                                     (l.Exception != null && l.Exception.Contains(parameters.SearchText)));

        if (!string.IsNullOrWhiteSpace(parameters.CorrelationId))
            query = query.Where(l => l.CorrelationId == parameters.CorrelationId);

        if (parameters.From.HasValue)
            query = query.Where(l => l.Timestamp >= parameters.From.Value);

        if (parameters.To.HasValue)
            query = query.Where(l => l.Timestamp <= parameters.To.Value);

        // Get total count
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply sorting
        query = parameters.SortBy.ToLower() switch
        {
            "level" => parameters.SortDescending
                ? query.OrderByDescending(l => l.Level)
                : query.OrderBy(l => l.Level),
            "message" => parameters.SortDescending
                ? query.OrderByDescending(l => l.Message)
                : query.OrderBy(l => l.Message),
            _ => parameters.SortDescending
                ? query.OrderByDescending(l => l.Timestamp)
                : query.OrderBy(l => l.Timestamp)
        };

        // Apply pagination
        var items = await query
            .Skip((parameters.Page - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .Include(l => l.Application)
            .ToListAsync(cancellationToken);

        return new PagedResult<LogEntry>
        {
            Items = items,
            TotalCount = totalCount,
            Page = parameters.Page,
            PageSize = parameters.PageSize
        };
    }

    public async Task<IEnumerable<LogEntry>> GetLogsByApplicationAsync(
        Guid applicationId,
        DateTimeOffset? from = null,
        DateTimeOffset? to = null,
        LogLevel? minLevel = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(l => l.ApplicationId == applicationId);

        if (from.HasValue)
            query = query.Where(l => l.Timestamp >= from.Value);

        if (to.HasValue)
            query = query.Where(l => l.Timestamp <= to.Value);

        if (minLevel.HasValue)
            query = query.Where(l => l.Level >= minLevel.Value);

        return await query
            .OrderByDescending(l => l.Timestamp)
            .ToListAsync(cancellationToken);
    }

    public async Task<LogStats> GetStatsAsync(
        Guid? applicationId = null,
        DateTimeOffset? from = null,
        DateTimeOffset? to = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();

        if (applicationId.HasValue)
            query = query.Where(l => l.ApplicationId == applicationId.Value);

        if (from.HasValue)
            query = query.Where(l => l.Timestamp >= from.Value);

        if (to.HasValue)
            query = query.Where(l => l.Timestamp <= to.Value);

        var logs = await query
            .Include(l => l.Application)
            .ToListAsync(cancellationToken);

        var stats = new LogStats
        {
            TotalLogs = logs.Count,
            TraceCount = logs.Count(l => l.Level == LogLevel.Trace),
            DebugCount = logs.Count(l => l.Level == LogLevel.Debug),
            InformationCount = logs.Count(l => l.Level == LogLevel.Information),
            WarningCount = logs.Count(l => l.Level == LogLevel.Warning),
            ErrorCount = logs.Count(l => l.Level == LogLevel.Error),
            CriticalCount = logs.Count(l => l.Level == LogLevel.Critical),
            LogsByApplication = logs
                .GroupBy(l => l.Application?.Name ?? "Unknown")
                .ToDictionary(g => g.Key, g => g.Count()),
            LogsByHour = logs
                .GroupBy(l => l.Timestamp.ToString("yyyy-MM-dd HH:00"))
                .OrderBy(g => g.Key)
                .ToDictionary(g => g.Key, g => g.Count()),
            LogsByDay = logs
                .GroupBy(l => l.Timestamp.ToString("yyyy-MM-dd"))
                .OrderBy(g => g.Key)
                .ToDictionary(g => g.Key, g => g.Count())
        };

        return stats;
    }

    public async Task<int> DeleteOldLogsAsync(
        int retentionDays,
        CancellationToken cancellationToken = default)
    {
        var cutoffDate = DateTimeOffset.UtcNow.AddDays(-retentionDays);

        return await _dbSet
            .Where(l => l.Timestamp < cutoffDate)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<IEnumerable<LogEntry>> GetRecentLogsAsync(
        Guid applicationId,
        int count = 100,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(l => l.ApplicationId == applicationId)
            .OrderByDescending(l => l.Timestamp)
            .Take(count)
            .ToListAsync(cancellationToken);
    }
}
