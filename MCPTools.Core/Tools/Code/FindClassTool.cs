using MCPTools.Core.Constants;
using MCPTools.Core.Exceptions;
using MCPTools.Core.Interfaces;
using MCPTools.Core.Models.Tools;
using Microsoft.Extensions.Logging;

namespace MCPTools.Core.Tools.Code;

/// <summary>
/// Finds classes in a .NET solution.
/// </summary>
public sealed class FindClassTool : ToolBase<FindClassRequest, FindClassResponse>
{
    private readonly ISolutionScanner _solutionScanner;
    private readonly IRoslynParser _roslynParser;
    private readonly ILogger<FindClassTool> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="FindClassTool"/> class.
    /// </summary>
    /// <param name="solutionScanner">The solution scanner.</param>
    /// <param name="roslynParser">The Roslyn parser.</param>
    /// <param name="logger">The logger.</param>
    public FindClassTool(
        ISolutionScanner solutionScanner,
        IRoslynParser roslynParser,
        ILogger<FindClassTool> logger)
        : base(new ToolMetadata
        {
            Name = "find-class",
            DisplayName = "Find Class",
            Category = ToolMetadataConstants.Categories.Code,
            Version = "1.0.0",
            Description = "Finds class declarations in a .NET solution.",
            Tags =
            [
                ToolMetadataConstants.Tags.Analysis,
                ToolMetadataConstants.Tags.Code,
                ToolMetadataConstants.Tags.Solution
            ]
        })
    {
        _solutionScanner = solutionScanner ?? throw new ArgumentNullException(nameof(solutionScanner));
        _roslynParser = roslynParser ?? throw new ArgumentNullException(nameof(roslynParser));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public override async Task<FindClassResponse> ExecuteAsync(
        FindClassRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.SolutionPath))
        {
            throw new ToolValidationException("SolutionPath is required.");
        }

        if (string.IsNullOrWhiteSpace(request.ClassName))
        {
            throw new ToolValidationException("ClassName is required.");
        }

        _logger.LogInformation("Finding class {ClassName} in solution {SolutionPath}.", request.ClassName, request.SolutionPath);

        var solution = await _solutionScanner.ScanAsync(SolutionPathResolver.Resolve(request.SolutionPath), cancellationToken);
        var parsedSolution = await _roslynParser.ParseSolutionAsync(solution, cancellationToken);
        var matches = parsedSolution.Projects
            .SelectMany(project => project.SourceFiles.SelectMany(sourceFile => sourceFile.Namespaces.SelectMany(namespaceModel => namespaceModel.Classes.Select(classModel => new
            {
                Project = project,
                SourceFile = sourceFile,
                Namespace = namespaceModel,
                Class = classModel
            }))))
            .Where(match => string.Equals(match.Class.Name, request.ClassName, StringComparison.OrdinalIgnoreCase))
            .Select(match => new SourceCodeMatch
            {
                ProjectName = match.Project.Name,
                FilePath = match.SourceFile.Path,
                Namespace = match.Namespace.Name,
                Name = match.Class.Name,
                Kind = "Class"
            })
            .ToArray();

        return new FindClassResponse
        {
            Success = true,
            Matches = matches,
            Count = matches.Length,
            Message = $"Found {matches.Length} class match(es)."
        };
    }
}
