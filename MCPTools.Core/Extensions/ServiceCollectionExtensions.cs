using MCPTools.Core.Configuration;
using MCPTools.Core.Services;
using MCPTools.Core.TemplateEngine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MCPTools.Core.Extensions;

/// <summary>
/// Provides dependency injection registration extensions for MCPTools.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers core MCPTools services with the specified service collection.
    /// </summary>
    /// <param name="services">The service collection to register services with.</param>
    /// <returns>The same service collection so calls can be chained.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> is <see langword="null"/>.</exception>
    public static IServiceCollection AddMCPTools(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddOptions<MCPToolsOptions>();
        services.AddOptions<TemplateOptions>();
        services.AddOptions<GeneratorOptions>();
        services.AddOptions<OutputOptions>();
        services.AddOptions<LoggingOptions>();

        services.TryAddSingleton<ITemplateEngine, TemplateEngine.TemplateEngine>();
        services.TryAddSingleton<FileTemplateLoader>();
        services.TryAddSingleton<ToolRegistry>();
        services.TryAddSingleton<ToolExecutor>();
        services.TryAddSingleton<PlaceholderBuilder>();
        services.TryAddSingleton<FileGenerator>();
        services.TryAddSingleton<TemplateDiscoveryService>();
        services.TryAddSingleton<NamingConventionService>();

        return services;
    }
}
