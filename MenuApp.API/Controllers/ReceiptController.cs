using MenuApp.BLL.DTO.RecipesDTOs;
using MenuApp.BLL.Services.RecipeService;
using MenuApp.BLL.Services.SubscriptionService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MenuApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RecipeController : ControllerBase
    {
        private readonly IRecipeService _RecipeService;

        public RecipeController(IRecipeService RecipeService)
        {
            _RecipeService = RecipeService;
        }

        [HttpPost("create_Recipe")]
        public async Task<IActionResult> AddRecipe(RecipesDTO Recipe)
        {
            var result = await _RecipeService.AddRecipe(Recipe);
            if (result.Success == true)
                return Ok(result);
            else
                return BadRequest(result.Message);
        }

        [HttpDelete("delete_Recipe")]
        public async Task<IActionResult> DeleteRecipe(DeleteRecipeDTO RecipeId)
        {
            var result = await _RecipeService.DeleteRecipe(RecipeId);
            if (result.Success == true)
                return Ok(result);
            else
                return BadRequest(result.Message);
        }
    }
}
