using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MenuApp.DAL.DataBaseContext;
using MenuApp.DAL.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MenuApp.DAL.Repositories
{
    public interface IConfirmationCodesRepository
    {
        Task UpsertConfirmationCode(ConfirmationCodes code);
        Task<ConfirmationCodes> GetConfirmationCodeByUserId(ObjectId userId);
        Task DeleteConfirmationCodeByUserId(ObjectId userId);
    }

    public class ConfirmationCodesRepository : IConfirmationCodesRepository
    {
        private readonly IMongoCollection<ConfirmationCodes> _collection;

        public ConfirmationCodesRepository(DBContext context)
        {
            _collection = context.GetCollection<ConfirmationCodes>();
        }

        public async Task UpsertConfirmationCode(ConfirmationCodes code)
        {
            var filter = Builders<ConfirmationCodes>.Filter.Eq(x => x.UserId, code.UserId);
            var existingCode = await _collection.Find(filter).FirstOrDefaultAsync();

            if (existingCode == null)
            {
                await _collection.InsertOneAsync(code);
            }
            else
            {
                await _collection.ReplaceOneAsync(filter, code);
            }
        }

        public async Task<ConfirmationCodes> GetConfirmationCodeByUserId(ObjectId userId)
        {
            var confirmationCode = await _collection
                .Find(e => e.UserId == userId)
                .FirstOrDefaultAsync();

            return confirmationCode;
        }

        public async Task DeleteConfirmationCodeByUserId(ObjectId userId)
        {
            var filter = Builders<ConfirmationCodes>.Filter.Eq(x => x.UserId, userId);
            await _collection.FindOneAndDeleteAsync(filter);
        }
    }
}
