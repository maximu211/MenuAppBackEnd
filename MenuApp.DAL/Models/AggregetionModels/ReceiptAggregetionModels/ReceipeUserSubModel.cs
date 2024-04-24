using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MenuApp.DAL.Models.EntityModels;
using MongoDB.Bson;

namespace MenuApp.DAL.Models.AggregetionModels.ReceiptAggregetionModels
{
    public class ReceipeUserSubModel : ReceipeSubsModel
    {
        public Users User { get; set; }
    }
}
