using MCPTools.Core.Extensions;

namespace MCPTools.Core.Services;

/// <summary>
/// Provides centralized naming conventions for generated MCPTools artifacts.
/// </summary>
public sealed class NamingConventionService
{
    /// <summary>
    /// Gets the entity name.
    /// </summary>
    /// <param name="name">The source name.</param>
    /// <returns>The normalized entity name.</returns>
    public string GetEntityName(string name)
    {
        return NormalizeName(name);
    }

    /// <summary>
    /// Gets the repository implementation name.
    /// </summary>
    /// <param name="entityName">The entity name.</param>
    /// <returns>The repository implementation name.</returns>
    public string GetRepositoryName(string entityName)
    {
        return $"{NormalizeName(entityName)}Repository";
    }

    /// <summary>
    /// Gets the repository interface name.
    /// </summary>
    /// <param name="entityName">The entity name.</param>
    /// <returns>The repository interface name.</returns>
    public string GetInterfaceRepositoryName(string entityName)
    {
        return $"I{GetRepositoryName(entityName)}";
    }

    /// <summary>
    /// Gets the service implementation name.
    /// </summary>
    /// <param name="entityName">The entity name.</param>
    /// <returns>The service implementation name.</returns>
    public string GetServiceName(string entityName)
    {
        return $"{NormalizeName(entityName)}Service";
    }

    /// <summary>
    /// Gets the service interface name.
    /// </summary>
    /// <param name="entityName">The entity name.</param>
    /// <returns>The service interface name.</returns>
    public string GetInterfaceServiceName(string entityName)
    {
        return $"I{GetServiceName(entityName)}";
    }

    /// <summary>
    /// Gets the manager implementation name.
    /// </summary>
    /// <param name="entityName">The entity name.</param>
    /// <returns>The manager implementation name.</returns>
    public string GetManagerName(string entityName)
    {
        return $"{NormalizeName(entityName)}Manager";
    }

    /// <summary>
    /// Gets the controller name.
    /// </summary>
    /// <param name="entityName">The entity name.</param>
    /// <returns>The controller name.</returns>
    public string GetControllerName(string entityName)
    {
        return $"{NormalizeName(entityName)}Controller";
    }

    /// <summary>
    /// Gets the DTO name.
    /// </summary>
    /// <param name="entityName">The entity name.</param>
    /// <returns>The DTO name.</returns>
    public string GetDtoName(string entityName)
    {
        return $"{NormalizeName(entityName)}Dto";
    }

    /// <summary>
    /// Gets the create request name.
    /// </summary>
    /// <param name="entityName">The entity name.</param>
    /// <returns>The create request name.</returns>
    public string GetCreateRequestName(string entityName)
    {
        return $"{NormalizeName(entityName)}CreateRequest";
    }

    /// <summary>
    /// Gets the update request name.
    /// </summary>
    /// <param name="entityName">The entity name.</param>
    /// <returns>The update request name.</returns>
    public string GetUpdateRequestName(string entityName)
    {
        return $"{NormalizeName(entityName)}UpdateRequest";
    }

    /// <summary>
    /// Gets the response name.
    /// </summary>
    /// <param name="entityName">The entity name.</param>
    /// <returns>The response name.</returns>
    public string GetResponseName(string entityName)
    {
        return $"{NormalizeName(entityName)}Response";
    }

    private static string NormalizeName(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return name.ToPascalCase();
    }
}
