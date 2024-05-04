using MenuApp.DAL.Models.EntityModels;
using MenuApp.DAL.Models.Enums;

namespace MenuApp.BLL.DTO.RecipesDTOs
{
    public class RecipeDTO
    {
        public required string Name { get; set; }
        public CookingTime CookTime { get; set; }
        public required string Image { get; set; }
        public CookingDifficulty CookingDifficulty { get; set; }
        public required string RecipeType { get; set; }
        public required List<RecipeDescriptionElement> RecipeDescriptionElements { get; set; }
        public required List<string> RecipeIngradients { get; set; }
    }
}
