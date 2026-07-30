using System.Collections.Concurrent;

namespace MCPTools.Core.Models.Tools;

/// <summary>
/// Represents execution-related context shared during the execution of a tool.
/// </summary>
public sealed class ToolContext
{
    /// <summary>
    /// Gets the unique identifier for the current tool execution.
    /// </summary>
    public Guid CorrelationId { get; init; } = Guid.NewGuid();

    /// <summary>
    /// Gets the timestamp when the context was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets custom execution data associated with the tool context.
    /// </summary>
    public IDictionary<string, object?> Properties { get; init; } =
        new ConcurrentDictionary<string, object?>();
}
