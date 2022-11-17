using API.Contexts.Interfaces;
using MongoDB.Driver;

namespace API.Contexts
{
    public class MongoDbContext : IMongoDbContext
    {
        private readonly IMongoDbConfiguration _configuration;
        private readonly MongoClient _client;
        private readonly IMongoDatabase _database;

        public MongoDbContext(MongoDbConfiguration configuration)
        {
            _configuration = configuration;
            _client = new MongoClient(_configuration.ConnectionString);
            _database = _client.GetDatabase(_configuration.Database);
        }

        public bool TryGetDatabase(out IMongoDatabase database)
        {
            database = _database;
            return !(_database is null);
        }
    }
}