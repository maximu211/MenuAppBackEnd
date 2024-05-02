using AutoMapper;
using MenuApp.BLL.DTO.UserDTOs;
using MenuApp.BLL.Mappers;
using MenuApp.BLL.Services.MenuApp.BLL.Services;
using MenuApp.BLL.Utils.Authorization;
using MenuApp.DAL.Models.EntityModels;
using MenuApp.DAL.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace MenuApp.BLL.Services.SearchService
{
    public interface ISearchService
    {
        Task<ServiceResult> GetSearchResultByQuery(string query);
    }

    public class SearchService : ISearchService
    {
        private readonly ILogger<SearchService> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IRecipeRepository _recipeRepository;
        private readonly IMapper _mapper;
        private readonly IGenerateJwtToken _jwtGenerator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SearchService(
            IUserRepository userRepository,
            IRecipeRepository recipeRepository,
            ILogger<SearchService> logger,
            IMapper mapper,
            IGenerateJwtToken jwtGenerator,
            IHttpContextAccessor httpContextAccessor
        )
        {
            _userRepository = userRepository;
            _recipeRepository = recipeRepository;
            _mapper = mapper;
            _httpContextAccessor =
                httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger;
            _jwtGenerator = jwtGenerator;
        }

        public async Task<ServiceResult> GetSearchResultByQuery(string query)
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

                var getUsersTask = _userRepository.GetUsersBySearch(query);
                var getRecipesTask = _recipeRepository.GetRecipesBySearch(query);

                await Task.WhenAll(getUsersTask, getRecipesTask);

                var getUsersResult = getUsersTask
                    .Result.Select(user => _mapper.Map<Users, UserDTO>(user))
                    .ToList();

                var getRecipesResult = getRecipesTask
                    .Result.Select(recipe =>
                        CardRecipeMapper.MapToCardRecipeDTO(recipe, userId, _mapper)
                    )
                    .ToList();

                _logger.LogInformation($"user {userId} successfuly get data by query {query}");
                return new ServiceResult(
                    true,
                    "Search successful",
                    (getUsersResult, getRecipesResult)
                );
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error ocurred while searching by query {query}: {ex}");
                return new ServiceResult(false, "An error ocurred while searching");
            }
        }
    }
}
