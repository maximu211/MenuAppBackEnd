using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MenuApp.BLL.DTO.UserDTOs;
using MenuApp.DAL.Models.EntityModels;
using MenuApp.DAL.Models.Enums;

namespace MenuApp.BLL.DTO.RecipesDTOs
{
    public class CardRecipeDTO
    {
        public required string Id { get; set; }
        public required string RecipeImage { get; set; }
        public required UserDTO User { get; set; }
        public required string Name { get; set; }
        public CookingDifficulty CookingDifficulty { get; set; }
        public CookingTime CookingTime { get; set; }
        public required string RecipeType { get; set; }
        public bool IsRecipeSaved { get; set; }
        public bool IsRecipeLiked { get; set; }
        public int LikesCount { get; set; }
        public bool IsOwner { get; set; }
    }
}
