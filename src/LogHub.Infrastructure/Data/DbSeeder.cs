using LogHub.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using LogLevel = LogHub.Core.Enums.LogLevel;

namespace LogHub.Infrastructure.Data;

public class DbSeeder
{
    private readonly LogHubDbContext _context;
    private readonly ILogger<DbSeeder> _logger;

    public DbSeeder(LogHubDbContext context, ILogger<DbSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        try
        {
            await SeedUsersAsync();
            await SeedApplicationsAsync();
            await SeedAlertRulesAsync();
            await SeedLogEntriesAsync();

            _logger.LogInformation("Database seeding completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }

    private async Task SeedUsersAsync()
    {
        if (await _context.Users.AnyAsync())
        {
            _logger.LogInformation("Users already seeded, skipping...");
            return;
        }

        var users = new List<User>
        {
            new()
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Email = "admin@loghub.com",
                Name = "Admin User",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                IsActive = true,
                CreatedAt = DateTimeOffset.UtcNow.AddDays(-30)
            },
            new()
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Email = "developer@loghub.com",
                Name = "Developer User",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Dev123!"),
                IsActive = true,
                CreatedAt = DateTimeOffset.UtcNow.AddDays(-15)
            },
            new()
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Email = "demo@loghub.com",
                Name = "Demo User",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Demo123!"),
                IsActive = true,
                CreatedAt = DateTimeOffset.UtcNow.AddDays(-7)
            }
        };

        await _context.Users.AddRangeAsync(users);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Seeded {Count} users", users.Count);
    }

    private async Task SeedApplicationsAsync()
    {
        if (await _context.Applications.AnyAsync())
        {
            _logger.LogInformation("Applications already seeded, skipping...");
            return;
        }

        var applications = new List<Application>
        {
            // Admin's applications
            new()
            {
                Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                Name = "E-Commerce API",
                Description = "Main e-commerce backend service",
                ApiKey = "ecom-api-key-12345678901234567890123456789012",
                UserId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                IsActive = true,
                CreatedAt = DateTimeOffset.UtcNow.AddDays(-28)
            },
            new()
            {
                Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                Name = "Payment Service",
                Description = "Payment processing microservice",
                ApiKey = "payment-api-key-12345678901234567890123456",
                UserId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                IsActive = true,
                CreatedAt = DateTimeOffset.UtcNow.AddDays(-25)
            },
            // Developer's applications
            new()
            {
                Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                Name = "Mobile App Backend",
                Description = "API for mobile applications",
                ApiKey = "mobile-api-key-12345678901234567890123456",
                UserId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                IsActive = true,
                CreatedAt = DateTimeOffset.UtcNow.AddDays(-14)
            },
            new()
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                Name = "Notification Service",
                Description = "Push notifications and email service",
                ApiKey = "notif-api-key-123456789012345678901234567",
                UserId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                IsActive = true,
                CreatedAt = DateTimeOffset.UtcNow.AddDays(-10)
            },
            // Demo user's application
            new()
            {
                Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                Name = "Demo Application",
                Description = "Demo application for testing",
                ApiKey = "demo-api-key-1234567890123456789012345678",
                UserId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                IsActive = true,
                CreatedAt = DateTimeOffset.UtcNow.AddDays(-5)
            }
        };

