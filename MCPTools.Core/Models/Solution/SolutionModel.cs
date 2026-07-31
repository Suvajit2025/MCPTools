namespace MCPTools.Core.Models.Solution;

/// <summary>
/// Represents a .NET solution and the projects it contains.
/// </summary>
public sealed class SolutionModel
{
    /// <summary>
    /// Gets the solution name.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Gets the absolute or relative path to the solution file.
    /// </summary>
    public string? Path { get; init; }

    /// <summary>
    /// Gets the projects contained in the solution.
    /// </summary>
    public IReadOnlyList<ProjectModel> Projects { get; init; } = [];
}
