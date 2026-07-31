using MCPTools.Core.Configuration;
using MCPTools.Core.Extensions;
using MCPTools.Core.Interfaces;
using MCPTools.Core.Models.Schema;
using MCPTools.Server.Adapters;
using MCPTools.Server.Models;
using MCPTools.Server.Security;
using MCPTools.Server.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MCPTools.Server.Extensions;

/// <summary>
/// Provides dependency injection registration extensions for the MCPTools server host.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the MCPTools server host and all MCPTools.Core services.
    /// </summary>
    /// <param name="services">The service collection to register services with.</param>
    /// <param name="configuration">The host configuration.</param>
    /// <returns>The same service collection so calls can be chained.</returns>
    /// <exception cref="ArgumentNullException">Thrown when a required argument is <see langword="null"/>.</exception>
    public static IServiceCollection AddMCPToolsServer(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddMCPTools();
        services.Configure<TemplateOptions>(configuration.GetSection("MCPTools:Templates"));
        services.Configure<OutputOptions>(configuration.GetSection("MCPTools:Output"));
        services.Configure<DatabaseConnectionOptions>(configuration.GetSection("MCPTools:Database"));
        RegisterToolRegistrations(services);
        services.TryAddSingleton<ToolDiscoveryService>();
        services.TryAddSingleton<JsonSchemaBuilder>();
        services.TryAddSingleton(serviceProvider =>
            serviceProvider.GetRequiredService<ToolDiscoveryService>().Discover());
        services.TryAddSingleton<IMcpAuthenticationHandler, AllowAnonymousAuthenticationHandler>();
        services.TryAddSingleton<IMcpAuthorizationHandler, AllowAllAuthorizationHandler>();
        services.TryAddSingleton<IMcpPermissionEvaluator, AllowAllPermissionEvaluator>();
        services.TryAddSingleton<McpSecurityMiddleware>();
        services.TryAddSingleton<McpRequestProcessor>();
        services.TryAddSingleton<McpToolAdapter>();

        return services;
    }

    private static void RegisterToolRegistrations(IServiceCollection services)
    {
        var toolTypes = services
            .Select(GetImplementationType)
            .Where(type => type is not null && IsToolType(type))
            .Select(type => type!)
            .Distinct()
            .OrderBy(type => type.FullName, StringComparer.OrdinalIgnoreCase);

        foreach (var toolType in toolTypes)
        {
            services.AddSingleton(new ToolRegistration(toolType));
        }
    }

    private static Type? GetImplementationType(ServiceDescriptor descriptor)
    {
        if (descriptor.ImplementationType is not null)
        {
            return descriptor.ImplementationType;
        }

        if (descriptor.ImplementationInstance is not null)
        {
            return descriptor.ImplementationInstance.GetType();
        }

        return null;
    }

    private static bool IsToolType(Type type)
    {
        return type.GetInterfaces().Any(interfaceType =>
            interfaceType.IsGenericType
            && interfaceType.GetGenericTypeDefinition() == typeof(ITool<,>));
    }
}
