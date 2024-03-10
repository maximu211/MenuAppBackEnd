using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace MenuApp.DAL.Models
{
    public class ConfirmationCodes
    {
        public ObjectId Id { get; set; }
        public ObjectId UserId { get; set; }
        public required string ConfirmationCode { get; set; }
    }
}
