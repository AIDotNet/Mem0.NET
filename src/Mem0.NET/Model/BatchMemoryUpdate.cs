using System.Text.Json.Serialization;

namespace Mem0.NET;

/// <summary>
/// 批量更新记忆
/// </summary>
public class BatchMemoryUpdate
{
    [JsonPropertyName("memory_id")] public string MemoryId { get; set; } = string.Empty;

    [JsonPropertyName("text")] public string Text { get; set; } = string.Empty;
}