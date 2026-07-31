using System.Diagnostics;
using MCPTools.Core.Constants;
using MCPTools.Core.Exceptions;
using MCPTools.Core.Interfaces;
using MCPTools.Core.Models.Solution;
using MCPTools.Core.Models.Solution.Syntax;
using MCPTools.Core.Models.Tools;
using Microsoft.Extensions.Logging;
using SyntaxClassModel = MCPTools.Core.Models.Solution.Syntax.ClassModel;

namespace MCPTools.Core.Tools.Solution;

/// <summary>
/// Analyzes a .NET solution and returns structural and dependency intelligence.
/// </summary>
public sealed class AnalyzeSolutionTool : ToolBase<AnalyzeSolutionRequest, SolutionAnalysisResult>
{
    private readonly ISolutionScanner _solutionScanner;
    private readonly IRoslynParser _roslynParser;
    private readonly IDependencyAnalyzer _dependencyAnalyzer;
    private readonly ILogger<AnalyzeSolutionTool> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AnalyzeSolutionTool"/> class.
    /// </summary>
    /// <param name="solutionScanner">The solution scanner used to discover projects and source files.</param>
    /// <param name="roslynParser">The Roslyn parser used to extract source structure.</param>
    /// <param name="dependencyAnalyzer">The dependency analyzer used to build dependency graph edges.</param>
    /// <param name="logger">The logger used to record analysis activity.</param>
    public AnalyzeSolutionTool(
        ISolutionScanner solutionScanner,
        IRoslynParser roslynParser,
        IDependencyAnalyzer dependencyAnalyzer,
        ILogger<AnalyzeSolutionTool> logger)
        : base(new ToolMetadata
        {
            Name = "analyze-solution",
            DisplayName = "Analyze Solution",
            Category = ToolMetadataConstants.Categories.Solution,
            Version = "1.0.0",
            Description = "Analyzes a .NET solution and returns projects, source structure, and dependencies.",
            Tags =
            [
                ToolMetadataConstants.Tags.Analysis,
                ToolMetadataConstants.Tags.Solution
            ]
        })
    {
        _solutionScanner = solutionScanner ?? throw new ArgumentNullException(nameof(solutionScanner));
        _roslynParser = roslynParser ?? throw new ArgumentNullException(nameof(roslynParser));
        _dependencyAnalyzer = dependencyAnalyzer ?? throw new ArgumentNullException(nameof(dependencyAnalyzer));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public override async Task<SolutionAnalysisResult> ExecuteAsync(
        AnalyzeSolutionRequest request,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            ValidateRequest(request);

            var solutionPath = ResolveSolutionPath(request.SolutionPath);
            _logger.LogInformation("Starting solution analysis for {SolutionPath}.", solutionPath);

            var scannedSolution = await _solutionScanner.ScanAsync(solutionPath, cancellationToken);
            var parsedSolution = await _roslynParser.ParseSolutionAsync(scannedSolution, cancellationToken);
            var dependencies = await _dependencyAnalyzer.AnalyzeSolutionAsync(parsedSolution, cancellationToken);

            stopwatch.Stop();

            _logger.LogInformation(
                "Completed solution analysis for {SolutionPath} in {ElapsedMilliseconds} ms.",
                solutionPath,
                stopwatch.ElapsedMilliseconds);

            return CreateResult(parsedSolution, dependencies, stopwatch.Elapsed);
        }
        catch (ToolValidationException)
        {
            throw;
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            _logger.LogError(exception, "Solution analysis failed for {SolutionPath}.", request?.SolutionPath);
            throw new ToolExecutionException(Metadata.Name, exception, includeToolName: true);
        }
    }

