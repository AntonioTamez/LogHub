using LogHub.Core.Entities;

namespace LogHub.Core.Interfaces;

public interface IApplicationRepository : IRepository<Application>
{
    Task<Application?> GetByApiKeyAsync(string apiKey, CancellationToken cancellationToken = default);
    Task<IEnumerable<Application>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> ApiKeyExistsAsync(string apiKey, CancellationToken cancellationToken = default);
}
