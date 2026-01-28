namespace LogHub.Client;

public interface ILogHubClient
{
    Task SendLogAsync(
        LogLevel level,
        string message,
        Exception? exception = null,
        Dictionary<string, object>? properties = null,
        string? correlationId = null,
        CancellationToken cancellationToken = default);

    void QueueLog(
        LogLevel level,
        string message,
        Exception? exception = null,
        Dictionary<string, object>? properties = null,
        string? correlationId = null);

    Task FlushAsync(CancellationToken cancellationToken = default);
}
