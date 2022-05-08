using DataInterfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Messaging
{
    public class NewMessageWorker : BackgroundService
    {
        ConnectionFactory _connectionFactory;
        IConnection _connection;
        IModel _channel;
        ILogger _logger;
        IMessageStorage _messageStorage;

        public NewMessageWorker(ILogger<NewMessageWorker> logger, IMessageStorage messageStorage)
        {
            _logger = logger;
            _messageStorage = messageStorage;
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
                };

            _channel.BasicConsume(queue: "message/new",
                                 autoAck: true,
                                 consumer: consumer);

            return Task.CompletedTask;
        }
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _connectionFactory = new ConnectionFactory
            {
                HostName = "rabbit",
                Port = 5672,
                UserName = "guest",
                Password = "guest",
                DispatchConsumersAsync = true
            };
            _connection = _connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "message/new",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
            _channel.BasicQos(0, 1, false);
            _logger.LogInformation($"Queue [hello] is waiting for messages.");

            return base.StartAsync(cancellationToken);
        }
    }
}
