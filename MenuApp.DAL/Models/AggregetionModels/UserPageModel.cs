using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MenuApp.DAL.Models.EntityModels;
using MongoDB.Bson;

namespace MenuApp.DAL.Models.AggregetionModels
{
    public class UserPageModel : Subscriptions
    {
        public List<Recipes>? Recipes { get; set; }
        public Users User { get; set; }
        public int SubsribedToCount { get; set; }
    }
}
