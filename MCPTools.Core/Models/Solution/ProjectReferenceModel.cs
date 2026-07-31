namespace MCPTools.Core.Models.Solution;

/// <summary>
/// Represents a project reference discovered within a .NET project.
/// </summary>
public sealed class ProjectReferenceModel
{
    /// <summary>
    /// Gets the referenced project name.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Gets the absolute or relative path to the referenced project.
    /// </summary>
    public string? Path { get; init; }
}
