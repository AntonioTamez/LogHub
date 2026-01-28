using LogHub.Core.Entities;
using LogHub.Core.Interfaces;
using LogHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LogHub.Infrastructure.Repositories;

public class ApplicationRepository : Repository<Application>, IApplicationRepository
{
    public ApplicationRepository(LogHubDbContext context) : base(context)
    {
    }

    public async Task<Application?> GetByApiKeyAsync(string apiKey, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.ApiKey == apiKey && a.IsActive, cancellationToken);
    }

    public async Task<IEnumerable<Application>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ApiKeyExistsAsync(string apiKey, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(a => a.ApiKey == apiKey, cancellationToken);
    }
}
