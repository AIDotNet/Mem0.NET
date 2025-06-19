using System.Text.Json.Serialization;

namespace Mem0.NET;

/// <summary>
/// 消息模型
/// </summary>
public class Message
{
    [JsonPropertyName("role")] public string Role { get; set; } = string.Empty;
    
    [JsonPropertyName("content")] public string Content { get; set; } = string.Empty;
}