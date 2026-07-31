namespace MCPTools.Core.Tools.Code;

/// <summary>
/// Represents the result of finding methods in a .NET solution.
/// </summary>
public sealed class FindMethodResponse
{
    /// <summary>
    /// Gets a value indicating whether the search completed successfully.
    /// </summary>
    public bool Success { get; init; }

    /// <summary>
    /// Gets the matched methods.
    /// </summary>
    public IReadOnlyList<SourceCodeMatch> Matches { get; init; } = [];

    /// <summary>
    /// Gets the number of matched methods.
    /// </summary>
    public int Count { get; init; }

    /// <summary>
    /// Gets a human-readable summary of the search result.
    /// </summary>
    public string? Message { get; init; }
}
