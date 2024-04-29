using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MenuApp.DAL.Models.EntityModels;

namespace MenuApp.DAL.Models.AggregetionModels
{
    public class CommentWithUserModel : Comments
    {
        public required Users User { get; set; }
    }
}
