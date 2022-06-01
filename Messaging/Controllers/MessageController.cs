using StorageInterfaces;
using Messaging;
using Microsoft.AspNetCore.Mvc;
using System;
using Google.Apis.Auth;
using static Google.Apis.Auth.GoogleJsonWebSignature;
using Microsoft.AspNetCore.Cors;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("")]
    [EnableCors("default")]
    public class MessageController : ControllerBase
    {
        private readonly ValidationSettings _validationSettings;
        private readonly ILogger<MessageController> _logger;
        private readonly IMessageStorage _messageStorage;
        private readonly IUserStorage _userStorage;
        public MessageController(ILogger<MessageController> logger, IMessageStorage messageStorage, IUserStorage userStorage, IConfiguration configuration)
        {
            _logger = logger;
            _messageStorage = messageStorage;
            _userStorage = userStorage;
            _validationSettings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new string[] { configuration["GOOGLE_CLIENT_ID"] }
            };
        }

        [HttpGet]
        public async Task<IEnumerable<Message>> GetAsync(string username, int amount = 100, int page = 1)
        {
            var user = await _userStorage.GetByUsername(username);
            if(user == null)
            {
                return new List<Message>();
            }
            return await _messageStorage.GetAllMessages(user.Id, amount, page);
        }
        [HttpPost("send")]
        public async Task<Message> SendMessage([FromHeader]string token, [FromBody] Message msg)
        {
            Payload? payload = await GoogleJsonWebSignature.ValidateAsync(token, _validationSettings);

            if (token == null) throw new Exception("Call can't be done while the user is not logged in.");

            var sender = await _userStorage.GetByEmail(payload.Email);
            msg.SenderId = sender.Id;
            return await _messageStorage.SaveMessageAsync(msg);
        }
    }
}