using Messaging;

namespace DataInterfaces
{
    public interface IMessageStorage
    {
        public Task<IEnumerable<Message>> GetAllMessages(string userId);
        public Task<Message> GetMessage(string Id);
        public Task<Message> SaveMessageAsync(Message message);
    }
}