        await _context.Applications.AddRangeAsync(applications);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Seeded {Count} applications", applications.Count);
    }

    private async Task SeedAlertRulesAsync()
    {
        if (await _context.AlertRules.AnyAsync())
        {
            _logger.LogInformation("Alert rules already seeded, skipping...");
            return;
        }

        var alertRules = new List<AlertRule>
        {
            new()
            {
                Id = Guid.NewGuid(),
                ApplicationId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                Name = "Critical Errors Alert",
                MinLevel = LogLevel.Critical,
                ThresholdCount = 1,
                ThresholdMinutes = 1,
                NotificationEmail = "admin@loghub.com",
                IsActive = true,
                CreatedAt = DateTimeOffset.UtcNow.AddDays(-20)
            },
            new()
            {
                Id = Guid.NewGuid(),
                ApplicationId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                Name = "High Error Rate",
                MinLevel = LogLevel.Error,
                ThresholdCount = 10,
                ThresholdMinutes = 5,
                NotificationEmail = "admin@loghub.com",
                WebhookUrl = "https://hooks.slack.com/services/xxx/yyy/zzz",
                IsActive = true,
                CreatedAt = DateTimeOffset.UtcNow.AddDays(-18)
            },
            new()
            {
                Id = Guid.NewGuid(),
                ApplicationId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                Name = "Payment Failures",
                MinLevel = LogLevel.Error,
                MessageContains = "payment",
                ThresholdCount = 3,
                ThresholdMinutes = 10,
                NotificationEmail = "payments@loghub.com",
                IsActive = true,
                CreatedAt = DateTimeOffset.UtcNow.AddDays(-15)
            },
            new()
            {
                Id = Guid.NewGuid(),
                ApplicationId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                Name = "Mobile API Errors",
                MinLevel = LogLevel.Error,
                ThresholdCount = 5,
                ThresholdMinutes = 15,
                NotificationEmail = "mobile-team@loghub.com",
                IsActive = true,
                CreatedAt = DateTimeOffset.UtcNow.AddDays(-10)
            }
        };

        await _context.AlertRules.AddRangeAsync(alertRules);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Seeded {Count} alert rules", alertRules.Count);
    }

    private async Task SeedLogEntriesAsync()
    {
        if (await _context.LogEntries.AnyAsync())
        {
            _logger.LogInformation("Log entries already seeded, skipping...");
            return;
        }

        var random = new Random(42); // Fixed seed for reproducibility
        var logEntries = new List<LogEntry>();
        var applicationIds = new[]
        {
            Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
            Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
            Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
            Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee")
        };

        var messages = new Dictionary<LogLevel, string[]>
        {
            [LogLevel.Trace] = new[]
            {
                "Entering method ProcessRequest",
                "Variable value: count=42",
                "Cache lookup for key: user_session_123",
                "SQL query executed in 2ms",
                "HTTP request headers logged"
            },
            [LogLevel.Debug] = new[]
            {
                "Processing user request for userId: 12345",
                "Cache hit for product catalog",
                "Database connection pool status: 5/10 active",
                "Request validation passed",
                "Starting background job: EmailNotification"
            },
            [LogLevel.Information] = new[]
            {
                "User logged in successfully",
                "Order created with ID: ORD-2024-001234",
                "Payment processed successfully for $99.99",
                "Email sent to customer@example.com",
                "API rate limit: 450/1000 requests used",
                "New user registered: user@example.com",
                "Product added to cart: SKU-12345",
                "Scheduled task completed: DataSync"
            },
            [LogLevel.Warning] = new[]
            {
                "High memory usage detected: 85%",
                "API rate limit approaching: 900/1000",
                "Slow database query detected (>1000ms)",
                "Deprecated API endpoint called: /api/v1/legacy",
                "Retry attempt 2/3 for external service",
                "User session about to expire in 5 minutes"
            },
            [LogLevel.Error] = new[]
            {
                "Failed to process payment: Card declined",
                "Database connection timeout after 30s",
                "External API returned 500 error",
                "Failed to send email: SMTP connection refused",
                "Invalid request data: Missing required field 'email'",
                "Authentication failed for user: invalid credentials"
            },
            [LogLevel.Critical] = new[]
            {
                "Database server unreachable",
                "Out of memory exception",
                "SSL certificate expired",
                "Payment gateway unavailable",
                "Data corruption detected in user table"
            }
        };

        var sources = new[] { "OrderService", "PaymentProcessor", "UserController", "AuthMiddleware", "CacheManager", "EmailService", "ApiGateway" };
        var correlationIds = Enumerable.Range(1, 20).Select(_ => Guid.NewGuid().ToString("N")[..16]).ToArray();

        // Generate logs for the past 7 days
        for (int day = 0; day < 7; day++)
        {
            var baseDate = DateTimeOffset.UtcNow.AddDays(-day);
            var logsPerDay = day == 0 ? 50 : random.Next(30, 80); // More logs today

            for (int i = 0; i < logsPerDay; i++)
            {
                var level = GetRandomLogLevel(random);
                var appId = applicationIds[random.Next(applicationIds.Length)];
                var messageOptions = messages[level];
                var message = messageOptions[random.Next(messageOptions.Length)];
                var timestamp = baseDate.AddHours(-random.Next(0, 24)).AddMinutes(-random.Next(0, 60));

                var logEntry = new LogEntry
                {
                    Id = Guid.NewGuid(),
                    ApplicationId = appId,
                    Level = level,
                    Message = message,
                    Source = sources[random.Next(sources.Length)],
                    CorrelationId = random.Next(100) < 30 ? correlationIds[random.Next(correlationIds.Length)] : null,
                    Timestamp = timestamp,
                    CreatedAt = timestamp
                };

                // Add exception info for errors and critical
                if (level >= LogLevel.Error && random.Next(100) < 70)
                {
                    logEntry.Exception = GetSampleException(level, random);
                    logEntry.StackTrace = GetSampleStackTrace();
                }

                // Add properties for some logs
                if (random.Next(100) < 40)
                {
                    logEntry.Properties = GetSampleProperties(random);
                }

                logEntries.Add(logEntry);
            }
        }

        // Batch insert for better performance
        const int batchSize = 100;
        for (int i = 0; i < logEntries.Count; i += batchSize)
        {
            var batch = logEntries.Skip(i).Take(batchSize);
            await _context.LogEntries.AddRangeAsync(batch);
            await _context.SaveChangesAsync();
        }

        _logger.LogInformation("Seeded {Count} log entries", logEntries.Count);
    }

    private static LogLevel GetRandomLogLevel(Random random)
    {
        // Weighted distribution: more info/debug, fewer errors/critical
        var value = random.Next(100);
        return value switch
        {
            < 10 => LogLevel.Trace,
            < 30 => LogLevel.Debug,
            < 65 => LogLevel.Information,
            < 85 => LogLevel.Warning,
            < 97 => LogLevel.Error,
            _ => LogLevel.Critical
        };
    }

    private static string GetSampleException(LogLevel level, Random random)
    {
        var exceptions = new[]
        {
            "System.NullReferenceException: Object reference not set to an instance of an object.",
            "System.InvalidOperationException: Sequence contains no elements.",
            "System.TimeoutException: The operation has timed out.",
            "System.Net.Http.HttpRequestException: Connection refused.",
            "Npgsql.PostgresException: 23505: duplicate key value violates unique constraint.",
            "System.UnauthorizedAccessException: Access to the path is denied.",
            "System.IO.FileNotFoundException: Could not find file 'config.json'.",
            "System.ArgumentException: Value cannot be null or empty."
        };
        return exceptions[random.Next(exceptions.Length)];
    }

    private static string GetSampleStackTrace()
    {
        return @"   at LogHub.Services.OrderService.ProcessOrder(Order order) in /src/Services/OrderService.cs:line 145
   at LogHub.Controllers.OrderController.CreateOrder(CreateOrderRequest request) in /src/Controllers/OrderController.cs:line 78
   at Microsoft.AspNetCore.Mvc.Infrastructure.ActionMethodExecutor.Execute()
   at Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker.InvokeActionMethodAsync()";
    }

    private static string GetSampleProperties(Random random)
    {
        var properties = new[]
        {
            "{\"userId\": 12345, \"action\": \"login\", \"ip\": \"192.168.1.100\"}",
            "{\"orderId\": \"ORD-2024-001234\", \"amount\": 99.99, \"currency\": \"USD\"}",
            "{\"requestId\": \"req-abc123\", \"duration_ms\": 245, \"status\": 200}",
            "{\"productId\": \"SKU-12345\", \"quantity\": 2, \"cartId\": \"cart-xyz789\"}",
            "{\"emailType\": \"order_confirmation\", \"recipient\": \"user@example.com\"}",
            "{\"cacheKey\": \"product_list_page_1\", \"hit\": true, \"ttl\": 3600}"
        };
        return properties[random.Next(properties.Length)];
    }
}
