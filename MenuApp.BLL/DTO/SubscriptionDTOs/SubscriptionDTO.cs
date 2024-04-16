using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace MenuApp.BLL.DTO.SubscriptionDTOs
{
    public class SubscriptionDTO
    {
        public required string Id { get; set; }
    }
}
