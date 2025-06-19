# Mem0.NET

[![NuGet Version](https://img.shields.io/nuget/v/Mem0.NET)](https://www.nuget.org/packages/Mem0.NET)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Mem0.NET)](https://www.nuget.org/packages/Mem0.NET)
[![License](https://img.shields.io/github/license/AIDotNet/Mem0.NET)](https://github.com/AIDotNet/Mem0.NET/blob/main/LICENSE)

[中文文档](README.zh-CN.md) | English

A .NET client library for [Mem0](https://mem0.ai/), providing easy-to-use APIs for memory management in AI applications.

## Features

- 🚀 **Easy Integration** - Simple and intuitive API design
- 📦 **Multiple .NET Versions** - Supports .NET 6.0, 7.0, 8.0, and 9.0
- 🔧 **Dependency Injection** - Built-in support for Microsoft.Extensions.DependencyInjection
- 🌐 **HTTP Client Management** - Flexible HTTP client configuration
- 📝 **Full API Coverage** - Complete implementation of Mem0 API
- ⚡ **Async/Await Support** - Modern asynchronous programming patterns

## Installation

Install the package via NuGet Package Manager:

```bash
dotnet add package Mem0.NET
```

Or via Package Manager Console:

```powershell
Install-Package Mem0.NET
```

## Quick Start

### Basic Usage

```csharp
using Mem0.NET;

// Initialize the client
var client = new Mem0Client("https://api.mem0.ai", "your-api-key");

// Create a memory
var createRequest = new MemoryCreateRequest
{
    Messages = new List<Message>
    {
        new Message { Role = "user", Content = "I love playing basketball" }
    },
    UserId = "user123"
};

var memories = await client.CreateMemoryAsync(createRequest);

// Search memories
var searchRequest = new SearchRequest
{
    Query = "What sports does the user like?",
    UserId = "user123"
};

var results = await client.SearchMemoriesAsync(searchRequest);

// Get all memories for a user
var userMemories = await client.GetAllMemoriesAsync(userId: "user123");
```

### Dependency Injection

```csharp
using Mem0.NET.Extensions;

// In your Program.cs or Startup.cs
services.AddMem0Client(options =>
{
    options.BaseUrl = "https://api.mem0.ai";
    options.ApiKey = "your-api-key";
});

// Use in your service
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
        // Your logic here
    }
}
```

## API Reference

### Core Methods

#### Memory Management

- `CreateMemoryAsync(MemoryCreateRequest request)` - Create new memories
- `GetAllMemoriesAsync(string? userId, string? runId, string? agentId)` - Retrieve all memories
- `GetMemoryAsync(string memoryId)` - Get a specific memory by ID
- `UpdateMemoryAsync(string memoryId, Dictionary<string, object> updatedMemory)` - Update a memory
- `DeleteMemoryAsync(string memoryId)` - Delete a specific memory
- `DeleteAllMemoriesAsync(string? userId, string? runId, string? agentId)` - Delete all memories

#### Search and History

- `SearchMemoriesAsync(SearchRequest request)` - Search memories with queries
- `GetMemoryHistoryAsync(string memoryId)` - Get memory history

#### Configuration

- `ConfigureAsync(Dictionary<string, object> config)` - Configure Mem0 settings
- `ResetMemoryAsync()` - Reset all memories

### Data Models

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

## Examples

### Creating Memories with Metadata

```csharp
var createRequest = new MemoryCreateRequest
{
    Messages = new List<Message>
    {
        new Message { Role = "user", Content = "I prefer working in the morning" },
        new Message { Role = "assistant", Content = "I'll remember your morning preference" }
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

### Advanced Search with Filters

```csharp
var searchRequest = new SearchRequest
{
    Query = "user preferences",
    UserId = "user123",
    Filters = new Dictionary<string, object>
    {
        ["category"] = "preference",
        ["priority"] = "high"
    }
};

var results = await client.SearchMemoriesAsync(searchRequest);
```

### Custom HTTP Client Configuration

```csharp
var httpClient = new HttpClient();
httpClient.Timeout = TimeSpan.FromSeconds(30);

var client = new Mem0Client("https://api.mem0.ai", "your-api-key", httpClient);
```

## Error Handling

The client throws exceptions for HTTP errors. Always wrap your calls in try-catch blocks:

```csharp
try
{
    var memories = await client.GetAllMemoriesAsync();
}
catch (HttpRequestException ex)
{
    // Handle HTTP errors
    Console.WriteLine($"HTTP Error: {ex.Message}");
}
catch (Exception ex)
{
    // Handle other errors
    Console.WriteLine($"Error: {ex.Message}");
}
```

## Contributing

We welcome contributions! Please see our [Contributing Guidelines](CONTRIBUTING.md) for details.

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for your changes
5. Submit a pull request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Support

- 📖 [Documentation](https://github.com/AIDotNet/Mem0.NET/wiki)
- 🐛 [Issue Tracker](https://github.com/AIDotNet/Mem0.NET/issues)
- 💬 [Discussions](https://github.com/AIDotNet/Mem0.NET/discussions)

## Changelog

See [CHANGELOG.md](CHANGELOG.md) for a list of changes and updates.

---

Made with ❤️ by [AIDotNet](https://github.com/AIDotNet) 