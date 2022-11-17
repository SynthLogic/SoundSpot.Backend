namespace API.Contexts.Interfaces
{
    public interface IConfigurationContext
    {
        MongoDbConfiguration MongoDbConfiguration { get; set; }
        CorsConfiguration CorsConfiguration { get; set; }
    }
}