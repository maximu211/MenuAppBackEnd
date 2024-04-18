using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MenuApp.BLL.DTO.SubscriptionDTOs;
using MenuApp.BLL.DTO.UserDTO;
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
        Task<ServiceResult> SubscribeTo(SubscriptionDTO subscribeTo);
        Task<ServiceResult> UnsubscribeFrom(SubscriptionDTO unsubscribeFrom);
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

                List<UsersDTO> subscribedUsersList = (
                    await _subscriptionRepository.GetSubscribedUsers(userId)
                )
                    .Select(user => _mapper.Map<UsersDTO>(user))
                    .ToList();

                _logger.LogInformation($"User {userId} successfuly get subscriberd users list");
                return new ServiceResult(
                    true,
                    "List succesfully sended",
                    new
                    {
                        SubscribedUsers = subscribedUsersList,
                        Count = subscribedUsersList.Count()
                    }
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

                List<UsersDTO> subscribersDTOList = _mapper.Map<List<UsersDTO>>(
                    await _subscriptionRepository.GetSubscribers(userId)
                );

                _logger.LogInformation($"User {userId} successfuly get subscribers list");
                return new ServiceResult(
                    true,
                    "List succesfully sended",
                    new { Subscribers = subscribersDTOList, Count = subscribersDTOList.Count() }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in getting list of subscribers user: {ex}");
                return new ServiceResult(false, "Error in getting subscribers");
            }
        }

        public async Task<ServiceResult> SubscribeTo(SubscriptionDTO subscribeTo)
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

                await _subscriptionRepository.SubscribeTo(userId, ObjectId.Parse(subscribeTo.Id));

                _logger.LogInformation(
                    $"User {userId} successfuly subscribed to user {subscribeTo}"
                );
                return new ServiceResult(true, "Successfuly subscribed");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in subscribe to user {subscribeTo}: {ex}");
                return new ServiceResult(false, "Subscribe Error");
            }
        }

        public async Task<ServiceResult> UnsubscribeFrom(SubscriptionDTO unsubscribeFrom)
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

                await _subscriptionRepository.UnsubscribeFrom(
                    userId,
                    ObjectId.Parse(unsubscribeFrom.Id)
                );

                _logger.LogInformation(
                    $"User {userId} successfuly unsubscribed from user {unsubscribeFrom}"
                );
                return new ServiceResult(true, "User successfuly unsubscribed");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in unsubscribe to user {unsubscribeFrom}: {ex}");
                return new ServiceResult(false, "Unsubscribe error");
            }
        }
    }
}
