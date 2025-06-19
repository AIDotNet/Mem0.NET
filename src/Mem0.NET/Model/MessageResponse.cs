using System.Text.Json.Serialization;

namespace Mem0.NET;

/// <summary>
/// 简单消息响应
/// </summary>
public class MessageResponse
{
    [JsonPropertyName("message")] public string Message { get; set; } = string.Empty;
}