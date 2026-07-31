using MCPTools.Server.Models;

namespace MCPTools.Server.Security;

/// <summary>
/// Defines an extension point for authorizing access to a discovered tool.
/// </summary>
public interface IMcpAuthorizationHandler
{
    /// <summary>
    /// Authorizes the specified request for a tool.
    /// </summary>
    /// <param name="request">The incoming MCP-style request.</param>
    /// <param name="toolDescriptor">The requested tool descriptor.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>The authorization decision.</returns>
    Task<McpSecurityDecision> AuthorizeAsync(
        McpRequest request,
        ToolDescriptor toolDescriptor,
        CancellationToken cancellationToken = default);
}
