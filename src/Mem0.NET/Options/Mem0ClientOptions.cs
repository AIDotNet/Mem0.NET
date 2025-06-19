namespace Mem0.NET.Options;

public sealed class Mem0ClientOptions
{
    public string? ApiKey { get; set; }

    public string BaseUrl { get; set; }

    public HttpClient? HttpClient { get; set; }
}