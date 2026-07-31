namespace MCPTools.Core.Models.Solution.Syntax;

/// <summary>
/// Represents an interface declaration discovered in source code.
/// </summary>
public sealed class InterfaceModel
{
    /// <summary>
    /// Gets the interface name.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Gets the namespace that contains the interface.
    /// </summary>
    public string? Namespace { get; init; }

    /// <summary>
    /// Gets the interface accessibility modifier.
    /// </summary>
    public string? Accessibility { get; init; }

    /// <summary>
    /// Gets the base interfaces declared by the interface.
    /// </summary>
    public IReadOnlyList<BaseTypeModel> BaseTypes { get; init; } = [];

    /// <summary>
    /// Gets the methods declared by the interface.
    /// </summary>
    public IReadOnlyList<MethodModel> Methods { get; init; } = [];

    /// <summary>
    /// Gets the properties declared by the interface.
    /// </summary>
    public IReadOnlyList<PropertyModel> Properties { get; init; } = [];

    /// <summary>
    /// Gets the attributes applied to the interface.
    /// </summary>
    public IReadOnlyList<AttributeModel> Attributes { get; init; } = [];
}
