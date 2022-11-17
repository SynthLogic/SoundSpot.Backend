using MongoDB.Driver;

namespace API.Contexts.Interfaces
{
    public interface IMongoDbContext
    {
        bool TryGetDatabase(out IMongoDatabase database);
    }
}