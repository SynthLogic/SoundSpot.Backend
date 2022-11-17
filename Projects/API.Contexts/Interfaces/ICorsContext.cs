namespace API.Contexts.Interfaces
{
    public interface ICorsContext
    {
        string PolicyName { get; set; }
        string[] ValidOrigins { get; set; }
    }
}