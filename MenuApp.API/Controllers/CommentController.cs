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

        [HttpDelete("delete_comment/{commentId}")]
        public async Task<IActionResult> DeleteComment(string commentId)
        {
            var result = await _commentService.DeleteComment(commentId);
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
