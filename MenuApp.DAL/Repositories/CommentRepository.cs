using MenuApp.DAL.DataBaseContext;
using MenuApp.DAL.Models.AggregetionModels;
using MenuApp.DAL.Models.EntityModels;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MenuApp.DAL.Repositories
{
    public interface ICommentRepository
    {
        Task LeaveComment(Comments comment);
        Task<List<CommentWithUserModel>> GetCommentsByRecipeId(ObjectId userId, ObjectId recipeId);
        Task DeleteComment(ObjectId commentId);
        Task UpdateComment(ObjectId commentId, string commentText);
    }

    public class CommentRepository : ICommentRepository
    {
        private readonly IMongoCollection<Comments> _commentsCollection;
        private readonly IMongoCollection<Users> _usersCollection;

        public CommentRepository(DBContext context)
        {
            _commentsCollection = context.GetCollection<Comments>();
            _usersCollection = context.GetCollection<Users>();
        }

        public async Task LeaveComment(Comments comment)
        {
            await _commentsCollection.InsertOneAsync(comment);
        }

        public async Task<List<CommentWithUserModel>> GetCommentsByRecipeId(
            ObjectId userId,
            ObjectId recipeId
        )
        {
            return await _commentsCollection
                .Aggregate()
                .Match(comment => comment.RecipeId == recipeId)
                .Lookup<Comments, Users, CommentWithUserModel>(
                    foreignCollection: _usersCollection,
                    localField: comment => comment.CommentorId,
                    foreignField: user => user.Id,
                    @as: cwu => cwu.User
                )
                .SortByDescending(cwu => cwu.CommentorId == userId)
                .ToListAsync();
        }

        public async Task DeleteComment(ObjectId commentId)
        {
            await _commentsCollection.DeleteOneAsync(com => com.Id == commentId);
        }

        public async Task UpdateComment(ObjectId commentId, string commentText)
        {
            var filter = Builders<Comments>.Filter.Eq(comment => comment.Id, commentId);
            var updateDefinition = Builders<Comments>.Update.Set(
                comment => comment.Comment,
                commentText
            );

            await _commentsCollection.UpdateOneAsync(filter, updateDefinition);
        }
    }
}
