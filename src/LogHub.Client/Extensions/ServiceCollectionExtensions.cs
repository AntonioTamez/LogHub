using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;

namespace LogHub.Client.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLogHub(
        this IServiceCollection services,
        Action<LogHubOptions> configure)
    {
        services.Configure(configure);

        // Add HTTP client with retry policy
        services.AddHttpClient<ILogHubClient, LogHubClient>()
            .AddPolicyHandler(GetRetryPolicy());

        // Add logger provider
        services.AddSingleton<ILoggerProvider, LogHubLoggerProvider>();

        return services;
    }

    public static IServiceCollection AddLogHub(
        this IServiceCollection services,
        string apiUrl,
        string apiKey,
        string applicationName)
    {
        return services.AddLogHub(options =>
        {
            options.ApiUrl = apiUrl;
            options.ApiKey = apiKey;
            options.ApplicationName = applicationName;
        });
    }

    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, retryAttempt =>
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }
}
