using MenuApp.DAL.DataBaseContext;
using MenuApp.DAL.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MenuApp.DAL.Repositories
{
    public interface ISubscriptionRepository
    {
        Task SubscribeTo(ObjectId user, ObjectId subscribeTo);
        Task UnsubscribeFrom(ObjectId user, ObjectId unsubscribeFrom);
        Task<List<string>> GetSubscribers(ObjectId userId);
        Task<List<string>> GetSubscribedUsers(ObjectId userId);
    }

    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly IMongoCollection<Subscription> _collection;

        public SubscriptionRepository(DBContext context)
        {
            _collection = context.GetCollection<Subscription>();
        }

        public async Task SubscribeTo(ObjectId user, ObjectId subscribeTo)
        {
            var existingSubscription = await _collection
                .Find(s => s.UserId == user)
                .FirstOrDefaultAsync();

            if (existingSubscription == null)
            {
                existingSubscription = new Subscription
                {
                    UserId = user,
                    Subscribers = new List<ObjectId>()
                };

                await _collection.InsertOneAsync(existingSubscription);
            }

            existingSubscription.Subscribers?.Add(subscribeTo);

            var filter = Builders<Subscription>.Filter.Eq(s => s.Id, existingSubscription.Id);
            var update = Builders<Subscription>.Update.Set(
                s => s.Subscribers,
                existingSubscription.Subscribers
            );
            await _collection.UpdateOneAsync(filter, update);
        }

        public async Task UnsubscribeFrom(ObjectId user, ObjectId unsubscribeFrom)
        {
            var filter = Builders<Subscription>.Filter.Eq(s => s.UserId, user);
            var update = Builders<Subscription>.Update.Pull(s => s.Subscribers, unsubscribeFrom);

            await _collection.UpdateOneAsync(filter, update);
        }

        public async Task<List<string>> GetSubscribers(ObjectId userId)
        {
            var document = await _collection.Find(s => s.UserId == userId).FirstOrDefaultAsync();

            return document?.Subscribers?.Select(id => id.ToString()).ToList()
                ?? new List<string>();
        }

        public async Task<List<string>> GetSubscribedUsers(ObjectId userId)
        {
            var subscribedUsers = await _collection
                .AsQueryable()
                .Where(s => s.Subscribers.Contains(userId))
                .Select(s => s.UserId.ToString())
                .ToListAsync();

            return subscribedUsers;
        }
    }
}
