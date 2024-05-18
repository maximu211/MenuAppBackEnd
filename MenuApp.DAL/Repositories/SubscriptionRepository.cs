using MenuApp.DAL.DataBaseContext;
using MenuApp.DAL.Models.AggregetionModels;
using MenuApp.DAL.Models.EntityModels;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MenuApp.DAL.Repositories
{
    public interface ISubscriptionRepository
    {
        Task SubscribeTo(ObjectId user, ObjectId subscribeTo);
        Task UnsubscribeFrom(ObjectId user, ObjectId unsubscribeFrom);
        Task<List<Users>> GetSubscribers(ObjectId userId);
        Task<List<Users>> GetSubscribedUsers(ObjectId userId);
        Task CreateSubsDocument(Subscriptions subs);
        Task<Subscriptions> GetSubsByUserId(ObjectId userId);
        Task<int> GetSubscribedUsersCount(ObjectId userId);
    }

    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly IMongoCollection<Subscriptions> _subscriptionsCollection;
        private readonly IMongoCollection<Users> _usersCollection;

        public SubscriptionRepository(DBContext context)
        {
            _subscriptionsCollection = context.GetCollection<Subscriptions>();
            _usersCollection = context.GetCollection<Users>();
        }

        public async Task SubscribeTo(ObjectId user, ObjectId subscribeTo)
        {
            var existingSubscription = await _subscriptionsCollection
                .Find(s => s.UserId == user)
                .FirstOrDefaultAsync();

            if (existingSubscription == null)
            {
                existingSubscription = new Subscriptions
                {
                    UserId = user,
                    Subscribers = new List<ObjectId>()
                };

                await _subscriptionsCollection.InsertOneAsync(existingSubscription);
            }

            existingSubscription.Subscribers.Add(subscribeTo);

            var filter = Builders<Subscriptions>.Filter.Eq(s => s.Id, existingSubscription.Id);
            var update = Builders<Subscriptions>.Update.Set(
                s => s.Subscribers,
                existingSubscription.Subscribers
            );
            await _subscriptionsCollection.UpdateOneAsync(filter, update);
        }

        public async Task UnsubscribeFrom(ObjectId user, ObjectId unsubscribeFrom)
        {
            var filter = Builders<Subscriptions>.Filter.Eq(s => s.UserId, user);
            var update = Builders<Subscriptions>.Update.Pull(s => s.Subscribers, unsubscribeFrom);

            await _subscriptionsCollection.UpdateOneAsync(filter, update);
        }

        public async Task<List<Users>> GetSubscribers(ObjectId userId)
        {
            var subscription = await _subscriptionsCollection
                .Aggregate()
                .Lookup<Subscriptions, Users, SubscriptionAggregetionModel>(
                    foreignCollection: _usersCollection,
                    localField: sub => sub.Subscribers,
                    foreignField: u => u.Id,
                    @as: swu => swu.SubscribedUsers
                )
                .Match(s => s.UserId == userId)
                .FirstOrDefaultAsync();

            return subscription?.SubscribedUsers ?? new List<Users>();
        }

        public async Task<List<Users>> GetSubscribedUsers(ObjectId userId)
        {
            var result = await _subscriptionsCollection
                .Aggregate()
                .Lookup<Subscriptions, Users, SubscriptionAggregetionModel>(
                    foreignCollection: _usersCollection,
                    localField: sub => sub.UserId,
                    foreignField: u => u.Id,
                    @as: swu => swu.SubscribedUsers
                )
                .Match(s => s.Subscribers.Contains(userId))
                .ToListAsync();

            var subscribedUsers = result.SelectMany(s => s.SubscribedUsers).ToList();

            return subscribedUsers ?? new List<Users>();
        }

        public async Task<int> GetSubscribedUsersCount(ObjectId userId)
        {
            var result = await _subscriptionsCollection
                .Aggregate()
                .Match(sub => sub.Subscribers.Contains(userId))
                .Count()
                .FirstOrDefaultAsync();

            return (int)(result?.Count ?? 0);
        }

        public async Task CreateSubsDocument(Subscriptions subs)
        {
            await _subscriptionsCollection.InsertOneAsync(subs);
        }

        public async Task<Subscriptions> GetSubsByUserId(ObjectId userId)
        {
            return await _subscriptionsCollection
                .AsQueryable()
                .Where(subs => subs.UserId.Equals(userId))
                .FirstOrDefaultAsync();
        }
    }
}
