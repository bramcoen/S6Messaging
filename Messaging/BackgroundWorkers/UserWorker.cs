using StorageInterfaces;
using Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Messaging
{
    public class UserWorker : BackgroundService
    {
        ConnectionFactory _connectionFactory;
        IConnection _connection;
        IModel _channel;
        ILogger _logger;
        IUserStorage _userStorage;

        public UserWorker(ILogger<UserWorker> logger, IUserStorage userStorage)
        {
            _logger = logger;
            _userStorage = userStorage;

        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var user = System.Text.Json.JsonSerializer.Deserialize<User>(body);
                    if (user != null) await _userStorage.RegisterOrUpdateUser(user.Name, user.Id);
                    _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };

            _channel.BasicConsume(queue: "message/user",
                                 autoAck: false,
                                 consumer: consumer);

            return Task.CompletedTask;
        }
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _connectionFactory = new ConnectionFactory
            {
                HostName = "localhost",
                Port = 5672,
                UserName = "guest",
                Password = "guest",
                DispatchConsumersAsync = true
            };
            _connection = _connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "message/user",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
            _channel.BasicQos(0, 30,false);
            _logger.LogInformation($"Queue [hello] is waiting for messages.");

            return base.StartAsync(cancellationToken);
        }
    }
}
