using MCPTools.Core.Models.Solution;
using MCPTools.Core.Models.Solution.Syntax;

namespace MCPTools.Core.Interfaces;

/// <summary>
/// Defines a read-only analyzer for discovering source and project dependencies.
/// </summary>
public interface IDependencyAnalyzer
{
    /// <summary>
    /// Analyzes dependencies for an existing solution model.
    /// </summary>
    /// <param name="solution">The solution model to analyze.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>The discovered dependency graph edges.</returns>
    Task<IReadOnlyList<DependencyModel>> AnalyzeSolutionAsync(
        SolutionModel solution,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Analyzes dependencies for an existing project model.
    /// </summary>
    /// <param name="project">The project model to analyze.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>The discovered dependency graph edges.</returns>
    Task<IReadOnlyList<DependencyModel>> AnalyzeProjectAsync(
        ProjectModel project,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Analyzes dependencies for a single C# source file.
    /// </summary>
    /// <param name="sourceFilePath">The C# source file path.</param>
    /// <param name="projectName">The optional project name that owns the file.</param>
    /// <param name="knownSolutionTypes">Known type names declared by the analyzed solution.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>The discovered dependency graph edges.</returns>
    Task<IReadOnlyList<DependencyModel>> AnalyzeFileAsync(
        string sourceFilePath,
        string? projectName = null,
        IReadOnlySet<string>? knownSolutionTypes = null,
        CancellationToken cancellationToken = default);
}
