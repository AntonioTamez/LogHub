using System.Text;
using System.Text.Json;
using LogHub.Core.Constants;
using LogHub.Core.DTOs;
using LogHub.Worker.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace LogHub.Worker.Workers;

public class LogProcessorWorker : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly ILogPersistenceService _persistenceService;
    private readonly ILogger<LogProcessorWorker> _logger;
    private IConnection? _connection;
    private IModel? _channel;

    public LogProcessorWorker(
        IConfiguration configuration,
        ILogPersistenceService persistenceService,
        ILogger<LogProcessorWorker> logger)
    {
        _configuration = configuration;
        _persistenceService = persistenceService;
        _logger = logger;
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        InitializeRabbitMQ();
        _logger.LogInformation("LogProcessorWorker started");
        return base.StartAsync(cancellationToken);
    }

    private void InitializeRabbitMQ()
    {
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQ:Host"] ?? "localhost",
            UserName = _configuration["RabbitMQ:Username"] ?? "guest",
            Password = _configuration["RabbitMQ:Password"] ?? "guest",
            Port = int.Parse(_configuration["RabbitMQ:Port"] ?? "5672"),
            DispatchConsumersAsync = true
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.QueueDeclare(
            queue: QueueNames.LogIngestion,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        _channel.BasicQos(prefetchSize: 0, prefetchCount: 10, global: false);

        _logger.LogInformation("RabbitMQ connection established");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_channel == null)
        {
            _logger.LogError("RabbitMQ channel is not initialized");
            return;
        }

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.Received += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);
                var logMessage = JsonSerializer.Deserialize<LogMessage>(json, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                if (logMessage != null)
                {
                    await _persistenceService.PersistLogAsync(logMessage, stoppingToken);
                    _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }
                else
                {
                    _logger.LogWarning("Failed to deserialize log message");
                    _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing log message");
                _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
            }
        };

        _channel.BasicConsume(
            queue: QueueNames.LogIngestion,
            autoAck: false,
            consumer: consumer);

        _logger.LogInformation("Started consuming from queue: {Queue}", QueueNames.LogIngestion);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _channel?.Close();
        _connection?.Close();
        _logger.LogInformation("LogProcessorWorker stopped");
        return base.StopAsync(cancellationToken);
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
    }
}
