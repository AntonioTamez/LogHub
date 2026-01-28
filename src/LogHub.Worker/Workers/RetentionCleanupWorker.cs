using LogHub.Core.Interfaces;

namespace LogHub.Worker.Workers;

public class RetentionCleanupWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<RetentionCleanupWorker> _logger;
    private readonly TimeSpan _cleanupInterval = TimeSpan.FromHours(1);

    public RetentionCleanupWorker(
        IServiceScopeFactory scopeFactory,
        IConfiguration configuration,
        ILogger<RetentionCleanupWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _configuration = configuration;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("RetentionCleanupWorker started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CleanupOldLogsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during log cleanup");
            }

            await Task.Delay(_cleanupInterval, stoppingToken);
        }
    }

    private async Task CleanupOldLogsAsync(CancellationToken cancellationToken)
    {
        var retentionDays = int.Parse(_configuration["LogRetention:Days"] ?? "30");

        _logger.LogInformation("Starting log cleanup for logs older than {Days} days", retentionDays);

        using var scope = _scopeFactory.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var deletedCount = await unitOfWork.Logs.DeleteOldLogsAsync(retentionDays, cancellationToken);

        if (deletedCount > 0)
        {
            _logger.LogInformation("Deleted {Count} old log entries", deletedCount);
        }
        else
        {
            _logger.LogDebug("No old logs to delete");
        }
    }
}
