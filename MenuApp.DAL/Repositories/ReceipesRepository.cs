using System;
using System.Collections.Generic;
using System.Linq;
using MenuApp.DAL.DataBaseContext;
using MenuApp.DAL.Models.AggregetionModels.ReceiptAggregetionModels;
using MenuApp.DAL.Models.EntityModels;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MenuApp.DAL.Repositories
{
    public interface IReceipesRepository
    {
        Task<List<ReceipeUserModel>> GetReceiptsByUserId(ObjectId userId);
        Task<List<ReceipeUserModel>> GetReceiptsBySubscriptions(ObjectId userId);
        Task<List<ReceipeUserModel>> GetSavedReceiptsByUserid(ObjectId userId);
        Task DeleteReceipt(ObjectId receiptId);
        Task UpdateReceipt(Receipes receipt);
        Task AddReceipt(Receipes receipt);
        Task LikeReceipt(ObjectId userId, ObjectId receiptId);
        Task SaveReceipt(ObjectId userId, ObjectId receiptId);
        Task DislikeReceipt(ObjectId userId, ObjectId receiptId);
        Task DeleteReceiptFromSaved(ObjectId userId, ObjectId receiptId);
    }

    public class ReceipesRepository : IReceipesRepository
    {
        private readonly IMongoCollection<Receipes> _receipesСollection;
        private readonly IMongoCollection<Users> _usersCollection;
        private readonly IMongoCollection<Subscriptions> _subscriptionsCollection;

        public ReceipesRepository(DBContext context)
        {
            _receipesСollection = context.GetCollection<Receipes>();
            _usersCollection = context.GetCollection<Users>();
            _subscriptionsCollection = context.GetCollection<Subscriptions>();
        }

        public async Task AddReceipt(Receipes receipt)
        {
            await _receipesСollection.InsertOneAsync(receipt);
        }

        public async Task DeleteReceipt(ObjectId receiptId)
        {
            await _receipesСollection.DeleteOneAsync(r => r.Id == receiptId);
        }

        public async Task DeleteReceiptFromSaved(ObjectId userId, ObjectId receiptId)
        {
            var filter = Builders<Receipes>.Filter.Eq(r => r.Id, receiptId);
            var update = Builders<Receipes>.Update.PullFilter(
                r => r.Saved,
                Builders<ObjectId>.Filter.Eq(savedUserId => savedUserId, userId)
            );

            await _receipesСollection.UpdateOneAsync(filter, update);
        }

        public async Task DislikeReceipt(ObjectId userId, ObjectId receiptId)
        {
            var filter = Builders<Receipes>.Filter.Eq(r => r.Id, receiptId);
            var update = Builders<Receipes>.Update.PullFilter(
                r => r.Likes,
                Builders<ObjectId>.Filter.Eq(savedUserId => savedUserId, userId)
            );

            await _receipesСollection.UpdateOneAsync(filter, update);
        }

        public async Task<List<ReceipeUserModel>> GetReceiptsByUserId(ObjectId userId)
        {
            var receiptList = await _receipesСollection
                .Aggregate()
                .Lookup<Receipes, Users, ReceipeUserModel>(
                    foreignCollection: _usersCollection,
                    localField: rec => rec.CreatorId,
                    foreignField: u => u.Id,
                    @as: ram => ram.User
                )
                .Match(r => r.CreatorId == userId)
                .SortByDescending(r => r.CreatedAt)
                .ToListAsync();

            return receiptList;
        }

        public async Task<List<ReceipeUserSubModel>> GetReceiptsBySubscriptions(ObjectId userId)
        {
            var receiptList = await _subscriptionsCollection
                .Aggregate()
                .Match(sub => sub.UserId == userId) // Фільтрація підписок за userId
                .Lookup<Subscriptions, Receipes, ReceipeSubModel>(
                    foreignCollection: _receipesСollection,
                    localField: sub => sub.Subscribers, // Поле підписників у колекції підписок
                    foreignField: rec => rec.CreatorId, // Поле створювача у колекції рецептів
                    @as: rsm => rsm.UserReceipts // Псевдонім для результатів з'єднання
                )
                .Unwind<ReceipeSubModel, Receipes>(rsm => rsm.UserReceipts) // Розгортання результатів з'єднання
                .Lookup<Receipes, Users, ReceipeUserSubModel>(
                    foreignCollection: _usersCollection,
                    localField: rec => rec.CreatorId,
                    foreignField: user => user.Id,
                    @as: (rec, user) => new ReceipeUserSubModel { Receipe = rec, User = user }
                )
                .SortByDescending(rusm => rusm.Receipe.CreatedAt) // Сортування за датою створення рецепту
                .ToListAsync();

            return receiptList;
        }

        public async Task LikeReceipt(ObjectId userId, ObjectId receiptId)
        {
            var filter = Builders<Receipes>.Filter.And(
                Builders<Receipes>.Filter.Eq(r => r.Id, receiptId),
                Builders<Receipes>.Filter.Not(
                    Builders<Receipes>.Filter.ElemMatch(
                        r => r.Likes,
                        savedUserId => savedUserId == userId
                    )
                )
            );

            var update = Builders<Receipes>.Update.AddToSet(r => r.Likes, userId);

            await _receipesСollection.UpdateOneAsync(filter, update);
        }

        public async Task SaveReceipt(ObjectId userId, ObjectId receiptId)
        {
            var filter = Builders<Receipes>.Filter.And(
                Builders<Receipes>.Filter.Eq(r => r.Id, receiptId),
                Builders<Receipes>.Filter.Not(
                    Builders<Receipes>.Filter.ElemMatch(
                        r => r.Saved,
                        savedUserId => savedUserId == userId
                    )
                )
            );

            var update = Builders<Receipes>.Update.AddToSet(r => r.Saved, userId);

            await _receipesСollection.UpdateOneAsync(filter, update);
        }

        public async Task UpdateReceipt(Receipes receipt)
        {
            var filter = Builders<Receipes>.Filter.Eq(r => r.Id, receipt.Id);
            var options = new ReplaceOptions { IsUpsert = false };

            await _receipesСollection.ReplaceOneAsync(filter, receipt, options);
        }

        public Task<List<ReceipeUserModel>> GetSavedReceiptsByUserid(ObjectId userId)
        {
            throw new NotImplementedException();
        }
    }

    public class ReceipeSubModel
    {
        public ObjectId Id { get; set; } // Ідентифікатор рецепту
        public ObjectId CreatorId { get; set; } // Ідентифікатор створювача рецепту
        // Додайте інші поля, які ви хочете включити з колекції рецептів
    }
}
