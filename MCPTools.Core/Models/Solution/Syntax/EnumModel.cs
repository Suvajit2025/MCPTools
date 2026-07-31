namespace MCPTools.Core.Models.Solution.Syntax;

/// <summary>
/// Represents an enum declaration discovered in source code.
/// </summary>
public sealed class EnumModel
{
    /// <summary>
    /// Gets the enum name.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Gets the namespace that contains the enum.
    /// </summary>
    public string? Namespace { get; init; }

    /// <summary>
    /// Gets the enum accessibility modifier.
    /// </summary>
    public string? Accessibility { get; init; }

    /// <summary>
    /// Gets the enum underlying type.
    /// </summary>
    public string? UnderlyingType { get; init; }

    /// <summary>
    /// Gets the enum member names.
    /// </summary>
    public IReadOnlyList<string> Members { get; init; } = [];

    /// <summary>
    /// Gets the attributes applied to the enum.
    /// </summary>
    public IReadOnlyList<AttributeModel> Attributes { get; init; } = [];
}
