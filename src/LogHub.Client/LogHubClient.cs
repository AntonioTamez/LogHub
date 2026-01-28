using System.Collections.Concurrent;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace LogHub.Client;

public class LogHubClient : ILogHubClient, IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly LogHubOptions _options;
    private readonly ConcurrentQueue<LogEntry> _logQueue = new();
    private readonly Timer _flushTimer;
    private readonly SemaphoreSlim _flushLock = new(1, 1);
    private bool _disposed;

    public LogHubClient(HttpClient httpClient, IOptions<LogHubOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;

        // Configure HTTP client
        _httpClient.BaseAddress = new Uri(_options.ApiUrl);
        _httpClient.DefaultRequestHeaders.Add("X-Api-Key", _options.ApiKey);

        // Start flush timer
        _flushTimer = new Timer(
            async _ => await FlushAsync(CancellationToken.None),
            null,
            _options.FlushInterval,
            _options.FlushInterval);
    }

    public async Task SendLogAsync(
        LogLevel level,
        string message,
        Exception? exception = null,
        Dictionary<string, object>? properties = null,
        string? correlationId = null,
        CancellationToken cancellationToken = default)
    {
        if (level < _options.MinimumLevel)
            return;

        var logEntry = CreateLogEntry(level, message, exception, properties, correlationId);

        var response = await _httpClient.PostAsJsonAsync(
            "/api/logs",
            logEntry,
            cancellationToken);

        response.EnsureSuccessStatusCode();
    }

    public void QueueLog(
        LogLevel level,
        string message,
        Exception? exception = null,
        Dictionary<string, object>? properties = null,
        string? correlationId = null)
    {
        if (level < _options.MinimumLevel)
            return;

        var logEntry = CreateLogEntry(level, message, exception, properties, correlationId);
        _logQueue.Enqueue(logEntry);

        // Auto-flush if queue is full
        if (_logQueue.Count >= _options.BatchSize)
        {
            _ = FlushAsync(CancellationToken.None);
        }
    }

    public async Task FlushAsync(CancellationToken cancellationToken = default)
    {
        if (_logQueue.IsEmpty)
            return;

        if (!await _flushLock.WaitAsync(0, cancellationToken))
            return; // Skip if already flushing

        try
        {
            var batch = new List<LogEntry>();

            while (batch.Count < _options.BatchSize && _logQueue.TryDequeue(out var entry))
            {
                batch.Add(entry);
            }

            if (batch.Count == 0)
                return;

            var request = new BatchLogRequest { Logs = batch };

            var response = await _httpClient.PostAsJsonAsync(
                "/api/logs/batch",
                request,
                cancellationToken);

            response.EnsureSuccessStatusCode();
        }
        catch (Exception)
        {
            // Re-queue failed logs
            // In production, you might want to log this error or use a dead-letter queue
        }
        finally
        {
            _flushLock.Release();
        }
    }

    private LogEntry CreateLogEntry(
        LogLevel level,
        string message,
        Exception? exception,
        Dictionary<string, object>? properties,
        string? correlationId)
    {
        return new LogEntry
        {
            Level = level,
            Message = message,
            Exception = exception?.Message,
            StackTrace = exception?.StackTrace,
            Properties = properties,
            CorrelationId = correlationId,
            Source = _options.ApplicationName,
            Timestamp = DateTimeOffset.UtcNow
        };
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        _flushTimer.Dispose();
        _flushLock.Dispose();

        // Final flush
        FlushAsync(CancellationToken.None).GetAwaiter().GetResult();
    }
}
