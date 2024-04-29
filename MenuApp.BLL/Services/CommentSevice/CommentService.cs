using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
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
    public interface ICommentService
    {
        Task<ServiceResult> LeaveComment(Comments comment);
        Task<ServiceResult> GetCommentsByRecipeId(ObjectId userId, ObjectId recipeId);
        Task<ServiceResult> DeleteComment(ObjectId commentId);
        Task<ServiceResult> UpdateComment(ObjectId commentId, string commentText);
    }

    public class CommentService
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
    }
}
