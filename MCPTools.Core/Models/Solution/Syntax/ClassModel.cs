namespace MCPTools.Core.Models.Solution.Syntax;

/// <summary>
/// Represents a class declaration discovered in source code.
/// </summary>
public sealed class ClassModel
{
    /// <summary>
    /// Gets the class name.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Gets the namespace that contains the class.
    /// </summary>
    public string? Namespace { get; init; }

    /// <summary>
    /// Gets the class accessibility modifier.
    /// </summary>
    public string? Accessibility { get; init; }

    /// <summary>
    /// Gets a value indicating whether the class is abstract.
    /// </summary>
    public bool IsAbstract { get; init; }

    /// <summary>
    /// Gets a value indicating whether the class is sealed.
    /// </summary>
    public bool IsSealed { get; init; }

    /// <summary>
    /// Gets the base types declared by the class.
    /// </summary>
    public IReadOnlyList<BaseTypeModel> BaseTypes { get; init; } = [];

    /// <summary>
    /// Gets the constructors declared by the class.
    /// </summary>
    public IReadOnlyList<ConstructorModel> Constructors { get; init; } = [];

    /// <summary>
    /// Gets the methods declared by the class.
    /// </summary>
    public IReadOnlyList<MethodModel> Methods { get; init; } = [];

    /// <summary>
    /// Gets the properties declared by the class.
    /// </summary>
    public IReadOnlyList<PropertyModel> Properties { get; init; } = [];

    /// <summary>
    /// Gets the fields declared by the class.
    /// </summary>
    public IReadOnlyList<FieldModel> Fields { get; init; } = [];

    /// <summary>
    /// Gets the attributes applied to the class.
    /// </summary>
    public IReadOnlyList<AttributeModel> Attributes { get; init; } = [];
}
