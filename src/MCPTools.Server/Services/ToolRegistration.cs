namespace MCPTools.Server.Services;

/// <summary>
/// Represents a tool implementation type registered with the server host.
/// </summary>
public sealed class ToolRegistration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ToolRegistration"/> class.
    /// </summary>
    /// <param name="toolType">The registered tool implementation type.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="toolType"/> is <see langword="null"/>.</exception>
    public ToolRegistration(Type toolType)
    {
        ToolType = toolType ?? throw new ArgumentNullException(nameof(toolType));
    }

    /// <summary>
    /// Gets the registered tool implementation type.
    /// </summary>
    public Type ToolType { get; }
}
