namespace MCPTools.Core.Models.Solution;

/// <summary>
/// Represents a class discovered within a source file.
/// </summary>
public sealed class ClassModel
{
    /// <summary>
    /// Gets the class name.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Gets the namespace that contains the class when available.
    /// </summary>
    public string? Namespace { get; init; }
}
