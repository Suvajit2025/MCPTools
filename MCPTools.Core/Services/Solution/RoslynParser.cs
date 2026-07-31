using MCPTools.Core.Interfaces;
using MCPTools.Core.Models.Solution;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using SolutionClassModel = MCPTools.Core.Models.Solution.ClassModel;
using SyntaxAttributeModel = MCPTools.Core.Models.Solution.Syntax.AttributeModel;
using SyntaxBaseTypeModel = MCPTools.Core.Models.Solution.Syntax.BaseTypeModel;
using SyntaxClassModel = MCPTools.Core.Models.Solution.Syntax.ClassModel;
using SyntaxConstructorModel = MCPTools.Core.Models.Solution.Syntax.ConstructorModel;
using SyntaxEnumModel = MCPTools.Core.Models.Solution.Syntax.EnumModel;
using SyntaxFieldModel = MCPTools.Core.Models.Solution.Syntax.FieldModel;
using SyntaxInterfaceModel = MCPTools.Core.Models.Solution.Syntax.InterfaceModel;
using SyntaxMethodModel = MCPTools.Core.Models.Solution.Syntax.MethodModel;
using SyntaxNamespaceModel = MCPTools.Core.Models.Solution.Syntax.NamespaceModel;
using SyntaxParameterModel = MCPTools.Core.Models.Solution.Syntax.ParameterModel;
using SyntaxPropertyModel = MCPTools.Core.Models.Solution.Syntax.PropertyModel;
using SyntaxRecordModel = MCPTools.Core.Models.Solution.Syntax.RecordModel;

namespace MCPTools.Core.Services.Solution;

/// <summary>
/// Parses C# source files using Roslyn and builds read-only source structure models.
/// </summary>
public sealed class RoslynParser : IRoslynParser
{
    private readonly ILogger<RoslynParser> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="RoslynParser"/> class.
    /// </summary>
    /// <param name="logger">The logger used to record parser activity.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="logger"/> is <see langword="null"/>.</exception>
    public RoslynParser(ILogger<RoslynParser> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<SourceFileModel> ParseFileAsync(
        string sourceFilePath,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceFilePath);

        var fullPath = Path.GetFullPath(sourceFilePath);

        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException("The C# source file could not be found.", fullPath);
        }

        _logger.LogDebug("Parsing C# source file {SourceFilePath}.", fullPath);

        var sourceText = await File.ReadAllTextAsync(fullPath, cancellationToken);
        var syntaxTree = CSharpSyntaxTree.ParseText(
            sourceText,
            cancellationToken: cancellationToken);
        var root = await syntaxTree.GetRootAsync(cancellationToken);
        var namespaces = CreateNamespaceModels(root);
        var firstNamespace = namespaces.FirstOrDefault(namespaceModel => !string.IsNullOrWhiteSpace(namespaceModel.Name));

        return new SourceFileModel
        {
            Name = Path.GetFileName(fullPath),
            Path = fullPath,
            Extension = Path.GetExtension(fullPath),
            Namespace = firstNamespace?.Name,
            Classes = root
                .DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .Select(classDeclaration => new SolutionClassModel
                {
                    Name = classDeclaration.Identifier.ValueText,
                    Namespace = GetContainingNamespaceName(classDeclaration)
                })
                .OrderBy(classModel => classModel.Name, StringComparer.OrdinalIgnoreCase)
                .ToArray(),
            Namespaces = namespaces
        };
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<SourceFileModel>> ParseFilesAsync(
        IEnumerable<string> sourceFilePaths,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(sourceFilePaths);

        var sourceFiles = new List<SourceFileModel>();

        foreach (var sourceFilePath in sourceFilePaths)
        {
            cancellationToken.ThrowIfCancellationRequested();
            sourceFiles.Add(await ParseFileAsync(sourceFilePath, cancellationToken));
        }

        return sourceFiles;
    }

    /// <inheritdoc />
    public async Task<SolutionModel> ParseSolutionAsync(
        SolutionModel solution,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(solution);

        var projects = new List<ProjectModel>();

        foreach (var project in solution.Projects)
        {
            cancellationToken.ThrowIfCancellationRequested();
            projects.Add(await ParseProjectAsync(project, cancellationToken));
        }

        return new SolutionModel
        {
            Name = solution.Name,
            Path = solution.Path,
            Projects = projects
        };
    }

