using System.Security.Claims;
using LogHub.API.DTOs;
using LogHub.Core.DTOs;
using LogHub.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(IUnitOfWork unitOfWork, ILogger<DashboardController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    private Guid GetUserId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>
    /// Get dashboard statistics
    /// </summary>
    [HttpGet("stats")]
    [ProducesResponseType(typeof(DashboardStatsDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStats([FromQuery] DashboardQueryRequest request)
    {
        var userId = GetUserId();
        var userApps = await _unitOfWork.Applications.GetByUserIdAsync(userId);
        var userAppsList = userApps.ToList();

        // Verify user owns the application if filtering by app
        if (request.ApplicationId.HasValue)
        {
            var app = userAppsList.FirstOrDefault(a => a.Id == request.ApplicationId.Value);
            if (app == null)
            {
                return Forbid();
            }
        }

        // Set default date range if not provided
        var from = request.From ?? DateTimeOffset.UtcNow.AddDays(-7);
        var to = request.To ?? DateTimeOffset.UtcNow;

        LogStats stats;
        if (request.ApplicationId.HasValue)
        {
            // Filter by specific application
            stats = await _unitOfWork.Logs.GetStatsAsync(
                request.ApplicationId,
                from,
                to
            );
        }
        else
        {
            // Filter by all user's applications
            var applicationIds = userAppsList.Select(a => a.Id).ToList();
            if (applicationIds.Any())
            {
                stats = await _unitOfWork.Logs.GetStatsByApplicationIdsAsync(applicationIds, from, to);
            }
            else
            {
                stats = new LogStats();
            }
        }

        return Ok(new DashboardStatsDto(
            stats.TotalLogs,
            stats.TraceCount,
            stats.DebugCount,
            stats.InformationCount,
            stats.WarningCount,
            stats.ErrorCount,
            stats.CriticalCount,
            stats.LogsByApplication,
            stats.LogsByHour,
            stats.LogsByDay
        ));
    }

    /// <summary>
    /// Get summary for dashboard overview
    /// </summary>
    [HttpGet("summary")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSummary()
    {
        var userId = GetUserId();
        var userApps = await _unitOfWork.Applications.GetByUserIdAsync(userId);
        var userAppsList = userApps.ToList();

        var totalApplications = userAppsList.Count;
        var activeApplications = userAppsList.Count(a => a.IsActive);

        // Get stats for last 24 hours, filtered by user's applications
        var from = DateTimeOffset.UtcNow.AddHours(-24);
        var applicationIds = userAppsList.Select(a => a.Id).ToList();

        LogStats stats;
        if (applicationIds.Any())
        {
            stats = await _unitOfWork.Logs.GetStatsByApplicationIdsAsync(applicationIds, from: from);
        }
        else
        {
            stats = new LogStats();
        }

        return Ok(new
        {
            TotalApplications = totalApplications,
            ActiveApplications = activeApplications,
            LogsLast24Hours = stats.TotalLogs,
            ErrorsLast24Hours = stats.ErrorCount + stats.CriticalCount,
            WarningsLast24Hours = stats.WarningCount
        });
    }
}
