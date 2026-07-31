namespace MCPTools.Server.Models;

/// <summary>
/// Describes an MCPTools tool available to the server host.
/// </summary>
public sealed class ToolDescriptor
{
    /// <summary>
    /// Gets the tool name.
    /// </summary>
    public required string ToolName { get; init; }

    /// <summary>
    /// Gets the human-friendly tool display name.
    /// </summary>
    public string? DisplayName { get; init; }

    /// <summary>
    /// Gets the tool description.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Gets the tool version.
    /// </summary>
    public string? Version { get; init; }

    /// <summary>
    /// Gets the tool category.
    /// </summary>
    public string? Category { get; init; }

    /// <summary>
    /// Gets the tool tags.
    /// </summary>
    public IReadOnlyCollection<string> Tags { get; init; } = [];

    /// <summary>
    /// Gets the tool author.
    /// </summary>
    public string? Author { get; init; }

    /// <summary>
    /// Gets the supported framework version.
    /// </summary>
    public string? SupportedFrameworkVersion { get; init; }

    /// <summary>
    /// Gets the tool implementation type.
    /// </summary>
    public required Type ToolType { get; init; }

    /// <summary>
    /// Gets the request model type accepted by the tool.
    /// </summary>
    public required Type RequestType { get; init; }

    /// <summary>
    /// Gets the request model schema.
    /// </summary>
    public required ToolSchema RequestSchema { get; init; }

    /// <summary>
    /// Gets the response model type returned by the tool.
    /// </summary>
    public required Type ResponseType { get; init; }

    /// <summary>
    /// Gets the response model schema.
    /// </summary>
    public required ToolSchema ResponseSchema { get; init; }
}
