using MCPTools.Server.Models;

namespace MCPTools.Server.Security;

/// <summary>
/// Provides the default authorization behavior that allows requests to continue.
/// </summary>
public sealed class AllowAllAuthorizationHandler : IMcpAuthorizationHandler
{
    /// <inheritdoc />
    public Task<McpSecurityDecision> AuthorizeAsync(
        McpRequest request,
        ToolDescriptor toolDescriptor,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new McpSecurityDecision { Allowed = true });
    }
}
