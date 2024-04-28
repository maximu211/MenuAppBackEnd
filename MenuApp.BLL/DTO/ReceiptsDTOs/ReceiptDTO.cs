using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MenuApp.DAL.Models.EntityModels;
using MenuApp.DAL.Models.Enums;
using MongoDB.Bson;

namespace MenuApp.BLL.DTO.RecipesDTOs
{
    public class RecipesDTO
    {
        public required string Id { get; set; }
        public required string CreatorId { get; set; }
        public required string Name { get; set; }
        public CookingTime CookTime { get; set; }
        public CookingDifficulty CookingDifficulty { get; set; }
        public required string RecipeType { get; set; }
        public required List<RecipeDescriptionElement> RecipeDescriptionElements { get; set; }
        public required List<string> RecipeIngradients { get; set; }
    }
}
