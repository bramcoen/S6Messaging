using RabbitMQ.Client;
using System.Text;

namespace Messaging.Services
{
    public class RabbitMQService
    {
        private readonly ConnectionFactory _factory;

        public RabbitMQService(IConfiguration configuration)
        {
            _factory = new ConnectionFactory()
            {
                HostName = configuration.GetValue<string>("RabbitMQHostname"),
                UserName = configuration.GetValue<string>("RabbitMQUsername"),
                Password = configuration.GetValue<string>("RabbitMQPassword"),
                Port = 5672
            };
        }
        public void SendMessage(object obj, string exchange, string routingkey)
        {
            using (var connection = _factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                var json = System.Text.Json.JsonSerializer.Serialize(obj);
                var body = Encoding.UTF8.GetBytes(json);
                channel.BasicPublish(exchange: exchange,
                                     routingKey: routingkey,
                                     basicProperties: null,
                                     body: body);
            }
        }
    }
}

