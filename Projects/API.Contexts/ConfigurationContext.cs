using API.Contexts.Interfaces;

namespace API.Contexts
{
    public class ConfigurationContext : IConfigurationContext
    {
        public const string Name = "ConfigurationContext";
        public MongoDbConfiguration MongoDbConfiguration { get; set; }
    }
}