namespace API.Contexts.Interfaces
{
    public interface IMongoDbConfiguration
    {
        string ConnectionString { get; set; }
        string Database { get; set; }
    }
}