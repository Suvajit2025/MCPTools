namespace MCPTools.Core.Models.Tools;

/// <summary>
/// Provides descriptive metadata for a tool.
/// </summary>
public sealed class ToolMetadata
{
    /// <summary>
    /// Gets the unique name of the tool.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets the human-friendly name of the tool.
    /// </summary>
    public required string DisplayName { get; init; }

    /// <summary>
    /// Gets a short description of what the tool does.
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// Gets the logical grouping of the tool.
    /// </summary>
    public required string Category { get; init; }

    /// <summary>
    /// Gets the version of the tool.
    /// </summary>
    public string Version { get; init; } = "1.0.0";

    /// <summary>
    /// Gets the tags associated with the tool.
    /// </summary>
    public IReadOnlyCollection<string> Tags { get; init; } = [];
}
