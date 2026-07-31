namespace MCPTools.Core.Models.Schema;

/// <summary>
/// Represents index metadata for a database table.
/// </summary>
public sealed class IndexSchema
{
    /// <summary>
    /// Gets the index name.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Gets a value indicating whether the index is unique.
    /// </summary>
    public bool IsUnique { get; init; }

    /// <summary>
    /// Gets the indexed columns.
    /// </summary>
    public List<string> Columns { get; init; } = [];
}
