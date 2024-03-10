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
        Task AddCode(ConfirmationCodes confirmationCode);
        Task<string> GetConfirmationCode(ObjectId userId);
    }

    public class ConfirmationCodesRepository : IConfirmationCodesRepository
    {
        private readonly IMongoCollection<ConfirmationCodes> _collection;

        public ConfirmationCodesRepository(DBContext context)
        {
            _collection = context.GetCollection<ConfirmationCodes>();
        }

        public async Task AddCode(ConfirmationCodes confirmationCode)
        {
            await _collection.InsertOneAsync(confirmationCode);
        }

        public async Task<string> GetConfirmationCode(ObjectId userId)
        {
            var confirmation = await _collection
                .Find(e => e.UserId == userId)
                .FirstOrDefaultAsync();

            return confirmation.ConfirmationCode;
        }
    }
}
