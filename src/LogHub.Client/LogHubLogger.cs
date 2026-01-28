using Microsoft.Extensions.Logging;
using MsLogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace LogHub.Client;

public class LogHubLogger : ILogger
{
    private readonly string _categoryName;
    private readonly ILogHubClient _client;
    private readonly LogHubOptions _options;

    public LogHubLogger(string categoryName, ILogHubClient client, LogHubOptions options)
    {
        _categoryName = categoryName;
        _client = client;
        _options = options;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }

    public bool IsEnabled(MsLogLevel logLevel)
    {
        return MapLogLevel(logLevel) >= _options.MinimumLevel;
    }

    public void Log<TState>(
        MsLogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
            return;

        var message = formatter(state, exception);
        var level = MapLogLevel(logLevel);

        var properties = new Dictionary<string, object>
        {
            ["Category"] = _categoryName,
            ["EventId"] = eventId.Id,
            ["EventName"] = eventId.Name ?? string.Empty
        };

        // Extract state properties if available
        if (state is IReadOnlyList<KeyValuePair<string, object?>> stateProperties)
        {
            foreach (var prop in stateProperties)
            {
                if (prop.Key != "{OriginalFormat}" && prop.Value != null)
                {
                    properties[prop.Key] = prop.Value;
                }
            }
        }

        _client.QueueLog(level, message, exception, properties);
    }

    private static LogLevel MapLogLevel(MsLogLevel level) => level switch
    {
        MsLogLevel.Trace => LogLevel.Trace,
        MsLogLevel.Debug => LogLevel.Debug,
        MsLogLevel.Information => LogLevel.Information,
        MsLogLevel.Warning => LogLevel.Warning,
        MsLogLevel.Error => LogLevel.Error,
        MsLogLevel.Critical => LogLevel.Critical,
        _ => LogLevel.Information
    };
}
