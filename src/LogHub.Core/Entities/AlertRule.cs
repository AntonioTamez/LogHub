using LogHub.Core.Enums;

namespace LogHub.Core.Entities;

public class AlertRule : BaseEntity
{
    public Guid ApplicationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public LogLevel MinLevel { get; set; } = LogLevel.Error;
    public string? MessageContains { get; set; }
    public int ThresholdCount { get; set; } = 1;
    public int ThresholdMinutes { get; set; } = 5;
    public string? NotificationEmail { get; set; }
    public string? WebhookUrl { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation property
    public Application Application { get; set; } = null!;
}
