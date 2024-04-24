using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MenuApp.DAL.Models.EntityModels;

namespace MenuApp.DAL.Models.AggregetionModels.ReceiptAggregetionModels
{
    public class ReceipeUserModel : Receipes
    {
        public Users User { get; set; }
    }
}
