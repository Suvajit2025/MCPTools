using System.Diagnostics;
using MCPTools.Core.Configuration;
using MCPTools.Core.Constants;
using MCPTools.Core.Exceptions;
using MCPTools.Core.Extensions;
using MCPTools.Core.Interfaces;
using MCPTools.Core.Models.Schema;
using MCPTools.Core.Models.Tools;
using MCPTools.Core.Services;
using MCPTools.Core.TemplateEngine;
using MCPTools.Core.Tools.Crud;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MCPTools.Core.Tools.Database;

/// <summary>
/// Generates CRUD source artifacts by reading metadata from an existing database table.
/// </summary>
public sealed class GenerateCrudFromDatabaseTool
    : ToolBase<GenerateCrudFromDatabaseRequest, GenerateCrudFromDatabaseResponse>
{
    private readonly ISchemaProvider _schemaProvider;
    private readonly NamingConventionService _namingConventionService;
    private readonly GenerateCrudTool _generateCrudTool;
    private readonly ILogger<GenerateCrudFromDatabaseTool> _logger;
    private readonly OutputOptions _outputOptions;
    private readonly DatabaseConnectionOptions _databaseOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="GenerateCrudFromDatabaseTool"/> class.
    /// </summary>
    /// <param name="schemaProvider">The schema provider used to read database metadata.</param>
    /// <param name="placeholderBuilder">The placeholder builder used to create template values.</param>
    /// <param name="fileGenerator">The file generator used to write generated files.</param>
    /// <param name="templateDiscoveryService">The template discovery service used to locate templates.</param>
    /// <param name="namingConventionService">The naming convention service used to normalize generated artifact names.</param>
    /// <param name="templateEngine">The template engine used to render templates.</param>
    /// <param name="generateCrudTool">The existing CRUD generation tool.</param>
    /// <param name="logger">The logger used to record execution details.</param>
    /// <param name="outputOptions">The output options used when the request does not provide an output directory.</param>
    /// <param name="databaseOptions">The database options used when the request does not provide a namespace.</param>
    public GenerateCrudFromDatabaseTool(
        ISchemaProvider schemaProvider,
        PlaceholderBuilder placeholderBuilder,
        FileGenerator fileGenerator,
        TemplateDiscoveryService templateDiscoveryService,
        NamingConventionService namingConventionService,
        ITemplateEngine templateEngine,
        GenerateCrudTool generateCrudTool,
        ILogger<GenerateCrudFromDatabaseTool> logger,
        IOptions<OutputOptions> outputOptions,
        IOptions<DatabaseConnectionOptions> databaseOptions)
        : base(new ToolMetadata
        {
            Name = "generate-crud-from-database",
            DisplayName = "Generate CRUD from Database",
            Category = ToolMetadataConstants.Categories.Database,
            Version = "1.0.0",
            Description = "Generates CRUD source code from an existing database table.",
            Tags =
            [
                ToolMetadataConstants.Tags.Crud,
                ToolMetadataConstants.Tags.Database,
                ToolMetadataConstants.Tags.Generation
            ]
        })
    {
        _schemaProvider = schemaProvider ?? throw new ArgumentNullException(nameof(schemaProvider));
        _ = placeholderBuilder ?? throw new ArgumentNullException(nameof(placeholderBuilder));
        _ = fileGenerator ?? throw new ArgumentNullException(nameof(fileGenerator));
        _ = templateDiscoveryService ?? throw new ArgumentNullException(nameof(templateDiscoveryService));
        _namingConventionService = namingConventionService ?? throw new ArgumentNullException(nameof(namingConventionService));
        _ = templateEngine ?? throw new ArgumentNullException(nameof(templateEngine));
        _generateCrudTool = generateCrudTool ?? throw new ArgumentNullException(nameof(generateCrudTool));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _outputOptions = outputOptions?.Value ?? throw new ArgumentNullException(nameof(outputOptions));
        _databaseOptions = databaseOptions?.Value ?? throw new ArgumentNullException(nameof(databaseOptions));
    }

    /// <inheritdoc />
    public override async Task<GenerateCrudFromDatabaseResponse> ExecuteAsync(
        GenerateCrudFromDatabaseRequest request,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            ValidateRequest(request);

            _logger.LogInformation("Starting database CRUD generation for table {TableName}.", request.TableName);

            if (!await _schemaProvider.TableExistsAsync(request.TableName, cancellationToken))
            {
                throw new ToolValidationException($"Table '{request.TableName}' does not exist.");
            }

            var table = await _schemaProvider.GetTableAsync(request.TableName, cancellationToken)
                ?? throw new ToolValidationException($"Table '{request.TableName}' could not be loaded.");

            var crudRequest = CreateCrudRequest(request, table);
            var crudResponse = await _generateCrudTool.ExecuteAsync(crudRequest, cancellationToken);

            _logger.LogInformation(
                "Completed database CRUD generation for table {TableName} in {ElapsedMilliseconds} ms.",
                request.TableName,
                stopwatch.ElapsedMilliseconds);

            return new GenerateCrudFromDatabaseResponse
            {
                TableName = table.Name ?? request.TableName,
                EntityName = crudRequest.EntityName,
                CrudGeneration = crudResponse
            };
        }
        catch (ToolValidationException)
        {
            throw;
        }
        catch (MCPToolsException exception)
        {
            _logger.LogError(exception, "Database CRUD generation failed for table {TableName}.", request.TableName);
            throw new ToolExecutionException(Metadata.Name, exception, includeToolName: true);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            _logger.LogError(exception, "Database CRUD generation failed unexpectedly for table {TableName}.", request.TableName);
            throw new ToolExecutionException(Metadata.Name, exception, includeToolName: true);
        }
    }

    private static void ValidateRequest(GenerateCrudFromDatabaseRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.TableName))
        {
            throw new ToolValidationException("TableName is required.");
        }
    }

    private GenerateCrudRequest CreateCrudRequest(
        GenerateCrudFromDatabaseRequest request,
        TableSchema table)
    {
        var tableName = table.Name ?? request.TableName;
        var entityName = _namingConventionService.GetEntityName(request.EntityName ?? tableName);
        var primaryKeyColumn = table.PrimaryKey?.Columns.FirstOrDefault()
            ?? table.Columns.FirstOrDefault(column => column.IsPrimaryKey)?.Name
            ?? throw new ToolValidationException($"Table '{tableName}' does not define a primary key.");

        var primaryKey = table.Columns.FirstOrDefault(column =>
            string.Equals(column.Name, primaryKeyColumn, StringComparison.OrdinalIgnoreCase));

        return new GenerateCrudRequest
        {
            Namespace = ResolveNamespace(request),
            EntityName = entityName,
            PluralEntityName = string.IsNullOrWhiteSpace(request.PluralEntityName)
                ? $"{entityName}s"
                : request.PluralEntityName,
            TableName = tableName,
            PrimaryKey = primaryKeyColumn.ToPascalCase(),
            PrimaryKeyType = MapSqlTypeToClrType(primaryKey),
            Author = request.Author,
            CompanyName = request.CompanyName,
            GenerateRepository = request.GenerateRepository,
            GenerateService = request.GenerateService,
            GenerateController = request.GenerateController,
            GenerateDto = request.GenerateDto,
            GenerateInterface = request.GenerateInterface,
            OutputDirectory = ResolveOutputDirectory(request),
            OverwriteExistingFiles = request.OverwriteExistingFiles ?? _outputOptions.OverwriteExistingFiles
        };
    }

    private string ResolveNamespace(GenerateCrudFromDatabaseRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.Namespace))
        {
            return request.Namespace;
        }

        if (!string.IsNullOrWhiteSpace(_databaseOptions.Database))
        {
            return _databaseOptions.Database.ToPascalCase();
        }

        return "Generated";
    }

    private string ResolveOutputDirectory(GenerateCrudFromDatabaseRequest request)
    {
        return string.IsNullOrWhiteSpace(request.OutputDirectory)
            ? _outputOptions.OutputDirectory
            : request.OutputDirectory;
    }

    private static string MapSqlTypeToClrType(ColumnSchema? column)
    {
        if (column is null || string.IsNullOrWhiteSpace(column.DataType))
        {
            return "string";
        }

        var type = column.DataType.ToLowerInvariant() switch
        {
            "bigint" => "long",
            "binary" => "byte[]",
            "bit" => "bool",
            "char" => "string",
            "date" => "DateOnly",
            "datetime" => "DateTime",
            "datetime2" => "DateTime",
            "datetimeoffset" => "DateTimeOffset",
            "decimal" => "decimal",
            "float" => "double",
            "image" => "byte[]",
            "int" => "int",
            "money" => "decimal",
            "nchar" => "string",
            "ntext" => "string",
            "numeric" => "decimal",
            "nvarchar" => "string",
            "real" => "float",
            "smalldatetime" => "DateTime",
            "smallint" => "short",
            "smallmoney" => "decimal",
            "text" => "string",
            "time" => "TimeOnly",
            "timestamp" => "byte[]",
            "tinyint" => "byte",
            "uniqueidentifier" => "Guid",
            "varbinary" => "byte[]",
            "varchar" => "string",
            "xml" => "string",
            _ => "string"
        };

        if (!column.IsNullable || type is "string" or "byte[]")
        {
            return type;
        }

        return $"{type}?";
    }
}
