using DataInterfaces;
using Messaging;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessageController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<MessageController> _logger;
        private readonly IMessageStorage _messageStorage;

        public MessageController(ILogger<MessageController> logger, IMessageStorage messageStorage)
        {
            _logger = logger;
            _messageStorage = messageStorage;
        }

        [HttpGet]
        public async Task<IEnumerable<Message>> GetAsync()
        {
            return await _messageStorage.GetAllMessages("");
        }
    }
}