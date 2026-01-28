using LogHub.Core.DTOs;

namespace LogHub.Worker.Services;

public interface ILogPersistenceService
{
    Task PersistLogAsync(LogMessage logMessage, CancellationToken cancellationToken = default);
    Task PersistLogsAsync(IEnumerable<LogMessage> logMessages, CancellationToken cancellationToken = default);
}
