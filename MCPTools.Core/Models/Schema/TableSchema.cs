namespace MCPTools.Core.Models.Schema;

/// <summary>
/// Represents schema metadata for a database table.
/// </summary>
public sealed class TableSchema
{
    /// <summary>
    /// Gets the table name.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Gets the table schema name.
    /// </summary>
    public string? Schema { get; init; }

    /// <summary>
    /// Gets the table columns.
    /// </summary>
    public List<ColumnSchema> Columns { get; init; } = [];

    /// <summary>
    /// Gets the table primary key metadata.
    /// </summary>
    public PrimaryKeySchema? PrimaryKey { get; init; }

    /// <summary>
    /// Gets the table foreign keys.
    /// </summary>
    public List<ForeignKeySchema> ForeignKeys { get; init; } = [];

    /// <summary>
    /// Gets the table indexes.
    /// </summary>
    public List<IndexSchema> Indexes { get; init; } = [];
}
