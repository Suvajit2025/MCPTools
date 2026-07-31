using System.Text.Json;
using MCPTools.Server.Models;
using Microsoft.Extensions.Logging;

namespace MCPTools.Server.Security;

/// <summary>
/// Validates and authorizes MCP-style requests before tool execution.
/// </summary>
public sealed class McpSecurityMiddleware
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    private readonly ToolCatalog _toolCatalog;
    private readonly IMcpAuthenticationHandler _authenticationHandler;
    private readonly IMcpAuthorizationHandler _authorizationHandler;
    private readonly IMcpPermissionEvaluator _permissionEvaluator;
    private readonly ILogger<McpSecurityMiddleware> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="McpSecurityMiddleware"/> class.
    /// </summary>
    /// <param name="toolCatalog">The catalog of available tools.</param>
    /// <param name="authenticationHandler">The authentication extension point.</param>
    /// <param name="authorizationHandler">The authorization extension point.</param>
    /// <param name="permissionEvaluator">The permission extension point.</param>
    /// <param name="logger">The logger used to record security decisions.</param>
    public McpSecurityMiddleware(
        ToolCatalog toolCatalog,
        IMcpAuthenticationHandler authenticationHandler,
        IMcpAuthorizationHandler authorizationHandler,
        IMcpPermissionEvaluator permissionEvaluator,
        ILogger<McpSecurityMiddleware> logger)
    {
        _toolCatalog = toolCatalog ?? throw new ArgumentNullException(nameof(toolCatalog));
        _authenticationHandler = authenticationHandler ?? throw new ArgumentNullException(nameof(authenticationHandler));
        _authorizationHandler = authorizationHandler ?? throw new ArgumentNullException(nameof(authorizationHandler));
        _permissionEvaluator = permissionEvaluator ?? throw new ArgumentNullException(nameof(permissionEvaluator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Validates the specified request before tool execution.
    /// </summary>
    /// <param name="request">The incoming MCP-style request.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>The request validation result.</returns>
    public async Task<McpSecurityValidationResult> ValidateAsync(
        McpRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(request.ToolName))
        {
            _logger.LogWarning("Rejected MCP request {RequestId} because ToolName was missing.", request.RequestId);
            return CreateFailure("ValidationError", "ToolName is required.", nameof(request.ToolName));
        }

        if (!_toolCatalog.TryGet(request.ToolName, out var descriptor) || descriptor is null)
        {
            _logger.LogWarning("Rejected MCP request {RequestId} for unknown tool {ToolName}.", request.RequestId, request.ToolName);
            return CreateFailure("ToolNotFound", $"Tool '{request.ToolName}' was not found.", request.ToolName);
        }

        var authenticationDecision = await _authenticationHandler.AuthenticateAsync(request, cancellationToken);

        if (!authenticationDecision.Allowed)
        {
            _logger.LogWarning("Rejected MCP request {RequestId} for tool {ToolName} during authentication.", request.RequestId, request.ToolName);
            return CreateFailure(authenticationDecision, "AuthenticationFailed", descriptor.ToolName);
        }

        var authorizationDecision = await _authorizationHandler.AuthorizeAsync(request, descriptor, cancellationToken);

        if (!authorizationDecision.Allowed)
        {
            _logger.LogWarning("Rejected MCP request {RequestId} for tool {ToolName} during authorization.", request.RequestId, descriptor.ToolName);
            return CreateFailure(authorizationDecision, "AuthorizationFailed", descriptor.ToolName);
        }

        var permissionDecision = await _permissionEvaluator.EvaluateAsync(request, descriptor, cancellationToken);

        if (!permissionDecision.Allowed)
        {
            _logger.LogWarning("Rejected MCP request {RequestId} for tool {ToolName} during permission evaluation.", request.RequestId, descriptor.ToolName);
            return CreateFailure(permissionDecision, "PermissionDenied", descriptor.ToolName);
        }

        try
        {
            var requestModel = request.Input.Deserialize(descriptor.RequestType, SerializerOptions);

            if (requestModel is null)
            {
                _logger.LogWarning("Rejected MCP request {RequestId} for tool {ToolName} because the input was null.", request.RequestId, descriptor.ToolName);
                return CreateFailure("ValidationError", "The request input could not be deserialized.", descriptor.ToolName);
            }

            return new McpSecurityValidationResult
            {
                Success = true,
                ToolDescriptor = descriptor,
                RequestModel = requestModel
            };
        }
        catch (JsonException exception)
        {
            _logger.LogWarning(exception, "Rejected MCP request {RequestId} for tool {ToolName} because the input model was invalid.", request.RequestId, descriptor.ToolName);
            return CreateFailure("ValidationError", "The request input could not be deserialized.", descriptor.ToolName);
        }
    }

    private static McpSecurityValidationResult CreateFailure(
        McpSecurityDecision decision,
        string defaultCode,
        string? target)
    {
        return CreateFailure(
            string.IsNullOrWhiteSpace(decision.ErrorCode) ? defaultCode : decision.ErrorCode,
            string.IsNullOrWhiteSpace(decision.Message) ? "The request was rejected." : decision.Message,
            target);
    }

    private static McpSecurityValidationResult CreateFailure(
        string code,
        string message,
        string? target)
    {
        return new McpSecurityValidationResult
        {
            Success = false,
            Error = new McpError
            {
                Code = code,
                Message = message,
                Target = target
            }
        };
    }
}
