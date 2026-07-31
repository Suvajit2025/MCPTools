using MCPTools.Core.Interfaces;
using MCPTools.Server.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace MCPTools.Server.Services;

/// <summary>
/// Discovers registered MCPTools tools and builds an internal tool catalog.
/// </summary>
public sealed class ToolDiscoveryService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IReadOnlyList<ToolRegistration> _toolRegistrations;
    private readonly ILogger<ToolDiscoveryService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ToolDiscoveryService"/> class.
    /// </summary>
    /// <param name="serviceScopeFactory">The service scope factory used to resolve registered tools.</param>
    /// <param name="toolRegistrations">The registered tool implementation types.</param>
    /// <param name="logger">The logger used to record discovery activity.</param>
    /// <exception cref="ArgumentNullException">Thrown when a required dependency is <see langword="null"/>.</exception>
    public ToolDiscoveryService(
        IServiceScopeFactory serviceScopeFactory,
        IEnumerable<ToolRegistration> toolRegistrations,
        ILogger<ToolDiscoveryService> logger)
    {
        _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
        _toolRegistrations = toolRegistrations?.ToArray() ?? throw new ArgumentNullException(nameof(toolRegistrations));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Discovers the registered tools and creates a tool catalog.
    /// </summary>
    /// <returns>The discovered tool catalog.</returns>
    public ToolCatalog Discover()
    {
        _logger.LogInformation("Discovering registered MCPTools tools.");

        using var scope = _serviceScopeFactory.CreateScope();
        var descriptors = _toolRegistrations
            .Select(registration => CreateDescriptor(scope.ServiceProvider, registration.ToolType))
            .Where(descriptor => descriptor is not null)
            .Select(descriptor => descriptor!)
            .GroupBy(descriptor => descriptor.ToolName, StringComparer.OrdinalIgnoreCase)
            .Select(group => group.First())
            .ToArray();

        _logger.LogInformation("Discovered {ToolCount} MCPTools tools.", descriptors.Length);

        return new ToolCatalog(descriptors);
    }

    private ToolDescriptor? CreateDescriptor(IServiceProvider serviceProvider, Type toolType)
    {
        var toolContract = GetToolContract(toolType);

        if (toolContract is null)
        {
            _logger.LogDebug("Skipping service type {ToolType} because it does not implement ITool.", toolType.FullName);
            return null;
        }

        var tool = serviceProvider.GetRequiredService(toolType);
        var metadataProperty = toolType.GetProperty(nameof(ITool<object, object>.Metadata))
            ?? throw new InvalidOperationException($"Tool type '{toolType.FullName}' does not expose metadata.");
        var metadata = metadataProperty.GetValue(tool)
            as MCPTools.Core.Models.Tools.ToolMetadata
            ?? throw new InvalidOperationException($"Tool type '{toolType.FullName}' returned invalid metadata.");
        var genericArguments = toolContract.GetGenericArguments();

        return new ToolDescriptor
        {
            ToolName = metadata.Name,
            DisplayName = metadata.DisplayName,
            Description = metadata.Description,
            Version = metadata.Version,
            Category = metadata.Category,
            Tags = metadata.Tags.ToArray(),
            Author = metadata.Author,
            SupportedFrameworkVersion = metadata.SupportedFrameworkVersion,
            ToolType = toolType,
            RequestType = genericArguments[0],
            RequestSchema = CreateSchema(genericArguments[0]),
            ResponseType = genericArguments[1],
            ResponseSchema = CreateSchema(genericArguments[1])
        };
    }

    private static ToolSchema CreateSchema(Type type)
    {
        return new ToolSchema
        {
            SchemaType = type,
            Properties = type
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(property => property.GetMethod is not null)
                .Select(property => new ToolSchemaProperty
                {
                    Name = property.Name,
                    PropertyType = property.PropertyType,
                    IsRequired = property.GetCustomAttribute<RequiredMemberAttribute>() is not null
                })
                .OrderBy(property => property.Name, StringComparer.OrdinalIgnoreCase)
                .ToArray()
        };
    }

    private static Type? GetToolContract(Type toolType)
    {
        return toolType
            .GetInterfaces()
            .FirstOrDefault(type => type.IsGenericType
                && type.GetGenericTypeDefinition() == typeof(ITool<,>));
    }
}
