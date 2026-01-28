using LogHub.API.DTOs;

namespace LogHub.API.Hubs;

public interface ILogsHubClient
{
    Task ReceiveLog(LogEntryDto log);
    Task ReceiveStats(DashboardStatsDto stats);
    Task ReceiveAlert(string message);
}
