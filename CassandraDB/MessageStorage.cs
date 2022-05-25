using DataInterfaces;
using Messaging;
using Cassandra.Mapping;
using Cassandra;

namespace CassandraDB
{
    public class MessageStorage : IMessageStorage
    {
        
        public MessageStorage()
        {
            cluster = Cluster.Builder()
                    .AddContactPoints("cassandra","localhost").WithDefaultKeyspace("Data")
                    .Build();
           
            // Connect to the nodes using a keyspace
            session = cluster.ConnectAndCreateDefaultKeyspaceIfNotExists();
           
            mapper = new Mapper(session);
            mapper.Execute("Create table IF NOT EXISTS Message(Id text, SenderId text, Text text, CreationDate timestamp,Primary key(SenderId, CreationDate));");
        }
        Cluster cluster;
        ISession session;
        IMapper mapper;
  
        public async Task<IEnumerable<Message>> GetAllMessages(string userId, int interval, int page)
        {
            int min = (page - 1) * interval;
            var result = await mapper.FetchAsync<Message>("SELECT * FROM Message where SenderId = ? ORDER BY CreationDate desc", userId);
            return result.Skip(min).Take(interval);
        }

        public async Task<Message> GetMessage(string Id)
        {
            var result = await mapper.FirstAsync<Message>("SELECT * Message where id = ?", Id);
            return result;
        }

        public async Task<Message> SaveMessageAsync(Message message)
        {
            message.Id = Guid.NewGuid().ToString();
            var result = await mapper.InsertIfNotExistsAsync<Message>(message);
            return result.Existing;
        }
    }
}