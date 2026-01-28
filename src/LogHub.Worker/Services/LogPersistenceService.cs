using System.Text.Json;
using LogHub.Core.DTOs;
using LogHub.Core.Entities;
using LogHub.Core.Interfaces;

namespace LogHub.Worker.Services;

public class LogPersistenceService : ILogPersistenceService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<LogPersistenceService> _logger;

    public LogPersistenceService(
        IServiceScopeFactory scopeFactory,
        ILogger<LogPersistenceService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task PersistLogAsync(LogMessage logMessage, CancellationToken cancellationToken = default)
    {
        using var scope = _scopeFactory.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var logEntry = MapToEntity(logMessage);

        await unitOfWork.Logs.AddAsync(logEntry, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogDebug("Log persisted: {LogId}", logEntry.Id);
    }

    public async Task PersistLogsAsync(IEnumerable<LogMessage> logMessages, CancellationToken cancellationToken = default)
    {
        using var scope = _scopeFactory.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var logEntries = logMessages.Select(MapToEntity).ToList();

        await unitOfWork.Logs.AddRangeAsync(logEntries, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogDebug("Batch of {Count} logs persisted", logEntries.Count);
    }

    private static LogEntry MapToEntity(LogMessage message)
    {
        return new LogEntry
        {
            ApplicationId = message.ApplicationId,
            Level = message.Level,
            Message = message.Message,
            Exception = message.Exception,
            StackTrace = message.StackTrace,
            Properties = message.Properties != null
                ? JsonSerializer.Serialize(message.Properties)
                : null,
            CorrelationId = message.CorrelationId,
            Source = message.Source,
            Timestamp = message.Timestamp
        };
    }
}
