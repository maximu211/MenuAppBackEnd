using AutoMapper;
using MenuApp.BLL.DTO.UserDTOs;
using MenuApp.BLL.Services.MenuApp.BLL.Services;
using MenuApp.BLL.Utils.Authorization;
using MenuApp.DAL.Models;
using MenuApp.DAL.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace MenuApp.BLL.Services.SubscriptionService
{
    public interface ISubscriptionService
    {
        Task<ServiceResult> SubscribeTo(string userId);
        Task<ServiceResult> UnsubscribeFrom(string userId);
        Task<ServiceResult> GetSubscribers();
        Task<ServiceResult> GetSubscribedUsers();
    }

    public class SubscriptionService : ISubscriptionService
    {
        private readonly ILogger<SubscriptionService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IGenerateJwtToken _jwtGenerator;
        private readonly IMapper _mapper;

        public SubscriptionService(
            ILogger<SubscriptionService> logger,
            IHttpContextAccessor httpContextAccessor,
            ISubscriptionRepository subscriptionRepository,
            IGenerateJwtToken generateJwtToken,
            IMapper mapper
        )
        {
            _logger = logger;
            _httpContextAccessor =
                httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _subscriptionRepository = subscriptionRepository;
            _jwtGenerator = generateJwtToken;
            _mapper = mapper;
        }

        public async Task<ServiceResult> GetSubscribedUsers()
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

                List<UserDTO> subscribedUsersList = (
                    await _subscriptionRepository.GetSubscribedUsers(userId)
                )
                    .Select(user => _mapper.Map<UserDTO>(user))
                    .ToList();

                _logger.LogInformation($"User {userId} successfuly get subscriberd users list");
                return new ServiceResult(
                    true,
                    "List succesfully sended",
                    new { Subscribers = subscribedUsersList, }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in getting list of subscriberd users: {ex}");
                return new ServiceResult(false, "Error in getting subscriberd users");
            }
        }

        public async Task<ServiceResult> GetSubscribers()
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

                List<UserDTO> subscribersDTOList = _mapper.Map<List<UserDTO>>(
                    await _subscriptionRepository.GetSubscribers(userId)
                );

                _logger.LogInformation($"User {userId} successfuly get subscribers list");
                return new ServiceResult(
                    true,
                    "List succesfully sended",
                    new { Subscribers = subscribersDTOList }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in getting list of subscribers user: {ex}");
                return new ServiceResult(false, "Error in getting subscribers");
            }
        }

        public async Task<ServiceResult> SubscribeTo(string userId)
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

                ObjectId requestorsId = _jwtGenerator.GetUserIdFromJwtToken(userIdClaim.Value);

                await _subscriptionRepository.SubscribeTo(requestorsId, ObjectId.Parse(userId));

                _logger.LogInformation(
                    $"User {requestorsId} successfuly subscribed to user {userId}"
                );
                return new ServiceResult(true, "Successfuly subscribed");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in subscribe to user {userId}: {ex}");
                return new ServiceResult(false, "Subscribe Error");
            }
        }

        public async Task<ServiceResult> UnsubscribeFrom(string userId)
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

                ObjectId requestorsId = _jwtGenerator.GetUserIdFromJwtToken(userIdClaim.Value);

                await _subscriptionRepository.UnsubscribeFrom(requestorsId, ObjectId.Parse(userId));

                _logger.LogInformation(
                    $"User {requestorsId} successfuly unsubscribed from user {userId}"
                );
                return new ServiceResult(true, "User successfuly unsubscribed");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in unsubscribe to user {userId}: {ex}");
                return new ServiceResult(false, "Unsubscribe error");
            }
        }
    }
}
