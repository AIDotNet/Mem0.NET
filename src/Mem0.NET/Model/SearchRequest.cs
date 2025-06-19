using System.Text.Json.Serialization;

namespace Mem0.NET;

/// <summary>
/// 搜索请求模型
/// </summary>
public class SearchRequest
{
    [JsonPropertyName("query")] public string Query { get; set; } = string.Empty;

    [JsonPropertyName("user_id")] public string? UserId { get; set; }

    [JsonPropertyName("run_id")] public string? RunId { get; set; }

    [JsonPropertyName("agent_id")] public string? AgentId { get; set; }

    [JsonPropertyName("filters")] public Dictionary<string, object>? Filters { get; set; }
}