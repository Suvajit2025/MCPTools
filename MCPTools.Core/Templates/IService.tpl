namespace {{Namespace}}.Application.Services;

using {{Namespace}}.Application.Dtos;

/// <summary>
/// Defines application service operations for {{ModelName}}.
/// </summary>
public interface I{{ModelName}}Service
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
