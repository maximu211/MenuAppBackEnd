using MenuApp.BLL.Services.SubscriptionService;
using MenuApp.DAL.Models.EntityModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace MenuApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;

        public SubscriptionController(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        [HttpPost("subscribe_to/{userId}")]
        public async Task<IActionResult> SubscribeTo(string userId)
        {
            var result = await _subscriptionService.SubscribeTo(userId);
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(new { result.Message, result.Success });
        }

        [HttpPost("unsubscribe_from/{userId}")]
        public async Task<IActionResult> UnsubscribeFrom(string userId)
        {
            var result = await _subscriptionService.UnsubscribeFrom(userId);
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(new { result.Message, result.Success });
        }

        [HttpGet("get_subscribers/{userId}")]
        public async Task<IActionResult> GetSubscribers(string userId)
        {
            var result = await _subscriptionService.GetSubscribers(userId);
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(new { result.Message, result.Success });
        }

        [HttpGet("get_subscribed_users/{userId}")]
        public async Task<IActionResult> GetSubscribedUsers(string userId)
        {
            var result = await _subscriptionService.GetSubscribedUsers(userId);
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(new { result.Message, result.Success });
        }
    }
}
