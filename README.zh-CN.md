# Mem0.NET

[![NuGet Version](https://img.shields.io/nuget/v/Mem0.NET)](https://www.nuget.org/packages/Mem0.NET)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Mem0.NET)](https://www.nuget.org/packages/Mem0.NET)
[![License](https://img.shields.io/github/license/AIDotNet/Mem0.NET)](https://github.com/AIDotNet/Mem0.NET/blob/main/LICENSE)

中文文档 | [English](README.md)

[Mem0](https://mem0.ai/) 的 .NET 客户端库，为 AI 应用程序提供易于使用的记忆管理 API。

## 特性

- 🚀 **易于集成** - 简单直观的 API 设计
- 📦 **多 .NET 版本支持** - 支持 .NET 6.0、7.0、8.0 和 9.0
- 🔧 **依赖注入** - 内置对 Microsoft.Extensions.DependencyInjection 的支持
- 🌐 **HTTP 客户端管理** - 灵活的 HTTP 客户端配置
- 📝 **完整 API 覆盖** - 完整实现 Mem0 API
- ⚡ **异步支持** - 现代异步编程模式

## 安装

通过 NuGet 包管理器安装：

```bash
dotnet add package Mem0.NET
```

或通过包管理器控制台：

```powershell
Install-Package Mem0.NET
```

## 快速开始

### 基本用法

```csharp
using Mem0.NET;

// 初始化客户端
var client = new Mem0Client("https://api.mem0.ai", "your-api-key");

// 创建记忆
var createRequest = new MemoryCreateRequest
{
    Messages = new List<Message>
    {
        new Message { Role = "user", Content = "我喜欢打篮球" }
    },
    UserId = "user123"
};

var memories = await client.CreateMemoryAsync(createRequest);

// 搜索记忆
var searchRequest = new SearchRequest
{
    Query = "用户喜欢什么运动？",
    UserId = "user123"
};

var results = await client.SearchMemoriesAsync(searchRequest);

// 获取用户的所有记忆
var userMemories = await client.GetAllMemoriesAsync(userId: "user123");
```

### 依赖注入

```csharp
using Mem0.NET.Extensions;

// 在 Program.cs 或 Startup.cs 中
services.AddMem0Client(options =>
{
    options.BaseUrl = "https://api.mem0.ai";
    options.ApiKey = "your-api-key";
});

// 在服务中使用
public class MyService
{
    private readonly Mem0Client _mem0Client;

    public MyService(Mem0Client mem0Client)
    {
        _mem0Client = mem0Client;
    }

    public async Task DoSomethingAsync()
    {
        var memories = await _mem0Client.GetAllMemoriesAsync();
        // 您的逻辑代码
    }
}
```

## API 参考

### 核心方法

#### 记忆管理

- `CreateMemoryAsync(MemoryCreateRequest request)` - 创建新记忆
- `GetAllMemoriesAsync(string? userId, string? runId, string? agentId)` - 获取所有记忆
- `GetMemoryAsync(string memoryId)` - 根据 ID 获取特定记忆
- `UpdateMemoryAsync(string memoryId, Dictionary<string, object> updatedMemory)` - 更新记忆
- `DeleteMemoryAsync(string memoryId)` - 删除特定记忆
- `DeleteAllMemoriesAsync(string? userId, string? runId, string? agentId)` - 删除所有记忆

#### 搜索和历史

- `SearchMemoriesAsync(SearchRequest request)` - 使用查询搜索记忆
- `GetMemoryHistoryAsync(string memoryId)` - 获取记忆历史

#### 配置

- `ConfigureAsync(Dictionary<string, object> config)` - 配置 Mem0 设置
- `ResetMemoryAsync()` - 重置所有记忆

### 数据模型

#### MemoryCreateRequest

```csharp
public class MemoryCreateRequest
{
    public List<Message> Messages { get; set; }
    public string? UserId { get; set; }
    public string? AgentId { get; set; }
    public string? RunId { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
}
```

#### SearchRequest

```csharp
public class SearchRequest
{
    public string Query { get; set; }
    public string? UserId { get; set; }
    public string? RunId { get; set; }
    public string? AgentId { get; set; }
    public Dictionary<string, object>? Filters { get; set; }
}
```

#### Memory

```csharp
public class Memory
{
    public string Id { get; set; }
    public string Content { get; set; }
    public string? UserId { get; set; }
    public string? AgentId { get; set; }
    public string? RunId { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
```

## 示例

### 创建带元数据的记忆

```csharp
var createRequest = new MemoryCreateRequest
{
    Messages = new List<Message>
    {
        new Message { Role = "user", Content = "我更喜欢在早上工作" },
        new Message { Role = "assistant", Content = "我会记住您的早上偏好" }
    },
    UserId = "user123",
    Metadata = new Dictionary<string, object>
    {
        ["category"] = "preference",
        ["priority"] = "high"
    }
};

var memories = await client.CreateMemoryAsync(createRequest);
```

### 带过滤器的高级搜索

```csharp
var searchRequest = new SearchRequest
{
    Query = "用户偏好",
    UserId = "user123",
    Filters = new Dictionary<string, object>
    {
        ["category"] = "preference",
        ["priority"] = "high"
    }
};

var results = await client.SearchMemoriesAsync(searchRequest);
```

### 自定义 HTTP 客户端配置

```csharp
var httpClient = new HttpClient();
httpClient.Timeout = TimeSpan.FromSeconds(30);

var client = new Mem0Client("https://api.mem0.ai", "your-api-key", httpClient);
```

## 错误处理

客户端会为 HTTP 错误抛出异常。请始终将调用包装在 try-catch 块中：

```csharp
try
{
    var memories = await client.GetAllMemoriesAsync();
}
catch (HttpRequestException ex)
{
    // 处理 HTTP 错误
    Console.WriteLine($"HTTP 错误: {ex.Message}");
}
catch (Exception ex)
{
    // 处理其他错误
    Console.WriteLine($"错误: {ex.Message}");
}
```

## 贡献

我们欢迎贡献！请查看我们的[贡献指南](CONTRIBUTING.md)了解详情。

1. Fork 仓库
2. 创建功能分支
3. 进行更改
4. 为您的更改添加测试
5. 提交拉取请求

## 许可证

此项目根据 MIT 许可证授权 - 请查看 [LICENSE](LICENSE) 文件了解详情。

## 支持

- 📖 [文档](https://github.com/AIDotNet/Mem0.NET/wiki)
- 🐛 [问题跟踪器](https://github.com/AIDotNet/Mem0.NET/issues)
- 💬 [讨论](https://github.com/AIDotNet/Mem0.NET/discussions)

## 更新日志

查看 [CHANGELOG.md](CHANGELOG.md) 了解更改和更新列表。

---

由 [AIDotNet](https://github.com/AIDotNet) 用 ❤️ 制作 