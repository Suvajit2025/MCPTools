using MCPTools.Core.Models.Schema;

namespace MCPTools.Core.Interfaces;

/// <summary>
/// Defines a provider capable of reading database schema metadata.
/// </summary>
public interface ISchemaProvider
{
    /// <summary>
    /// Gets all user tables from the configured database.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>The discovered table schemas.</returns>
    Task<List<TableSchema>> GetTablesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a table schema by table name.
    /// </summary>
    /// <param name="tableName">The table name to locate.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>The table schema when found; otherwise, <see langword="null"/>.</returns>
    Task<TableSchema?> GetTableAsync(
        string tableName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Determines whether a table exists in the configured database.
    /// </summary>
    /// <param name="tableName">The table name to locate.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns><see langword="true"/> when the table exists; otherwise, <see langword="false"/>.</returns>
    Task<bool> TableExistsAsync(
        string tableName,
        CancellationToken cancellationToken = default);
}
