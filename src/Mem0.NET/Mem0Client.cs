using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Mem0.NET;

public class Mem0Client
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;
    private bool _disposed = false;
    private string? _apiKey;

    public Mem0Client(string baseUrl,
        string? apiKey,
        HttpClient? httpClient = null)
    {
        _httpClient = httpClient ?? new HttpClient();
        _httpClient.BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/");
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mem0.NET/1.0");
        _apiKey = apiKey;
        if (!string.IsNullOrEmpty(_apiKey))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", _apiKey);
        }

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
    }

    /// <summary>
    /// 配置Mem0
    /// </summary>
    public async Task<MessageResponse> ConfigureAsync(Dictionary<string, object> config,
        CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(config, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("configure", content, cancellationToken);
        await EnsureSuccessStatusCodeAsync(response);

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<MessageResponse>(responseContent, _jsonOptions) ?? new MessageResponse();
    }

    /// <summary>
    /// 创建内存
    /// </summary>
    public async Task<List<Memory>> CreateMemoryAsync(MemoryCreateRequest request,
        CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(request, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("memories", content, cancellationToken);
        await EnsureSuccessStatusCodeAsync(response);

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<List<Memory>>(responseContent, _jsonOptions) ?? new List<Memory>();
    }

    /// <summary>
    /// 获取所有内存
    /// </summary>
    public async Task<List<Memory>> GetAllMemoriesAsync(string? userId = null, string? runId = null,
        string? agentId = null, CancellationToken cancellationToken = default)
    {
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(userId)) queryParams.Add($"user_id={Uri.EscapeDataString(userId)}");
        if (!string.IsNullOrEmpty(runId)) queryParams.Add($"run_id={Uri.EscapeDataString(runId)}");
        if (!string.IsNullOrEmpty(agentId)) queryParams.Add($"agent_id={Uri.EscapeDataString(agentId)}");

        var url = "memories";
        if (queryParams.Count > 0)
        {
            url += "?" + string.Join("&", queryParams);
        }

        var response = await _httpClient.GetAsync(url, cancellationToken);
        await EnsureSuccessStatusCodeAsync(response);

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<List<Memory>>(responseContent, _jsonOptions) ?? new List<Memory>();
    }

    /// <summary>
    /// 根据ID获取特定内存
    /// </summary>
    public async Task<Memory> GetMemoryAsync(string memoryId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"memories/{Uri.EscapeDataString(memoryId)}", cancellationToken);
        await EnsureSuccessStatusCodeAsync(response);

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<Memory>(responseContent, _jsonOptions) ?? new Memory();
    }

    /// <summary>
    /// 搜索内存
    /// </summary>
    public async Task<List<Memory>> SearchMemoriesAsync(SearchRequest request,
        CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(request, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("search", content, cancellationToken);
        await EnsureSuccessStatusCodeAsync(response);

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<List<Memory>>(responseContent, _jsonOptions) ?? new List<Memory>();
    }

    /// <summary>
    /// 更新内存
    /// </summary>
    public async Task<Memory> UpdateMemoryAsync(string memoryId, Dictionary<string, object> updatedMemory,
        CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(updatedMemory, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response =
            await _httpClient.PutAsync($"memories/{Uri.EscapeDataString(memoryId)}", content, cancellationToken);
        await EnsureSuccessStatusCodeAsync(response);

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<Memory>(responseContent, _jsonOptions) ?? new Memory();
    }

    /// <summary>
    /// 获取内存历史
    /// </summary>
    public async Task<List<Memory>> GetMemoryHistoryAsync(string memoryId,
        CancellationToken cancellationToken = default)
    {
        var response =
            await _httpClient.GetAsync($"memories/{Uri.EscapeDataString(memoryId)}/history", cancellationToken);
        await EnsureSuccessStatusCodeAsync(response);

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<List<Memory>>(responseContent, _jsonOptions) ?? new List<Memory>();
    }

    /// <summary>
    /// 删除特定内存
    /// </summary>
    public async Task<MessageResponse> DeleteMemoryAsync(string memoryId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.DeleteAsync($"memories/{Uri.EscapeDataString(memoryId)}", cancellationToken);
        await EnsureSuccessStatusCodeAsync(response);

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<MessageResponse>(responseContent, _jsonOptions) ?? new MessageResponse();
    }

    /// <summary>
    /// 删除所有内存
    /// </summary>
    public async Task<MessageResponse> DeleteAllMemoriesAsync(string? userId = null, string? runId = null,
        string? agentId = null, CancellationToken cancellationToken = default)
    {
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(userId)) queryParams.Add($"user_id={Uri.EscapeDataString(userId)}");
        if (!string.IsNullOrEmpty(runId)) queryParams.Add($"run_id={Uri.EscapeDataString(runId)}");
        if (!string.IsNullOrEmpty(agentId)) queryParams.Add($"agent_id={Uri.EscapeDataString(agentId)}");

        var url = "memories";
        if (queryParams.Count > 0)
        {
            url += "?" + string.Join("&", queryParams);
        }

        var response = await _httpClient.DeleteAsync(url, cancellationToken);
        await EnsureSuccessStatusCodeAsync(response);

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<MessageResponse>(responseContent, _jsonOptions) ?? new MessageResponse();
    }

    /// <summary>
    /// 重置所有内存
    /// </summary>
    public async Task<MessageResponse> ResetMemoryAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsync("reset", null, cancellationToken);
        await EnsureSuccessStatusCodeAsync(response);

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<MessageResponse>(responseContent, _jsonOptions) ?? new MessageResponse();
    }

    private static async Task EnsureSuccessStatusCodeAsync(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"API请求失败: {response.StatusCode} - {response.ReasonPhrase}. 详情: {errorContent}");
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _httpClient?.Dispose();
            _disposed = true;
        }
    }
}

