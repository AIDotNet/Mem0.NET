namespace Mem0.NET.Options;

public sealed class Mem0ClientOptions
{
    public string? ApiKey { get; set; }
    
    public string Host { get; set; }
    
    public string? OrgId { get; set; }
    
    public string? ProjectId { get; set; }

    public HttpClient? HttpClient { get; set; }
}