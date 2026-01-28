using System.ComponentModel.DataAnnotations;

namespace LogHub.API.DTOs;

public record CreateApplicationRequest(
    [Required][MaxLength(100)] string Name,
    [MaxLength(500)] string? Description
);

public record UpdateApplicationRequest(
    [MaxLength(100)] string? Name,
    [MaxLength(500)] string? Description,
    bool? IsActive
);

public record ApplicationDto(
    Guid Id,
    string Name,
    string ApiKey,
    string? Description,
    bool IsActive,
    DateTimeOffset CreatedAt
);

public record RegenerateApiKeyResponse(
    string NewApiKey
);
