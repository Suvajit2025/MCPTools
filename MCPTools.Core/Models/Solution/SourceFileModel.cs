namespace MCPTools.Core.Models.Solution;

/// <summary>
/// Represents a source file discovered within a .NET project.
/// </summary>
public sealed class SourceFileModel
{
    /// <summary>
    /// Gets the source file name.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Gets the absolute or relative source file path.
    /// </summary>
    public string? Path { get; init; }

    /// <summary>
    /// Gets the source file extension.
    /// </summary>
    public string? Extension { get; init; }

    /// <summary>
    /// Gets the namespace declared by the source file when available.
    /// </summary>
    public string? Namespace { get; init; }

    /// <summary>
    /// Gets the classes declared by the source file.
    /// </summary>
    public IReadOnlyList<ClassModel> Classes { get; init; } = [];

    /// <summary>
    /// Gets the namespace-level syntax declarations discovered in the source file.
    /// </summary>
    public IReadOnlyList<Syntax.NamespaceModel> Namespaces { get; init; } = [];
}