/// <summary>
/// 消息模型
/// </summary>
public class Message
{
    [JsonPropertyName("role")] public string Role { get; set; } = string.Empty;

    [JsonPropertyName("content")] public string Content { get; set; } = string.Empty;
}

/// <summary>
/// 创建内存的请求模型
/// </summary>
public class MemoryCreateRequest
{
    [JsonPropertyName("messages")] public List<Message> Messages { get; set; } = new List<Message>();

    [JsonPropertyName("user_id")] public string? UserId { get; set; }

    [JsonPropertyName("agent_id")] public string? AgentId { get; set; }

    [JsonPropertyName("run_id")] public string? RunId { get; set; }

    [JsonPropertyName("metadata")] public Dictionary<string, object>? Metadata { get; set; }
}

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

/// <summary>
/// 内存项模型
/// </summary>
public class Memory
{
    [JsonPropertyName("id")] public string Id { get; set; } = string.Empty;

    [JsonPropertyName("memory")] public string Content { get; set; } = string.Empty;

    [JsonPropertyName("user_id")] public string? UserId { get; set; }

    [JsonPropertyName("agent_id")] public string? AgentId { get; set; }

    [JsonPropertyName("run_id")] public string? RunId { get; set; }

    [JsonPropertyName("metadata")] public Dictionary<string, object>? Metadata { get; set; }

    [JsonPropertyName("created_at")] public DateTime? CreatedAt { get; set; }

    [JsonPropertyName("updated_at")] public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// API响应基类
/// </summary>
public class ApiResponse<T>
{
    [JsonPropertyName("data")] public T? Data { get; set; }

    [JsonPropertyName("message")] public string? Message { get; set; }

    [JsonPropertyName("success")] public bool Success { get; set; } = true;
}

/// <summary>
/// 简单消息响应
/// </summary>
public class MessageResponse
{
    [JsonPropertyName("message")] public string Message { get; set; } = string.Empty;
}