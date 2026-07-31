namespace MCPTools.Core.Models.Solution.Syntax;

/// <summary>
/// Represents a source-level dependency used by a declaration.
/// </summary>
public sealed class DependencyModel
{
    /// <summary>
    /// Gets the source project name when available.
    /// </summary>
    public string? ProjectName { get; init; }

    /// <summary>
    /// Gets the source file path when available.
    /// </summary>
    public string? SourcePath { get; init; }

    /// <summary>
    /// Gets the source declaration that owns the dependency.
    /// </summary>
    public string? SourceName { get; init; }

    /// <summary>
    /// Gets the source namespace when available.
    /// </summary>
    public string? SourceNamespace { get; init; }

    /// <summary>
    /// Gets the dependency name.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Gets the dependency namespace when available.
    /// </summary>
    public string? Namespace { get; init; }

    /// <summary>
    /// Gets the dependency type or category.
    /// </summary>
    public string? Type { get; init; }

    /// <summary>
    /// Gets the dependency relationship.
    /// </summary>
    public string? Relationship { get; init; }

    /// <summary>
    /// Gets a value indicating whether the dependency appears to be external to the analyzed solution.
    /// </summary>
    public bool IsExternal { get; init; }
}
