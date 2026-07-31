namespace MCPTools.Core.Tools.Code;

/// <summary>
/// Represents a request to find classes in a .NET solution.
/// </summary>
public sealed class FindClassRequest
{
    /// <summary>
    /// Gets the solution file path or directory containing a solution file.
    /// </summary>
    public required string SolutionPath { get; init; }

    /// <summary>
    /// Gets the class name to find.
    /// </summary>
    public required string ClassName { get; init; }
}
