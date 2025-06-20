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
    
    /// <summary>
    /// 搜索相似度阈值，范围从0到1，默认值为0.3
    /// </summary>
    [JsonPropertyName("threshold")]
    public double Threshold { get; set; } = 0.3;
    
    /// <summary>
    /// 搜索结果的最大数量，默认值为10
    /// </summary>
    [JsonPropertyName("limit")]
    public int Limit { get; set; } = 10;
}