    private async Task<ProjectModel> ParseProjectAsync(
        ProjectModel project,
        CancellationToken cancellationToken)
    {
        var parsedSourceFiles = await ParseFilesAsync(
            project.SourceFiles
                .Select(sourceFile => sourceFile.Path)
                .Where(path => !string.IsNullOrWhiteSpace(path))
                .Select(path => path!),
            cancellationToken);
        var sourceFileLookup = parsedSourceFiles
            .Where(sourceFile => !string.IsNullOrWhiteSpace(sourceFile.Path))
            .ToDictionary(sourceFile => sourceFile.Path!, StringComparer.OrdinalIgnoreCase);

        return new ProjectModel
        {
            Name = project.Name,
            Path = project.Path,
            TargetFramework = project.TargetFramework,
            OutputType = project.OutputType,
            SourceFiles = parsedSourceFiles,
            References = project.References,
            Folders = project.Folders
                .Select(folder => ParseFolder(folder, sourceFileLookup))
                .ToArray()
        };
    }

    private static FolderModel ParseFolder(
        FolderModel folder,
        IReadOnlyDictionary<string, SourceFileModel> sourceFiles)
    {
        return new FolderModel
        {
            Name = folder.Name,
            Path = folder.Path,
            Children = folder.Children
                .Select(child => ParseFolder(child, sourceFiles))
                .ToArray(),
            Files = folder.Files
                .Select(file => !string.IsNullOrWhiteSpace(file.Path) && sourceFiles.TryGetValue(file.Path, out var parsedFile)
                    ? parsedFile
                    : file)
                .ToArray()
        };
    }

