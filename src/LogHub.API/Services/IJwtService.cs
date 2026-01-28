using LogHub.Core.Entities;

namespace LogHub.API.Services;

public interface IJwtService
{
    string GenerateToken(User user);
    (bool isValid, Guid userId) ValidateToken(string token);
}
