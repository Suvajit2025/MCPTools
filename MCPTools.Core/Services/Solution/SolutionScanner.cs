using System.Xml.Linq;
using MCPTools.Core.Interfaces;
using MCPTools.Core.Models.Solution;
using Microsoft.Extensions.Logging;

namespace MCPTools.Core.Services.Solution;

/// <summary>
/// Scans .NET solution files and builds a file-system-based solution model.
/// </summary>
public sealed class SolutionScanner : ISolutionScanner
{
    private static readonly HashSet<string> ExcludedFolders = new(StringComparer.OrdinalIgnoreCase)
    {
        ".git",
        ".vs",
        "bin",
        "node_modules",
        "obj",
        "packages"
    };

    private readonly ILogger<SolutionScanner> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SolutionScanner"/> class.
    /// </summary>
    /// <param name="logger">The logger used to record scanner activity.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="logger"/> is <see langword="null"/>.</exception>
    public SolutionScanner(ILogger<SolutionScanner> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<SolutionModel> ScanAsync(
        string solutionPath,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(solutionPath);

        var fullSolutionPath = System.IO.Path.GetFullPath(solutionPath);

        if (!File.Exists(fullSolutionPath))
        {
            throw new FileNotFoundException("The solution file could not be found.", fullSolutionPath);
        }

        _logger.LogInformation("Scanning solution {SolutionPath}.", fullSolutionPath);

        var solutionDirectory = System.IO.Path.GetDirectoryName(fullSolutionPath)
            ?? Directory.GetCurrentDirectory();
        var projectPaths = await ReadProjectPathsAsync(fullSolutionPath, solutionDirectory, cancellationToken);
        var projects = new List<ProjectModel>(projectPaths.Count);

        foreach (var projectPath in projectPaths)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!File.Exists(projectPath))
            {
                _logger.LogWarning("Project file {ProjectPath} referenced by solution {SolutionPath} was not found.", projectPath, fullSolutionPath);
                continue;
            }

            projects.Add(await ScanProjectAsync(projectPath, cancellationToken));
        }

