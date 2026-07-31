using System.Text.Json;

namespace MCPTools.Server.Models;

/// <summary>
/// Represents an incoming MCP-style tool request.
/// </summary>
public sealed class McpRequest
{
    /// <summary>
    /// Gets the request identifier.
    /// </summary>
    public string? RequestId { get; init; }

    /// <summary>
    /// Gets the requested tool name.
    /// </summary>
    public required string ToolName { get; init; }

    /// <summary>
    /// Gets the serialized tool input payload.
    /// </summary>
    public JsonElement Input { get; init; }
}
