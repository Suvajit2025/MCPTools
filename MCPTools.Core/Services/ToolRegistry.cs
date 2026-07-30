using MCPTools.Core.Interfaces;

namespace MCPTools.Core.Services;

/// <summary>
/// Maintains the collection of registered MCPTools tools.
/// </summary>
public sealed class ToolRegistry
{
    private readonly Dictionary<string, object> _tools = new(StringComparer.OrdinalIgnoreCase);
    private readonly object _syncRoot = new();

    /// <summary>
    /// Gets the number of registered tools.
    /// </summary>
    public int Count
    {
        get
        {
            lock (_syncRoot)
            {
                return _tools.Count;
            }
        }
    }

    /// <summary>
    /// Registers the specified tool.
    /// </summary>
    /// <typeparam name="TRequest">The type of request accepted by the tool.</typeparam>
    /// <typeparam name="TResponse">The type of response returned by the tool.</typeparam>
    /// <param name="tool">The tool to register.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="tool"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">Thrown when a tool with the same name is already registered.</exception>
    public void Register<TRequest, TResponse>(ITool<TRequest, TResponse> tool)
    {
        ArgumentNullException.ThrowIfNull(tool);

        var toolName = tool.Metadata.Name;

        lock (_syncRoot)
        {
            if (_tools.ContainsKey(toolName))
            {
                throw new InvalidOperationException(
                    $"A tool with the name '{toolName}' is already registered.");
            }

            _tools.Add(toolName, tool);
        }
    }

    /// <summary>
    /// Attempts to get a registered tool by name.
    /// </summary>
    /// <param name="toolName">The name of the tool to retrieve.</param>
    /// <param name="tool">When this method returns, contains the registered tool if found; otherwise, <see langword="null"/>.</param>
    /// <returns><see langword="true"/> when the tool is found; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="toolName"/> is <see langword="null"/>.</exception>
    public bool TryGet(string toolName, out object? tool)
    {
        ArgumentNullException.ThrowIfNull(toolName);

        lock (_syncRoot)
        {
            return _tools.TryGetValue(toolName, out tool);
        }
    }

    /// <summary>
    /// Determines whether a tool with the specified name is registered.
    /// </summary>
    /// <param name="toolName">The name of the tool to locate.</param>
    /// <returns><see langword="true"/> when the tool is registered; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="toolName"/> is <see langword="null"/>.</exception>
    public bool Contains(string toolName)
    {
        ArgumentNullException.ThrowIfNull(toolName);

        lock (_syncRoot)
        {
            return _tools.ContainsKey(toolName);
        }
    }

    /// <summary>
    /// Gets all registered tools.
    /// </summary>
    /// <returns>A read-only collection containing all registered tools.</returns>
    public IReadOnlyCollection<object> GetAll()
    {
        lock (_syncRoot)
        {
            return _tools.Values.ToArray();
        }
    }
}
