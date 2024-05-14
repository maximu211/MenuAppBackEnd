using MenuApp.BLL.DTO.CommentDTOs;
using MenuApp.BLL.Services.CommentSevice;
using MenuApp.BLL.Services.RecipeService;
using MenuApp.DAL.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MenuApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CommentController : Controller
    {
        private readonly ICommentSevice _commentService;

        public CommentController(ICommentSevice commentService)
        {
            _commentService = commentService;
        }

        [HttpPost("leave_comment/{recipeId}")]
        public async Task<IActionResult> LeaveComment(string recipeId, [FromBody]string commentText)
        {
            var result = await _commentService.LeaveComment(recipeId, commentText);
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(new { result.Message, result.Success });
        }

        [HttpGet("get_comments_by_recipeId/{recipeId}")]
        public async Task<IActionResult> GetCommentsByRecipeId(string recipeId)
        {
            var result = await _commentService.GetCommentsByRecipeId(recipeId);
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(new { result.Message, result.Success });
        }

        [HttpDelete("delete_comment/{commentId}")]
        public async Task<IActionResult> DeleteComment(string commentId)
        {
            var result = await _commentService.DeleteComment(commentId);
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(new { result.Message, result.Success });
        }

        [HttpPatch("update_comment/{commentId}")]
        public async Task<IActionResult> UpdateComment(string commentId, string comment)
        {
            var result = await _commentService.UpdateComment(commentId, comment);
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(new { result.Message, result.Success });
        }
    }
}
