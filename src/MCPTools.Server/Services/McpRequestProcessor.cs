using System.Reflection;
using System.Text.Json;
using MCPTools.Core.Exceptions;
using MCPTools.Core.Interfaces;
using MCPTools.Server.Models;
using MCPTools.Server.Security;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MCPTools.Server.Services;

/// <summary>
/// Processes MCP-style tool requests against the registered MCPTools tool catalog.
/// </summary>
public sealed class McpRequestProcessor
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly McpSecurityMiddleware _securityMiddleware;
    private readonly ILogger<McpRequestProcessor> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="McpRequestProcessor"/> class.
    /// </summary>
    /// <param name="serviceScopeFactory">The service scope factory used to resolve tool instances.</param>
    /// <param name="securityMiddleware">The security middleware used to validate requests before execution.</param>
    /// <param name="logger">The logger used to record processing activity.</param>
    /// <exception cref="ArgumentNullException">Thrown when a required dependency is <see langword="null"/>.</exception>
    public McpRequestProcessor(
        IServiceScopeFactory serviceScopeFactory,
        McpSecurityMiddleware securityMiddleware,
        ILogger<McpRequestProcessor> logger)
    {
        _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
        _securityMiddleware = securityMiddleware ?? throw new ArgumentNullException(nameof(securityMiddleware));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Processes the specified MCP-style request.
    /// </summary>
    /// <param name="request">The incoming request.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>The structured processing result.</returns>
    public async Task<McpResult> ProcessAsync(
        McpRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        McpSecurityValidationResult validationResult;

        try
        {
            validationResult = await _securityMiddleware.ValidateAsync(request, cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation("MCP request {RequestId} for tool {ToolName} was cancelled during security validation.", request.RequestId, request.ToolName);
            return CreateErrorResult(request.RequestId, "Cancelled", "The request was cancelled.", request.ToolName);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unhandled security error while processing tool {ToolName}.", request.ToolName);
            return CreateErrorResult(request.RequestId, "InternalError", "An unexpected error occurred while processing the request.", request.ToolName);
        }

        if (!validationResult.Success)
        {
            return CreateErrorResult(request.RequestId, validationResult.Error);
        }

        var descriptor = validationResult.ToolDescriptor
            ?? throw new InvalidOperationException("Security validation succeeded without a tool descriptor.");
        var toolRequest = validationResult.RequestModel
            ?? throw new InvalidOperationException("Security validation succeeded without a request model.");

        _logger.LogInformation(
            "Processing MCP request {RequestId} for tool {ToolName}.",
            request.RequestId,
            descriptor.ToolName);

        try
        {
            var response = await ExecuteToolAsync(descriptor, toolRequest, cancellationToken);
            var serializedResponse = JsonSerializer.SerializeToElement(response, descriptor.ResponseType, SerializerOptions);

            _logger.LogInformation(
                "Processed MCP request {RequestId} for tool {ToolName}.",
                request.RequestId,
                descriptor.ToolName);

            return new McpResult
            {
                RequestId = request.RequestId,
                Success = true,
                Result = serializedResponse
            };
        }
        catch (ToolValidationException exception)
        {
            _logger.LogWarning(exception, "Validation failed for tool {ToolName}.", descriptor.ToolName);
            return CreateErrorResult(request.RequestId, "ValidationError", exception.Message, descriptor.ToolName);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation("MCP request {RequestId} for tool {ToolName} was cancelled.", request.RequestId, descriptor.ToolName);
            return CreateErrorResult(request.RequestId, "Cancelled", "The request was cancelled.", descriptor.ToolName);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unhandled error while processing tool {ToolName}.", descriptor.ToolName);
            return CreateErrorResult(request.RequestId, "InternalError", "An unexpected error occurred while processing the request.", descriptor.ToolName);
        }
    }

    private async Task<object> ExecuteToolAsync(
        ToolDescriptor descriptor,
        object request,
        CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var tool = scope.ServiceProvider.GetRequiredService(descriptor.ToolType);
        var toolContract = typeof(ITool<,>).MakeGenericType(descriptor.RequestType, descriptor.ResponseType);
        var executeMethod = toolContract.GetMethod(
            nameof(ITool<object, object>.ExecuteAsync),
            BindingFlags.Instance | BindingFlags.Public)
            ?? throw new InvalidOperationException($"Tool '{descriptor.ToolName}' does not expose ExecuteAsync.");
        object execution;

        try
        {
            execution = executeMethod.Invoke(tool, [request, cancellationToken])
                ?? throw new InvalidOperationException($"Tool '{descriptor.ToolName}' returned a null task.");
        }
        catch (TargetInvocationException exception) when (exception.InnerException is not null)
        {
            throw exception.InnerException;
        }

        try
        {
            await ((Task)execution).ConfigureAwait(false);
        }
        catch (TargetInvocationException exception) when (exception.InnerException is not null)
        {
            throw exception.InnerException;
        }

        var resultProperty = execution.GetType().GetProperty(nameof(Task<object>.Result))
            ?? throw new InvalidOperationException($"Tool '{descriptor.ToolName}' did not return a Task result.");

        return resultProperty.GetValue(execution)
            ?? throw new InvalidOperationException($"Tool '{descriptor.ToolName}' returned a null response.");
    }

    private static McpResult CreateErrorResult(
        string? requestId,
        string code,
        string message,
        string? target)
    {
        return new McpResult
        {
            RequestId = requestId,
            Success = false,
            Error = new McpError
            {
                Code = code,
                Message = message,
                Target = target
            }
        };
    }

    private static McpResult CreateErrorResult(string? requestId, McpError? error)
    {
        return new McpResult
        {
            RequestId = requestId,
            Success = false,
            Error = error ?? new McpError
            {
                Code = "InternalError",
                Message = "An unexpected error occurred while processing the request."
            }
        };
    }
}
