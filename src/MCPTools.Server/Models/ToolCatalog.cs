namespace MCPTools.Server.Models;

/// <summary>
/// Represents the internal catalog of MCPTools tools available to the server host.
/// </summary>
public sealed class ToolCatalog
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ToolCatalog"/> class.
    /// </summary>
    /// <param name="tools">The discovered tool descriptors.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="tools"/> is <see langword="null"/>.</exception>
    public ToolCatalog(IEnumerable<ToolDescriptor> tools)
    {
        ArgumentNullException.ThrowIfNull(tools);

        Tools = tools
            .OrderBy(tool => tool.ToolName, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    /// <summary>
    /// Gets the discovered tool descriptors.
    /// </summary>
    public IReadOnlyList<ToolDescriptor> Tools { get; }

    /// <summary>
    /// Gets the number of discovered tools.
    /// </summary>
    public int Count => Tools.Count;

    /// <summary>
    /// Attempts to get a tool descriptor by tool name.
    /// </summary>
    /// <param name="toolName">The tool name.</param>
    /// <param name="descriptor">When this method returns, contains the matching descriptor if found.</param>
    /// <returns><see langword="true"/> when the descriptor exists; otherwise, <see langword="false"/>.</returns>
    public bool TryGet(string toolName, out ToolDescriptor? descriptor)
    {
        ArgumentNullException.ThrowIfNull(toolName);

        descriptor = Tools.FirstOrDefault(tool =>
            string.Equals(tool.ToolName, toolName, StringComparison.OrdinalIgnoreCase));

        return descriptor is not null;
    }
}
