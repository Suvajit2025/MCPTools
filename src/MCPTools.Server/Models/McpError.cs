namespace MCPTools.Server.Models;

/// <summary>
/// Represents a structured MCP-style processing error.
/// </summary>
public sealed class McpError
{
    /// <summary>
    /// Gets the machine-readable error code.
    /// </summary>
    public required string Code { get; init; }

    /// <summary>
    /// Gets the safe human-readable error message.
    /// </summary>
    public required string Message { get; init; }

    /// <summary>
    /// Gets the error target when available.
    /// </summary>
    public string? Target { get; init; }
}
