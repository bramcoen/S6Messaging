using Models;

namespace StorageInterfaces
{
    public interface IUserStorage
    {
 
        public Task<User> RegisterOrUpdateUser(string name, string userId);
        public Task<User> GetByUsername(string name);
        public Task<User> GetById(string id);
        public Task DeleteUser(string id);
    }
}