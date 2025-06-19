using System.Text.Json.Serialization;

namespace Mem0.NET;

/// <summary>
/// 实体信息
/// </summary>
public class EntityInfo
{
    [JsonPropertyName("type")] public string Type { get; set; } = string.Empty;
    [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
}