using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MenuApp.DAL.Models.EntityModels;

namespace MenuApp.DAL.Models.AggregetionModels
{
    internal class SubscriptionAggregetionModel : Subscriptions
    {
        public List<Users> SubscribedUsers { get; set; } = new List<Users>();
    }
}
