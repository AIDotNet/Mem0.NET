using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Mem0.NET;

/// <summary>
/// Mem0 API异常
/// </summary>
public class Mem0ApiException : Exception
{
    public Mem0ApiException(string message) : base(message)
    {
    }

    public Mem0ApiException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

/// <summary>
/// Mem0 .NET 客户端
/// </summary>
public class Mem0Client : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly string _apiKey;
    private readonly string _userId;
    private bool _disposed = false;

    public string? OrgId { get; private set; }
    public string? ProjectId { get; private set; }
    public string? UserEmail { get; private set; }

    public Mem0Client(string? apiKey = null,
        string host = "https://api.mem0.ai",
        string? orgId = null,
        string? projectId = null,
        HttpClient? httpClient = null)
    {
        _apiKey = apiKey ?? Environment.GetEnvironmentVariable("MEM0_API_KEY")
            ?? throw new ArgumentException("Mem0 API Key not provided. Please provide an API Key.");

        OrgId = orgId;
        ProjectId = projectId;

        // 创建API密钥的MD5哈希作为用户ID
        using var md5 = MD5.Create();
        var apiKeyBytes = Encoding.UTF8.GetBytes(_apiKey);
        var hashBytes = md5.ComputeHash(apiKeyBytes);
        _userId = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

        _httpClient = httpClient ?? new HttpClient();
        _httpClient.BaseAddress = new Uri(host.TrimEnd('/') + "/");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", _apiKey);
        _httpClient.DefaultRequestHeaders.Add("Mem0-User-ID", _userId);
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mem0.NET/1.0");
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _httpClient.Timeout = TimeSpan.FromSeconds(300);

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
    }

