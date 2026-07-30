namespace MCPTools.Core.Models.Tools;

/// <summary>
/// Represents the common outcome of a tool execution.
/// </summary>
public sealed class ToolResult
{
    /// <summary>
    /// Gets a value indicating whether execution completed successfully.
    /// </summary>
    public bool Success { get; init; }

    /// <summary>
    /// Gets a human-readable summary of the result.
    /// </summary>
    public string? Message { get; init; }

    /// <summary>
    /// Gets the error messages produced during execution.
    /// </summary>
    public IReadOnlyList<string> Errors { get; init; } = [];

    /// <summary>
    /// Gets the warning messages produced during execution.
    /// </summary>
    public IReadOnlyList<string> Warnings { get; init; } = [];
}
