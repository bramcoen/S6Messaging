using StorageInterfaces;
using Messaging;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;

namespace MongoDBRepository
{
    public class MessageStorage : IMessageStorage
    {
        MongoClient _dbClient;
        IMongoDatabase _mongoDatabase;
        IMongoCollection<Message> _messageCollection;
        public MessageStorage(IConfiguration configuration)
        {
            _dbClient = new MongoClient(configuration.GetValue<string>("MongoDBConnectionString"));
            _mongoDatabase = _dbClient.GetDatabase("S6");
            _messageCollection = _mongoDatabase.GetCollection<Message>("Messaging_Users");
        }
        public async Task<IEnumerable<Message>> GetAllMessages(string userId, int interval, int page)
        {
            return await _messageCollection.Find(i => i.SenderId == userId).ToListAsync();
        }

        public Task<Message> GetMessage(string Id)
        {
            throw new NotImplementedException();
        }

        public Task<Message> SaveMessageAsync(Message message)
        {
            throw new NotImplementedException();
        }
    }
}