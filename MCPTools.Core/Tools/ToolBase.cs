using MCPTools.Core.Interfaces;
using MCPTools.Core.Models.Tools;

namespace MCPTools.Core.Tools;

/// <summary>
/// Provides a minimal base implementation for MCPTools tools.
/// </summary>
/// <typeparam name="TRequest">The type of request accepted by the tool.</typeparam>
/// <typeparam name="TResponse">The type of response returned by the tool.</typeparam>
public abstract class ToolBase<TRequest, TResponse> : ITool<TRequest, TResponse>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ToolBase{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="metadata">The metadata that describes the tool.</param>
    protected ToolBase(ToolMetadata metadata)
    {
        Metadata = metadata;
    }

    /// <inheritdoc />
    public ToolMetadata Metadata { get; }

    /// <inheritdoc />
    public abstract Task<TResponse> ExecuteAsync(
        TRequest request,
        CancellationToken cancellationToken = default);
}
