using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace MenuApp.BLL.Services.SubscriptionService
{
    interface ISubscriptionService
    {
        public void SubscribeTo(ObjectId user, ObjectId subscribeTo);
        public void UnsubscribeFrom(ObjectId user, ObjectId unsubscribeFrom);
    }

    public class SubscriptionService { }
}
