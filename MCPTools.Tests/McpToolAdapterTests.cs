using MCPTools.Core.Interfaces;
using MCPTools.Core.Models.Tools;
using MCPTools.Server.Adapters;
using MCPTools.Server.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MCPTools.Tests;

public sealed class McpToolAdapterTests
{
    [Fact]
    public void CreateProtocolTools_ExposesDiscoveredTools_WhenToolsAreRegistered()
    {
        using var serviceProvider = CreateServiceProvider(services =>
        {
            services.AddTransient<EchoTool>();
        });
        var adapter = serviceProvider.GetRequiredService<McpToolAdapter>();

        var tools = adapter.CreateProtocolTools();

        Assert.Contains(tools, tool =>
            tool.Name == "echo"
            && tool.Description == "Echoes a message."
            && tool.InputSchema.GetProperty("type").GetString() == "object"
            && tool.OutputSchema?.GetProperty("type").GetString() == "object");
    }

    [Fact]
    public void CreateProtocolTools_AddsItemsSchema_WhenRequestContainsArray()
    {
        using var serviceProvider = CreateServiceProvider(services =>
        {
            services.AddTransient<ArrayTool>();
        });
        var adapter = serviceProvider.GetRequiredService<McpToolAdapter>();

        var tools = adapter.CreateProtocolTools();
        var tool = Assert.Single(tools, protocolTool => protocolTool.Name == "array-tool");
        var itemsSchema = tool.InputSchema
            .GetProperty("properties")
            .GetProperty("names");

        Assert.Equal("array", itemsSchema.GetProperty("type").GetString());
        Assert.True(itemsSchema.TryGetProperty("items", out var items));
        Assert.Equal("string", items.GetProperty("type").GetString());
    }

    [Fact]
    public void CreateProtocolTools_AddsItemsSchema_WhenGenerateCrudRequestContainsProperties()
    {
        using var serviceProvider = CreateServiceProvider();
        var adapter = serviceProvider.GetRequiredService<McpToolAdapter>();

        var tools = adapter.CreateProtocolTools();
        var tool = tools.Single(protocolTool => protocolTool.Name == "generate-crud");
        var propertiesSchema = tool.InputSchema
            .GetProperty("properties")
            .GetProperty("properties");

        Assert.Equal("array", propertiesSchema.GetProperty("type").GetString());
        Assert.True(propertiesSchema.TryGetProperty("items", out var items));
        Assert.Equal("object", items.GetProperty("type").GetString());
    }

    private static ServiceProvider CreateServiceProvider(Action<IServiceCollection>? configureServices = null)
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        services.AddLogging();
        configureServices?.Invoke(services);
        services.AddMCPToolsServer(configuration);

        return services.BuildServiceProvider();
    }

    private sealed class EchoRequest
    {
        public required string Message { get; init; }
    }

    private sealed class EchoResponse
    {
        public required string Message { get; init; }
    }

    private sealed class EchoTool : ITool<EchoRequest, EchoResponse>
    {
        public ToolMetadata Metadata { get; } = new()
        {
            Name = "echo",
            DisplayName = "Echo",
            Category = "Test",
            Version = "1.0.0",
            Description = "Echoes a message."
        };

        public Task<EchoResponse> ExecuteAsync(
            EchoRequest request,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new EchoResponse { Message = request.Message });
        }
    }

    private sealed class ArrayRequest
    {
        public required IReadOnlyList<string> Names { get; init; }
    }

    private sealed class ArrayResponse
    {
        public IReadOnlyList<string> Names { get; init; } = [];
    }

    private sealed class ArrayTool : ITool<ArrayRequest, ArrayResponse>
    {
        public ToolMetadata Metadata { get; } = new()
        {
            Name = "array-tool",
            DisplayName = "Array Tool",
            Category = "Test",
            Version = "1.0.0",
            Description = "Tests array schema generation."
        };

        public Task<ArrayResponse> ExecuteAsync(
            ArrayRequest request,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new ArrayResponse { Names = request.Names });
        }
    }
}
