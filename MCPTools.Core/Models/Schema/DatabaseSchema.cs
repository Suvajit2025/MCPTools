namespace MCPTools.Core.Models.Schema;

/// <summary>
/// Represents the schema metadata for a database.
/// </summary>
public sealed class DatabaseSchema
{
    /// <summary>
    /// Gets the database name.
    /// </summary>
    public string? DatabaseName { get; init; }

    /// <summary>
    /// Gets the tables in the database.
    /// </summary>
    public List<TableSchema> Tables { get; init; } = [];
}
