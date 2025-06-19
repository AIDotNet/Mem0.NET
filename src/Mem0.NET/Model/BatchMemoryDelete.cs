using System.Text.Json.Serialization;

namespace Mem0.NET;

/// <summary>
/// 批量删除记忆
/// </summary>
public class BatchMemoryDelete
{
    [JsonPropertyName("memory_id")] public string MemoryId { get; set; } = string.Empty;
}