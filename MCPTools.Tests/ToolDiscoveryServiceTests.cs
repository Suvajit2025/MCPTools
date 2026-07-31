using MCPTools.Core.Tools.Crud;
using MCPTools.Core.Tools.Code;
using MCPTools.Core.Tools.Database;
using MCPTools.Core.Tools.Solution;
using MCPTools.Core.Constants;
using MCPTools.Server.Extensions;
using MCPTools.Server.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MCPTools.Tests;

public sealed class ToolDiscoveryServiceTests
{
    [Fact]
    public void AddMCPToolsServer_RegistersToolCatalog_WithRegisteredTools()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        services.AddLogging();
        services.AddMCPToolsServer(configuration);

        using var serviceProvider = services.BuildServiceProvider();

        var catalog = serviceProvider.GetRequiredService<ToolCatalog>();

        Assert.True(catalog.Count >= 7);
        Assert.Contains(catalog.Tools, descriptor =>
            descriptor.ToolName == "generate-crud"
            && descriptor.RequestType == typeof(GenerateCrudRequest)
            && descriptor.ResponseType == typeof(GenerateCrudResponse)
            && descriptor.Category == ToolMetadataConstants.Categories.Generation
            && descriptor.Author == ToolMetadataConstants.DefaultAuthor
            && descriptor.SupportedFrameworkVersion == ToolMetadataConstants.SupportedFrameworkVersion
            && descriptor.Tags.Contains(ToolMetadataConstants.Tags.Crud)
            && descriptor.RequestSchema.Properties.Any(property => property.Name == nameof(GenerateCrudRequest.EntityName))
            && descriptor.ResponseSchema.Properties.Any(property => property.Name == nameof(GenerateCrudResponse.GeneratedFiles)));
        Assert.Contains(catalog.Tools, descriptor =>
            descriptor.ToolName == "generate-crud-from-database"
            && descriptor.RequestType == typeof(GenerateCrudFromDatabaseRequest)
            && descriptor.ResponseType == typeof(GenerateCrudFromDatabaseResponse));
        Assert.Contains(catalog.Tools, descriptor =>
            descriptor.ToolName == "analyze-solution"
            && descriptor.RequestType == typeof(AnalyzeSolutionRequest)
            && descriptor.ResponseType == typeof(SolutionAnalysisResult));
        Assert.Contains(catalog.Tools, descriptor =>
            descriptor.ToolName == "modify-source-code"
            && descriptor.RequestType == typeof(ModifySourceCodeRequest)
            && descriptor.ResponseType == typeof(ModifySourceCodeResponse));
        Assert.Contains(catalog.Tools, descriptor =>
            descriptor.ToolName == "find-class"
            && descriptor.RequestType == typeof(FindClassRequest)
            && descriptor.ResponseType == typeof(FindClassResponse));
        Assert.Contains(catalog.Tools, descriptor =>
            descriptor.ToolName == "find-method"
            && descriptor.RequestType == typeof(FindMethodRequest)
            && descriptor.ResponseType == typeof(FindMethodResponse));
        Assert.Contains(catalog.Tools, descriptor =>
            descriptor.ToolName == "find-references"
            && descriptor.RequestType == typeof(FindReferencesRequest)
            && descriptor.ResponseType == typeof(FindReferencesResponse));
    }

    [Fact]
    public void ToolCatalog_OrdersTools_ByToolName()
    {
        var catalog = new ToolCatalog(
        [
            new ToolDescriptor
            {
                ToolName = "z-tool",
                Category = "Test",
                Tags = ["test"],
                Author = "Test",
                SupportedFrameworkVersion = "Test",
                ToolType = typeof(object),
                RequestType = typeof(object),
                RequestSchema = new ToolSchema { TypeName = nameof(Object) },
                ResponseType = typeof(object),
                ResponseSchema = new ToolSchema { TypeName = nameof(Object) }
            },
            new ToolDescriptor
            {
                ToolName = "a-tool",
                Category = "Test",
                Tags = ["test"],
                Author = "Test",
                SupportedFrameworkVersion = "Test",
                ToolType = typeof(object),
                RequestType = typeof(object),
                RequestSchema = new ToolSchema { TypeName = nameof(Object) },
                ResponseType = typeof(object),
                ResponseSchema = new ToolSchema { TypeName = nameof(Object) }
            }
        ]);

        Assert.Equal("a-tool", catalog.Tools[0].ToolName);
        Assert.Equal("z-tool", catalog.Tools[1].ToolName);
    }
}
