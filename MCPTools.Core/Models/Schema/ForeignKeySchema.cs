namespace MCPTools.Core.Models.Schema;

/// <summary>
/// Represents foreign key metadata for a database table.
/// </summary>
public sealed class ForeignKeySchema
{
    /// <summary>
    /// Gets the foreign key name.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Gets the local column name.
    /// </summary>
    public string? Column { get; init; }

    /// <summary>
    /// Gets the referenced table name.
    /// </summary>
    public string? ReferencedTable { get; init; }

    /// <summary>
    /// Gets the referenced column name.
    /// </summary>
    public string? ReferencedColumn { get; init; }
}
