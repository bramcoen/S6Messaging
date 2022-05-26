using MongoDB.Bson.Serialization.Attributes;

namespace Models
{
    [BsonIgnoreExtraElements]
    public class User
    {
        public User()
        {

        }
        public User(string name, string userId)
        {
            Name = name;
            Id = userId;
        }
        public string Name { get; set; }
        public string Id { get; set; }
        public string Email { get; set; }
    }
}