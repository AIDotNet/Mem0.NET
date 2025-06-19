using System.Text.Json.Serialization;

namespace Mem0.NET;

/// <summary>
/// 实体响应
/// </summary>
public class EntitiesResponse
{
    [JsonPropertyName("results")] public List<EntityInfo> Results { get; set; } = new();
}