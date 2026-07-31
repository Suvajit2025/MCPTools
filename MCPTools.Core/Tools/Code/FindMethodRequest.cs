namespace MCPTools.Core.Tools.Code;

/// <summary>
/// Represents a request to find methods in a .NET solution.
/// </summary>
public sealed class FindMethodRequest
{
    /// <summary>
    /// Gets the solution file path or directory containing a solution file.
    /// </summary>
    public required string SolutionPath { get; init; }

    /// <summary>
    /// Gets the method name to find.
    /// </summary>
    public required string MethodName { get; init; }
}
