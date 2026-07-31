using MCPTools.Core.Models.Solution;
using MCPTools.Core.Models.Solution.Syntax;

namespace MCPTools.Core.Tools.Solution;

/// <summary>
/// Represents the result of analyzing a .NET solution.
/// </summary>
public sealed class SolutionAnalysisResult
{
    /// <summary>
    /// Gets a value indicating whether solution analysis completed successfully.
    /// </summary>
    public bool Success { get; init; }

    /// <summary>
    /// Gets the analyzed solution name.
    /// </summary>
    public string? SolutionName { get; init; }

    /// <summary>
    /// Gets the analyzed solution path.
    /// </summary>
    public string? SolutionPath { get; init; }

    /// <summary>
    /// Gets the analyzed projects.
    /// </summary>
    public IReadOnlyList<ProjectModel> Projects { get; init; } = [];

    /// <summary>
    /// Gets discovered namespace names.
    /// </summary>
    public IReadOnlyList<string> Namespaces { get; init; } = [];

    /// <summary>
    /// Gets discovered class names.
    /// </summary>
    public IReadOnlyList<string> Classes { get; init; } = [];

    /// <summary>
    /// Gets discovered interface names.
    /// </summary>
    public IReadOnlyList<string> Interfaces { get; init; } = [];

    /// <summary>
    /// Gets discovered method names.
    /// </summary>
    public IReadOnlyList<string> Methods { get; init; } = [];

    /// <summary>
    /// Gets discovered property names.
    /// </summary>
    public IReadOnlyList<string> Properties { get; init; } = [];

    /// <summary>
    /// Gets discovered dependencies.
    /// </summary>
    public IReadOnlyList<DependencyModel> Dependencies { get; init; } = [];

    /// <summary>
    /// Gets detected controller type names.
    /// </summary>
    public IReadOnlyList<string> ControllersDetected { get; init; } = [];

    /// <summary>
    /// Gets detected repository type names.
    /// </summary>
    public IReadOnlyList<string> RepositoriesDetected { get; init; } = [];

    /// <summary>
    /// Gets detected service type names.
    /// </summary>
    public IReadOnlyList<string> ServicesDetected { get; init; } = [];

    /// <summary>
    /// Gets detected DTO type names.
    /// </summary>
    public IReadOnlyList<string> DtosDetected { get; init; } = [];

    /// <summary>
    /// Gets detected entity type names.
    /// </summary>
    public IReadOnlyList<string> EntitiesDetected { get; init; } = [];

    /// <summary>
    /// Gets target frameworks by project name.
    /// </summary>
    public IReadOnlyDictionary<string, string?> TargetFrameworks { get; init; } = new Dictionary<string, string?>();

    /// <summary>
    /// Gets output types by project name.
    /// </summary>
    public IReadOnlyDictionary<string, string?> OutputTypes { get; init; } = new Dictionary<string, string?>();

    /// <summary>
    /// Gets project references discovered in the solution.
    /// </summary>
    public IReadOnlyList<ProjectReferenceModel> ProjectReferences { get; init; } = [];

    /// <summary>
    /// Gets the number of projects discovered.
    /// </summary>
    public int ProjectCount { get; init; }

    /// <summary>
    /// Gets the number of namespaces discovered.
    /// </summary>
    public int NamespaceCount { get; init; }

    /// <summary>
    /// Gets the number of classes discovered.
    /// </summary>
    public int ClassCount { get; init; }

    /// <summary>
    /// Gets the number of interfaces discovered.
    /// </summary>
    public int InterfaceCount { get; init; }

    /// <summary>
    /// Gets the number of methods discovered.
    /// </summary>
    public int MethodCount { get; init; }

    /// <summary>
    /// Gets the number of properties discovered.
    /// </summary>
    public int PropertyCount { get; init; }

    /// <summary>
    /// Gets the number of dependencies discovered.
    /// </summary>
    public int DependencyCount { get; init; }

    /// <summary>
    /// Gets the elapsed analysis time.
    /// </summary>
    public TimeSpan ElapsedTime { get; init; }

    /// <summary>
    /// Gets a human-readable summary of the analysis result.
    /// </summary>
    public string? Message { get; init; }
}
