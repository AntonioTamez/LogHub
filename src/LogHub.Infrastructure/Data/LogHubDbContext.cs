using LogHub.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace LogHub.Infrastructure.Data;

public class LogHubDbContext : DbContext
{
    public LogHubDbContext(DbContextOptions<LogHubDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Application> Applications => Set<Application>();
    public DbSet<LogEntry> LogEntries => Set<LogEntry>();
    public DbSet<AlertRule> AlertRules => Set<AlertRule>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Email).HasMaxLength(256).IsRequired();
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
        });

        // Application configuration
        modelBuilder.Entity<Application>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ApiKey).IsUnique();
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.ApiKey).HasMaxLength(64).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(500);

            entity.HasOne(e => e.User)
                .WithMany(u => u.Applications)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // LogEntry configuration
        modelBuilder.Entity<LogEntry>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ApplicationId);
            entity.HasIndex(e => e.Timestamp);
            entity.HasIndex(e => e.Level);
            entity.HasIndex(e => e.CorrelationId);
            entity.HasIndex(e => new { e.ApplicationId, e.Timestamp });
            entity.HasIndex(e => new { e.ApplicationId, e.Level, e.Timestamp });

            entity.Property(e => e.Message).IsRequired();
            entity.Property(e => e.Properties).HasColumnType("jsonb");
            entity.Property(e => e.Source).HasMaxLength(256);
            entity.Property(e => e.CorrelationId).HasMaxLength(64);

            entity.HasOne(e => e.Application)
                .WithMany(a => a.LogEntries)
                .HasForeignKey(e => e.ApplicationId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // AlertRule configuration
        modelBuilder.Entity<AlertRule>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ApplicationId);
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.MessageContains).HasMaxLength(500);
            entity.Property(e => e.NotificationEmail).HasMaxLength(256);
            entity.Property(e => e.WebhookUrl).HasMaxLength(500);

            entity.HasOne(e => e.Application)
                .WithMany()
                .HasForeignKey(e => e.ApplicationId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
