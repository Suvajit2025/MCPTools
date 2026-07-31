namespace MCPTools.Core.Models.Solution.Syntax;

/// <summary>
/// Represents a base class or implemented interface declared by a type.
/// </summary>
public sealed class BaseTypeModel
{
    /// <summary>
    /// Gets the base type name.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Gets the fully qualified base type name when available.
    /// </summary>
    public string? FullName { get; init; }

    /// <summary>
    /// Gets a value indicating whether the base type is an interface.
    /// </summary>
    public bool IsInterface { get; init; }
}
