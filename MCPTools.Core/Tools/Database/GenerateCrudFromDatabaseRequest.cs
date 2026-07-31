namespace MCPTools.Core.Tools.Database;

/// <summary>
/// Represents the information required to generate CRUD artifacts from an existing database table.
/// </summary>
public sealed class GenerateCrudFromDatabaseRequest
{
    /// <summary>
    /// Gets the SQL Server name or address.
    /// </summary>
    public string? Server { get; init; }

    /// <summary>
    /// Gets the authentication type. Supported values are Windows and Sql.
    /// </summary>
    public string? Authentication { get; init; }

    /// <summary>
    /// Gets the SQL login user name when SQL authentication is used.
    /// </summary>
    public string? Username { get; init; }

    /// <summary>
    /// Gets the SQL login password when SQL authentication is used.
    /// </summary>
    public string? Password { get; init; }

    /// <summary>
    /// Gets the database name.
    /// </summary>
    public string? Database { get; init; }

    /// <summary>
    /// Gets the database schema name.
    /// </summary>
    public string? Schema { get; init; }

    /// <summary>
    /// Gets the table name using the compact MCP request field name.
    /// </summary>
    public string? Table { get; init; }

    /// <summary>
    /// Gets the project name associated with generated artifacts.
    /// </summary>
    public string? ProjectName { get; init; }

    /// <summary>
    /// Gets the database table name used as the source for CRUD generation.
    /// </summary>
    public string? TableName { get; init; }

    /// <summary>
    /// Gets the optional root namespace used for generated code.
    /// </summary>
    public string? Namespace { get; init; }

    /// <summary>
    /// Gets the output folder using the compact MCP request field name.
    /// </summary>
    public string? OutputFolder { get; init; }

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
