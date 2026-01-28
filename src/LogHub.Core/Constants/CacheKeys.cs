namespace LogHub.Core.Constants;

public static class CacheKeys
{
    public const string RecentLogs = "logs:recent:{0}"; // {applicationId}
    public const string ApplicationByApiKey = "app:apikey:{0}"; // {apiKey}
    public const string LogStats = "stats:{0}:{1}"; // {applicationId}:{date}
    public const string UserById = "user:{0}"; // {userId}

    public static string GetRecentLogsKey(Guid applicationId) =>
        string.Format(RecentLogs, applicationId);

    public static string GetApplicationByApiKeyKey(string apiKey) =>
        string.Format(ApplicationByApiKey, apiKey);

    public static string GetLogStatsKey(Guid? applicationId, DateTimeOffset date) =>
        string.Format(LogStats, applicationId?.ToString() ?? "all", date.ToString("yyyy-MM-dd"));

    public static string GetUserByIdKey(Guid userId) =>
        string.Format(UserById, userId);
}
