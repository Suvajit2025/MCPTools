namespace MCPTools.Core.Tools.Solution;

/// <summary>
/// Represents a request to analyze a .NET solution.
/// </summary>
public sealed class AnalyzeSolutionRequest
{
    /// <summary>
    /// Gets the path to a solution file or a directory containing a solution file.
    /// </summary>
    public required string SolutionPath { get; init; }
}
