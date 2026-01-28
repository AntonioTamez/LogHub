using System.Security.Claims;
using LogHub.API.DTOs;
using LogHub.API.Services;
using LogHub.Core.DTOs;
using LogHub.Core.Entities;
using LogHub.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LogsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogIngestionService _logIngestionService;
    private readonly ILogger<LogsController> _logger;

    public LogsController(
        IUnitOfWork unitOfWork,
        ILogIngestionService logIngestionService,
        ILogger<LogsController> logger)
    {
        _unitOfWork = unitOfWork;
        _logIngestionService = logIngestionService;
        _logger = logger;
    }

    private Guid GetUserId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>
    /// Ingest a single log entry (requires API Key)
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> IngestLog([FromBody] CreateLogRequest request)
    {
        var application = HttpContext.Items["Application"] as Application;

        if (application == null)
        {
            return Unauthorized(new { error = "Invalid or missing API Key" });
        }

        await _logIngestionService.IngestLogAsync(application, request);

        return Accepted(new { message = "Log accepted for processing" });
    }

    /// <summary>
    /// Ingest multiple log entries (requires API Key)
    /// </summary>
    [HttpPost("batch")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> IngestLogs([FromBody] BatchLogRequest request)
    {
        var application = HttpContext.Items["Application"] as Application;

        if (application == null)
        {
            return Unauthorized(new { error = "Invalid or missing API Key" });
        }

        var count = await _logIngestionService.IngestLogsAsync(application, request.Logs);

        return Accepted(new { message = $"{count} logs accepted for processing" });
    }

    /// <summary>
    /// Query logs with filters (requires JWT)
    /// </summary>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(PagedResponse<LogEntryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLogs([FromQuery] LogQueryRequest request)
    {
        var userId = GetUserId();

        // Verify user owns the application if filtering by app
        if (request.ApplicationId.HasValue)
        {
            var app = await _unitOfWork.Applications.GetByIdAsync(request.ApplicationId.Value);
            if (app == null || app.UserId != userId)
            {
                return Forbid();
            }
        }

        // Get user's applications for filtering
        var userApps = await _unitOfWork.Applications.GetByUserIdAsync(userId);
        var userAppIds = userApps.Select(a => a.Id).ToHashSet();

        var parameters = new LogQueryParameters
        {
            ApplicationId = request.ApplicationId,
            MinLevel = request.MinLevel,
            MaxLevel = request.MaxLevel,
            SearchText = request.SearchText,
            CorrelationId = request.CorrelationId,
            From = request.From,
            To = request.To,
            Page = request.Page,
            PageSize = Math.Min(request.PageSize, 100), // Max 100 per page
            SortBy = request.SortBy,
            SortDescending = request.SortDescending
        };

        var result = await _unitOfWork.Logs.GetLogsPagedAsync(parameters);

        // Filter to only user's applications
        var filteredItems = result.Items
            .Where(l => userAppIds.Contains(l.ApplicationId))
            .Select(l => new LogEntryDto(
                l.Id,
                l.ApplicationId,
                l.Application?.Name ?? "Unknown",
                l.Level,
                l.Message,
                l.Exception,
                l.StackTrace,
                l.Properties,
                l.CorrelationId,
                l.Source,
                l.Timestamp
            ));

        return Ok(new PagedResponse<LogEntryDto>(
            filteredItems,
            result.TotalCount,
            result.Page,
            result.PageSize,
            result.TotalPages,
            result.HasPreviousPage,
            result.HasNextPage
        ));
    }

    /// <summary>
    /// Get a specific log entry (requires JWT)
    /// </summary>
    [HttpGet("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(LogEntryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var userId = GetUserId();
        var log = await _unitOfWork.Logs.GetByIdAsync(id);

        if (log == null)
        {
            return NotFound(new { error = "Log not found" });
        }

        // Verify user owns the application
        var app = await _unitOfWork.Applications.GetByIdAsync(log.ApplicationId);
        if (app == null || app.UserId != userId)
        {
            return Forbid();
        }

        return Ok(new LogEntryDto(
            log.Id,
            log.ApplicationId,
            app.Name,
            log.Level,
            log.Message,
            log.Exception,
            log.StackTrace,
            log.Properties,
            log.CorrelationId,
            log.Source,
            log.Timestamp
        ));
    }
}
