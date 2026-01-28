using System.Text.Json;
using LogHub.API.DTOs;
using LogHub.API.Hubs;
using LogHub.Core.DTOs;
using LogHub.Core.Entities;
using LogHub.Infrastructure.Redis;
using Microsoft.AspNetCore.SignalR;

namespace LogHub.API.Services;

public class LogIngestionService : ILogIngestionService
{
    private readonly IRabbitMQPublisher _rabbitMQPublisher;
    private readonly IRedisCacheService _cacheService;
    private readonly IHubContext<LogsHub, ILogsHubClient> _hubContext;
    private readonly ILogger<LogIngestionService> _logger;

    public LogIngestionService(
        IRabbitMQPublisher rabbitMQPublisher,
        IRedisCacheService cacheService,
        IHubContext<LogsHub, ILogsHubClient> hubContext,
        ILogger<LogIngestionService> logger)
    {
        _rabbitMQPublisher = rabbitMQPublisher;
        _cacheService = cacheService;
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task<bool> IngestLogAsync(
        Application application,
        CreateLogRequest request,
        CancellationToken cancellationToken = default)
    {
        var logMessage = CreateLogMessage(application.Id, request);

        // Publish to RabbitMQ for persistence
        await _rabbitMQPublisher.PublishLogAsync(logMessage, cancellationToken);

        // Notify connected clients via SignalR
        var logDto = new LogEntryDto(
            Id: Guid.NewGuid(),
            ApplicationId: application.Id,
            ApplicationName: application.Name,
            Level: request.Level,
            Message: request.Message,
            Exception: request.Exception,
            StackTrace: request.StackTrace,
            Properties: request.Properties != null ? JsonSerializer.Serialize(request.Properties) : null,
            CorrelationId: request.CorrelationId,
            Source: request.Source,
            Timestamp: request.Timestamp ?? DateTimeOffset.UtcNow
        );

        await _hubContext.Clients.Group($"app_{application.Id}").ReceiveLog(logDto);
        await _hubContext.Clients.Group("all").ReceiveLog(logDto);

        _logger.LogDebug("Log ingested for application {AppId}: {Level}", application.Id, request.Level);

        return true;
    }

    public async Task<int> IngestLogsAsync(
        Application application,
        IEnumerable<CreateLogRequest> requests,
        CancellationToken cancellationToken = default)
    {
        var logMessages = requests.Select(r => CreateLogMessage(application.Id, r)).ToList();

        // Publish batch to RabbitMQ
        await _rabbitMQPublisher.PublishLogsAsync(logMessages, cancellationToken);

        // Notify connected clients via SignalR
        foreach (var request in requests)
        {
            var logDto = new LogEntryDto(
                Id: Guid.NewGuid(),
                ApplicationId: application.Id,
                ApplicationName: application.Name,
                Level: request.Level,
                Message: request.Message,
                Exception: request.Exception,
                StackTrace: request.StackTrace,
                Properties: request.Properties != null ? JsonSerializer.Serialize(request.Properties) : null,
                CorrelationId: request.CorrelationId,
                Source: request.Source,
                Timestamp: request.Timestamp ?? DateTimeOffset.UtcNow
            );

            await _hubContext.Clients.Group($"app_{application.Id}").ReceiveLog(logDto);
            await _hubContext.Clients.Group("all").ReceiveLog(logDto);
        }

        _logger.LogDebug("Batch of {Count} logs ingested for application {AppId}", logMessages.Count, application.Id);

        return logMessages.Count;
    }

    private static LogMessage CreateLogMessage(Guid applicationId, CreateLogRequest request)
    {
        return new LogMessage
        {
            ApplicationId = applicationId,
            Level = request.Level,
            Message = request.Message,
            Exception = request.Exception,
            StackTrace = request.StackTrace,
            Properties = request.Properties,
            CorrelationId = request.CorrelationId,
            Source = request.Source,
            Timestamp = request.Timestamp ?? DateTimeOffset.UtcNow
        };
    }
}
