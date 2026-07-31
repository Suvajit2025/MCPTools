namespace MCPTools.Core.Tools.Database;

/// <summary>
/// Represents the information required to generate CRUD artifacts from an existing database table.
/// </summary>
public sealed class GenerateCrudFromDatabaseRequest
{
    /// <summary>
    /// Gets the database table name used as the source for CRUD generation.
    /// </summary>
    public required string TableName { get; init; }

    /// <summary>
    /// Gets the optional root namespace used for generated code.
    /// </summary>
    public string? Namespace { get; init; }

    /// <summary>
    /// Gets the optional output directory where generated files will be written.
    /// </summary>
    public string? OutputDirectory { get; init; }

    /// <summary>
    /// Gets the optional entity name. When omitted, the table name is used.
    /// </summary>
    public string? EntityName { get; init; }

    /// <summary>
    /// Gets the optional plural entity name. When omitted, the entity name is pluralized using the framework default.
    /// </summary>
    public string? PluralEntityName { get; init; }

    /// <summary>
    /// Gets the optional author associated with generated artifacts.
    /// </summary>
    public string? Author { get; init; }

    /// <summary>
    /// Gets the optional company name associated with generated artifacts.
    /// </summary>
    public string? CompanyName { get; init; }

    /// <summary>
    /// Gets a value indicating whether repository artifacts should be generated.
    /// </summary>
    public bool GenerateRepository { get; init; } = true;

    /// <summary>
    /// Gets a value indicating whether service artifacts should be generated.
    /// </summary>
    public bool GenerateService { get; init; } = true;

    /// <summary>
    /// Gets a value indicating whether controller artifacts should be generated.
    /// </summary>
    public bool GenerateController { get; init; } = true;

    /// <summary>
    /// Gets a value indicating whether DTO artifacts should be generated.
    /// </summary>
    public bool GenerateDto { get; init; } = true;

    /// <summary>
    /// Gets a value indicating whether interface artifacts should be generated.
    /// </summary>
    public bool GenerateInterface { get; init; } = true;

    /// <summary>
    /// Gets a value indicating whether existing generated files should be overwritten.
    /// </summary>
    public bool? OverwriteExistingFiles { get; init; }
}
