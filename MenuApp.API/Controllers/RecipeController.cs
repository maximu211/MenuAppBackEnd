using MenuApp.BLL.DTO.RecipesDTOs;
using MenuApp.BLL.Services.RecipeService;
using MenuApp.DAL.Models.EntityModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace MenuApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RecipeController : ControllerBase
    {
        private readonly IRecipeService _recipeService;

        public RecipeController(IRecipeService RecipeService)
        {
            _recipeService = RecipeService;
        }

        [HttpPost("create_recipe")]
        public async Task<IActionResult> AddRecipe(RecipeDTO recipe)
        {
            var result = await _recipeService.AddRecipe(recipe);
            if (result.Success == true)
                return Ok(result);
            else
                return BadRequest(new { result.Message, result.Success });
        }

        [HttpGet("get_recipes_by_userId/{userId}")]
        public async Task<IActionResult> GetRecipesByUserId(string userId)
        {
            var result = await _recipeService.GetRecipesByUserId(userId);
            if (result.Success == true)
                return Ok(result);
            else
                return BadRequest(new { result.Message, result.Success });
        }

        [HttpGet("get_recipes_by_subscriptions")]
        public async Task<IActionResult> GetRecipesBySubscriptions()
        {
            var result = await _recipeService.GetRecipesBySubscriptions();
            if (result.Success == true)
                return Ok(result);
            else
                return BadRequest(new { result.Message, result.Success });
        }

        [HttpGet("get_user_saved_recipes")]
        public async Task<IActionResult> GetUserSavedRecipes()
        {
            var result = await _recipeService.GetUserSavedRecipes();
            if (result.Success == true)
                return Ok(result);
            else
                return BadRequest(new { result.Message, result.Success });
        }

        [HttpDelete("delete_recipe/{recipeId}")]
        public async Task<IActionResult> DeleteRecipe(string recipeId)
        {
            var result = await _recipeService.DeleteRecipe(recipeId);
            if (result.Success == true)
                return Ok(result);
            else
                return BadRequest(new { result.Message, result.Success });
        }

        [HttpPut("update_recipe/{recipeId}")]
        public async Task<IActionResult> UpdateRecipe(string recipeId, RecipeDTO recipe)
        {
            var result = await _recipeService.UpdateRecipe(recipeId, recipe);
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(new { result.Message, result.Success });
        }

        [HttpPost("like_recipe/{recipeId}")]
        public async Task<IActionResult> LikeRecipe(string recipeId)
        {
            var result = await _recipeService.LikeRecipe(recipeId);
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(new { result.Message, result.Success });
        }

        [HttpPost("save_recipe/{recipeId}")]
        public async Task<IActionResult> SaveRecipe(string recipeId)
        {
            var result = await _recipeService.SaveRecipe(recipeId);
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(new { result.Message, result.Success });
        }

        [HttpPost("dislike_recipe/{recipeId}")]
        public async Task<IActionResult> DislikeRecipe(string recipeId)
        {
            var result = await _recipeService.DislikeRecipe(recipeId);
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(new { result.Message, result.Success });
        }

        [HttpPost("delete_recipe_from_saved/{recipeId}")]
        public async Task<IActionResult> DeleteFromSavedRecipe(string recipeId)
        {
            var result = await _recipeService.DeleteFromSavedRecipe(recipeId);
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(new { result.Message, result.Success });
        }

        [HttpGet("get_recipe_details/{recipeId}")]
        public async Task<IActionResult> GetRecipeDetails(string recipeId)
        {
            var result = await _recipeService.GetRecipeDetails(recipeId);
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(new { result.Message, result.Success });
        }

        [HttpGet("get_recipe_by_id/{recipeId}")]
        public async Task<IActionResult> GetRecipeById(string recipeId)
        {
            var result = await _recipeService.GetRecipeById(recipeId);
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(new { result.Message, result.Success });
        }
    }
}
