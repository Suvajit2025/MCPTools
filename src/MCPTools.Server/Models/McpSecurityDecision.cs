namespace MCPTools.Server.Models;

/// <summary>
/// Represents the result of a security decision made by the MCPTools server.
/// </summary>
public sealed class McpSecurityDecision
{
    /// <summary>
    /// Gets a value indicating whether the request is allowed to continue.
    /// </summary>
    public bool Allowed { get; init; }

    /// <summary>
    /// Gets the error code to return when the request is rejected.
    /// </summary>
    public string? ErrorCode { get; init; }

    /// <summary>
    /// Gets the safe error message to return when the request is rejected.
    /// </summary>
    public string? Message { get; init; }
}
