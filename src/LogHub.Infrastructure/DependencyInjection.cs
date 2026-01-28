using LogHub.Core.Interfaces;
using LogHub.Infrastructure.Data;
using LogHub.Infrastructure.Redis;
using LogHub.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LogHub.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // PostgreSQL
        services.AddDbContext<LogHubDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(LogHubDbContext).Assembly.FullName)));

        // Redis
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration["Redis:Configuration"];
            options.InstanceName = "LogHub_";
        });

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IApplicationRepository, ApplicationRepository>();
        services.AddScoped<ILogRepository, LogRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Cache service
        services.AddScoped<IRedisCacheService, RedisCacheService>();

        return services;
    }
}
