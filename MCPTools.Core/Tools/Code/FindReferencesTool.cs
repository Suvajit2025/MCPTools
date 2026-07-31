using MCPTools.Core.Constants;
using MCPTools.Core.Exceptions;
using MCPTools.Core.Interfaces;
using MCPTools.Core.Models.Tools;
using Microsoft.Extensions.Logging;

namespace MCPTools.Core.Tools.Code;

/// <summary>
/// Finds source-level references in a .NET solution.
/// </summary>
public sealed class FindReferencesTool : ToolBase<FindReferencesRequest, FindReferencesResponse>
{
    private readonly ISolutionScanner _solutionScanner;
    private readonly IRoslynParser _roslynParser;
    private readonly IDependencyAnalyzer _dependencyAnalyzer;
    private readonly ILogger<FindReferencesTool> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="FindReferencesTool"/> class.
    /// </summary>
    /// <param name="solutionScanner">The solution scanner.</param>
    /// <param name="roslynParser">The Roslyn parser.</param>
    /// <param name="dependencyAnalyzer">The dependency analyzer.</param>
    /// <param name="logger">The logger.</param>
    public FindReferencesTool(
        ISolutionScanner solutionScanner,
        IRoslynParser roslynParser,
        IDependencyAnalyzer dependencyAnalyzer,
        ILogger<FindReferencesTool> logger)
        : base(new ToolMetadata
        {
            Name = "find-references",
            DisplayName = "Find References",
            Category = ToolMetadataConstants.Categories.Code,
            Version = "1.0.0",
            Description = "Finds source-level references in a .NET solution.",
            Tags =
            [
                ToolMetadataConstants.Tags.Analysis,
                ToolMetadataConstants.Tags.Code,
                ToolMetadataConstants.Tags.References
            ]
        })
    {
        _solutionScanner = solutionScanner ?? throw new ArgumentNullException(nameof(solutionScanner));
        _roslynParser = roslynParser ?? throw new ArgumentNullException(nameof(roslynParser));
        _dependencyAnalyzer = dependencyAnalyzer ?? throw new ArgumentNullException(nameof(dependencyAnalyzer));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public override async Task<FindReferencesResponse> ExecuteAsync(
        FindReferencesRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.SolutionPath))
        {
            throw new ToolValidationException("SolutionPath is required.");
        }

        if (string.IsNullOrWhiteSpace(request.SymbolName))
        {
            throw new ToolValidationException("SymbolName is required.");
        }

        _logger.LogInformation("Finding references for {SymbolName} in solution {SolutionPath}.", request.SymbolName, request.SolutionPath);

        var solution = await _solutionScanner.ScanAsync(SolutionPathResolver.Resolve(request.SolutionPath), cancellationToken);
        var parsedSolution = await _roslynParser.ParseSolutionAsync(solution, cancellationToken);
        var dependencies = await _dependencyAnalyzer.AnalyzeSolutionAsync(parsedSolution, cancellationToken);
        var matches = dependencies
            .Where(dependency => string.Equals(dependency.Name, request.SymbolName, StringComparison.OrdinalIgnoreCase))
            .ToArray();

        return new FindReferencesResponse
        {
            Success = true,
            References = matches,
            Count = matches.Length,
            Message = $"Found {matches.Length} reference match(es)."
        };
    }
}
