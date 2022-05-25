using Messaging;

namespace StorageInterfaces
{
    public interface IMessageStorage
    {
        public Task<IEnumerable<Message>> GetAllMessages(string userId, int interval, int page);
        public Task<Message> GetMessage(string Id);
        public Task<Message> SaveMessageAsync(Message message);
    }
}