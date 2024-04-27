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

namespace MenuApp.BLL.Services.RecipeService
{
    public interface IRecipeService
    {
        Task<ServiceResult> GetRecipesByUserId(GetRecipeByUserIdDTO userId);
        Task<ServiceResult> GetRecipesBySubscriptions(ObjectId userId);
        Task<ServiceResult> GetSavedRecipesByUserid(ObjectId userId);
        Task<ServiceResult> DeleteRecipe(DeleteRecipeDTO RecipeId);
        Task<ServiceResult> UpdateRecipe(Recipes Recipe);
        Task<ServiceResult> AddRecipe(RecipesDTO Recipe);
        Task<ServiceResult> LikeRecipe(ObjectId userId, ObjectId RecipeId);
        Task<ServiceResult> SaveRecipe(ObjectId userId, ObjectId RecipeId);
        Task<ServiceResult> DislikeRecipe(ObjectId userId, ObjectId RecipeId);
        Task<ServiceResult> DeleteFromSavedRecipe(ObjectId userId, ObjectId RecipeId);
    }

    public class ReceipService : IRecipeService
    {
        private readonly ILogger<ReceipService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IReceipesRepository _recipesRepository;
        private readonly IGenerateJwtToken _jwtGenerator;
        private readonly IMapper _mapper;

        public ReceipService(
            ILogger<ReceipService> logger,
            IHttpContextAccessor httpContextAccessor,
            IReceipesRepository recipesRepository,
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

        public async Task<ServiceResult> AddRecipe(RecipesDTO Recipe)
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

                Recipe.UserId = userIdClaim.Value;

                await _recipesRepository.AddRecipe(_mapper.Map<Recipes>(Recipe));

                _logger.LogInformation($"Recipe successfuly created by user: {userIdClaim.Value}");
                return new ServiceResult(true, "Recipe successfuly created");
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while creating Recipe: {ex}");
                return new ServiceResult(true, "An error occurred while creating Recipe");
            }
        }

        public async Task<ServiceResult> DeleteRecipe(DeleteRecipeDTO RecipeId)
        {
            try
            {
                await _recipesRepository.DeleteRecipe(ObjectId.Parse(RecipeId.RecipeId));
                _logger.LogInformation($"Recipe {RecipeId} successfuly deleted");
                return new ServiceResult(true, "Recipe successfuly deleted");
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while deleting Recipe: {ex}");
                return new ServiceResult(false, "An error occurred while deleting Recipe");
            }
        }

        public async Task<ServiceResult> GetRecipesByUserId(GetRecipeByUserIdDTO getRecipeByUserId)
        {
            try
            {
                var RecipeList = await _recipesRepository.GetRecipesByUserId(
                    ObjectId.Parse(getRecipeByUserId.UserId)
                );

                _logger.LogInformation(
                    $"Data successfuly sended by userid: {getRecipeByUserId.UserId}"
                );
                return new ServiceResult(true, "Recipe List succesfuly sended", RecipeList);
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
            }
            catch (Exception ex) { }
        }

        public Task<ServiceResult> UpdateRecipe(Recipes Recipe)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResult> GetSavedRecipesByUserid(ObjectId userId)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResult> LikeRecipe(ObjectId userId, ObjectId RecipeId)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResult> SaveRecipe(ObjectId userId, ObjectId RecipeId)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResult> DislikeRecipe(ObjectId userId, ObjectId RecipeId)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResult> DeleteFromSavedRecipe(ObjectId userId, ObjectId RecipeId)
        {
            throw new NotImplementedException();
        }
    }
}
