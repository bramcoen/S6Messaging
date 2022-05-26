using StorageInterfaces;
using Microsoft.Extensions.Configuration;
using Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoDBRepository
{
    public class UserStorage : IUserStorage
    {

        MongoClient _dbClient;
        IMongoDatabase _mongoDatabase;
        IMongoCollection<User> _usersCollection;
        public UserStorage(IConfiguration configuration)
        {
            _dbClient = new MongoClient(configuration.GetValue<string>("MongoDBConnectionString"));
            _mongoDatabase = _dbClient.GetDatabase("S6");
            _usersCollection = _mongoDatabase.GetCollection<User>("Message_Users");
        }

        public async Task DeleteUser(string id)
        {
            await _usersCollection.DeleteOneAsync(i => i.Id == id);
        }

        /*  public async Task<User> SaveUserAsync(User user)
          {
              if (user.Id != null) throw new InvalidOperationException("Could not save user with an user ID");
              user.Id = Guid.NewGuid().ToString();
              if (user.Reference == null || user.Email == null) throw new InvalidDataException("Could not save an user without Reference or Email");
              await _usersCollection.InsertOneAsync(user);
              return user;
          }

          public async Task<Token> UpdateToken(string email, Token token)
          {
              var user = Get(email);
              if (user == null)
              {
                  throw new InvalidOperationException("Could not get token for non existing user");
              }
              var update = Builders<User>.Update.Set(i => i.Token, token.Value).Set(i => i.TokenValidity, token.ValidUntil);
              var result = await _usersCollection.UpdateOneAsync(i => i.Email == email, update);
              return token;

          }*/

        public async Task<User> GetById(string id)
        {
            return await _usersCollection.Find(i => i.Id == id).FirstOrDefaultAsync();
        }

        public async Task<User> GetByUsername(string name)
        {
            return await _usersCollection.Find(i => i.Name == name).FirstOrDefaultAsync();
        }

        public async Task<User> RegisterOrUpdateUser(string name, string userId)
        {
            try
            {
                var user = await GetById(userId);
                if (user == null)
                {
                    user = new User(name, userId);
                    await _usersCollection.InsertOneAsync(user);
                    return user;
                }

                var update = Builders<User>.Update.Set(i => i.Name, name);
                var result = await _usersCollection.UpdateOneAsync(i => i.Id == user.Id, update);
                user.Name = name;
                return user;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}