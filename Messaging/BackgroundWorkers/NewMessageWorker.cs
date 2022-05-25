using StorageInterfaces;
using Messaging.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Messaging
{
    public class NewMessageWorker : BackgroundService
    {
        private ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _channel;
        private readonly ILogger _logger;
        private readonly IMessageStorage _messageStorage;
        private readonly IConfiguration _configuration;

        public NewMessageWorker(ILogger<NewMessageWorker> logger, IMessageStorage messageStorage, IConfiguration configuration)
        {
            _logger = logger;
            _messageStorage = messageStorage;
            _configuration = configuration;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = System.Text.Json.JsonSerializer.Deserialize<Message>(body);
                    if (message != null) await _messageStorage.SaveMessageAsync(message);
                    _logger.LogInformation(" [x] Received {0}", message!.Text);
                    _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };

            _channel.BasicConsume(queue: "message/new",
                                 autoAck: false,
                                 consumer: consumer);

            return Task.CompletedTask;
        }
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _connectionFactory = new ConnectionFactory()
            {
                HostName =
                _configuration.GetValue<string>("RabbitMQHostname"),
                UserName = _configuration.GetValue<string>("RabbitMQUsername"),
                Password = _configuration.GetValue<string>("RabbitMQPassword"),
                Port = 5672,
                DispatchConsumersAsync = true
            };
            _connection = _connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "message/new",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
            _channel.BasicQos(0, 30, false);
            _logger.LogInformation($"Queue [message/new] is waiting for messages.");

            return base.StartAsync(cancellationToken);
        }
    }
}
