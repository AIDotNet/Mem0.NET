using System.Text.Json.Serialization;

namespace Mem0.NET;

/// <summary>
/// 获取所有记忆响应
/// </summary>
public class GetAllMemoriesResponse
{
    [JsonPropertyName("results")] public List<Memory> Results { get; set; } = new();
    [JsonPropertyName("next_token")] public string? NextToken { get; set; }
}