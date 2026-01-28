using LogHub.Core.DTOs;

namespace LogHub.API.Services;

public interface IRabbitMQPublisher
{
    Task PublishLogAsync(LogMessage logMessage, CancellationToken cancellationToken = default);
    Task PublishLogsAsync(IEnumerable<LogMessage> logMessages, CancellationToken cancellationToken = default);
}
