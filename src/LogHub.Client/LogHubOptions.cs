namespace LogHub.Client;

public class LogHubOptions
{
    public string ApiUrl { get; set; } = "http://localhost:5100";
    public string ApiKey { get; set; } = string.Empty;
    public string ApplicationName { get; set; } = "Unknown";
    public int BatchSize { get; set; } = 100;
    public TimeSpan FlushInterval { get; set; } = TimeSpan.FromSeconds(5);
    public int MaxRetries { get; set; } = 3;
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(1);
    public bool IncludeScopes { get; set; } = true;
    public LogLevel MinimumLevel { get; set; } = LogLevel.Information;
}

public enum LogLevel
{
    Trace = 0,
    Debug = 1,
    Information = 2,
    Warning = 3,
    Error = 4,
    Critical = 5
}
