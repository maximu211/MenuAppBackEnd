using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MenuApp.BLL.DTO.UserDTOs;
using MenuApp.DAL.Models.EntityModels;
using MongoDB.Bson;

namespace MenuApp.BLL.DTO.CommentDTOs
{
    public class CommentDTO
    {
        public required string Id { get; set; }
        public required string RecipeId { get; set; }
        public required string Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public required UserDTO User { get; set; }
        public bool IsOwner { get; set; }
    }
}
