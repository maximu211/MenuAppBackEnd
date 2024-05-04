using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MenuApp.BLL.DTO;
using MenuApp.BLL.DTO.CommentDTOs;
using MenuApp.BLL.DTO.UserDTOs;
using MenuApp.BLL.Mappers;
using MenuApp.BLL.Services.MenuApp.BLL.Services;
using MenuApp.BLL.Utils.Authorization;
using MenuApp.DAL.Models.AggregetionModels;
using MenuApp.DAL.Models.EntityModels;
using MenuApp.DAL.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace MenuApp.BLL.Services.CommentSevice
{
    public interface ICommentSevice
    {
        Task<ServiceResult> LeaveComment(LeaveCommentDTO comment);
        Task<ServiceResult> GetCommentsByRecipeId(string recipeId);
        Task<ServiceResult> DeleteComment(string commentId);
        Task<ServiceResult> UpdateComment(UpdateCommentDTO updateComment);
    }

    public class CommentService : ICommentSevice
    {
        private readonly ILogger<CommentService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICommentRepository _commentsRepository;
        private readonly IGenerateJwtToken _jwtGenerator;
        private readonly IMapper _mapper;

        public CommentService(
            ILogger<CommentService> logger,
            IHttpContextAccessor httpContextAccessor,
            ICommentRepository commentsRepository,
            IGenerateJwtToken generateJwtToken,
            IMapper mapper
        )
        {
            _logger = logger;
            _httpContextAccessor =
                httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _commentsRepository = commentsRepository;
            _jwtGenerator = generateJwtToken;
            _mapper = mapper;
        }

        public async Task<ServiceResult> DeleteComment(string commentId)
        {
            try
            {
                await _commentsRepository.DeleteComment(ObjectId.Parse(commentId));

                _logger.LogInformation($"Comment {commentId} successfuly deleted");
                return new ServiceResult(true, "Cooment successfuly deleted");
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error ocured while deleting comment {ex}");
                return new ServiceResult(false, "An error ocured while deleting comment");
            }
        }

        public async Task<ServiceResult> GetCommentsByRecipeId(string recipeId)
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

                var commentList = (
                    await _commentsRepository.GetCommentsByRecipeId(
                        userId,
                        ObjectId.Parse(recipeId)
                    )
                )
                    .Select(comment => CommentMapper.MapToCommentDTO(comment, userId, _mapper))
                    .ToList();

                _logger.LogInformation(
                    $"Comments successfuly sended to recipe {recipeId} by userId {userId}"
                );

                return new ServiceResult(true, "Comments successfuly sended", commentList);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    $"An error ocured while sending comment list recipe: {recipeId}: {ex}"
                );
                return new ServiceResult(false, "An error ocured while sending comment list");
            }
        }

        public async Task<ServiceResult> LeaveComment(LeaveCommentDTO commentDto)
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

                Comments comment = new Comments
                {
                    Comment = commentDto.Comment,
                    CommentorId = userId,
                    RecipeId = ObjectId.Parse(commentDto.RecipeId),
                };

                _logger.LogInformation(
                    $"User {userId} leave comment to recipe {commentDto.RecipeId}"
                );
                await _commentsRepository.LeaveComment(comment);

                return new ServiceResult(true, "Comment successfuly leaved");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    $"An error ocured while leaving comment to recipe: {commentDto.RecipeId}: {ex}"
                );
                return new ServiceResult(false, "An error ocured while leaving comment to recipe");
            }
        }

        public async Task<ServiceResult> UpdateComment(UpdateCommentDTO updateComment)
        {
            try
            {
                await _commentsRepository.UpdateComment(
                    ObjectId.Parse(updateComment.CommentId),
                    updateComment.Comment
                );

                _logger.LogInformation($"Comment {updateComment.CommentId} succesfully updated");
                return new ServiceResult(true, "Comment succesfully updated");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    $"An error ocured while updating comment {updateComment.CommentId} to recipe {ex}"
                );
                return new ServiceResult(false, "An error ocured while updating comment to recipe");
            }
        }
    }
}
