using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MenuApp.DAL.DataBaseContext;
using MenuApp.DAL.Models.EntityModels;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MenuApp.DAL.Repositories
{
    public interface IReceiptsRepository
    {
        Task<List<Receipts>> GetReceiptsById(ObjectId userId);
        Task<List<Receipts>> GetReceiptsBySubscriptions(ObjectId userId);
        Task DeleteReceipt(ObjectId receiptId);
        Task UpdateReceipt(Receipts receipt);
        Task AddReceipt(Receipts receipt);
        Task LikeReceipt(ObjectId userId, ObjectId receiptId);
        Task SaveReceipt(ObjectId userId, ObjectId receiptId);
        Task DislikeReceipt(ObjectId userId, ObjectId receiptId);
        Task DeleteReceiptFromSaved(ObjectId userId, ObjectId receiptId);
    }

    public class ReceiptsRepository : IReceiptsRepository
    {
        private readonly IMongoCollection<Receipts> _receiptСollection;
        private readonly IMongoCollection<Users> _userCollection;
        private readonly IMongoCollection<Subscriptions> _subscriptionCollection;

        public ReceiptsRepository(DBContext context)
        {
            _receiptСollection = context.GetCollection<Receipts>();
            _userCollection = context.GetCollection<Users>();
            _subscriptionCollection = context.GetCollection<Subscriptions>();
        }

        public async Task AddReceipt(Receipts receipt)
        {
            await _receiptСollection.InsertOneAsync(receipt);
        }

        public Task DeleteReceipt(ObjectId receiptId)
        {
            throw new NotImplementedException();
        }

        public Task DeleteReceiptFromSaved(ObjectId userId, ObjectId receiptId)
        {
            throw new NotImplementedException();
        }

        public Task DislikeReceipt(ObjectId userId, ObjectId receiptId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Receipts>> GetReceiptsById(ObjectId userId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Receipts>> GetReceiptsBySubscriptions(ObjectId userId)
        {
            throw new NotImplementedException();
        }

        public Task LikeReceipt(ObjectId userId, ObjectId receiptId)
        {
            throw new NotImplementedException();
        }

        public Task SaveReceipt(ObjectId userId, ObjectId receiptId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateReceipt(Receipts receipt)
        {
            throw new NotImplementedException();
        }
    }
}
