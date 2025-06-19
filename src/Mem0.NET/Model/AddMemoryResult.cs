using System.Text.Json.Serialization;

namespace Mem0.NET;

public class AddMemoryResult
{
    [JsonPropertyName("id")] public string Id { get; set; } = string.Empty;

    [JsonPropertyName("memory")] public string Memory { get; set; } = string.Empty;

    [JsonPropertyName("event")] public string Event { get; set; } = string.Empty;
}