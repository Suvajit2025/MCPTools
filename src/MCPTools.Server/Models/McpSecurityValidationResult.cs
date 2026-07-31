namespace MCPTools.Server.Models;

/// <summary>
/// Represents the result of validating an MCP-style tool request before execution.
/// </summary>
public sealed class McpSecurityValidationResult
{
    /// <summary>
    /// Gets a value indicating whether validation succeeded.
    /// </summary>
    public bool Success { get; init; }

    /// <summary>
    /// Gets the matched tool descriptor when validation succeeds.
    /// </summary>
    public ToolDescriptor? ToolDescriptor { get; init; }

    /// <summary>
    /// Gets the strongly typed request model when validation succeeds.
    /// </summary>
    public object? RequestModel { get; init; }

    /// <summary>
    /// Gets the structured error when validation fails.
    /// </summary>
    public McpError? Error { get; init; }
}
