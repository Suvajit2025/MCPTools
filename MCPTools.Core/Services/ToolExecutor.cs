using MCPTools.Core.Interfaces;

namespace MCPTools.Core.Services;

/// <summary>
/// Executes tools that have been registered in a <see cref="ToolRegistry"/>.
/// </summary>
public sealed class ToolExecutor
{
    private readonly ToolRegistry _toolRegistry;

    /// <summary>
    /// Initializes a new instance of the <see cref="ToolExecutor"/> class.
    /// </summary>
    /// <param name="toolRegistry">The registry used to locate registered tools.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="toolRegistry"/> is <see langword="null"/>.</exception>
    public ToolExecutor(ToolRegistry toolRegistry)
    {
        _toolRegistry = toolRegistry ?? throw new ArgumentNullException(nameof(toolRegistry));
    }

    /// <summary>
    /// Executes a registered tool asynchronously.
    /// </summary>
    /// <typeparam name="TRequest">The type of request accepted by the tool.</typeparam>
    /// <typeparam name="TResponse">The type of response returned by the tool.</typeparam>
    /// <param name="toolName">The name of the registered tool to execute.</param>
    /// <param name="request">The request used to execute the tool.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>The response produced by the tool.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="toolName"/> is empty or whitespace.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="toolName"/> or <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the tool is not registered or does not match the requested contract.</exception>
    public Task<TResponse> ExecuteAsync<TRequest, TResponse>(
        string toolName,
        TRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(toolName);
        ArgumentNullException.ThrowIfNull(request);

        if (!_toolRegistry.TryGet(toolName, out var registeredTool))
        {
            throw new InvalidOperationException(
                $"No tool with the name '{toolName}' is registered.");
        }

        if (registeredTool is not ITool<TRequest, TResponse> tool)
        {
            throw new InvalidOperationException(
                $"The registered tool '{toolName}' does not implement ITool<{typeof(TRequest).Name}, {typeof(TResponse).Name}>.");
        }

        return tool.ExecuteAsync(request, cancellationToken);
    }
}
