using MCPTools.Core.Models.Solution;

namespace MCPTools.Core.Interfaces;

/// <summary>
/// Defines a scanner capable of building a model of a .NET solution from the file system.
/// </summary>
public interface ISolutionScanner
{
    /// <summary>
    /// Scans the specified solution file and builds a solution model.
    /// </summary>
    /// <param name="solutionPath">The path to a .NET solution file.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>The discovered solution model.</returns>
    Task<SolutionModel> ScanAsync(
        string solutionPath,
        CancellationToken cancellationToken = default);
}
