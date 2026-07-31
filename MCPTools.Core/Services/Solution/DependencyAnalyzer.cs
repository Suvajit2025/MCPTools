using MCPTools.Core.Interfaces;
using MCPTools.Core.Models.Solution;
using MCPTools.Core.Models.Solution.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;

namespace MCPTools.Core.Services.Solution;

/// <summary>
/// Analyzes read-only source and project dependencies for .NET solutions.
/// </summary>
public sealed class DependencyAnalyzer : IDependencyAnalyzer
{
    private readonly ILogger<DependencyAnalyzer> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DependencyAnalyzer"/> class.
    /// </summary>
    /// <param name="logger">The logger used to record analyzer activity.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="logger"/> is <see langword="null"/>.</exception>
    public DependencyAnalyzer(ILogger<DependencyAnalyzer> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<DependencyModel>> AnalyzeSolutionAsync(
        SolutionModel solution,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(solution);

        _logger.LogInformation("Analyzing dependencies for solution {SolutionName}.", solution.Name);

        var dependencies = new List<DependencyModel>();
        var knownTypes = CreateKnownTypeSet(solution);

        foreach (var project in solution.Projects)
        {
            cancellationToken.ThrowIfCancellationRequested();

            dependencies.AddRange(CreateProjectReferenceDependencies(project));
            dependencies.AddRange(await AnalyzeProjectSourceFilesAsync(project, knownTypes, cancellationToken));
        }

        return DistinctDependencies(dependencies);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<DependencyModel>> AnalyzeProjectAsync(
        ProjectModel project,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(project);

        _logger.LogInformation("Analyzing dependencies for project {ProjectName}.", project.Name);

        var knownTypes = CreateKnownTypeSet(project);
        var dependencies = new List<DependencyModel>();

        dependencies.AddRange(CreateProjectReferenceDependencies(project));
        dependencies.AddRange(await AnalyzeProjectSourceFilesAsync(project, knownTypes, cancellationToken));

        return DistinctDependencies(dependencies);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<DependencyModel>> AnalyzeFileAsync(
        string sourceFilePath,
        string? projectName = null,
        IReadOnlySet<string>? knownSolutionTypes = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceFilePath);

        var fullPath = Path.GetFullPath(sourceFilePath);

        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException("The C# source file could not be found.", fullPath);
        }

        _logger.LogDebug("Analyzing dependencies for source file {SourceFilePath}.", fullPath);

        var sourceText = await File.ReadAllTextAsync(fullPath, cancellationToken);
        var syntaxTree = CSharpSyntaxTree.ParseText(sourceText, cancellationToken: cancellationToken);
        var root = await syntaxTree.GetRootAsync(cancellationToken);

        return DistinctDependencies(AnalyzeRoot(root, fullPath, projectName, knownSolutionTypes));
    }

    private async Task<IReadOnlyList<DependencyModel>> AnalyzeProjectSourceFilesAsync(
        ProjectModel project,
        IReadOnlySet<string> knownTypes,
        CancellationToken cancellationToken)
    {
        var dependencies = new List<DependencyModel>();

        foreach (var sourceFile in project.SourceFiles)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(sourceFile.Path) || !File.Exists(sourceFile.Path))
            {
                continue;
            }

            dependencies.AddRange(await AnalyzeFileAsync(
                sourceFile.Path,
                project.Name,
                knownTypes,
                cancellationToken));
        }