    private static void ValidateRequest(AnalyzeSolutionRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.SolutionPath))
        {
            throw new ToolValidationException("SolutionPath is required.");
        }
    }

    private static string ResolveSolutionPath(string solutionPath)
    {
        var fullPath = Path.GetFullPath(solutionPath);

        if (File.Exists(fullPath))
        {
            return fullPath;
        }

        if (!Directory.Exists(fullPath))
        {
            throw new ToolValidationException($"Solution path '{solutionPath}' does not exist.");
        }

        var solutionFiles = Directory
            .EnumerateFiles(fullPath, "*.sln", SearchOption.TopDirectoryOnly)
            .Concat(Directory.EnumerateFiles(fullPath, "*.slnx", SearchOption.TopDirectoryOnly))
            .Order(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return solutionFiles.Length switch
        {
            1 => solutionFiles[0],
            0 => throw new ToolValidationException($"Directory '{solutionPath}' does not contain a solution file."),
            _ => throw new ToolValidationException($"Directory '{solutionPath}' contains multiple solution files.")
        };
    }

    private static SolutionAnalysisResult CreateResult(
        SolutionModel solution,
        IReadOnlyList<DependencyModel> dependencies,
        TimeSpan elapsedTime)
    {
        var namespaces = GetNamespaces(solution);
        var classes = GetClasses(solution);
        var interfaces = GetInterfaces(solution);
        var methods = GetMethods(solution);
        var properties = GetProperties(solution);
        var projectReferences = solution.Projects
            .SelectMany(project => project.References)
            .OrderBy(reference => reference.Name, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return new SolutionAnalysisResult
        {
            Success = true,
            SolutionName = solution.Name,
            SolutionPath = solution.Path,
            Projects = solution.Projects,
            Namespaces = namespaces,
            Classes = classes.Select(type => type.Name).WhereNotNullOrWhiteSpace(),
            Interfaces = interfaces.Select(type => type.Name).WhereNotNullOrWhiteSpace(),
            Methods = methods.Select(method => method.Name).WhereNotNullOrWhiteSpace(),
            Properties = properties.Select(property => property.Name).WhereNotNullOrWhiteSpace(),
            Dependencies = dependencies,
            ControllersDetected = DetectControllers(classes),
            RepositoriesDetected = DetectRepositories(classes, interfaces),
            ServicesDetected = DetectServices(classes, interfaces),
            DtosDetected = DetectDtos(classes),
            EntitiesDetected = DetectEntities(classes),
            TargetFrameworks = solution.Projects.ToDictionary(project => project.Name ?? string.Empty, project => project.TargetFramework),
            OutputTypes = solution.Projects.ToDictionary(project => project.Name ?? string.Empty, project => project.OutputType),
            ProjectReferences = projectReferences,
            ProjectCount = solution.Projects.Count,
            NamespaceCount = namespaces.Count,
            ClassCount = classes.Count,
            InterfaceCount = interfaces.Count,
            MethodCount = methods.Count,
            PropertyCount = properties.Count,
            DependencyCount = dependencies.Count,
            ElapsedTime = elapsedTime,
            Message = "Solution analysis completed successfully."
        };
    }

    private static IReadOnlyList<string> GetNamespaces(SolutionModel solution)
    {
        return solution.Projects
            .SelectMany(project => project.SourceFiles)
            .SelectMany(sourceFile => sourceFile.Namespaces)
            .Select(namespaceModel => namespaceModel.Name)
            .WhereNotNullOrWhiteSpace()
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Order(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static IReadOnlyList<SyntaxClassModel> GetClasses(SolutionModel solution)
    {
        return solution.Projects
            .SelectMany(project => project.SourceFiles)
            .SelectMany(sourceFile => sourceFile.Namespaces)
            .SelectMany(namespaceModel => namespaceModel.Classes)
            .OrderBy(type => type.Name, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static IReadOnlyList<InterfaceModel> GetInterfaces(SolutionModel solution)
    {
        return solution.Projects
            .SelectMany(project => project.SourceFiles)
            .SelectMany(sourceFile => sourceFile.Namespaces)
            .SelectMany(namespaceModel => namespaceModel.Interfaces)
            .OrderBy(type => type.Name, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static IReadOnlyList<MethodModel> GetMethods(SolutionModel solution)
    {
        return solution.Projects
            .SelectMany(project => project.SourceFiles)
            .SelectMany(sourceFile => sourceFile.Namespaces)
            .SelectMany(namespaceModel => namespaceModel.Classes.SelectMany(type => type.Methods)
                .Concat(namespaceModel.Interfaces.SelectMany(type => type.Methods))
                .Concat(namespaceModel.Records.SelectMany(type => type.Methods)))
            .OrderBy(method => method.Name, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static IReadOnlyList<PropertyModel> GetProperties(SolutionModel solution)
    {
        return solution.Projects
            .SelectMany(project => project.SourceFiles)
            .SelectMany(sourceFile => sourceFile.Namespaces)
            .SelectMany(namespaceModel => namespaceModel.Classes.SelectMany(type => type.Properties)
                .Concat(namespaceModel.Interfaces.SelectMany(type => type.Properties))
                .Concat(namespaceModel.Records.SelectMany(type => type.Properties)))
            .OrderBy(property => property.Name, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static IReadOnlyList<string> DetectControllers(IEnumerable<SyntaxClassModel> classes)
    {
        return classes
            .Where(type => EndsWith(type.Name, "Controller")
                || type.BaseTypes.Any(baseType => IsNamed(baseType.Name, "ControllerBase") || IsNamed(baseType.Name, "Controller")))
            .Select(type => type.Name)
            .WhereNotNullOrWhiteSpace();
    }

    private static IReadOnlyList<string> DetectRepositories(
        IEnumerable<SyntaxClassModel> classes,
        IEnumerable<InterfaceModel> interfaces)
    {
        return classes.Select(type => type.Name)
            .Concat(interfaces.Select(type => type.Name))
            .Where(name => EndsWith(name, "Repository"))
            .WhereNotNullOrWhiteSpace();
    }

    private static IReadOnlyList<string> DetectServices(
        IEnumerable<SyntaxClassModel> classes,
        IEnumerable<InterfaceModel> interfaces)
    {
        return classes.Select(type => type.Name)
            .Concat(interfaces.Select(type => type.Name))
            .Where(name => EndsWith(name, "Service"))
            .WhereNotNullOrWhiteSpace();
    }

    private static IReadOnlyList<string> DetectDtos(IEnumerable<SyntaxClassModel> classes)
    {
        return classes
            .Select(type => type.Name)
            .Where(name => EndsWith(name, "Dto")
                || EndsWith(name, "Request")
                || EndsWith(name, "Response"))
            .WhereNotNullOrWhiteSpace();
    }

    private static IReadOnlyList<string> DetectEntities(IEnumerable<SyntaxClassModel> classes)
    {
        return classes
            .Where(type => Contains(type.Namespace, ".Domain")
                || Contains(type.Namespace, ".Entities")
                || EndsWith(type.Namespace, ".Models"))
            .Select(type => type.Name)
            .WhereNotNullOrWhiteSpace();
    }

    private static bool EndsWith(string? value, string suffix)
    {
        return value?.EndsWith(suffix, StringComparison.OrdinalIgnoreCase) == true;
    }

    private static bool Contains(string? value, string pattern)
    {
        return value?.Contains(pattern, StringComparison.OrdinalIgnoreCase) == true;
    }

    private static bool IsNamed(string? value, string name)
    {
        return string.Equals(value, name, StringComparison.OrdinalIgnoreCase);
    }
}

internal static class AnalyzeSolutionToolEnumerableExtensions
{
    public static IReadOnlyList<string> WhereNotNullOrWhiteSpace(this IEnumerable<string?> values)
    {
        return values
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Order(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }
}
