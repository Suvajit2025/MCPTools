namespace {{Namespace}}.Application.Managers;

using {{Namespace}}.Application.Dtos;

/// <summary>
/// Defines manager operations for {{ModelName}} workflows.
/// </summary>
public interface I{{ModelName}}Manager
{
    /// <summary>
    /// Gets all {{ModelName}} records.
    /// </summary>
    Task<IReadOnlyList<{{ModelName}}Response>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a {{ModelName}} record by its primary key.
    /// </summary>
    Task<{{ModelName}}Response?> GetByIdAsync({{PrimaryKeyType}} {{PrimaryKey}}, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new {{ModelName}} record.
    /// </summary>
    Task<int> CreateAsync(Create{{ModelName}}Request request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing {{ModelName}} record.
    /// </summary>
    Task<int> UpdateAsync({{PrimaryKeyType}} {{PrimaryKey}}, Update{{ModelName}}Request request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a {{ModelName}} record by its primary key.
    /// </summary>
    Task<int> DeleteAsync({{PrimaryKeyType}} {{PrimaryKey}}, CancellationToken cancellationToken = default);
}
