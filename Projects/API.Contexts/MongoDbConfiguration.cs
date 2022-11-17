using API.Contexts.Interfaces;

namespace API.Contexts
{
    public class MongoDbConfiguration : IMongoDbConfiguration
    {
        public string ConnectionString { get; set; }
        public string Database { get; set; }
    }
}