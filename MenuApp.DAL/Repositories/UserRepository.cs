using System.Collections.Generic;
using System.Threading.Tasks;
using MenuApp.DAL.DataBaseContext;
using MenuApp.DAL.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MenuApp.DAL.Repositories
{
    public interface IUsersRepository
    {
        Task<IEnumerable<Users>> GetUsers();
        Task AddUser(Users user);
        Task<Users> GetUserByEmailOrUsername(string userName, string email);
        Task<Users> GetUserByRefreshToken(string refreshToken);
        Task<Users> GetUserByUsername(string userName);
        Task UpdateUserRefreshTokenByUserId(ObjectId userId, string refreshToken);
        Task SubmitUserEmail(ObjectId userId);
        Task DeleteRefreshTokenByUserId(ObjectId userId);
    }

    public class UserRepository : IUsersRepository
    {
        private readonly IMongoCollection<Users> _collection;

        public UserRepository(DBContext context)
        {
            _collection = context.GetCollection<Users>();
        }

        public async Task<IEnumerable<Users>> GetUsers()
        {
            return await _collection.Find(entity => true).ToListAsync();
        }

        public async Task AddUser(Users user)
        {
            await _collection.InsertOneAsync(user);
        }

        public async Task<Users> GetUserByEmailOrUsername(string userName, string email)
        {
            return await _collection
                .Find(e => e.Username == userName || e.Email == email)
                .FirstOrDefaultAsync();
        }

        public async Task<Users> GetUserByUsername(string userName)
        {
            return await _collection.Find(e => e.Username == userName).FirstOrDefaultAsync();
        }

        public async Task<Users> GetUserByRefreshToken(string refreshToken)
        {
            return await _collection
                .Find(e => e.RefreshToken == refreshToken)
                .FirstOrDefaultAsync();
        }

        public async Task UpdateUserRefreshTokenByUserId(ObjectId userId, string refreshToken)
        {
            var filter = Builders<Users>.Filter.Eq(u => u.Id, userId);
            var update = Builders<Users>.Update.Set(u => u.RefreshToken, refreshToken);

            await _collection.UpdateOneAsync(filter, update);
        }

        public async Task SubmitUserEmail(ObjectId userId)
        {
            var filter = Builders<Users>.Filter.Eq(u => u.Id, userId);
            var update = Builders<Users>.Update.Set(u => u.IsEmailSubmited, true);

            await _collection.UpdateOneAsync(filter, update);
        }

        public async Task DeleteRefreshTokenByUserId(ObjectId userId)
        {
            var filter = Builders<Users>.Filter.Eq(u => u.Id, userId);
            var update = Builders<Users>.Update.Set(u => u.RefreshToken, string.Empty);

            await _collection.UpdateOneAsync(filter, update);
        }
    }
}
