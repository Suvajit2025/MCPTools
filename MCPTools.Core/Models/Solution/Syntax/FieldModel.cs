namespace MCPTools.Core.Models.Solution.Syntax;

/// <summary>
/// Represents a field declaration discovered in source code.
/// </summary>
public sealed class FieldModel
{
    /// <summary>
    /// Gets the field name.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Gets the field type.
    /// </summary>
    public string? Type { get; init; }

    /// <summary>
    /// Gets the field accessibility modifier.
    /// </summary>
    public string? Accessibility { get; init; }

    /// <summary>
    /// Gets a value indicating whether the field is read-only.
    /// </summary>
    public bool IsReadOnly { get; init; }

    /// <summary>
    /// Gets a value indicating whether the field is static.
    /// </summary>
    public bool IsStatic { get; init; }

    /// <summary>
    /// Gets the attributes applied to the field.
    /// </summary>
    public IReadOnlyList<AttributeModel> Attributes { get; init; } = [];
}
