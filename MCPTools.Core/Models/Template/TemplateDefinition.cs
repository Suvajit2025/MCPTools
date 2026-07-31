namespace MCPTools.Core.Models.Template;

/// <summary>
/// Represents metadata for a discovered template file.
/// </summary>
public sealed class TemplateDefinition
{
    /// <summary>
    /// Gets the template file name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets the full template path.
    /// </summary>
    public required string FullPath { get; init; }

    /// <summary>
    /// Gets the template path relative to the template root.
    /// </summary>
    public required string RelativePath { get; init; }

    /// <summary>
    /// Gets the template category inferred from its folder.
    /// </summary>
    public string? Category { get; init; }

    /// <summary>
    /// Gets the UTC timestamp when the template file was last modified.
    /// </summary>
    public DateTimeOffset LastModifiedUtc { get; init; }
}
