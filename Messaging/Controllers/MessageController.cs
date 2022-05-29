using StorageInterfaces;
using Messaging;
using Microsoft.AspNetCore.Mvc;
using System;
using Google.Apis.Auth;
using static Google.Apis.Auth.GoogleJsonWebSignature;

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
        public async Task<IEnumerable<Message>> GetAsync(string username, int amount, int page)
        {
            var user = await _userStorage.GetByUsername(username);
            return await _messageStorage.GetAllMessages(user.Id, amount, page);
        }
        [HttpPost]
        public async Task<Message> SendMessage(string token, [FromBody] string content)
        {
            Payload? payload = await GoogleJsonWebSignature.ValidateAsync(token, _validationSettings);

            if (token == null) throw new Exception("Call can't be done while the user is not logged in.");

            var sender = await _userStorage.GetByEmail(payload.Email);
            return await _messageStorage.SaveMessageAsync(new Message() { Text = content, SenderId = sender.Id });
        }
    }
}