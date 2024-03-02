using System.Collections.Generic;
using MenuApp.DAL.DataBaseContext;
using MenuApp.DAL.Models;
using MongoDB.Driver;

namespace MenuApp.DAL.Repositories
{
    public interface IUsersRepository
    {
        IEnumerable<Users> GetUsers();
        void AddUser(Users user);
        Users GetUserByEmailOrUsesrname(string userName, string email);
        Users GetUserByRefreshToken(string refreshToken);
        void UpdateUserRefreshToken(Users user);
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

        public Users GetUserByEmailOrUsesrname(string userName, string email)
        {
            return _collection
                .Find(e => e.Username == userName || e.Email == email)
                .FirstOrDefault();
        }

        public Users GetUserByRefreshToken(string refreshToken)
        {
            return _collection.Find(e => e.RefreshToken == refreshToken).FirstOrDefault();
        }

        public void UpdateUserRefreshToken(Users user)
        {
            var filter = Builders<Users>.Filter.Eq(u => u.Id, user.Id);
            var update = Builders<Users>.Update.Set(u => u.RefreshToken, user.RefreshToken);

            _collection.UpdateOne(filter, update);
        }
    }
}
