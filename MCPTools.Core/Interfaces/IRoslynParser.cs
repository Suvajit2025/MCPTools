using MCPTools.Core.Models.Solution;

namespace MCPTools.Core.Interfaces;

/// <summary>
/// Defines a read-only parser for extracting source code structure from C# files using Roslyn.
/// </summary>
public interface IRoslynParser
{
    /// <summary>
    /// Parses a single C# source file.
    /// </summary>
    /// <param name="sourceFilePath">The path to the C# source file.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>The parsed source file model.</returns>
    Task<SourceFileModel> ParseFileAsync(
        string sourceFilePath,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Parses multiple C# source files.
    /// </summary>
    /// <param name="sourceFilePaths">The source file paths to parse.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>The parsed source file models.</returns>
    Task<IReadOnlyList<SourceFileModel>> ParseFilesAsync(
        IEnumerable<string> sourceFilePaths,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Parses all source files contained in an existing solution model.
    /// </summary>
    /// <param name="solution">The solution model to enrich with parsed source metadata.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>A solution model containing parsed source metadata.</returns>
    Task<SolutionModel> ParseSolutionAsync(
        SolutionModel solution,
        CancellationToken cancellationToken = default);
}
