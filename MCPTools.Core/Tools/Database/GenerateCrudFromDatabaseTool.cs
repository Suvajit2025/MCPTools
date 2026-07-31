using System.Diagnostics;
using MCPTools.Core.Configuration;
using MCPTools.Core.Constants;
using MCPTools.Core.Exceptions;
using MCPTools.Core.Extensions;
using MCPTools.Core.Interfaces;
using MCPTools.Core.Models.Generation;
using MCPTools.Core.Models.Schema;
using MCPTools.Core.Models.Tools;
using MCPTools.Core.Services;
using MCPTools.Core.Services.Schema;
using MCPTools.Core.TemplateEngine;
using MCPTools.Core.Tools.Crud;
using Microsoft.Data.SqlClient;
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
    private readonly SqlServerTypeMapper _typeMapper;
    private readonly GenerateCrudTool _generateCrudTool;
    private readonly ILogger<GenerateCrudFromDatabaseTool> _logger;
    private readonly ILogger<SqlServerSchemaProvider> _schemaProviderLogger;
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
        SqlServerTypeMapper typeMapper,
        ITemplateEngine templateEngine,
        GenerateCrudTool generateCrudTool,
        ILogger<GenerateCrudFromDatabaseTool> logger,
        ILogger<SqlServerSchemaProvider> schemaProviderLogger,
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
        _typeMapper = typeMapper ?? throw new ArgumentNullException(nameof(typeMapper));
        _ = templateEngine ?? throw new ArgumentNullException(nameof(templateEngine));
        _generateCrudTool = generateCrudTool ?? throw new ArgumentNullException(nameof(generateCrudTool));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _schemaProviderLogger = schemaProviderLogger ?? throw new ArgumentNullException(nameof(schemaProviderLogger));
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

            var tableName = ResolveTableName(request);
            var qualifiedTableName = ResolveQualifiedTableName(request, tableName);
            var schemaProvider = CreateSchemaProvider(request);

            _logger.LogInformation("Connecting to SQL Server for table {TableName}.", qualifiedTableName);

            if (!await schemaProvider.TableExistsAsync(qualifiedTableName, cancellationToken))
            {
                throw new ToolValidationException($"Table '{qualifiedTableName}' was not found.");
            }

            _logger.LogInformation("Reading schema for table {TableName}.", qualifiedTableName);

            var table = await schemaProvider.GetTableAsync(qualifiedTableName, cancellationToken)
                ?? throw new ToolValidationException($"Table '{qualifiedTableName}' could not be loaded.");

            _logger.LogInformation("Found {ColumnCount} columns in table {TableName}.", table.Columns.Count, qualifiedTableName);

            var crudRequest = CreateCrudRequest(request, table);

            _logger.LogInformation("Generating CRUD files for entity {EntityName}.", crudRequest.EntityName);

            var crudResponse = await _generateCrudTool.ExecuteAsync(crudRequest, cancellationToken);

            _logger.LogInformation(
                "Completed database CRUD generation for table {TableName} in {ElapsedMilliseconds} ms.",
                qualifiedTableName,
                stopwatch.ElapsedMilliseconds);

            return new GenerateCrudFromDatabaseResponse
            {
                TableName = table.Name ?? tableName,
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
            _logger.LogError(exception, "Database CRUD generation failed.");
            throw new ToolExecutionException(Metadata.Name, exception, includeToolName: true);
        }
        catch (SqlException exception)
        {
            _logger.LogError(exception, "SQL Server error during database CRUD generation.");
            throw new ToolExecutionException(Metadata.Name, exception, includeToolName: true);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            _logger.LogError(exception, "Database CRUD generation failed unexpectedly.");
            throw new ToolExecutionException(Metadata.Name, exception, includeToolName: true);
        }
    }

    private static void ValidateRequest(GenerateCrudFromDatabaseRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.TableName) && string.IsNullOrWhiteSpace(request.Table))
        {
            throw new ToolValidationException("Table name is required.");
        }

        if (HasRequestConnection(request))
        {
            if (string.IsNullOrWhiteSpace(request.Server))
            {
                throw new ToolValidationException("Server is required.");
            }

            if (string.IsNullOrWhiteSpace(request.Database))
            {
                throw new ToolValidationException("Database is required.");
            }

            if (IsSqlAuthentication(request) &&
                (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password)))
            {
                throw new ToolValidationException("Username and Password are required for SQL authentication.");
            }
        }
    }

    private ISchemaProvider CreateSchemaProvider(GenerateCrudFromDatabaseRequest request)
    {
        if (!HasRequestConnection(request))
        {
            return _schemaProvider;
        }

        var options = new DatabaseConnectionOptions
        {
            Server = request.Server,
            Database = request.Database,
            UserId = request.Username,
            Password = request.Password,
            IntegratedSecurity = !IsSqlAuthentication(request),
            Encrypt = _databaseOptions.Encrypt,
            TrustServerCertificate = _databaseOptions.TrustServerCertificate
        };

        return new SqlServerSchemaProvider(
            new SqlConnectionFactory(Options.Create(options)),
            _schemaProviderLogger);
    }

    private GenerateCrudRequest CreateCrudRequest(
        GenerateCrudFromDatabaseRequest request,
        TableSchema table)
    {
        var tableName = table.Name ?? ResolveTableName(request);
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
            PrimaryKeyType = primaryKey is null ? "string" : _typeMapper.MapToClrType(primaryKey),
            Author = request.Author,
            CompanyName = request.CompanyName,
            GenerateRepository = request.GenerateRepository,
            GenerateService = request.GenerateService,
            GenerateController = request.GenerateController,
            GenerateDto = request.GenerateDto,
            GenerateInterface = request.GenerateInterface,
            OutputDirectory = ResolveOutputDirectory(request),
            OverwriteExistingFiles = request.OverwriteExistingFiles ?? _outputOptions.OverwriteExistingFiles,
            Properties = BuildProperties(table)
        };
    }

    private IReadOnlyList<PropertyDefinition> BuildProperties(TableSchema table)
    {
        return table.Columns
            .OrderBy(column => column.Order)
            .Select(column => new PropertyDefinition
            {
                Name = (column.Name ?? string.Empty).ToPascalCase(),
                ColumnName = column.Name,
                Type = _typeMapper.MapToClrType(column),
                ClrType = _typeMapper.MapToClrType(column),
                SqlType = column.DataType,
                IsPrimaryKey = column.IsPrimaryKey,
                IsNullable = column.IsNullable,
                IsIdentity = column.IsIdentity,
                IsComputed = column.IsComputed,
                DefaultValue = column.DefaultValue,
                MaxLength = column.MaxLength,
                Precision = column.Precision,
                Scale = column.Scale,
                Order = column.Order
            })
            .ToArray();
    }

    private string ResolveNamespace(GenerateCrudFromDatabaseRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.Namespace))
        {
            return request.Namespace;
        }

        if (!string.IsNullOrWhiteSpace(request.ProjectName))
        {
            return request.ProjectName;
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
            ? string.IsNullOrWhiteSpace(request.OutputFolder) ? _outputOptions.OutputDirectory : request.OutputFolder
            : request.OutputDirectory;
    }

    private static string ResolveTableName(GenerateCrudFromDatabaseRequest request)
    {
        return string.IsNullOrWhiteSpace(request.TableName)
            ? request.Table!
            : request.TableName;
    }

    private static string ResolveQualifiedTableName(
        GenerateCrudFromDatabaseRequest request,
        string tableName)
    {
        if (tableName.Contains('.', StringComparison.Ordinal) || string.IsNullOrWhiteSpace(request.Schema))
        {
            return tableName;
        }

        return $"{request.Schema}.{tableName}";
    }

    private static bool HasRequestConnection(GenerateCrudFromDatabaseRequest request)
    {
        return !string.IsNullOrWhiteSpace(request.Server)
            || !string.IsNullOrWhiteSpace(request.Database)
            || !string.IsNullOrWhiteSpace(request.Authentication);
    }

    private static bool IsSqlAuthentication(GenerateCrudFromDatabaseRequest request)
    {
        return string.Equals(request.Authentication, "Sql", StringComparison.OrdinalIgnoreCase)
            || string.Equals(request.Authentication, "SQL", StringComparison.OrdinalIgnoreCase)
            || string.Equals(request.Authentication, "SqlAuthentication", StringComparison.OrdinalIgnoreCase);
    }
}
