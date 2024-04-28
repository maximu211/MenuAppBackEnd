using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                RecipePhoto = recipeWithUser.ReceipeImage,
                ReceiptId = recipeWithUser.Id.ToString(),
                User = mapper.Map<Users, UserDTO>(recipeWithUser.User),
                Name = recipeWithUser.Name,
                CookingDifficulty = recipeWithUser.CookingDifficulty,
                CookingTime = recipeWithUser.CookTime,
                ReceiptType = recipeWithUser.RecipeType,
                IsDishSaved = recipeWithUser.Saved.Contains(userId),
                IsDishLiked = recipeWithUser.Likes.Contains(userId),
                LikesCount = recipeWithUser.Likes.Count,
                IsOwner = recipeWithUser.CreatorId == userId
            };
        }
    }
}
