using MCPTools.Server.Models;

namespace MCPTools.Server.Security;

/// <summary>
/// Defines an extension point for authenticating incoming MCP-style requests.
/// </summary>
public interface IMcpAuthenticationHandler
{
    /// <summary>
    /// Authenticates the specified request.
    /// </summary>
    /// <param name="request">The incoming MCP-style request.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>The authentication decision.</returns>
    Task<McpSecurityDecision> AuthenticateAsync(
        McpRequest request,
        CancellationToken cancellationToken = default);
}
