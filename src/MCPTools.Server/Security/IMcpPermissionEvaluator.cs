using MCPTools.Server.Models;

namespace MCPTools.Server.Security;

/// <summary>
/// Defines an extension point for evaluating tool-level permissions.
/// </summary>
public interface IMcpPermissionEvaluator
{
    /// <summary>
    /// Evaluates whether the request has permission to use the specified tool.
    /// </summary>
    /// <param name="request">The incoming MCP-style request.</param>
    /// <param name="toolDescriptor">The requested tool descriptor.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>The permission decision.</returns>
    Task<McpSecurityDecision> EvaluateAsync(
        McpRequest request,
        ToolDescriptor toolDescriptor,
        CancellationToken cancellationToken = default);
}
