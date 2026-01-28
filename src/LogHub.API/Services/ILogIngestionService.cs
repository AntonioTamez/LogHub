using LogHub.API.DTOs;
using LogHub.Core.Entities;

namespace LogHub.API.Services;

public interface ILogIngestionService
{
    Task<bool> IngestLogAsync(Application application, CreateLogRequest request, CancellationToken cancellationToken = default);
    Task<int> IngestLogsAsync(Application application, IEnumerable<CreateLogRequest> requests, CancellationToken cancellationToken = default);
}
