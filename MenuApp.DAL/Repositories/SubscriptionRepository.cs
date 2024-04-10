using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MenuApp.DAL.DataBaseContext;
using MenuApp.DAL.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MenuApp.DAL.Repositories
{
    interface ISubscriptionRepository
    {
        public void SubscribeTo(ObjectId user, ObjectId subscribeTo);
        public void UnsubscribeFrom(ObjectId user, ObjectId unsubscribeFrom);
    }

    public class SubscriptionRepository
    {
        private readonly IMongoCollection<Subscription> _collection;

        public SubscriptionRepository(DBContext context)
        {
            _collection = context.GetCollection<Subscription>();
        }

        public void Subscribe(ObjectId user, ObjectId subscribeTo)
        {
            var filter = Builders<Subscription>.Filter.Eq(s => s.UserId, user);
            var update = Builders<Subscription>.Update.AddToSet(s => s.Subscribers, subscribeTo);

            _collection.UpdateOne(filter, update);
        }

        public void Unsubscribe(ObjectId user, ObjectId unsubscribeFrom)
        {
            var filter = Builders<Subscription>.Filter.Eq(s => s.UserId, user);
            var update = Builders<Subscription>.Update.Pull(s => s.Subscribers, unsubscribeFrom);

            _collection.UpdateOne(filter, update);
        }
    }
}
