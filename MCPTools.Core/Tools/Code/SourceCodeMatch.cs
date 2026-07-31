namespace MCPTools.Core.Tools.Code;

/// <summary>
/// Represents a source code location matched by a code intelligence tool.
/// </summary>
public sealed class SourceCodeMatch
{
    /// <summary>
    /// Gets the project name that contains the match.
    /// </summary>
    public string? ProjectName { get; init; }

    /// <summary>
    /// Gets the source file path that contains the match.
    /// </summary>
    public string? FilePath { get; init; }

    /// <summary>
    /// Gets the namespace that contains the match.
    /// </summary>
    public string? Namespace { get; init; }

    /// <summary>
    /// Gets the matched symbol name.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Gets the matched symbol kind.
    /// </summary>
    public string? Kind { get; init; }
}
