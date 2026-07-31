namespace MCPTools.Core.Models.Solution.Syntax;

/// <summary>
/// Represents a namespace declaration discovered in source code.
/// </summary>
public sealed class NamespaceModel
{
    /// <summary>
    /// Gets the namespace name.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Gets the classes declared in the namespace.
    /// </summary>
    public IReadOnlyList<ClassModel> Classes { get; init; } = [];

    /// <summary>
    /// Gets the interfaces declared in the namespace.
    /// </summary>
    public IReadOnlyList<InterfaceModel> Interfaces { get; init; } = [];

    /// <summary>
    /// Gets the records declared in the namespace.
    /// </summary>
    public IReadOnlyList<RecordModel> Records { get; init; } = [];

    /// <summary>
    /// Gets the enums declared in the namespace.
    /// </summary>
    public IReadOnlyList<EnumModel> Enums { get; init; } = [];
}
