using StorageInterfaces;
using Messaging;
using Microsoft.AspNetCore.Mvc;
using System;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly ILogger<MessageController> _logger;
        private readonly IMessageStorage _messageStorage;

        public MessageController(ILogger<MessageController> logger, IMessageStorage messageStorage)
        {
            _logger = logger;
            _messageStorage = messageStorage;
        }

        [HttpGet]
        public async Task<IEnumerable<Message>> GetAsync(string userid, int amount,int page)
        {
            return await _messageStorage.GetAllMessages(userid, amount,page);
        }
    }
}