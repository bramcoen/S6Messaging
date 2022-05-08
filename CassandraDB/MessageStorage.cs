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
                    .AddContactPoints("cassandra").WithDefaultKeyspace("Messaging")
                    .Build();
           
            // Connect to the nodes using a keyspace
            session = cluster.ConnectAndCreateDefaultKeyspaceIfNotExists();
            mapper = new Mapper(session);
        }
        Cluster cluster;
        ISession session;
        IMapper mapper;

        public async Task<IEnumerable<Message>> GetAllMessages(string userId)
        {
           return await mapper.FetchAsync<Message>("SELECT * FROM Messaging");
        }

        public async Task<Message> GetMessage(string Id)
        {
            var result = await mapper.FirstAsync<Message>("SELECT * FROM Messaging where id = ?", Id);
            return result;
        }

        public async Task<Message> SaveMessageAsync(Message message)
        {
            var result = await mapper.InsertIfNotExistsAsync<Message>(message);
            return result.Existing;
        }
    }
}