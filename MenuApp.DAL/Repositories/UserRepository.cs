using MenuApp.DAL.Models;
using MenuApp.DAL.DataBaseContext;
using MongoDB.Driver;
using System.Collections.Generic;

namespace MenuApp.DAL.Repositories
{
    public interface IUsersRepository
    {
        IEnumerable<Users> GetUsers();
        void AddUser(Users user);
    }

    public class UserRepository : IUsersRepository
    {
        private readonly IMongoCollection<Users> _collection;

        public UserRepository(DBContext context)
        {
            _collection = context.GetCollection<Users>();
        }

        public IEnumerable<Users> GetUsers()
        {
            return _collection.Find(entity => true).ToList();
        }

        public void AddUser(Users user)
        {
            _collection.InsertOne(user);
        }
    }
}
