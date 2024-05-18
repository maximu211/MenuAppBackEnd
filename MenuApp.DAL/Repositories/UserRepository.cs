using MenuApp.DAL.DataBaseContext;
using MenuApp.DAL.Models.AggregetionModels;
using MenuApp.DAL.Models.EntityModels;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MenuApp.DAL.Repositories
{
    public interface IUserRepository
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
        Task<List<Users>> GetUsersBySearch(string query);
        Task<UserPageModel> GetUserPageModel(ObjectId userId);
    }

    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<Users> _usersCollection;
        private readonly IMongoCollection<Subscriptions> _subscriptionsCollection;
        private readonly IMongoCollection<Recipes> _recipesCollection;

        public UserRepository(DBContext context)
        {
            _usersCollection = context.GetCollection<Users>();
            _subscriptionsCollection = context.GetCollection<Subscriptions>();
            _recipesCollection = context.GetCollection<Recipes>();
        }

        public async Task<IEnumerable<Users>> GetUsers()
        {
            return await _usersCollection.Find(entity => true).ToListAsync();
        }

        public async Task AddUser(Users user)
        {
            await _usersCollection.InsertOneAsync(user);
        }

        public async Task<bool> IsUserExistsByEmail(string email)
        {
            var user = await _usersCollection.Find(e => e.Email == email).FirstOrDefaultAsync();

            return user != null;
        }

        public async Task<bool> IsUserExistsByUsername(string userName)
        {
            var user = await _usersCollection
                .Find(e => e.Username == userName)
                .FirstOrDefaultAsync();

            return user != null;
        }

        public async Task<Users> GetUserByRefreshToken(string refreshToken)
        {
            return await _usersCollection
                .AsQueryable()
                .Where(e => e.RefreshToken == refreshToken)
                .FirstOrDefaultAsync();
        }

        public async Task UpdateUserRefreshTokenByUserId(ObjectId userId, string refreshToken)
        {
            var filter = Builders<Users>.Filter.Eq(u => u.Id, userId);
            var update = Builders<Users>.Update.Set(u => u.RefreshToken, refreshToken);

            await _usersCollection.UpdateOneAsync(filter, update);
        }

        public async Task SubmitUserEmail(ObjectId userId)
        {
            var filter = Builders<Users>.Filter.Eq(u => u.Id, userId);
            var update = Builders<Users>.Update.Set(u => u.IsEmailSubmited, true);

            await _usersCollection.UpdateOneAsync(filter, update);
        }

        public async Task DeleteRefreshTokenByUserId(ObjectId userId)
        {
            var filter = Builders<Users>.Filter.Eq(u => u.Id, userId);
            var update = Builders<Users>.Update.Set(u => u.RefreshToken, string.Empty);

            await _usersCollection.UpdateOneAsync(filter, update);
        }

        public async Task UpdateUserEmail(ObjectId userId, string newEmail)
        {
            var filter = Builders<Users>.Filter.Eq(u => u.Id, userId);
            var update = Builders<Users>.Update.Set(u => u.Email, newEmail);

            await _usersCollection.UpdateOneAsync(filter, update);
        }

        public async Task SetNewPassword(ObjectId userId, string newPassword)
        {
            var filter = Builders<Users>.Filter.Eq(u => u.Id, userId);
            var update = Builders<Users>.Update.Set(u => u.Password, newPassword);

            await _usersCollection.UpdateOneAsync(filter, update);
        }

        public async Task SetUsernameAndPassword(ObjectId userId, string username, string password)
        {
            var filter = Builders<Users>.Filter.Eq(u => u.Id, userId);

            var update = Builders<Users>
                .Update.Set(u => u.Username, username)
                .Set(u => u.Password, password);

            await _usersCollection.UpdateOneAsync(filter, update);
        }

        public async Task<Users> GetUserByUsername(string username)
        {
            return await _usersCollection.Find(e => e.Username == username).FirstOrDefaultAsync();
        }

        public async Task SetRefreshTokenById(ObjectId userId, string refreshToken)
        {
            var filter = Builders<Users>.Filter.Eq(u => u.Id, userId);
            var update = Builders<Users>.Update.Set(u => u.RefreshToken, refreshToken);

            await _usersCollection.UpdateOneAsync(filter, update);
        }

        public async Task DeleteNonConfirmedEmails()
        {
            var filter = Builders<Users>.Filter.Where(x =>
                x.IsEmailSubmited == false & x.CreatedAt < DateTime.UtcNow.AddDays(-1)
            );

            await _usersCollection.DeleteManyAsync(filter);
        }

        public async Task<Users> GetUserEmailByUserId(ObjectId userId)
        {
            return await _usersCollection.Find(e => e.Id == userId).FirstOrDefaultAsync();
        }

        public async Task SetUserImage(ObjectId userId, string image)
        {
            var filter = Builders<Users>.Filter.Where(x => x.Id == userId);
            var update = Builders<Users>.Update.Set(x => x.Image, image);

            await _usersCollection.UpdateOneAsync(filter, update);
        }

        public async Task<string> GetUserImageByUserId(ObjectId userId)
        {
            var filter = Builders<Users>.Filter.Where((x) => x.Id == userId);
            var user = await _usersCollection.Find(filter).FirstOrDefaultAsync();

            return user.Image ?? string.Empty;
        }

        public async Task<List<Users>> GetUsersBySearch(string query)
        {
            var lowerCaseQuery = query.ToLowerInvariant();

            return await _usersCollection
                .AsQueryable()
                .Where(u => u.Username.ToLowerInvariant().Contains(lowerCaseQuery))
                .ToListAsync();
        }

        public async Task<UserPageModel> GetUserPageModel(ObjectId userId)
        {
            return await _subscriptionsCollection
                .Aggregate()
                .Match(subs => subs.UserId.Equals(userId))
                .Lookup<Subscriptions, Users, UserPageModel>(
                    _usersCollection,
                    subs => subs.UserId,
                    user => user.Id,
                    model => model.User
                )
                .Unwind<UserPageModel, UserPageModel>(model => model.User)
                .Lookup<UserPageModel, Recipes, UserPageModel>(
                    _recipesCollection,
                    u => u.Id,
                    r => r.CreatorId,
                    model => model.Recipes
                )
                .As<UserPageModel>()
                .FirstAsync();
        }
    }
}
