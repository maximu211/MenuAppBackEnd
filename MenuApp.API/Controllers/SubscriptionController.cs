using MenuApp.BLL.DTO.SubscriptionDTOs;
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

        [HttpPost("subscribe_to")]
        public async Task<IActionResult> SubscribeTo(SubscriptionDTO subscribeTo)
        {
            var result = await _subscriptionService.SubscribeTo(subscribeTo);
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result.Message);
        }

        [HttpPost("unsubscribe_from")]
        public async Task<IActionResult> UnsubscribeFrom(SubscriptionDTO unsubscribeFrom)
        {
            var result = await _subscriptionService.UnsubscribeFrom(unsubscribeFrom);
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result.Message);
        }

        [HttpGet("get_subscribers")]
        public async Task<IActionResult> GetSubscribers()
        {
            var result = await _subscriptionService.GetSubscribers();
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result.Message);
        }

        [HttpGet("get_subscribed_users")]
        public async Task<IActionResult> GetSubscribedUsers()
        {
            var result = await _subscriptionService.GetSubscribedUsers();
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result.Message);
        }
    }
}
