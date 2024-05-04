using AutoMapper;
using MenuApp.BLL.DTO.CommentDTOs;
using MenuApp.BLL.DTO.UserDTOs;
using MenuApp.DAL.Models.AggregetionModels;
using MenuApp.DAL.Models.EntityModels;
using MongoDB.Bson;

namespace MenuApp.BLL.Mappers
{
    public class CommentMapper
    {
        public static CommentDTO MapToCommentDTO(
            CommentWithUserModel commentWithUser,
            ObjectId userId,
            IMapper mapper
        )
        {
            return new CommentDTO
            {
                Id = commentWithUser.Id.ToString(),
                Comment = commentWithUser.Comment,
                CreatedAt = commentWithUser.CreatedAt,
                User = mapper.Map<Users, UserDTO>(commentWithUser.User),
                IsOwner = commentWithUser.CommentorId == userId,
            };
        }
    }
}
