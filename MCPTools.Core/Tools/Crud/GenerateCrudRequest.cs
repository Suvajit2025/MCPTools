using MCPTools.Core.Models.Generation;

namespace MCPTools.Core.Tools.Crud;

/// <summary>
/// Represents the information required to generate CRUD code for a single database table.
/// </summary>
public sealed class GenerateCrudRequest
{
    /// <summary>
    /// Gets the directory where generated files will be written.
    /// </summary>
    public required string OutputDirectory { get; init; }

    /// <summary>
    /// Gets the root namespace of the generated project.
    /// </summary>
    public required string Namespace { get; init; }

    /// <summary>
    /// Gets the database table name.
    /// </summary>
    public required string TableName { get; init; }

    /// <summary>
    /// Gets the entity name.
    /// </summary>
    public required string EntityName { get; init; }

    /// <summary>
    /// Gets the plural entity name.
    /// </summary>
    public string? PluralEntityName { get; init; }

    /// <summary>
    /// Gets the primary key column.
    /// </summary>
    public required string PrimaryKey { get; init; }

    /// <summary>
    /// Gets the primary key type.
    /// </summary>
    public required string PrimaryKeyType { get; init; }

    /// <summary>
    /// Gets the author associated with generated artifacts.
    /// </summary>
    public string? Author { get; init; }

    /// <summary>
    /// Gets the company name associated with generated artifacts.
    /// </summary>
    public string? CompanyName { get; init; }

    /// <summary>
    /// Gets a value indicating whether a controller should be generated.
    /// </summary>
    public bool GenerateController { get; init; }

    /// <summary>
    /// Gets a value indicating whether a service should be generated.
    /// </summary>
    public bool GenerateService { get; init; }

    /// <summary>
    /// Gets a value indicating whether a repository should be generated.
    /// </summary>
    public bool GenerateRepository { get; init; }

    /// <summary>
    /// Gets a value indicating whether interfaces should be generated.
    /// </summary>
    public bool GenerateInterface { get; init; }

    /// <summary>
    /// Gets a value indicating whether DTOs should be generated.
    /// </summary>
    public bool GenerateDto { get; init; }

    /// <summary>
    /// Gets a value indicating whether existing generated files should be overwritten.
    /// </summary>
    public bool OverwriteExistingFiles { get; init; }

    /// <summary>
    /// Gets the discovered entity properties to use during generation.
    /// </summary>
    public IReadOnlyList<PropertyDefinition> Properties { get; init; } = [];
}
