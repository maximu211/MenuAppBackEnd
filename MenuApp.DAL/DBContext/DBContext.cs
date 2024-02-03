using MongoDB.Driver;

namespace MenuApp.DAL.DataBaseContext
{
    public class DBContext
    {
        private readonly IMongoDatabase _database;

        public DBContext(string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }

        public IMongoCollection<T> GetCollection<T>()
        {
            return _database.GetCollection<T>(typeof(T).Name);
        }
    }
}
