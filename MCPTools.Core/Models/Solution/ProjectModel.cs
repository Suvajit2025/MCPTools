namespace MCPTools.Core.Models.Solution;

/// <summary>
/// Represents a .NET project discovered within a solution.
/// </summary>
public sealed class ProjectModel
{
    /// <summary>
    /// Gets the project name.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Gets the absolute or relative path to the project file.
    /// </summary>
    public string? Path { get; init; }

    /// <summary>
    /// Gets the target framework declared by the project.
    /// </summary>
    public string? TargetFramework { get; init; }

    /// <summary>
    /// Gets the project output type.
    /// </summary>
    public string? OutputType { get; init; }

    /// <summary>
    /// Gets the folders contained in the project.
    /// </summary>
    public IReadOnlyList<FolderModel> Folders { get; init; } = [];

    /// <summary>
    /// Gets the source files contained in the project.
    /// </summary>
    public IReadOnlyList<SourceFileModel> SourceFiles { get; init; } = [];

    /// <summary>
    /// Gets the project references declared by the project.
    /// </summary>
    public IReadOnlyList<ProjectReferenceModel> References { get; init; } = [];
}
