using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace MenuApp.DAL.Models.EntityModels
{
    public class Comments
    {
        public ObjectId Id { get; set; }
        public ObjectId CommentorId { get; set; }
        public ObjectId RecipeId { get; set; }
        public required string Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
