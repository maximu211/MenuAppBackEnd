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
        Task<IEnumerable<ObjectId>> GetSubscribers(ObjectId userId);
        Task<IEnumerable<ObjectId>> GetSubscribedUsers(ObjectId userId);
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
                List<ObjectId> subscribers = new List<ObjectId>();
                subscribers.Append(subscribeTo);
                existingSubscription = new Subscription
                {
                    UserId = user,
                    Subscribers = subscribers
                };

                await _collection.InsertOneAsync(existingSubscription);
            }

            existingSubscription.Subscribers.Add(subscribeTo);
        }

        public async Task UnsubscribeFrom(ObjectId user, ObjectId unsubscribeFrom)
        {
            var filter = Builders<Subscription>.Filter.Eq(s => s.UserId, user);
            var update = Builders<Subscription>.Update.Pull(s => s.Subscribers, unsubscribeFrom);

            await _collection.UpdateOneAsync(filter, update);
        }

        public async Task<IEnumerable<ObjectId>> GetSubscribers(ObjectId userId)
        {
            var document = await _collection.Find(s => s.UserId == userId).FirstOrDefaultAsync();

            return document.Subscribers ?? Enumerable.Empty<ObjectId>();
        }

        public async Task<IEnumerable<ObjectId>> GetSubscribedUsers(ObjectId userId)
        {
            var subscribedUsers = await _collection
                .AsQueryable()
                .Where(s => s.Subscribers.Contains(userId))
                .Select(s => s.UserId)
                .ToListAsync();

            return subscribedUsers;
        }
    }
}
