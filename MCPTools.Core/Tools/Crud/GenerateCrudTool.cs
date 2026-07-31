using System.Diagnostics;
using System.Text;
using MCPTools.Core.Constants;
using MCPTools.Core.Exceptions;
using MCPTools.Core.Models.Generation;
using MCPTools.Core.Models.Tools;
using MCPTools.Core.Services;
using MCPTools.Core.TemplateEngine;

namespace MCPTools.Core.Tools.Crud;

/// <summary>
/// Orchestrates CRUD source generation for a single entity.
/// </summary>
public sealed class GenerateCrudTool : ToolBase<GenerateCrudRequest, GenerateCrudResponse>
{
    private readonly ITemplateEngine _templateEngine;
    private readonly PlaceholderBuilder _placeholderBuilder;
    private readonly FileGenerator _fileGenerator;
    private readonly TemplateDiscoveryService _templateDiscoveryService;
    private readonly NamingConventionService _namingConventionService;

    /// <summary>
    /// Initializes a new instance of the <see cref="GenerateCrudTool"/> class.
    /// </summary>
    /// <param name="templateEngine">The template engine used to render templates.</param>
    /// <param name="placeholderBuilder">The placeholder builder used to create template values.</param>
    /// <param name="fileGenerator">The file generator used to write rendered artifacts.</param>
    /// <param name="templateDiscoveryService">The template discovery service used to locate templates.</param>
    /// <param name="namingConventionService">The naming service used to create standard artifact names.</param>
    public GenerateCrudTool(
        ITemplateEngine templateEngine,
        PlaceholderBuilder placeholderBuilder,
        FileGenerator fileGenerator,
        TemplateDiscoveryService templateDiscoveryService,
        NamingConventionService namingConventionService)
        : base(new ToolMetadata
        {
            Name = "generate-crud",
            DisplayName = "Generate CRUD",
            Category = "Generation",
            Version = "1.0.0",
            Description = "Generates CRUD source code for a database entity."
        })
    {
        _templateEngine = templateEngine ?? throw new ArgumentNullException(nameof(templateEngine));
        _placeholderBuilder = placeholderBuilder ?? throw new ArgumentNullException(nameof(placeholderBuilder));
        _fileGenerator = fileGenerator ?? throw new ArgumentNullException(nameof(fileGenerator));
        _templateDiscoveryService = templateDiscoveryService ?? throw new ArgumentNullException(nameof(templateDiscoveryService));
        _namingConventionService = namingConventionService ?? throw new ArgumentNullException(nameof(namingConventionService));
    }

