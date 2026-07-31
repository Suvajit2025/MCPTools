using MCPTools.Core.Models.Solution.Syntax;

namespace MCPTools.Core.Tools.Code;

/// <summary>
/// Represents the result of finding references in a .NET solution.
/// </summary>
public sealed class FindReferencesResponse
{
    /// <summary>
    /// Gets a value indicating whether the search completed successfully.
    /// </summary>
    public bool Success { get; init; }

    /// <summary>
    /// Gets the matched dependency references.
    /// </summary>
    public IReadOnlyList<DependencyModel> References { get; init; } = [];

    /// <summary>
    /// Gets the number of matched references.
    /// </summary>
    public int Count { get; init; }

    /// <summary>
    /// Gets a human-readable summary of the search result.
    /// </summary>
    public string? Message { get; init; }
}
