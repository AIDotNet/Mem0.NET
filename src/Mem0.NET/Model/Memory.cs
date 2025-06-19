using System.Text.Json.Serialization;

namespace Mem0.NET;

/// <summary>
/// 记忆项模型
/// </summary>
public class Memory
{
    [JsonPropertyName("id")] public string Id { get; set; } = string.Empty;
    [JsonPropertyName("memory")] public string Content { get; set; } = string.Empty;
    [JsonPropertyName("text")] public string Text { get; set; } = string.Empty;
    [JsonPropertyName("user_id")] public string? UserId { get; set; }
    [JsonPropertyName("agent_id")] public string? AgentId { get; set; }
    [JsonPropertyName("app_id")] public string? AppId { get; set; }
    [JsonPropertyName("run_id")] public string? RunId { get; set; }
    [JsonPropertyName("metadata")] public Dictionary<string, object>? Metadata { get; set; }
    [JsonPropertyName("created_at")] public DateTime? CreatedAt { get; set; }
    [JsonPropertyName("updated_at")] public DateTime? UpdatedAt { get; set; }
    [JsonPropertyName("score")] public double? Score { get; set; }
}


public class SearchResult
{
    public Memory[] results { get; set; }
    
    public object[] relations { get; set; }
}

