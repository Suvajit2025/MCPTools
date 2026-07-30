using MCPTools.Core.Models.Tools;

namespace MCPTools.Core.Interfaces;

/// <summary>
/// Defines the core contract for an executable MCPTools tool.
/// </summary>
/// <typeparam name="TRequest">The type of request accepted by the tool.</typeparam>
/// <typeparam name="TResponse">The type of response returned by the tool.</typeparam>
public interface ITool<TRequest, TResponse>
{
    /// <summary>
    /// Gets the metadata that describes the tool.
    /// </summary>
    ToolMetadata Metadata { get; }

    /// <summary>
    /// Executes the tool asynchronously using the specified request.
    /// </summary>
    /// <param name="request">The request used to execute the tool.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>The response produced by the tool.</returns>
    Task<TResponse> ExecuteAsync(
        TRequest request,
        CancellationToken cancellationToken = default);
}
