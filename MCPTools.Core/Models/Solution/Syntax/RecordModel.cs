namespace MCPTools.Core.Models.Solution.Syntax;

/// <summary>
/// Represents a record declaration discovered in source code.
/// </summary>
public sealed class RecordModel
{
    /// <summary>
    /// Gets the record name.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Gets the namespace that contains the record.
    /// </summary>
    public string? Namespace { get; init; }

    /// <summary>
    /// Gets the record accessibility modifier.
    /// </summary>
    public string? Accessibility { get; init; }

    /// <summary>
    /// Gets a value indicating whether the record is a record struct.
    /// </summary>
    public bool IsStruct { get; init; }

    /// <summary>
    /// Gets the base types declared by the record.
    /// </summary>
    public IReadOnlyList<BaseTypeModel> BaseTypes { get; init; } = [];

    /// <summary>
    /// Gets the primary constructor parameters declared by the record.
    /// </summary>
    public IReadOnlyList<ParameterModel> Parameters { get; init; } = [];

    /// <summary>
    /// Gets the properties declared by the record.
    /// </summary>
    public IReadOnlyList<PropertyModel> Properties { get; init; } = [];

    /// <summary>
    /// Gets the methods declared by the record.
    /// </summary>
    public IReadOnlyList<MethodModel> Methods { get; init; } = [];

    /// <summary>
    /// Gets the attributes applied to the record.
    /// </summary>
    public IReadOnlyList<AttributeModel> Attributes { get; init; } = [];
}