    /// <summary>
    /// 添加新记忆
    /// </summary>
    public async Task AddAsync(List<Message> messages,
        string? userId = null,
        string? agentId = null,
        string? appId = null,
        string? runId = null,
        Dictionary<string, object>? metadata = null,
        Dictionary<string, object>? filters = null,
        string outputFormat = ".1",
        CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, object>
        {
            ["user_id"] = userId,
            ["agent_id"] = agentId,
            ["app_id"] = appId,
            ["run_id"] = runId,
            ["metadata"] = metadata,
            ["filters"] = filters,
            ["output_format"] = outputFormat,
            ["version"] = "v2"
        };

        var payload = PreparePayload(messages, parameters);
        var json = JsonSerializer.Serialize(payload, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("memories/", content, cancellationToken);
        await EnsureSuccessStatusCodeAsync(response);

        await response.Content.ReadAsStringAsync(cancellationToken);
    }

    /// <summary>
    /// 根据ID获取特定记忆
    /// </summary>
    public async Task<Memory> GetAsync(string memoryId, CancellationToken cancellationToken = default)
    {
        var queryParams = PrepareParams(new Dictionary<string, object>());
        var response =
            await _httpClient.GetAsync($"memories/{Uri.EscapeDataString(memoryId)}/?{BuildQueryString(queryParams)}",
                cancellationToken);
        await EnsureSuccessStatusCodeAsync(response);

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<Memory>(responseContent, _jsonOptions) ?? new Memory();
    }

    /// <summary>
    /// 获取所有记忆
    /// </summary>
    public async Task<GetAllMemoriesResponse> GetAllAsync(string version = "",
        string? userId = null,
        string? agentId = null,
        string? appId = null,
        string? runId = null,
        int? topK = null,
        int? page = null,
        int? pageSize = null,
        Dictionary<string, object>? metadata = null,
        CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, object>
        {
            ["user_id"] = userId,
            ["agent_id"] = agentId,
            ["app_id"] = appId,
            ["run_id"] = runId,
            ["top_k"] = topK,
            ["metadata"] = metadata
        };

        var preparedParams = PrepareParams(parameters);

        HttpResponseMessage response;
        if (version == "")
        {
            response = await _httpClient.GetAsync($"{version}/memories/?{BuildQueryString(preparedParams)}",
                cancellationToken);
        }
        else if (version == "v2")
        {
            var queryParams = new Dictionary<string, object>();
            if (page.HasValue) queryParams["page"] = page.Value;
            if (pageSize.HasValue) queryParams["page_size"] = pageSize.Value;

            var json = JsonSerializer.Serialize(preparedParams, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var url = $"{version}/memories/";
            if (queryParams.Count > 0)
                url += $"?{BuildQueryString(queryParams)}";

            response = await _httpClient.PostAsync(url, content, cancellationToken);
        }
        else
        {
            throw new ArgumentException($"Unsupported version: {version}");
        }

        await EnsureSuccessStatusCodeAsync(response);
        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<GetAllMemoriesResponse>(responseContent, _jsonOptions) ??
               new GetAllMemoriesResponse();
    }

    /// <summary>
    /// 搜索记忆
    /// </summary>
    public async Task<List<Memory>> SearchAsync(string query,
        string version = "",
        string? userId = null,
        string? agentId = null,
        string? appId = null,
        string? runId = null,
        int? topK = null,
        Dictionary<string, object>? filters = null,
        Dictionary<string, object>? metadata = null,
        CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, object>
        {
            ["user_id"] = userId,
            ["agent_id"] = agentId,
            ["app_id"] = appId,
            ["run_id"] = runId,
            ["top_k"] = topK,
            ["filters"] = filters,
            ["metadata"] = metadata
        };

        var payload = new Dictionary<string, object> { ["query"] = query };
        payload = payload.Concat(PrepareParams(parameters)).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        var json = JsonSerializer.Serialize(payload, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{version}/memories/search/", content, cancellationToken);
        await EnsureSuccessStatusCodeAsync(response);

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<List<Memory>>(responseContent, _jsonOptions) ?? new List<Memory>();
    }

    /// <summary>
    /// 搜索内存
    /// </summary>
    public async Task<SearchResult> SearchMemoriesAsync(SearchRequest request,
        CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(request, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("search", content, cancellationToken);
        await EnsureSuccessStatusCodeAsync(response);

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<SearchResult>(responseContent, _jsonOptions);
    }
    
    /// <summary>
    /// 更新记忆
    /// </summary>
    public async Task<Memory> UpdateAsync(string memoryId,
        string? text = null,
        Dictionary<string, object>? metadata = null,
        CancellationToken cancellationToken = default)
    {
        if (text == null && metadata == null)
            throw new ArgumentException("Either text or metadata must be provided for update.");

        var payload = new Dictionary<string, object>();
        if (text != null) payload["text"] = text;
        if (metadata != null) payload["metadata"] = metadata;

        var queryParams = PrepareParams(new Dictionary<string, object>());
        var json = JsonSerializer.Serialize(payload, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PutAsync(
            $"memories/{Uri.EscapeDataString(memoryId)}/?{BuildQueryString(queryParams)}", content, cancellationToken);
        await EnsureSuccessStatusCodeAsync(response);

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<Memory>(responseContent, _jsonOptions) ?? new Memory();
    }

    /// <summary>
    /// 删除特定记忆
    /// </summary>
    public async Task<MessageResponse> DeleteAsync(string memoryId, CancellationToken cancellationToken = default)
    {
        var queryParams = PrepareParams(new Dictionary<string, object>());
        var response =
            await _httpClient.DeleteAsync($"memories/{Uri.EscapeDataString(memoryId)}/?{BuildQueryString(queryParams)}",
                cancellationToken);
        await EnsureSuccessStatusCodeAsync(response);

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<MessageResponse>(responseContent, _jsonOptions) ?? new MessageResponse();
    }

    /// <summary>
    /// 删除所有记忆
    /// </summary>
    public async Task<MessageResponse> DeleteAllAsync(
        string? userId = null,
        string? agentId = null,
        string? appId = null,
        string? runId = null,
        CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, object>
        {
            ["user_id"] = userId,
            ["agent_id"] = agentId,
            ["app_id"] = appId,
            ["run_id"] = runId
        };

        var queryParams = PrepareParams(parameters);
        var response = await _httpClient.DeleteAsync($"memories/?{BuildQueryString(queryParams)}", cancellationToken);
        await EnsureSuccessStatusCodeAsync(response);

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<MessageResponse>(responseContent, _jsonOptions) ?? new MessageResponse();
    }

    /// <summary>
    /// 获取记忆历史
    /// </summary>
    public async Task<List<Memory>> GetHistoryAsync(string memoryId, CancellationToken cancellationToken = default)
    {
        var queryParams = PrepareParams(new Dictionary<string, object>());
        var response = await _httpClient.GetAsync(
            $"memories/{Uri.EscapeDataString(memoryId)}/history/?{BuildQueryString(queryParams)}", cancellationToken);
        await EnsureSuccessStatusCodeAsync(response);

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<List<Memory>>(responseContent, _jsonOptions) ?? new List<Memory>();
    }

    /// <summary>
    /// 获取所有用户、代理和会话
    /// </summary>
    public async Task<EntitiesResponse> GetUsersAsync(CancellationToken cancellationToken = default)
    {
        var queryParams = PrepareParams(new Dictionary<string, object>());
        var response = await _httpClient.GetAsync($"entities/?{BuildQueryString(queryParams)}", cancellationToken);
        await EnsureSuccessStatusCodeAsync(response);

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<EntitiesResponse>(responseContent, _jsonOptions) ?? new EntitiesResponse();
    }

    /// <summary>
    /// 删除用户实体
    /// </summary>
    public async Task<MessageResponse> DeleteUsersAsync(
        string? userId = null,
        string? agentId = null,
        string? appId = null,
        string? runId = null,
        CancellationToken cancellationToken = default)
    {
        var entities = await GetUsersAsync(cancellationToken);
        var toDelete = new List<EntityInfo>();

        if (userId != null)
            toDelete.Add(new EntityInfo { Type = "user", Name = userId });
        else if (agentId != null)
            toDelete.Add(new EntityInfo { Type = "agent", Name = agentId });
        else if (appId != null)
            toDelete.Add(new EntityInfo { Type = "app", Name = appId });
        else if (runId != null)
            toDelete.Add(new EntityInfo { Type = "run", Name = runId });
        else
            toDelete.AddRange(entities.Results.Select(e => new EntityInfo { Type = e.Type, Name = e.Name }));

        if (!toDelete.Any())
            throw new ArgumentException("No entities to delete");

        var queryParams = PrepareParams(new Dictionary<string, object>());
        foreach (var entity in toDelete)
        {
            var response = await _httpClient.DeleteAsync(
                $"entities/{entity.Type}/{Uri.EscapeDataString(entity.Name)}/?{BuildQueryString(queryParams)}",
                cancellationToken);
            await EnsureSuccessStatusCodeAsync(response);
        }

        var message = userId != null || agentId != null || appId != null || runId != null
            ? "Entity deleted successfully."
            : "All users, agents, apps and runs deleted.";

        return new MessageResponse { Message = message };
    }

    /// <summary>
    /// 重置客户端
    /// </summary>
    public async Task<MessageResponse> ResetAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response =
                await _httpClient.PostAsync($"reset/",null,
                    cancellationToken);
            await EnsureSuccessStatusCodeAsync(response);

            await response.Content.ReadAsStringAsync(cancellationToken);
            
            return new MessageResponse { Message = "All memories reset" };
        }
        catch (Exception ex)
        {
            throw new Mem0ApiException("Error in ResetAsync", ex);
        }
    }

    /// <summary>
    /// 批量更新记忆
    /// </summary>
    public async Task<MessageResponse> BatchUpdateAsync(List<BatchMemoryUpdate> memories,
        CancellationToken cancellationToken = default)
    {
        var payload = new { memories };
        var json = JsonSerializer.Serialize(payload, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PutAsync("batch/", content, cancellationToken);
        await EnsureSuccessStatusCodeAsync(response);

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<MessageResponse>(responseContent, _jsonOptions) ?? new MessageResponse();
    }

    /// <summary>
    /// 批量删除记忆
    /// </summary>
    public async Task<MessageResponse> BatchDeleteAsync(List<BatchMemoryDelete> memories,
        CancellationToken cancellationToken = default)
    {
        var payload = new { memories };
        var json = JsonSerializer.Serialize(payload, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var request = new HttpRequestMessage(HttpMethod.Delete, "batch/")
        {
            Content = content
        };

        var response = await _httpClient.SendAsync(request, cancellationToken);
        await EnsureSuccessStatusCodeAsync(response);

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<MessageResponse>(responseContent, _jsonOptions) ?? new MessageResponse();
    }

    /// <summary>
    /// 提供反馈
    /// </summary>
    public async Task<MessageResponse> FeedbackAsync(string memoryId,
        MemoryFeedback? feedback = null,
        string? feedbackReason = null,
        CancellationToken cancellationToken = default)
    {
        var validFeedbacks = new[] { MemoryFeedback.Positive, MemoryFeedback.Negative, MemoryFeedback.VeryNegative };

        if (feedback.HasValue && !validFeedbacks.Contains(feedback.Value))
            throw new ArgumentException($"feedback must be one of {string.Join(", ", validFeedbacks)} or null");

        var payload = new Dictionary<string, object>
        {
            ["memory_id"] = memoryId,
            ["feedback"] = feedback?.ToString().ToUpper(),
            ["feedback_reason"] = feedbackReason
        };

        var json = JsonSerializer.Serialize(payload, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("feedback/", content, cancellationToken);
        await EnsureSuccessStatusCodeAsync(response);

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<MessageResponse>(responseContent, _jsonOptions) ?? new MessageResponse();
    }

    /// <summary>
    /// 准备请求参数
    /// </summary>
    private Dictionary<string, object> PrepareParams(Dictionary<string, object>? kwargs = null)
    {
        kwargs ??= new Dictionary<string, object>();

        if (!string.IsNullOrEmpty(OrgId) && !string.IsNullOrEmpty(ProjectId))
        {
            kwargs["org_id"] = OrgId;
            kwargs["project_id"] = ProjectId;
        }
        else if (!string.IsNullOrEmpty(OrgId) || !string.IsNullOrEmpty(ProjectId))
        {
            throw new InvalidOperationException("Please provide both org_id and project_id");
        }

        return kwargs.Where(kvp => kvp.Value != null).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }

    /// <summary>
    /// 准备请求负载
    /// </summary>
    private Dictionary<string, object> PreparePayload(List<Message> messages, Dictionary<string, object> kwargs)
    {
        var payload = new Dictionary<string, object> { ["messages"] = messages };

        foreach (var kvp in kwargs.Where(kvp => kvp.Value != null))
        {
            payload[kvp.Key] = kvp.Value;
        }

        return payload;
    }

    /// <summary>
    /// 构建查询字符串
    /// </summary>
    private static string BuildQueryString(Dictionary<string, object> parameters)
    {
        if (!parameters.Any()) return string.Empty;

        return string.Join("&", parameters.Select(kvp =>
            $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value?.ToString() ?? string.Empty)}"));
    }

    /// <summary>
    /// 确保HTTP响应成功
    /// </summary>
    private static async Task EnsureSuccessStatusCodeAsync(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Mem0ApiException($"API请求失败: {response.StatusCode} - {response.ReasonPhrase}. 详情: {errorContent}");
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