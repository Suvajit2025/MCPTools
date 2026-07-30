namespace {{Namespace}}.Application.Repositories;

using {{Namespace}}.Domain.Entities;

/// <summary>
/// Defines repository operations for {{ModelName}}.
/// </summary>
public interface I{{ModelName}}Repository
{
    /// <summary>
    /// Gets all {{ModelName}} records.
    /// </summary>
    Task<IReadOnlyList<{{ModelName}}>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a {{ModelName}} record by its primary key.
    /// </summary>
    Task<{{ModelName}}?> GetByIdAsync({{PrimaryKeyType}} {{PrimaryKey}}, CancellationToken cancellationToken = default);

    /// <summary>
    /// Inserts a new {{ModelName}} record.
    /// </summary>
    Task<int> InsertAsync({{ModelName}} entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing {{ModelName}} record.
    /// </summary>
    Task<int> UpdateAsync({{ModelName}} entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a {{ModelName}} record by its primary key.
    /// </summary>
    Task<int> DeleteAsync({{PrimaryKeyType}} {{PrimaryKey}}, CancellationToken cancellationToken = default);
}
