using System.Text.Json.Serialization;

namespace Mem0.NET;

/// <summary>
/// Ping响应
/// </summary>
public class PingResponse
{
    [JsonPropertyName("user_email")] public string? UserEmail { get; set; }
    [JsonPropertyName("org_id")] public string? OrgId { get; set; }
    [JsonPropertyName("project_id")] public string? ProjectId { get; set; }
}