        return dependencies;
    }

    private static IReadOnlyList<DependencyModel> AnalyzeRoot(
        SyntaxNode root,
        string sourcePath,
        string? projectName,
        IReadOnlySet<string>? knownTypes)
    {
        var dependencies = new List<DependencyModel>();

        dependencies.AddRange(CreateNamespaceDependencies(root, sourcePath, projectName, knownTypes));

        foreach (var declaration in root.DescendantNodes().OfType<TypeDeclarationSyntax>())
        {
            dependencies.AddRange(CreateTypeDependencies(declaration, sourcePath, projectName, knownTypes));
        }

        foreach (var enumDeclaration in root.DescendantNodes().OfType<EnumDeclarationSyntax>())
        {
            dependencies.AddRange(CreateAttributeDependencies(
                enumDeclaration.AttributeLists,
                enumDeclaration.Identifier.ValueText,
                GetContainingNamespaceName(enumDeclaration),
                sourcePath,
                projectName,
                knownTypes));
        }

        return dependencies;
    }

    private static IEnumerable<DependencyModel> CreateProjectReferenceDependencies(ProjectModel project)
    {
        return project.References.Select(reference => new DependencyModel
        {
            ProjectName = project.Name,
            SourceName = project.Name,
            SourcePath = project.Path,
            Name = reference.Name,
            Namespace = null,
            Type = "Project",
            Relationship = "ProjectReference",
            IsExternal = false
        });
    }

    private static IEnumerable<DependencyModel> CreateNamespaceDependencies(
        SyntaxNode root,
        string sourcePath,
        string? projectName,
        IReadOnlySet<string>? knownTypes)
    {
        return root
            .DescendantNodes()
            .OfType<UsingDirectiveSyntax>()
            .Where(usingDirective => usingDirective.Name is not null)
            .Select(usingDirective => CreateDependency(
                sourceName: Path.GetFileName(sourcePath),
                sourceNamespace: null,
                dependencyName: usingDirective.Name!.ToString(),
                dependencyNamespace: usingDirective.Name.ToString(),
                dependencyType: "Namespace",
                relationship: "NamespaceReference",
                sourcePath,
                projectName,
                knownTypes));
    }

    private static IEnumerable<DependencyModel> CreateTypeDependencies(
        TypeDeclarationSyntax declaration,
        string sourcePath,
        string? projectName,
        IReadOnlySet<string>? knownTypes)
    {
        var sourceName = declaration.Identifier.ValueText;
        var sourceNamespace = GetContainingNamespaceName(declaration);
        var dependencies = new List<DependencyModel>();

        dependencies.AddRange(CreateBaseTypeDependencies(declaration, sourceName, sourceNamespace, sourcePath, projectName, knownTypes));
        dependencies.AddRange(CreateAttributeDependencies(declaration.AttributeLists, sourceName, sourceNamespace, sourcePath, projectName, knownTypes));
        dependencies.AddRange(CreateConstructorInjectionDependencies(declaration, sourceName, sourceNamespace, sourcePath, projectName, knownTypes));
        dependencies.AddRange(CreateMemberTypeDependencies(declaration, sourceName, sourceNamespace, sourcePath, projectName, knownTypes));
        dependencies.AddRange(CreateMethodCallDependencies(declaration, sourceName, sourceNamespace, sourcePath, projectName, knownTypes));
        dependencies.AddRange(CreateClassReferenceDependencies(declaration, sourceName, sourceNamespace, sourcePath, projectName, knownTypes));

        return dependencies;
    }

    private static IEnumerable<DependencyModel> CreateBaseTypeDependencies(
        TypeDeclarationSyntax declaration,
        string sourceName,
        string? sourceNamespace,
        string sourcePath,
        string? projectName,
        IReadOnlySet<string>? knownTypes)
    {
        if (declaration.BaseList is null)
        {
            return [];
        }

        return declaration.BaseList.Types.Select((baseType, index) =>
        {
            var relationship = declaration switch
            {
                InterfaceDeclarationSyntax => "InterfaceInheritance",
                ClassDeclarationSyntax => index == 0 && !LooksLikeInterface(baseType.Type.ToString())
                    ? "Inheritance"
                    : "InterfaceImplementation",
                RecordDeclarationSyntax => index == 0 && !LooksLikeInterface(baseType.Type.ToString())
                    ? "Inheritance"
                    : "InterfaceImplementation",
                _ => "BaseType"
            };

            return CreateDependency(
                sourceName,
                sourceNamespace,
                baseType.Type.ToString(),
                null,
                "Type",
                relationship,
                sourcePath,
                projectName,
                knownTypes);
        });
    }

    private static IEnumerable<DependencyModel> CreateConstructorInjectionDependencies(
        TypeDeclarationSyntax declaration,
        string sourceName,
        string? sourceNamespace,
        string sourcePath,
        string? projectName,
        IReadOnlySet<string>? knownTypes)
    {
        return declaration
            .Members
            .OfType<ConstructorDeclarationSyntax>()
            .SelectMany(constructor => constructor.ParameterList.Parameters)
            .Where(parameter => parameter.Type is not null)
            .Select(parameter => CreateDependency(
                sourceName,
                sourceNamespace,
                parameter.Type!.ToString(),
                null,
                "Type",
                "ConstructorInjection",
                sourcePath,
                projectName,
                knownTypes));
    }

    private static IEnumerable<DependencyModel> CreateMemberTypeDependencies(
        TypeDeclarationSyntax declaration,
        string sourceName,
        string? sourceNamespace,
        string sourcePath,
        string? projectName,
        IReadOnlySet<string>? knownTypes)
    {
        var dependencies = new List<DependencyModel>();

        dependencies.AddRange(declaration
            .Members
            .OfType<FieldDeclarationSyntax>()
            .Select(field => field.Declaration.Type.ToString())
            .Select(typeName => CreateDependency(sourceName, sourceNamespace, typeName, null, "Type", "FieldReference", sourcePath, projectName, knownTypes)));

        dependencies.AddRange(declaration
            .Members
            .OfType<PropertyDeclarationSyntax>()
            .Select(property => property.Type.ToString())
            .Select(typeName => CreateDependency(sourceName, sourceNamespace, typeName, null, "Type", "PropertyReference", sourcePath, projectName, knownTypes)));

        dependencies.AddRange(declaration
            .Members
            .OfType<MethodDeclarationSyntax>()
            .SelectMany(method => GetMethodTypeReferences(method))
            .Select(typeName => CreateDependency(sourceName, sourceNamespace, typeName, null, "Type", "MethodSignatureReference", sourcePath, projectName, knownTypes)));

        return dependencies;
    }

    private static IEnumerable<string> GetMethodTypeReferences(MethodDeclarationSyntax method)
    {
        yield return method.ReturnType.ToString();

        foreach (var parameter in method.ParameterList.Parameters.Where(parameter => parameter.Type is not null))
        {
            yield return parameter.Type!.ToString();
        }
    }

    private static IEnumerable<DependencyModel> CreateMethodCallDependencies(
        TypeDeclarationSyntax declaration,
        string sourceName,
        string? sourceNamespace,
        string sourcePath,
        string? projectName,
        IReadOnlySet<string>? knownTypes)
    {
        return declaration
            .DescendantNodes()
            .OfType<InvocationExpressionSyntax>()
            .Select(invocation => GetInvocationName(invocation.Expression))
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Select(name => CreateDependency(sourceName, sourceNamespace, name!, null, "Method", "MethodCall", sourcePath, projectName, knownTypes));
    }

    private static IEnumerable<DependencyModel> CreateClassReferenceDependencies(
        TypeDeclarationSyntax declaration,
        string sourceName,
        string? sourceNamespace,
        string sourcePath,
        string? projectName,
        IReadOnlySet<string>? knownTypes)
    {
        return declaration
            .DescendantNodes()
            .OfType<ObjectCreationExpressionSyntax>()
            .Select(objectCreation => objectCreation.Type.ToString())
            .Concat(declaration.DescendantNodes()
                .OfType<LocalDeclarationStatementSyntax>()
                .Select(localDeclaration => localDeclaration.Declaration.Type.ToString()))
            .Where(typeName => !string.Equals(typeName, "var", StringComparison.Ordinal))
            .Select(typeName => CreateDependency(sourceName, sourceNamespace, typeName, null, "Type", "ClassReference", sourcePath, projectName, knownTypes));
    }

    private static IEnumerable<DependencyModel> CreateAttributeDependencies(
        SyntaxList<AttributeListSyntax> attributeLists,
        string sourceName,
        string? sourceNamespace,
        string sourcePath,
        string? projectName,
        IReadOnlySet<string>? knownTypes)
    {
        return attributeLists
            .SelectMany(attributeList => attributeList.Attributes)
            .Select(attribute => CreateDependency(
                sourceName,
                sourceNamespace,
                attribute.Name.ToString(),
                null,
                "Attribute",
                "AttributeReference",
                sourcePath,
                projectName,
                knownTypes));
    }

    private static DependencyModel CreateDependency(
        string? sourceName,
        string? sourceNamespace,
        string dependencyName,
        string? dependencyNamespace,
        string dependencyType,
        string relationship,
        string? sourcePath,
        string? projectName,
        IReadOnlySet<string>? knownTypes)
    {
        var normalizedName = NormalizeTypeName(dependencyName);

        return new DependencyModel
        {
            ProjectName = projectName,
            SourcePath = sourcePath,
            SourceName = sourceName,
            SourceNamespace = sourceNamespace,
            Name = normalizedName,
            Namespace = dependencyNamespace,
            Type = dependencyType,
            Relationship = relationship,
            IsExternal = knownTypes is not null && !knownTypes.Contains(normalizedName)
        };
    }

    private static IReadOnlyList<DependencyModel> DistinctDependencies(IEnumerable<DependencyModel> dependencies)
    {
        return dependencies
            .Where(dependency => !string.IsNullOrWhiteSpace(dependency.Name))
            .GroupBy(
                dependency => new
                {
                    dependency.ProjectName,
                    dependency.SourcePath,
                    dependency.SourceName,
                    dependency.Name,
                    dependency.Type,
                    dependency.Relationship
                })
            .Select(group => group.First())
            .OrderBy(dependency => dependency.ProjectName, StringComparer.OrdinalIgnoreCase)
            .ThenBy(dependency => dependency.SourceName, StringComparer.OrdinalIgnoreCase)
            .ThenBy(dependency => dependency.Relationship, StringComparer.OrdinalIgnoreCase)
            .ThenBy(dependency => dependency.Name, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static IReadOnlySet<string> CreateKnownTypeSet(SolutionModel solution)
    {
        return solution
            .Projects
            .SelectMany(GetKnownTypeNames)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    private static IReadOnlySet<string> CreateKnownTypeSet(ProjectModel project)
    {
        return GetKnownTypeNames(project).ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    private static IEnumerable<string> GetKnownTypeNames(ProjectModel project)
    {
        return project.SourceFiles
            .SelectMany(sourceFile => sourceFile.Namespaces)
            .SelectMany(namespaceModel => namespaceModel.Classes.Select(type => type.Name)
                .Concat(namespaceModel.Interfaces.Select(type => type.Name))
                .Concat(namespaceModel.Records.Select(type => type.Name))
                .Concat(namespaceModel.Enums.Select(type => type.Name)))
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Select(name => name!);
    }

    private static string? GetContainingNamespaceName(SyntaxNode node)
    {
        return node
            .Ancestors()
            .OfType<BaseNamespaceDeclarationSyntax>()
            .FirstOrDefault()
            ?.Name
            .ToString();
    }

    private static string? GetInvocationName(ExpressionSyntax expression)
    {
        return expression switch
        {
            MemberAccessExpressionSyntax memberAccess => memberAccess.Name.Identifier.ValueText,
            IdentifierNameSyntax identifierName => identifierName.Identifier.ValueText,
            GenericNameSyntax genericName => genericName.Identifier.ValueText,
            _ => expression.ToString()
        };
    }

    private static bool LooksLikeInterface(string typeName)
    {
        var normalizedName = NormalizeTypeName(typeName);
        return normalizedName.Length > 1
            && normalizedName[0] == 'I'
            && char.IsUpper(normalizedName[1]);
    }

    private static string NormalizeTypeName(string typeName)
    {
        var trimmed = typeName.Trim();
        var nullableRemoved = trimmed.TrimEnd('?');
        var genericIndex = nullableRemoved.IndexOf('<', StringComparison.Ordinal);

        if (genericIndex >= 0)
        {
            nullableRemoved = nullableRemoved[..genericIndex];
        }

        var namespaceIndex = nullableRemoved.LastIndexOf('.');

        return namespaceIndex >= 0
            ? nullableRemoved[(namespaceIndex + 1)..]
            : nullableRemoved;
    }
}
