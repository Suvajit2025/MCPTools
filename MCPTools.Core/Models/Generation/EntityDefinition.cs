namespace MCPTools.Core.Models.Generation;

/// <summary>
/// Represents an entity used for code generation.
/// </summary>
public sealed class EntityDefinition
{
    /// <summary>
    /// Gets the root namespace used for generated code.
    /// </summary>
    public required string Namespace { get; init; }

    /// <summary>
    /// Gets the entity name.
    /// </summary>
    public required string EntityName { get; init; }

    /// <summary>
    /// Gets the plural entity name.
    /// </summary>
    public required string PluralEntityName { get; init; }

    /// <summary>
    /// Gets the database table name.
    /// </summary>
    public required string TableName { get; init; }

    /// <summary>
    /// Gets the database schema name.
    /// </summary>
    public string Schema { get; init; } = "dbo";

    /// <summary>
    /// Gets the primary key property or column name.
    /// </summary>
    public required string PrimaryKey { get; init; }

    /// <summary>
    /// Gets the primary key type.
    /// </summary>
    public required string PrimaryKeyType { get; init; }

    /// <summary>
    /// Gets the entity description.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Gets the author associated with generated artifacts.
    /// </summary>
    public string? Author { get; init; }

    /// <summary>
    /// Gets the company name associated with generated artifacts.
    /// </summary>
    public string? CompanyName { get; init; }

    /// <summary>
    /// Gets the optional base class used by generated entity types.
    /// </summary>
    public string? BaseClass { get; init; }

    /// <summary>
    /// Gets the interfaces implemented by generated entity types.
    /// </summary>
    public IReadOnlyList<string> Interfaces { get; init; } = [];

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
    /// Gets a value indicating whether SQL artifacts should be generated.
    /// </summary>
    public bool GenerateSql { get; init; } = true;

    /// <summary>
    /// Gets a value indicating whether DTO artifacts should be generated.
    /// </summary>
    public bool GenerateDto { get; init; } = true;

    /// <summary>
    /// Gets a value indicating whether mapping artifacts should be generated.
    /// </summary>
    public bool GenerateMapping { get; init; } = true;

    /// <summary>
    /// Gets the properties that belong to the entity.
    /// </summary>
    public IReadOnlyList<PropertyDefinition> Properties { get; init; } = [];
}
