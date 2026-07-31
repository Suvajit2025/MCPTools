namespace MCPTools.Core.Models.Solution.Syntax;

/// <summary>
/// Represents a property declaration discovered in source code.
/// </summary>
public sealed class PropertyModel
{
    /// <summary>
    /// Gets the property name.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Gets the property type.
    /// </summary>
    public string? Type { get; init; }

    /// <summary>
    /// Gets the property accessibility modifier.
    /// </summary>
    public string? Accessibility { get; init; }

    /// <summary>
    /// Gets a value indicating whether the property has a getter.
    /// </summary>
    public bool HasGetter { get; init; }

    /// <summary>
    /// Gets a value indicating whether the property has a setter.
    /// </summary>
    public bool HasSetter { get; init; }

    /// <summary>
    /// Gets a value indicating whether the property is required.
    /// </summary>
    public bool IsRequired { get; init; }

    /// <summary>
    /// Gets the attributes applied to the property.
    /// </summary>
    public IReadOnlyList<AttributeModel> Attributes { get; init; } = [];
}
