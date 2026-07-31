namespace MCPTools.Core.Models.Schema;

/// <summary>
/// Represents schema metadata for a database column.
/// </summary>
public sealed class ColumnSchema
{
    /// <summary>
    /// Gets the column name.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Gets the database data type.
    /// </summary>
    public string? DataType { get; init; }

    /// <summary>
    /// Gets a value indicating whether the column allows null values.
    /// </summary>
    public bool IsNullable { get; init; }

    /// <summary>
    /// Gets a value indicating whether the column is an identity column.
    /// </summary>
    public bool IsIdentity { get; init; }

    /// <summary>
    /// Gets a value indicating whether the column is part of the primary key.
    /// </summary>
    public bool IsPrimaryKey { get; init; }

    /// <summary>
    /// Gets the maximum column length.
    /// </summary>
    public int? MaxLength { get; init; }

    /// <summary>
    /// Gets the numeric precision.
    /// </summary>
    public int? Precision { get; init; }

    /// <summary>
    /// Gets the numeric scale.
    /// </summary>
    public int? Scale { get; init; }
}
