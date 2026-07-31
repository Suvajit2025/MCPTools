namespace MCPTools.Core.Tools.Code;

/// <summary>
/// Represents a request to find references in a .NET solution.
/// </summary>
public sealed class FindReferencesRequest
{
    /// <summary>
    /// Gets the solution file path or directory containing a solution file.
    /// </summary>
    public required string SolutionPath { get; init; }

    /// <summary>
    /// Gets the symbol name to find references for.
    /// </summary>
    public required string SymbolName { get; init; }
}
