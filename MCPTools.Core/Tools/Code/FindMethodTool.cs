using MCPTools.Core.Constants;
using MCPTools.Core.Exceptions;
using MCPTools.Core.Interfaces;
using MCPTools.Core.Models.Tools;
using Microsoft.Extensions.Logging;

namespace MCPTools.Core.Tools.Code;

/// <summary>
/// Finds methods in a .NET solution.
/// </summary>
public sealed class FindMethodTool : ToolBase<FindMethodRequest, FindMethodResponse>
{
    private readonly ISolutionScanner _solutionScanner;
    private readonly IRoslynParser _roslynParser;
    private readonly ILogger<FindMethodTool> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="FindMethodTool"/> class.
    /// </summary>
    /// <param name="solutionScanner">The solution scanner.</param>
    /// <param name="roslynParser">The Roslyn parser.</param>
    /// <param name="logger">The logger.</param>
    public FindMethodTool(
        ISolutionScanner solutionScanner,
        IRoslynParser roslynParser,
        ILogger<FindMethodTool> logger)
        : base(new ToolMetadata
        {
            Name = "find-method",
            DisplayName = "Find Method",
            Category = ToolMetadataConstants.Categories.Code,
            Version = "1.0.0",
            Description = "Finds method declarations in a .NET solution.",
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
    public override async Task<FindMethodResponse> ExecuteAsync(
        FindMethodRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.SolutionPath))
        {
            throw new ToolValidationException("SolutionPath is required.");
        }

        if (string.IsNullOrWhiteSpace(request.MethodName))
        {
            throw new ToolValidationException("MethodName is required.");
        }

        _logger.LogInformation("Finding method {MethodName} in solution {SolutionPath}.", request.MethodName, request.SolutionPath);

        var solution = await _solutionScanner.ScanAsync(SolutionPathResolver.Resolve(request.SolutionPath), cancellationToken);
        var parsedSolution = await _roslynParser.ParseSolutionAsync(solution, cancellationToken);
        var matches = parsedSolution.Projects
            .SelectMany(project => project.SourceFiles.SelectMany(sourceFile => sourceFile.Namespaces.SelectMany(namespaceModel =>
                namespaceModel.Classes.SelectMany(classModel => classModel.Methods.Select(method => new
                {
                    Project = project,
                    SourceFile = sourceFile,
                    Namespace = namespaceModel,
                    Method = method
                }))
                .Concat(namespaceModel.Interfaces.SelectMany(interfaceModel => interfaceModel.Methods.Select(method => new
                {
                    Project = project,
                    SourceFile = sourceFile,
                    Namespace = namespaceModel,
                    Method = method
                })))
                .Concat(namespaceModel.Records.SelectMany(recordModel => recordModel.Methods.Select(method => new
                {
                    Project = project,
                    SourceFile = sourceFile,
                    Namespace = namespaceModel,
                    Method = method
                }))))))
            .Where(match => string.Equals(match.Method.Name, request.MethodName, StringComparison.OrdinalIgnoreCase))
            .Select(match => new SourceCodeMatch
            {
                ProjectName = match.Project.Name,
                FilePath = match.SourceFile.Path,
                Namespace = match.Namespace.Name,
                Name = match.Method.Name,
                Kind = "Method"
            })
            .ToArray();

        return new FindMethodResponse
        {
            Success = true,
            Matches = matches,
            Count = matches.Length,
            Message = $"Found {matches.Length} method match(es)."
        };
    }
}
