namespace {{Namespace}}.Domain.Repositories;

using {{Namespace}}.Domain.Entities;

/// <summary>
/// Defines persistence operations for {{EntityName}}.
/// </summary>
public interface I{{RepositoryName}}
{
    /// <summary>
    /// Gets all {{PluralEntityName}}.
    /// </summary>
    Task<IReadOnlyList<{{EntityName}}>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a {{EntityName}} by its primary key.
    /// </summary>
    Task<{{EntityName}}?> GetByIdAsync({{PrimaryKeyType}} {{PrimaryKey}}, CancellationToken cancellationToken = default);

    /// <summary>
    /// Inserts a new {{EntityName}}.
    /// </summary>
    Task<int> InsertAsync({{EntityName}} entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing {{EntityName}}.
    /// </summary>
    Task<int> UpdateAsync({{EntityName}} entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a {{EntityName}} by its primary key.
    /// </summary>
    Task<int> DeleteAsync({{PrimaryKeyType}} {{PrimaryKey}}, CancellationToken cancellationToken = default);
}
