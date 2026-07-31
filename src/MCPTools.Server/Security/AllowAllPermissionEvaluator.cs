using MCPTools.Server.Models;

namespace MCPTools.Server.Security;

/// <summary>
/// Provides the default permission behavior that allows requests to continue.
/// </summary>
public sealed class AllowAllPermissionEvaluator : IMcpPermissionEvaluator
{
    /// <inheritdoc />
    public Task<McpSecurityDecision> EvaluateAsync(
        McpRequest request,
        ToolDescriptor toolDescriptor,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new McpSecurityDecision { Allowed = true });
    }
}
