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
        Task<string> GetConfirmationCodeByUserId(ObjectId userId);
        Task DeleteExpiredCodes();
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
            var options = new FindOneAndReplaceOptions<ConfirmationCodes, ConfirmationCodes>
            {
                IsUpsert = true,
                ReturnDocument = ReturnDocument.After
            };

            await _collection.FindOneAndReplaceAsync(filter, code, options);
        }

        public async Task<string> GetConfirmationCodeByUserId(ObjectId userId)
        {
            var confirmation = await _collection
                .Find(e => e.UserId == userId)
                .FirstOrDefaultAsync();

            return confirmation.ConfirmationCode;
        }

        public async Task DeleteExpiredCodes()
        {
            var thresholdDate = DateTime.UtcNow.AddMinutes(1);

            var filter = Builders<ConfirmationCodes>.Filter.Lt(x => x.CreatedAt, thresholdDate);
            await _collection.DeleteManyAsync(filter);
        }

        public async Task DeleteConfirmationCodeByUserId(ObjectId userId)
        {
            var filter = Builders<ConfirmationCodes>.Filter.Eq(x => x.UserId, userId);
            await _collection.FindOneAndDeleteAsync(filter);
        }
    }
}
