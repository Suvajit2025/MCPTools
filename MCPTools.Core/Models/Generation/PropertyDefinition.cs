namespace MCPTools.Core.Models.Generation;

/// <summary>
/// Represents a property that belongs to a generated entity.
/// </summary>
public sealed class PropertyDefinition
{
    /// <summary>
    /// Gets the property name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets the property type.
    /// </summary>
    public required string Type { get; init; }

    /// <summary>
    /// Gets the CLR type mapped from the source data type.
    /// </summary>
    public string? ClrType { get; init; }

    /// <summary>
    /// Gets the source SQL type when the property was created from database metadata.
    /// </summary>
    public string? SqlType { get; init; }

    /// <summary>
    /// Gets the database column name mapped to the property.
    /// </summary>
    public string? ColumnName { get; init; }

    /// <summary>
    /// Gets the property description.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Gets a value indicating whether the property is the primary key.
    /// </summary>
    public bool IsPrimaryKey { get; init; }

    /// <summary>
    /// Gets a value indicating whether the property can be null.
    /// </summary>
    public bool IsNullable { get; init; }

    /// <summary>
    /// Gets a value indicating whether the property is database-generated.
    /// </summary>
    public bool IsIdentity { get; init; }

    /// <summary>
    /// Gets a value indicating whether the property is computed by the database.
    /// </summary>
    public bool IsComputed { get; init; }

    /// <summary>
    /// Gets a value indicating whether the property is required.
    /// </summary>
    public bool IsRequired { get; init; }

    /// <summary>
    /// Gets the default value for the property.
    /// </summary>
    public string? DefaultValue { get; init; }

    /// <summary>
    /// Gets the maximum length for text-based properties.
    /// </summary>
    public int? MaxLength { get; init; }

    /// <summary>
    /// Gets the numeric precision for decimal properties.
    /// </summary>
    public int? Precision { get; init; }

    /// <summary>
    /// Gets the numeric scale for decimal properties.
    /// </summary>
    public int? Scale { get; init; }

    /// <summary>
    /// Gets the generation order of the property.
    /// </summary>
    public int Order { get; init; }

    /// <summary>
    /// Gets a value indicating whether the property should be generated in DTO models.
    /// </summary>
    public bool GenerateInDto { get; init; } = true;

    /// <summary>
    /// Gets a value indicating whether the property should be generated in request models.
    /// </summary>
    public bool GenerateInRequest { get; init; } = true;

    /// <summary>
    /// Gets a value indicating whether the property should be generated in response models.
    /// </summary>
    public bool GenerateInResponse { get; init; } = true;
}