        return new SolutionModel
        {
            Name = System.IO.Path.GetFileNameWithoutExtension(fullSolutionPath),
            Path = fullSolutionPath,
            Projects = projects
        };
    }

    private static async Task<IReadOnlyList<string>> ReadProjectPathsAsync(
        string solutionPath,
        string solutionDirectory,
        CancellationToken cancellationToken)
    {
        var extension = System.IO.Path.GetExtension(solutionPath);

        if (string.Equals(extension, ".slnx", StringComparison.OrdinalIgnoreCase))
        {
            return await ReadSlnxProjectPathsAsync(solutionPath, solutionDirectory, cancellationToken);
        }

        return await ReadSlnProjectPathsAsync(solutionPath, solutionDirectory, cancellationToken);
    }

    private static async Task<IReadOnlyList<string>> ReadSlnProjectPathsAsync(
        string solutionPath,
        string solutionDirectory,
        CancellationToken cancellationToken)
    {
        var lines = await File.ReadAllLinesAsync(solutionPath, cancellationToken);
        var projectPaths = new List<string>();

        foreach (var line in lines)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!line.TrimStart().StartsWith("Project(", StringComparison.Ordinal)
                || !line.Contains(".csproj", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var parts = line.Split(',', StringSplitOptions.TrimEntries);

            if (parts.Length < 2)
            {
                continue;
            }

            var projectPath = TrimQuotedValue(parts[1]);

            if (projectPath.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase))
            {
                projectPaths.Add(ResolvePath(solutionDirectory, projectPath));
            }
        }

        return projectPaths
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Order(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static async Task<IReadOnlyList<string>> ReadSlnxProjectPathsAsync(
        string solutionPath,
        string solutionDirectory,
        CancellationToken cancellationToken)
    {
        await using var stream = File.OpenRead(solutionPath);
        var document = await XDocument.LoadAsync(stream, LoadOptions.None, cancellationToken);

        return document
            .Descendants()
            .Where(element => string.Equals(element.Name.LocalName, "Project", StringComparison.OrdinalIgnoreCase))
            .Select(element => element.Attribute("Path")?.Value)
            .Where(path => !string.IsNullOrWhiteSpace(path) && path.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase))
            .Select(path => ResolvePath(solutionDirectory, path!))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Order(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private async Task<ProjectModel> ScanProjectAsync(
        string projectPath,
        CancellationToken cancellationToken)
    {
        var projectDirectory = System.IO.Path.GetDirectoryName(projectPath)
            ?? Directory.GetCurrentDirectory();
        var projectDocument = await LoadProjectDocumentAsync(projectPath, cancellationToken);
        var sourceFiles = EnumerateSourceFiles(projectDirectory)
            .Select(CreateSourceFileModel)
            .OrderBy(sourceFile => sourceFile.Path, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return new ProjectModel
        {
            Name = System.IO.Path.GetFileNameWithoutExtension(projectPath),
            Path = projectPath,
            TargetFramework = GetProjectProperty(projectDocument, "TargetFramework")
                ?? GetProjectProperty(projectDocument, "TargetFrameworks"),
            OutputType = GetProjectProperty(projectDocument, "OutputType") ?? "Library",
            Folders = EnumerateProjectFolders(projectDirectory)
                .Select(folderPath => CreateFolderModel(folderPath, cancellationToken))
                .OrderBy(folder => folder.Path, StringComparer.OrdinalIgnoreCase)
                .ToArray(),
            SourceFiles = sourceFiles,
            References = GetProjectReferences(projectDocument, projectDirectory)
        };
    }

    private static async Task<XDocument> LoadProjectDocumentAsync(
        string projectPath,
        CancellationToken cancellationToken)
    {
        await using var stream = File.OpenRead(projectPath);
        return await XDocument.LoadAsync(stream, LoadOptions.None, cancellationToken);
    }

    private static string? GetProjectProperty(XDocument document, string propertyName)
    {
        return document
            .Descendants()
            .FirstOrDefault(element => string.Equals(element.Name.LocalName, propertyName, StringComparison.OrdinalIgnoreCase))
            ?.Value
            .Trim();
    }

    private static IReadOnlyList<ProjectReferenceModel> GetProjectReferences(
        XDocument document,
        string projectDirectory)
    {
        return document
            .Descendants()
            .Where(element => string.Equals(element.Name.LocalName, "ProjectReference", StringComparison.OrdinalIgnoreCase))
            .Select(element => element.Attribute("Include")?.Value)
            .Where(path => !string.IsNullOrWhiteSpace(path))
            .Select(path => ResolvePath(projectDirectory, path!))
            .Select(path => new ProjectReferenceModel
            {
                Name = System.IO.Path.GetFileNameWithoutExtension(path),
                Path = path
            })
            .OrderBy(reference => reference.Path, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static IEnumerable<string> EnumerateProjectFolders(string projectDirectory)
    {
        return Directory
            .EnumerateDirectories(projectDirectory, "*", SearchOption.TopDirectoryOnly)
            .Where(ShouldIncludeDirectory);
    }

    private static IEnumerable<string> EnumerateSourceFiles(string projectDirectory)
    {
        return Directory
            .EnumerateFiles(projectDirectory, "*.cs", SearchOption.AllDirectories)
            .Where(path => !IsInExcludedDirectory(path, projectDirectory));
    }

    private static FolderModel CreateFolderModel(
        string folderPath,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return new FolderModel
        {
            Name = System.IO.Path.GetFileName(folderPath),
            Path = System.IO.Path.GetFullPath(folderPath),
            Children = Directory
                .EnumerateDirectories(folderPath, "*", SearchOption.TopDirectoryOnly)
                .Where(ShouldIncludeDirectory)
                .Select(path => CreateFolderModel(path, cancellationToken))
                .OrderBy(folder => folder.Path, StringComparer.OrdinalIgnoreCase)
                .ToArray(),
            Files = Directory
                .EnumerateFiles(folderPath, "*.cs", SearchOption.TopDirectoryOnly)
                .Select(CreateSourceFileModel)
                .OrderBy(file => file.Path, StringComparer.OrdinalIgnoreCase)
                .ToArray()
        };
    }

    private static SourceFileModel CreateSourceFileModel(string sourceFilePath)
    {
        return new SourceFileModel
        {
            Name = System.IO.Path.GetFileName(sourceFilePath),
            Path = System.IO.Path.GetFullPath(sourceFilePath),
            Extension = System.IO.Path.GetExtension(sourceFilePath),
            Namespace = null,
            Classes = []
        };
    }

    private static bool ShouldIncludeDirectory(string directoryPath)
    {
        return !ExcludedFolders.Contains(System.IO.Path.GetFileName(directoryPath));
    }

    private static bool IsInExcludedDirectory(string filePath, string projectDirectory)
    {
        var relativePath = System.IO.Path.GetRelativePath(projectDirectory, filePath);
        var segments = relativePath.Split(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar);

        return segments.Any(segment => ExcludedFolders.Contains(segment));
    }

    private static string ResolvePath(string baseDirectory, string path)
    {
        return System.IO.Path.GetFullPath(System.IO.Path.Combine(baseDirectory, path.Replace('\\', System.IO.Path.DirectorySeparatorChar)));
    }

    private static string TrimQuotedValue(string value)
    {
        return value.Trim().Trim('"');
    }
}
