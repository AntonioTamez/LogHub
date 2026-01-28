using LogHub.Core.Constants;
using LogHub.Core.Interfaces;
using LogHub.Infrastructure.Redis;

namespace LogHub.API.Middleware;

public class ApiKeyAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiKeyAuthMiddleware> _logger;
    private const string ApiKeyHeader = "X-Api-Key";

    public ApiKeyAuthMiddleware(RequestDelegate next, ILogger<ApiKeyAuthMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IUnitOfWork unitOfWork, IRedisCacheService cacheService)
    {
        // Only apply to log ingestion endpoints
        if (!context.Request.Path.StartsWithSegments("/api/logs") ||
            context.Request.Method == "GET")
        {
            await _next(context);
            return;
        }

        // Check for API Key header
        if (!context.Request.Headers.TryGetValue(ApiKeyHeader, out var apiKeyHeader))
        {
            _logger.LogWarning("API Key header missing");
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { error = "API Key is required" });
            return;
        }

        var apiKey = apiKeyHeader.ToString();

        // Try to get application from cache first
        var cacheKey = CacheKeys.GetApplicationByApiKeyKey(apiKey);
        var application = await cacheService.GetAsync<Core.Entities.Application>(cacheKey);

        if (application == null)
        {
            // Get from database
            application = await unitOfWork.Applications.GetByApiKeyAsync(apiKey);

            if (application == null)
            {
                _logger.LogWarning("Invalid API Key: {ApiKey}", apiKey);
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { error = "Invalid API Key" });
                return;
            }

            // Cache for 5 minutes
            await cacheService.SetAsync(cacheKey, application, TimeSpan.FromMinutes(5));
        }

        if (!application.IsActive)
        {
            _logger.LogWarning("Application is inactive: {AppId}", application.Id);
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsJsonAsync(new { error = "Application is inactive" });
            return;
        }

        // Store application in context for use in controllers
        context.Items["Application"] = application;

        await _next(context);
    }
}

public static class ApiKeyAuthMiddlewareExtensions
{
    public static IApplicationBuilder UseApiKeyAuth(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ApiKeyAuthMiddleware>();
    }
}
