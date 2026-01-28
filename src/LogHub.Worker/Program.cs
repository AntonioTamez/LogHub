using LogHub.Infrastructure;
using LogHub.Worker.Services;
using LogHub.Worker.Workers;

var builder = Host.CreateApplicationBuilder(args);

// Add Infrastructure services (DbContext, Repositories, Redis)
builder.Services.AddInfrastructure(builder.Configuration);

// Add Worker services
builder.Services.AddScoped<ILogPersistenceService, LogPersistenceService>();

// Add Hosted Services
builder.Services.AddHostedService<LogProcessorWorker>();
builder.Services.AddHostedService<RetentionCleanupWorker>();

var host = builder.Build();
host.Run();
