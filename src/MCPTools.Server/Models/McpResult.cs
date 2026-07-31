using System.Text.Json;

namespace MCPTools.Server.Models;

/// <summary>
/// Represents an MCP-style tool processing result.
/// </summary>
public sealed class McpResult
{
    /// <summary>
    /// Gets the request identifier.
    /// </summary>
    public string? RequestId { get; init; }

    /// <summary>
    /// Gets a value indicating whether request processing completed successfully.
    /// </summary>
    public bool Success { get; init; }

    /// <summary>
    /// Gets the serialized response payload when processing succeeds.
    /// </summary>
    public JsonElement? Result { get; init; }

    /// <summary>
    /// Gets the structured error when processing fails.
    /// </summary>
    public McpError? Error { get; init; }
}
