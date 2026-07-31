namespace MCPTools.Core.Tools.Code;

/// <summary>
/// Represents the result of finding classes in a .NET solution.
/// </summary>
public sealed class FindClassResponse
{
    /// <summary>
    /// Gets a value indicating whether the search completed successfully.
    /// </summary>
    public bool Success { get; init; }

    /// <summary>
    /// Gets the matched classes.
    /// </summary>
    public IReadOnlyList<SourceCodeMatch> Matches { get; init; } = [];

    /// <summary>
    /// Gets the number of matched classes.
    /// </summary>
    public int Count { get; init; }

    /// <summary>
    /// Gets a human-readable summary of the search result.
    /// </summary>
    public string? Message { get; init; }
}
