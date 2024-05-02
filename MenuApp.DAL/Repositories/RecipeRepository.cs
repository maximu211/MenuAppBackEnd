using MenuApp.DAL.DataBaseContext;
using MenuApp.DAL.Models.AggregetionModels;
using MenuApp.DAL.Models.EntityModels;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MenuApp.DAL.Repositories
{
    public interface IRecipeRepository
    {
        Task<List<RecipeWithUserModel>> GetRecipesByUserId(ObjectId userId);
        Task<List<RecipeWithUserModel>> GetRecipesBySubscriptions(ObjectId userId);
        Task<List<RecipeWithUserModel>> GetSavedRecipesByUserid(ObjectId userId);
        Task DeleteRecipe(ObjectId RecipeId);
        Task UpdateRecipe(Recipes Recipe);
        Task AddRecipe(Recipes Recipe);
        Task LikeRecipe(ObjectId userId, ObjectId recipeId);
        Task SaveRecipe(ObjectId userId, ObjectId recipeId);
        Task DislikeRecipe(ObjectId userId, ObjectId recipeId);
        Task DeleteFromSavedRecipe(ObjectId userId, ObjectId recipeId);
        Task<Recipes> GetRecipeById(ObjectId recipeId);
        Task<List<RecipeWithUserModel>> GetRecipesBySearch(string query);
    }

    public class RecipeRepository : IRecipeRepository
    {
        private readonly IMongoCollection<Recipes> _recipesCollection;
        private readonly IMongoCollection<Users> _usersCollection;
        private readonly IMongoCollection<Subscriptions> _subscriptionsCollection;

        public RecipeRepository(DBContext context)
        {
            _recipesCollection = context.GetCollection<Recipes>();
            _usersCollection = context.GetCollection<Users>();
            _subscriptionsCollection = context.GetCollection<Subscriptions>();
        }

        public async Task AddRecipe(Recipes recipe)
        {
            await _recipesCollection.InsertOneAsync(recipe);
        }

        public async Task DeleteRecipe(ObjectId recipeId)
        {
            await _recipesCollection.DeleteOneAsync(r => r.Id == recipeId);
        }

        public async Task DeleteFromSavedRecipe(ObjectId userId, ObjectId recipeId)
        {
            var filter = Builders<Recipes>.Filter.Eq(r => r.Id, recipeId);
            var update = Builders<Recipes>.Update.PullFilter(
                r => r.Saved,
                Builders<ObjectId>.Filter.Eq(savedUserId => savedUserId, userId)
            );

            await _recipesCollection.UpdateOneAsync(filter, update);
        }

        public async Task DislikeRecipe(ObjectId userId, ObjectId recipeId)
        {
            var filter = Builders<Recipes>.Filter.Eq(r => r.Id, recipeId);
            var update = Builders<Recipes>.Update.PullFilter(
                r => r.Likes,
                Builders<ObjectId>.Filter.Eq(savedUserId => savedUserId, userId)
            );

            await _recipesCollection.UpdateOneAsync(filter, update);
        }

        public async Task<List<RecipeWithUserModel>> GetRecipesByUserId(ObjectId userId)
        {
            var RecipeList = await _recipesCollection
                .Aggregate()
                .Lookup<Recipes, Users, RecipeWithUserModel>(
                    foreignCollection: _usersCollection,
                    localField: rec => rec.CreatorId,
                    foreignField: u => u.Id,
                    @as: ram => ram.User
                )
                .Match(r => r.CreatorId == userId)
                .SortByDescending(r => r.CreatedAt)
                .ToListAsync();

            return RecipeList;
        }

        public async Task<List<RecipeWithUserModel>> GetRecipesBySubscriptions(ObjectId userId)
        {
            {
                var userSubscriptions = await _subscriptionsCollection
                    .Find(sub => sub.UserId == userId)
                    .FirstOrDefaultAsync();

                if (userSubscriptions == null)
                    return new List<RecipeWithUserModel>();

                var subscribedUserIds = userSubscriptions.Subscribers;

                var pipeline = await _recipesCollection
                    .Aggregate()
                    .Lookup(
                        foreignCollection: _usersCollection,
                        localField: recipe => recipe.CreatorId,
                        foreignField: user => user.Id,
                        @as: (RecipeWithUserModel recipeWithUser) => recipeWithUser.User
                    )
                    .Match(recipe => subscribedUserIds.Contains(recipe.User.Id))
                    .ToListAsync();

                return pipeline;
            }
        }

        public async Task LikeRecipe(ObjectId userId, ObjectId recipeId)
        {
            var filter = Builders<Recipes>.Filter.And(
                Builders<Recipes>.Filter.Eq(r => r.Id, recipeId),
                Builders<Recipes>.Filter.Not(
                    Builders<Recipes>.Filter.ElemMatch(
                        r => r.Likes,
                        savedUserId => savedUserId == userId
                    )
                )
            );

            var update = Builders<Recipes>.Update.AddToSet(r => r.Likes, userId);

            await _recipesCollection.UpdateOneAsync(filter, update);
        }

        public async Task SaveRecipe(ObjectId userId, ObjectId recipeId)
        {
            var filter = Builders<Recipes>.Filter.And(
                Builders<Recipes>.Filter.Eq(r => r.Id, recipeId),
                Builders<Recipes>.Filter.Not(
                    Builders<Recipes>.Filter.ElemMatch(
                        r => r.Saved,
                        savedUserId => savedUserId == userId
                    )
                )
            );

            var update = Builders<Recipes>.Update.AddToSet(r => r.Saved, userId);

            await _recipesCollection.UpdateOneAsync(filter, update);
        }

        public async Task UpdateRecipe(Recipes recipe)
        {
            var filter = Builders<Recipes>.Filter.Eq(r => r.Id, recipe.Id);
            var options = new ReplaceOptions { IsUpsert = false };

            await _recipesCollection.ReplaceOneAsync(filter, recipe, options);
        }

        public async Task<List<RecipeWithUserModel>> GetSavedRecipesByUserid(ObjectId userId)
        {
            var RecipeList = await _recipesCollection
                .Aggregate()
                .Lookup<Recipes, Users, RecipeWithUserModel>(
                    foreignCollection: _usersCollection,
                    localField: rec => rec.CreatorId,
                    foreignField: u => u.Id,
                    @as: ram => ram.User
                )
                .Match(r => r.Saved.Contains(userId))
                .SortByDescending(r => r.CreatedAt)
                .ToListAsync();

            return RecipeList;
        }

        public async Task<Recipes> GetRecipeById(ObjectId recipeId)
        {
            return await _recipesCollection
                .AsQueryable()
                .Where(r => r.Id == recipeId)
                .FirstOrDefaultAsync();
        }

        public async Task<List<RecipeWithUserModel>> GetRecipesBySearch(string query)
        {
            return await _recipesCollection
                .Aggregate()
                .Lookup<Recipes, Users, RecipeWithUserModel>(
                    foreignCollection: _usersCollection,
                    localField: rec => rec.CreatorId,
                    foreignField: u => u.Id,
                    @as: ram => ram.User
                )
                .Match(ram => ram.Name.Contains(query))
                .ToListAsync();
        }
    }
}
