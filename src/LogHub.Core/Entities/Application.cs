namespace LogHub.Core.Entities;

public class Application : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string ApiKey { get; set; } = Guid.NewGuid().ToString("N");
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    // Foreign key
    public Guid UserId { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public ICollection<LogEntry> LogEntries { get; set; } = new List<LogEntry>();
}
