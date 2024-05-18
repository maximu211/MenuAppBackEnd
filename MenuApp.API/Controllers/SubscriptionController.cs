using MenuApp.BLL.Services.SubscriptionService;
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

        [HttpGet("get_subscribed_users")]
        public async Task<IActionResult> GetSubscribers()
        {
            var result = await _subscriptionService.GetSubscribers();
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(new { result.Message, result.Success });
        }

        [HttpGet("get_subscribers")]
        public async Task<IActionResult> GetSubscribedUsers()
        {
            var result = await _subscriptionService.GetSubscribedUsers();
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(new { result.Message, result.Success });
        }
    }
}
