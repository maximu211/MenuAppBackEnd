using AutoMapper;
using MenuApp.BLL.DTO.RecipesDTOs;
using MenuApp.BLL.Mappers;
using MenuApp.BLL.Services.MenuApp.BLL.Services;
using MenuApp.BLL.Utils.Authorization;
using MenuApp.DAL.Models.EntityModels;
using MenuApp.DAL.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MenuApp.BLL.Services.RecipeService
{
    public interface IRecipeService
    {
        Task<ServiceResult> GetRecipesByUserId(string userId);
        Task<ServiceResult> GetRecipesBySubscriptions();
        Task<ServiceResult> GetUserSavedRecipes();
        Task<ServiceResult> DeleteRecipe(string recipeId);
        Task<ServiceResult> UpdateRecipe(string recipeId, RecipeDTO recipe);
        Task<ServiceResult> AddRecipe(RecipeDTO recipe);
        Task<ServiceResult> LikeRecipe(string recipeId);
        Task<ServiceResult> SaveRecipe(string recipeId);
        Task<ServiceResult> DislikeRecipe(string recipeId);
        Task<ServiceResult> DeleteFromSavedRecipe(string recipeId);
        Task<ServiceResult> GetRecipeDetails(string recipeId);
        Task<ServiceResult> GetRecipeById(string recipeId);
    }

    public class RecipeService : IRecipeService
    {
        private readonly ILogger<RecipeService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRecipeRepository _recipesRepository;
        private readonly IGenerateJwtToken _jwtGenerator;
        private readonly IMapper _mapper;

        public RecipeService(
            ILogger<RecipeService> logger,
            IHttpContextAccessor httpContextAccessor,
            IRecipeRepository recipesRepository,
            IGenerateJwtToken generateJwtToken,
            IMapper mapper
        )
        {
            _logger = logger;
            _httpContextAccessor =
                httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _recipesRepository = recipesRepository;
            _jwtGenerator = generateJwtToken;
            _mapper = mapper;
        }

        public async Task<ServiceResult> AddRecipe(RecipeDTO recipeDto)
        {
            try
            {
                var userIdClaim = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c =>
                    c.Type == "userId"
                );
                if (userIdClaim == null)
                {
                    throw new Exception("userId claim is missing in the token");
                }

                ObjectId userId = _jwtGenerator.GetUserIdFromJwtToken(userIdClaim.Value);

                Recipes recipe = new Recipes
                {
                    Name = recipeDto.Name,
                    RecipeImage = recipeDto.Image,
                    RecipeDescriptionElements = recipeDto.RecipeDescriptionElements,
                    RecipeIngradients = recipeDto.RecipeIngredients,
                    RecipeType = recipeDto.RecipeType,
                    CookingDifficulty = recipeDto.CookingDifficulty,
                    CookTime = recipeDto.CookTime,
                    CreatorId = userId,
                };

                await _recipesRepository.AddRecipe(_mapper.Map<Recipes>(recipe));

                _logger.LogInformation($"Recipe successfuly created by user: {userIdClaim.Value}");
                return new ServiceResult(true, "Recipe successfuly created");
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while creating Recipe: {ex}");
                return new ServiceResult(true, "An error occurred while creating Recipe");
            }
        }

        public async Task<ServiceResult> DeleteRecipe(string recipeId)
        {
            try
            {
                var userIdClaim = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c =>
                    c.Type == "userId"
                );
                if (userIdClaim == null)
                {
                    throw new Exception("userId claim is missing in the token");
                }

                ObjectId userId = _jwtGenerator.GetUserIdFromJwtToken(userIdClaim.Value);

                var recipe = await _recipesRepository.GetRecipeById(ObjectId.Parse(recipeId));

                if (recipe.CreatorId != userId)
                    throw new Exception("userId not equals to creatoId");

                await _recipesRepository.DeleteRecipe(ObjectId.Parse(recipeId));
                _logger.LogInformation($"Recipe {recipeId} successfuly deleted");
                return new ServiceResult(true, "Recipe successfuly deleted");
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while deleting Recipe: {ex}");
                return new ServiceResult(false, "An error occurred while deleting Recipe");
            }
        }

        public async Task<ServiceResult> GetRecipesByUserId(string userId)
        {
            try
            {
                var userIdClaim = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c =>
                    c.Type == "userId"
                );
                if (userIdClaim == null)
                {
                    throw new Exception("userId claim is missing in the token");
                }

                ObjectId userIdRequests = _jwtGenerator.GetUserIdFromJwtToken(userIdClaim.Value);

                var recipeList = await _recipesRepository.GetRecipesByUserId(
                    ObjectId.Parse(userId)
                );

                var recipeListMapped = recipeList
                    .Select(r => CardRecipeMapper.MapToCardRecipeDTO(r, userIdRequests, _mapper))
                    .ToList();

                _logger.LogInformation($"Data successfuly sended by userid: {userId}");
                return new ServiceResult(true, "Recipe List succesfuly sended", recipeListMapped);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while sending Recipe list by userId {ex}");
                return new ServiceResult(false, "An error occurred while sending Recipe list");
            }
        }

        public async Task<ServiceResult> GetRecipesBySubscriptions()
        {
            try
            {
                var userIdClaim = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c =>
                    c.Type == "userId"
                );
                if (userIdClaim == null)
                {
                    throw new Exception("userId claim is missing in the token");
                }

                ObjectId userId = _jwtGenerator.GetUserIdFromJwtToken(userIdClaim.Value);

                var cardRecipeDTOList = (await _recipesRepository.GetRecipesBySubscriptions(userId))
                    .Select(recipe => CardRecipeMapper.MapToCardRecipeDTO(recipe, userId, _mapper))
                    .ToList();

                _logger.LogInformation($"Data successfuly sended by userid: {userId}");
                return new ServiceResult(true, "Recipe List succesfuly sended", cardRecipeDTOList);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    $"An error occurred while sending Recipe list by subscriptions {ex}"
                );
                return new ServiceResult(
                    false,
                    "An error occurred while sending Recipe list by subscriptions"
                );
            }
        }

        public async Task<ServiceResult> UpdateRecipe(string recipeId, RecipeDTO recipeDto)
        {
            try
            {
                var userIdClaim = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c =>
                    c.Type == "userId"
                );
                if (userIdClaim == null)
                    throw new Exception("userId claim is missing in the token");

                ObjectId userId = _jwtGenerator.GetUserIdFromJwtToken(userIdClaim.Value);

                var recipe = RecipeMapper.MapRecipeDTOToRecipe(recipeDto, ObjectId.Parse(recipeId));
                await _recipesRepository.UpdateRecipe(recipe);

                _logger.LogInformation($"Recipe {recipeId} successfuly updated by user {userId}");
                return new ServiceResult(true, "Recipe successfuly updated");
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while updating recipe {ex}");
                return new ServiceResult(false, "An error occurred while updating recipe");
            }
        }

        public async Task<ServiceResult> GetUserSavedRecipes()
        {
            try
            {
                var userIdClaim = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c =>
                    c.Type == "userId"
                );
                if (userIdClaim == null)
                    throw new Exception("userId claim is missing in the token");

                ObjectId userId = _jwtGenerator.GetUserIdFromJwtToken(userIdClaim.Value);

                var cardRecipeDTOList = (await _recipesRepository.GetSavedRecipesByUserid(userId))
                    .Select(recipe => CardRecipeMapper.MapToCardRecipeDTO(recipe, userId, _mapper))
                    .ToList();

                _logger.LogInformation($"Data successfuly sended by userid: {userId}");
                return new ServiceResult(true, "Data succesfully sended", cardRecipeDTOList);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while sending data {ex}");
                return new ServiceResult(false, "An error occurred while sending data");
            }
        }

        public async Task<ServiceResult> LikeRecipe(string recipeId)
        {
            try
            {
                var userIdClaim = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c =>
                    c.Type == "userId"
                );
                if (userIdClaim == null)
                    throw new Exception("userId claim is missing in the token");

                ObjectId userId = _jwtGenerator.GetUserIdFromJwtToken(userIdClaim.Value);

                await _recipesRepository.LikeRecipe(userId, ObjectId.Parse(recipeId));

                _logger.LogInformation($"User {userId} liked recipe {recipeId}");
                return new ServiceResult(true, "User successfuly like recipe");
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while liking recipe {ex}");
                return new ServiceResult(false, "An error occurred while liking recipe");
            }
        }

        public async Task<ServiceResult> SaveRecipe(string recipeId)
        {
            try
            {
                var userIdClaim = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c =>
                    c.Type == "userId"
                );
                if (userIdClaim == null)
                    throw new Exception("userId claim is missing in the token");

                ObjectId userId = _jwtGenerator.GetUserIdFromJwtToken(userIdClaim.Value);

                await _recipesRepository.SaveRecipe(userId, ObjectId.Parse(recipeId));

                _logger.LogInformation($"User {userId} successfuly save recipe {recipeId}");
                return new ServiceResult(true, "Recipe successfuly saved");
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"An error occurred while saving recipe {ex}");
                return new ServiceResult(false, "An error occurred while saving recipe");
            }
        }

        public async Task<ServiceResult> DislikeRecipe(string recipeId)
        {
            try
            {
                var userIdClaim = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c =>
                    c.Type == "userId"
                );
                if (userIdClaim == null)
                    throw new Exception("userId claim is missing in the token");

                ObjectId userId = _jwtGenerator.GetUserIdFromJwtToken(userIdClaim.Value);

                await _recipesRepository.DislikeRecipe(userId, ObjectId.Parse(recipeId));

                _logger.LogInformation($"User {userId} disliked recipe {recipeId}");
                return new ServiceResult(true, "User successfuly disliking recipe");
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while disliking recipe {ex}");
                return new ServiceResult(false, "An error occurred while disliking recipe");
            }
        }

        public async Task<ServiceResult> DeleteFromSavedRecipe(string recipeId)
        {
            try
            {
                var userIdClaim = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c =>
                    c.Type == "userId"
                );
                if (userIdClaim == null)
                    throw new Exception("userId claim is missing in the token");

                ObjectId userId = _jwtGenerator.GetUserIdFromJwtToken(userIdClaim.Value);

                await _recipesRepository.DeleteFromSavedRecipe(userId, ObjectId.Parse(recipeId));

                _logger.LogInformation(
                    $"User {userId} successfuly save delete from saved recipe {recipeId}"
                );
                return new ServiceResult(true, "Recipe successfuly deleted from saved recipe");
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"An error occurred while deleting from saved recipe {ex}");
                return new ServiceResult(
                    false,
                    "An error occurred while deleting from saved recipe"
                );
            }
        }

        public async Task<ServiceResult> GetRecipeDetails(string recipeId)
        {
            try
            {
                var recipeDetail = _mapper.Map<Recipes, RecipeDetailDTO>(
                    await _recipesRepository.GetRecipeById(ObjectId.Parse(recipeId))
                );

                _logger.LogInformation($"Recipe {recipeId} details successfuly sended");
                return new ServiceResult(true, "Recipe details successfuly sended", recipeDetail);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"An error occurred while saving recipe {ex}");
                return new ServiceResult(false, "An error occurred while sending recipe details");
            }
        }

        public async Task<ServiceResult> GetRecipeById(string recipeId)
        {
            try
            {
                var recipe = await _recipesRepository.GetRecipeById(ObjectId.Parse(recipeId));

                RecipeDTO result = new RecipeDTO
                {
                    Image = recipe.RecipeImage,
                    Name = recipe.Name,
                    RecipeDescriptionElements = recipe.RecipeDescriptionElements,
                    RecipeType = recipe.RecipeType,
                    RecipeIngredients = recipe.RecipeIngradients,
                    CookingDifficulty = recipe.CookingDifficulty,
                    CookTime = recipe.CookTime
                };

                _logger.LogInformation($"Recipe {recipeId} successfuly sended");
                return new ServiceResult(true, "Recipe successfuly sended", result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An Error occured while sending recipe {recipeId}: {ex}");
                return new ServiceResult(false, "An Error occured while sending recipe");
            }
        }
    }
}
