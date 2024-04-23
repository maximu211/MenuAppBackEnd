using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MenuApp.DAL.Models.EntityModels;
using MongoDB.Bson;

namespace MenuApp.BLL.DTO.ReceiptsDTOs
{
    public class ReceiptsDTO
    {
        public required string Id { get; set; }
        public required string UserId { get; set; }
        public required string Name { get; set; }
        public CookingTime CookTime { get; set; }
        public CookingDifficulty CookingDifficulty { get; set; }
        public required string ReceiptType { get; set; }
        public List<ReceiptDescriptionElement> ReceiptDescriptionElements { get; set; } =
            new List<ReceiptDescriptionElement>();
        public List<string> ReceiptIngradients { get; set; } = new List<string>();
    }
}
