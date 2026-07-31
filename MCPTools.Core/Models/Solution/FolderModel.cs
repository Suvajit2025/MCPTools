namespace MCPTools.Core.Models.Solution;

/// <summary>
/// Represents a folder discovered within a .NET project.
/// </summary>
public sealed class FolderModel
{
    /// <summary>
    /// Gets the folder name.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Gets the absolute or relative folder path.
    /// </summary>
    public string? Path { get; init; }

    /// <summary>
    /// Gets the child folders contained in this folder.
    /// </summary>
    public IReadOnlyList<FolderModel> Children { get; init; } = [];

    /// <summary>
    /// Gets the source files contained directly in this folder.
    /// </summary>
    public IReadOnlyList<SourceFileModel> Files { get; init; } = [];
}
