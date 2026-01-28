using System.Security.Claims;
using LogHub.API.DTOs;
using LogHub.Core.Entities;
using LogHub.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ApplicationsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ApplicationsController> _logger;

    public ApplicationsController(IUnitOfWork unitOfWork, ILogger<ApplicationsController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    private Guid GetUserId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ApplicationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var userId = GetUserId();
        var applications = await _unitOfWork.Applications.GetByUserIdAsync(userId);

        var dtos = applications.Select(a => new ApplicationDto(
            a.Id,
            a.Name,
            a.ApiKey,
            a.Description,
            a.IsActive,
            a.CreatedAt
        ));

        return Ok(dtos);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApplicationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var userId = GetUserId();
        var application = await _unitOfWork.Applications.GetByIdAsync(id);

        if (application == null || application.UserId != userId)
        {
            return NotFound(new { error = "Application not found" });
        }

        return Ok(new ApplicationDto(
            application.Id,
            application.Name,
            application.ApiKey,
            application.Description,
            application.IsActive,
            application.CreatedAt
        ));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApplicationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateApplicationRequest request)
    {
        var userId = GetUserId();

        var application = new Application
        {
            Name = request.Name,
            Description = request.Description,
            UserId = userId,
            IsActive = true
        };

        await _unitOfWork.Applications.AddAsync(application);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Application created: {AppName} by user {UserId}", application.Name, userId);

        var dto = new ApplicationDto(
            application.Id,
            application.Name,
            application.ApiKey,
            application.Description,
            application.IsActive,
            application.CreatedAt
        );

        return CreatedAtAction(nameof(GetById), new { id = application.Id }, dto);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApplicationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateApplicationRequest request)
    {
        var userId = GetUserId();
        var application = await _unitOfWork.Applications.GetByIdAsync(id);

        if (application == null || application.UserId != userId)
        {
            return NotFound(new { error = "Application not found" });
        }

        if (!string.IsNullOrWhiteSpace(request.Name))
            application.Name = request.Name;

        if (request.Description != null)
            application.Description = request.Description;

        if (request.IsActive.HasValue)
            application.IsActive = request.IsActive.Value;

        await _unitOfWork.Applications.UpdateAsync(application);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Application updated: {AppId}", application.Id);

        return Ok(new ApplicationDto(
            application.Id,
            application.Name,
            application.ApiKey,
            application.Description,
            application.IsActive,
            application.CreatedAt
        ));
    }

    [HttpPost("{id:guid}/regenerate-key")]
    [ProducesResponseType(typeof(RegenerateApiKeyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RegenerateApiKey(Guid id)
    {
        var userId = GetUserId();
        var application = await _unitOfWork.Applications.GetByIdAsync(id);

        if (application == null || application.UserId != userId)
        {
            return NotFound(new { error = "Application not found" });
        }

        application.ApiKey = Guid.NewGuid().ToString("N");

        await _unitOfWork.Applications.UpdateAsync(application);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("API Key regenerated for application: {AppId}", application.Id);

        return Ok(new RegenerateApiKeyResponse(application.ApiKey));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = GetUserId();
        var application = await _unitOfWork.Applications.GetByIdAsync(id);

        if (application == null || application.UserId != userId)
        {
            return NotFound(new { error = "Application not found" });
        }

        await _unitOfWork.Applications.DeleteAsync(application);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Application deleted: {AppId}", application.Id);

        return NoContent();
    }
}