    /// <inheritdoc />
    public override async Task<GenerateCrudResponse> ExecuteAsync(
        GenerateCrudRequest request,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            ValidateRequest(request);

            var entity = CreateEntityDefinition(request);
            var placeholders = BuildPlaceholders(request, entity);
            var result = await GenerateAllTemplatesAsync(request, entity, placeholders, cancellationToken);

            stopwatch.Stop();
            return CreateResponse(result, stopwatch.Elapsed);
        }
        catch (ToolValidationException)
        {
            throw;
        }
        catch (MCPToolsException exception)
        {
            throw new ToolExecutionException(Metadata.Name, exception, includeToolName: true);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            throw new ToolExecutionException(Metadata.Name, exception, includeToolName: true);
        }
    }

    private static void ValidateRequest(GenerateCrudRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.EntityName))
        {
            throw new ToolValidationException("EntityName is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Namespace))
        {
            throw new ToolValidationException("Namespace is required.");
        }

        if (string.IsNullOrWhiteSpace(request.OutputDirectory))
        {
            throw new ToolValidationException("OutputDirectory is required.");
        }
    }

    private EntityDefinition CreateEntityDefinition(GenerateCrudRequest request)
    {
        var entityName = _namingConventionService.GetEntityName(request.EntityName);

        return new EntityDefinition
        {
            Namespace = request.Namespace,
            EntityName = entityName,
            PluralEntityName = string.IsNullOrWhiteSpace(request.PluralEntityName)
                ? $"{entityName}s"
                : request.PluralEntityName,
            TableName = request.TableName,
            PrimaryKey = request.PrimaryKey,
            PrimaryKeyType = request.PrimaryKeyType,
            Author = request.Author,
            CompanyName = request.CompanyName,
            GenerateRepository = request.GenerateRepository,
            GenerateService = request.GenerateService,
            GenerateController = request.GenerateController,
            GenerateDto = request.GenerateDto,
            GenerateMapping = true,
            GenerateSql = true
        };
    }

    private IReadOnlyDictionary<string, string> BuildPlaceholders(
        GenerateCrudRequest request,
        EntityDefinition entity)
    {
        var placeholders = new Dictionary<string, string>(_placeholderBuilder.Build(entity), StringComparer.Ordinal)
        {
            [PlaceholderConstants.Route] = entity.PluralEntityName.ToLowerInvariant(),
            [PlaceholderConstants.ApiVersion] = "1",
            [PlaceholderConstants.Database] = entity.TableName,
            [PlaceholderConstants.ProjectName] = request.Namespace,
            [PlaceholderConstants.ConnectionString] = string.Empty,
            [PlaceholderConstants.RepositoryName] = _namingConventionService.GetRepositoryName(entity.EntityName),
            [PlaceholderConstants.ServiceName] = _namingConventionService.GetServiceName(entity.EntityName),
            [PlaceholderConstants.ManagerName] = _namingConventionService.GetManagerName(entity.EntityName),
            [PlaceholderConstants.ControllerName] = _namingConventionService.GetControllerName(entity.EntityName),
            [PlaceholderConstants.DtoName] = _namingConventionService.GetDtoName(entity.EntityName)
        };

        return placeholders;
    }

    private async Task<CrudGenerationResult> GenerateAllTemplatesAsync(
        GenerateCrudRequest request,
        EntityDefinition entity,
        IReadOnlyDictionary<string, string> placeholders,
        CancellationToken cancellationToken)
    {
        var result = new CrudGenerationResult();

        foreach (var template in GetSupportedTemplates(entity))
        {
            await GenerateTemplateAsync(request, template, placeholders, result, cancellationToken);
        }

        return result;
    }

    private async Task GenerateTemplateAsync(
        GenerateCrudRequest request,
        CrudTemplate template,
        IReadOnlyDictionary<string, string> placeholders,
        CrudGenerationResult result,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var templateDefinition = _templateDiscoveryService.GetTemplate(template.TemplatePath)
            ?? throw new TemplateNotFoundException(template.TemplatePath, includeTemplateName: true);

        var templateContent = await File.ReadAllTextAsync(templateDefinition.FullPath, Encoding.UTF8, cancellationToken);
        var renderedContent = _templateEngine.Render(templateContent, placeholders);
        var outputPath = DetermineOutputPath(request, template);

        if (File.Exists(outputPath) && !request.OverwriteExistingFiles)
        {
            result.AddSkipped(outputPath);
            return;
        }

        var generatedFile = await _fileGenerator.GenerateFileAsync(
            outputPath,
            renderedContent,
            request.OverwriteExistingFiles,
            cancellationToken);

        result.AddGenerated(generatedFile);
    }

    private IReadOnlyList<CrudTemplate> GetSupportedTemplates(EntityDefinition entity)
    {
        return
        [
            new("Domain/Entity.tpl", "Domain/Entities", $"{entity.EntityName}.cs"),
            new("Domain/IRepository.tpl", "Domain/Repositories", $"{_namingConventionService.GetInterfaceRepositoryName(entity.EntityName)}.cs"),
            new("Infrastructure/Repository.tpl", "Infrastructure/Repositories", $"{_namingConventionService.GetRepositoryName(entity.EntityName)}.cs"),
            new("Application/IService.tpl", "Application/Services", $"{_namingConventionService.GetInterfaceServiceName(entity.EntityName)}.cs"),
            new("Application/Service.tpl", "Application/Services", $"{_namingConventionService.GetServiceName(entity.EntityName)}.cs"),
            new("Api/Controller.tpl", "Api/Controllers", $"{_namingConventionService.GetControllerName(entity.EntityName)}.cs"),
            new("Application/Dto.tpl", "Application/Dtos", $"{_namingConventionService.GetDtoName(entity.EntityName)}.cs"),
            new("Application/CreateRequest.tpl", "Application/Requests", $"{_namingConventionService.GetCreateRequestName(entity.EntityName)}.cs"),
            new("Application/UpdateRequest.tpl", "Application/Requests", $"{_namingConventionService.GetUpdateRequestName(entity.EntityName)}.cs"),
            new("Application/Response.tpl", "Application/Responses", $"{_namingConventionService.GetResponseName(entity.EntityName)}.cs"),
            new("Application/Mapping.tpl", "Application/Mapping", $"{entity.EntityName}Mapper.cs"),
            new("SqlServer/InsertProcedure.tpl", "SqlServer", $"{entity.EntityName}.Insert.sql"),
            new("SqlServer/UpdateProcedure.tpl", "SqlServer", $"{entity.EntityName}.Update.sql"),
            new("SqlServer/DeleteProcedure.tpl", "SqlServer", $"{entity.EntityName}.Delete.sql"),
            new("SqlServer/GetByIdProcedure.tpl", "SqlServer", $"{entity.EntityName}.GetById.sql"),
            new("SqlServer/GetAllProcedure.tpl", "SqlServer", $"{entity.EntityName}.GetAll.sql")
        ];
    }

    private static string DetermineOutputPath(
        GenerateCrudRequest request,
        CrudTemplate template)
    {
        return Path.Combine(request.OutputDirectory, template.OutputDirectory, template.OutputFileName);
    }

    private static GenerateCrudResponse CreateResponse(
        CrudGenerationResult result,
        TimeSpan elapsedTime)
    {
        return new GenerateCrudResponse
        {
            Success = result.Success,
            GeneratedFiles = result.GeneratedFiles,
            SkippedFiles = result.SkippedFiles,
            Errors = result.Errors,
            ElapsedTime = elapsedTime,
            Message = result.Success
                ? "CRUD generation completed successfully."
                : "CRUD generation completed with errors."
        };
    }

    private sealed record CrudTemplate(
        string TemplatePath,
        string OutputDirectory,
        string OutputFileName);
}
