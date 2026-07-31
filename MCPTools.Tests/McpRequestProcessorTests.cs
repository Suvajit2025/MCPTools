using System.Text.Json;
using MCPTools.Core.Exceptions;
using MCPTools.Core.Interfaces;
using MCPTools.Core.Models.Tools;
using MCPTools.Server.Extensions;
using MCPTools.Server.Models;
using MCPTools.Server.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MCPTools.Tests;

public sealed class McpRequestProcessorTests
{
    [Fact]
    public async Task ProcessAsync_ReturnsSerializedResult_WhenRequestIsValid()
    {
        using var serviceProvider = CreateServiceProvider(services =>
        {
            services.AddTransient<EchoTool>();
        });
        var processor = serviceProvider.GetRequiredService<McpRequestProcessor>();

        var result = await processor.ProcessAsync(new McpRequest
        {
            RequestId = "request-1",
            ToolName = "echo",
            Input = JsonSerializer.SerializeToElement(new EchoRequest { Message = "hello" })
        });

        Assert.True(result.Success);
        Assert.Equal("request-1", result.RequestId);
        Assert.Equal("hello", result.Result?.GetProperty("message").GetString());
    }

    [Fact]
    public async Task ProcessAsync_ReturnsToolNotFound_WhenToolIsUnknown()
    {
        using var serviceProvider = CreateServiceProvider();
        var processor = serviceProvider.GetRequiredService<McpRequestProcessor>();

        var result = await processor.ProcessAsync(new McpRequest
        {
            RequestId = "request-2",
            ToolName = "missing-tool",
            Input = JsonSerializer.SerializeToElement(new { })
        });

        Assert.False(result.Success);
        Assert.Equal("ToolNotFound", result.Error?.Code);
    }

    [Fact]
    public async Task ProcessAsync_ReturnsValidationError_WhenRequestCannotDeserialize()
    {
        using var serviceProvider = CreateServiceProvider(services =>
        {
            services.AddTransient<EchoTool>();
        });
        var processor = serviceProvider.GetRequiredService<McpRequestProcessor>();

        var result = await processor.ProcessAsync(new McpRequest
        {
            RequestId = "request-3",
            ToolName = "echo",
            Input = JsonSerializer.SerializeToElement("not-an-object")
        });

        Assert.False(result.Success);
        Assert.Equal("ValidationError", result.Error?.Code);
    }

    [Fact]
    public async Task ProcessAsync_ReturnsValidationError_WhenToolValidationFails()
    {
        using var serviceProvider = CreateServiceProvider(services =>
        {
            services.AddTransient<ValidationFailureTool>();
        });
        var processor = serviceProvider.GetRequiredService<McpRequestProcessor>();

        var result = await processor.ProcessAsync(new McpRequest
        {
            RequestId = "request-4",
            ToolName = "validation-failure",
            Input = JsonSerializer.SerializeToElement(new EchoRequest { Message = "hello" })
        });

        Assert.False(result.Success);
        Assert.Equal("ValidationError", result.Error?.Code);
        Assert.Equal("Request is invalid.", result.Error?.Message);
    }

    [Fact]
    public async Task ProcessAsync_ReturnsInternalError_WhenToolThrowsUnhandledException()
    {
        using var serviceProvider = CreateServiceProvider(services =>
        {
            services.AddTransient<UnhandledFailureTool>();
        });
        var processor = serviceProvider.GetRequiredService<McpRequestProcessor>();

        var result = await processor.ProcessAsync(new McpRequest
        {
            RequestId = "request-5",
            ToolName = "unhandled-failure",
            Input = JsonSerializer.SerializeToElement(new EchoRequest { Message = "hello" })
        });

        Assert.False(result.Success);
        Assert.Equal("InternalError", result.Error?.Code);
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

    private sealed class ValidationFailureTool : ITool<EchoRequest, EchoResponse>
    {
        public ToolMetadata Metadata { get; } = new()
        {
            Name = "validation-failure",
            DisplayName = "Validation Failure",
            Category = "Test",
            Version = "1.0.0",
            Description = "Throws a validation exception."
        };

        public Task<EchoResponse> ExecuteAsync(
            EchoRequest request,
            CancellationToken cancellationToken = default)
        {
            throw new ToolValidationException("Request is invalid.");
        }
    }

    private sealed class UnhandledFailureTool : ITool<EchoRequest, EchoResponse>
    {
        public ToolMetadata Metadata { get; } = new()
        {
            Name = "unhandled-failure",
            DisplayName = "Unhandled Failure",
            Category = "Test",
            Version = "1.0.0",
            Description = "Throws an unhandled exception."
        };

        public Task<EchoResponse> ExecuteAsync(
            EchoRequest request,
            CancellationToken cancellationToken = default)
        {
            throw new InvalidOperationException("Boom.");
        }
    }
}
