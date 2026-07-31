using System.Text.Json;
using MCPTools.Core.Interfaces;
using MCPTools.Core.Models.Tools;
using MCPTools.Server.Extensions;
using MCPTools.Server.Models;
using MCPTools.Server.Security;
using MCPTools.Server.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MCPTools.Tests;

public sealed class McpSecurityMiddlewareTests
{
    [Fact]
    public async Task ProcessAsync_DoesNotExecuteTool_WhenAuthorizationFails()
    {
        DeniedTool.ExecutionCount = 0;
        using var serviceProvider = CreateServiceProvider(services =>
        {
            services.AddTransient<DeniedTool>();
            services.AddSingleton<IMcpAuthorizationHandler, DenyAuthorizationHandler>();
        });
        var processor = serviceProvider.GetRequiredService<McpRequestProcessor>();

        var result = await processor.ProcessAsync(new McpRequest
        {
            RequestId = "security-1",
            ToolName = "denied-tool",
            Input = JsonSerializer.SerializeToElement(new SecurityTestRequest { Value = "test" })
        });

        Assert.False(result.Success);
        Assert.Equal("AuthorizationFailed", result.Error?.Code);
        Assert.Equal(0, DeniedTool.ExecutionCount);
    }

    [Fact]
    public async Task ValidateAsync_RejectsUnknownTool()
    {
        using var serviceProvider = CreateServiceProvider();
        var middleware = serviceProvider.GetRequiredService<McpSecurityMiddleware>();

        var result = await middleware.ValidateAsync(new McpRequest
        {
            RequestId = "security-2",
            ToolName = "unknown-tool",
            Input = JsonSerializer.SerializeToElement(new { })
        });

        Assert.False(result.Success);
        Assert.Equal("ToolNotFound", result.Error?.Code);
    }

    [Fact]
    public async Task ValidateAsync_RejectsInvalidRequestModel()
    {
        using var serviceProvider = CreateServiceProvider(services =>
        {
            services.AddTransient<DeniedTool>();
        });
        var middleware = serviceProvider.GetRequiredService<McpSecurityMiddleware>();

        var result = await middleware.ValidateAsync(new McpRequest
        {
            RequestId = "security-3",
            ToolName = "denied-tool",
            Input = JsonSerializer.SerializeToElement("not-an-object")
        });

        Assert.False(result.Success);
        Assert.Equal("ValidationError", result.Error?.Code);
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

    private sealed class SecurityTestRequest
    {
        public required string Value { get; init; }
    }

    private sealed class SecurityTestResponse
    {
        public required string Value { get; init; }
    }

    private sealed class DeniedTool : ITool<SecurityTestRequest, SecurityTestResponse>
    {
        public static int ExecutionCount { get; set; }

        public ToolMetadata Metadata { get; } = new()
        {
            Name = "denied-tool",
            DisplayName = "Denied Tool",
            Category = "Test",
            Version = "1.0.0",
            Description = "A tool used to verify security middleware behavior."
        };

        public Task<SecurityTestResponse> ExecuteAsync(
            SecurityTestRequest request,
            CancellationToken cancellationToken = default)
        {
            ExecutionCount++;
            return Task.FromResult(new SecurityTestResponse { Value = request.Value });
        }
    }

    private sealed class DenyAuthorizationHandler : IMcpAuthorizationHandler
    {
        public Task<McpSecurityDecision> AuthorizeAsync(
            McpRequest request,
            ToolDescriptor toolDescriptor,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new McpSecurityDecision
            {
                Allowed = false,
                ErrorCode = "AuthorizationFailed",
                Message = "The request is not authorized."
            });
        }
    }
}
