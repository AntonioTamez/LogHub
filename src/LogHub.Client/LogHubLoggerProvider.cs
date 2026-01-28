using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LogHub.Client;

public class LogHubLoggerProvider : ILoggerProvider
{
    private readonly ILogHubClient _client;
    private readonly LogHubOptions _options;
    private readonly ConcurrentDictionary<string, LogHubLogger> _loggers = new();

    public LogHubLoggerProvider(ILogHubClient client, IOptions<LogHubOptions> options)
    {
        _client = client;
        _options = options.Value;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return _loggers.GetOrAdd(categoryName, name => new LogHubLogger(name, _client, _options));
    }

    public void Dispose()
    {
        _loggers.Clear();
    }
}
