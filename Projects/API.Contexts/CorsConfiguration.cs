using API.Contexts.Interfaces;

namespace API.Contexts
{
    public class CorsConfiguration : ICorsContext
    {
        public const string Name = "CorsConfiguration";
        public string PolicyName { get; set; }
        public string[] ValidOrigins { get; set; }

        public CorsConfiguration()
        {
            PolicyName = "Default";
        }
    }
}