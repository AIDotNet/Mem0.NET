using System.Text.Json.Serialization;

namespace Mem0.NET;

/// <summary>
/// 添加记忆响应
/// </summary>
public class AddMemoryResponse
{
    [JsonPropertyName("results")] public List<AddMemoryResult> Results { get; set; } = new();
}