    private static IReadOnlyList<SyntaxNamespaceModel> CreateNamespaceModels(SyntaxNode root)
    {
        var namespaces = root
            .DescendantNodes()
            .OfType<BaseNamespaceDeclarationSyntax>()
            .Select(CreateNamespaceModel)
            .ToList();
        var globalTypes = GetTypeDeclarations(root)
            .Where(declaration => GetContainingNamespace(declaration) is null)
            .ToArray();

        if (globalTypes.Length > 0)
        {
            namespaces.Add(CreateNamespaceModel(null, globalTypes));
        }

        return namespaces
            .OrderBy(namespaceModel => namespaceModel.Name, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static SyntaxNamespaceModel CreateNamespaceModel(BaseNamespaceDeclarationSyntax namespaceDeclaration)
    {
        var typeDeclarations = GetTypeDeclarations(namespaceDeclaration)
            .Where(declaration => GetContainingNamespace(declaration) == namespaceDeclaration)
            .ToArray();

        return CreateNamespaceModel(namespaceDeclaration.Name.ToString(), typeDeclarations);
    }

    private static SyntaxNamespaceModel CreateNamespaceModel(
        string? namespaceName,
        IReadOnlyList<MemberDeclarationSyntax> typeDeclarations)
    {
        return new SyntaxNamespaceModel
        {
            Name = namespaceName,
            Classes = typeDeclarations
                .OfType<ClassDeclarationSyntax>()
                .Select(CreateClassModel)
                .OrderBy(classModel => classModel.Name, StringComparer.OrdinalIgnoreCase)
                .ToArray(),
            Interfaces = typeDeclarations
                .OfType<InterfaceDeclarationSyntax>()
                .Select(CreateInterfaceModel)
                .OrderBy(interfaceModel => interfaceModel.Name, StringComparer.OrdinalIgnoreCase)
                .ToArray(),
            Records = typeDeclarations
                .OfType<RecordDeclarationSyntax>()
                .Select(CreateRecordModel)
                .OrderBy(recordModel => recordModel.Name, StringComparer.OrdinalIgnoreCase)
                .ToArray(),
            Enums = typeDeclarations
                .OfType<EnumDeclarationSyntax>()
                .Select(CreateEnumModel)
                .OrderBy(enumModel => enumModel.Name, StringComparer.OrdinalIgnoreCase)
                .ToArray()
        };
    }

    private static SyntaxClassModel CreateClassModel(ClassDeclarationSyntax declaration)
    {
        return new SyntaxClassModel
        {
            Name = declaration.Identifier.ValueText,
            Namespace = GetContainingNamespaceName(declaration),
            Accessibility = GetAccessibility(declaration.Modifiers),
            IsAbstract = HasModifier(declaration.Modifiers, SyntaxKind.AbstractKeyword),
            IsSealed = HasModifier(declaration.Modifiers, SyntaxKind.SealedKeyword),
            BaseTypes = CreateBaseTypeModels(declaration.BaseList, markFirstAsInterface: false),
            Constructors = declaration.Members
                .OfType<ConstructorDeclarationSyntax>()
                .Select(CreateConstructorModel)
                .ToArray(),
            Methods = declaration.Members
                .OfType<MethodDeclarationSyntax>()
                .Select(CreateMethodModel)
                .ToArray(),
            Properties = declaration.Members
                .OfType<PropertyDeclarationSyntax>()
                .Select(CreatePropertyModel)
                .ToArray(),
            Fields = declaration.Members
                .OfType<FieldDeclarationSyntax>()
                .SelectMany(CreateFieldModels)
                .ToArray(),
            Attributes = CreateAttributeModels(declaration.AttributeLists)
        };
    }

    private static SyntaxInterfaceModel CreateInterfaceModel(InterfaceDeclarationSyntax declaration)
    {
        return new SyntaxInterfaceModel
        {
            Name = declaration.Identifier.ValueText,
            Namespace = GetContainingNamespaceName(declaration),
            Accessibility = GetAccessibility(declaration.Modifiers),
            BaseTypes = CreateBaseTypeModels(declaration.BaseList, markFirstAsInterface: true),
            Methods = declaration.Members
                .OfType<MethodDeclarationSyntax>()
                .Select(CreateMethodModel)
                .ToArray(),
            Properties = declaration.Members
                .OfType<PropertyDeclarationSyntax>()
                .Select(CreatePropertyModel)
                .ToArray(),
            Attributes = CreateAttributeModels(declaration.AttributeLists)
        };
    }

    private static SyntaxRecordModel CreateRecordModel(RecordDeclarationSyntax declaration)
    {
        return new SyntaxRecordModel
        {
            Name = declaration.Identifier.ValueText,
            Namespace = GetContainingNamespaceName(declaration),
            Accessibility = GetAccessibility(declaration.Modifiers),
            IsStruct = declaration.ClassOrStructKeyword.IsKind(SyntaxKind.StructKeyword),
            BaseTypes = CreateBaseTypeModels(declaration.BaseList, markFirstAsInterface: false),
            Parameters = declaration.ParameterList is null
                ? []
                : CreateParameterModels(declaration.ParameterList.Parameters),
            Properties = declaration.Members
                .OfType<PropertyDeclarationSyntax>()
                .Select(CreatePropertyModel)
                .ToArray(),
            Methods = declaration.Members
                .OfType<MethodDeclarationSyntax>()
                .Select(CreateMethodModel)
                .ToArray(),
            Attributes = CreateAttributeModels(declaration.AttributeLists)
        };
    }

    private static SyntaxEnumModel CreateEnumModel(EnumDeclarationSyntax declaration)
    {
        return new SyntaxEnumModel
        {
            Name = declaration.Identifier.ValueText,
            Namespace = GetContainingNamespaceName(declaration),
            Accessibility = GetAccessibility(declaration.Modifiers),
            UnderlyingType = declaration.BaseList?.Types.FirstOrDefault()?.Type.ToString(),
            Members = declaration.Members
                .Select(member => member.Identifier.ValueText)
                .ToArray(),
            Attributes = CreateAttributeModels(declaration.AttributeLists)
        };
    }

    private static SyntaxConstructorModel CreateConstructorModel(ConstructorDeclarationSyntax declaration)
    {
        return new SyntaxConstructorModel
        {
            Name = declaration.Identifier.ValueText,
            Accessibility = GetAccessibility(declaration.Modifiers),
            IsStatic = HasModifier(declaration.Modifiers, SyntaxKind.StaticKeyword),
            Parameters = CreateParameterModels(declaration.ParameterList.Parameters),
            Attributes = CreateAttributeModels(declaration.AttributeLists)
        };
    }

    private static SyntaxMethodModel CreateMethodModel(MethodDeclarationSyntax declaration)
    {
        return new SyntaxMethodModel
        {
            Name = declaration.Identifier.ValueText,
            ReturnType = declaration.ReturnType.ToString(),
            Accessibility = GetAccessibility(declaration.Modifiers),
            IsAsync = HasModifier(declaration.Modifiers, SyntaxKind.AsyncKeyword),
            IsStatic = HasModifier(declaration.Modifiers, SyntaxKind.StaticKeyword),
            Parameters = CreateParameterModels(declaration.ParameterList.Parameters),
            Attributes = CreateAttributeModels(declaration.AttributeLists)
        };
    }

    private static SyntaxPropertyModel CreatePropertyModel(PropertyDeclarationSyntax declaration)
    {
        return new SyntaxPropertyModel
        {
            Name = declaration.Identifier.ValueText,
            Type = declaration.Type.ToString(),
            Accessibility = GetAccessibility(declaration.Modifiers),
            HasGetter = declaration.ExpressionBody is not null
                || declaration.AccessorList?.Accessors.Any(accessor => accessor.IsKind(SyntaxKind.GetAccessorDeclaration)) == true,
            HasSetter = declaration.AccessorList?.Accessors.Any(accessor =>
                accessor.IsKind(SyntaxKind.SetAccessorDeclaration)
                || accessor.IsKind(SyntaxKind.InitAccessorDeclaration)) == true,
            IsRequired = HasModifier(declaration.Modifiers, SyntaxKind.RequiredKeyword),
            Attributes = CreateAttributeModels(declaration.AttributeLists)
        };
    }

    private static IReadOnlyList<SyntaxFieldModel> CreateFieldModels(FieldDeclarationSyntax declaration)
    {
        return declaration.Declaration.Variables
            .Select(variable => new SyntaxFieldModel
            {
                Name = variable.Identifier.ValueText,
                Type = declaration.Declaration.Type.ToString(),
                Accessibility = GetAccessibility(declaration.Modifiers),
                IsReadOnly = HasModifier(declaration.Modifiers, SyntaxKind.ReadOnlyKeyword),
                IsStatic = HasModifier(declaration.Modifiers, SyntaxKind.StaticKeyword),
                Attributes = CreateAttributeModels(declaration.AttributeLists)
            })
            .ToArray();
    }

    private static IReadOnlyList<SyntaxParameterModel> CreateParameterModels(
        SeparatedSyntaxList<ParameterSyntax> parameters)
    {
        return parameters
            .Select(parameter => new SyntaxParameterModel
            {
                Name = parameter.Identifier.ValueText,
                Type = parameter.Type?.ToString(),
                DefaultValue = parameter.Default?.Value.ToString(),
                IsOptional = parameter.Default is not null,
                IsParams = HasModifier(parameter.Modifiers, SyntaxKind.ParamsKeyword),
                Attributes = CreateAttributeModels(parameter.AttributeLists)
            })
            .ToArray();
    }

    private static IReadOnlyList<SyntaxAttributeModel> CreateAttributeModels(
        SyntaxList<AttributeListSyntax> attributeLists)
    {
        return attributeLists
            .SelectMany(attributeList => attributeList.Attributes)
            .Select(attribute => new SyntaxAttributeModel
            {
                Name = attribute.Name.ToString(),
                Arguments = attribute.ArgumentList?.Arguments
                    .Select(argument => argument.ToString())
                    .ToArray() ?? []
            })
            .ToArray();
    }

    private static IReadOnlyList<SyntaxBaseTypeModel> CreateBaseTypeModels(
        BaseListSyntax? baseList,
        bool markFirstAsInterface)
    {
        if (baseList is null)
        {
            return [];
        }

        return baseList.Types
            .Select((baseType, index) => new SyntaxBaseTypeModel
            {
                Name = baseType.Type.ToString(),
                FullName = baseType.Type.ToString(),
                IsInterface = markFirstAsInterface || index > 0
            })
            .ToArray();
    }

    private static IEnumerable<MemberDeclarationSyntax> GetTypeDeclarations(SyntaxNode node)
    {
        return node
            .DescendantNodes()
            .OfType<MemberDeclarationSyntax>()
            .Where(member => member is ClassDeclarationSyntax
                or InterfaceDeclarationSyntax
                or RecordDeclarationSyntax
                or EnumDeclarationSyntax);
    }

    private static BaseNamespaceDeclarationSyntax? GetContainingNamespace(SyntaxNode node)
    {
        return node
            .Ancestors()
            .OfType<BaseNamespaceDeclarationSyntax>()
            .FirstOrDefault();
    }

    private static string? GetContainingNamespaceName(SyntaxNode node)
    {
        return GetContainingNamespace(node)?.Name.ToString();
    }

    private static string? GetAccessibility(SyntaxTokenList modifiers)
    {
        if (HasModifier(modifiers, SyntaxKind.PublicKeyword))
        {
            return "public";
        }

        if (HasModifier(modifiers, SyntaxKind.PrivateKeyword))
        {
            return "private";
        }

        if (HasModifier(modifiers, SyntaxKind.ProtectedKeyword) && HasModifier(modifiers, SyntaxKind.InternalKeyword))
        {
            return "protected internal";
        }

        if (HasModifier(modifiers, SyntaxKind.ProtectedKeyword))
        {
            return "protected";
        }

        if (HasModifier(modifiers, SyntaxKind.InternalKeyword))
        {
            return "internal";
        }

        return null;
    }

    private static bool HasModifier(SyntaxTokenList modifiers, SyntaxKind kind)
    {
        return modifiers.Any(modifier => modifier.IsKind(kind));
    }
}
