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
        Task<bool> IsUserExistsByEmail(string userName);
        Task<Users> GetUserByRefreshToken(string refreshToken);
        Task<bool> IsUserExistsByUsername(string userName);
        Task UpdateUserRefreshTokenByUserId(ObjectId userId, string refreshToken);
        Task SubmitUserEmail(ObjectId userId);
        Task DeleteRefreshTokenByUserId(ObjectId userId);
        Task UpdateUserEmail(ObjectId userId, string newEmail);
        Task SetNewPassword(ObjectId userId, string newPassword);
        Task SetUsernameAndPassword(ObjectId userId, string username, string password);
        Task<Users> GetUserByUsername(string username);
        Task SetRefreshTokenById(ObjectId userId, string refreshToken);
        Task DeleteNonConfirmedEmails();
        Task<Users> GetUserEmailByUserId(ObjectId userId);
        Task SetUserImage(ObjectId userId, string image);
        Task<string> GetUserImageByUserId(ObjectId userId);
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

        public async Task<bool> IsUserExistsByEmail(string email)
        {
            var user = await _collection.Find(e => e.Email == email).FirstOrDefaultAsync();

            return user != null;
        }

        public async Task<bool> IsUserExistsByUsername(string userName)
        {
            var user = await _collection.Find(e => e.Username == userName).FirstOrDefaultAsync();

            return user != null;
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

        public async Task UpdateUserEmail(ObjectId userId, string newEmail)
        {
            var filter = Builders<Users>.Filter.Eq(u => u.Id, userId);
            var update = Builders<Users>.Update.Set(u => u.Email, newEmail);

            await _collection.UpdateOneAsync(filter, update);
        }

        public async Task SetNewPassword(ObjectId userId, string newPassword)
        {
            var filter = Builders<Users>.Filter.Eq(u => u.Id, userId);
            var update = Builders<Users>.Update.Set(u => u.Password, newPassword);

            await _collection.UpdateOneAsync(filter, update);
        }

        public async Task SetUsernameAndPassword(ObjectId userId, string username, string password)
        {
            var filter = Builders<Users>.Filter.Eq(u => u.Id, userId);

            var update = Builders<Users>
                .Update.Set(u => u.Username, username)
                .Set(u => u.Password, password);

            await _collection.UpdateOneAsync(filter, update);
        }

        public async Task<Users> GetUserByUsername(string username)
        {
            return await _collection.Find(e => e.Username == username).FirstOrDefaultAsync();
        }

        public async Task SetRefreshTokenById(ObjectId userId, string refreshToken)
        {
            var filter = Builders<Users>.Filter.Eq(u => u.Id, userId);
            var update = Builders<Users>.Update.Set(u => u.RefreshToken, refreshToken);

            await _collection.UpdateOneAsync(filter, update);
        }

        public async Task DeleteNonConfirmedEmails()
        {
            var filter = Builders<Users>.Filter.Where(x =>
                x.IsEmailSubmited == false & x.CreatedAt < DateTime.UtcNow.AddDays(-1)
            );

            await _collection.DeleteManyAsync(filter);
        }

        public async Task<Users> GetUserEmailByUserId(ObjectId userId)
        {
            return await _collection.Find(e => e.Id == userId).FirstOrDefaultAsync();
        }

        public async Task SetUserImage(ObjectId userId, string image)
        {
            var filter = Builders<Users>.Filter.Where(x => x.Id == userId);
            var update = Builders<Users>.Update.Set(x => x.Image, image);

            await _collection.UpdateOneAsync(filter, update);
        }

        public async Task<string> GetUserImageByUserId(ObjectId userId)
        {
            var filter = Builders<Users>.Filter.Where((x) => x.Id == userId);
            var user = await _collection.Find(filter).FirstOrDefaultAsync();

            return user.Image ?? string.Empty;
        }
    }
}
