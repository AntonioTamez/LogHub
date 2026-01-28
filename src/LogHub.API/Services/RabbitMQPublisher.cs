using System.Text;
using System.Text.Json;
using LogHub.Core.Constants;
using LogHub.Core.DTOs;
using RabbitMQ.Client;

namespace LogHub.API.Services;

public class RabbitMQPublisher : IRabbitMQPublisher, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly ILogger<RabbitMQPublisher> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public RabbitMQPublisher(IConfiguration configuration, ILogger<RabbitMQPublisher> logger)
    {
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var factory = new ConnectionFactory
        {
            HostName = configuration["RabbitMQ:Host"] ?? "localhost",
            UserName = configuration["RabbitMQ:Username"] ?? "guest",
            Password = configuration["RabbitMQ:Password"] ?? "guest",
            Port = int.Parse(configuration["RabbitMQ:Port"] ?? "5672")
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        // Declare queues
        _channel.QueueDeclare(
            queue: QueueNames.LogIngestion,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        _channel.QueueDeclare(
            queue: QueueNames.DeadLetter,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        _logger.LogInformation("RabbitMQ Publisher initialized");
    }

    public Task PublishLogAsync(LogMessage logMessage, CancellationToken cancellationToken = default)
    {
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(logMessage, _jsonOptions));

        var properties = _channel.CreateBasicProperties();
        properties.Persistent = true;
        properties.ContentType = "application/json";

        _channel.BasicPublish(
            exchange: "",
            routingKey: QueueNames.LogIngestion,
            basicProperties: properties,
            body: body);

        _logger.LogDebug("Published log to queue: {Level} - {Message}", logMessage.Level, logMessage.Message);

        return Task.CompletedTask;
    }

    public Task PublishLogsAsync(IEnumerable<LogMessage> logMessages, CancellationToken cancellationToken = default)
    {
        var batch = _channel.CreateBasicPublishBatch();
        var properties = _channel.CreateBasicProperties();
        properties.Persistent = true;
        properties.ContentType = "application/json";

        foreach (var logMessage in logMessages)
        {
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(logMessage, _jsonOptions));
            batch.Add(
                exchange: "",
                routingKey: QueueNames.LogIngestion,
                mandatory: false,
                properties: properties,
                body: new ReadOnlyMemory<byte>(body));
        }

        batch.Publish();

        _logger.LogDebug("Published batch of logs to queue");

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _channel?.Close();
        _channel?.Dispose();
        _connection?.Close();
        _connection?.Dispose();
    }
}
