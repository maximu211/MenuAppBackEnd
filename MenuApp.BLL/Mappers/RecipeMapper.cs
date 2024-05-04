using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MenuApp.BLL.DTO.CommentDTOs;
using MenuApp.BLL.DTO.RecipesDTOs;
using MenuApp.BLL.DTO.UserDTOs;
using MenuApp.DAL.Models.AggregetionModels;
using MenuApp.DAL.Models.EntityModels;
using MongoDB.Bson;

namespace MenuApp.BLL.Mappers
{
    public class RecipeMapper
    {
        public static Recipes MapRecipeDTOToRecipe(RecipeDTO recipeDto, ObjectId recipeId)
        {
            return new Recipes
            {
                Id = recipeId,
                Name = recipeDto.Name,
                RecipeImage = recipeDto.Image,
                RecipeDescriptionElements = recipeDto.RecipeDescriptionElements,
                RecipeType = recipeDto.RecipeType,
                RecipeIngradients = recipeDto.RecipeIngradients,
                CookingDifficulty = recipeDto.CookingDifficulty,
                CookTime = recipeDto.CookTime,
            };
        }
    }
}
