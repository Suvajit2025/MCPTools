namespace MCPTools.Core.Models.Schema;

/// <summary>
/// Represents primary key metadata for a database table.
/// </summary>
public sealed class PrimaryKeySchema
{
    /// <summary>
    /// Gets the primary key name.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Gets the primary key columns.
    /// </summary>
    public List<string> Columns { get; init; } = [];
}
