using MenuApp.BLL.DTO.CommentDTOs;
using MenuApp.BLL.Services.CommentSevice;
using MenuApp.BLL.Services.RecipeService;
using MenuApp.DAL.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MenuApp.API.Controllers
{
    public class CommentController : Controller
    {
        [Route("api/[controller]")]
        [ApiController]
        [Authorize]
        public class RecipeController : ControllerBase
        {
            private readonly ICommentSevice _commentService;

            public RecipeController(ICommentSevice commentService)
            {
                _commentService = commentService;
            }

            [HttpPost("leave_comment")]
            public async Task<IActionResult> LeaveComment(LeaveCommentDTO comment)
            {
                var result = await _commentService.LeaveComment(comment);
                if (result.Success)
                    return Ok(result);
                else
                    return BadRequest(result.Message);
            }

            [HttpGet("get_comments_by_recipeId/{recipeId}")]
            public async Task<IActionResult> GetCommentsByRecipeId(string recipeId)
            {
                var result = await _commentService.GetCommentsByRecipeId(recipeId);
                if (result.Success)
                    return Ok(result);
                else
                    return BadRequest(result.Message);
            }

            [HttpDelete("delete_comment/{recipeId}")]
            public async Task<IActionResult> DeleteComment(string recipeId)
            {
                var result = await _commentService.DeleteComment(recipeId);
                if (result.Success)
                    return Ok(result);
                else
                    return BadRequest(result.Message);
            }

            [HttpPatch("update_comment")]
            public async Task<IActionResult> UpdateComment(UpdateCommentDTO updateComment)
            {
                var result = await _commentService.UpdateComment(updateComment);
                if (result.Success)
                    return Ok(result);
                else
                    return BadRequest(result.Message);
            }
        }
    }
}
