using AutoMapper;
using MenuApp.BLL.DTO.RecipesDTOs;
using MenuApp.BLL.DTO.UserDTOs;
using MenuApp.DAL.Models.AggregetionModels;
using MenuApp.DAL.Models.EntityModels;
using MongoDB.Bson;

namespace MenuApp.BLL.Mappers
{
    public class CardRecipeMapper
    {
        public static CardRecipeDTO MapToCardRecipeDTO(
            RecipeWithUserModel recipeWithUser,
            ObjectId userId,
            IMapper mapper
        )
        {
            return new CardRecipeDTO
            {
                Id = recipeWithUser.Id.ToString(),
                RecipeImage = recipeWithUser.RecipeImage,
                User = mapper.Map<Users, UserDTO>(recipeWithUser.User),
                Name = recipeWithUser.Name,
                CookingDifficulty = recipeWithUser.CookingDifficulty,
                CookingTime = recipeWithUser.CookTime,
                RecipeType = recipeWithUser.RecipeType,
                IsRecipeSaved = recipeWithUser.Saved.Contains(userId),
                IsRecipeLiked = recipeWithUser.Likes.Contains(userId),
                LikesCount = recipeWithUser.Likes.Count,
                IsOwner = recipeWithUser.CreatorId == userId
            };
        }
    }
}
