using MenuApp.BLL.Services.CommentSevice;
using MenuApp.BLL.Services.SearchService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MenuApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SearchController : ControllerBase
    {
        private readonly ISearchService _searchService;

        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
        }

        [HttpGet("search/{query}")]
        public async Task<IActionResult> SearchByQuery(string query)
        {
            var result = await _searchService.GetSearchResultByQuery(query);
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(new { result.Message, result.Success });
        }
    }
}
