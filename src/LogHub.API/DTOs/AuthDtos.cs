using System.ComponentModel.DataAnnotations;

namespace LogHub.API.DTOs;

public record RegisterRequest(
    [Required][EmailAddress] string Email,
    [Required][MinLength(6)] string Password,
    [Required][MaxLength(100)] string Name
);

public record LoginRequest(
    [Required][EmailAddress] string Email,
    [Required] string Password
);

public record AuthResponse(
    string Token,
    string Email,
    string Name,
    DateTime ExpiresAt
);

public record UserDto(
    Guid Id,
    string Email,
    string Name,
    DateTimeOffset CreatedAt,
    bool IsActive
);
