using MenuApp.DAL.Models.Enums;
using MongoDB.Bson;

namespace MenuApp.DAL.Models.EntityModels
{
    public class Recipes
    {
        public ObjectId Id { get; set; }
        public ObjectId CreatorId { get; set; }
        public required string Name { get; set; }
        public CookingTime CookTime { get; set; }
        public CookingDifficulty CookingDifficulty { get; set; }
        public required string RecipeType { get; set; }
        public required string RecipeImage { get; set; }
        public List<ObjectId> Likes { get; set; } = new List<ObjectId>();
        public List<ObjectId> Saved { get; set; } = new List<ObjectId>();
        public required List<RecipeDescriptionElement> RecipeDescriptionElements { get; set; }
        public required List<string> RecipeIngradients { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class RecipeDescriptionElement
    {
        public required string RecipeDescriptionElementText { get; set; }
        public string? RecipeDescriptionElementImage { get; set; }
    }
}
