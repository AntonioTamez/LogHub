using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace LogHub.API.Hubs;

[Authorize]
public class LogsHub : Hub<ILogsHubClient>
{
    private readonly ILogger<LogsHub> _logger;

    public LogsHub(ILogger<LogsHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "all");
        _logger.LogInformation("Client connected: {ConnectionId}", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, "all");
        _logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SubscribeToApplication(Guid applicationId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"app_{applicationId}");
        _logger.LogInformation("Client {ConnectionId} subscribed to application {AppId}",
            Context.ConnectionId, applicationId);
    }

    public async Task UnsubscribeFromApplication(Guid applicationId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"app_{applicationId}");
        _logger.LogInformation("Client {ConnectionId} unsubscribed from application {AppId}",
            Context.ConnectionId, applicationId);
    }
}
