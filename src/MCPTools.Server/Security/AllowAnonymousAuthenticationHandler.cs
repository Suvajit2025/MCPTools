using MCPTools.Server.Models;

namespace MCPTools.Server.Security;

/// <summary>
/// Provides the default authentication behavior that allows requests to continue.
/// </summary>
public sealed class AllowAnonymousAuthenticationHandler : IMcpAuthenticationHandler
{
    /// <inheritdoc />
    public Task<McpSecurityDecision> AuthenticateAsync(
        McpRequest request,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new McpSecurityDecision { Allowed = true });
    }
}
